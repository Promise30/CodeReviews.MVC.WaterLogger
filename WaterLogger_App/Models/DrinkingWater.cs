﻿using System.ComponentModel.DataAnnotations;

namespace HabitLogger_App.Models
{
    public class DrinkingWater
    {
        public int Id { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yy}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }
        [Range(0,Int32.MaxValue, ErrorMessage = "Value for {0} must be positive.")]
        public decimal Quantity { get; set; } 
        public string? ContainerType { get; set; } 
    }
}
