using LightNap.Core.Configuration;
using LightNap.Core.Extensions;
using LightNap.Core.Interfaces;
using LightNap.WebApi.Extensions;

namespace LightNap.WebApi.Services
{
    /// <summary>
    /// Provides methods to access user-specific information from the HTTP context.
    /// </summary>
    public class WebUserContext(IHttpContextAccessor httpContextAccessor) : IUserContext
    {
        /// <summary>
        /// True if the user is an administrator; otherwise, false.
        /// </summary>
        public bool IsAdministrator => this.IsInRole(Constants.Roles.Administrator);

        /// <summary>
        /// True if the user is logged in; otherwise, false.
        /// </summary>
        public bool IsAuthenticated => httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated ?? throw new InvalidOperationException();

        /// <summary>
        /// Gets the user ID from the HTTP context.
        /// </summary>
        /// <returns>The user ID.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the user ID cannot be retrieved.</exception>
        public string GetUserId()
        {
            return httpContextAccessor.HttpContext?.User.GetUserId() ?? throw new InvalidOperationException();
        }

        /// <summary>
        /// Gets the IP address from the HTTP context.
        /// </summary>
        /// <returns>The IP address, or null if it cannot be retrieved.</returns>
        public string? GetIpAddress()
        {
            return httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();
        }

        /// <summary>
        /// Determines whether the user is in the specified role.
        /// </summary>
        /// <param name="role">The role to check.</param>
        /// <returns>True if the user is in the specified role; otherwise, false.</returns>
        public bool IsInRole(string role)
        {
            return httpContextAccessor.HttpContext?.User.IsInRole(role) ?? false;
        }

        /// <summary>
        /// Determines whether the user has the specified claim.
        /// </summary>
        /// <param name="claimType">The type of the claim to check.</param>
        /// <param name="claimValue">The value of the claim to check.</param>
        /// <returns>True if the user has the specified claim; otherwise, false.</returns>
        public bool HasClaim(string claimType, string claimValue)
        {
            return httpContextAccessor.HttpContext?.User.HasClaim(claimType, claimValue) ?? false;
        }
    }

}
