using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using KinopoiskUnofficialInfo.ApiClient;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.Movies;
using MediaBrowser.Controller.Entities.TV;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Providers;
using Microsoft.Extensions.Logging;
using Jellyfin.Data.Enums;
using Jellyfin.Extensions;

namespace Jellyfin.Plugin.Kinopoisk
{
    public static class ApiModelExtensions
    {
        public static RemoteSearchResult ToRemoteSearchResult(this Film src)
        {
            if (src is null)
                return null;

            var res = new RemoteSearchResult() {
                Name = src.GetLocalName(),
                ImageUrl = src.PosterUrl,
                PremiereDate = src.GetPremiereDate(),
                Overview = src.Description,
                SearchProviderName = Constants.ProviderName
            };
            res.SetProviderId(Constants.ProviderId, Convert.ToString(src.KinopoiskId));

            return res;
        }

        public static IEnumerable<RemoteSearchResult> ToRemoteSearchResults(this FilmSearchResponse src, ILogger logger)
        {
            if (src?.Films is null)
                return Enumerable.Empty<RemoteSearchResult>();

            return src.Films
                .Select(s => s.ToRemoteSearchResult(logger))
                .Where(s => s != null);
        }

        public static RemoteSearchResult ToRemoteSearchResult(this FilmSearchResponse_films src, ILogger logger)
        {
            try {
                if (src is null)
                    return null;

                var res = new RemoteSearchResult() {
                    Name = src.GetLocalName(),
                    ImageUrl = src.PosterUrl,
                    PremiereDate = src.GetPremiereDate(),
                    Overview = src.Description,
                    SearchProviderName = Constants.ProviderName
                };
                res.SetProviderId(Constants.ProviderId, Convert.ToString(src.FilmId));

                return res;
            }
            catch (Exception e) {
                logger.LogError(e, "Exception during parse");
                return null;
            }
        }

        public static Series ToSeries(this Film src)
        {
            if (src is null)
                return null;

            var res = new Series();

            FillCommonFilmInfo(src, res);

            // res.EndDate = src.Data.GetEndDate();
            // res.Status = src.Data.IsContinuing()
            //     ? SeriesStatus.Continuing
            //     : SeriesStatus.Ended;

            return res;
        }

        public static Movie ToMovie(this Film src)
        {
            if (src is null)
                return null;

            var res = new Movie();

            FillCommonFilmInfo(src, res);

            return res;
        }

        private static void FillCommonFilmInfo(Film src, BaseItem dst)
        {
            dst.SetProviderId(Constants.ProviderId, Convert.ToString(src.KinopoiskId));
            dst.Name = src.GetLocalName();
            dst.OriginalTitle = src.GetOriginalNameIfNotSame();
            dst.PremiereDate = src.GetPremiereDate();
            if (!string.IsNullOrWhiteSpace(src.Slogan))
                dst.Tagline = src.Slogan;
            dst.Overview = src.Description;
            if (src.Countries != null)
                dst.ProductionLocations = src.Countries.Select(c => c.Country1).ToArray();
            if (src.Genres != null)
                foreach(var genre in src.Genres.Select(c => c.Genre1))
                    dst.AddGenre(genre);
            if (!string.IsNullOrEmpty(src.RatingAgeLimits))
                dst.OfficialRating = $"{src.RatingAgeLimits}+";
            else
                dst.OfficialRating = src.RatingMpaa;

            dst.CommunityRating = (float)src.RatingKinopoisk;
            if (dst.CommunityRating < 0.1)
                dst.CommunityRating = (float)src.RatingImdb;
            if (dst.CommunityRating < 0.1)
                dst.CommunityRating = null;
            dst.CriticRating = src.GetCriticRatingAsTenPointBased();

            if (!string.IsNullOrWhiteSpace(src.ImdbId))
                dst.SetProviderId(MetadataProvider.Imdb, src.ImdbId);
        }

        public static float? GetCriticRatingAsTenPointBased(this Film src)
        {
            if (src is null)
                return null;

            if (src.RatingRfCritics > 0.0)
                return (float)src.RatingRfCritics;

            if (src.RatingFilmCritics > 0.0)
                return (float)src.RatingFilmCritics;

            return null;
        }

        public static IEnumerable<RemoteImageInfo> ToRemoteImageInfos(this Film src)
        {
            var res = Enumerable.Empty<RemoteImageInfo>();
            if (src is null)
                return res;

            if (src?.PosterUrl != null)
            {
                var mainPoster = new RemoteImageInfo(){
                    Type = ImageType.Primary,
                    Url = src.PosterUrl,
                    Language = Constants.ProviderMetadataLanguage,
                    ProviderName = Constants.ProviderName
                };
                res = res.Concat(Enumerable.Repeat(mainPoster, 1));
            }

            // if (src.Images != null)
            // {
            //     if (src.Images.Posters != null)
            //         res = res.Concat(src.Images.Posters.ToRemoteImageInfos(ImageType.Primary));
            //     if  (src.Images.Backdrops != null)
            //         res = res.Concat(src.Images.Backdrops.ToRemoteImageInfos(ImageType.Backdrop));
            // }

            return res;
        }

        // public static IEnumerable<RemoteImageInfo> ToRemoteImageInfos(this IEnumerable<Images_posters> src, ImageType imageType)
        // {
        //     return src.Select(s => s.ToRemoteImageInfo(imageType))
        //         .Where(s => s != null);
        // }

        // public static RemoteImageInfo ToRemoteImageInfo(this Images_posters src, ImageType imageType)
        // {
        //     if (src is null)
        //         return null;

        //     return new RemoteImageInfo(){
        //         Type = imageType,
        //         Url = src.Url,
        //         Language = src.Language,
        //         Height = src.Height,
        //         Width = src.Width,
        //         ProviderName = Constants.ProviderName
        //     };
        // }

        public static IReadOnlyList<MediaUrl> ToMediaUrls(this VideoResponse src)
        {
            if (src is null || src.Items is null || src.Items.Count < 1)
                return null;

            return src.Items.Select(t => t.ToMediaUrl())
                .Where(mu => mu != null)
                .ToList();
        }

        public static MediaUrl ToMediaUrl(this VideoResponse_items src) {
            if (src is null || !VideoResponse_itemsSite.YOUTUBE.Equals(src.Site))
                return null;

            return new MediaUrl
            {
                Name = src.Name,
                Url = src.Url.SanitizeYoutubeLink()
            };
        }

        public static string SanitizeYoutubeLink(this string src)
        {
            // Jellyfin web currently recognizes only https://www.youtube.com/watch?v=xxx links
            return src
                .Replace("http://", "https://")
                .Replace("https://youtu.be/", "https://www.youtube.com/watch?v=")
                .Replace("https://www.youtube.com/v/", "https://www.youtube.com/watch?v=");
        }

        public static RemoteImageInfo ToRemoteImageInfo(this PersonResponse src)
        {
            if (src is null || string.IsNullOrEmpty(src.PosterUrl))
                return null;

            return new RemoteImageInfo(){
                Type = ImageType.Primary,
                Url = src.PosterUrl,
                ProviderName = Constants.ProviderName
            };
        }

        public static PersonInfo ToPersonInfo(this StaffResponse src)
        {
            if (src is null)
                return null;

            var res = new PersonInfo()
            {
                Name = src.NameRu,
                ImageUrl = src.PosterUrl,
                Role = src.ProfessionText ?? null,
                Type = src.ProfessionKey.ToPersonType()
            };
            if (string.IsNullOrWhiteSpace(res.Name))
                res.Name = src.NameEn ?? string.Empty;
            if (src.AdditionalProperties.TryGetValue("description", out var description))
                res.Role = description as string;

            res.SetProviderId(Constants.ProviderId, Convert.ToString(src.StaffId));

            return res;
        }

        public static IEnumerable<PersonInfo> ToPersonInfos(this ICollection<StaffResponse> src)
        {
            var res = src.Select(s => s.ToPersonInfo())
                .Where(s => s != null)
                .ToArray();

            var i = 0;
            foreach(var item in res)
                item.SortOrder = ++i;

            return res;
        }

        public static PersonKind ToPersonType(this StaffResponseProfessionKey src)
        {
            return src switch
            {
                StaffResponseProfessionKey.ACTOR => PersonKind.Actor,
                StaffResponseProfessionKey.DIRECTOR => PersonKind.Director,
                StaffResponseProfessionKey.WRITER => PersonKind.Writer,
                StaffResponseProfessionKey.COMPOSER => PersonKind.Composer,
                StaffResponseProfessionKey.PRODUCER or StaffResponseProfessionKey.PRODUCER_USSR => PersonKind.Producer,
               // _ => PersonKind,
            };
        }

        public static DateTime? ParseDate(this string src){
            if (src == null)
                return null;

            if (DateTime.TryParseExact(src, "o", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var res))
                return res;

            return null;
        }

        public static DateTime? GetPremiereDate(this Film src)
        {
            // var res = src.IsRussianSpokenOriginated()
            //     ? src.PremiereRu.ParseDate()
            //     : src.PremiereWorld.ParseDate();
            // if (src.PremiereRu.ParseDate() < res)
            //     res = src.PremiereRu.ParseDate();
            // if (src.PremiereWorld.ParseDate() < res)
            //     res = src.PremiereWorld.ParseDate();
            // if (src.PremiereDigital.ParseDate() < res)
            //     res = src.PremiereDigital.ParseDate();
            // if (src.PremiereDvd.ParseDate() < res)
            //     res = src.PremiereDvd.ParseDate();
            // if (src.PremiereBluRay.ParseDate() < res)
            //     res = src.PremiereBluRay.ParseDate();

            // if (res.HasValue)
            //     return res;

            if (src.Year > 1900)
                return new DateTime(src.Year, 1, 1);

            return null;
        }

        public static DateTime? GetPremiereDate(this FilmSearchResponse_films src)
        {
            var firstYear = GetFirstYear(src.Year);
            if (firstYear != null)
                return new DateTime(firstYear.Value, 1, 1);

            return null;
        }

        public static string GetLocalName(this Film src)
        {
            var res = src?.NameRu;
            if (string.IsNullOrWhiteSpace(res))
                res = src?.NameOriginal;
            if (string.IsNullOrWhiteSpace(res))
                res = src?.NameEn;
            return res;
        }

        public static string GetLocalName(this FilmSearchResponse_films src)
        {
            var res = src?.NameRu;
            if (string.IsNullOrWhiteSpace(res))
                res = src?.NameEn;
            return res;
        }

        public static string GetOriginalName(this Film src)
            => src?.NameOriginal ??
                (src.IsRussianSpokenOriginated()
                    ? src?.NameRu
                    : src?.NameEn);

        public static string GetOriginalNameIfNotSame(this Film src)
        {
            var localName = src.GetLocalName();
            var originalName = src.GetOriginalName();
            if (!string.IsNullOrWhiteSpace(originalName) && !string.Equals(localName, originalName))
                return originalName;

            return string.Empty;
        }

        public static bool IsRussianSpokenOriginated(this Film src)
            => src?.Countries?.IsRussianSpokenOriginated() ?? false;

        public static bool IsRussianSpokenOriginated(this IEnumerable<Country> src)
        {
            if (src is null)
                return false;

            foreach(var country in src)
                switch(country.Country1)
                {
                    case "Россия":
                        return true;
                }

            return false;
        }

        public static int? GetFirstYear(string years)
        {
            if (string.IsNullOrWhiteSpace(years) || years.ToLower() == "null")
                return null;

            years = years.Trim();

            if (int.TryParse(years, out var res))
                return res;

            var i = 0;
            while (true) {
                if (i > 4)
                    return null;
                if (!char.IsDigit(years[i]))
                    break;
                i++;
            }

            return Convert.ToInt32(years.Substring(0, i));
        }

        public static bool IsСontinuing(string years)
            => years?.EndsWith("-...") ?? false;

        public static int? GetLastYear(string years)
        {
            if (string.IsNullOrWhiteSpace(years))
                return null;

            years = years.Trim();

            if (int.TryParse(years, out var res))
                return res;

            var i = 0;
            int startindex() => years.Length - 1 - i;
            while (true) {
                if (i > 4)
                    return null;
                if (!char.IsDigit(years[startindex()]))
                {
                    i--;
                    break;
                }
                i++;
            }

            return i > 0
                ? (int?)Convert.ToInt32(years[startindex()..])
                : null;
        }

        public static Person ToPerson(this PersonResponse src)
        {
            if (src is null)
                return null;

            var res = new Person()
            {
                Name = src.GetLocalName(),
                PremiereDate = src.Birthday.ParseDate(),
                EndDate = src.Death.ParseDate()
            };

            if (!string.IsNullOrWhiteSpace(src.Birthplace))
                res.ProductionLocations = new[] { src.Birthplace };

            return res;
        }

        public static string GetLocalName(this PersonResponse src)
        {
            var res = src?.NameRu;
            if (string.IsNullOrWhiteSpace(res))
                res = src?.NameEn;
            return res;
        }
    }
}
