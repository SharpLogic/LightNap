using LightNap.Core.Identity.Models;

namespace LightNap.Core.Identity.Dto.Response
{
    /// <summary>
    /// Represents the result of a successful login operation.
    /// </summary>
    public class LoginSuccessDto
    {
        /// <summary>
        /// Indicates if a token was issued or if further steps are required.
        /// </summary>
        public required LoginSuccessType Type { get; set; }

        /// <summary>
        /// Gets or sets the access token for the authenticated user.
        /// </summary>
        public string? AccessToken { get; set; }
    }
}