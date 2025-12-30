using LightNap.Core.Api;
using LightNap.Core.Identity.Dto.Response;
using LightNap.Core.Identity.Interfaces;
using LightNap.Core.Integrations.Dto.Request;
using LightNap.Core.Integrations.Dto.Response;
using LightNap.Core.Integrations.Interfaces;
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
    public class MeController(IProfileService profileService, INotificationService notificationService, IClaimsService claimsService,
        IUserSettingsService userSettingsService, IExternalLoginService externalLoginService, IIntegrationsService integrationsService)
            : ControllerBase
    {
        /// <summary>
        /// Retrieves the profile of the current user.
        /// </summary>
        /// <returns>
        /// An <see cref="ApiResponseDto{T}"/> containing the profile of the current user.
        /// </returns>
        /// <response code="200">Returns the profile of the current user.</response>
        /// <response code="401">If the user is not authenticated.</response>
        [HttpGet("profile", Name = nameof(GetProfile))]
        public async Task<ApiResponseDto<ProfileDto>> GetProfile()
        {
            return new ApiResponseDto<ProfileDto>(await profileService.GetMyProfileAsync());
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
        [HttpPut("profile", Name = nameof(UpdateMyProfile))]
        public async Task<ApiResponseDto<ProfileDto>> UpdateMyProfile(UpdateProfileRequestDto updateProfileRequest)
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
        [HttpPost("notifications", Name = nameof(SearchMyNotifications))]
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
        [HttpPut("notifications/mark-all-as-read", Name = nameof(MarkAllMyNotificationsAsRead))]
        public async Task<ApiResponseDto<bool>> MarkAllMyNotificationsAsRead()
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
        [HttpPut("notifications/{id}/mark-as-read", Name = nameof(MarkMyNotificationAsRead))]
        public async Task<ApiResponseDto<bool>> MarkMyNotificationAsRead(int id)
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
        [HttpGet("claims", Name = nameof(GetMyUserClaims))]
        public async Task<ApiResponseDto<PagedResponseDto<ClaimDto>>> GetMyUserClaims(PagedRequestDtoBase pagedRequestDto)
        {
            return new ApiResponseDto<PagedResponseDto<ClaimDto>>(await claimsService.GetMyClaimsAsync(pagedRequestDto));
        }

        /// <summary>
        /// Gets the user settings for the current user.
        /// </summary>
        /// <returns>The list of user settings.</returns>
        [HttpGet("settings", Name = nameof(GetMyUserSettings))]
        public async Task<ApiResponseDto<List<UserSettingDto>>> GetMyUserSettings()
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
        [HttpPatch("settings", Name = nameof(SetMyUserSetting))]
        public async Task<ApiResponseDto<UserSettingDto>> SetMyUserSetting([FromBody] SetUserSettingRequestDto setSettingDto)
        {
            return new ApiResponseDto<UserSettingDto>(await userSettingsService.SetMySettingAsync(setSettingDto));
        }

        /// <summary>
        /// Retrieves all external login providers linked to the current user.
        /// </summary>
        /// <returns>
        /// An <see cref="ApiResponseDto{T}"/> containing a list of external login providers associated with the current user.
        /// </returns>
        [HttpGet("external-logins", Name = nameof(GetMyExternalLogins))]
        public async Task<ApiResponseDto<List<ExternalLoginDto>>> GetMyExternalLogins()
        {
            return new ApiResponseDto<List<ExternalLoginDto>>(await externalLoginService.GetMyExternalLoginsAsync());
        }

        /// <summary>
        /// Removes an external login provider from the current user's account.
        /// </summary>
        /// <param name="loginProvider">The name of the external login provider (e.g., "Google", "Facebook").</param>
        /// <param name="providerKey">The unique identifier provided by the external login provider.</param>
        /// <returns>
        /// An <see cref="ApiResponseDto{T}"/> indicating whether the external login was successfully removed.
        /// </returns>
        [HttpDelete("external-logins/{loginProvider}/{providerKey}", Name = nameof(RemoveMyExternalLogin))]
        public async Task<ApiResponseDto<bool>> RemoveMyExternalLogin(string loginProvider, string providerKey)
        {
            await externalLoginService.RemoveMyLoginAsync(loginProvider, providerKey);
            return new ApiResponseDto<bool>(true);
        }

        #region Integrations

        /// <summary>
        /// Retrieves all integrations for the current user.
        /// </summary>
        /// <returns>The list of integrations.</returns>
        /// <response code="200">Returns the list of integrations.</response>
        [HttpGet("integrations", Name = nameof(GetMyIntegrations))]
        public async Task<ApiResponseDto<List<IntegrationDto>>> GetMyIntegrations()
        {
            return new ApiResponseDto<List<IntegrationDto>>(await integrationsService.GetMyIntegrationsAsync());
        }

        /// <summary>
        /// Creates a new integration for the current user.
        /// </summary>
        /// <param name="createIntegrationRequest">The integration creation parameters.</param>
        /// <returns>The created integration details.</returns>
        /// <response code="200">Returns the created integration details.</response>
        /// <response code="400">If there was an error creating the integration.</response>
        [HttpPost("integrations", Name = nameof(CreateMyIntegration))]
        public async Task<ApiResponseDto<IntegrationDto>> CreateMyIntegration([FromBody] CreateIntegrationRequestDto createIntegrationRequest)
        {
            return new ApiResponseDto<IntegrationDto>(await integrationsService.CreateMyIntegrationAsync(createIntegrationRequest));
        }

        /// <summary>
        /// Updates an integration for the current user.
        /// </summary>
        /// <param name="integrationId">The ID of the integration to update.</param>
        /// <param name="updateIntegrationRequest">The integration update parameters.</param>
        /// <returns>The updated integration details.</returns>
        /// <response code="200">Returns the updated integration details.</response>
        /// <response code="400">If there was an error updating the integration.</response>
        [HttpPut("integrations/{integrationId}", Name = nameof(UpdateMyIntegration))]
        public async Task<ApiResponseDto<IntegrationDto>> UpdateMyIntegration(int integrationId, [FromBody] UpdateIntegrationRequestDto updateIntegrationRequest)
        {
            return new ApiResponseDto<IntegrationDto>(await integrationsService.UpdateMyIntegrationAsync(integrationId, updateIntegrationRequest));
        }

        /// <summary>
        /// Deletes an integration for the current user.
        /// </summary>
        /// <param name="integrationId">The ID of the integration to delete.</param>
        /// <returns>True if the integration was successfully deleted.</returns>
        /// <response code="200">Integration successfully deleted.</response>
        /// <response code="400">If there was an error deleting the integration.</response>
        [HttpDelete("integrations/{integrationId}", Name = nameof(DeleteMyIntegration))]
        public async Task<ApiResponseDto<bool>> DeleteMyIntegration(int integrationId)
        {
            await integrationsService.DeleteMyIntegrationAsync(integrationId);
            return new ApiResponseDto<bool>(true);
        }

        #endregion

    }
}