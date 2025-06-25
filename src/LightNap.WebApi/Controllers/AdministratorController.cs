using LightNap.Core.Administrator.Dto.Request;
using LightNap.Core.Administrator.Dto.Response;
using LightNap.Core.Administrator.Interfaces;
using LightNap.Core.Api;
using LightNap.Core.Configuration;
using LightNap.Core.Identity.Dto.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LightNap.WebApi.Controllers
{
    /// <summary>
    /// Controller for managing administrative tasks.
    /// </summary>
    [ApiController]
    [Authorize(Roles = Constants.Roles.Administrator)]
    [Route("api/[controller]")]
    public class AdministratorController(IAdministratorService administratorService) : ControllerBase
    {
        /// <summary>
        /// Retrieves a user by ID.
        /// </summary>
        /// <param name="userId">The ID of the user to retrieve.</param>
        /// <returns>The user details.</returns>
        /// <response code="200">Returns the user details.</response>
        [HttpGet("users/{userId}")]
        [ProducesResponseType(typeof(ApiResponseDto<AdminUserDto?>), 200)]
        public async Task<ApiResponseDto<AdminUserDto?>> GetUser(string userId)
        {
            return new ApiResponseDto<AdminUserDto?>(await administratorService.GetUserAsync(userId));
        }

        /// <summary>
        /// Searches for users based on the specified criteria.
        /// </summary>
        /// <param name="requestDto">The search criteria.</param>
        /// <returns>The list of users matching the criteria.</returns>
        /// <response code="200">Returns the list of users.</response>
        [HttpPost("users/search")]
        [ProducesResponseType(typeof(ApiResponseDto<PagedResponse<AdminUserDto>>), 200)]
        [ProducesResponseType(400)]
        public async Task<ApiResponseDto<PagedResponse<AdminUserDto>>> SearchUsers(SearchAdminUsersRequestDto requestDto)
        {
            return new ApiResponseDto<PagedResponse<AdminUserDto>>(await administratorService.SearchUsersAsync(requestDto));
        }

        /// <summary>
        /// Updates a user.
        /// </summary>
        /// <param name="userId">The ID of the user to update.</param>
        /// <param name="requestDto">The updated user information.</param>
        /// <returns>The updated user details.</returns>
        /// <response code="200">Returns the updated user details.</response>
        [HttpPut("users/{userId}")]
        [ProducesResponseType(typeof(ApiResponseDto<AdminUserDto>), 200)]
        [ProducesResponseType(400)]
        public async Task<ApiResponseDto<AdminUserDto>> UpdateUser(string userId, UpdateAdminUserDto requestDto)
        {
            return new ApiResponseDto<AdminUserDto>(await administratorService.UpdateUserAsync(userId, requestDto));
        }

        /// <summary>
        /// Deletes a user.
        /// </summary>
        /// <param name="userId">The ID of the user to delete.</param>
        /// <returns>True if the user was successfully deleted.</returns>
        /// <response code="200">User successfully deleted.</response>
        /// <response code="400">If the user is an administrator and cannot be deleted.</response>
        [HttpDelete("users/{userId}")]
        [ProducesResponseType(typeof(ApiResponseDto<bool>), 200)]
        public async Task<ApiResponseDto<bool>> DeleteUser(string userId)
        {
            await administratorService.DeleteUserAsync(userId);
            return new ApiResponseDto<bool>(true);
        }

        /// <summary>
        /// Retrieves all available roles.
        /// </summary>
        /// <returns>The list of roles.</returns>
        /// <response code="200">Returns the list of roles.</response>
        [HttpGet("roles")]
        [ProducesResponseType(typeof(ApiResponseDto<IList<RoleDto>>), 200)]
        public ApiResponseDto<IList<RoleDto>> GetRoles()
        {
            return new ApiResponseDto<IList<RoleDto>>(administratorService.GetRoles());
        }

        /// <summary>
        /// Retrieves the roles for a user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>The list of roles for the user.</returns>
        /// <response code="200">Returns the list of roles.</response>
        [HttpGet("users/{userId}/roles")]
        [ProducesResponseType(typeof(ApiResponseDto<IList<string>>), 200)]
        public async Task<ApiResponseDto<IList<string>>> GetRolesForUser(string userId)
        {
            return new ApiResponseDto<IList<string>>(await administratorService.GetRolesForUserAsync(userId));
        }

        /// <summary>
        /// Retrieves the users in a specific role.
        /// </summary>
        /// <param name="role">The role to search for.</param>
        /// <returns>The list of users in the specified role.</returns>
        /// <response code="200">Returns the list of users.</response>
        [HttpGet("roles/{role}")]
        [ProducesResponseType(typeof(ApiResponseDto<IList<AdminUserDto>>), 200)]
        public async Task<ApiResponseDto<IList<AdminUserDto>>> GetUsersInRole(string role)
        {
            return new ApiResponseDto<IList<AdminUserDto>>(await administratorService.GetUsersInRoleAsync(role));
        }

        /// <summary>
        /// Adds a user to a role.
        /// </summary>
        /// <param name="role">The role to add the user to.</param>
        /// <param name="userId">The ID of the user to add to the role.</param>
        /// <returns>True if the user was successfully added to the role.</returns>
        /// <response code="200">User successfully added to the role.</response>
        /// <response code="400">If there was an error adding the user to the role.</response>
        [HttpPost("roles/{role}/{userId}")]
        [ProducesResponseType(typeof(ApiResponseDto<bool>), 200)]
        [ProducesResponseType(400)]
        public async Task<ApiResponseDto<bool>> AddUserToRole(string role, string userId)
        {
            await administratorService.AddUserToRoleAsync(role, userId);
            return new ApiResponseDto<bool>(true);
        }

        /// <summary>
        /// Removes a user from a role.
        /// </summary>
        /// <param name="role">The role to remove the user from.</param>
        /// <param name="userId">The ID of the user to remove from the role.</param>
        /// <returns>True if the user was successfully removed from the role.</returns>
        /// <response code="200">User successfully removed from the role.</response>
        /// <response code="400">If there was an error removing the user from the role.</response>
        [HttpDelete("roles/{role}/{userId}")]
        [ProducesResponseType(typeof(ApiResponseDto<bool>), 200)]
        [ProducesResponseType(400)]
        public async Task<ApiResponseDto<bool>> RemoveUserFromRole(string role, string userId)
        {
            await administratorService.RemoveUserFromRoleAsync(role, userId);
            return new ApiResponseDto<bool>(true);
        }

        /// <summary>
        /// Retrieves the claims for a user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>The list of claims for the user.</returns>
        /// <response code="200">Returns the list of claims.</response>
        [HttpGet("users/{userId}/claims")]
        [ProducesResponseType(typeof(ApiResponseDto<IList<ClaimDto>>), 200)]
        public async Task<ApiResponseDto<IList<ClaimDto>>> GetClaimsForUser(string userId)
        {
            return new ApiResponseDto<IList<ClaimDto>>(await administratorService.GetClaimsForUserAsync(userId));
        }

        /// <summary>
        /// Retrieves the users for a specific claim.
        /// </summary>
        /// <param name="claimType">The claim type.</param>
        /// <param name="claimValue">The claim value.</param>
        /// <returns>The list of users in the specified claim.</returns>
        /// <response code="200">Returns the list of users.</response>
        [HttpGet("claims/{claimType}/{claimValue}")]
        [ProducesResponseType(typeof(ApiResponseDto<IList<AdminUserDto>>), 200)]
        public async Task<ApiResponseDto<IList<AdminUserDto>>> GetUsersForClaimAsync(string claimType, string claimValue)
        {
            return new ApiResponseDto<IList<AdminUserDto>>(await administratorService.GetUsersForClaimAsync(new ClaimDto { Type = claimType, Value = claimValue }));
        }

        /// <summary>
        /// Adds a claim to a user.
        /// </summary>
        /// <param name="claimType">The claim type.</param>
        /// <param name="claimValue">The claim value.</param>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>True if the claim was successfully added to the user.</returns>
        /// <response code="200">Claim successfully added to the user.</response>
        /// <response code="400">If there was an error adding the claim to the user.</response>
        [HttpPost("users/{userId}/claims/{claimType}/{claimValue}")]
        [ProducesResponseType(typeof(ApiResponseDto<bool>), 200)]
        [ProducesResponseType(400)]
        public async Task<ApiResponseDto<bool>> AddClaimToUser(string userId, string claimType, string claimValue)
        {
            await administratorService.AddClaimToUserAsync(userId, new ClaimDto() { Type = claimType, Value = claimValue });
            return new ApiResponseDto<bool>(true);
        }

        /// <summary>
        /// Removes a claim from a user.
        /// </summary>
        /// <param name="claimType">The claim type.</param>
        /// <param name="claimValue">The claim value.</param>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>True if the claim was successfully removed from the user.</returns>
        /// <response code="200">Claim successfully removed from the user.</response>
        /// <response code="400">If there was an error removing the claim from the user.</response>
        [HttpDelete("users/{userId}/claims/{claimType}/{claimValue}")]
        [ProducesResponseType(typeof(ApiResponseDto<bool>), 200)]
        [ProducesResponseType(400)]
        public async Task<ApiResponseDto<bool>> RemoveClaimFromUser(string userId, string claimType, string claimValue)
        {
            await administratorService.RemoveClaimFromUserAsync(userId, new ClaimDto() { Type = claimType, Value = claimValue });
            return new ApiResponseDto<bool>(true);
        }

        /// <summary>
        /// Locks a user account.
        /// </summary>
        /// <param name="userId">The ID of the user to lock.</param>
        /// <returns>True if the user account was successfully locked.</returns>
        /// <response code="200">User account successfully locked.</response>
        /// <response code="400">If there was an error locking the user account.</response>
        [HttpPost("users/{userId}/lock")]
        [ProducesResponseType(typeof(ApiResponseDto<bool>), 200)]
        [ProducesResponseType(400)]
        public async Task<ApiResponseDto<bool>> LockUserAccount(string userId)
        {
            await administratorService.LockUserAccountAsync(userId);
            return new ApiResponseDto<bool>(true);
        }

        /// <summary>
        /// Unlocks a user account.
        /// </summary>
        /// <param name="userId">The ID of the user to unlock.</param>
        /// <returns>True if the user account was successfully unlocked.</returns>
        /// <response code="200">User account successfully unlocked.</response>
        /// <response code="400">If there was an error unlocking the user account.</response>
        [HttpPost("users/{userId}/unlock")]
        [ProducesResponseType(typeof(ApiResponseDto<bool>), 200)]
        [ProducesResponseType(400)]
        public async Task<ApiResponseDto<bool>> UnlockUserAccount(string userId)
        {
            await administratorService.UnlockUserAccountAsync(userId);
            return new ApiResponseDto<bool>(true);
        }
    }
}