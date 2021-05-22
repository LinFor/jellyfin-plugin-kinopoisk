using Xunit;

namespace Jellyfin.Plugin.Kinopoisk.Tests
{
    public class TransliterationTests
    {
        [Theory]
        [InlineData("Илья Куликов", "Ilya Kulikov")]
        [InlineData("Ирина Старшенбаум", "Irina Starshenbaum")]
        public void ShouldTransliterateToLatin(string text, string expected)
        {
            Assert.Equal(expected, text.TransliterateToLatin());
        }
    }
}
