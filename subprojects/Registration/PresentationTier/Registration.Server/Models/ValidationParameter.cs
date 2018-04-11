using Newtonsoft.Json;

namespace PerkinElmer.COE.Registration.Server.Models
{
    public class ValidationParameter
    {
        public ValidationParameter()
        {
        }

        [JsonConstructor]
        public ValidationParameter(string name, string value)
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