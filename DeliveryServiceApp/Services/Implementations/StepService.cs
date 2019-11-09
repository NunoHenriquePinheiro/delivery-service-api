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
    public class StepService : BaseService, IStepService
    {
        private readonly IStepRepository _stepRepository;
        private readonly IPointRepository _pointRepository;

        public StepService(ICacheManager cacheManager,
            IStepRepository stepRepository,
            IPointRepository pointRepository) : base(cacheManager)
        {
            _stepRepository = stepRepository;
            _pointRepository = pointRepository;
        }


        public async Task<Step> CreateAsync(Step step)
        {
            // Error if start and end are the same point
            PointsValidation.ValidateEqualPointsAsync(step.Start?.Description, step.End?.Description);

            // Error if point description does not exist
            var start = await PointsValidation.ValidatePointExistsAsync(_pointRepository, step.Start?.Description);
            var end = await PointsValidation.ValidatePointExistsAsync(_pointRepository, step.End?.Description);

            var stepToAdd = new Step
            {
                Start = start,
                End = end
            };

            // Error if step with such points already exists
            await ValidateStepAlreadyExistsAsync(stepToAdd);

            // Error if time/cost is zero or less
            ValidatePositiveTimeCost(step.Cost);
            ValidatePositiveTimeCost(step.Time);

            stepToAdd.Cost = step.Cost;
            stepToAdd.Time = step.Time;

            await _stepRepository.AddAsync(stepToAdd);
            _cacheManager.RemoveBulkWithExpression(CacheKeys.Route);

            return (await _stepRepository.GetAsync()).FirstOrDefault(x =>
                (x.Start?.Description?.Equals(stepToAdd.Start.Description) == true || x.StartId == stepToAdd.Start.Id)
                && (x.End?.Description?.Equals(stepToAdd.End.Description) == true || x.EndId == stepToAdd.End.Id));
        }


        public async Task<IEnumerable<Step>> GetAsync(Step stepFilter)
        {
            return await _stepRepository.GetAsync(stepFilter);
        }


        public async Task<Step> GetByIdAsync(int id)
        {
            return (await _stepRepository.GetAsync()).FirstOrDefault(x => x.Id == id);
        }


        public async Task UpdateAsync(Step stepUpdate)
        {
            var step = await ValidateStepExistsAsync(stepUpdate.Id);

            // Error if start and end are the same point
            PointsValidation.ValidateEqualPointsAsync(stepUpdate.Start?.Description, stepUpdate.End?.Description);

            if (!string.IsNullOrWhiteSpace(stepUpdate.Start?.Description))
            {
                // Error if point description does not exist
                var start = await PointsValidation.ValidatePointExistsAsync(_pointRepository, stepUpdate.Start.Description);
                step.Start = start;
            }

            if (!string.IsNullOrWhiteSpace(stepUpdate.End?.Description))
            {
                // Error if point description does not exist
                var end = await PointsValidation.ValidatePointExistsAsync(_pointRepository, stepUpdate.End.Description);
                step.End = end;
            }

            // Error if step with such points already exists
            await ValidateStepAlreadyExistsAsync(step);

            if (stepUpdate.Cost > 0)
            {
                step.Cost = stepUpdate.Cost;
            }
            if (stepUpdate.Time > 0)
            {
                step.Time = stepUpdate.Time;
            }

            await _stepRepository.UpdateAsync(step);
            _cacheManager.RemoveBulkWithExpression(CacheKeys.Route);
        }


        public async Task DeleteAsync(int id)
        {
            var step = await ValidateStepExistsAsync(id);
            await _stepRepository.RemoveAsync(step);
            _cacheManager.RemoveBulkWithExpression(CacheKeys.Route);
        }


        #region Private methods

        /// <summary>Validates decimals (time and cost) for positive values.</summary>
        /// <param name="number">The number.</param>
        private void ValidatePositiveTimeCost(decimal number)
        {
            if (number <= 0)
            {
                throw AppCustomExceptions.StepTimeCostPositive;
            }
        }

        /// <summary>Validates if the step base already exists.</summary>
        /// <param name="stepCheck">The step to check.</param>
        private async Task ValidateStepAlreadyExistsAsync(Step stepCheck)
        {
            if ((await _stepRepository.GetAsync(stepCheck)).Any())
            {
                throw AppCustomExceptions.StepAlreadyExists;
            }
        }

        /// <summary>Validates if a step exists.</summary>
        /// <param name="id">The identifier of the step.</param>
        /// <returns>A step matching the given identifier.</returns>
        private async Task<Step> ValidateStepExistsAsync(int id)
        {
            var step = (await _stepRepository.GetAsync()).FirstOrDefault(x => x.Id == id);

            // Error if step does not exist
            if (step == null)
            {
                throw AppCustomExceptions.StepNotFound;
            }
            return step;
        }

        #endregion
    }
}