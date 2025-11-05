using LightNap.Core.Configuration;
using LightNap.Core.Data;
using LightNap.Core.Data.Entities;
using LightNap.Core.Extensions;
using LightNap.Core.Interfaces;
using LightNap.Core.UserSettings.Dto.Request;
using LightNap.Core.UserSettings.Dto.Response;
using LightNap.Core.UserSettings.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace LightNap.Core.UserSettings.Services
{
    /// <summary>
    /// Provides services for managing user settings, including retrieval, update, and cleanup operations.
    /// </summary>
    public class UserSettingsService(ApplicationDbContext db, IUserContext userContext, ILogger<UserSettingsService> logger) : IUserSettingsService
    {
        /// <summary>
        /// Retrieves a specific user setting for the given user and key.
        /// </summary>
        /// <typeparam name="T">The type to deserialize the setting value to.</typeparam>
        /// <param name="userId">The ID of the user whose setting is being retrieved.</param>
        /// <param name="key">The key identifying the setting.</param>
        /// <returns>The deserialized value of the user setting.</returns>
        public async Task<T> GetUserSettingAsync<T>(string userId, string key)
        {
            userContext.AssertAdministrator();

            var setting = await db.UserSettings
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.UserId == userId && s.Key == key);
            if (setting is not null)
            {
                var value = JsonSerializer.Deserialize<T>(setting.Value);
                if (value is not null)
                {
                    return value;
                }
                throw new Exception($"The setting '{key}' for user '{userId}' could not be deserialized to type '{typeof(T).FullName}'");
            }

            var definition = UserSettingsConfig.GetActiveSetting(key);
            if (string.IsNullOrEmpty(definition.DefaultValue))
            {
                return default!;
            }
            return JsonSerializer.Deserialize<T>(UserSettingsConfig.GetActiveSetting(key).DefaultValue)!;
        }

        /// <summary>
        /// Retrieves all user settings for the specified user, including defaults for missing settings.
        /// </summary>
        /// <param name="userId">The ID of the user whose settings are being retrieved.</param>
        /// <returns>A list of <see cref="UserSettingDto"/> representing the user's settings.</returns>
        public async Task<List<UserSettingDto>> GetUserSettingsAsync(string userId)
        {
            userContext.AssertAdministrator();

            var existingSettings = await db.UserSettings
                .AsNoTracking()
                .Where(s => s.UserId == userId)
                .Select(s => s.ToDto())
                .ToDictionaryAsync(key => key.Key);

            List<UserSettingDto> settings = [];
            foreach (var definition in UserSettingsConfig.AdminSettings)
            {
                if (existingSettings.TryGetValue(definition.Key, out var existingSetting))
                {
                    settings.Add(existingSetting);
                }
                else
                {
                    settings.Add(definition.ToDto());
                }
            }

            return settings;
        }

        /// <summary>
        /// Retrieves all settings for the currently authenticated user, including defaults for missing settings.
        /// </summary>
        /// <returns>A list of <see cref="UserSettingDto"/> representing the current user's settings.</returns>
        public async Task<List<UserSettingDto>> GetMySettingsAsync()
        {
            userContext.AssertAuthenticated();

            var userId = userContext.GetUserId();

            var existingSettings = await db.UserSettings
                .AsNoTracking()
                .Where(s => s.UserId == userId)
                .Select(s => s.ToDto())
                .ToDictionaryAsync(key => key.Key);

            List<UserSettingDto> settings = [];
            foreach (var definition in UserSettingsConfig.UserSettings)
            {
                if (existingSettings.TryGetValue(definition.Key, out var existingSetting))
                {
                    settings.Add(existingSetting);
                }
                else
                {
                    settings.Add(definition.ToDto());
                }
            }

            return settings;
        }

        /// <summary>
        /// Sets or updates a user setting for the specified user.
        /// </summary>
        /// <param name="userId">The ID of the user whose setting is being updated.</param>
        /// <param name="setSettingDto">The DTO containing the key and value to set.</param>
        /// <returns>The updated <see cref="UserSettingDto"/>.</returns>
        public async Task<UserSettingDto> SetUserSettingAsync(string userId, SetUserSettingRequestDto setSettingDto)
        {
            userContext.AssertAdministrator();

            return await this.UpdateSettingInternalAsync(userId, UserSettingsConfig.GetActiveSetting(setSettingDto.Key), setSettingDto.Value);
        }

        /// <summary>
        /// Sets or updates a setting for the currently authenticated user.
        /// </summary>
        /// <param name="setSettingDto">The DTO containing the key and value to set.</param>
        /// <returns>The updated <see cref="UserSettingDto"/>.</returns>
        /// <exception cref="Exception">Thrown if the setting is not readable or writeable by the user.</exception>
        public async Task<UserSettingDto> SetMySettingAsync(SetUserSettingRequestDto setSettingDto)
        {
            userContext.AssertAuthenticated();

            var definition = UserSettingsConfig.GetActiveSetting(setSettingDto.Key);
            if (!definition.IsUserReadable)
            {
                throw new Exception($"The setting '{setSettingDto.Key}' is not supported or not accessible to users");
            }
            if (!definition.IsUserWriteable)
            {
                throw new Exception($"The setting '{setSettingDto.Key}' is not user-editable");
            }

            return await this.UpdateSettingInternalAsync(userContext.GetUserId(), definition, setSettingDto.Value);
        }

        /// <summary>
        /// Updates or creates a user setting in the database.
        /// </summary>
        /// <param name="user">The ID of the user whose setting is being updated.</param>
        /// <param name="definition">The definition of the setting.</param>
        /// <param name="value">The value to set.</param>
        /// <returns>The updated <see cref="UserSettingDto"/>.</returns>
        private async Task<UserSettingDto> UpdateSettingInternalAsync(string user, UserSettingDefinition definition, string value)
        {
            var setting = await db.UserSettings.FirstOrDefaultAsync(s => s.UserId == user && s.Key == definition.Key);
            if (setting == null)
            {
                setting = new UserSetting
                {
                    UserId = user,
                    Key = definition.Key,
                    Value = value,
                    CreatedDate = DateTime.UtcNow,
                    LastModifiedDate = DateTime.UtcNow
                };
                db.UserSettings.Add(setting);
            }
            else
            {
                setting.Value = value;
                setting.LastModifiedDate = DateTime.UtcNow;
                db.UserSettings.Update(setting);
            }
            await db.SaveChangesAsync();
            return setting.ToDto();
        }

        /// <summary>
        /// Removes user settings from the database that are no longer defined in the configuration.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task PurgeUnusedUserSettingsAsync()
        {
            userContext.AssertAdministrator();

            var keysInDb = await db.UserSettings
                .AsNoTracking()
                .Select(s => s.Key)
                .Distinct()
                .ToListAsync();

            int batchSize = 100;

            foreach (var key in keysInDb.Where(key => !UserSettingsConfig.AllSettingsLookup.ContainsKey(key)))
            {
                logger.LogInformation("Purging user settings with key '{key}'", key);
                int deletedRecords = 0;

                while (true)
                {
                    var settingsToRemove = await db.UserSettings
                        .Where(s => s.Key == key)
                        .Take(batchSize)
                        .ToListAsync();
                    if (settingsToRemove.Count == 0)
                    {
                        break;
                    }
                    db.UserSettings.RemoveRange(settingsToRemove);
                    deletedRecords += await db.SaveChangesAsync();
                }

                logger.LogInformation("Purged {count} user settings with key '{key}'", deletedRecords, key);
            }
        }
    }
}