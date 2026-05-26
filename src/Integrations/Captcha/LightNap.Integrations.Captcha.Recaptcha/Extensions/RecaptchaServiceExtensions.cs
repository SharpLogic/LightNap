using LightNap.Core.Captcha.Interfaces;
using LightNap.Core.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LightNap.Integrations.Captcha.Recaptcha.Extensions
{
    /// <summary>
    /// Extension methods for registering reCAPTCHA-backed <see cref="ICaptchaService"/> implementations.
    /// </summary>
    public static class RecaptchaServiceExtensions
    {
        extension(IServiceCollection services)
        {
            /// <summary>
            /// Registers <see cref="RecaptchaV2CaptchaService"/> as the <see cref="ICaptchaService"/>
            /// implementation with the LightNap standard resilience handler.
            /// </summary>
            public IServiceCollection AddRecaptchaV2CaptchaService(ILogger? logger = null)
            {
                logger?.LogInformation("Configuring Google reCAPTCHA v2 CAPTCHA service");
                services.AddLightNapResilientHttpClient<ICaptchaService, RecaptchaV2CaptchaService>();
                return services;
            }

            /// <summary>
            /// Registers <see cref="RecaptchaV3CaptchaService"/> as the <see cref="ICaptchaService"/>
            /// implementation with the LightNap standard resilience handler.
            /// </summary>
            public IServiceCollection AddRecaptchaV3CaptchaService(ILogger? logger = null)
            {
                logger?.LogInformation("Configuring Google reCAPTCHA v3 CAPTCHA service");
                services.AddLightNapResilientHttpClient<ICaptchaService, RecaptchaV3CaptchaService>();
                return services;
            }
        }
    }
}
