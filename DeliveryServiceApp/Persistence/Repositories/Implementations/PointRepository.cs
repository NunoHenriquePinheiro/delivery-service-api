using DeliveryServiceApp.Models.Entities;
using DeliveryServiceApp.Persistence.Contexts;
using DeliveryServiceApp.Persistence.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeliveryServiceApp.Persistence.Repositories.Implementations
{
    public class PointRepository : BaseRepository, IPointRepository
    {
        public PointRepository(DataContext context) : base(context) { }

        public async Task<IEnumerable<Point>> GetAsync()
        {
            return await _context.Points.ToListAsync();
        }

        public async Task<IEnumerable<Point>> GetAsync(Point pointFilter)
        {
            if (pointFilter == null)
            {
                return await _context.Points.ToListAsync();
            }

            return await _context.Points.Where(x =>
                (string.IsNullOrWhiteSpace(pointFilter.Description) || x.Description == pointFilter.Description)).ToListAsync();
        }

        public async Task AddAsync(Point point)
        {
            _context.Points.Add(point);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Point point)
        {
            _context.Points.Update(point);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveAsync(Point point)
        {
            _context.Points.Remove(point);
            await _context.SaveChangesAsync();
        }
    }
}
