using DeliveryServiceApp.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DeliveryServiceApp.Persistence.Repositories.Interfaces
{
    public interface IRoleRepository
    {
        Task<IEnumerable<Role>> GetAsync();
    }
}
