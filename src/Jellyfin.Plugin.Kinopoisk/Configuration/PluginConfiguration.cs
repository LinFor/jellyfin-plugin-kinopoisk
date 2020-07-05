using MediaBrowser.Model.Plugins;

namespace Jellyfin.Plugin.Kinopoisk.Configuration
{
    public class PluginConfiguration : BasePluginConfiguration
    {
        // https://kinopoiskapiunofficial.tech/
        public string ApiToken { get; set; } = string.Empty;
    }
}
