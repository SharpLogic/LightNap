using Microsoft.AspNetCore.Identity;

namespace LightNap.Core.Data.Entities
{
    /// <summary>
    /// Represents an application user with additional properties.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="ApplicationUser"/> class.
    /// </remarks>
    public class ApplicationUser : IdentityUser
    {
        /// <summary>
        /// The date when the user was created.
        /// </summary>
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// The date when the user was last modified.
        /// </summary>
        public DateTime LastModifiedDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Whether the user has opted into receiving marketing email.
        /// </summary>
        public bool MarketingOptIn { get; set; }

        /// <summary>
        /// When the user most recently opted into marketing email. Null when no opt-in has been recorded.
        /// </summary>
        public DateTime? MarketingOptInAt { get; set; }

        /// <summary>
        /// The notifications associated with the user.
        /// </summary>
        public ICollection<Notification>? Notifications { get; set; }

        /// <summary>
        /// The refresh tokens associated with the user.
        /// </summary>
        public ICollection<RefreshToken>? RefreshTokens { get; set; }

        /// <summary>
        /// The settings associated with the user.
        /// </summary>
        public ICollection<UserSetting>? UserSettings { get; set; }
    }
}
