using System.Collections.Generic;

namespace Jellyfin.Plugin.Kinopoisk.ApiModel
{
    public class FilmDetails
    {
        public FilmData Data { get; set; }

        public FilmExtId ExternalId { get; set; }

        public FilmRating Rating { get; set; }

        //   "budget": {
        //     "grossRu": null,
        //     "grossUsa": null,
        //     "grossWorld": null,
        //     "budget": null,
        //     "marketing": null
        //   },

        //   "review": {
        //     "reviewsCount": 0,
        //     "ratingGoodReview": "",
        //     "ratingGoodReviewVoteCount": 0
        //   },

        public FilmImages Images { get; set; }

        public class FilmData
        {
            public int FilmId { get; set; }
            public string NameRu { get; set; }
            public string NameEn { get; set; }
            public string WebUrl { get; set; }
            public string PosterUrl { get; set; }
            public string Year { get; set; }
            public string FilmLength { get; set; }
            public string Slogan { get; set; }
            public string Description { get; set; }
            public string Type { get; set; } // FILM, TV_SHOW
            public string RatingMpaa { get; set; }
            public string RatingAgeLimits { get; set; }
            public string PremiereRu { get; set; }
            public string Distributors { get; set; }
            public string PremiereWorld { get; set; }
            public string PremiereDigital { get; set; }
            public string PremiereWorldCountry { get; set; }
            public string PremiereDvd { get; set; }
            public string PremiereBluRay { get; set; }
            public string DistributorRelease { get; set; }
            public CountryItem[] Countries { get; set; }
            public GenreItem[] Genres { get; set; }

            public class CountryItem
            {
                public string Country { get; set; }
            }

            public class GenreItem
            {
                public string Genre { get; set; }
            }
        }

        public class FilmExtId
        {
            public string ImdbId { get; set; }
        }

        public class FilmRating
        {
            public float Rating { get; set; }
            public int RatingVoteCount { get; set; }
            public float? RatingImdb { get; set; }
            public int RatingImdbVoteCount { get; set; }
            public string RatingFilmCritics { get; set; }
            public int RatingFilmCriticsVoteCount { get; set; }
        }

        public class FilmImages
        {
            public FilmImageRef[] Posters { get; set; }
            public FilmImageRef[] Backdrops { get; set; }
        }

        public class FilmImageRef
        {
            public string Language { get; set; }
            public string Url { get; set; }
            public int Height { get; set; }
            public int Width { get; set; }
        }
    }
}
