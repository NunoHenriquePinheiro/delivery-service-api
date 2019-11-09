using DeliveryServiceApp.Helpers.Exceptions;
using DeliveryServiceApp.Models.Entities;
using DeliveryServiceApp.Persistence.Repositories.Interfaces;
using System.Linq;
using System.Threading.Tasks;

namespace DeliveryServiceApp.Helpers.Validations
{
    /// <summary>Class with validations related with points, accessible to other classes.</summary>
    public static class PointsValidation
    {
        /// <summary>Validates for equal points (in a route or step).</summary>
        /// <param name="originPoint">The description of the origin point.</param>
        /// <param name="destinationPoint">The description of the destination point.</param>
        public static void ValidateEqualPointsAsync(string originPoint, string destinationPoint)
        {
            if (!string.IsNullOrWhiteSpace(originPoint) && !string.IsNullOrWhiteSpace(destinationPoint) && originPoint.Equals(destinationPoint))
            {
                throw AppCustomExceptions.EqualPointsInRouteStep;
            }
        }

        /// <summary>Validates if a point exists.</summary>
        /// <param name="pointRepository">The point repository.</param>
        /// <param name="description">The description of the point.</param>
        /// <returns>The existing point.</returns>
        public static async Task<Point> ValidatePointExistsAsync(IPointRepository pointRepository, string description)
        {
            var point = (await pointRepository.GetAsync(new Point { Description = description })).FirstOrDefault();
            if (point == null)
            {
                throw AppCustomExceptions.PointNotFound;
            }
            return point;
        }
    }
}
