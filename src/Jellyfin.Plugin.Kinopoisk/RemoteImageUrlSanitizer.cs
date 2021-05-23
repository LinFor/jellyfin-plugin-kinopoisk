using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Jellyfin.Plugin.Kinopoisk
{
    public class RemoteImageUrlSanitizer
    {
        private readonly HttpClient _httpClient;

        public RemoteImageUrlSanitizer(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new System.ArgumentNullException(nameof(httpClient));
        }

        public async Task<string> SanitizeRemoteImageUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
                return null;

            var currentUrl = url;
            while (!currentUrl.Contains("no-poster"))
            {
                var response = await _httpClient.SendAsync(
                    new HttpRequestMessage(HttpMethod.Get, currentUrl), HttpCompletionOption.ResponseHeadersRead);

                if ((int)response.StatusCode <= 299)
                    return currentUrl;
                else if (response.Headers.Location != null)
                {
                    currentUrl = response.Headers.Location.ToString();
                    continue;
                }
                else
                    throw new InvalidOperationException($"Unexpected answer HTTP {response.StatusCode}");
            }

            return null;
        }
    }
}
