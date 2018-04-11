using Newtonsoft.Json;

namespace PerkinElmer.COE.Registration.Server.Models
{
    /// <summary>
    /// The class for the simple data object
    /// </summary>
    public partial class QueryData
    {
        [JsonConstructor]
        public QueryData(bool temporary, string searchCriteria, bool highlightSubStructures)
        {
            Temporary = temporary;
            SearchCriteria = searchCriteria;
            HighlightSubStructures = highlightSubStructures;
        }
      
        public QueryData(bool temporary, string searchCriteria)
        {
            Temporary = temporary;
            SearchCriteria = searchCriteria;         
        }

        /// <summary>
        /// Gets or sets a value indicating whether it is for temporary records
        /// </summary>
        [JsonProperty(PropertyName = "temporary")]
        public bool Temporary { get; set; }

        /// <summary>
        /// Gets or sets the search criteria
        /// </summary>
        [JsonProperty(PropertyName = "searchCriteria")]
        public string SearchCriteria { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to Highlight SubStructures in the query result
        /// </summary>
        [JsonProperty(PropertyName = "highlightSubStructures")]
        public bool HighlightSubStructures { get; set; }
    }
}
