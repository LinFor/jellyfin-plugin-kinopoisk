using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using MediaBrowser.Controller.Providers;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.Kinopoisk.ProviderIdResolvers
{
    public class CommonLookupInfoResolver<T> : CommonResolver<T>
        where T : ItemLookupInfo
    {
        private readonly Regex _kinopoiskIdRegex = new(@"kp-?(?<kinopoiskId>\d+)", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public CommonLookupInfoResolver(ILogger<CommonLookupInfoResolver<T>> logger)
            : base(logger)
        {
        }
        public override async Task<(bool IsSuccess, int ProviderId)> TryResolve(T info, CancellationToken? ct = null)
        {
            // Try to get from stored metadata
            var baseResult = await base.TryResolve(info, ct);
            if (baseResult.IsSuccess)
                return baseResult;

            // Try to get from filename
            if (!string.IsNullOrEmpty(info.Path))
            {
                var match = _kinopoiskIdRegex.Match(info.Path);
                if (match.Success && int.TryParse(match.Groups["kinopoiskId"].Value, out var result))
                {
                    _logger.LogDebug($"Got KinopoiskProviderId from filename ({result}, {info.Path})");
                    return (true, result);
                }
            }

            return (false, 0);
        }
    }
}
