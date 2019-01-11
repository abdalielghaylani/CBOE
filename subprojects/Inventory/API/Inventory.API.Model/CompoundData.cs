using Newtonsoft.Json;
using System;

namespace PerkinElmer.COE.Inventory.Model
{
    public class CompoundData
    {
        /// <summary>
        /// Gets or sets the internal identifier of the compound.
        /// </summary>
        [JsonProperty(PropertyName = "Compound ID")]
        public int CompoundId { get; set; }

        /// <summary>
        /// Gets or sets the Molecular identifier of the compound.
        /// </summary>
        [JsonProperty(PropertyName = "Mol ID")]
        public Nullable<int> MolId { get; set; }

        /// <summary>
        /// Gets or sets the CAS of the compound.
        /// </summary>
        [JsonProperty(PropertyName = "Cas")]
        public string Cas { get; set; }

        /// <summary>
        /// Gets or sets the ACX identifier of the compound.
        /// </summary>
        [JsonProperty(PropertyName = "ACX ID")]
        public string AcxId { get; set; }

        /// <summary>
        /// Gets or sets the Substance Name of the compound.
        /// </summary>
        [JsonProperty(PropertyName = "Substance Name")]
        public string SubstanceName { get; set; }

        /// <summary>
        /// Gets or sets the Base64 CDX of the compound.
        /// </summary>
        [JsonProperty(PropertyName = "Base64 CDX")]
        public string Base64Cdx { get; set; }

        /// <summary>
        /// Gets or sets the Molecular Weight of the compound.
        /// </summary>
        [JsonProperty(PropertyName = "Molecular Weight")]
        public Nullable<decimal> MolecularWeight { get; set; }

        /// <summary>
        /// Gets or sets the Density of the compound.
        /// </summary>
        [JsonProperty(PropertyName = "Density")]
        public decimal Density { get; set; }
    }
}