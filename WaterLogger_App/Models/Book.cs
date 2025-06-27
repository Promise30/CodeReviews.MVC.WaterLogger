using System.ComponentModel.DataAnnotations;

namespace HabitLogger_App.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Author { get; set; }
        [Range(0, 5, ErrorMessage = "Rating must be between 0 and 5.")]
        public int? Rating { get; set; }

    }
}
