using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using MediaBrowser.Common.Net;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Providers;

namespace Jellyfin.Plugin.Kinopoisk.MetadataProviders
{
    public abstract class BaseImageProvider : IRemoteImageProvider
    {
        private readonly IHttpClientFactory _httpClientFactory;

        protected BaseImageProvider(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        }

        public abstract string Name { get; }

        public abstract bool Supports(BaseItem item);

        public async Task<HttpResponseMessage> GetImageResponse(string url, CancellationToken cancellationToken)
        {
            return await _httpClientFactory.CreateClient(NamedClient.Default).GetAsync(url, cancellationToken);
        }

        public abstract Task<IEnumerable<RemoteImageInfo>> GetImages(BaseItem item, CancellationToken cancellationToken);

        public abstract IEnumerable<ImageType> GetSupportedImages(BaseItem item);

    }
}
