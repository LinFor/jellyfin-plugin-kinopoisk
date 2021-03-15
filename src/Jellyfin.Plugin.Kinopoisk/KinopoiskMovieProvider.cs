using System.Net.Http;
using Jellyfin.Plugin.Kinopoisk.ApiModel;
using MediaBrowser.Controller.Entities.Movies;
using MediaBrowser.Controller.Providers;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.Kinopoisk
{
    public class KinopoiskMovieProvider : KinopoiskVideoBaseProvider<Movie, MovieInfo>
    {
        public KinopoiskMovieProvider(ILogger<KinopoiskMovieProvider> logger, IHttpClientFactory httpClientFactory) : base(logger, httpClientFactory)
        {
        }

        protected override Movie ConvertResponseToItem(FilmDetails apiResponse)
            => apiResponse.ToMovie();
    }
}
