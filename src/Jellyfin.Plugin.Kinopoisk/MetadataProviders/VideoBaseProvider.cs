using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Jellyfin.Plugin.Kinopoisk.Api;
using Jellyfin.Plugin.Kinopoisk.Api.Model;
using MediaBrowser.Common.Net;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Providers;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.Kinopoisk.MetadataProviders
{
    public abstract class VideoBaseProvider<TItemType, TLookupInfoType> : IRemoteMetadataProvider<TItemType, TLookupInfoType>
        where TItemType : BaseItem, IHasLookupInfo<TLookupInfoType>
        where TLookupInfoType : ItemLookupInfo, new()
    {
        private readonly ILogger _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly KinopoiskApiProxy _apiProxy;

        public string Name => Utils.ProviderName;

        public VideoBaseProvider(KinopoiskApiProxy kinopoiskApiProxy, ILogger logger, IHttpClientFactory httpClientFactory)
        {
            this._logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
            this._httpClientFactory = httpClientFactory ?? throw new System.ArgumentNullException(nameof(httpClientFactory));
            this._apiProxy = kinopoiskApiProxy ?? throw new System.ArgumentNullException(nameof(kinopoiskApiProxy));
        }

        protected abstract TItemType ConvertResponseToItem(FilmDetails apiResponse);

        public async Task<HttpResponseMessage> GetImageResponse(string url, CancellationToken cancellationToken)
        {
            return await _httpClientFactory.CreateClient(NamedClient.Default).GetAsync(url, cancellationToken);
        }

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

            var film = await _apiProxy.GetSingleFilm(kinopoiskId, cancellationToken);

            result.Item = ConvertResponseToItem(film);
            if (result.Item != null)
                result.HasMetadata = true;

            cancellationToken.ThrowIfCancellationRequested();

            var staff = await _apiProxy.GetStaff(kinopoiskId, cancellationToken);

            foreach (var item in staff.ToPersonInfos())
                result.AddPerson(item);

            return result;
        }

        public async Task<IEnumerable<RemoteSearchResult>> GetSearchResults(TLookupInfoType searchInfo, CancellationToken cancellationToken)
        {
            if (Utils.TryGetKinopoiskId(searchInfo, _logger, out var kinopoiskId))
            {
                var singleResult = (await _apiProxy.GetSingleFilm(kinopoiskId, cancellationToken)).ToRemoteSearchResult();
                return Enumerable.Repeat(singleResult, 1);
            }
            else
            {
                return (await _apiProxy.SearchByKeyword(searchInfo.Name, cancellationToken: cancellationToken)).ToRemoteSearchResults();
            }
        }
    }
}
