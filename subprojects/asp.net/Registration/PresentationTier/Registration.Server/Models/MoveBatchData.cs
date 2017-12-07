using Newtonsoft.Json;

namespace PerkinElmer.COE.Registration.Server.Models
{
    /// <summary>
    /// The class for the MoveBatchData 
    /// </summary>
    public class MoveBatchData
    {
        [JsonConstructor]
        public MoveBatchData(string sourceRegNum, string targetRegNum)
        {
            SourceRegNum = sourceRegNum;
            TargetRegNum = targetRegNum;
        }

        /// <summary>
        /// Gets or sets the Target Registration Number
        /// </summary>
        [JsonProperty(PropertyName = "targetRegNum")]
        public string TargetRegNum { get; set; }

        /// <summary>
        /// Gets or sets the Source Registration Number
        /// </summary>
        [JsonProperty(PropertyName = "sourceRegNum")]
        public string SourceRegNum { get; set; }
    }
}