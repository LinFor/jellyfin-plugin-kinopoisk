using System;
using Xunit;

namespace Jellyfin.Plugin.Kinopoisk.Tests
{
    public class UtilsTests
    {
        [Fact]
        public void GetFirstYear_ShouldParseYearsSpan()
        {
            var inp = "2014-2017";
            var res = Utils.GetFirstYear(inp);
            Assert.Equal(2014, res);
        }
    }
}
