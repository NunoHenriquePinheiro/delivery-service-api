using System;
using System.ComponentModel.DataAnnotations;

namespace DeliveryServiceApp.Models.Steps
{
    public class StepModel
    {
        public string PointStart { get; set; }
        public string PointEnd { get; set; }
        
        [Range(0.0, double.MaxValue)]
        public decimal Cost { get; set; }

        [Range(0.0, double.MaxValue)]
        public int Time { get; set; }
    }
}
