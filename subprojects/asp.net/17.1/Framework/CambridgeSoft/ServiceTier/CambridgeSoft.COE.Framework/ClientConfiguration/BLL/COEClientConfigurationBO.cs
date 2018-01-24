using System;
using System.Data;
using System.Data.SqlClient;
using Csla;
using Csla.Data;
using Csla.Validation;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.Properties;
using CambridgeSoft.COE.Framework.COELoggingService;
using System.Collections.Specialized;
using System.Xml;
using CambridgeSoft.COE.Framework.Common.Messaging;
using CambridgeSoft.COE.Framework.COEConfigurationService;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using System.Configuration;
using System.Reflection;

namespace CambridgeSoft.COE.Framework.COEConfigurationService
{
    [Serializable()]
    public class COEClientConfigurationBO : Csla.BusinessBase<COEClientConfigurationBO>
    {
        #region Member variables
        private string _sectionName;
        private ConfigurationSection _configurationSection;

        private string _filePath;

        [NonSerialized]
        IConfigurationSource _configurationSource;

        [NonSerialized]
        IConfigurationParameter configParameter = null;
       
        [NonSerialized]
        private string _serviceName = "COEConfiguration";

      
        
        [NonSerialized]
        static COELog _coeLog = COELog.GetSingleton("COEConfiguration");
        #endregion

        #region Properties
     

        public string SectionName
        {
            set
            {
                CanWriteProperty();
                if (value == null)
                    value = string.Empty;
                if (_sectionName != value.Trim())
                {
                    _sectionName = value.Trim();
                    PropertyHasChanged();
                }
                this.MarkOld();
                this.MarkDirty();
            }
            get
            {
                CanReadProperty();
                return _sectionName;
            }
        }

        public string FilePath
        {
            set
            {
                CanWriteProperty();
                if (value == null)
                    value = string.Empty;

                if (_filePath != value.Trim())
                {
                    _filePath = value.Trim();
                    PropertyHasChanged();
                }
            }
            get
            {
                CanReadProperty();
                return _filePath;
            }
        }

        public ConfigurationSection ConfigurationSection
        {
            get {
                CanReadProperty();
                return _configurationSection;
            }
            set { 
                CanWriteProperty();

                if (_configurationSection != value)
                {
                    _configurationSection = value;
                    PropertyHasChanged();
                }
            }
        }

     

        
        #endregion

        #region Constructors

        private COEClientConfigurationBO()
        { /* require use of factory method */ }
        #endregion

        #region Validation Rules

        private void AddCommonRules()
        {
            ValidationRules.AddRule(CommonRules.StringRequired, "SectionName");
            ValidationRules.AddRule(CommonRules.StringMaxLength, new CommonRules.MaxLengthRuleArgs("SectionName", 255));
        }

        protected override void AddBusinessRules()
        {
            AddCommonRules();
        }

        #endregion //Validation Rules

        #region BusinessMethods
        private IConfigurationSource GetConfigurationSource()
        {
           
            this.FilePath = GetFilePath();
            if (this.FilePath == string.Empty)
            {
                return new SystemConfigurationSource();
            }
            else
            {
                return new FileConfigurationSource(this.FilePath);
            }
            
        }

        private string GetFilePath(){
            
            IConfigurationSource source = new SystemConfigurationSource();
            COEClientConfigurationSettings configSettings = (COEClientConfigurationSettings)source.GetSection("coeClientConfiguration");
            if(configSettings != null)
                return configSettings.File;
            else
                return null;
        }
        
        #endregion

        #region Factory Methods

        public static COEClientConfigurationBO New(string sectionName)
        {
            if (!CanAddObject())
                throw new System.Security.SecurityException(Resources.UserNotAuthorizedForAddObject + " COEConfigurationBO");

            return DataPortal.Create<COEClientConfigurationBO>(new Criteria(sectionName));
        }



        public static COEClientConfigurationBO Get(string sectionName)
        {
            if (!CanGetObject())
                throw new System.Security.SecurityException(Resources.UserNotAuthorizedForViewObject + " COEConfigurationBO");
            return DataPortal.Fetch<COEClientConfigurationBO>(new Criteria(sectionName));
        }



        public static void Delete(string sectionName)
        {
            if (!CanDeleteObject())
                throw new System.Security.SecurityException(Resources.UserNotAuthorizedForDeleteObject + " COEConfigurationBO");
            DataPortal.Delete(new Criteria(sectionName));
        }

        public void Save(ConfigurationSection newConfigSection)
        {
            if (IsDeleted && !CanDeleteObject())
                throw new System.Security.SecurityException(Resources.UserNotAuthorizedForDeleteObject + " COEConfigurationBO");
            else if (IsNew && !CanAddObject())
                throw new System.Security.SecurityException(Resources.UserNotAuthorizedForAddObject + " COEConfigurationBO");
            else if (!CanEditObject())
                throw new System.Security.SecurityException(Resources.UserNotAuthorizedForEditObject + " COEConfigurationBO");

            this.Update(newConfigSection);
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

        #endregion //Factory Methods

        #region Data Access

        #region Criteria

        [Serializable()]
        private class Criteria
        {
            internal string _sectionName;
            public Criteria(string sectionName)
            {
                _sectionName = sectionName;
            }
           
           
        }
        #endregion //Criteria


        [RunLocal]
        private void DataPortal_Create(Criteria criteria)
        {
            SectionName = criteria._sectionName;
            ConfigurationSection = null;
        }
        [RunLocal]
        private void DataPortal_Fetch(Criteria criteria)
        {
            SectionName = criteria._sectionName;
            _configurationSource = this.GetConfigurationSource();
            ConfigurationSection = _configurationSource.GetSection(criteria._sectionName);
        }
        [RunLocal]
        protected override void DataPortal_Insert()
        {
            _configurationSource.Add(null, _sectionName, this._configurationSection);
        }
        [RunLocal]
        protected override void DataPortal_Update()
        {
           
        }
        [RunLocal]
        protected  void  Update(ConfigurationSection clone)
        {
          //here we need to use a workaround to avoid the error"Cannot add a ConfigurationSection that already belongs to the Configuration." The problem occurs after you all.remove follwed by .add
          //the source still has a handle on the section and will no let you readd it.  The issue showed up on google, there is the best
          //solution I found (that actually worked ) is to require configuration sections that we write should have clone method and the caller of .save must pass in the clone
            this._configurationSource.Add(new FileConfigurationParameter(_filePath), _sectionName, clone);
        }


        [RunLocal]
        protected override void DataPortal_DeleteSelf()
        {
            DataPortal_Delete(new Criteria(_sectionName));
        }

        [RunLocal]
        private void DataPortal_Delete(Criteria criteria)
        {
            _configurationSource.Remove(new FileConfigurationParameter(_filePath), criteria._sectionName);
        }

        #endregion //Data Access


        protected override object GetIdValue()
        {
            return _sectionName;
        }

        
    }
}
