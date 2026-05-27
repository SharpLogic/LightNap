namespace LightNap.Core.Configuration.Captcha
{
    /// <summary>
    /// CAPTCHA configuration. The <see cref="Provider"/> selects which implementation is registered;
    /// the corresponding provider-specific settings sub-section must be present for any provider
    /// other than <see cref="CaptchaProvider.None"/>.
    /// </summary>
    public sealed class CaptchaSettings
    {
        /// <summary>
        /// The active CAPTCHA provider. Defaults to <see cref="CaptchaProvider.None"/>
        /// (NoOp, always succeeds).
        /// </summary>
        public CaptchaProvider Provider { get; set; } = CaptchaProvider.None;

        /// <summary>
        /// When <c>true</c>, requests that fail because the CAPTCHA provider is unreachable are
        /// rejected (fail-closed). When <c>false</c>, those requests are accepted (fail-open,
        /// prioritizes availability over strict bot mitigation).
        /// </summary>
        public bool RejectOnProviderError { get; set; } = true;

        /// <summary>Settings for the Cloudflare Turnstile provider.</summary>
        public TurnstileSettings? Turnstile { get; set; }

        /// <summary>Settings for the Google reCAPTCHA v2 provider.</summary>
        public RecaptchaV2Settings? RecaptchaV2 { get; set; }

        /// <summary>Settings for the Google reCAPTCHA v3 provider.</summary>
        public RecaptchaV3Settings? RecaptchaV3 { get; set; }
    }
}
