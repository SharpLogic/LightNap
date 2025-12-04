namespace LightNap.Core.Identity.Dto.Response
{
    /// <summary>
    /// Represents external login information for an administrative user, including the user's identifier and associated
    /// external authentication details.
    /// </summary>
    /// <param name="UserId">The unique identifier of the administrative user associated with the external login.</param>
    /// <param name="LoginProvider">The name of the external authentication provider (for example, 'Google' or 'Facebook').</param>
    /// <param name="ProviderKey">The unique key provided by the external authentication provider to identify the user.</param>
    /// <param name="ProviderDisplayName">The display name of the external authentication provider, as shown to users.</param>
    public record AdminExternalLoginDto(string UserId, string LoginProvider, string ProviderKey, string ProviderDisplayName) : ExternalLoginDto(LoginProvider, ProviderKey, ProviderDisplayName);
}