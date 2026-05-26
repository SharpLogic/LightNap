using System.ComponentModel.DataAnnotations;

namespace LightNap.Core.Configuration.Captcha
{
    /// <summary>
    /// Configuration for Cloudflare Turnstile.
    /// </summary>
    public sealed class TurnstileSettings
    {
        /// <summary>
        /// Public site key, exposed to the browser to render the widget.
        /// </summary>
        [Required]
        public required string SiteKey { get; set; }

        /// <summary>
        /// Server-side secret key used to verify tokens against Cloudflare.
        /// </summary>
        [Required]
        public required string SecretKey { get; set; }
    }
}
