using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Jellyfin.Plugin.Kinopoisk.Api;
using Jellyfin.Plugin.Kinopoisk.Api.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Vcr;
using Xunit;

namespace Jellyfin.Plugin.Kinopoisk.Tests
{
    public class KinopoiskApiTests
    {
        private const string ApiToken = "85d30ae5-d875-4c5f-900d-8e37bb20625e";
        private readonly ILoggerFactory _loggerFactory = NullLoggerFactory.Instance;
        private readonly VCR _vcr;
        private readonly Mock<IHttpClientFactory> _clientFactoryMock;
        public KinopoiskApiTests()
        {
            var dirInfo = new System.IO.DirectoryInfo("../../../cassettes"); //3 levels up to get to the root of the test project
            _vcr = new VCR(new FileSystemCassetteStorage(dirInfo));

            _clientFactoryMock = new Mock<IHttpClientFactory>();
            _clientFactoryMock
                .Setup(f => f.CreateClient(It.IsAny<string>()))
                .Returns(() =>
                {
                    var httpHandler = _vcr.GetVcrHandler();
                    httpHandler.InnerHandler = new HttpClientHandler();
                    return new HttpClient(httpHandler);
                });
        }

        private string GetMethodName([CallerMemberName] string memberName = "") => memberName;

        [Theory]
        [InlineData(1142206, "Пальма", "")]
        [InlineData(1044982, "Шпион, который меня кинул", "The Spy Who Dumped Me")]
        [InlineData(1445243, "Будь моим Кириллом", "")]
        public async Task GetSingleFilm_ShouldParseFilm(int filmId, string nameRu, string nameEn)
        {
            using (_vcr.UseCassette($"{GetMethodName()}_{filmId}", RecordMode.NewEpisodes))
            {
                var apiClient = new KinopoiskApiProxy(ApiToken, _loggerFactory.CreateLogger<KinopoiskApiProxy>(), _clientFactoryMock.Object);

                var res = await apiClient.GetSingleFilm(filmId);

                Assert.Equal(FilmType.Film, res.Data.Type);
                Assert.Equal(nameRu, res.GetLocalName());
                Assert.Equal(nameEn, res.GetOriginalNameIfNotSame());
            }
        }

        [Theory]
        [InlineData(4416198, "В активном поиске", "")]
        [InlineData(77298, "Вавилон 5", "Babylon 5")]
        public async Task GetSingleFilm_ShouldParseTvShow(int filmId, string nameRu, string nameEn)
        {
            using (_vcr.UseCassette($"{GetMethodName()}_{filmId}", RecordMode.NewEpisodes))
            {
                var apiClient = new KinopoiskApiProxy(ApiToken, _loggerFactory.CreateLogger<KinopoiskApiProxy>(), _clientFactoryMock.Object);

                var res = await apiClient.GetSingleFilm(filmId);

                Assert.Equal(FilmType.TvShow, res.Data.Type);
                Assert.Equal(nameRu, res.GetLocalName());
                Assert.Equal(nameEn, res.GetOriginalNameIfNotSame());
            }
        }

        [Theory]
        [InlineData(89540, 81, ProfessionEnum.ProducerUssr)]
        public async Task GetSingleFilm_ShouldParseProducerUssr(int filmId, int index, ProfessionEnum profession)
        {
            using (_vcr.UseCassette($"{GetMethodName()}_{filmId}", RecordMode.NewEpisodes))
            {
                var apiClient = new KinopoiskApiProxy(ApiToken, _loggerFactory.CreateLogger<KinopoiskApiProxy>(), _clientFactoryMock.Object);

                var res = await apiClient.GetStaff(filmId);

                Assert.Equal(profession, res.ToArray()[index].ProfessionKey);
            }
        }
    }
}
