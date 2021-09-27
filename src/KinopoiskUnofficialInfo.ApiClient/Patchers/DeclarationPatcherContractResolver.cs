using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace KinopoiskUnofficialInfo.ApiClient
{
    public class DeclarationPatcherContractResolver : DefaultContractResolver
    {
        private readonly ICollection<JsonConverter> _badConverters = new JsonConverter[] {
            new BadIntegerConverter(),
            new BadDoubleConverter(),
            new BadProfessionKeyConverter(),
            new BadProductionStatusConverter(),
            new BadFilmSearchResponse_filmsTypeConverter(),
        };

        protected override IList<JsonProperty> CreateProperties(System.Type type, MemberSerialization memberSerialization)
        {
            var list = base.CreateProperties(type, memberSerialization);
            foreach (var property in list)
            {
                foreach (var badConverter in _badConverters)
                {
                    if (badConverter.CanConvert(property.PropertyType))
                    {
#pragma warning disable CS0618
                        property.MemberConverter = null;
#pragma warning restore CS0618
                        property.Converter = badConverter;

                        property.Required = Required.Default;
                        // property.DefaultValue = -1;
                        break;
                    }
                }

                if (property.PropertyType == typeof(string))
                {
                    property.Required = Required.Default;
                }
            }
            return list;
        }
    }
}
