using System;
using System.Data;
using System.Data.SqlClient;
using Csla;
using Csla.Data;
using Csla.Validation;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.Properties;
using CambridgeSoft.COE.Framework.COELoggingService;
using System.Xml;
using CambridgeSoft.COE.Framework.Common.Messaging;
using CambridgeSoft.COE.Framework.COEConfigurationService;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using System.Configuration;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using CambridgeSoft.COE.Framework.Caching;


namespace CambridgeSoft.COE.Framework.COEConfigurationService
{
    [Serializable()]
    public class COEConfigurationBO : Csla.BusinessBase<COEConfigurationBO>, ICacheable
    {
        #region Member variables
        private string _description;
        private string _sectionHandlerClassName;
        private ConfigurationSection _configurationSection;
        private string _applicationName;        

        [NonSerialized]
        IConfigurationSource _configurationSource;

        [NonSerialized]
        private DAL _coeDAL = null;

        [NonSerialized]
        private DALFactory _dalFactory = new DALFactory();
        private string _serviceName = "COEConfiguration";

        [NonSerialized]
        public const string FrameworkName = "cambridgesoft.coe.framework";

        [NonSerialized]
        static COELog _coeLog = COELog.GetSingleton("COEConfiguration");
        #endregion

        #region Properties
        public string ApplicationName
        {
            set
            {
                CanWriteProperty();
                if (value == null)
                    value = string.Empty;

                if (_applicationName != value.Trim())
                {
                    _applicationName = value.Trim();
                    _configurationSource = this.GetConfigurationSource(_applicationName);
                    PropertyHasChanged();
                }
            }
            get
            {
                CanReadProperty();
                return _applicationName;
            }
        }

        public string Description
        {
            set
            {
                CanWriteProperty();
                if (value == null)
                    value = string.Empty;

                if (_description != value.Trim())
                {
                    _description = value.Trim();
                    PropertyHasChanged();
                }
            }
            get
            {
                CanReadProperty();
                return _description;
            }
        }

        public ConfigurationSection ConfigurationSection
        {
            get
            {
                CanReadProperty();
                return _configurationSection;
            }
            set
            {
                CanWriteProperty();

                if (_configurationSection != value)
                {
                    _configurationSection = value;
                    PropertyHasChanged();
                }
            }
        }


        public static string DefaultConfigurationFilePath
        {
            get
            {
                return ConfigurationBaseFilePath + "COEFrameworkConfig.xml";
            }
        }


        public static string ConfigurationBaseFilePath
        {
            get
            {
                System.Reflection.Assembly MyAssembly =
                System.Reflection.Assembly.GetExecutingAssembly();
                System.Version AppVersion = MyAssembly.GetName().Version;
                string returnValue = string.Empty;
                string COEConfigPathOverride = string.Empty;
                string currentPath = string.Empty;
                string appName = COEAppName.Get();
                if (AppDomain.CurrentDomain.GetData(appName + "configPath") != null && AppDomain.CurrentDomain.GetData(appName + "configPath").ToString().Contains("PerkinElmer"))
                {
                    currentPath = AppDomain.CurrentDomain.GetData(appName + "configPath").ToString();
                }
                else
                {
                    if (Csla.ApplicationContext.GlobalContext["COEConfigPathOverride"] != null)
                    {
                        currentPath = Csla.ApplicationContext.GlobalContext["COEConfigPathOverride"].ToString() + @"\PerkinElmer\ChemOfficeEnterprise" + @"\";
                        AppDomain.CurrentDomain.SetData(appName + "configPath", currentPath);
                    }
                    else
                    {

                        currentPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\PerkinElmer\ChemOfficeEnterprise" + @"\";
                        AppDomain.CurrentDomain.SetData(appName + "configPath", currentPath);
                    }
                }


                return currentPath;

            }
        }

        #endregion

        #region Constructors

        private COEConfigurationBO()
        { /* require use of factory method */ }
        #endregion

        #region Validation Rules

        private void AddCommonRules()
        {
            ValidationRules.AddRule(CommonRules.StringRequired, "Description");
            ValidationRules.AddRule(CommonRules.StringMaxLength, new CommonRules.MaxLengthRuleArgs("Description", 255));
            /*ValidationRules.AddRule(CommonRules.MinValue<SmartDate>, new CommonRules.MinValueRuleArgs<SmartDate>("DateCreated", new SmartDate("1/1/2005")));
            ValidationRules.AddRule(CommonRules.RegExMatch, new CommonRules.RegExRuleArgs("DateCreated", @"(0[1-9]|1[012])[- /.](0[1-9]|[12][0-9]|3[01])[- /.](19|20)\d\d"));
             */
        }

        protected override void AddBusinessRules()
        {
            AddCommonRules();
        }
        #endregion //Validation Rules

        #region BusinessMethods
        private IConfigurationSource GetConfigurationSource(string applicationName)
        {
            switch (applicationName.Trim().ToLower())
            {
                case FrameworkName:
                    return new FileConfigurationSource(DefaultConfigurationFilePath);
                default:

                    if (_coeDAL == null)
                        LoadDAL();

                    return new SqlConfigurationSource(_coeDAL);
            }
        }

        private void LoadDAL()
        {
            if (_dalFactory == null) { _dalFactory = new DALFactory(); }
            this._dalFactory.GetDAL<CambridgeSoft.COE.Framework.COEConfigurationService.DAL>(ref _coeDAL, _serviceName, Resources.CentralizedStorageDB, true);
        }

        private void LoadDAL(ApplicationData appConfigData, ServiceData serviceConfigData, DatabaseData databaseConfigData, DBMSTypeData dbmsTypeData)
        {
            if (_dalFactory == null) { _dalFactory = new DALFactory(); }
            this._dalFactory.GetDAL<CambridgeSoft.COE.Framework.COEConfigurationService.DAL>(ref _coeDAL, appConfigData, serviceConfigData, databaseConfigData, dbmsTypeData, false);
        }
        #endregion

        #region Factory Methods

        public static COEConfigurationBO New(string description)
        {
            return New(new Criteria(description));
        }

        public static COEConfigurationBO New(string applicationName, string description)
        {
            return New(new Criteria(applicationName, description));
        }

        private static COEConfigurationBO New(Criteria criteria)
        {
            if (!CanAddObject())
                throw new System.Security.SecurityException(Resources.UserNotAuthorizedForAddObject + " COEConfigurationBO");

            string keyString = criteria.ToString();
            COEConfigurationBO result = LocalCache.Get(keyString, typeof(COEConfigurationBO)) as COEConfigurationBO;
            if (!ServerCache.Exists(keyString, typeof(COEConfigurationBO)) && result == null) //Exists is first to refresh the sliding time
            {
                result = DataPortal.Create<COEConfigurationBO>(criteria);
                LocalCache.Add(keyString, typeof(COEConfigurationBO), result, LocalCache.NoAbsoluteExpiration, TimeSpan.FromMinutes(60), COECacheItemPriority.Normal);
            }
            else if (result == null)
            {
                result = ServerCache.Get(keyString, typeof(COEConfigurationBO)) as COEConfigurationBO;
                LocalCache.Add(keyString, typeof(COEConfigurationBO), result, LocalCache.NoAbsoluteExpiration, TimeSpan.FromMinutes(60), COECacheItemPriority.Normal);
            }
            return result;
        }

        /// <summary>
        /// Obtains "description" configuration section from Framework configuration file
        /// </summary>
        /// <param name="description"></param>
        /// <returns></returns>
        public static COEConfigurationBO Get(string description)
        {
            return Get(new Criteria(description), false);
        }
        /// <summary>
        /// Obtains "description" configuration entry from "applicationName" configuration. If applicationName is different to cambridgesoft.coe.framework then the configuration is stored on database.
        /// </summary>
        /// <param name="applicationName"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        public static COEConfigurationBO Get(string applicationName, string description)
        {
            return Get(new Criteria(applicationName, description), false);
        }

        /// <summary>
        /// Gets CoeConfigBo checking if it's have to read from cache or re read them from the source
        /// </summary>
        /// <param name="applicationName"></param>
        /// <param name="description"></param>
        /// <param name="notFromCache"></param>
        /// <returns></returns>
        public static COEConfigurationBO Get(string applicationName, string description, bool notFromCache)
        {
            return Get(new Criteria(applicationName, description), notFromCache);
        }

        private static COEConfigurationBO Get(Criteria criteria, bool notFromCache)
        {
            if (!CanGetObject())
                throw new System.Security.SecurityException(Resources.UserNotAuthorizedForViewObject + " COEConfigurationBO");

            string keyString = criteria.ToString();
            COEConfigurationBO result = null;
            if (notFromCache)
            {
                result = DataPortal.Fetch<COEConfigurationBO>(criteria);
                LocalCache.Add(criteria.ToString(), typeof(COEConfigurationBO), result, LocalCache.NoAbsoluteExpiration, TimeSpan.FromMinutes(60), COECacheItemPriority.Normal);
            }
            else
            {
                result = LocalCache.Get(keyString, typeof(COEConfigurationBO)) as COEConfigurationBO;

                if (!ServerCache.Exists(keyString, typeof(COEConfigurationBO)) && result == null) //Exists is first to refresh the sliding time
                {
                    result = DataPortal.Fetch<COEConfigurationBO>(criteria);
                    LocalCache.Add(keyString, typeof(COEConfigurationBO), result, LocalCache.NoAbsoluteExpiration, TimeSpan.FromMinutes(60), COECacheItemPriority.Normal);
                }
                else if (result == null)
                {
                    result = ServerCache.Get(keyString, typeof(COEConfigurationBO)) as COEConfigurationBO;
                    LocalCache.Add(keyString, typeof(COEConfigurationBO), result, LocalCache.NoAbsoluteExpiration, TimeSpan.FromMinutes(60), COECacheItemPriority.Normal);
                }
            }

            return result;
        }

        /// <summary>
        /// Remove the BO from local cache. The key string of the cache is in below format:
        ///   <applicationName> + "_" + <description> + "COEConfigurationBO"
        /// For example "cambridgesoft.coe.framework_coeconfiguration_COEConfigurationBO".
        /// </summary>
        /// <param name="applicationName"> application name to build the cache key string </param>
        /// <param name="description"> description name to build the cache key string </param>
        public static void RemoveLocalCache(string applicationName, string description)
        {
            var criteria = new Criteria(applicationName, description);
            LocalCache.Remove(criteria.ToString(), typeof(COEConfigurationBO));
        }

        public static void Delete(string description)
        {
            if (!CanDeleteObject())
                throw new System.Security.SecurityException(Resources.UserNotAuthorizedForDeleteObject + " COEConfigurationBO");
            Criteria criteria = new Criteria(description);
            DataPortal.Delete(criteria);
            LocalCache.Remove(criteria.ToString(), typeof(COEConfigurationBO));
            if (ServerCache.Exists(criteria.ToString(), typeof(COEConfigurationBO)))
                ServerCache.Remove(criteria.ToString(), typeof(COEConfigurationBO));
        }

        public override COEConfigurationBO Save()
        {
            if (IsDeleted && !CanDeleteObject())
                throw new System.Security.SecurityException(Resources.UserNotAuthorizedForDeleteObject + " COEConfigurationBO");
            else if (IsNew && !CanAddObject())
                throw new System.Security.SecurityException(Resources.UserNotAuthorizedForAddObject + " COEConfigurationBO");
            else if (!CanEditObject())
                throw new System.Security.SecurityException(Resources.UserNotAuthorizedForEditObject + " COEConfigurationBO");

            return base.Save();
        }

        public static bool CanAddObject()
        {
            // return Csla.ApplicationContext.User.IsInRole("CanSearch");
            return true;
        }

        public static bool CanGetObject()
        {
            // return Csla.ApplicationContext.User.IsInRole("CanSearch");
            return true;
        }

        public static bool CanEditObject()
        {
            // return Csla.ApplicationContext.User.IsInRole("CanSearch");
            return true;
        }

        public static bool CanDeleteObject()
        {
            // return Csla.ApplicationContext.User.IsInRole("CanSearch");
            return true;
        }

        public string GetConfigurationSettingsXml()
        {
            return this._coeDAL.GetConfigurationSettingsXml(this.ApplicationName);
        }

        public void SetConfigurationSettingsFromXml(string xml)
        {
            ConfigurationSection configSection = null;
            string sectionXmlStr = string.Empty;
            string sectionTypeClassName = string.Empty;

            if (_coeDAL == null)
                LoadDAL();

            this._coeDAL.GetSection(this.Description, ref sectionTypeClassName);

            sectionXmlStr = xml;

            if (!string.IsNullOrEmpty(sectionXmlStr))
            {
                Type sectionManagerType = Type.GetType(sectionTypeClassName);
                configSection = (ConfigurationSection)Activator.CreateInstance(sectionManagerType);

                StringReader stringReader = new StringReader(sectionXmlStr);

                XmlReaderSettings settings = new XmlReaderSettings();
                settings.CloseInput = true;
                settings.IgnoreComments = true;
                settings.IgnoreWhitespace = true;

                XmlReader reader = XmlReader.Create(stringReader, settings);

                MethodInfo info = configSection.GetType().GetMethod("DeserializeSection", BindingFlags.NonPublic | BindingFlags.Instance);
                info.Invoke(configSection, new object[] { reader });
                reader.Close();
            }

            if(this.ConfigurationSection is ApplicationDataConfigurationSection && configSection is ApplicationDataConfigurationSection)
            {
                ((ApplicationDataConfigurationSection) this.ConfigurationSection).Merge((ApplicationDataConfigurationSection) configSection);
                this.MarkDirty();
            }
            else
                this.ConfigurationSection = configSection;

            this.Save();
        }



        #endregion //Factory Methods

        #region Data Access

        #region Criteria

        [Serializable()]
        private class Criteria : IComparable
        {
            internal string _applicationName;
            internal string _description;

            //constructors
            public Criteria(string description)
                : this(COEConfigurationBO.FrameworkName, description)
            {
            }

            public Criteria(string applicationName, string description)
            {
                _applicationName = applicationName;
                _description = description;
            }

            public override string ToString()
            {
                return GetKey(_applicationName, _description);
            }
            #region IComparable Members
            public int CompareTo(object obj)
            {
                if (obj is Criteria)
                {
                    return CompareTo((Criteria)obj);
                }
                else
                    throw new Exception("Invalid types to Compare");
            }

            public int CompareTo(Criteria obj)
            {
                int result = -1;
                if (obj == null)
                    return 1;

                if (string.IsNullOrEmpty(_applicationName))
                {
                    if (string.IsNullOrEmpty(obj._applicationName))
                        return 0;
                    else
                        return -1;
                }
                else
                {
                    if (string.IsNullOrEmpty(obj._applicationName))
                        return 1;
                    else
                        result = this._applicationName.CompareTo(obj._applicationName);
                }

                if (result == 0)
                {
                    if (string.IsNullOrEmpty(_description))
                    {
                        if (string.IsNullOrEmpty(obj._description))
                            return 0;
                        else
                            return -1;
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(obj._description))
                            return 1;
                        else
                            result = this._description.CompareTo(obj._description);
                    }
                }
                return result;
            }

            #endregion
        }
        #endregion //Criteria
        [RunLocal]
        private void DataPortal_Create(Criteria criteria)
        {
            Description = criteria._description;
            ApplicationName = criteria._applicationName;
            ConfigurationSection = null;
            _cacheDependency = this.GetDependency(criteria._applicationName);
            LocalCache.Add(criteria.ToString(), typeof(COEConfigurationBO), this, LocalCache.NoAbsoluteExpiration, TimeSpan.FromMinutes(60), COECacheItemPriority.Normal);
        }

        private void DataPortal_Fetch(Criteria criteria)
        {
            ApplicationName = criteria._applicationName;
            Description = criteria._description;

            ConfigurationSection = _configurationSource.GetSection(Description);

            this.EncryptPasswords();

            this.InitializeCartridgeVersion();

            _cacheDependency = this.GetDependency(criteria._applicationName);
            LocalCache.Add(criteria.ToString(), typeof(COEConfigurationBO), this, LocalCache.NoAbsoluteExpiration, TimeSpan.FromMinutes(60), COECacheItemPriority.Normal);
        }

        /// <summary>
        /// If there are passwords in plain text, encrypt them.
        /// </summary>
        private void EncryptPasswords()
        {
            if (ApplicationName.ToLower() == FrameworkName && _configurationSection.SectionInformation.Name == "coeConfiguration")
            {
                bool unencryptedPasswords = false;
                bool fipsEnabled = ((COEConfigurationSettings)_configurationSection).ApplicationDefaults.FipsEnabled;
                foreach (DatabaseData dbData in ((COEConfigurationSettings)_configurationSection).Databases)
                {
                    if (!CambridgeSoft.COE.Framework.Common.Utilities.IsEncrypted(fipsEnabled, dbData.Password))
                    {
                        unencryptedPasswords = true;
                        break;
                    }
                }
                if (unencryptedPasswords)
                {
                    COEConfigurationSettings newSection = new COEConfigurationSettings();
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(_configurationSection.SectionInformation.GetRawXml());

                    foreach (XmlAttribute dbPass in doc.SelectNodes("//databases/add[string-length(@password) > 0]/@password"))
                    {
                        if (!CambridgeSoft.COE.Framework.Common.Utilities.IsEncrypted(fipsEnabled, dbPass.Value))
                            dbPass.Value = CambridgeSoft.COE.Framework.Common.Utilities.Encrypt(fipsEnabled, dbPass.Value);
                    }

                    newSection.SectionInformation.SetRawXml(doc.OuterXml);

                    ((FileConfigurationSource)_configurationSource).Remove(DefaultConfigurationFilePath, _configurationSection.SectionInformation.Name);
                    ((FileConfigurationSource)_configurationSource).Add(new FileConfigurationParameter(DefaultConfigurationFilePath), _configurationSection.SectionInformation.Name, newSection);
                }
            }
        }

        /// <summary>
        /// When fetching the configuration for the first time, the cartridge version needs to be retrieved from database.
        /// </summary>
        private void InitializeCartridgeVersion()
        {
            if (ApplicationName.ToLower() == FrameworkName)
            {
                COEConfigurationSettings settings = ConfigurationSection as COEConfigurationSettings;
                if (settings != null)
                {
                    DBMSTypeData dbmstype = settings.DBMSTypes.Get("ORACLE");
                    SQLGeneratorData sqlGeneratorData = dbmstype.SQLGeneratorData.Get("CSORACLECARTRIDGE");

                    if ((String.IsNullOrEmpty(sqlGeneratorData.ChemMajorVersion)) || (sqlGeneratorData.ChemMajorVersion == "0"))
                    {
                        //Typically the user is not loggued in yet, and thus the following is null, but just in case, lets remember the original user.
                        object originalUser = Csla.ApplicationContext.GlobalContext["TEMPUSERNAME"];
                        if (_coeDAL == null)
                        {
                            // Here we are trying to force a connection as COEDB
                            // This is accomplished by setting the TEMPMUSERNAME variable in GlobalContext
                            // but that variable is only used if the CSLA user is null
                            // In some cases the CSLA user may have been set to the windows process 
                            // identity so we must clear it first
                            //Csla.ApplicationContext.User = null;
                            //Will use COEDB as connection for this DAL.
                            Csla.ApplicationContext.GlobalContext["TEMPUSERNAME"] = Resources.CentralizedStorageDB;

                            var dataBaseConfigurationData = settings.Databases.Get(dbmstype.DatabaseGlobalUser);
                            dataBaseConfigurationData.FipsEabled = settings.ApplicationDefaults.FipsEnabled;

                            this.LoadDAL(settings.Applications.Get(COEAppName.Get()), settings.Services.Get(_serviceName), dataBaseConfigurationData, settings.DBMSTypes.Get("ORACLE"));
                        }
                        // Coverity Fix CID - 11480 
                        if (_coeDAL != null)
                        {
                            // Validate CSCARTRIDGE is installed or not
                            var isInstalled = _coeDAL.HasSchemaInstalled(sqlGeneratorData.Schema);
                            if (!isInstalled)
                            {
                                return;
                            }

                            string fullversion = _coeDAL.GetConfigurationValueFromTable(sqlGeneratorData.Schema, "Globals", "Value", "ID", "VERSION");
                            fullversion = fullversion.Replace("\"", ""); // Seems that some cartridge versions stores the value double quoted, as in "12.0.0.63"
                            string[] splitVersion = fullversion.Split((new string[] { "." }), StringSplitOptions.None);

                            sqlGeneratorData.ChemMajorVersion = splitVersion[0];
                            sqlGeneratorData.ChemMinorVersion = splitVersion[1];
                            //Then no one else should use this artificial dal we built:
                            _coeDAL = null;
                            //And we can restore the original user, if any:
                            Csla.ApplicationContext.GlobalContext["TEMPUSERNAME"] = originalUser;
                        }
                        else
                            throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));
                    }
                }
            }
        }

        protected override void DataPortal_Insert()
        {
            _configurationSource.Add(null, _description, this._configurationSection);
            LocalCache.Add(GetKey(_applicationName, _description), typeof(COEConfigurationBO), this, LocalCache.NoAbsoluteExpiration, TimeSpan.FromMinutes(60), COECacheItemPriority.Normal);
        }

        protected override void DataPortal_Update()
        {
            IConfigurationSource source = this.GetConfigurationSource(_applicationName);
            _configurationSource.Remove(null, _description);
            _configurationSource.Add(null, _description, this._configurationSection);
            LocalCache.Remove(GetKey(_applicationName, _description), typeof(COEConfigurationBO));
        }

        protected override void DataPortal_DeleteSelf()
        {
            DataPortal_Delete(new Criteria(_description));
        }

        private void DataPortal_Delete(Criteria criteria)
        {
            _configurationSource.Remove(null, _description);
            LocalCache.Remove(criteria.ToString(), typeof(COEConfigurationBO));
        }
        #endregion //Data Access


        protected override object GetIdValue()
        {
            return _description;
        }

        #region ICacheable Members
        [NonSerialized]
        private COECacheDependency _cacheDependency;
        /// <summary>
        /// Cache dependency that is build from the dal at dataportal_fetch time. Is the mechanism to get the cache updated when the underlying
        /// record changed in database.
        /// </summary>
        public COECacheDependency CacheDependency
        {
            get { return _cacheDependency; }
            set { _cacheDependency = value; }
        }

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
        /// Based on the application name, a file dependency to framework config is created, or a sql dependency.
        /// </summary>
        /// <param name="applicationName">application name</param>
        /// <returns>The proper cache dependency</returns>
        private COECacheDependency GetDependency(string applicationName)
        {
            COECacheDependency dependency = null;
            switch (applicationName.Trim().ToLower())
            {
                case FrameworkName:
                    dependency = new COECacheDependency(DefaultConfigurationFilePath);
                    break;
                default:

                    if (_coeDAL == null)
                        LoadDAL();

                    // Coverity Fix CID - 11479 
                    if (_coeDAL != null)                   
                        dependency = _coeDAL.GetDependency(applicationName);                   
                    else                    
                        throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));                    

                    break;
            }
            return dependency;
        }

        private static string GetKey(string applicationName, string description)
        {
            string result = string.IsNullOrEmpty(applicationName) ? string.Empty : applicationName.Trim().ToLower();
            result += string.IsNullOrEmpty(description) ? string.Empty : "_" + description.Trim().ToLower();
            return result;
        }
        #endregion
    }
}
