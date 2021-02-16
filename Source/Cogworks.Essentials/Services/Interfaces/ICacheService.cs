namespace Cogworks.Essentials.Services
{
    public interface ICacheService
    {
        T GetCacheItem<T>(string cacheKey);

        void SetCacheItem(string cacheKey, object value, int cacheDuration = 24 * 60 * 60);

        void RemoveCacheItem(string cacheKey);

        bool Contains(string cacheKey);
    }
}