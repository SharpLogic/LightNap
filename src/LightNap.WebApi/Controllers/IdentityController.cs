using LightNap.Core.Api;
using LightNap.Core.Identity.Dto.Request;
using LightNap.Core.Identity.Dto.Response;
using LightNap.Core.Identity.Interfaces;
using LightNap.WebApi.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace LightNap.WebApi.Controllers
{
    /// <summary>
    /// Controller for handling identity-related actions such as login, registration, and password reset.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [EnableRateLimiting(WebConstants.RateLimiting.AuthPolicyName)]
    public class IdentityController(IIdentityService identityService) : ControllerBase
    {
        /// <summary>
        /// Logs in a user.
        /// </summary>
        /// <param name="loginRequest">The login request DTO.</param>
        /// <returns>The API response containing the login result.</returns>
        [HttpPost("login", Name = nameof(LogIn))]
        public async Task<ApiResponseDto<LoginSuccessDto>> LogIn(LoginRequestDto loginRequest)
        {
            return new ApiResponseDto<LoginSuccessDto>(await identityService.LogInAsync(loginRequest));
        }

        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <param name="registerRequest">The registration request DTO.</param>
        /// <returns>The API response containing the login result.</returns>
        [HttpPost("register", Name = nameof(Register))]
        [EnableRateLimiting(WebConstants.RateLimiting.RegistrationPolicyName)]  // Override the controller-level "Auth" policy
        public async Task<ApiResponseDto<LoginSuccessDto>> Register(RegisterRequestDto registerRequest)
        {
            return new ApiResponseDto<LoginSuccessDto>(await identityService.RegisterAsync(registerRequest));
        }

        /// <summary>
        /// Logs out the current user.
        /// </summary>
        /// <returns>The API response indicating the success of the operation.</returns>
        [HttpGet("logout", Name = nameof(LogOut))]
        public async Task<ApiResponseDto<bool>> LogOut()
        {
            await identityService.LogOutAsync();
            return new ApiResponseDto<bool>(true);
        }

        /// <summary>
        /// Changes the password of the current user.
        /// </summary>
        /// <param name="changePasswordRequest">The password change request.</param>
        /// <returns>
        /// An <see cref="ApiResponseDto{T}"/> indicating whether the password was changed successfully.
        /// </returns>
        /// <response code="200">If the password was changed successfully.</response>
        /// <response code="400">If the request is invalid or the current password is incorrect.</response>
        /// <response code="401">If the user is not authenticated.</response>
        [HttpPost("change-password", Name = nameof(ChangePassword))]
        public async Task<ApiResponseDto<bool>> ChangePassword(ChangePasswordRequestDto changePasswordRequest)
        {
            await identityService.ChangePasswordAsync(changePasswordRequest);
            return new ApiResponseDto<bool>(true);
        }

        /// <summary>
        /// Changes the email of the current user.
        /// </summary>
        /// <param name="changeEmailRequest">The email change request.</param>
        /// <returns>
        /// An <see cref="ApiResponseDto{T}"/> indicating whether the email was changed successfully.
        /// </returns>
        /// <response code="200">If the email was changed successfully.</response>
        /// <response code="400">If the request is invalid or the current email is incorrect.</response>
        /// <response code="401">If the user is not authenticated.</response>
        [HttpPost("change-email", Name = nameof(ChangeEmail))]
        [Authorize]
        public async Task<ApiResponseDto<bool>> ChangeEmail(ChangeEmailRequestDto changeEmailRequest)
        {
            await identityService.ChangeEmailAsync(changeEmailRequest);
            return new ApiResponseDto<bool>(true);
        }

        /// <summary>
        /// Confirms the email change of the current user.
        /// </summary>
        /// <param name="confirmEmailChangeRequest">The email change confirmation details.</param>
        /// <returns>
        /// An <see cref="ApiResponseDto{T}"/> indicating whether the email change was confirmed successfully.
        /// </returns>
        /// <response code="200">If the email change was confirmed successfully.</response>
        /// <response code="400">If the token is invalid or expired.</response>
        /// <response code="401">If the user is not authenticated.</response>
        [HttpPost("confirm-email-change", Name = nameof(ConfirmEmailChange))]
        [Authorize]
        public async Task<ApiResponseDto<bool>> ConfirmEmailChange(ConfirmEmailChangeRequestDto confirmEmailChangeRequest)
        {
            await identityService.ConfirmEmailChangeAsync(confirmEmailChangeRequest);
            return new ApiResponseDto<bool>(true);
        }

        /// <summary>
        /// Resets the password for a user.
        /// </summary>
        /// <param name="resetPasswordRequest">The reset password request DTO.</param>
        /// <returns>The API response indicating the success of the operation.</returns>
        [HttpPost("reset-password", Name = nameof(ResetPassword))]
        public async Task<ApiResponseDto<bool>> ResetPassword(ResetPasswordRequestDto resetPasswordRequest)
        {
            await identityService.ResetPasswordAsync(resetPasswordRequest);
            return new ApiResponseDto<bool>(true);
        }

        /// <summary>
        /// Sets a new password for a user.
        /// </summary>
        /// <param name="newPasswordRequest">The new password request DTO.</param>
        /// <returns>The API response containing the login result.</returns>
        [HttpPost("new-password", Name = nameof(NewPassword))]
        public async Task<ApiResponseDto<LoginSuccessDto>> NewPassword(NewPasswordRequestDto newPasswordRequest)
        {
            return new ApiResponseDto<LoginSuccessDto>(await identityService.NewPasswordAsync(newPasswordRequest));
        }

        /// <summary>
        /// Verifies the two-factor authentication code.
        /// </summary>
        /// <param name="verifyCodeRequest">The verify code request DTO.</param>
        /// <returns>The API response containing the login result.</returns>
        [HttpPost("verify-code", Name = nameof(VerifyCode))]
        public async Task<ApiResponseDto<string>> VerifyCode(VerifyCodeRequestDto verifyCodeRequest)
        {
            return new ApiResponseDto<string>(await identityService.VerifyCodeAsync(verifyCodeRequest));
        }

        /// <summary>
        /// Refreshes the access token using the refresh token.
        /// </summary>
        /// <returns>The API response containing the new access token.</returns>
        [HttpGet("access-token", Name = nameof(GetAccessToken))]
        public async Task<ApiResponseDto<string>> GetAccessToken()
        {
            return new ApiResponseDto<string>(await identityService.GetAccessTokenAsync());
        }

        /// <summary>
        /// Requests an email verification email for a user.
        /// </summary>
        /// <param name="verificationEmailRequest">Contains the email address of the user.</param>
        /// <returns>The API response indicating the success of the operation.</returns>
        [HttpPost("request-verification-email", Name = nameof(RequestVerificationEmail))]
        public async Task<ApiResponseDto<bool>> RequestVerificationEmail(SendVerificationEmailRequestDto verificationEmailRequest)
        {
            await identityService.RequestVerificationEmailAsync(verificationEmailRequest);
            return new ApiResponseDto<bool>(true);
        }

        /// <summary>
        /// Verifies the email address of a user.
        /// </summary>
        /// <param name="verifyEmailRequest">The verify email request DTO.</param>
        /// <returns>The API response indicating the success of the operation.</returns>
        [HttpPost("verify-email", Name = nameof(VerifyEmail))]
        public async Task<ApiResponseDto<bool>> VerifyEmail(VerifyEmailRequestDto verifyEmailRequest)
        {
            await identityService.VerifyEmailAsync(verifyEmailRequest);
            return new ApiResponseDto<bool>(true);
        }

        /// <summary>
        /// Requests a magic link the user can use to log in.
        /// </summary>
        /// <param name="magicLinkRequest">Contains the email address of the user.</param>
        /// <returns>The API response indicating the success of the operation.</returns>
        [HttpPost("request-magic-link", Name = nameof(RequestMagicLinkEmail))]
        public async Task<ApiResponseDto<bool>> RequestMagicLinkEmail(SendMagicLinkRequestDto magicLinkRequest)
        {
            await identityService.RequestMagicLinkEmailAsync(magicLinkRequest);
            return new ApiResponseDto<bool>(true);
        }

        /// <summary>
        /// Retrieves the list of devices.
        /// </summary>
        /// <returns>The list of devices.</returns>
        /// <response code="200">Returns the list of devices.</response>
        /// <response code="401">Unauthorized access.</response>
        [HttpGet("devices", Name = nameof(GetDevices))]
        [Authorize]
        public async Task<ApiResponseDto<IList<DeviceDto>>> GetDevices()
        {
            return new ApiResponseDto<IList<DeviceDto>>(await identityService.GetDevicesAsync());
        }

        /// <summary>
        /// Revokes a device.
        /// </summary>
        /// <param name="deviceId">The ID of the device to revoke.</param>
        /// <returns>A response indicating whether the device was successfully revoked.</returns>
        /// <response code="200">Device successfully revoked.</response>
        /// <response code="401">Unauthorized access.</response>
        /// <response code="404">Device not found.</response>
        [HttpDelete("devices/{deviceId}", Name = nameof(RevokeDevice))]
        [Authorize]
        public async Task<ApiResponseDto<bool>> RevokeDevice(string deviceId)
        {
            await identityService.RevokeDeviceAsync(deviceId);
            return new ApiResponseDto<bool>(true);
        }
    }}