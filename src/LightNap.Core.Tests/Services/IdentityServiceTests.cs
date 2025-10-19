using LightNap.Core.Api;
using LightNap.Core.Configuration;
using LightNap.Core.Data;
using LightNap.Core.Data.Entities;
using LightNap.Core.Email.Interfaces;
using LightNap.Core.Extensions;
using LightNap.Core.Identity.Dto.Request;
using LightNap.Core.Identity.Dto.Response;
using LightNap.Core.Identity.Interfaces;
using LightNap.Core.Identity.Models;
using LightNap.Core.Identity.Services;
using LightNap.Core.Notifications.Dto.Request;
using LightNap.Core.Notifications.Interfaces;
using LightNap.Core.Tests.Utilities;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace LightNap.Core.Tests.Services
{
    [TestClass]
    public class IdentityServiceTests
    {
        // Hardcoded in Core library. Not great since name might change, but good enough for now.
        private const string _refreshTokenCookieName = "refreshToken";

        const string _userId = "test-user-id";
        const string _userEmail = "user@test.com";
        const string _userName = "UserName";

        // These will be initialized during TestInitialize.
#pragma warning disable CS8618
        private UserManager<ApplicationUser> _userManager;
        private IServiceProvider _serviceProvider;
        private ApplicationDbContext _dbContext;
        private IdentityService _identityService;
        private TestCookieManager _cookieManager;
        private Mock<IEmailService> _emailServiceMock;
        private Mock<ITokenService> _tokenServiceMock;
        private Mock<INotificationService> _notificationServiceMock;
#pragma warning restore CS8618

        [TestInitialize]
        public void TestInitialize()
        {
            var services = new ServiceCollection();
            services.AddLogging()
                .AddLightNapInMemoryDatabase()
                .AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // Use EphemeralDataProtectionProvider for testing things like generating a password reset token.
            services.AddSingleton<IDataProtectionProvider, EphemeralDataProtectionProvider>();

            this._serviceProvider = services.BuildServiceProvider();
            this._dbContext = this._serviceProvider.GetRequiredService<ApplicationDbContext>();
            this._userManager = this._serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var signInManager = this._serviceProvider.GetRequiredService<SignInManager<ApplicationUser>>();
            var logger = this._serviceProvider.GetRequiredService<ILogger<IdentityService>>();

            this._tokenServiceMock = new Mock<ITokenService>();
            this._tokenServiceMock.Setup(ts => ts.GenerateRefreshToken()).Returns("refresh-token");
            this._tokenServiceMock.Setup(ts => ts.GenerateAccessTokenAsync(It.IsAny<ApplicationUser>())).ReturnsAsync("access-token");

            this._emailServiceMock = new Mock<IEmailService>();

            this._notificationServiceMock = new Mock<INotificationService>();
            this._notificationServiceMock.Setup(ns => ns.CreateSystemNotificationForUserAsync(ApplicationRoles.Administrator.Name!, It.IsAny<CreateNotificationRequestDto>()));

            var applicationSettings = Options.Create(
                new ApplicationSettings
                {
                    AutomaticallyApplyEfMigrations = false,
                    LogOutInactiveDeviceDays = 30,
                    RequireTwoFactorForNewUsers = false,
                    SiteUrlRootForEmails = "https://example.com/",
                    UseSameSiteStrictCookies = true
                });

            this._cookieManager = new TestCookieManager();

            TestUserContext userContext = new()
            {
                UserId = _userId,
                IpAddress = "127.0.0.1"
            };

            this._identityService = new IdentityService(logger, this._userManager, signInManager, this._tokenServiceMock.Object, this._emailServiceMock.Object,
                this._notificationServiceMock.Object, applicationSettings, this._dbContext, this._cookieManager, userContext);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            this._dbContext.Database.EnsureDeleted();
            this._dbContext.Dispose();
        }

        [TestMethod]
        public async Task LogInAsync_ValidCredentials_ReturnsSuccess()
        {
            // Arrange
            var requestDto = new LoginRequestDto
            {
                Login = "test@test.com",
                Password = "ValidPassword123!",
                RememberMe = true,
                DeviceDetails = "TestDevice",
            };

            var user = await TestHelper.CreateTestUserAsync(this._userManager, "user-id", "UserName", requestDto.Login);
            await this._userManager.AddPasswordAsync(user, requestDto.Password);

            // Act
            var result = await this._identityService.LogInAsync(requestDto);

            // Assert
            Assert.AreEqual(LoginSuccessType.AccessToken, result.Type);
            Assert.IsNotNull(result.AccessToken);

            var cookie = this._cookieManager.GetCookie(_refreshTokenCookieName);
            Assert.IsNotNull(cookie);
        }

        [TestMethod]
        public async Task LogInAsync_BadEmail_ThrowsError()
        {
            // Arrange
            var requestDto = new LoginRequestDto
            {
                Login = "test@test.com",
                Password = "ValidPassword123!",
                RememberMe = true,
                DeviceDetails = "TestDevice",
            };

            // Act & Assert
            await Assert.ThrowsExactlyAsync<UserFriendlyApiException>(async () => await this._identityService.LogInAsync(requestDto));
        }

        [TestMethod]
        public async Task LogInAsync_BadPassword_ThrowsError()
        {
            // Arrange
            var requestDto = new LoginRequestDto
            {
                Login = "test@test.com",
                Password = "BadPassword123!",
                RememberMe = true,
                DeviceDetails = "TestDevice",
            };

            var user = await TestHelper.CreateTestUserAsync(this._userManager, "user-id", "UserName", requestDto.Login);
            await this._userManager.AddPasswordAsync(user, "GoodPassword123!");

            // Act & Assert
            await Assert.ThrowsExactlyAsync<UserFriendlyApiException>(async () => await this._identityService.LogInAsync(requestDto));
        }

        [TestMethod]
        public async Task GetAccessTokenAsync_LoggedIn_ReturnsSuccess()
        {
            // Arrange
            var requestDto = new LoginRequestDto
            {
                Login = "test@test.com",
                Password = "ValidPassword123!",
                RememberMe = true,
                DeviceDetails = "TestDevice",
            };
            var user = await TestHelper.CreateTestUserAsync(this._userManager, "user-id", "UserName", requestDto.Login);
            await this._userManager.AddPasswordAsync(user, requestDto.Password);
            var loginResult = await this._identityService.LogInAsync(requestDto);
            var newToken = "new-token";
            Assert.AreNotEqual(newToken, loginResult.AccessToken);
            this._tokenServiceMock.Setup(ts => ts.GenerateAccessTokenAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(newToken);

            // Act
            var accessTokenResult = await this._identityService.GetAccessTokenAsync();

            // Assert
            Assert.AreEqual(newToken, accessTokenResult);
        }

        [TestMethod]
        public async Task GetAccessTokenAsync_NotLoggedIn_ReturnsEmpty()
        {
            // Arrange

            // Act & Assert
            var token = await this._identityService.GetAccessTokenAsync();
            Assert.AreEqual(string.Empty, token);
        }

        [TestMethod]
        public async Task RegisterAsync_ValidData_ReturnsSuccess()
        {
            // Arrange
            var requestDto = new RegisterRequestDto
            {
                Email = "newuser@test.com",
                Password = "NewUserPassword123!",
                UserName = "NewUser",
                ConfirmPassword = "NewUserPassword123!",
                RememberMe = true,
                DeviceDetails = "TestDevice"
            };

            this._emailServiceMock.Setup(ts => ts.SendRegistrationWelcomeAsync(It.IsAny<ApplicationUser>())).Returns(Task.CompletedTask);

            // Act
            var registeredUser = await this._identityService.RegisterAsync(requestDto);

            // Assert
            Assert.AreEqual(LoginSuccessType.AccessToken, registeredUser.Type);
            Assert.IsNotNull(registeredUser.AccessToken);

            var user = await this._userManager.FindByEmailAsync(requestDto.Email);
            Assert.IsNotNull(user);

            var cookie = this._cookieManager.GetCookie(_refreshTokenCookieName);
            Assert.IsNotNull(cookie);

            this._emailServiceMock.Verify(ts => ts.SendRegistrationWelcomeAsync(It.IsAny<ApplicationUser>()), Times.Once);
        }

        [TestMethod]
        public async Task LogOutAsync_UserLoggedIn_ReturnsSuccess()
        {
            // Arrange
            var requestDto = new RegisterRequestDto
            {
                Email = "newuser@test.com",
                Password = "NewUserPassword123!",
                UserName = "NewUser",
                ConfirmPassword = "NewUserPassword123!",
                RememberMe = true,
                DeviceDetails = "TestDevice"
            };
            await this._identityService.RegisterAsync(requestDto);
            var cookie = this._cookieManager.GetCookie(_refreshTokenCookieName);
            Assert.IsNotNull(cookie);

            // Act
            await this._identityService.LogOutAsync();

            // Assert
            cookie = this._cookieManager.GetCookie("refreshCookie");
            Assert.IsNull(cookie);
        }

        [TestMethod]
        public async Task ResetPasswordAsync_ValidData_ReturnsSuccess()
        {
            // Arrange
            var requestDto = new ResetPasswordRequestDto
            {
                Email = "test@test.com"
            };

            var user = await TestHelper.CreateTestUserAsync(this._userManager, "user-id", "UserName", requestDto.Email);
            await this._userManager.AddPasswordAsync(user, "OldPassword123!");

            string capturedPasswordResetUrl = string.Empty;
            this._emailServiceMock
                .Setup(ts => ts.SendPasswordResetAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .Callback<ApplicationUser, string>((user, url) => capturedPasswordResetUrl = url)
                .Returns(Task.CompletedTask);

            // Act
            await this._identityService.ResetPasswordAsync(requestDto);

            // Assert
            this._emailServiceMock.Verify(ts => ts.SendPasswordResetAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Once);
            Assert.IsFalse(string.IsNullOrEmpty(capturedPasswordResetUrl), "Password reset URL should be captured.");
        }

        [TestMethod]
        public async Task NewPasswordAsync_ValidData_ReturnsSuccess()
        {
            // Arrange
            var passwordResetRequestDto = new ResetPasswordRequestDto
            {
                Email = "test@test.com"
            };

            var user = await TestHelper.CreateTestUserAsync(this._userManager, "user-id", "UserName", passwordResetRequestDto.Email);
            await this._userManager.AddPasswordAsync(user, "OldPassword123!");

            string passwordResetToken = string.Empty;
            this._emailServiceMock
                .Setup(ts => ts.SendPasswordResetAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .Callback<ApplicationUser, string>((user, token) => passwordResetToken = token)
                .Returns(Task.CompletedTask);

            await this._identityService.ResetPasswordAsync(passwordResetRequestDto);

            var newPasswordRequestDto = new NewPasswordRequestDto
            {
                Email = passwordResetRequestDto.Email,
                DeviceDetails = "TestDevice",
                Password = "NewPassword123!",
                RememberMe = true,
                Token = passwordResetToken
            };

            // Act
            await this._identityService.NewPasswordAsync(newPasswordRequestDto);
        }

        [TestMethod]
        public async Task NewPasswordAsync_InvalidData_ThrowsError()
        {
            // Arrange
            var newPasswordRequestDto = new NewPasswordRequestDto
            {
                Email = "test@test.com",
                DeviceDetails = "TestDevice",
                Password = "NewPassword123!",
                RememberMe = true,
                Token = "bad-token"
            };

            // Act & Assert
            await Assert.ThrowsExactlyAsync<UserFriendlyApiException>(async () => await this._identityService.NewPasswordAsync(newPasswordRequestDto));
        }

        [TestMethod]
        public async Task LogInAsync_ValidEmailOnly_ReturnsSuccess()
        {
            // Arrange
            var requestDto = new LoginRequestDto
            {
                Login = "test@test.com",
                Password = "ValidPassword123!",
                RememberMe = true,
                DeviceDetails = "TestDevice",
                Type = LoginType.Email
            };

            var user = await TestHelper.CreateTestUserAsync(this._userManager, "user-id", "UserName", requestDto.Login);
            await this._userManager.AddPasswordAsync(user, requestDto.Password);

            // Act
            var result = await this._identityService.LogInAsync(requestDto);

            // Assert
            Assert.AreEqual(LoginSuccessType.AccessToken, result.Type);
            Assert.IsNotNull(result.AccessToken);

            var cookie = this._cookieManager.GetCookie(_refreshTokenCookieName);
            Assert.IsNotNull(cookie);
        }

        [TestMethod]
        public async Task LogInAsync_EmailLoginForUserNameType_ThrowsError()
        {
            // Arrange
            var requestDto = new LoginRequestDto
            {
                Login = "test@test.com",
                Password = "ValidPassword123!",
                RememberMe = true,
                DeviceDetails = "TestDevice",
                Type = LoginType.UserName
            };

            // Act & Assert
            await Assert.ThrowsExactlyAsync<UserFriendlyApiException>(async () => await this._identityService.LogInAsync(requestDto));
        }

        [TestMethod]
        public async Task LogInAsync_ValidUserNameOnly_ReturnsSuccess()
        {
            // Arrange
            var requestDto = new LoginRequestDto
            {
                Login = "UserName",
                Password = "ValidPassword123!",
                RememberMe = true,
                DeviceDetails = "TestDevice",
                Type = LoginType.UserName
            };

            var user = await TestHelper.CreateTestUserAsync(this._userManager, "user-id", requestDto.Login, "test@test.com");
            await this._userManager.AddPasswordAsync(user, requestDto.Password);

            // Act
            var result = await this._identityService.LogInAsync(requestDto);

            // Assert
            Assert.AreEqual(LoginSuccessType.AccessToken, result.Type);
            Assert.IsNotNull(result.AccessToken);

            var cookie = this._cookieManager.GetCookie(_refreshTokenCookieName);
            Assert.IsNotNull(cookie);
        }

        [TestMethod]
        public async Task LogInAsync_UserNameLoginForEmailType_ThrowsError()
        {
            // Arrange
            var requestDto = new LoginRequestDto
            {
                Login = "UserName",
                Password = "ValidPassword123!",
                RememberMe = true,
                DeviceDetails = "TestDevice",
                Type = LoginType.Email
            };

            // Act & Assert
            await Assert.ThrowsExactlyAsync<UserFriendlyApiException>(async () => await this._identityService.LogInAsync(requestDto));
        }

        [TestMethod]
        public async Task RequestMagicLinkEmailAsync_ValidEmail_SendsEmail()
        {
            // Arrange
            var requestDto = new SendMagicLinkRequestDto
            {
                Email = "test@test.com"
            };

            var user = await TestHelper.CreateTestUserAsync(this._userManager, "user-id", "UserName", requestDto.Email);
            string capturedMagicLinkUrl = string.Empty;
            this._emailServiceMock
                .Setup(ts => ts.SendMagicLinkAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .Callback<ApplicationUser, string>((user, url) => capturedMagicLinkUrl = url)
                .Returns(Task.CompletedTask);

            // Act
            await this._identityService.RequestMagicLinkEmailAsync(requestDto);

            // Assert
            this._emailServiceMock.Verify(ts => ts.SendMagicLinkAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Once);
            Assert.IsFalse(string.IsNullOrEmpty(capturedMagicLinkUrl), "Magic link URL should be captured.");
        }

        [TestMethod]
        public async Task RequestMagicLinkEmailAsync_InvalidEmail_ThrowsError()
        {
            // Arrange
            var requestDto = new SendMagicLinkRequestDto
            {
                Email = "invalid@test.com"
            };

            // Act & Assert
            await Assert.ThrowsExactlyAsync<UserFriendlyApiException>(async () => await this._identityService.RequestMagicLinkEmailAsync(requestDto));
        }

        [TestMethod]
        public async Task LogInFromMagicLinkEmailAsync_ValidEmail_Succeeds()
        {
            // Arrange
            var requestDto = new SendMagicLinkRequestDto
            {
                Email = "test@test.com"
            };

            var user = await TestHelper.CreateTestUserAsync(this._userManager, "user-id", "UserName", requestDto.Email);
            string magicLinkToken = string.Empty;
            this._emailServiceMock
                .Setup(ts => ts.SendMagicLinkAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .Callback<ApplicationUser, string>((user, token) => magicLinkToken = token)
                .Returns(Task.CompletedTask);
            await this._identityService.RequestMagicLinkEmailAsync(requestDto);

            var loginRequest = new LoginRequestDto
            {
                Type = LoginType.MagicLink,
                DeviceDetails = "",
                Login = requestDto.Email,
                Password = magicLinkToken
            };

            // Act
            var result = await this._identityService.LogInAsync(loginRequest);

            // Assert
            Assert.AreEqual(LoginSuccessType.AccessToken, result.Type);
            Assert.IsNotNull(result.AccessToken);

            var cookie = this._cookieManager.GetCookie(_refreshTokenCookieName);
            Assert.IsNotNull(cookie);

        }

        [TestMethod]
        public async Task ChangePassword_ShouldChangeUserPassword()
        {
            await TestHelper.CreateTestUserAsync(this._userManager, _userId, _userName, _userEmail);

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
            await this._identityService.ChangePasswordAsync(changePasswordDto);

            // Assert
            var loginResult = await this._identityService.LogInAsync(new LoginRequestDto
            {
                Login = _userEmail,
                Password = changePasswordDto.NewPassword,
                DeviceDetails = "device-details",
                RememberMe = false
            });

            Assert.IsNotNull(loginResult.AccessToken);
        }

        [TestMethod]
        public async Task ChangePassword_ShouldFailWithWrongCurrentPassword()
        {
            await TestHelper.CreateTestUserAsync(this._userManager, _userId, _userName, _userEmail);

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

            // Act & Assert
            await Assert.ThrowsExactlyAsync<UserFriendlyApiException>(async () => await this._identityService.ChangePasswordAsync(changePasswordDto));
        }

        [TestMethod]
        public async Task ChangePassword_ShouldFailWithWrongMistmatchedNewPassword()
        {
            // Arrange
            await TestHelper.CreateTestUserAsync(this._userManager, _userId, _userName, _userEmail);

            var changePasswordDto = new ChangePasswordRequestDto
            {
                CurrentPassword = "OldPassword123!",
                NewPassword = "NewPassword123!",
                ConfirmNewPassword = "NotNewPassword123!"
            };

            var user = await this._userManager.FindByIdAsync(_userId);
            var identityResult = await this._userManager.AddPasswordAsync(user!, "OldPassword123!");
            if (!identityResult.Succeeded) { Assert.Fail("Failed to add password to user."); }

            // Act & Assert
            await Assert.ThrowsExactlyAsync<UserFriendlyApiException>(async () => await this._identityService.ChangePasswordAsync(changePasswordDto));
        }

        [TestMethod]
        public async Task ChangeEmail_ShouldStartEmailChangeProcess()
        {
            // Arrange
            await TestHelper.CreateTestUserAsync(this._userManager, _userId, _userName, _userEmail);

            var changeEmailDto = new ChangeEmailRequestDto
            {
                NewEmail = "newuser@test.com"
            };

            this._emailServiceMock
                .Setup(ts => ts.SendChangeEmailAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Act
            await this._identityService.ChangeEmailAsync(changeEmailDto);

            // Assert
            this._emailServiceMock.Verify(
               ts => ts.SendChangeEmailAsync(It.IsAny<ApplicationUser>(), changeEmailDto.NewEmail, It.IsAny<string>()),
               Times.Once);
        }

        [TestMethod]
        public async Task ConfirmEmailChange_ShouldConfirmEmailChange()
        {
            // Arrange
            await TestHelper.CreateTestUserAsync(this._userManager, _userId, _userName, _userEmail);

            var changeEmailDto = new ChangeEmailRequestDto
            {
                NewEmail = "newuser@test.com"
            };

            string emailChangeToken = string.Empty;
            this._emailServiceMock
                .Setup(ts => ts.SendChangeEmailAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), It.IsAny<string>()))
                .Callback<ApplicationUser, string, string>((user, newEmail, token) => emailChangeToken = token)
                .Returns(Task.CompletedTask);
            await this._identityService.ChangeEmailAsync(changeEmailDto);

            var confirmEmailChangeDto = new ConfirmEmailChangeRequestDto
            {
                NewEmail = "newuser@test.com",
                Code = emailChangeToken
            };

            // Act
            await this._identityService.ConfirmEmailChangeAsync(confirmEmailChangeDto);

            // Assert
            var updatedUser = await this._userManager.FindByIdAsync(_userId);
            Assert.AreEqual(confirmEmailChangeDto.NewEmail, updatedUser!.Email);
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
            var result = await this._identityService.GetDevicesAsync();

            // Assert
            Assert.HasCount(expectedDevices.Count, result);
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
            await this._identityService.RevokeDeviceAsync(deviceId);

            // Assert
            var revokedToken = await this._dbContext.RefreshTokens.FindAsync(deviceId);
            Assert.IsNotNull(revokedToken);
            Assert.IsTrue(revokedToken.IsRevoked);
        }

        [TestMethod]
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
            await Assert.ThrowsExactlyAsync<UserFriendlyApiException>(async () => await this._identityService.RevokeDeviceAsync(deviceId));
        }
    }
}