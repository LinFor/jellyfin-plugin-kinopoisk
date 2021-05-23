using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Jellyfin.Plugin.Kinopoisk.ProviderIdResolvers;
using KinopoiskUnofficialInfo.ApiClient;
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
        private readonly IKinopoiskApiClient _apiClient;
        private readonly IProviderIdResolver<BaseItem> _providerIdResolver;

        public override string Name => Constants.ProviderName;

        public VideoImageProvider(IKinopoiskApiClient kinopoiskApiClient, IProviderIdResolver<BaseItem> providerIdResolver, ILogger<VideoImageProvider> logger, IHttpClientFactory httpClientFactory)
            : base(httpClientFactory)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _apiClient = kinopoiskApiClient ?? throw new ArgumentNullException(nameof(kinopoiskApiClient));
            _providerIdResolver = providerIdResolver ?? throw new ArgumentNullException(nameof(providerIdResolver));
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
            var (resolveResult, kinopoiskId) = await _providerIdResolver.TryResolve(item, cancellationToken);
            if (!resolveResult)
                return Enumerable.Empty<RemoteImageInfo>();

            var film = await _apiClient.GetSingleFilm(kinopoiskId, cancellationToken);

            return await FilterEmptyImages(film.ToRemoteImageInfos());
        }
    }
}
