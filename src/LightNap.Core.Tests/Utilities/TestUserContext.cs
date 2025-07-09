using LightNap.Core.Configuration;
using LightNap.Core.Interfaces;

namespace LightNap.Core.Tests.Utilities
{
    /// <summary>
    /// Test implementation of IUserContext for unit testing purposes.
    /// </summary>
    internal class TestUserContext : IUserContext
    {
        /// <summary>
        /// True if the user is an administrator; otherwise, false.
        /// </summary>
        public bool IsAdministrator => this.IsInRole(Constants.Roles.Administrator);

        /// <summary>
        /// True if the user is logged in; otherwise, false.
        /// </summary>
        public bool IsAuthenticated => this.UserId is not null;

        /// <summary>
        /// Gets or sets the IP address of the user.
        /// </summary>
        public string? IpAddress { get; set; }

        /// <summary>
        /// Gets or sets the user ID.
        /// </summary>
        public string? UserId { get; set; }

        /// <summary>
        /// The roles.
        /// </summary>
        public List<string> Roles { get; set; } = [];

        /// <summary>
        /// The claims.
        /// </summary>
        public List<(string ClaimType, List<string> ClaimValues)> Claims { get; set; } = [];

        /// <summary>
        /// Retrieves the IP address of the user.
        /// </summary>
        /// <returns>The IP address of the user, or null if not set.</returns>
        public string? GetIpAddress()
        {
            return this.IpAddress;
        }

        /// <summary>
        /// Retrieves the user ID.
        /// </summary>
        /// <returns>The user ID.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the user ID is not set.</exception>
        public string GetUserId()
        {
            if (this.UserId is null) { throw new InvalidOperationException("GetUserId was called without having UserId set first"); }
            return this.UserId;
        }

        /// <summary>
        /// Determines whether the user is in the specified role.
        /// </summary>
        /// <param name="role">The role to check.</param>
        /// <returns>True if the user is in the specified role; otherwise, false.</returns>
        public bool IsInRole(string role)
        {
            return this.Roles.Contains(role);
        }

        /// <summary>
        /// Determines whether the current instance contains a claim with the specified type and value.
        /// </summary>
        /// <param name="claimType">The type of the claim to search for. This value cannot be <see langword="null"/> or empty.</param>
        /// <param name="claimValue">The value of the claim to search for. This value cannot be <see langword="null"/> or empty.</param>
        /// <returns><see langword="true"/> if a claim with the specified type and value exists; otherwise, <see
        /// langword="false"/>.</returns>
        public bool HasClaim(string claimType, string claimValue)
        {
            return this.Claims.Any(c => c.ClaimType == claimType && c.ClaimValues.Contains(claimValue));
        }
    }
}