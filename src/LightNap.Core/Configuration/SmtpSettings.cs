using System.ComponentModel.DataAnnotations;

namespace LightNap.Core.Configuration
{
    /// <summary>
    /// Represents the SMTP settings for email.
    /// </summary>
    public class SmtpSettings
    {
        /// <summary>
        /// Gets or sets the SMTP host URL.
        /// </summary>
        [Required(ErrorMessage = "SMTP host is required")]
        [MinLength(1)]
        public required string Host { get; set; }

        /// <summary>
        /// Gets or sets the SMTP port number.
        /// </summary>
        [Range(1, 65535)]
        public int Port { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether SSL is enabled for SMTP.
        /// </summary>
        public bool EnableSsl { get; set; }

        /// <summary>
        /// Gets or sets the SMTP user name.
        /// </summary>
        [Required]
        public required string User { get; set; }

        /// <summary>
        /// Gets or sets the SMTP password.
        /// </summary>
        [Required]
        public required string Password { get; set; }
    }
}