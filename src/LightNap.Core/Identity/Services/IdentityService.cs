using LightNap.Core.Api;
using LightNap.Core.Configuration;
using LightNap.Core.Data;
using LightNap.Core.Data.Entities;
using LightNap.Core.Email.Interfaces;
using LightNap.Core.Extensions;
using LightNap.Core.Identity.Dto.Request;
using LightNap.Core.Identity.Dto.Response;
using LightNap.Core.Identity.Interfaces;
using LightNap.Core.Identity.Models;
using LightNap.Core.Interfaces;
using LightNap.Core.Notifications.Dto.Request;
using LightNap.Core.Notifications.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LightNap.Core.Identity.Services
{
    /// <summary>
    /// Service for managing identity.
    /// </summary>
    public class IdentityService(ILogger<IdentityService> logger,
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ITokenService tokenService,
        IEmailService emailService,
        INotificationService notificationService,
        IOptions<ApplicationSettings> applicationSettings,
        ApplicationDbContext db,
        ICookieManager cookieManager,
        IUserContext userContext) : IIdentityService
    {
        /// <summary>
        /// Handles user login asynchronously.
        /// </summary>
        /// <param name="user">The application user.</param>
        /// <param name="rememberMe">Indicates whether to remember the user.</param>
        /// <param name="deviceDetails">The device details.</param>
        /// <returns>The login result DTO containing the access token or a flag indicating whether further steps are required.</returns>
        private async Task<LoginSuccessDto> HandleUserLoginAsync(ApplicationUser user, bool rememberMe, string deviceDetails)
        {
            if (applicationSettings.Value.RequireEmailVerification && !user.EmailConfirmed)
            {
                return new LoginSuccessDto() { Type = LoginSuccessType.EmailVerificationRequired };
            }

            if (user.TwoFactorEnabled)
            {
                string code = await userManager.GenerateTwoFactorTokenAsync(user, TokenOptions.DefaultEmailProvider);
                await emailService.SendTwoFactorAsync(user, code);
                return new LoginSuccessDto() { Type = LoginSuccessType.TwoFactorRequired };
            }

            await this.CreateRefreshTokenAsync(user, rememberMe, deviceDetails);
            return new LoginSuccessDto()
            {
                AccessToken = await tokenService.GenerateAccessTokenAsync(user),
                Type = LoginSuccessType.AccessToken
            };
        }

        /// <summary>
        /// Validates the refresh token asynchronously.
        /// </summary>
        /// <returns>The application user if the refresh token is valid; otherwise, null.</returns>
        private async Task<ApplicationUser?> ValidateRefreshTokenAsync()
        {
            string? refreshTokenCookie = cookieManager.GetCookie(Constants.Cookies.RefreshToken);
            if (refreshTokenCookie is null) { return null; }

            var refreshToken = await db.RefreshTokens.Include(token => token.User).FirstOrDefaultAsync(token => token.Token == refreshTokenCookie);
            if (refreshToken is null || refreshToken.IsRevoked || refreshToken.Expires < DateTime.UtcNow) { return null; }

            refreshToken.LastSeen = DateTime.UtcNow;
            refreshToken.IpAddress = userContext.GetIpAddress() ?? Constants.RefreshTokens.NoIpProvided;
            refreshToken.Expires = DateTime.UtcNow.AddDays(refreshToken.IsPersistent ? applicationSettings.Value.LogOutInactiveDeviceDays : (tokenService.ExpirationMinutes / (60.0 * 24)));
            refreshToken.Token = tokenService.GenerateRefreshToken();

            await db.SaveChangesAsync();

            cookieManager.SetCookie(Constants.Cookies.RefreshToken, refreshToken.Token, refreshToken.IsPersistent, refreshToken.Expires);

            return refreshToken.User;
        }

        /// <summary>
        /// Creates a refresh token asynchronously.
        /// </summary>
        /// <param name="user">The application user.</param>
        /// <param name="rememberMe">Indicates whether to remember the user.</param>
        /// <param name="deviceDetails">The device details.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        private async Task CreateRefreshTokenAsync(ApplicationUser user, bool rememberMe, string deviceDetails)
        {
            DateTime expires = DateTime.UtcNow.AddDays(rememberMe ? applicationSettings.Value.LogOutInactiveDeviceDays : tokenService.ExpirationMinutes / (60.0 * 24));
            string refreshToken = tokenService.GenerateRefreshToken();

            db.RefreshTokens.Add(
                new RefreshToken()
                {
                    Id = Guid.NewGuid().ToString(),
                    Token = refreshToken,
                    Expires = expires,
                    LastSeen = DateTime.UtcNow,
                    IpAddress = userContext.GetIpAddress() ?? Constants.RefreshTokens.NoIpProvided,
                    Details = deviceDetails,
                    IsPersistent = rememberMe,
                    UserId = user.Id
                });
            await db.SaveChangesAsync();

            cookieManager.SetCookie(Constants.Cookies.RefreshToken, refreshToken, rememberMe, expires);
        }

        /// <summary>
        /// Sends a verification email asynchronously.
        /// </summary>
        /// <param name="user">The application user.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        private async Task SendVerificationEmailAsync(ApplicationUser user)
        {
            string token = await userManager.GenerateEmailConfirmationTokenAsync(user);

            try
            {
                await emailService.SendEmailVerificationAsync(user, token);
            }
            catch (Exception e)
            {
                logger.LogError(e, "An error occurred while sending an email verification link to '{email}': {e}", user.Email, e);
                throw new UserFriendlyApiException("An unexpected error occurred while sending the email verification link.");
            }
        }

        /// <summary>
        /// Logs in a user.
        /// </summary>
        /// <param name="requestDto">The login request DTO.</param>
        /// <returns>The login result.</returns>
        public async Task<LoginSuccessDto> LogInAsync(LoginRequestDto requestDto)
        {
            ApplicationUser user = requestDto.Type switch
            {
                LoginType.Email or LoginType.MagicLink => await userManager.FindByEmailAsync(requestDto.Login) ?? throw new UserFriendlyApiException("Invalid email/password combination."),
                LoginType.UserName => await userManager.FindByNameAsync(requestDto.Login) ?? throw new UserFriendlyApiException("Invalid username/password combination."),
                _ => await userManager.FindByEmailAsync(requestDto.Login) ??
                                        await userManager.FindByNameAsync(requestDto.Login) ??
                                        throw new UserFriendlyApiException("Invalid login/password combination."),
            };
            if (await userManager.IsLockedOutAsync(user))
            {
                throw new UserFriendlyApiException("This account is locked.");
            }

            if (requestDto.Type == LoginType.MagicLink)
            {
                bool isValid = await userManager.VerifyUserTokenAsync(user, TokenOptions.DefaultProvider, Constants.Identity.MagicLinkTokenPurpose, requestDto.Password);
                if (!isValid)
                {
                    throw new UserFriendlyApiException("Invalid email/token combination.");
                }
            }
            else
            {
                var signInResult = await signInManager.CheckPasswordSignInAsync(user, requestDto.Password, true);
                if (!signInResult.Succeeded)
                {
                    if (signInResult.IsNotAllowed)
                    {
                        throw new UserFriendlyApiException("This account is not allowed to log in.");
                    }
                    throw new UserFriendlyApiException("Invalid login/password combination.");
                }
            }

            return await this.HandleUserLoginAsync(user, requestDto.RememberMe, requestDto.DeviceDetails);
        }

        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <param name="requestDto">The registration request DTO.</param>
        /// <returns>The login result.</returns>
        public async Task<LoginSuccessDto> RegisterAsync(RegisterRequestDto requestDto)
        {
            ApplicationUser user = requestDto.ToCreate(applicationSettings.Value.RequireTwoFactorForNewUsers);
            var result = await userManager.CreateAsync(user, requestDto.Password);
            if (!result.Succeeded)
            {
                if (result.Errors.Any()) { throw new UserFriendlyApiException(result.Errors.Select(error => error.Description)); }
                throw new UserFriendlyApiException("Unable to create user.");
            }

            if (!user.TwoFactorEnabled)
            {
                await emailService.SendRegistrationWelcomeAsync(user);
            }

            if (applicationSettings.Value.RequireEmailVerification)
            {
                await this.SendVerificationEmailAsync(user);
            }

            await notificationService.CreateSystemNotificationForRoleAsync(ApplicationRoles.Administrator.Name!,
                new CreateNotificationRequestDto()
                {
                    Type = NotificationType.AdministratorNewUserRegistration,
                    Data = new Dictionary<string, object>()
                    {
                        { "userId", user.Id }
                    }
                });

            logger.LogInformation("New user '{userName}' ('{email}') registered!", user.Email, user.UserName);

            return await this.HandleUserLoginAsync(user, requestDto.RememberMe, requestDto.DeviceDetails);
        }

        /// <summary>
        /// Logs out the current user.
        /// </summary>
        /// <returns>The success of the operation.</returns>
        public async Task LogOutAsync()
        {
            string? refreshTokenCookie = cookieManager.GetCookie(Constants.Cookies.RefreshToken) ?? throw new UserFriendlyApiException("You are not logged in");
            RefreshToken? refreshToken = await db.RefreshTokens.FirstOrDefaultAsync(token => token.Token == refreshTokenCookie) ?? throw new UserFriendlyApiException("You are not logged in");
            db.RefreshTokens.Remove(refreshToken);
            await db.SaveChangesAsync();
            cookieManager.RemoveCookie(Constants.Cookies.RefreshToken);
        }

        /// <summary>  
        /// Changes the password for the specified user.  
        /// </summary>  
        /// <param name="requestDto">The data transfer object containing the current and new passwords.</param>  
        /// <returns>A task that represents the asynchronous operation.</returns>  
        /// <exception cref="UserFriendlyApiException">Thrown when the new password does not match the confirmation password or if the password change fails.</exception>  
        public async Task ChangePasswordAsync(ChangePasswordRequestDto requestDto)
        {
            if (requestDto.NewPassword != requestDto.ConfirmNewPassword) { throw new UserFriendlyApiException("New password does not match confirmation password."); }

            ApplicationUser user = await userManager.FindByIdAsync(userContext.GetUserId()) ?? throw new UserFriendlyApiException("Unable to change password.");

            var result = await userManager.ChangePasswordAsync(user, requestDto.CurrentPassword, requestDto.NewPassword);
            if (!result.Succeeded)
            {
                if (result.Errors.Any()) { throw new UserFriendlyApiException(result.Errors.Select(error => error.Description)); }
                throw new UserFriendlyApiException("Unable to change password.");
            }
        }

        /// <summary>
        /// Starts the email change process for the logged-in user.
        /// </summary>
        /// <param name="requestDto">The data transfer object containing the new email.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        /// <exception cref="UserFriendlyApiException">Thrown when the email change fails.</exception>
        public async Task ChangeEmailAsync(ChangeEmailRequestDto requestDto)
        {
            var user = await userManager.FindByIdAsync(userContext.GetUserId()) ?? throw new UserFriendlyApiException("Unable to change email.");
            var token = await userManager.GenerateChangeEmailTokenAsync(user, requestDto.NewEmail);

            try
            {
                await emailService.SendChangeEmailAsync(user, requestDto.NewEmail, token);
            }
            catch (Exception e)
            {
                logger.LogError(e, "An error occurred while sending an email change link to '{email}': {e}", user.Email, e);
                throw new UserFriendlyApiException("An unexpected error occurred while sending the email change link.");
            }
        }

        /// <summary>
        /// Confirms the email change for the specified user.
        /// </summary>
        /// <param name="requestDto">The data transfer object containing the new email and the confirmation code.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        /// <exception cref="UserFriendlyApiException">Thrown when the email confirmation fails.</exception>
        public async Task ConfirmEmailChangeAsync(ConfirmEmailChangeRequestDto requestDto)
        {
            var user = await userManager.FindByIdAsync(userContext.GetUserId()) ?? throw new UserFriendlyApiException("Unable to confirm email change.");

            var result = await userManager.ChangeEmailAsync(user, requestDto.NewEmail, requestDto.Code);
            if (!result.Succeeded)
            {
                if (result.Errors.Any()) { throw new UserFriendlyApiException(result.Errors.Select(error => error.Description)); }
                throw new UserFriendlyApiException("Unable to confirm email change.");
            }

            user.EmailConfirmed = true;

            await userManager.UpdateAsync(user);
        }

        /// <summary>
        /// Resets the password for a user.
        /// </summary>
        /// <param name="requestDto">The reset password request DTO.</param>
        /// <returns>The success of the operation.</returns>
        public async Task ResetPasswordAsync(ResetPasswordRequestDto requestDto)
        {
            ApplicationUser? user = await userManager.FindByEmailAsync(requestDto.Email) ?? throw new UserFriendlyApiException("An account with this email was not found.");

            string token = await userManager.GeneratePasswordResetTokenAsync(user);

            try
            {
                await emailService.SendPasswordResetAsync(user, token);
            }
            catch (Exception e)
            {
                logger.LogError(e, "An error occurred while sending a password reset link to '{email}': {e}", user.Email, e);
                throw new UserFriendlyApiException("An unexpected error occurred while sending the password reset link.");
            }
        }

        /// <summary>
        /// Sets a new password for a user.
        /// </summary>
        /// <param name="requestDto">The new password request DTO.</param>
        /// <returns>The login result.</returns>
        public async Task<LoginSuccessDto> NewPasswordAsync(NewPasswordRequestDto requestDto)
        {
            ApplicationUser user = await userManager.FindByEmailAsync(requestDto.Email) ?? throw new UserFriendlyApiException("An account with this email was not found.");

            IdentityResult result = await userManager.ResetPasswordAsync(user, requestDto.Token, requestDto.Password);
            if (!result.Succeeded)
            {
                if (result.Errors.Any()) { throw new UserFriendlyApiException(result.Errors.Select(error => error.Description)); }
                throw new UserFriendlyApiException("Unable to set new password.");
            }

            if (!user.EmailConfirmed)
            {
                user.EmailConfirmed = true;
                await db.SaveChangesAsync();
            }

            return await this.HandleUserLoginAsync(user, requestDto.RememberMe, requestDto.DeviceDetails);
        }

        /// <summary>
        /// Verifies the two-factor authentication code.
        /// </summary>
        /// <param name="requestDto">The verify code request DTO.</param>
        /// <returns>The access token.</returns>
        public async Task<string> VerifyCodeAsync(VerifyCodeRequestDto requestDto)
        {
            ApplicationUser user = await userManager.FindByEmailAsync(requestDto.Login) ?? await userManager.FindByNameAsync(requestDto.Login) ?? throw new UserFriendlyApiException("An account with this email was not found.");
            if (!await userManager.VerifyTwoFactorTokenAsync(user, TokenOptions.DefaultEmailProvider, requestDto.Code))
            {
                throw new UserFriendlyApiException("Unable to verify code. Please try again or log in again to resend a new code.");
            }

            if (!user.EmailConfirmed)
            {
                user.EmailConfirmed = true;
                await userManager.UpdateAsync(user);
                await emailService.SendRegistrationWelcomeAsync(user);
            }

            await this.CreateRefreshTokenAsync(user, requestDto.RememberMe, requestDto.DeviceDetails);

            return await tokenService.GenerateAccessTokenAsync(user);
        }

        /// <summary>
        /// Gets a new access token using the refresh token.
        /// </summary>
        /// <returns>The access token or an empty string if the user is not logged in.</returns>
        public async Task<string> GetAccessTokenAsync()
        {
            var user = await this.ValidateRefreshTokenAsync();
            if (user is null) { return string.Empty; }

            if (!await signInManager.CanSignInAsync(user)) { throw new UserFriendlyApiException("This account may not sign in."); }

            return await tokenService.GenerateAccessTokenAsync(user);
        }

        /// <summary>
        /// Requests an email verification email to be sent to the user.
        /// </summary>
        /// <param name="requestDto">Contains the email address of the user.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task RequestVerificationEmailAsync(SendVerificationEmailRequestDto requestDto)
        {
            ApplicationUser user = await userManager.FindByEmailAsync(requestDto.Email) ?? throw new UserFriendlyApiException("An account with this email was not found.");
            if (user.EmailConfirmed) { throw new UserFriendlyApiException("This email is already verified."); }
            await this.SendVerificationEmailAsync(user);
        }

        /// <summary>
        /// Verifies an email verification code.
        /// </summary>
        /// <param name="requestDto">The details to verify.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task VerifyEmailAsync(VerifyEmailRequestDto requestDto)
        {
            ApplicationUser user = await userManager.FindByEmailAsync(requestDto.Email) ?? throw new UserFriendlyApiException("An account with this email was not found.");
            if (user.EmailConfirmed) { throw new UserFriendlyApiException("This email is already verified."); }
            IdentityResult result = await userManager.ConfirmEmailAsync(user, requestDto.Code);
            if (!result.Succeeded)
            {
                if (result.Errors.Any()) { throw new UserFriendlyApiException(result.Errors.Select(error => error.Description)); }
                throw new UserFriendlyApiException("Unable to verify email.");
            }
        }

        /// <summary>
        /// Requests a magic link the user can use to log in.
        /// </summary>
        /// <param name="requestDto">Contains the email address of the user.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task RequestMagicLinkEmailAsync(SendMagicLinkRequestDto requestDto)
        {
            ApplicationUser user = await userManager.FindByEmailAsync(requestDto.Email) ?? throw new UserFriendlyApiException("An account with this email was not found.");

            string token = await userManager.GenerateUserTokenAsync(user, TokenOptions.DefaultProvider, Constants.Identity.MagicLinkTokenPurpose);

            await emailService.SendMagicLinkAsync(user, token);
        }

        /// <summary>  
        /// Retrieves the list of devices for the specified user.  
        /// </summary>  
        /// <returns>A list of devices associated with the user.</returns>  
        public async Task<IList<DeviceDto>> GetDevicesAsync()
        {
            userContext.AssertAuthenticated();

            var tokens = await db.RefreshTokens
                            .Where(token => token.UserId == userContext.GetUserId() && !token.IsRevoked && token.Expires > DateTime.UtcNow)
                            .OrderByDescending(device => device.Expires)
                            .ToListAsync();

            return tokens.ToDtoList();
        }

        /// <summary>  
        /// Revokes a device for the specified user.  
        /// </summary>  
        /// <param name="deviceId">The ID of the device to be revoked.</param>  
        /// <returns>A task that represents the asynchronous operation.</returns>  
        public async Task RevokeDeviceAsync(string deviceId)
        {
            userContext.AssertAuthenticated();

            var token = await db.RefreshTokens.FindAsync(deviceId) ?? throw new UserFriendlyApiException("Device not found.");
            if (token.UserId != userContext.GetUserId()) { throw new UserFriendlyApiException("Device not found."); }

            token.IsRevoked = true;
            await db.SaveChangesAsync();
        }

        /// <summary>
        /// Removes all expired refresh tokens from the data store.
        /// </summary>
        /// <remarks>This method is intended to clean up expired refresh tokens to maintain the integrity
        /// of the token store  and prevent unnecessary storage usage. It does not affect active or unexpired
        /// tokens.</remarks>
        public async Task PurgeExpiredRefreshTokens()
        {
            userContext.AssertAdministrator();

            logger.LogInformation("Starting with {count} refresh tokens", await db.RefreshTokens.CountAsync());

            const int batchSize = 100;

            int deletedCount = 0;

            while (true)
            {
                var expiredTokens = await db.RefreshTokens
                    .Where(token => token.Expires < DateTime.UtcNow)
                    .OrderByDescending(token => token.Id)
                    .Take(batchSize)
                    .ToListAsync();
                if (expiredTokens.Count == 0) { break; }

                db.RefreshTokens.RemoveRange(expiredTokens);
                deletedCount += await db.SaveChangesAsync();
            }

            logger.LogInformation("Deleted {deletedCount} expired refresh tokens", deletedCount);

            // It's possible that some may have been created since we started.
            logger.LogInformation("Finished with {count} refresh tokens", await db.RefreshTokens.CountAsync());
        }
    }
}
