namespace LightNap.Core.Configuration
{
    /// <summary>
    /// Represents a definition for a user setting, including its key, value, access level, and activation status.
    /// </summary>
    /// <param name="key">The setting key.</param>
    /// <param name="defaultJson">The default value for the setting.</param>
    /// <param name="accessLevel">The access level required for the user setting.</param>
    /// <param name="isActive">
    /// True if the setting is currently active. If false the setting is generally ignored by the system. Set this to false if
    /// you have existing settings you want to phase out but not automatically purge from the store (which happens if you remove).
    /// </param>
    public record UserSettingDefinition(
        string Key,
        string DefaultValue,
        UserSettingAccessLevel AccessLevel,
        bool IsActive
    )
    {
        /// <summary>
        /// True if the user can read this setting.
        /// </summary>
        public bool IsUserReadable => this.AccessLevel <= UserSettingAccessLevel.UserRead;

        /// <summary>
        /// True if the user can edit this setting. Read support is implied.
        /// </summary>
        public bool IsUserWriteable => this.AccessLevel <= UserSettingAccessLevel.UserReadWrite;
    }
}
