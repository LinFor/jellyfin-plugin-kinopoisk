using System;
using Newtonsoft.Json;

namespace KinopoiskUnofficialInfo.ApiClient
{
    public class BadDoubleConverter : JsonConverter
    {
        public override bool CanRead => true;
        public override bool CanWrite => false;
        public override bool CanConvert(System.Type objectType)
            => objectType == typeof(double);

        public override object ReadJson(JsonReader reader, System.Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (double.TryParse(Convert.ToString(reader.Value), out var result))
                return result;

            return 0.0d;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
