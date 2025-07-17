using LightNap.Core.Api;
using LightNap.Core.Data;
using LightNap.Core.Extensions;
using LightNap.Core.Interfaces;
using LightNap.Core.Profile.Dto.Request;
using LightNap.Core.Profile.Dto.Response;
using LightNap.Core.Profile.Interfaces;

namespace LightNap.Core.Profile.Services
{
    /// <summary>  
    /// Service for managing user profiles.  
    /// </summary>  
    public class ProfileService(ApplicationDbContext db, IUserContext userContext) : IProfileService
    {
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
        public async Task<ProfileDto> UpdateProfileAsync(UpdateProfileRequestDto requestDto)
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
    }
}
