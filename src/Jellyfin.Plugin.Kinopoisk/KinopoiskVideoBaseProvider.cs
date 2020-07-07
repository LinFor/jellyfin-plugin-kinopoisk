using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Jellyfin.Plugin.Kinopoisk.ApiModel;
using MediaBrowser.Common.Net;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Providers;
using MediaBrowser.Model.Serialization;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.Kinopoisk
{
    public abstract class KinopoiskVideoBaseProvider<TItemType, TLookupInfoType> : IRemoteMetadataProvider<TItemType, TLookupInfoType>
        where TItemType : BaseItem, IHasLookupInfo<TLookupInfoType>
        where TLookupInfoType : ItemLookupInfo, new()
    {
        private readonly ILogger _logger;
        private readonly IHttpClient _httpClient;
        private readonly IJsonSerializer _jsonSerializer;
        private readonly KinopoiskApiProxy _apiProxy;

        public string Name => Utils.ProviderName;

        public KinopoiskVideoBaseProvider(ILogger logger,
                                       IHttpClient httpClient,
                                       IJsonSerializer jsonSerializer)
        {
            if (logger is null)
            {
                throw new System.ArgumentNullException(nameof(logger));
            }

            if (httpClient is null)
            {
                throw new System.ArgumentNullException(nameof(httpClient));
            }

            if (jsonSerializer is null)
            {
                throw new System.ArgumentNullException(nameof(jsonSerializer));
            }

            this._logger = logger;
            this._httpClient = httpClient;
            this._jsonSerializer = jsonSerializer;
            this._apiProxy = new KinopoiskApiProxy(logger, httpClient, jsonSerializer);
        }

        protected abstract TItemType ConvertResponseToItem(FilmDetails apiResponse);

        public Task<HttpResponseInfo> GetImageResponse(string url, CancellationToken cancellationToken)
        {
            return _httpClient.GetResponse(new HttpRequestOptions
            {
                CancellationToken = cancellationToken,
                Url = url
            });
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

            foreach(var item in staff.ToPersonInfos())
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
