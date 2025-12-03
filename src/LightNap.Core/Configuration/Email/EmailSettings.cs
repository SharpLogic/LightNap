using System.ComponentModel.DataAnnotations;

namespace LightNap.Core.Configuration.Email
{
    /// <summary>
    /// Represents the email settings for the web API.
    /// </summary>
    public record EmailSettings
    {
        /// <summary>
        /// The email provider to use for sending emails.
        /// </summary>
        public EmailProvider Provider { get; set; }

        /// <summary>
        /// The email address from which emails are sent.
        /// </summary>
        [Required]
        [EmailAddress]
        public required string FromEmail { get; set; }

        /// <summary>
        /// The display name for the sender of the emails.
        /// </summary>
        [Required]
        public required string FromDisplayName { get; set; }

        /// <summary>
        /// The root URL for emails sent by the site.
        /// </summary>
        [Required]
        [Url]
        public required string SiteUrlRootForLinks { get; set; }

        /// <summary>
        /// The SMTP settings for sending emails, if applicable.
        /// </summary>
        public SmtpSettings? Smtp { get; set; }
    }
}