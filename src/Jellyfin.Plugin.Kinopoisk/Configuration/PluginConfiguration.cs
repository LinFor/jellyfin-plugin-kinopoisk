using MediaBrowser.Model.Plugins;

namespace Jellyfin.Plugin.Kinopoisk.Configuration
{
    public class PluginConfiguration : BasePluginConfiguration
    {
        // https://kinopoiskapiunofficial.tech/
        public string ApiToken { get; set; } = "85d30ae5-d875-4c5f-900d-8e37bb20625e";
    }
}
