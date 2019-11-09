using DeliveryServiceApp.Models.Entities;
using System.Threading.Tasks;

namespace DeliveryServiceApp.Algorithms.Interfaces
{
    /// <summary>Interface mapping the logic methods that compute paths (step collections) in routes.</summary>
    public interface IComputeRoutePaths
    {
        /// <summary>Gets all possible paths related with a route, between the origin and destination points.</summary>
        /// <param name="routeBase">The route base with the origin and destination points.</param>
        /// <returns>Route with all possible paths (step collections).</returns>
        Task<Route> GetAllPathsFromRouteAsync(RouteBase routeBase);

        /// <summary>Gets the step collection with the best performance, between the origin and destination points.</summary>
        /// <param name="routeBase">The route base with the origin and destination points.</param>
        /// <param name="option">The option measure to evaluate performance (time or cost).</param>
        /// <returns>Step collection with the best performance in the route.</returns>
        Task<StepsCollection> GetBestOptionPathFromRouteAsync(RouteBase routeBase, PathPerformanceOptions option);
    }
}
