using System;
using System.ComponentModel.DataAnnotations;

namespace DeliveryServiceApp.Models.Steps
{
    public class CreateModel
    {
        [Required]
        public string PointStart { get; set; }

        [Required]
        public string PointEnd { get; set; }

        [Required]
        [Range(0.0, double.MaxValue)]
        public decimal Cost { get; set; }

        [Required]
        [Range(0.0, double.MaxValue)]
        public int Time { get; set; }
    }
}
