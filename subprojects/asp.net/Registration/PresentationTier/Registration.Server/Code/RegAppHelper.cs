using System.Collections.Generic;
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
                    settingList.Add(new SettingData(group, setting));
                }
            }
            return settingList;
        }
    }
}