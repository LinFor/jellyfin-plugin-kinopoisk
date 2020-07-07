using System;
using MediaBrowser.Model.Entities;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.Kinopoisk
{
    public static class Utils
    {
        public const string ProviderId = "kinopoisk";
        public const string ProviderName = "КиноПоиск";
        public const string ProviderDescription = "Загружает рейтинги сайта КиноПоиск";
        public const string ProviderMetadataLanguage = "ru";

        public static bool TryGetKinopoiskId(IHasProviderIds item, ILogger logger, out int result)
        {
            var kinopoiskIdStr = item.GetProviderId(Utils.ProviderId);

            if (string.IsNullOrWhiteSpace(kinopoiskIdStr))
            {
                logger.LogDebug("Empty ProviderId, skipping");
                result = 0;
                return false;
            }

            if (!int.TryParse(kinopoiskIdStr, out result))
            {
                logger.LogWarning($"Can't parse Kinopoisk ProviderId to int, skipping ({kinopoiskIdStr})");
                result = 0;
                return false;
            }

            return true;
        }
    }
}
