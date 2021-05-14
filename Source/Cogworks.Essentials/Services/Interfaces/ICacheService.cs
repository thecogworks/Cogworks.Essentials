using System;
using System.Threading.Tasks;

namespace Cogworks.Essentials.Services.Interfaces
{
    public interface ICacheService
    {
        T GetOrAddCacheItem<T>(string cacheKey, Func<T> getValueFunction, int? cacheDurationInSeconds);

        Task<T> GetOrAddCacheItemAsync<T>(string cacheKey, Func<Task<T>> getValueFunction,
            int? cacheDurationInSeconds);

        T GetCacheItem<T>(string cacheKey);

        void AddCacheItem(string cacheKey, object value, int? cacheDurationInSeconds);

        void RemoveCacheItem(string cacheKey);

        bool Contains(string cacheKey);
    }
}