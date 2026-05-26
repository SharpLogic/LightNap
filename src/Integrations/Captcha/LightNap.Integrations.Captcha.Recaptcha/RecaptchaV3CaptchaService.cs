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
    /// <see cref="ICaptchaService"/> implementation for Google reCAPTCHA v3.
    /// Returns success when the provider reports the token is valid AND the score
    /// meets <see cref="RecaptchaV3Settings.MinScore"/>.
    /// </summary>
    public sealed class RecaptchaV3CaptchaService(
        HttpClient httpClient,
        IOptions<CaptchaSettings> captchaOptions,
        ILogger<RecaptchaV3CaptchaService> logger) : ICaptchaService
    {
        /// <inheritdoc />
        public async Task<CaptchaResult> ValidateAsync(string token, string? remoteIp = null, CancellationToken cancellationToken = default)
        {
            var settings = captchaOptions.Value.RecaptchaV3
                ?? throw new InvalidOperationException("RecaptchaV3Settings is required when Captcha:Provider is RecaptchaV3.");

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
                using var response = await httpClient.PostAsync(RecaptchaV2CaptchaService.SiteverifyUrl, content, cancellationToken);
                response.EnsureSuccessStatusCode();

                var payload = await response.Content.ReadFromJsonAsync<RecaptchaV3Response>(cancellationToken: cancellationToken);
                if (payload is null)
                {
                    return CaptchaResult.Failed("unknown-error");
                }

                if (!payload.Success)
                {
                    return CaptchaResult.Failed(payload.ErrorCodes ?? ["unknown-error"]);
                }

                if (payload.Score < settings.MinScore)
                {
                    return new CaptchaResult
                    {
                        Success = false,
                        Score = payload.Score,
                        Action = payload.Action,
                        ErrorCodes = ["score-too-low"]
                    };
                }

                return new CaptchaResult { Success = true, Score = payload.Score, Action = payload.Action };
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "reCAPTCHA v3 validation failed due to provider error");
                return captchaOptions.Value.RejectOnProviderError
                    ? CaptchaResult.Failed("provider-error")
                    : CaptchaResult.Succeeded();
            }
        }

        private sealed record RecaptchaV3Response(
            bool Success,
            double Score,
            string? Action,
            [property: JsonPropertyName("error-codes")] string[]? ErrorCodes);
    }
}
