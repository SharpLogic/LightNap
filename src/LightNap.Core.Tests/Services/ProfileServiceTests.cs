using LightNap.Core.Data;
using LightNap.Core.Data.Entities;
using LightNap.Core.Extensions;
using LightNap.Core.Interfaces;
using LightNap.Core.Profile.Dto.Request;
using LightNap.Core.Profile.Dto.Response;
using LightNap.Core.Profile.Services;
using LightNap.Core.Tests.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
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
            this._userManager = this._serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            await TestHelper.CreateTestUserAsync(this._userManager, _userId, _userName, _userEmail);

            this._userContext = new TestUserContext()
            {
                UserId = _userId
            };

            var userContextMock = new Mock<IUserContext>();
            userContextMock.Setup(uc => uc.GetUserId()).Returns(_userId);
            this._userContext = userContextMock.Object;

            this._profileService = new ProfileService(this._dbContext, this._userContext);
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
            var updateProfileDto = new UpdateProfileRequestDto
            {
                // Set properties to update
            };

            // Act
            await this._profileService.UpdateProfileAsync(updateProfileDto);
        }
    }
}
