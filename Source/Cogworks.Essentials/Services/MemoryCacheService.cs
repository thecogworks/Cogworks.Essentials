using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cogworks.Essentials.Constants;
using Cogworks.Essentials.EventArgs;
using Cogworks.Essentials.Extensions;
using Cogworks.Essentials.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace Cogworks.Essentials.Services
{
    public class MemoryCacheService : ICacheService, IDisposable
    {
        private readonly IMemoryCache _memoryCache;

        private ImmutableHashSet<string> _cacheKeys = ImmutableHashSet<string>.Empty;

        private static ConcurrentDictionary<object, SemaphoreSlim> Locks => new ConcurrentDictionary<object, SemaphoreSlim>();

        public MemoryCacheService(IMemoryCache memoryCache)
            => _memoryCache = memoryCache;

        public MemoryCacheService()
            => _memoryCache = new MemoryCache(new MemoryCacheOptions());

        public bool Contains(string key)
            => _memoryCache.TryGetValue(key, out _);

        public T Get<T>(string key)
            => _memoryCache.Get(key) is T cachedObject
                ? cachedObject
                : default;

        public void Remove(string key)
            => _memoryCache.Remove(key);

        public void Add(string key, object value, int? cacheDurationInSeconds = null)
        {
            AddCacheKeyToCacheKeysDefinitions(key);

            cacheDurationInSeconds ??= DateTimeConstants.TimeInSecondsConstants.Hour;
            var cacheDurationDateTime = DateTime.UtcNow.AddSeconds(cacheDurationInSeconds.Value);

            var entryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(cacheDurationDateTime)
                .RegisterPostEvictionCallback(CacheCallback);

            _memoryCache.Set(key, value, entryOptions);
        }

        public T GetOrAdd<T>(string key, Func<T> getValueFunction, int? cacheDurationInSeconds = null)
        {
            var cacheEntry = _memoryCache.GetOrCreate(key, entry =>
            {
                cacheDurationInSeconds ??= DateTimeConstants.TimeInSecondsConstants.Hour;
                var cacheDurationDateTime = DateTime.UtcNow.AddSeconds(cacheDurationInSeconds.Value);

                entry.AbsoluteExpiration = cacheDurationDateTime;

                entry.PostEvictionCallbacks.Add(new PostEvictionCallbackRegistration()
                {
                    EvictionCallback = CacheCallback
                });

                AddCacheKeyToCacheKeysDefinitions(key);

                var factoryItem = new Lazy<T>(getValueFunction);

                return factoryItem.Value;
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
        public async Task<T> GetOrAddAsync<T>(string key, Func<Task<T>> getValueFunction, int? cacheDurationInSeconds = null)
        {
            var hasEntry = _memoryCache.TryGetValue(key, out T cacheEntry);

            if (hasEntry)
            {
                return cacheEntry;
            }

            var myLock = Locks.GetOrAdd(key, k => new SemaphoreSlim(1, 1));
            await myLock.WaitAsync();

            try
            {
                if (!hasEntry)
                {
                    cacheEntry = await getValueFunction();

                    cacheDurationInSeconds ??= DateTimeConstants.TimeInSecondsConstants.Hour;
                    var cacheDurationDateTime = DateTime.UtcNow.AddSeconds(cacheDurationInSeconds.Value);

                    var entryOptions = new MemoryCacheEntryOptions()
                        .SetAbsoluteExpiration(cacheDurationDateTime)
                        .RegisterPostEvictionCallback(CacheCallback);

                    _memoryCache.Set(key, cacheEntry, entryOptions);
                }
            }
            finally
            {
                myLock.Release();
            }

            AddCacheKeyToCacheKeysDefinitions(key);
            return cacheEntry;
        }

        public void ClearAllStartingWith(string keyPrefix)
        {
            var cacheKeys = _cacheKeys
                .Where(x => x.StartsWith(keyPrefix))
                .ToList();

            if (!cacheKeys.HasAny())
            {
                return;
            }

            foreach (var key in cacheKeys)
            {
                _memoryCache.Remove(key);
            }
        }

        public bool TryGetValue<T>(string key, out T value)
            => _memoryCache.TryGetValue(key, out value);

        public void ClearAll()
        {
            var cacheKeys = _cacheKeys.ToArray();

            if (!_cacheKeys.HasAny())
            {
                return;
            }

            foreach (var key in cacheKeys)
            {
                _memoryCache.Remove(key);
            }
        }

        public IEnumerable<string> GetKeys()
            => _cacheKeys.ToList();

        public void Dispose()
        {
            _cacheKeys = _cacheKeys.Clear();
            _memoryCache.Dispose();
        }

        private void AddCacheKeyToCacheKeysDefinitions(string cacheKey)
            => ImmutableInterlocked.Update(
                ref _cacheKeys,
                (collection, item) => collection.Add(item),
                cacheKey);

        private void RemoveCacheKeyFromCacheKeysDefinitions(string cacheKey)
            => ImmutableInterlocked.Update(
                ref _cacheKeys,
                (collection, item) => collection.Remove(item),
                cacheKey);

        private void CacheCallback(object key, object value, EvictionReason reason, object state)
        {
            if (reason == EvictionReason.Replaced || !(key is string cacheKey))
            {
                return;
            }

            CacheEvictionEvent?.Invoke(
                this,
                new CacheEvictionArgs(key, value, reason));

            RemoveCacheKeyFromCacheKeysDefinitions(cacheKey);
        }

        public event EventHandler<CacheEvictionArgs> CacheEvictionEvent;
    }
}