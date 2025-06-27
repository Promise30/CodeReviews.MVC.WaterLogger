using HabitLogger_App.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.Sqlite;
using System;
using System.Globalization;

namespace HabitLogger_App.Pages.ReadingLogger
{
    public class DetailsModel : PageModel
    {
        private readonly IConfiguration _configuration;
        [BindProperty]
        public ReadingLog ReadingLog { get; set; } = new ReadingLog();
        public List<ReadingLog> ReadingLogsList { get; set; } = new List<ReadingLog>();

        public DetailsModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult OnGet(int bookId)
        {
            ReadingLogsList = ReadingLogs(bookId);
            if (ReadingLogsList == null || ReadingLogsList.Count == 0)
            {
                ModelState.AddModelError(string.Empty, "No reading logs found for this book.");
                return Page();
            }
            return Page();
        }
        private List<ReadingLog> ReadingLogs(int bookId)
        {
            var readingLogs = new List<ReadingLog>();
            using (var connection = new SqliteConnection(_configuration.GetConnectionString("ConnectionString")))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText =
                    @"
                        SELECT rl.Id, rl.Date, rl.PagesRead, rl.MinutesRead, rl.BookId , b.Title
                        FROM reading_log rl
                        INNER JOIN Books b ON rl.BookId = b.Id
                        WHERE rl.BookId = @bookId
                    ";
                command.Parameters.AddWithValue("@bookId", bookId);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var log = new ReadingLog
                        {
                            Id = reader.GetInt32(0),
                            Date = DateTime.Parse(reader.GetString(1)),
                            PagesRead = reader.GetInt32(2),
                            MinutesRead = reader.GetInt32(3),
                            BookId = reader.GetInt32(4),
                            Book = new Book
                            {
                                Id = reader.GetInt32(4),
                                Title = reader.GetString(5)
                            }
                        };
                        readingLogs.Add(log);
                    }
                }
            }
            return readingLogs;

        }
    }
}
