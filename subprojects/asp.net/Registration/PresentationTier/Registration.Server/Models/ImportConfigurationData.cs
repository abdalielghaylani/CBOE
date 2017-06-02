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
        /// Gets or Sets LocalImport
        /// </summary>
        [JsonProperty(PropertyName = "serverPath")]
        public string ServerPath { get; set; }

        /// <summary>
        /// Gets or Sets ForceImport
        /// </summary>
        [JsonProperty(PropertyName = "forceImport")]
        public bool ForceImport { get; set; }
    }
}
