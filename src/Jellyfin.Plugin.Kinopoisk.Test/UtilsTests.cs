using System;
using Xunit;

namespace Jellyfin.Plugin.Kinopoisk.Tests
{
    public class UtilsTests
    {
        [Theory]
        [InlineData("2015", 2015)]
        [InlineData("2014-2017", 2014)]
        [InlineData("2014-20317", 2014)]
        [InlineData("20314-2017", null)]
        [InlineData("2020-...", 2020)]
        public void GetFirstYear_ShouldParseYearsSpan(string src, int? expected)
        {
            var res = ApiModelExtensions.GetFirstYear(src);
            Assert.Equal(expected, res);
        }

        [Theory]
        [InlineData("2015", false)]
        [InlineData("2014-2017", false)]
        [InlineData("2020-...", true)]
        public void IsСontinues_ShouldParseYearsSpan(string src, bool expected)
        {
            var res = ApiModelExtensions.IsСontinuing(src);
            Assert.Equal(expected, res);
        }

        [Theory]
        [InlineData("2015", 2015)]
        [InlineData("2014-2017", 2017)]
        [InlineData("2014-20317", null)]
        [InlineData("20314-2017", 2017)]
        [InlineData("2020-...", null)]
        public void GetLastYear_ShouldFindContinuing(string src, int? expected)
        {
            var res = ApiModelExtensions.GetLastYear(src);
            Assert.Equal(expected, res);
        }
    }
}
