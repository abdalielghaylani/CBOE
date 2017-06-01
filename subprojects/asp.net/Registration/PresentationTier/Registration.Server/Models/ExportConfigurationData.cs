using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace PerkinElmer.COE.Registration.Server.Models
{
    /// <summary>
    /// The class for the Export Configuration data object
    /// </summary>
    public partial class ExportConfigurationData
    {
        [JsonConstructor]
        public ExportConfigurationData(string exportDir, List<string> tableNames)
        {
            ExportDir = exportDir;
            TableNames = tableNames;
        }

        /// <summary>
        /// Gets or Sets ExportDir
        /// </summary>
        [JsonProperty(PropertyName = "exportDir")]
        public string ExportDir { get; set; }

        /// <summary>
        /// Gets or sets the TableNames 
        /// </summary>
        [JsonProperty(PropertyName = "tableNames")]
        public List<string> TableNames { get; set; }
    }
}