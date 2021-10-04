using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bookalytics.Services.Contracts
{
    public interface IBookAnalyzerService
    {
        public IDictionary<string, int> GetDict();
        public int GetWordsCount();
        public string GetShortestWord();
        public string GetLongestWord();
        public string GetMostCommonWord();
        public int GetMostCommonWordCount();
        public string GetLeastCommonWord();
        public int GetLeastCommonWordCount();
        public double AverageWordLength();
    }
}
