using System;
using System.Collections.Generic;
using System.Text;
using CambridgeSoft.COE.Framework.COEConfigurationService;
using CambridgeSoft.COE.Framework.Common;
using Csla;
using CambridgeSoft.COE.Framework.ExceptionHandling;
using System.Configuration;
using System.IO;
using CambridgeSoft.COE.Framework.COEPageControlSettingsService;
using CambridgeSoft.COE.Framework.GUIShell;
using System.Web;
using System.Xml;

namespace CambridgeSoft.COE.RLSConfigurationTool
{
    class RLSConfigurationHandler
    {
        #region Constants
        /// <summary>
        /// XPATH Expression to locate Table Editor's Project table in COEFrameworkConfig.xml
        /// </summary>
        const string PROJECT_TABLE_XPATH = "configuration/coeConfiguration/applications/add[@name='REGISTRATION']/tableEditor/add[@name='VW_PROJECT']";
        /// <summary>
        /// XPATH Expression to locate Table Editor's People table in COEFrameworkConfig.xml
        /// </summary>
        const string PEOPLE_TABLE_XPATH = "configuration/coeConfiguration/applications/add[@name='REGISTRATION']/tableEditor/add[@name='COEDB.PEOPLE']";

        /// <summary>
        /// Name of the xml attribute that shows/hides child tables for Table Editor in COEFrameworkConfig.xml
        /// </summary>
        const string DISABLE_CHILD_TABLE_ATTRIBUTE = "disableChildTables";

        /// <summary>
        /// Name of the xml attribute that shows/hides tables for Table Editor in COEFrameworkConfig.xml
        /// </summary>
        const string DISABLE_TABLE_ATTRIBUTE = "disableTable";

        /// <summary>
        /// Name of the xml attribute that sets the Privilege/Privileges required to Add rows on Table Editor in COEFrameworkConfig.xml
        /// </summary>
        const string ADD_PRIVILEGE_TABLE_ATTRIBUTE = "addPriv";

        /// <summary>
        /// Name of the xml attribute that sets the Privilege/Privileges required to Edit rows on Table Editor in COEFrameworkConfig.xml
        /// </summary>
        const string EDIT_PRIVILEGE_TABLE_ATTRIBUTE = "editPriv";

        /// <summary>
        /// Name of the xml attribute that sets the Privilege/Privileges required to Edit rows on Table Editor in COEFrameworkConfig.xml
        /// </summary>
        const string DELETE_PRIVILEGE_TABLE_ATTRIBUTE = "deletePriv";

        /// <summary>
        /// XPATH Expression to locate Table Editor's Project's table isPublic field in COEFrameworkConfig.xml
        /// </summary>
        const string ISPUBLIC_FIELD_XPATH = "configuration/coeConfiguration/applications/add[@name='REGISTRATION']/tableEditor/add[@name='VW_PROJECT']/tableEditorData/add[@name='ISPUBLIC']";

        /// <summary>
        /// XPATH Expression to locate Table Editor's Project's TableEditorData in COEFrameworkConfig.xml
        /// </summary>
        const string PROJECTTABLE_TYPES_XPATH = "configuration/coeConfiguration/applications/add[@name='REGISTRATION']/tableEditor/add[@name='VW_PROJECT']/tableEditorData/add[@name='TYPE']";

        /// <summary>
        /// XPATH Expression to locate innerXml PROJECTTYPES node in COEFrameworkConfig.xml
        /// </summary>
        const string REGISTRATION_PROJECTTYPES_XPATH = "configuration/coeConfiguration/applications/add[@name='REGISTRATION']/innerXml/add[@name='PROJECTTYPES']";
        
        #endregion

        #region Methods

        /// <summary>
        /// Utility method that changes the RLS status on the db and updates COEFrameworkConfig.xml to enable/disable 
        /// Table Editor's People Project child table according the currentValue of the ActiveRLS configuration setting
        /// </summary>
        /// <param name="currentValue"></param>
        /// <param name="oracleInstanceName"></param>
        /// <param name="oracleUser"></param>
        /// <param name="oraclePassword"></param>
        public static void ChangeRLSValue(string currentValue, string oracleInstanceName, string oracleUser, string oraclePassword)
        {
            try
            {
                if (!string.IsNullOrEmpty(currentValue))
                {
                    OracleDataAccessClientDAL DAL = new OracleDataAccessClientDAL();
                    if (currentValue.ToLower() == Resources.RegistryLevelProjectsValue.ToLower() || currentValue.ToLower() == Resources.BatchLevelProjectsValue.ToLower())
                    {
                        DAL.ChangeRLS(currentValue, oracleInstanceName, oracleUser, oraclePassword);
                        // Modify COEFrameworkConfig.xml to make Table Editor's People Project child table visible
                        UpdatePeopleProjectTableVisibilty(false);
                        if (currentValue.ToLower() == Resources.RegistryLevelProjectsValue.ToLower())
                        {
                            UpdateConfigurationSettings(true, false);
                            UpdateAvailableProyectTypes(RSLStatus.RegistryLevel);
                        }
                        else if (currentValue.ToLower() == Resources.BatchLevelProjectsValue.ToLower())
                        {
                            UpdateConfigurationSettings(false, true);
                            UpdateAvailableProyectTypes(RSLStatus.BatchLevel);
                        }
                    }
                    else
                    {
                        DAL.ChangeRLS(Resources.DisableRLS, oracleInstanceName, oracleUser, oraclePassword);
                        // Modify COEFrameworkConfig.xml to make Table Editor's People Project child table invisible
                        UpdatePeopleProjectTableVisibilty(true);
                        UpdateIsAdminSetting(false);
                        UpdateAvailableProyectTypes(RSLStatus.Off);
                    }
                }
                else
                {
                    throw new Exception("RLS Value not given");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        
        /// <summary>
        /// Method that updates COEFrameworkConfig.xml to enable/disable Table Editor's People Project child table visibilty
        /// </summary>
        /// <param name="isDisabled">disable/enable Project's People table</param>
        private static void UpdatePeopleProjectTableVisibilty(bool isDisabled)
        {
            string COEConfigPath = COEConfigurationManager.GetDefaultConfigurationFilePath();

            System.Xml.XmlDocument xmldoc = new System.Xml.XmlDocument();
            xmldoc.Load(COEConfigPath);
            System.Xml.XmlNode projectTableNode = xmldoc.SelectSingleNode(PROJECT_TABLE_XPATH);
            projectTableNode.Attributes[DISABLE_CHILD_TABLE_ATTRIBUTE].Value = isDisabled.ToString();
            projectTableNode.Attributes[DISABLE_CHILD_TABLE_ATTRIBUTE].Value = isDisabled.ToString();
            if (isDisabled)
            {
                projectTableNode.Attributes[EDIT_PRIVILEGE_TABLE_ATTRIBUTE].Value = Resources.EditPrivilegeRLSOff;
                projectTableNode.Attributes[ADD_PRIVILEGE_TABLE_ATTRIBUTE].Value = Resources.AddPrivilegeRLSOff;
                projectTableNode.Attributes[DELETE_PRIVILEGE_TABLE_ATTRIBUTE].Value = Resources.DeletePrivilegeRLSOff;
            }
            else
            {
                projectTableNode.Attributes[EDIT_PRIVILEGE_TABLE_ATTRIBUTE].Value = Resources.EditPrivilegeRLSActivated;
                projectTableNode.Attributes[ADD_PRIVILEGE_TABLE_ATTRIBUTE].Value = Resources.AddPrivilegeRLSActivated;
                projectTableNode.Attributes[DELETE_PRIVILEGE_TABLE_ATTRIBUTE].Value = Resources.DeletePrivilegeRLSActivated;
            }
            System.Xml.XmlNode peopleTableNode = xmldoc.SelectSingleNode(PEOPLE_TABLE_XPATH);
            peopleTableNode.Attributes[DISABLE_CHILD_TABLE_ATTRIBUTE].Value = isDisabled.ToString();
            peopleTableNode.Attributes[DISABLE_TABLE_ATTRIBUTE].Value = isDisabled.ToString();
            System.Xml.XmlNode isPublicFieldNode = xmldoc.SelectSingleNode(ISPUBLIC_FIELD_XPATH);
            if (!isDisabled)
            {
                if (isPublicFieldNode==null)
                {
                    XmlTextReader xmlReader = new XmlTextReader(new
StringReader(Resources.IsPublicXmlNodeContent));
                    XmlNode newIsPublicNode = xmldoc.ReadNode(xmlReader);
                    projectTableNode.FirstChild.AppendChild(newIsPublicNode);
                }
            }
            else
            {
                if (isPublicFieldNode != null)
                {
                    isPublicFieldNode.ParentNode.RemoveChild(isPublicFieldNode);
                }
            }
            try
            {
                File.SetAttributes(COEConfigPath, FileAttributes.Normal);
                xmldoc.Save(COEConfigPath);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static void UpdateAvailableProyectTypes(RSLStatus status)
        {
            string COEConfigPath = COEConfigurationManager.GetDefaultConfigurationFilePath();

            XmlDocument xmldoc = new System.Xml.XmlDocument();
            xmldoc.Load(COEConfigPath);
            XmlNode projectTabletTypesDataNode = xmldoc.SelectSingleNode(PROJECTTABLE_TYPES_XPATH);
            XmlNode projectsTypesNode = xmldoc.SelectSingleNode(REGISTRATION_PROJECTTYPES_XPATH);
            if (projectTabletTypesDataNode != null)
            {
                if (projectTabletTypesDataNode.Attributes["hidden"] == null)
                    projectTabletTypesDataNode.Attributes.Append(xmldoc.CreateAttribute("hidden"));

                switch (status)
                {
                    case RSLStatus.Off:
                        projectTabletTypesDataNode.Attributes["hidden"].Value = Boolean.FalseString.ToLower();

                        if (projectsTypesNode != null)                        
                            projectsTypesNode.InnerXml = @"<innerXmlData>
              <add name=""allValue"" value=""A"" display=""All"" />
              <add name=""batchValue"" value=""B"" display=""Batch"" />
              <add name=""registryValue"" value=""R"" display=""Registry"" />
            </innerXmlData>";
                        break;
                    case RSLStatus.RegistryLevel:
                        projectTabletTypesDataNode.Attributes["hidden"].Value = Boolean.TrueString.ToLower();                        
                        if (projectsTypesNode != null)                        
                            projectsTypesNode.InnerXml = @"<innerXmlData>                            
              <add name=""registryValue"" value=""R"" display=""Registry"" />              
              <add name=""allValue"" value=""A"" display=""All"" />
              <add name=""batchValue"" value=""B"" display=""Batch"" />
            </innerXmlData>";                        
                        break;
                    case RSLStatus.BatchLevel:
                        projectTabletTypesDataNode.Attributes["hidden"].Value = Boolean.TrueString.ToLower();
                        if (projectsTypesNode != null)
                            projectsTypesNode.InnerXml = @"<innerXmlData>                            
              <add name=""batchValue"" value=""B"" display=""Batch"" />
              <add name=""allValue"" value=""A"" display=""All"" />
              <add name=""registryValue"" value=""R"" display=""Registry"" />
            </innerXmlData>";
                        break;
                }
            }
            try
            {
                xmldoc.Save(COEConfigPath);
            }
            catch (Exception e) 
            {
                throw e;
            }
        }

        /// <summary>
        /// Method that updates COEDB.COEConfiguration table to update ActiveRLS and enable/disable Project and Batch controls visibility
        /// </summary>
        /// <param name="projectsUsed">value to set to ProjectsUsed setting</param>
        /// <param name="batchProjectsUsed">value to set to BatchProjectsUsed setting</param>
        private static void UpdateConfigurationSettings(bool projectsUsed, bool batchProjectsUsed)
        {
            AppSettingsData AppSettings = FrameworkUtils.GetAppConfigSettings(Resources.ApplicationName, true);
            foreach (SettingsGroup settingsGroup in AppSettings.SettingsGroup)
            {
                if (settingsGroup.Name == Resources.ProjectsAndBatchSettingsSection)
                {
                    foreach (CambridgeSoft.COE.Framework.Common.AppSetting appSetting in settingsGroup.Settings)
                    {
                        if (appSetting.Name == Resources.ProjectsUsedSettingName)
                        {
                            appSetting.Value = projectsUsed.ToString();
                            appSetting.IsAdmin = bool.TrueString;
                        }
                        else if (appSetting.Name == Resources.BatchLevelProjectsSettingName)
                        {
                            appSetting.Value = batchProjectsUsed.ToString();
                            appSetting.IsAdmin = bool.TrueString;
                        }
                    }
                }
            }
            FrameworkUtils.SaveAppConfigSettings(Resources.ApplicationName, AppSettings);
        }

        /// <summary>
        /// Method that updates COEDB.COEConfiguration table to update the IsAdmin attribute on ProjectsUsed and BatchProjectsUsed settings
        /// </summary>
        /// <param name="newValue">value to set to IsAdmin attribute</param>
        private static void UpdateIsAdminSetting(bool newValue)
        {
            AppSettingsData AppSettings = FrameworkUtils.GetAppConfigSettings(Resources.ApplicationName, true);
            foreach (SettingsGroup settingsGroup in AppSettings.SettingsGroup)
            {
                if (settingsGroup.Name == Resources.ProjectsAndBatchSettingsSection)
                {
                    foreach (CambridgeSoft.COE.Framework.Common.AppSetting appSetting in settingsGroup.Settings)
                    {
                        if (appSetting.Name == Resources.ProjectsUsedSettingName)
                        {
                            appSetting.IsAdmin = newValue.ToString();
                        }
                        else if (appSetting.Name == Resources.BatchLevelProjectsSettingName)
                        {
                            appSetting.IsAdmin = newValue.ToString();
                        }
                    }
                }
            }
            FrameworkUtils.SaveAppConfigSettings(Resources.ApplicationName, AppSettings);
        }

        #endregion

        #region Enums        
        
        private enum RSLStatus
        {
            Off,
            RegistryLevel,
            BatchLevel,
        }

        #endregion

    }
}
