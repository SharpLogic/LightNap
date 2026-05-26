namespace LightNap.WebApi.Tests
{
    /// <summary>
    /// Confirms the project builds and the test runner discovers tests in this assembly.
    /// Real web-layer tests (middleware, action filters, WebUserContext, etc.) will
    /// be added in follow-up changes that move them out of LightNap.Core.Tests.
    /// </summary>
    [TestClass]
    public class SmokeTest
    {
        [TestMethod]
        public void TestRunner_DiscoversThisProject()
        {
            Assert.IsTrue(true);
        }
    }
}
