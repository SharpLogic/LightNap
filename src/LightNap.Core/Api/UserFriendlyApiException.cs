namespace LightNap.Core.Api
{
    /// <summary>
    /// Represents an exception that is user-friendly and contains a list of error messages. The provided string/s will be
    /// returned to the user as-is, so it is recommended to use this exception wen you want the user to see those specific errors.
    /// For example, an invalid login attempt or unsuccessful attempt to delete an item that doesn't exist. However, the inner
    /// exception, if provided, is logged but not returned to the user.
    /// </summary>
    public class UserFriendlyApiException : Exception
    {
        /// <summary>
        /// Gets the collection of error messages.
        /// </summary>
        public readonly IEnumerable<string> Errors;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserFriendlyApiException"/> class with a collection of error messages and an optional inner exception.
        /// </summary>
        /// <param name="errors">The collection of error messages to be returned to the end user.</param>
        /// <param name="innerException">The optional inner exception to be logged but not returned to the end user.</param>
        public UserFriendlyApiException(IEnumerable<string> errors, Exception? innerException = null) : this(errors?.FirstOrDefault() ?? "No error messages provided", innerException)
        {
            this.Errors = errors ?? [this.Message];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserFriendlyApiException"/> class with a single error message and an optional inner exception.
        /// </summary>
        /// <param name="error">The error message to be returned to the end user.</param>
        /// <param name="innerException">The optional inner exception to be logged but not returned to the end user.</param>
        public UserFriendlyApiException(string error, Exception? innerException = null) : base(error, innerException)
        {
            this.Errors = [error];
        }
    }
}
