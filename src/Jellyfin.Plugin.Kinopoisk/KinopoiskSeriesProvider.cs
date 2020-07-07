using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Jellyfin.Plugin.Kinopoisk.ApiModel;
using MediaBrowser.Common.Net;
using MediaBrowser.Controller.Entities.TV;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Providers;
using MediaBrowser.Model.Serialization;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.Kinopoisk
{
    public class KinopoiskSeriesProvider : KinopoiskVideoBaseProvider<Series, SeriesInfo>
    {
        public KinopoiskSeriesProvider(ILogger<KinopoiskSeriesProvider> logger,
                                       IHttpClient httpClient,
                                       IJsonSerializer jsonSerializer)
            : base(logger, httpClient, jsonSerializer)
        {

        }

        protected override Series ConvertResponseToItem(FilmDetails apiResponse)
            => apiResponse.ToSeries();
    }
}
