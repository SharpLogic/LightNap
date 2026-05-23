using LightNap.Core.Configuration.Authentication;

namespace LightNap.Configuration.Authentication
{
    /// <summary>
    /// Settings for the OAuth providers supported by the current instance.
    /// </summary>
    public record SupportedOAuthProviderSettings
    {
        /// <summary>
        /// Google OAuth settings.
        /// </summary>
        public OAuthProviderSettings? Google { get; init; }

        /// <summary>
        /// Microsoft OAuth settings.
        /// </summary>
        public OAuthProviderSettings? Microsoft { get; init; }

        /// <summary>
        /// GitHub OAuth settings.
        /// </summary>
        public OAuthProviderSettings? GitHub { get; init; }
    }
}
