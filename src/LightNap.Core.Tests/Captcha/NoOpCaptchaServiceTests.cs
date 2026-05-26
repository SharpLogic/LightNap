using LightNap.Core.Captcha.Services;

namespace LightNap.Core.Tests.Captcha
{
    [TestClass]
    public class NoOpCaptchaServiceTests
    {
        [TestMethod]
        public async Task ValidateAsync_AlwaysSucceeds()
        {
            var service = new NoOpCaptchaService();

            var result = await service.ValidateAsync("any-token", "127.0.0.1");

            Assert.IsTrue(result.Success);
            Assert.AreEqual(0, result.ErrorCodes.Length);
        }

        [TestMethod]
        public async Task ValidateAsync_EmptyToken_StillSucceeds()
        {
            var service = new NoOpCaptchaService();

            var result = await service.ValidateAsync(string.Empty);

            Assert.IsTrue(result.Success);
        }
    }
}
