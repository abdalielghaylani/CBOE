using System.Collections.Generic;
using Newtonsoft.Json;

namespace PerkinElmer.COE.Registration.Server.Models
{
    public class ValidationRuleData
    {
        public ValidationRuleData()
        {
        }

        [JsonConstructor]
        public ValidationRuleData(string name, string min, string max, int maxLength, string error, string defaultValue, List<ValidationParameter> parameters)
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
        [JsonProperty(PropertyName = "parameters", NullValueHandling = NullValueHandling.Ignore)]
        public List<ValidationParameter> Parameters { get; set; }
    }
}