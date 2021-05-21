using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cogworks.Essentials.Constants;
using Cogworks.Essentials.Extensions;
using Cogworks.Essentials.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace Cogworks.Essentials.Services
{
    public class MemoryCacheService : ICacheService, IDisposable
    {
        private const string CacheKeyList = "CacheKeyList";

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
        {
            RemoveCacheKeyList(cacheKey);
            _memoryCache.Remove(cacheKey);
        }

        public void AddCacheItem(string cacheKey, object value, int? cacheDurationInSeconds = null)
        {
            AddCacheKeyList(cacheKey);

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
                AddCacheKeyList(cacheKey);

                return getValueFunction();
            });

            return cacheEntry;
        }

        /// <summary>
        /// This is a MultiThread save method but PLEASE use only when:
        /// - When the creation time of an item has some sort of cost, and you want to minimize creations as much as possible.
        /// - When the creation time of an item is very long.
        /// - When the creation of an item has to be ensured to be done once per key.
        /// </summary>
        /// https://michaelscodingspot.com/cache-implementations-in-csharp-net/
        public async Task<T> GetOrAddCacheItemAsync<T>(string cacheKey, Func<Task<T>> getValueFunction, int? cacheDurationInSeconds = null)
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

            AddCacheKeyList(cacheKey);
            return cacheEntry;
        }

        public void ClearAllStartingWith(string prefixKey)
        {
            var cacheKeys = GetOrAddCacheKeyList()
                .Where(x => x.StartsWith(prefixKey))
                .ToList();

            if (!cacheKeys.HasAny())
            {
                return;
            }

            foreach (var key in cacheKeys)
            {
                _memoryCache.Remove(key);
            }

            RemoveCacheKeyList(cacheKeys);
        }

        public void ClearAll()
        {
            var cacheKeys = GetOrAddCacheKeyList();

            if (!cacheKeys.HasAny())
            {
                return;
            }

            foreach (var key in cacheKeys)
            {
                _memoryCache.Remove(key);
            }

            RemoveCacheKeyList(cacheKeys);
        }

        public void Dispose()
            => _memoryCache.Dispose();

        private void AddCacheKeyList(string cacheKey)
        {
            var cacheKeyList = GetOrAddCacheKeyList();

            cacheKeyList.AddUnique(cacheKey);

            var cacheDurationDateTime = DateTime.UtcNow.AddSeconds(DateTimeConstants.TimeInSecondsConstants.Year);

            _memoryCache.Set(CacheKeyList, cacheKeyList, cacheDurationDateTime);
        }

        private void RemoveCacheKeyList(string cacheKey)
        {
            var cacheKeyList = GetOrAddCacheKeyList();

            cacheKeyList.Remove(cacheKey);

            UpdateCacheKeyList(cacheKeyList);
        }

        private void RemoveCacheKeyList(IEnumerable<string> toBeRemovedItems)
        {
            var cacheKeys = GetOrAddCacheKeyList();

            cacheKeys = cacheKeys.Except(toBeRemovedItems).ToList();

            UpdateCacheKeyList(cacheKeys);
        }

        private void UpdateCacheKeyList(IEnumerable<string> cacheKeys)
            => _memoryCache.Set(
                CacheKeyList,
                cacheKeys,
                DateTime.UtcNow.AddSeconds(DateTimeConstants.TimeInSecondsConstants.Year));

        private List<string> GetOrAddCacheKeyList()
            => _memoryCache.GetOrCreate(CacheKeyList, entry =>
            {
                var cacheDurationDateTime = DateTime.UtcNow.AddSeconds(DateTimeConstants.TimeInSecondsConstants.Year);
                entry.AbsoluteExpiration = cacheDurationDateTime;

                return new List<string>();
            });
    }
}