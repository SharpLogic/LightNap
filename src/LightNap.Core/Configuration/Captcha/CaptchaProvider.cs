namespace LightNap.Core.Configuration.Captcha
{
    /// <summary>
    /// Supported CAPTCHA providers.
    /// </summary>
    public enum CaptchaProvider
    {
        /// <summary>
        /// No CAPTCHA enforcement. Backed by <c>NoOpCaptchaService</c> which always succeeds.
        /// Use for local development and test environments.
        /// </summary>
        None,

        /// <summary>
        /// Cloudflare Turnstile.
        /// </summary>
        Turnstile,

        /// <summary>
        /// Google reCAPTCHA v2 (checkbox / invisible). Binary success/failure.
        /// </summary>
        RecaptchaV2,

        /// <summary>
        /// Google reCAPTCHA v3. Score-based; configurable minimum score threshold.
        /// </summary>
        RecaptchaV3
    }
}
