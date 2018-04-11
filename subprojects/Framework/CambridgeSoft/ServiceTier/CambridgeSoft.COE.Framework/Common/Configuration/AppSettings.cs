using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using System.Configuration;
using System.ComponentModel;

namespace CambridgeSoft.COE.Framework.Common
{
    [Serializable]
    public class AppSettingsData : COENamedConfigurationElement
    {
        private const string groupsKey = "groups";
        
        [Browsable(true)]
        [ReadOnly(false)]
        [Description("List of groups")]
        [Category("Items")]
        [DisplayName("Groups")]
        [ConfigurationProperty(groupsKey, IsRequired = true)]
        public COENamedElementCollection<SettingsGroup> SettingsGroup
        {
            get { return (COENamedElementCollection<SettingsGroup>)base[groupsKey]; }
            set { base[groupsKey] = value; }
        }

       public AppSettingsData() {}
    }

    [Serializable]
    public class SettingsGroup : COENamedConfigurationElement
    {
        private const string _nameKey = "name";
        private const string _titleKey = "title";
        private const string _descriptionKey = "description";
        private const string _settingsKey = "settings";

        public SettingsGroup() { }

        /// <summary>
        /// unique GroupName
        /// </summary>
        /// 
        [Browsable(true)]
        [ReadOnly(false)]
        [Description("Name of the group")]
        [Category("SettingsGroup")]
        [DisplayName("Name")]
        [ConfigurationProperty(_nameKey, IsRequired = true)]
        public string Name
        {
            get { return (string)base[_nameKey]; }
            set { base[_nameKey] = value; }
        }

        [Browsable(true)]
        [ReadOnly(false)]
        [Description("Title of the group")]
        [Category("Groups")]
        [DisplayName("Title")]
        [ConfigurationProperty(_titleKey, IsRequired = false)]
        public string Title
        {
            get { return (string)base[_titleKey]; }
            set { base[_titleKey] = value; }
        }

        [Browsable(true)]
        [ReadOnly(false)]
        [Description("Description of the group")]
        [Category("Groups")]
        [DisplayName("Description")]
        [ConfigurationProperty(_descriptionKey, IsRequired = false)]
        public string Description
        {
            get { return (string)base[_descriptionKey]; }
            set { base[_descriptionKey] = value; }
        }

        [Browsable(true)]
        [ReadOnly(false)]
        [Description("Settings of the group")]
        [Category("Gropus")]
        [DisplayName("Settings")]
        [ConfigurationProperty(_settingsKey, IsRequired = false)]
        public COENamedElementCollection<AppSetting> Settings
        {
            get { return (COENamedElementCollection<AppSetting>)base[_settingsKey]; }
            set { base[_settingsKey] = value; }
        }
    }

    [Serializable]
    public class AppSetting : COENamedConfigurationElement
    {
        private const string _name = "name";
        private const string _value = "value";
        private const string _controlType = "controlType";
        private const string _description = "description";
        private const string _picklistDatabaseName = "picklistDatabaseName";
        private const string _picklistType = "picklistType";
        private const string _isAdmin = "isAdmin";
        private const string _allowedValues = "allowedValues";
        private const string _processorClass = "processorClass";
        private const string _isHidden = "isHidden";

        /// <summary>
        /// Name that uniquely identifies the configuration entry
        /// </summary>
        /// 
        [Browsable(true)]
        [ReadOnly(false)]
        [Description("Entry name")]
        [Category("Entry")]
        [DisplayName("Name")]
        [ConfigurationProperty(_name, IsRequired = true)]
        public string Name
        {
            get { return (string)base[_name]; }
            set { base[_name] = value; }
        }

        /// <summary>
        /// Value of the configuration entry
        /// </summary>
        /// 
        [Browsable(true)]
        [ReadOnly(false)]
        [Description("Entry value")]
        [Category("Entry")]
        [DisplayName("Value")]
        [ConfigurationProperty(_value, IsRequired = false)]
        public string Value
        {
            get { return (string)base[_value]; }
            set { base[_value] = value; }
        }

        /// <summary>
        /// Type of the configuration entry
        /// </summary>
        /// 
        [Browsable(true)]
        [ReadOnly(false)]
        [Description("Entry controlType")]
        [Category("Entry")]
        [DisplayName("ControlType")]
        [ConfigurationProperty(_controlType, IsRequired = false)]
        public string ControlType
        {
            get { return (string)base[_controlType]; }
            set { base[_controlType] = value; }
        }

        /// <summary>
        /// Value of the configuration entry
        /// </summary>
        /// 
        [Browsable(true)]
        [ReadOnly(false)]
        [Description("Entry description")]
        [Category("Entry")]
        [DisplayName("Description")]
        [ConfigurationProperty(_description, IsRequired = false)]
        public string Description
        {
            get { return (string)base[_description]; }
            set { base[_description] = value; }
        }

        /// <summary>
        /// Value of the configuration entry
        /// </summary>
        /// 
        [Browsable(true)]
        [ReadOnly(false)]
        [Description("Entry allowedValues")]
        [Category("Entry")]
        [DisplayName("AllowedValues")]
        [ConfigurationProperty(_allowedValues, IsRequired = false)]
        public string AllowedValues
        {
            get { return (string)base[_allowedValues]; }
            set { base[_allowedValues] = value; }
        }

        /// <summary>
        /// DatabaseName for Picklist entries
        /// </summary>
        /// 
        [Browsable(true)]
        [ReadOnly(false)]
        [Description("Entry picklistDatabaseName")]
        [Category("Entry")]
        [DisplayName("PicklistDatabaseType")]
        [ConfigurationProperty(_picklistDatabaseName, IsRequired = false)]
        public string PicklistDatabaseName
        {
            get { return (string)base[_picklistDatabaseName]; }
            set { base[_picklistDatabaseName] = value; }
        }

        /// <summary>
        /// Picklist type for Picklist entries
        /// </summary>
        /// 
        [Browsable(true)]
        [ReadOnly(false)]
        [Description("Entry picklistType")]
        [Category("Entry")]
        [DisplayName("PicklistType")]
        [ConfigurationProperty(_picklistType, IsRequired = false)]
        public string PicklistType
        {
            get { return (string)base[_picklistType]; }
            set { base[_picklistType] = value; }
        }

        /// <summary>
        /// IsAdmin flag 
        /// </summary>
        /// 
        [Browsable(true)]
        [ReadOnly(false)]
        [Description("Entry isAdmin")]
        [Category("Entry")]
        [DisplayName("IsAdmin")]
        [ConfigurationProperty(_isAdmin, IsRequired = false)]
        public string IsAdmin
        {
            get { return (string)base[_isAdmin]; }
            set { base[_isAdmin] = value; }
        }

        /// <summary>
        /// Processor class name of the configuration entry
        /// </summary>
        /// 
        [Browsable(true)]
        [ReadOnly(false)]
        [Description("Entry processorClass")]
        [Category("Entry")]
        [DisplayName("ProcessorClass")]
        [ConfigurationProperty(_processorClass, IsRequired = false)]
        public string ProcessorClass
        {
            get { return (string)base[_processorClass]; }
            set { base[_processorClass] = value; }
        }

        /// <summary>
        /// Hidden flag
        /// </summary>
        /// 
        [Browsable(true)]
        [ReadOnly(false)]
        [Description("Entry isHidden")]
        [Category("Entry")]
        [DisplayName("IsHidden")]
        [ConfigurationProperty(_isHidden, IsRequired = false)]
        public string IsHidden
        {
            get { return (string)base[_isHidden]; }
            set { base[_isHidden] = value; }
        }

    }

    /// <summary>
    /// Valid values for the AppSetting type field
    /// </summary>
    public enum AppSettingType
    {
        TEXT,
        PICKLIST
    }

}