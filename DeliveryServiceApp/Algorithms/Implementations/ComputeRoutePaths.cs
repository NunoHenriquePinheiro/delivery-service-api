using DeliveryServiceApp.Algorithms.Calculations;
using DeliveryServiceApp.Algorithms.Interfaces;
using DeliveryServiceApp.Cache;
using DeliveryServiceApp.Models.Entities;
using DeliveryServiceApp.Persistence.Repositories.Interfaces;
using System.Linq;
using System.Threading.Tasks;

namespace DeliveryServiceApp.Algorithms.Implementations
{
    public class ComputeRoutePaths : IComputeRoutePaths
    {
        private readonly IStepRepository _stepRepository;
        private readonly IPointRepository _pointRepository;
        private readonly ICacheManager _cacheManager;

        public ComputeRoutePaths(ICacheManager cacheManager, IStepRepository stepRepository, IPointRepository pointRepository)
        {
            _cacheManager = cacheManager;
            _stepRepository = stepRepository;
            _pointRepository = pointRepository;
        }


        public async Task<Route> GetAllPathsFromRouteAsync(RouteBase routeBase)
        {
            // Get route from cache, if it exists
            var route = GetRouteAllPathsFromCache(routeBase.Origin, routeBase.Destination, out string cacheKey);
            if (route != null)
            {
                return route;
            }

            // Algorithm
            var allPathsCalculator = new AllPathsCalculator(_pointRepository, _stepRepository, routeBase);
            await allPathsCalculator.CalculatePaths();
            route = allPathsCalculator.Route;

            // Set route to cache
            _cacheManager.Set(cacheKey, route);
            return route;
        }


        public async Task<StepsCollection> GetBestOptionPathFromRouteAsync(RouteBase routeBase, PathPerformanceOptions option)
        {
            // Get steps collection from cache, if they exist
            var stepsCollection = GetRouteBestPathFromCache(routeBase.Origin, routeBase.Destination, option, out string cacheKey);
            if (stepsCollection != null)
            {
                return stepsCollection;
            }

            // Algorithm
            var bestPathCalculator = new BestPathCalculator(_pointRepository, _stepRepository, routeBase, option);
            await bestPathCalculator.CalculatePath();
            stepsCollection = bestPathCalculator.StepsCollection;

            // Set steps collection to cache
            _cacheManager.Set(cacheKey, stepsCollection);
            return stepsCollection;
        }


        #region Private methods

        /// <summary>Gets the route all paths from cache.</summary>
        /// <param name="origin">The origin.</param>
        /// <param name="destination">The destination.</param>
        /// <param name="cacheKey">The cache key.</param>
        /// <returns>All possible paths of a route.</returns>
        private Route GetRouteAllPathsFromCache(Point origin, Point destination, out string cacheKey)
        {
            cacheKey = CacheKeys.RouteAllPaths.ComposeCacheKey(new string[2] { origin.Id.ToString(), destination.Id.ToString() });
            if (_cacheManager.TryGet(cacheKey, out Route route))
            {
                return route;
            }
            return null;
        }

        /// <summary>Gets the route best path from cache.</summary>
        /// <param name="origin">The origin.</param>
        /// <param name="destination">The destination.</param>
        /// <param name="option">The option.</param>
        /// <param name="cacheKey">The cache key.</param>
        /// <returns>Collection of steps that constitute the best option path.</returns>
        private StepsCollection GetRouteBestPathFromCache(Point origin, Point destination, PathPerformanceOptions option, out string cacheKey)
        {
            cacheKey = option.Equals(PathPerformanceOptions.Cost) ? CacheKeys.RoutePathLeastCost.ComposeCacheKey(new string[2] { origin.Id.ToString(), destination.Id.ToString() }) :
                CacheKeys.RoutePathLeastTime.ComposeCacheKey(new string[2] { origin.Id.ToString(), destination.Id.ToString() });

            // Get a best option path saved in cache, if there is one
            if (!_cacheManager.TryGet(cacheKey, out StepsCollection stepsCollection))
            {
                // Get all paths from cache, if they are saved, and select the best one
                var route = GetRouteAllPathsFromCache(origin, destination, out _);

                stepsCollection = option.Equals(PathPerformanceOptions.Cost) ? route?.StepsCollectionList?.OrderBy(x => x.TotalCost).FirstOrDefault() :
                    route?.StepsCollectionList?.OrderBy(x => x.TotalTime).FirstOrDefault();

                if (stepsCollection != null)
                {
                    _cacheManager.Set(cacheKey, stepsCollection);
                }
            }

            return stepsCollection;
        }

        #endregion
    }
}
