namespace LightNap.Core.Data.Entities
{
    /// <summary>
    /// Represents a refresh token used for authentication.
    /// </summary>
    public class RefreshToken
    {
        /// <summary>
        /// Gets or sets the unique identifier for the refresh token.
        /// </summary>
        public required string Id { get; set; }

        /// <summary>
        /// Gets or sets the token string.
        /// </summary>
        public required string Token { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the token was last seen.
        /// </summary>
        public DateTime LastSeen { get; set; }

        /// <summary>
        /// Gets or sets the IP address from which the token was issued.
        /// </summary>
        public required string IpAddress { get; set; }

        /// <summary>
        /// Gets or sets the expiration date and time of the token.
        /// </summary>
        public DateTime Expires { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the token is revoked.
        /// </summary>
        public bool IsRevoked { get; set; }

        /// <summary>
        /// True if the cookie generated for this token is persistent across browser sessions.
        /// </summary>
        public bool IsPersistent { get; set; }

        /// <summary>
        /// Gets or sets additional details about the token.
        /// </summary>
        public required string Details { get; set; }

        /// <summary>
        /// Gets or sets the user identifier associated with the token.
        /// </summary>
        public required string UserId { get; set; }

        /// <summary>
        /// Gets or sets the user associated with the token.
        /// </summary>
        public ApplicationUser? User { get; set; }
    }

}
