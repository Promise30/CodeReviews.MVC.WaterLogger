using HabitLogger_App.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.Sqlite;
using System.Globalization;

namespace HabitLogger_App.Pages.ReadingLogger
{
    public class UpdateModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public UpdateModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [BindProperty]
        public ReadingLog ReadingLog { get; set; } = new ReadingLog();
        public SelectList BookSelectList { get; set; }
        public List<Book> Books { get; set; } = new List<Book>();

        public IActionResult OnGet(int id)
        {
            Books = GetBooks();
            BookSelectList = new SelectList(Books, "Id", "Title");

            ReadingLog = GetReadingLog(id);

            if (ReadingLog == null || ReadingLog.Id == 0)
            {
                ModelState.AddModelError(string.Empty, "Reading log not found.");
                return Page();
            }

            return Page();
        }

        public IActionResult OnPost()
        {
            Books = GetBooks();
            BookSelectList = new SelectList(Books, "Id", "Title");

            if (!ModelState.IsValid)
            {
                return Page();
            }

            using (var connection = new SqliteConnection(_configuration.GetConnectionString("ConnectionString")))
            {
                connection.Open();
                var tableCommand = connection.CreateCommand();
                tableCommand.CommandText = "UPDATE reading_log SET Date = @date, PagesRead = @pagesRead, MinutesRead = @minutesRead, BookId = @bookId WHERE Id = @id";
                tableCommand.Parameters.AddWithValue("@id", ReadingLog.Id);
                tableCommand.Parameters.AddWithValue("@bookId", ReadingLog.BookId);
                tableCommand.Parameters.AddWithValue("@date", ReadingLog.Date.ToString("yyyy-MM-dd"));
                tableCommand.Parameters.AddWithValue("@pagesRead", ReadingLog.PagesRead);
                tableCommand.Parameters.AddWithValue("@minutesRead", ReadingLog.MinutesRead);

                try
                {
                    tableCommand.ExecuteNonQuery();
                    return RedirectToPage("/ReadingLogger/Details", new { bookId = ReadingLog.BookId });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, "An error occurred while updating the reading log: " + ex.Message);
                    return Page();
                }
            }
        }

        private ReadingLog GetReadingLog(int id)
        {
            ReadingLog? readingLog = null;
            using (var connection = new SqliteConnection(_configuration.GetConnectionString("ConnectionString")))
            {
                connection.Open();
                var tableCommand = connection.CreateCommand();
                tableCommand.CommandText = "SELECT Id, Date, PagesRead, MinutesRead, BookId FROM reading_log WHERE Id = @id";
                tableCommand.Parameters.AddWithValue("@id", id);
                using (var reader = tableCommand.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        readingLog = new ReadingLog
                        {
                            Id = reader.GetInt32(0),
                            Date = DateTime.Parse(reader.GetString(1)),
                            PagesRead = reader.GetInt32(2),
                            MinutesRead = reader.GetInt32(3),
                            BookId = reader.GetInt32(4)
                        };
                    }
                }
            }
            return readingLog ?? new ReadingLog();
        }

        private List<Book> GetBooks()
        {
            var books = new List<Book>();
            using (var connection = new SqliteConnection(_configuration.GetConnectionString("ConnectionString")))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT Id, Title, Author FROM Books";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        books.Add(new Book
                        {
                            Id = reader.GetInt32(0),
                            Title = reader.GetString(1),
                            Author = reader.IsDBNull(2) ? null : reader.GetString(2)
                        });
                    }
                }
            }
            return books;
        }
    }
}
