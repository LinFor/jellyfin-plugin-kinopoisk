using System.Net.Http;
using Jellyfin.Plugin.Kinopoisk.ApiModel;
using MediaBrowser.Controller.Entities.TV;
using MediaBrowser.Controller.Providers;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.Kinopoisk
{
    public class KinopoiskSeriesProvider : KinopoiskVideoBaseProvider<Series, SeriesInfo>
    {
        public KinopoiskSeriesProvider(ILogger<KinopoiskSeriesProvider> logger, IHttpClientFactory httpClientFactory) : base(logger, httpClientFactory)
        {
        }

        protected override Series ConvertResponseToItem(FilmDetails apiResponse)
            => apiResponse.ToSeries();
    }
}
