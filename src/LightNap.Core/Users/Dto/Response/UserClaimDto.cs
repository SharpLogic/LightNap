using LightNap.Core.Identity.Dto.Response;

namespace LightNap.Core.Users.Dto.Response
{
    /// <summary>
    /// Data transfer object for user claims, extending base claim information with user identification.
    /// </summary>
    public class UserClaimDto : ClaimDto
    {
        /// <summary>
        /// Gets or sets the unique identifier of the user associated with this claim.
        /// </summary>
        public required string UserId { get; set; }
    }
}