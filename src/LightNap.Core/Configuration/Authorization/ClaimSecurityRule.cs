using LightNap.Core.Data.Entities;

namespace LightNap.Core.Configuration.Authorization
{
    internal record ClaimSecurityRule(string ClaimType, IEnumerable<ApplicationRole> Roles);
}
