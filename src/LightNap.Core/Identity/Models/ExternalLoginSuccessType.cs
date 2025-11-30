namespace LightNap.Core.Identity.Models
{
    /// <summary>
    /// Represents the result of a successful external login operation.
    /// </summary>
    public enum ExternalLoginSuccessType
    {
        /// <summary>
        /// The user has successfully logged in.
        /// </summary>
        AlreadyLinked,

        /// <summary>
        /// The user has linked an external account to an existing account.
        /// </summary>
        NewAccountLink,

        /// <summary>
        /// The user needs to complete registration.
        /// </summary>
        RequiresRegistration,
    }
}
