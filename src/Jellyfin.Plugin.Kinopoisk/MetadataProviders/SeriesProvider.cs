using System.Net.Http;
using Jellyfin.Plugin.Kinopoisk.Api;
using Jellyfin.Plugin.Kinopoisk.Api.Model;
using MediaBrowser.Controller.Entities.TV;
using MediaBrowser.Controller.Providers;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.Kinopoisk.MetadataProviders
{
    public class SeriesProvider : VideoBaseProvider<Series, SeriesInfo>
    {
        public SeriesProvider(KinopoiskApiProxy kinopoiskApiProxy, ILogger<SeriesProvider> logger, IHttpClientFactory httpClientFactory) : base(kinopoiskApiProxy, logger, httpClientFactory)
        {
        }

        protected override Series ConvertResponseToItem(FilmDetails apiResponse)
            => apiResponse.ToSeries();
    }
}
