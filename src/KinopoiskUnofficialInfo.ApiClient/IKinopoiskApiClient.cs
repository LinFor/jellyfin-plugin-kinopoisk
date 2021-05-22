using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace KinopoiskUnofficialInfo.ApiClient
{
    public interface IKinopoiskApiClient
    {
        Task<PersonResponse> GetPerson(int personId, CancellationToken? cancellationToken = null);
        Task<Film> GetSingleFilm(int filmId, CancellationToken? cancellationToken = null);
        Task<ICollection<StaffResponse>> GetStaff(int filmId, CancellationToken? cancellationToken = null);
        Task<VideoResponse> GetTrailers(int filmId, CancellationToken? cancellationToken = null);
        Task<FilmSearchResponse> SearchByKeyword(string keyword, int page = 1, CancellationToken? cancellationToken = null);
    }
}
