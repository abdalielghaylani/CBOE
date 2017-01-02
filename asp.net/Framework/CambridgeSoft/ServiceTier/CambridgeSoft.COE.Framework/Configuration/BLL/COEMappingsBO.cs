// --------------------------------------------------------------------------------------------------------------------
// <copyright file="COEMappingsBO.cs" company="PerkinElmer Inc.">
//   Copyright (c) 2013 PerkinElmer Inc.,
//   940 Winter Street, Waltham, MA 02451.
//   All rights reserved.
//   This software is the confidential and proprietary information
//   of PerkinElmer Inc. ("Confidential Information"). You shall not
//   disclose such Confidential Information and may not use it in any way,
//   absent an express written license agreement between you and PerkinElmer Inc.
//   that authorizes such use.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CambridgeSoft.COE.Framework.COEConfigurationService
{
    using System;
    using CambridgeSoft.COE.Framework.Caching;
    using Csla;

    /// <summary>
    /// The BO for getting COE Mappings setting
    /// </summary>
    [Serializable()]
    class COEMappingsBO : Csla.BusinessBase<COEMappingsBO>, ICacheable
    {
        #region Member variables

        /// <summary>
        /// key for spotfire cache in local cache
        /// </summary>
        private const string CacheKeyString = "COEMappings";

        [NonSerialized]
        private COECacheDependency cacheDependency;

        #endregion

        #region Properties

        public string MappingXml { get; private set; }

        public COECacheDependency CacheDependency
        {
            get { return cacheDependency; }
            set { cacheDependency = value; }
        }

        #endregion

        #region Constructors

        private COEMappingsBO()
        {
            /* require use of factory method */
        }

        #endregion

        #region Methods

        /// <summary>
        /// Method triggered when the object is removed from cache. Currently display information in the debug console, if in debug mode.
        /// </summary>
        /// <param name="key">The object id</param>
        /// <param name="value">The actual dataviewbo</param>
        /// <param name="reason">The reason why it was removed from cache</param>
        public void ItemRemovedFromCache(string key, object value, COECacheItemRemovedReason reason)
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine("");
            System.Diagnostics.Debug.WriteLine("*****************************");
            System.Diagnostics.Debug.WriteLine("Item Removed from cache.");
            System.Diagnostics.Debug.WriteLine("Key: " + key);
            System.Diagnostics.Debug.WriteLine("Reason: " + reason.ToString());
            System.Diagnostics.Debug.WriteLine("Current Time: " + DateTime.Now);
            System.Diagnostics.Debug.WriteLine("*****************************");
#endif
        }

        /// <summary>
        /// Get COEMappingsBO object from cache or from data portal
        /// </summary>
        /// <param name="notFromCache"> Read from cache or data portal </param>
        /// <returns> The COEMappingsBO object </returns>
        public static COEMappingsBO Get(bool notFromCache)
        {
            COEMappingsBO result;

            if (notFromCache)
            {
                result = GetFromPortal();
            }
            else
            {
                result = LocalCache.Get(CacheKeyString, typeof(COEMappingsBO)) as COEMappingsBO;

                if (result == null)
                {
                    // Get from server catch or read from config file by csla
                    if (ServerCache.Exists(CacheKeyString, typeof(COEMappingsBO))) //Exists is first to refresh the sliding time
                    {
                        result = ServerCache.Get(CacheKeyString, typeof(COEMappingsBO)) as COEMappingsBO;
                        LocalCache.Add(CacheKeyString, typeof(COEMappingsBO), result, LocalCache.NoAbsoluteExpiration, TimeSpan.FromMinutes(60), COECacheItemPriority.Normal);
                    }
                    else
                    {
                        result = GetFromPortal();
                    }
                }
            }

            return result;
        }

        protected override object GetIdValue()
        {
            return CacheKeyString;
        }

        // Get mapping data by csla protral
        private static COEMappingsBO GetFromPortal()
        {
            var result = DataPortal.Fetch<COEMappingsBO>();

            // Update cache
            LocalCache.Remove(CacheKeyString, typeof(COEMappingsBO));
            LocalCache.Add(CacheKeyString, typeof(COEMappingsBO), result, LocalCache.NoAbsoluteExpiration, TimeSpan.FromMinutes(60), COECacheItemPriority.Normal);

            return result;
        }

        // Fetch function running on server side
        private void DataPortal_Fetch()
        {
            var result = LocalCache.Get(CacheKeyString, typeof(COEMappingsBO)) as COEMappingsBO;
            if (result != null)
            {
                this.MappingXml = result.MappingXml;
                return;
            }

            this.MappingXml = ConfigurationUtilities.GetConfigurationManager().GetSectionXml(null, "CambridgeSoft.COE.Framework", "mappings");

            this.CacheDependency = new COECacheDependency(COEConfigurationBO.DefaultConfigurationFilePath);
            LocalCache.Add(CacheKeyString, typeof(COEMappingsBO), this, LocalCache.NoAbsoluteExpiration, TimeSpan.FromDays(1), COECacheItemPriority.Normal);
        }

        #endregion
    }
}
