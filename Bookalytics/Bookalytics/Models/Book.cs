using System.ComponentModel.DataAnnotations;

namespace Bookalytics.Data.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }

        [Required]
        public string Author { get; set; }

        public int? Year { get; set; }

        public string ImageUrl { get; set; }

        [Required]
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
