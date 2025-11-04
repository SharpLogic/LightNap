using LightNap.Core.Notifications.Enums;

namespace LightNap.Core.Data.Entities
{
    public class Notification
    {
        public int Id { get; set; }
        public required string UserId { get; set; }
        public ApplicationUser? User { get; set; }
        public DateTime Timestamp { get; set; }
        public NotificationType Type { get; set; }
        public NotificationStatus Status { get; set; }
        public Dictionary<string, object> Data { get; set; } = [];
    }
}
