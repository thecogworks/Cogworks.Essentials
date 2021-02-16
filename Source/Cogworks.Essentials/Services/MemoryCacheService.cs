using System;
using Microsoft.Extensions.Caching.Memory;

namespace Cogworks.Essentials.Services
{
    public class MemoryCacheService : ICacheService
    {
        private static MemoryCacheOptions Options => new MemoryCacheOptions();

        private static MemoryCache Cache => new MemoryCache(Options);

        public bool Contains(string cacheKey)
            => Cache.TryGetValue(cacheKey, out _);

        public T GetCacheItem<T>(string cacheKey)
            => Cache.Get(cacheKey) is T cachedObject
                ? cachedObject
                : default;

        public void RemoveCacheItem(string cacheKey)
            => Cache.Remove(cacheKey);

        public void SetCacheItem(string cacheKey, object value, int cacheDuration = 24 * 60 * 60)
        {
            var policy = new MemoryCacheEntryOptions() { AbsoluteExpiration = DateTime.Now.AddSeconds(cacheDuration) };

            Cache.Set(cacheKey, value, policy);
        }
    }
}