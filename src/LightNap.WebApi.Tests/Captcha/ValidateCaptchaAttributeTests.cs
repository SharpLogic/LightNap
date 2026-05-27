using LightNap.Core.Captcha.Interfaces;
using LightNap.Core.Captcha.Models;
using LightNap.Core.Captcha.Services;
using LightNap.Core.Configuration.Captcha;
using LightNap.WebApi.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace LightNap.WebApi.Tests.Captcha
{
    [TestClass]
    public class ValidateCaptchaAttributeTests
    {
        private sealed class FailingCaptchaService : ICaptchaService
        {
            public Task<CaptchaResult> ValidateAsync(string token, string? remoteIp = null, CancellationToken cancellationToken = default)
                => Task.FromResult(CaptchaResult.Failed("invalid-token"));
        }

        private static ActionExecutingContext BuildContext(
            ICaptchaService captchaService,
            string? headerValue,
            CaptchaProvider provider = CaptchaProvider.Turnstile)
        {
            var services = new ServiceCollection();
            services.AddSingleton(captchaService);
            services.AddSingleton<IOptions<CaptchaSettings>>(
                Options.Create(new CaptchaSettings { Provider = provider }));
            var serviceProvider = services.BuildServiceProvider();

            var httpContext = new DefaultHttpContext { RequestServices = serviceProvider };
            if (headerValue is not null)
            {
                httpContext.Request.Headers[ValidateCaptchaAttribute.TokenHeaderName] = headerValue;
            }

            var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
            return new ActionExecutingContext(actionContext, [], new Dictionary<string, object?>(), controller: new object());
        }

        [TestMethod]
        public async Task MissingHeader_Returns400_WhenProviderIsEnabled()
        {
            var filter = new ValidateCaptchaAttribute();
            var context = BuildContext(new NoOpCaptchaService(), headerValue: null);
            bool nextCalled = false;

            await filter.OnActionExecutionAsync(context, () =>
            {
                nextCalled = true;
                return Task.FromResult<ActionExecutedContext>(null!);
            });

            Assert.IsFalse(nextCalled, "Action should not run when CAPTCHA header is missing.");
            Assert.IsInstanceOfType(context.Result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task InvalidToken_Returns400()
        {
            var filter = new ValidateCaptchaAttribute();
            var context = BuildContext(new FailingCaptchaService(), headerValue: "bad-token");
            bool nextCalled = false;

            await filter.OnActionExecutionAsync(context, () =>
            {
                nextCalled = true;
                return Task.FromResult<ActionExecutedContext>(null!);
            });

            Assert.IsFalse(nextCalled, "Action should not run when CAPTCHA validation fails.");
            Assert.IsInstanceOfType(context.Result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task ValidToken_InvokesAction()
        {
            var filter = new ValidateCaptchaAttribute();
            var context = BuildContext(new NoOpCaptchaService(), headerValue: "good-token");
            bool nextCalled = false;

            await filter.OnActionExecutionAsync(context, () =>
            {
                nextCalled = true;
                var executed = new ActionExecutedContext(context, [], controller: new object());
                return Task.FromResult(executed);
            });

            Assert.IsTrue(nextCalled, "Action should run when CAPTCHA token is valid.");
            Assert.IsNull(context.Result);
        }

        [TestMethod]
        public async Task ProviderNone_BypassesHeaderRequirement_InvokesAction()
        {
            // When the configured provider is None, dev/test environments should be able to
            // call decorated endpoints without sending an X-Captcha-Token header.
            var filter = new ValidateCaptchaAttribute();
            var context = BuildContext(
                new NoOpCaptchaService(),
                headerValue: null,
                provider: CaptchaProvider.None);
            bool nextCalled = false;

            await filter.OnActionExecutionAsync(context, () =>
            {
                nextCalled = true;
                var executed = new ActionExecutedContext(context, [], controller: new object());
                return Task.FromResult(executed);
            });

            Assert.IsTrue(nextCalled, "Action should run without a header when Provider is None.");
            Assert.IsNull(context.Result);
        }
    }
}
