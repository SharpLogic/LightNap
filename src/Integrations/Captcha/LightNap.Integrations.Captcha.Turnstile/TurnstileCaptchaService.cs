using System.Net.Http.Json;
using System.Text.Json.Serialization;
using LightNap.Core.Captcha.Interfaces;
using LightNap.Core.Captcha.Models;
using LightNap.Core.Configuration.Captcha;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LightNap.Integrations.Captcha.Turnstile
{
    /// <summary>
    /// <see cref="ICaptchaService"/> implementation for Cloudflare Turnstile.
    /// </summary>
    public sealed class TurnstileCaptchaService(
        HttpClient httpClient,
        IOptions<CaptchaSettings> captchaOptions,
        ILogger<TurnstileCaptchaService> logger) : ICaptchaService
    {
        private const string SiteverifyUrl = "https://challenges.cloudflare.com/turnstile/v0/siteverify";

        /// <inheritdoc />
        public async Task<CaptchaResult> ValidateAsync(string token, string? remoteIp = null, CancellationToken cancellationToken = default)
        {
            var settings = captchaOptions.Value.Turnstile
                ?? throw new InvalidOperationException("TurnstileSettings is required when Captcha:Provider is Turnstile.");

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

                var payload = await response.Content.ReadFromJsonAsync<TurnstileResponse>(cancellationToken: cancellationToken);
                if (payload is null)
                {
                    return CaptchaResult.Failed("unknown-error");
                }

                if (payload.Success)
                {
                    return new CaptchaResult { Success = true, Action = payload.Action };
                }

                return CaptchaResult.Failed(payload.ErrorCodes ?? ["unknown-error"]);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Turnstile validation failed due to provider error");
                return captchaOptions.Value.RejectOnProviderError
                    ? CaptchaResult.Failed("provider-error")
                    : CaptchaResult.Succeeded();
            }
        }

        private sealed record TurnstileResponse(
            bool Success,
            [property: JsonPropertyName("error-codes")] string[]? ErrorCodes,
            string? Action);
    }
}
