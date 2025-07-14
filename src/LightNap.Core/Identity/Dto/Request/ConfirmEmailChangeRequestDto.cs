using LightNap.Core.Configuration;
using System.ComponentModel.DataAnnotations;

namespace LightNap.Core.Identity.Dto.Request
{
    /// <summary>
    /// Data transfer object for confirming an email change.
    /// </summary>
    public class ConfirmEmailChangeRequestDto
    {
        /// <summary>
        /// The email to change to.
        /// </summary>
        [Required]
        [StringLength(Constants.Dto.MaxLoginLength)]
        public required string NewEmail { get; set; }

        /// <summary>
        /// The change confirmation code.
        /// </summary>
        [Required]
        [StringLength(Constants.Dto.MaxVerificationCodeLength)]
        public required string Code { get; set; }
    }
}
