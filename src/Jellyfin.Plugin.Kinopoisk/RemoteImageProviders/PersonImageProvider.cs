using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Jellyfin.Plugin.Kinopoisk.ProviderIdResolvers;
using KinopoiskUnofficialInfo.ApiClient;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Providers;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.Kinopoisk.MetadataProviders
{
    public class PersonImageProvider : BaseImageProvider
    {
        private readonly IKinopoiskApiClient _apiClient;
        private readonly IProviderIdResolver<BaseItem> _providerIdResolver;
        private readonly ILogger<PersonImageProvider> _logger;

        public PersonImageProvider(IKinopoiskApiClient kinopoiskApiClient, IProviderIdResolver<BaseItem> providerIdResolver, ILogger<PersonImageProvider> logger, IHttpClientFactory httpClientFactory)
            : base(httpClientFactory)
        {
            _apiClient = kinopoiskApiClient ?? throw new System.ArgumentNullException(nameof(kinopoiskApiClient));
            _providerIdResolver = providerIdResolver ?? throw new System.ArgumentNullException(nameof(providerIdResolver));
            _logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
        }

        public override string Name => Constants.ProviderName;

        public override bool Supports(BaseItem item)
            => item is Person;

        public override async Task<IEnumerable<RemoteImageInfo>> GetImages(BaseItem item, CancellationToken cancellationToken)
        {
            var (resolveResult, kinopoiskId) = await _providerIdResolver.TryResolve(item, cancellationToken);
            if (!resolveResult)
                return Enumerable.Empty<RemoteImageInfo>();

            var person = await _apiClient.GetPerson(kinopoiskId, cancellationToken);

            var res = new[] { person.ToRemoteImageInfo() };
            return await FilterEmptyImages(res);
        }

        public override IEnumerable<ImageType> GetSupportedImages(BaseItem item)
        {
            yield return ImageType.Primary;
        }
    }
}
