using System;
using System.Collections.Generic;
using System.Text;
using CambridgeSoft.COE.Framework.Properties;
using CambridgeSoft.COE.Framework.Common;
using System.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using System.Reflection;
using CambridgeSoft.COE.Framework.COELoggingService;


namespace CambridgeSoft.COE.Framework.COEConfigurationService
{
    /// <summary>
    /// Class for managing tasks releated to Query Managdment
    /// </summary>
    public class COEConfigurationManager : BLLBase
    {
        #region constants
        public const string FrameworkName = "cambridgesoft.coe.framework";
        [NonSerialized]
         static COELog _coeLog = COELog.GetSingleton("COEConfiguration");

        #endregion

        #region  manager implementation required by COE
        #region variables
        private DAL configurationDAL = null;
        private DALFactory dalFactory = new DALFactory();
        private Dictionary<string, IConfigurationSource> configurationSources = null;
        #endregion

        #region Constructors
        /// <summary>
        /// Default construction for the User Setting Manager. This is used when
        /// instantiation is done via a client directly referencing the services assembly
        /// </summary>
        public COEConfigurationManager()
        {
            this.ServiceName = "Configuration";
            this.AppName = "CORE";
            this.configurationSources = new Dictionary<string, IConfigurationSource>();
        }

        /// <summary>
        /// Constructor used for the Schema Manager when instantiation is through a webservice
        /// </summary>
        /// <param name="securityInfo"></param>
        public COEConfigurationManager(SecurityInfo securityInfo) : this()
        {
            this.SecurityInfo = securityInfo;
        }
        #endregion

        /// <summary>
        /// Load the ConfigurationDAL used by the HitListManager
        /// </summary>
        private void LoadDAL()
        {
            if (dalFactory == null) { dalFactory = new DALFactory(); }
            this.dalFactory.GetDAL<CambridgeSoft.COE.Framework.COEConfigurationService.DAL>(ref this.configurationDAL, this.ServiceName, this.AppName);
        }
        #endregion


        //methods that form the public api and call methods in the managers DAL classes to perform storage operations
        #region Manager specific implemenation


        public ConfigurationSection GetSection(SecurityInfo securityInfo, string applicationName, string sectionName)
        {
            ConfigurationSection section = null;

            try
            {
                if ((section = this.RetrieveSection(securityInfo, applicationName, sectionName)) == null &&
                        applicationName.Trim().ToLower() != FrameworkName)
                    section = this.RetrieveSection(securityInfo, FrameworkName, sectionName);
                return section;
            }
            catch (Exception ex)
            {
                
                throw;
            }
            
            
        }
        
        private ConfigurationSection RetrieveSection(SecurityInfo securityInfo, string applicationName, string sectionName)
        {
            this.SecurityInfo = securityInfo;

            IConfigurationSource source = null;

            try
            {
                if (configurationSources.ContainsKey(applicationName))
                    source = configurationSources[applicationName];
                else
                {
                    source = this.GetConfigurationSource(applicationName);
                    configurationSources.Add(applicationName, source);
                }
                return source.GetSection(sectionName);
            }
            catch (Exception ex)
            {
                
                throw;
            }
             
           
        }

        public void AddSection(SecurityInfo securityInfo, string applicationName, string sectionName, ConfigurationSection configurationSection)
        {
            IConfigurationSource source = null;

            this.SecurityInfo = securityInfo;

            try
            {
                if (configurationSources.ContainsKey(applicationName))
                    source = configurationSources[applicationName];
                else
                {
                    source = this.GetConfigurationSource(applicationName);
                    configurationSources.Add(applicationName, source);
                }

                source.Add(null, sectionName, configurationSection);
            }
            catch (Exception)
            {
                
                throw;
            }
        }

        public string GetSectionXml(SecurityInfo securityInfo, string applicationName, string sectionName)
        {
            this.SecurityInfo = securityInfo;

            ConfigurationSection section = this.GetSection(securityInfo, applicationName, sectionName);
            string sectionXml = string.Empty;
            if (section != null)
            {
                try
                {
                    MethodInfo info = section.GetType().GetMethod("SerializeSection", BindingFlags.NonPublic | BindingFlags.Instance);
                    sectionXml = (string)info.Invoke(section, new object[] { section, sectionName, ConfigurationSaveMode.Modified });
                    return sectionXml;
                }
                catch (Exception)
                {
                    
                    throw;
                }
                
            }
            else
                return string.Empty;
        }

        private IConfigurationSource GetConfigurationSource(string applicationName)
        {
            switch (applicationName.Trim().ToLower())
            {
                case FrameworkName:
                    return new FileConfigurationSource(GetDefaultConfigurationFilePath());
                default:
                    this.LoadDAL(applicationName);
                    return new SqlConfigurationSource(this.configurationDAL);
            }
        }

        public static string GetDefaultConfigurationFilePath()
        {
            return COEConfigurationBO.DefaultConfigurationFilePath;
        }



        private void LoadDAL(string appName)
        {
            //if (this.SecurityInfo == null)
            //    throw new Exception("Must supply security Info");
            if(dalFactory == null) { dalFactory = new DALFactory(); } 
            this.dalFactory.GetDAL<CambridgeSoft.COE.Framework.COEConfigurationService.DAL>(ref this.configurationDAL, this.ServiceName, appName);
        }

        #endregion
    }
}
