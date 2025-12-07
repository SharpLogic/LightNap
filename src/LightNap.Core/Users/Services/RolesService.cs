using LightNap.Core.Api;
using LightNap.Core.Configuration.Authorization;
using LightNap.Core.Data;
using LightNap.Core.Data.Entities;
using LightNap.Core.Extensions;
using LightNap.Core.Identity.Dto.Response;
using LightNap.Core.Interfaces;
using LightNap.Core.Users.Dto.Response;
using LightNap.Core.Users.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace LightNap.Core.Users.Services
{
    /// <summary>
    /// Service for managing roles.
    /// </summary>
    public class RolesService(UserManager<ApplicationUser> userManager, ApplicationDbContext db, IUserContext userContext) : IRolesService
    {
        /// <summary>
        /// Retrieves all available roles.
        /// </summary>
        /// <returns>The list of roles.</returns>
        public IList<RoleDto> GetRoles()
        {
            userContext.AssertAuthenticated();

            return ApplicationRoles.All.ToDtoList();
        }

        /// <summary>
        /// Retrieves the roles for a user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>The list of roles for the user.</returns>
        public async Task<IList<string>> GetRolesForUserAsync(string userId)
        {
            userContext.AssertAdministrator();

            var user = await db.Users.FindAsync(userId) ?? throw new UserFriendlyApiException("The specified user was not found.");

            var roles = await userManager.GetRolesAsync(user);

            return roles;
        }

        /// <summary>
        /// Retrieves the users in a specific role.
        /// </summary>
        /// <param name="role">The role to search for.</param>
        /// <returns>The list of users in the specified role.</returns>
        public async Task<IList<AdminUserDto>> GetUsersInRoleAsync(string role)
        {
            userContext.AssertAdministrator();

            var users = await userManager.GetUsersInRoleAsync(role);
            return users.OrderBy(user => user.UserName).ToAdminUserDtoList();
        }

        /// <summary>
        /// Adds a user to a role.
        /// </summary>
        /// <param name="role">The role to add the user to.</param>
        /// <param name="userId">The ID of the user to add to the role.</param>
        public async Task AddUserToRoleAsync(string role, string userId)
        {
            userContext.AssertAdministrator();

            var user = await db.Users.FindAsync(userId) ?? throw new UserFriendlyApiException("The specified user was not found.");

            var result = await userManager.AddToRoleAsync(user, role);
            if (!result.Succeeded) { throw new UserFriendlyApiException(result.Errors.Select(error => error.Description)); }
        }

        /// <summary>
        /// Removes a user from a role.
        /// </summary>
        /// <param name="role">The role to remove the user from.</param>
        /// <param name="userId">The ID of the user to remove from the role.</param>
        public async Task RemoveUserFromRoleAsync(string role, string userId)
        {
            userContext.AssertAdministrator();

            if ((userId == userContext.GetUserId()) && (role == ApplicationRoles.Administrator.Name)) { throw new UserFriendlyApiException("You may not remove yourself from the Administrator role."); }

            var user = await db.Users.FindAsync(userId) ?? throw new UserFriendlyApiException("The specified user was not found.");
            var result = await userManager.RemoveFromRoleAsync(user, role);
            if (!result.Succeeded) { throw new UserFriendlyApiException(result.Errors.Select(error => error.Description)); }
        }
    }
}
