using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace PerkinElmer.COE.Registration.Server.Models
{
    /// <summary>
    /// The class for the CriteriaTable data object
    /// </summary>
    public class CriteriaTableData
    {
        public CriteriaTableData()
        {
        }

        [JsonConstructor]
        public CriteriaTableData(int tableId, List<CriteriaFieldData> fields)
        {
            TableId = tableId;
            Fields = fields;

        }

        /// <summary>
        /// Gets or sets the tableId
        /// </summary>
        [JsonProperty(PropertyName = "tableId")]
        public int TableId { get; set; }

        /// <summary>
        /// Gets or sets the fields
        /// </summary>
        [JsonProperty(PropertyName = "fields")]
        public List<CriteriaFieldData> Fields { get; set; }

        /// <summary>
        /// Gets or sets the molWeights
        /// </summary>
        [JsonProperty(PropertyName = "molWeights")]
        public List<CriteriaFieldData> MolWeights { get; set; }

        /// <summary>
        /// Gets or sets the formulas
        /// </summary>
        [JsonProperty(PropertyName = "formulas")]
        public List<CriteriaFieldData> Formulas { get; set; }

        /// <summary>
        /// Gets or sets the CDXToMolFiles
        /// </summary>
        [JsonProperty(PropertyName = "CDXToMolFiles")]
        public List<CriteriaFieldData> CDXToMolFiles { get; set; }
    }
}