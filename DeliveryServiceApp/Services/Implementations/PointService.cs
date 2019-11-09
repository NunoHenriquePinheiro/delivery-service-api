using DeliveryServiceApp.Cache;
using DeliveryServiceApp.Helpers.Exceptions;
using DeliveryServiceApp.Models.Entities;
using DeliveryServiceApp.Persistence.Repositories.Interfaces;
using DeliveryServiceApp.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeliveryServiceApp.Services.Implementations
{
    public class PointService : BaseService, IPointService
    {
        private readonly IPointRepository _pointRepository;

        public PointService(ICacheManager cacheManager,
            IPointRepository pointRepository) : base(cacheManager)
        {
            _pointRepository = pointRepository;
        }


        public async Task<Point> CreateAsync(string description)
        {
            // Error if point description is invalid value or already in use
            await ValidatePointInvalidAsync(description);

            await _pointRepository.AddAsync(new Point { Description = description });
            _cacheManager.RemoveBulkWithExpression(CacheKeys.Route);

            return (await _pointRepository.GetAsync()).FirstOrDefault(x => x.Description.Equals(description));
        }


        public async Task<IEnumerable<Point>> GetAsync(Point pointFilter)
        {
            return await _pointRepository.GetAsync(pointFilter);
        }


        public async Task<Point> GetByIdAsync(int id)
        {
            return (await _pointRepository.GetAsync()).FirstOrDefault(x => x.Id == id);
        }


        public async Task UpdateAsync(Point pointUpdate)
        {
            var point = await ValidatePointExistsAsync(pointUpdate.Id);

            // Error if point description is invalid value or already in use
            await ValidatePointInvalidAsync(pointUpdate.Description);
            point.Description = pointUpdate.Description;

            await _pointRepository.UpdateAsync(point);
            _cacheManager.RemoveBulkWithExpression(CacheKeys.Route);
        }


        public async Task DeleteAsync(int id)
        {
            var point = await ValidatePointExistsAsync(id);
            await _pointRepository.RemoveAsync(point);
            _cacheManager.RemoveBulkWithExpression(CacheKeys.Route);
        }


        #region Private methods

        /// <summary>Validates if a point is invalid (invalid value or already in use).</summary>
        /// <param name="role">The description of the point.</param>
        private async Task ValidatePointInvalidAsync(string description)
        {
            if (string.IsNullOrWhiteSpace(description))
            {
                throw AppCustomExceptions.PointInvalidValue;
            }
            else if ((await _pointRepository.GetAsync(new Point { Description = description })).Any())
            {
                throw AppCustomExceptions.PointDescriptionAlreadyInUse;
            }
        }

        /// <summary>Validates if a point exists.</summary>
        /// <param name="id">The identifier of the point.</param>
        /// <returns>A point matching the given identifier.</returns>
        private async Task<Point> ValidatePointExistsAsync(int id)
        {
            var point = (await _pointRepository.GetAsync()).FirstOrDefault(x => x.Id == id);

            // Error if point does not exist
            if (point == null)
            {
                throw AppCustomExceptions.PointNotFound;
            }
            return point;
        }

        #endregion
    }
}