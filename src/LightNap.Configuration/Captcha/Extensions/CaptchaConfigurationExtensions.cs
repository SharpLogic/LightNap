using LightNap.Core.Captcha.Interfaces;
using LightNap.Core.Captcha.Services;
using LightNap.Core.Configuration.Captcha;
using LightNap.Integrations.Captcha.Recaptcha.Extensions;
using LightNap.Integrations.Captcha.Turnstile.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LightNap.Configuration.Captcha.Extensions
{
    /// <summary>
    /// Hub that selects and registers the configured CAPTCHA provider, mirroring the OAuth hub
    /// pattern: the WebApi host never references vendor integration projects directly.
    /// </summary>
    public static class CaptchaConfigurationExtensions
    {
        extension(IServiceCollection services)
        {
            /// <summary>
            /// Registers the configured <see cref="ICaptchaService"/> based on
            /// <see cref="CaptchaSettings.Provider"/>.
            /// </summary>
            /// <param name="settings">The CAPTCHA settings (bound from configuration).</param>
            /// <param name="logger">An optional logger used to report what was wired up.</param>
            /// <returns>The updated service collection.</returns>
            /// <exception cref="ArgumentException">Thrown when the configured provider is unsupported.</exception>
            public IServiceCollection AddLightNapCaptchaService(CaptchaSettings settings, ILogger? logger = null)
            {
                logger?.LogInformation("Configuring CAPTCHA provider: {Provider}", settings.Provider);
                switch (settings.Provider)
                {
                    case CaptchaProvider.None:
                        services.AddSingleton<ICaptchaService, NoOpCaptchaService>();
                        break;
                    case CaptchaProvider.Turnstile:
                        services.AddTurnstileCaptchaService(logger);
                        break;
                    case CaptchaProvider.RecaptchaV2:
                        services.AddRecaptchaV2CaptchaService(logger);
                        break;
                    case CaptchaProvider.RecaptchaV3:
                        services.AddRecaptchaV3CaptchaService(logger);
                        break;
                    default:
                        throw new ArgumentException($"Unsupported 'Captcha:Provider' setting: '{settings.Provider}'");
                }
                return services;
            }
        }
    }
}
