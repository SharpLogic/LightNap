using LightNap.Core.Notifications.Dto.Response;

namespace LightNap.Core.Hubs;

/// <summary>
/// Defines the contract for a notification client that can send notifications to users via a real-time notification hub.
/// </summary>
public interface INotificationsClient
{
    /// <summary>
    /// Asynchronously sends a notification to the specified user via the real-time notification hub.
    /// </summary>
    /// <param name="notificationDto">The notification data to send to the user. Cannot be null.</param>
    /// <returns>A task that represents the asynchronous send operation.</returns>
    Task ReceiveNotification(NotificationDto notificationDto);
}