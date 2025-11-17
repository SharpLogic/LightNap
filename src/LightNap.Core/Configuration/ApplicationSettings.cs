namespace LightNap.Core.Configuration
{
    /// <summary>
    /// Represents the site settings for the web API.
    /// </summary>
    public class ApplicationSettings
    {
        /// <summary>
        /// True to automatically apply Entity Framework migrations on startup.
        /// </summary>
        public bool AutomaticallyApplyEfMigrations { get; set; }

        /// <summary>
        /// How long a device can stay logged in without refreshing an access token. In other words, how far out we push refresh token expirations.
        /// </summary>
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
        /// The root URL for emails sent by the site.
        /// </summary>
        public required string SiteUrlRootForEmails { get; set; }
    }
}
