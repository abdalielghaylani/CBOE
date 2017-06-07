using Newtonsoft.Json;
namespace PerkinElmer.COE.Registration.Server.Models
{
    /// <summary>
    /// The class for the CheckDuplicate data object
    /// </summary>
    public class CheckDuplicateData
    {
        [JsonConstructor]
        public CheckDuplicateData(string dataXML, string duplicateCheck)
        {
            DataXML = dataXML;
            DuplicateCheck = duplicateCheck;
        }

        /// <summary>
        /// Gets or sets the dataXML
        /// </summary>
        [JsonProperty(PropertyName = "dataXML")]
        public string DataXML { get; set; }

        /// <summary>
        /// Gets or sets the duplicateCheck
        /// </summary>
        [JsonProperty(PropertyName = "duplicateCheck")]
        public string DuplicateCheck { get; set; }
    }
}