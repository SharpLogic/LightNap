using LightNap.Core.Identity.Models;

namespace LightNap.Core.Identity.Dto.Response
{
    /// <summary>
    /// Represents the result of a successful external login operation.
    /// </summary>
    public class ExternalLoginSuccessDto
    {
        /// <summary>
        /// Indicates whether the external login resulted in a new user registration or an existing user login.
        /// </summary>
        public required ExternalLoginSuccessType Type { get; set; }

        /// <summary>
        /// Provided when the external login requires registration and included an email.
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// Provided when the external login requires registration and included a user name.
        /// </summary>
        public string? UserName { get; set; }
    }
}