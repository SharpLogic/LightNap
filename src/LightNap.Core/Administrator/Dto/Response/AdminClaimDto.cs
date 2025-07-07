using LightNap.Core.Identity.Dto.Response;

namespace LightNap.Core.Administrator.Dto.Response
{
    public class AdminClaimDto : ClaimDto
    {
        /// <summary>
        /// The ID of the user.
        /// </summary>
        public required string UserId { get; set; }
    }
}