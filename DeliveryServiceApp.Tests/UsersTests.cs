using DeliveryServiceApp.Helpers.Exceptions;
using DeliveryServiceApp.Models.Entities;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DeliveryServiceApp.Tests
{
    /// <summary>Tests class for "users".</summary>
    /// <seealso cref="TestsBase" />
    public class UsersTests : TestsBase
    {
        public UsersTests() : base() { }

        /// <summary>Authenticates the predefined user. Expects successful authentication.</summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        [Theory]
        [InlineData("nuno.admin", "12345")]
        public void Authenticate_PredefinedUser_ReturnsAuthenticated(string username, string password)
        {
            try
            {
                var isAuthenticated = _userService.Authenticate(username, password, out User userN);
                var isPassed = isAuthenticated && (userN?.Username == username);

                Assert.True(isPassed);
            }
            catch (AppCustomException)
            {
                Assert.True(false);
            }
        }

        /// <summary>Registers, updates and deletes a new user. Expects a successful completion.</summary>
        /// <param name="username">The username.</param>
        /// <param name="firstName">The first name.</param>
        /// <param name="lastName">The last name.</param>
        /// <param name="password">The password.</param>
        /// <param name="modifiedLastName">Modified last name (for update).</param>
        [Theory]
        [InlineData("nuno.new", "Nuno", "New", "123", "Updated")]
        public async Task RegisterUpdateDelete_NewUser_ReturnsCompleted(string username, string firstName,
            string lastName, string password, string modifiedLastName)
        {
            try
            {
                // REGISTER
                CreatePasswordHash(password, out byte[] passHash, out byte[] passSalt);
                var newUser = new User
                {
                    Username = username,
                    FirstName = firstName,
                    LastName = lastName,
                    PasswordHash = passHash,
                    PasswordSalt = passSalt
                };
                var user = await _userService.CreateAsync(newUser, password);

                Assert.NotNull(user);

                // UPDATE
                user.LastName = modifiedLastName;
                await _userService.UpdateAsync(user, password);

                var userUpdated = await _userService.GetByIdAsync(user.Id);
                var isUpdated = userUpdated.LastName == modifiedLastName;

                Assert.True(isUpdated);

                // DELETE
                await _userService.DeleteAsync(user.Id);

                var userDeleted = await _userService.GetByIdAsync(user.Id);
                var isDeleted = userDeleted == null;

                Assert.True(isDeleted);
            }
            catch (AppCustomException)
            {
                Assert.True(false);
            }
        }

        /// <summary>Gets the predefined user by its username. Expects the return of one user.</summary>
        /// <param name="username">The username.</param>
        [Theory]
        [InlineData("nuno.admin")]
        public async Task Get_PredefinedUser_ReturnsOneUser(string username)
        {
            try
            {
                var users = await _userService.GetAsync(new User { Username = username });
                var isOneUser = users.Count() == 1;

                Assert.True(isOneUser);
            }
            catch (AppCustomException)
            {
                Assert.True(false);
            }
        }

        /// <summary>Gets the predefined user by its identifier. Expects the return of a user.</summary>
        /// <param name="id">The identifier.</param>
        [Theory]
        [InlineData(1)]
        public async Task GetById_PredefinedUser_ReturnsUser(int id)
        {
            try
            {
                var user = await _userService.GetByIdAsync(id);

                Assert.NotNull(user);
            }
            catch (AppCustomException)
            {
                Assert.True(false);
            }
        }


        #region Private methods

        /// <summary>Creates the password hash.</summary>
        /// <param name="password">The password.</param>
        /// <param name="passwordHash">The password hash.</param>
        /// <param name="passwordSalt">The password salt.</param>
        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        #endregion
    }
}
