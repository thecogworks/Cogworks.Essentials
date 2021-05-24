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
    public class MemoryCacheServiceTests : IDisposable
    {
        private const string CacheKeyList = "CacheKeyList";

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
            _cacheService.AddCacheItem(cacheKey, cacheValue);
            var cacheKeys = _cacheService.GetCacheItem<List<string>>(CacheKeyList);

            // Assert
            _cacheService.Contains(cacheKey)
                .Should().BeTrue();

            _cacheService.GetCacheItem<string>(cacheKey)
                .Should().Be(cacheValue);

            cacheKeys
                .Should().NotBeEmpty();

            cacheKeys.Contains(cacheKey)
                .Should().BeTrue();
        }

        [Fact]
        public void Should_AddCacheItem_With_CacheDuration()
        {
            // Arrange
            var cacheKey = _fixture.Create<string>();
            var cacheValue = _fixture.Create<string>();

            // Act
            _cacheService.AddCacheItem(cacheKey, cacheValue, 5);
            var cacheKeys = _cacheService.GetCacheItem<List<string>>(CacheKeyList);

            // Assert
            _cacheService.Contains(cacheKey)
                .Should().BeTrue();

            _cacheService.GetCacheItem<string>(cacheKey)
                .Should().Be(cacheValue);

            cacheKeys
                .Should().NotBeEmpty();

            cacheKeys.Contains(cacheKey)
                .Should().BeTrue();
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
            var result = _cacheService.GetCacheItem<TOutput>(cacheKey);

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
            var result = _cacheService.GetOrAddCacheItem(cacheKey, getValueFunction);
            var cacheKeys = _cacheService.GetCacheItem<List<string>>(CacheKeyList);

            // Assert
            result.Should().Be(expectedValue);
            counter.Should().Be(1);

            _cacheService.Contains(cacheKey)
                .Should().BeTrue();

            cacheKeys
                .Should().NotBeEmpty();

            cacheKeys.Contains(cacheKey)
                .Should().BeTrue();

            // Act
            result = _cacheService.GetOrAddCacheItem(cacheKey, getValueFunction);
            cacheKeys = _cacheService.GetCacheItem<List<string>>(CacheKeyList);

            // Assert
            result.Should().Be(expectedValue);
            counter.Should().Be(1);

            _cacheService.Contains(cacheKey)
                .Should().BeTrue();

            cacheKeys
                .Should().NotBeEmpty();

            cacheKeys.Contains(cacheKey)
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
            var result = await _cacheService.GetOrAddCacheItemAsync(cacheKey, getValueFunction);
            var cacheKeys = _cacheService.GetCacheItem<List<string>>(CacheKeyList);

            // Assert
            result.Should().Be(expectedValue);
            counter.Should().Be(1);

            _cacheService.Contains(cacheKey)
                .Should().BeTrue();

            cacheKeys
                .Should().NotBeEmpty();

            cacheKeys.Contains(cacheKey)
                .Should().BeTrue();

            // Act
            result = await _cacheService.GetOrAddCacheItemAsync(cacheKey, getValueFunction);
            cacheKeys = _cacheService.GetCacheItem<List<string>>(CacheKeyList);

            // Assert
            result.Should().Be(expectedValue);
            counter.Should().Be(1);

            _cacheService.Contains(cacheKey)
                .Should().BeTrue();

            cacheKeys
                .Should().NotBeEmpty();

            cacheKeys.Contains(cacheKey)
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
            var result = _cacheService.GetOrAddCacheItem(cacheKey, getValueFunction, 10);
            var cacheKeys = _cacheService.GetCacheItem<List<string>>(CacheKeyList);

            // Assert
            result.Should().Be(expectedValue);
            counter.Should().Be(1);

            _cacheService.Contains(cacheKey)
                .Should().BeTrue();

            cacheKeys
                .Should().NotBeEmpty();

            cacheKeys.Contains(cacheKey)
                .Should().BeTrue();

            // Act
            result = _cacheService.GetOrAddCacheItem(cacheKey, getValueFunction);
            cacheKeys = _cacheService.GetCacheItem<List<string>>(CacheKeyList);

            // Assert
            result.Should().Be(expectedValue);
            counter.Should().Be(1);

            _cacheService.Contains(cacheKey)
                .Should().BeTrue();

            cacheKeys
                .Should().NotBeEmpty();

            cacheKeys.Contains(cacheKey)
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
            var result = await _cacheService.GetOrAddCacheItemAsync(cacheKey, getValueFunction, 10);
            var cacheKeys = _cacheService.GetCacheItem<List<string>>(CacheKeyList);

            // Assert
            result.Should().Be(expectedValue);
            counter.Should().Be(1);

            _cacheService.Contains(cacheKey)
                .Should().BeTrue();

            cacheKeys
                .Should().NotBeEmpty();

            cacheKeys.Contains(cacheKey)
                .Should().BeTrue();

            // Act
            result = await _cacheService.GetOrAddCacheItemAsync(cacheKey, getValueFunction);
            cacheKeys = _cacheService.GetCacheItem<List<string>>(CacheKeyList);

            // Assert
            result.Should().Be(expectedValue);
            counter.Should().Be(1);

            _cacheService.Contains(cacheKey)
                .Should().BeTrue();

            cacheKeys
                .Should().NotBeEmpty();

            cacheKeys.Contains(cacheKey)
                .Should().BeTrue();
        }

        [Fact]
        public void Should_RemoveCacheItem()
        {
            var firstCacheKey = _fixture.Create<string>();
            var firstCacheValue = _fixture.Create<string>();

            _cacheService.AddCacheItem(firstCacheKey, firstCacheValue);

            var cacheKeys = _cacheService.GetCacheItem<List<string>>(CacheKeyList);

            cacheKeys
                .Should()
                .NotBeEmpty();

            cacheKeys.Contains(firstCacheKey)
                .Should()
                .BeTrue();

            _cacheService.RemoveCacheItem(firstCacheKey);

            cacheKeys = _cacheService.GetCacheItem<List<string>>(CacheKeyList);

            cacheKeys
                .Should()
                .BeEmpty();

            _cacheService.Contains(firstCacheKey)
                .Should()
                .BeFalse();
        }

        [Fact]
        public void Should_Not_ThrowException_On_RemoveCacheItem_When_ItemNotInCache()
        {
            var firstCacheKey = _fixture.Create<string>();

            var cacheKeys = _cacheService.GetCacheItem<List<string>>(CacheKeyList);

            cacheKeys
                .Should()
                .BeEmpty();

            var exception = Record.Exception(() =>
                _cacheService.RemoveCacheItem(firstCacheKey));

            exception.Should().BeNull();

            cacheKeys = _cacheService.GetCacheItem<List<string>>(CacheKeyList);

            cacheKeys
                .Should()
                .BeEmpty();

            _cacheService.Contains(firstCacheKey)
                .Should()
                .BeFalse();
        }

        [Fact]
        public void Should_ClearAllCache()
        {
            var firstCacheKey = _fixture.Create<string>();
            var firstCacheValue = _fixture.Create<string>();

            var secondCacheKey = _fixture.Create<string>();
            var secondCacheValue = _fixture.Create<string>();

            _cacheService.AddCacheItem(firstCacheKey, firstCacheValue);
            _cacheService.AddCacheItem(secondCacheKey, secondCacheValue);

            var cacheKeys = _cacheService.GetCacheItem<List<string>>(CacheKeyList);

            cacheKeys
                .Should()
                .NotBeEmpty();

            _cacheService.ClearAll();

            cacheKeys = _cacheService.GetCacheItem<List<string>>(CacheKeyList);

            cacheKeys
                .Should()
                .BeEmpty();

            _cacheService.Contains(firstCacheKey)
                .Should()
                .BeFalse();

            _cacheService.Contains(secondCacheKey)
                .Should()
                .BeFalse();
        }

        [Fact]
        public void Should_Not_ThrowException_On_ClearingAllCache_When_NoItemsInCache()
        {
            var exception = Record.Exception(()
                => _cacheService.ClearAll());

            exception.Should().BeNull();

            var cacheKeys = _cacheService.GetCacheItem<List<string>>(CacheKeyList);

            cacheKeys
                .Should()
                .BeEmpty();
        }

        [Fact]
        public void Should_ClearAllStartingWithPrefix()
        {
            const string prefix = "prefix";

            var cachePrefixKeys = Enumerable.Range(0, 100)
                .Select(index => $"{prefix}_{index}")
                .ToArray();

            foreach (var cacheKey in cachePrefixKeys)
            {
                _cacheService.AddCacheItem(cacheKey, _fixture.Create<string>());
            }

            var cacheKeys = _cacheService.GetCacheItem<List<string>>(CacheKeyList);

            cacheKeys
                .Should()
                .NotBeEmpty();

            cacheKeys.Should().Contain(cachePrefixKeys);

            foreach (var cachePrefixKey in cachePrefixKeys)
            {
                _cacheService.Contains(cachePrefixKey)
                    .Should().BeTrue();
            }

            _cacheService.ClearAllStartingWith(prefix);

            cacheKeys = _cacheService.GetCacheItem<List<string>>(CacheKeyList);

            cacheKeys
                .Should()
                .BeEmpty();

            foreach (var cachePrefixKey in cachePrefixKeys)
            {
                _cacheService.Contains(cachePrefixKey)
                    .Should().BeFalse();
            }
        }

        [Fact]
        public void Should_Not_ThrowException_On_ClearingAllStartingWithPrefix_When_NoItemsInCache()
        {
            const string prefix = "prefix";

            var cachePrefixKeys = Enumerable.Range(0, 100)
                .Select(index => $"{prefix}_{index}")
                .ToArray();

            foreach (var cachePrefixKey in cachePrefixKeys)
            {
                _cacheService.Contains(cachePrefixKey)
                    .Should().BeFalse();
            }

            var exception = Record.Exception(()
                => _cacheService.ClearAllStartingWith(prefix));

            exception.Should().BeNull();

            var cacheKeys = _cacheService.GetCacheItem<List<string>>(CacheKeyList);

            cacheKeys
                .Should()
                .BeEmpty();

            foreach (var cachePrefixKey in cachePrefixKeys)
            {
                _cacheService.Contains(cachePrefixKey)
                    .Should().BeFalse();
            }
        }

        // test for update item in cache with same key
        // test for parallel invocation

        public void Dispose()
            => _inMemoryCache.Dispose();
    }
}