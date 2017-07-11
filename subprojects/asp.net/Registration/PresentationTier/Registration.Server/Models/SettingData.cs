using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using CambridgeSoft.COE.Framework;
using CambridgeSoft.COE.Framework.Common;
using Csla;
using Newtonsoft.Json.Linq;

namespace PerkinElmer.COE.Registration.Server.Models
{
    /// <summary>
    /// The class for the setting data object
    /// </summary>
    public class SettingData
    {
        [JsonConstructor]
        public SettingData(string groupName, string groupLabel, string name, string controlType, string value, string description, string picklistDatabaseName, string allowedValues, string processorClass, bool? isAdmin, bool? isHidden)
        {
            GroupName = groupName;
            GroupLabel = groupLabel;
            Name = name;
            ControlType = controlType;
            Value = value;
            Description = description;
            PicklistDatabaseName = picklistDatabaseName;
            AllowedValues = allowedValues;
            IsAdmin = isAdmin;
            IsHidden = isHidden;
        }

        public SettingData(SettingsGroup group, AppSetting setting)
        {
            GroupName = group.Name;
            GroupLabel = GroupName.Equals("CBV") ? "Search" :
                GroupName.Equals("DUPLICATE_CHECKING") ? "Duplicate Checking" :
                GroupName.Equals("ENHANCED_DUPLICATE_SCAN") ? "Extended Duplicate Checking" :
                GroupName.Equals("INVENTORY") ? "Inventory" :
                GroupName.Equals("MISC") ? "Advanced" :
                "Registration";
            Name = setting.Name;
            ControlType = setting.ControlType;
            Value = setting.Value;
            Description = setting.Description;
            PicklistDatabaseName = setting.PicklistDatabaseName;
            AllowedValues = setting.AllowedValues;
            ProcessorClass = setting.ProcessorClass;
            var isAdminValue = setting.IsAdmin;
            bool isAdmin;
            if (!string.IsNullOrEmpty(isAdminValue) && bool.TryParse(isAdminValue, out isAdmin))
                IsAdmin = isAdmin;
            var isHiddenValue = setting.IsHidden;
            bool isHidden;
            if (!string.IsNullOrEmpty(isHiddenValue) && bool.TryParse(isHiddenValue, out isHidden))
                IsHidden = isHidden;
        }

        /// <summary>
        /// Gets or sets the setting group name
        /// </summary>
        [JsonProperty(PropertyName = "groupName")]
        public string GroupName { get; set; }

        /// <summary>
        /// Gets or sets the setting group label
        /// </summary>
        [JsonProperty(PropertyName = "groupLabel")]
        public string GroupLabel { get; set; }

        /// <summary>
        /// Gets or sets the setting name
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the control type
        /// </summary>
        [JsonProperty(PropertyName = "controlType")]
        public string ControlType { get; set; }

        /// <summary>
        /// Gets or sets the setting value
        /// </summary>
        [JsonProperty(PropertyName = "value")]
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets the setting description
        /// </summary>
        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the pick-list database name
        /// </summary>
        [JsonProperty(PropertyName = "picklistDatabaseName")]
        public string PicklistDatabaseName { get; set; }

        /// <summary>
        /// Gets or sets the allowed values
        /// </summary>
        [JsonProperty(PropertyName = "allowedValues")]
        public string AllowedValues { get; set; }

        /// <summary>
        /// Gets or sets the processor class
        /// </summary>
        [JsonProperty(PropertyName = "processorClass")]
        public string ProcessorClass { get; set; }

        /// <summary>
        /// Gets or sets the hidden flag
        /// </summary>
        [JsonProperty(PropertyName = "isAdmin")]
        public bool? IsAdmin { get; set; }

        /// <summary>
        /// Gets or sets the hidden flag
        /// </summary>
        [JsonProperty(PropertyName = "isHidden")]
        public bool? IsHidden { get; set; }
    }
}
