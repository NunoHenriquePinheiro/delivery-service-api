using DeliveryServiceApp.Models.Entities;
using DeliveryServiceApp.Persistence.Contexts;
using DeliveryServiceApp.Persistence.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeliveryServiceApp.Persistence.Repositories.Implementations
{
    public class UserRepository : BaseRepository, IUserRepository
    {
        public UserRepository(DataContext context) : base(context) { }


        public IEnumerable<User> Get(User userFilter)
        {
            if (userFilter == null)
            {
                return _context.Users.ToList();
            }

            return _context.Users.Where(x =>
                (string.IsNullOrWhiteSpace(userFilter.Username) || x.Username == userFilter.Username)
                && (string.IsNullOrWhiteSpace(userFilter.FirstName) || x.FirstName == userFilter.FirstName)
                && (string.IsNullOrWhiteSpace(userFilter.LastName) || x.LastName == userFilter.LastName)
                && (string.IsNullOrWhiteSpace(userFilter.RoleName) || x.RoleName == userFilter.RoleName)).ToList();
        }

        public async Task<IEnumerable<User>> GetAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<IEnumerable<User>> GetAsync(User userFilter)
        {
            if (userFilter == null)
            {
                return await _context.Users.ToListAsync();
            }

            return await _context.Users.Where(x =>
                (string.IsNullOrWhiteSpace(userFilter.Username) || x.Username == userFilter.Username)
                && (string.IsNullOrWhiteSpace(userFilter.FirstName) || x.FirstName == userFilter.FirstName)
                && (string.IsNullOrWhiteSpace(userFilter.LastName) || x.LastName == userFilter.LastName)
                && (string.IsNullOrWhiteSpace(userFilter.RoleName) || x.RoleName == userFilter.RoleName)).ToListAsync();
        }

        public async Task AddAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveAsync(User user)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }
    }
}
