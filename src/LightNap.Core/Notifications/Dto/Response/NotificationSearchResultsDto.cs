using LightNap.Core.Api;

namespace LightNap.Core.Notifications.Dto.Response
{
    /// <summary>
    /// Contains the search results for notifications.
    /// </summary>
    public class NotificationSearchResultsDto(IList<NotificationDto> data, int pageNumber, int pageSize, int totalResults)
        : PagedResponseDto<NotificationDto>(data, pageNumber, pageSize, totalResults)
    {
        /// <summary>
        /// The number of unread notifications.
        /// </summary>
        public int UnreadCount { get; set; }
    }
}