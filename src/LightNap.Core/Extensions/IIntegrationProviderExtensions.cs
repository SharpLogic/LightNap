using LightNap.Core.Integrations.Interfaces;
using LightNap.Core.Integrations.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;

namespace LightNap.Core.Extensions;

/// <summary>
/// Provides extension methods for retrieving authentication token values and related information from an external login
/// provider.
/// </summary>
/// <remarks>These extension methods simplify access to common authentication tokens and related metadata when
/// working with external authentication providers. They are intended to be used with implementations of the
/// IIntegrationProvider interface to facilitate token management and validation.</remarks>
public static class IIntegrationProviderExtensions
{
    extension(IIntegrationProvider integrationProvider)
    {
        /// <summary>
        /// Throws an exception if the specified integration feature is not supported by the current integration
        /// provider.
        /// </summary>
        /// <param name="feature">The integration feature to check for support.</param>
        /// <exception cref="InvalidOperationException">Thrown if the specified feature is not supported by the integration provider.</exception>
        public void AssertFeatureSupported(IntegrationFeature feature)
        {
            if (!integrationProvider.Definition.Features.Contains(feature))
            {
                throw new InvalidOperationException($"Integration of type '{integrationProvider.Definition.Key}' does not support feature '{feature}'.");
            }
        }
    }
}