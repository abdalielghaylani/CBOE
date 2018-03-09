// --------------------------------------------------------------------------------------------------------------------
// <copyright file="COESpotFireSettingsBO.cs" company="PerkinElmer Inc.">
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
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Text;
    using CambridgeSoft.COE.Framework.Caching;
    using CambridgeSoft.COE.Framework.COEConfigurationService;
    using CambridgeSoft.COE.Framework.COELoggingService;
    using CambridgeSoft.COE.Framework.Common;
    using CambridgeSoft.COE.Framework.Common.Configuration;
    using Csla;
    using Microsoft.Practices.EnterpriseLibrary.Common.Configuration; 

    /// <summary>
    /// The BO for getting spotfire setting
    /// </summary>
     [Serializable]
    public class COESpotFireSettingsBO : Csla.BusinessBase<COESpotFireSettingsBO>, ICacheable
    {
        #region fields
        private static COELog _coeLog = COELog.GetSingleton("COEConfiguration"); 

        /// <summary>
        /// key for spotfire cache in local cache
        /// </summary>
        private static string _cacheKeyString = "SpotfireSetting";

         private string _spotfireURL;

         private string _spotfireUser;

         private string _spotfirePassword;

         [NonSerialized]
         private COECacheDependency _cacheDependency;  
        #endregion

         #region Properties
         /// <summary>
         /// Gets or sets COECacheDependency
         /// </summary>
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

         /// <summary>
         /// Gets or sets Spotfire URL
         /// </summary>
         public string SpotfireURL
         {
             get 
             {
                 if (!_spotfireURL.StartsWith("http://", StringComparison.InvariantCultureIgnoreCase) 
                     && !_spotfireURL.StartsWith("https://", StringComparison.InvariantCultureIgnoreCase))
                 {
                     _spotfireURL = _spotfireURL.Insert(0, "http://");
                 }

                 return _spotfireURL; 
             }
             set { _spotfireURL = value; }
         }

         /// <summary>
         /// Gets or sets Spotfire User
         /// </summary>
         public string SpotfireUser
         {
             get { return _spotfireUser; }
             set { _spotfireUser = value; }
         }

         /// <summary>
         /// Gets or sets Spotfire password
         /// </summary> 
         public string SpotfirePassword
         {
             get
             {
                 if (Common.Utilities.IsRijndaelEncrypted(_spotfirePassword))
                 {
                     return Common.Utilities.DecryptRijndael(_spotfirePassword);
                 }
                 else
                 {
                     return _spotfirePassword;
                 }
             }

             set
             {
                 if (Common.Utilities.IsRijndaelEncrypted(value))
                 {
                     _spotfirePassword = value;
                 }
                 else
                 {
                     _spotfirePassword = Common.Utilities.EncryptRijndael(value);
                 }
             }
         }
         #endregion 

         #region public methods 
         /// <summary>
         /// Get the BO
         /// </summary>
         /// <param name="notFromCache">indicate if use cache</param>
         /// <returns>the BO</returns>
         public static COESpotFireSettingsBO Get(bool notFromCache = false)
         {
             if (notFromCache)
             {
                 return GetFromPortal();
             }
             else
             {
                 // from "local" cache, actaully "local" can be on server or client side, 
                 // but client side has shorter expiration time span and not affected by File
                 var result = LocalCache.Get(_cacheKeyString, typeof(COESpotFireSettingsBO)) as COESpotFireSettingsBO;
                 if (result == null)
                 {
                     result = GetFromPortal();
                 }

                 return result;
             }
         }

         /// <summary>
         /// save method
         /// </summary>
         /// <returns>the BO</returns>
         public override COESpotFireSettingsBO Save()
         {
             MarkDirty();
             return base.Save();
         }

         /// <summary>
         /// triggered when the item is removed from cache 
         /// </summary>
         /// <param name="key">the key</param>
         /// <param name="value">the value</param>
         /// <param name="reason">the reason to remove</param>
         public void ItemRemovedFromCache(string key, object value, COECacheItemRemovedReason reason)
         {
#if DEBUG
             System.Diagnostics.Debug.WriteLine(string.Empty);
             System.Diagnostics.Debug.WriteLine("*****************************");
             System.Diagnostics.Debug.WriteLine("Item Removed from cache.");
             System.Diagnostics.Debug.WriteLine("Key: " + key);
             System.Diagnostics.Debug.WriteLine("Reason: " + reason.ToString());
             System.Diagnostics.Debug.WriteLine("Current Time: " + DateTime.Now);
             System.Diagnostics.Debug.WriteLine("*****************************");
#endif
         }
         #endregion 

         #region protected methods
         /// <summary>
         /// called by framework
         /// </summary>
         protected override void DataPortal_Update()
         {
             var section = DALUtils.GetXmlConfigCache();
             var spotFireSetting = section.SpotfireSettingProperty;
             if (spotFireSetting != null)
             {
                 spotFireSetting.Url = this.SpotfireURL;
                 spotFireSetting.User = this.SpotfireUser;
             }
             else
             {
                 section.SpotfireSettingProperty
                     = new SpotfireSettingElement() { Url = this.SpotfireURL, User = this.SpotfireUser };
             }

             section.SpotfireSettingProperty.Password = !Common.Utilities.IsRijndaelEncrypted(this.SpotfirePassword) ? Common.Utilities.EncryptRijndael(this.SpotfirePassword) : this.SpotfirePassword;
             var configurationSource = new FileConfigurationSource(COEConfigurationBO.DefaultConfigurationFilePath);
             configurationSource.Remove(new FileConfigurationParameter(COEConfigurationBO.DefaultConfigurationFilePath), COEConfigurationSettings.SectionName);
             configurationSource.Add(new FileConfigurationParameter(COEConfigurationBO.DefaultConfigurationFilePath), COEConfigurationSettings.SectionName, section);
         }

         /// <summary>
         /// return the id Value
         /// </summary>
         /// <returns>the id value</returns>
         protected override object GetIdValue()
         {
             return _cacheKeyString;
         }
         #endregion

         #region private methods
         private static COESpotFireSettingsBO GetFromPortal()
         {
             LocalCache.Remove(_cacheKeyString, typeof(COESpotFireSettingsBO));
             var result = DataPortal.Fetch<COESpotFireSettingsBO>();
             if (LocalCache.Get(_cacheKeyString, typeof(COESpotFireSettingsBO)) == null) 
             {
                 // when local is client
                 LocalCache.Add(_cacheKeyString, typeof(COESpotFireSettingsBO), result, LocalCache.NoAbsoluteExpiration, TimeSpan.FromMinutes(60), COECacheItemPriority.Normal);
             }

             return result;
         }

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

             // we can also use the COEConfigurationBO to do the job
             // var configurationBO = COEConfigurationBO.Get(COEConfigurationSettings.SectionName);
             // var spSetting = (configurationBO.ConfigurationSection as COEConfigurationSettings).SpotfireSettingProperty;
             var configurationSource = new FileConfigurationSource(COEConfigurationBO.DefaultConfigurationFilePath);
             var spotFireSetting = (configurationSource.GetSection(COEConfigurationSettings.SectionName) as COEConfigurationSettings).SpotfireSettingProperty;
             if (spotFireSetting != null)
             {
                 this.SpotfireURL = spotFireSetting.Url;
                 this.SpotfireUser = spotFireSetting.User;
                 this.SpotfirePassword = spotFireSetting.Password;
             }

             this.CacheDependency = new COECacheDependency(COEConfigurationBO.DefaultConfigurationFilePath);
             LocalCache.Add(_cacheKeyString, typeof(COESpotFireSettingsBO), this, LocalCache.NoAbsoluteExpiration, TimeSpan.FromDays(1), COECacheItemPriority.Normal);
         }
         #endregion
    }  
}
