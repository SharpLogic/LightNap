using System.ComponentModel.DataAnnotations;

namespace LightNap.WebApi.Configuration
{
    internal static class WebConstants
    {
        internal static class RateLimiting
        {
            internal const string AuthPolicyName = "Auth";
            internal const string ContentPolicyName = "Content";
            internal const string RegistrationPolicyName = "Registration";

        }
    }
}