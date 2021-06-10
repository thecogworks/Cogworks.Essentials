using System.Linq;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using Cogworks.Essentials.Services;
using Cogworks.Essentials.Services.Interfaces;

namespace BenchmarkTests.Benchmarks
{
    [SimpleJob(RunStrategy.ColdStart, invocationCount: 5)]
    public class MemoryCacheBenchmark
    {
        private class TestData<T>
        {
            public string Key { get; set; }

            public T Value { get; set; }
        }

        private const string Prefix = "prefix";

        private TestData<int>[] _testData;

        [Params(1, 2, 1000, 10000, 100000)]
        public int ItemsCount { get; set; }

        [GlobalSetup]
        public void GlobalSetup()
            => _testData = Enumerable.Range(0, ItemsCount)
                .Select(index => new TestData<int>
                {
                    Key = $"{Prefix}_{index}",
                    Value = index
                })
                .ToArray();

        [Benchmark]
        public ICacheService AddingCacheItems()
        {
            var memoryCacheService = new MemoryCacheService();

            for (var index = 0; index < ItemsCount; index++)
            {
                var item = _testData[index];

                memoryCacheService.Add(item.Key, item.Value);
            }

            return memoryCacheService;
        }

        [Benchmark]
        public string[] GetKeys()
        {
            var memoryCacheService = new MemoryCacheService();

            for (var index = 0; index < ItemsCount; index++)
            {
                var item = _testData[index];

                memoryCacheService.Add(item.Key, item.Value);
            }

            return memoryCacheService.GetKeys().ToArray();
        }

        [Benchmark]
        public void GetCacheItems()
        {
            var memoryCacheService = new MemoryCacheService();

            for (var index = 0; index < ItemsCount; index++)
            {
                var item = _testData[index];

                memoryCacheService.Add(item.Key, item.Value);
            }

            for (var index = 0; index < ItemsCount; index++)
            {
                _ = memoryCacheService.Get<int>(_testData[index].Key);
            }
        }

        [Benchmark]
        public void GetOrAddCacheItem()
        {
            var memoryCacheService = new MemoryCacheService();

            for (var index = 0; index < ItemsCount; index++)
            {
                var item = _testData[index];

                _ = memoryCacheService.GetOrAdd(item.Key, () => item.Value);
            }

            for (var index = 0; index < ItemsCount; index++)
            {
                var item = _testData[index];

                _ = memoryCacheService.GetOrAdd(item.Key, () => item.Value);
            }
        }

        [Benchmark]
        public async Task GetOrAddAsyncCacheItem()
        {
            var memoryCacheService = new MemoryCacheService();

            for (var index = 0; index < ItemsCount; index++)
            {
                var item = _testData[index];

                _ = await memoryCacheService.GetOrAddAsync(
                    item.Key,
                    () => Task.FromResult(item.Value));
            }

            for (var index = 0; index < ItemsCount; index++)
            {
                var item = _testData[index];

                _ = await memoryCacheService.GetOrAddAsync(
                    item.Key,
                    () => Task.FromResult(item.Value));
            }
        }

        [Benchmark]
        public void TryGetCacheItem()
        {
            var memoryCacheService = new MemoryCacheService();

            for (var index = 0; index < ItemsCount; index++)
            {
                var item = _testData[index];

                memoryCacheService.Add(item.Key, item.Value);
            }

            for (var index = 0; index < ItemsCount; index++)
            {
                var item = _testData[index];

                _ = memoryCacheService.TryGetValue<int>(item.Key, out _);
            }
        }

        [Benchmark]
        public void RemoveCacheItem()
        {
            var memoryCacheService = new MemoryCacheService();

            for (var index = 0; index < ItemsCount; index++)
            {
                var item = _testData[index];

                memoryCacheService.Add(item.Key, item.Value);
            }

            for (var index = 0; index < ItemsCount; index++)
            {
                var item = _testData[index];

                memoryCacheService.Remove(item.Key);
            }
        }

        [Benchmark]
        public void ClearAllCacheItem()
        {
            var memoryCacheService = new MemoryCacheService();

            for (var index = 0; index < ItemsCount; index++)
            {
                var item = _testData[index];

                memoryCacheService.Add(item.Key, item.Value);
            }

            memoryCacheService.ClearAll();
        }

        [Benchmark]
        public void ClearAllStartingWithPrefix()
        {
            var memoryCacheService = new MemoryCacheService();

            for (var index = 0; index < ItemsCount; index++)
            {
                var item = _testData[index];

                memoryCacheService.Add(item.Key, item.Value);
            }

            memoryCacheService.ClearAllStartingWith(Prefix);
        }
    }
}