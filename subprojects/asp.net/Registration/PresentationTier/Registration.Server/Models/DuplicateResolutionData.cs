using Newtonsoft.Json;

namespace PerkinElmer.COE.Registration.Server.Models
{
    /// <summary>
    /// The class for the DuplicateResolution data object
    /// </summary>
    public class DuplicateResolutionData
    {
        [JsonConstructor]
        public DuplicateResolutionData(string data, string duplicateCheck, bool copyAction = false, bool checkOtherMixtures = false, bool createCopies = false)
        {
            Data = data;
            DuplicateCheckOption = duplicateCheck;          
            CheckOtherMixtures = checkOtherMixtures;
            CopyAction = copyAction;
            CreateCopies = createCopies;
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
        /// Gets or sets a value indicating whether the CopiesAction should be applied
        /// </summary>
        [JsonProperty(PropertyName = "copyAction")]
        public bool CopyAction { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to check other mixtures affected
        /// </summary>
        [JsonProperty(PropertyName = "checkOtherMixtures")]
        public bool CheckOtherMixtures { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'CreateCopies' or Continue
        /// True if 'CreateCopies' option selected, false if 'Continue' option selected
        /// </summary>
        [JsonProperty(PropertyName = "createCopies")]
        public bool CreateCopies { get; set; }
    }
}