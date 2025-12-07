using LightNap.Core.Data;
using LightNap.Core.Data.Entities;
using LightNap.Core.Identity.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using LightNap.Core.Extensions;
using LightNap.Core.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;
using LightNap.Core.Configuration.Authentication;
using LightNap.Core.Configuration.Authorization;

namespace LightNap.Core.Tests.Services
{
    [TestClass]
    public class TokenServiceTests
    {
        private IServiceProvider _serviceProvider = null!;
        private UserManager<ApplicationUser> _userManager = null!;
        private RoleManager<ApplicationRole> _roleManager = null!;

        [TestInitialize]
        public void Initialize()
        {
            var services = new ServiceCollection();
            services.AddLogging()
                .AddLightNapInMemoryDatabase($"TokenServiceTestDb_{Guid.NewGuid()}")
                .AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            var inMemorySettings = new Dictionary<string, string?>
            {
                { "Jwt:Key", "01234567890123456789012345678901" }, // 32 chars
                { "Jwt:Issuer", "test-issuer" },
                { "Jwt:Audience", "test-audience" },
                { "Jwt:ExpirationMinutes", "120" }
            };
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(inMemorySettings).Build();
            services.AddSingleton<IConfiguration>(configuration);
            services.Configure<JwtSettings>(configuration.GetSection("Jwt"));

            services.AddScoped<ITokenService, TokenService>();

            this._serviceProvider = services.BuildServiceProvider();
            this._userManager = this._serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            this._roleManager = this._serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
        }

        [TestMethod]
        public async Task GenerateAccessToken_IncludesStandardClaimsAndRoles()
        {
            // Arrange
            var role = ApplicationRoles.Administrator;
            await this._roleManager.CreateAsync(role);

            var user = new ApplicationUser { UserName = "user1", Email = "user1@test.com" };
            var result = await this._userManager.CreateAsync(user, "Password123!");
            Assert.IsTrue(result.Succeeded, "User creation failed");

            await this._userManager.AddToRoleAsync(user, role.Name!);
            await this._roleManager.AddClaimAsync(role, new Claim("custom-role-claim", "role-value"));
            await this._userManager.AddClaimAsync(user, new Claim("custom-user-claim", "user-value"));

            var tokenService = this._serviceProvider.GetRequiredService<ITokenService>();

            // Act
            var tokenString = await tokenService.GenerateAccessTokenAsync(user);

            // Assert
            Assert.IsFalse(string.IsNullOrEmpty(tokenString));

            var jsonHandler = new Microsoft.IdentityModel.JsonWebTokens.JsonWebTokenHandler();
            var jwt = jsonHandler.ReadJsonWebToken(tokenString);

            var emailClaim = jwt.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var nameClaim = jwt.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            var roleClaims = jwt.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList();
            var customRole = jwt.Claims.FirstOrDefault(c => c.Type == "custom-role-claim")?.Value;
            var customUser = jwt.Claims.FirstOrDefault(c => c.Type == "custom-user-claim")?.Value;

            Assert.AreEqual(user.Email, emailClaim);
            Assert.AreEqual(user.UserName, nameClaim);
            Assert.Contains(ApplicationRoles.Administrator.Name, roleClaims);
            Assert.AreEqual("role-value", customRole);
            Assert.AreEqual("user-value", customUser);

            // The token expiration should be close to configured expiration
            var expirationMinutes = tokenService.ExpirationMinutes;
            var expectedExpiry = DateTime.UtcNow.AddMinutes(expirationMinutes);
            Assert.IsLessThan(1, (jwt.ValidTo - expectedExpiry).TotalMinutes, "Expiration is not within expected range");
        }

        [TestMethod]
        public void GenerateRefreshToken_ReturnsGuid()
        {
            var tokenService = this._serviceProvider.GetRequiredService<ITokenService>();
            var refreshToken = tokenService.GenerateRefreshToken();
            Assert.IsFalse(string.IsNullOrEmpty(refreshToken));
            Assert.IsTrue(Guid.TryParse(refreshToken, out _));
        }

        [TestMethod]
        public void Constructor_InvalidExpiration_ThrowsArgumentException()
        {
            var services = new ServiceCollection();
            services.AddLogging()
                .AddLightNapInMemoryDatabase($"TokenServiceTestDbInvalidExpire_{Guid.NewGuid()}")
                .AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            var inMemorySettingsInvalid = new Dictionary<string, string?>
            {
                { "Jwt:Key", "01234567890123456789012345678901" },
                { "Jwt:Issuer", "test-issuer" },
                { "Jwt:Audience", "test-audience" },
                { "Jwt:ExpirationMinutes", "not-a-number" }
            };
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(inMemorySettingsInvalid).Build();
            services.AddSingleton<IConfiguration>(configuration);
            services.Configure<JwtSettings>(configuration.GetSection("Jwt"));

            var serviceProvider = services.BuildServiceProvider();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
            var jwtSettings = serviceProvider.GetRequiredService<IOptions<JwtSettings>>();

            Assert.ThrowsExactly<InvalidOperationException>(() =>
            {
                _ = new TokenService(jwtSettings, userManager, roleManager);
            });
        }

        [TestMethod]
        public void Constructor_ExpirationTooSHort_ThrowsArgumentException()
        {
            var services = new ServiceCollection();
            services.AddLogging()
                .AddLightNapInMemoryDatabase($"TokenServiceTestDbInvalidExpire_{Guid.NewGuid()}")
                .AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            var inMemorySettingsInvalid = new Dictionary<string, string?>
            {
                { "Jwt:Key", "01234567890123456789012345678901" },
                { "Jwt:Issuer", "test-issuer" },
                { "Jwt:Audience", "test-audience" },
                { "Jwt:ExpirationMinutes", "4" }
            };
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(inMemorySettingsInvalid).Build();
            services.AddSingleton<IConfiguration>(configuration);
            services.Configure<JwtSettings>(configuration.GetSection("Jwt"));

            var serviceProvider = services.BuildServiceProvider();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
            var jwtSettings = serviceProvider.GetRequiredService<IOptions<JwtSettings>>();

            Assert.ThrowsExactly<ValidationException>(() =>
            {
                _ = new TokenService(jwtSettings, userManager, roleManager);
            });
        }
    }
}
