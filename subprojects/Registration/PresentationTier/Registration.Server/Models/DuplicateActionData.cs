using Newtonsoft.Json;

namespace PerkinElmer.COE.Registration.Server.Models
{
    /// <summary>
    /// The class for the DuplicateActionData data object
    /// </summary>
    public class DuplicateActionData
    {
        [JsonConstructor]
        public DuplicateActionData(string data, string duplicateAction, string regNo)
        {
            Data = data;
            DuplicateAction = duplicateAction;
            RegNo = regNo;
        }

        /// <summary>
        /// Gets or sets the data XML in xml format
        /// </summary>
        [JsonProperty(PropertyName = "data")]
        public string Data { get; set; }

        /// <summary>
        /// Gets or sets the DuplicateAction
        /// </summary>
        [JsonProperty(PropertyName = "duplicateAction")]
        public string DuplicateAction { get; set; }

        /// <summary>
        /// Gets or sets the Registration Number
        /// </summary>
        [JsonProperty(PropertyName = "regNo")]
        public string RegNo { get; set; }
    }
}