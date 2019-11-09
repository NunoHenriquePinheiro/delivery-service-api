using AutoMapper;
using DeliveryServiceApp.Helpers.Settings;
using DeliveryServiceApp.Models.Entities;
using DeliveryServiceApp.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace DeliveryServiceApp.Controllers
{
    [Route("route-steps")]
    public class RouteStepsController : BaseController
    {
        private readonly IRouteStepService _routeStepService;

        public RouteStepsController(
            IRouteStepService routeStepService,
            IMapper mapper,
            IOptions<AppSettings> appSettings) : base(mapper, appSettings)
        {
            _routeStepService = routeStepService;
        }


        /// <summary>Gets a collection of all lists of route steps.</summary>
        /// <param name="pointOrigin">The origin point description.</param>
        /// <param name="pointDestination">The destination point description.</param>
        /// <returns>The lists of route steps.</returns>
        [HttpGet("{pointOrigin}/{pointDestination}")]
        public async Task<IActionResult> GetAll(string pointOrigin, string pointDestination)
        {
            var routeStepsLists = await _routeStepService.GetRouteAllStepCollectionsAsync(pointOrigin, pointDestination);
            return OkOptionalContent(routeStepsLists);
        }

        /// <summary>Gets a list of route steps with the least total time.</summary>
        /// <param name="pointOrigin">The origin point description.</param>
        /// <param name="pointDestination">The destination point description.</param>
        /// <returns>Lists of route steps with the least total time.</returns>
        [HttpGet("least-time/{pointOrigin}/{pointDestination}")]
        public async Task<IActionResult> GetLeastTime(string pointOrigin, string pointDestination)
        {
            var routeSteps = await _routeStepService.GetStepCollectionLeastTimeAsync(pointOrigin, pointDestination);
            return OkOptionalContent(routeSteps);
        }

        /// <summary>Gets a list of route steps with the least total cost.</summary>
        /// <param name="pointOrigin">The origin point description.</param>
        /// <param name="pointDestination">The destination point description.</param>
        /// <returns>Lists of route steps with the least total cost.</returns>
        [HttpGet("least-cost/{pointOrigin}/{pointDestination}")]
        public async Task<IActionResult> GetLeastCost(string pointOrigin, string pointDestination)
        {
            var routeSteps = await _routeStepService.GetStepCollectionLeastCostAsync(pointOrigin, pointDestination);
            return OkOptionalContent(routeSteps);
        }
    }
}
