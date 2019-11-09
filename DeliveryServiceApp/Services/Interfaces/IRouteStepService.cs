using DeliveryServiceApp.Models.Entities;
using System.Threading.Tasks;

namespace DeliveryServiceApp.Services.Interfaces
{
    /// <summary>Interface mapping the logic methods that manage the step collections in routes.</summary>
    public interface IRouteStepService
    {
        /// <summary>Gets the route with all possible step collections, between the origin and destination points.</summary>
        /// <param name="originPoint">The origin point.</param>
        /// <param name="destinationPoint">The destination point.</param>
        /// <returns>Route with all possible step collections.</returns>
        Task<Route> GetRouteAllStepCollectionsAsync(string originPoint, string destinationPoint);

        /// <summary>Gets the step collection with the least time, between the origin and destination points.</summary>
        /// <param name="originPoint">The origin point.</param>
        /// <param name="destinationPoint">The destination point.</param>
        /// <returns>Step collection with the least time.</returns>
        Task<StepsCollection> GetStepCollectionLeastTimeAsync(string originPoint, string destinationPoint);

        /// <summary>Gets the step collection with the least cost, between the origin and destination points.</summary>
        /// <param name="originPoint">The origin point.</param>
        /// <param name="destinationPoint">The destination point.</param>
        /// <returns>Step collection with the least cost.</returns>
        Task<StepsCollection> GetStepCollectionLeastCostAsync(string originPoint, string destinationPoint);
    }
}
