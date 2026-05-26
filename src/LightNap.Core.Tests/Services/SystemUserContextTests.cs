using LightNap.Core.Api;
using LightNap.Core.Configuration;
using LightNap.Core.Interfaces;

namespace LightNap.Core.Tests.Services
{
    [TestClass]
    public class SystemUserContextTests
    {
        [TestMethod]
        public void SystemUserContext_KindIsSystem()
        {
            var context = new SystemUserContext();

            Assert.AreEqual(UserContextKind.System, context.Kind);
            Assert.AreEqual(Constants.Identity.SystemUserId, context.GetActorId());
            Assert.AreEqual("system", context.GetActorId());
        }
    }
}
