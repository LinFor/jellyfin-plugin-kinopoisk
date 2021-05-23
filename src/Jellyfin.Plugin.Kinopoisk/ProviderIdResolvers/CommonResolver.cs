using System.Threading;
using System.Threading.Tasks;
using MediaBrowser.Model.Entities;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.Kinopoisk.ProviderIdResolvers
{
    public class CommonResolver<T> : IProviderIdResolver<T>
        where T : IHasProviderIds
    {
        protected readonly ILogger<CommonResolver<T>> _logger;

        public CommonResolver(ILogger<CommonResolver<T>> logger)
        {
            _logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
        }

        public virtual Task<(bool IsSuccess, int ProviderId)> TryResolve(T info, CancellationToken? ct = null)
        {
            // Try to get from stored metadata
            var kinopoiskIdStr = info.GetProviderId(Constants.ProviderId);

            // Try to get from stored metadata
            if (!string.IsNullOrEmpty(kinopoiskIdStr) && int.TryParse(kinopoiskIdStr, out var result))
            {
                _logger.LogDebug($"Got KinopoiskProviderId from metadata ({result})");
                return Task.FromResult((true, result));
            }

            return Task.FromResult((false, 0));
        }
    }
}
