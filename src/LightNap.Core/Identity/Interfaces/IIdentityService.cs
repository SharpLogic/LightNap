using LightNap.Core.Identity.Dto.Request;
using LightNap.Core.Identity.Dto.Response;
using LightNap.Core.Identity.Models;
using Microsoft.AspNetCore.Authentication;

namespace LightNap.Core.Identity.Interfaces
{
    /// <summary>  
    /// Provides methods to manage identity.  
    /// </summary>  
    public interface IIdentityService
    {
        /// <summary>
        /// Logs in a user asynchronously.
        /// </summary>
        /// <param name="loginRequest">The login request data transfer object.</param>
        /// <returns>The result of the login operation which may be a token or indicate additional required steps.</returns>
        Task<LoginSuccessDto> LogInAsync(LoginRequestDto loginRequest);

        /// <summary>
        /// Registers a new user asynchronously.
        /// </summary>
        /// <param name="registerRequest">The register request data transfer object.</param>
        /// <returns>The result of the login operation which may be a token or indicate additional required steps.</returns>
        Task<LoginSuccessDto> RegisterAsync(RegisterRequestDto registerRequest);

        /// <summary>
        /// Logs out the current user asynchronously.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task LogOutAsync();

        /// <summary>
        /// Changes the password of the requesting user.
        /// </summary>
        /// <param name="requestDto">The data transfer object containing the password change information.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task ChangePasswordAsync(ChangePasswordRequestDto requestDto);

        /// <summary>
        /// Starts the email change process for the logged-in user.
        /// </summary>
        /// <param name="requestDto">The data transfer object containing the new email.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        /// <exception cref="UserFriendlyApiException">Thrown when the email change fails.</exception>
        Task ChangeEmailAsync(ChangeEmailRequestDto requestDto);

        /// <summary>
        /// Confirms the email change for the specified user.
        /// </summary>
        /// <param name="requestDto">The data transfer object containing the new email and the confirmation code.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        /// <exception cref="UserFriendlyApiException">Thrown when the email confirmation fails.</exception>
        Task ConfirmEmailChangeAsync(ConfirmEmailChangeRequestDto requestDto);

        /// <summary>
        /// Resets the password of a user asynchronously.
        /// </summary>
        /// <param name="resetPasswordRequest">The reset password request data transfer object.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task ResetPasswordAsync(ResetPasswordRequestDto resetPasswordRequest);

        /// <summary>
        /// Sets a new password for a user asynchronously.
        /// </summary>
        /// <param name="newPasswordRequest">The new password request data transfer object.</param>
        /// <returns>The result of the login operation which may be a token or indicate additional required steps.</returns>
        Task<LoginSuccessDto> NewPasswordAsync(NewPasswordRequestDto newPasswordRequest);

        /// <summary>
        /// Verifies a 2FA code asynchronously.
        /// </summary>
        /// <param name="verifyCodeRequest">The verify code request data transfer object.</param>
        /// <returns>The access token.</returns>
        Task<string> VerifyCodeAsync(VerifyCodeRequestDto verifyCodeRequest);

        /// <summary>
        /// Gets the access token of the current user asynchronously.
        /// </summary>
        /// <returns>A new access token.</returns>
        Task<string> GetAccessTokenAsync();

        /// <summary>
        /// Requests email verification for a user asynchronously.
        /// </summary>
        /// <param name="verificationEmailRequest">Contains the email address of the user.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task RequestVerificationEmailAsync(SendVerificationEmailRequestDto verificationEmailRequest);

        /// <summary>
        /// Verifies an email asynchronously.
        /// </summary>
        /// <param name="verifyEmailRequest">The verify email request data transfer object.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task VerifyEmailAsync(VerifyEmailRequestDto verifyEmailRequest);

        /// <summary>
        /// Requests a magic link the user can use to log in.
        /// </summary>
        /// <param name="magicLinkRequest">The request data transfer object containing the email address.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task RequestMagicLinkEmailAsync(SendMagicLinkRequestDto magicLinkRequest);

        /// <summary>  
        /// Retrieves a list of devices for the requesting user.  
        /// </summary>  
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of <see cref="DeviceDto"/>.</returns>  
        Task<IList<DeviceDto>> GetDevicesAsync();

        /// <summary>  
        /// Revokes a device for the requesting user.  
        /// </summary>  
        /// <param name="deviceId">The ID of the device to be revoked.</param>  
        /// <returns>A task that represents the asynchronous operation.</returns>  
        Task RevokeDeviceAsync(string deviceId);

        /// <summary>
        /// Configures the external authentication properties for the specified provider.
        /// </summary>
        /// <param name="provider">The external authentication provider.</param>
        /// <param name="redirectUrl">The URL to redirect to after authentication.</param>
        /// <returns>The authentication properties.</returns>
        AuthenticationProperties ConfigureExternalAuthenticationProperties(string provider, string redirectUrl);

        /// <summary>
        /// Handles the callback from an external authentication provider.
        /// </summary>
        /// <returns>The confirmation token required to complete the process.</returns>
        Task<string> ExternalLoginCallbackAsync();

        /// <summary>
        /// Completes the external login registration.
        /// </summary>
        /// <param name="confirmationToken">The confirmation token for the login process.</param>
        /// <returns>The result of the external login.</returns>
        Task<ExternalLoginSuccessDto> GetExternalLoginResultAsync(string confirmationToken);

        /// <summary>
        /// Completes the external login registration.
        /// </summary>
        /// <param name="confirmationToken">The confirmation token for the login process.</param>
        /// <param name="requestDto">The completion request DTO.</param>
        /// <returns>The login result.</returns>
        Task<LoginSuccessDto> CompleteExternalLoginAsync(string confirmationToken, ExternalLoginRequestDto requestDto);

        /// <summary>
        /// Completes the external login registration.
        /// </summary>
        /// <param name="confirmationToken">The confirmation token for the login process.</param>
        /// <param name="requestDto">The completion request DTO.</param>
        /// <returns>The login result.</returns>
        Task<LoginSuccessDto> CompleteExternalLoginRegistrationAsync(string confirmationToken, ExternalLoginRegisterRequestDto requestDto);
    }
}