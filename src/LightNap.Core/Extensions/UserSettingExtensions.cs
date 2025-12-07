using LightNap.Core.Configuration.UserSettings;
using LightNap.Core.Data.Entities;
using LightNap.Core.UserSettings.Dto.Response;

namespace LightNap.Core.Extensions
{
    public static class UserSettingExtensions
    {
        /// <summary>
        /// Converts a <see cref="UserSetting"/> entity to its corresponding <see cref="UserSettingDto"/> representation.
        /// </summary>
        /// <param name="userSetting">The <see cref="UserSetting"/> entity to convert.</param>
        /// <returns>A <see cref="UserSettingDto"/> containing the key, value, created date, and last modified date.</returns>
        public static UserSettingDto ToDto(this UserSetting userSetting)
        {
            return new UserSettingDto()
            {
                Key = userSetting.Key,
                Value = userSetting.Value,
                CreatedDate = userSetting.CreatedDate,
                LastModifiedDate = userSetting.LastModifiedDate,
            };
        }

        /// <summary>
        /// Converts a <see cref="UserSettingDefinition"/> entity to its corresponding <see cref="UserSettingDto"/> representation.
        /// </summary>
        /// <param name="userSetting">The <see cref="UserSettingDefinition"/> entity to convert.</param>
        /// <returns>A <see cref="UserSettingDto"/> containing the key, value, created date, and last modified date.</returns>
        internal static UserSettingDto ToDto(this UserSettingDefinition userSettingDefinition)
        {
            return new UserSettingDto()
            {
                Key = userSettingDefinition.Key,
                Value = userSettingDefinition.DefaultValue,
            };
        }
    }
}