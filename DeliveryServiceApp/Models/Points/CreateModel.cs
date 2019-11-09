using System.ComponentModel.DataAnnotations;

namespace DeliveryServiceApp.Models.Points
{
    public class CreateModel
    {
        [Required]
        public string Description { get; set; }
    }
}
