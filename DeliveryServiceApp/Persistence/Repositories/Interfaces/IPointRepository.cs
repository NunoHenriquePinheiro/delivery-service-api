using DeliveryServiceApp.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DeliveryServiceApp.Persistence.Repositories.Interfaces
{
    public interface IPointRepository
    {
        Task<IEnumerable<Point>> GetAsync();
        Task<IEnumerable<Point>> GetAsync(Point pointFilter);
        Task AddAsync(Point point);
        Task UpdateAsync(Point point);
        Task RemoveAsync(Point point);
    }
}
