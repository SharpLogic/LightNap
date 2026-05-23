using LightNap.Core.Hubs;
using LightNap.Core.Notifications.Dto.Response;
using Microsoft.AspNetCore.SignalR;

namespace LightNap.Core.Extensions
{
    /// <summary>
    /// Helpers for targeting SignalR messages to per-user groups, so push-message format strings stay
    /// in one place.
    /// </summary>
    public static class SignalRClientExtensions
    {
        /// <summary>
        /// Adds the specified connection to the SignalR group associated with the given user ID.
        /// </summary>
        public static async Task AddToUserGroupAsync(this Hub<IRealTimeClient> hub, string connectionId, string userId)
        {
            await hub.Groups.AddToGroupAsync(connectionId, FormatUserGroupName(userId));
        }

        /// <summary>
        /// Sends a notification to every active connection for the specified user.
        /// </summary>
        public static async Task SendNotificationToUserAsync(this IHubContext<RealTimeHub, IRealTimeClient> hub, string userId, NotificationDto notification)
        {
            await hub.Clients.Group(FormatUserGroupName(userId)).ReceiveNotification(notification);
        }

        private static string FormatUserGroupName(string userId) => $"user:{userId}";
    }
}
