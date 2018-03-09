using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using CambridgeSoft.COE.Framework.Common;
using System.Configuration;
using CambridgeSoft.COE.Framework.Properties;
using CambridgeSoft.COE.Framework.COELoggingService;
using System.Linq;
using Csla;

namespace CambridgeSoft.COE.Framework.COEConfigurationService
{
    public class ConfigurationUtilities
    {
        [NonSerialized]
        static COELog _coeLog = COELog.GetSingleton("COEConfiguration");
        private static COEConfigurationManager configurationManager = null;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        public static ServiceData GetServiceData(string serviceName)
        {
            COEConfigurationBO configurationBO = COEConfigurationBO.Get("CambridgeSoft.COE.Framework", COEConfigurationSettings.SectionName);
            COEConfigurationSettings configSettings = (COEConfigurationSettings)configurationBO.ConfigurationSection;

            ServiceData serviceData = configSettings.Services.Get(serviceName);
            if (serviceData == null)
            {
                throw new ConfigurationErrorsException(string.Format(Resources.Culture, Resources.UnableToFindServicesInstance, serviceName));
            }
            return serviceData;
        }


        private static InstanceData _COEInstance;

        public static InstanceData GetCOEInstance()
        {
            if (_COEInstance==null)
            {
                COEConfigurationBO configurationBO = COEConfigurationBO.Get("CambridgeSoft.COE.Framework", COEConfigurationSettings.SectionName);
                COEConfigurationSettings configSettings = (COEConfigurationSettings)configurationBO.ConfigurationSection; 
                _COEInstance= configSettings.Instances.FirstOrDefault(ins=>ins.IsCBOEInstance);
                if (_COEInstance == null)
                {
                    _coeLog.Log("COEFrameworkConfig.xml error, must have one COE instance configured.");
                }
            }
            return _COEInstance;
        }

        public static string GetDefaultDataSource()
        {
            COEConfigurationBO configurationBO = COEConfigurationBO.Get("CambridgeSoft.COE.Framework", COEConfigurationSettings.SectionName);
            COEConfigurationSettings configSettings = (COEConfigurationSettings)configurationBO.ConfigurationSection;

            InstanceData ins = GetCOEInstance();

            return ins.SID;
        }

        public static DBMSTypeData GetDBMSTypeData(DBMSType dbmsType)
        {
            COEConfigurationBO configurationBO = COEConfigurationBO.Get("CambridgeSoft.COE.Framework", COEConfigurationSettings.SectionName);
            COEConfigurationSettings configSettings = (COEConfigurationSettings)configurationBO.ConfigurationSection;

            DBMSTypeData dbmsTypeData = configSettings.DBMSTypes.Get(dbmsType.ToString());

            return dbmsTypeData;
        }

        public static ApplicationData GetApplicationData(string appName)
        {
            COEConfigurationBO configurationBO = COEConfigurationBO.Get("CambridgeSoft.COE.Framework", COEConfigurationSettings.SectionName);
            COEConfigurationSettings configSettings = (COEConfigurationSettings)configurationBO.ConfigurationSection;

            ApplicationData appData = configSettings.Applications.Get(appName);
            if (appData == null)
            {
                throw new ConfigurationErrorsException(string.Format(Resources.Culture, Resources.UnableToFindApplicationsInstance, appName));
            }

            FillEmptyAppDataWithDefault(appData, configSettings.ApplicationDefaults);

            return appData;
        }

        private static void FillEmptyAppDataWithDefault(ApplicationData appData, ApplicationDefaultsData defaults)
        {
            if (defaults.SearchServiceData.Count > 0)
            {
                if (appData.MaxRecordCount < 1 && defaults.SearchServiceData.MaxRecordCount > 0)
                    appData.MaxRecordCount = defaults.SearchServiceData.MaxRecordCount;
                if (appData.PartialHitlistCommitSize < 1 && defaults.SearchServiceData.PartialHitlistCommitSize > 0)
                    appData.PartialHitlistCommitSize = defaults.SearchServiceData.PartialHitlistCommitSize;
                if (appData.PartialHitlistFirstCommitSize < 1 && defaults.SearchServiceData.PartialHitlistFirstCommitSize > 0)
                    appData.PartialHitlistFirstCommitSize = defaults.SearchServiceData.PartialHitlistFirstCommitSize;
                if (string.IsNullOrEmpty(appData.ReturnPartialHitlist) && !string.IsNullOrEmpty(defaults.SearchServiceData.ReturnPartialHitlist))
                    appData.ReturnPartialHitlist = defaults.SearchServiceData.ReturnPartialHitlist;
                if (string.IsNullOrEmpty(appData.SaveQueryHistory) && !string.IsNullOrEmpty(defaults.SearchServiceData.SaveQueryHistory))
                    appData.SaveQueryHistory = defaults.SearchServiceData.SaveQueryHistory;
                if (string.IsNullOrEmpty(appData.UseRealTableNames) && !string.IsNullOrEmpty(defaults.SearchServiceData.UseRealTableNames))
                    appData.UseRealTableNames = defaults.SearchServiceData.UseRealTableNames;
            }
            if (defaults.ChemDrawEmbedData.Count > 0)
            {
                if (string.IsNullOrEmpty(appData.ShowPluginDownload) && !string.IsNullOrEmpty(defaults.ChemDrawEmbedData.ShowPluginDownload))
                    appData.ShowPluginDownload = defaults.ChemDrawEmbedData.ShowPluginDownload;
                if (string.IsNullOrEmpty(appData.PluginDownloadURL) && !string.IsNullOrEmpty(defaults.ChemDrawEmbedData.PluginDownloadURL))
                    appData.PluginDownloadURL = defaults.ChemDrawEmbedData.PluginDownloadURL;
                if (string.IsNullOrEmpty(appData.DownloadChemDrawImageSrc) && !string.IsNullOrEmpty(defaults.ChemDrawEmbedData.DownloadChemDrawImageSrc))
                    appData.DownloadChemDrawImageSrc = defaults.ChemDrawEmbedData.DownloadChemDrawImageSrc;
            }

            if (appData.CachingData == null)
            {
                appData.CachingData = new CachingData();
            }

            if ((appData.CachingData.Dataview == null ||
                appData.CachingData.Dataview.Count == 0) && 
                defaults.CachingData != null && defaults.CachingData.Dataview != null)
            {
                appData.CachingData.Dataview = defaults.CachingData.Dataview;
            }

            if ((appData.CachingData.SearchCriteria == null ||
                appData.CachingData.SearchCriteria.Count == 0) &&
                defaults.CachingData != null && defaults.CachingData.SearchCriteria != null)
            {
                appData.CachingData.SearchCriteria = defaults.CachingData.SearchCriteria;
            }

            if ((appData.CachingData.Form == null ||
                appData.CachingData.Form.Count == 0) &&
                defaults.CachingData != null && defaults.CachingData.Form != null)
            {
                appData.CachingData.Form = defaults.CachingData.Form;
            }

            if (string.IsNullOrEmpty(appData.DisplayCulture) && !string.IsNullOrEmpty(defaults.DisplayCulture))
                appData.DisplayCulture = defaults.DisplayCulture;

            if (string.IsNullOrEmpty(appData.DateFormat) && !string.IsNullOrEmpty(defaults.DateFormat))
                appData.DateFormat = defaults.DateFormat;

            if (appData.SafeExportSize < 1)
                appData.SafeExportSize = defaults.SafeExportSize;
        }

        public static ApplicationDefaultsData GetApplicationDefaultsData()
        {
            COEConfigurationBO configurationBO = COEConfigurationBO.Get("CambridgeSoft.COE.Framework", COEConfigurationSettings.SectionName);
            COEConfigurationSettings configSettings = (COEConfigurationSettings)configurationBO.ConfigurationSection;
            return configSettings.ApplicationDefaults;
        }

        public static ExportFormatterData GetExportFormatterData(string exportTypeName)
        {
            COEConfigurationBO configurationBO = COEConfigurationBO.Get("CambridgeSoft.COE.Framework", COEConfigurationSettings.SectionName);
            COEConfigurationSettings configSettings = (COEConfigurationSettings)configurationBO.ConfigurationSection;

            ExportFormatterData exportFormatterData = configSettings.ExportFormatters.Get(exportTypeName);
            if (exportFormatterData == null)
            {
                throw new ConfigurationErrorsException(string.Format(Resources.Culture, Resources.UnableToFindApplicationsInstance, exportTypeName));
            }

            return exportFormatterData;
        }


        public static List<string> GetExportFormatterTypeNames()
        {
            COEConfigurationBO configurationBO = COEConfigurationBO.Get("CambridgeSoft.COE.Framework", COEConfigurationSettings.SectionName);
            COEConfigurationSettings configSettings = (COEConfigurationSettings)configurationBO.ConfigurationSection;

            COENamedElementCollection<ExportFormatterData> exportFormatters = configSettings.ExportFormatters;
            List<string> _formatTypesList = new List<string>();
            foreach (ExportFormatterData exportFormatterData in exportFormatters)
            {
                _formatTypesList.Add(exportFormatterData.Name);
            }
            return _formatTypesList;
        }

        public static bool ExportTypeExists(string exportTypeName)
        {
            bool result = false;
            try
            {
                ExportFormatterData exporterData = GetExportFormatterData(exportTypeName);
                if (exporterData != null)
                {
                    result = true;
                }
            }
            catch
            {

            }

            return result;
        }

        /// <summary>
        /// Gets the database data forcing to get the latest saved configuration
        /// </summary>
        /// <param name="databaseName">Db bname</param>
        /// <param name="notFromCache">Search from cache or not</param>
        /// <returns></returns>
        public static DatabaseData GetDatabaseData(string databaseName, bool notFromCache = false)
        {
            COEConfigurationBO configurationBO = COEConfigurationBO.Get("CambridgeSoft.COE.Framework", COEConfigurationSettings.SectionName, notFromCache);
            var configSettings = (COEConfigurationSettings)configurationBO.ConfigurationSection;
            
            DatabaseData databaseData = configSettings.Databases.Get(databaseName);

			if (databaseData == null)
            {
                throw new ApplicationException(string.Format("{0} tag not found in <databases> section of COEFrameworkConfig.xml file", databaseName));
            }

            if (Guid.Empty == databaseData.InstanceId)
            {
                databaseData.InstanceData = GetMainInstance();
                databaseData.InstanceId = databaseData.InstanceData.Id;
            }
            else
            {
                databaseData.InstanceData = GetInstanceData(databaseData.InstanceId);
            }

            return databaseData;
        }

        /// <summary>
        /// Get static COEConfigurationManager object of this class.
        /// </summary>
        /// <returns></returns>
        public static COEConfigurationManager GetConfigurationManager()
        {
            if (configurationManager == null)
            {
                //BLLFactory factory = new BLLFactory();
                //factory.GetBLL<COEConfigurationManager>(ref configurationManager, "Cambridgesoft.COE.Framework", "Configuration");
                configurationManager = new COEConfigurationManager();
            }

            return configurationManager;
        }

        public static bool ReloadConfigurationManager(string appName)
        {
            // configurationManager = null;
            //GetConfigurationManager();
            //GetApplicationData(appName);
            return true;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceData"></param>
        /// <param name="providerName"></param>
        /// <returns></returns>
        public static string GetDALProviderClass(ServiceData serviceData, string providerName)
        {
            string returnString = string.Empty;
            DALProviderData dalProviderData = serviceData.DALProviders.Get(providerName);

            if (dalProviderData == null)
            {
                //since there are no dalProviders in there configuration file, we will use the default name for the name provider
                //this default name is the provider name with the "." removed plus the work "DAL".

                string[] inputTokens = providerName.Split('.');
                string outputToken = "";

                foreach (string inputToken in inputTokens)
                {
                    if (outputToken == string.Empty)
                    {
                        outputToken = inputToken;
                    }
                    else
                    {
                        outputToken = outputToken + inputToken;
                    }
                }
                outputToken = outputToken + "DAL";
                returnString = outputToken;
            }
            else
            {   //if a dal provider name IS in the configuration file it is likely an overload
                returnString = dalProviderData.DALClass;
            }
            return returnString;
        }


        /// <summary>
        /// This is just a hack to avoid harcoding in SQLGenerator.
        /// </summary>
        /// <returns></returns>
        public static string GetMappingsPath()
        {
            AppSettingsSection appSettingsSection = (AppSettingsSection)GetConfigurationManager().GetSection(null, "CambridgeSoft.COE.Framework", "appSettings");
            string path = appSettingsSection.Settings["COEFilesPath"].Value + @"xsd\XML Examples\SelectClauseClassMappings.xml";
            return path;
        }

        /// <summary>
        /// Get COE Mappings XML string
        /// </summary>
        /// <param name="notFromCache"> read from cache or data portal </param>
        /// <returns>the XML strin gof Mappings section </returns>
        public static string GetMappingsXml(bool notFromCache = false)
        {
            return COEMappingsBO.Get(notFromCache).MappingXml;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appData"></param>
        /// <param name="engineName"></param>
        /// <returns></returns>
        public static string GetChemEngineSchema(DatabaseData databaseData)
        {
            string returnString = GetSQLGeneratorData(databaseData).Schema;
            return returnString;
        }

        public static string GetChemEngineSchema(string databaseName)
        {
            DatabaseData databaseData = GetDatabaseData(databaseName);
            string returnString = GetSQLGeneratorData(databaseData).Schema;
            return returnString;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static string GetChemEngineSchema(DBMSType dbmsType)
        {
            var chemEngineName = string.Empty;
 
            switch (dbmsType)
            {
                case DBMSType.ORACLE:
                    chemEngineName = "CSORACLECARTRIDGE";
                    break;
                case DBMSType.SQLSERVER:
                    chemEngineName = "MOLSERVER";
                    break;
                case DBMSType.MSACCESS:
                    chemEngineName = "MOLSERVER";
                    break;
            }

            var configurationBO = COEConfigurationBO.Get(
                "CambridgeSoft.COE.Framework", COEConfigurationSettings.SectionName);

            var configSettings = (COEConfigurationSettings)configurationBO.ConfigurationSection;
            DBMSTypeData dbmsTypeData = configSettings.DBMSTypes.Get(dbmsType.ToString());
            SQLGeneratorData sqlGeneratorData = dbmsTypeData.SQLGeneratorData.Get(chemEngineName);

            if (sqlGeneratorData == null)
            {
                return ConfigurationUtilities.GetCOEInstance().SQLGeneratorData.Get("CSORACLECARTRIDGE").Schema;
                //TODO: this is not the correct way to get sqlGeneratorData
                //throw new ConfigurationErrorsException(
                //  string.Format(Resources.Culture, Resources.UnableToFindDAlProviderInstance, chemEngineName));
            }
              
            return sqlGeneratorData.Schema;
        }


        public static SQLGeneratorData GetSQLGeneratorData(DatabaseData databaseData)
        {
            string ChemEngineName = string.Empty;

            switch (databaseData.DBMSType)
            {
                case DBMSType.ORACLE:
                    ChemEngineName = "CSORACLECARTRIDGE";
                    break;
                case DBMSType.SQLSERVER:
                    ChemEngineName = "MOLSERVER";
                    break;
                case DBMSType.MSACCESS:
                    ChemEngineName = "MOLSERVER";
                    break;

            }
            var configurationBO = COEConfigurationBO.Get("CambridgeSoft.COE.Framework", COEConfigurationSettings.SectionName);
            var configSettings = (COEConfigurationSettings)configurationBO.ConfigurationSection;

            InstanceData instacneData = ConfigurationUtilities.GetInstanceData(databaseData.InstanceId);
            var sqlGeneratorData = instacneData.SQLGeneratorData.Get(ChemEngineName);

            return sqlGeneratorData;
        }

        /// <summary>
        /// Gets the temp query table name.
        /// </summary>
        /// <param name="databaseName">The database name.</param>
        /// <returns>The temp query table name with schema name combined.</returns>
        public static string GetTempQueryTable(string databaseName)
        {
            DatabaseData databaseData = GetDatabaseData(databaseName);
            SQLGeneratorData sqlGeneratorData = GetSQLGeneratorData(databaseData);

            string returnString = sqlGeneratorData.Schema;
            returnString += returnString.Length > 0 ? "." + sqlGeneratorData.TempQueries : sqlGeneratorData.TempQueries;
            return returnString.ToUpper();
        }

        /// <summary>
        /// Gets the sql generate data for cs cartridge.
        /// </summary>
        /// <param name="databaseName">The database name.</param>
        /// <returns>The sql generate data instance.</returns>
        public static SQLGeneratorData GetCartridgeData(string databaseName)
        {
            DatabaseData databaseData = GetDatabaseData(databaseName);
            SQLGeneratorData sqlGeneratorData = GetSQLGeneratorData(databaseData);

            return sqlGeneratorData;
        }

        public static string GetCartridgeMajorVersion(DBMSType dbmsType)
        {
            string ChemEngineName = string.Empty;

            switch (dbmsType)
            {
                case DBMSType.ORACLE:
                    ChemEngineName = "CSORACLECARTRIDGE";
                    break;
                case DBMSType.SQLSERVER:
                    ChemEngineName = "MOLSERVER";
                    break;
                case DBMSType.MSACCESS:
                    ChemEngineName = "MOLSERVER";
                    break;

            }
            InstanceData instanceData = ConfigurationUtilities.GetInstanceData(GetMainInstance().Id);
            var sqlGeneratorData = instanceData.SQLGeneratorData.Get(ChemEngineName);

            return sqlGeneratorData.ChemMajorVersion;
        }

        public static string GetCartridgeMinorVersion(DBMSType dbmsType)
        {
            string ChemEngineName = string.Empty;

            switch (dbmsType)
            {
                case DBMSType.ORACLE:
                    ChemEngineName = "CSORACLECARTRIDGE";
                    break;
                case DBMSType.SQLSERVER:
                    ChemEngineName = "MOLSERVER";
                    break;
                case DBMSType.MSACCESS:
                    ChemEngineName = "MOLSERVER";
                    break;

            }
            COEConfigurationBO configurationBO = COEConfigurationBO.Get("CambridgeSoft.COE.Framework", COEConfigurationSettings.SectionName);
            COEConfigurationSettings configSettings = (COEConfigurationSettings)configurationBO.ConfigurationSection;

            DBMSTypeData dbmsTypeData = configSettings.DBMSTypes.Get(dbmsType.ToString());
            SQLGeneratorData sqlGeneratorData = dbmsTypeData.SQLGeneratorData.Get(ChemEngineName);

            return sqlGeneratorData.ChemMinorVersion;
        }

        public static string GetSingleSignOnURL()
        {
            COEConfigurationBO configurationBO = COEConfigurationBO.Get("CambridgeSoft.COE.Framework", COEConfigurationSettings.SectionName);
            COEConfigurationSettings configSettings = (COEConfigurationSettings)configurationBO.ConfigurationSection;

            string returnString = string.Empty;
            if (configSettings.SingleSignOnURL != string.Empty)
            {
                returnString = configSettings.SingleSignOnURL;
            }
            else
            {
                returnString = string.Empty;
            }
            return returnString;
        }

        /// <summary>
        /// Gets the value of the Manage Configuration Settings attribute that allows the edition of admin settings
        /// </summary>
        /// <returns></returns>
        public static bool GetManageConfigurationSettings()
        {
            COEConfigurationBO configurationBO = COEConfigurationBO.Get("CambridgeSoft.COE.Framework", COEConfigurationSettings.SectionName);
            COEConfigurationSettings configSettings = (COEConfigurationSettings)configurationBO.ConfigurationSection;

            bool returnBoolean = false;
            if (configSettings.ManageConfigurationSettings != string.Empty)
            {
                returnBoolean = bool.Parse(configSettings.ManageConfigurationSettings);
            }
            else
            {
                returnBoolean = false;
            }
            return returnBoolean;
        }


        /// <summary>
        /// Gets the default SSO provider (COELDAP, CSSecurity) which determines the behavior of the CBOE Manager Security
        /// </summary>
        /// <returns></returns>
        public static string GetDefaultSingleSignOnProvider()
        {
            coesinglesignon.SingleSignOn singleSignOn = new coesinglesignon.SingleSignOn();
            singleSignOn.Url = ConfigurationUtilities.GetSingleSignOnURL();

            return singleSignOn.GetDefaultAuthenticationProvider();
        }
        /// <summary>
        /// Retrieves all the AppNames reading from the  configuration file that is associated with the framework located in  C:\Documents and Settings\All Users\Application Data\CambridgeSoft\ChemOfficeEnterprisex.x.x.x COEFrameworkConfig.xml
        /// </summary>
        /// <returns></returns>
        public static List<string> GetAllAppNamesInConfig()
        {
            COEConfigurationBO configurationBO = COEConfigurationBO.Get("CambridgeSoft.COE.Framework", COEConfigurationSettings.SectionName);
            COEConfigurationSettings configSettings = (COEConfigurationSettings)configurationBO.ConfigurationSection;

            List<string> myAppNameList = null;
            myAppNameList = new List<string>();
            COENamedElementCollection<ApplicationData> myApps = configSettings.Applications;
            foreach (ApplicationData var in myApps)
            {
                myAppNameList.Add(var.Name);
            }
            return myAppNameList;
        }

        public static List<string> GetAppByDatabase(DBMSType dbType)
        {
            COEConfigurationBO configurationBO = COEConfigurationBO.Get("CambridgeSoft.COE.Framework", COEConfigurationSettings.SectionName);
            COEConfigurationSettings configSettings = (COEConfigurationSettings)configurationBO.ConfigurationSection;

            List<string> myAppNameList = null;
            List<string> myDBNameList = null;
            myDBNameList = new List<string>();//Sumeet: Change should be done from myAppNameList to myDBNameList.
            myAppNameList = new List<string>();
            COENamedElementCollection<DatabaseData> myDatabases = configSettings.Databases;
            COENamedElementCollection<ApplicationData> myApps = configSettings.Applications;
            foreach (DatabaseData var in myDatabases)
            {
                if (var.DBMSType == dbType)
                    myDBNameList.Add(var.Name);
            }
            foreach (ApplicationData var2 in myApps)
            {
                for (int i = 0; i < myDBNameList.Count - 1; i++)
                {
                    if (var2.Database == myDBNameList[i].ToString()) { myAppNameList.Add(var2.Name); };
                }
            }
            return myAppNameList;
        }

        public static List<DBMSTypeData> GetAllDBMSTypesInConfig()
        {
            COEConfigurationBO configurationBO = COEConfigurationBO.Get("CambridgeSoft.COE.Framework", COEConfigurationSettings.SectionName);
            COEConfigurationSettings configSettings = (COEConfigurationSettings)configurationBO.ConfigurationSection;

            List<DBMSTypeData> myDBMSTypes = configSettings.DBMSTypes.ToList();

            return myDBMSTypes;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="databaseName"></param>
        /// <returns></returns>
        public static ApplicationData GetAppNameByDatabase(string databaseName)
        {
            COEConfigurationBO configurationBO = COEConfigurationBO.Get("CambridgeSoft.COE.Framework", COEConfigurationSettings.SectionName);
            COEConfigurationSettings configSettings = (COEConfigurationSettings)configurationBO.ConfigurationSection;

            COENamedElementCollection<ApplicationData> myApps = configSettings.Applications;

            try
            {
                foreach (ApplicationData var in myApps)
                {
                    DatabaseData myDatabase = GetDatabaseData(var.Database);
                    if (myDatabase.Name.ToUpper() == databaseName.ToUpper())
                    {
                        return var;
                    }
                }
            }
            catch (Exception e)
            {
            }
            return null;


        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="owner"></param>
        /// <returns></returns>
        public static ApplicationData GetAppNameByOwner(string owner)
        {
            COEConfigurationBO configurationBO = COEConfigurationBO.Get("CambridgeSoft.COE.Framework", COEConfigurationSettings.SectionName);
            COEConfigurationSettings configSettings = (COEConfigurationSettings)configurationBO.ConfigurationSection;

            COENamedElementCollection<ApplicationData> myApps = configSettings.Applications;

            try
            {
                foreach (ApplicationData var in myApps)
                {
                    DatabaseData myDatabase = GetDatabaseData(var.Database);
                    if (myDatabase.Owner.ToString().ToUpper() == owner.ToUpper())
                    {
                        return var;
                    }
                }
            }
            catch (Exception e)
            {
            }
            return null;

        }

        public static List<String> GetAllInstancesNameInConfig()
        {
            List<String> instanceList = new List<String>();

            COEConfigurationBO configurationBO = COEConfigurationBO.Get("CambridgeSoft.COE.Framework", COEConfigurationSettings.SectionName);
            COEConfigurationSettings configSettings = (COEConfigurationSettings)configurationBO.ConfigurationSection;

            COENamedElementCollection<InstanceData> instancesColl = configSettings.Instances;

            foreach (InstanceData item in instancesColl)
            {
                instanceList.Add(item.InstanceName);
            }

            return instanceList;
        }

        public static List<InstanceData> GetAllInstancesInConfig()
        {
            List<InstanceData> instanceList = new List<InstanceData>();

            COEConfigurationBO configurationBO = COEConfigurationBO.Get("CambridgeSoft.COE.Framework", COEConfigurationSettings.SectionName, true);
            COEConfigurationSettings configSettings = (COEConfigurationSettings)configurationBO.ConfigurationSection;

            COENamedElementCollection<InstanceData> instancesColl = configSettings.Instances;

            foreach (InstanceData item in instancesColl)
            {
                instanceList.Add(item);
            }

            return instanceList;
        }

        public static InstanceData GetMainInstance()
        {
            List<InstanceData> instanceList = new List<InstanceData>();

            COEConfigurationBO configurationBO = COEConfigurationBO.Get("CambridgeSoft.COE.Framework", COEConfigurationSettings.SectionName);
            COEConfigurationSettings configSettings = (COEConfigurationSettings)configurationBO.ConfigurationSection;

            COENamedElementCollection<InstanceData> instancesColl = configSettings.Instances;

            return instancesColl.FirstOrDefault(instance => instance.IsCBOEInstance);
        }

        public static InstanceData GetInstanceData(string instanceName)
        {
            COEConfigurationBO configurationBO = COEConfigurationBO.Get("CambridgeSoft.COE.Framework", COEConfigurationSettings.SectionName);
            COEConfigurationSettings configSettings = (COEConfigurationSettings)configurationBO.ConfigurationSection;
            InstanceData instanceData = configSettings.Instances.Get(instanceName);

            return instanceData;
        }

        /// <summary>
        /// Gets the instance data by instance id.
        /// </summary>
        /// <param name="instanceId">The instance unique id.</param>
        /// <returns>
        /// The matched instance data will be returned.
        /// </returns>
        public static InstanceData GetInstanceData(Guid instanceId)
        {
            COEConfigurationBO configurationBO = COEConfigurationBO.Get("CambridgeSoft.COE.Framework", COEConfigurationSettings.SectionName);
            COEConfigurationSettings configSettings = (COEConfigurationSettings)configurationBO.ConfigurationSection;

            InstanceData instanceData = configSettings.Instances.Where(i => i.Id == instanceId).FirstOrDefault();

            return instanceData;
        }

        /// <summary>
        /// Gets the instance name of the database.
        /// </summary>
        /// <param name="database">The database name.</param>
        /// <returns>
        /// The matched database inst
        /// </returns>
        public static string GetInstanceNameByDatabaseName(string database)
        {
            COEConfigurationBO configurationBO = COEConfigurationBO.Get("CambridgeSoft.COE.Framework", COEConfigurationSettings.SectionName);
            COEConfigurationSettings configSettings = (COEConfigurationSettings)configurationBO.ConfigurationSection;

            var databaseData = configSettings.Databases.Where(d => d.Name.Equals(database, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();

            if (databaseData != null)
            {
                // The legacy schema would not have instance id defined in the configuration. In this case, it should come from the primary(main) datasource(instance).
                if (null == databaseData.InstanceId || databaseData.InstanceId.Equals(Guid.Empty))
                {
                    databaseData.InstanceId = GetMainInstance().Id;
                }

                var instanceData = GetInstanceData(databaseData.InstanceId);
                if (instanceData != null)
                {
                    return instanceData.Name;
                }
            }

            return null;
        }

        /// <summary>
        /// Get instance global user for the given database
        /// </summary>
        /// <param name="databaseName"> A database name </param>
        /// <returns> The global user name </returns>
        public static string GetDatabaseGlobalUser(string databaseName)
        {
            var instanceName = GetInstanceNameByDatabaseName(databaseName);
            var instance = GetInstanceData(instanceName);
            var databases = GetDatabasesByInstance(instance);
            return databases.First(db => db.Owner == instance.DatabaseGlobalUser).Name;
        }

        /// <summary>
        /// Retrieves all the DatabaseNames reading from the  configuration file that is associated with the framework located in C:\Documents and Settings\All Users\Application Data\CambridgeSoft\ChemOfficeEnterprisex.x.x.x COEFrameworkConfig.xml
        /// </summary>
        /// <returns></returns>
        public static List<string> GetAllDatabaseNamesInConfig()
        {
            COEConfigurationBO configurationBO = COEConfigurationBO.Get("CambridgeSoft.COE.Framework", COEConfigurationSettings.SectionName);
            COEConfigurationSettings configSettings = (COEConfigurationSettings)configurationBO.ConfigurationSection;

            List<string> myDatabaseNameList = null;
            myDatabaseNameList = new List<string>();
            COENamedElementCollection<DatabaseData> myDatbaseNames = configSettings.Databases;
            foreach (DatabaseData var in myDatbaseNames)
            {
                myDatabaseNameList.Add(var.Name);
            }
            return myDatabaseNameList;
        }
        /// <summary>
        /// Creates and returns a list of DatabaseNames in FrameWorkConfig that has the DBMSType of type provided.
        /// </summary>
        /// <param name="dbType">Type of the DBMS required</param>
        /// <returns>List of Database Names in the frameworkConfig</returns>
        public static List<string> GetDatabaseNameByType(DBMSType dbType)
        {
            COEConfigurationBO configurationBO = COEConfigurationBO.Get("CambridgeSoft.COE.Framework", COEConfigurationSettings.SectionName);
            COEConfigurationSettings configSettings = (COEConfigurationSettings)configurationBO.ConfigurationSection;

            List<string> myDatabaseNameList = new List<string>();
            COENamedElementCollection<DatabaseData> myDatbaseNames = configSettings.Databases;
            foreach (DatabaseData var in myDatbaseNames)
            {
                if (var.DBMSType == dbType)
                    myDatabaseNameList.Add(var.Name);
            }
            return myDatabaseNameList;
        }

        /// <summary>
        /// Retrieves all the DatabaseNames reading from the  configuration file that is associated with the framework located in  C:\Documents and Settings\All Users\Application Data\CambridgeSoft\ChemOfficeEnterprisex.x.x.x COEFrameworkConfig.xml
        /// </summary>
        /// <returns></returns>
        public static List<string> GetAllDatabaseNamesInConfig(bool filterByUserPrivs)
        {
            COEConfigurationBO configurationBO = COEConfigurationBO.Get("CambridgeSoft.COE.Framework", COEConfigurationSettings.SectionName);
            COEConfigurationSettings configSettings = (COEConfigurationSettings)configurationBO.ConfigurationSection;

            //toDO filter by user privs
            List<string> myDatabaseNameList = null;
            myDatabaseNameList = new List<string>();
            COENamedElementCollection<DatabaseData> myDatbaseNames = configSettings.Databases;
            foreach (DatabaseData var in myDatbaseNames)
            {
                myDatabaseNameList.Add(var.Name);
            }
            return myDatabaseNameList;
        }

        /// <summary>
        /// Get the databaseName associated with an Application section in the config file
        /// </summary>
        /// <param name="appName"></param>
        /// <returns></returns>
        public static string GetDatabaseNameFromAppName(string appName)
        {
            ApplicationData myAppData = GetApplicationData(appName);
            return myAppData.Database;

        }
        public static COEHomeSettings GetHomeData()
        {

            COEConfigurationBO configurationBO = COEConfigurationBO.Get("CambridgeSoft.COE.Framework", COEHomeSettings.SectionName);
            COEHomeSettings configSettings = (COEHomeSettings)configurationBO.ConfigurationSection;


            return configSettings;
        }

        public static ApplicationHome GetApplicationHomeData(string appName)
        {
            ApplicationData appData = GetApplicationData(appName);
            ApplicationHome appHome = appData.ApplicationHome;
            return appHome;
        }

        public static string GetFrameworkVersion()
        {
            System.Reflection.Assembly MyAssembly = System.Reflection.Assembly.GetExecutingAssembly();
            System.Version AppVersion = MyAssembly.GetName().Version;
            return AppVersion.ToString();
        }

        /// <summary>
        /// Return the assembly file version
        /// </summary>
        /// <returns>Assembly file version</returns>
        public static string GetFrameworkFileVersion()
        {
            string retVal = "Uknown";
            object[] fileversionAttributte;
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            if (assembly != null)
            {
                fileversionAttributte = assembly.GetCustomAttributes(typeof(System.Reflection.AssemblyFileVersionAttribute), false);
                if (fileversionAttributte != null && fileversionAttributte.Length > 0)
                    retVal = ((System.Reflection.AssemblyFileVersionAttribute)fileversionAttributte[0]).Version;
            }
            return retVal;
        }

        public static ActionLinkCollection GetActionLinks(string appName, string parentAppName, string formId)
        {
            ApplicationData appData = GetApplicationData(appName);
            if (appData != null)
            {
                if (appData.FormBehaviour.Count > 0 &&
                    !string.IsNullOrEmpty(formId) &&
                    appData.FormBehaviour[formId] != null)
                {
                    return appData.FormBehaviour[formId].ActionLinks;
                }
                else
                {
                    if (appData.ParentApplication.Count > 0 &&
                        !string.IsNullOrEmpty(parentAppName) &&
                        appData.ParentApplication.Get(parentAppName) != null &&
                        appData.ParentApplication.Get(parentAppName).ApplicationBehaviour != null)
                    {

                        return appData.ParentApplication.Get(parentAppName).ApplicationBehaviour.ActionLinks;
                    }
                }
            }
            return null;
        }

        public static ChemDrawOptions GetChemDrawOptions(string appName, string parentAppName, string formId)
        {
            ApplicationData appData = GetApplicationData(appName);
            if (appData != null)
            {
                if (appData.FormBehaviour.Count > 0 &&
                    !string.IsNullOrEmpty(formId) &&
                    appData.FormBehaviour[formId] != null)
                {
                    return appData.FormBehaviour[formId].ChemDrawOptions;
                }
                else
                {
                    if (appData.ParentApplication.Count > 0 &&
                        !string.IsNullOrEmpty(parentAppName) &&
                        appData.ParentApplication.Get(parentAppName) != null &&
                        appData.ParentApplication.Get(parentAppName).ApplicationBehaviour != null)
                    {

                        return appData.ParentApplication.Get(parentAppName).ApplicationBehaviour.ChemDrawOptions;
                    }
                }
            }
            return null;
        }

        public static QueryManagement GetQueryManagement(string appName, string parentAppName, string formId)
        {
            ApplicationData appData = GetApplicationData(appName);
            if (appData != null)
            {
                if (appData.FormBehaviour.Count > 0 &&
                    !string.IsNullOrEmpty(formId) &&
                    appData.FormBehaviour[formId] != null &&
                    appData.FormBehaviour[formId].LeftPanelOptions != null)
                {
                    return appData.FormBehaviour[formId].LeftPanelOptions.QueryManagement;
                }
                else
                {
                    if (appData.ParentApplication.Count > 0 &&
                        !string.IsNullOrEmpty(parentAppName) &&
                        appData.ParentApplication.Get(parentAppName) != null &&
                        appData.ParentApplication.Get(parentAppName).ApplicationBehaviour != null &&
                        appData.ParentApplication.Get(parentAppName).ApplicationBehaviour.LeftPanelOptions != null)
                    {

                        return appData.ParentApplication.Get(parentAppName).ApplicationBehaviour.LeftPanelOptions.QueryManagement;
                    }
                }
            }
            return null;
        }

        public static ExportManagement GetExportManagement(string appName, string parentAppName, string formId)
        {
            ApplicationData appData = GetApplicationData(appName);
            if (appData != null)
            {
                if (appData.FormBehaviour.Count > 0 &&
                    !string.IsNullOrEmpty(formId) &&
                    appData.FormBehaviour[formId] != null &&
                    appData.FormBehaviour[formId].LeftPanelOptions != null)
                {
                    return appData.FormBehaviour[formId].LeftPanelOptions.ExportManagement;
                }
                else
                {
                    if (appData.ParentApplication.Count > 0 &&
                        !string.IsNullOrEmpty(parentAppName) &&
                        appData.ParentApplication.Get(parentAppName) != null &&
                        appData.ParentApplication.Get(parentAppName).ApplicationBehaviour != null &&
                        appData.ParentApplication.Get(parentAppName).ApplicationBehaviour.LeftPanelOptions != null)
                    {

                        return appData.ParentApplication.Get(parentAppName).ApplicationBehaviour.LeftPanelOptions.ExportManagement;
                    }
                }
            }
            return null;
        }

        public static LeftPanelOptions.PanelState GetDefaultPanelState(string appName, string parentAppName, string formId)
        {
            ApplicationData appData = GetApplicationData(appName);
            if (appData != null)
            {
                if (appData.FormBehaviour.Count > 0 &&
                    !string.IsNullOrEmpty(formId) &&
                    appData.FormBehaviour[formId] != null &&
                    appData.FormBehaviour[formId].LeftPanelOptions != null &&
                    appData.FormBehaviour[formId].LeftPanelOptions.DefaultPanelState != LeftPanelOptions.PanelState.NotSet)
                {
                    return appData.FormBehaviour[formId].LeftPanelOptions.DefaultPanelState;
                }
                else
                {
                    if (appData.ParentApplication.Count > 0 &&
                        !string.IsNullOrEmpty(parentAppName) &&
                        appData.ParentApplication.Get(parentAppName) != null &&
                        appData.ParentApplication.Get(parentAppName).ApplicationBehaviour != null &&
                        appData.ParentApplication.Get(parentAppName).ApplicationBehaviour.LeftPanelOptions != null &&
                        appData.ParentApplication.Get(parentAppName).ApplicationBehaviour.LeftPanelOptions.DefaultPanelState != LeftPanelOptions.PanelState.NotSet)
                    {

                        return appData.ParentApplication.Get(parentAppName).ApplicationBehaviour.LeftPanelOptions.DefaultPanelState;
                    }
                }
            }
            return LeftPanelOptions.PanelState.Expanded;
        }

        public static SearchPreferences GetSearchPreferences(string appName, string parentAppName, string formId)
        {
            ApplicationData appData = GetApplicationData(appName);
            if (appData != null)
            {
                if (appData.FormBehaviour.Count > 0 &&
                    !string.IsNullOrEmpty(formId) &&
                    appData.FormBehaviour[formId] != null &&
                    appData.FormBehaviour[formId].LeftPanelOptions != null)
                {
                    return appData.FormBehaviour[formId].LeftPanelOptions.SearchPreferences;
                }
                else
                {
                    if (appData.ParentApplication.Count > 0 &&
                        !string.IsNullOrEmpty(parentAppName) &&
                        appData.ParentApplication.Get(parentAppName) != null &&
                        appData.ParentApplication.Get(parentAppName).ApplicationBehaviour != null &&
                        appData.ParentApplication.Get(parentAppName).ApplicationBehaviour.LeftPanelOptions != null)
                    {

                        return appData.ParentApplication.Get(parentAppName).ApplicationBehaviour.LeftPanelOptions.SearchPreferences;
                    }
                }
            }
            return null;
        }

        public static MenuOptions GetMenuOptions(string appName, string parentAppName, string formId)
        {
            ApplicationData appData = GetApplicationData(appName);
            if (appData != null)
            {
                if (appData.FormBehaviour.Count > 0 &&
                    !string.IsNullOrEmpty(formId) &&
                    appData.FormBehaviour[formId] != null &&
                    appData.FormBehaviour[formId].MenuOptions != null)
                {
                    return appData.FormBehaviour[formId].MenuOptions;
                }
                else
                {
                    if (appData.ParentApplication.Count > 0 &&
                        !string.IsNullOrEmpty(parentAppName) &&
                        appData.ParentApplication.Get(parentAppName) != null &&
                        appData.ParentApplication.Get(parentAppName).ApplicationBehaviour != null &&
                        appData.ParentApplication.Get(parentAppName).ApplicationBehaviour.MenuOptions != null)
                    {

                        return appData.ParentApplication.Get(parentAppName).ApplicationBehaviour.MenuOptions;
                    }
                }
            }
            return null;
        }

        public static COENamedElementCollection<CambridgeSoft.COE.Framework.Common.NameValueConfigurationElement> GetCBVGeneralOptions(string appName, string parentAppName, string formId)
        {
            ApplicationData appData = GetApplicationData(appName);
            if (appData != null)
            {
                if (appData.FormBehaviour.Count > 0 &&
                    !string.IsNullOrEmpty(formId) &&
                    appData.FormBehaviour[formId] != null &&
                    appData.FormBehaviour[formId].GeneralOptions != null &&
                    appData.FormBehaviour[formId].GeneralOptions.Count > 0)
                {
                    return appData.FormBehaviour[formId].GeneralOptions;
                }
                else
                {
                    if (appData.ParentApplication.Count > 0 &&
                        !string.IsNullOrEmpty(parentAppName) &&
                        appData.ParentApplication.Get(parentAppName) != null &&
                        appData.ParentApplication.Get(parentAppName).ApplicationBehaviour != null &&
                        appData.ParentApplication.Get(parentAppName).ApplicationBehaviour.GeneralOptions != null)
                    {

                        return appData.ParentApplication.Get(parentAppName).ApplicationBehaviour.GeneralOptions;
                    }
                }
            }
            return null;
        }
        /// <summary>
        /// Fetching the SQLGeneratorData 
        /// Setting the MolFileFormat Default value to V2000
        /// </summary>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        public static SQLGeneratorData GetMolFileFormat(DBMSType dbmsType)
        {
            string ChemEngineName = string.Empty;
            switch (dbmsType)
            {
                case DBMSType.ORACLE:
                    ChemEngineName = "CSORACLECARTRIDGE";
                    break;
                case DBMSType.SQLSERVER:
                    ChemEngineName = "MOLSERVER";
                    break;
                case DBMSType.MSACCESS:
                    ChemEngineName = "MOLSERVER";
                    break;

            }

            COEConfigurationBO configurationBO = COEConfigurationBO.Get("CambridgeSoft.COE.Framework", COEConfigurationSettings.SectionName);
            COEConfigurationSettings configSettings = (COEConfigurationSettings)configurationBO.ConfigurationSection;

            DBMSTypeData dbmsTypeData = configSettings.DBMSTypes.Get(dbmsType.ToString());
            SQLGeneratorData sqlGeneratorData = dbmsTypeData.SQLGeneratorData.Get(ChemEngineName);
            //Setting Default Value to V2000
            if (sqlGeneratorData.MolFileFormat == null || sqlGeneratorData.MolFileFormat == string.Empty)
            {
                sqlGeneratorData.MolFileFormat = "V2000";
            }
            return sqlGeneratorData;
        }

        /// <summary>
        /// Retrieves all the AppNames reading from the  configuration file that is associated with the framework located in  C:\Documents and Settings\All Users\Application Data\CambridgeSoft\ChemOfficeEnterprisex.x.x.x COEFrameworkConfig.xml
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<DatabaseData> GetDatabasesByInstance(InstanceData instanceData)
        {
            COEConfigurationBO configurationBO = COEConfigurationBO.Get("CambridgeSoft.COE.Framework", COEConfigurationSettings.SectionName);
            var configSettings = (COEConfigurationSettings)configurationBO.ConfigurationSection;
            IEnumerable<DatabaseData> result = configSettings.Databases.Where(db => db.InstanceId == instanceData.Id);

            if (instanceData.IsCBOEInstance)
            {
                IEnumerable<DatabaseData> databasesWithoutInstanceName =
                    configSettings.Databases.Where(db => db.InstanceId == Guid.Empty || db.InstanceId == instanceData.Id);
                result = result.Union(databasesWithoutInstanceName);
            }

            return result;
        }

        public static COESpotFireSettingsBO GetSpotFireSettings(bool notFromCache = false)
        {
            return COESpotFireSettingsBO.Get(notFromCache);
        }

        public static void SetSpotFireSettings(string url,string user, string password)
        {
            var bo = COESpotFireSettingsBO.Get();
            bo.SpotfireURL = url;
            bo.SpotfireUser = user;
            bo.SpotfirePassword = password;
            bo.Save();
        }
    }
}
