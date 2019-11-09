using DeliveryServiceApp.Helpers.Exceptions;
using DeliveryServiceApp.Models.Entities;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace DeliveryServiceApp.Tests
{
    /// <summary>Tests class for "points".</summary>
    /// <seealso cref="TestsBase" />
    public class PointsTests : TestsBase
    {
        public PointsTests() : base() { }

        /// <summary>Creates, updates and deletes a new point. Expects a successful completion.</summary>
        /// <param name="pointDescription">The point description.</param>
        [Theory]
        [InlineData("pointN", "pointUpdated")]
        public async Task CreateUpdateDelete_NewPoint_ReturnsCompleted(string pointDescription, string updatedDescription)
        {
            try
            {
                // CREATE
                var point = await _pointService.CreateAsync(pointDescription);
                var isCreated = point.Description.Equals(pointDescription);

                Assert.NotNull(point);
                Assert.True(isCreated);

                // UPDATE
                var pointUpdated = new Point
                {
                    Id = point.Id,
                    Description = updatedDescription
                };
                await _pointService.UpdateAsync(pointUpdated);

                var pointVerified = await _pointService.GetByIdAsync(point.Id);
                var isUpdated = pointVerified.Description == updatedDescription;

                Assert.True(isUpdated);

                // DELETE
                await _pointService.DeleteAsync(point.Id);

                var pointDeleted = await _pointService.GetByIdAsync(point.Id);
                var isDeleted = pointDeleted == null;

                Assert.True(isDeleted);
            }
            catch (AppCustomException)
            {
                Assert.True(false);
            }
        }

        /// <summary>Gets a predefined point by its description. Expects the return of one point.</summary>
        /// <param name="description">The description.</param>
        [Theory]
        [InlineData("A")]
        public async Task Get_PredefinedPoint_ReturnsOnePoint(string description)
        {
            try
            {
                var collectList = await _pointService.GetAsync(new Point { Description = description });
                var isOne = collectList.Count() == 1;

                Assert.True(isOne);
            }
            catch (AppCustomException)
            {
                Assert.True(false);
            }
        }

        /// <summary>Gets a predefined point by its identifier. Expects the return of a point.</summary>
        /// <param name="id">The identifier.</param>
        [Theory]
        [InlineData(1)]
        public async Task GetById_PredefinedPoint_ReturnsPoint(int id)
        {
            try
            {
                var chosen = await _pointService.GetByIdAsync(id);

                Assert.NotNull(chosen);
            }
            catch (AppCustomException)
            {
                Assert.True(false);
            }
        }
    }
}
