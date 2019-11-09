using DeliveryServiceApp.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DeliveryServiceApp.Services.Interfaces
{
    /// <summary>Interface mapping the logic methods that manage the points.</summary>
    public interface IPointService
    {
        /// <summary>Creates the specified point.</summary>
        /// <param name="description">The point description.</param>
        /// <returns>The created point.</returns>
        Task<Point> CreateAsync(string description);

        /// <summary>Gets a collection of points.</summary>
        /// <param name="pointFilter">The filter.</param>
        /// <returns>Collection of points.</returns>
        Task<IEnumerable<Point>> GetAsync(Point pointFilter);

        /// <summary>Gets the point by its identifier.</summary>
        /// <param name="id">The identifier.</param>
        /// <returns>The point.</returns>
        Task<Point> GetByIdAsync(int id);

        /// <summary>Updates the specified point.</summary>
        /// <param name="pointUpdate">The point.</param>
        Task UpdateAsync(Point pointUpdate);

        /// <summary>Deletes the point with the specified identifier.</summary>
        /// <param name="id">The identifier.</param>
        Task DeleteAsync(int id);
    }
}
