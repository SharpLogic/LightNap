namespace LightNap.Core.Interfaces
{
    /// <summary>  
    /// Provides methods to access content specific to a user request.
    /// </summary>  
    public interface IUserContext
    {
        /// <summary>
        /// True if the user is an administrator; otherwise, false.
        /// </summary>
        bool IsAdministrator { get; }

        /// <summary>
        /// True if the user is logged in; otherwise, false.
        /// </summary>
        bool IsAuthenticated { get; }

        /// <summary>
        /// Gets the user ID associated with the current request.
        /// </summary>
        /// <returns>The user ID.</returns>
        string GetUserId();

        /// <summary>
        /// Gets the IP address associated with the current request, if available.
        /// </summary>
        /// <returns>The IP address.</returns>
        string? GetIpAddress();

        /// <summary>
        /// Determines whether the user is in the specified role.
        /// </summary>
        /// <param name="role">The role to check.</param>
        /// <returns>True if the user is in the specified role; otherwise, false.</returns>
        bool IsInRole(string role);

        /// <summary>
        /// Determines whether the user has the specified claim.
        /// </summary>
        /// <param name="claimType">The type of the claim to check.</param>
        /// <param name="claimValue">The value of the claim to check.</param>
        /// <returns>True if the user has the specified claim; otherwise, false.</returns>
        bool HasClaim(string claimType, string claimValue);
    }
}