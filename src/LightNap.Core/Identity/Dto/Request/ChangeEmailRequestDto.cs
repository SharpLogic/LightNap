using LightNap.Core.Configuration;
using System.ComponentModel.DataAnnotations;

namespace LightNap.Core.Identity.Dto.Request
{
    /// <summary>
    /// Data transfer object for requesting an email change.
    /// </summary>
    public class ChangeEmailRequestDto
    {
        /// <summary>
        /// The email to change to.
        /// </summary>
        [Required]
        [StringLength(Constants.Dto.MaxLoginLength)]
        public required string NewEmail { get; set; }
    }
}
