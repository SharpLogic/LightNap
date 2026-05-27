using System.ComponentModel.DataAnnotations;

namespace LightNap.Core.Configuration.Captcha
{
    /// <summary>
    /// Configuration for Google reCAPTCHA v3.
    /// </summary>
    public sealed class RecaptchaV3Settings
    {
        /// <summary>
        /// Public site key, exposed to the browser to fetch tokens.
        /// </summary>
        [Required]
        public required string SiteKey { get; set; }

        /// <summary>
        /// Server-side secret key used to verify tokens against Google.
        /// </summary>
        [Required]
        public required string SecretKey { get; set; }

        /// <summary>
        /// Minimum score (0.0 to 1.0) required for a request to be considered human.
        /// Tokens scoring below this threshold fail validation. Default 0.5.
        /// </summary>
        public double MinScore { get; set; } = 0.5;
    }
}
