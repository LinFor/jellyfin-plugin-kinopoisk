// Copyright: https://calmsen.ru/transliteraciya-na-c
// Copyright: http://usanov.net/748-transliteraciya-rus-2-lat-na-c

using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Jellyfin.Plugin.Kinopoisk
{
    public static class TransliterationStringExtensions
    {
        //ISO 9-95
        private static Dictionary<string, string> _replaceDictionary = new()
        {
                { "Є", "Ye" },
                { "І", "I" },
                { "Ѓ", "G" },
                { "і", "i" },
                { "№", "#" },
                { "є", "ye" },
                { "ѓ", "g" },
                { "А", "A" },
                { "Б", "B" },
                { "В", "V" },
                { "Г", "G" },
                { "Д", "D" },
                { "Е", "E" },
                { "Ё", "Yo" },
                { "Ж", "Zh" },
                { "З", "Z" },
                { "И", "I" },
                { "Й", "J" },
                { "К", "K" },
                { "Л", "L" },
                { "М", "M" },
                { "Н", "N" },
                { "О", "O" },
                { "П", "P" },
                { "Р", "R" },
                { "С", "S" },
                { "Т", "T" },
                { "У", "U" },
                { "Ф", "F" },
                { "Х", "X" },
                { "Ц", "C" },
                { "Ч", "Ch" },
                { "Ш", "Sh" },
                { "Щ", "Shh" },
                { "Ъ", "'" },
                { "Ы", "Y" },
                { "Ь", "" },
                { "Э", "E" },
                { "Ю", "Yu" },
                { "Я", "Ya" },
                { "а", "a" },
                { "б", "b" },
                { "в", "v" },
                { "г", "g" },
                { "д", "d" },
                { "е", "e" },
                { "ё", "yo" },
                { "ж", "zh" },
                { "з", "z" },
                { "и", "i" },
                { "й", "j" },
                { "к", "k" },
                { "л", "l" },
                { "м", "m" },
                { "н", "n" },
                { "о", "o" },
                { "п", "p" },
                { "р", "r" },
                { "с", "s" },
                { "т", "t" },
                { "у", "u" },
                { "ф", "f" },
                { "х", "x" },
                { "ц", "c" },
                { "ч", "ch" },
                { "ш", "sh" },
                { "щ", "shh" },
                { "ъ", "" },
                { "ы", "y" },
                { "ь", "" },
                { "э", "e" },
                { "ю", "yu" },
                { "я", "ya" },
            };

        public static string TransliterateToLatin(this string text)
        {
            string output = text;

            output = Regex.Replace(output, @"\s|\.|\(", " ");
            output = Regex.Replace(output, @"\s+", " ");
            output = Regex.Replace(output, @"[^\s\w\d-]", "");
            output = output.Trim();

            foreach (KeyValuePair<string, string> key in _replaceDictionary)
            {
                output = output.Replace(key.Key, key.Value);
            }
            return output;
        }

        public static string TransliterateToCyrillic(this string text)
        {
            string output = text;

            foreach (KeyValuePair<string, string> key in _replaceDictionary)
            {
                output = output.Replace(key.Value, key.Key);
            }
            return output;
        }
    }
}
