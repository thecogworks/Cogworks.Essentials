namespace Cogworks.Essentials.Helpers
{
    public static class CacheKeyGeneratorHelper
    {
        public static string GetCacheKey<TCachedProxy, TReturnedType>()
        {
            var cacheKey = $"{typeof(TCachedProxy)}_{typeof(TReturnedType)}";

            return cacheKey;
        }

        public static string GetCacheKey<TCachedProxy, TReturnedType>(int nodeId)
        {
            var cacheKey = $"{typeof(TCachedProxy)}_{typeof(TReturnedType)}_{nodeId}";

            return cacheKey;
        }
    }
}