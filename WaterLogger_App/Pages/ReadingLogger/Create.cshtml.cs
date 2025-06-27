using HabitLogger_App.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.Sqlite;

namespace HabitLogger_App.Pages.ReadingLogger
{
    public class CreateModel : PageModel
    {
        private readonly ILogger<CreateModel> _logger;
        private readonly IConfiguration _configuration;

        public CreateModel(IConfiguration configuration, ILogger<CreateModel> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        [BindProperty]
        public ReadingLog ReadingLog { get; set; }
        public SelectList BookSelectList { get; set; }
        public List<Book> Books { get; set; } = new List<Book>();

        public IActionResult OnGet()
        {
            Books = GetBooks();
            BookSelectList = new SelectList(Books, "Id", "Title");
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
                tableCommand.CommandText = "SELECT COUNT(*) FROM reading_log WHERE Date = @date AND BookId = @bookId";
                tableCommand.Parameters.AddWithValue("@date", ReadingLog.Date.ToString());
                tableCommand.Parameters.AddWithValue("@bookId", ReadingLog.BookId);
                var count = Convert.ToInt32(tableCommand.ExecuteScalar());
                if (count > 0)
                {
                    ModelState.AddModelError(string.Empty, "A reading log for this date and book already exists.");
                    return Page();
                }
                var insertCommand = connection.CreateCommand();

                insertCommand.CommandText = "INSERT INTO reading_log (Date, PagesRead, MinutesRead, BookId) VALUES (@date, @pagesRead, @minutesRead, @bookId)";
                insertCommand.Parameters.AddWithValue("@date", ReadingLog.Date.ToString());
                insertCommand.Parameters.AddWithValue("@pagesRead", ReadingLog.PagesRead);
                insertCommand.Parameters.AddWithValue("@minutesRead", ReadingLog.MinutesRead);
                insertCommand.Parameters.AddWithValue("@bookId", ReadingLog.BookId);
                try
                {
                    insertCommand.ExecuteNonQuery();
                    return RedirectToPage("/Books/Index");

                }
                catch (Exception)
                {
                    ModelState.AddModelError(string.Empty, "An error occurred while saving the reading log. Please try again.");
                    _logger.LogError("Error saving reading log: {Message}", "An error occurred while saving the reading log.");
                    return Page();
                }

            }
        }
        private List<Book> GetBooks()
        {
            using (var connection = new SqliteConnection(_configuration.GetConnectionString("ConnectionString")))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM Books";
                using (var reader = command.ExecuteReader())
                {
                    var books = new List<Book>();
                    while (reader.Read())
                    {
                        var book = new Book
                        {
                            Id = reader.GetInt32(0),
                            Title = reader.GetString(1),
                            Author = reader.GetString(2)
                        };
                        books.Add(book);
                    }
                    return books;
                }
            }
        }
    }
}
