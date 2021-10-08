using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Bookalytics.Services
{
    public static class BookSerializer
    {
        private const string Pattern = @"[+-]?\d+[\p{L}’-]*|[\p{L}\d’-]+|[„“][\p{L}\d’-]+[„“]-[\p{L}’-]+";
        private static List<string> words;

        public static void SplitWords(string text)
        {
            words = Regex.Matches(text, Pattern).Select(x => x.Value).ToList();
        }

        public static List<string> ReturnWordsToUpper(string text)
        {
            SplitWords(text);

            return words.Select(x => x.ToUpperInvariant()).ToList();
        }
    }
}
