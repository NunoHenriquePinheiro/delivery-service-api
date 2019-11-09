using System.ComponentModel.DataAnnotations;

namespace DeliveryServiceApp.Models.Routes
{
    public class CreateModel
    {
        [Required]
        public string PointOrigin { get; set; }

        [Required]
        public string PointDestination { get; set; }
    }
}
