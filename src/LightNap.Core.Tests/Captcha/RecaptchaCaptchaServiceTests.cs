using System.Net;
using LightNap.Core.Configuration.Captcha;
using LightNap.Integrations.Captcha.Recaptcha;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace LightNap.Core.Tests.Captcha
{
    [TestClass]
    public class RecaptchaCaptchaServiceTests
    {
        private static RecaptchaV2CaptchaService CreateV2(TestHttpMessageHandler handler, bool rejectOnProviderError = true)
        {
            var settings = new CaptchaSettings
            {
                Provider = CaptchaProvider.RecaptchaV2,
                RejectOnProviderError = rejectOnProviderError,
                RecaptchaV2 = new RecaptchaV2Settings { SiteKey = "site-key", SecretKey = "secret-key" }
            };
            var httpClient = new HttpClient(handler);
            return new RecaptchaV2CaptchaService(httpClient, Options.Create(settings), NullLogger<RecaptchaV2CaptchaService>.Instance);
        }

        private static RecaptchaV3CaptchaService CreateV3(TestHttpMessageHandler handler, double minScore = 0.5, bool rejectOnProviderError = true)
        {
            var settings = new CaptchaSettings
            {
                Provider = CaptchaProvider.RecaptchaV3,
                RejectOnProviderError = rejectOnProviderError,
                RecaptchaV3 = new RecaptchaV3Settings { SiteKey = "site-key", SecretKey = "secret-key", MinScore = minScore }
            };
            var httpClient = new HttpClient(handler);
            return new RecaptchaV3CaptchaService(httpClient, Options.Create(settings), NullLogger<RecaptchaV3CaptchaService>.Instance);
        }

        [TestMethod]
        public async Task RecaptchaV2_ProviderReportsSuccess_ReturnsSuccess()
        {
            var service = CreateV2(new TestHttpMessageHandler(HttpStatusCode.OK, """{"success":true}"""));

            var result = await service.ValidateAsync("token");

            Assert.IsTrue(result.Success);
        }

        [TestMethod]
        public async Task RecaptchaV2_ProviderReportsFailure_ReturnsErrorCodes()
        {
            var service = CreateV2(new TestHttpMessageHandler(
                HttpStatusCode.OK,
                """{"success":false,"error-codes":["invalid-input-response"]}"""));

            var result = await service.ValidateAsync("token");

            Assert.IsFalse(result.Success);
            CollectionAssert.Contains(result.ErrorCodes, "invalid-input-response");
        }

        [TestMethod]
        public async Task RecaptchaV2_TransportError_FailClosed_Fails()
        {
            var service = CreateV2(new TestHttpMessageHandler(new HttpRequestException("boom")), rejectOnProviderError: true);

            var result = await service.ValidateAsync("token");

            Assert.IsFalse(result.Success);
            CollectionAssert.Contains(result.ErrorCodes, "provider-error");
        }

        [TestMethod]
        public async Task RecaptchaV2_TransportError_FailOpen_Succeeds()
        {
            var service = CreateV2(new TestHttpMessageHandler(new HttpRequestException("boom")), rejectOnProviderError: false);

            var result = await service.ValidateAsync("token");

            Assert.IsTrue(result.Success);
        }

        [TestMethod]
        public async Task RecaptchaV3_ScoreAboveThreshold_ReturnsSuccess()
        {
            var service = CreateV3(
                new TestHttpMessageHandler(HttpStatusCode.OK, """{"success":true,"score":0.9,"action":"submit"}"""),
                minScore: 0.5);

            var result = await service.ValidateAsync("token");

            Assert.IsTrue(result.Success);
            Assert.AreEqual(0.9, result.Score);
            Assert.AreEqual("submit", result.Action);
        }

        [TestMethod]
        public async Task RecaptchaV3_ScoreBelowThreshold_ReturnsFailure()
        {
            var service = CreateV3(
                new TestHttpMessageHandler(HttpStatusCode.OK, """{"success":true,"score":0.2,"action":"submit"}"""),
                minScore: 0.5);

            var result = await service.ValidateAsync("token");

            Assert.IsFalse(result.Success);
            Assert.AreEqual(0.2, result.Score);
            CollectionAssert.Contains(result.ErrorCodes, "score-too-low");
        }

        [TestMethod]
        public async Task RecaptchaV3_ProviderReportsFailure_ReturnsErrorCodes()
        {
            var service = CreateV3(new TestHttpMessageHandler(
                HttpStatusCode.OK,
                """{"success":false,"score":0.0,"error-codes":["invalid-input-response"]}"""));

            var result = await service.ValidateAsync("token");

            Assert.IsFalse(result.Success);
            CollectionAssert.Contains(result.ErrorCodes, "invalid-input-response");
        }
    }
}
