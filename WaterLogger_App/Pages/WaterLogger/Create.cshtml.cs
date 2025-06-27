using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.Sqlite;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using HabitLogger_App.Models;

namespace HabitLogger_App.Pages.WaterLogger
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

        public IActionResult OnGet()
        {
            var types = Enum.GetValues(typeof(ContainerType))
                    .Cast<ContainerType>()
                    .Select(ct => new SelectListItem
                    {
                        Value = ct.ToString(), 
                        Text = ct.GetType()
                                 .GetMember(ct.ToString())
                                 .First()
                                 .GetCustomAttribute<DisplayAttribute>()?
                                 .Name ?? ct.ToString()
                    })
                    .ToList();

            ContainerTypeList = new SelectList(types, "Value", "Text");
            return Page();
        }
        [BindProperty]
        public DrinkingWater DrinkingWater { get; set; }
        public SelectList ContainerTypeList { get; set; }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            using(var connection = new SqliteConnection(_configuration.GetConnectionString("ConnectionString")))
            {
                connection.Open();
                var tableCommand = connection.CreateCommand();
                tableCommand.CommandText = "SELECT COUNT(*) FROM drinking_water WHERE date = @date AND containertype = @containertype";
                tableCommand.Parameters.AddWithValue("@date", DrinkingWater.Date.ToString("dd/MM/yyyy HH:mm:ss"));
                tableCommand.Parameters.AddWithValue("@containertype", DrinkingWater.ContainerType);

                var count = Convert.ToInt32(tableCommand.ExecuteScalar());

                if (count > 0)
                {
                    ModelState.AddModelError(string.Empty, "A record for this date and container type already exists. Please update the existing record or choose a different date.");
                    var types = Enum.GetValues(typeof(ContainerType))
                        .Cast<ContainerType>()
                        .Select(ct => new SelectListItem
                        {
                            Value = ct.ToString(),
                            Text = ct.GetType()
                                .GetMember(ct.ToString())
                                .First()
                                .GetCustomAttribute<DisplayAttribute>()?.Name ?? ct.ToString()
                        })
                        .ToList();
                    ContainerTypeList = new SelectList(types, "Value", "Text");
                    return Page();
                }
                tableCommand.CommandText = "INSERT INTO drinking_water (date, quantity, containertype) VALUES (@date, @quantity, @containertype)";

                tableCommand.Parameters.AddWithValue("@quantity", DrinkingWater.Quantity);
                tableCommand.ExecuteNonQuery();
                connection.Close();

            }
            return RedirectToPage("/WaterLogger/Index");
        }
    }
}
