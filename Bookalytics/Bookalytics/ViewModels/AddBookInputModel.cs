using System.ComponentModel.DataAnnotations;

namespace Bookalytics.ViewModels
{
    public class AddBookInputModel
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string Author { get; set; }

        public int? Year { get; set; }

        public string ImageUrl { get; set; }

        [Required]
        public string Text { get; set; }

    }
}
