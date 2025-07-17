using LightNap.Core.Data.Entities;

namespace LightNap.Core.Notifications.Dto.Request
{
    /// <summary>
    /// Data Transfer Object for creating a notification.
    /// </summary>
    public class CreateNotificationRequestDto
    {
        /// <summary>
        /// The type of the notification.
        /// </summary>
        public NotificationType Type { get; set; }

        /// <summary>
        /// Data used to specify the details of the given type of notification.
        /// </summary>
        public Dictionary<string, object> Data { get; set; } = [];
    }
}