using System;
using System.Threading.Tasks;

namespace Cogworks.Essentials.Services.Interfaces
{
    public interface ICacheService
    {
        T GetOrAdd<T>(string key, Func<T> getValueFunction, int? cacheDurationInSeconds = null);

        Task<T> GetOrAddAsync<T>(string key, Func<Task<T>> getValueFunction,
            int? cacheDurationInSeconds = null);

        T Get<T>(string key);

        void Add(string key, object value, int? cacheDurationInSeconds = null);

        void Remove(string key);

        bool Contains(string key);

        void ClearAll();

        void ClearAllStartingWith(string keyPrefix);
    }
}