using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Jellyfin.Plugin.Kinopoisk.Api;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.Movies;
using MediaBrowser.Controller.Entities.TV;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Providers;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.Kinopoisk.MetadataProviders
{
    public class VideoImageProvider : BaseImageProvider
    {
        private readonly ILogger<VideoImageProvider> _logger;
        private readonly KinopoiskApiProxy _apiProxy;

        public override string Name => Utils.ProviderName;

        public VideoImageProvider(KinopoiskApiProxy kinopoiskApiProxy, ILogger<VideoImageProvider> logger, IHttpClientFactory httpClientFactory)
            : base(httpClientFactory)
        {
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this._apiProxy = kinopoiskApiProxy ?? throw new ArgumentNullException(nameof(kinopoiskApiProxy));
        }

        public override bool Supports(BaseItem item)
            => item is Movie || item is Series;

        public override IEnumerable<ImageType> GetSupportedImages(BaseItem item)
            => new ImageType[]
            {
                ImageType.Primary,
                ImageType.Backdrop
            };

        public override async Task<IEnumerable<RemoteImageInfo>> GetImages(BaseItem item, CancellationToken cancellationToken)
        {
            if (!Utils.TryGetKinopoiskId(item, _logger, out var kinopoiskId))
                return Enumerable.Empty<RemoteImageInfo>();

            var film = await _apiProxy.GetSingleFilmImages(kinopoiskId, cancellationToken);

            return await FilterEmptyImages(film.ToRemoteImageInfos());

        }

        private async Task<IEnumerable<RemoteImageInfo>> FilterEmptyImages(IEnumerable<RemoteImageInfo> images)
        {
            using var httpClient = new HttpClient(new HttpClientHandler() { AllowAutoRedirect = false }, true);
            var res = await Task.WhenAll(images.Select(i => NullIfEmptyImage(i, httpClient)));

            return res.Where(i => i != null);
        }

        private async Task<RemoteImageInfo> NullIfEmptyImage(RemoteImageInfo info, HttpClient httpClient)
        {
            var currentUrl = info.Url;
            while (!currentUrl.Contains("no-poster"))
            {
                var response = await httpClient.SendAsync(
                    new HttpRequestMessage(HttpMethod.Get, currentUrl), HttpCompletionOption.ResponseHeadersRead);

                if ((int)response.StatusCode <= 299)
                    return info;
                else if (response.Headers.Location != null)
                {
                    currentUrl = response.Headers.Location.ToString();
                    continue;
                }
                else
                    throw new InvalidOperationException("Not expected answer");
            }

            return null;
        }
    }
}
