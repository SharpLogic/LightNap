using LightNap.Core.Api;
using LightNap.Core.Data.Entities;
using LightNap.Core.Integrations.Dto.Request;
using LightNap.Core.Integrations.Dto.Response;

namespace LightNap.Core.Integrations.Interfaces;

/// <summary>
/// Service for managing user integrations.
/// </summary>
public interface IIntegrationsService
{
    /// <summary>
    /// Gets the current user's integrations.
    /// </summary>
    /// <returns>The list of integrations.</returns>
    Task<List<IntegrationDto>> GetMyIntegrationsAsync();

    /// <summary>
    /// Creates a new integration for the current user.
    /// </summary>
    /// <param name="createDto">The creation parameters.</param>
    /// <returns>The created integration.</returns>
    Task<IntegrationDto> CreateMyIntegrationAsync(CreateIntegrationRequestDto createDto);

    /// <summary>
    /// Searches all integrations.
    /// </summary>
    /// <param name="searchDto">The search parameters.</param>
    /// <returns>The integration search results.</returns>
    Task<PagedResponseDto<AdminIntegrationDto>> SearchIntegrationsAsync(SearchIntegrationsRequestDto searchDto);

    /// <summary>
    /// Updates the current user's specified integration.
    /// </summary>
    /// <param name="integrationId">The integration ID,</param>
    /// <param name="updateDto">The update parameters.</param>
    /// <returns></returns>
    Task<IntegrationDto> UpdateMyIntegrationAsync(int integrationId, UpdateIntegrationRequestDto updateDto);

    /// <summary>
    /// Deltes the current user's integration with the specified ID.
    /// </summary>
    /// <param name="integrationId">The integration ID.</param>
    /// <returns>A task.</returns>
    Task DeleteMyIntegrationAsync(int integrationId);

    /// <summary>
    /// Deltes the integration with the specified ID.
    /// </summary>
    /// <param name="integrationId">The integration ID.</param>
    /// <returns>A task.</returns>
    Task DeleteIntegrationAsync(int integrationId);

}