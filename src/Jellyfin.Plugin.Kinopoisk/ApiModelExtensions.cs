using System;
using System.Collections.Generic;
using System.Linq;
using Jellyfin.Plugin.Kinopoisk.ApiModel;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.Movies;
using MediaBrowser.Controller.Entities.TV;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Providers;

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
                    dst.SetProviderId(MetadataProviders.Imdb, src.ExternalId.ImdbId);
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
                    res = res.Concat(src.Images.Posters.Select(i => new RemoteImageInfo(){
                        Type = ImageType.Primary,
                        Url = i.Url,
                        Language = i.Language,
                        Height = i.Height,
                        Width = i.Width,
                        ProviderName = Utils.ProviderName
                    }));
                if  (src.Images.Backdrops != null)
                    res = res.Concat(src.Images.Backdrops.Select(i => new RemoteImageInfo(){
                        Type = ImageType.Backdrop,
                        Url = i.Url,
                        Language = i.Language,
                        Height = i.Height,
                        Width = i.Width,
                        ProviderName = Utils.ProviderName
                    }));
            }

            return res;
        }
    }
}
