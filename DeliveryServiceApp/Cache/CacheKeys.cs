namespace DeliveryServiceApp.Cache
{
    /// <summary>Class that holds the keys for the cache items.</summary>
    public static class CacheKeys
    {
        public static string Route { get { return "_Route"; } }
        public static string RouteAllPaths { get { return "_RouteAllPaths"; } }
        public static string RoutePathLeastCost { get { return "_RoutePathLeastCost"; } }
        public static string RoutePathLeastTime { get { return "_RoutePathLeastTime"; } }


        /// <summary>Composes the cache key.</summary>
        /// <param name="genericKey">The generic key.</param>
        /// <param name="customParams">The custom parameters.</param>
        /// <returns>A composed cache key.</returns>
        public static string ComposeCacheKey(this string genericKey, params string[] customParams)
        {
            return genericKey + "_" + string.Join("_", customParams);
        }
    }
}
