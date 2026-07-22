using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;
using Microsoft.Extensions.Caching.Memory;

namespace KinopoiskUnofficialInfo.ApiClient.Tests
{
    public class ApiQuotaTests
    {
        [Fact]
        public async Task ShouldNotSendAnotherRequestAfterQuotaExceeded()
        {
            var handler = new QuotaExceededHandler();
            var httpClientFactory = new Mock<IHttpClientFactory>();

            httpClientFactory
                .Setup(factory => factory.CreateClient(It.IsAny<string>()))
                .Returns(new HttpClient(handler));

            using var cache = new MemoryCache(new MemoryCacheOptions());

            var innerClient = new KinopoiskApiClient(
                "test-token",
                NullLogger<KinopoiskApiClient>.Instance,
                httpClientFactory.Object);

            var client = new CachedKinopoiskApiClient(
                innerClient,
                cache,
                NullLogger<CachedKinopoiskApiClient>.Instance);

            var firstResult = await client.GetSingleFilm(1);
            var secondResult = await client.GetSingleFilm(2);

            Assert.Null(firstResult);
            Assert.Null(secondResult);
            Assert.Equal(1, handler.RequestCount);
        }

        private sealed class QuotaExceededHandler : HttpMessageHandler
        {
            private int _requestCount;

            public int RequestCount => _requestCount;

            protected override Task<HttpResponseMessage> SendAsync(
                HttpRequestMessage request,
                CancellationToken cancellationToken)
            {
                Interlocked.Increment(ref _requestCount);

                return Task.FromResult(new HttpResponseMessage(
                    (HttpStatusCode)402)
                {
                    Content = new StringContent(
                        "{\"message\":\"Daily request limit exceeded\"}")
                });
            }
        }
    }
}
