using LightNap.Core.Captcha.Interfaces;
using LightNap.Core.Captcha.Models;

namespace LightNap.Core.Captcha.Services
{
    /// <summary>
    /// A <see cref="ICaptchaService"/> that always succeeds. Used when CAPTCHA enforcement
    /// is disabled (development, tests).
    /// </summary>
    public sealed class NoOpCaptchaService : ICaptchaService
    {
        /// <inheritdoc />
        public Task<CaptchaResult> ValidateAsync(string token, string? remoteIp = null, CancellationToken cancellationToken = default)
            => Task.FromResult(CaptchaResult.Succeeded());
    }
}
