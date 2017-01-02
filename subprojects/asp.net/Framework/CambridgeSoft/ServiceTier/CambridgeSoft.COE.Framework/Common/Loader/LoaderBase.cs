using System;
using System.Collections.Generic;
using System.Text;
using CambridgeSoft.COE.Framework.COEConfigurationService;
using System.Linq;
using System.Diagnostics;

namespace CambridgeSoft.COE.Framework.Common
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class LoaderBase
    {
        [NonSerialized]
        protected ApplicationData _appConfigData = null;
        [NonSerialized]
        protected ServiceData _serviceConfigData = null;
        [NonSerialized]
        private DatabaseData _databaseConfigData = null;
        [NonSerialized]
        private DBMSTypeData _dbmsTypeData = null;

        /// <summary>
        /// Application confituration data
        /// </summary>
        public ApplicationData AppConfigData
        {
            get { return _appConfigData; }
            set { _appConfigData = value; }
        }

        /// <summary>
        /// Service confituration data
        /// </summary>
        public ServiceData ServiceConfigData
        {
            get { return _serviceConfigData; }
            set { _serviceConfigData = value; }
        }

        /// <summary>
        /// Database configuration data
        /// </summary>
        public DatabaseData DatabaseConfigData
        {
            get { return _databaseConfigData; }
            set { _databaseConfigData = value; }
        }

        /// <summary>
        /// DBMSType  confituration data
        /// </summary>
        public DBMSTypeData DBMSTypeData
        {
            get { return _dbmsTypeData; }
            set { _dbmsTypeData = value; }
        }
        /// <summary>
        /// Populates the database, service and application configuration data from its names.
        /// </summary>
        /// <param name="databaseName">The database schema name</param>
        /// <param name="serviceName">The service name</param>
        /// <param name="appName">The application name</param>
        public void GetConfigData(string databaseName, string serviceName, string appName, bool notFromCache = false)
        {
            if (_databaseConfigData == null)
            {
                _databaseConfigData = ConfigurationUtilities.GetDatabaseData(databaseName, notFromCache);
            }

            InstanceData instanceData = _databaseConfigData.InstanceData;
            Debug.Assert(instanceData != null);

            string dbOwner = _databaseConfigData.Owner;

            if (instanceData.DatabaseGlobalUser != string.Empty)
            {
                // Override the current database with global user database
                if (_databaseConfigData.Owner != instanceData.DatabaseGlobalUser)
                {
                    var globalUserDatabase = ConfigurationUtilities.GetDatabasesByInstance(instanceData).
                       First(db => db.Owner == instanceData.DatabaseGlobalUser);
                    var orginalOwner = _databaseConfigData.Owner;

                    _databaseConfigData = globalUserDatabase;
                    _databaseConfigData.InstanceData = instanceData;
                    _databaseConfigData.OriginalOwner = orginalOwner;
                }
            }
            else
            {
                //this is old code, I don't know why,we seem not to reach here--Jason Yan
                _databaseConfigData.OriginalOwner = databaseName;
            }

            if (_appConfigData == null && appName != string.Empty)
            {
                _appConfigData = ConfigurationUtilities.GetApplicationData(appName);
            }

            if (_serviceConfigData == null)
            {
                _serviceConfigData = ConfigurationUtilities.GetServiceData(serviceName);
            }   
        }
    }
}
