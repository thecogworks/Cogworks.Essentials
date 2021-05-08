using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Cogworks.Essentials.Constants;
using Cogworks.Essentials.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace Cogworks.Essentials.Services
{
    public class MemoryCacheService : ICacheService
    {
        private static MemoryCache Cache => new MemoryCache(new MemoryCacheOptions());

        private static ConcurrentDictionary<object, SemaphoreSlim> Locks => new ConcurrentDictionary<object, SemaphoreSlim>();

        public bool Contains(string cacheKey)
            => Cache.TryGetValue(cacheKey, out _);

        public T GetCacheItem<T>(string cacheKey)
            => Cache.Get(cacheKey) is T cachedObject
                ? cachedObject
                : default;

        public void RemoveCacheItem(string cacheKey)
            => Cache.Remove(cacheKey);

        public void AddCacheItem(string cacheKey, object value, int? cacheDurationInSeconds)
        {
            cacheDurationInSeconds ??= DateTimeConstants.TimeInSecondsConstants.Hour;
            var cacheDurationDateTime = DateTime.UtcNow.AddSeconds(cacheDurationInSeconds.Value);

            Cache.Set(cacheKey, value, cacheDurationDateTime);
        }

        public T GetOrAddCacheItem<T>(string cacheKey, Func<T> getValueFunction, int? cacheDurationInSeconds)
        {
            var cacheEntry = Cache.GetOrCreate(cacheKey, entry =>
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
        public async Task<T> MultiThreadProofGetOrAddCacheItem<T>(string cacheKey, Func<Task<T>> getValueFunction, int? cacheDurationInSeconds)
        {
            if (Cache.TryGetValue(cacheKey, out T cacheEntry))
            {
                return cacheEntry;
            }

            var myLock = Locks.GetOrAdd(cacheKey, k => new SemaphoreSlim(1, 1));
            await myLock.WaitAsync();

            try
            {
                if (!Cache.TryGetValue(cacheKey, out cacheEntry))
                {
                    cacheEntry = await getValueFunction();

                    cacheDurationInSeconds ??= DateTimeConstants.TimeInSecondsConstants.Hour;
                    var cacheDurationDateTime = DateTime.UtcNow.AddSeconds(cacheDurationInSeconds.Value);

                    Cache.Set(cacheKey, cacheEntry, cacheDurationDateTime);
                }
            }
            finally
            {
                myLock.Release();
            }

            return cacheEntry;
        }
    }
}