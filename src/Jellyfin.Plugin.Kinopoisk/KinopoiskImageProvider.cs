using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediaBrowser.Common.Net;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.Movies;
using MediaBrowser.Controller.Entities.TV;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Providers;
using MediaBrowser.Model.Serialization;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.Kinopoisk
{
    public class KinopoiskImageProvider : IRemoteImageProvider
    {
        private readonly ILogger<KinopoiskImageProvider> _logger;
        private readonly IHttpClient _httpClient;
        private readonly IJsonSerializer _jsonSerializer;
        private readonly KinopoiskApiProxy _apiProxy;

        public string Name => Utils.ProviderName;

        public KinopoiskImageProvider(ILogger<KinopoiskImageProvider> logger,
                                      IHttpClient httpClient,
                                      IJsonSerializer jsonSerializer)
        {
            if (logger is null)
            {
                throw new System.ArgumentNullException(nameof(logger));
            }

            if (httpClient is null)
            {
                throw new System.ArgumentNullException(nameof(httpClient));
            }

            if (jsonSerializer is null)
            {
                throw new System.ArgumentNullException(nameof(jsonSerializer));
            }

            this._logger = logger;
            this._httpClient = httpClient;
            this._jsonSerializer = jsonSerializer;
            this._apiProxy = new KinopoiskApiProxy(logger, httpClient, jsonSerializer);
        }

        public bool Supports(BaseItem item)
            => item is Movie || item is Series;

        public IEnumerable<ImageType> GetSupportedImages(BaseItem item)
            => new ImageType[]
            {
                ImageType.Primary,
                ImageType.Backdrop
            };

        public Task<HttpResponseInfo> GetImageResponse(string url, CancellationToken cancellationToken)
        {
            return _httpClient.GetResponse(new HttpRequestOptions
            {
                CancellationToken = cancellationToken,
                Url = url
            });
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
