using LightNap.Core.Administrator.Dto.Request;
using LightNap.Core.Administrator.Dto.Response;
using LightNap.Core.Administrator.Interfaces;
using LightNap.Core.Administrator.Models;
using LightNap.Core.Api;
using LightNap.Core.Data;
using LightNap.Core.Data.Entities;
using LightNap.Core.Extensions;
using LightNap.Core.Identity.Dto.Response;
using LightNap.Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LightNap.Core.Administrator.Services
{
    /// <summary>
    /// Service for managing administrator-related operations.
    /// </summary>
    public class AdministratorService(UserManager<ApplicationUser> userManager, ApplicationDbContext db, IUserContext userContext) : IAdministratorService
    {
        /// <summary>
        /// Retrieves a user by ID.
        /// </summary>
        /// <param name="userId">The ID of the user to retrieve.</param>
        /// <returns>The user details or null if not found.</returns>
        public async Task<AdminUserDto?> GetUserAsync(string userId)
        {
            var user = await db.Users.FindAsync(userId);
            return user?.ToAdminUserDto();
        }

        /// <summary>
        /// Searches for users based on the specified criteria.
        /// </summary>
        /// <param name="requestDto">The search criteria.</param>
        /// <returns>The list of users matching the criteria.</returns>
        public async Task<PagedResponse<AdminUserDto>> SearchUsersAsync(SearchAdminUsersRequestDto requestDto)
        {
            IQueryable<ApplicationUser> query = db.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(requestDto.Email))
            {
                query = query.Where(user => EF.Functions.Like(user.NormalizedEmail!, $"%{requestDto.Email.ToUpper()}%"));
            }

            if (!string.IsNullOrWhiteSpace(requestDto.UserName))
            {
                query = query.Where(user => EF.Functions.Like(user.NormalizedUserName!, $"%{requestDto.UserName.ToUpper()}%"));
            }

            query = requestDto.SortBy switch
            {
                ApplicationUserSortBy.Email => requestDto.ReverseSort ? query.OrderByDescending(user => user.Email) : query.OrderBy(user => user.Email),
                ApplicationUserSortBy.UserName => requestDto.ReverseSort ? query.OrderByDescending(user => user.UserName) : query.OrderBy(user => user.UserName),
                ApplicationUserSortBy.CreatedDate => requestDto.ReverseSort ? query.OrderByDescending(user => user.CreatedDate) : query.OrderBy(user => user.CreatedDate),
                ApplicationUserSortBy.LastModifiedDate => requestDto.ReverseSort ? query.OrderByDescending(user => user.LastModifiedDate) : query.OrderBy(user => user.LastModifiedDate),
                _ => throw new ArgumentException("Invalid sort field: '{sortBy}'", requestDto.SortBy.ToString()),
            };
            int totalCount = await query.CountAsync();

            if (requestDto.PageNumber > 1)
            {
                query = query.Skip((requestDto.PageNumber - 1) * requestDto.PageSize);
            }

            var users = await query.Take(requestDto.PageSize).Select(user => user.ToAdminUserDto()).ToListAsync();

            return new PagedResponse<AdminUserDto>(users, requestDto.PageNumber, requestDto.PageSize, totalCount);
        }

        /// <summary>
        /// Updates a user.
        /// </summary>
        /// <param name="userId">The ID of the user to update.</param>
        /// <param name="requestDto">The updated user information.</param>
        /// <returns>The updated user details.</returns>
        public async Task<AdminUserDto> UpdateUserAsync(string userId, UpdateAdminUserDto requestDto)
        {
            var user = await db.Users.FindAsync(userId);
            if (user is null) { throw new UserFriendlyApiException("The specified user was not found."); }

            user.UpdateAdminUserDto(requestDto);

            await db.SaveChangesAsync();

            return user.ToAdminUserDto();
        }

        /// <summary>
        /// Deletes a user.
        /// </summary>
        /// <param name="userId">The ID of the user to delete.</param>
        public async Task DeleteUserAsync(string userId)
        {
            var user = await db.Users.FindAsync(userId);
            if (user is null) { throw new UserFriendlyApiException("The specified user was not found."); }

            if (await userManager.IsInRoleAsync(user, ApplicationRoles.Administrator.Name!)) { throw new UserFriendlyApiException("You may not delete an Administrator."); }

            db.Users.Remove(user);

            await db.SaveChangesAsync();
        }

        /// <summary>
        /// Retrieves all available roles.
        /// </summary>
        /// <returns>The list of roles.</returns>
        public IList<RoleDto> GetRoles()
        {
            return ApplicationRoles.All.ToDtoList();
        }

        /// <summary>
        /// Retrieves the roles for a user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>The list of roles for the user.</returns>
        public async Task<IList<string>> GetRolesForUserAsync(string userId)
        {
            var user = await db.Users.FindAsync(userId);
            if (user is null) { throw new UserFriendlyApiException("The specified user was not found."); }

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
            var users = await userManager.GetUsersInRoleAsync(role);
            return users.ToAdminUserDtoList();
        }

        /// <summary>
        /// Adds a user to a role.
        /// </summary>
        /// <param name="role">The role to add the user to.</param>
        /// <param name="userId">The ID of the user to add to the role.</param>
        public async Task AddUserToRoleAsync(string role, string userId)
        {
            var user = await db.Users.FindAsync(userId);
            if (user is null) { throw new UserFriendlyApiException("The specified user was not found."); }

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
            if ((userId == userContext.GetUserId()) && (role == ApplicationRoles.Administrator.Name)) { throw new UserFriendlyApiException("You may not remove yourself from the Administrator role."); }

            var user = await db.Users.FindAsync(userId);
            if (user is null) { throw new UserFriendlyApiException("The specified user was not found."); }

            var result = await userManager.RemoveFromRoleAsync(user, role);
            if (!result.Succeeded) { throw new UserFriendlyApiException(result.Errors.Select(error => error.Description)); }
        }

        /// <summary>
        /// Locks a user account.
        /// </summary>
        /// <param name="userId">The ID of the user to lock.</param>
        public async Task LockUserAccountAsync(string userId)
        {
            var user = await db.Users.FindAsync(userId);
            if (user is null) { throw new UserFriendlyApiException("The specified user was not found."); }

            user.LockoutEnd = DateTimeOffset.MaxValue;

            await db.SaveChangesAsync();
        }

        /// <summary>
        /// Unlocks a user account.
        /// </summary>
        /// <param name="userId">The ID of the user to unlock.</param>
        public async Task UnlockUserAccountAsync(string userId)
        {
            var user = await db.Users.FindAsync(userId);
            if (user is null) { throw new UserFriendlyApiException("The specified user was not found."); }

            user.LockoutEnd = null;

            await db.SaveChangesAsync();
        }
    }
}
