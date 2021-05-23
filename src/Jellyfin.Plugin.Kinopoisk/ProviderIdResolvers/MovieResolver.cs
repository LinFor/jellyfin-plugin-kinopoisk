using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using KinopoiskUnofficialInfo.ApiClient;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.Kinopoisk.ProviderIdResolvers
{
    public class VideoResolver<T> : CommonLookupInfoResolver<T>
        where T : ItemLookupInfo
    {
        private readonly IKinopoiskApiClient _kinopoiskApiClient;

        public VideoResolver(IKinopoiskApiClient kinopoiskApiClient, ILogger<VideoResolver<T>> logger) : base(logger)
        {
            _kinopoiskApiClient = kinopoiskApiClient ?? throw new ArgumentNullException(nameof(kinopoiskApiClient));
        }

        public override async Task<(bool IsSuccess, int ProviderId)> TryResolve(T info, CancellationToken? ct = null)
        {
            // Try to get from standart sources
            var possibleResult = await base.TryResolve(info, ct);
            if (possibleResult.IsSuccess)
                return possibleResult;

            // Trying to find empirically on kinopoisk
            if (string.IsNullOrWhiteSpace(info.Name))
            {
                _logger.LogDebug($"Film name is empty, skipping KinopoiskProviderId search");
                return (false, 0);
            }

            _logger.LogDebug($"Trying to get suitable film with name '{info.Name}'...");
            var searchResult = await _kinopoiskApiClient.SearchByKeyword(info.Name, 1, ct ?? CancellationToken.None);
            if (searchResult.SearchFilmsCountResult < 1 || searchResult?.Films.Count < 1)
            {
                _logger.LogDebug($"Received empty search result");
                return (false, 0);
            }
            var candidates = searchResult.Films.ToArray();
            _logger.LogDebug($"Received {candidates.Length} results, trying to filter and match...");

            // Check if there are single candidate filtered by year
            possibleResult = await TryResolveBySingleCandidateLeft(info, FilterByYear(info, candidates), ct);
            if (possibleResult.IsSuccess)
                return possibleResult;

            // Try to resolve by ImdbId match filtered by year
            possibleResult = await TryResolveByImdbMatch(info, FilterByYear(info, candidates), ct);
            if (possibleResult.IsSuccess)
                return possibleResult;

            // Try to resolve by ImdbId match without filtering
            possibleResult = await TryResolveByImdbMatch(info, candidates, ct);
            if (possibleResult.IsSuccess)
                return possibleResult;

            _logger.LogDebug($"Suitable result not found");
            return (false, 0);
        }

        public async Task<(bool IsSuccess, int ProviderId)> TryResolveByImdbMatch(T info, ICollection<FilmSearchResponse_films> candidates, CancellationToken? ct = null)
        {
            if (info.TryGetProviderId(MetadataProvider.Imdb, out var imdbId))
            {
                _logger.LogDebug($"Trying to find result with ImdbId '{imdbId}'...");
                var index = 0;
                foreach (var candidate in candidates)
                {
                    try
                    {
                        var film = await _kinopoiskApiClient.GetSingleFilm(candidate.FilmId, ct);

                        if (imdbId == film?.ExternalId?.ImdbId)
                        {
                            _logger.LogDebug($"Found match: {candidate.FilmId} '{film.GetLocalName()}', ImdbId '{film?.ExternalId?.ImdbId}', setting KinopoiskProviderId to {candidate.FilmId}");
                            return (true, candidate.FilmId);
                        }

                        _logger.LogDebug($"Film {candidate.FilmId} '{film.GetLocalName()}' has ImdbId '{film?.ExternalId?.ImdbId}', skipping, {candidates.Count - ++index} candidates left...");
                    } catch (Exception e)
                    {
                        _logger.LogError(e, $"Error while retrieving film {candidate.FilmId}");
                        continue;
                    }
                }
            }

            return (false, 0);
        }

        public Task<(bool IsSuccess, int ProviderId)> TryResolveBySingleCandidateLeft(T info, ICollection<FilmSearchResponse_films> candidates, CancellationToken? ct = null)
        {
            if (candidates.Count == 1)
            {
                var kinopoiskId = candidates.Single().FilmId;
                _logger.LogDebug($"There is single candidate left, setting KinopoiskProviderId to {kinopoiskId} ({info.Name})");
                return Task.FromResult((true, kinopoiskId));
            }

            return Task.FromResult((false, 0));
        }

        public ICollection<FilmSearchResponse_films> FilterByYear(T info, ICollection<FilmSearchResponse_films> candidates)
        {
            if (!info.Year.HasValue)
            {
                _logger.LogDebug($"Can't filter by year, no year set in metadata...");
                return Array.Empty<FilmSearchResponse_films>();
            }

            var targetYear = info.Year.Value.ToString();
            var res = candidates.Where(f => f.Year == targetYear).ToArray();
            _logger.LogDebug($"Filtered by year {targetYear}, {res.Length} results left...");
            return res;
        }
    }
}
