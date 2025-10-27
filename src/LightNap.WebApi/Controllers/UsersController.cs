using LightNap.Core.Api;
using LightNap.Core.Configuration;
using LightNap.Core.Identity.Dto.Response;
using LightNap.Core.Users.Dto.Request;
using LightNap.Core.Users.Dto.Response;
using LightNap.Core.Users.Interfaces;
using LightNap.Core.UserSettings.Dto.Request;
using LightNap.Core.UserSettings.Dto.Response;
using LightNap.Core.UserSettings.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LightNap.WebApi.Controllers
{
    /// <summary>
    /// Controller for managing users.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController(IUsersService usersService, IRolesService rolesService, IClaimsService claimsService,
        IUserSettingsService userSettingsService) : ControllerBase
    {
        /// <summary>
        /// Retrieves a user by ID.
        /// </summary>
        /// <param name="userId">The ID of the user to retrieve.</param>
        /// <returns>The user details.</returns>
        /// <response code="200">Returns the user details.</response>
        [HttpGet("{userId}")]
        [ProducesResponseType(typeof(ApiResponseDto<PublicUserDto?>), 200)]
        public async Task<ApiResponseDto<PublicUserDto?>> GetUser(string userId)
        {
            return new ApiResponseDto<PublicUserDto?>(await usersService.GetUserAsync(userId));
        }

        /// <summary>
        /// Retrieves a user by username.
        /// </summary>
        /// <param name="userName">The username of the user to retrieve.</param>
        /// <returns>The user details.</returns>
        /// <response code="200">Returns the user details.</response>
        [HttpGet("user-name/{userName}")]
        [ProducesResponseType(typeof(ApiResponseDto<PublicUserDto?>), 200)]
        public async Task<ApiResponseDto<PublicUserDto?>> GetUserByUserName(string userName)
        {
            return new ApiResponseDto<PublicUserDto?>(await usersService.GetUserByUserNameAsync(userName));
        }

        /// <summary>
        /// Searches for users based on the specified criteria.
        /// </summary>
        /// <param name="adminSearchUsersRequest">The search criteria.</param>
        /// <returns>The list of users matching the criteria.</returns>
        /// <response code="200">Returns the list of users.</response>
        [HttpPost("search")]
        [ProducesResponseType(typeof(ApiResponseDto<PagedResponseDto<PublicUserDto>>), 200)]
        [ProducesResponseType(400)]
        public async Task<ApiResponseDto<PagedResponseDto<PublicUserDto>>> SearchUsers(AdminSearchUsersRequestDto adminSearchUsersRequest)
        {
            return new ApiResponseDto<PagedResponseDto<PublicUserDto>>(await usersService.SearchUsersAsync(adminSearchUsersRequest));
        }

        /// <summary>
        /// Retrieves a list of users by their IDs.
        /// </summary>
        /// <param name="userIds">The collection of user IDs.</param>
        /// <returns>The list of users matching the provided IDs.</returns>
        /// <response code="200">Returns the list of users.</response>
        [HttpPost("get-by-ids")]
        [ProducesResponseType(typeof(ApiResponseDto<IList<PublicUserDto>>), 200)]
        [ProducesResponseType(400)]
        public async Task<ApiResponseDto<IList<PublicUserDto>>> GetUsersByIds([FromBody] IEnumerable<string> userIds)
        {
            var users = await usersService.GetUsersByIdsAsync(userIds);
            return new ApiResponseDto<IList<PublicUserDto>>(users);
        }

        /// <summary>
        /// Updates a user.
        /// </summary>
        /// <param name="userId">The ID of the user to update.</param>
        /// <param name="adminUpdateUserRequest">The updated user information.</param>
        /// <returns>The updated user details.</returns>
        /// <response code="200">Returns the updated user details.</response>
        [HttpPut("{userId}")]
        [ProducesResponseType(typeof(ApiResponseDto<AdminUserDto>), 200)]
        [ProducesResponseType(400)]
        public async Task<ApiResponseDto<AdminUserDto>> UpdateUser(string userId, AdminUpdateUserRequestDto adminUpdateUserRequest)
        {
            return new ApiResponseDto<AdminUserDto>(await usersService.UpdateUserAsync(userId, adminUpdateUserRequest));
        }

        /// <summary>
        /// Deletes a user.
        /// </summary>
        /// <param name="userId">The ID of the user to delete.</param>
        /// <returns>True if the user was successfully deleted.</returns>
        /// <response code="200">User successfully deleted.</response>
        /// <response code="400">If the user is an administrator and cannot be deleted.</response>
        [HttpDelete("{userId}")]
        [ProducesResponseType(typeof(ApiResponseDto<bool>), 200)]
        public async Task<ApiResponseDto<bool>> DeleteUser(string userId)
        {
            await usersService.DeleteUserAsync(userId);
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
            return new ApiResponseDto<IList<RoleDto>>(rolesService.GetRoles());
        }

        /// <summary>
        /// Retrieves the roles for a user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>The list of roles for the user.</returns>
        /// <response code="200">Returns the list of roles.</response>
        [HttpGet("{userId}/roles")]
        [ProducesResponseType(typeof(ApiResponseDto<IList<string>>), 200)]
        public async Task<ApiResponseDto<IList<string>>> GetRolesForUser(string userId)
        {
            return new ApiResponseDto<IList<string>>(await rolesService.GetRolesForUserAsync(userId));
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
            return new ApiResponseDto<IList<AdminUserDto>>(await rolesService.GetUsersInRoleAsync(role));
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
            await rolesService.AddUserToRoleAsync(role, userId);
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
            await rolesService.RemoveUserFromRoleAsync(role, userId);
            return new ApiResponseDto<bool>(true);
        }

        /// <summary>
        /// Searches claims.
        /// </summary>
        /// <param name="searchClaimsRequest">The search parameters.</param>
        /// <returns>The list of matching claims.</returns>
        /// <response code="200">Returns the list of claims.</response>
        [HttpPost("claims/search")]
        [ProducesResponseType(typeof(ApiResponseDto<PagedResponseDto<ClaimDto>>), 200)]
        public async Task<ApiResponseDto<PagedResponseDto<ClaimDto>>> SearchClaimsAsync(SearchClaimsRequestDto searchClaimsRequest)
        {
            return new ApiResponseDto<PagedResponseDto<ClaimDto>>(await claimsService.SearchClaimsAsync(searchClaimsRequest));
        }

        /// <summary>
        /// Searches user claims.
        /// </summary>
        /// <param name="searchUserClaimsRequest">The search parameters.</param>
        /// <returns>The list of matching claims.</returns>
        /// <response code="200">Returns the list of claims.</response>
        [HttpPost("user-claims/search")]
        [ProducesResponseType(typeof(ApiResponseDto<PagedResponseDto<UserClaimDto>>), 200)]
        public async Task<ApiResponseDto<PagedResponseDto<UserClaimDto>>> SearchUserClaimsAsync(SearchUserClaimsRequestDto searchUserClaimsRequest)
        {
            return new ApiResponseDto<PagedResponseDto<UserClaimDto>>(await claimsService.SearchUserClaimsAsync(searchUserClaimsRequest));
        }

        /// <summary>
        /// Returns users with a specific claim.
        /// </summary>
        /// <param name="searchClaimRequestDto">The claim to search.</param>
        /// <returns>The list of matching claims.</returns>
        /// <response code="200">Returns the list of claims.</response>
        [HttpPost("claim-users")]
        [ProducesResponseType(typeof(ApiResponseDto<PagedResponseDto<string>>), 200)]
        public async Task<ApiResponseDto<PagedResponseDto<string>>> GetUsersWithClaim(SearchClaimRequestDto searchClaimRequestDto)
        {
            return new ApiResponseDto<PagedResponseDto<string>>(await claimsService.GetUsersWithClaimAsync(searchClaimRequestDto));
        }

        /// <summary>
        /// Adds a claim to a user.
        /// </summary>
        /// <param name="claim">The claim.</param>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>True if the claim was successfully added to the user.</returns>
        /// <response code="200">Claim successfully added to the user.</response>
        /// <response code="400">If there was an error adding the claim to the user.</response>
        [HttpPost("{userId}/claims")]
        [ProducesResponseType(typeof(ApiResponseDto<bool>), 200)]
        [ProducesResponseType(400)]
        public async Task<ApiResponseDto<bool>> AddUserClaim(string userId, ClaimDto claim)
        {
            await claimsService.AddUserClaimAsync(userId, claim);
            return new ApiResponseDto<bool>(true);
        }

        /// <summary>
        /// Removes a claim from a user.
        /// </summary>
        /// <param name="claim">The claim.</param>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>True if the claim was successfully removed from the user.</returns>
        /// <response code="200">Claim successfully removed from the user.</response>
        /// <response code="400">If there was an error removing the claim from the user.</response>
        [HttpDelete("{userId}/claims")]
        [ProducesResponseType(typeof(ApiResponseDto<bool>), 200)]
        [ProducesResponseType(400)]
        public async Task<ApiResponseDto<bool>> RemoveUserClaim(string userId, ClaimDto claim)
        {
            await claimsService.RemoveUserClaimAsync(userId, claim);
            return new ApiResponseDto<bool>(true);
        }

        /// <summary>
        /// Locks a user account.
        /// </summary>
        /// <param name="userId">The ID of the user to lock.</param>
        /// <returns>True if the user account was successfully locked.</returns>
        /// <response code="200">User account successfully locked.</response>
        /// <response code="400">If there was an error locking the user account.</response>
        [HttpPost("{userId}/lock")]
        [ProducesResponseType(typeof(ApiResponseDto<bool>), 200)]
        [ProducesResponseType(400)]
        public async Task<ApiResponseDto<bool>> LockUserAccount(string userId)
        {
            await usersService.LockUserAccountAsync(userId);
            return new ApiResponseDto<bool>(true);
        }

        /// <summary>
        /// Unlocks a user account.
        /// </summary>
        /// <param name="userId">The ID of the user to unlock.</param>
        /// <returns>True if the user account was successfully unlocked.</returns>
        /// <response code="200">User account successfully unlocked.</response>
        /// <response code="400">If there was an error unlocking the user account.</response>
        [HttpPost("{userId}/unlock")]
        [ProducesResponseType(typeof(ApiResponseDto<bool>), 200)]
        [ProducesResponseType(400)]
        public async Task<ApiResponseDto<bool>> UnlockUserAccount(string userId)
        {
            await usersService.UnlockUserAccountAsync(userId);
            return new ApiResponseDto<bool>(true);
        }

        /// <summary>
        /// Gets the settings for a user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>The list of user settings.</returns>
        [Authorize(Roles = Constants.Roles.Administrator)]
        [HttpGet("{userId}/settings")]
        public async Task<ApiResponseDto<List<UserSettingDto>>> GetUserSettings(string userId)
        {
            return new ApiResponseDto<List<UserSettingDto>>(await userSettingsService.GetUserSettingsAsync(userId));
        }

        /// <summary>
        /// Updates the settings for a specific user.
        /// </summary>
        /// <remarks>This method requires the caller to have the Administrator role. It is intended to be
        /// used for managing user-specific settings.</remarks>
        /// <param name="userId">The unique identifier of the user whose settings are being updated.</param>
        /// <param name="setSettingDto">An object containing the new settings to apply to the user.</param>
        /// <returns>An <see cref="ApiResponseDto{T}"/> containing the updated user settings as a <see cref="UserSettingDto"/>.</returns>
        [Authorize(Roles = Constants.Roles.Administrator)]
        [HttpPut("{userId}/settings")]
        public async Task<ApiResponseDto<UserSettingDto>> SetUserSetting(string userId, [FromBody] SetUserSettingRequestDto setSettingDto)
        {
            return new ApiResponseDto<UserSettingDto>(await userSettingsService.SetUserSettingAsync(userId, setSettingDto));
        }

    }
}