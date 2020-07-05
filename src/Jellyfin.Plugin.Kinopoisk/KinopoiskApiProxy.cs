using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Jellyfin.Plugin.Kinopoisk.ApiModel;
using MediaBrowser.Common.Net;
using MediaBrowser.Model.Serialization;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.Kinopoisk
{
    public class KinopoiskApiProxy
    {
        private readonly ILogger _logger;
        private readonly IHttpClient _httpClient;
        private readonly IJsonSerializer _jsonSerializer;
        private readonly string _apiToken;

        public KinopoiskApiProxy(ILogger logger, IHttpClient httpClient, IJsonSerializer jsonSerializer)
        {
            this._logger = logger;
            this._httpClient = httpClient;
            this._jsonSerializer = jsonSerializer;
            this._apiToken = Plugin.Instance.Configuration.ApiToken;
        }

        public async Task<SearchResult> SearchByKeyword(string keyword, int page = 1, CancellationToken? cancellationToken = null)
        {
            if (!cancellationToken.HasValue)
                cancellationToken = CancellationToken.None;
            var uriBuilder = new UriBuilder("https://kinopoiskapiunofficial.tech/api/v2.1/films/search-by-keyword");

            var @params = HttpUtility.ParseQueryString(string.Empty);
            @params["keyword"] = keyword;
            @params["page"] = Convert.ToString(page);
            uriBuilder.Query = @params.ToString();

            var requestOptions = new HttpRequestOptions
            {
                Url = uriBuilder.Uri.AbsoluteUri,
                CancellationToken = cancellationToken.Value,
                BufferContent = true,
                EnableDefaultUserAgent = true,
                AcceptHeader = "application/json"
            };
            requestOptions.RequestHeaders.Add("X-API-KEY", _apiToken);
            using(var response = await _httpClient.SendAsync(requestOptions, HttpMethod.Get).ConfigureAwait(false))
            // todo: status code!
            using(var jsonStream = response.Content)
            {
                return await _jsonSerializer.DeserializeFromStreamAsync<SearchResult>(jsonStream).ConfigureAwait(false);
            }
        }

        public async Task<FilmDetails> GetSingleFilm(int filmId, CancellationToken? cancellationToken = null)
        {
            if (!cancellationToken.HasValue)
                cancellationToken = CancellationToken.None;

            var uri = new Uri($"https://kinopoiskapiunofficial.tech/api/v2.1/films/{filmId}?append_to_response=EXTERNAL_ID&append_to_response=RATING");

            var requestOptions = new HttpRequestOptions
            {
                Url = uri.AbsoluteUri,
                CancellationToken = cancellationToken.Value,
                BufferContent = true,
                EnableDefaultUserAgent = true,
                AcceptHeader = "application/json"
            };
            requestOptions.RequestHeaders.Add("X-API-KEY", _apiToken);
            using(var response = await _httpClient.SendAsync(requestOptions, HttpMethod.Get).ConfigureAwait(false))
            // todo: status code!
            using(var jsonStream = response.Content)
            {
                return await _jsonSerializer.DeserializeFromStreamAsync<FilmDetails>(jsonStream).ConfigureAwait(false);
            }
        }

        public async Task<FilmDetails> GetSingleFilmImages(int filmId, CancellationToken? cancellationToken = null)
        {
            if (!cancellationToken.HasValue)
                cancellationToken = CancellationToken.None;

            var uri = new Uri($"https://kinopoiskapiunofficial.tech/api/v2.1/films/{filmId}?append_to_response=POSTERS");

            var requestOptions = new HttpRequestOptions
            {
                Url = uri.AbsoluteUri,
                CancellationToken = cancellationToken.Value,
                BufferContent = true,
                EnableDefaultUserAgent = true,
                AcceptHeader = "application/json"
            };
            requestOptions.RequestHeaders.Add("X-API-KEY", _apiToken);
            using(var response = await _httpClient.SendAsync(requestOptions, HttpMethod.Get).ConfigureAwait(false))
            // todo: status code!
            using(var jsonStream = response.Content)
            {
                return await _jsonSerializer.DeserializeFromStreamAsync<FilmDetails>(jsonStream).ConfigureAwait(false);
            }
        }
    }
}
