using AutoMapper;
using DeliveryServiceApp.Helpers;
using DeliveryServiceApp.Helpers.Settings;
using DeliveryServiceApp.Models.Entities;
using DeliveryServiceApp.Models.Users;
using DeliveryServiceApp.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DeliveryServiceApp.Controllers
{
    [Route("users")]
    public class UsersController : BaseController
    {
        private readonly IUserService _userService;

        public UsersController(
            IUserService userService,
            IMapper mapper,
            IOptions<AppSettings> appSettings) : base(mapper, appSettings)
        {
            _userService = userService;
        }


        /// <summary>Authenticates the specified username/password.</summary>
        /// <param name="model">The username/password model.</param>
        /// <returns>The authenticated user and its token.</returns>
        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody]AuthenticateModel model)
        {
            if (!_userService.Authenticate(model.Username, model.Password, out User user))
            {
                return Unauthorized();
            }

            var tokenString = _userService.GenerateSecurityToken(user.Id);
            var userModel = _mapper.Map<UserModel>(user);
            return Ok(new
            {
                userModel,
                Token = tokenString
            });
        }

        /// <summary>Registers the specified user.</summary>
        /// <param name="model">The user model.</param>
        /// <returns>The user.</returns>
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody]RegisterModel model)
        {
            var user = _mapper.Map<User>(model);
            var userReturn = await _userService.CreateAsync(user, model.Password);
            var userModel = _mapper.Map<UserModel>(userReturn);
            return Ok(userModel);
        }

        /// <summary>Gets a collection of users.</summary>
        /// <returns>Collection of users.</returns>
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery]GetModel model)
        {
            var userFilter = _mapper.Map<User>(model);
            var users = await _userService.GetAsync(userFilter);
            var outModel = _mapper.Map<List<UserModel>>(users);

            return OkOptionalContent<UserModel>(outModel);
        }

        /// <summary>Gets the user by identifier.</summary>
        /// <param name="id">The identifier.</param>
        /// <returns>The user.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _userService.GetByIdAsync(id);
            var model = _mapper.Map<UserModel>(user);
            return Ok(model);
        }

        /// <summary>Updates the specified user.</summary>
        /// <param name="id">The user identifier.</param>
        /// <param name="model">The user model.</param>
        /// <returns>Success or unsuccess status.</returns>
        [Authorize(Roles = Constants.RoleAdmin)]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody]UpdateModel model)
        {
            var user = _mapper.Map<User>(model);
            user.Id = id;
            await _userService.UpdateAsync(user, model.Password);
            return NoContent();
        }

        /// <summary>Deletes the specified user.</summary>
        /// <param name="id">The user identifier.</param>
        /// <returns>Success or unsuccess status.</returns>
        [Authorize(Roles = Constants.RoleAdmin)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _userService.DeleteAsync(id);
            return NoContent();
        }
    }
}
