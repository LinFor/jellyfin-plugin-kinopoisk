using System;
using System.Text.RegularExpressions;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.Kinopoisk
{
    public static class Constants
    {
        public const string ProviderId = "kinopoisk";
        public const string ProviderName = "КиноПоиск";
        public const string ProviderDescription = "Информация о фильмах и сериалах с КиноПоиска";
        public const string ProviderMetadataLanguage = "ru";
    }
}
