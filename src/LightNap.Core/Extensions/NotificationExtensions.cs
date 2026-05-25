using LightNap.Core.Data.Entities;
using LightNap.Core.Notifications.Dto.Request;
using LightNap.Core.Notifications.Dto.Response;
using LightNap.Core.Notifications.Enums;

namespace LightNap.Core.Extensions
{
    /// <summary>
    /// Provides extension methods for converting between Notification and related DTOs.
    /// </summary>
    internal static class NotificationExtensions
    {
        extension(CreateNotificationRequestDto dto)
        {
            /// <summary>
            /// Converts a CreateNotificationDto to a Notification entity.
            /// </summary>
            /// <param name="userId">The ID of the user associated with the notification.</param>
            /// <returns>A new Notification entity.</returns>
            public Notification ToCreate(string userId)
            {
                return new Notification()
                {
                    Data = dto.Data,
                    Status = NotificationStatus.Unread,
                    Timestamp = DateTime.UtcNow,
                    Type = dto.Type,
                    UserId = userId,
                };
            }
        }

        extension(Notification notification)
        {
            /// <summary>
            /// Converts a Notification entity to a NotificationDto.
            /// </summary>
            /// <returns>A new NotificationDto.</returns>
            public NotificationDto ToDto()
            {
                return new NotificationDto()
                {
                    Data = notification.Data,
                    Id = notification.Id,
                    Status = notification.Status,
                    Type = notification.Type,
                    Timestamp = notification.Timestamp,
                };
            }
        }
    }
}
