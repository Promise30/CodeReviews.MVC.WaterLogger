using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.Sqlite;
using System.Globalization;
using HabitLogger_App.Models;

namespace HabitLogger_App.Pages.WaterLogger
{
    public class DeleteModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public DeleteModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        [BindProperty]
        public DrinkingWater DrinkingWater { get; set; }

        public IActionResult OnGet(int id)
        {
            DrinkingWater = GetById(id);
            return Page();
        }

        private DrinkingWater GetById(int id)
        {
            var drinkingWaterRecord = new DrinkingWater();
            using (var connection = new SqliteConnection(_configuration.GetConnectionString("ConnectionString")))
            {
                connection.Open();
                var tableCommand = connection.CreateCommand();
                tableCommand.CommandText = $"SELECT * FROM drinking_water WHERE Id = {id}";
                SqliteDataReader reader = tableCommand.ExecuteReader();
                while (reader.Read())
                {
                    drinkingWaterRecord.Id = reader.GetInt32(0);
                    drinkingWaterRecord.Date = DateTime.ParseExact(reader.GetString(1), "dd/MM/yyyy HH:mm:ss", CultureInfo.CurrentUICulture.DateTimeFormat);
                    drinkingWaterRecord.Quantity = reader.GetDecimal(2);
                }
                return drinkingWaterRecord;
            }
        }
        
        public IActionResult OnPost(int id)
        {
            using(var connection = new SqliteConnection(_configuration.GetConnectionString("ConnectionString")))
            {
                connection.Open();
                var tableCommand = connection.CreateCommand();
                tableCommand.CommandText = $"DELETE FROM drinking_water WHERE Id = {id}";
                tableCommand.ExecuteNonQuery();
            }
            return RedirectToPage("./Index");
        }
    }
}
