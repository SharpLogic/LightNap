using LightNap.Core.Configuration;
using System.ComponentModel.DataAnnotations;

namespace LightNap.Core.Identity.Dto.Request
{
    /// <summary>
    /// Represents the data transfer object for completing external login registration.
    /// </summary>
    public class ExternalLoginRegisterRequestDto
    {
        /// <summary>
        /// Gets or sets the confirmation token.
        /// </summary>
        [Required]
        public required string ConfirmationToken { get; set; }

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

        /// <summary>
        /// Gets or sets a value indicating whether to remember the user.
        /// </summary>
        public bool RememberMe { get; set; }

        /// <summary>
        /// Gets or sets the device details of the user.
        /// </summary>
        [Required]
        [StringLength(Constants.Dto.MaxDeviceDetailsLength)]
        public required string DeviceDetails { get; set; }
    }
}