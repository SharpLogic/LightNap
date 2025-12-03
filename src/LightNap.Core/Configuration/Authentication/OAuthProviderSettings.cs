using System.ComponentModel.DataAnnotations;

namespace LightNap.Core.Configuration.Authentication
{
    /// <summary>
    /// Settings for an individual OAuth provider.
    /// </summary>
    public record OAuthProviderSettings
    {
        /// <summary>
        /// The client ID for the OAuth provider.
        /// </summary>
        [Required]
        public required string ClientId { get; set; }

        /// <summary>
        /// The client secret for the OAuth provider.
        /// </summary>
        [Required]
        public required string ClientSecret { get; set; }
    }
}
