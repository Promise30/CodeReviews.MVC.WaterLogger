using HabitLogger_App.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.Sqlite;

namespace HabitLogger_App.Pages.Books
{
    public class IndexModel : PageModel
    {
        private readonly IConfiguration _configuration;
        public List<Book> Books { get; set; } = new List<Book>();

        public IndexModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        [BindProperty]
        public Book Book { get; set; } = new Book();

        public void OnGet()
        {
            Books = GetAllBooks();
        }
        private List<Book> GetAllBooks()
        {
            using (var connection = new SqliteConnection(_configuration.GetConnectionString("ConnectionString")))
            {
                connection.Open();
                var tableCommand = connection.CreateCommand();
                tableCommand.CommandText = "SELECT * FROM books";
                var tableData = new List<Book>();
                SqliteDataReader reader = tableCommand.ExecuteReader();
                while (reader.Read())
                {
                    tableData.Add(
                        new Book
                        {
                            Id = reader.GetInt32(0),
                            Title = reader.GetString(1),
                            Author = reader.IsDBNull(2) ? null : reader.GetString(2),
                            Rating = reader.IsDBNull(3) ? null : (int?)reader.GetInt32(3)
                        }
                    );
                }
                return tableData;
            }
        }
    }
}
