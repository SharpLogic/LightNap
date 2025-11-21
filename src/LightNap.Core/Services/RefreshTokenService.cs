using LightNap.Core.Api;
using LightNap.Core.Configuration;
using LightNap.Core.Data;
using LightNap.Core.Data.Entities;
using LightNap.Core.Extensions;
using LightNap.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LightNap.Core.Services
{
    /// <summary>
    /// Service for managing refresh tokens in the database.
    /// </summary>
    public class RefreshTokenService(ApplicationDbContext db, IUserContext userContext, ILogger<RefreshTokenService> logger) : IRefreshTokenService
    {
        /// <inheritdoc />
        public async Task<RefreshToken> CreateRefreshTokenAsync(ApplicationUser user, string deviceDetails, bool isPersistent, DateTime expires)
        {
            var token = new RefreshToken
            {
                Id = Guid.NewGuid().ToString(),
                Token = Guid.NewGuid().ToString(),
                Expires = expires,
                LastSeen = DateTime.UtcNow,
                IpAddress = userContext.GetIpAddress() ?? Constants.RefreshTokens.NoIpProvided,
                Details = deviceDetails,
                IsPersistent = isPersistent,
                UserId = user.Id
            };

            db.RefreshTokens.Add(token);
            await db.SaveChangesAsync();
            return token;
        }

        /// <inheritdoc />
        public async Task<RefreshToken?> ValidateAndRefreshTokenAsync(string tokenValue)
        {
            var token = await db.RefreshTokens.Include(t => t.User).FirstOrDefaultAsync(t => t.Token == tokenValue);
            if (token == null || token.IsRevoked || token.Expires < DateTime.UtcNow)
            {
                return null;
            }

            token.LastSeen = DateTime.UtcNow;
            token.IpAddress = userContext.GetIpAddress() ?? Constants.RefreshTokens.NoIpProvided;

            // Update expires if needed
            await db.SaveChangesAsync();
            return token;
        }

        /// <inheritdoc />
        public async Task RevokeRefreshTokenAsync(string deviceId)
        {
            userContext.AssertAuthenticated();

            var token = await db.RefreshTokens.FindAsync(deviceId) ?? throw new UserFriendlyApiException("Device not found.");
            if (token.UserId != userContext.GetUserId()) { throw new UserFriendlyApiException("Device not found."); }

            token.IsRevoked = true;
            await db.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task PurgeExpiredRefreshTokens()
        {
            userContext.AssertAdministrator();

            logger.LogInformation("Starting with {count} refresh tokens", await db.RefreshTokens.CountAsync());

            const int batchSize = 100;
            int deletedCount = 0;

            while (true)
            {
                var expiredTokens = await db.RefreshTokens
                    .Where(token => token.Expires < DateTime.UtcNow)
                    .OrderByDescending(token => token.Id)
                    .Take(batchSize)
                    .ToListAsync();
                if (expiredTokens.Count == 0) { break; }

                db.RefreshTokens.RemoveRange(expiredTokens);
                deletedCount += await db.SaveChangesAsync();
            }

            logger.LogInformation("Deleted {deletedCount} expired refresh tokens", deletedCount);

            // It's possible that some may have been created since we started.
            logger.LogInformation("Finished with {count} refresh tokens", await db.RefreshTokens.CountAsync());
        }
    }
}