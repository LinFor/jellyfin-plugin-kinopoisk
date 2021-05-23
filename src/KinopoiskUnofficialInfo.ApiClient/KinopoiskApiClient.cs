using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace KinopoiskUnofficialInfo.ApiClient
{
    public class KinopoiskApiClient : IKinopoiskApiClient
    {
        private readonly string _apiToken;
        private readonly ILogger<KinopoiskApiClient> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly Client _apiClient;

        public KinopoiskApiClient(string apiToken, ILogger<KinopoiskApiClient> logger, IHttpClientFactory httpClientFactory)
        {
            if (string.IsNullOrEmpty(apiToken))
            {
                throw new System.ArgumentException($"'{nameof(apiToken)}' cannot be null or empty.", nameof(apiToken));
            }

            _apiToken = apiToken;
            _logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
            _httpClientFactory = httpClientFactory ?? throw new System.ArgumentNullException(nameof(httpClientFactory));

            var httpClient = _httpClientFactory.CreateClient();
            httpClient.DefaultRequestHeaders.Add("X-API-KEY", _apiToken);
            _apiClient = new Client(httpClient);
        }

        private async Task<T> Invoke<T>(Func<CancellationToken, Task<T>> method, CancellationToken? ct, [CallerMemberName] string memberName = "")
        {
            try
            {
                _logger.LogDebug($"{memberName} request starting...");
                var res = await method.Invoke(ct ?? CancellationToken.None);
                _logger.LogDebug($"{memberName} request complete successfully");
                return res;
            }
            catch (ApiException e)
            {
                _logger.LogError($"Received non-success result status code {e.StatusCode} from Kinopoisk API, response content is:\n{e.Response}");
                throw;
            }
        }

        public Task<Film> GetSingleFilm(int filmId, CancellationToken? cancellationToken = null)
            => Invoke((ct) => _apiClient.FilmsAsync(filmId, new[] { Anonymous.BUDGET, Anonymous.REVIEW, Anonymous.RATING, Anonymous.POSTERS }, ct), cancellationToken);

        public Task<ICollection<StaffResponse>> GetStaff(int filmId, CancellationToken? cancellationToken = null)
            => Invoke((ct) => _apiClient.StaffAllAsync(filmId, ct), cancellationToken);

        public Task<FilmSearchResponse> SearchByKeyword(string keyword, int page = 1, CancellationToken? cancellationToken = null)
            => Invoke((ct) => _apiClient.SearchByKeywordAsync(keyword, null, ct), cancellationToken);

        public Task<PersonResponse> GetPerson(int personId, CancellationToken? cancellationToken = null)
            => Invoke((ct) => _apiClient.StaffAsync(personId, ct), cancellationToken);

        public Task<VideoResponse> GetTrailers(int filmId, CancellationToken? cancellationToken = null)
        {
            return Invoke(async (ct) => {
                try {
                    return await _apiClient.VideosAsync(filmId, ct);
                } catch (ApiException e)
                {
                    if (e.StatusCode == 404)
                        return new VideoResponse();
                    throw;
                }
            }, cancellationToken);
        }
    }
}
