using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace PerkinElmer.COE.Inventory.Model
{
    public class CompoundData
    {
        /// <summary>
        /// Gets or sets the internal identifier of the compound.
        /// </summary>
        [JsonProperty(PropertyName = "compoundId")]
        public int CompoundId { get; set; }

        /// <summary>
        /// Gets or sets the Molecular identifier of the compound.
        /// </summary>
        [JsonProperty(PropertyName = "molId")]
        public Nullable<int> MolId { get; set; }

        /// <summary>
        /// Gets or sets the CAS of the compound.
        /// </summary>
        [JsonProperty(PropertyName = "cas")]
        public string Cas { get; set; }

        /// <summary>
        /// Gets or sets the ACX identifier of the compound.
        /// </summary>
        [JsonProperty(PropertyName = "acxId")]
        public string AcxId { get; set; }

        /// <summary>
        /// Gets or sets the Substance Name of the compound.
        /// </summary>
        [JsonProperty(PropertyName = "substanceName")]
        public string SubstanceName { get; set; }

        /// <summary>
        /// Gets or sets the Base64 CDX of the compound.
        /// </summary>
        [JsonProperty(PropertyName = "base64Cdx")]
        public string Base64Cdx { get; set; }

        /// <summary>
        /// Gets or sets the Molecular Weight of the compound.
        /// </summary>
        [JsonProperty(PropertyName = "molecularWeight")]
        public Nullable<decimal> MolecularWeight { get; set; }

        /// <summary>
        /// Gets or sets the Safety Data of the compound.
        /// </summary>
        [JsonProperty(PropertyName = "safetyData")]
        public List<CustomFieldData> SafetyData { get; set; }
    }
}