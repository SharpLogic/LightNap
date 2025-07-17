using LightNap.Core.Data.Entities;
using LightNap.Core.Notifications.Dto.Request;
using LightNap.Core.Notifications.Dto.Response;

namespace LightNap.Core.Extensions
{
    /// <summary>
    /// Provides extension methods for converting between Notification and related DTOs.
    /// </summary>
    public static class NotificationExtensions
    {
        /// <summary>
        /// Converts a CreateNotificationDto to a Notification entity.
        /// </summary>
        /// <param name="dto">The CreateNotificationDto to convert.</param>
        /// <param name="userId">The ID of the user associated with the notification.</param>
        /// <returns>A new Notification entity.</returns>
        public static Notification ToCreate(this CreateNotificationRequestDto dto, string userId)
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

        /// <summary>
        /// Converts a Notification entity to a NotificationDto.
        /// </summary>
        /// <param name="notification">The Notification entity to convert.</param>
        /// <returns>A new NotificationDto.</returns>
        public static NotificationDto ToDto(this Notification notification)
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