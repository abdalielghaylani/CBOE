using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace PerkinElmer.COE.Registration.Server.Models
{
    /// <summary>
    /// The class for the CriteriaField data object
    /// </summary>
    public partial class CriteriaFieldData
    {
        public CriteriaFieldData() 
        {
        }

        [JsonConstructor]
        public CriteriaFieldData(int fieldId, string alias, bool visible)
        {
            FieldId = fieldId;
            Alias = alias;
            Visible = visible;
        }

        /// <summary>
        /// Gets or sets the fieldId
        /// </summary>
        [JsonProperty(PropertyName = "fieldId")]
        public int FieldId { get; set; }

        /// <summary>
        /// Gets or sets the alias
        /// </summary>
        [JsonProperty(PropertyName = "alias")]
        public string Alias { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not this criterion is visible
        /// </summary>
        [JsonProperty(PropertyName = "visible")]
        public bool Visible { get; set; }
    }
}