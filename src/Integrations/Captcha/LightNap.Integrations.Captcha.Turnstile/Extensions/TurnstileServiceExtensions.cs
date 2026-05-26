using LightNap.Core.Captcha.Interfaces;
using LightNap.Core.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LightNap.Integrations.Captcha.Turnstile.Extensions
{
    /// <summary>
    /// Extension methods for registering <see cref="TurnstileCaptchaService"/>.
    /// </summary>
    public static class TurnstileServiceExtensions
    {
        extension(IServiceCollection services)
        {
            /// <summary>
            /// Registers <see cref="TurnstileCaptchaService"/> as the <see cref="ICaptchaService"/>
            /// implementation, with the LightNap standard resilience handler applied to its
            /// underlying <see cref="HttpClient"/>.
            /// </summary>
            /// <param name="logger">An optional logger used to report what was wired up.</param>
            /// <returns>The updated service collection.</returns>
            public IServiceCollection AddTurnstileCaptchaService(ILogger? logger = null)
            {
                logger?.LogInformation("Configuring Cloudflare Turnstile CAPTCHA service");
                services.AddLightNapResilientHttpClient<ICaptchaService, TurnstileCaptchaService>();
                return services;
            }
        }
    }
}
