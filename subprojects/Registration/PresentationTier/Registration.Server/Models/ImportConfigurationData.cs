using Newtonsoft.Json;

namespace PerkinElmer.COE.Registration.Server.Models
{
    /// <summary>
    /// The class for the Import Configuration data object
    /// </summary>
    public partial class ImportConfigurationData
    {
        [JsonConstructor]
        public ImportConfigurationData(string serverPath, bool forceImport)
        {
            ServerPath = serverPath;
            ForceImport = forceImport;
        }

        /// <summary>
        /// Gets or sets the server path
        /// </summary>
        [JsonProperty(PropertyName = "serverPath")]
        public string ServerPath { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to force importing
        /// </summary>
        [JsonProperty(PropertyName = "forceImport")]
        public bool ForceImport { get; set; }
    }
}
