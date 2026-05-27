using LightNap.Core.Captcha.Dto.Response;
using LightNap.Core.Configuration.Captcha;
using LightNap.Core.Public.Interfaces;
using Microsoft.Extensions.Options;

namespace LightNap.Core.Public.Services
{
    /// <summary>
    /// Service for accessing public data.
    /// </summary>
    public class PublicService(IOptions<CaptchaSettings> captchaOptions) : IPublicService
    {
        /// <inheritdoc />
        public CaptchaClientConfigDto GetCaptchaConfig()
        {
            var settings = captchaOptions.Value;
            return new CaptchaClientConfigDto
            {
                Provider = settings.Provider,
                SiteKey = settings.Provider switch
                {
                    CaptchaProvider.Turnstile => settings.Turnstile?.SiteKey,
                    CaptchaProvider.RecaptchaV2 => settings.RecaptchaV2?.SiteKey,
                    CaptchaProvider.RecaptchaV3 => settings.RecaptchaV3?.SiteKey,
                    _ => null
                }
            };
        }
    }
}
