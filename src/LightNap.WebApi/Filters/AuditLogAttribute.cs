using LightNap.Core.Audit.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace LightNap.WebApi.Filters
{
    /// <summary>
    /// Records an audit log entry on successful execution of the decorated action. Captures the
    /// actor (from <c>IUserContext</c>), the action name, and the action arguments as the "after"
    /// snapshot. For richer before/after capture, inject <see cref="IAuditLogger"/> directly in
    /// the action and call <see cref="IAuditLogger.WriteAsync"/> explicitly.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class AuditLogAttribute(string action) : Attribute, IAsyncActionFilter
    {
        /// <summary>
        /// The action name recorded on the audit row.
        /// </summary>
        public string Action { get; } = action;

        /// <inheritdoc />
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var executed = await next();

            if (executed.Exception is null && executed.Result is not BadRequestObjectResult)
            {
                var logger = context.HttpContext.RequestServices.GetRequiredService<IAuditLogger>();
                object? args = context.ActionArguments.Count > 0 ? context.ActionArguments : null;
                await logger.WriteAsync(this.Action, after: args, cancellationToken: context.HttpContext.RequestAborted);
            }
        }
    }
}
