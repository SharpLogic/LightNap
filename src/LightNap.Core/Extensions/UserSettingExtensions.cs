using LightNap.Core.Configuration.UserSettings;
using LightNap.Core.Data.Entities;
using LightNap.Core.UserSettings.Dto.Response;

namespace LightNap.Core.Extensions
{
    internal static class UserSettingExtensions
    {
        extension(UserSetting userSetting)
        {
            /// <summary>
            /// Converts a <see cref="UserSetting"/> entity to its corresponding <see cref="UserSettingDto"/> representation.
            /// </summary>
            /// <returns>A <see cref="UserSettingDto"/> containing the key, value, created date, and last modified date.</returns>
            public UserSettingDto ToDto()
            {
                return new UserSettingDto()
                {
                    Key = userSetting.Key,
                    Value = userSetting.Value,
                    CreatedDate = userSetting.CreatedDate,
                    LastModifiedDate = userSetting.LastModifiedDate,
                };
            }
        }

        extension(UserSettingDefinition userSettingDefinition)
        {
            /// <summary>
            /// Converts a <see cref="UserSettingDefinition"/> entity to its corresponding <see cref="UserSettingDto"/> representation.
            /// </summary>
            /// <returns>A <see cref="UserSettingDto"/> containing the key, value, created date, and last modified date.</returns>
            internal UserSettingDto ToDto()
            {
                return new UserSettingDto()
                {
                    Key = userSettingDefinition.Key,
                    Value = userSettingDefinition.DefaultJson,
                };
            }
        }
    }
}
