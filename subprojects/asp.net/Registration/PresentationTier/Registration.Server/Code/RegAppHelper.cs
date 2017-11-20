using System.Collections.Generic;
using System.Text.RegularExpressions;
using CambridgeSoft.COE.Framework.Common;
using PerkinElmer.COE.Registration.Server.Models;

namespace PerkinElmer.COE.Registration.Server.Code
{
    public static class RegAppHelper
    {
        public static List<SettingData> RetrieveSettings()
        {
            var settingList = new List<SettingData>();
            var currentApplicationName = RegUtilities.GetApplicationName();
            var appConfigSettings = FrameworkUtils.GetAppConfigSettings(currentApplicationName, true);
            var groups = appConfigSettings.SettingsGroup;
            foreach (var group in groups)
            {
                var settings = group.Settings;
                foreach (var setting in settings)
                {
                    if (!(setting.IsHidden.ToLower().Equals(bool.TrueString.ToLower())))
                    {
                        if (setting.IsHidden.ToLower().Equals(bool.TrueString.ToLower()))
                            continue;

                        if ((setting.IsAdmin.Equals(bool.TrueString) && CambridgeSoft.COE.Framework.COEConfigurationService.ConfigurationUtilities.GetManageConfigurationSettings())
                        || !setting.IsAdmin.Equals(bool.TrueString))
                        {
                            settingList.Add(new SettingData(group, setting));
                        }
                    }  
                }
            }
            return settingList;
        }

        public static string ExtractHtmlInnerText(string htmlText)
        {
            if (string.IsNullOrEmpty(htmlText))
                return string.Empty;

            // Match any Html tag (opening or closing tags) 
            // followed by any successive whitespaces
            // consider the Html text as a single line
            Regex regex = new Regex("(<.*?>\\s*)+", RegexOptions.Singleline);
            // replace all html tags (and consequtive whitespaces) by spaces
            // trim the first and last space
            return regex.Replace(htmlText, " ").Trim();
        }
    }
}