using LightNap.Core.Captcha.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace LightNap.WebApi.Filters
{
    /// <summary>
    /// Validates a CAPTCHA token from the <c>X-Captcha-Token</c> request header before invoking
    /// the decorated action. Returns 400 with a structured error body on failure.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public sealed class ValidateCaptchaAttribute : Attribute, IAsyncActionFilter
    {
        /// <summary>
        /// The name of the request header that carries the client-submitted CAPTCHA token.
        /// </summary>
        public const string TokenHeaderName = "X-Captcha-Token";

        /// <inheritdoc />
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var token = context.HttpContext.Request.Headers[TokenHeaderName].ToString();
            if (string.IsNullOrEmpty(token))
            {
                context.Result = new BadRequestObjectResult(new
                {
                    error = "captcha_required",
                    message = $"CAPTCHA token required in {TokenHeaderName} header."
                });
                return;
            }

            var captchaService = context.HttpContext.RequestServices.GetRequiredService<ICaptchaService>();
            var remoteIp = context.HttpContext.Connection.RemoteIpAddress?.ToString();
            var result = await captchaService.ValidateAsync(token, remoteIp, context.HttpContext.RequestAborted);

            if (!result.Success)
            {
                context.Result = new BadRequestObjectResult(new
                {
                    error = "captcha_invalid",
                    message = "CAPTCHA validation failed.",
                    errorCodes = result.ErrorCodes
                });
                return;
            }

            await next();
        }
    }
}
