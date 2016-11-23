using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Resources;
using System.Text;
using System.Web;

using Csla;

using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.COEConfigurationService;
using CambridgeSoft.COE.Framework.COEPageControlSettingsService;
using CambridgeSoft.COE.Framework.GUIShell;
using CambridgeSoft.COE.Framework.ExceptionHandling;

using CambridgeSoft.COE.Registration;
using CambridgeSoft.COE.RegistrationAdmin.Services.Properties;

namespace CambridgeSoft.COE.RegistrationAdmin.Services
{
    class RLSConfigurationProcessor : CambridgeSoft.COE.Registration.RegAdminBusinessBase<RLSConfigurationProcessor>, ICOEConfigurationProcessor
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

        #endregion

        #region ICOEConfigurationProcessor Members

        /// <summary>
        /// ICOEConfiguration Handler that updates COEFrameworkConfig.xml to enable/disable Table Editor's People Project child table 
        /// according the currentValue of the configuration setting
        /// </summary>
        /// <param name="settingName"></param>
        /// <param name="previousValue"></param>
        /// <param name="currentValue"></param>
        [COEUserActionDescription("UpdatePeopleProjectTableVisibilty")]
        public void Process(string settingName, string previousValue, string currentValue)
        {
            try
            {
                if (currentValue.ToLower() == Resources.RegistryLevelProjectsValue.ToLower() || currentValue.ToLower() == Resources.BatchLevelProjectsValue.ToLower())
                {
                    this.RegDal.ChangeRLS(currentValue);
                    // Modify COEFrameworkConfig.xml to make Table Editor's People Project child table visible
                    UpdatePeopleProjectTableVisibilty(false);
                    if (currentValue.ToLower() == Resources.RegistryLevelProjectsValue.ToLower())
                        UpdateProjectAndBatchSettings(true, false);
                    else if (currentValue.ToLower() == Resources.BatchLevelProjectsValue.ToLower())
                        UpdateProjectAndBatchSettings(false, true);
                }
                else
                {
                    this.RegDal.ChangeRLS(Resources.DisableRLS);
                    // Modify COEFrameworkConfig.xml to make Table Editor's People Project child table invisible
                    UpdatePeopleProjectTableVisibilty(true);
                }

            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
            }
        }

        #endregion

        #region Methods
        /// <summary>
        /// Method that updates COEFrameworkConfig.xml to enable/disable Table Editor's People Project child table visibilty
        /// </summary>
        /// <param name="isDisabled">disable/enable Project's People table</param>
        private void UpdatePeopleProjectTableVisibilty(bool isDisabled)
        {
            string COEConfigPath = COEConfigurationManager.GetDefaultConfigurationFilePath();

            System.Xml.XmlDocument xmldoc = new System.Xml.XmlDocument();
            xmldoc.Load(COEConfigPath);
            System.Xml.XmlNode projectTableNode = xmldoc.SelectSingleNode(PROJECT_TABLE_XPATH);
            projectTableNode.Attributes[DISABLE_CHILD_TABLE_ATTRIBUTE].Value = isDisabled.ToString();
            System.Xml.XmlNode peopleTableNode = xmldoc.SelectSingleNode(PEOPLE_TABLE_XPATH);
            peopleTableNode.Attributes[DISABLE_CHILD_TABLE_ATTRIBUTE].Value = isDisabled.ToString();
            peopleTableNode.Attributes[DISABLE_TABLE_ATTRIBUTE].Value = isDisabled.ToString();
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

        /// <summary>
        /// Method that updates COEDB.COEConfiguration table to enable/disable Project and Batch controls visibility
        /// </summary>
        /// <param name="projectsUsed">value to set to ProjectsUsed setting</param>
        /// <param name="batchProjectsUsed">value to set to BatchProjectsUsed setting</param>
        private void UpdateProjectAndBatchSettings(bool projectsUsed, bool batchProjectsUsed)
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

        #endregion

    }
}
