using DeliveryServiceApp.Helpers.Exceptions;
using DeliveryServiceApp.Models.Entities;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace DeliveryServiceApp.Tests
{
    /// <summary>Tests class for "steps".</summary>
    /// <seealso cref="TestsBase" />
    public class StepsTests : TestsBase
    {
        public StepsTests() : base() { }

        /// <summary>Creates, updates and deletes a new step. Expects a successful completion.</summary>
        /// <param name="pointOrigin">The point origin.</param>
        /// <param name="pointDestination">The point destination.</param>
        /// <param name="updatedDestination">The updated destination.</param>
        [Theory]
        [InlineData("E", "A", "C")]
        public async Task CreateUpdateDelete_NewStep_ReturnsCompleted(string pointOrigin, string pointDestination, string updatedDestination)
        {
            try
            {
                // CREATE
                var step = await _stepService.CreateAsync(new Step
                {
                    Start = new Point { Description = pointOrigin },
                    End = new Point { Description = pointDestination },
                    Time = 3,
                    Cost = 4
                });
                var isCreated = step.Start.Description.Equals(pointOrigin) && step.End.Description.Equals(pointDestination);

                Assert.NotNull(step);
                Assert.True(isCreated);

                // UPDATE
                var stepUpdated = new Step
                {
                    Id = step.Id,
                    End = new Point { Description = updatedDestination }
                };
                await _stepService.UpdateAsync(stepUpdated);

                var stepVerified = await _stepService.GetByIdAsync(step.Id);
                var isUpdated = stepVerified.Start.Description.Equals(pointOrigin) && stepVerified.End.Description.Equals(updatedDestination);

                Assert.True(isUpdated);

                // DELETE
                await _stepService.DeleteAsync(step.Id);

                var stepDeleted = await _stepService.GetByIdAsync(step.Id);
                var isDeleted = stepDeleted == null;

                Assert.True(isDeleted);
            }
            catch (AppCustomException)
            {
                Assert.True(false);
            }
        }

        /// <summary>Gets a predefined step by its origin and destination. Expects the return of one step.</summary>
        /// <param name="pointOrigin">The point origin.</param>
        /// <param name="pointDestination">The point destination.</param>
        [Theory]
        [InlineData("A", "C")]
        public async Task Get_PredefinedStep_ReturnsOneStep(string pointOrigin, string pointDestination)
        {
            try
            {
                var stepFilter = new Step
                {
                    Start = new Point
                    {
                        Description = pointOrigin
                    },
                    End = new Point
                    {
                        Description = pointDestination
                    }
                };

                var collectList = await _stepService.GetAsync(stepFilter);
                var isOne = collectList.Count() == 1;

                Assert.True(isOne);
            }
            catch (AppCustomException)
            {
                Assert.True(false);
            }
        }

        /// <summary>Gets a predefined step by its identifier. Expects the return of a step.</summary>
        /// <param name="id">The identifier.</param>
        [Theory]
        [InlineData(1)]
        public async Task GetById_PredefinedStep_ReturnsStep(int id)
        {
            try
            {
                var chosen = await _stepService.GetByIdAsync(id);

                Assert.NotNull(chosen);
            }
            catch (AppCustomException)
            {
                Assert.True(false);
            }
        }
    }
}
