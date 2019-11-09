using AutoMapper;
using DeliveryServiceApp.Helpers;
using DeliveryServiceApp.Helpers.Settings;
using DeliveryServiceApp.Models.Entities;
using DeliveryServiceApp.Models.Points;
using DeliveryServiceApp.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace DeliveryServiceApp.Controllers
{
    [Route("points")]
    public class PointsController : BaseController
    {
        private readonly IPointService _pointService;

        public PointsController(
            IPointService pointService,
            IMapper mapper,
            IOptions<AppSettings> appSettings) : base(mapper, appSettings)
        {
            _pointService = pointService;
        }


        /// <summary>Creates the specified point.</summary>
        /// <param name="pointDescription">The point description.</param>
        /// <returns>The point.</returns>
        [Authorize(Roles = Constants.RoleAdmin)]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody]CreateModel model)
        {
            var point = await _pointService.CreateAsync(model.Description);
            return Ok(point);
        }

        /// <summary>Gets a collection of points.</summary>
        /// <param name="pointDescription">The point description.</param>
        /// <returns>Collection of points.</returns>
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery]string pointDescription)
        {
            var points = await _pointService.GetAsync(new Point { Description = pointDescription });
            return OkOptionalContent(points);
        }

        /// <summary>Gets the point by identifier.</summary>
        /// <param name="id">The identifier.</param>
        /// <returns>The point.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var point = await _pointService.GetByIdAsync(id);
            return OkOptionalContent(point);
        }

        /// <summary>Updates the specified point.</summary>
        /// <param name="id">The point identifier.</param>
        /// <param name="pointDescription">The point description.</param>
        /// <returns>Success or unsuccess status.</returns>
        [Authorize(Roles = Constants.RoleAdmin)]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody]string pointDescription)
        {
            var point = new Point
            {
                Id = id,
                Description = pointDescription
            };
            await _pointService.UpdateAsync(point);
            return NoContent();
        }

        /// <summary>Deletes the specified point.</summary>
        /// <param name="id">The point identifier.</param>
        /// <returns>Success or unsuccess status.</returns>
        [Authorize(Roles = Constants.RoleAdmin)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _pointService.DeleteAsync(id);
            return NoContent();
        }
    }
}
