using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Jellyfin.Plugin.Kinopoisk.ApiModel;
using MediaBrowser.Common.Json;
using MediaBrowser.Common.Net;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.Kinopoisk
{
    public class KinopoiskApiProxy
    {
        private readonly ILogger _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _apiToken;
        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);

        public KinopoiskApiProxy(ILogger logger, IHttpClientFactory httpClientFactory)
        {
            this._logger = logger;
            this._httpClientFactory = httpClientFactory;
            this._apiToken = Plugin.Instance.Configuration.ApiToken;
            this._jsonOptions.Converters.Add(new JsonStringEnumMemberConverter());
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

            return await GetAsync<SearchResult>(uriBuilder.Uri, cancellationToken.Value);
        }

        public async Task<FilmDetails> GetSingleFilm(int filmId, CancellationToken? cancellationToken = null)
        {
            if (!cancellationToken.HasValue)
                cancellationToken = CancellationToken.None;

            Uri uri = new Uri($"https://kinopoiskapiunofficial.tech/api/v2.1/films/{filmId}?append_to_response=EXTERNAL_ID&append_to_response=RATING");

            return await GetAsync<FilmDetails>(uri, cancellationToken.Value);
        }

        public async Task<FilmDetails> GetSingleFilmImages(int filmId, CancellationToken? cancellationToken = null)
        {
            if (!cancellationToken.HasValue)
                cancellationToken = CancellationToken.None;

            Uri uri = new Uri($"https://kinopoiskapiunofficial.tech/api/v2.1/films/{filmId}?append_to_response=POSTERS");

            return await GetAsync<FilmDetails>(uri, cancellationToken.Value);
        }

        public async Task<IEnumerable<StaffItem>> GetStaff(int filmId, CancellationToken? cancellationToken = null)
        {
            if (!cancellationToken.HasValue)
                cancellationToken = CancellationToken.None;

            Uri uri = new Uri($"https://kinopoiskapiunofficial.tech/api/v1/staff?filmId={filmId}");

            return await GetAsync<IEnumerable<StaffItem>>(uri, cancellationToken.Value); 
        }

        private async Task<T> GetAsync<T>(Uri uri, CancellationToken cancellationToken) where T: class
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, uri);
            request.Headers.Add("Accept", "application/json");
            request.Headers.Add("X-API-KEY", _apiToken);

            var client = _httpClientFactory.CreateClient(NamedClient.Default);
            using var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                string content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                _logger.LogError($"Received non-success result status code {response.StatusCode} from Kinopoisk API, response content is:\n{content}");
                return null;
            }

            using var jsonStream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
            return await JsonSerializer.DeserializeAsync<T>(jsonStream, _jsonOptions, cancellationToken).ConfigureAwait(false);
        }
    }
}
