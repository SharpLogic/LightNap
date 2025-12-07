namespace LightNap.Core.Configuration.Email
{
    /// <summary>
    /// Represents the email provider options.
    /// </summary>
    public enum EmailProvider
    {
        /// <summary>
        /// Logs emails to the console instead of sending them.
        /// </summary>
        LogToConsole,

        /// <summary>
        /// Sends emails using SMTP.
        /// </summary>
        Smtp
    }
}