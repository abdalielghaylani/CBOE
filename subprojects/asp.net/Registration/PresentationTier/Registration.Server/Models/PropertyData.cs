using Newtonsoft.Json;
using System.Collections.Generic;

namespace PerkinElmer.COE.Registration.Server.Models
{
    public class PropertyData
    {
        public PropertyData()
        {
        }

        [JsonConstructor]
        public PropertyData(string name, string typeName, string typeLabel, string pickListDisplayValue, string pickListDomainId, string value,
            string defaultValue, string precision, int sortOrder, string subType, string friendlyName, List<ValidationRuleData> validationRules)
        {
            Name = name;
            TypeName = typeName;
            TypeLabel = typeLabel;
            PickListDisplayValue = pickListDisplayValue;
            PickListDomainId = pickListDomainId;
            value = Value;
            DefaultValue = defaultValue;
            Precision = precision;
            SortOrder = sortOrder;
            SubType = subType;
            FriendlyName = friendlyName;
            ValidationRules = validationRules;
        }

        /// <summary>
        /// Gets or sets the Name
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the Type
        /// </summary>
        [JsonProperty(PropertyName = "type")]
        public string TypeName { get; set; }

        /// <summary>
        /// Gets or sets the TypeLabel
        /// </summary>
        [JsonProperty(PropertyName = "typeLabel")]
        public string TypeLabel { get; set; }

        /// <summary>
        /// Gets or sets the PickListDisplayValue
        /// </summary>
        [JsonProperty(PropertyName = "pickListDisplayValue")]
        public string PickListDisplayValue { get; set; }

        /// <summary>
        /// Gets or sets the pickListDomainId
        /// </summary>
        [JsonProperty(PropertyName = "pickListDomainId")]
        public string PickListDomainId { get; set; }

        /// <summary>
        /// Gets or sets the Value 
        /// </summary>
        [JsonProperty(PropertyName = "value")]
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets the DefaultValue 
        /// </summary>
        [JsonProperty(PropertyName = "defaultValue")]
        public string DefaultValue { get; set; }

        /// <summary>
        /// Gets or sets the Precision
        /// </summary>
        [JsonProperty(PropertyName = "precision")]
        public string Precision { get; set; }

        /// <summary>
        /// Gets or sets the sortOrder 
        /// </summary>
        [JsonProperty(PropertyName = "sortOrder")]
        public int SortOrder { get; set; }

        /// <summary>
        /// Gets or sets the SubType
        /// </summary>
        [JsonProperty(PropertyName = "subType")]
        public string SubType { get; set; }

        /// <summary>
        /// Gets or sets the FriendlyName
        /// </summary>
        [JsonProperty(PropertyName = "friendlyName")]
        public string FriendlyName { get; set; }

        /// <summary>
        /// Gets or sets the Events 
        /// </summary>
        [JsonProperty(PropertyName = "validationRules")]
        public List<ValidationRuleData> ValidationRules { get; set; }
    }

    public class ValidationRuleData
    {
        public ValidationRuleData()
        {
        }

        [JsonConstructor]
        public ValidationRuleData(string name, string min, string max, int maxLength,
            string error, string defaultValue, List<ParameterData> parameters)
        {
            Name = name;
            Min = min;
            Max = max;
            MaxLength = maxLength;
            Error = error;
            DefaultValue = defaultValue;
            Parameters = parameters;
        }

        /// <summary>
        /// Gets or sets the Name
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the Min
        /// </summary>
        [JsonProperty(PropertyName = "min")]
        public string Min { get; set; }

        /// <summary>
        /// Gets or sets the Max
        /// </summary>
        [JsonProperty(PropertyName = "max")]
        public string Max { get; set; }

        /// <summary>
        /// Gets or sets the MaxLength
        /// </summary>
        [JsonProperty(PropertyName = "maxLength")]
        public int MaxLength { get; set; }

        /// <summary>
        /// Gets or sets the Error
        /// </summary>
        [JsonProperty(PropertyName = "error")]
        public string Error { get; set; }

        /// <summary>
        /// Gets or sets the Default Value
        /// </summary>
        [JsonProperty(PropertyName = "defaultValue")]
        public string DefaultValue { get; set; }

        /// <summary>
        /// Gets or sets the Parameters 
        /// </summary>
        [JsonProperty(PropertyName = "parameters")]
        public List<ParameterData> Parameters { get; set; }
    }

    public class ParameterData
    {
        public ParameterData()
        {
        }

        [JsonConstructor]
        public ParameterData(string name, string value)
        {
            Name = name;
            Value = value;
        }

        /// <summary>
        /// Gets or sets the Name
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the Value
        /// </summary>
        [JsonProperty(PropertyName = "value")]
        public string Value { get; set; }
    }
}