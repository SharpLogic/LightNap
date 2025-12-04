using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Text.RegularExpressions;

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
            ArgumentNullException.ThrowIfNull(info);

            Email = info.Principal.FindFirstValue(ClaimTypes.Email);
            UserName = Regex.Replace(info.Principal.FindFirstValue(ClaimTypes.Name) ?? "", @"[^\w.-]", "");
            FirstName = info.Principal.FindFirstValue(ClaimTypes.GivenName);
            LastName = info.Principal.FindFirstValue(ClaimTypes.Surname);
        }

        public PendingExternalUserLoginInfo() : base("", "", "")
        {
        }
    }
}