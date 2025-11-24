using LightNap.Core.Identity.Dto.Response;

namespace LightNap.Core.Identity.Models
{
    /// <summary>
    /// Represents the result of an external login callback.
    /// </summary>
    public class ExternalLoginCallbackResult
    {
        /// <summary>
        /// Gets or sets a value indicating whether the external login was successful.
        /// </summary>
        public bool Succeeded { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether additional information is required from the user.
        /// </summary>
        public bool RequiresAdditionalInfo { get; set; }

        /// <summary>
        /// Gets or sets the login result if the external login was successful.
        /// </summary>
        public LoginSuccessDto? LoginResult { get; set; }

        /// <summary>
        /// Gets or sets the error message if the external login failed.
        /// </summary>
        public string? Error { get; set; }
    }
}