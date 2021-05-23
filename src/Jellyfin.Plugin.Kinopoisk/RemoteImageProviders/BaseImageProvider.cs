using System;
using System.Collections.Generic;
using System.Linq;
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

        protected async Task<IEnumerable<RemoteImageInfo>> FilterEmptyImages(IEnumerable<RemoteImageInfo> images)
        {
            using var httpClient = new HttpClient(new HttpClientHandler() { AllowAutoRedirect = false }, true);
            var sanitizer = new RemoteImageUrlSanitizer(httpClient);
            var res = await Task.WhenAll(images.Select(async i => {
                var sanitizedUrl = await sanitizer.SanitizeRemoteImageUrl(i.Url);
                if (string.IsNullOrEmpty(sanitizedUrl))
                    return null;

                i.Url = sanitizedUrl;
                return i;
            }));

            return res.Where(i => i != null).ToArray();
        }
    }
}
