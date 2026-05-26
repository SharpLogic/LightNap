using System.Security.Claims;
using LightNap.Core.Api;
using LightNap.Core.Configuration;
using LightNap.Core.Interfaces;
using LightNap.WebApi.Services;
using Microsoft.AspNetCore.Http;
using Moq;

namespace LightNap.Core.Tests.Services
{
    [TestClass]
    public class UserContextTests
    {
        private static WebUserContext CreateWebUserContext(HttpContext httpContext)
        {
            var accessor = new Mock<IHttpContextAccessor>();
            accessor.Setup(a => a.HttpContext).Returns(httpContext);
            return new WebUserContext(accessor.Object);
        }

        private static HttpContext CreateAuthenticatedHttpContext(string userId)
        {
            var identity = new ClaimsIdentity(
                [new Claim(ClaimTypes.NameIdentifier, userId)],
                authenticationType: "TestAuth");
            return new DefaultHttpContext { User = new ClaimsPrincipal(identity) };
        }

        private static HttpContext CreateUnauthenticatedHttpContext()
        {
            return new DefaultHttpContext { User = new ClaimsPrincipal(new ClaimsIdentity()) };
        }

        [TestMethod]
        public void WebUserContext_Authenticated_KindIsAuthenticated()
        {
            var userId = Guid.NewGuid().ToString();
            var context = CreateWebUserContext(CreateAuthenticatedHttpContext(userId));

            Assert.AreEqual(UserContextKind.Authenticated, context.Kind);
            Assert.AreEqual(userId, context.GetActorId());
        }

        [TestMethod]
        public void WebUserContext_UnauthenticatedNoVisitor_KindIsAnonymous()
        {
            var context = CreateWebUserContext(CreateUnauthenticatedHttpContext());

            Assert.AreEqual(UserContextKind.Anonymous, context.Kind);
            Assert.ThrowsExactly<InvalidOperationException>(() => context.GetActorId());
        }

        [TestMethod]
        public void WebUserContext_UnauthenticatedWithVisitor_KindIsAnonymousVisitor()
        {
            var visitorId = Guid.NewGuid().ToString();
            var httpContext = CreateUnauthenticatedHttpContext();
            httpContext.Items[WebUserContext.AnonymousVisitorItemKey] = visitorId;

            var context = CreateWebUserContext(httpContext);

            Assert.AreEqual(UserContextKind.AnonymousVisitor, context.Kind);
            Assert.AreEqual(visitorId, context.GetActorId());
        }

        [TestMethod]
        public void WebUserContext_UnauthenticatedWithEmptyVisitor_KindIsAnonymous()
        {
            var httpContext = CreateUnauthenticatedHttpContext();
            httpContext.Items[WebUserContext.AnonymousVisitorItemKey] = string.Empty;

            var context = CreateWebUserContext(httpContext);

            Assert.AreEqual(UserContextKind.Anonymous, context.Kind);
        }

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
