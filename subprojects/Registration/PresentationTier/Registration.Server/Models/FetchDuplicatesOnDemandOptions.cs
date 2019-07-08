using Newtonsoft.Json;

namespace PerkinElmer.COE.Registration.Server.Models
{
    //FetchDuplicatesOnDemandOptions
    /// <summary>
    /// The class for the registry record data with fetch on demand duplicates options
    /// </summary>
    public class FetchDuplicatesOnDemandOptions
    {
        [JsonConstructor]
        public FetchDuplicatesOnDemandOptions(string data, int count, int skip, string sort, string sortOrder)
        {
            Data = data;
            Count = count;
            Skip = skip;
            Sort = sort;
            SortOrder = sortOrder;
        }

        /// <summary>
        /// Gets or sets the data in XML format
        /// </summary>
        [JsonProperty(PropertyName = "data")]
        public string Data { get; set; }        
       
        /// <summary>
        /// Gets or sets the count of duplicate records requested
        /// </summary>
        [JsonProperty(PropertyName = "count")]
        public int Count { get; set; }

        /// <summary>
        /// Gets or sets the skip count of duplicate records requested in a sorted record list
        /// </summary>
        [JsonProperty(PropertyName = "skip")]
        public int Skip { get; set; }

        /// <summary>
        /// Gets or sets the sort by record field name to sort the returning duplcates array
        /// </summary>
        [JsonProperty(PropertyName = "sort")]
        public string Sort { get; set; }

        /// <summary>
        /// Gets or sets the sort orer to sort the returning duplcates array
        /// </summary>
        [JsonProperty(PropertyName = "sortOrder")]
        public string SortOrder { get; set; }
    }
}