using LightNap.Core.UserSettings.Dto.Request;
using LightNap.Core.UserSettings.Dto.Response;

namespace LightNap.Core.UserSettings.Interfaces
{
    /// <summary>
    /// Service interface for managing user settings.
    /// </summary>
    public interface IUserSettingsService
    {
        /// <summary>
        /// Retrieves a specific user setting by key for the given user.
        /// </summary>
        /// <typeparam name="T">Type to deserialize the setting value to.</typeparam>
        /// <param name="userId">The user's unique identifier.</param>
        /// <param name="key">The key of the setting.</param>
        /// <returns>The setting value as type T.</returns>
        Task<T> GetUserSettingAsync<T>(string userId, string key);

        /// <summary>
        /// Retrieves all settings for the specified user.
        /// </summary>
        /// <param name="userId">The user's unique identifier.</param>
        /// <returns>List of user settings.</returns>
        Task<List<UserSettingDto>> GetUserSettingsAsync(string userId);

        /// <summary>
        /// Retrieves all settings for the current user.
        /// </summary>
        /// <returns>List of user settings.</returns>
        Task<List<UserSettingDto>> GetMySettingsAsync();

        /// <summary>
        /// Sets or updates a specific setting for the given user.
        /// </summary>
        /// <param name="userId">The user's unique identifier.</param>
        /// <param name="setSettingDto">DTO containing key and value to set.</param>
        /// <returns>The updated user setting.</returns>
        Task<UserSettingDto> SetUserSettingAsync(string userId, SetUserSettingRequestDto setSettingDto);

        /// <summary>
        /// Sets or updates a specific setting for the current user.
        /// </summary>
        /// <param name="setSettingDto">DTO containing key and value to set.</param>
        /// <returns>The updated user setting.</returns>
        Task<UserSettingDto> SetMySettingAsync(SetUserSettingRequestDto setSettingDto);

        /// <summary>
        /// Removes unused or obsolete user settings.
        /// </summary>
        /// <returns>A task representing the purge operation.</returns>
        Task PurgeUnusedUserSettingsAsync();
    }
}