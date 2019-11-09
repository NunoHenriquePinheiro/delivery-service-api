using DeliveryServiceApp.Cache;
using DeliveryServiceApp.Helpers.Exceptions;
using DeliveryServiceApp.Helpers.Validations;
using DeliveryServiceApp.Models.Entities;
using DeliveryServiceApp.Persistence.Repositories.Interfaces;
using DeliveryServiceApp.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeliveryServiceApp.Services.Implementations
{
    public class RouteBaseService : BaseService, IRouteBaseService
    {
        private readonly IRouteBaseRepository _routeBaseRepository;
        private readonly IPointRepository _pointRepository;

        public RouteBaseService(ICacheManager cacheManager,
            IRouteBaseRepository routeBaseRepository,
            IPointRepository pointRepository) : base(cacheManager)
        {
            _routeBaseRepository = routeBaseRepository;
            _pointRepository = pointRepository;
        }


        public async Task<RouteBase> CreateAsync(RouteBase routeBase)
        {
            // Error if origin and destination are the same point
            PointsValidation.ValidateEqualPointsAsync(routeBase.Origin?.Description, routeBase.Destination?.Description);

            // Error if point description does not exist
            var origin = await PointsValidation.ValidatePointExistsAsync(_pointRepository, routeBase.Origin?.Description);
            var destination = await PointsValidation.ValidatePointExistsAsync(_pointRepository, routeBase.Destination?.Description);

            var routeBaseToAdd = new RouteBase
            {
                Origin = origin,
                Destination = destination
            };

            // Error if route base with such points already exists
            await ValidateRouteBaseAlreadyExistsAsync(routeBaseToAdd);

            await _routeBaseRepository.AddAsync(routeBaseToAdd);

            var routeBases = await _routeBaseRepository.GetAsync();
            var routeReturn = routeBases.FirstOrDefault(x =>
                (x.Origin?.Description?.Equals(routeBaseToAdd.Origin.Description) == true || x.OriginId == routeBaseToAdd.OriginId)
                && (x.Destination?.Description?.Equals(routeBaseToAdd.Destination.Description) == true || x.DestinationId == routeBaseToAdd.DestinationId));

            return routeReturn;
        }


        public async Task<IEnumerable<RouteBase>> GetAsync(RouteBase routeBaseFilter)
        {
            return await _routeBaseRepository.GetAsync(routeBaseFilter);
        }


        public async Task<RouteBase> GetByIdAsync(int id)
        {
            return (await _routeBaseRepository.GetAsync()).FirstOrDefault(x => x.Id == id);
        }


        public async Task UpdateAsync(RouteBase routeBaseUpdate)
        {
            // Error if route base does not exist
            var routeBase = await ValidateRouteBaseExistsAsync(routeBaseUpdate.Id);
            var routeBaseControl = routeBase;

            // Error if origin and destination are the same point
            PointsValidation.ValidateEqualPointsAsync(routeBaseUpdate.Origin?.Description, routeBaseUpdate.Destination?.Description);

            if (!string.IsNullOrWhiteSpace(routeBaseUpdate.Origin?.Description))
            {
                // Error if point description does not exist
                var origin = await PointsValidation.ValidatePointExistsAsync(_pointRepository, routeBaseUpdate.Origin.Description);
                routeBase.Origin = origin;
            }

            if (!string.IsNullOrWhiteSpace(routeBaseUpdate.Destination?.Description))
            {
                // Error if point description does not exist
                var destination = await PointsValidation.ValidatePointExistsAsync(_pointRepository, routeBaseUpdate.Destination.Description);
                routeBase.Destination = destination;
            }

            // Error if route base with such points already exists
            await ValidateRouteBaseAlreadyExistsAsync(routeBase);

            await _routeBaseRepository.UpdateAsync(routeBase);
            RemoveDependentObjectsFromCache(routeBaseControl);
        }


        public async Task DeleteAsync(int id)
        {
            var routeBase = await ValidateRouteBaseExistsAsync(id);
            await _routeBaseRepository.RemoveAsync(routeBase);
            RemoveDependentObjectsFromCache(routeBase);
        }


        #region Private methods

        /// <summary>Validates if the route base already exists.</summary>
        /// <param name="routeBaseCheck">The route base to check.</param>
        private async Task ValidateRouteBaseAlreadyExistsAsync(RouteBase routeBaseCheck)
        {
            if ((await _routeBaseRepository.GetAsync(routeBaseCheck)).Any())
            {
                throw AppCustomExceptions.RouteBaseAlreadyExists;
            }
        }

        /// <summary>Validates if a route base exists.</summary>
        /// <param name="id">The identifier of the route.</param>
        /// <returns>A route base matching the given identifier.</returns>
        private async Task<RouteBase> ValidateRouteBaseExistsAsync(int id)
        {
            var routeBase = (await _routeBaseRepository.GetAsync()).FirstOrDefault(x => x.Id == id);

            // Error if route base does not exist
            if (routeBase == null)
            {
                throw AppCustomExceptions.RouteBaseNotFound;
            }
            return routeBase;
        }

        /// <summary>Removes the objects depending on the route base from cache.</summary>
        /// <param name="routeBase">The route base.</param>
        private void RemoveDependentObjectsFromCache(RouteBase routeBase)
        {
            _cacheManager.RemoveBulkWithExpression(CacheKeys.RouteAllPaths.ComposeCacheKey(
                new string[2] { routeBase.Origin.Id.ToString(), routeBase.Destination.Id.ToString() }));

            _cacheManager.RemoveBulkWithExpression(CacheKeys.RoutePathLeastCost.ComposeCacheKey(
                new string[2] { routeBase.Origin.Id.ToString(), routeBase.Destination.Id.ToString() }));

            _cacheManager.RemoveBulkWithExpression(CacheKeys.RoutePathLeastTime.ComposeCacheKey(
                new string[2] { routeBase.Origin.Id.ToString(), routeBase.Destination.Id.ToString() }));
        }

        #endregion
    }
}