using LightNap.Core.Identity.Dto.Response;

namespace LightNap.Core.Identity.Models
{
    /// <summary>
    /// Represents the result of an external login callback.
    /// </summary>
    public class ExternalLoginCallbackResult
    {
        /// <summary>
        /// True if the user is not yet registered.
        /// </summary>
        public bool RequiresRegistration { get; set; }

        /// <summary>
        /// The confirmation token for completing the login.
        /// </summary>
        public string? ConfirmationToken { get; set; }
    }
}