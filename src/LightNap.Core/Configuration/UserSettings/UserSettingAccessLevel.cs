namespace LightNap.Core.Configuration.UserSettings
{
    /// <summary>
    /// Specifies the access levels available for user settings.
    /// </summary>
    /// <remarks>
    /// This enumeration defines the different levels of access that can be granted to user settings.
    /// Use these values to control whether a user can read, modify, or manage settings. The order here is important
    /// as they are used in comparisons to determine access rights. It should always be ordered from least to most privileged.
    /// </remarks>
    public enum UserSettingAccessLevel
    {
        /// <summary>
        /// Specifies read and write access permissions for a user.
        /// </summary>
        UserReadWrite,

        /// <summary>
        /// User can only read settings without the ability to modify them.
        /// </summary>  
        UserRead,

        /// <summary>
        /// Administrator level access with full control over all settings.
        /// </summary>
        Admin,
    }
}
