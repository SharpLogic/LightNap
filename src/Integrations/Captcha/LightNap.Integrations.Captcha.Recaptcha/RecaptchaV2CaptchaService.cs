using System.Net.Http.Json;
using System.Text.Json.Serialization;
using LightNap.Core.Captcha.Interfaces;
using LightNap.Core.Captcha.Models;
using LightNap.Core.Configuration.Captcha;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LightNap.Integrations.Captcha.Recaptcha
{
    /// <summary>
    /// <see cref="ICaptchaService"/> implementation for Google reCAPTCHA v2.
    /// </summary>
    public sealed class RecaptchaV2CaptchaService(
        HttpClient httpClient,
        IOptions<CaptchaSettings> captchaOptions,
        ILogger<RecaptchaV2CaptchaService> logger) : ICaptchaService
    {
        internal const string SiteverifyUrl = "https://www.google.com/recaptcha/api/siteverify";

        /// <inheritdoc />
        public async Task<CaptchaResult> ValidateAsync(string token, string? remoteIp = null, CancellationToken cancellationToken = default)
        {
            var settings = captchaOptions.Value.RecaptchaV2
                ?? throw new InvalidOperationException("RecaptchaV2Settings is required when Captcha:Provider is RecaptchaV2.");

            var formData = new Dictionary<string, string>
            {
                ["secret"] = settings.SecretKey,
                ["response"] = token
            };
            if (!string.IsNullOrEmpty(remoteIp))
            {
                formData["remoteip"] = remoteIp;
            }

            try
            {
                using var content = new FormUrlEncodedContent(formData);
                using var response = await httpClient.PostAsync(SiteverifyUrl, content, cancellationToken);
                response.EnsureSuccessStatusCode();

                var payload = await response.Content.ReadFromJsonAsync<RecaptchaV2Response>(cancellationToken: cancellationToken);
                if (payload is null)
                {
                    return CaptchaResult.Failed("unknown-error");
                }

                if (payload.Success)
                {
                    return CaptchaResult.Succeeded();
                }

                return CaptchaResult.Failed(payload.ErrorCodes ?? ["unknown-error"]);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "reCAPTCHA v2 validation failed due to provider error");
                return captchaOptions.Value.RejectOnProviderError
                    ? CaptchaResult.Failed("provider-error")
                    : CaptchaResult.Succeeded();
            }
        }

        internal sealed record RecaptchaV2Response(
            bool Success,
            [property: JsonPropertyName("error-codes")] string[]? ErrorCodes);
    }
}
