using DeliveryServiceApp.Models.Entities;
using DeliveryServiceApp.Persistence.Contexts;
using DeliveryServiceApp.Persistence.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeliveryServiceApp.Persistence.Repositories.Implementations
{
    public class StepRepository : BaseRepository, IStepRepository
    {
        public StepRepository(DataContext context) : base(context) { }

        public async Task<IEnumerable<Step>> GetAsync()
        {
            return await _context.Steps.ToListAsync();
        }

        public async Task<IEnumerable<Step>> GetAsync(Step stepFilter)
        {
            if (stepFilter == null)
            {
                return await _context.Steps.ToListAsync();
            }

            return await _context.Steps.Where(x =>
                (stepFilter.Start == null || string.IsNullOrWhiteSpace(stepFilter.Start.Description) || x.Start.Description == stepFilter.Start.Description)
                && (stepFilter.End == null || string.IsNullOrWhiteSpace(stepFilter.End.Description) || x.End.Description == stepFilter.End.Description)
                && (stepFilter.Cost <= 0 || x.Cost == stepFilter.Cost)
                && (stepFilter.Time <= 0 || x.Time == stepFilter.Time)
                ).ToListAsync();
        }

        public async Task AddAsync(Step step)
        {
            _context.Steps.Add(step);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Step step)
        {
            _context.Steps.Update(step);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveAsync(Step step)
        {
            _context.Steps.Remove(step);
            await _context.SaveChangesAsync();
        }
    }
}
