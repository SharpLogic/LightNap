using LightNap.Configuration.Idempotency;
using LightNap.WebApi.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace LightNap.WebApi.Tests.Filters
{
    [TestClass]
    public class IdempotentAttributeTests
    {
        private static ServiceProvider BuildProvider(TimeSpan? ttl = null)
        {
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddHybridCache();
            services.Configure<IdempotencySettings>(o =>
            {
                o.Ttl = ttl ?? TimeSpan.FromHours(1);
            });
            return services.BuildServiceProvider();
        }

        private static ActionExecutingContext BuildContext(
            IServiceProvider provider,
            string method,
            string path,
            string? idempotencyKey)
        {
            var httpContext = new DefaultHttpContext
            {
                RequestServices = provider
            };
            httpContext.Request.Method = method;
            httpContext.Request.Path = path;
            if (idempotencyKey is not null)
            {
                httpContext.Request.Headers[IdempotentAttribute.HeaderName] = idempotencyKey;
            }
            var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
            return new ActionExecutingContext(actionContext, [], new Dictionary<string, object?>(), controller: new object());
        }

        [TestMethod]
        public async Task SameKey_SameRoute_SecondCallReplaysCachedResponse()
        {
            using var provider = BuildProvider();
            var filter = new IdempotentAttribute();

            int callCount = 0;
            Task<ActionExecutedContext> RunOnce(ActionExecutingContext ctx)
            {
                callCount++;
                var executed = new ActionExecutedContext(ctx, [], controller: new object())
                {
                    Result = new OkObjectResult(new { value = callCount })
                };
                ctx.Result = executed.Result;
                return Task.FromResult(executed);
            }

            var firstCtx = BuildContext(provider, "POST", "/api/widgets", "key-1");
            await filter.OnActionExecutionAsync(firstCtx, () => RunOnce(firstCtx));

            var secondCtx = BuildContext(provider, "POST", "/api/widgets", "key-1");
            await filter.OnActionExecutionAsync(secondCtx, () => RunOnce(secondCtx));

            Assert.AreEqual(1, callCount, "Second call should not re-invoke the action.");
            Assert.IsInstanceOfType(secondCtx.Result, typeof(ContentResult));
            var contentResult = (ContentResult)secondCtx.Result!;
            StringAssert.Contains(contentResult.Content ?? "", "\"value\":1");
        }

        [TestMethod]
        public async Task NoHeader_AlwaysInvokesAction()
        {
            using var provider = BuildProvider();
            var filter = new IdempotentAttribute();

            int callCount = 0;
            Task<ActionExecutedContext> RunOnce(ActionExecutingContext ctx)
            {
                callCount++;
                var executed = new ActionExecutedContext(ctx, [], controller: new object())
                {
                    Result = new OkObjectResult(new { value = callCount })
                };
                ctx.Result = executed.Result;
                return Task.FromResult(executed);
            }

            var first = BuildContext(provider, "POST", "/api/widgets", idempotencyKey: null);
            await filter.OnActionExecutionAsync(first, () => RunOnce(first));

            var second = BuildContext(provider, "POST", "/api/widgets", idempotencyKey: null);
            await filter.OnActionExecutionAsync(second, () => RunOnce(second));

            Assert.AreEqual(2, callCount);
        }

        [TestMethod]
        public async Task SameKey_DifferentRoute_BothInvokeAction()
        {
            using var provider = BuildProvider();
            var filter = new IdempotentAttribute();

            int callCount = 0;
            Task<ActionExecutedContext> RunOnce(ActionExecutingContext ctx)
            {
                callCount++;
                var executed = new ActionExecutedContext(ctx, [], controller: new object())
                {
                    Result = new OkObjectResult(new { value = callCount })
                };
                ctx.Result = executed.Result;
                return Task.FromResult(executed);
            }

            var first = BuildContext(provider, "POST", "/api/widgets", "shared-key");
            await filter.OnActionExecutionAsync(first, () => RunOnce(first));

            var second = BuildContext(provider, "POST", "/api/gadgets", "shared-key");
            await filter.OnActionExecutionAsync(second, () => RunOnce(second));

            Assert.AreEqual(2, callCount);
        }

        [TestMethod]
        public async Task BadRequestResponse_IsNotCached()
        {
            using var provider = BuildProvider();
            var filter = new IdempotentAttribute();

            int callCount = 0;
            Task<ActionExecutedContext> RunOnce(ActionExecutingContext ctx, bool firstFails)
            {
                callCount++;
                ActionResult result = firstFails && callCount == 1
                    ? new BadRequestObjectResult(new { error = "validation" })
                    : new OkObjectResult(new { value = callCount });
                ctx.Result = result;
                var executed = new ActionExecutedContext(ctx, [], controller: new object())
                {
                    Result = result
                };
                return Task.FromResult(executed);
            }

            var first = BuildContext(provider, "POST", "/api/widgets", "retry-key");
            await filter.OnActionExecutionAsync(first, () => RunOnce(first, firstFails: true));

            var second = BuildContext(provider, "POST", "/api/widgets", "retry-key");
            await filter.OnActionExecutionAsync(second, () => RunOnce(second, firstFails: true));

            Assert.AreEqual(2, callCount, "Failed responses should not be cached; the retry must re-execute the action.");
        }
    }
}
