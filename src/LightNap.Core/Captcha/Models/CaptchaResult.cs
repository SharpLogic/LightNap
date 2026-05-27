namespace LightNap.Core.Captcha.Models
{
    /// <summary>
    /// Result of a CAPTCHA verification call.
    /// </summary>
    public sealed class CaptchaResult
    {
        /// <summary>
        /// True when the provider considers the token valid.
        /// </summary>
        public required bool Success { get; init; }

        /// <summary>
        /// For score-based providers (reCAPTCHA v3, Turnstile). Null for binary providers.
        /// </summary>
        public double? Score { get; init; }

        /// <summary>
        /// For providers that echo back the action the user took (e.g., "login", "submit").
        /// </summary>
        public string? Action { get; init; }

        /// <summary>
        /// Provider-supplied error codes when <see cref="Success"/> is false. Empty otherwise.
        /// </summary>
        public string[] ErrorCodes { get; init; } = [];

        /// <summary>
        /// Convenience factory for a successful result.
        /// </summary>
        public static CaptchaResult Succeeded() => new() { Success = true };

        /// <summary>
        /// Convenience factory for a failed result with the given error codes.
        /// </summary>
        public static CaptchaResult Failed(params string[] errorCodes) => new()
        {
            Success = false,
            ErrorCodes = errorCodes
        };
    }
}
