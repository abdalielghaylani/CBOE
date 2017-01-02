using System;
using System.Collections.Generic;
using System.Text;
using CambridgeSoft.COE.Framework.COEConfigurationService;
using CambridgeSoft.COE.Framework.Properties;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data.Common;
using System.Data;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using System.Linq;
using System.Xml;
using System.IO;
using CambridgeSoft.COE.Framework.Caching;


namespace CambridgeSoft.COE.Framework.Common
{
    /// <summary>
    /// Common Data Access Logic utils.
    /// </summary>
    public class DALUtils
    {
        public static string GetDefaultQualifyDbName(string databaseName)
        {
            return databaseName;
            //var coeInstance = ConfigurationUtilities.GetCOEInstance();
            //string instanceName = "Main"; //Catcher
            //if (coeInstance!=null)
            //{
            //    instanceName = coeInstance.InstanceName;
            //}
            //return string.Format("{0}.{1}", instanceName, databaseName);
        }

        /// <summary>
        /// <para>Gets a <see cref="DALManager"/> based on the configuration.</para>
        /// </summary>
        /// <param name="databaseData">Database configuration section</param>
        /// <param name="serviceData">Service configuration section</param>
        /// <param name="connStringType">Connection string type</param>
        /// <returns>A <see cref="DALManager"/></returns>
        public static DALManager GetDALManager(DatabaseData databaseData, ServiceData serviceData, DBMSTypeData dbmsTypeData, bool adminOverride)
        {
            DALManager dALManager = new DALManager(databaseData, serviceData, dbmsTypeData, adminOverride);
            return dALManager;
        }

        /// <summary>
        /// Build a connection string for calling the datastore
        /// </summary>
        /// <param name="databaseData">Database configuration section</param>
        /// <param name="connStringType">Connection string type</param>
        /// <returns>The connection string</returns>
        public static string GetConnString(DatabaseData databaseData, DBMSTypeData dbmsTypeData, bool adminOverride)
        {
            InstanceData instacneData = databaseData.InstanceData;
            //set default
            ConnStringType connStringType = ConnStringType.OWNER;
            //check is this is a proxy
            bool useProxy = System.Convert.ToBoolean(instacneData.UseProxy);

            //determine connType for builing string
            switch (databaseData.DBMSType)
            {
                case DBMSType.ORACLE:
                    if (useProxy == true)
                    {
                        if (adminOverride == true)
                        {
                            connStringType = ConnStringType.OWNERPROXY;
                        }
                        else
                        {
                            connStringType = ConnStringType.PROXY;
                        }
                    }
                    else
                    {
                        connStringType = ConnStringType.OWNER;
                    }
                    break;
                case DBMSType.SQLSERVER:
                    connStringType = ConnStringType.OWNER;
                    break;
                case DBMSType.MSACCESS:
                    connStringType = ConnStringType.OWNER;
                    break;
            }

            string connString = AddCredentials(databaseData, connStringType);

            //Will change this during refactor
            if (databaseData.InstanceData != null)
            {
                if (!string.IsNullOrEmpty(databaseData.InstanceData.HostName))
                {
                    connString = AddDataSource(databaseData.InstanceData, connString);
                }
                else
                {
                    connString = AddDataSource(databaseData.InstanceData.SID, connString);
                }
            }
           
            connString = AddProviderOptions(databaseData.ProviderOptions, connString);

            return connString;
        }

        /// <summary>
        /// Adds credentials to the connection string.
        /// </summary>
        /// <param name="providerName">Fully calified class name of the data access provider</param>
        /// <param name="userName">The user name</param>
        /// <param name="password">The password</param>
        /// <param name="connStringType">The connection string type</param>
        /// <returns>The connection string with the added credentials</returns>
        public static string AddCredentials(DatabaseData databaseData, ConnStringType connStringType)
        {
            string providerName = databaseData.ProviderName;
            string userName = databaseData.Owner;
            string password = Utilities.IsRijndaelEncrypted(databaseData.Password) ? Utilities.DecryptRijndael(databaseData.Password) : databaseData.Password;
            string userNameKeyword = string.Empty;
            string passwordKeyword = string.Empty;
            string proxyuserNameKeyword = string.Empty;
            string proxypasswordKeyword = string.Empty;
            string connString = string.Empty;

            //get keywords based on provider 
            switch (providerName)
            {
                case "System.Data.OracleClient":
                    userNameKeyword = "user id=";
                    passwordKeyword = "password=";
                    proxyuserNameKeyword = "proxy user id=";
                    proxypasswordKeyword = "proxy password=";
                    break;
                case "Oracle.DataAccess.Client":
                    userNameKeyword = "user id=";
                    passwordKeyword = "password=";
                    proxyuserNameKeyword = "proxy user id=";
                    proxypasswordKeyword = "proxy password=";
                    break;
                case "System.Data.SqlClient":
                    userNameKeyword = "user id=";
                    passwordKeyword = "password=";
                    proxyuserNameKeyword = "proxy user id=";
                    proxypasswordKeyword = "proxy password=";
                    break;
                case "System.Data.Oledb":
                    userNameKeyword = "uid=";
                    passwordKeyword = "pwd=";
                    proxyuserNameKeyword = "puid=";
                    proxypasswordKeyword = "ppwd=";
                    break;
                case "System.Data.Odbc":
                    userNameKeyword = "uid=";
                    passwordKeyword = "pwd=";
                    proxyuserNameKeyword = "puid=";
                    proxypasswordKeyword = "ppwd=";
                    break;
            }

            if (databaseData.OriginalOwner.Equals("originalOwner", StringComparison.InvariantCultureIgnoreCase)) 
            {
                connStringType = ConnStringType.OWNER;
            }

            //build credentials string based on connstringtype input
            switch (connStringType)
            {
                case ConnStringType.PROXY:
                    connString = userNameKeyword + COEUser.Get().ToString() + ";";
                    connString = connString + proxyuserNameKeyword + userName + ";";
                    connString = connString + proxypasswordKeyword + password;
                    break;
                case ConnStringType.OWNERPROXY:
                    connString = userNameKeyword + databaseData.OriginalOwner + ";";
                    connString = connString + proxyuserNameKeyword + userName + ";";
                    connString = connString + proxypasswordKeyword + password;
                    break;
                case ConnStringType.OWNER:
                    connString = userNameKeyword + userName + ";";
                    connString = connString + passwordKeyword + password;
                    break;
            }

            return connString;
        }

        /// <summary>
        /// Adds a datasource to the connection string
        /// </summary>
        /// <param name="dataSource">The datasource name</param>
        /// <param name="connString">The connection string without datasource</param>
        /// <returns>The connection string with the added datasource</returns>
        public static string AddDataSource(string dataSource, string connString)
        {
            return connString + ";Data Source=" + dataSource;
        }

        /// <summary>
        /// Adds a datasource to the connection string
        /// </summary>
        /// <param name="instanceData">The instance data</param>
        /// <param name="connString">The connection string without datasource</param>
        /// <returns>The connection string with the added datasource</returns>
        public static string AddDataSource(InstanceData instanceData, string connString)
        {
            return connString + ";" + Utilities.GetConnectString(instanceData.HostName, instanceData.Port, instanceData.SID);
        }

        /// <summary>
        /// Adds specific provider options to the connection string
        /// </summary>
        /// <param name="providerOptions">The provider options</param>
        /// <param name="connString">The connection string</param>
        /// <returns>The connection string with the added provider options</returns>
        public static string AddProviderOptions(string providerOptions, string connString)
        {
            return connString + ";" + providerOptions;
        }

        /// <summary>
        /// Disposes a the command object in all cases.
        /// Disposes the connection object only when it is not part of the transaction.
        /// </summary>
        /// <param name="connection">The DBCommand object to destroy (connection is part of the command)</param>
        public static void DestroyCommandAndConnection(DbCommand command)
        {
            //Coverity Bug Fix :- CID : 11850  Jira Id :CBOE-194
            if (command != null && command.Connection != null && command.Transaction == null)
            {
                if (command.Connection.State != ConnectionState.Closed)
                {
                    command.Connection.Close();
                }
                command.Dispose();
            }

        }

            private static object _lockOjbect = new object();
        private static string _xmlConfigCacheKey = "XmlConfigCacheKey";

        private class XmlConfigCache : ICacheable
        {

            public string XmlString { get; set; }
            private COECacheDependency _cacheDependency = new COECacheDependency(COEConfigurationBO.DefaultConfigurationFilePath); 
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
        }


        private static void SetXmlConfigCache(COEConfigurationSettings setting)
        {
          
            StringBuilder output = new StringBuilder();
            XmlWriterSettings wSettings = new XmlWriterSettings();
            wSettings.Indent = true;

            using (XmlWriter writer = XmlWriter.Create(output, wSettings))
            {
                setting.WriteXml(writer);
                writer.Flush();
                writer.Close();
            }
            LocalCache.Add(_xmlConfigCacheKey, typeof(XmlConfigCache), new XmlConfigCache() { XmlString = output.ToString() }, LocalCache.NoAbsoluteExpiration, TimeSpan.FromDays(1), COECacheItemPriority.Normal);
        
        }


       
        /// <summary>
        ///  this method should always return the un-modified setting from xml
        ///  http://msdn.microsoft.com/en-us/library/microsoft.practices.enterpriselibrary.common.configuration.configurationsectioncloner%28v=pandp.50%29.aspx
        ///  if we can upgrade EntLib, we can use configurationsectioncloner in the future.
        ///  Attention, this mechanism is only for use on server, mainly for loading DAL. 
        ///  if you want to get setting on client, use xxx BO
        /// </summary>
        internal static COEConfigurationSettings GetXmlConfigCache()
        {
            lock (_lockOjbect)
            {
                var cache = LocalCache.Get(_xmlConfigCacheKey, typeof(XmlConfigCache)) as XmlConfigCache;
                if (cache == null)  
                {
                    var configurationSource = new FileConfigurationSource(COEConfigurationBO.DefaultConfigurationFilePath);
                    var settings = configurationSource.GetSection(COEConfigurationSettings.SectionName) as COEConfigurationSettings;
                    SetXmlConfigCache(settings);
                    cache = LocalCache.Get(_xmlConfigCacheKey, typeof(XmlConfigCache)) as XmlConfigCache;
                }
                

                COEConfigurationSettings newSection = new COEConfigurationSettings();
                XmlReaderSettings rSetting = new XmlReaderSettings();
                rSetting.CloseInput = true;

                StringReader sr = new StringReader(cache.XmlString);
                using (XmlReader reader = XmlReader.Create(sr, rSetting))
                {
                    newSection.ReadXml(reader);
                    reader.Close();
                }
                sr.Close();
                return newSection;
            }
        }
  
        
    }
}
