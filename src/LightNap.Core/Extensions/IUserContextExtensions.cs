using LightNap.Core.Api;
using LightNap.Core.Interfaces;

namespace LightNap.Core.Extensions
{
    /// <summary>
    /// Provides extension methods for <see cref="IUserContext"/> that enforce authentication, role membership,
    /// administrator privileges, and claim requirements by throwing user-friendly exceptions when access conditions are
    /// not met.
    /// </summary>
    /// <remarks>These methods are intended to simplify access control checks in application code by throwing
    /// <see cref="UserFriendlyApiException"/> when a user does not meet the required criteria. Use these extensions to
    /// ensure that users are properly authenticated and authorized before performing sensitive operations. All methods
    /// throw exceptions with descriptive messages suitable for displaying to end users.</remarks>
    public static class IUserContextExtensions
    {
        /// <summary>
        /// Throws a <see cref="UserFriendlyApiException"/> if the user is not authenticated.
        /// </summary>
        /// <param name="userContext">The user context to check for authentication.</param>
        /// <exception cref="UserFriendlyApiException">Thrown if the user is not authenticated.</exception>
        public static void AssertAuthenticated(this IUserContext userContext)
        {
            if (!userContext.IsAuthenticated) { throw new UserFriendlyApiException($"You must be authenticated to perform this action."); }
        }

        /// <summary>
        /// Throws a <see cref="UserFriendlyApiException"/> if the user is not in the specified role.
        /// </summary>
        /// <param name="userContext">The user context to check for the role.</param>
        /// <param name="role">The role to check.</param>
        /// <exception cref="UserFriendlyApiException">Thrown if the user is not in the specified role.</exception>
        public static void AssertRole(this IUserContext userContext, string role)
        {
            if (!userContext.IsInRole(role)) { throw new UserFriendlyApiException($"You must be in the '{role}' role to perform this action."); }
        }

        /// <summary>
        /// Throws a <see cref="UserFriendlyApiException"/> if the user is not an administrator.
        /// </summary>
        /// <param name="userContext">The user context to check for administrator privileges.</param>
        /// <exception cref="UserFriendlyApiException">Thrown if the user is not an administrator.</exception>
        public static void AssertAdministrator(this IUserContext userContext)
        {
            if (!userContext.IsAdministrator) { throw new UserFriendlyApiException($"You must be an administrator to perform this action."); }
        }

        /// <summary>
        /// Throws a <see cref="UserFriendlyApiException"/> if the user does not have the specified claim.
        /// </summary>
        /// <param name="userContext">The user context to check for the claim.</param>
        /// <param name="claimType">The type of the claim to check.</param>
        /// <param name="claimValue">The value of the claim to check.</param>
        /// <exception cref="UserFriendlyApiException">Thrown if the user does not have the specified claim.</exception>
        public static void AssertClaim(this IUserContext userContext, string claimType, string claimValue)
        {
            if (!userContext.HasClaim(claimType, claimValue)) { throw new UserFriendlyApiException($"You do not have permission to perform this action."); }
        }
    }
}