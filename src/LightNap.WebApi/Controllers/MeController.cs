using LightNap.Core.Api;
using LightNap.Core.Notifications.Dto.Request;
using LightNap.Core.Notifications.Dto.Response;
using LightNap.Core.Notifications.Interfaces;
using LightNap.Core.Profile.Dto.Request;
using LightNap.Core.Profile.Dto.Response;
using LightNap.Core.Profile.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LightNap.WebApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/users/me")]
    public class MeController(IProfileService profileService, INotificationService notificationService) : ControllerBase
    {
        /// <summary>
        /// Retrieves the profile of the current user.
        /// </summary>
        /// <returns>
        /// An <see cref="ApiResponseDto{T}"/> containing the profile of the current user.
        /// </returns>
        /// <response code="200">Returns the profile of the current user.</response>
        /// <response code="401">If the user is not authenticated.</response>
        [HttpGet("profile")]
        [ProducesResponseType(typeof(ApiResponseDto<ProfileDto>), 200)]
        [ProducesResponseType(401)]
        public async Task<ApiResponseDto<ProfileDto>> GetProfile()
        {
            return new ApiResponseDto<ProfileDto>(await profileService.GetProfileAsync());
        }

        /// <summary>
        /// Updates the profile of the current user.
        /// </summary>
        /// <param name="updateProfileRequest">The updated profile information.</param>
        /// <returns>
        /// An <see cref="ApiResponseDto{T}"/> containing the updated profile of the current user.
        /// </returns>
        /// <response code="200">Returns the updated profile of the current user.</response>
        /// <response code="401">If the user is not authenticated.</response>
        /// <response code="400">If the request is invalid.</response>
        [HttpPut("profile")]
        [ProducesResponseType(typeof(ApiResponseDto<ProfileDto>), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(400)]
        public async Task<ApiResponseDto<ProfileDto>> UpdateProfile(UpdateProfileRequestDto updateProfileRequest)
        {
            return new ApiResponseDto<ProfileDto>(await profileService.UpdateProfileAsync(updateProfileRequest));
        }

        /// <summary>
        /// Retrieves the settings of the current user.
        /// </summary>
        /// <returns>
        /// An <see cref="ApiResponseDto{T}"/> containing the settings of the current user.
        /// </returns>
        /// <response code="200">Returns the settings of the current user.</response>
        /// <response code="401">If the user is not authenticated.</response>
        [HttpGet("settings")]
        [ProducesResponseType(typeof(ApiResponseDto<BrowserSettingsDto>), 200)]
        [ProducesResponseType(401)]
        public async Task<ApiResponseDto<BrowserSettingsDto>> GetSettings()
        {
            return new ApiResponseDto<BrowserSettingsDto>(await profileService.GetSettingsAsync());
        }

        /// <summary>
        /// Updates the settings of the current user.
        /// </summary>
        /// <param name="browserSettings">The updated settings information.</param>
        /// <returns>
        /// A <see cref="ApiResponseDto{T}"/> containing true if the update succeeded.
        /// </returns>
        /// <response code="200">Returns the updated settings of the current user.</response>
        /// <response code="401">If the user is not authenticated.</response>
        /// <response code="400">If the request is invalid.</response>
        [HttpPut("settings")]
        [ProducesResponseType(typeof(ApiResponseDto<bool>), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(400)]
        public async Task<ApiResponseDto<bool>> UpdateSettings(BrowserSettingsDto browserSettings)
        {
            await profileService.UpdateSettingsAsync(browserSettings);
            return new ApiResponseDto<bool>(true);
        }

        /// <summary>
        /// Searches the notifications of the current user.
        /// </summary>
        /// <param name="searchNotificationsRequest">The search criteria for notifications.</param>
        /// <returns>
        /// An <see cref="ApiResponseDto{T}"/> containing a paginated list of notifications.
        /// </returns>
        /// <response code="200">Returns the list of notifications.</response>
        /// <response code="401">If the user is not authenticated.</response>
        [HttpPost("notifications")]
        public async Task<ApiResponseDto<NotificationSearchResultsDto>> SearchMyNotifications(SearchNotificationsRequestDto searchNotificationsRequest)
        {
            return new ApiResponseDto<NotificationSearchResultsDto>(await notificationService.SearchMyNotificationsAsync(searchNotificationsRequest));
        }

        /// <summary>
        /// Marks all notifications as read for the current user.
        /// </summary>
        /// <returns>
        /// An <see cref="ApiResponseDto{T}"/> indicating whether the operation was successful.
        /// </returns>
        /// <response code="200">If all notifications were marked as read successfully.</response>
        /// <response code="401">If the user is not authenticated.</response>
        [HttpPut("notifications/mark-all-as-read")]
        public async Task<ApiResponseDto<bool>> MarkAllNotificationsAsRead()
        {
            await notificationService.MarkAllMyNotificationsAsReadAsync();
            return new ApiResponseDto<bool>(true);
        }

        /// <summary>
        /// Marks a specific notification as read for the current user.
        /// </summary>
        /// <param name="id">The ID of the notification to mark as read.</param>
        /// <returns>
        /// An <see cref="ApiResponseDto{T}"/> indicating whether the operation was successful.
        /// </returns>
        /// <response code="200">If the notification was marked as read successfully.</response>
        /// <response code="401">If the user is not authenticated.</response>
        [HttpPut("notifications/{id}/mark-as-read")]
        public async Task<ApiResponseDto<bool>> MarkNotificationAsRead(int id)
        {
            await notificationService.MarkMyNotificationAsReadAsync(id);
            return new ApiResponseDto<bool>(true);
        }
    }
}