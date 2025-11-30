using LightNap.Core.Configuration;
using System.ComponentModel.DataAnnotations;

namespace LightNap.Core.Identity.Dto.Request
{
    /// <summary>
    /// Represents the data transfer object for completing external login registration.
    /// </summary>
    public class ExternalLoginRegisterRequestDto : ExternalLoginRequestDto
    {
        /// <summary>
        /// Gets or sets the email address of the user.
        /// </summary>
        [Required]
        [EmailAddress]
        [StringLength(Constants.Dto.MaxUserNameLength)]
        public required string Email { get; set; }

        /// <summary>
        /// Gets or sets the username of the user.
        /// </summary>
        [Required]
        [StringLength(Constants.Dto.MaxUserNameLength)]
        public required string UserName { get; set; }
    }
}