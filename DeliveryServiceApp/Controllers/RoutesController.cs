using AutoMapper;
using DeliveryServiceApp.Helpers;
using DeliveryServiceApp.Helpers.Settings;
using DeliveryServiceApp.Models.Entities;
using DeliveryServiceApp.Models.Routes;
using DeliveryServiceApp.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace DeliveryServiceApp.Controllers
{
    [Route("routes")]
    public class RoutesController : BaseController
    {
        private readonly IRouteBaseService _routeBaseService;

        public RoutesController(
            IRouteBaseService routeBaseService,
            IMapper mapper,
            IOptions<AppSettings> appSettings) : base(mapper, appSettings)
        {
            _routeBaseService = routeBaseService;
        }


        /// <summary>Creates the specified route base.</summary>
        /// <param name="model">The create model.</param>
        /// <returns>The route base.</returns>
        [Authorize(Roles = Constants.RoleAdmin)]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody]CreateModel model)
        {
            var routeBase = await _routeBaseService.CreateAsync(new RouteBase
            {
                Origin = new Point { Description = model.PointOrigin },
                Destination = new Point { Description = model.PointDestination }
            });
            return Ok(routeBase);
        }

        /// <summary>Gets a collection of route bases.</summary>
        /// <param name="pointOrigin">The origin point description.</param>
        /// <param name="pointDestination">The destination point description.</param>
        /// <returns>The route bases.</returns>
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery][Required]string pointOrigin, [FromQuery][Required]string pointDestination)
        {
            var routeBases = await _routeBaseService.GetAsync(new RouteBase
            {
                Origin = new Point { Description = pointOrigin },
                Destination = new Point { Description = pointDestination }
            });
            return OkOptionalContent(routeBases);
        }

        /// <summary>Gets the route base by identifier.</summary>
        /// <param name="id">The identifier.</param>
        /// <returns>The route base.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var routeBase = await _routeBaseService.GetByIdAsync(id);
            return OkOptionalContent(routeBase);
        }

        /// <summary>Updates the specified route base.</summary>
        /// <param name="id">The route base identifier.</param>
        /// <param name="pointOrigin">The origin point description.</param>
        /// <param name="pointDestination">The destination point description.</param>
        /// <returns>Success or unsuccess status.</returns>
        [Authorize(Roles = Constants.RoleAdmin)]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody]UpdateModel updateModel)
        {
            var routeBase = new RouteBase
            {
                Id = id,
                Origin = new Point { Description = updateModel.PointOrigin },
                Destination = new Point { Description = updateModel.PointDestination }
            };
            await _routeBaseService.UpdateAsync(routeBase);
            return NoContent();
        }

        /// <summary>Deletes the specified route base.</summary>
        /// <param name="id">The route base identifier.</param>
        /// <returns>Success or unsuccess status.</returns>
        [Authorize(Roles = Constants.RoleAdmin)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _routeBaseService.DeleteAsync(id);
            return NoContent();
        }
    }
}
