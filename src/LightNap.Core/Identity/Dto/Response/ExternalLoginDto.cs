namespace LightNap.Core.Identity.Dto.Response
{
    /// <summary>
    /// Represents external login information for a user, including the authentication provider and associated identifiers.
    /// </summary>
    /// <param name="LoginProvider">The name of the external authentication provider (for example, "Google" or "Facebook").</param>
    /// <param name="ProviderKey">The unique identifier assigned to the user by the external authentication provider.</param>
    /// <param name="ProviderDisplayName">The display name of the external authentication provider, as shown to users.</param>
    public record ExternalLoginDto(string LoginProvider, string ProviderKey, string ProviderDisplayName);
}