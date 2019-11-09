using DeliveryServiceApp.Algorithms.Holders;
using DeliveryServiceApp.Models.Entities;
using DeliveryServiceApp.Persistence.Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeliveryServiceApp.Algorithms.Calculations
{
    public class AllPathsCalculator
    {
        private readonly IPointRepository _pointRepository;
        private readonly IStepRepository _stepRepository;

        private readonly List<PointSteps> _pointStepsList;
        private List<Point> _isVisited;

        public Route Route { get; }


        /// <summary>Constructor to the calculator of all paths between an origin and a destination point.</summary>
        /// <param name="pointRepository">Point repository.</param>
        /// <param name="stepRepository">Step repository.</param>
        /// <param name="routeBase">Route base holding the origin and destination points.</param>
        public AllPathsCalculator(IPointRepository pointRepository, IStepRepository stepRepository, RouteBase routeBase)
        {
            _pointRepository = pointRepository;
            _stepRepository = stepRepository;
            _pointStepsList = new List<PointSteps>();
            _isVisited = new List<Point>();

            Route = new Route
            {
                Id = routeBase.Id,
                Origin = routeBase.Origin,
                OriginId = routeBase.Origin.Id,
                Destination = routeBase.Destination,
                DestinationId = routeBase.Destination.Id,
                StepsCollectionList = new List<StepsCollection>()
            };
        }

        /// <summary>Calculates all the possible paths in the calculator.</summary>
        public async Task CalculatePaths()
        {
            await GetAllPointSteps();

            // BUSINESS REQUIREMENT: Avoid steps that connect directly the Origin and the Destination
            var firstSteps = _pointStepsList.FirstOrDefault(x => x.Point.Equals(Route.Origin)).Steps.Where(y => !y.End.Equals(Route.Destination));
            
            foreach (var firstStep in firstSteps)
            {
                _isVisited = new List<Point>()
                {
                    Route.Origin
                };

                var newStepList = new List<Step>()
                {
                    firstStep
                };
                GetFollowingStep(firstStep.End, newStepList);
            }
        }


        #region Private methods

        /// <summary>Gets all the steps started by each point.</summary>
        private async Task GetAllPointSteps()
        {
            var allPoints = await _pointRepository.GetAsync();
            foreach (var point in allPoints)
            {
                var steps = await _stepRepository.GetAsync(new Step { Start = point });
                _pointStepsList.Add(new PointSteps
                {
                    Point = point,
                    Steps = steps.ToList()
                });
            }
        }

        /// <summary>Gets the following step(s) for a given point.</summary>
        /// <param name="nextPoint">The next point.</param>
        /// <param name="previousStepsList">The steps list being iterated.</param>
        private void GetFollowingStep(Point nextPoint, List<Step> previousStepsList)
        {
            var nextSteps = _pointStepsList.FirstOrDefault(x => x.Point.Equals(nextPoint)).Steps.Where(y => !_isVisited.Contains(y.End));
            _isVisited.Add(nextPoint);

            foreach(var nextStep in nextSteps)
            {
                var stepsList = previousStepsList;
                stepsList.Add(nextStep);

                var newPoint = nextStep.End;
                if (newPoint.Equals(Route.Destination))
                {
                    Route.StepsCollectionList.Add(new StepsCollection
                    {
                        Steps = stepsList,
                        TotalCost = stepsList.Sum(x => x.Cost),
                        TotalTime = stepsList.Sum(x => x.Time)
                    });
                }
                else
                {
                    GetFollowingStep(newPoint, stepsList);
                }
            }
        }

        #endregion
    }
}
