using System.ComponentModel.DataAnnotations;

namespace LightNap.Core.Configuration
{
    /// <summary>
    /// Represents the site settings for the web API.
    /// </summary>
    public record AuthenticationSettings
    {
        /// <summary>
        /// How long a device can stay logged in without refreshing an access token. In other words, how far out we push refresh token expirations.
        /// </summary>
        [Range(1, 365)]
        public int LogOutInactiveDeviceDays { get; set; }

        /// <summary>
        /// True to require two-factor authentication for new users. This does not affect existing users.
        /// </summary>
        public bool RequireTwoFactorForNewUsers { get; set; }

        /// <summary>
        /// True to require email verification before a user can log in.
        /// </summary>
        public bool RequireEmailVerification { get; set; }

        /// <summary>
        /// OAuth provider settings.
        /// </summary>
        public OAuthSettings? OAuth { get; set; }

        /// <summary>
        /// Windows authentication settings.
        /// </summary>
        public WindowsAuthSettings? WindowsAuth { get; set; }
    }

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

    /// <summary>
    /// Settings for Windows authentication.
    /// </summary>
    public record WindowsAuthSettings
    {
        /// <summary>
        /// Whether Windows authentication is enabled.
        /// </summary>
        public bool Enabled { get; set; }
    }
}
