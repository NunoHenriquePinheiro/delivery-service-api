using System.Collections.Generic;

namespace DeliveryServiceApp.Models.Entities
{
    public class StepsCollection
    {
        public List<Step> Steps { get; set; }
        public decimal TotalCost { get; set; }
        public decimal TotalTime { get; set; }
    }
}
