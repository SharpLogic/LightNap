using LightNap.Core.Data.Entities;

namespace LightNap.Core.Configuration
{
    internal record ClaimSecurityRule(string ClaimType, IEnumerable<ApplicationRole> Roles);
}
