using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Caching;

namespace CambridgeSoft.COE.Framework.Caching
{
    /// <summary>
    /// Base class for building dependencies to be used in the cache machinery. If not inherited it represents a File dependency.
    /// </summary>
    public class COECacheDependency : CacheDependency
    {
        /// <summary>
        /// Builds a generic dependency to be used for inheritors only
        /// </summary>
        protected COECacheDependency() : base() { }

        /// <summary>
        ///  Initializes a new instance of the COECacheDependency class that monitors a file or directory for changes.
        /// </summary>
        /// <param name="fileName">The path to a file or directory that the cached object is dependent upon. When this resource changes, the cached object becomes obsolete and is removed from the cache.</param>
        public COECacheDependency(string fileName) : base(fileName) { }
 
        /// <summary>
        /// Initializes a new instance of the COECacheDependency class that monitors a cache keys for changes.
        /// </summary>
        /// <param name="cachekey">A cache key that the new object monitors for changes. When this cache key changes, the cached object associated with this dependency object becomes obsolete and is removed from the cache.</param>
        /// <param name="cacheType">The cached key type</param>
        public COECacheDependency(string cachekey, Type cacheType)
            : base(null, new string[] { cachekey + "_" + cacheType.Name })
        {
        }

        /// <summary>
        /// Initializes a new instance of the CacheDependency class that monitors an array of paths (to files or directories), an array of cache keys, or both for changes.
        /// </summary>
        /// <param name="fileKeys">An array of paths (to files or directories) that the cached object is dependent upon. When any of these resources changes, the cached object becomes obsolete and is removed from the cache.</param>
        /// <param name="cacheKeys">An array of cache keys that the new object monitors for changes. When any of these cache keys changes, the cached object associated with this dependency object becomes obsolete and is removed from the cache.</param>
        public COECacheDependency(string[] fileKeys, string[] cacheKeys)
            : base(fileKeys, cacheKeys)
        {
        }
    }

    /// <summary>
    /// Cache priority
    /// </summary>
    public enum COECacheItemPriority
    {
        AboveNormal = CacheItemPriority.AboveNormal,
        BelowNormal = CacheItemPriority.BelowNormal,
        Default = CacheItemPriority.Default,
        High = CacheItemPriority.High,
        Low = CacheItemPriority.Low,
        Normal = CacheItemPriority.Normal,
        NotRemovable = CacheItemPriority.NotRemovable
    }

    /// <summary>
    /// List of reasons why an item is removed from cache
    /// </summary>
    public enum COECacheItemRemovedReason
    {
        /// <summary>
        /// The underlying dependency changed invalidating the cached object
        /// </summary>
        DependencyChanged = CacheItemRemovedReason.DependencyChanged,
        /// <summary>
        /// The item is invalid due to time expiration. (Wheter its absolute or sliding time)
        /// </summary>
        Expired = CacheItemRemovedReason.Expired,
        /// <summary>
        /// The item was explicitly removed from cache.
        /// </summary>
        Removed = CacheItemRemovedReason.Removed,
        /// <summary>
        /// The cache needed space and removed the ite as it is the most unused
        /// </summary>
        Underused = CacheItemRemovedReason.Underused
    }

    /// <summary>
    /// Delegate for notifying that an item was removed from cache and the reasons of that removal.
    /// </summary>
    /// <param name="key">The key in the cache</param>
    /// <param name="value">The object removed</param>
    /// <param name="reason">The reason why it was removed</param>
    public delegate void COECacheItemRemovedCallback(string key, object value, COECacheItemRemovedReason reason);
}
