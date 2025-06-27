using HabitLogger_App.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.Sqlite;
using System.Globalization;

namespace HabitLogger_App.Pages.ReadingLogger
{
    public class DeleteModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public DeleteModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [BindProperty]
        public ReadingLog ReadingLog { get; set; } = new ReadingLog();
        public IActionResult OnGet(int id)
        {

            ReadingLog = GetReadingLog(id);
            if (ReadingLog == null)
            {
                ModelState.AddModelError(string.Empty, "Reading log not found.");
                return Page();
            }
            return Page();
        }
        public IActionResult OnPost(int id)
        {
            using (var connection = new SqliteConnection(_configuration.GetConnectionString("ConnectionString")))
            {
                connection.Open();
                var tableCommand = connection.CreateCommand();
                tableCommand.CommandText = "DELETE FROM reading_log WHERE id = @id";
                tableCommand.Parameters.AddWithValue("@id", id);
                tableCommand.ExecuteNonQuery();
            }
            return RedirectToPage("/ReadingLogger/Details", new { bookId = ReadingLog.BookId });
        }
        private ReadingLog GetReadingLog(int id)
        {
            var readingLog = new ReadingLog();
            using (var connection = new SqliteConnection(_configuration.GetConnectionString("ConnectionString")))
            {
                connection.Open();
                var tableCommand = connection.CreateCommand();
                tableCommand.CommandText = "SELECT * FROM reading_log WHERE id = @id";
                tableCommand.Parameters.AddWithValue("@id", id);
                SqliteDataReader reader = tableCommand.ExecuteReader();
                while (reader.Read())
                {
                    readingLog.Id = reader.GetInt32(0);
                    readingLog.BookId = reader.GetInt32(1);
                    readingLog.Date = DateTime.Parse(reader.GetString(2));
                    readingLog.PagesRead = reader.GetInt32(3);
                    readingLog.MinutesRead = reader.GetInt32(4);
                }
            }
            return readingLog;
        }

    }
}
