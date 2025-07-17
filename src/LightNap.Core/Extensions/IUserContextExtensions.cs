using LightNap.Core.Api;
using LightNap.Core.Interfaces;

namespace LightNap.Core.Extensions
{
    public static class IUserContextExtensions
    {
        public static void AssertAuthenticated(this IUserContext userContext)
        {
            if (!userContext.IsAuthenticated) { throw new UserFriendlyApiException($"You must be authenticated to perform this action."); }
        }

        public static void AssertRole(this IUserContext userContext, string role)
        {
            if (!userContext.IsInRole(role)) { throw new UserFriendlyApiException($"You must be in the '{role}' role to perform this action."); }
        }

        public static void AssertAdministrator(this IUserContext userContext)
        {
            if (!userContext.IsAdministrator) { throw new UserFriendlyApiException($"You must be an administrator to perform this action."); }
        }

        public static void AssertClaim(this IUserContext userContext, string claimType, string claimValue)
        {
            if (!userContext.HasClaim(claimType, claimValue)) { throw new UserFriendlyApiException($"You do not have permission to perform this action."); }
        }
    }
}