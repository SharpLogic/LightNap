using System.Security.Claims;

namespace LightNap.Core.Extensions
{
    /// <summary>
    /// Extension methods for ClaimsPrincipal.
    /// </summary>
    public static class ClaimsPrincipalExtensions
    {
        /// <summary>
        /// Gets the user ID from the ClaimsPrincipal.
        /// </summary>
        /// <param name="user">The ClaimsPrincipal instance.</param>
        /// <returns>The user ID.</returns>
        /// <exception cref="Exception">Thrown when the JWT does not include the required ID claim.</exception>
        public static string GetUserId(this ClaimsPrincipal user)
        {
            return user.TryGetUserId() ?? throw new Exception("JWT did not include required ID claim");
        }

        /// <summary>
        /// Tries to get the user ID from the ClaimsPrincipal.
        /// </summary>
        /// <param name="user">The ClaimsPrincipal instance.</param>
        /// <returns>The user ID or null.</returns>
        public static string? TryGetUserId(this ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
    }
}