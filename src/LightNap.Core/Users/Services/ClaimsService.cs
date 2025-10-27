using LightNap.Core.Api;
using LightNap.Core.Configuration;
using LightNap.Core.Data;
using LightNap.Core.Data.Entities;
using LightNap.Core.Extensions;
using LightNap.Core.Identity.Dto.Response;
using LightNap.Core.Interfaces;
using LightNap.Core.Users.Dto.Request;
using LightNap.Core.Users.Dto.Response;
using LightNap.Core.Users.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;

namespace LightNap.Core.Users.Services
{
    /// <summary>
    /// Service for managing claims.
    /// </summary>
    public class ClaimsService(UserManager<ApplicationUser> userManager, ApplicationDbContext db, IUserContext userContext, ILogger<ClaimsService> logger) : IClaimsService
    {
        /// <summary>
        /// Asserts that the current user has permission to manage the specified claim type.
        /// </summary>
        /// <param name="claimType">The claim type.</param>
        private void AssertClaimSecurity(string claimType)
        {
            ArgumentException.ThrowIfNullOrEmpty(claimType);
            userContext.AssertAuthenticated();

            if (userContext.IsAdministrator) { return; }

            if (ClaimSecurityConfig.RulesLookup.TryGetValue(claimType, out var allowedRoles) &&
                allowedRoles.Any(role => userContext.IsInRole(role)))
            {
                return;
            }

            logger.LogWarning("User '{UserId}' attempted to manage claim type '{ClaimType}' without sufficient permissions.",
                userContext.GetUserId(), claimType);
            throw new UserFriendlyApiException("You do not have permission to manage this claim type.");
        }

        /// <inheritdoc />
        public async Task<PagedResponseDto<ClaimDto>> SearchClaimsAsync(SearchClaimsRequestDto searchClaimsRequest)
        {
            Validator.ValidateObject(searchClaimsRequest, new ValidationContext(searchClaimsRequest), true);
            userContext.AssertAdministrator();

            var baseQuery = db.UserClaims.AsQueryable();

            var query = baseQuery
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

            return new PagedResponseDto<ClaimDto>(claims, searchClaimsRequest.PageNumber, searchClaimsRequest.PageSize, totalCount);
        }

        /// <inheritdoc />
        public async Task<PagedResponseDto<ClaimDto>> GetMyClaimsAsync(PagedRequestDtoBase pagedRequestDto)
        {
            Validator.ValidateObject(pagedRequestDto, new ValidationContext(pagedRequestDto), true);
            userContext.AssertAuthenticated();
            var query = db.UserClaims
                .Where(uc => uc.UserId == userContext.GetUserId())
                .Select(uc => new ClaimDto
                {
                    Type = uc.ClaimType!,
                    Value = uc.ClaimValue!
                });
            int totalCount = await query.CountAsync();
            if (pagedRequestDto.PageNumber > 1)
            {
                query = query.Skip((pagedRequestDto.PageNumber - 1) * pagedRequestDto.PageSize);
            }
            var claims = await query
                .OrderBy(claim => claim.Type)
                .ThenBy(claim => claim.Value)
                .Take(pagedRequestDto.PageSize)
                .ToListAsync();
            return new PagedResponseDto<ClaimDto>(claims, pagedRequestDto.PageNumber, pagedRequestDto.PageSize, totalCount);
        }

        /// <inheritdoc />
        public async Task<PagedResponseDto<string>> GetUsersWithClaimAsync(SearchClaimRequestDto searchClaimRequestDto)
        {
            Validator.ValidateObject(searchClaimRequestDto, new ValidationContext(searchClaimRequestDto), true);
            this.AssertClaimSecurity(searchClaimRequestDto.Type);

            // Return the list of user IDs sorted by username.

            var query = db.UserClaims
                .Where(uc => uc.ClaimType == searchClaimRequestDto.Type && uc.ClaimValue == searchClaimRequestDto.Value)
                .Join(db.Users,
                    uc => uc.UserId,
                    u => u.Id,
                    (uc, u) => new { uc.UserId, u.NormalizedUserName })
                .Distinct()
                .OrderBy(x => x.NormalizedUserName)
                .Select(x => x.UserId);

            int totalCount = await query.CountAsync();

            if (searchClaimRequestDto.PageNumber > 1)
            {
                query = query.Skip((searchClaimRequestDto.PageNumber - 1) * searchClaimRequestDto.PageSize);
            }

            var userIds = await query
                .Take(searchClaimRequestDto.PageSize)
                .ToListAsync();

            return new PagedResponseDto<string>(userIds, searchClaimRequestDto.PageNumber, searchClaimRequestDto.PageSize, totalCount);
        }

        /// <inheritdoc />
        public async Task<PagedResponseDto<UserClaimDto>> SearchUserClaimsAsync(SearchUserClaimsRequestDto searchClaimsRequest)
        {
            Validator.ValidateObject(searchClaimsRequest, new ValidationContext(searchClaimsRequest), true);
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

            return new PagedResponseDto<UserClaimDto>(claims.ToUserClaimDtoList(), searchClaimsRequest.PageNumber, searchClaimsRequest.PageSize, totalCount);
        }

        /// <inheritdoc />
        public async Task AddUserClaimAsync(string userId, ClaimDto claim)
        {
            ArgumentException.ThrowIfNullOrEmpty(userId);
            Validator.ValidateObject(claim, new ValidationContext(claim), true);
            this.AssertClaimSecurity(claim.Type);

            if (await db.UserClaims.AnyAsync(c => c.UserId == userId && c.ClaimType == claim.Type && c.ClaimValue == claim.Value))
            {
                throw new UserFriendlyApiException("This user already has this claim.");
            }

            var user = await db.Users.FindAsync(userId) ?? throw new UserFriendlyApiException("The specified user was not found.");
            var result = await userManager.AddClaimAsync(user, claim.ToClaim());
            if (!result.Succeeded) { throw new UserFriendlyApiException(result.Errors.Select(error => error.Description)); }
        }

        /// <inheritdoc />
        public async Task RemoveUserClaimAsync(string userId, ClaimDto claim)
        {
            ArgumentException.ThrowIfNullOrEmpty(userId);
            Validator.ValidateObject(claim, new ValidationContext(claim), true);
            this.AssertClaimSecurity(claim.Type);

            var user = await db.Users.FindAsync(userId) ?? throw new UserFriendlyApiException("The specified user was not found.");
            var result = await userManager.RemoveClaimAsync(user, claim.ToClaim());
            if (!result.Succeeded) { throw new UserFriendlyApiException(result.Errors.Select(error => error.Description)); }
        }
    }
}
