using System.Collections.Generic;
using Newtonsoft.Json;

namespace PerkinElmer.COE.Registration.Server.Models
{
    public class PropertyData
    {
        public PropertyData()
        {
        }

        [JsonConstructor]
        public PropertyData(
            string name,
            string typeName,
            string typeLabel,
            string pickListDisplayValue,
            string pickListDomainId,
            string value,
            string defaultValue,
            string precision,
            int sortOrder,
            string subType,
            string friendlyName,
            List<ValidationRuleData> validationRules)
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
}