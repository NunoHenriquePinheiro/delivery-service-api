using DeliveryServiceApp.Models.Entities;
using DeliveryServiceApp.Persistence.Contexts;
using DeliveryServiceApp.Persistence.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DeliveryServiceApp.Persistence.Repositories.Implementations
{
    public class RoleRepository : BaseRepository, IRoleRepository
    {
        public RoleRepository(DataContext context) : base(context) { }

        public async Task<IEnumerable<Role>> GetAsync()
        {
            return await _context.Roles.ToListAsync();
        }
    }
}
