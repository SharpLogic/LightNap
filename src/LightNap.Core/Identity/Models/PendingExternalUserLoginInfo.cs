using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace LightNap.Core.Identity.Models
{
    public class PendingExternalUserLoginInfo : UserLoginInfo
    {
        // Track claims so that to enable the frontend to offer default values when the user need to register.
        public string? Email { get; set; }
        public string? UserName { get; set; }

        // Additional claims can be accessed similarly.
        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        public PendingExternalUserLoginInfo(ExternalLoginInfo info) : base(info.LoginProvider, info.ProviderKey, info.ProviderDisplayName)
        {
            Email = info.Principal.FindFirstValue(ClaimTypes.Email);
            UserName = info.Principal.FindFirstValue(ClaimTypes.Name);
            FirstName = info.Principal.FindFirstValue(ClaimTypes.GivenName);
            LastName = info.Principal.FindFirstValue(ClaimTypes.Surname);
        }

        public PendingExternalUserLoginInfo() : base("", "", "")
        {
        }
    }
}