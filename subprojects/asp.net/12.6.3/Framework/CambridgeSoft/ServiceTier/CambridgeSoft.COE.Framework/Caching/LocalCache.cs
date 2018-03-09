using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Caching;
using System.Web;
using System.Runtime.Serialization;

namespace CambridgeSoft.COE.Framework.Caching
{
    /// <summary>
    /// A cache that resides in the current tier/application domain.
    /// </summary>
    public static class LocalCache
    {
        /// <summary>
        /// Used in the absoluteExpiration parameter while adding objects to LocalCache to indicate the item should never expire. 
        /// This field is read-only.
        /// </summary>
        public static readonly DateTime NoAbsoluteExpiration = Cache.NoAbsoluteExpiration;

        /// <summary>
        /// Used as the slidingExpiration parameter while adding objects to LocalCache to disable sliding expirations.
        /// This field is read-only.
        /// </summary>
        public static readonly TimeSpan NoSlidingExpiration = Cache.NoSlidingExpiration;

        /// <summary>
        /// Retrieves the specified item from the Local Cache
        /// </summary>
        /// <param name="key">The identifier for the cache item to retrieve.</param>
        /// <param name="type">The type of the object to get.</param>
        /// <returns>The retrieved cache item, or null if the key is not found.</returns>
        public static object Get(string key, Type type)
        {
            //get the date key to keep it alive at the same times as the cached object
            HttpRuntime.Cache.Get(BuildDateKey(key, type));

            return HttpRuntime.Cache.Get(BuildKey(key, type));
        }

        /// <summary>
        /// Retrieves the cached date for a cached item.
        /// This cached date is not part of the cached object itself but is instead a parallel item that
        /// is kept alive and destroyed as needed.
        /// </summary>
        /// <param name="key">The identifier for the cache item to retrieve.</param>
        /// <param name="type">The type of the object to get.</param>
        /// <returns>The retrieved cache item, or null if the key is not found.</returns>
        public static object GetDate(string key, Type type)
        {
            //get the object to keep the cached object alive/synced with the date
            HttpRuntime.Cache.Get(BuildKey(key, type));

            return HttpRuntime.Cache.Get(BuildDateKey(key, type)); 
        }

        /// <summary>
        /// Adds the specified item to the local cache with dependencies, expiration and priority policies, 
        /// and a delegate you can use to notify your application when the inserted item is removed from the Cache.
        /// </summary>
        /// <param name="key">The cache key used to reference the item.</param>
        /// <param name="type">The object's type being inserted.</param>
        /// <param name="value">The item to be added to the cache.</param>
        /// <param name="absoluteExpiration">The time at which the added object expires and is removed from the cache. If you are using sliding expiration, the absoluteExpiration parameter must be LocalCache.NoAbsoluteExpiration.</param>
        /// <param name="slidingExpiration">The interval between the time the added object was last accessed and the time at which that object expires. If this value is the equivalent of 20 minutes, the object expires and is removed from the cache 20 minutes after it is last accessed. If you are using absolute expiration, the slidingExpiration parameter must be LocalCache.NoSlidingExpiration</param>
        /// <param name="priority">The relative cost of the object, as expressed by theCambridgeSoft.COE.Framework.Caching.COECacheItemPriority enumeration. The cache uses this value when it evicts objects; 
        /// objects with a lower cost are removed from the cache before objects with a higher cost</param>
        /// <returns>An System.Object if the item was previously stored in the Cache; otherwise, null</returns>
        public static object Add(string key, Type type, object value, DateTime absoluteExpiration, TimeSpan slidingExpiration, COECacheItemPriority priority)
        {
            return Add(key, type, value, absoluteExpiration, slidingExpiration, priority, DateTime.Now);
        }

        /// <summary>
        /// Adds the specified item to the local cache with dependencies, expiration and priority policies, 
        /// and a delegate you can use to notify your application when the inserted item is removed from the Cache.
        /// This override allows you to force the cached date to sync it with another object, like the server version in 3-Tier mode.
        /// </summary>
        /// <param name="key">The cache key used to reference the item.</param>
        /// <param name="type">The object's type being inserted.</param>
        /// <param name="value">The item to be added to the cache.</param>
        /// <param name="absoluteExpiration">The time at which the added object expires and is removed from the cache. If you are using sliding expiration, the absoluteExpiration parameter must be LocalCache.NoAbsoluteExpiration.</param>
        /// <param name="slidingExpiration">The interval between the time the added object was last accessed and the time at which that object expires. If this value is the equivalent of 20 minutes, the object expires and is removed from the cache 20 minutes after it is last accessed. If you are using absolute expiration, the slidingExpiration parameter must be LocalCache.NoSlidingExpiration</param>
        /// <param name="priority">The relative cost of the object, as expressed by theCambridgeSoft.COE.Framework.Caching.COECacheItemPriority enumeration. The cache uses this value when it evicts objects; 
        /// objects with a lower cost are removed from the cache before objects with a higher cost</param>
        /// <param name="date">If you want to provide a date to override the current time. </param>
        /// <returns>An System.Object if the item was previously stored in the Cache; otherwise, null</returns>
        public static object Add(string key, Type type, object value, DateTime absoluteExpiration, TimeSpan slidingExpiration, COECacheItemPriority priority, DateTime dateOverride)
        {
            COECacheDependency dependency = null;
            CacheDependency dateCacheDependency = null;

            CacheItemRemovedCallback callback = delegate(string keyA, object valueA, CacheItemRemovedReason reason) { };

            if (value is ICacheable)
            {
                dependency = ((ICacheable)value).CacheDependency;

                callback = delegate(string keyA, object valueA, CacheItemRemovedReason reason)
                {
                    ((ICacheable)value).ItemRemovedFromCache(key, value, ToCOEItemRemovedReason(reason));
                };
            }

            object result = null;
            try
            {
                result = HttpRuntime.Cache.Add(BuildKey(key, type), value, dependency, absoluteExpiration, slidingExpiration, ToCacheItemPriority(priority), new System.Web.Caching.CacheItemRemovedCallback(callback));
            }
            catch
            {
                //The following error occurs when there is no try catch.  Something must be recreating a dependency but 
                //I have not been able to track it down.
                //An attempt was made to reference a CacheDependency object from more than one Cache entry.
                //This catch block doesn't seem to have any adverse affect (at least for dataviews)
            }

            //This is separate from the above check because the Cached item must first be created prior to adding a dependency.
            if (value is ICacheable)
            {
                dateCacheDependency = new COECacheDependency(null, new string[] { BuildKey(key, type) });
            }
            //add hidden date used for checking whether you need to pull down the object
            HttpRuntime.Cache.Insert(BuildDateKey(key, type), dateOverride, dateCacheDependency, absoluteExpiration, slidingExpiration, ToCacheItemPriority(priority), null);


            return result;
        }
        
        /// <summary>
        /// Removes the specified item from the Local Cache.
        /// </summary>
        /// <param name="key">A System.String identifier for the cache item to remove.</param>
        /// <param name="type">A System.Type of the cached item to remove.</param>
        /// <returns>The item removed from the Cache. If the value in the key parameter is not found, returns null.</returns>
        public static object Remove(string key, Type type)
        {
            return HttpRuntime.Cache.Remove(BuildKey(key, type));
        }

        private static string BuildKey(string id, Type type)
        {
            return string.Format("{0}_{1}", id, type.Name);
        }
        
        private static string BuildDateKey(string id, Type type)
        {
            return string.Format("{0}_LastUpdated", BuildKey(id, type));
        }

        private static COECacheItemRemovedReason ToCOEItemRemovedReason(CacheItemRemovedReason reason)
        {
            switch (reason)
            {
                case CacheItemRemovedReason.DependencyChanged:
                    return COECacheItemRemovedReason.DependencyChanged;
                case CacheItemRemovedReason.Expired:
                    return COECacheItemRemovedReason.Expired;
                case CacheItemRemovedReason.Removed:
                    return COECacheItemRemovedReason.Removed;
                case CacheItemRemovedReason.Underused:
                    return COECacheItemRemovedReason.Underused;
                default:
                    return COECacheItemRemovedReason.Underused;

            }
        }

        private static CacheItemPriority ToCacheItemPriority(COECacheItemPriority priority)
        {
            switch(priority)
            {
                case COECacheItemPriority.AboveNormal:
                    return CacheItemPriority.AboveNormal;
                case COECacheItemPriority.BelowNormal:
                    return CacheItemPriority.BelowNormal;
                case COECacheItemPriority.Normal:
                    return CacheItemPriority.Normal;
                case COECacheItemPriority.High:
                    return  CacheItemPriority.High;
                case COECacheItemPriority.Low:
                    return CacheItemPriority.Low;
                case COECacheItemPriority.NotRemovable:
                    return CacheItemPriority.NotRemovable;
                default:
                    return CacheItemPriority.Default;
            }
        }
    }
}
