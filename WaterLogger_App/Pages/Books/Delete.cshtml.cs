using HabitLogger_App.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.Sqlite;

namespace HabitLogger_App.Pages.Books
{
    public class DeleteModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public DeleteModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [BindProperty]
        public Book Book { get; set; } = new Book();
        public IActionResult OnGet(int id)
        {
            Book = GetBook(id);
            return Page();
        }
        private Book GetBook(int id)
        {
            var book = new Book();
            using (var connection = new SqliteConnection(_configuration.GetConnectionString("ConnectionString")))
            {
                connection.Open();
                var tableCommand = connection.CreateCommand();
                tableCommand.CommandText = "SELECT * FROM books WHERE id = @id";
                tableCommand.Parameters.AddWithValue("@id", id);
                SqliteDataReader reader = tableCommand.ExecuteReader();
                while (reader.Read())
                {
                    book.Id = reader.GetInt32(0);
                    book.Title = reader.GetString(1);
                    book.Author = reader.IsDBNull(2) ? null : reader.GetString(2);
                    book.Rating = reader.IsDBNull(3) ? null : (int?)reader.GetInt32(3);
                }
                return book;
            }
        }
        public IActionResult OnPost(int id)
        {
            using (var connection = new SqliteConnection(_configuration.GetConnectionString("ConnectionString")))
            {
                connection.Open();

                // First, delete all reading logs for this book
                var deleteLogsCommand = connection.CreateCommand();
                deleteLogsCommand.CommandText = "DELETE FROM reading_log WHERE BookId = @id";
                deleteLogsCommand.Parameters.AddWithValue("@id", id);
                deleteLogsCommand.ExecuteNonQuery();

                // Then, delete the book itself
                var deleteBookCommand = connection.CreateCommand();
                deleteBookCommand.CommandText = "DELETE FROM books WHERE id = @id";
                deleteBookCommand.Parameters.AddWithValue("@id", id);
                deleteBookCommand.ExecuteNonQuery();
            }
            return RedirectToPage("/Books/Index");
        }

    }
}
