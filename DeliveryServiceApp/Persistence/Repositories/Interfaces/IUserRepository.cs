using DeliveryServiceApp.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DeliveryServiceApp.Persistence.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAsync();
        IEnumerable<User> Get(User userFilter);
        Task<IEnumerable<User>> GetAsync(User userFilter);
        Task AddAsync(User user);
        Task UpdateAsync(User user);
        Task RemoveAsync(User user);
    }
}
