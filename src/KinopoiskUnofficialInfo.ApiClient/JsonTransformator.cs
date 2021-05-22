using Newtonsoft.Json;

namespace KinopoiskUnofficialInfo.ApiClient
{
    internal static class JsonTransformator
    {
        public static JsonSerializerSettings TransformSettings(
            JsonSerializerSettings settings)
        {
            // Опубликованный openapi.json малость кривоват, многое помечено required, но возвращается null
            // Либа генерации прокси - тоже малость кривовата, т.к. для string считает явно указанный null
            // недопустимым, независимо от required для данного поля.
            // С задекларированным составом перечислений тоже беда
            // В общем, приходится патчить контракты на лету.
            settings.ContractResolver = new DeclarationPatcherContractResolver();

            return settings;
        }
    }
}
