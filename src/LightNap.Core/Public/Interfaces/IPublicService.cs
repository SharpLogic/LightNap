using LightNap.Core.Captcha.Dto.Response;

namespace LightNap.Core.Public.Interfaces
{
    /// <summary>
    /// Service for public data access.
    /// </summary>
    public interface IPublicService
    {
        /// <summary>
        /// Returns the browser-safe CAPTCHA configuration: the active provider and (when
        /// applicable) the public site key. The SPA uses this to render the matching widget
        /// before submitting protected requests.
        /// </summary>
        /// <returns>The current CAPTCHA client configuration.</returns>
        CaptchaClientConfigDto GetCaptchaConfig();
    }
}
