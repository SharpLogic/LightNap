using LightNap.Core.Data;
using LightNap.Core.Data.Entities;
using LightNap.Core.Extensions;
using LightNap.Core.Profile.Dto.Request;
using LightNap.Core.Profile.Dto.Response;
using LightNap.Core.Profile.Services;
using LightNap.Core.Tests.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace LightNap.Core.Tests.Services
{
    [TestClass]
    public class ProfileServiceTests
    {
        const string _userId = "test-user-id";
        const string _userEmail = "user@test.com";
        const string _userName = "UserName";

        private UserManager<ApplicationUser> _userManager = null!;
        private ApplicationDbContext _dbContext = null!;
        private TestUserContext _userContext = null!;
        private ProfileService _profileService = null!;
        private IServiceProvider _serviceProvider = null!;

        [TestInitialize]
        public async Task TestInitialize()
        {
            var services = new ServiceCollection();
            services.AddLogging()
                .AddLightNapInMemoryDatabase($"TestDb_{Guid.NewGuid()}")
                .AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            this._serviceProvider = services.BuildServiceProvider();
            this._dbContext = this._serviceProvider.GetRequiredService<ApplicationDbContext>();
            this._userManager = this._serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            await TestHelper.CreateTestUserAsync(this._userManager, _userId, _userName, _userEmail);

            this._userContext = new TestUserContext();
            this._userContext.LogIn(ProfileServiceTests._userId);

            this._profileService = new ProfileService(this._dbContext, this._userContext);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            this._dbContext.Database.EnsureDeleted();
            this._dbContext.Dispose();
        }

        [TestMethod]
        public async Task GetProfileAsync_ShouldReturnUserProfile()
        {
            // Arrange
            var expectedProfile = new ProfileDto
            {
                Id = _userId,
                Email = _userEmail,
                UserName = _userName
            };

            // Act
            var profile = await this._profileService.GetMyProfileAsync();

            // Assert
            Assert.AreEqual(expectedProfile.Id, profile.Id);
            Assert.AreEqual(expectedProfile.Email, profile.Email);
            Assert.AreEqual(expectedProfile.UserName, profile.UserName);
        }

        [TestMethod]
        public async Task UpdateProfileAsync_ShouldUpdateUserProfile()
        {
            // Arrange
            var updateProfileDto = new UpdateProfileRequestDto
            {
                // Set properties to update
            };

            // Act
            var result = await this._profileService.UpdateProfileAsync(updateProfileDto);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(_userId, result.Id);
        }

        [TestMethod]
        public async Task UpdateProfileAsync_ShouldReturnUpdatedProfile()
        {
            // Arrange
            var updateProfileDto = new UpdateProfileRequestDto
            {
                // Set properties to update
            };

            // Act
            var updatedProfile = await this._profileService.UpdateProfileAsync(updateProfileDto);
            var retrievedProfile = await this._profileService.GetMyProfileAsync();

            // Assert
            Assert.AreEqual(updatedProfile.Id, retrievedProfile.Id);
            Assert.AreEqual(updatedProfile.Email, retrievedProfile.Email);
            Assert.AreEqual(updatedProfile.UserName, retrievedProfile.UserName);
        }
    }
}
