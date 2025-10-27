using LightNap.Core.Api;
using LightNap.Core.Configuration;
using LightNap.Core.Data;
using LightNap.Core.Data.Entities;
using LightNap.Core.Extensions;
using LightNap.Core.Identity.Dto.Response;
using LightNap.Core.Interfaces;
using LightNap.Core.Tests.Utilities;
using LightNap.Core.Users.Dto.Request;
using LightNap.Core.Users.Interfaces;
using LightNap.Core.Users.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;

namespace LightNap.Core.Tests.Services
{
    [TestClass]
    public class ClaimsServiceTests
    {
#pragma warning disable CS8618
        private UserManager<ApplicationUser> _userManager;
        private ApplicationDbContext _dbContext;
        private TestUserContext _userContext;
        private IClaimsService _claimsService;
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

            this._userContext = new TestUserContext();
            services.AddScoped<IUserContext>(sp => this._userContext);
            services.AddScoped<IClaimsService, ClaimsService>();

            var serviceProvider = services.BuildServiceProvider();
            this._dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();
            this._userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            this._claimsService = serviceProvider.GetRequiredService<IClaimsService>();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            this._dbContext.Database.EnsureDeleted();
            this._dbContext.Dispose();
        }

        private void LogInAdministrator()
        {
            this._userContext.UserId = "admin-user-id";
            this._userContext.Roles.Add(Constants.Roles.Administrator);
        }

        private void LogInNormalUser(string userId)
        {
            this._userContext.UserId = userId;
            this._userContext.Roles.Clear();
        }

        private void LogOut()
        {
            this._userContext.UserId = null;
            this._userContext.Roles.Clear();
        }

        [TestMethod]
        public async Task AddUserClaimAsync_UserAndClaimExist_AddsClaimToUser()
        {
            // Arrange
            var userId = "test-user-id";
            var claimType = "test-claim-type";
            var claimValue = "test-claim-value";
            var user = await TestHelper.CreateTestUserAsync(this._userManager, userId);
            this.LogInAdministrator();

            // Act
            await this._claimsService.AddUserClaimAsync(userId, new ClaimDto { Type = claimType, Value = claimValue });

            // Assert
            var claims = await this._userManager.GetClaimsAsync(user);
            Assert.IsTrue(claims.Any(c => c.Type == claimType && c.Value == claimValue));
        }

        [TestMethod]
        public async Task AddUserClaimAsync_DuplicateClaim_ThrowsError()
        {
            // Arrange
            var userId = "test-user-id";
            var claimType = "test-claim-type";
            var claimValue = "test-claim-value";
            var user = await TestHelper.CreateTestUserAsync(this._userManager, userId);
            this.LogInAdministrator();

            // Add the claim once
            await this._claimsService.AddUserClaimAsync(userId, new ClaimDto { Type = claimType, Value = claimValue });

            // Act & Assert: Adding the same claim again should throw
            await Assert.ThrowsExactlyAsync<UserFriendlyApiException>(async () =>
                await this._claimsService.AddUserClaimAsync(userId, new ClaimDto { Type = claimType, Value = claimValue }));
        }

        [TestMethod]
        public async Task AddUserClaimAsync_UserNotAdmin_ThrowsError()
        {
            // Arrange
            var userId = "test-user-id";
            var claimType = "test-claim-type";
            var claimValue = "test-claim-value";
            var user = await TestHelper.CreateTestUserAsync(this._userManager, userId);
            this.LogInNormalUser(userId);

            // Act & Assert
            await Assert.ThrowsExactlyAsync<UserFriendlyApiException>(async () =>
                await this._claimsService.AddUserClaimAsync(userId, new ClaimDto { Type = claimType, Value = claimValue }));
        }

        [TestMethod]
        public async Task AddUserClaimAsync_UserLoggedOut_ThrowsError()
        {
            // Arrange
            var userId = "test-user-id";
            var claimType = "test-claim-type";
            var claimValue = "test-claim-value";
            var user = await TestHelper.CreateTestUserAsync(this._userManager, userId);
            this.LogOut();

            // Act & Assert
            await Assert.ThrowsExactlyAsync<UserFriendlyApiException>(async () =>
                await this._claimsService.AddUserClaimAsync(userId, new ClaimDto { Type = claimType, Value = claimValue }));
        }

        [TestMethod]
        public async Task AddUserClaimAsync_UserDoesNotExist_ThrowsError()
        {
            // Arrange
            var userId = "non-existent-user-id";
            var claimType = "test-claim-type";
            var claimValue = "test-claim-value";
            this.LogInAdministrator();

            // Act
            await Assert.ThrowsExactlyAsync<UserFriendlyApiException>(async () => await this._claimsService.AddUserClaimAsync(userId, new ClaimDto { Type = claimType, Value = claimValue }));
        }

        [TestMethod]
        public async Task GetClaimsForUserAsync_UserExists_ReturnsClaims()
        {
            // Arrange
            var userId = "test-user-id";
            var claimType = "test-claim-type";
            var claimValue = "test-claim-value";
            var user = await TestHelper.CreateTestUserAsync(this._userManager, userId);
            await this._userManager.AddClaimAsync(user, new Claim(claimType, claimValue));

            var otherUserId = "other-user-id";
            var otherClaimType = "other-claim-type";
            var otherClaimValue = "other-claim-value";
            var otherUser = await TestHelper.CreateTestUserAsync(this._userManager, otherUserId);
            await this._userManager.AddClaimAsync(otherUser, new Claim(otherClaimType, otherClaimValue));

            this.LogInAdministrator();

            // Act
            var claims = await this._claimsService.SearchClaimsAsync(new SearchUserClaimsRequestDto() { UserId = userId });

            // Assert
            Assert.HasCount(2, claims.Data);
            Assert.IsTrue(claims.Data.Any(c => c.Type == claimType && c.Value == claimValue));
            Assert.IsTrue(claims.Data.Any(c => c.Type == otherClaimType && c.Value == otherClaimValue));
        }

        [TestMethod]
        public async Task RemoveClaimFromUserAsync_UserAndClaimExist_RemovesClaimFromUser()
        {
            // Arrange
            var userId = "test-user-id";
            var claimType = "test-claim-type";
            var claimValue = "test-claim-value";
            var user = await TestHelper.CreateTestUserAsync(this._userManager, userId);
            this.LogInAdministrator();
            await this._userManager.AddClaimAsync(user, new Claim(claimType, claimValue));
            var claims = await this._userManager.GetClaimsAsync(user);
            Assert.IsTrue(claims.Any(c => c.Type == claimType && c.Value == claimValue));
            this._userContext.UserId = user.Id;

            // Act
            await this._claimsService.RemoveUserClaimAsync(userId, new ClaimDto { Type = claimType, Value = claimValue });

            // Assert
            claims = await this._userManager.GetClaimsAsync(user);
            Assert.IsFalse(claims.Any(c => c.Type == claimType && c.Value == claimValue));
        }

        [TestMethod]
        public async Task RemoveUserClaimAsync_UserNotAdmin_ThrowsError()
        {
            // Arrange
            var userId = "test-user-id";
            var claimType = "test-claim-type";
            var claimValue = "test-claim-value";
            var user = await TestHelper.CreateTestUserAsync(this._userManager, userId);
            this.LogInNormalUser(userId);
            await this._userManager.AddClaimAsync(user, new Claim(claimType, claimValue));

            // Act & Assert
            await Assert.ThrowsExactlyAsync<UserFriendlyApiException>(async () =>
                await this._claimsService.RemoveUserClaimAsync(userId, new ClaimDto { Type = claimType, Value = claimValue }));
        }

        [TestMethod]
        public async Task RemoveUserClaimAsync_UserLoggedOut_ThrowsError()
        {
            // Arrange
            var userId = "test-user-id";
            var claimType = "test-claim-type";
            var claimValue = "test-claim-value";
            var user = await TestHelper.CreateTestUserAsync(this._userManager, userId);
            this.LogOut();
            await this._userManager.AddClaimAsync(user, new Claim(claimType, claimValue));

            // Act & Assert
            await Assert.ThrowsExactlyAsync<UserFriendlyApiException>(async () =>
                await this._claimsService.RemoveUserClaimAsync(userId, new ClaimDto { Type = claimType, Value = claimValue }));
        }

        [TestMethod]
        public async Task RemoveUserClaimAsync_UserDoesNotExist_ThrowsError()
        {
            // Arrange
            var userId = "non-existent-user-id";
            var claimType = "test-claim-type";
            var claimValue = "test-claim-value";
            this.LogInAdministrator();

            // Act & Assert
            await Assert.ThrowsExactlyAsync<UserFriendlyApiException>(async () =>
                await this._claimsService.RemoveUserClaimAsync(userId, new ClaimDto { Type = claimType, Value = claimValue }));
        }

        [TestMethod]
        public async Task RemoveUserClaimAsync_UserExistsButClaimDoesNotExist_DoesNotThrowAndDoesNotRemoveAnyClaim()
        {
            // Arrange
            var userId = "test-user-id";
            var claimType = "non-existent-claim-type";
            var claimValue = "non-existent-claim-value";
            var user = await TestHelper.CreateTestUserAsync(this._userManager, userId);
            this.LogInAdministrator();

            // Ensure user has no claims
            var claimsBefore = await this._userManager.GetClaimsAsync(user);
            Assert.IsFalse(claimsBefore.Any(c => c.Type == claimType && c.Value == claimValue));

            // Act
            await this._claimsService.RemoveUserClaimAsync(userId, new ClaimDto { Type = claimType, Value = claimValue });

            // Assert
            var claimsAfter = await this._userManager.GetClaimsAsync(user);
            Assert.HasCount(claimsBefore.Count, claimsAfter);
            Assert.IsFalse(claimsAfter.Any(c => c.Type == claimType && c.Value == claimValue));
        }

        [TestMethod]
        public async Task SearchClaimsAsync_NonAdminUserLoggedIn_ReturnsOwnClaims()
        {
            // Arrange
            var userId = "test-user-id";
            var claimType = "test-claim-type";
            var claimValue = "test-claim-value";
            var user = await TestHelper.CreateTestUserAsync(this._userManager, userId);
            await this._userManager.AddClaimAsync(user, new Claim(claimType, claimValue));

            var otherUserId = "other-user-id";
            var otherClaimType = "other-claim-type";
            var otherClaimValue = "other-claim-value";
            var otherUser = await TestHelper.CreateTestUserAsync(this._userManager, otherUserId);
            await this._userManager.AddClaimAsync(otherUser, new Claim(otherClaimType, otherClaimValue));

            this.LogInNormalUser(userId);

            // Act
            var claims = await this._claimsService.GetMyClaimsAsync(new PagedRequestDtoBase());

            // Assert
            Assert.HasCount(1, claims.Data);
            Assert.IsTrue(claims.Data.Any(c => c.Type == claimType && c.Value == claimValue));
        }

        [TestMethod]
        public async Task GetMyClaimsAsync_UserLoggedOut_ThrowsError()
        {
            // Arrange

            // Act & Assert
            await Assert.ThrowsExactlyAsync<UserFriendlyApiException>(async () =>
                await this._claimsService.GetMyClaimsAsync(new PagedRequestDtoBase()));
        }

        [TestMethod]
        public async Task GetMyClaimsAsync_UserHasNoClaims_ReturnsEmptyList()
        {
            // Arrange
            var userId = "test-user-id";
            await TestHelper.CreateTestUserAsync(this._userManager, userId);
            this.LogInNormalUser(userId);

            // Act
            var claims = await this._claimsService.GetMyClaimsAsync(new PagedRequestDtoBase());

            // Assert
            Assert.IsNotNull(claims);
            Assert.IsEmpty(claims.Data);
        }

        [TestMethod]
        public async Task GetMyClaims_UserAuthenticated_ReturnsUserClaims()
        {
            // Arrange
            var userId = "test-user-id";
            var claimType = "test-claim-type";
            var claimValue = "test-claim-value";
            var user = await TestHelper.CreateTestUserAsync(this._userManager, userId);
            await this._userManager.AddClaimAsync(user, new Claim(claimType, claimValue));
            this.LogInNormalUser(userId);

            // Act
            var claims = await this._claimsService.GetMyClaimsAsync(new PagedRequestDtoBase());

            // Assert
            Assert.IsNotNull(claims);
            Assert.HasCount(1, claims.Data);
            Assert.IsTrue(claims.Data.Any(c => c.Type == claimType && c.Value == claimValue));
        }

        [TestMethod]
        public async Task GetMyClaims_UserNotAuthenticated_ThrowsError()
        {
            // Arrange
            this.LogOut();

            // Act & Assert
            await Assert.ThrowsExactlyAsync<UserFriendlyApiException>(async () =>
                await this._claimsService.GetMyClaimsAsync(new PagedRequestDtoBase()));
        }

        [TestMethod]
        public async Task GetMyClaims_UserHasNoClaims_ReturnsEmptyList()
        {
            // Arrange
            var userId = "test-user-id";
            await TestHelper.CreateTestUserAsync(this._userManager, userId);
            this.LogInNormalUser(userId);

            // Act
            var claims = await this._claimsService.GetMyClaimsAsync(new PagedRequestDtoBase());

            // Assert
            Assert.IsNotNull(claims);
            Assert.IsEmpty(claims.Data);
        }

        [TestMethod]
        public async Task GetUsersWithClaim_ClaimExists_ReturnsUserIds()
        {
            // Arrange
            var claimType = "test-claim-type";
            var claimValue = "test-claim-value";
            var user1 = await TestHelper.CreateTestUserAsync(this._userManager, "user-1");
            var user2 = await TestHelper.CreateTestUserAsync(this._userManager, "user-2");
            var user3 = await TestHelper.CreateTestUserAsync(this._userManager, "user-3");

            await this._userManager.AddClaimAsync(user1, new Claim(claimType, claimValue));
            await this._userManager.AddClaimAsync(user2, new Claim(claimType, claimValue));
            await this._userManager.AddClaimAsync(user3, new Claim(claimType, "different-value"));

            this.LogInAdministrator();

            // Act
            var results = await this._claimsService.GetUsersWithClaimAsync(
                new SearchClaimRequestDto()
                {
                    Type = claimType,
                    Value = claimValue
                });

            // Assert
            Assert.IsNotNull(results);
            Assert.HasCount(2, results.Data);
            Assert.Contains("user-1", results.Data);
            Assert.Contains("user-2", results.Data);
            Assert.DoesNotContain("user-3", results.Data);
        }

        [TestMethod]
        public async Task GetUsersWithClaim_NoUsersWithClaim_ReturnsEmptyList()
        {
            // Arrange
            var claimType = "test-claim-type";
            var claimValue = "test-claim-value";
            await TestHelper.CreateTestUserAsync(this._userManager, "user-1");
            this.LogInAdministrator();

            // Act
            var userIds = await this._claimsService.GetUsersWithClaimAsync(
                new SearchClaimRequestDto()
                {
                    Type = claimType,
                    Value = claimValue
                });

            // Assert
            Assert.IsNotNull(userIds);
            Assert.IsEmpty(userIds.Data);
        }

        [TestMethod]
        public async Task GetUsersWithClaim_UserNotAdmin_ThrowsError()
        {
            // Arrange
            var userId = "test-user-id";
            await TestHelper.CreateTestUserAsync(this._userManager, userId);
            this.LogInNormalUser(userId);

            // Act & Assert
            await Assert.ThrowsExactlyAsync<UserFriendlyApiException>(async () =>
                await this._claimsService.GetUsersWithClaimAsync(
                    new SearchClaimRequestDto()
                    {
                        Type = "test-claim-type",
                        Value = "test-claim-value"
                    }));
        }

        [TestMethod]
        public async Task GetUsersWithClaim_UserLoggedOut_ThrowsError()
        {
            // Arrange
            this.LogOut();

            // Act & Assert
            await Assert.ThrowsExactlyAsync<UserFriendlyApiException>(async () =>
                await this._claimsService.GetUsersWithClaimAsync(
                    new SearchClaimRequestDto()
                    {
                        Type = "test-claim-type",
                        Value = "test-claim-value"
                    }));
        }

        [TestMethod]
        public async Task SearchClaimsAsync_AdminUser_ReturnsAllDistinctClaims()
        {
            // Arrange
            var user1 = await TestHelper.CreateTestUserAsync(this._userManager, "user-1");
            var user2 = await TestHelper.CreateTestUserAsync(this._userManager, "user-2");

            await this._userManager.AddClaimAsync(user1, new Claim("claim-type-1", "claim-value-1"));
            await this._userManager.AddClaimAsync(user1, new Claim("claim-type-2", "claim-value-2"));
            await this._userManager.AddClaimAsync(user2, new Claim("claim-type-1", "claim-value-1")); // Duplicate
            await this._userManager.AddClaimAsync(user2, new Claim("claim-type-3", "claim-value-3"));

            this.LogInAdministrator();

            // Act
            var result = await this._claimsService.SearchClaimsAsync(new SearchClaimsRequestDto());

            // Assert
            Assert.IsNotNull(result);
            Assert.HasCount(3, result.Data); // Should return distinct claims only
            Assert.IsTrue(result.Data.Any(c => c.Type == "claim-type-1" && c.Value == "claim-value-1"));
            Assert.IsTrue(result.Data.Any(c => c.Type == "claim-type-2" && c.Value == "claim-value-2"));
            Assert.IsTrue(result.Data.Any(c => c.Type == "claim-type-3" && c.Value == "claim-value-3"));
        }

        [TestMethod]
        public async Task SearchClaimsAsync_WithTypeFilter_ReturnsFilteredClaims()
        {
            // Arrange
            var user = await TestHelper.CreateTestUserAsync(this._userManager, "user-1");
            await this._userManager.AddClaimAsync(user, new Claim("type-a", "value-1"));
            await this._userManager.AddClaimAsync(user, new Claim("type-b", "value-2"));

            this.LogInAdministrator();

            // Act
            var result = await this._claimsService.SearchClaimsAsync(new SearchClaimsRequestDto { Type = "type-a" });

            // Assert
            Assert.HasCount(1, result.Data);
            Assert.IsTrue(result.Data.Any(c => c.Type == "type-a"));
        }

        [TestMethod]
        public async Task SearchClaimsAsync_WithTypeContainsFilter_ReturnsMatchingClaims()
        {
            // Arrange
            var user = await TestHelper.CreateTestUserAsync(this._userManager, "user-1");
            await this._userManager.AddClaimAsync(user, new Claim("user-permission", "value-1"));
            await this._userManager.AddClaimAsync(user, new Claim("admin-permission", "value-2"));
            await this._userManager.AddClaimAsync(user, new Claim("other-type", "value-3"));

            this.LogInAdministrator();

            // Act
            var result = await this._claimsService.SearchClaimsAsync(new SearchClaimsRequestDto { TypeContains = "permission" });

            // Assert
            Assert.HasCount(2, result.Data);
            Assert.IsTrue(result.Data.Any(c => c.Type == "user-permission"));
            Assert.IsTrue(result.Data.Any(c => c.Type == "admin-permission"));
        }

        [TestMethod]
        public async Task SearchUserClaimsAsync_AdminUser_ReturnsAllUserClaims()
        {
            // Arrange
            var user1 = await TestHelper.CreateTestUserAsync(this._userManager, "user-1");
            var user2 = await TestHelper.CreateTestUserAsync(this._userManager, "user-2");

            await this._userManager.AddClaimAsync(user1, new Claim("claim-type-1", "claim-value-1"));
            await this._userManager.AddClaimAsync(user2, new Claim("claim-type-2", "claim-value-2"));

            this.LogInAdministrator();

            // Act
            var result = await this._claimsService.SearchUserClaimsAsync(new SearchUserClaimsRequestDto());

            // Assert
            Assert.IsNotNull(result);
            Assert.HasCount(2, result.Data);
        }

        [TestMethod]
        public async Task SearchUserClaimsAsync_WithUserIdFilter_ReturnsOnlySpecifiedUserClaims()
        {
            // Arrange
            var user1 = await TestHelper.CreateTestUserAsync(this._userManager, "user-1");
            var user2 = await TestHelper.CreateTestUserAsync(this._userManager, "user-2");

            await this._userManager.AddClaimAsync(user1, new Claim("claim-type-1", "claim-value-1"));
            await this._userManager.AddClaimAsync(user2, new Claim("claim-type-2", "claim-value-2"));

            this.LogInAdministrator();

            // Act
            var result = await this._claimsService.SearchUserClaimsAsync(new SearchUserClaimsRequestDto { UserId = "user-1" });

            // Assert
            Assert.HasCount(1, result.Data);
            Assert.AreEqual("user-1", result.Data[0].UserId);
        }

        [TestMethod]
        public async Task SearchUserClaimsAsync_NonAdminUser_ThrowsError()
        {
            // Arrange
            var userId = "test-user-id";
            await TestHelper.CreateTestUserAsync(this._userManager, userId);
            this.LogInNormalUser(userId);

            // Act & Assert
            await Assert.ThrowsExactlyAsync<UserFriendlyApiException>(async () =>
                await this._claimsService.SearchUserClaimsAsync(new SearchUserClaimsRequestDto()));
        }

        [TestMethod]
        public async Task SearchUserClaimsAsync_UserLoggedOut_ThrowsError()
        {
            // Arrange
            this.LogOut();

            // Act & Assert
            await Assert.ThrowsExactlyAsync<UserFriendlyApiException>(async () =>
                await this._claimsService.SearchUserClaimsAsync(new SearchUserClaimsRequestDto()));
        }

        [TestMethod]
        public async Task SearchUserClaimsAsync_UserWithNoClaims_ReturnsEmptyList()
        {
            // Arrange
            await TestHelper.CreateTestUserAsync(this._userManager, "user-1");
            this.LogInAdministrator();

            // Act
            var result = await this._claimsService.SearchUserClaimsAsync(new SearchUserClaimsRequestDto { UserId = "user-1" });

            // Assert
            Assert.IsNotNull(result.Data);
            Assert.IsEmpty(result.Data);
        }
    }
}
