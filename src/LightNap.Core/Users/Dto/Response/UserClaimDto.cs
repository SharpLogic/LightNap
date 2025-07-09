using LightNap.Core.Identity.Dto.Response;

namespace LightNap.Core.Users.Dto.Response
{
    public class UserClaimDto : ClaimDto
    {
        /// <summary>
        /// The ID of the user.
        /// </summary>
        public required string UserId { get; set; }
    }
}