using System.Threading;
using System.Threading.Tasks;
using MediaBrowser.Model.Entities;

namespace Jellyfin.Plugin.Kinopoisk.ProviderIdResolvers
{
    public interface IProviderIdResolver<T>
        where T: IHasProviderIds
    {
        Task<(bool IsSuccess, int ProviderId)> TryResolve(T info, CancellationToken? ct = null);
    }
}
