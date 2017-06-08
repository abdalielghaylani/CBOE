using Newtonsoft.Json;

namespace PerkinElmer.COE.Registration.Server.Models
{
    /// <summary>
    /// The class for the DuplicateResolution data object
    /// </summary>
    public class DuplicateResolutionData
    {
        [JsonConstructor]
        public DuplicateResolutionData(string dataXML, string duplicateCheck)
        {
            DataXML = dataXML;
            DuplicateCheckOption = duplicateCheck;
        }

        /// <summary>
        /// Gets or sets the dataXML
        /// </summary>
        [JsonProperty(PropertyName = "dataXML")]
        public string DataXML { get; set; }

        /// <summary>
        /// Gets or sets the DuplicateCheckOption
        /// </summary>
        [JsonProperty(PropertyName = "duplicateCheck")]
        public string DuplicateCheckOption { get; set; }
    }
}