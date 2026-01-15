using System.Collections.Generic;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.Movies;
using MediaBrowser.Controller.Entities.TV;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Providers;

namespace Jellyfin.Plugin.Kinopoisk.Model
{
    /// <summary>
    /// External URLs for Kinopoisk.
    /// </summary>
    public class KinopoiskExternalUrlProvider : IExternalUrlProvider
    {
        /// <inheritdoc/>
        public string Name => Constants.ProviderName;

        /// <inheritdoc/>
        public IEnumerable<string> GetExternalUrls(BaseItem item)
        {
            var baseUrl = "https://www.kinopoisk.ru/";
            if (item.TryGetProviderId(Constants.ProviderId, out var externalId))
            {
                if (item is Person)
                {
                    yield return baseUrl + $"name/{externalId}/";
                }
                else if (item is Movie)
                {
                    yield return baseUrl + $"film/{externalId}/";
                }
                else if (item is Series)
                {
                    yield return baseUrl + $"series/{externalId}/";
                }
            }
        }
    }
}
