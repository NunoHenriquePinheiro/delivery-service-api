namespace DeliveryServiceApp.Models.Entities
{
    public class Step
    {
        public int Id { get; set; }
        public Point Start { get; set; }
        public Point End { get; set; }
        public decimal Time { get; set; }
        public decimal Cost { get; set; }
        public int StartId { get; set; }
        public int EndId { get; set; }
    }
}
