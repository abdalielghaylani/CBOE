using Newtonsoft.Json;

namespace PerkinElmer.COE.Registration.Server.Models
{
    /// <summary>
    /// The class for the XML form data object
    /// </summary>
    public partial class XmlFormData
    {
        [JsonConstructor]
        public XmlFormData(string name, string data)
        {
            Name = name;
            Data = data;
        }

        /// <summary>
        /// Gets or sets the form name
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the form data
        /// </summary>
        [JsonProperty(PropertyName = "data")]
        public string Data { get; set; }
    }
}
