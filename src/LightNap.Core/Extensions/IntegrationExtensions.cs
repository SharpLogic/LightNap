using LightNap.Core.Data.Entities;
using LightNap.Core.Integrations.Dto.Request;
using LightNap.Core.Integrations.Dto.Response;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.VisualBasic;
using System.Text;

namespace LightNap.Core.Extensions
{
    internal static class IntegrationExtensions
    {
        /// <summary>
        /// Maps a create DTO to an entity.
        /// </summary>
        /// <param name="createDto">The creation parameters.</param>
        /// <param name="userId">The user this integration is for.</param>
        /// <param name="dataProtector">A data protector.</param>
        /// <returns></returns>
        public static Integration ToEntity(this CreateIntegrationRequestDto createDto, string userId, IDataProtector dataProtector)
        {
            createDto.Credentials = createDto.Credentials ?? string.Empty;

            return new Integration()
            {
                CreatedDate = DateTime.UtcNow,
                EncryptedCredentials = dataProtector.Protect(createDto.Credentials!),
                FriendlyName = createDto.FriendlyName,
                LastUpdated = DateTime.UtcNow,
                Provider = createDto.Provider,
                ShareWithClient = createDto.ShareWithClient,
                UserId = userId,
            };
        }

        /// <summary>
        /// Updates an entity from the update DTO.
        /// </summary>
        /// <param name="updateDto">The update parameters.</param>
        /// <param name="integration">The entity.</param>
        /// <param name="dataProtector">A data protector.</param>
        /// <returns></returns>
        public static void UpdateEntity(this UpdateIntegrationRequestDto updateDto, Integration integration, IDataProtector dataProtector)
        {
            if (!string.IsNullOrWhiteSpace(updateDto.Credentials))
            {
                integration.EncryptedCredentials = dataProtector.Protect(updateDto.Credentials);
            }

            integration.Error = string.Empty;
            integration.FriendlyName = updateDto.FriendlyName;
            integration.LastUpdated = DateTime.UtcNow;
            integration.ShareWithClient = updateDto.ShareWithClient;
        }

        /// <summary>
        /// Converts an integration to a DTO.
        /// </summary>
        /// <param name="integration">The integration.</param>
        /// <param name="dataProtector">A data protector.</param>
        /// <returns>The converted DTO.</returns>
        public static IntegrationDto ToDto(this Integration integration, IDataProtector dataProtector)
        {
            return new IntegrationDto()
            {
                CreatedDate = integration.CreatedDate,
                Credentials = integration.ShareWithClient ? dataProtector.Unprotect(integration.EncryptedCredentials) : null,
                Error = integration.Error,
                Expiration = integration.Expiration,
                FriendlyName = integration.FriendlyName,
                Id = integration.Id,
                IsExpired = integration.IsExpired,
                LastUpdated = integration.LastUpdated,
                Provider = integration.Provider,
                ShareWithClient = integration.ShareWithClient,
            };
        }

        /// <summary>
        /// Converts an integration to an admun DTO for search results.
        /// </summary>
        /// <param name="integration">The integration.</param>
        /// <param name="dataProtector">A data protector.</param>
        /// <returns>The converted DTO.</returns>
        internal static AdminIntegrationDto ToAdminDto(this Integration integration, IDataProtector dataProtector)
        {
            return new AdminIntegrationDto()
            {
                CreatedDate = integration.CreatedDate,
                Credentials = integration.ShareWithClient ? dataProtector.Unprotect(integration.EncryptedCredentials) : null,
                Error = integration.Error,
                Expiration = integration.Expiration,
                FriendlyName = integration.FriendlyName,
                Id = integration.Id,
                IsExpired = integration.IsExpired,
                LastUpdated = integration.LastUpdated,
                Provider = integration.Provider,
                ShareWithClient = integration.ShareWithClient,
                UserId = integration.UserId,
            };
        }
    }
}