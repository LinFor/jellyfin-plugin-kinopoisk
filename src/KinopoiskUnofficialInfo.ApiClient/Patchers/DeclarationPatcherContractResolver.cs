using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace KinopoiskUnofficialInfo.ApiClient
{
    public class DeclarationPatcherContractResolver : DefaultContractResolver
    {
        private readonly JsonConverter _integerConverter = new IntegerOverflowConverter();

        private readonly ICollection<(System.Type, string)> _allowNull = new[] {
            (typeof(CommonFilmData), "ratingAgeLimits"),
            (typeof(Budget), null),
        };

        private readonly ICollection<(System.Type, string, JsonConverter)> _patchConverters = new[] {
            (typeof(object), (string)null, (JsonConverter)new ProfessionKeyConverter()),
        };

        protected override IList<JsonProperty> CreateProperties(System.Type type, MemberSerialization memberSerialization)
        {
            var list = base.CreateProperties(type, memberSerialization);
            foreach (var property in list)
            {
                if (_allowNull.Any((config) => type == config.Item1
                    && (string.IsNullOrEmpty(config.Item2) || property.PropertyName == config.Item2)))
                {
                    property.Required = Required.Default;
                    property.DefaultValue = -1;

                    if (_integerConverter.CanConvert(property.PropertyType))
                    {
#pragma warning disable CS0618
                        property.MemberConverter = null;
#pragma warning restore CS0618
                        property.Converter = _integerConverter;
                    }
                }

                var patchConverters = _patchConverters
                    .Where((config) => (config.Item1 == typeof(object) || type == config.Item1)
                        && (string.IsNullOrEmpty(config.Item2) || property.PropertyName == config.Item2))
                    .ToArray();
                foreach (var patchConverter in patchConverters)
                {
                    if (patchConverter.Item3.CanConvert(property.PropertyType))
                    {
#pragma warning disable CS0618
                        property.MemberConverter = null;
#pragma warning restore CS0618
                        property.Converter = patchConverter.Item3;
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
