using LightNap.Core.Administrator.Dto.Request;
using LightNap.Core.Administrator.Dto.Response;
using LightNap.Core.Data.Entities;
using LightNap.Core.Profile.Dto.Request;
using LightNap.Core.Profile.Dto.Response;

namespace LightNap.Core.Extensions
{
    /// <summary>
    /// Provides extension methods for working with ApplicationUser objects.
    /// </summary>
    public static class ApplicationUserExtensions
    {
        /// <summary>
        /// Converts an ApplicationUser object to a ProfileDto object representing the logged-in user's profile.
        /// </summary>
        /// <param name="user">The ApplicationUser object to convert.</param>
        /// <returns>A ProfileDto object representing the logged-in user's profile.</returns>
        public static ProfileDto ToLoggedInUserDto(this ApplicationUser user)
        {
            return new ProfileDto()
            {
                Email = user.Email!,
                Id = user.Id,
                UserName = user.UserName!
            };
        }

        /// <summary>
        /// Updates the ApplicationUser object with the values from the UpdateProfileDto object.
        /// </summary>
        /// <param name="user">The ApplicationUser object to update.</param>
        /// <param name="dto">The UpdateProfileDto object containing the updated values.</param>
        public static void UpdateLoggedInUser(this ApplicationUser user,
            // Suppress IDE0060 warning for unused parameter 'dto'. Remove this if actually using the parameter.
#pragma warning disable IDE0060
            UpdateProfileDto dto)
#pragma warning restore IDE0060
        {
            user.LastModifiedDate = DateTime.UtcNow;

            // Update other fields from the DTO.
        }

        /// <summary>
        /// Converts an ApplicationUser object to an AdminUserDto object.
        /// </summary>
        /// <param name="user">The ApplicationUser object to convert.</param>
        /// <returns>An AdminUserDto object representing the ApplicationUser object.</returns>
        public static AdminUserDto ToAdminUserDto(this ApplicationUser user)
        {
            long? lockoutEnd = (user.LockoutEnd.GetValueOrDefault(DateTimeOffset.MinValue) < DateTimeOffset.UtcNow) ? null : user.LockoutEnd?.ToUnixTimeMilliseconds();

            return new AdminUserDto()
            {
                CreatedDate = new DateTimeOffset(user.CreatedDate).ToUnixTimeMilliseconds(),
                LastModifiedDate = new DateTimeOffset(user.LastModifiedDate).ToUnixTimeMilliseconds(),
                Email = user.Email!,
                Id = user.Id,
                LockoutEnd = lockoutEnd,
                UserName = user.UserName!
            };
        }

        /// <summary>
        /// Converts a collection of ApplicationUser objects to a list of AdminUserDto objects.
        /// </summary>
        /// <param name="users">The collection of ApplicationUser objects to convert.</param>
        /// <returns>A list of AdminUserDto objects representing the ApplicationUser objects.</returns>
        public static List<AdminUserDto> ToAdminUserDtoList(this IEnumerable<ApplicationUser> users)
        {
            return users.Select(user => user.ToAdminUserDto()).ToList();
        }

        /// <summary>
        /// Updates the ApplicationUser object with the values from the UpdateAdminUserDto object.
        /// </summary>
        /// <param name="user">The ApplicationUser object to update.</param>
        /// <param name="dto">The UpdateAdminUserDto object containing the updated values.</param>
        public static void UpdateAdminUserDto(this ApplicationUser user,
            // Suppress IDE0060 warning for unused parameter 'dto'. Remove this if actually using the parameter.
#pragma warning disable IDE0060
            UpdateAdminUserDto dto)
#pragma warning restore IDE0060

        {
            user.LastModifiedDate = DateTime.UtcNow;

            // Update other fields from the DTO.
        }
    }
}