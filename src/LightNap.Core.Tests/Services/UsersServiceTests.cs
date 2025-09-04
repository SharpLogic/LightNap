using LightNap.Core.Api;
using LightNap.Core.Configuration;
using LightNap.Core.Data;
using LightNap.Core.Data.Entities;
using LightNap.Core.Extensions;
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
        private RoleManager<ApplicationRole> _roleManager;
        private UserManager<ApplicationUser> _userManager;
        private ApplicationDbContext _dbContext;
        private TestUserContext _userContext;
        private UsersService _administratorService;
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

            var serviceProvider = services.BuildServiceProvider();
            this._dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();
            this._userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            this._roleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
            this._userContext = new TestUserContext();
            this._userContext.Roles.Add(Constants.Roles.Administrator); // Set the user context to be an administrator for testing purposes.
            this._administratorService = new UsersService(this._userManager, this._dbContext, this._userContext);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            this._dbContext.Database.EnsureDeleted();
            this._dbContext.Dispose();
        }

        [TestMethod]
        public async Task GetUserAsync_UserExists_ReturnsUser()
        {
            // Arrange
            var userId = "test-user-id";
            await TestHelper.CreateTestUserAsync(this._userManager, userId);

            // Act
            var user = await this._administratorService.GetUserAsync(userId);

            // Assert
            Assert.IsNotNull(user);
            Assert.AreEqual(userId, user.Id);
        }

        [TestMethod]
        public async Task GetUserAsync_UserDoesNotExist_ReturnsNull()
        {
            // Arrange
            var userId = "non-existent-user-id";

            // Act
            var user = await this._administratorService.GetUserAsync(userId);

            // Assert
            Assert.IsNull(user);
        }

        [TestMethod]
        public async Task UpdateUserAsync_UserExists_UpdatesUser()
        {
            // Arrange
            var userId = "test-user-id";
            AdminUpdateUserRequestDto updateDto = new();
            await TestHelper.CreateTestUserAsync(this._userManager, userId);

            // Act
            var user = await this._administratorService.UpdateUserAsync(userId, updateDto);

            // Assert
            Assert.AreEqual(userId, user.Id);
        }

        [TestMethod]
        public async Task UpdateUserAsync_UserDoesNotExist_ThrowsError()
        {
            // Arrange
            var userId = "non-existent-user-id";
            var updateDto = new AdminUpdateUserRequestDto();

            // Act & Assert
            await Assert.ThrowsExactlyAsync<UserFriendlyApiException>(async () =>
            {
                await this._administratorService.UpdateUserAsync(userId, updateDto);
            });
        }

        [TestMethod]
        public async Task DeleteUserAsync_UserExists_DeletesUser()
        {
            // Arrange
            var userId = "test-user-id";
            await TestHelper.CreateTestUserAsync(this._userManager, userId);

            // Act
            await this._administratorService.DeleteUserAsync(userId);
        }

        [TestMethod]
        public async Task DeleteUserAsync_UserDoesNotExist_ThrowsError()
        {
            // Arrange
            var userId = "non-existent-user-id";

            // Act & Assert
            await Assert.ThrowsExactlyAsync<UserFriendlyApiException>(async () =>
            {
                await this._administratorService.DeleteUserAsync(userId);
            });
        }

        [TestMethod]
        public async Task SearchUsersAsync_ValidRequest_ReturnsPagedResponse()
        {
            // Arrange
            var searchDto = new AdminSearchUsersRequestDto { Email = "example" };
            await TestHelper.CreateTestUserAsync(this._userManager, "test-user-id1", "testuser1", "test1@example.com");
            await TestHelper.CreateTestUserAsync(this._userManager, "test-user-id2", "testuser2", "test2@exNOTample.com");
            await TestHelper.CreateTestUserAsync(this._userManager, "test-user-id3", "testuser3", "test3@example.com");

            // Act
            var result = await this._administratorService.SearchUsersAsync(searchDto);

            // Assert
            Assert.AreEqual(2, result.TotalCount);
        }

        [TestMethod]
        public async Task LockUserAsync_UserExists_LocksUser()
        {
            // Arrange
            var userId = "test-user-id";
            await TestHelper.CreateTestUserAsync(this._userManager, userId);

            // Act
            await this._administratorService.LockUserAccountAsync(userId);

            // Assert
            var user = await this._userManager.FindByIdAsync(userId);
            Assert.IsNotNull(user);
            Assert.IsNotNull(user.LockoutEnd);
        }

        [TestMethod]
        public async Task LockUserAsync_UserDoesNotExist_ThrowsError()
        {
            // Arrange
            var userId = "non-existent-user-id";

            // Act & Assert
            await Assert.ThrowsExactlyAsync<UserFriendlyApiException>(async () =>
            {
                await this._administratorService.LockUserAccountAsync(userId);
            });
        }

        [TestMethod]
        public async Task UnlockUserAsync_UserExists_UnlocksUser()
        {
            // Arrange
            var userId = "test-user-id";
            await TestHelper.CreateTestUserAsync(this._userManager, userId);
            await this._administratorService.LockUserAccountAsync(userId);

            // Act
            await this._administratorService.UnlockUserAccountAsync(userId);

            // Assert
            var user = await this._userManager.FindByIdAsync(userId);
            Assert.IsNotNull(user);
            Assert.IsNull(user.LockoutEnd);
        }

        [TestMethod]
        public async Task UnlockUserAsync_UserDoesNotExist_ThrowsError()
        {
            // Arrange
            var userId = "non-existent-user-id";

            // Act & Assert
            await Assert.ThrowsExactlyAsync<UserFriendlyApiException>(async () =>
            {
                await this._administratorService.UnlockUserAccountAsync(userId);
            });
        }


    }
}
