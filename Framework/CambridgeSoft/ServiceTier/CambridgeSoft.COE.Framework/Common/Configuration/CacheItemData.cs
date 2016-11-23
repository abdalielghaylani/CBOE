using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using System.ComponentModel;

namespace CambridgeSoft.COE.Framework.Common
{
    public class CacheItemData : COENamedElementCollection<NameValueConfigurationElement>
    {
        private const string cache = "cache";
        private const string absoluteExpiration = "absoluteExpiration";
        private const string slidingExpiration = "slidingExpiration";
        private const string defaultPriority = "defaultPriority";

        /// <summary>
        /// Specifies if the item will be cached and where. By default it is disabled
        /// </summary>
        public CacheType Cache
        {
            get
            {
                if (this.Get(cache) != null)
                    return (CacheType)Enum.Parse(typeof(CacheType), this.Get(cache).Value, true);
                else
                    return CacheType.Disabled;
            }
            set { this.Get(cache).Value = value.ToString(); }
        }

        /// <summary>
        /// Specifies an Absolute expiration expressed in the following format: "MM/dd/yyyy HH:mm:ss"
        /// Absolute and sliding expiration are mutually exclusive.
        /// </summary>
        public DateTime AbsoluteExpiration
        {
            get
            {
                if (this.Get(absoluteExpiration) != null && !string.IsNullOrEmpty(this.Get(absoluteExpiration).Value))
                    return DateTime.Parse(this.Get(absoluteExpiration).Value);
                else
                    return Caching.ServerCache.NoAbsoluteExpiration;
            }
            set
            {
                this.Get(absoluteExpiration).Value = value.ToString("MM/dd/yyyy HH:mm:ss");
            }
        }

        /// <summary>
        /// Specifies an Sliding expiration expressed in minutes.
        /// Absolute and sliding expiration are mutually exclusive.
        /// </summary>
        public TimeSpan SlidingExpiration
        {
            get
            {
                if (this.Get(slidingExpiration) != null && !string.IsNullOrEmpty(this.Get(slidingExpiration).Value))
                    return TimeSpan.FromMinutes(int.Parse(this.Get(slidingExpiration).Value));
                else
                    return Caching.ServerCache.NoSlidingExpiration;
            }
            set
            {
                this.Get(slidingExpiration).Value = value.Minutes.ToString();
            }
        }

        /// <summary>
        /// Specifies the default priority to be used while adding dataviews to the cache
        /// </summary>
        public Caching.COECacheItemPriority DefaultPriority
        {
            get
            {
                if (this.Get(defaultPriority) != null)
                    return (Caching.COECacheItemPriority)Enum.Parse(typeof(Caching.COECacheItemPriority), this.Get(defaultPriority).Value, true);
                else
                    return Caching.COECacheItemPriority.Default;
            }
            set { this.Get(cache).Value = value.ToString(); }
        }
    }

    [DefaultValue(CacheType.Disabled)]
    /// <summary>
    /// Cache types is used to indicate where is the cache created. By default it is Disabled
    /// </summary>
    public enum CacheType
    {
        /// <summary>
        /// No cache is going to be used.
        /// </summary>
        Disabled,
        /// <summary>
        /// Server side cache is to be used.
        /// </summary>
        ServerCache,
        /// <summary>
        /// Client cache is to be used.
        /// </summary>
        ClientCache,
        /// <summary>
        /// Client and server cache are to be used and kept in synch.
        /// </summary>
        ServerAndClientCache
    }
}
