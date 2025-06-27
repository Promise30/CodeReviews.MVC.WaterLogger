using System.ComponentModel.DataAnnotations;

namespace HabitLogger_App.Models
{
    public enum ContainerType
    {
        [Display(Name = "Glass")]
        Glass,
        [Display(Name = "Bottle")]
        Bottle,
        [Display(Name = "Big Bottle")]
        BigBottle
    }

}
