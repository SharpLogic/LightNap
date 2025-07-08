using LightNap.Core.Api;
using LightNap.Core.Data;
using LightNap.Core.Data.Entities;
using LightNap.Core.Data.Extensions;
using LightNap.Core.Email.Interfaces;
using LightNap.Core.Interfaces;
using LightNap.Core.Profile.Dto.Request;
using LightNap.Core.Profile.Dto.Response;
using LightNap.Core.Profile.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LightNap.Core.Profile.Services
{
    /// <summary>  
    /// Service for managing user profiles.  
    /// </summary>  
    public class ProfileService(ILogger<ProfileService> logger, ApplicationDbContext db, UserManager<ApplicationUser> userManager, IUserContext userContext,
        IEmailService emailService) : IProfileService
    {
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
        /// Retrieves the profile of the specified user.  
        /// </summary>  
        /// <returns>A <see cref="ProfileDto"/> containing the user's profile.</returns>  
        public async Task<ProfileDto> GetProfileAsync()
        {
            var user = await db.Users.FindAsync(userContext.GetUserId()) ?? throw new UserFriendlyApiException("Please log in");
            return user.ToLoggedInUserDto();
        }

        /// <summary>  
        /// Updates the profile of the specified user.  
        /// </summary>  
        /// <param name="requestDto">The data transfer object containing the updated profile information.</param>  
        /// <returns>A <see cref="ProfileDto"/> with the updated profile.</returns>  
        public async Task<ProfileDto> UpdateProfileAsync(UpdateProfileDto requestDto)
        {
            var user = await db.Users.FindAsync(userContext.GetUserId()) ?? throw new UserFriendlyApiException("Unable to update profile.");

            user.UpdateLoggedInUser(requestDto);

            await db.SaveChangesAsync();

            return user.ToLoggedInUserDto();
        }

        /// <summary>  
        /// Retrieves the settings of the specified user.  
        /// </summary>  
        /// <returns>A <see cref="BrowserSettingsDto"/> containing the user's settings.</returns>  
        public async Task<BrowserSettingsDto> GetSettingsAsync()
        {
            var user = await db.Users.FindAsync(userContext.GetUserId()) ?? throw new UserFriendlyApiException("Unable to load settings");
            return user.BrowserSettings;
        }

        /// <summary>  
        /// Updates the settings of the specified user.  
        /// </summary>  
        /// <param name="requestDto">The data transfer object containing the updated settings information.</param>  
        /// <returns>A task that represents the asynchronous operation.</returns>  
        public async Task UpdateSettingsAsync(BrowserSettingsDto requestDto)
        {
            var user = await db.Users.FindAsync(userContext.GetUserId()) ?? throw new UserFriendlyApiException("Unable to update settings");
            user.BrowserSettings = requestDto;
            await db.SaveChangesAsync();
        }

        /// <summary>  
        /// Retrieves the list of devices for the specified user.  
        /// </summary>  
        /// <returns>A list of devices associated with the user.</returns>  
        public async Task<IList<DeviceDto>> GetDevicesAsync()
        {
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
            var token = await db.RefreshTokens.FindAsync(deviceId) ?? throw new UserFriendlyApiException("Device not found.");
            if (token.UserId != userContext.GetUserId()) { throw new UserFriendlyApiException("Device not found."); }

            token.IsRevoked = true;
            await db.SaveChangesAsync();
        }
    }
}
