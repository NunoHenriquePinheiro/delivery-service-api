using DeliveryServiceApp.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DeliveryServiceApp.Services.Interfaces
{
    /// <summary>Interface mapping the logic methods that manage the steps.</summary>
    public interface IStepService
    {
        /// <summary>Creates the specified step.</summary>
        /// <param name="step">The step.</param>
        /// <returns>The created step.</returns>
        Task<Step> CreateAsync(Step step);

        /// <summary>Gets a collection of steps.</summary>
        /// <param name="stepFilter">The filter.</param>
        /// <returns>Collection of steps.</returns>
        Task<IEnumerable<Step>> GetAsync(Step stepFilter);

        /// <summary>Gets the step by its identifier.</summary>
        /// <param name="id">The identifier.</param>
        /// <returns>The step.</returns>
        Task<Step> GetByIdAsync(int id);

        /// <summary>Updates the specified step.</summary>
        /// <param name="stepUpdate">The step.</param>
        Task UpdateAsync(Step stepUpdate);

        /// <summary>Deletes the step with the specified identifier.</summary>
        /// <param name="id">The identifier.</param>
        Task DeleteAsync(int id);
    }
}
