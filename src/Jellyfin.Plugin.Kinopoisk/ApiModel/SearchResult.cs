using System.Collections.Generic;

namespace Jellyfin.Plugin.Kinopoisk.ApiModel
{
    public class SearchResult
    {
        public string Keyword { get; set; }
        public int PagesCount { get; set; }
        public List<Film> Films { get; set; }

        public class Film
        {
            public int FilmId { get; set; }
            public string NameRu { get; set; }
            public string NameEn { get; set; }
            public FilmType Type { get; set; }
            public string Year { get; set; }
            public string Description { get; set; }
            public string FilmLength { get; set; }
            public CountryItem[] Countries { get; set; }
            public GenreItem[] Genres { get; set; }
            public string Rating { get; set; }
            public int RatingVoteCount { get; set; }
            public string PosterUrl { get; set; }
        }
    }
}
