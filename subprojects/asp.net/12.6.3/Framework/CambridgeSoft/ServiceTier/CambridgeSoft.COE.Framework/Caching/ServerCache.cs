using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Caching;
using System.Web;
using System.Runtime.Serialization;
using Csla;

namespace CambridgeSoft.COE.Framework.Caching
{
    /// <summary>
    /// A cache that resides in the server tier/application domain. Depending on CSLA configuration it would be remote or not.
    /// </summary>
    public static class ServerCache
    {
        /// <summary>
        /// Used in the absoluteExpiration parameter while adding objects to ServerCache to indicate the item should never expire. 
        /// This field is read-only.
        /// </summary>
        public static readonly DateTime NoAbsoluteExpiration = Cache.NoAbsoluteExpiration;
        /// <summary>
        /// Used as the slidingExpiration parameter while adding objects to ServerCache to disable sliding expirations.
        /// This field is read-only.
        /// </summary>
        public static readonly TimeSpan NoSlidingExpiration = Cache.NoSlidingExpiration;

        /// <summary>
        /// Retrieves the specified item from the Server Cache
        /// </summary>
        /// <param name="key">The identifier for the cache item to retrieve.</param>
        /// <param name="type">The type of the object to get.</param>
        /// <returns>The retrieved cache item, or null if the key is not found.</returns>
        public static object Get(string key, Type type)
        {
            return DataPortal.Execute<CacheCommand>(new CacheCommand(BuildKey(key, type), "GET")).result;
        }

        public static object GetDate(string key, Type type)
        {
            return DataPortal.Execute<CacheCommand>(new CacheCommand(BuildKey(key, type), "GETDATE")).result;
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
            return DataPortal.Execute<CacheCommand>(new CacheCommand(BuildKey(key, type), value, absoluteExpiration, slidingExpiration, ToCacheItemPriority(priority))).result;
        }

        /// <summary>
        /// Removes the specified item from the Server Cache.
        /// </summary>
        /// <param name="key">A System.String identifier for the cache item to remove.</param>
        /// <param name="type">A System.Type of the cached item to remove.</param>
        /// <returns>The item removed from the Cache. If the value in the key parameter is not found, returns null.</returns>
        public static object Remove(string key, Type type)
        {
            return DataPortal.Execute<CacheCommand>(new CacheCommand(BuildKey(key, type), "REMOVE")).result;
        }

        /// <summary>
        /// Verifies if an object is stored in the cache.
        /// </summary>
        /// <param name="key">A System.String identifier for the cache item to check.</param>
        /// <param name="type">A System.Type of the cached item to check.</param>
        /// <returns>True if the item exists, false otherwise.</returns>
        public static bool Exists(string key, Type type)
        {
            return (bool)DataPortal.Execute<CacheCommand>(new CacheCommand(BuildKey(key, type), "EXIST")).result;
        }

        private static string BuildKey(string id, Type type)
        {
            return string.Format("{0}_{1}", id, type.Name);
        }

        private static CacheItemPriority ToCacheItemPriority(COECacheItemPriority priority)
        {
            switch (priority)
            {
                case COECacheItemPriority.AboveNormal:
                    return CacheItemPriority.AboveNormal;
                case COECacheItemPriority.BelowNormal:
                    return CacheItemPriority.BelowNormal;
                case COECacheItemPriority.Normal:
                    return CacheItemPriority.Normal;
                case COECacheItemPriority.High:
                    return CacheItemPriority.High;
                case COECacheItemPriority.Low:
                    return CacheItemPriority.Low;
                case COECacheItemPriority.NotRemovable:
                    return CacheItemPriority.NotRemovable;
                default:
                    return CacheItemPriority.Default;
            }
        }
    }

    /// <summary>
    /// CSLA Based command used to assure server side operations. The command in this case is meant to perform operations agains the HttpRuntime.Cache object
    /// </summary>
    [Serializable]
    class CacheCommand : CommandBase
    {
        internal object result;

        internal string key;
        internal object value;
        internal DateTime absoluteExpiration;
        internal TimeSpan slidingExpiration;
        internal CacheItemPriority priority;
        internal string action;

        internal CacheCommand(string key, object value, DateTime absoluteExpiration, TimeSpan slidingExpiration, CacheItemPriority priority)
        {
            this.key = key;
            this.value = value;
            this.absoluteExpiration = absoluteExpiration;
            this.slidingExpiration = slidingExpiration;
            this.priority = priority;
            this.action = "ADD";
        }

        internal CacheCommand(string key, string action)
        {
            this.key = key;
            this.action = (string.IsNullOrEmpty(action) ? "GET" : action.ToUpper());
        }
        
        /// <summary>
        /// The actions specified in the constructors are performed against the server chache object.
        /// </summary>
        protected override void DataPortal_Execute()
        {
            switch (action)
            {
                case "REMOVE":
                    result = HttpRuntime.Cache.Remove(key);
                    break;
                case "ADD":
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

                    result = HttpRuntime.Cache.Add(key, value, dependency, absoluteExpiration, slidingExpiration, priority, callback);

                    if (value is ICacheable)
                    {
                        dateCacheDependency = new COECacheDependency(null, new string[] { key }); 
                    }
                    HttpRuntime.Cache.Insert(BuildDateKey(key), DateTime.Now, dateCacheDependency, absoluteExpiration, slidingExpiration, priority, null);
                    
                    break;
                case "EXIST":
                    HttpRuntime.Cache.Get(BuildDateKey(key));
                    result = HttpRuntime.Cache.Get(key) != null;
                    break;
                case "GETDATE":
                    HttpRuntime.Cache.Get(key); //Get the object to keep it alive
                    result = HttpRuntime.Cache.Get(BuildDateKey(key));
                    break;
                case "GET":
                default:
                    HttpRuntime.Cache.Get(BuildDateKey(key));
                    result = HttpRuntime.Cache.Get(key);
                    break;
            }
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

        private static string BuildDateKey(string cacheKey)
        {
            return string.Format("{0}_LastUpdated", cacheKey);
        }
    }
}
