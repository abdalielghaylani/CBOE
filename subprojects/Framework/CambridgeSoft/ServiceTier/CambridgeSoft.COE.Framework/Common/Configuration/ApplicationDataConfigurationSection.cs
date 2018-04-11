using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using System.Configuration;

namespace CambridgeSoft.COE.Framework.Common
{
    public class ApplicationDataConfigurationSection : COESerializableConfigurationSection
    {
        private const string _appSettingsProperty = "applicationSettings";

        [ConfigurationProperty(_appSettingsProperty, IsRequired = false)]
        public AppSettingsData AppSettings
        {
            get { return (AppSettingsData) base[_appSettingsProperty]; }
            set { base[_appSettingsProperty] = value; }
        }

        public ApplicationDataConfigurationSection Merge(ApplicationDataConfigurationSection section)
        {
            foreach(SettingsGroup settingGroup in section.AppSettings.SettingsGroup)
            {
                if(this.AppSettings.SettingsGroup.Contains(settingGroup.Name))
                {
                    this.AppSettings.SettingsGroup.Get(settingGroup.Name).Description = settingGroup.Description;
                    this.AppSettings.SettingsGroup.Get(settingGroup.Name).Title = settingGroup.Title;
                    foreach(AppSetting item in settingGroup.Settings)
                    {
                        if(this.AppSettings.SettingsGroup.Get(settingGroup.Name).Settings.Contains(item.Name))
                        {
                            this.AppSettings.SettingsGroup.Get(settingGroup.Name).Settings.Get(item.Name).AllowedValues = item.AllowedValues;
                            this.AppSettings.SettingsGroup.Get(settingGroup.Name).Settings.Get(item.Name).ControlType = item.ControlType;
                            this.AppSettings.SettingsGroup.Get(settingGroup.Name).Settings.Get(item.Name).Description = item.Description;
                            this.AppSettings.SettingsGroup.Get(settingGroup.Name).Settings.Get(item.Name).IsAdmin = item.IsAdmin;
                            this.AppSettings.SettingsGroup.Get(settingGroup.Name).Settings.Get(item.Name).PicklistDatabaseName = item.PicklistDatabaseName;
                            this.AppSettings.SettingsGroup.Get(settingGroup.Name).Settings.Get(item.Name).PicklistType = item.PicklistType;
                            this.AppSettings.SettingsGroup.Get(settingGroup.Name).Settings.Get(item.Name).ProcessorClass = item.ProcessorClass;
                            this.AppSettings.SettingsGroup.Get(settingGroup.Name).Settings.Get(item.Name).Value = item.Value;
                        }
                        else
                            this.AppSettings.SettingsGroup.Get(settingGroup.Name).Settings.Add(item);
                    }
                }
                else
                    this.AppSettings.SettingsGroup.Add(settingGroup);
            }
            return this;
        }
    }
}
