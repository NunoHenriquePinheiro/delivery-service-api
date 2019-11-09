using DeliveryServiceApp.Cache;
using DeliveryServiceApp.Helpers.Exceptions;
using DeliveryServiceApp.Helpers.Settings;
using DeliveryServiceApp.Models.Entities;
using DeliveryServiceApp.Persistence.Repositories.Interfaces;
using DeliveryServiceApp.Services.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryServiceApp.Services.Implementations
{
    public class UserService : BaseService, IUserService
    {
        private readonly AppSettings _appSettings;
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;

        public UserService(ICacheManager cacheManager,
            IOptions<AppSettings> appSettings,
            IUserRepository userRepository,
            IRoleRepository roleRepository) : base(cacheManager)
        {
            _appSettings = appSettings.Value;
            _userRepository = userRepository;
            _roleRepository = roleRepository;
        }


        public bool Authenticate(string username, string password, out User user)
        {
            ValidateUsernamePassword(username, password);

            var users = _userRepository.Get(new User { Username = username });
            user = users.FirstOrDefault();
            if (user != null)
            {
                return VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt);
            }
            return false;
        }


        public string GenerateSecurityToken(int userId)
        {
            double tokenExpireTime = double.TryParse(_appSettings?.TokenOptions?.ExpireTimeMinutes, out tokenExpireTime) ? tokenExpireTime : 10; // in minutes, respecting the appsetting
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings?.TokenOptions?.Secret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, userId.ToString())
                }),
                Expires = DateTime.UtcNow.AddMinutes(tokenExpireTime),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }


        public async Task<User> CreateAsync(User user, string password)
        {
            // Error if username/password are invalid values, or if the username is already in use
            ValidateUsernamePassword(user.Username, password);
            await ValidateUsernameAlreadyExistsAsync(user.Username);

            CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            await _userRepository.AddAsync(user);
            return user;
        }


        public async Task<IEnumerable<User>> GetAsync(User userFilter)
        {
            return await _userRepository.GetAsync(userFilter);
        }


        public async Task<User> GetByIdAsync(int id)
        {
            return (await _userRepository.GetAsync()).FirstOrDefault(x => x.Id == id);
        }


        public User GetById(int id)
        {
            return _userRepository.Get(new User { Id = id }).FirstOrDefault();
        }


        public async Task UpdateAsync(User userUpdate, string password = null)
        {
            var user = await ValidateUserExistsAsync(userUpdate.Id);

            // Change username if it is defined and different from the current one
            if (!string.IsNullOrWhiteSpace(userUpdate.Username) && userUpdate.Username != user.Username)
            {
                // Error if the new username is already taken by other user
                await ValidateUsernameAlreadyExistsAsync(userUpdate.Username);
                user.Username = userUpdate.Username;
            }

            if (!string.IsNullOrWhiteSpace(userUpdate.FirstName))
            {
                user.FirstName = userUpdate.FirstName;
            }

            if (!string.IsNullOrWhiteSpace(userUpdate.LastName))
            {
                user.LastName = userUpdate.LastName;
            }

            if (!string.IsNullOrWhiteSpace(userUpdate.RoleName))
            {
                // Error if the role does not exist
                await ValidateRoleAsync(userUpdate.RoleName);
                user.RoleName = userUpdate.RoleName;
            }

            if (!string.IsNullOrWhiteSpace(password))
            {
                CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);
                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
            }

            await _userRepository.UpdateAsync(user);
        }


        public async Task DeleteAsync(int id)
        {
            var user = await ValidateUserExistsAsync(id);
            await _userRepository.RemoveAsync(user);
        }


        #region Private methods

        /// <summary>Validates the username password.</summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        private void ValidateUsernamePassword(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                throw AppCustomExceptions.UsernameInvalidValue;
            }
            else if (string.IsNullOrWhiteSpace(password))
            {
                throw AppCustomExceptions.PasswordInvalidValue;
            }
        }

        /// <summary>Verifies the password hash.</summary>
        /// <param name="password">The password.</param>
        /// <param name="storedHash">The stored hash.</param>
        /// <param name="storedSalt">The stored salt.</param>
        /// <returns>Success/unsuccess in the verification.</returns>
        private bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(storedSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != storedHash[i])
                    {
                        return false;
                    }
                }
            }
            return true;
        }

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

        /// <summary>Validates if the username already exists.</summary>
        /// <param name="role">The username.</param>
        private async Task ValidateUsernameAlreadyExistsAsync(string username)
        {
            if ((await _userRepository.GetAsync(new User { Username = username })).Any())
            {
                throw AppCustomExceptions.UsernameAlreadyInUse;
            }
        }

        /// <summary>Validates if a user exists.</summary>
        /// <param name="id">The identifier of the user.</param>
        /// <returns>A step matching the given identifier.</returns>
        private async Task<User> ValidateUserExistsAsync(int id)
        {
            var user = (await _userRepository.GetAsync()).FirstOrDefault(x => x.Id == id);

            // Error if user does not exist
            if (user == null)
            {
                throw AppCustomExceptions.UserNotFound;
            }
            return user;
        }

        /// <summary>Validates the user's role.</summary>
        /// <param name="role">The role.</param>
        private async Task ValidateRoleAsync(string roleName)
        {
            if (!(await _roleRepository.GetAsync()).Any(x => x.Name.Equals(roleName, StringComparison.InvariantCultureIgnoreCase)))
            {
                throw AppCustomExceptions.RoleInvalidValue;
            }
        }

        #endregion
    }
}