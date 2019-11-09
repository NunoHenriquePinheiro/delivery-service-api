using DeliveryServiceApp.Cache;

namespace DeliveryServiceApp.Services
{
    public class BaseService
    {
        protected readonly ICacheManager _cacheManager;

        public BaseService(ICacheManager cacheManager)
        {
            _cacheManager = cacheManager;
        }
    }
}