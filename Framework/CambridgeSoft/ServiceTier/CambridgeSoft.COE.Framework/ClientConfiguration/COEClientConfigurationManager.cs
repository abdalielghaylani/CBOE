using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.ComponentModel;
using CambridgeSoft.COE.Framework.COELoggingService;
using CambridgeSoft.COE.Framework.Common;
using System.Xml;


namespace CambridgeSoft.COE.Framework.COEConfigurationService
{
    /// <summary>
    /// This class provides the interface for desktop clients to manager client specific configuration
    /// </summary>
    [Serializable]
    public class COEClientConfigurationManager
    {
       #region Member Variables
        protected COEClientConfigurationBO _coeLoggingConfigurationBO = null;
		 //this class will contian the items that a client can set.
        private bool _loggingEnabled;
        private string _serverName;
        private string _serverProtocol;
        const string WEBSERVICE_URL = "/COEWebServiceHost/WebServicePortal.asmx";
        //const string webServerProxy="Csla.DataPortalClient.WebServicesProxy";
        private string _logEntryIdentifier;
        private CatagoryTypes _categories;
        private System.Diagnostics.SourceLevels _severity;
        private int _priority;

	   #endregion

       #region Constructor
		 public COEClientConfigurationManager()
        {
            
            GetSettings();
        } 
	   #endregion

       #region Properties

         #region CSLA settings
        /// <summary>
        /// Server name for connecting to middle tier
        /// </summary>

        [Browsable(true)]
        [ReadOnly(false)]
        [Description("CBOE middle tier server name")]
        [Category("Middle Tier Server")]
        [DisplayName("Server Name")]
        public string ServerName
        {
            get { return _serverName; }
            set { _serverName = value; } 
	    
        }
        #endregion

        #region Logging settings

        /// <summary>
        /// Specifies whether logging is turned on or off.
        /// </summary>
		[Browsable(true)]
        [ReadOnly(false)] 
        [Description("turns logging on and off")] 
        [Category("Logging")]
        [DisplayName("Logging Enabled")] 
        public bool LoggingEnabled
        {
            get {  return _loggingEnabled; }
            set { _loggingEnabled = value; }
        }

        /// <summary>
        /// Specifies whether logging is turned on or off.
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Description("indicates the name used when making log entries. Also specifies the folder name where the logging info is ssaved")]
        [Category("Logging")]
        [DisplayName("Log entry identifier")]
        public string LogEntryIdentifier
        {
            get { return _logEntryIdentifier; }
            set { _logEntryIdentifier = value; }
        }
       
        /// <summary>
        /// Specifies whether logging is turned on or off.
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Description("indicates which framework services to log. ")]
        [Category("Logging")]
        [DisplayName("Categories to log")]
        public CatagoryTypes Categories
        {
            get { return _categories; }
            set { _categories = value; }
        }

        ///// <summary>
        ///// Specifies whether logging is turned on or off.
        ///// </summary>
        //[Browsable(true)]
        //[ReadOnly(false)]
        //[Description("a value between 1 and 10 that indicates sub levels of logging")]
        //[Category("Logging")]
        //[DisplayName("Log entry identifier")]
        //public int Priority
        //{
        //    get { return _priority; }
        //    set { _priority = value; }
        //}

         
        /// <summary>
        /// Specifies whether logging is turned on or off.
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Description("indicates the level of information to log")]
        [Category("Logging")]
        [DisplayName("Severity")]
        public System.Diagnostics.SourceLevels Severity
        {
            get { return _severity; }
            set { _severity = value; }
        }
 
	#endregion

        #endregion

       #region Save Settings Public Method

        public void SaveSettings()
        {
            //csla seetings

             UpdateAppSetting("CslaDataPortalUrl", GetFullServerURL());

           //because of stranges issues with saving editted custom configuraiton sections, you need to create a new configuration section object and pass that into the save method

            COELoggingConfiguration coeLoggingConfiguration = new COELoggingConfiguration();
            //set to new property values

            coeLoggingConfiguration.Enabled = System.Convert.ToString(_loggingEnabled);
            coeLoggingConfiguration.LogEntryIdentifier = _logEntryIdentifier;
            coeLoggingConfiguration.Categories = _categories;
            coeLoggingConfiguration.Priority = "10";
            coeLoggingConfiguration.Severity = _severity;
            _coeLoggingConfigurationBO.Save(coeLoggingConfiguration);

        }
		 
	   #endregion		

       #region Get Settings Private Method
        private void GetSettings()
        {
            //csla settings
             _serverName = GetServerName(ConfigurationManager.AppSettings["CslaDataPortalUrl"]);
             _serverProtocol = GetServerProtocol(ConfigurationManager.AppSettings["CslaDataPortalUrl"]);

            //logging settings
            _coeLoggingConfigurationBO = COEClientConfigurationBO.Get(COELoggingConfiguration.SectionName);
            //coverity fix
            ConfigurationSection coeLoggingConfigurationSection = _coeLoggingConfigurationBO.ConfigurationSection;
            if (coeLoggingConfigurationSection is COELoggingConfiguration)
            {
                COELoggingConfiguration coeLoggingConfiguration = (COELoggingConfiguration)coeLoggingConfigurationSection;
                _loggingEnabled = System.Convert.ToBoolean(coeLoggingConfiguration.Enabled);
                _severity = coeLoggingConfiguration.Severity;
                _logEntryIdentifier = coeLoggingConfiguration.LogEntryIdentifier;
                _priority = System.Convert.ToInt16(coeLoggingConfiguration.Priority);
                _categories = coeLoggingConfiguration.Categories;
            }
           
        }
        #endregion

       #region Private Helper Methods
        private string GetServerName(string serverName)
        {
            if (serverName != null && serverName != string.Empty)
            {
                string[] urlTokens = serverName.Split('/');
                serverName = urlTokens[2];

            }
            else
            {
                serverName = string.Empty;
            }

            return serverName;
        }

        private string GetServerProtocol(string serverPath)
        {
            if(!string.IsNullOrEmpty(serverPath) && serverPath.ToLower().StartsWith("https"))
                return "https://";
            else
                return "http://";
        }

        private string GetFullServerURL()
        {
            
            return _serverProtocol + _serverName + WEBSERVICE_URL;
        }

        private void UpdateAppSetting(string key, string val)
        {

            bool remoteConfig= false;
            string configurationPath = string.Empty;
                        
            Configuration appSettingsSection = ConfigurationManager.OpenExeConfiguration(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile.Replace(".config", ""));

            //need to get the actual location of the file, because saving to a config file with AppSettings file="" doens't work.
            //first we need to determine where the file is. If it is next to the exe and it has the full Configuration section.
            //If it is relocated using the file= attribute then it only has appSettings section
            string remotePath = appSettingsSection.AppSettings.File;
            if (remotePath != string.Empty && remotePath != null)
            {
                remoteConfig = true;
                configurationPath = remotePath;
            }else{
                remoteConfig= false;
                configurationPath = appSettingsSection.FilePath;
            }

            XmlDocument xDoc = new XmlDocument();
            string sFileName = configurationPath;
            try
            {
                // load the configuration file
                xDoc.Load(sFileName);
                // find the node of interest containing the key using XPATH
                XmlNode theNode = null;
                if (remoteConfig == true)
                {
                     theNode = xDoc.SelectSingleNode(@"/appSettings/add[@key = '" + key + "\']");
                }
                else
                {
                     theNode = xDoc.SelectSingleNode(@"/configuration/appSettings/add[@key = '" + key + "\']");
                }
                // Set the new value for the node
                if (theNode != null)
                    theNode.Attributes["value"].Value = val;
                // lop off file prefix of the filename if it exists
                if (sFileName.StartsWith("file:///"))
                    sFileName = sFileName.Remove(0, 8);
                // save the new configuration settings
                xDoc.Save(sFileName);
                xDoc = null;
                ConfigurationManager.RefreshSection("appSettings");
                // ConfigurationManager.RefreshSection(sFileName +  "/AppSettings");
            }
            catch (Exception ex)
            {
                
                xDoc = null;
            }
        }
	#endregion
    }
}
