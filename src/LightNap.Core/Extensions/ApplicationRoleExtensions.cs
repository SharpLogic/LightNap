using LightNap.Core.Data.Entities;
using LightNap.Core.Identity.Dto.Response;

namespace LightNap.Core.Extensions
{
    /// <summary>
    /// Provides extension methods for converting ApplicationRole objects to RoleDto objects.
    /// </summary>
    internal static class ApplicationRoleExtensions
    {
        extension(ApplicationRole role)
        {
            /// <summary>
            /// Converts an ApplicationRole object to a RoleDto object.
            /// </summary>
            /// <returns>A RoleDto object.</returns>
            public RoleDto ToDto()
            {
                return new RoleDto()
                {
                    Description = role.Description,
                    DisplayName = role.DisplayName,
                    Name = role.Name!
                };
            }
        }

        extension(IEnumerable<ApplicationRole> roles)
        {
            /// <summary>
            /// Converts a collection of ApplicationRole objects to a list of RoleDto objects.
            /// </summary>
            /// <returns>A list of RoleDto objects.</returns>
            public List<RoleDto> ToDtoList()
            {
                return [.. roles.Select(role => role.ToDto())];
            }
        }
    }
}
