using System;
using System.Collections.Generic;
using System.Text;
using CambridgeSoft.COE.Framework.Common;

namespace CambridgeSoft.COE.Framework.COEPageControlSettingsService
{
    class COEConfigurationReader : ISettingReader
    {
        #region Variables
            private int _priority;
        #endregion

        #region Properties
        /// <summary>
        /// Priority of the reader
        /// </summary>
        public int Priority
        {
            get
            {
                return _priority;
            }
        }
        #endregion

        #region Constructors
        public COEConfigurationReader()
        { 
            // Probably this priority could be configured
            this._priority = 0;
        }

        #endregion

        #region ISettingReader Methods
        
        public string getData(string variableName)
        {
            string retVal = String.Empty;
            if (variableName!=null && variableName.Length>0)
            {
                // Variable Name format should be in the format {APPLICATION_NAME}/{GROUP_NAME}/{SETTING_NAME}
                string[] variableToFind = variableName.Split('/');
                if (variableToFind.Length==3)
                {
                    AppSettingsData appSettings = FrameworkUtils.GetAppConfigSettings(variableToFind[0], variableToFind[0]);
                    // Coverity Fix CID - 10491 (from local server)
                    if (appSettings != null)
                    {
                        foreach (SettingsGroup settingsGroup in appSettings.SettingsGroup)
                        {
                            if (settingsGroup.Name.ToUpper() == variableToFind[1].ToUpper())
                            {
                                foreach (CambridgeSoft.COE.Framework.Common.AppSetting setting in settingsGroup.Settings)
                                {
                                    if (setting.Name == variableToFind[2])
                                    {
                                        retVal = setting.Value;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return retVal;
        }

        #endregion
    }
}
