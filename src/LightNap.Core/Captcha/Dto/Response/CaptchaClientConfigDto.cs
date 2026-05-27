using LightNap.Core.Configuration.Captcha;

namespace LightNap.Core.Captcha.Dto.Response
{
    /// <summary>
    /// Browser-safe subset of the CAPTCHA configuration. Exposes the active provider and the
    /// public site key (when applicable) so an SPA can render the matching widget. Never
    /// exposes any secret key.
    /// </summary>
    public sealed class CaptchaClientConfigDto
    {
        /// <summary>
        /// The active CAPTCHA provider. When <see cref="CaptchaProvider.None"/>, the SPA
        /// should skip the widget render and the <see cref="ValidateCaptchaAttribute"/>-style
        /// header injection altogether.
        /// </summary>
        public required CaptchaProvider Provider { get; init; }

        /// <summary>
        /// Public site key for the active provider, or null when <see cref="Provider"/> is
        /// <see cref="CaptchaProvider.None"/>.
        /// </summary>
        public string? SiteKey { get; init; }
    }
}
