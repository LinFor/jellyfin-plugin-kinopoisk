using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.Movies;
using MediaBrowser.Controller.Entities.TV;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Providers;

namespace Jellyfin.Plugin.Kinopoisk.Model
{
    public class KinopoiskExternalId : IExternalId
    {
        public string ProviderName => Utils.ProviderName;

        public string Key => Utils.ProviderId;

        public string UrlFormatString => "https://www.kinopoisk.ru/film/{0}";

        public ExternalIdMediaType? Type => null;

        public bool Supports(IHasProviderIds item)
        {
            return item is Movie || item is Series;
        }
    }
}
