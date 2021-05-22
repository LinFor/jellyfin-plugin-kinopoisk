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
            var res = await Task.WhenAll(images.Select(i => NullIfEmptyImage(i, httpClient)));

            return res.Where(i => i != null);
        }

        protected async Task<RemoteImageInfo> NullIfEmptyImage(RemoteImageInfo info, HttpClient httpClient)
        {
            if (info == null)
                return null;

            var currentUrl = info.Url;
            while (!currentUrl.Contains("no-poster"))
            {
                var response = await httpClient.SendAsync(
                    new HttpRequestMessage(HttpMethod.Get, currentUrl), HttpCompletionOption.ResponseHeadersRead);

                if ((int)response.StatusCode <= 299)
                    return info;
                else if (response.Headers.Location != null)
                {
                    currentUrl = response.Headers.Location.ToString();
                    continue;
                }
                else
                    throw new InvalidOperationException("Not expected answer");
            }

            return null;
        }
    }
}
