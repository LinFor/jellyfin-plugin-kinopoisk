using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Jellyfin.Plugin.Kinopoisk.Tests
{
    public class RemoteImageUrlSanitizerTests
    {
        [Fact]
        public async Task SanitizeRemoteImageUrlDisposesRequestAndResponse()
        {
            using var handler = new TrackingHandler();
            using var httpClient = new HttpClient(handler);

            var sanitizer = new RemoteImageUrlSanitizer(httpClient);

            var result = await sanitizer.SanitizeRemoteImageUrl(
                "https://example.com/image.jpg");

            Assert.Equal("https://example.com/image.jpg", result);
            Assert.True(handler.RequestContent.IsDisposed);
            Assert.True(handler.ResponseContent.IsDisposed);
        }

        private sealed class TrackingHandler : HttpMessageHandler
        {
            public TrackingContent RequestContent { get; } = new TrackingContent();

            public TrackingContent ResponseContent { get; } = new TrackingContent();

            protected override Task<HttpResponseMessage> SendAsync(
                HttpRequestMessage request,
                CancellationToken cancellationToken)
            {
                request.Content = RequestContent;

                return Task.FromResult(
                    new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = ResponseContent
                    });
            }
        }

        private sealed class TrackingContent : HttpContent
        {
            public bool IsDisposed { get; private set; }

            protected override Task SerializeToStreamAsync(
                Stream stream,
                TransportContext context)
            {
                return Task.CompletedTask;
            }

            protected override bool TryComputeLength(out long length)
            {
                length = 0;
                return true;
            }

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    IsDisposed = true;
                }

                base.Dispose(disposing);
            }
        }
    }
}
