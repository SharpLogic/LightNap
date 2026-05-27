using System.Net;
using System.Text;

namespace LightNap.Core.Tests.Captcha
{
    internal sealed class TestHttpMessageHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, HttpResponseMessage> respond;

        public TestHttpMessageHandler(HttpStatusCode status, string body)
            : this(_ => new HttpResponseMessage(status)
            {
                Content = new StringContent(body, Encoding.UTF8, "application/json")
            })
        { }

        public TestHttpMessageHandler(Exception toThrow)
            : this(_ => throw toThrow)
        { }

        public TestHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> respond)
        {
            this.respond = respond;
        }

        public int CallCount { get; private set; }

        public HttpRequestMessage? LastRequest { get; private set; }

        public string? LastRequestBody { get; private set; }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            this.CallCount++;
            this.LastRequest = request;
            if (request.Content is not null)
            {
                this.LastRequestBody = await request.Content.ReadAsStringAsync(cancellationToken);
            }
            return this.respond(request);
        }
    }
}
