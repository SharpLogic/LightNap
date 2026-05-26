using LightNap.Configuration.AnonymousVisitor;
using LightNap.WebApi.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace LightNap.Core.Tests.Middleware
{
    [TestClass]
    public class AnonymousVisitorIdMiddlewareTests
    {
        private static AnonymousVisitorIdMiddleware CreateMiddleware(
            AnonymousVisitorSettings? settings = null,
            RequestDelegate? next = null)
        {
            return new AnonymousVisitorIdMiddleware(
                next ?? (_ => Task.CompletedTask),
                Options.Create(settings ?? new AnonymousVisitorSettings()));
        }

        [TestMethod]
        public async Task NoCookie_MintsNewIdentifier_AndSetsCookie()
        {
            var settings = new AnonymousVisitorSettings();
            var middleware = CreateMiddleware(settings);
            var context = new DefaultHttpContext();

            await middleware.InvokeAsync(context);

            Assert.IsNotNull(context.Items[AnonymousVisitorIdMiddleware.ItemKey]);
            var minted = (string)context.Items[AnonymousVisitorIdMiddleware.ItemKey]!;
            Assert.IsTrue(Guid.TryParse(minted, out _), $"Expected a GUID, got: {minted}");

            var setCookieHeaders = context.Response.Headers.SetCookie.ToString();
            StringAssert.Contains(setCookieHeaders, settings.CookieName);
            StringAssert.Contains(setCookieHeaders, minted);
        }

        [TestMethod]
        public async Task ExistingValidCookie_UsesCookieValue_NoSetCookieResponse()
        {
            var settings = new AnonymousVisitorSettings();
            var middleware = CreateMiddleware(settings);
            var existingId = Guid.NewGuid().ToString();

            var context = new DefaultHttpContext();
            context.Request.Headers.Cookie = $"{settings.CookieName}={existingId}";

            await middleware.InvokeAsync(context);

            Assert.AreEqual(existingId, context.Items[AnonymousVisitorIdMiddleware.ItemKey]);
            Assert.IsTrue(string.IsNullOrEmpty(context.Response.Headers.SetCookie.ToString()),
                "Expected no Set-Cookie response header when the request already carried a valid cookie.");
        }

        [TestMethod]
        public async Task MalformedCookie_MintsNewIdentifier()
        {
            var settings = new AnonymousVisitorSettings();
            var middleware = CreateMiddleware(settings);

            var context = new DefaultHttpContext();
            context.Request.Headers.Cookie = $"{settings.CookieName}=not-a-guid";

            await middleware.InvokeAsync(context);

            var minted = (string)context.Items[AnonymousVisitorIdMiddleware.ItemKey]!;
            Assert.AreNotEqual("not-a-guid", minted);
            Assert.IsTrue(Guid.TryParse(minted, out _));
        }

        [TestMethod]
        public async Task CallsNextDelegate()
        {
            bool nextCalled = false;
            var middleware = CreateMiddleware(next: _ =>
            {
                nextCalled = true;
                return Task.CompletedTask;
            });

            await middleware.InvokeAsync(new DefaultHttpContext());

            Assert.IsTrue(nextCalled);
        }
    }
}
