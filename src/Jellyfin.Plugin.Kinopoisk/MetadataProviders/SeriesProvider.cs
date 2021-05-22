using System.Net.Http;
using KinopoiskUnofficialInfo.ApiClient;
using MediaBrowser.Controller.Entities.TV;
using MediaBrowser.Controller.Providers;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.Kinopoisk.MetadataProviders
{
    public class SeriesProvider : VideoBaseProvider<Series, SeriesInfo>
    {
        public SeriesProvider(IKinopoiskApiClient kinopoiskApiClient, ILogger<SeriesProvider> logger, IHttpClientFactory httpClientFactory) : base(kinopoiskApiClient, logger, httpClientFactory)
        {
        }

        protected override Series ConvertResponseToItem(Film apiResponse)
            => apiResponse.ToSeries();
    }
}
