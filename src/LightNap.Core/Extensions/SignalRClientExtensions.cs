using LightNap.Core.Extensions;
using LightNap.Core.Hubs;
using LightNap.Core.Notifications.Dto.Response;
using Microsoft.AspNetCore.SignalR;

namespace LightNap.Core.Extensions
{
    /// <summary>
    /// Extension methods for sending SignalR messages to clients.
    /// Consolidates message formatting to avoid scattered format strings throughout the codebase.
    /// </summary>
    public static class SignalRClientExtensions
    {
        /// <summary>
        /// Adds the specified connection to the SignalR group associated with the given user identifier.
        /// </summary>
        /// <remarks>This method enables targeted messaging to a specific user by adding their connection
        /// to a user-specific group. Use this to ensure that notifications or messages can be sent to all active
        /// connections for a particular user.</remarks>
        /// <param name="hub">The SignalR hub context used to manage group membership.</param>
        /// <param name="connectionId">The unique identifier for the client connection to add to the group. Cannot be null or empty.</param>
        /// <param name="userId">The user identifier that determines the group to which the connection will be added. Cannot be null or
        /// empty.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public static async Task AddToUserGroupAsync(this Hub<INotificationsClient> hub, string connectionId, string userId)
        {
            await hub.Groups.AddToGroupAsync(connectionId, SignalRClientExtensions.FormatUserGroupName(userId));
        }

        /// <summary>
        /// Sends a notification to a specific user by their ID.
        /// </summary>
        /// <param name="clients">The clients proxy for sending messages.</param>
        /// <param name="notification">The notification DTO to send.</param>
        public static async Task SendAsync(this IClientProxy clients, NotificationDto notification)
        {
            await clients.SendAsync("ReceiveNotification", notification);
        }

        /// <summary>
        /// Sends a notification to a user group identified by their user ID.
        /// </summary>
        /// <param name="hub">The hub for sending messages.</param>
        /// <param name="userId">The ID of the user to notify.</param>
        /// <param name="notification">The notification DTO to send.</param>
        public static async Task SendNotificationToUserAsync(this IHubContext<NotificationHub> hub, string userId, NotificationDto notification)
        {
            await hub.Clients.Group(SignalRClientExtensions.FormatUserGroupName(userId)).SendAsync(notification);
        }

        /// <summary>
        /// Gets the group name for a specific user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>The formatted group name.</returns>
        private static string FormatUserGroupName(string userId) => $"user:{userId}";
    }
}