using DeliveryServiceApp.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DeliveryServiceApp.Services.Interfaces
{
    /// <summary>Interface mapping the logic methods that manage the users' access to the app.</summary>
    public interface IUserService
    {
        /// <summary>Authenticates the specified username.</summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <param name="user">The user.</param>
        /// <returns>Success/Unsuccess.</returns>
        bool Authenticate(string username, string password, out User user);

        /// <summary>Generates the security token.</summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>The token.</returns>
        string GenerateSecurityToken(int userId);

        /// <summary>Creates the specified user.</summary>
        /// <param name="user">The user.</param>
        /// <param name="password">The password.</param>
        /// <returns>The created user.</returns>
        Task<User> CreateAsync(User user, string password);

        /// <summary>Gets a collection of users.</summary>
        /// <param name="userFilter">The filter.</param>
        /// <returns>Collection of users.</returns>
        Task<IEnumerable<User>> GetAsync(User userFilter);

        /// <summary>Gets the user by its identifier.</summary>
        /// <param name="id">The identifier.</param>
        /// <returns>The user.</returns>
        Task<User> GetByIdAsync(int id);

        /// <summary>Gets the user by its identifier.</summary>
        /// <param name="id">The identifier.</param>
        /// <returns>The user.</returns>
        User GetById(int id);

        /// <summary>Updates the specified user.</summary>
        /// <param name="user">The user.</param>
        /// <param name="password">The password.</param>
        Task UpdateAsync(User userUpdate, string password = null);

        /// <summary>Deletes the user with the specified identifier.</summary>
        /// <param name="id">The identifier.</param>
        Task DeleteAsync(int id);
    }
}
