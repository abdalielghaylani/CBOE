using System.Collections.Generic;
using Newtonsoft.Json;

namespace PerkinElmer.COE.Registration.Server.Models
{
    /// <summary>
    /// The class for the BulkRegistrationData
    /// </summary>
    public class BulkRegistrationData
    {
        [JsonConstructor]
        public BulkRegistrationData(string duplicateAction, List<string> records, string description = null)
        {
            DuplicateAction = duplicateAction;
            Records = records;
            Description = description;
        }

        /// <summary>
        /// Gets or sets the DuplicateAction
        /// </summary>
        [JsonProperty(PropertyName = "duplicateAction")]
        public string DuplicateAction { get; set; }

        /// <summary>
        /// Gets or sets the Records
        /// </summary>
        [JsonProperty(PropertyName = "records")]
        public List<string> Records { get; set; }

        /// <summary>
        /// Gets or sets the Records
        /// </summary>
        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }
    }
}