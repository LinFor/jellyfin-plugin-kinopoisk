using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Jellyfin.Plugin.Kinopoisk.Api;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Providers;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.Kinopoisk.MetadataProviders
{
    public class PersonImageProvider : BaseImageProvider
    {
        private readonly KinopoiskApiProxy _kinopoiskApiProxy;
        private readonly ILogger<PersonImageProvider> _logger;

        public PersonImageProvider(KinopoiskApiProxy kinopoiskApiProxy, ILogger<PersonImageProvider> logger, IHttpClientFactory httpClientFactory)
            : base(httpClientFactory)
        {
            _kinopoiskApiProxy = kinopoiskApiProxy ?? throw new System.ArgumentNullException(nameof(kinopoiskApiProxy));
            _logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
        }

        public override string Name => Utils.ProviderName;

        public override bool Supports(BaseItem item)
            => item is Person;

        public override Task<IEnumerable<RemoteImageInfo>> GetImages(BaseItem item, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public override IEnumerable<ImageType> GetSupportedImages(BaseItem item)
        {
            yield return ImageType.Primary;
        }
    }
}
