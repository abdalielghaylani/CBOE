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
    /// The class for the response data object
    /// </summary>
    public partial class SettingData
    {
        [JsonConstructor]
        public SettingData(string groupName, string name, string controlType, string value, string description, string picklistDatabaseName, string allowedValues, string processorClass, bool? isHidden)
        {
            GroupName = groupName;
            Name = name;
            ControlType = controlType;
            Value = value;
            Description = description;
            PicklistDatabaseName = picklistDatabaseName;
            AllowedValues = allowedValues;
            IsHidden = isHidden;
        }

        public SettingData(SettingsGroup group, AppSetting setting)
        {
            GroupName = group.Name;
            Name = setting.Name;
            ControlType = setting.ControlType;
            Value = setting.Value;
            Description = setting.Description;
            PicklistDatabaseName = setting.PicklistDatabaseName;
            AllowedValues = setting.AllowedValues;
            ProcessorClass = setting.ProcessorClass;
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
        [JsonProperty(PropertyName = "isHidden")]
        public bool? IsHidden { get; set; }

        /// <summary>
        /// Returns the JSON string presentation of the object
        /// </summary>
        /// <returns>JSON string presentation of the object</returns>
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Newtonsoft.Json.Formatting.Indented);
        }

    }
}
