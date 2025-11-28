using LightNap.Core.Data.Entities;
using LightNap.Core.Identity.Dto.Response;

namespace LightNap.Core.Extensions
{
    /// <summary>
    /// Provides extension methods for converting ApplicationRole objects to RoleDto objects.
    /// </summary>
    public static class ApplicationRoleExtensions
    {
        /// <summary>
        /// Converts an ApplicationRole object to a RoleDto object.
        /// </summary>
        /// <param name="role">The ApplicationRole object to convert.</param>
        /// <returns>A RoleDto object.</returns>
        public static RoleDto ToDto(this ApplicationRole role)
        {
            return new RoleDto()
            {
                Description = role.Description,
                DisplayName = role.DisplayName,
                Name = role.Name!
            };
        }

        /// <summary>
        /// Converts a collection of ApplicationRole objects to a list of RoleDto objects.
        /// </summary>
        /// <param name="roles">The collection of ApplicationRole objects to convert.</param>
        /// <returns>A list of RoleDto objects.</returns>
        public static List<RoleDto> ToDtoList(this IEnumerable<ApplicationRole> roles)
        {
            return [.. roles.Select(role => role.ToDto())];
        }
    }
}