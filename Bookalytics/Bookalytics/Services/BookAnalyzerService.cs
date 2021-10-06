using Bookalytics.Services.Contracts;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bookalytics.Services
{
    public class BookAnalyzerService : IBookAnalyzerService
    {
        private readonly List<string> words;
        private readonly ConcurrentDictionary<string, int> wordsAppearances;
        public BookAnalyzerService()
        {
            wordsAppearances = new ConcurrentDictionary<string, int>();
            words = GetWords();
            WordsAppearances();
        }
        public ConcurrentDictionary<string, int> GetDict() => this.wordsAppearances;

        public string BookText { get; set; }

        public void GetText(string bookText)
        {
            this.BookText = bookText;
        }
        public double AverageWordLength()
        {
            return words.Select(x => x.Length).Average();
        }

        //Alphabetically
        public string GetLeastCommonWord()
        {
            var word = this.wordsAppearances.OrderBy(x => x.Value).ThenBy(x => x.Key).FirstOrDefault().Key;

            return word;
        }

        public int GetLeastCommonWordCount(string word)
        {
            var count = wordsAppearances[word];

            return count;
        }

        public string GetMostCommonWord()
        {
            var word = this.wordsAppearances.OrderByDescending(x => x.Value).ThenBy(x => x.Key).FirstOrDefault().Key;

            return word;
        }

        public int GetMostCommonWordCount(string word)
        {
            var count = wordsAppearances[word];

            return count;
        }

        public string GetLongestWord()
        {
            var word = this.wordsAppearances.Keys.OrderByDescending(x => x.Length).ThenBy(x => x).FirstOrDefault();

            return word;
        }
        public string GetShortestWord()
        {
            var word = this.wordsAppearances.Keys.OrderBy(x => x.Length).ThenBy(x=>x).FirstOrDefault();

            return word;
        }

        public int GetWordsCount()
        {
            var count = words.Count();

            return count;
        }

        private List<string> GetWords() => BookSerializer.ReturnWordsToUpper(this.BookText);

        private void WordsAppearances()
        {
            Parallel.ForEach(words, (currentWord) =>
            {
                wordsAppearances.AddOrUpdate(currentWord, 1, (string x, int y) => y + 1);
            });
        }
    }
}
