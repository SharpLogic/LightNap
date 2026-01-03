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
        public required string ClientId { get; init; }

        /// <summary>
        /// The client secret for the OAuth provider.
        /// </summary>
        [Required]
        public required string ClientSecret { get; init; }

        /// <summary>
        /// Gets the space-delimited list of OAuth scopes associated with the current context.
        /// </summary>
        public virtual string Scopes => string.Empty;
    }
}
