using LightNap.Core.Administrator.Dto.Request;
using LightNap.Core.Administrator.Dto.Response;
using LightNap.Core.Api;
using LightNap.Core.Identity.Dto.Response;

namespace LightNap.Core.Administrator.Interfaces
{
    /// <summary>  
    /// Interface for administrator services.  
    /// </summary>  
    public interface IAdministratorService
    {
        /// <summary>  
        /// Gets a user asynchronously by user ID.  
        /// </summary>  
        /// <param name="userId">The user ID.</param>  
        /// <returns>A task that represents the asynchronous operation. The task result contains the user data.</returns>  
        Task<AdminUserDto?> GetUserAsync(string userId);

        /// <summary>  
        /// Searches users asynchronously based on the specified request DTO.  
        /// </summary>  
        /// <param name="requestDto">The request DTO containing search parameters.</param>  
        /// <returns>A task that represents the asynchronous operation. The task result contains the paged user data.</returns>  
        Task<PagedResponse<AdminUserDto>> SearchUsersAsync(SearchAdminUsersRequestDto requestDto);

        /// <summary>  
        /// Updates a user asynchronously by user ID.  
        /// </summary>  
        /// <param name="userId">The user ID.</param>  
        /// <param name="requestDto">The request DTO containing update information.</param>  
        /// <returns>A task that represents the asynchronous operation. The task result contains the updated user data.</returns>  
        Task<AdminUserDto> UpdateUserAsync(string userId, UpdateAdminUserDto requestDto);

        /// <summary>  
        /// Deletes a user asynchronously by user ID.  
        /// </summary>  
        /// <param name="userId">The user ID.</param>  
        /// <returns>A task that represents the asynchronous operation.</returns>  
        Task DeleteUserAsync(string userId);

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

        /// <summary>
        /// Searches claims.
        /// </summary>
        /// <param name="requestDto">The search parameters.</param>
        /// <returns>The paginated list of claims.</returns>
        Task<PagedResponse<AdminClaimDto>> SearchClaimsAsync(SearchClaimsRequestDto requestDto);

        /// <summary>
        /// Adds a claim to the specified user asynchronously.
        /// </summary>
        /// <remarks>This method associates the provided claim with the specified user. Ensure that the
        /// user exists and that the claim is valid before calling this method. The operation is performed
        /// asynchronously.</remarks>
        /// <param name="userId">The unique identifier of the user to whom the claim will be added. Cannot be null or empty.</param>
        /// <param name="claim">The claim to add to the user. Cannot be null.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task AddClaimToUserAsync(string userId, ClaimDto claim);

        /// <summary>
        /// Removes a specific claim from the specified user.
        /// </summary>
        /// <remarks>This method removes the specified claim from the user's claim collection.  If the
        /// user does not have the specified claim, the operation will complete without making changes.</remarks>
        /// <param name="userId">The unique identifier of the user from whom the claim will be removed. Cannot be null or empty.</param>
        /// <param name="claim">The claim to be removed from the user. Cannot be null.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task RemoveClaimFromUserAsync(string userId, ClaimDto claim);

        /// <summary>  
        /// Locks a user account asynchronously by user ID.  
        /// </summary>  
        /// <param name="userId">The user ID.</param>  
        /// <returns>A task that represents the asynchronous operation.</returns>  
        Task LockUserAccountAsync(string userId);

        /// <summary>  
        /// Unlocks a user account asynchronously by user ID.  
        /// </summary>  
        /// <param name="userId">The user ID.</param>  
        /// <returns>A task that represents the asynchronous operation.</returns>  
        Task UnlockUserAccountAsync(string userId);
    }
}