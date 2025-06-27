using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.Sqlite;
using System.Globalization;
using HabitLogger_App.Models;

namespace HabitLogger_App.Pages.WaterLogger
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IConfiguration _configuration;
        public List<DrinkingWater> Records { get; set; } = new List<DrinkingWater>();

        public IndexModel(ILogger<IndexModel> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }
        public void OnGet()
        {
            Records = GetAllRecords();
            ViewData["Total"] = Records.AsEnumerable().Sum(x => x.Quantity);

        }
        private List<DrinkingWater> GetAllRecords()
        {
            using (var connection = new SqliteConnection(_configuration.GetConnectionString("ConnectionString")))
            {
                connection.Open();
                var tableCommand = connection.CreateCommand();
                tableCommand.CommandText = "SELECT * FROM drinking_water";
                var tableData = new List<DrinkingWater>();
                SqliteDataReader reader = tableCommand.ExecuteReader();
                while (reader.Read())
                {
                    tableData.Add(
                        new DrinkingWater
                        {
                            Id = reader.GetInt32(0),
                            Date = DateTime.ParseExact(reader.GetString(1), "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture),
                            Quantity = reader.GetDecimal(2),
                            ContainerType = reader.GetString(3)
                        }
                    ); 
                };
                return tableData;
            }
        }
    }
}
