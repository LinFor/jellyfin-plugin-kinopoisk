using System.Net.Http;
using KinopoiskUnofficialInfo.ApiClient;
using MediaBrowser.Controller.Entities.Movies;
using MediaBrowser.Controller.Providers;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.Kinopoisk.MetadataProviders
{
    public class MovieProvider : VideoBaseProvider<Movie, MovieInfo>
    {
        public MovieProvider(IKinopoiskApiClient kinopoiskApiClient, ILogger<MovieProvider> logger, IHttpClientFactory httpClientFactory) : base(kinopoiskApiClient, logger, httpClientFactory)
        {
        }

        protected override Movie ConvertResponseToItem(Film apiResponse)
            => apiResponse.ToMovie();
    }
}
