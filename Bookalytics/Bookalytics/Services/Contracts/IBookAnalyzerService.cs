using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bookalytics.Services.Contracts
{
    public interface IBookAnalyzerService
    {
        public void GetText(string bookText);
        public ConcurrentDictionary<string, int> GetDict();
        public int GetWordsCount();
        public string GetShortestWord();
        public string GetLongestWord();
        public string GetMostCommonWord();
        public int GetMostCommonWordCount(string word);
        public string GetLeastCommonWord();
        public int GetLeastCommonWordCount(string word);
        public double AverageWordLength();
    }
}
