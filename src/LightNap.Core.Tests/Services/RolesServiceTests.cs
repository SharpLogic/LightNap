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
    public class RolesServiceTests
    {
        // These will be initialized during TestInitialize.
#pragma warning disable CS8618
        private RoleManager<ApplicationRole> _roleManager;
        private UserManager<ApplicationUser> _userManager;
        private ApplicationDbContext _dbContext;
        private TestUserContext _userContext;
        private RolesService _rolesService;
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
            this._rolesService = new RolesService(this._userManager, this._dbContext, this._userContext);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            this._dbContext.Database.EnsureDeleted();
            this._dbContext.Dispose();
        }

        [TestMethod]
        public async Task AddUserToRoleAsync_UserAndRoleExist_AddsUserToRole()
        {
            // Arrange
            var userId = "test-user-id";
            var role = "test-role";
            await TestHelper.CreateTestUserAsync(this._userManager, userId);
            await TestHelper.CreateTestRoleAsync(this._roleManager, role);

            // Act
            await this._rolesService.AddUserToRoleAsync(role, userId);
        }

        [TestMethod]
        [ExpectedException(typeof(UserFriendlyApiException))]
        public async Task AddUserToRoleAsync_UserDoesNotExist_ThrowsError()
        {
            // Arrange
            var userId = "non-existent-user-id";
            var role = "test-role";

            // Act
            await this._rolesService.AddUserToRoleAsync(role, userId);
        }

        [TestMethod]
        public void GetRoles_ReturnsRoles()
        {
            // Arrange
            var allRoles = ApplicationRoles.All;

            // Act
            var roles = this._rolesService.GetRoles();

            // Assert
            Assert.AreEqual(allRoles.Count, roles.Count);

            for (int i = 0; i < allRoles.Count; i++)
            {
                Assert.AreEqual(allRoles[i].Name, roles[i].Name);
                Assert.AreEqual(allRoles[i].DisplayName, roles[i].DisplayName);
                Assert.AreEqual(allRoles[i].Description, roles[i].Description);
            }
        }

        [TestMethod]
        public async Task GetRolesForUserAsync_UserExists_ReturnsRoles()
        {
            // Arrange
            var userId = "test-user-id";
            List<string> roles = ["Admin", "User"];
            var user = await TestHelper.CreateTestUserAsync(this._userManager, userId);

            await TestHelper.CreateTestRoleAsync(this._roleManager, roles[0]);
            await TestHelper.CreateTestRoleAsync(this._roleManager, roles[1]);
            await this._userManager.AddToRolesAsync(user, roles);

            // Act
            var result = await this._rolesService.GetRolesForUserAsync(userId);

            // Assert
            Assert.AreEqual(2, result.Count);
        }

        [TestMethod]
        public async Task GetUsersInRoleAsync_RoleExists_ReturnsUsers()
        {
            // Arrange
            var role = "test-role";
            await TestHelper.CreateTestRoleAsync(this._roleManager, role);
            var user1 = await TestHelper.CreateTestUserAsync(this._userManager, "test-user-id-1");
            var user2 = await TestHelper.CreateTestUserAsync(this._userManager, "test-user-id-2");
            await this._userManager.AddToRoleAsync(user1, role);
            await this._userManager.AddToRoleAsync(user2, role);

            // Act
            var users = await this._rolesService.GetUsersInRoleAsync(role);

            // Assert
            Assert.AreEqual(2, users.Count);
        }

        [TestMethod]
        public async Task RemoveUserFromRoleAsync_UserAndRoleExist_RemovesUserFromRole()
        {
            // Arrange
            var userId = "test-user-id";
            var role = "test-role";
            await TestHelper.CreateTestRoleAsync(this._roleManager, role);
            var user = await TestHelper.CreateTestUserAsync(this._userManager, userId);
            await this._userManager.AddToRoleAsync(user, role);
            var roles = await this._userManager.GetRolesAsync(user);
            Assert.AreEqual(1, roles.Count);
            this._userContext.UserId = user.Id;

            // Act
            await this._rolesService.RemoveUserFromRoleAsync(role, userId);

            // Assert
            roles = await this._userManager.GetRolesAsync(user);
            Assert.AreEqual(0, roles.Count);
        }
    }
}
