using Microsoft.EntityFrameworkCore;

namespace LightNap.Core.Data.Entities
{
    /// <summary>
    /// Represents a user-specific setting stored in the database.
    /// </summary>
    [PrimaryKey(nameof(UserId), nameof(Key))]
    public class UserSetting
    {
        /// <summary>
        /// Gets or sets the unique identifier of the user associated with this setting.
        /// </summary>
        public required string UserId { get; set; }

        /// <summary>
        /// Gets or sets the user entity associated with this setting.
        /// </summary>
        public ApplicationUser? User { get; set; }

        /// <summary>
        /// Gets or sets the key identifying the setting.
        /// </summary>
        public required string Key { get; set; }

        /// <summary>
        /// Gets or sets the value of the setting.
        /// </summary>
        public required string Value { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the setting was created.
        /// </summary>
        public required DateTime CreatedDate { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the setting was last modified.
        /// </summary>
        public required DateTime LastModifiedDate { get; set; }
    }
}
