using System;
using System.Threading.Tasks;

namespace Cogworks.Essentials.Services.Interfaces
{
    public interface ICacheService
    {
        T GetOrAddCacheItem<T>(string cacheKey, Func<T> getValueFunction, int? cacheDurationInSeconds = null);

        Task<T> GetOrAddCacheItemAsync<T>(string cacheKey, Func<Task<T>> getValueFunction,
            int? cacheDurationInSeconds = null);

        T GetCacheItem<T>(string cacheKey);

        void AddCacheItem(string cacheKey, object value, int? cacheDurationInSeconds = null);

        void RemoveCacheItem(string cacheKey);

        bool Contains(string cacheKey);

        void ClearAll();

        void ClearAllStartingWith(string prefixKey);
    }
}