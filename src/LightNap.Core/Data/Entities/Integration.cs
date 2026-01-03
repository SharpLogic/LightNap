using LightNap.Core.Configuration;
using LightNap.Core.Integrations.Models;
using System.ComponentModel.DataAnnotations;

namespace LightNap.Core.Data.Entities
{
    /// <summary>
    /// Integration data for a user's access to an external API.
    /// </summary>
    public class Integration
    {
        /// <summary>
        /// The ID.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The user ID.
        /// </summary>
        public required string UserId { get; set; }

        /// <summary>
        /// The user.
        /// </summary>
        public ApplicationUser? User { get; set; }

        /// <summary>
        /// When the integration was created.
        /// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// The external provider this integration is for.
        /// </summary>
        public required string ProviderKey { get; set; }

        /// <summary>
        /// The expiration for integrations that can be automatically refreshed.
        /// </summary>
        public DateTime? CredentialsExpiration { get; set; }

        /// <summary>
        /// The expiration for integrations that need to be manually reauthorized.
        /// </summary>
        public DateTime? AuthorizationExpiration { get; set; }

        /// <summary>
        /// THe last time the integration was updated.
        /// </summary>
        public DateTime LastUpdated { get; set; }

        /// <summary>
        /// The friendly name for the integration assigned by the user.
        /// </summary>
        [MaxLength(Constants.Dto.MaxIntegrationFriendlyNameLength)]
        public required string FriendlyName { get; set; }

        /// <summary>
        /// True if the credentials should be included when the user requests the integration from a client request.
        /// </summary>
        public bool ShareWithClient { get; set; }

        /// <summary>
        /// The encrypted bits for the integration credentials (like an access token).
        /// </summary>
        public required string EncryptedCredentials { get; set; }

        /// <summary>
        /// The encrypted bits for the refresh credentials (like a refresh token).
        /// </summary>
        public string? EncryptedRefreshCredentials { get; set; }

        /// <summary>
        /// True if the integration is expired.
        /// </summary>
        public bool IsExpired { get; set; }

        /// <summary>
        /// The active error, if any.
        /// </summary>
        public string? Error { get; set; }
    }
}
