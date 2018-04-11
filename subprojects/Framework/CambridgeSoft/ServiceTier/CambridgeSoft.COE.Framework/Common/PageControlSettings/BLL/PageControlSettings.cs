using System;
using System.Collections.Generic;
using System.Text;
using Csla;
using CambridgeSoft.COE.Framework.COEPageControlSettingsService;
using System.Xml.XPath;
using CambridgeSoft.COE.Framework.COELoggingService;
using CambridgeSoft.COE.Framework.COEConfigurationService;
using System.IO;
using Infragistics.WebUI.UltraWebNavigator;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.COESecurityService;
using CambridgeSoft.COE.Framework.Properties;

namespace CambridgeSoft.COE.Framework.COEPageControlSettingsService
{
    [Serializable()]
    public class COEPageControlSettings : BusinessBase<COEPageControlSettings>
    {
        #region Variables

        [NonSerialized, NotUndoable]
        private DAL _coeDAL = null;
        [NonSerialized, NotUndoable]
        private DALFactory _dalFactory = new DALFactory();
        private string _serviceName = "COEPageControlSettings";
        
        private int _id = 0;
        private string _application = string.Empty;

        private PageList _customPageList = null;
        private PageList _masterPageList = null;
        private PageList _pageList = null;
        private PrivilegeList _fullListOfPrivileges = null;
        private AppSettingList _fullListOfAppSettings = null;
        private ControlList _cleanList = null;

        private string _xmlCustom = string.Empty;
        private string _xmlMaster = string.Empty;
        private string _xmlPrivileges = string.Empty;
       
        public enum Type
        {
            Master = 0,
            Custom = 1,
            Privileges = 2
        }

        #endregion

        #region Properties

        /// <summary>
        /// Identifier of the class
        /// </summary>
        [System.ComponentModel.DataObjectField(true, true)]
        public int ID
        {
            get
            {
                CanReadProperty(true);
                return _id;
            }
        }

        /// <summary>
        /// Full list of privileges.
        /// </summary>
        public PrivilegeList FullListOfPrivileges
        {
            get
            {
                CanReadProperty(true);
                return _fullListOfPrivileges;
            }
        }

        /// <summary>
        /// Full list of AppSettings (HardCoded until defined)
        /// </summary>
        public AppSettingList FullListOfAppSettings
        {
            get
            {
                CanReadProperty(true);
                return _fullListOfAppSettings;
            }
        }

        /// <summary>
        /// Entire list of pages.
        /// </summary>
        public PageList PageList
        {
            get { return _pageList; }
        }

        #endregion

        #region Criterias

        [Serializable()]
        private class Criteria
        {
            private string _appName;
            
            public string AppName
            {
                get { return _appName; }
            }

            public Criteria(string appName)
            {
                _appName = appName;
            }
        }

        [Serializable()]
        private class ControlSettingsCriteria
        {
            private string _appName;

            public string AppName
            {
                get { return _appName; }
            }

            public ControlSettingsCriteria(string appName)
            {
                _appName = appName;
            }
        }

        #endregion

        #region Methods

        #region Factory Methods

        /// <summary>
        /// Returns an object with all the settings for the given application
        /// </summary>
        /// <param name="appName"></param>
        /// <returns></returns>
        public static COEPageControlSettings GetSettings(string appName)
        {
            return !string.IsNullOrEmpty(appName) ? DataPortal.Fetch<COEPageControlSettings>(new Criteria(appName)) : null;
        }

        /// <summary>
        /// Returns an object with all the settings for the given application
        /// </summary>
        /// <param name="appName"></param>
        /// <returns></returns>
        public static COEPageControlSettingsService.ControlList GetControlListToDisableForCurrentUser(string appName)
        {
            return DataPortal.Fetch<COEPageControlSettings>(new ControlSettingsCriteria(appName))._cleanList;
        }

        #endregion

        #region Business Methods

        /// <summary>
        /// Update the configuration settings into the DB.
        /// </summary>
        /// <param name="appName"></param>
        public void UpdateSettings(string appName)
        {
            this._application = appName;
            DataPortal.Update(this);
        }

        /// <summary>
        /// Returns all the controls to disable for a given page/application.
        /// </summary>
        /// <param name="pageName"></param>
        /// <returns></returns>
        private ControlList FilterByPageAndUser(string pageName)
        {
            return this.FindOutControlsToDisable(_pageList.GetByID(pageName));
        }

        /// <summary>
        /// Returns the list of controls to disable given the current user and application.
        /// </summary>
        /// <returns></returns>
        private ControlList FilterByAppAndUser()
        {
            return this.FindOutControlsToDisable(_customPageList);
        }

        /// <summary>
        /// eturns the list of controls to disable given the current user and a page.
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        private ControlList FindOutControlsToDisable(Page page)
        {
            ControlList retVal = ControlList.NewControlList();
            bool matchedAppSettings = true;
            foreach (ControlSetting controlSetting in page.CtrlSettingList)
            {
                if (!SettingReaderUtilities.MatchedAppSettings(controlSetting.AppSettingList, controlSetting.AppSettingList.OperatorToApply) || !COEPrincipal.HasPrivilege(controlSetting.PrivilegeList, controlSetting.PrivilegeList.OperatorToApply))
                    retVal.AddControls(controlSetting.CtrlList);
            }
            return retVal;
        }

        /// <summary>
        /// Returns the list of controls to disable given the current user and a list of pages.
        /// </summary>
        /// <param name="pageList">Application Pages</param>
        /// <returns></returns>
        private ControlList FindOutControlsToDisable(PageList pageList)
        {
            if (pageList.Count > 0)
            {
                ControlList retVal = ControlList.NewControlList();
                foreach (Page page in pageList)
                    retVal.AddControls(this.FindOutControlsToDisable(page));
                return retVal;
            }
            else return null;
        }

        /// <summary>
        /// Method to check if the AppSettings variable force the control to be added
        /// </summary>
        /// <param name="appSettings">The list of appSettings to check</param>
        /// <param name="logOperator">The Logical operator to apply</param>
        /// <returns></returns>
        public static bool MatchAppSettings(COEPageControlSettingsService.AppSettingList appSettings, COEPageControlSettingsService.AppSettingList.Operators logOperator)
        {
            bool retVal = logOperator == CambridgeSoft.COE.Framework.COEPageControlSettingsService.AppSettingList.Operators.OR ? false : true;
            foreach (COEPageControlSettingsService.AppSetting appSetting in appSettings)
            {
                // Get the value from the corresponding class that implements IReader
                bool isSettingSet = SettingReaderUtilities.IsSettingValueSet(appSetting.Key, appSetting.Value, appSetting.Type);
                switch (logOperator)
                {
                    case CambridgeSoft.COE.Framework.COEPageControlSettingsService.AppSettingList.Operators.AND:
                        retVal &= isSettingSet; break;
                    case CambridgeSoft.COE.Framework.COEPageControlSettingsService.AppSettingList.Operators.OR:
                        retVal |= isSettingSet; break;
                    default:
                        retVal |= isSettingSet; break;
                }
            }
            return retVal;
        }

        #endregion

        #region Filtering class

        //private class Filters
        //{
        //    public static bool GetByPageAndUser(object item, object filter1, object filter2)
        //    {
        //        //int id = 0;
        //        //FieldBO field = null;
        //        ////Filter is a int
        //        //if (filter is int)
        //        //    id = Convert.ToInt32((string)filter.ToString());
        //        ////assume item is the object it self. (See string.empty parameter)
        //        //if (item is FieldBO)
        //        //    field = ((FieldBO)item);
        //        //if (field.ID == id)
        //        //    return true;
        //        return false;
        //    }

        //}
        #endregion

        #region Data Access Methods

        /// <summary>
        /// Get all info(e.g. pages, controls, privileges etc) to initialize BO. 
        /// </summary>
        /// <param name="criteria"></param>
        private void DataPortal_Fetch(Criteria criteria)
        {
            if (_coeDAL == null)
                LoadDAL();

            // Coverity Fix CID - 11469
            if (_coeDAL != null)
            {
                //Get the full privileges for given application.
                _xmlPrivileges = _coeDAL.GetConfigurationXMLFromPrivsPage(criteria.AppName, Type.Privileges);
                _fullListOfPrivileges = PrivilegeList.NewPrivilegeList(_xmlPrivileges, string.Empty);

                //Get the Master Page control settings for the given Application
                _xmlMaster = _coeDAL.GetConfigurationXML(criteria.AppName, Type.Master);
                this.InitializeFromXML(_xmlMaster, ref _masterPageList, Type.Master);
                //Get the customized page control settings for a given App
                _xmlCustom = _coeDAL.GetConfigurationXML(criteria.AppName, Type.Custom);
                if (!string.IsNullOrEmpty(_xmlCustom))
                {
                    this.InitializeFromXML(_xmlCustom, ref _customPageList, Type.Custom);
                    //we have to merge the master and custom into the _pageList.
                    //So now we have a valid _pageList for a given App that contains all the detailed info needed to be shown in the GUI.
                    this.Merge();
                }
                _pageList = _masterPageList;
            }
            else            
                throw new System.Security.SecurityException(string.Format(Resources.Culture,Resources.NullObjectError, "DAL"));            
        }

        /// <summary>
        /// Get all info(e.g. pages, controls, privileges etc) to initialize BO. 
        /// </summary>
        /// <param name="criteria"></param>
        private void DataPortal_Fetch(ControlSettingsCriteria criteria)
        {
            if (_coeDAL == null)
                LoadDAL();

            // Coverity Fix CID - 11468 
            if (_coeDAL != null)
            {
                //Get the full privileges for given application.
                _xmlPrivileges = _coeDAL.GetConfigurationXMLFromPrivsPage(criteria.AppName, Type.Privileges);
                _fullListOfPrivileges = PrivilegeList.NewPrivilegeList(_xmlPrivileges, string.Empty);
                // Setting the current appname being processed.
                ControlIdChangeUtility.SelectedApp = criteria.AppName;
                //Get the Master Page control settings for the given Application
                _xmlMaster = _coeDAL.GetConfigurationXML(criteria.AppName, Type.Master);
                this.InitializeFromXML(_xmlMaster, ref _masterPageList, Type.Master);
                //Get the customized page control settings for a given App

                _xmlCustom = _coeDAL.GetConfigurationXML(criteria.AppName, Type.Custom);
                if (!string.IsNullOrEmpty(_xmlCustom))
                {
                    this.InitializeFromXML(_xmlCustom, ref _customPageList, Type.Custom);
                    //we have to merge the master and custom into the _pageList.
                    //So now we have a valid _pageList for a given App that contains all the detailed info needed to be shown in the GUI.
                    this.Merge();
                }

                _cleanList = this.FilterByAppAndUser();
            }
            else            
                throw new System.Security.SecurityException(string.Format(Resources.Culture,Resources.NullObjectError, "DAL"));            
        }

        
        /// <summary>
        /// Update the configuration settings into the DB.
        /// </summary>
        protected override void DataPortal_Update()
        {
            if (_coeDAL == null)
                LoadDAL();

            // Coverity Fix CID - 11470
            if (_coeDAL != null)
            {
                this.UpdateXML(Type.Custom);
                _coeDAL.UpdateConfigurationXML(this._application, Type.Custom, this._xmlCustom);
            }
            else            
                throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));            
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Load DAL
        /// </summary>
        /// <remarks>See that the connection is through the COEDB user as all the coepagesettings are stored in this schema</remarks>
        private void LoadDAL()
        {
            if (_dalFactory == null) { _dalFactory = new DALFactory(); }
            _dalFactory.GetDAL<DAL>(ref _coeDAL, _serviceName, CambridgeSoft.COE.Framework.Properties.Resources.CentralizedStorageDB, true);
        }

        /// <summary>
        /// Read data from xml to object.
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="pageList"></param>
        /// <param name="type"></param>
        private void InitializeFromXML(string xml, ref PageList pageList,Type type)
        {
            //Setting the type processing i:e Master or Custom;
            ControlIdChangeUtility.SelectedType = type;
            XPathDocument xDocument = new XPathDocument(new StringReader(xml));
            XPathNavigator xNavigator = xDocument.CreateNavigator();
            XPathNodeIterator xIterator = xNavigator.Select("COEPageControlSettings/Pages");
            if (xIterator.MoveNext())
                pageList = PageList.NewPageList(xIterator.Current.OuterXml, type);

            xIterator = xNavigator.Select("COEPageControlSettings/Application");
            if (xIterator.MoveNext())
                if (!string.IsNullOrEmpty(xIterator.Current.Value))
                    _application = xIterator.Current.Value;

            xIterator = xNavigator.Select("COEPageControlSettings/AppSettings");
            if (xIterator.MoveNext())
                _fullListOfAppSettings = AppSettingList.NewAppSettingList(xIterator.Current.OuterXml, String.Empty);

        }

        /// <summary>
        /// Build data into custom.xml.
        /// </summary>
        /// <param name="type"></param>
        private void UpdateXML(COEPageControlSettings.Type type)
        {
            StringBuilder builder = new StringBuilder("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            builder.Append("<COEPageControlSettings>");
            builder.Append("<Type>" + type.ToString() + "</Type>");
            builder.Append("<Application>" + this._application + "</Application>");
            builder.Append(this._masterPageList.UpdateSelf(type));
            builder.Append("</COEPageControlSettings>");
            this._xmlCustom = builder.ToString();
        }

        /// <summary>
        /// Merge both pagelist so we can take the information from Master (e.g name control, etc) and have a PageList with all its fields fullfiled.
        /// </summary>
        /// <param name="masterPageList">Master PageControl settings (no privileges associated)</param>
        /// <param name="pageList">Template of the settings for a given app</param>
        /// <remarks>We do this because we don't want to save names, descriptions, etc of the pagesettings that might be changed in the Master
        /// For instance: if the description text of the Save button in Reg is changed, it has to be changed just in the master... no need to update 
        /// the customized pageControl settings for Reg.
        //</remarks>
        private void Merge()
        {
            foreach (Page masterPage in _masterPageList)
            {
                Page customPage = _customPageList.GetByID(masterPage.ID);
                if (customPage != null)
                {
                    masterPage.CtrlSettingList = customPage.CtrlSettingList;
                }
            }
        }

        #endregion

        #region Tree

        /// <summary>
        /// Returns a UltraWebTree object with all pages and privilegeList to show the tree view.
        /// </summary>
        /// <returns></returns>
        public UltraWebTree CreateTree()
        {
            UltraWebTree tree = new UltraWebTree();
            Node root = new Node();
            root.Text = _application;
            root.Tag = this;
            foreach (Node node in _pageList.CreatePageNodes())
            {
                root.Nodes.Add(node);
            }
            tree.Nodes.Add(root);
            return tree;
        }
        #endregion


        protected override object GetIdValue()
        {
            return _id;
        }

        #endregion
    }
}
