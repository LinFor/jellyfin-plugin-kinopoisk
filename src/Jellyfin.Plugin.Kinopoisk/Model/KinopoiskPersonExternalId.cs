using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Providers;

namespace Jellyfin.Plugin.Kinopoisk.Model
{
    public class KinopoiskPersonExternalId : IExternalId
    {
        public string ProviderName => Constants.ProviderName;

        public string Key => Constants.ProviderId;

        public string UrlFormatString => "https://www.kinopoisk.ru/name/{0}";

        public ExternalIdMediaType? Type => null;

        public bool Supports(IHasProviderIds item)
        {
            return item is Person;
        }
    }
}
