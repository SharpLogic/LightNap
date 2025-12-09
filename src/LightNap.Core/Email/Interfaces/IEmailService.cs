using LightNap.Core.Data.Entities;
using System.Net.Mail;

namespace LightNap.Core.Email.Interfaces
{
    /// <summary>
    /// Interface for email services. Besides the basic SendEmailAsync method, it also includes methods for sending 
    /// user lifecycle messages (like 2FA) so that it's easier to override that functionality with templates from
    /// email service providers without having to touch the identity code.
    /// </summary>
    public interface IEmailService
    {
        /// <summary>
        /// Sends an email asynchronously.
        /// </summary>
        /// <param name="message">The email message to send.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task SendMailAsync(MailMessage message);

        /// <summary>
        /// Sends a email change email to the specified user.
        /// </summary>
        /// <param name="user">The user to send the email to.</param>
        /// <param name="newEmail">The new email address.</param>
        /// <param name="token">The token for verifying the email change.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task SendChangeEmailAsync(ApplicationUser user, string newEmail, string token);

        /// <summary>
        /// Sends a two-factor authentication email to the specified user.
        /// </summary>
        /// <param name="user">The user to send the email to.</param>
        /// <param name="code">The two-factor authentication code.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task SendTwoFactorAsync(ApplicationUser user, string code);

        /// <summary>
        /// Sends a password reset email to the specified user.
        /// </summary>
        /// <param name="user">The user to send the email to.</param>
        /// <param name="token">The token for resetting the password.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task SendPasswordResetAsync(ApplicationUser user, string token);

        /// <summary>
        /// Sends a registration email to the specified user.
        /// </summary>
        /// <param name="user">The user to send the email to.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task SendRegistrationWelcomeAsync(ApplicationUser user);

        /// <summary>
        /// Sends a email verification email to the specified user.
        /// </summary>
        /// <param name="user">The user to send the email to.</param>
        /// <param name="token">The token for verifying the email.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task SendEmailVerificationAsync(ApplicationUser user, string token);

        /// <summary>
        /// Sends a magic link email to the specified user.
        /// </summary>
        /// <param name="user">The user to send the email to.</param>
        /// <param name="token">The magic link token.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task SendMagicLinkAsync(ApplicationUser user, string token);
    }
}