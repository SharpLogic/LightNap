using LightNap.Core.Configuration.Captcha;
using LightNap.Core.Data;
using LightNap.Core.Data.Entities;
using LightNap.Core.Extensions;
using LightNap.Core.Public.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace LightNap.Core.Tests.Services
{
    [TestClass]
    public class PublicServiceTests
    {
        // These will be initialized during TestInitialize.
#pragma warning disable CS8618
        private ApplicationDbContext _dbContext;
        // Remove when using this member.
#pragma warning disable IDE0052
        private PublicService _publicService;
#pragma warning restore IDE0052
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

            var serviceProvider = services.BuildServiceProvider();
            this._dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();

            this._publicService = new PublicService(Options.Create(new CaptchaSettings()));
        }

        [TestCleanup]
        public void TestCleanup()
        {
            this._dbContext.Database.EnsureDeleted();
            this._dbContext.Dispose();
        }

        private static PublicService CreateWithSettings(CaptchaSettings settings)
            => new(Options.Create(settings));

        [TestMethod]
        public void GetCaptchaConfig_ProviderNone_ReturnsNoneAndNullSiteKey()
        {
            var service = CreateWithSettings(new CaptchaSettings { Provider = CaptchaProvider.None });

            var config = service.GetCaptchaConfig();

            Assert.AreEqual(CaptchaProvider.None, config.Provider);
            Assert.IsNull(config.SiteKey);
        }

        [TestMethod]
        public void GetCaptchaConfig_Turnstile_ReturnsTurnstileSiteKey()
        {
            var service = CreateWithSettings(new CaptchaSettings
            {
                Provider = CaptchaProvider.Turnstile,
                Turnstile = new TurnstileSettings { SiteKey = "turnstile-public", SecretKey = "turnstile-secret" }
            });

            var config = service.GetCaptchaConfig();

            Assert.AreEqual(CaptchaProvider.Turnstile, config.Provider);
            Assert.AreEqual("turnstile-public", config.SiteKey);
        }

        [TestMethod]
        public void GetCaptchaConfig_RecaptchaV2_ReturnsRecaptchaV2SiteKey()
        {
            var service = CreateWithSettings(new CaptchaSettings
            {
                Provider = CaptchaProvider.RecaptchaV2,
                RecaptchaV2 = new RecaptchaV2Settings { SiteKey = "recaptcha-v2-public", SecretKey = "recaptcha-v2-secret" }
            });

            var config = service.GetCaptchaConfig();

            Assert.AreEqual(CaptchaProvider.RecaptchaV2, config.Provider);
            Assert.AreEqual("recaptcha-v2-public", config.SiteKey);
        }

        [TestMethod]
        public void GetCaptchaConfig_RecaptchaV3_ReturnsRecaptchaV3SiteKey()
        {
            var service = CreateWithSettings(new CaptchaSettings
            {
                Provider = CaptchaProvider.RecaptchaV3,
                RecaptchaV3 = new RecaptchaV3Settings { SiteKey = "recaptcha-v3-public", SecretKey = "recaptcha-v3-secret", MinScore = 0.7 }
            });

            var config = service.GetCaptchaConfig();

            Assert.AreEqual(CaptchaProvider.RecaptchaV3, config.Provider);
            Assert.AreEqual("recaptcha-v3-public", config.SiteKey);
        }

        [TestMethod]
        public void GetCaptchaConfig_NeverExposesSecretKey()
        {
            var service = CreateWithSettings(new CaptchaSettings
            {
                Provider = CaptchaProvider.Turnstile,
                Turnstile = new TurnstileSettings { SiteKey = "turnstile-public", SecretKey = "DO-NOT-LEAK" }
            });

            var config = service.GetCaptchaConfig();

            // CaptchaClientConfigDto has no SecretKey property by design; this assertion
            // pins the public surface so any future addition of a secret-shaped field
            // forces an explicit decision.
            Assert.AreEqual(2, typeof(LightNap.Core.Captcha.Dto.Response.CaptchaClientConfigDto).GetProperties().Length,
                "CaptchaClientConfigDto must only expose Provider + SiteKey.");
        }
    }
}
