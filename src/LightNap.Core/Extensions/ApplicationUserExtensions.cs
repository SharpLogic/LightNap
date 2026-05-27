using LightNap.Core.Data.Entities;
using LightNap.Core.Identity.Dto.Request;
using LightNap.Core.Profile.Dto.Request;
using LightNap.Core.Profile.Dto.Response;
using LightNap.Core.Users.Dto.Request;
using LightNap.Core.Users.Dto.Response;

namespace LightNap.Core.Extensions
{
    /// <summary>
    /// Provides extension methods for working with ApplicationUser objects.
    /// </summary>
    public static class ApplicationUserExtensions
    {
        extension(ApplicationUser user)
        {
            /// <summary>
            /// Converts an ApplicationUser object to a ProfileDto object representing the logged-in user's profile.
            /// </summary>
            /// <returns>A ProfileDto object representing the logged-in user's profile.</returns>
            internal ProfileDto ToLoggedInUserDto()
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
            /// <param name="dto">The UpdateProfileDto object containing the updated values.</param>
            internal void UpdateLoggedInUser(
                // Suppress IDE0060 warning for unused parameter 'dto'. Remove this if actually using the parameter.
#pragma warning disable IDE0060
                UpdateProfileRequestDto dto)
#pragma warning restore IDE0060
            {
                user.LastModifiedDate = DateTime.UtcNow;

                // Update other fields from the DTO.
            }

            /// <summary>
            /// Converts an ApplicationUser object to an AdminUserDto object.
            /// </summary>
            /// <returns>An AdminUserDto object representing the ApplicationUser object.</returns>
            internal AdminUserDto ToAdminUserDto()
            {
                return new AdminUserDto()
                {
                    CreatedDate = user.CreatedDate,
                    LastModifiedDate = user.LastModifiedDate,
                    Email = user.Email!,
                    Id = user.Id,
                    LockoutEnd = user.LockoutEnd?.UtcDateTime,
                    UserName = user.UserName!
                };
            }

            /// <summary>
            /// Converts an ApplicationUser object to an PrivilegedUserDto object.
            /// </summary>
            /// <returns>An PrivilegedUserDto object representing the ApplicationUser object.</returns>
            internal PrivilegedUserDto ToPrivilegedUserDto()
            {
                return new PrivilegedUserDto()
                {
                    CreatedDate = user.CreatedDate,
                    Email = user.Email!,
                    Id = user.Id,
                    UserName = user.UserName!
                };
            }

            /// <summary>
            /// Converts an ApplicationUser object to an PublicUserDto object.
            /// </summary>
            /// <returns>An PublicUserDto object representing the ApplicationUser object.</returns>
            internal PublicUserDto ToPublicUserDto()
            {
                return new PublicUserDto()
                {
                    CreatedDate = user.CreatedDate,
                    Id = user.Id,
                    UserName = user.UserName!
                };
            }

            /// <summary>
            /// Updates the ApplicationUser object with the values from the UpdateAdminUserDto object.
            /// </summary>
            /// <param name="dto">The UpdateAdminUserDto object containing the updated values.</param>
            internal void UpdateAdminUserDto(
                // Suppress IDE0060 warning for unused parameter 'dto'. Remove this if actually using the parameter.
#pragma warning disable IDE0060
                AdminUpdateUserRequestDto dto)
#pragma warning restore IDE0060
            {
                user.LastModifiedDate = DateTime.UtcNow;

                // Update other fields from the DTO.
            }
        }

        extension(RegisterRequestDto dto)
        {
            /// <summary>
            /// Converts a RegisterRequestDto object to an ApplicationUser object.
            /// </summary>
            /// <param name="twoFactorEnabled">A boolean indicating if two-factor authentication is enabled.</param>
            /// <returns>An ApplicationUser object created from the registration details.</returns>
            public ApplicationUser ToCreate(bool twoFactorEnabled)
            {
                return new ApplicationUser()
                {
                    Email = dto.Email,
                    TwoFactorEnabled = twoFactorEnabled,
                    UserName = dto.UserName,
                };
            }
        }

        extension(ExternalLoginRegisterRequestDto dto)
        {
            /// <summary>
            /// Converts a ExternalLoginRegisterRequestDto object to an ApplicationUser object.
            /// </summary>
            /// <param name="twoFactorEnabled">A boolean indicating if two-factor authentication is enabled.</param>
            /// <returns>An ApplicationUser object created from the registration details.</returns>
            internal ApplicationUser ToCreate(bool twoFactorEnabled)
            {
                return new ApplicationUser()
                {
                    Email = dto.Email,
                    TwoFactorEnabled = twoFactorEnabled,
                    UserName = dto.UserName,
                };
            }
        }

        extension(IEnumerable<ApplicationUser> users)
        {
            /// <summary>
            /// Converts a collection of ApplicationUser objects to a list of AdminUserDto objects.
            /// </summary>
            /// <returns>A list of AdminUserDto objects representing the ApplicationUser objects.</returns>
            internal List<AdminUserDto> ToAdminUserDtoList()
            {
                return [.. users.Select(user => user.ToAdminUserDto())];
            }

            /// <summary>
            /// Converts a collection of ApplicationUser objects to a list of PrivilegedUserDto objects.
            /// </summary>
            /// <returns>A list of PrivilegedUserDto objects representing the ApplicationUser objects.</returns>
            internal List<PrivilegedUserDto> ToPrivilegedUserDtoList()
            {
                return [.. users.Select(user => user.ToPrivilegedUserDto())];
            }

            /// <summary>
            /// Converts a collection of ApplicationUser objects to a list of PublicUserDto objects.
            /// </summary>
            /// <returns>A list of PublicUserDto objects representing the ApplicationUser objects.</returns>
            internal List<PublicUserDto> ToPublicUserDtoList()
            {
                return [.. users.Select(user => user.ToPublicUserDto())];
            }
        }
    }
}
