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
        public static Series ToSeries(this FilmDetails src)
        {
            if (src.Data is null)
                return null;

            var res = new Series();

            FillCommonFilmInfo(src, res);

            return res;
        }

        public static Movie ToMovie(this FilmDetails src)
        {
            if (src.Data is null)
                return null;

            var res = new Movie();

            FillCommonFilmInfo(src, res);

            return res;
        }

        private static void FillCommonFilmInfo(FilmDetails src, BaseItem dst)
        {
            dst.SetProviderId(Utils.ProviderId, Convert.ToString(src.Data.FilmId));
            dst.Name = src.Data.NameRu;
            if (string.IsNullOrWhiteSpace(dst.Name))
                dst.Name = src.Data.NameEn;
            // if (!string.IsNullOrWhiteSpace(film.Data.WebUrl))
            //     result.Item.HomePageUrl = film.Data.WebUrl;
            dst.ProductionYear = Utils.GetFirstYear(src.Data.Year);
            dst.Overview = src.Data.Description;
            if (src.Data.Countries != null)
                dst.ProductionLocations = src.Data.Countries.Select(c => c.Country).ToArray();
            if (src.Data.Genres != null)
                foreach(var genre in src.Data.Genres.Select(c => c.Genre))
                    dst.AddGenre(genre);
            if (!string.IsNullOrWhiteSpace(src.Data.RatingAgeLimits))
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
    }
}
