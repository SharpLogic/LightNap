using LightNap.Core.Api;
using LightNap.Core.Identity.Dto.Request;
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
        /// Retrieves the claims associated with the currently authenticated user.
        /// </summary>
        /// <remarks>This method requires the user to be authenticated. If the user is not found or
        /// authentication fails, an exception is thrown. The returned claims are converted to <see cref="ClaimDto"/>
        /// objects.</remarks>
        /// <param name="pagedRequestDto">The paging details for the request.</param>
        /// <returns>A list of <see cref="ClaimDto"/> objects representing the claims of the authenticated user.</returns>
        /// <exception cref="UserFriendlyApiException">Thrown if the authenticated user cannot be found.</exception>
        Task<PagedResponseDto<ClaimDto>> GetMyClaimsAsync(PagedRequestDtoBase pagedRequestDto);

        /// <summary>
        /// Retrieves a list of user IDs that have the specified claim type and value.
        /// </summary>
        /// <remarks>This method ensures that the specified claim type is valid before querying the
        /// database. The query retrieves distinct user IDs associated with the given claim type and value.</remarks>
        /// <param name="searchClaimRequestDto">The claim and paging details to get users for.</param>
        /// <returns>A list of user IDs that match the specified claim type and value. The list will be empty if no users match
        /// the criteria.</returns>
        Task<PagedResponseDto<string>> GetUsersWithClaimAsync(SearchClaimRequestDto searchClaimRequestDto);

        /// <summary>
        /// Searches claims.
        /// </summary>
        /// <param name="searchRequest">The search parameters.</param>
        /// <returns>The paginated list of claims.</returns>
        Task<PagedResponseDto<ClaimDto>> SearchClaimsAsync(SearchClaimsRequestDto searchRequest);

        /// <summary>
        /// Searches user claims.
        /// </summary>
        /// <param name="searchRequest">The search parameters.</param>
        /// <returns>The paginated list of claims.</returns>
        Task<PagedResponseDto<UserClaimDto>> SearchUserClaimsAsync(SearchUserClaimsRequestDto searchRequest);

        /// <summary>
        /// Adds a claim to the specified user asynchronously.
        /// </summary>
        /// <remarks>This method associates the provided claim with the specified user. Ensure that the
        /// user exists and that the claim is valid before calling this method. The operation is performed
        /// asynchronously.</remarks>
        /// <param name="userId">The unique identifier of the user to whom the claim will be added. Cannot be null or empty.</param>
        /// <param name="claim">The claim to add to the user. Cannot be null.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task AddUserClaimAsync(string userId, ClaimDto claim);

        /// <summary>
        /// Removes a specific claim from the specified user.
        /// </summary>
        /// <remarks>This method removes the specified claim from the user's claim collection.  If the
        /// user does not have the specified claim, the operation will complete without making changes.</remarks>
        /// <param name="userId">The unique identifier of the user from whom the claim will be removed. Cannot be null or empty.</param>
        /// <param name="claim">The claim to be removed from the user. Cannot be null.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task RemoveUserClaimAsync(string userId, ClaimDto claim);

    }
}