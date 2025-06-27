using HabitLogger_App.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.Sqlite;

namespace HabitLogger_App.Pages.Books
{
    public class UpdateModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public UpdateModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [BindProperty]
        public Book Book { get; set; } = new Book();
        public void OnGet(int id)
        {
            Book = GetBook(id);
            if (Book == null || Book.Id == 0)
            {
                ModelState.AddModelError(string.Empty, "Book not found.");
                return;
            }
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
                tableCommand.CommandText = "UPDATE books SET title = @title, author = @author, rating = @rating WHERE id = @id";
                tableCommand.Parameters.AddWithValue("@id", Book.Id);
                tableCommand.Parameters.AddWithValue("@title", Book.Title);
                tableCommand.Parameters.AddWithValue("@author", Book.Author ?? (object)DBNull.Value);
                tableCommand.Parameters.AddWithValue("@rating", Book.Rating ?? (object)DBNull.Value);
                try
                {
                    tableCommand.ExecuteNonQuery();
                    return RedirectToPage("./Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, "An error occurred while updating the book: " + ex.Message);
                    return Page();
                }
            }

        }
        private Book GetBook(int id)
        {
            var book = new Book();
            using(var connection = new SqliteConnection(_configuration.GetConnectionString("ConnectionString")))
            {
                connection.Open();
                var tableCommand = connection.CreateCommand();
                tableCommand.CommandText = "SELECT * FROM books WHERE id = @id";
                tableCommand.Parameters.AddWithValue("@id", id);
                SqliteDataReader reader = tableCommand.ExecuteReader();
                if (reader.Read())
                {
                    book.Id = reader.GetInt32(0);
                    book.Title = reader.GetString(1);
                    book.Author = reader.IsDBNull(2) ? null : reader.GetString(2);
                    book.Rating = reader.IsDBNull(3) ? null : (int?)reader.GetInt32(3);
                }
            }
            return book;

        }
    }
}
