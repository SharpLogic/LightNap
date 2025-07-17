using LightNap.Core.Identity.Dto.Response;
using LightNap.Core.Users.Dto.Response;

namespace LightNap.Core.Users.Interfaces
{
    /// <summary>  
    /// Interface for managing roles.
    /// </summary>  
    public interface IRolesService
    {
        /// <summary>  
        /// Gets all roles.  
        /// </summary>  
        /// <returns>The list of roles.</returns>  
        IList<RoleDto> GetRoles();

        /// <summary>  
        /// Gets roles for a user asynchronously by user ID.  
        /// </summary>  
        /// <param name="userId">The user ID.</param>  
        /// <returns>A task that represents the asynchronous operation. The task result contains the list of roles for the user.</returns>  
        Task<IList<string>> GetRolesForUserAsync(string userId);

        /// <summary>  
        /// Gets users in a role asynchronously by role name.  
        /// </summary>  
        /// <param name="role">The role name.</param>  
        /// <returns>A task that represents the asynchronous operation. The task result contains the list of users in the role.</returns>  
        Task<IList<AdminUserDto>> GetUsersInRoleAsync(string role);

        /// <summary>  
        /// Adds a user to a role asynchronously.  
        /// </summary>  
        /// <param name="role">The role name.</param>  
        /// <param name="userId">The user ID.</param>  
        /// <returns>A task that represents the asynchronous operation.</returns>  
        Task AddUserToRoleAsync(string role, string userId);

        /// <summary>  
        /// Removes a user from a role asynchronously.  
        /// </summary>  
        /// <param name="role">The role name.</param>  
        /// <param name="userId">The user ID.</param>  
        /// <returns>A task that represents the asynchronous operation.</returns>  
        Task RemoveUserFromRoleAsync(string role, string userId);
    }
}