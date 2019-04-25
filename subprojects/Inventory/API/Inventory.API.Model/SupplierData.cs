using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerkinElmer.COE.Inventory.Model
{
    public class SupplierData
    {
        /// <summary>
        /// Gets or sets the supplier id.
        /// </summary>
        [JsonProperty(PropertyName = "supplierId")]
        public short SupplierId { get; set; }

        /// <summary>
        /// Gets or sets the supplier name.
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the cat number.
        /// </summary>
        [JsonProperty(PropertyName = "catNumber")]
        public string CatNumber { get; set; }
    }
}
