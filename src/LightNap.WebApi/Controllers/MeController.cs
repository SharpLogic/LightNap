using LightNap.Core.Api;
using LightNap.Core.Identity.Dto.Response;
using LightNap.Core.Notifications.Dto.Request;
using LightNap.Core.Notifications.Dto.Response;
using LightNap.Core.Notifications.Interfaces;
using LightNap.Core.Profile.Dto.Request;
using LightNap.Core.Profile.Dto.Response;
using LightNap.Core.Profile.Interfaces;
using LightNap.Core.Users.Interfaces;
using LightNap.Core.UserSettings.Dto.Request;
using LightNap.Core.UserSettings.Dto.Response;
using LightNap.Core.UserSettings.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LightNap.WebApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/users/me")]
    public class MeController(IProfileService profileService, INotificationService notificationService, IClaimsService claimsService, IUserSettingsService userSettingsService) : ControllerBase
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

        /// <summary>
        /// Retrieves the claims associated with the currently authenticated user.
        /// </summary>
        /// <remarks>This method returns a list of claims for the user making the request. The claims
        /// provide  information about the user's identity, roles, or other attributes. The user must be authenticated
        /// to access this endpoint.</remarks>
        /// <param name="pagedRequestDto">The pagination and sorting information.</param>
        /// <returns>A <see cref="PagedResponseDto{ClaimDto}"/> containing the user's claims.</returns>
        [HttpGet("claims")]
        public async Task<ApiResponseDto<PagedResponseDto<ClaimDto>>> GetUserClaimsAsync(PagedRequestDtoBase pagedRequestDto)
        {
            return new ApiResponseDto<PagedResponseDto<ClaimDto>>(await claimsService.GetMyClaimsAsync(pagedRequestDto));
        }

        /// <summary>
        /// Gets the user settings for the current user.
        /// </summary>
        /// <returns>The list of user settings.</returns>
        [HttpGet("settings")]
        public async Task<ApiResponseDto<List<UserSettingDto>>> GetUserSettingsAsync()
        {
            return new ApiResponseDto<List<UserSettingDto>>(await userSettingsService.GetMySettingsAsync());
        }

        /// <summary>
        /// Updates the user setting based on the provided request data.
        /// </summary>
        /// <remarks>This method applies the changes to the user's settings and returns the updated
        /// setting in the response. Ensure that the provided request data is valid and complete before calling this
        /// method.</remarks>
        /// <param name="setSettingDto">The request data containing the user setting to be updated. This parameter cannot be <see langword="null"/>.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an <see
        /// cref="ApiResponseDto{T}"/> object wrapping the updated <see cref="UserSettingDto"/>.</returns>
        [HttpPatch("settings")]
        public async Task<ApiResponseDto<UserSettingDto>> SetUserSettingAsync([FromBody] SetUserSettingRequestDto setSettingDto)
        {
            return new ApiResponseDto<UserSettingDto>(await userSettingsService.SetMySettingAsync(setSettingDto));
        }

    }
}