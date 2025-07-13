using LightNap.Core.Api;
using LightNap.Core.Data;
using LightNap.Core.Data.Entities;
using LightNap.Core.Data.Extensions;
using LightNap.Core.Extensions;
using LightNap.Core.Identity.Dto.Response;
using LightNap.Core.Identity.Extensions;
using LightNap.Core.Interfaces;
using LightNap.Core.Users.Dto.Request;
using LightNap.Core.Users.Dto.Response;
using LightNap.Core.Users.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LightNap.Core.Users.Services
{
    /// <summary>
    /// Service for managing administrator-related operations.
    /// </summary>
    public class ClaimsService(UserManager<ApplicationUser> userManager, ApplicationDbContext db, IUserContext userContext) : IClaimsService
    {
        /// <summary>
        /// Searches claims.
        /// </summary>
        /// <param name="searchClaimsRequest">The search parameters.</param>
        /// <returns>The paginated list of claims.</returns>
        public async Task<PagedResponse<ClaimDto>> SearchClaimsAsync(SearchClaimsRequestDto searchClaimsRequest)
        {
            userContext.AssertAdministrator();

            var query = db.UserClaims.AsQueryable()
                .Select(claim => new ClaimDto
                {
                    Type = claim.ClaimType!,
                    Value = claim.ClaimValue!
                })
                .Distinct();

            if (!string.IsNullOrWhiteSpace(searchClaimsRequest.Type))
            {
                query = query.Where(claim => claim.Type == searchClaimsRequest.Type);
            }

            if (!string.IsNullOrWhiteSpace(searchClaimsRequest.TypeContains))
            {
                query = query.Where(claim => EF.Functions.Like(claim.Type.ToUpper(), $"%{searchClaimsRequest.TypeContains.ToUpper()}%"));
            }

            if (!string.IsNullOrWhiteSpace(searchClaimsRequest.Value))
            {
                query = query.Where(claim => claim.Value == searchClaimsRequest.Value);
            }

            if (!string.IsNullOrWhiteSpace(searchClaimsRequest.ValueContains))
            {
                query = query.Where(claim => EF.Functions.Like(claim.Value.ToUpper(), $"%{searchClaimsRequest.ValueContains.ToUpper()}%"));
            }

            int totalCount = await query.CountAsync();

            if (searchClaimsRequest.PageNumber > 1)
            {
                query = query.Skip((searchClaimsRequest.PageNumber - 1) * searchClaimsRequest.PageSize);
            }

            var claims = await query
                .OrderBy(claim => claim.Type)
                .ThenBy(claim => claim.Value)
                .Take(searchClaimsRequest.PageSize)
                .ToListAsync();

            return new PagedResponse<ClaimDto>(claims, searchClaimsRequest.PageNumber, searchClaimsRequest.PageSize, totalCount);
        }

        /// <summary>
        /// Searches claims.
        /// </summary>
        /// <param name="searchClaimsRequest">The search parameters.</param>
        /// <returns>The paginated list of claims.</returns>
        public async Task<PagedResponse<UserClaimDto>> SearchUserClaimsAsync(SearchUserClaimsRequestDto searchClaimsRequest)
        {
            userContext.AssertAdministrator();

            var query = db.UserClaims.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchClaimsRequest.UserId))
            {
                query = query.Where(claim => claim.UserId == searchClaimsRequest.UserId);
            }

            if (!string.IsNullOrWhiteSpace(searchClaimsRequest.Type))
            {
                query = query.Where(claim => claim.ClaimType == searchClaimsRequest.Type);
            }

            if (!string.IsNullOrWhiteSpace(searchClaimsRequest.Value))
            {
                query = query.Where(claim => claim.ClaimValue == searchClaimsRequest.Value);
            }

            int totalCount = await query.CountAsync();

            if (searchClaimsRequest.PageNumber > 1)
            {
                query = query.Skip((searchClaimsRequest.PageNumber - 1) * searchClaimsRequest.PageSize);
            }

            var claims = await query
                .OrderBy(claim => claim.UserId)
                .ThenBy(claim => claim.ClaimType)
                .ThenBy(claim => claim.ClaimValue)
                .Take(searchClaimsRequest.PageSize)
                .ToListAsync();

            return new PagedResponse<UserClaimDto>(claims.ToUserClaimDtoList(), searchClaimsRequest.PageNumber, searchClaimsRequest.PageSize, totalCount);
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
        public async Task AddUserClaimAsync(string userId, ClaimDto claim)
        {
            userContext.AssertAdministrator();

            if (await db.UserClaims.AnyAsync(c => c.UserId == userId && c.ClaimType == claim.Type && c.ClaimValue == claim.Value))
            {
                throw new UserFriendlyApiException("This user already has this claim.");
            }

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
        public async Task RemoveUserClaimAsync(string userId, ClaimDto claim)
        {
            userContext.AssertAdministrator();
            var user = await db.Users.FindAsync(userId) ?? throw new UserFriendlyApiException("The specified user was not found.");
            var result = await userManager.RemoveClaimAsync(user, claim.ToClaim());
            if (!result.Succeeded) { throw new UserFriendlyApiException(result.Errors.Select(error => error.Description)); }
        }
    }
}
