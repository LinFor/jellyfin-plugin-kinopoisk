using Newtonsoft.Json;
using Xunit;

namespace KinopoiskUnofficialInfo.ApiClient.Tests
{
    public class BadSpouseSexConverterTests
    {
        [Fact]
        public void ShouldDeserializeUnknownSpouseSex()
        {
            const string json =
                "{\"personId\":1,\"name\":\"Test\",\"divorced\":false," +
                "\"divorcedReason\":\"\",\"sex\":\"UNKNOWN\",\"children\":0," +
                "\"webUrl\":\"\",\"relation\":\"\"}";

            var settings = new JsonSerializerSettings
            {
                ContractResolver = new DeclarationPatcherContractResolver()
            };

            var result = JsonConvert.DeserializeObject<PersonResponse_spouses>(
                json,
                settings);

            Assert.NotNull(result);
            Assert.Equal(
                (PersonResponse_spousesSex)(-1),
                result.Sex);
        }
    }
}
