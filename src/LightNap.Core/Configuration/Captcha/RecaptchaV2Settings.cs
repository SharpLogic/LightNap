using System.ComponentModel.DataAnnotations;

namespace LightNap.Core.Configuration.Captcha
{
    /// <summary>
    /// Configuration for Google reCAPTCHA v2.
    /// </summary>
    public sealed class RecaptchaV2Settings
    {
        /// <summary>
        /// Public site key, exposed to the browser to render the widget.
        /// </summary>
        [Required]
        public required string SiteKey { get; set; }

        /// <summary>
        /// Server-side secret key used to verify tokens against Google.
        /// </summary>
        [Required]
        public required string SecretKey { get; set; }
    }
}
