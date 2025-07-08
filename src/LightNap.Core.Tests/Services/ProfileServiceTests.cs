using LightNap.Core.Api;
using LightNap.Core.Configuration;
using LightNap.Core.Data;
using LightNap.Core.Data.Entities;
using LightNap.Core.Email.Interfaces;
using LightNap.Core.Extensions;
using LightNap.Core.Identity.Dto.Request;
using LightNap.Core.Identity.Services;
using LightNap.Core.Interfaces;
using LightNap.Core.Notifications.Dto.Request;
using LightNap.Core.Notifications.Interfaces;
using LightNap.Core.Profile.Dto.Request;
using LightNap.Core.Profile.Dto.Response;
using LightNap.Core.Profile.Services;
using LightNap.Core.Tests.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace LightNap.Core.Tests.Services
{
    [TestClass]
    public class ProfileServiceTests
    {
        const string _userId = "test-user-id";
        const string _userEmail = "user@test.com";
        const string _userName = "UserName";

        // These will be initialized during TestInitialize.
#pragma warning disable CS8618
        private UserManager<ApplicationUser> _userManager;
        private ApplicationDbContext _dbContext;
        private IUserContext _userContext;
        private ProfileService _profileService;
        private IServiceProvider _serviceProvider;
        private Mock<IEmailService> _emailServiceMock;
#pragma warning restore CS8618

        [TestInitialize]
        public async Task TestInitialize()
        {
            var services = new ServiceCollection();
            services.AddLogging()
                .AddLightNapInMemoryDatabase()
                .AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            this._serviceProvider = services.BuildServiceProvider();
            this._dbContext = this._serviceProvider.GetRequiredService<ApplicationDbContext>();
            var logger = this._serviceProvider.GetRequiredService<ILogger<ProfileService>>();

            this._userManager = this._serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            await TestHelper.CreateTestUserAsync(this._userManager, _userId, _userName, _userEmail);

            this._userContext = new TestUserContext()
            {
                UserId = _userId
            };

            var userContextMock = new Mock<IUserContext>();
            userContextMock.Setup(uc => uc.GetUserId()).Returns(_userId);
            this._userContext = userContextMock.Object;

            this._emailServiceMock = new Mock<IEmailService>();

            this._profileService = new ProfileService(logger, this._dbContext, this._userManager, this._userContext, this._emailServiceMock.Object);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            this._dbContext.Database.EnsureDeleted();
            this._dbContext.Dispose();
        }

        [TestMethod]
        public async Task GetProfile_ShouldReturnUserProfile()
        {
            // Arrange
            var expectedProfile = new ProfileDto
            {
                Id = _userId,
                Email = _userEmail,
                UserName = _userName
            };

            // Act
            var profile = await this._profileService.GetProfileAsync();

            // Assert
            Assert.AreEqual(expectedProfile.Id, profile.Id);
            Assert.AreEqual(expectedProfile.Email, profile.Email);
            Assert.AreEqual(expectedProfile.UserName, profile.UserName);
        }

        [TestMethod]
        public async Task UpdateProfile_ShouldUpdateUserProfile()
        {
            // Arrange
            var updateProfileDto = new UpdateProfileDto
            {
                // Set properties to update
            };

            // Act
            await this._profileService.UpdateProfileAsync(updateProfileDto);
        }

        [TestMethod]
        public async Task ChangePassword_ShouldChangeUserPassword()
        {
            // Arrange
            var changePasswordDto = new ChangePasswordRequestDto
            {
                CurrentPassword = "OldPassword123!",
                NewPassword = "NewPassword123!",
                ConfirmNewPassword = "NewPassword123!"
            };

            var user = await this._userManager.FindByIdAsync(_userId);
            var identityResult = await this._userManager.AddPasswordAsync(user!, changePasswordDto.CurrentPassword);
            if (!identityResult.Succeeded) { Assert.Fail("Failed to add password to user."); }

            // Act
            await this._profileService.ChangePasswordAsync(changePasswordDto);

            // Assert
            var tokenServiceMock = new Mock<ITokenService>();
            tokenServiceMock.Setup(ts => ts.GenerateRefreshToken()).Returns("refresh-token");
            tokenServiceMock.Setup(ts => ts.GenerateAccessTokenAsync(It.IsAny<ApplicationUser>())).ReturnsAsync("access-token");
            var emailServiceMock = new Mock<IEmailService>();
            var notificationServiceMock = new Mock<INotificationService>();
            notificationServiceMock.Setup(ns => ns.CreateSystemNotificationForRoleAsync(ApplicationRoles.Administrator.Name!, It.IsAny<CreateNotificationDto>()));
            var signInManager = this._serviceProvider.GetRequiredService<SignInManager<ApplicationUser>>();
            var logger = this._serviceProvider.GetRequiredService<ILogger<IdentityService>>();
            var applicationSettings = Options.Create(
                new ApplicationSettings
                {
                    AutomaticallyApplyEfMigrations = false,
                    LogOutInactiveDeviceDays = 30,
                    RequireTwoFactorForNewUsers = false,
                    SiteUrlRootForEmails = "https://example.com/",
                    UseSameSiteStrictCookies = true
                });
            var cookieManagerMock = new Mock<ICookieManager>();

            var identityService = new IdentityService(
                logger,
                this._userManager,
                signInManager,
                tokenServiceMock.Object,
                emailServiceMock.Object,
                notificationServiceMock.Object,
                applicationSettings,
                this._dbContext,
                cookieManagerMock.Object,
                this._userContext
            );

            var loginResult = await identityService.LogInAsync(new LoginRequestDto
            {
                Login = _userEmail,
                Password = changePasswordDto.NewPassword,
                DeviceDetails = "device-details",
                RememberMe = false
            });

            Assert.IsNotNull(loginResult.AccessToken);
        }

        [TestMethod]
        [ExpectedException(typeof(UserFriendlyApiException))]
        public async Task ChangePassword_ShouldFailWithWrongCurrentPassword()
        {
            // Arrange
            var changePasswordDto = new ChangePasswordRequestDto
            {
                CurrentPassword = "WrongPassword123!",
                NewPassword = "NewPassword123!",
                ConfirmNewPassword = "NewPassword123!"
            };

            var user = await this._userManager.FindByIdAsync(_userId);
            var identityResult = await this._userManager.AddPasswordAsync(user!, "DifferentP@ssw0rd");
            if (!identityResult.Succeeded) { Assert.Fail("Failed to add password to user."); }

            // Act
            await this._profileService.ChangePasswordAsync(changePasswordDto);
        }

        [TestMethod]
        [ExpectedException(typeof(UserFriendlyApiException))]
        public async Task ChangePassword_ShouldFailWithWrongMistmatchedNewPassword()
        {
            // Arrange
            var changePasswordDto = new ChangePasswordRequestDto
            {
                CurrentPassword = "OldPassword123!",
                NewPassword = "NewPassword123!",
                ConfirmNewPassword = "NotNewPassword123!"
            };

            var user = await this._userManager.FindByIdAsync(_userId);
            var identityResult = await this._userManager.AddPasswordAsync(user!, "OldPassword123!");
            if (!identityResult.Succeeded) { Assert.Fail("Failed to add password to user."); }

            // Act
            await this._profileService.ChangePasswordAsync(changePasswordDto);
        }

        [TestMethod]
        public async Task ChangeEmail_ShouldStartEmailChangeProcess()
        {
            // Arrange
            var changeEmailDto = new ChangeEmailRequestDto
            {
                NewEmail = "newuser@test.com"
            };

            this._emailServiceMock
                .Setup(ts => ts.SendChangeEmailAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Act
            await this._profileService.ChangeEmailAsync(changeEmailDto);

            // Assert
            this._emailServiceMock.Verify(
               ts => ts.SendChangeEmailAsync(It.IsAny<ApplicationUser>(), changeEmailDto.NewEmail, It.IsAny<string>()),
               Times.Once);
        }

        [TestMethod]
        public async Task ConfirmEmailChange_ShouldConfirmEmailChange()
        {
            // Arrange
            var changeEmailDto = new ChangeEmailRequestDto
            {
                NewEmail = "newuser@test.com"
            };

            string emailChangeToken = string.Empty;
            this._emailServiceMock
                .Setup(ts => ts.SendChangeEmailAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), It.IsAny<string>()))
                .Callback<ApplicationUser, string, string>((user, newEmail, token) => emailChangeToken = token)
                .Returns(Task.CompletedTask);
            await this._profileService.ChangeEmailAsync(changeEmailDto);

            var confirmEmailChangeDto = new ConfirmEmailChangeRequestDto
            {
                NewEmail = "newuser@test.com",
                Code = emailChangeToken
            };

            // Act
            await this._profileService.ConfirmEmailChangeAsync(confirmEmailChangeDto);

            // Assert
            var updatedUser = await this._userManager.FindByIdAsync(_userId);
            Assert.AreEqual(confirmEmailChangeDto.NewEmail, updatedUser!.Email);
        }

        [TestMethod]
        public async Task GetSettings_ShouldReturnUserSettings()
        {
            // Arrange
            BrowserSettingsDto browserSettings = new();

            // Act
            var settings = await this._profileService.GetSettingsAsync();

            // Assert
            Assert.AreEqual(browserSettings.Version, settings.Version);
        }

        [TestMethod]
        public async Task UpdateSettings_ShouldUpdateUserSettings()
        {
            // Arrange
            var updateSettingsDto = new BrowserSettingsDto
            {
                Version = 2,
                Style = [],
                Preferences = [],
                Features = [],
                Extended = []
            };

            // Act
            await this._profileService.UpdateSettingsAsync(updateSettingsDto);
        }

        [TestMethod]
        public async Task GetDevices_ShouldReturnUserDevices()
        {
            // Arrange
            // Note the LastSeen timestamp is descending to match the descending order expected from the API.
            var expectedDevices = new List<DeviceDto>
            {
                new() { Id = "device1", LastSeen = new DateTime(2024, 12, 7), IpAddress = "192.168.1.1", Details = "Device 1" },
                new() { Id = "device2", LastSeen = new DateTime(2024, 12, 6), IpAddress = "192.168.1.2", Details = "Device 2" }
            };

            this._dbContext.RefreshTokens.AddRange(expectedDevices.Select(d => new RefreshToken
            {
                Id = d.Id,
                Token = "token",
                LastSeen = d.LastSeen,
                IpAddress = d.IpAddress,
                Expires = DateTime.UtcNow.AddDays(1),
                IsRevoked = false,
                Details = d.Details,
                UserId = _userId
            }));
            await this._dbContext.SaveChangesAsync();

            // Act
            var result = await this._profileService.GetDevicesAsync();

            // Assert
            Assert.AreEqual(expectedDevices.Count, result.Count);
            expectedDevices.Reverse();
            for (int i = 0; i < expectedDevices.Count; i++)
            {
                Assert.AreEqual(expectedDevices[i].Id, result[i].Id);
                Assert.AreEqual(expectedDevices[i].IpAddress, result[i].IpAddress);
                Assert.AreEqual(expectedDevices[i].Details, result[i].Details);
            }
        }

        [TestMethod]
        public async Task RevokeDevice_ShouldRevokeUserDevice()
        {
            // Arrange
            var deviceId = "device1";
            var refreshToken = new RefreshToken
            {
                Id = deviceId,
                Token = "token",
                LastSeen = DateTime.UtcNow,
                IpAddress = "192.168.1.1",
                Expires = DateTime.UtcNow.AddDays(1),
                IsRevoked = false,
                Details = "Device 1",
                UserId = _userId
            };
            this._dbContext.RefreshTokens.Add(refreshToken);
            await this._dbContext.SaveChangesAsync();

            // Act
            await this._profileService.RevokeDeviceAsync(deviceId);

            // Assert
            var revokedToken = await this._dbContext.RefreshTokens.FindAsync(deviceId);
            Assert.IsNotNull(revokedToken);
            Assert.IsTrue(revokedToken.IsRevoked);
        }

        [TestMethod]
        [ExpectedException(typeof(UserFriendlyApiException))]
        public async Task RevokeDevice_ShouldNotAllowRevokingOtherUsersDevice()
        {
            // Arrange
            var otherUserId = "otherUserId";
            var deviceId = "device1";
            var refreshToken = new RefreshToken
            {
                Id = deviceId,
                Token = "token",
                LastSeen = DateTime.UtcNow,
                IpAddress = "192.168.1.1",
                Expires = DateTime.UtcNow.AddDays(1),
                IsRevoked = false,
                Details = "Device 1",
                UserId = otherUserId
            };
            this._dbContext.RefreshTokens.Add(refreshToken);
            await this._dbContext.SaveChangesAsync();

            // Act
            await this._profileService.RevokeDeviceAsync(deviceId);
        }

    }
}
