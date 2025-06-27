using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.Sqlite;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;
using HabitLogger_App.Models;

namespace HabitLogger_App.Pages.WaterLogger
{
    public class UpdateModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public UpdateModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        [BindProperty]
        public DrinkingWater DrinkingWater { get; set; }
        public SelectList ContainerTypeList { get; set; }

        public IActionResult OnGet(int id)
        {
            DrinkingWater = GetById(id);
            var types = Enum.GetValues(typeof(ContainerType))
                        .Cast<ContainerType>()
                        .Select(ct => new SelectListItem
                        {
                            Value = ct.ToString(),
                            Text = ct.GetType()
                                     .GetMember(ct.ToString())
                                     .First()
                                     .GetCustomAttribute<DisplayAttribute>()?.Name ?? ct.ToString()
                        }).ToList();

            ContainerTypeList = new SelectList(types, "Value", "Text", DrinkingWater.ContainerType);

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
                    drinkingWaterRecord.Date = DateTime.ParseExact(reader.GetString(1), "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                    drinkingWaterRecord.Quantity = reader.GetDecimal(2);
                    drinkingWaterRecord.ContainerType = reader.GetString(3);
                }
                return drinkingWaterRecord;
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
        tableCommand.CommandText =
            "UPDATE drinking_water " +
            "SET date = @date, quantity = @quantity, containertype = @containertype " +
            "WHERE id = @id";

        tableCommand.Parameters.AddWithValue("@date", DrinkingWater.Date.ToString("dd/MM/yyyy HH:mm:ss")); 
        tableCommand.Parameters.AddWithValue("@quantity", DrinkingWater.Quantity);
        tableCommand.Parameters.AddWithValue("@containertype", DrinkingWater.ContainerType ?? string.Empty);
        tableCommand.Parameters.AddWithValue("@id", DrinkingWater.Id);

        tableCommand.ExecuteNonQuery();
    }

    return RedirectToPage("./Index");
}


    }
}
