namespace LightNap.Core.UserSettings.Dto.Response
{
    /// <summary>
    /// Data transfer object for user settings.
    /// </summary>
    public class UserSettingDto
    {
        /// <summary>
        /// Gets or sets the unique identifier key for the setting.
        /// </summary>
        public required string Key { get; set; }

        /// <summary>
        /// Gets or sets the value associated with the setting key.
        /// </summary>
        public required string Value { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the setting was created.
        /// </summary>
        public DateTime? CreatedDate { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the setting was last modified.
        /// </summary>
        public DateTime? LastModifiedDate { get; set; }
    }
}