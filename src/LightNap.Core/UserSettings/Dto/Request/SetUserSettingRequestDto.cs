namespace LightNap.Core.UserSettings.Dto.Request;

/// <summary>
/// Data transfer object for setting a user setting.
/// </summary>
/// <param name="Key">The unique identifier for the user setting.</param>
/// <param name="Value">The value to assign to the user setting.</param>
public record SetUserSettingRequestDto(string Key, string Value) { }