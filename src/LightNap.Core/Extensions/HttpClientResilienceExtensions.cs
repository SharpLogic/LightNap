using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http.Resilience;

namespace LightNap.Core.Extensions
{
    /// <summary>
    /// Extension methods that register <see cref="HttpClient"/> instances with LightNap's
    /// standard resilience handler (retry with exponential backoff and jitter, total and
    /// per-attempt timeouts, circuit breaker, rate limiter).
    /// </summary>
    public static class HttpClientResilienceExtensions
    {
        extension(IServiceCollection services)
        {
            /// <summary>
            /// Registers a typed <see cref="HttpClient"/> with LightNap's standard resilience
            /// handler. Use for outbound calls to third-party services.
            /// </summary>
            /// <typeparam name="TClient">The client interface or abstract type.</typeparam>
            /// <typeparam name="TImpl">The concrete implementation.</typeparam>
            /// <param name="name">An optional logical name for the typed client.</param>
            /// <returns>The <see cref="IHttpClientBuilder"/> for further configuration.</returns>
            public IHttpClientBuilder AddLightNapResilientHttpClient<TClient, TImpl>(string? name = null)
                where TClient : class
                where TImpl : class, TClient
            {
                var builder = name is null
                    ? services.AddHttpClient<TClient, TImpl>()
                    : services.AddHttpClient<TClient, TImpl>(name);
                builder.AddStandardResilienceHandler();
                return builder;
            }

            /// <summary>
            /// Registers a typed <see cref="HttpClient"/> bound to a single client type with
            /// LightNap's standard resilience handler.
            /// </summary>
            /// <typeparam name="TClient">The client type to register.</typeparam>
            /// <returns>The <see cref="IHttpClientBuilder"/> for further configuration.</returns>
            public IHttpClientBuilder AddLightNapResilientHttpClient<TClient>()
                where TClient : class
            {
                var builder = services.AddHttpClient<TClient>();
                builder.AddStandardResilienceHandler();
                return builder;
            }

            /// <summary>
            /// Registers a named <see cref="HttpClient"/> with LightNap's standard resilience handler.
            /// </summary>
            /// <param name="name">The logical name of the client.</param>
            /// <returns>The <see cref="IHttpClientBuilder"/> for further configuration.</returns>
            public IHttpClientBuilder AddLightNapResilientHttpClient(string name)
            {
                var builder = services.AddHttpClient(name);
                builder.AddStandardResilienceHandler();
                return builder;
            }
        }
    }
}
