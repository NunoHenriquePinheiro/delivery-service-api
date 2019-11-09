namespace DeliveryServiceApp.Cache
{
    /// <summary>Interface that allows the management of information in cache.</summary>
    public interface ICacheManager
    {
        /// <summary>Tries to get an object from cache.</summary>
        /// <typeparam name="T">The type of the object to get from cache.</typeparam>
        /// <param name="cacheKey">The cache key.</param>
        /// <param name="outValue">The value of the gotten object (output).</param>
        /// <returns>True, if an object was returned from cache. Otherwise, false.</returns>
        bool TryGet<T>(string cacheKey, out T outValue);

        /// <summary>Sets an object to cache.</summary>
        /// <typeparam name="T">The type of the object to set to cache.</typeparam>
        /// <param name="cacheKey">The cache key.</param>
        /// <param name="inValue">The value of the object to set to cache.</param>
        void Set<T>(string cacheKey, T inValue);

        /// <summary>Removes an object from cache.</summary>
        /// <param name="cacheKey">The cache key.</param>
        void Remove(string cacheKey);

        /// <summary>Removes from cache all the objects whose key contains the specified expression.</summary>
        /// <param name="keyExpression">The key expression.</param>
        void RemoveBulkWithExpression(string keyExpression);
    }
}
