using CambridgeSoft.COE.Framework.Caching;
using CambridgeSoft.COE.Framework.COEConfigurationService;
using CambridgeSoft.COE.Framework.COELoggingService;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.Common.Configuration;
using Csla;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace CambridgeSoft.COE.Framework.COEConfigurationService
{
    /// <summary>
    /// The BO for getting spotfire setting
    /// </summary>
    [Serializable]
    public class COESpotFireSettingsBO : Csla.BusinessBase<COESpotFireSettingsBO>, ICacheable
    {
        #region fields
        private string _spotfireURL;
        private string _spotfireUser;
        private string _spotfirePassword;
        private static COELog _coeLog = COELog.GetSingleton("COEConfiguration");

        /// <summary>
        /// key for spotfire cache in local cache
        /// </summary>
        private static string _cacheKeyString = "SpotfireSetting";

        #endregion


        /// <summary>
        ///Gets and Sets Spotfire URL
        /// </summary>
        public string SpotfireURL
        {
            get { return _spotfireURL; }
            set { _spotfireURL = value; }
        }

        /// <summary>
        ///Gets and Sets Spotfire User
        /// </summary>
        public string SpotfireUser
        {
            get { return _spotfireUser; }
            set { _spotfireUser = value; }
        }

        /// <summary>
        ///Gets and Sets Spotfire password
        /// </summary> 
        public string SpotfirePassword
        {
            get { return _spotfirePassword; }
            set { _spotfirePassword = value; }
        }




        /// <summary>
        /// Get the BO
        /// </summary>
        /// <param name="notFromCache"></param>
        /// <returns></returns>
        public static COESpotFireSettingsBO Get(bool notFromCache = false)
        {
            if (notFromCache)
            {
                return GetFromPortal();
            }
            else
            {
                //from "local" cache, actaully "local" can be on server or client side, but client side has shorter expiration time span and not affected by File
                var result = LocalCache.Get(_cacheKeyString, typeof(COESpotFireSettingsBO)) as COESpotFireSettingsBO;
                if (result == null)
                {
                    result = GetFromPortal();
                }
                return result;
            }
        }

        /// <summary>
        /// comments for private method
        /// </summary>
        /// <returns></returns>
        private static COESpotFireSettingsBO GetFromPortal()
        {
            LocalCache.Remove(_cacheKeyString, typeof(COESpotFireSettingsBO));
            var result = DataPortal.Fetch<COESpotFireSettingsBO>();
            if (LocalCache.Get(_cacheKeyString, typeof(COESpotFireSettingsBO)) == null) //when local is client
            {
                LocalCache.Add(_cacheKeyString, typeof(COESpotFireSettingsBO), result, LocalCache.NoAbsoluteExpiration, TimeSpan.FromMinutes(60), COECacheItemPriority.Normal);
            }
            return result;
        }


        /// <summary>
        /// comments for private method
        /// </summary>
        private void DataPortal_Fetch()
        {
            var result = LocalCache.Get(_cacheKeyString, typeof(COESpotFireSettingsBO)) as COESpotFireSettingsBO;
            if (result != null)
            {
                this.SpotfireURL = result.SpotfireURL;
                this.SpotfireUser = result.SpotfireUser;
                this.SpotfirePassword = result.SpotfirePassword;
                return;
            }

            //we can also use the COEConfigurationBO to do the job
            // var configurationBO = COEConfigurationBO.Get(COEConfigurationSettings.SectionName);
            // var spSetting = (configurationBO.ConfigurationSection as COEConfigurationSettings).SpotfireSettingProperty;
            var configurationSource = new FileConfigurationSource(COEConfigurationBO.DefaultConfigurationFilePath);
            var spSetting = (configurationSource.GetSection(COEConfigurationSettings.SectionName) as COEConfigurationSettings).SpotfireSettingProperty;
            if (spSetting != null)
            {
                this.SpotfireURL = spSetting.Url;
                this.SpotfireUser = spSetting.User;
                this.SpotfirePassword = spSetting.Password;
            }
            this.CacheDependency = new COECacheDependency(COEConfigurationBO.DefaultConfigurationFilePath);
            LocalCache.Add(_cacheKeyString, typeof(COESpotFireSettingsBO), this, LocalCache.NoAbsoluteExpiration, TimeSpan.FromDays(1), COECacheItemPriority.Normal);
        }


        /// <summary>
        /// save metod
        /// </summary>
        /// <returns></returns>
        public override COESpotFireSettingsBO Save()
        {
            MarkDirty();
            return base.Save();
        }

        /// <summary>
        /// called by csla
        /// </summary>
        protected override void DataPortal_Update()
        {
            var section = DALUtils.GetXmlConfigCache();
            var spSetting = section.SpotfireSettingProperty;
            if (spSetting != null)
            {
                spSetting.Url = this.SpotfireURL;
                spSetting.User = this.SpotfireUser;
                spSetting.Password = this.SpotfirePassword;
            }
            else
            {
                section.SpotfireSettingProperty
                    = new SpotfireSettingElement() { Url = this.SpotfireURL, User = this.SpotfireUser, Password = this.SpotfirePassword };
            }

            var configurationSource = new FileConfigurationSource(COEConfigurationBO.DefaultConfigurationFilePath);
            configurationSource.Remove(new FileConfigurationParameter(COEConfigurationBO.DefaultConfigurationFilePath), COEConfigurationSettings.SectionName);
            configurationSource.Add(new FileConfigurationParameter(COEConfigurationBO.DefaultConfigurationFilePath), COEConfigurationSettings.SectionName, section);

        }

        [NonSerialized]
        private COECacheDependency _cacheDependency;
        public COECacheDependency CacheDependency
        {
            get
            {
                return _cacheDependency;
            }
            set
            {
                _cacheDependency = value;
            }
        }

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

        protected override object GetIdValue()
        {
            return _cacheKeyString;
        }
    }


}
