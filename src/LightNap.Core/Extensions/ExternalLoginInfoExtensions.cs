using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;

namespace LightNap.Core.Extensions;

/// <summary>
/// Provides extension methods for easily accesing common ExternalLoginInfo data.
/// </summary>
public static class ExternalLoginInfoExtensions
{
    extension(ExternalLoginInfo externalLoginInfo)
    {
        /// <summary>
        /// Tries to get the value of an authentication token from the external login information.
        /// </summary>
        /// <param name="tokenName">The name of the authentication token to retrieve. Cannot be null or empty.</param>
        /// <returns>The value of the specified authentication token.</returns>
        public string? TryGetRequiredTokenValue(string tokenName) => externalLoginInfo.AuthenticationProperties?.GetTokenValue(tokenName);

        /// <summary>
        /// Retrieves the value of a required authentication token from the external login information.
        /// </summary>
        /// <param name="tokenName">The name of the authentication token to retrieve. Cannot be null or empty.</param>
        /// <returns>The value of the specified authentication token.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the required token is missing from the external login information.</exception>
        public string GetRequiredTokenValue(string tokenName) =>
            externalLoginInfo.TryGetRequiredTokenValue(tokenName)
            ?? throw new InvalidOperationException($"The required token '{tokenName}' is missing from the external login information.");

        /// <summary>
        /// The provided access token.
        /// </summary>
        public string AccessToken => externalLoginInfo.GetRequiredTokenValue("access_token");

        /// <summary>
        /// The provided refresh token.
        /// </summary>
        public string RefreshToken => externalLoginInfo.GetRequiredTokenValue("refresh_token");

        /// <summary>
        /// The expiration time of the access token based on the "expires_in" value.
        /// </summary>
        public DateTime Expiration => DateTime.UtcNow.AddSeconds(int.Parse(externalLoginInfo.GetRequiredTokenValue("expires_in")));
    }
}