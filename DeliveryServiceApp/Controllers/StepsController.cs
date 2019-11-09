using AutoMapper;
using DeliveryServiceApp.Helpers;
using DeliveryServiceApp.Helpers.Settings;
using DeliveryServiceApp.Models.Entities;
using DeliveryServiceApp.Models.Steps;
using DeliveryServiceApp.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace DeliveryServiceApp.Controllers
{
    [Route("steps")]
    public class StepsController : BaseController
    {
        private readonly IStepService _stepService;

        public StepsController(
            IStepService stepService,
            IMapper mapper,
            IOptions<AppSettings> appSettings) : base(mapper, appSettings)
        {
            _stepService = stepService;
        }


        /// <summary>Creates the specified step.</summary>
        /// <param name="model">The create model.</param>
        /// <returns>The step.</returns>
        [Authorize(Roles = Constants.RoleAdmin)]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody]CreateModel model)
        {
            var stepToAdd = _mapper.Map<Step>(model);
            stepToAdd.Start = new Point { Description = model.PointStart };
            stepToAdd.End = new Point { Description = model.PointEnd };

            var step = await _stepService.CreateAsync(stepToAdd);
            return Ok(step);
        }

        /// <summary>Gets a collection of steps.</summary>
        /// <param name="model">The get model.</param>
        /// <returns>The steps.</returns>
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery]StepModel model)
        {
            var stepFilter = _mapper.Map<Step>(model);
            stepFilter.Start = new Point { Description = model.PointStart };
            stepFilter.End = new Point { Description = model.PointEnd };

            var steps = await _stepService.GetAsync(stepFilter);
            return OkOptionalContent(steps);
        }

        /// <summary>Gets the step by identifier.</summary>
        /// <param name="id">The identifier.</param>
        /// <returns>The step.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var step = await _stepService.GetByIdAsync(id);
            return OkOptionalContent(step);
        }

        /// <summary>Updates the specified step.</summary>
        /// <param name="model">The model.</param>
        /// <returns>Success or unsuccess status.</returns>
        [Authorize(Roles = Constants.RoleAdmin)]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody]StepModel model)
        {
            var stepToUpdate = _mapper.Map<Step>(model);
            stepToUpdate.Id = id;
            stepToUpdate.Start = new Point { Description = model.PointStart };
            stepToUpdate.End = new Point { Description = model.PointEnd };

            await _stepService.UpdateAsync(stepToUpdate);
            return NoContent();
        }

        /// <summary>Deletes the specified step.</summary>
        /// <param name="id">The route base identifier.</param>
        /// <returns>Success or unsuccess status.</returns>
        [Authorize(Roles = Constants.RoleAdmin)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _stepService.DeleteAsync(id);
            return NoContent();
        }
    }
}
