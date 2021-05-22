using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using KinopoiskUnofficialInfo.ApiClient;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Providers;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.Kinopoisk.MetadataProviders
{
    public abstract class VideoBaseProvider<TItemType, TLookupInfoType> : BaseMetadataProvider, IRemoteMetadataProvider<TItemType, TLookupInfoType>
        where TItemType : BaseItem, IHasLookupInfo<TLookupInfoType>
        where TLookupInfoType : ItemLookupInfo, new()
    {
        private readonly ILogger _logger;
        private readonly IKinopoiskApiClient _apiClient;

        public VideoBaseProvider(IKinopoiskApiClient kinopoiskApiClient, ILogger logger, IHttpClientFactory httpClientFactory)
            : base(httpClientFactory)
        {
            this._logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
            this._apiClient = kinopoiskApiClient ?? throw new System.ArgumentNullException(nameof(kinopoiskApiClient));
        }

        protected abstract TItemType ConvertResponseToItem(Film apiResponse);

        public async Task<MetadataResult<TItemType>> GetMetadata(TLookupInfoType info, CancellationToken cancellationToken)
        {
            var result = new MetadataResult<TItemType>()
            {
                QueriedById = true,
                Provider = Utils.ProviderName,
                ResultLanguage = Utils.ProviderMetadataLanguage
            };

            if (!Utils.TryGetKinopoiskId(info, _logger, out var kinopoiskId))
                return result;

            var film = await _apiClient.GetSingleFilm(kinopoiskId, cancellationToken);

            cancellationToken.ThrowIfCancellationRequested();

            result.Item = ConvertResponseToItem(film);
            if (result.Item != null)
                result.HasMetadata = true;

            var staff = await _apiClient.GetStaff(kinopoiskId, cancellationToken);

            cancellationToken.ThrowIfCancellationRequested();

            foreach (var item in staff.ToPersonInfos())
                result.AddPerson(item);

            var trailers = await _apiClient.GetTrailers(kinopoiskId, cancellationToken);

            var remoteTrailers = trailers.ToMediaUrls();
            if (remoteTrailers is not null)
                result.Item.RemoteTrailers = remoteTrailers;

            return result;
        }

        public async Task<IEnumerable<RemoteSearchResult>> GetSearchResults(TLookupInfoType searchInfo, CancellationToken cancellationToken)
        {
            if (Utils.TryGetKinopoiskId(searchInfo, _logger, out var kinopoiskId))
            {
                var singleResult = (await _apiClient.GetSingleFilm(kinopoiskId, cancellationToken)).ToRemoteSearchResult();
                return Enumerable.Repeat(singleResult, 1);
            }
            else
            {
                return (await _apiClient.SearchByKeyword(searchInfo.Name, cancellationToken: cancellationToken)).ToRemoteSearchResults();
            }
        }
    }
}
