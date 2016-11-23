using System;
using System.Collections.Generic;
using System.Text;

namespace CambridgeSoft.COE.Framework.Caching
{
    public interface ICacheable
    {
        /// <summary>
        /// CacheDependency to be used to get notifications when the cached item has become invalid due to changes in the underlying dependency. IE: OracleCacheDependency, FileCacheDependency, etc
        /// </summary>
        COECacheDependency CacheDependency
        {
            get;
            set;
        }

        /// <summary>
        /// Method called when an item is removed from cache.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="reason"></param>
        void ItemRemovedFromCache(string key, object value, COECacheItemRemovedReason reason);
    }
}
