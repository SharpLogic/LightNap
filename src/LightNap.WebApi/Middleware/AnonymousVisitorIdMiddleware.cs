using LightNap.Configuration.AnonymousVisitor;
using LightNap.WebApi.Services;
using Microsoft.Extensions.Options;

namespace LightNap.WebApi.Middleware
{
    /// <summary>
    /// Reads or mints a per-browser anonymous visitor identifier and exposes it on the request
    /// via <c>HttpContext.Items["AnonymousVisitorId"]</c>. Consumed by <see cref="WebUserContext"/>
    /// to resolve <c>UserContextKind.AnonymousVisitor</c> and by the rate-limit partitioner to
    /// prefer the visitor identifier over the remote IP.
    /// </summary>
    public sealed class AnonymousVisitorIdMiddleware(RequestDelegate next, IOptions<AnonymousVisitorSettings> options)
    {
        /// <summary>
        /// The <see cref="HttpContext.Items"/> key used to expose the visitor identifier.
        /// Matches <see cref="WebUserContext.AnonymousVisitorItemKey"/>.
        /// </summary>
        public const string ItemKey = WebUserContext.AnonymousVisitorItemKey;

        /// <summary>
        /// Handles the request: reads the existing visitor cookie if present, mints a new one
        /// otherwise, and stores the identifier on <see cref="HttpContext.Items"/>.
        /// </summary>
        public async Task InvokeAsync(HttpContext context)
        {
            var settings = options.Value;
            var cookieName = settings.CookieName;

            string visitorId;
            if (context.Request.Cookies.TryGetValue(cookieName, out var existing)
                && Guid.TryParse(existing, out var parsed))
            {
                visitorId = parsed.ToString();
            }
            else
            {
                visitorId = Guid.NewGuid().ToString();
                context.Response.Cookies.Append(cookieName, visitorId, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = settings.SecureOnly,
                    SameSite = SameSiteMode.Lax,
                    Expires = DateTimeOffset.UtcNow.Add(settings.Lifetime),
                    Path = "/"
                });
            }

            context.Items[ItemKey] = visitorId;
            await next(context);
        }
    }
}
