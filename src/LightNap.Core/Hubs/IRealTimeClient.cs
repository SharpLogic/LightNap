using LightNap.Core.Notifications.Dto.Response;

namespace LightNap.Core.Hubs;

/// <summary>
/// Defines a contract for clients that can receive real-time notifications from a notification hub.
/// </summary>
/// <remarks></remarks>
public interface IRealTimeClient
{
    /// <summary>
    /// Asynchronously sends a notification to the specified user via the real-time notification hub.
    /// </summary>
    /// <param name="notificationDto">The notification data to send to the user. Cannot be null.</param>
    /// <returns>A task that represents the asynchronous send operation.</returns>
    Task ReceiveNotification(NotificationDto notificationDto);
}