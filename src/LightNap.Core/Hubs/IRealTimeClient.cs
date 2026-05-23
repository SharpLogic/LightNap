using LightNap.Core.Notifications.Dto.Response;

namespace LightNap.Core.Hubs
{
    /// <summary>
    /// Strongly-typed contract for SignalR clients that receive real-time updates from the server.
    /// Extend this interface as new push events are added.
    /// </summary>
    public interface IRealTimeClient
    {
        /// <summary>
        /// Pushes a notification to the connected client.
        /// </summary>
        /// <param name="notificationDto">The notification payload.</param>
        Task ReceiveNotification(NotificationDto notificationDto);
    }
}
