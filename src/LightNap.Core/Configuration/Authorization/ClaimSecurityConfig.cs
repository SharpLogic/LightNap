using System.Collections.ObjectModel;

namespace LightNap.Core.Configuration.Authorization
{
    /// <summary>
    /// Configuration for claim-based security.
    /// </summary>
    public static class ClaimSecurityConfig
    {
        /// <summary>
        /// Gets the comma-separated list of roles authorized to manage user claims. This is used to secure the claim management endpoints.
        /// If you add new roles that need to be able to manage user claims, include them here so that they update the controller authorization.
        /// </summary>
        /// <remarks>
        /// Note that this is just a list of all roles that manage any claims. Each claim is still independently secured for the roles
        /// specified in its configuration. In other words, a ContentEditor can access the role management HTTP endpoints but will still be
        /// evaluated for permission to make the claim change they're requesting based on security defined in the AllRules list.
        /// </remarks>
        public const string ClaimManagementRoles = $"{Constants.Roles.Administrator},{Constants.Roles.ContentEditor}";

        /// <summary>
        /// The claim security rules that define which roles grant which claims. Any claims not listed here require an admin. If you add
        /// new roles here, be sure to update <see cref="ClaimManagementRoles"/> so users in those roles can pass controller authorization.
        /// </summary>
        internal static IReadOnlyList<ClaimSecurityRule> AllRules =>
        [
            new ClaimSecurityRule(Constants.Claims.ContentEditor, [ApplicationRoles.ContentEditor]),
            new ClaimSecurityRule(Constants.Claims.ContentReader, [ApplicationRoles.ContentEditor]),
        ];

        /// <summary>
        /// Lookup dictionary for claim keys to roles that grant those claims.
        /// </summary>
        internal static ReadOnlyDictionary<string, string[]> RulesLookup { get; } =
            ClaimSecurityConfig.AllRules.ToDictionary(
                rule => rule.ClaimType,
                rule => rule.Roles.Select(role => role.Name!).ToArray()
                ).AsReadOnly();
    }
}
