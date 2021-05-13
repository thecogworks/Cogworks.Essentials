using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Cogworks.Essentials.Constants;
using Cogworks.Essentials.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace Cogworks.Essentials.Services
{
    public class MemoryCacheService : ICacheService, IDisposable
    {
        private readonly IMemoryCache _memoryCache;

        private static ConcurrentDictionary<object, SemaphoreSlim> Locks => new ConcurrentDictionary<object, SemaphoreSlim>();

        public MemoryCacheService(IMemoryCache memoryCache)
            => _memoryCache = memoryCache;

        public MemoryCacheService()
            => _memoryCache = new MemoryCache(new MemoryCacheOptions());

        public bool Contains(string cacheKey)
            => _memoryCache.TryGetValue(cacheKey, out _);

        public T GetCacheItem<T>(string cacheKey)
            => _memoryCache.Get(cacheKey) is T cachedObject
                ? cachedObject
                : default;

        public void RemoveCacheItem(string cacheKey)
            => _memoryCache.Remove(cacheKey);

        public void AddCacheItem(string cacheKey, object value, int? cacheDurationInSeconds = null)
        {
            cacheDurationInSeconds ??= DateTimeConstants.TimeInSecondsConstants.Hour;
            var cacheDurationDateTime = DateTime.UtcNow.AddSeconds(cacheDurationInSeconds.Value);

            _memoryCache.Set(cacheKey, value, cacheDurationDateTime);
        }

        public T GetOrAddCacheItem<T>(string cacheKey, Func<T> getValueFunction, int? cacheDurationInSeconds = null)
        {
            var cacheEntry = _memoryCache.GetOrCreate(cacheKey, entry =>
            {
                cacheDurationInSeconds ??= DateTimeConstants.TimeInSecondsConstants.Hour;
                var cacheDurationDateTime = DateTime.UtcNow.AddSeconds(cacheDurationInSeconds.Value);

                entry.AbsoluteExpiration = cacheDurationDateTime;
                return getValueFunction();
            });

            return cacheEntry;
        }

        /// <summary>
        /// Use MultiThreadProofGetOrAddCacheItem when:
        /// - When the creation time of an item has some sort of cost, and you want to minimize creations as much as possible.
        /// - When the creation time of an item is very long.
        /// - When the creation of an item has to be ensured to be done once per key.
        /// </summary>
        /// https://michaelscodingspot.com/cache-implementations-in-csharp-net/
        public async Task<T> MultiThreadProofGetOrAddCacheItem<T>(string cacheKey, Func<Task<T>> getValueFunction, int? cacheDurationInSeconds = null)
        {
            if (_memoryCache.TryGetValue(cacheKey, out T cacheEntry))
            {
                return cacheEntry;
            }

            var myLock = Locks.GetOrAdd(cacheKey, k => new SemaphoreSlim(1, 1));
            await myLock.WaitAsync();

            try
            {
                if (!_memoryCache.TryGetValue(cacheKey, out cacheEntry))
                {
                    cacheEntry = await getValueFunction();

                    cacheDurationInSeconds ??= DateTimeConstants.TimeInSecondsConstants.Hour;
                    var cacheDurationDateTime = DateTime.UtcNow.AddSeconds(cacheDurationInSeconds.Value);

                    _memoryCache.Set(cacheKey, cacheEntry, cacheDurationDateTime);
                }
            }
            finally
            {
                myLock.Release();
            }

            return cacheEntry;
        }

        public void Dispose()
            => _memoryCache.Dispose();
    }
}