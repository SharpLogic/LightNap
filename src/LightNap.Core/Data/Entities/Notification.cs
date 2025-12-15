using LightNap.Core.Notifications.Enums;

namespace LightNap.Core.Data.Entities
{
    /// <summary>
    /// Represents a notification sent to a user, including its type, status, and associated metadata.
    /// </summary>
    public class Notification
    {
        /// <summary>
        /// Gets or sets the unique identifier for the notification.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the user ID associated with this notification.
        /// </summary>
        public required string UserId { get; set; }

        /// <summary>
        /// Gets or sets the user that received this notification.
        /// </summary>
        public ApplicationUser? User { get; set; }

        /// <summary>
        /// Gets or sets the timestamp when the notification was created.
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Gets or sets the type of notification.
        /// </summary>
        public NotificationType Type { get; set; }

        /// <summary>
        /// Gets or sets the current status of the notification.
        /// </summary>
        public NotificationStatus Status { get; set; }

        /// <summary>
        /// Gets or sets the additional data associated with this notification.
        /// </summary>
        public Dictionary<string, object> Data { get; set; } = [];
    }
}
