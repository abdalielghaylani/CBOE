using System;
using System.Collections.Generic;
using Microsoft.Practices.EnterpriseLibrary.Caching;
using Microsoft.Practices.EnterpriseLibrary.Caching.BackingStoreImplementations;
using Microsoft.Practices.EnterpriseLibrary.Caching.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Caching.Instrumentation;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;

namespace CambridgeSoft.COE.DataLoader.Core.Caching
{
    /// <summary>
    /// Contains static methods for retrieving EntLib CacheManager instances.
    /// </summary>
    public class COECacheManager
    {
        /// <summary>
        /// Maintains a dictionary of CacheManager instances created and maintained by this factory
        /// class. CacheManager objects not already described in the applciation's configuration
        /// context will not be available via the static 'CacheFactory.GetCacheManager()' method,
        /// so this dictionary is a surrogate 'cache manager'.
        /// </summary>
        private static Dictionary<string, CacheManager> _cacheManagers
             = new Dictionary<string,CacheManager>();
        
        /// <summary>
        /// Fetches a specific CacheManager instance associated with the applciation context's configuration.
        /// </summary>
        /// <param name="cacheManagerName">Key value for fetching/creating the CacheManager instance</param>
        /// <returns>Instance of the EntLib CacheManager class</returns>
        public static CacheManager GetCacheManager(string cacheManagerName)
        {
/* JED: Could we make an instance of the Cache class itself?
            IBackingStore backer = new NullBackingStore();
            CacheCapacityScavengingPolicy scavenger = new CacheCapacityScavengingPolicy(1000);
            CachingInstrumentationProvider tracer = new CachingInstrumentationProvider();
            Cache c = new Cache(backer, scavenger, tracer);
*/
            CacheManager cm = CacheFactory.GetCacheManager(cacheManagerName);
            return cm;
        }

        /// <summary>
        /// Fetches a specific CacheManager instance described in an external xml configuration file.
        /// </summary>
        /// <param name="cacheManagerName"></param>
        /// <param name="configFilePath"></param>
        /// <returns></returns>
        public static CacheManager GetCacheManager(string cacheManagerName, string configFilePath)
        {
            CacheManager cm = null;
            if (!_cacheManagers.ContainsKey(cacheManagerName))
            {
                FileConfigurationSource fcs = new FileConfigurationSource(configFilePath);
                CacheManagerFactory cmf = new CacheManagerFactory(fcs);
                cm = cmf.Create(cacheManagerName);
                _cacheManagers.Add(cacheManagerName, (CacheManager)cm);

                //NOTE: Just because a CacheManager has been created does NOT mean
                // that you will have access to it via "CacheFactory.GetCacheManager("MyManager")".
                try
                {
                    CacheManager w = CacheFactory.GetCacheManager(cacheManagerName);
                }
                catch (Exception ex)
                {
                    string err = ex.Message;
                }
            }
            else
            {
                cm = _cacheManagers[cacheManagerName];
            }

            return cm;
        }

        /// <summary>
        /// Fetches the default CacheManager instance associated with the applciation context's configuration.
        /// </summary>
        /// <returns>Instance of the EntLib CacheManager class</returns>
        public static CacheManager GetCacheManager()
        {
            CacheManager cm = CacheFactory.GetCacheManager();
            return cm;
        }

        /// <summary>
        /// Fetches a CacheManager instance generated from simple scalar parameters.
        /// </summary>
        /// <remarks>
        /// Make a Common.Configuration.DictionaryConfigurationSource (container for configs)
        /// Create a Caching.Configuration.CacheManagerData with the cache's details
        /// Create a Caching.Configuration.CacheManagerSettings to:
        ///     define the backing store
        ///     house the CacheManagerData
        /// Add the CacheManagerSettings to the DictionaryConfigurationSource
        /// Create a Caching.CacheManagerFactory, initializing with the DictionaryConfigurationSource
        /// Finally, have the factory create the Caching.CacheManager instance
        /// </remarks>
        public static CacheManager GetCacheManager(
            string cacheManagerName
            , int expirationInSeconds
            , int maxItemsBeforeScavenging
            , int scavengingSize
            , string storageMechanism
            )
        {
            CacheManager cm = null;
            if (!_cacheManagers.ContainsKey(cacheManagerName))
            {
                DictionaryConfigurationSource dcs = new DictionaryConfigurationSource();
                CacheManagerData data = new CacheManagerData(
                    cacheManagerName, expirationInSeconds, maxItemsBeforeScavenging, scavengingSize, storageMechanism
                );

                CacheManagerSettings settings = new CacheManagerSettings();
                settings.BackingStores.Add(new CacheStorageData(storageMechanism, typeof(NullBackingStore)));
                settings.CacheManagers.Add(data);
                settings.DefaultCacheManager = cacheManagerName;

                dcs.Add(CacheManagerSettings.SectionName, settings);
                CacheManagerFactory cmf = new CacheManagerFactory(dcs);

                //We can't use this method because the cache factory doesn't know about our Dictionary
                //for some reason!
                //  CacheManager cm = CacheFactory.GetCacheManager(cacheManagerName);
                //So use this instead 
                cm = cmf.Create(cacheManagerName);
            }
            else
            {
                cm = _cacheManagers[cacheManagerName];
            }

            return cm;
        }
    }
}
