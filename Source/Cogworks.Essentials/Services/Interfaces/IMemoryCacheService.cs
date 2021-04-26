using System;

namespace Cogworks.Essentials.Services.Interfaces
{
    public interface IMemoryCacheService
    {
        string GetCacheKey<TCachedProxy, TReturnedType>();

        T GetOrAddValue<T>(string cacheKey, Func<T> getValueFunction, int? cacheDuration = null);

        void DeleteAllStartingWith(string key);

        void DeleteAll();
    }
}
