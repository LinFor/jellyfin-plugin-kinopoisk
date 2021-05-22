using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Jellyfin.Plugin.Kinopoisk.Api.Model;

namespace Jellyfin.Plugin.Kinopoisk.Api
{
    public interface IKinopoiskApiProxy
    {
        Task<FilmDetails> GetSingleFilm(int filmId, CancellationToken? cancellationToken = null);
        Task<FilmDetails> GetSingleFilmImages(int filmId, CancellationToken? cancellationToken = null);
        Task<IEnumerable<StaffItem>> GetStaff(int filmId, CancellationToken? cancellationToken = null);
        Task<SearchResult> SearchByKeyword(string keyword, int page = 1, CancellationToken? cancellationToken = null);
    }
}
