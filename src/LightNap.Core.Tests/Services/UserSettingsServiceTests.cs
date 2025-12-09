using LightNap.Core.Api;
using LightNap.Core.Data;
using LightNap.Core.Data.Entities;
using LightNap.Core.Extensions;
using LightNap.Core.Tests.Utilities;
using LightNap.Core.UserSettings.Dto.Request;
using LightNap.Core.UserSettings.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace LightNap.Core.Tests.Services
{
    [TestClass]
    public class UserSettingsServiceTests
    {
        private const string _testUserId = "test-user-id";
        private const string _otherUserId = "other-user-id";
        private const string _testSettingKey = "BrowserSettings";

        // These will be initialized during TestInitialize.
#pragma warning disable CS8618
        private UserManager<ApplicationUser> _userManager;
        private ApplicationDbContext _dbContext;
        private TestUserContext _userContext;
        private UserSettingsService _userSettingsService;
        private ILogger<UserSettingsService> _logger;
#pragma warning restore CS8618

        [TestInitialize]
        public async Task TestInitialize()
        {
            var services = new ServiceCollection();
            services.AddLogging()
                .AddLightNapInMemoryDatabase($"TestDb_{Guid.NewGuid()}")
                .AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            var serviceProvider = services.BuildServiceProvider();
            this._dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();
            this._userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            this._logger = serviceProvider.GetRequiredService<ILogger<UserSettingsService>>();

            await TestHelper.CreateTestUserAsync(this._userManager, _testUserId);
            await TestHelper.CreateTestUserAsync(this._userManager, _otherUserId);

            this._userContext = new TestUserContext();
            this._userSettingsService = new UserSettingsService(this._dbContext, this._userContext, this._logger);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            this._dbContext.Database.EnsureDeleted();
            this._dbContext.Dispose();
        }

        #region GetUserSettingAsync Tests

        [TestMethod]
        public async Task GetUserSettingAsync_AdminUser_ReturnsExistingSetting()
        {
            // Arrange
            this._userContext.LogInAdministrator();
            var expectedValue = new { theme = "dark" };
            var setting = new UserSetting
            {
                UserId = _testUserId,
                Key = _testSettingKey,
                Value = JsonSerializer.Serialize(expectedValue),
                CreatedDate = DateTime.UtcNow,
                LastModifiedDate = DateTime.UtcNow
            };
            this._dbContext.UserSettings.Add(setting);
            await this._dbContext.SaveChangesAsync();

            // Act
            var result = await this._userSettingsService.GetUserSettingAsync<object>(_testUserId, _testSettingKey);

            // Assert
            Assert.IsNotNull(result);
            var jsonResult = JsonSerializer.Serialize(result);
            var jsonExpected = JsonSerializer.Serialize(expectedValue);
            Assert.AreEqual(jsonExpected, jsonResult);
        }

        [TestMethod]
        public async Task GetUserSettingAsync_SettingDoesNotExist_ReturnsDefaultValue()
        {
            // Arrange
            this._userContext.LogInAdministrator();

            // Act
            var result = await this._userSettingsService.GetUserSettingAsync<string>(_testUserId, _testSettingKey);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task GetUserSettingAsync_NonAdminUser_ThrowsException()
        {
            // Arrange
            this._userContext.LogIn(_testUserId);

            // Act & Assert
            await Assert.ThrowsExactlyAsync<UserFriendlyApiException>(async () =>
            {
                await this._userSettingsService.GetUserSettingAsync<string>(_testUserId, _testSettingKey);
            });
        }

        [TestMethod]
        public async Task GetUserSettingAsync_UserLoggedOut_ThrowsException()
        {
            // Arrange
            this._userContext.LogOut();

            // Act & Assert
            await Assert.ThrowsExactlyAsync<UserFriendlyApiException>(async () =>
            {
                await this._userSettingsService.GetUserSettingAsync<string>(_testUserId, _testSettingKey);
            });
        }

        [TestMethod]
        public async Task GetUserSettingAsync_InvalidDeserialization_ThrowsException()
        {
            // Arrange
            this._userContext.LogInAdministrator();
            var setting = new UserSetting
            {
                UserId = _testUserId,
                Key = _testSettingKey,
                Value = "invalid-json",
                CreatedDate = DateTime.UtcNow,
                LastModifiedDate = DateTime.UtcNow
            };
            this._dbContext.UserSettings.Add(setting);
            await this._dbContext.SaveChangesAsync();

            // Act & Assert
            await Assert.ThrowsExactlyAsync<JsonException>(async () =>
            {
                await this._userSettingsService.GetUserSettingAsync<int>(_testUserId, _testSettingKey);
            });
        }

        #endregion

        #region GetUserSettingsAsync Tests

        [TestMethod]
        public async Task GetUserSettingsAsync_AdminUser_ReturnsAllSettings()
        {
            // Arrange
            this._userContext.LogInAdministrator();
            var setting = new UserSetting
            {
                UserId = _testUserId,
                Key = _testSettingKey,
                Value = "test-value",
                CreatedDate = DateTime.UtcNow,
                LastModifiedDate = DateTime.UtcNow
            };
            this._dbContext.UserSettings.Add(setting);
            await this._dbContext.SaveChangesAsync();

            // Act
            var result = await this._userSettingsService.GetUserSettingsAsync(_testUserId);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotEmpty(result);
            Assert.IsTrue(result.Any(s => s.Key == _testSettingKey));
        }

        [TestMethod]
        public async Task GetUserSettingsAsync_UserWithNoSettings_ReturnsDefaults()
        {
            // Arrange
            this._userContext.LogInAdministrator();

            // Act
            var result = await this._userSettingsService.GetUserSettingsAsync(_testUserId);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotEmpty(result);
            // Should contain default settings
        }

        [TestMethod]
        public async Task GetUserSettingsAsync_NonAdminUser_ThrowsException()
        {
            // Arrange
            this._userContext.LogIn(_testUserId);

            // Act & Assert
            await Assert.ThrowsExactlyAsync<UserFriendlyApiException>(async () =>
            {
                await this._userSettingsService.GetUserSettingsAsync(_testUserId);
            });
        }

        #endregion

        #region GetMySettingsAsync Tests

        [TestMethod]
        public async Task GetMySettingsAsync_AuthenticatedUser_ReturnsUserSettings()
        {
            // Arrange
            this._userContext.LogIn(_testUserId);
            var setting = new UserSetting
            {
                UserId = _testUserId,
                Key = _testSettingKey,
                Value = "my-value",
                CreatedDate = DateTime.UtcNow,
                LastModifiedDate = DateTime.UtcNow
            };
            this._dbContext.UserSettings.Add(setting);
            await this._dbContext.SaveChangesAsync();

            // Act
            var result = await this._userSettingsService.GetMySettingsAsync();

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Any(s => s.Key == _testSettingKey && s.Value == "my-value"));
        }

        [TestMethod]
        public async Task GetMySettingsAsync_UserLoggedOut_ThrowsException()
        {
            // Arrange
            this._userContext.LogOut();

            // Act & Assert
            await Assert.ThrowsExactlyAsync<UserFriendlyApiException>(async () =>
            {
                await this._userSettingsService.GetMySettingsAsync();
            });
        }

        [TestMethod]
        public async Task GetMySettingsAsync_OnlyReturnsUserReadableSettings()
        {
            // Arrange
            this._userContext.LogIn(_testUserId);

            // Act
            var result = await this._userSettingsService.GetMySettingsAsync();

            // Assert
            Assert.IsNotNull(result);
            // All returned settings should be user-readable
            Assert.IsNotEmpty(result);
        }

        #endregion

        #region SetUserSettingAsync Tests

        [TestMethod]
        public async Task SetUserSettingAsync_AdminUser_CreatesNewSetting()
        {
            // Arrange
            this._userContext.LogInAdministrator();
            var dto = new SetUserSettingRequestDto(_testSettingKey, "new-value");

            // Act
            var result = await this._userSettingsService.SetUserSettingAsync(_testUserId, dto);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(_testSettingKey, result.Key);
            Assert.AreEqual("new-value", result.Value);
            Assert.IsNotNull(result.CreatedDate);
            Assert.IsNotNull(result.LastModifiedDate);

            var settingInDb = await this._dbContext.UserSettings
                .FirstOrDefaultAsync(s => s.UserId == _testUserId && s.Key == _testSettingKey);
            Assert.IsNotNull(settingInDb);
            Assert.AreEqual("new-value", settingInDb.Value);
        }

        [TestMethod]
        public async Task SetUserSettingAsync_AdminUser_UpdatesExistingSetting()
        {
            // Arrange
            this._userContext.LogInAdministrator();
            DateTime originalTimestamp = DateTime.UtcNow.AddDays(-1);
            var originalSetting = new UserSetting
            {
                UserId = _testUserId,
                Key = _testSettingKey,
                Value = "old-value",
                CreatedDate = originalTimestamp,
                LastModifiedDate = originalTimestamp,
            };
            this._dbContext.UserSettings.Add(originalSetting);
            await this._dbContext.SaveChangesAsync();

            var dto = new SetUserSettingRequestDto(_testSettingKey, "updated-value");

            // Act
            var result = await this._userSettingsService.SetUserSettingAsync(_testUserId, dto);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(_testSettingKey, result.Key);
            Assert.AreEqual("updated-value", result.Value);

            var settingInDb = await this._dbContext.UserSettings
                .FirstOrDefaultAsync(s => s.UserId == _testUserId && s.Key == _testSettingKey);
            Assert.IsNotNull(settingInDb);
            Assert.AreEqual("updated-value", settingInDb.Value);
            Assert.IsTrue(settingInDb.LastModifiedDate > originalTimestamp);
        }

        [TestMethod]
        public async Task SetUserSettingAsync_NonAdminUser_ThrowsException()
        {
            // Arrange
            this._userContext.LogIn(_testUserId);
            var dto = new SetUserSettingRequestDto(_testSettingKey, "new-value");

            // Act & Assert
            await Assert.ThrowsExactlyAsync<UserFriendlyApiException>(async () =>
            {
                await this._userSettingsService.SetUserSettingAsync(_testUserId, dto);
            });
        }

        #endregion

        #region SetMySettingAsync Tests

        [TestMethod]
        public async Task SetMySettingAsync_AuthenticatedUser_CreatesSetting()
        {
            // Arrange
            this._userContext.LogIn(_testUserId);
            var dto = new SetUserSettingRequestDto(_testSettingKey, "my-new-value");

            // Act
            var result = await this._userSettingsService.SetMySettingAsync(dto);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(_testSettingKey, result.Key);
            Assert.AreEqual("my-new-value", result.Value);

            var settingInDb = await this._dbContext.UserSettings
                .FirstOrDefaultAsync(s => s.UserId == _testUserId && s.Key == _testSettingKey);
            Assert.IsNotNull(settingInDb);
            Assert.AreEqual("my-new-value", settingInDb.Value);
        }

        [TestMethod]
        public async Task SetMySettingAsync_AuthenticatedUser_UpdatesExistingSetting()
        {
            // Arrange
            this._userContext.LogIn(_testUserId);
            var originalSetting = new UserSetting
            {
                UserId = _testUserId,
                Key = _testSettingKey,
                Value = "old-value",
                CreatedDate = DateTime.UtcNow.AddDays(-1),
                LastModifiedDate = DateTime.UtcNow.AddDays(-1)
            };
            this._dbContext.UserSettings.Add(originalSetting);
            await this._dbContext.SaveChangesAsync();

            var dto = new SetUserSettingRequestDto(_testSettingKey, "my-updated-value");

            // Act
            var result = await this._userSettingsService.SetMySettingAsync(dto);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("my-updated-value", result.Value);
        }

        [TestMethod]
        public async Task SetMySettingAsync_UserLoggedOut_ThrowsException()
        {
            // Arrange
            this._userContext.LogOut();
            var dto = new SetUserSettingRequestDto(_testSettingKey, "value");

            // Act & Assert
            await Assert.ThrowsExactlyAsync<UserFriendlyApiException>(async () =>
            {
                await this._userSettingsService.SetMySettingAsync(dto);
            });
        }

        #endregion

        #region PurgeUnusedUserSettingsAsync Tests

        [TestMethod]
        public async Task PurgeUnusedUserSettingsAsync_AdminUser_RemovesUnusedSettings()
        {
            // Arrange
            this._userContext.LogInAdministrator();

            // Add a valid setting
            var validSetting = new UserSetting
            {
                UserId = _testUserId,
                Key = _testSettingKey,
                Value = "valid-value",
                CreatedDate = DateTime.UtcNow,
                LastModifiedDate = DateTime.UtcNow
            };
            this._dbContext.UserSettings.Add(validSetting);

            // Add an invalid/unused setting
            var unusedSetting = new UserSetting
            {
                UserId = _testUserId,
                Key = "NonExistentSettingKey",
                Value = "unused-value",
                CreatedDate = DateTime.UtcNow,
                LastModifiedDate = DateTime.UtcNow
            };
            this._dbContext.UserSettings.Add(unusedSetting);
            await this._dbContext.SaveChangesAsync();

            var countBefore = await this._dbContext.UserSettings.CountAsync();
            Assert.AreEqual(2, countBefore);

            // Act
            await this._userSettingsService.PurgeUnusedUserSettingsAsync();

            // Assert
            var countAfter = await this._dbContext.UserSettings.CountAsync();
            Assert.AreEqual(1, countAfter);

            var remainingSetting = await this._dbContext.UserSettings.FirstOrDefaultAsync();
            Assert.IsNotNull(remainingSetting);
            Assert.AreEqual(_testSettingKey, remainingSetting.Key);
        }

        [TestMethod]
        public async Task PurgeUnusedUserSettingsAsync_PurgesMultipleUsersSettings()
        {
            // Arrange
            this._userContext.LogInAdministrator();

            var unusedKey = "ObsoleteSettingKey";
            var settings = new List<UserSetting>
            {
                new() {
                    UserId = _testUserId,
                    Key = unusedKey,
                    Value = "value1",
                    CreatedDate = DateTime.UtcNow,
                    LastModifiedDate = DateTime.UtcNow
                },
                new() {
                    UserId = _otherUserId,
                    Key = unusedKey,
                    Value = "value2",
                    CreatedDate = DateTime.UtcNow,
                    LastModifiedDate = DateTime.UtcNow
                }
            };
            this._dbContext.UserSettings.AddRange(settings);
            await this._dbContext.SaveChangesAsync();

            // Act
            await this._userSettingsService.PurgeUnusedUserSettingsAsync();

            // Assert
            var remainingCount = await this._dbContext.UserSettings.CountAsync(s => s.Key == unusedKey);
            Assert.AreEqual(0, remainingCount);
        }

        [TestMethod]
        public async Task PurgeUnusedUserSettingsAsync_NonAdminUser_ThrowsException()
        {
            // Arrange
            this._userContext.LogIn(_testUserId);

            // Act & Assert
            await Assert.ThrowsExactlyAsync<UserFriendlyApiException>(async () =>
            {
                await this._userSettingsService.PurgeUnusedUserSettingsAsync();
            });
        }

        [TestMethod]
        public async Task PurgeUnusedUserSettingsAsync_NoUnusedSettings_DoesNothing()
        {
            // Arrange
            this._userContext.LogInAdministrator();

            var validSetting = new UserSetting
            {
                UserId = _testUserId,
                Key = _testSettingKey,
                Value = "valid-value",
                CreatedDate = DateTime.UtcNow,
                LastModifiedDate = DateTime.UtcNow
            };
            this._dbContext.UserSettings.Add(validSetting);
            await this._dbContext.SaveChangesAsync();

            var countBefore = await this._dbContext.UserSettings.CountAsync();

            // Act
            await this._userSettingsService.PurgeUnusedUserSettingsAsync();

            // Assert
            var countAfter = await this._dbContext.UserSettings.CountAsync();
            Assert.AreEqual(countBefore, countAfter);
        }

        #endregion
    }
}
