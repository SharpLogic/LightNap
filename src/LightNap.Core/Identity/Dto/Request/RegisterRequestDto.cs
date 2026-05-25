using LightNap.Core.Configuration;
using System.ComponentModel.DataAnnotations;

namespace LightNap.Core.Identity.Dto.Request
{
    /// <summary>
    /// Represents the data transfer object for registering a user.
    /// </summary>
    public class RegisterRequestDto
    {
        /// <summary>
        /// Gets or sets the email address of the user.
        /// </summary>
        [EmailAddress]
        [Required]
        [StringLength(Constants.Dto.MaxLoginLength)]
        public required string Email { get; set; }

        /// <summary>
        /// Gets or sets the password of the user.
        /// </summary>
        [Required]
        [StringLength(Constants.Dto.MaxPasswordLength)]
        public required string Password { get; set; }

        /// <summary>
        /// Gets or sets the confirmation password of the user.
        /// </summary>
        [Required]
        [StringLength(Constants.Dto.MaxPasswordLength)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public required string ConfirmPassword { get; set; }

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

        /// <summary>
        /// Whether the user is opting into marketing email at registration.
        /// </summary>
        public bool MarketingOptIn { get; set; }
    }
}
