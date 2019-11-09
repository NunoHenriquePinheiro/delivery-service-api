using DeliveryServiceApp.Models.Entities;
using DeliveryServiceApp.Persistence.Contexts;
using DeliveryServiceApp.Persistence.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeliveryServiceApp.Persistence.Repositories.Implementations
{
    public class RouteBaseRepository : BaseRepository, IRouteBaseRepository
    {
        public RouteBaseRepository(DataContext context) : base(context) { }

        public async Task<IEnumerable<RouteBase>> GetAsync()
        {
            return await _context.RouteBases.ToListAsync();
        }

        public async Task<IEnumerable<RouteBase>> GetAsync(RouteBase routeBaseFilter)
        {
            if (routeBaseFilter == null)
            {
                return await _context.RouteBases.ToListAsync();
            }

            return await _context.RouteBases.Where(x =>
                (routeBaseFilter.Origin == null || string.IsNullOrWhiteSpace(routeBaseFilter.Origin.Description) || x.Origin.Description == routeBaseFilter.Origin.Description)
                && (routeBaseFilter.Destination == null || string.IsNullOrWhiteSpace(routeBaseFilter.Destination.Description) || x.Destination.Description == routeBaseFilter.Destination.Description)
                ).ToListAsync();
        }

        public async Task AddAsync(RouteBase routeBase)
        {
            _context.RouteBases.Add(routeBase);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(RouteBase routeBase)
        {
            _context.RouteBases.Update(routeBase);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveAsync(RouteBase routeBase)
        {
            _context.RouteBases.Remove(routeBase);
            await _context.SaveChangesAsync();
        }
    }
}
