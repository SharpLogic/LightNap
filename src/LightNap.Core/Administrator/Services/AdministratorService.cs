using LightNap.Core.Administrator.Dto.Request;
using LightNap.Core.Administrator.Dto.Response;
using LightNap.Core.Administrator.Interfaces;
using LightNap.Core.Administrator.Models;
using LightNap.Core.Api;
using LightNap.Core.Configuration;
using LightNap.Core.Data;
using LightNap.Core.Data.Entities;
using LightNap.Core.Data.Extensions;
using LightNap.Core.Identity.Dto.Response;
using LightNap.Core.Identity.Extensions;
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
        /// Throws if the user is not an administrator.
        /// </summary>
        /// <exception cref="UserFriendlyApiException"></exception>
        private void AssertUserIsAdministrator()
        {
            if (!userContext.IsAdministrator)
            {
                throw new UserFriendlyApiException("You must be an Administrator to perform this action.");
            }
        }

        /// <summary>
        /// Retrieves a user by ID.
        /// </summary>
        /// <param name="userId">The ID of the user to retrieve.</param>
        /// <returns>The user details or null if not found.</returns>
        public async Task<AdminUserDto?> GetUserAsync(string userId)
        {
            this.AssertUserIsAdministrator();

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
            this.AssertUserIsAdministrator();

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
            this.AssertUserIsAdministrator();

            var user = await db.Users.FindAsync(userId) ?? throw new UserFriendlyApiException("The specified user was not found.");

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
            this.AssertUserIsAdministrator();

            var user = await db.Users.FindAsync(userId) ?? throw new UserFriendlyApiException("The specified user was not found.");

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
            this.AssertUserIsAdministrator();

            return ApplicationRoles.All.ToDtoList();
        }

        /// <summary>
        /// Retrieves the roles for a user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>The list of roles for the user.</returns>
        public async Task<IList<string>> GetRolesForUserAsync(string userId)
        {
            this.AssertUserIsAdministrator();

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
            this.AssertUserIsAdministrator();

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
            this.AssertUserIsAdministrator();

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
            this.AssertUserIsAdministrator();

            if ((userId == userContext.GetUserId()) && (role == ApplicationRoles.Administrator.Name)) { throw new UserFriendlyApiException("You may not remove yourself from the Administrator role."); }

            var user = await db.Users.FindAsync(userId) ?? throw new UserFriendlyApiException("The specified user was not found.");
            var result = await userManager.RemoveFromRoleAsync(user, role);
            if (!result.Succeeded) { throw new UserFriendlyApiException(result.Errors.Select(error => error.Description)); }
        }

        /// <summary>
        /// Searches claims.
        /// </summary>
        /// <param name="requestDto">The search parameters.</param>
        /// <returns>The paginated list of claims.</returns>
        public async Task<PagedResponse<AdminClaimDto>> SearchClaimsAsync(SearchClaimsRequestDto requestDto)
        {
            this.AssertUserIsAdministrator();

            var query = db.UserClaims.AsQueryable();

            if (!string.IsNullOrWhiteSpace(requestDto.UserId))
            {
                query = query.Where(claim => claim.UserId == requestDto.UserId);
            }

            if (!string.IsNullOrWhiteSpace(requestDto.Type))
            {
                query = query.Where(claim => claim.ClaimType == requestDto.Type);
            }

            if (!string.IsNullOrWhiteSpace(requestDto.Value))
            {
                query = query.Where(claim => EF.Functions.Like(claim.ClaimValue, $"%{requestDto.Value}%"));
            }

            int totalCount = await query.CountAsync();

            if (requestDto.PageNumber > 1)
            {
                query = query.Skip((requestDto.PageNumber - 1) * requestDto.PageSize);
            }


            var claims = await query
                .OrderBy(claim => claim.UserId)
                .ThenBy(claim => claim.ClaimType)
                .ThenBy(claim => claim.ClaimValue)
                .ToListAsync();

            return new PagedResponse<AdminClaimDto>(claims.ToDtoList(), requestDto.PageNumber, requestDto.PageSize, totalCount);
        }

        /// <summary>
        /// Adds a claim to the specified user asynchronously.
        /// </summary>
        /// <remarks>This method associates the provided claim with the specified user. Ensure that the
        /// user exists and that the claim is valid before calling this method. The operation is performed
        /// asynchronously.</remarks>
        /// <param name="userId">The unique identifier of the user to whom the claim will be added. Cannot be null or empty.</param>
        /// <param name="claim">The claim to add to the user. Cannot be null.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task AddClaimToUserAsync(string userId, ClaimDto claim)
        {
            this.AssertUserIsAdministrator();
            var user = await db.Users.FindAsync(userId) ?? throw new UserFriendlyApiException("The specified user was not found.");
            var result = await userManager.AddClaimAsync(user, claim.ToClaim());
            if (!result.Succeeded) { throw new UserFriendlyApiException(result.Errors.Select(error => error.Description)); }
        }

        /// <summary>
        /// Removes a specific claim from the specified user.
        /// </summary>
        /// <remarks>This method removes the specified claim from the user's claim collection.  If the
        /// user does not have the specified claim, the operation will complete without making changes.</remarks>
        /// <param name="userId">The unique identifier of the user from whom the claim will be removed. Cannot be null or empty.</param>
        /// <param name="claim">The claim to be removed from the user. Cannot be null.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task RemoveClaimFromUserAsync(string userId, ClaimDto claim)
        {
            this.AssertUserIsAdministrator();
            var user = await db.Users.FindAsync(userId) ?? throw new UserFriendlyApiException("The specified user was not found.");
            var result = await userManager.RemoveClaimAsync(user, claim.ToClaim());
            if (!result.Succeeded) { throw new UserFriendlyApiException(result.Errors.Select(error => error.Description)); }
        }

        /// <summary>
        /// Locks a user account.
        /// </summary>
        /// <param name="userId">The ID of the user to lock.</param>
        public async Task LockUserAccountAsync(string userId)
        {
            this.AssertUserIsAdministrator();

            var user = await db.Users.FindAsync(userId) ?? throw new UserFriendlyApiException("The specified user was not found.");

            if (await userManager.IsInRoleAsync(user, ApplicationRoles.Administrator.Name!)) { throw new UserFriendlyApiException("You may not lock an Administrator account."); }

            user.LockoutEnd = DateTimeOffset.MaxValue;

            await db.SaveChangesAsync();
        }

        /// <summary>
        /// Unlocks a user account.
        /// </summary>
        /// <param name="userId">The ID of the user to unlock.</param>
        public async Task UnlockUserAccountAsync(string userId)
        {
            this.AssertUserIsAdministrator();

            var user = await db.Users.FindAsync(userId) ?? throw new UserFriendlyApiException("The specified user was not found.");

            user.LockoutEnd = null;

            await db.SaveChangesAsync();
        }
    }
}
