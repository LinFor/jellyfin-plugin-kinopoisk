using System;
using Newtonsoft.Json;

namespace KinopoiskUnofficialInfo.ApiClient
{
    public class BadIntegerConverter : JsonConverter
    {
        public override bool CanRead => true;
        public override bool CanWrite => false;
        public override bool CanConvert(System.Type objectType)
            => objectType == typeof(Int32);

        public override object ReadJson(JsonReader reader, System.Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (int.TryParse(Convert.ToString(reader.Value), out var result))
                return result;

            return -1;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
