namespace Bookalytics.ViewModels
{
    public class BookViewModel
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Author { get; set; }

        public int? Year { get; set; }

        public string ImageUrl { get; set; }

        public string Text { get; set; }

        public int WordsCount { get; set; }

        public string ShortestWord { get; set; }

        public string LongestWord { get; set; }

        public string MostCommonWord { get; set; }

        public int MostCommonWordCount { get; set; }

        public string LeastCommonWord { get; set; }

        public int LeastCommonWordCount { get; set; }

        public double AverageWordLength { get; set; }
    }
}
