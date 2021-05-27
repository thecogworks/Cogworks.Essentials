using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using Cogworks.Essentials.Services;
using Cogworks.Essentials.Services.Interfaces;
using Cogworks.Essentials.UnitTests.Models;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Xunit;

namespace Cogworks.Essentials.UnitTests.Services
{
    [CollectionDefinition("Memory Cache Service tests", DisableParallelization = false)]
    [Collection("Memory Cache Service tests")]
    public class MemoryCacheServiceTests : IDisposable
    {
        private readonly IMemoryCache _inMemoryCache;
        private readonly ICacheService _cacheService;
        private readonly IFixture _fixture;

        public MemoryCacheServiceTests()
        {
            _inMemoryCache = new MemoryCache(new MemoryCacheOptions());
            _cacheService = new MemoryCacheService(_inMemoryCache);
            _fixture = new Fixture()
                .Customize(new AutoNSubstituteCustomization());
        }

        [Fact]
        public void Should_AddCacheItem_Without_CacheDuration()
        {
            // Arrange
            var cacheKey = _fixture.Create<string>();
            var cacheValue = _fixture.Create<string>();

            // Act
            _cacheService.Add(cacheKey, cacheValue);

            // Assert
            _cacheService.Contains(cacheKey)
                .Should().BeTrue();

            _cacheService.Get<string>(cacheKey)
                .Should().Be(cacheValue);
        }

        [Fact]
        public void Should_AddCacheItem_With_CacheDuration()
        {
            // Arrange
            var cacheKey = _fixture.Create<string>();
            var cacheValue = _fixture.Create<string>();

            // Act
            _cacheService.Add(cacheKey, cacheValue, 5);

            // Assert
            _cacheService.Contains(cacheKey)
                .Should().BeTrue();

            _cacheService.Get<string>(cacheKey)
                .Should().Be(cacheValue);
        }

        public static IEnumerable<object[]> DefaultTestData => new List<object[]>
        {
            new object[] { string.Empty, null },
            new object[] { 0, 0 },
            new object[] { new { string.Empty }, null },
            new object[] { new object(), null },
        };

        [Theory]
        [MemberData(nameof(DefaultTestData))]
        public void Should_ReturnDefault_When_CacheDoesNotContainKey<TInput, TOutput>(TInput _, TOutput expected)
        {
            // Arrange
            var cacheKey = _fixture.Create<string>();

            // Act
            var result = _cacheService.Get<TOutput>(cacheKey);

            // Assert
            result.Should().Be(expected);

            _cacheService.Contains(cacheKey)
                .Should().BeFalse();
        }

        public static IEnumerable<object[]> GetOrAddTestData => new List<object[]>
        {
            new object[] { "test_value" },
            new object[] { 123 },
            new object[] { new TestObject("123") },
        };

        [Theory]
        [MemberData(nameof(GetOrAddTestData))]
        public void Should_GetOrAdd<TResultType>(TResultType expectedValue)
        {
            // Arrange
            var cacheKey = _fixture.Create<string>();
            var counter = 0;

            Func<TResultType> getValueFunction = () =>
            {
                counter++;
                return expectedValue;
            };

            // Act
            var result = _cacheService.GetOrAdd(cacheKey, getValueFunction);

            // Assert
            result.Should().Be(expectedValue);
            counter.Should().Be(1);

            _cacheService.Contains(cacheKey)
                .Should().BeTrue();

            // Act
            result = _cacheService.GetOrAdd(cacheKey, getValueFunction);

            // Assert
            result.Should().Be(expectedValue);
            counter.Should().Be(1);

            _cacheService.Contains(cacheKey)
                .Should().BeTrue();
        }

        [Theory]
        [MemberData(nameof(GetOrAddTestData))]
        public async Task Should_GetOrAddAsync<TResultType>(TResultType expectedValue)
        {
            // Arrange
            var cacheKey = _fixture.Create<string>();
            var counter = 0;

            Func<Task<TResultType>> getValueFunction = async () =>
            {
                await Task.Delay(50);
                counter++;
                return expectedValue;
            };

            // Act
            var result = await _cacheService.GetOrAddAsync(cacheKey, getValueFunction);

            // Assert
            result.Should().Be(expectedValue);
            counter.Should().Be(1);

            _cacheService.Contains(cacheKey)
                .Should().BeTrue();

            // Act
            result = await _cacheService.GetOrAddAsync(cacheKey, getValueFunction);

            // Assert
            result.Should().Be(expectedValue);
            counter.Should().Be(1);

            _cacheService.Contains(cacheKey)
                .Should().BeTrue();
        }

        [Theory]
        [MemberData(nameof(GetOrAddTestData))]
        public void Should_GetOrAdd_With_CacheDuration<TResultType>(TResultType expectedValue)
        {
            // Arrange
            var cacheKey = _fixture.Create<string>();
            var counter = 0;

            Func<TResultType> getValueFunction = () =>
            {
                counter++;
                return expectedValue;
            };

            // Act
            var result = _cacheService.GetOrAdd(cacheKey, getValueFunction, 10);

            // Assert
            result.Should().Be(expectedValue);
            counter.Should().Be(1);

            _cacheService.Contains(cacheKey)
                .Should().BeTrue();

            // Act
            result = _cacheService.GetOrAdd(cacheKey, getValueFunction);

            // Assert
            result.Should().Be(expectedValue);
            counter.Should().Be(1);

            _cacheService.Contains(cacheKey)
                .Should().BeTrue();
        }

        [Theory]
        [MemberData(nameof(GetOrAddTestData))]
        public async Task Should_GetOrAddAsync_With_CacheDuration<TResultType>(TResultType expectedValue)
        {
            // Arrange
            var cacheKey = _fixture.Create<string>();
            var counter = 0;

            Func<Task<TResultType>> getValueFunction = async () =>
            {
                await Task.Delay(50);
                counter++;
                return expectedValue;
            };

            // Act
            var result = await _cacheService.GetOrAddAsync(cacheKey, getValueFunction, 10);

            // Assert
            result.Should().Be(expectedValue);
            counter.Should().Be(1);

            _cacheService.Contains(cacheKey)
                .Should().BeTrue();

            // Act
            result = await _cacheService.GetOrAddAsync(cacheKey, getValueFunction);

            // Assert
            result.Should().Be(expectedValue);
            counter.Should().Be(1);

            _cacheService.Contains(cacheKey)
                .Should().BeTrue();
        }

        [Fact]
        public void Should_RemoveCacheItem()
        {
            // Arrange
            var firstCacheKey = _fixture.Create<string>();
            var firstCacheValue = _fixture.Create<string>();

            // Act
            _cacheService.Add(firstCacheKey, firstCacheValue);

            _cacheService.Remove(firstCacheKey);

            // Assert
            _cacheService.Contains(firstCacheKey)
                .Should()
                .BeFalse();
        }

        [Fact]
        public void Should_Not_ThrowException_On_RemoveCacheItem_When_ItemNotInCache()
        {
            // Arrange
            var firstCacheKey = _fixture.Create<string>();

            // Act
            var exception = Record.Exception(() =>
                _cacheService.Remove(firstCacheKey));

            // Assert
            exception.Should().BeNull();

            _cacheService.Contains(firstCacheKey)
                .Should()
                .BeFalse();
        }

        [Fact]
        public void Should_ClearAllCache()
        {
            // Arrange
            var firstCacheKey = _fixture.Create<string>();
            var firstCacheValue = _fixture.Create<string>();

            var secondCacheKey = _fixture.Create<string>();
            var secondCacheValue = _fixture.Create<string>();

            // Act
            _cacheService.Add(firstCacheKey, firstCacheValue);
            _cacheService.Add(secondCacheKey, secondCacheValue);

            // Assert
            _cacheService.Contains(firstCacheKey)
                .Should()
                .BeTrue();

            _cacheService.Contains(secondCacheKey)
                .Should()
                .BeTrue();

            // Act
            _cacheService.ClearAll();

            // Assert
            _cacheService.Contains(firstCacheKey)
                .Should()
                .BeFalse();

            _cacheService.Contains(secondCacheKey)
                .Should()
                .BeFalse();
        }

        [Fact]
        public void Should_Not_ThrowException_On_ClearingAllCache_When_NoItemsInCache()
            => Record
                .Exception(() => _cacheService.ClearAll())
                .Should().BeNull();

        [Fact]
        public void Should_ClearAllStartingWithPrefix()
        {
            // Arrange
            const string prefix = "prefix";

            var cachePrefixKeys = Enumerable.Range(0, 100)
                .Select(index => $"{prefix}_{index}")
                .ToArray();

            // Act
            foreach (var cacheKey in cachePrefixKeys)
            {
                _cacheService.Add(cacheKey, _fixture.Create<string>());
            }

            // Assert
            foreach (var cachePrefixKey in cachePrefixKeys)
            {
                _cacheService.Contains(cachePrefixKey)
                    .Should().BeTrue();
            }

            // Act
            _cacheService.ClearAllStartingWith(prefix);

            // Assert
            foreach (var cachePrefixKey in cachePrefixKeys)
            {
                _cacheService.Contains(cachePrefixKey)
                    .Should().BeFalse();
            }
        }

        [Fact]
        public void Should_Not_ThrowException_On_ClearingAllStartingWithPrefix_When_NoItemsInCache()
        {
            // Arrange
            const string prefix = "prefix";

            var cachePrefixKeys = Enumerable.Range(0, 100)
                .Select(index => $"{prefix}_{index}")
                .ToArray();

            // Act
            foreach (var cachePrefixKey in cachePrefixKeys)
            {
                _cacheService.Contains(cachePrefixKey)
                    .Should().BeFalse();
            }

            var exception = Record.Exception(()
                => _cacheService.ClearAllStartingWith(prefix));

            // Assert
            exception.Should().BeNull();

            foreach (var cachePrefixKey in cachePrefixKeys)
            {
                _cacheService.Contains(cachePrefixKey)
                    .Should().BeFalse();
            }
        }

        public static IEnumerable<object[]> UpdateTestData => new List<object[]>
        {
            new object[] { "value", "updateValue" },
            new object[] { 123, 234 },
            new object[] { new TestObject("123"), new TestObject("234") },
            new object[] { 123, "234" },
        };

        [Theory]
        [MemberData(nameof(UpdateTestData))]
        public void Should_UpdateExistingItemValue<TInput, TUpdate>(TInput input, TUpdate update)
        {
            // Arrange
            var cacheKey = _fixture.Create<string>();

            // Act
            _cacheService.Add(cacheKey, input);

            // Assert
            var inputResult = _cacheService.Get<TInput>(cacheKey);

            inputResult.Should().Be(input).And.BeOfType<TInput>();

            // Act
            _cacheService.Add(cacheKey, update);

            // Assert
            var outputResult = _cacheService.Get<TUpdate>(cacheKey);

            outputResult.Should()
                .Be(outputResult)
                .And
                .BeOfType<TUpdate>()
                .And
                .NotBe(inputResult);
        }

        [Theory]
        [MemberData(nameof(UpdateTestData))]
        public async Task Should_UpdateExistingItemValueAsync<TInput, TUpdate>(TInput input, TUpdate update)
        {
            // Arrange
            var cacheKey = _fixture.Create<string>();

            // Act
            var inputResult = await _cacheService.GetOrAddAsync<TInput>(
                cacheKey,
                () => Task.FromResult(input));

            // Assert
            inputResult.Should().Be(input).And.BeOfType<TInput>();

            // Act
            _cacheService.Add(cacheKey, update);

            var outputResult = await _cacheService.GetOrAddAsync<TUpdate>(
                cacheKey,
                () => Task.FromResult(update));

            // Assert
            outputResult.Should()
                .Be(outputResult)
                .And
                .BeOfType<TUpdate>()
                .And
                .NotBe(inputResult);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(10)]
        public void Should_AddItems_When_LongThreadOperations(int threadsSize)
        {
            // Arrange
            var cacheInputs = Enumerable.Range(0, threadsSize)
                .Select(index => new
                {
                    CacheKey = $"cache_key_{index}",
                    CacheValue = index
                })
                .ToArray();

            // Act
            var cacheActions = cacheInputs
                .Select(x =>
                    Task.Run(()
                        => _cacheService.GetOrAddAsync(
                            x.CacheKey,
                            async () =>
                            {
                                await Task.Delay(x.CacheValue * 100);
                                return x.CacheValue;
                            })))
                .ToArray();

            Task.WaitAll(cacheActions);

            // Assert
            foreach (var cacheInput in cacheInputs)
            {
                _cacheService.Contains(cacheInput.CacheKey).Should().BeTrue();
                var cacheItem = _cacheService.Get<int>(cacheInput.CacheKey);

                cacheItem.Should().Be(cacheInput.CacheValue);
            }
        }

        [Fact]
        public async Task Should_RemoveCacheKeyFromCacheKeysList_When_RemoveItemFromCache()
        {
            // Arrange
            var cacheKey = _fixture.Create<string>();
            var cacheValue = _fixture.Create<string>();

            // Act
            _cacheService.Add(cacheKey, cacheValue, 2);

            // Assert
            _cacheService.Contains(cacheKey).Should().BeTrue();

            // Act
            _cacheService.Remove(cacheKey);

            await Task.Delay(200);

            // Assert
            _cacheService.Contains(cacheKey).Should().BeFalse();

            var memoryCacheService = _cacheService as MemoryCacheService;

            memoryCacheService.GetKeys().Should().BeEmpty();
        }

        [Fact]
        public async Task Should_RemoveCacheKeyFromCacheKeysList_When_CacheItemExpired()
        {
            // Arrange
            var cacheKey = _fixture.Create<string>();
            var cacheValue = _fixture.Create<string>();

            var memoryCacheService = _cacheService as MemoryCacheService;

            var counter = 0;

            memoryCacheService.CacheEvictionEvent += (_, args) =>
            {
                counter++;

                // Assert
                args.EvictionReason.Should().Be(EvictionReason.Expired);
            };

            // Act
            memoryCacheService.Add(cacheKey, cacheValue, 1);

            memoryCacheService.Contains(cacheKey).Should().BeTrue();

            var delayCount = 1100;

            do
            {
                delayCount -= 100;
                await Task.Delay(100);
            }
            while (delayCount > 0);

            memoryCacheService.Contains(cacheKey).Should().BeFalse();

            await Task.Delay(100);

            counter.Should().BeGreaterThan(0);
            memoryCacheService.GetKeys().Should().BeEmpty();
        }

        [Fact]
        public async Task Should_RemoveCacheKeyFromCacheKeysList_When_CacheItemExpired_On_GetOrAdd()
        {
            // Arrange
            var cacheKey = _fixture.Create<string>();
            var cacheValue = _fixture.Create<string>();

            var memoryCacheService = _cacheService as MemoryCacheService;

            var counter = 0;

            memoryCacheService.CacheEvictionEvent += (_, args) =>
            {
                counter++;

                // Assert
                args.EvictionReason.Should().Be(EvictionReason.Expired);
            };

            // Act
            memoryCacheService.GetOrAdd<string>(cacheKey, () => cacheValue, 1);

            // Assert
            memoryCacheService.Contains(cacheKey).Should().BeTrue();

            var delayCount = 1100;

            do
            {
                delayCount -= 100;
                await Task.Delay(100);
            }
            while (delayCount > 0);

            memoryCacheService.Contains(cacheKey).Should().BeFalse();

            await Task.Delay(100);

            counter.Should().BeGreaterThan(0);
            memoryCacheService.GetKeys().Should().BeEmpty();
        }

        [Fact]
        public async Task Should_RemoveCacheKeyFromCacheKeysList_When_CacheItemExpired_On_GetOrAddAsync()
        {
            // Arrange
            var cacheKey = _fixture.Create<string>();
            var cacheValue = _fixture.Create<string>();

            var memoryCacheService = _cacheService as MemoryCacheService;

            var counter = 0;

            memoryCacheService.CacheEvictionEvent += (_, args) =>
            {
                counter++;

                // Assert
                args.EvictionReason.Should().Be(EvictionReason.Expired);
            };

            // Act
            await memoryCacheService.GetOrAddAsync<string>(cacheKey, () => Task.FromResult(cacheValue), 1);

            // Assert
            memoryCacheService.Contains(cacheKey).Should().BeTrue();

            var delayCount = 1100;

            do
            {
                delayCount -= 100;
                await Task.Delay(100);
            }
            while (delayCount > 0);

            memoryCacheService.Contains(cacheKey).Should().BeFalse();

            await Task.Delay(100);

            counter.Should().BeGreaterThan(0);
            memoryCacheService.GetKeys().Should().BeEmpty();
        }

        [Fact]
        public void Should_Return_True_When_CacheContainsItem()
        {
            // Arrange
            var cacheKey = _fixture.Create<string>();
            var cacheValue = _fixture.Create<string>();

            // Act
            _cacheService.Add(cacheKey, cacheValue);

            // Assert
            _cacheService.Contains(cacheKey).Should().BeTrue();
        }

        [Fact]
        public void Should_Return_False_When_CacheDoesNotContainsItem()
        {
            // Arrange
            var cacheKey = _fixture.Create<string>();

            // Act + Assert
            _cacheService.Contains(cacheKey).Should().BeFalse();
        }

        [Fact]
        public void Should_DisposeCache()
        {
            // Arrange
            var memoryCacheService = _cacheService as MemoryCacheService;

            // Act
            memoryCacheService.Add(
                _fixture.Create<string>(),
                _fixture.Create<string>());

            memoryCacheService.Dispose();

            // Assert
            memoryCacheService.GetKeys().Should().BeEmpty();
        }

        public void Dispose()
            => _inMemoryCache.Dispose();
    }
}