using System;
using System.Collections.Generic;
using System.Text;
using CambridgeSoft.COE.Framework.COEConfigurationService;


namespace CambridgeSoft.COE.Framework.Common
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class LoaderBase
    {
        [NonSerialized]
        private ApplicationData _appConfigData = null;
        [NonSerialized]
        private ServiceData _serviceConfigData = null;
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
        public void GetConfigData(string databaseName, string serviceName, string appName)
        {
            if (_databaseConfigData == null) _databaseConfigData = ConfigurationUtilities.GetDatabaseData(databaseName);
            if (_dbmsTypeData == null) _dbmsTypeData = ConfigurationUtilities.GetDBMSTypeData(_databaseConfigData.DBMSType);
            if (_dbmsTypeData.DatabaseGlobalUser != string.Empty)
            {  //re get databaseConfigData based on global user
                _databaseConfigData = ConfigurationUtilities.GetDatabaseData(_dbmsTypeData.DatabaseGlobalUser);
                _databaseConfigData.OriginalOwner = databaseName;
            }
            else { _databaseConfigData.OriginalOwner = databaseName; }
            if (_appConfigData == null && appName != string.Empty) _appConfigData = ConfigurationUtilities.GetApplicationData(appName);
            if (_serviceConfigData == null) _serviceConfigData = ConfigurationUtilities.GetServiceData(serviceName);

        }

        /// <summary>
        /// Populates the database, service and application configuration data from its names.
        /// </summary>
        /// <param name="databaseName">The database schema name</param>
        /// <param name="serviceName">The service name</param>
        /// <param name="appName">The application name</param>
        /// <param name="notFromCache">use cached items or not</param>
        public void GetConfigData(string databaseName, string serviceName, string appName, bool notFromCache)
        {
            if (_databaseConfigData == null) _databaseConfigData = ConfigurationUtilities.GetDatabaseData(databaseName, notFromCache);
            if (_dbmsTypeData == null) _dbmsTypeData = ConfigurationUtilities.GetDBMSTypeData(_databaseConfigData.DBMSType);
            if (_dbmsTypeData.DatabaseGlobalUser != string.Empty)
            {  //re get databaseConfigData based on global user
                _databaseConfigData = ConfigurationUtilities.GetDatabaseData(_dbmsTypeData.DatabaseGlobalUser);
                _databaseConfigData.OriginalOwner = databaseName;
            }
            else { _databaseConfigData.OriginalOwner = databaseName; }
            if (_appConfigData == null && appName != string.Empty) _appConfigData = ConfigurationUtilities.GetApplicationData(appName);
            if (_serviceConfigData == null) _serviceConfigData = ConfigurationUtilities.GetServiceData(serviceName);

        }
    }
}

