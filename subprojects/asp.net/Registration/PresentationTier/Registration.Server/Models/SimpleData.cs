using Newtonsoft.Json;

namespace PerkinElmer.COE.Registration.Server.Models
{
    /// <summary>
    /// The class for the simple data object
    /// </summary>
    public partial class SimpleData
    {
        [JsonConstructor]
        public SimpleData(string data)
        {
            Data = data;
        }

        /// <summary>
        /// Gets or sets the data
        /// </summary>
        [JsonProperty(PropertyName = "data")]
        public string Data { get; set; }
    }
}
