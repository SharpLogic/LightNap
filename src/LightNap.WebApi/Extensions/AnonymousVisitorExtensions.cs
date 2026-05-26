using LightNap.Configuration.AnonymousVisitor;
using LightNap.WebApi.Middleware;
using Microsoft.Extensions.Options;

namespace LightNap.WebApi.Extensions
{
    /// <summary>
    /// Service and pipeline registration helpers for anonymous visitor tracking.
    /// </summary>
    public static class AnonymousVisitorExtensions
    {
        extension(IServiceCollection services)
        {
            /// <summary>
            /// Registers <see cref="AnonymousVisitorSettings"/> so the visitor middleware can resolve them.
            /// </summary>
            /// <param name="settings">The visitor settings, typically bound from configuration.</param>
            /// <param name="logger">An optional logger used to report what was wired up.</param>
            /// <returns>The updated service collection.</returns>
            public IServiceCollection AddLightNapAnonymousVisitorTracking(AnonymousVisitorSettings settings, ILogger? logger = null)
            {
                logger?.LogInformation("Configuring anonymous visitor tracking (cookie: {Cookie}, lifetime: {Lifetime})",
                    settings.CookieName, settings.Lifetime);
                services.Configure<AnonymousVisitorSettings>(o =>
                {
                    o.CookieName = settings.CookieName;
                    o.Lifetime = settings.Lifetime;
                    o.SecureOnly = settings.SecureOnly;
                });
                return services;
            }
        }

        extension(IApplicationBuilder app)
        {
            /// <summary>
            /// Adds the anonymous visitor middleware to the request pipeline.
            /// Place after authentication and before endpoint mapping.
            /// </summary>
            /// <returns>The same <see cref="IApplicationBuilder"/> for chaining.</returns>
            public IApplicationBuilder UseLightNapAnonymousVisitorTracking()
                => app.UseMiddleware<AnonymousVisitorIdMiddleware>();
        }
    }
}
