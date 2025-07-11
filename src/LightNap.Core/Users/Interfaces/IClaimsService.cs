using LightNap.Core.Api;
using LightNap.Core.Identity.Dto.Response;
using LightNap.Core.Users.Dto.Request;
using LightNap.Core.Users.Dto.Response;

namespace LightNap.Core.Users.Interfaces
{
    /// <summary>  
    /// Interface for managing claims.
    /// </summary>  
    public interface IClaimsService
    {
        /// <summary>
        /// Searches claims.
        /// </summary>
        /// <param name="searchRequest">The search parameters.</param>
        /// <returns>The paginated list of claims.</returns>
        Task<PagedResponse<UserClaimDto>> SearchClaimsAsync(SearchClaimsRequestDto searchRequest);

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
    }
}