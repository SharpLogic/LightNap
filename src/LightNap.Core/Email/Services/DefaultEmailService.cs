using LightNap.Core.Configuration;
using LightNap.Core.Data.Entities;
using LightNap.Core.Email.Interfaces;
using LightNap.Core.Email.Templates;
using LightNap.Core.Extensions;
using Microsoft.Extensions.Options;
using System.Net.Mail;

namespace LightNap.Core.Email.Services
{
    /// <summary>
    /// Service for sending emails using SMTP.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="DefaultEmailService"/> class.
    /// </remarks>
    /// <param name="emailSettings">The email settings.</param>
    /// <param name="emailSender">The email sending service.</param>
    public class DefaultEmailService(IOptions<EmailSettings> emailSettings, IEmailSender emailSender) : IEmailService
    {
        /// <summary>
        /// Sends an email asynchronously.
        /// </summary>
        /// <param name="message">The email message to send.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task SendMailAsync(MailMessage message)
        {
            await emailSender.SendMailAsync(message);
        }

        /// <summary>
        /// Sends an email asynchronously.
        /// </summary>
        /// <param name="message">The email message to send.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task SendMailAsync(ApplicationUser user, string subject, string body)
        {
            await emailSender.SendMailAsync(
                new MailMessage(new MailAddress(emailSettings.Value.FromEmail, emailSettings.Value.FromDisplayName), new MailAddress(user.Email!, user.UserName))
                {
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                });
        }

        /// <summary>
        /// Sends a password reset email to the specified user.
        /// </summary>
        /// <param name="user">The user to send the email to.</param>
        /// <param name="token">The token required to reset the password.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task SendPasswordResetAsync(ApplicationUser user, string token)
        {
            await this.SendMailAsync(user, "Reset your password",
                new ResetPasswordTemplate()
                {
                    FromDisplayName = emailSettings.Value.FromDisplayName,
                    SiteUrlRoot = emailSettings.Value.SiteUrlRootForLinks,
                    Token = token,
                    User = user
                }.TransformText());
        }

        /// <summary>
        /// Sends an email change email to the specified user.
        /// </summary>
        /// <param name="user">The user to send the email to.</param>
        /// <param name="token">The token for verifying the email change.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task SendChangeEmailAsync(ApplicationUser user, string newEmail, string token)
        {
            await emailSender.SendMailAsync(
                new MailMessage(emailSettings.Value.FromEmail, newEmail, "Confirm your email change",
                    new ChangeEmailTemplate()
                    {
                        FromDisplayName = emailSettings.Value.FromDisplayName,
                        NewEmail = newEmail,
                        SiteUrlRoot = emailSettings.Value.SiteUrlRootForLinks,
                        Token = token,
                        User = user
                    }.TransformText()));
        }

        /// <summary>
        /// Sends an email verification email to the specified user.
        /// </summary>
        /// <param name="user">The user to send the email to.</param>
        /// <param name="token">The token for verifying the email.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task SendEmailVerificationAsync(ApplicationUser user, string token)
        {
            await this.SendMailAsync(user, "Confirm your email",
                new ConfirmEmailTemplate()
                {
                    FromDisplayName = emailSettings.Value.FromDisplayName,
                    SiteUrlRoot = emailSettings.Value.SiteUrlRootForLinks,
                    Token = token,
                    User = user
                }.TransformText());
        }

        /// <summary>
        /// Sends a registration email to the specified user.
        /// </summary>
        /// <param name="user">The user to send the email to.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task SendRegistrationWelcomeAsync(ApplicationUser user)
        {
            await this.SendMailAsync(user, "Welcome to our site",
                new RegistrationWelcomeTemplate()
                {
                    FromDisplayName = emailSettings.Value.FromDisplayName,
                    SiteUrlRoot = emailSettings.Value.SiteUrlRootForLinks,
                    User = user
                }.TransformText());
        }

        /// <summary>
        /// Sends a two-factor authentication email to the specified user.
        /// </summary>
        /// <param name="user">The user to send the email to.</param>
        /// <param name="code">The two-factor authentication code.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task SendTwoFactorAsync(ApplicationUser user, string code)
        {
            await this.SendMailAsync(user, "Your login security code",
                new TwoFactorTemplate()
                {
                    Code = code,
                    FromDisplayName = emailSettings.Value.FromDisplayName,
                    SiteUrlRoot = emailSettings.Value.SiteUrlRootForLinks,
                    User = user
                }.TransformText());
        }

        /// <summary>
        /// Sends a magic link email to the specified user.
        /// </summary>
        /// <param name="user">The user to send the email to.</param>
        /// <param name="token">The magic link token.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task SendMagicLinkAsync(ApplicationUser user, string token)
        {
            await this.SendMailAsync(user, "Your login link",
                new MagicLinkTemplate()
                {
                    FromDisplayName = emailSettings.Value.FromDisplayName,
                    SiteUrlRoot = emailSettings.Value.SiteUrlRootForLinks,
                    Token = token,
                    User = user
                }.TransformText());
        }
    }
}
