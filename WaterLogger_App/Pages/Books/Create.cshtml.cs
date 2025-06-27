using HabitLogger_App.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.Sqlite;

namespace HabitLogger_App.Pages.Books
{
    public class CreateModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public CreateModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [BindProperty]
        public Book Book { get; set; } = new Book();
        public void OnGet()
        {
        }
        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            using (var connection = new SqliteConnection(_configuration.GetConnectionString("ConnectionString")))
            {
                connection.Open();
                var tableCommand = connection.CreateCommand();
                tableCommand.CommandText = "INSERT INTO books (title, author, rating) VALUES (@title, @author, @rating)";
                tableCommand.Parameters.AddWithValue("@title", Book.Title);
                tableCommand.Parameters.AddWithValue("@author", Book.Author ?? (object)DBNull.Value);
                tableCommand.Parameters.AddWithValue("@rating", Book.Rating ?? (object)DBNull.Value);
                try
                {
                    tableCommand.ExecuteNonQuery();
                    return RedirectToPage("/Books/Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, "An error occurred while saving the book: " + ex.Message);
                    return Page();
                }

            }
        }
    }
}
