using System.Collections.Generic;

namespace DeliveryServiceApp.Models.Entities
{
    public class Route : RouteBase
    {
        public List<StepsCollection> StepsCollectionList { get; set; }
    }
}
