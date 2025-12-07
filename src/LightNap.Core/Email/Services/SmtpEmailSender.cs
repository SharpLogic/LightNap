using LightNap.Core.Configuration.Email;
using LightNap.Core.Email.Interfaces;
using LightNap.Core.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace LightNap.Core.Email.Services
{
    /// <summary>
    /// Service for sending emails using SMTP.
    /// </summary>
    public class SmtpEmailSender : IEmailSender
    {
        private readonly SmtpClient _smtpClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="SmtpEmailSender"/> class.
        /// </summary>
        /// <param name="smtpSettings">The SMTP settings.</param>
        public SmtpEmailSender(SmtpSettings smtpSettings)
        {
            this._smtpClient = new SmtpClient(smtpSettings.Host, smtpSettings.Port)
            {
                Credentials = new NetworkCredential(smtpSettings.User, smtpSettings.Password),
                EnableSsl = smtpSettings.EnableSsl
            };
        }

        /// <summary>
        /// Sends an email asynchronously.
        /// </summary>
        /// <param name="message">The email message to send.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task SendMailAsync(MailMessage message)
        {
            await this._smtpClient.SendMailAsync(message);
        }
    }
}
