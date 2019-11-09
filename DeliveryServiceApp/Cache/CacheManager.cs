using DeliveryServiceApp.Helpers.Settings;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DeliveryServiceApp.Cache
{
    public class CacheManager : ICacheManager
    {
        private readonly IMemoryCache _cache;
        private readonly bool _useCache;
        private readonly MemoryCacheEntryOptions _cacheOptions;

        // Used to list the maximum amount of keys that are identifying cache items at a moment
        private static string ListKeysInUse { get { return "_ListKeysInUse"; } }

        public CacheManager(IMemoryCache memoryCache, IOptions<AppSettings> appSettings)
        {
            _cache = memoryCache;
            _useCache = !bool.TryParse(appSettings.Value?.CacheOptions?.UseCache, out bool useCache) || useCache;

            var expireTime = double.TryParse(appSettings.Value?.CacheOptions?.ExpireTimeMinutes, out double cacheExpireTime) ? cacheExpireTime : 5; // in minutes, respecting the appsetting
            _cacheOptions = new MemoryCacheEntryOptions()
            {
                SlidingExpiration = TimeSpan.FromMinutes(expireTime),
                Priority = CacheItemPriority.Normal
            };
        }

        public bool TryGet<T>(string cacheKey, out T outValue)
        {
            outValue = default;
            if (_useCache && _cache.TryGetValue(cacheKey, out outValue))
            {
                return true;
            }
            return false;
        }

        public void Set<T>(string cacheKey, T inValue)
        {
            if (_useCache && !cacheKey.Equals(ListKeysInUse))
            {
                _cache.Set(cacheKey, inValue, _cacheOptions);
                RegisterKey(cacheKey);
            }
        }

        public void Remove(string cacheKey)
        {
            if (_useCache && !cacheKey.Equals(ListKeysInUse))
            {
                _cache.Remove(cacheKey);
                RemoveKey(cacheKey);
            }
        }

        public void RemoveBulkWithExpression(string keyExpression)
        {
            if (_useCache && !keyExpression.Equals(ListKeysInUse))
            {
                RemoveBulkByExpressionKey(keyExpression);
            }
        }


        #region Private methods

        private void RegisterKey(string newCacheKey)
        {
            _cache.TryGetValue(ListKeysInUse, out List<string> keysList);
            
            keysList ??= new List<string>();
            if (!keysList.Contains(newCacheKey))
            {
                keysList.Add(newCacheKey);
            }

            _cache.Set(ListKeysInUse, keysList, _cacheOptions);
        }

        private void RemoveKey(string oldCacheKey)
        {
            _cache.TryGetValue(ListKeysInUse, out List<string> keysList);
            keysList?.RemoveAll(x => x.Equals(oldCacheKey));

            _cache.Set(ListKeysInUse, keysList, _cacheOptions);
        }

        private void RemoveBulkByExpressionKey(string keyExpression)
        {
            _cache.TryGetValue(ListKeysInUse, out List<string> keysList);

            var keysToRemove = keysList?.Where(x => x.Contains(keyExpression));
            if (keysToRemove != null)
            {
                foreach (var cacheKey in keysToRemove)
                {
                    _cache.Remove(cacheKey);
                }
                keysList.RemoveAll(x => keysToRemove?.Contains(x) == true);

                _cache.Set(ListKeysInUse, keysList, _cacheOptions);
            }
        }

        #endregion
    }
}
