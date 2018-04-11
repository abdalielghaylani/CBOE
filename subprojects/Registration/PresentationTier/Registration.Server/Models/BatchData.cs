using Newtonsoft.Json;

namespace PerkinElmer.COE.Registration.Server.Models
{
    /// <summary>
    /// The class for the batch data
    /// </summary>
    public class BatchData
    {
        [JsonConstructor]
        public BatchData(string data, string regNum)
        {
            Data = data;
            RegNum = regNum;
        }

        /// <summary>
        /// Gets or sets the data in XML format
        /// </summary>
        [JsonProperty(PropertyName = "data")]
        public string Data { get; set; }

        /// <summary>
        /// Gets or sets the Registration Number
        /// </summary>
        [JsonProperty(PropertyName = "regNum")]
        public string RegNum { get; set; }
    }
}