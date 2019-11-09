using DeliveryServiceApp.Algorithms.Holders;
using DeliveryServiceApp.Models.Entities;
using DeliveryServiceApp.Persistence.Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeliveryServiceApp.Algorithms.Calculations
{
    public class BestPathCalculator
    {
        private readonly IPointRepository _pointRepository;
        private readonly IStepRepository _stepRepository;

        private readonly RouteBase _routeBase;
        private readonly PathPerformanceOptions _pathOption;
        private readonly List<PointSteps> _pointStepsList;

        private List<Point> _isVisited;

        public StepsCollection StepsCollection { get; }


        /// <summary>Constructor to the calculator of the best path between an origin and a destination point.</summary>
        /// <param name="pointRepository">Point repository.</param>
        /// <param name="stepRepository">Step repository.</param>
        /// <param name="routeBase">Route base holding the origin and destination points.</param>
        public BestPathCalculator(IPointRepository pointRepository, IStepRepository stepRepository, RouteBase routeBase, PathPerformanceOptions option)
        {
            _pointRepository = pointRepository;
            _stepRepository = stepRepository;
            _routeBase = routeBase;
            _pathOption = option;
            _pointStepsList = new List<PointSteps>();

            _isVisited = new List<Point>();

            StepsCollection = new StepsCollection();
        }

        /// <summary>Calculates the best path in the calculator.</summary>
        public async Task CalculatePath()
        {
            await GetAllPointSteps();

            // BUSINESS REQUIREMENT: Avoid steps that connect directly the Origin and the Destination
            var firstSteps = _pointStepsList.FirstOrDefault(x => x.Point.Equals(_routeBase.Origin)).Steps.Where(y => !y.End.Equals(_routeBase.Destination));
            if (_pathOption.Equals(PathPerformanceOptions.Cost))
            {
                firstSteps = firstSteps.OrderBy(z => z.Cost).ToList();
            }
            else
            {
                firstSteps = firstSteps.OrderBy(z => z.Time).ToList();
            }

            foreach (var firstStep in firstSteps)
            {
                _isVisited = new List<Point>()
                {
                    _routeBase.Origin
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
                    Steps = _pathOption.Equals(PathPerformanceOptions.Cost) ? steps.OrderBy(x => x.Cost).ToList() : steps.OrderBy(x => x.Time).ToList()
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

            foreach (var nextStep in nextSteps)
            {
                var stepsList = previousStepsList;
                stepsList.Add(nextStep);
                
                // Current performance = current total cost OR current total time
                var currentPerformance = _pathOption.Equals(PathPerformanceOptions.Cost) ? stepsList.Sum(x => x.Cost) : stepsList.Sum(x => x.Time);

                // If any StepsCollection is still not defined OR the current performance is less than the saved StepsCollection, the operation continues
                if (StepsCollection.Steps?.Any() != true ||
                    currentPerformance < (_pathOption.Equals(PathPerformanceOptions.Cost) ? StepsCollection.TotalCost : StepsCollection.TotalTime))
                {
                    var newPoint = nextStep.End;
                    if (newPoint.Equals(_routeBase.Destination))
                    {
                        StepsCollection.Steps = stepsList;
                        StepsCollection.TotalCost = stepsList.Sum(x => x.Cost);
                        StepsCollection.TotalTime = stepsList.Sum(x => x.Time);
                    }
                    else
                    {
                        GetFollowingStep(newPoint, stepsList);
                    }
                }
            }
        }

        #endregion
    }
}
