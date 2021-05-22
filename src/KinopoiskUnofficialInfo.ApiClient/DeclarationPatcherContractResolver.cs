using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace KinopoiskUnofficialInfo.ApiClient
{
    public class DeclarationPatcherContractResolver : DefaultContractResolver
    {
        protected override IList<JsonProperty> CreateProperties(System.Type type, MemberSerialization memberSerialization)
        {
            var list = base.CreateProperties(type, memberSerialization);
            foreach (var property in list)
            {
                if (property.PropertyType == typeof(string))
                {
                    property.Required = Required.Default;
                }

                if (property.PropertyType == typeof(PersonResponse_filmsProfessionKey))
                {
#pragma warning disable CS0618
                    property.MemberConverter = null;
#pragma warning restore CS0618
                    property.Converter = new JsonProfessionKeyConverter();
                }

                if (type == typeof(VideoResponse_trailers))
                {
                    if (property.PropertyName == "size")
                    {
                        property.Required = Required.Default;
                        property.DefaultValue = -1;
                    }
                }
            }
            return list;
        }
    }
}
