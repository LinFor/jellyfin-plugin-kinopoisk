using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using MediaBrowser.Common.Net;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.Movies;
using MediaBrowser.Controller.Entities.TV;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Providers;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.Kinopoisk
{
    public class KinopoiskImageProvider : IRemoteImageProvider
    {
        private readonly ILogger<KinopoiskImageProvider> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly KinopoiskApiProxy _apiProxy;

        public string Name => Utils.ProviderName;

        public KinopoiskImageProvider(ILogger<KinopoiskImageProvider> logger, IHttpClientFactory httpClientFactory)
        {
            if (logger is null)
            {
                throw new System.ArgumentNullException(nameof(logger));
            }

            if (httpClientFactory is null)
            {
                throw new System.ArgumentNullException(nameof(httpClientFactory));
            }

            this._logger = logger;
            this._httpClientFactory = httpClientFactory;
            this._apiProxy = new KinopoiskApiProxy(logger, httpClientFactory);
        }

        public bool Supports(BaseItem item)
            => item is Movie || item is Series;

        public IEnumerable<ImageType> GetSupportedImages(BaseItem item)
            => new ImageType[]
            {
                ImageType.Primary,
                ImageType.Backdrop
            };

        public async Task<HttpResponseMessage> GetImageResponse(string url, CancellationToken cancellationToken)
        {
            return await _httpClientFactory.CreateClient(NamedClient.Default).GetAsync(url, cancellationToken);
        }

        public async Task<IEnumerable<RemoteImageInfo>> GetImages(BaseItem item, CancellationToken cancellationToken)
        {
            if (!Utils.TryGetKinopoiskId(item, _logger, out var kinopoiskId))
                return Enumerable.Empty<RemoteImageInfo>();

            var film = await _apiProxy.GetSingleFilmImages(kinopoiskId, cancellationToken);

            return film.ToRemoteImageInfos();
        }
    }
}
