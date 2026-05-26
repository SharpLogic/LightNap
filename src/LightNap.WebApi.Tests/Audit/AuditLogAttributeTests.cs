using LightNap.Core.Audit.Interfaces;
using LightNap.Core.Audit.Services;
using LightNap.Core.Data;
using LightNap.Core.Extensions;
using LightNap.Core.Interfaces;
using LightNap.WebApi.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace LightNap.WebApi.Tests.Audit
{
    [TestClass]
    public class AuditLogAttributeTests
    {
        private static IUserContext CreateUserContext(string userId)
        {
            var mock = new Mock<IUserContext>();
            mock.Setup(c => c.GetUserId()).Returns(userId);
            mock.Setup(c => c.GetActorId()).Returns(userId);
            mock.Setup(c => c.Kind).Returns(UserContextKind.Authenticated);
            mock.Setup(c => c.IsAuthenticated).Returns(true);
            return mock.Object;
        }

        private static (ActionExecutingContext context, ApplicationDbContext db) BuildContext(
            IUserContext userContext,
            Dictionary<string, object?>? arguments = null)
        {
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddLightNapInMemoryDatabase($"AuditDb_{Guid.NewGuid()}");
            services.AddSingleton(userContext);
            services.AddScoped<IAuditLogger, AuditLogger>();

            var provider = services.BuildServiceProvider();
            var scope = provider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var httpContext = new DefaultHttpContext { RequestServices = scope.ServiceProvider };
            var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
            var ctx = new ActionExecutingContext(actionContext, [], arguments ?? [], controller: new object());
            return (ctx, db);
        }

        [TestMethod]
        public async Task OnSuccess_WritesAuditEntry_WithActionArguments()
        {
            var (context, db) = BuildContext(CreateUserContext("admin-1"), new Dictionary<string, object?> { ["id"] = 42 });

            var filter = new AuditLogAttribute("test.action");
            await filter.OnActionExecutionAsync(context, () =>
            {
                var executed = new ActionExecutedContext(context, [], controller: new object())
                {
                    Result = new OkObjectResult(new { ok = true })
                };
                return Task.FromResult(executed);
            });

            var row = await db.AdminAuditLogs.SingleAsync();
            Assert.AreEqual("test.action", row.Action);
            Assert.AreEqual("admin-1", row.ActorId);
            StringAssert.Contains(row.AfterJson ?? "", "\"id\":42");
        }

        [TestMethod]
        public async Task OnBadRequest_DoesNotWriteEntry()
        {
            var (context, db) = BuildContext(CreateUserContext("admin-1"));

            var filter = new AuditLogAttribute("test.action");
            await filter.OnActionExecutionAsync(context, () =>
            {
                var executed = new ActionExecutedContext(context, [], controller: new object())
                {
                    Result = new BadRequestObjectResult("nope")
                };
                return Task.FromResult(executed);
            });

            Assert.AreEqual(0, await db.AdminAuditLogs.CountAsync());
        }

        [TestMethod]
        public async Task OnException_DoesNotWriteEntry()
        {
            var (context, db) = BuildContext(CreateUserContext("admin-1"));

            var filter = new AuditLogAttribute("test.action");
            await filter.OnActionExecutionAsync(context, () =>
            {
                var executed = new ActionExecutedContext(context, [], controller: new object())
                {
                    Exception = new InvalidOperationException("boom")
                };
                return Task.FromResult(executed);
            });

            Assert.AreEqual(0, await db.AdminAuditLogs.CountAsync());
        }
    }
}
