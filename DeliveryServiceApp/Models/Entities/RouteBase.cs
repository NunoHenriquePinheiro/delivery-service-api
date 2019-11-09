namespace DeliveryServiceApp.Models.Entities
{
    public class RouteBase
    {
        public int Id { get; set; }
        public Point Origin { get; set; }
        public Point Destination { get; set; }
        public int OriginId { get; set; }
        public int DestinationId { get; set; }
    }
}
