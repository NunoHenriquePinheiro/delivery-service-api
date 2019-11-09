using DeliveryServiceApp.Helpers.Exceptions;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace DeliveryServiceApp.Tests
{
    /// <summary>Tests class for "route steps".</summary>
    /// <seealso cref="TestsBase" />
    public class RouteStepsTests : TestsBase
    {
        public RouteStepsTests() : base() { }

        /// <summary>Gets the predefined steps route with the least time, between an origin and destination. Expects the matching between the result of two calculation methods.</summary>
        /// <param name="pointOrigin">The point origin.</param>
        /// <param name="pointDestination">The point destination.</param>
        [Theory]
        [InlineData("A", "B")]
        public async Task Get_PredefinedRouteSteps_ReturnsLeastTime(string pointOrigin, string pointDestination)
        {
            try
            {
                // GET LEAST TIME FROM ALL STEPS LIST
                var routeStepsLists = await _routeStepService.GetRouteAllStepCollectionsAsync(pointOrigin, pointDestination);
                var stepListAllLeastTime = routeStepsLists.StepsCollectionList.OrderBy(x => x.TotalTime).FirstOrDefault();
                var hasValidStepsLeastTime1 = stepListAllLeastTime?.Steps?.Count > 1;

                Assert.NotNull(stepListAllLeastTime);
                Assert.True(hasValidStepsLeastTime1);

                // GET BEST TIME STEPS LIST
                var stepListLeastTime = await _routeStepService.GetStepCollectionLeastTimeAsync(pointOrigin, pointDestination);
                var hasValidStepsLeastTime2 = stepListAllLeastTime?.Steps?.Count > 1;

                Assert.NotNull(stepListLeastTime);
                Assert.True(hasValidStepsLeastTime2);

                // Verify if the time of the two lists match
                var isSameTime = stepListAllLeastTime.TotalTime == stepListLeastTime.TotalTime;

                Assert.True(isSameTime);
            }
            catch (AppCustomException)
            {
                Assert.True(false);
            }
        }

        /// <summary>Gets the predefined steps route with the least cost, between an origin and destination. Expects the matching between the result of two calculation methods.</summary>
        /// <param name="pointOrigin">The point origin.</param>
        /// <param name="pointDestination">The point destination.</param>
        [Theory]
        [InlineData("A", "B")]
        public async Task Get_PredefinedRouteSteps_ReturnsLeastCost(string pointOrigin, string pointDestination)
        {
            try
            {
                // GET LEAST COST FROM ALL STEPS LIST
                var routeStepsLists = await _routeStepService.GetRouteAllStepCollectionsAsync(pointOrigin, pointDestination);
                var stepListAllLeastCost = routeStepsLists.StepsCollectionList.OrderBy(x => x.TotalCost).FirstOrDefault();
                var hasValidStepsLeastCost1 = stepListAllLeastCost?.Steps?.Count > 1;

                Assert.NotNull(stepListAllLeastCost);
                Assert.True(hasValidStepsLeastCost1);

                // GET BEST COST STEPS LIST
                var stepListLeastCost = await _routeStepService.GetStepCollectionLeastCostAsync(pointOrigin, pointDestination);
                var hasValidStepsLeastCost2 = stepListAllLeastCost?.Steps?.Count > 1;

                Assert.NotNull(stepListLeastCost);
                Assert.True(hasValidStepsLeastCost2);

                // Verify if the time of the two lists match
                var isSameTime = stepListAllLeastCost.TotalCost == stepListLeastCost.TotalCost;

                Assert.True(isSameTime);
            }
            catch (AppCustomException)
            {
                Assert.True(false);
            }
        }
    }
}
