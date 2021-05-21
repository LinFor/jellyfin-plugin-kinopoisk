using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Jellyfin.Plugin.Kinopoisk.ApiModel;
using Xunit;

namespace Jellyfin.Plugin.Kinopoisk.Tests
{
    public class DeserializationTests
    {
        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);

        public DeserializationTests() {
            this._jsonOptions.Converters.Add(new JsonStringEnumMemberConverter());
        }

        private Stream GetTestDataStream(string name) {
            var assembly = typeof(DeserializationTests).GetTypeInfo().Assembly;
            return assembly.GetManifestResourceStream($"Jellyfin.Plugin.Kinopoisk.Tests.Data.{name}");
        }

        [Fact]
        public async Task FilmDetails_ShouldParseFilm()
        {
            using var testData = GetTestDataStream("response_1616059004666.json");
            var res = await JsonSerializer.DeserializeAsync<FilmDetails>(testData, _jsonOptions).ConfigureAwait(false);
            Assert.Equal(FilmType.Film, res.Data.Type);
        }

        [Fact]
        public async Task FilmDetails_ShouldParseTvShow()
        {
            using var testData = GetTestDataStream("response_1616058514535.json");
            var res = await JsonSerializer.DeserializeAsync<FilmDetails>(testData, _jsonOptions).ConfigureAwait(false);
            Assert.Equal(FilmType.TvShow, res.Data.Type);
        }

        [Fact]
        public async Task StaffItem_ShouldParseProducerUssr()
        {
            using var testData = GetTestDataStream("response_1616062293018.json");
            var res = await JsonSerializer.DeserializeAsync<IEnumerable<StaffItem>>(testData, _jsonOptions).ConfigureAwait(false);
            Assert.Equal(ProfessionEnum.ProducerUssr, res.ToArray()[81].ProfessionKey);
        }
    }
}
