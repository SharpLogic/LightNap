using LightNap.Core.Captcha.Models;

namespace LightNap.Core.Captcha.Interfaces
{
    /// <summary>
    /// Validates client-submitted CAPTCHA tokens against the configured provider.
    /// </summary>
    public interface ICaptchaService
    {
        /// <summary>
        /// Validates the client-submitted CAPTCHA token.
        /// </summary>
        /// <param name="token">The token issued by the client-side widget.</param>
        /// <param name="remoteIp">The client's IP address, if available. Some providers use it as a signal.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A <see cref="CaptchaResult"/> describing the verification outcome.</returns>
        Task<CaptchaResult> ValidateAsync(string token, string? remoteIp = null, CancellationToken cancellationToken = default);
    }
}
