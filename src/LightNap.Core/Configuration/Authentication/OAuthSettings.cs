using System.ComponentModel.DataAnnotations;

namespace LightNap.Core.Configuration.Authentication
{
    /// <summary>
    /// Settings for OAuth providers.
    /// </summary>
    public record OAuthSettings
    {
        /// <summary>
        /// Google OAuth settings.
        /// </summary>
        public OAuthProviderSettings? Google { get; set; }

        /// <summary>
        /// Microsoft OAuth settings.
        /// </summary>
        public OAuthProviderSettings? Microsoft { get; set; }

        /// <summary>
        /// GitHub OAuth settings.
        /// </summary>
        public OAuthProviderSettings? GitHub { get; set; }
    }
}
