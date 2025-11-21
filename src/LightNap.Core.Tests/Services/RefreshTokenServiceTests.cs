using LightNap.Core.Api;
using LightNap.Core.Data;
using LightNap.Core.Data.Entities;
using LightNap.Core.Extensions;
using LightNap.Core.Interfaces;
using LightNap.Core.Services;
using LightNap.Core.Tests.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace LightNap.Core.Tests.Services
{
    [TestClass]
    public class RefreshTokenServiceTests
    {
#pragma warning disable CS8618
        private ApplicationDbContext _dbContext;
        private TestUserContext _userContext;
        private Mock<ILogger<RefreshTokenService>> _loggerMock;
        private RefreshTokenService _refreshTokenService;
#pragma warning restore CS8618

        [TestInitialize]
        public void TestInitialize()
        {
            var services = new ServiceCollection();
            services.AddLogging()
                .AddLightNapInMemoryDatabase($"TestDb_{Guid.NewGuid()}")
                .AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            this._userContext = new TestUserContext();
            this._userContext.IpAddress = "127.0.0.1"; // Set default IP
            services.AddScoped<IUserContext>(sp => this._userContext);

            var serviceProvider = services.BuildServiceProvider();
            this._dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();
            this._loggerMock = new Mock<ILogger<RefreshTokenService>>();
            this._refreshTokenService = new RefreshTokenService(this._dbContext, this._userContext, this._loggerMock.Object);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            this._dbContext.Database.EnsureDeleted();
            this._dbContext.Dispose();
        }

        [TestMethod]
        public async Task CreateRefreshTokenAsync_WithValidData_CreatesToken()
        {
            // Arrange
            var user = new ApplicationUser { Id = "user1", UserName = "testuser" };
            this._dbContext.Users.Add(user);
            await this._dbContext.SaveChangesAsync();

            var deviceDetails = "Test Device";
            var isPersistent = true;
            var expires = DateTime.UtcNow.AddDays(7);

            // Act
            var result = await this._refreshTokenService.CreateRefreshTokenAsync(user, deviceDetails, isPersistent, expires);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(user.Id, result.UserId);
            Assert.AreEqual(deviceDetails, result.Details);
            Assert.AreEqual(isPersistent, result.IsPersistent);
            Assert.AreEqual(expires, result.Expires);
            Assert.IsFalse(result.IsRevoked);
            Assert.IsNotNull(result.Token);
            Assert.AreEqual("127.0.0.1", result.IpAddress); // Default IP since not set

            var dbToken = await this._dbContext.RefreshTokens.FindAsync(result.Id);
            Assert.IsNotNull(dbToken);
        }

        [TestMethod]
        public async Task ValidateAndRefreshTokenAsync_WithValidToken_ReturnsToken()
        {
            // Arrange
            var user = new ApplicationUser { Id = "user1", UserName = "testuser" };
            this._dbContext.Users.Add(user);
            await this._dbContext.SaveChangesAsync();

            var token = new RefreshToken
            {
                Id = "token1",
                Token = "validtoken",
                UserId = user.Id,
                Expires = DateTime.UtcNow.AddDays(1),
                IsRevoked = false,
                Details = "Test Device",
                IsPersistent = false,
                IpAddress = "192.168.1.1",
                LastSeen = DateTime.UtcNow.AddMinutes(-10)
            };
            this._dbContext.RefreshTokens.Add(token);
            await this._dbContext.SaveChangesAsync();

            // Act
            var result = await this._refreshTokenService.ValidateAndRefreshTokenAsync("validtoken");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(token.Id, result.Id);
            Assert.AreEqual(user.Id, result.UserId);

            // Verify LastSeen and IpAddress were updated
            var updatedToken = await this._dbContext.RefreshTokens.FindAsync(token.Id);
            Assert.IsNotNull(updatedToken);
            Assert.AreEqual("127.0.0.1", updatedToken.IpAddress); // Updated IP
        }

        [TestMethod]
        public async Task ValidateAndRefreshTokenAsync_WithExpiredToken_ReturnsNull()
        {
            // Arrange
            var user = new ApplicationUser { Id = "user1", UserName = "testuser" };
            this._dbContext.Users.Add(user);
            await this._dbContext.SaveChangesAsync();

            var token = new RefreshToken
            {
                Id = "token1",
                Token = "expiredtoken",
                UserId = user.Id,
                Expires = DateTime.UtcNow.AddDays(-1),
                IsRevoked = false,
                Details = "Test Device",
                IsPersistent = false,
                IpAddress = "192.168.1.1",
                LastSeen = DateTime.UtcNow.AddMinutes(-10)
            };
            this._dbContext.RefreshTokens.Add(token);
            await this._dbContext.SaveChangesAsync();

            // Act
            var result = await this._refreshTokenService.ValidateAndRefreshTokenAsync("expiredtoken");

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task ValidateAndRefreshTokenAsync_WithRevokedToken_ReturnsNull()
        {
            // Arrange
            var user = new ApplicationUser { Id = "user1", UserName = "testuser" };
            this._dbContext.Users.Add(user);
            await this._dbContext.SaveChangesAsync();

            var token = new RefreshToken
            {
                Id = "token1",
                Token = "revokedtoken",
                UserId = user.Id,
                Expires = DateTime.UtcNow.AddDays(1),
                IsRevoked = true,
                Details = "Test Device",
                IsPersistent = false,
                IpAddress = "192.168.1.1",
                LastSeen = DateTime.UtcNow.AddMinutes(-10)
            };
            this._dbContext.RefreshTokens.Add(token);
            await this._dbContext.SaveChangesAsync();

            // Act
            var result = await this._refreshTokenService.ValidateAndRefreshTokenAsync("revokedtoken");

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task ValidateAndRefreshTokenAsync_WithNonExistentToken_ReturnsNull()
        {
            // Act
            var result = await this._refreshTokenService.ValidateAndRefreshTokenAsync("nonexistent");

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task RevokeRefreshTokenAsync_WithValidToken_RevokesToken()
        {
            // Arrange
            var user = new ApplicationUser { Id = "user1", UserName = "testuser" };
            this._dbContext.Users.Add(user);
            await this._dbContext.SaveChangesAsync();

            var token = new RefreshToken
            {
                Id = "token1",
                Token = "tokentorevoke",
                UserId = user.Id,
                Expires = DateTime.UtcNow.AddDays(1),
                IsRevoked = false,
                Details = "Test Device",
                IsPersistent = false,
                IpAddress = "192.168.1.1",
                LastSeen = DateTime.UtcNow
            };
            this._dbContext.RefreshTokens.Add(token);
            await this._dbContext.SaveChangesAsync();

            this._userContext.LogIn(user.Id);

            // Act
            await this._refreshTokenService.RevokeRefreshTokenAsync("token1");

            // Assert
            var dbToken = await this._dbContext.RefreshTokens.FindAsync("token1");
            Assert.IsNotNull(dbToken);
            Assert.IsTrue(dbToken.IsRevoked);
        }

        [TestMethod]
        public async Task RevokeRefreshTokenAsync_WithTokenBelongingToDifferentUser_ThrowsException()
        {
            // Arrange
            var user1 = new ApplicationUser { Id = "user1", UserName = "testuser1" };
            var user2 = new ApplicationUser { Id = "user2", UserName = "testuser2" };
            this._dbContext.Users.AddRange(user1, user2);
            await this._dbContext.SaveChangesAsync();

            var token = new RefreshToken
            {
                Id = "token1",
                Token = "tokentorevoke",
                UserId = user2.Id, // Belongs to user2
                Expires = DateTime.UtcNow.AddDays(1),
                IsRevoked = false,
                Details = "Test Device",
                IsPersistent = false,
                IpAddress = "192.168.1.1",
                LastSeen = DateTime.UtcNow
            };
            this._dbContext.RefreshTokens.Add(token);
            await this._dbContext.SaveChangesAsync();

            this._userContext.LogIn(user1.Id); // Logged in as user1

            // Act & Assert
            await Assert.ThrowsExactlyAsync<UserFriendlyApiException>(async () =>
            {
                await this._refreshTokenService.RevokeRefreshTokenAsync("token1");
            });
        }

        [TestMethod]
        public async Task RevokeRefreshTokenAsync_WithNonExistentToken_ThrowsException()
        {
            // Arrange
            this._userContext.LogIn("user1");

            // Act & Assert
            await Assert.ThrowsExactlyAsync<UserFriendlyApiException>(async () =>
            {
                await this._refreshTokenService.RevokeRefreshTokenAsync("nonexistent");
            });
        }

        [TestMethod]
        public async Task RevokeRefreshTokenAsync_WithoutAuthentication_ThrowsException()
        {
            // Arrange - user not logged in

            // Act & Assert
            await Assert.ThrowsExactlyAsync<UserFriendlyApiException>(async () =>
            {
                await this._refreshTokenService.RevokeRefreshTokenAsync("token1");
            });
        }

        [TestMethod]
        public async Task PurgeExpiredRefreshTokens_WithExpiredTokens_PurgesThem()
        {
            // Arrange
            var user = new ApplicationUser { Id = "user1", UserName = "testuser" };
            this._dbContext.Users.Add(user);
            await this._dbContext.SaveChangesAsync();

            var expiredToken1 = new RefreshToken
            {
                Id = "expired1",
                Token = "expired1",
                UserId = user.Id,
                Expires = DateTime.UtcNow.AddDays(-2),
                IsRevoked = false,
                Details = "Device1",
                IsPersistent = false,
                IpAddress = "192.168.1.1",
                LastSeen = DateTime.UtcNow.AddDays(-2)
            };

            var expiredToken2 = new RefreshToken
            {
                Id = "expired2",
                Token = "expired2",
                UserId = user.Id,
                Expires = DateTime.UtcNow.AddDays(-1),
                IsRevoked = false,
                Details = "Device2",
                IsPersistent = false,
                IpAddress = "192.168.1.2",
                LastSeen = DateTime.UtcNow.AddDays(-1)
            };

            var validToken = new RefreshToken
            {
                Id = "valid",
                Token = "valid",
                UserId = user.Id,
                Expires = DateTime.UtcNow.AddDays(1),
                IsRevoked = false,
                Details = "Device3",
                IsPersistent = false,
                IpAddress = "192.168.1.3",
                LastSeen = DateTime.UtcNow
            };

            this._dbContext.RefreshTokens.AddRange(expiredToken1, expiredToken2, validToken);
            await this._dbContext.SaveChangesAsync();

            this._userContext.LogInAdministrator();

            // Act
            await this._refreshTokenService.PurgeExpiredRefreshTokens();

            // Assert
            var remainingTokens = await this._dbContext.RefreshTokens.ToListAsync();
            Assert.HasCount(1, remainingTokens);
            Assert.AreEqual("valid", remainingTokens[0].Id);
        }

        [TestMethod]
        public async Task PurgeExpiredRefreshTokens_WithoutAdminRights_ThrowsException()
        {
            // Arrange
            this._userContext.LogIn("user1"); // Not admin

            // Act & Assert
            await Assert.ThrowsExactlyAsync<UserFriendlyApiException>(async () =>
            {
                await this._refreshTokenService.PurgeExpiredRefreshTokens();
            });
        }
    }
}