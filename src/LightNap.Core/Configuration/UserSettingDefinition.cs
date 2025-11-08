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
    internal class UserSettingDefinition(string key, string defaultJson, UserSettingAccessLevel accessLevel, bool isActive)
    {
        /// <summary>
        /// Gets the unique key identifying the user setting.
        /// </summary>
        public readonly string Key = key;

        /// <summary>
        /// Gets the value associated with the user setting.
        /// </summary>
        public readonly string DefaultValue = defaultJson;

        /// <summary>
        /// Gets the access level required for the user setting.
        /// </summary>
        public readonly UserSettingAccessLevel AccessLevel = accessLevel;

        /// <summary>
        /// Gets a value indicating whether the user setting is currently active. Inactive settings are generally ignored,
        /// but are not purged from the store.
        /// </summary>
        public readonly bool IsActive = isActive;

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
