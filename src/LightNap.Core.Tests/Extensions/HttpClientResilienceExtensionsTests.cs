using System.Net;
using LightNap.Core.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;

namespace LightNap.Core.Tests.Extensions
{
    [TestClass]
    public class HttpClientResilienceExtensionsTests
    {
        private sealed class SampleClient(HttpClient httpClient)
        {
            public HttpClient HttpClient { get; } = httpClient;
        }

        private sealed class ScriptedHandler(Queue<HttpStatusCode> scriptedStatuses) : HttpMessageHandler
        {
            public int CallCount { get; private set; }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                this.CallCount++;
                var status = scriptedStatuses.Count > 0 ? scriptedStatuses.Dequeue() : HttpStatusCode.OK;
                return Task.FromResult(new HttpResponseMessage(status));
            }
        }

        [TestMethod]
        public void AddLightNapResilientHttpClient_Typed_RegistersClient()
        {
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddLightNapResilientHttpClient<SampleClient>();

            using var provider = services.BuildServiceProvider();
            var client = provider.GetRequiredService<SampleClient>();

            Assert.IsNotNull(client);
            Assert.IsNotNull(client.HttpClient);
        }

        [TestMethod]
        public void AddLightNapResilientHttpClient_Named_RegistersClient()
        {
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddLightNapResilientHttpClient("test-named");

            using var provider = services.BuildServiceProvider();
            var factory = provider.GetRequiredService<IHttpClientFactory>();
            using var client = factory.CreateClient("test-named");

            Assert.IsNotNull(client);
        }

        [TestMethod]
        public async Task AddLightNapResilientHttpClient_Named_RetriesTransientFailures()
        {
            // The standard resilience handler retries 5xx responses on idempotent requests
            // (GET). Script three failures followed by success — the retry policy should
            // surface the eventual success.
            var scripted = new Queue<HttpStatusCode>(
            [
                HttpStatusCode.ServiceUnavailable,
                HttpStatusCode.ServiceUnavailable,
                HttpStatusCode.OK
            ]);
            var handler = new ScriptedHandler(scripted);

            var services = new ServiceCollection();
            services.AddLogging();
            services.AddLightNapResilientHttpClient("retry-client")
                .ConfigurePrimaryHttpMessageHandler(() => handler);

            using var provider = services.BuildServiceProvider();
            var factory = provider.GetRequiredService<IHttpClientFactory>();
            using var client = factory.CreateClient("retry-client");

            using var response = await client.GetAsync("https://example.invalid/resource");

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.IsTrue(handler.CallCount >= 3, $"Expected the handler to be retried; observed {handler.CallCount} call(s).");
        }
    }
}
