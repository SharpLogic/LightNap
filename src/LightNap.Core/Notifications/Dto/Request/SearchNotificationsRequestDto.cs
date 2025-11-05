using LightNap.Core.Api;
using LightNap.Core.Notifications.Enums;

namespace LightNap.Core.Notifications.Dto.Request
{
    /// <summary>
    /// Data transfer object for searching notifications with pagination support.
    /// </summary>
    public class SearchNotificationsRequestDto : PagedRequestDtoBase
    {
        /// <summary>
        /// Gets or sets the ID of the notification to start the search from.
        /// </summary>
        /// <value>The ID of the notification to start the search from.</value>
        public int? SinceId { get; set; }

        /// <summary>
        /// Gets or sets the ID of the notification to end the search at.
        /// </summary>
        /// <value>The ID of the notification to end the search at.</value>
        public int? PriorToId { get; set; }

        /// <summary>
        /// Gets or sets the status of the notifications to search for.
        /// </summary>
        /// <value>The status of the notifications to search for.</value>
        public NotificationStatus? Status { get; set; }

        /// <summary>
        /// Gets or sets the type of the notifications to search for.
        /// </summary>
        /// <value>The type of the notifications to search for.</value>
        public NotificationType? Type { get; set; }
    }
}