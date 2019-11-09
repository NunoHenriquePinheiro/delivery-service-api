using DeliveryServiceApp.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DeliveryServiceApp.Services.Interfaces
{
    /// <summary>Interface mapping the logic methods that manage the route bases.</summary>
    public interface IRouteBaseService
    {
        /// <summary>Creates the specified route base.</summary>
        /// <param name="routeBase">The route base.</param>
        /// <returns>The created route base.</returns>
        Task<RouteBase> CreateAsync(RouteBase routeBase);

        /// <summary>Gets a collection of route bases.</summary>
        /// <param name="stepFilter">The filter.</param>
        /// <returns>Collection of route bases.</returns>
        Task<IEnumerable<RouteBase>> GetAsync(RouteBase routeBaseFilter);

        /// <summary>Gets the route base by its identifier.</summary>
        /// <param name="id">The identifier.</param>
        /// <returns>The route base.</returns>
        Task<RouteBase> GetByIdAsync(int id);

        /// <summary>Updates the specified route base.</summary>
        /// <param name="routeBase">The route base.</param>
        Task UpdateAsync(RouteBase routeBaseUpdate);

        /// <summary>Deletes the route base with the specified identifier.</summary>
        /// <param name="id">The identifier.</param>
        Task DeleteAsync(int id);
    }
}
