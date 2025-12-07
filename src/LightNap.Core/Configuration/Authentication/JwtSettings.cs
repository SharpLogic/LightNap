using System.ComponentModel.DataAnnotations;

namespace LightNap.Core.Configuration.Authentication
{
    /// <summary>
    /// Represents the JWT settings for the web API.
    /// </summary>
    public class JwtSettings
    {
        /// <summary>
        /// Gets or sets the JWT key. This must be a string between 32 and 100 characters.
        /// </summary>
        [Required(ErrorMessage = "JWT key is required")]
        [StringLength(100, MinimumLength = 32)]
        public required string Key { get; set; }

        /// <summary>
        /// Gets or sets the JWT issuer URL.
        /// </summary>
        [Required(ErrorMessage = "JWT issuer is required")]
        [MinLength(1)]
        public required string Issuer { get; set; }

        /// <summary>
        /// Gets or sets the JWT audience URL.
        /// </summary>
        [Required(ErrorMessage = "JWT audience is required")]
        [MinLength(1)]
        public required string Audience { get; set; }

        /// <summary>
        /// Gets or sets the expiration time in minutes for the JWT token. Must be between 1 and 1440.
        /// </summary>
        [Range(5, 1440)]
        public int ExpirationMinutes { get; set; }
    }
}
