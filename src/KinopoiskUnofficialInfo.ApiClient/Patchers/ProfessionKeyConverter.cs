using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace KinopoiskUnofficialInfo.ApiClient
{
    public class ProfessionKeyConverter : StringEnumConverter
    {
        public override bool CanConvert(System.Type objectType)
            => objectType == typeof(PersonResponse_filmsProfessionKey);

        public override object ReadJson(JsonReader reader, System.Type objectType, object existingValue, JsonSerializer serializer)
        {
            try
            {
                return base.ReadJson(reader, objectType, existingValue, serializer);
            }
            catch (Exception)
            {
                return PersonResponse_filmsProfessionKey.UNKNOWN;
            }
        }
    }
}
