using LightNap.Core.Api;
using LightNap.Core.Data;
using LightNap.Core.Data.Entities;
using LightNap.Core.Extensions;
using LightNap.Core.Interfaces;
using LightNap.Core.Tests.Utilities;
using LightNap.Core.Users.Dto.Request;
using LightNap.Core.Users.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace LightNap.Core.Tests.Services
{
    [TestClass]
    public class UsersServiceTests
    {
        // These will be initialized during TestInitialize.
#pragma warning disable CS8618
        private UserManager<ApplicationUser> _userManager;
        private ApplicationDbContext _dbContext;
        private TestUserContext _userContext;
        private UsersService _usersService;
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

            services.AddScoped<UsersService>();

            this._userContext = new TestUserContext();
            services.AddScoped<IUserContext>(sp => this._userContext);

            var serviceProvider = services.BuildServiceProvider();
            this._dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();
            this._userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            this._userContext.LogInAdministrator();
            this._usersService = serviceProvider.GetRequiredService<UsersService>();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            this._dbContext.Database.EnsureDeleted();
            this._dbContext.Dispose();
        }

        [TestMethod]
        public async Task GetUserAsync_WithExistingUserId_ReturnsUser()
        {
            // Arrange
            var userId = "test-user-id";
            await TestHelper.CreateTestUserAsync(this._userManager, userId);

            // Act
            var user = await this._usersService.GetUserAsync(userId);

            // Assert
            Assert.IsNotNull(user);
            Assert.AreEqual(userId, user.Id);
        }

        [TestMethod]
        public async Task GetUserAsync_WithNonExistentUserId_ReturnsNull()
        {
            // Arrange
            var userId = "non-existent-user-id";

            // Act
            var user = await this._usersService.GetUserAsync(userId);

            // Assert
            Assert.IsNull(user);
        }

        [TestMethod]
        public async Task GetUserByUserNameAsync_WithExistingUserName_ReturnsUser()
        {
            // Arrange
            var userId = "test-user-id";
            var userName = "testuser";
            await TestHelper.CreateTestUserAsync(this._userManager, userId, userName);

            // Act
            var user = await this._usersService.GetUserByUserNameAsync(userName);

            // Assert
            Assert.IsNotNull(user);
            Assert.AreEqual(userName, user.UserName);
        }

        [TestMethod]
        public async Task GetUserByUserNameAsync_WithNonExistentUserName_ReturnsNull()
        {
            // Arrange
            var userName = "nonexistentuser";

            // Act
            var user = await this._usersService.GetUserByUserNameAsync(userName);

            // Assert
            Assert.IsNull(user);
        }

        [TestMethod]
        public async Task GetUsersByIdsAsync_WithMultipleValidIds_ReturnsAllUsers()
        {
            // Arrange
            var userId1 = "test-user-id1";
            var userId2 = "test-user-id2";
            var userId3 = "test-user-id3";
            await TestHelper.CreateTestUserAsync(this._userManager, userId1);
            await TestHelper.CreateTestUserAsync(this._userManager, userId2);
            await TestHelper.CreateTestUserAsync(this._userManager, userId3);

            // Act
            var users = await this._usersService.GetUsersByIdsAsync([userId1, userId2, userId3]);

            // Assert
            Assert.HasCount(3, users);
        }

        [TestMethod]
        public async Task GetUsersByIdsAsync_WithMixedValidAndInvalidIds_ReturnsOnlyValidUsers()
        {
            // Arrange
            var userId1 = "test-user-id1";
            var userId2 = "test-user-id2";
            await TestHelper.CreateTestUserAsync(this._userManager, userId1);
            await TestHelper.CreateTestUserAsync(this._userManager, userId2);

            // Act
            var users = await this._usersService.GetUsersByIdsAsync([userId1, "invalid-id", userId2]);

            // Assert
            Assert.HasCount(2, users);
        }

        [TestMethod]
        public async Task UpdateUserAsync_WithExistingUser_UpdatesAndReturnsUser()
        {
            // Arrange
            var userId = "test-user-id";
            AdminUpdateUserRequestDto updateDto = new();
            await TestHelper.CreateTestUserAsync(this._userManager, userId);

            // Act
            var user = await this._usersService.UpdateUserAsync(userId, updateDto);

            // Assert
            Assert.AreEqual(userId, user.Id);
        }

        [TestMethod]
        public async Task UpdateUserAsync_WithNonExistentUser_ThrowsUserFriendlyApiException()
        {
            // Arrange
            var userId = "non-existent-user-id";
            var updateDto = new AdminUpdateUserRequestDto();

            // Act & Assert
            await Assert.ThrowsExactlyAsync<UserFriendlyApiException>(async () =>
            {
                await this._usersService.UpdateUserAsync(userId, updateDto);
            });
        }

        [TestMethod]
        public async Task DeleteUserAsync_WithExistingUser_DeletesUserSuccessfully()
        {
            // Arrange
            var userId = "test-user-id";
            await TestHelper.CreateTestUserAsync(this._userManager, userId);

            // Act
            await this._usersService.DeleteUserAsync(userId);

            // Assert
            var user = await this._userManager.FindByIdAsync(userId);
            Assert.IsNull(user);
        }

        [TestMethod]
        public async Task DeleteUserAsync_WithNonExistentUser_ThrowsUserFriendlyApiException()
        {
            // Arrange
            var userId = "non-existent-user-id";

            // Act & Assert
            await Assert.ThrowsExactlyAsync<UserFriendlyApiException>(async () =>
            {
                await this._usersService.DeleteUserAsync(userId);
            });
        }

        [TestMethod]
        public async Task SearchUsersAsync_WithEmailFilter_ReturnsMatchingUsers()
        {
            // Arrange
            var searchDto = new AdminSearchUsersRequestDto { Email = "example" };
            await TestHelper.CreateTestUserAsync(this._userManager, "test-user-id1", "testuser1", "test1@example.com");
            await TestHelper.CreateTestUserAsync(this._userManager, "test-user-id2", "testuser2", "test2@exNOTample.com");
            await TestHelper.CreateTestUserAsync(this._userManager, "test-user-id3", "testuser3", "test3@example.com");

            // Act
            var result = await this._usersService.SearchUsersAsync(searchDto);

            // Assert
            Assert.AreEqual(2, result.TotalCount);
        }

        [TestMethod]
        public async Task SearchUsersAsync_WithNoMatches_ReturnsEmptyResult()
        {
            // Arrange
            var searchDto = new AdminSearchUsersRequestDto { Email = "nonexistent" };
            await TestHelper.CreateTestUserAsync(this._userManager, "test-user-id1", "testuser1", "test1@example.com");

            // Act
            var result = await this._usersService.SearchUsersAsync(searchDto);

            // Assert
            Assert.AreEqual(0, result.TotalCount);
            Assert.IsEmpty(result.Data);
        }

        [TestMethod]
        public async Task SearchUsersAsync_WithPagination_ReturnsCorrectPage()
        {
            // Arrange
            await TestHelper.CreateTestUserAsync(this._userManager, "test-user-id1", "testuser1", "test1@example.com");
            await TestHelper.CreateTestUserAsync(this._userManager, "test-user-id2", "testuser2", "test2@example.com");
            await TestHelper.CreateTestUserAsync(this._userManager, "test-user-id3", "testuser3", "test3@example.com");

            var searchDto = new AdminSearchUsersRequestDto
            {
                Email = "example",
                PageNumber = 1,
                PageSize = 2
            };

            // Act
            var result = await this._usersService.SearchUsersAsync(searchDto);

            // Assert
            Assert.AreEqual(3, result.TotalCount);
            Assert.HasCount(2, result.Data);
            Assert.AreEqual(1, result.PageNumber);
        }

        [TestMethod]
        public async Task LockUserAccountAsync_WithExistingUser_LocksUserSuccessfully()
        {
            // Arrange
            var userId = "test-user-id";
            await TestHelper.CreateTestUserAsync(this._userManager, userId);

            // Act
            await this._usersService.LockUserAccountAsync(userId);

            // Assert
            var user = await this._userManager.FindByIdAsync(userId);
            Assert.IsNotNull(user);
            Assert.IsNotNull(user.LockoutEnd);
            Assert.IsTrue(user.LockoutEnd > DateTimeOffset.UtcNow);
        }

        [TestMethod]
        public async Task LockUserAccountAsync_WithNonExistentUser_ThrowsUserFriendlyApiException()
        {
            // Arrange
            var userId = "non-existent-user-id";

            // Act & Assert
            await Assert.ThrowsExactlyAsync<UserFriendlyApiException>(async () =>
            {
                await this._usersService.LockUserAccountAsync(userId);
            });
        }

        [TestMethod]
        public async Task UnlockUserAccountAsync_WithLockedUser_UnlocksUserSuccessfully()
        {
            // Arrange
            var userId = "test-user-id";
            await TestHelper.CreateTestUserAsync(this._userManager, userId);
            await this._usersService.LockUserAccountAsync(userId);

            // Act
            await this._usersService.UnlockUserAccountAsync(userId);

            // Assert
            var user = await this._userManager.FindByIdAsync(userId);
            Assert.IsNotNull(user);
            Assert.IsNull(user.LockoutEnd);
        }

        [TestMethod]
        public async Task UnlockUserAccountAsync_WithNonExistentUser_ThrowsUserFriendlyApiException()
        {
            // Arrange
            var userId = "non-existent-user-id";

            // Act & Assert
            await Assert.ThrowsExactlyAsync<UserFriendlyApiException>(async () =>
            {
                await this._usersService.UnlockUserAccountAsync(userId);
            });
        }
    }
}
