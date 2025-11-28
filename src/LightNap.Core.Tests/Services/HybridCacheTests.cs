using Microsoft.Extensions.Caching.Hybrid;
using Moq;
using LightNap.Core.Extensions;

namespace LightNap.Core.Tests.Services
{
    /// Originally from From https://github.com/dotnet/aspnetcore/discussions/57191#discussioncomment-11898121.
    [TestClass]
    public class HybridCacheExtensionsTests
    {
#pragma warning disable CS8618
        private Mock<HybridCache> _cacheMock;
#pragma warning restore CS8618
        private readonly HybridCacheEntryFlags _flags = HybridCacheEntryFlags.DisableLocalCacheWrite | HybridCacheEntryFlags.DisableDistributedCacheWrite;

        [TestInitialize]
        public void TestInitialize()
        {
            this._cacheMock = new Mock<HybridCache>();
        }

        #region ExistsAsync Tests

        [TestMethod]
        public async Task ExistsAsync_WhenKeyExists_ShouldReturnTrue()
        {
            // Arrange
            string key = "test-key";
            var expectedValue = "test-value";

            this._cacheMock
                .Setup(cache => cache.GetOrCreateAsync(
                    key,
                    null!,
                    It.IsAny<Func<object, CancellationToken, ValueTask<object>>>(),
                    It.Is<HybridCacheEntryOptions>(value => value.Flags == _flags),
                    null,
                    TestContext.CancellationToken))
                .ReturnsAsync(expectedValue);

            // Act
            var exists = await HybridCacheExtensions.ExistsAsync(this._cacheMock.Object, key, TestContext.CancellationToken);

            // Assert
            Assert.IsTrue(exists);
        }

        [TestMethod]
        public async Task ExistsAsync_WhenKeyDoesNotExist_ShouldReturnFalse()
        {
            // Arrange
            string key = "test-key";

            this._cacheMock
                .Setup(cache => cache.GetOrCreateAsync(
                    key,
                    null!,
                    It.IsAny<Func<object, CancellationToken, ValueTask<object>>>(),
                    It.Is<HybridCacheEntryOptions>(value => value.Flags == _flags),
                    null,
                    TestContext.CancellationToken))
                .Returns((
                    string key,
                    object? state,
                    Func<object, CancellationToken, ValueTask<object>> factory,
                    HybridCacheEntryOptions? options,
                    IEnumerable<string>? tags,
                    CancellationToken token) =>
                {
                    return factory(state!, token);
                });

            // Act
            var exists = await HybridCacheExtensions.ExistsAsync(this._cacheMock.Object, key, TestContext.CancellationToken);

            // Assert
            Assert.IsFalse(exists);
        }

        #endregion

        #region TryGetValueAsync Tests

        [TestMethod]
        public async Task TryGetValueAsync_WhenKeyExists_ShouldReturnTrueAndValueAsString()
        {
            // Arrange
            string key = "test-key";
            var expectedValue = "test-value";

            this._cacheMock
                .Setup(cache => cache.GetOrCreateAsync(
                    key,
                    null!,
                    It.IsAny<Func<object, CancellationToken, ValueTask<string>>>(),
                    It.Is<HybridCacheEntryOptions>(value => value.Flags == _flags),
                    null,
                    TestContext.CancellationToken))
                .ReturnsAsync(expectedValue);

            // Act
            var (exists, value) = await HybridCacheExtensions.TryGetValueAsync<string>(this._cacheMock.Object, key, TestContext.CancellationToken);

            // Assert
            Assert.IsTrue(exists);
            Assert.AreEqual(expectedValue, value);
        }

        [TestMethod]
        public async Task TryGetValueAsync_WhenKeyExists_ShouldReturnTrueAndValueAsInteger()
        {
            // Arrange
            string key = "test-key";
            var expectedValue = 5;

            this._cacheMock
                .Setup(cache => cache.GetOrCreateAsync(
                    key,
                    null!,
                    It.IsAny<Func<object, CancellationToken, ValueTask<int>>>(),
                    It.Is<HybridCacheEntryOptions>(value => value.Flags == _flags),
                    null,
                    TestContext.CancellationToken))
                .ReturnsAsync(expectedValue);

            // Act
            var (exists, value) = await HybridCacheExtensions.TryGetValueAsync<int>(this._cacheMock.Object, key, TestContext.CancellationToken);

            // Assert
            Assert.IsTrue(exists);
            Assert.AreEqual(expectedValue, value);
        }

        [TestMethod]
        public async Task TryGetValueAsync_WhenKeyExistsButValueIsNull_ShouldReturnTrueAndNullValue()
        {
            // Arrange
            string key = "test-key";

            this._cacheMock
                .Setup(cache => cache.GetOrCreateAsync(
                    key,
                    null!,
                    It.IsAny<Func<object, CancellationToken, ValueTask<object>>>(),
                    It.Is<HybridCacheEntryOptions>(value => value.Flags == _flags),
                    null,
                    TestContext.CancellationToken))
                .ReturnsAsync(null!);

            // Act
            var (exists, value) = await HybridCacheExtensions.TryGetValueAsync<int?>(this._cacheMock.Object, key, TestContext.CancellationToken);

            // Assert
            Assert.IsTrue(exists);
            Assert.IsNull(value);
        }

        [TestMethod]
        public async Task TryGetValueAsync_WhenKeyDoesNotExist_ShouldReturnFalseAndNull()
        {
            // Arrange
            string key = "test-key";

            this._cacheMock.Setup(cache => cache.GetOrCreateAsync(
                key,
                null,
                It.IsAny<Func<object?, CancellationToken, ValueTask<string>>>(),
                It.Is<HybridCacheEntryOptions>(value => value.Flags == _flags),
                null,
                TestContext.CancellationToken))
                .Returns((
                    string key,
                    object? state,
                    Func<object?, CancellationToken, ValueTask<string>> factory,
                    HybridCacheEntryOptions? options,
                    IEnumerable<string>? tags,
                    CancellationToken token) =>
                {
                    return factory(state, token);
                });

            // Act
            var (exists, value) = await HybridCacheExtensions.TryGetValueAsync<string>(this._cacheMock.Object, key, TestContext.CancellationToken);

            // Assert
            Assert.IsFalse(exists);
            Assert.IsNull(value);
        }

        #endregion

        public TestContext TestContext { get; set; }
    }
}