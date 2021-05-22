using Jellyfin.Plugin.Kinopoisk.Api;
using MediaBrowser.Common.Plugins;
using Microsoft.Extensions.DependencyInjection;

namespace Jellyfin.Plugin.Kinopoisk
{
    /// <summary>
    /// Registers services
    /// </summary>
    public class KinopoiskPluginServiceRegistrator : IPluginServiceRegistrator
    {
        public void RegisterServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<KinopoiskApiProxy>();
        }
    }
}
