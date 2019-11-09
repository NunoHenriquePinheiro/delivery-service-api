using DeliveryServiceApp.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DeliveryServiceApp.Persistence.Repositories.Interfaces
{
    public interface IStepRepository
    {
        Task<IEnumerable<Step>> GetAsync();
        Task<IEnumerable<Step>> GetAsync(Step stepFilter);
        Task AddAsync(Step step);
        Task UpdateAsync(Step step);
        Task RemoveAsync(Step step);
    }
}
