using LightNap.Core.Api;
using LightNap.Core.Data;
using LightNap.Core.Data.Entities;
using LightNap.Core.Identity.Dto.Response;
using LightNap.Core.Identity.Extensions;
using LightNap.Core.Interfaces;
using LightNap.Core.Notifications.Dto.Request;
using LightNap.Core.Notifications.Dto.Response;
using LightNap.Core.Notifications.Extensions;
using LightNap.Core.Notifications.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LightNap.Core.Notifications.Services
{
    /// <summary>  
    /// Service for managing user notifications.
    /// </summary>  
    public class NotificationService(ApplicationDbContext db, UserManager<ApplicationUser> userManager, IUserContext userContext) : INotificationService
    {
        /// <summary>
        /// Creates a notification for a specific user.
        /// </summary>
        /// <param name="userId">The ID of the user to notify.</param>
        /// <param name="requestDto">The notification data transfer object containing the notification details.</param>
        internal async Task CreateUserNotificationAsync(string userId, CreateNotificationRequestDto requestDto)
        {
            // TODO: Confirm the user wants this kind of notification (if implemented).

            Notification notification = requestDto.ToCreate(userId);
            db.Notifications.Add(notification);
            await db.SaveChangesAsync();

            // TODO: Send notification to SignalR
        }

        /// <summary>
        /// Creates a notification for a specific user. Only use this method if you have authorized the workflow.
        /// </summary>
        /// <param name="userId">The ID of the user to notify.</param>
        /// <param name="requestDto">The notification data transfer object containing the notification details.</param>
        public async Task CreateSystemNotificationForUserAsync(string userId, CreateNotificationRequestDto requestDto)
        {
            await this.CreateUserNotificationAsync(userId, requestDto);
        }

        /// <summary>
        /// Creates a notification for all users in a specified role. Only use this method if you have authorized the workflow.
        /// </summary>
        /// <param name="role">The role for which to create notifications.</param>
        /// <param name="requestDto">The notification data transfer object containing the notification details.</param>
        public async Task CreateSystemNotificationForRoleAsync(string role, CreateNotificationRequestDto requestDto)
        {
            foreach (var user in await userManager.GetUsersInRoleAsync(role))
            {
                await this.CreateUserNotificationAsync(user.Id, requestDto);
            }
        }

        /// <summary>
        /// Creates a notification for all users with a specified claim. Only use this method if you have authorized the workflow.
        /// </summary>
        /// <param name="claim">The claim to identify users for the notification.</param>
        /// <param name="requestDto">The notification data transfer object containing the notification details.</param>
        public async Task CreateSystemNotificationForClaimAsync(ClaimDto claim, CreateNotificationRequestDto requestDto)
        {
            foreach (var user in await userManager.GetUsersForClaimAsync(claim.ToClaim()))
            {
                await this.CreateUserNotificationAsync(user.Id, requestDto);
            }
        }

        /// <summary>
        /// Retrieves a notification by its ID.
        /// </summary>
        /// <param name="id">The ID of the notification to retrieve.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the notification data transfer object.</returns>
        public async Task<NotificationDto?> GetNotificationAsync(int id)
        {
            var notification = await db.Notifications.FindAsync(id);
            return notification?.ToDto();
        }

        /// <summary>
        /// Marks all notifications as read for a specific user.
        /// </summary>
        /// <param name="userId">The ID of the user whose notifications should be marked as read.</param>
        public async Task MarkAllAsReadAsync(string userId)
        {
            // ExecuteUpdateAsync is not universally supported.
            foreach (var notification in await db.Notifications.Where(n => n.UserId == userId).ToListAsync())
            {
                notification.Status = NotificationStatus.Read;
            }
            await db.SaveChangesAsync();
        }

        /// <summary>
        /// Marks a specific notification as read.
        /// </summary>
        /// <param name="id">The ID of the notification to mark as read.</param>
        public async Task MarkAsReadAsync(int id)
        {
            Notification notification = db.Notifications.Find(id) ?? throw new UserFriendlyApiException("Notification not found");
            notification.Status = NotificationStatus.Read;
            await db.SaveChangesAsync();
        }

        /// <summary>
        /// Searches for notifications based on the specified criteria.
        /// </summary>
        /// <param name="userId">The ID of the user whose notifications to search.</param>
        /// <param name="requestDto">The search criteria for the notifications.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the search results data transfer object.</returns>
        public async Task<NotificationSearchResultsDto> SearchNotificationsAsync(string userId, SearchNotificationsRequestDto requestDto)
        {
            IQueryable<Notification> baseQuery = db.Notifications.Where(n => n.UserId == userId);

            if (requestDto.SinceId is not null)
            {
                baseQuery = baseQuery.Where(n => n.Id > requestDto.SinceId);
            }

            if (requestDto.PriorToId is not null)
            {
                baseQuery = baseQuery.Where(n => n.Id < requestDto.PriorToId);
            }

            if (requestDto.Status is not null)
            {
                baseQuery = baseQuery.Where(n => n.Status == requestDto.Status);
            }
            else
            {
                baseQuery = baseQuery.Where(n => n.Status != NotificationStatus.Archived);
            }

            if (requestDto.Type is not null)
            {
                baseQuery = baseQuery.Where(n => n.Type == requestDto.Type);
            }

            var query = baseQuery.OrderByDescending(item => item.Id);

            int skip = (requestDto.PageNumber - 1) * requestDto.PageSize;

            // Batch the queries for totalCount, unreadCount, and page items
            var totalCountTask = baseQuery.CountAsync();
            var unreadCountTask = db.Notifications.CountAsync(n => n.UserId == userId && n.Status == NotificationStatus.Unread);
            var itemsTask = query.Skip(skip).Take(requestDto.PageSize).Select(item => item.ToDto()).ToListAsync();

            await Task.WhenAll(totalCountTask, unreadCountTask, itemsTask);

            return new NotificationSearchResultsDto(itemsTask.Result, requestDto.PageNumber, requestDto.PageSize, totalCountTask.Result)
            {
                UnreadCount = unreadCountTask.Result
            };
        }

        /// <summary>
        /// Searches for notifications of the logged-in user based on the specified criteria.
        /// </summary>
        /// <param name="requestDto">The data transfer object containing the search criteria.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the search results.</returns>
        public async Task<NotificationSearchResultsDto> SearchMyNotificationsAsync(SearchNotificationsRequestDto requestDto)
        {
            return await this.SearchNotificationsAsync(userContext.GetUserId(), requestDto);
        }

        /// <summary>
        /// Marks all notifications as read for the logged-in user.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task MarkAllMyNotificationsAsReadAsync()
        {
            await this.MarkAllAsReadAsync(userContext.GetUserId());
        }

        /// <summary>
        /// Marks a specific notification as read for the logged-in user.
        /// </summary>
        /// <param name="id">The ID of the notification to be marked as read.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        /// <exception cref="UserFriendlyApiException">Thrown when the notification is not found.</exception>
        public async Task MarkMyNotificationAsReadAsync(int id)
        {
            Notification notification = await db.Notifications.FirstOrDefaultAsync(n => n.Id == id && n.UserId == userContext.GetUserId()) ?? throw new UserFriendlyApiException("Notification not found.");
            await this.MarkAsReadAsync(id);
        }
    }
}