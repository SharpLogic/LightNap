using LightNap.Core.Data.Entities;

namespace LightNap.Core.Interfaces
{
    /// <summary>
    /// Interface for managing refresh tokens.
    /// </summary>
    public interface IRefreshTokenService
    {
        /// <summary>
        /// Creates a new refresh token for the specified user.
        /// </summary>
        /// <param name="user">The user for whom the refresh token is being created.</param>
        /// <param name="deviceDetails">Details about the device requesting the token.</param>
        /// <param name="isPersistent">Indicates whether the token should be persistent across sessions.</param>
        /// <param name="expires">The expiration date and time for the token.</param>
        /// <returns>A task that represents the asynchronous operation, containing the created refresh token.</returns>
        Task<RefreshToken> CreateRefreshTokenAsync(ApplicationUser user, string deviceDetails, bool isPersistent, DateTime expires);

        /// <summary>
        /// Validates and updates a refresh token.
        /// </summary>
        /// <param name="tokenValue">The value of the refresh token to validate.</param>
        /// <returns>A task that represents the asynchronous operation, containing the associated user if valid, otherwise null.</returns>
        Task<RefreshToken?> ValidateAndRefreshTokenAsync(string tokenValue);

        /// <summary>
        /// Revokes a refresh token.
        /// </summary>
        /// <param name="deviceId">The device identifier associated with the token to revoke.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task RevokeRefreshTokenAsync(string deviceId);

        /// <summary>
        /// Purges expired refresh tokens.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task PurgeExpiredRefreshTokens();
    }
}