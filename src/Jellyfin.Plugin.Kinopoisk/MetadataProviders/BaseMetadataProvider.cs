using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using MediaBrowser.Common.Net;
using MediaBrowser.Controller.Providers;

namespace Jellyfin.Plugin.Kinopoisk.MetadataProviders
{
    public abstract class BaseMetadataProvider : IMetadataProvider
    {
        protected IHttpClientFactory _httpClientFactory;

        protected BaseMetadataProvider(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory ?? throw new System.ArgumentNullException(nameof(httpClientFactory));
        }

        public string Name => Constants.ProviderName;

        public async Task<HttpResponseMessage> GetImageResponse(string url, CancellationToken cancellationToken)
        {
            return await _httpClientFactory.CreateClient(NamedClient.Default).GetAsync(url, cancellationToken);
        }
    }
}
