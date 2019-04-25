using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerkinElmer.COE.Inventory.Model
{
    public class LocationTypeData
    {
        /// <summary>
        /// Gets or sets the location type id
        /// </summary>
        [JsonProperty(PropertyName = "locationTypeId")]
        public int LocationTypeId { get; set; }

        /// <summary>
        /// Gets or sets the location type name
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
    }
}
