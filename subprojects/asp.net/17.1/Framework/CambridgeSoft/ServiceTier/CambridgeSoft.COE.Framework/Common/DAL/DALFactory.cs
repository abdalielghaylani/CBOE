using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Reflection;
using CambridgeSoft.COE.Framework.Properties;
using CambridgeSoft.COE.Framework.COEConfigurationService;


namespace CambridgeSoft.COE.Framework.Common
{
    /// <summary>
    /// Class for loading a DAL for a service. 
    /// </summary>
    /// <remarks>This will likely move to core.  However, at this time it doens't instantiate properly when in COE.CORE.COMMON</remarks>
    [Serializable]
    public class DALFactory : DALBase
    {
        private string ServiceBaseTypeName = null; //ConfigUtils.GetServicesBaseTypeName();
        private string ServiceBaseAssemblyName = null; //ConfigUtils.GetServicesBaseAssemblyName();

        /// <summary>
        /// Creates a DAL object for calling the DAL layer
        /// </summary>
        /// <param name="adminConn">Indicates if the connection is and admin connection or not</param>
        /// <param name="databaseName">The database schema name</param>
        /// <param name="serviceName">The service name</param>
        /// <param name="Tvariable">The DAL class modified by ref</param>
        public void GetDAL<T>(ref T Tvariable, string serviceName, string databaseName, bool adminOverride) where T : DALBase
        {
            this.GetConfigData(databaseName, serviceName, COEAppName.Get().ToString());
            if (DALManager == null) DALManager = DALUtils.GetDALManager(DatabaseConfigData, ServiceConfigData, DBMSTypeData,adminOverride);
            string dalProviderName = ConfigurationUtilities.GetDALProviderClass(ServiceConfigData, DatabaseConfigData.ProviderName);
            if (Tvariable == null) Tvariable = CreateDAL<T>(ServiceConfigData, dalProviderName);
            Tvariable.DALManager = DALManager;
            Tvariable.SetServiceSpecificVariables();
        }
        
        /// <summary>
        /// Configures the DAL class via reflection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Tvariable"></param>
        /// <param name="serviceName"></param>
        /// <param name="databaseName"></param>
        /// <param name="adminOverride"></param>
        /// <param name="typeName"></param>
        public void GetDAL<T>(ref T Tvariable, string serviceName, string databaseName, bool adminOverride, string typeName) where T : DALBase
        {
            this.GetConfigData(databaseName, serviceName, COEAppName.Get().ToString());
            if (DALManager == null) DALManager = DALUtils.GetDALManager(DatabaseConfigData, ServiceConfigData, DBMSTypeData, adminOverride);
            string dalProviderName = ConfigurationUtilities.GetDALProviderClass(ServiceConfigData, DatabaseConfigData.ProviderName);
            if (Tvariable == null) Tvariable = CreateDAL<T>(ServiceConfigData.DALProviderAssemblyNameFull, typeName);

            Tvariable.DALManager = DALManager;
            Tvariable.SetServiceSpecificVariables();
        }

        /// <summary>
        /// Creates a DAL object for calling the DAL layer
        /// </summary>
        /// <param name="adminConn">Indicates if the connection is and admin connection or not</param>
        /// <param name="databaseName">The database schema name</param>
        /// <param name="serviceName">The service name</param>
        /// <param name="Tvariable">The DAL class modified by ref</param>
        public void GetDAL<T>(ref T Tvariable, string serviceName, string databaseName, bool adminOverride, bool notFromCache) where T : DALBase
        {
            this.GetConfigData(databaseName, serviceName, COEAppName.Get().ToString(), notFromCache);
            if (DALManager == null) DALManager = DALUtils.GetDALManager(DatabaseConfigData, ServiceConfigData, DBMSTypeData, adminOverride);
            string dalProviderName = ConfigurationUtilities.GetDALProviderClass(ServiceConfigData, DatabaseConfigData.ProviderName);
            if (Tvariable == null) Tvariable = CreateDAL<T>(ServiceConfigData, dalProviderName);
            Tvariable.DALManager = DALManager;
            Tvariable.SetServiceSpecificVariables();
        }

        /// <summary>
        /// Overload to create default dal using user connection
        /// </summary>
        /// <param name="databaseName">The database schema name</param>
        /// <param name="serviceName">The service name</param>
        /// <param name="Tvariable">The DAL class modified by ref</param>
        public void GetDAL<T>(ref T Tvariable, string serviceName, string databaseName) where T : DALBase
        {
            this.GetDAL(ref Tvariable, serviceName, databaseName, false);
        }

        public void GetDAL<T>(ref T Tvariable, ApplicationData appConfigData, ServiceData serviceConfigData, DatabaseData databaseConfigData, DBMSTypeData dbmsTypeData, bool adminOverride) where T : DALBase
        {
            AppConfigData = appConfigData;
            ServiceConfigData = serviceConfigData;
            DatabaseConfigData = databaseConfigData;
            DBMSTypeData = dbmsTypeData;
            
            if (DALManager == null) DALManager = DALUtils.GetDALManager(DatabaseConfigData, ServiceConfigData, DBMSTypeData, adminOverride);
            string dalProviderName = ConfigurationUtilities.GetDALProviderClass(ServiceConfigData, DatabaseConfigData.ProviderName);
            if (Tvariable == null) Tvariable = CreateDAL<T>(ServiceConfigData, dalProviderName);
            Tvariable.DALManager = DALManager;
            Tvariable.SetServiceSpecificVariables();
        }

        private void GetConfigData(string databaseData,string serviceName,string appName)
        {
            base.GetConfigData(databaseData, serviceName, appName);
        }

        private T CreateDAL<T>(string assemblyName, string typeName) where T : DALBase
        {
            object DAL = null;
            try
            {
                DAL = Activator.CreateInstance(Assembly.Load(assemblyName).GetType(typeName, true));
                if (DAL is T)
                {
                    return DAL as T;
                }
                else
                {
                    throw new InvalidOperationException(Resources.DoesNotExtendDAL + typeName);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return DAL as T;
        }
        
        private T CreateDAL<T>(ServiceData serviceData, string dalTypeName) where T : DALBase
        {
            object DAL = null;
            try
            {
                string className = serviceData.DALProviderAssemblyNameShort + "." + serviceData.Name + "Service." + dalTypeName;
                DAL = Activator.CreateInstance(Assembly.Load(serviceData.DALProviderAssemblyNameFull).GetType(className, true));
                if (DAL is T)
                {
                    return DAL as T;
                }
                else
                {
                    throw new InvalidOperationException(Resources.DoesNotExtendDAL + serviceData.Name);
                }
            }
            catch (Exception e)
            {
                string className = serviceData.DALProviderAssemblyNameShort + "." + serviceData.Name + "Service.DAL";
                DAL = Activator.CreateInstance(Assembly.Load(serviceData.DALProviderAssemblyNameFull).GetType(className, true));
                
                return DAL as T;
            }
        }       
    }
}
