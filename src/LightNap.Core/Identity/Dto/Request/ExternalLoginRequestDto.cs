using LightNap.Core.Configuration;
using System.ComponentModel.DataAnnotations;

namespace LightNap.Core.Identity.Dto.Request
{
    /// <summary>
    /// Represents the data transfer object for completing external login completion.
    /// </summary>
    public class ExternalLoginRequestDto
    {
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