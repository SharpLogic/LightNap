using System.Net;
using LightNap.Core.Configuration.Captcha;
using LightNap.Integrations.Captcha.Turnstile;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace LightNap.Core.Tests.Captcha
{
    [TestClass]
    public class TurnstileCaptchaServiceTests
    {
        private static (TurnstileCaptchaService service, TestHttpMessageHandler handler) CreateService(
            TestHttpMessageHandler handler,
            bool rejectOnProviderError = true)
        {
            var settings = new CaptchaSettings
            {
                Provider = CaptchaProvider.Turnstile,
                RejectOnProviderError = rejectOnProviderError,
                Turnstile = new TurnstileSettings { SiteKey = "site-key", SecretKey = "secret-key" }
            };
            var options = Options.Create(settings);
            var httpClient = new HttpClient(handler);
            return (new TurnstileCaptchaService(httpClient, options, NullLogger<TurnstileCaptchaService>.Instance), handler);
        }

        [TestMethod]
        public async Task ValidateAsync_ProviderReportsSuccess_ReturnsSuccess()
        {
            var (service, _) = CreateService(new TestHttpMessageHandler(HttpStatusCode.OK, """{"success":true,"action":"login"}"""));

            var result = await service.ValidateAsync("token");

            Assert.IsTrue(result.Success);
            Assert.AreEqual("login", result.Action);
        }

        [TestMethod]
        public async Task ValidateAsync_ProviderReportsFailure_ReturnsErrorCodes()
        {
            var (service, _) = CreateService(new TestHttpMessageHandler(
                HttpStatusCode.OK,
                """{"success":false,"error-codes":["invalid-input-response","timeout-or-duplicate"]}"""));

            var result = await service.ValidateAsync("token");

            Assert.IsFalse(result.Success);
            CollectionAssert.AreEquivalent(new[] { "invalid-input-response", "timeout-or-duplicate" }, result.ErrorCodes);
        }

        [TestMethod]
        public async Task ValidateAsync_TransportError_FailClosed_ReturnsProviderError()
        {
            var (service, _) = CreateService(new TestHttpMessageHandler(new HttpRequestException("boom")), rejectOnProviderError: true);

            var result = await service.ValidateAsync("token");

            Assert.IsFalse(result.Success);
            CollectionAssert.Contains(result.ErrorCodes, "provider-error");
        }

        [TestMethod]
        public async Task ValidateAsync_TransportError_FailOpen_ReturnsSuccess()
        {
            var (service, _) = CreateService(new TestHttpMessageHandler(new HttpRequestException("boom")), rejectOnProviderError: false);

            var result = await service.ValidateAsync("token");

            Assert.IsTrue(result.Success);
        }

        [TestMethod]
        public async Task ValidateAsync_PassesRemoteIp_WhenProvided()
        {
            var (service, handler) = CreateService(new TestHttpMessageHandler(HttpStatusCode.OK, """{"success":true}"""));

            await service.ValidateAsync("token", "203.0.113.42");

            Assert.IsNotNull(handler.LastRequestBody);
            StringAssert.Contains(handler.LastRequestBody, "remoteip=203.0.113.42");
        }
    }
}
