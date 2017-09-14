using Newtonsoft.Json;

namespace PerkinElmer.COE.Registration.Server.Models
{
    /// <summary>
    /// The class for the DuplicateResolution data object
    /// </summary>
    public class DuplicateResolutionData
    {
        [JsonConstructor]
        public DuplicateResolutionData(string data, string duplicateCheck, string action = null)
        {
            Data = data;
            DuplicateCheckOption = duplicateCheck;
            Action = action;
        }

        /// <summary>
        /// Gets or sets the data XML in xml format
        /// </summary>
        [JsonProperty(PropertyName = "data")]
        public string Data { get; set; }

        /// <summary>
        /// Gets or sets the DuplicateCheckOption
        /// </summary>
        [JsonProperty(PropertyName = "duplicateCheckOption")]
        public string DuplicateCheckOption { get; set; }

        /// <summary>
        /// Gets or sets the duplicate resolution action ( CopiesAction or DuplicateMixture or DuplicateRecords )
        /// </summary>
        [JsonProperty(PropertyName = "action")]
        public string Action { get; set; }
    }
}