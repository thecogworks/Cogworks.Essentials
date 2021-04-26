using System;
using System.Linq;
using System.Runtime.Caching;
using Cogworks.Essentials.Constants;
using Cogworks.Essentials.Services.Interfaces;

namespace Cogworks.Essentials.Services
{
    public class MemoryCacheService : IMemoryCacheService
    {
        private static MemoryCache MemoryCache => MemoryCache.Default;

        public string GetCacheKey<TCachedProxy, TReturnedType>()
        {
            var cacheKey = $"{typeof(TCachedProxy)}_{typeof(TReturnedType)}";
            return cacheKey;
        }

        public T GetOrAddValue<T>(string cacheKey, Func<T> getValueFunction, int? cacheDuration)
        {
            if (MemoryCache.Get(cacheKey) is T itemToBeCached)
            {
                return itemToBeCached;
            }

            itemToBeCached = getValueFunction();

            if (itemToBeCached == null)
            {
                return default;
            }

            if (!cacheDuration.HasValue)
            {
                cacheDuration = DateTimeConstants.TimeInMillisecondsConstants.Hour;
            }

            var cacheDurationDateTime = DateTime.UtcNow.AddMilliseconds(cacheDuration.Value);
            MemoryCache.Add(cacheKey, itemToBeCached, cacheDurationDateTime);

            return itemToBeCached;
        }

        public void DeleteAllStartingWith(string key)
        {
            if (MemoryCache.Any(kv => kv.Key.StartsWith(key)))
            {
                var entries = MemoryCache.Where(kv => kv.Key.StartsWith(key)).ToList();

                foreach (var entry in entries)
                {
                    MemoryCache.Remove(entry.Key);
                }
            }
        }

        public void DeleteAll()
        {
            if (!MemoryCache.Any())
            {
                return;
            }

            var entries = MemoryCache.ToList();

            foreach (var entry in entries)
            {
                MemoryCache.Remove(entry.Key);
            }
        }
    }
}
