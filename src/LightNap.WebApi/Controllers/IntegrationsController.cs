using LightNap.Core.Api;
using LightNap.Core.Configuration;
using LightNap.Core.Integrations.Dto.Request;
using LightNap.Core.Integrations.Dto.Response;
using LightNap.Core.Integrations.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LightNap.WebApi.Controllers
{
    /// <summary>
    /// Controller for managing user integrations with external services.
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class IntegrationsController(IIntegrationsService integrationsService) : ControllerBase
    {
        /// <summary>
        /// Searches all integrations (admin only).
        /// </summary>
        /// <param name="searchIntegrationsRequest">The search parameters.</param>
        /// <returns>The integrations matching the search criteria.</returns>
        /// <response code="200">Returns the search results.</response>
        [Authorize(Roles = Constants.Roles.Administrator)]
        [HttpPost("search", Name = nameof(SearchIntegrations))]
        public async Task<ApiResponseDto<PagedResponseDto<AdminIntegrationDto>>> SearchIntegrations([FromBody] SearchIntegrationsRequestDto searchIntegrationsRequest)
        {
            return new ApiResponseDto<PagedResponseDto<AdminIntegrationDto>>(await integrationsService.SearchIntegrationsAsync(searchIntegrationsRequest));
        }

        /// <summary>
        /// Deletes an integration by ID (admin only).
        /// </summary>
        /// <param name="integrationId">The ID of the integration to delete.</param>
        /// <returns>True if the integration was successfully deleted.</returns>
        /// <response code="200">Integration successfully deleted.</response>
        /// <response code="400">If there was an error deleting the integration.</response>
        [Authorize(Roles = Constants.Roles.Administrator)]
        [HttpDelete("{integrationId}", Name = nameof(DeleteIntegration))]
        public async Task<ApiResponseDto<bool>> DeleteIntegration(int integrationId)
        {
            await integrationsService.DeleteIntegrationAsync(integrationId);
            return new ApiResponseDto<bool>(true);
        }
    }
}
