using LightNap.Core.Configuration.Integrations;
using LightNap.Core.Integrations.Dto.Request;
using LightNap.Core.Integrations.Models;
using Microsoft.AspNetCore.Identity;

namespace LightNap.Core.Integrations.Interfaces;

/// <summary>
/// Defines the contract for integration APIs that facilitate communication between external systems and the
/// application.
/// </summary>
/// <remarks>Implementations of this interface should provide methods and properties necessary for interacting
/// with external services or platforms. This interface serves as an abstraction layer, enabling decoupling and easier
/// testing of integration logic.</remarks>
public interface IIntegrationProvider
{
    /// <summary>
    /// The definition details for the integration provider associated with this instance.
    /// </summary>
    IntegrationProviderDefinition Definition { get; }

    /// <summary>
    /// Builds a request object for creating an integration using information from an external OAuth login.
    /// </summary>
    /// <param name="loginInfo">The external login information obtained from the OAuth provider. Must not be null.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a request DTO for creating the
    /// integration based on the provided external login information.</returns>
    Task<CreateIntegrationFromOAuthRequestDto> BuildCreateIntegrationRequest(ExternalLoginInfo loginInfo);
}