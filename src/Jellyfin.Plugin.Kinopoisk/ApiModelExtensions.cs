using System;
using System.Collections.Generic;
using System.Linq;
using Jellyfin.Plugin.Kinopoisk.ApiModel;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.Movies;
using MediaBrowser.Controller.Entities.TV;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Providers;
using static Jellyfin.Plugin.Kinopoisk.ApiModel.FilmDetails;

namespace Jellyfin.Plugin.Kinopoisk
{
    public static class ApiModelExtensions
    {
        public static RemoteSearchResult ToRemoteSearchResult(this FilmDetails src)
        {
            if (src?.Data is null)
                return null;

            var res = new RemoteSearchResult() {
                Name = src.GetLocalName(),
                ImageUrl = src.Data.PosterUrl,
                PremiereDate = src.Data.GetPremiereDate(),
                Overview = src.Data.Description,
                SearchProviderName = Utils.ProviderName
            };
            res.SetProviderId(Utils.ProviderId, Convert.ToString(src.Data.FilmId));

            return res;
        }

        public static IEnumerable<RemoteSearchResult> ToRemoteSearchResults(this SearchResult src)
        {
            if (src?.Films is null)
                return Enumerable.Empty<RemoteSearchResult>();

            return src.Films.Select(s => s.ToRemoteSearchResult());
        }

        public static RemoteSearchResult ToRemoteSearchResult(this SearchResult.Film src)
        {
            if (src is null)
                return null;

            var res = new RemoteSearchResult() {
                Name = src.GetLocalName(),
                ImageUrl = src.PosterUrl,
                PremiereDate = src.GetPremiereDate(),
                Overview = src.Description,
                SearchProviderName = Utils.ProviderName
            };
            res.SetProviderId(Utils.ProviderId, Convert.ToString(src.FilmId));

            return res;
        }

        public static Series ToSeries(this FilmDetails src)
        {
            if (src?.Data is null)
                return null;

            var res = new Series();

            FillCommonFilmInfo(src, res);

            res.EndDate = src.Data.GetEndDate();
            res.Status = src.Data.IsContinuing()
                ? SeriesStatus.Continuing
                : SeriesStatus.Ended;

            return res;
        }

        public static Movie ToMovie(this FilmDetails src)
        {
            if (src?.Data is null)
                return null;

            var res = new Movie();

            FillCommonFilmInfo(src, res);

            return res;
        }

        private static void FillCommonFilmInfo(FilmDetails src, BaseItem dst)
        {
            dst.SetProviderId(Utils.ProviderId, Convert.ToString(src.Data.FilmId));
            dst.Name = src.GetLocalName();
            dst.OriginalTitle = src.GetOriginalNameIfNotSame();
            dst.PremiereDate = src.Data.GetPremiereDate();
            dst.Overview = src.Data.Description;
            if (src.Data.Countries != null)
                dst.ProductionLocations = src.Data.Countries.Select(c => c.Country).ToArray();
            if (src.Data.Genres != null)
                foreach(var genre in src.Data.Genres.Select(c => c.Genre))
                    dst.AddGenre(genre);
            if (src.Data.RatingAgeLimits.HasValue)
                dst.OfficialRating = $"{src.Data.RatingAgeLimits}+";
            else
                dst.OfficialRating = src.Data.RatingMpaa;

            if (src.Rating != null)
            {
                dst.CommunityRating = src.Rating.Rating > 0
                    ? src.Rating.Rating
                    : src.Rating.RatingImdb;
                dst.CriticRating = src.Rating.GetCriticRatingAsTenPointBased();
            }

            if (src.ExternalId != null)
            {
                if (!string.IsNullOrWhiteSpace(src.ExternalId.ImdbId))
                    dst.SetProviderId(MetadataProvider.Imdb, src.ExternalId.ImdbId);
            }
        }

        public static float? GetCriticRatingAsTenPointBased(this FilmDetails.FilmRating src)
        {
            if (src is null)
                return null;

            if (string.IsNullOrWhiteSpace(src.RatingFilmCritics))
                return null;

            if (float.TryParse(src.RatingFilmCritics, out var res))
                return res;

            var ratingStr = src.RatingFilmCritics.Replace("%", string.Empty);
            if (int.TryParse(ratingStr, out var res_pct))
                return res_pct * 0.1f;

            return null;
        }

        public static IEnumerable<RemoteImageInfo> ToRemoteImageInfos(this FilmDetails src)
        {
            var res = Enumerable.Empty<RemoteImageInfo>();
            if (src is null)
                return res;

            if (src?.Data?.PosterUrl != null)
            {
                var mainPoster = new RemoteImageInfo(){
                    Type = ImageType.Primary,
                    Url = src.Data.PosterUrl,
                    Language = Utils.ProviderMetadataLanguage,
                    ProviderName = Utils.ProviderName
                };
                res = res.Concat(Enumerable.Repeat(mainPoster, 1));
            }

            if (src.Images != null)
            {
                if (src.Images.Posters != null)
                    res = res.Concat(src.Images.Posters.ToRemoteImageInfos(ImageType.Primary));
                if  (src.Images.Backdrops != null)
                    res = res.Concat(src.Images.Backdrops.ToRemoteImageInfos(ImageType.Backdrop));
            }

            return res;
        }

        public static IEnumerable<RemoteImageInfo> ToRemoteImageInfos(this IEnumerable<FilmImageRef> src, ImageType imageType)
        {
            return src.Select(s => s.ToRemoteImageInfo(imageType))
                .Where(s => s != null);
        }

        public static RemoteImageInfo ToRemoteImageInfo(this FilmImageRef src, ImageType imageType)
        {
            if (src is null)
                return null;

            return new RemoteImageInfo(){
                Type = imageType,
                Url = src.Url,
                Language = src.Language,
                Height = src.Height,
                Width = src.Width,
                ProviderName = Utils.ProviderName
            };
        }

        public static PersonInfo ToPersonInfo(this StaffItem src)
        {
            if (src is null)
                return null;

            var res = new PersonInfo()
            {
                Name = src.NameRu,
                ImageUrl = src.PosterUrl,
                Role = src.Description,
                Type = src.ProfessionKey.ToPersonType()
            };
            if (string.IsNullOrWhiteSpace(res.Name))
                res.Name = src.NameEn;
            res.SetProviderId(Utils.ProviderId, Convert.ToString(src.StaffId));

            return res;
        }

        public static IEnumerable<PersonInfo> ToPersonInfos(this IEnumerable<StaffItem> src)
        {
            var res = src.Select(s => s.ToPersonInfo())
                .Where(s => s != null)
                .ToArray();

            var i = 0;
            foreach(var item in res)
                item.SortOrder = ++i;

            return res;
        }

        public static string ToPersonType(this ProfessionEnum src)
        {
            switch (src)
            {
                case ProfessionEnum.Actor:
                    return PersonType.Actor;
                case ProfessionEnum.Director:
                    return PersonType.Director;
                case ProfessionEnum.Writer:
                    return PersonType.Writer;
                case ProfessionEnum.Composer:
                    return PersonType.Composer;
                case ProfessionEnum.Producer:
                case ProfessionEnum.ProducerUssr:
                    return PersonType.Producer;
                default:
                    return null;
            }
        }

        public static DateTime? GetPremiereDate(this FilmData src)
        {
            var res = src.IsRussianSpokenOriginated()
                ? src.PremiereRu
                : src.PremiereWorld;
            if (src.PremiereRu < res)
                res = src.PremiereRu;
            if (src.PremiereWorld < res)
                res = src.PremiereWorld;
            if (src.PremiereDigital < res)
                res = src.PremiereDigital;
            if (src.PremiereDvd < res)
                res = src.PremiereDvd;
            if (src.PremiereBluRay < res)
                res = src.PremiereBluRay;

            if (res.HasValue)
                return res;

            var firstYear = GetFirstYear(src.Year);
            if (firstYear != null)
                return new DateTime(firstYear.Value, 1, 1);

            return null;
        }

        public static DateTime? GetPremiereDate(this SearchResult.Film src)
        {
            var firstYear = GetFirstYear(src.Year);
            if (firstYear != null)
                return new DateTime(firstYear.Value, 1, 1);

            return null;
        }

        public static DateTime? GetEndDate(this FilmData src)
        {
            var lastYear = GetLastYear(src.Year);
            if (lastYear != null)
                return new DateTime(lastYear.Value, 12, 31);

            return null;
        }

        public static bool IsContinuing(this FilmData src)
            => IsСontinuing(src?.Year);

        public static string GetLocalName(this FilmDetails src)
        {
            var res = src?.Data?.NameRu;
            if (string.IsNullOrWhiteSpace(res))
                res = src?.Data?.NameEn;
            return res;
        }

        public static string GetLocalName(this SearchResult.Film src)
        {
            var res = src?.NameRu;
            if (string.IsNullOrWhiteSpace(res))
                res = src?.NameEn;
            return res;
        }

        public static string GetOriginalName(this FilmDetails src)
            => src.IsRussianSpokenOriginated()
                ? src?.Data?.NameRu
                : src?.Data?.NameEn;

        public static string GetOriginalNameIfNotSame(this FilmDetails src)
        {
            var localName = src.GetLocalName();
            var originalName = src.GetOriginalName();
            if (!string.IsNullOrWhiteSpace(originalName) && !string.Equals(localName, originalName))
                return originalName;

            return string.Empty;
        }

        public static bool IsRussianSpokenOriginated(this FilmDetails src)
            => src?.Data?.IsRussianSpokenOriginated() ?? false;

        public static bool IsRussianSpokenOriginated(this FilmData src)
            => src?.Countries?.IsRussianSpokenOriginated() ?? false;

        public static bool IsRussianSpokenOriginated(this IEnumerable<CountryItem> src)
        {
            if (src is null)
                return false;

            foreach(var country in src)
                switch(country.Country)
                {
                    case "Россия":
                        return true;
                }

            return false;
        }

        public static int? GetFirstYear(string years)
        {
            if (string.IsNullOrWhiteSpace(years))
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
            Func<int> startindex = () => years.Length - 1 - i;
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
                ? (int?)Convert.ToInt32(years.Substring(startindex()))
                : null;
        }
    }
}
