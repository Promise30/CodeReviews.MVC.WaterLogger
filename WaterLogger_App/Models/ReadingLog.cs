using System.ComponentModel.DataAnnotations;

namespace HabitLogger_App.Models
{
    public class ReadingLog
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        [Range(0, Int32.MaxValue, ErrorMessage = "Value for {0} must be positive.")]
        public int PagesRead { get; set; }
        [Range(0, Int32.MaxValue, ErrorMessage = "Value for {0} must be positive.")]
        public int MinutesRead { get; set; }
        [Required]
        [Display(Name = "Book")]
        public int BookId { get; set; }
        public virtual Book? Book { get; set; } 

    }
}
