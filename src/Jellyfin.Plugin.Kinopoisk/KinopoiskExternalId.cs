using MediaBrowser.Controller.Entities.Movies;
using MediaBrowser.Controller.Entities.TV;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;

namespace Jellyfin.Plugin.Kinopoisk
{
    public class KinopoiskExternalId : IExternalId
    {
        public string ProviderName => Utils.ProviderName;

        public string Name => Utils.ProviderName;

        public string Key => Utils.ProviderId;

        public string UrlFormatString => "https://www.kinopoisk.ru/film/{0}";

        public bool Supports(IHasProviderIds item)
        {
            return item is Movie || item is Series;
        }
    }
}
