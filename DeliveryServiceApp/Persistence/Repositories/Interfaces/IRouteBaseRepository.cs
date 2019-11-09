using DeliveryServiceApp.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DeliveryServiceApp.Persistence.Repositories.Interfaces
{
    public interface IRouteBaseRepository
    {
        Task<IEnumerable<RouteBase>> GetAsync();
        Task<IEnumerable<RouteBase>> GetAsync(RouteBase routeBaseFilter);
        Task AddAsync(RouteBase routeBase);
        Task UpdateAsync(RouteBase routeBase);
        Task RemoveAsync(RouteBase routeBase);
    }
}
