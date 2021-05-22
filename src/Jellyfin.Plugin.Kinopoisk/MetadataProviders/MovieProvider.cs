using System.Net.Http;
using Jellyfin.Plugin.Kinopoisk.Api;
using Jellyfin.Plugin.Kinopoisk.Api.Model;
using MediaBrowser.Controller.Entities.Movies;
using MediaBrowser.Controller.Providers;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.Kinopoisk.MetadataProviders
{
    public class MovieProvider : VideoBaseProvider<Movie, MovieInfo>
    {
        public MovieProvider(KinopoiskApiProxy kinopoiskApiProxy, ILogger<MovieProvider> logger, IHttpClientFactory httpClientFactory) : base(kinopoiskApiProxy, logger, httpClientFactory)
        {
        }

        protected override Movie ConvertResponseToItem(FilmDetails apiResponse)
            => apiResponse.ToMovie();
    }
}
