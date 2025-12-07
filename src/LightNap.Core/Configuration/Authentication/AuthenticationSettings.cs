using System.ComponentModel.DataAnnotations;

namespace LightNap.Core.Configuration.Authentication
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
}
