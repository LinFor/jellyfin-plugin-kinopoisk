using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace KinopoiskUnofficialInfo.ApiClient
{
    public class BadFilmSearchResponse_filmsTypeConverter : StringEnumConverter
    {
        public override bool CanConvert(System.Type objectType)
            => objectType == typeof(FilmSearchResponse_filmsType);

        public override object ReadJson(JsonReader reader, System.Type objectType, object existingValue, JsonSerializer serializer)
        {
            try
            {
                return base.ReadJson(reader, objectType, existingValue, serializer);
            }
            catch (Exception)
            {
                return FilmSearchResponse_filmsType.UNKNOWN;
            }
        }
    }
}
