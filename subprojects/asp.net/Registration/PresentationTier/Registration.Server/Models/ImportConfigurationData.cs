using Newtonsoft.Json;

namespace PerkinElmer.COE.Registration.Server.Models
{
    /// <summary>
    /// The class for the Import Configuration data object
    /// </summary>
    public partial class ImportConfigurationData
    {
        [JsonConstructor]
        public ImportConfigurationData(string localImport, bool forceImport)
        {
            LocalImport = localImport;
            ForceImport = forceImport;
        }

        /// <summary>
        /// Gets or Sets LocalImport
        /// </summary>
        [JsonProperty(PropertyName = "localImport")]
        public string LocalImport { get; set; }

        /// <summary>
        /// Gets or Sets ForceImport
        /// </summary>
        [JsonProperty(PropertyName = "forceImport")]
        public bool ForceImport { get; set; }
    }
}
