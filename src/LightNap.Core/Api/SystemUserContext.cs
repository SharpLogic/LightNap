using LightNap.Core.Interfaces;

namespace LightNap.Core.Api
{
    /// <summary>
    /// Represents a system-level user context that always has full privileges.
    /// Used for operations that require elevated or system-wide access.
    /// </summary>
    public class SystemUserContext : IUserContext
    {
        /// <summary>
        /// Gets a value indicating whether the user is an administrator.
        /// Always returns true for the system user.
        /// </summary>
        public bool IsAdministrator => true;

        /// <summary>
        /// Gets a value indicating whether the user is authenticated.
        /// Always returns true for the system user.
        /// </summary>
        public bool IsAuthenticated => true;

        /// <summary>
        /// Gets the IP address associated with the current request.
        /// Always returns null for the system user.
        /// </summary>
        /// <returns>Always null.</returns>
        public string? GetIpAddress() => null;

        /// <summary>
        /// Gets the user ID associated with the current request.
        /// Always returns "system" for the system user.
        /// </summary>
        /// <returns>The string "system".</returns>
        public string GetUserId() => "system";

        /// <summary>
        /// Determines whether the user has the specified claim.
        /// Always returns true for the system user.
        /// </summary>
        /// <param name="claimType">The type of the claim to check.</param>
        /// <param name="claimValue">The value of the claim to check.</param>
        /// <returns>Always true.</returns>
        public bool HasClaim(string claimType, string claimValue) => true;

        /// <summary>
        /// Determines whether the user is in the specified role.
        /// Always returns true for the system user.
        /// </summary>
        /// <param name="role">The role to check.</param>
        /// <returns>Always true.</returns>
        public bool IsInRole(string role) => true;
    }
}
