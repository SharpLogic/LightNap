using LightNap.Core.Profile.Dto.Request;
using LightNap.Core.Profile.Dto.Response;

namespace LightNap.Core.Profile.Interfaces
{
    /// <summary>
    /// Service for managing the logged-in user's profile.
    /// </summary>
    public interface IProfileService
    {
        /// <summary>
        /// Retrieves the profile of the requesting user.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="ProfileDto"/> with the user's profile.</returns>
        Task<ProfileDto> GetProfileAsync();

        /// <summary>
        /// Updates the profile of the requesting user.
        /// </summary>
        /// <param name="requestDto">The data transfer object containing the profile update information.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="ProfileDto"/> with the updated profile.</returns>
        Task<ProfileDto> UpdateProfileAsync(UpdateProfileRequestDto requestDto);
    }
}