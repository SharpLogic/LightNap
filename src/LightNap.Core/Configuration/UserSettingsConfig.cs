using System.Collections.ObjectModel;

namespace LightNap.Core.Configuration
{
    /// <summary>
    /// Provides configuration and lookup for all user settings in the system.
    /// </summary>
    internal static class UserSettingsConfig
    {
        /// <summary>
        /// Array containing all user setting definitions.
        /// Note: Removing a setting from this array may result in it being purged from the database. 
        /// Mark it as inactive instead if you want to retain existing values.
        /// </summary>
        private static readonly UserSettingDefinition[] _allSettings =
            [
                new UserSettingDefinition(
                    Constants.UserSettingKeys.BrowserSettings,
                    "",
                    UserSettingAccessLevel.UserReadWrite,
                    true),
            ];

        /// <summary>
        /// Gets a read-only dictionary mapping setting keys to their definitions.
        /// </summary>
        public static ReadOnlyDictionary<string, UserSettingDefinition> AllSettingsLookup { get; } =
            new ReadOnlyDictionary<string, UserSettingDefinition>(
                UserSettingsConfig._allSettings.ToDictionary(us => us.Key, us => us)
            );

        /// <summary>
        /// Gets a read-only collection of all settings available to administrators.
        /// </summary>
        public static ReadOnlyCollection<UserSettingDefinition> AdminSettings { get; } =
            UserSettingsConfig._allSettings.ToList().AsReadOnly();

        /// <summary>
        /// Gets a read-only collection of all settings available to users with read/write access.
        /// </summary>
        public static ReadOnlyCollection<UserSettingDefinition> UserSettings { get; } =
            UserSettingsConfig._allSettings.Where(us => us.AccessLevel <= UserSettingAccessLevel.UserReadWrite).ToList().AsReadOnly();

        /// <summary>
        /// Retrieves the active setting definition for the specified key.
        /// Throws an exception if the setting is not active.
        /// </summary>
        /// <param name="key">The key of the setting to retrieve.</param>
        /// <returns>The <see cref="UserSettingDefinition"/> for the specified key.</returns>
        /// <exception cref="Exception">Thrown if the setting is no longer active.</exception>
        public static UserSettingDefinition GetActiveSetting(string key)
        {
            var setting = UserSettingsConfig.AllSettingsLookup[key];
            if (!setting.IsActive) { throw new Exception($"Setting '{key}' is no longer active"); }
            return setting;
        }
    }
}
