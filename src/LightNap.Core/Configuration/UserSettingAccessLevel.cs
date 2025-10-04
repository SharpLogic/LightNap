namespace LightNap.Core.Configuration
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
        UserReadWrite,
        UserRead,
        Admin,
    }
}
