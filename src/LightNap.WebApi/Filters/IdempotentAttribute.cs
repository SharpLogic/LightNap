using System.Text.Json;
using LightNap.Configuration.Idempotency;
using LightNap.Core.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace LightNap.WebApi.Filters
{
    /// <summary>
    /// Caches the first successful response keyed by the <c>Idempotency-Key</c> header and
    /// replays it for subsequent calls with the same key within the configured TTL. Use on
    /// mutating endpoints where double-click protection or retry safety matters.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class IdempotentAttribute : Attribute, IAsyncActionFilter
    {
        /// <summary>
        /// The name of the request header that carries the client-supplied idempotency key.
        /// </summary>
        public const string HeaderName = "Idempotency-Key";

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        };

        /// <inheritdoc />
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var key = context.HttpContext.Request.Headers[HeaderName].ToString();
            if (string.IsNullOrEmpty(key))
            {
                // No key — pass through without caching.
                await next();
                return;
            }

            var cache = context.HttpContext.RequestServices.GetRequiredService<HybridCache>();
            var options = context.HttpContext.RequestServices.GetRequiredService<IOptions<IdempotencySettings>>();
            var cacheKey = BuildCacheKey(context, key);

            // Check whether we already have a cached response for this key. If so, replay
            // it and skip invoking the action entirely.
            var (cacheHit, cached) = await cache.TryGetValueAsync<string>(cacheKey, context.HttpContext.RequestAborted);
            if (cacheHit && cached is not null)
            {
                context.Result = new ContentResult
                {
                    Content = cached,
                    ContentType = "application/json",
                    StatusCode = StatusCodes.Status200OK
                };
                return;
            }

            // Cache miss: invoke the action.
            var executed = await next();

            // Only cache a successful response. Errors, exceptions, and 4xx/5xx do not poison the cache.
            if (executed.Exception is null
                && executed.Result is ObjectResult or
                && (or.StatusCode is null || (or.StatusCode >= 200 && or.StatusCode < 300)))
            {
                var json = JsonSerializer.Serialize(or.Value, JsonOptions);
                await cache.GetOrCreateAsync<string>(
                    cacheKey,
                    _ => Task.FromResult(json),
                    new HybridCacheEntryOptions { Expiration = options.Value.Ttl });
            }
        }

        private static string BuildCacheKey(ActionExecutingContext context, string idempotencyKey)
        {
            var route = context.HttpContext.Request.Path.ToString();
            var method = context.HttpContext.Request.Method;
            return $"idem:{method}:{route}:{idempotencyKey}";
        }
    }
}
