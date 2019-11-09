using DeliveryServiceApp.Helpers.Exceptions;
using DeliveryServiceApp.Models.Entities;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace DeliveryServiceApp.Tests
{
    /// <summary>Tests class for "route bases".</summary>
    /// <seealso cref="TestsBase" />
    public class RoutesTests : TestsBase
    {
        public RoutesTests() : base() { }

        /// <summary>Creates, updates and deletes a new route base. Expects a successful completion.</summary>
        /// <param name="pointOrigin">The point origin.</param>
        /// <param name="pointDestination">The point destination.</param>
        /// <param name="updatedDestination">The updated destination.</param>
        [Theory]
        [InlineData("C", "F", "B")]
        public async Task CreateUpdateDelete_NewRouteBase_ReturnsCompleted(string pointOrigin, string pointDestination, string updatedDestination)
        {
            try
            {
                // CREATE
                var routeBase = await _routeBaseService.CreateAsync(new RouteBase
                {
                    Origin = new Point { Description = pointOrigin },
                    Destination = new Point { Description = pointDestination }
                });
                var isCreated = routeBase.Origin.Description.Equals(pointOrigin) && routeBase.Destination.Description.Equals(pointDestination);

                Assert.NotNull(routeBase);
                Assert.True(isCreated);

                // UPDATE
                var routeUpdated = new RouteBase
                {
                    Id = routeBase.Id,
                    Destination = new Point { Description = updatedDestination }
                };
                await _routeBaseService.UpdateAsync(routeUpdated);

                var routeVerified = await _routeBaseService.GetByIdAsync(routeBase.Id);
                var isUpdated = routeVerified.Origin.Description.Equals(pointOrigin) && routeVerified.Destination.Description.Equals(updatedDestination);

                Assert.True(isUpdated);

                // DELETE
                await _routeBaseService.DeleteAsync(routeBase.Id);

                var routeBaseDeleted = await _routeBaseService.GetByIdAsync(routeBase.Id);
                var isDeleted = routeBaseDeleted == null;

                Assert.True(isDeleted);
            }
            catch (AppCustomException)
            {
                Assert.True(false);
            }
        }

        /// <summary>Gets a predefined route base by its origin and destination. Expects the return of one route base.</summary>
        /// <param name="pointOrigin">The point origin.</param>
        /// <param name="pointDestination">The point destination.</param>
        [Theory]
        [InlineData("A", "B")]
        public async Task Get_PredefinedRouteBase_ReturnsOneRouteBase(string pointOrigin, string pointDestination)
        {
            try
            {
                var routeBaseFilter = new RouteBase
                {
                    Origin = new Point
                    {
                        Description = pointOrigin
                    },
                    Destination = new Point
                    {
                        Description = pointDestination
                    }
                };

                var collectList = await _routeBaseService.GetAsync(routeBaseFilter);
                var isOne = collectList.Count() == 1;

                Assert.True(isOne);
            }
            catch (AppCustomException)
            {
                Assert.True(false);
            }
        }

        /// <summary>Gets a predefined route base by its identifier. Expects the return of a route base.</summary>
        /// <param name="id">The identifier.</param>
        [Theory]
        [InlineData(1)]
        public async Task GetById_PredefinedRouteBase_ReturnsRouteBase(int id)
        {
            try
            {
                var chosen = await _routeBaseService.GetByIdAsync(id);

                Assert.NotNull(chosen);
            }
            catch (AppCustomException)
            {
                Assert.True(false);
            }
        }
    }
}
