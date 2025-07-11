using LightNap.Core.Identity.Dto.Response;
using LightNap.Core.Notifications.Dto.Request;
using LightNap.Core.Notifications.Dto.Response;

namespace LightNap.Core.Notifications.Interfaces
{
    public interface INotificationService
    {
        /// <summary>
        /// Retrieves a notification by its ID.
        /// </summary>
        /// <param name="id">The ID of the notification.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the notification DTO.</returns>
        Task<NotificationDto?> GetNotificationAsync(int id);

        /// <summary>
        /// Searches for notifications based on the specified criteria.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="requestDto">The search criteria.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the search results DTO.</returns>
        Task<NotificationSearchResultsDto> SearchNotificationsAsync(string userId, SearchNotificationsRequestDto requestDto);

        /// <summary>
        /// Creates a notification for a specific user. Only use this method if you have authorized the workflow.
        /// </summary>
        /// <param name="userId">The ID of the user to notify.</param>
        /// <param name="requestDto">The notification data transfer object containing the notification details.</param>
        Task CreateSystemNotificationForUserAsync(string userId, CreateNotificationRequestDto requestDto);

        /// <summary>
        /// Creates a notification for all users in a specified role. Only use this method if you have authorized the workflow.
        /// </summary>
        /// <param name="role">The role for which to create notifications.</param>
        /// <param name="requestDto">The notification data transfer object containing the notification details.</param>
        Task CreateSystemNotificationForRoleAsync(string role, CreateNotificationRequestDto requestDto);

        /// <summary>
        /// Creates a notification for all users with a specified claim. Only use this method if you have authorized the workflow.
        /// </summary>
        /// <param name="claim">The claim to identify users for the notification.</param>
        /// <param name="requestDto">The notification data transfer object containing the notification details.</param>
        Task CreateSystemNotificationForClaimAsync(ClaimDto claim, CreateNotificationRequestDto requestDto);

        /// <summary>
        /// Marks a notification as read by its ID.
        /// </summary>
        /// <param name="id">The ID of the notification.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task MarkAsReadAsync(int id);

        /// <summary>
        /// Marks all notifications as read for a user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task MarkAllAsReadAsync(string userId);

        /// <summary>
        /// Searches the logged-in user's notifications based on the provided criteria.
        /// </summary>
        /// <param name="requestDto">The data transfer object containing the search criteria.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="NotificationSearchResultsDto"/> with the search results.</returns>
        Task<NotificationSearchResultsDto> SearchMyNotificationsAsync(SearchNotificationsRequestDto requestDto);

        /// <summary>
        /// Marks all notifications as read for the requesting user.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task MarkAllMyNotificationsAsReadAsync();

        /// <summary>
        /// Marks a specific notification as read for the requesting user.
        /// </summary>
        /// <param name="id">The ID of the notification to be marked as read.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task MarkMyNotificationAsReadAsync(int id);
    }
}