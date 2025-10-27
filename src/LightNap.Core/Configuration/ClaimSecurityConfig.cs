using System.Collections.ObjectModel;

namespace LightNap.Core.Configuration
{
    /// <summary>
    /// Configuration for claim-based security.
    /// </summary>
    internal static class ClaimSecurityConfig
    {
        /// <summary>
        /// The claim security rules that define which roles grant which claims. Any claims not listed here require an admin.
        /// </summary>
        public static IReadOnlyList<ClaimSecurityRule> AllRules =>
        [
            new ClaimSecurityRule(Constants.Claims.ContentEditor, [ApplicationRoles.ContentEditor]),
            new ClaimSecurityRule(Constants.Claims.ContentReader, [ApplicationRoles.ContentEditor]),
        ];

        /// <summary>
        /// Lookup dictionary for claim keys to roles that grant those claims.
        /// </summary>
        public static ReadOnlyDictionary<string, string[]> RulesLookup { get; } =
            ClaimSecurityConfig.AllRules.ToDictionary(
                rule => rule.ClaimType,
                rule => rule.Roles.Select(role => role.Name!).ToArray()
                ).AsReadOnly();
    }
}
