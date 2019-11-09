using DeliveryServiceApp.Algorithms.Interfaces;
using DeliveryServiceApp.Cache;
using DeliveryServiceApp.Helpers.Exceptions;
using DeliveryServiceApp.Helpers.Validations;
using DeliveryServiceApp.Models.Entities;
using DeliveryServiceApp.Persistence.Repositories.Interfaces;
using DeliveryServiceApp.Services.Interfaces;
using System.Linq;
using System.Threading.Tasks;

namespace DeliveryServiceApp.Services.Implementations
{
    public class RouteStepService : BaseService, IRouteStepService
    {
        private readonly IRouteBaseRepository _routeBaseRepository;
        private readonly IPointRepository _pointRepository;
        private readonly IStepRepository _stepRepository;
        private readonly IComputeRoutePaths _computeRoutePaths;

        public RouteStepService(ICacheManager cacheManager,
            IRouteBaseRepository routeBaseRepository,
            IPointRepository pointRepository,
            IStepRepository stepRepository,
            IComputeRoutePaths computeRoutePaths) : base(cacheManager)
        {
            _routeBaseRepository = routeBaseRepository;
            _pointRepository = pointRepository;
            _stepRepository = stepRepository;
            _computeRoutePaths = computeRoutePaths;
        }


        public async Task<Route> GetRouteAllStepCollectionsAsync(string originPoint, string destinationPoint)
        {
            // Calculate the route base. An error is thrown, if any validation is not passed.
            var routeBase = await CalculateRouteBaseAsync(originPoint, destinationPoint);

            var route = await _computeRoutePaths.GetAllPathsFromRouteAsync(routeBase);
            return route;
        }


        public async Task<StepsCollection> GetStepCollectionLeastTimeAsync(string originPoint, string destinationPoint)
        {
            // Calculate the route base. An error is thrown, if there is a validation not passed
            var routeBase = await CalculateRouteBaseAsync(originPoint, destinationPoint);

            var route = await _computeRoutePaths.GetBestOptionPathFromRouteAsync(routeBase, Algorithms.PathPerformanceOptions.Time);
            return route;
        }


        public async Task<StepsCollection> GetStepCollectionLeastCostAsync(string originPoint, string destinationPoint)
        {
            // Calculate the route base. An error is thrown, if there is a validation not passed
            var routeBase = await CalculateRouteBaseAsync(originPoint, destinationPoint);

            var route = await _computeRoutePaths.GetBestOptionPathFromRouteAsync(routeBase, Algorithms.PathPerformanceOptions.Cost);
            return route;
        }


        #region Private methods

        /// <summary>Calculates a route base for a given origin and destination.</summary>
        /// <param name="originPoint">The origin point.</param>
        /// <param name="destinationPoint">The destination point.</param>
        /// <returns>The route base.</returns>
        private async Task<RouteBase> CalculateRouteBaseAsync(string originPoint, string destinationPoint)
        {
            // Error if origin and destination are the same point
            PointsValidation.ValidateEqualPointsAsync(originPoint, destinationPoint);

            // Error if point description does not exist
            var origin = await PointsValidation.ValidatePointExistsAsync(_pointRepository, originPoint);
            var destination = await PointsValidation.ValidatePointExistsAsync(_pointRepository, destinationPoint);

            var routeBase = new RouteBase
            {
                Origin = origin,
                Destination = destination
            };

            // Error if route base with such points does not exist
            await ValidateRouteBaseExistsAsync(routeBase);

            // Error if there is not any step to begin the route, or if there is not any step to finish the route
            await ValidateStepStartExistsAsync(origin);
            await ValidateStepEndExistsAsync(destination);

            return routeBase;
        }

        /// <summary>Validates if the route base exists.</summary>
        /// <param name="routeBaseCheck">The route base to check.</param>
        private async Task ValidateRouteBaseExistsAsync(RouteBase routeBaseCheck)
        {
            if (!(await _routeBaseRepository.GetAsync(routeBaseCheck)).Any())
            {
                throw AppCustomExceptions.RouteBaseNotExists;
            }
        }

        /// <summary>Validates if there is any step that starts with the origin point of the route.</summary>
        /// <param name="point">The point to check.</param>
        private async Task ValidateStepStartExistsAsync(Point point)
        {
            if (!(await _stepRepository.GetAsync(new Step { Start = point })).Any())
            {
                throw AppCustomExceptions.StepBeginRouteNotFound;
            }
        }

        /// <summary>Validates if there is any step that ends with the destination point of the route.</summary>
        /// <param name="point">The point to check.</param>
        private async Task ValidateStepEndExistsAsync(Point point)
        {
            if (!(await _stepRepository.GetAsync(new Step { End = point })).Any())
            {
                throw AppCustomExceptions.StepEndRouteNotFound;
            }
        }

        #endregion
    }
}