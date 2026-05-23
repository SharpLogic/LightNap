using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.DependencyInjection;
using LightNap.Core.Extensions;

namespace LightNap.Core.Tests.Services
{
    /// Originally from From https://github.com/dotnet/aspnetcore/discussions/57191#discussioncomment-11898121.
    [TestClass]
    public class HybridCacheExtensionsTests
    {
#pragma warning disable CS8618
        private HybridCache _cache;
#pragma warning restore CS8618

        [TestInitialize]
        public void TestInitialize()
        {
            var services = new ServiceCollection();
            services.AddHybridCache();
            var serviceProvider = services.BuildServiceProvider();
            this._cache = serviceProvider.GetRequiredService<HybridCache>();
        }

        #region TryGetValueAsync Tests

        [TestMethod]
        public async Task TryGetValueAsync_WhenKeyExists_ShouldReturnTrueAndValueAsString()
        {
            // Arrange
            string key = "test-key";
            var expectedValue = "test-value";

            await this._cache.SetAsync(key, expectedValue);

            // Act
            var (exists, value) = await this._cache.TryGetValueAsync<string>(key);

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

            await this._cache.SetAsync(key, expectedValue);

            // Act
            var (exists, value) = await this._cache.TryGetValueAsync<int>(key);

            // Assert
            Assert.IsTrue(exists);
            Assert.AreEqual(expectedValue, value);
        }

        [TestMethod]
        public async Task TryGetValueAsync_WhenKeyExistsButValueIsNull_ShouldReturnTrueAndNullValue()
        {
            // Arrange
            string key = "test-key";
            string? nullValue = null;

            await this._cache.SetAsync(key, nullValue);

            // Act
            var (exists, value) = await this._cache.TryGetValueAsync<string>(key);

            // Assert
            Assert.IsTrue(exists);
            Assert.IsNull(value);
        }

        [TestMethod]
        public async Task TryGetValueAsync_WhenKeyDoesNotExist_ShouldReturnFalseAndNull()
        {
            // Arrange
            string key = "non-existent-key";

            // Act
            var (exists, value) = await this._cache.TryGetValueAsync<string>(key);

            // Assert
            Assert.IsFalse(exists);
            Assert.IsNull(value);
        }

        #endregion
    }
}
