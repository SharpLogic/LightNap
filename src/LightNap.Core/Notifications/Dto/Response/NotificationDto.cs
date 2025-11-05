using LightNap.Core.Notifications.Enums;

namespace LightNap.Core.Notifications.Dto.Response
{
    /// <summary>
    /// Represents a notification data transfer object.
    /// </summary>
    public class NotificationDto
    {
        /// <summary>
        /// Gets or sets the notification identifier.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the timestamp of the notification.
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Gets or sets the status of the notification.
        /// </summary>
        public NotificationStatus Status { get; set; }

        /// <summary>
        /// Gets or sets the type of the notification.
        /// </summary>
        public NotificationType Type { get; set; }

        /// <summary>
        /// Gets or sets the additional data associated with the notification.
        /// </summary>
        public Dictionary<string, object> Data { get; set; } = [];
    }
}