using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerkinElmer.COE.Inventory.Model
{
    public class UnitTypeData
    {
        /// <summary>
        /// Gets or sets the unit type id
        /// </summary>
        [JsonProperty(PropertyName = "unitTypeId")]
        public short UnitTypeId { get; set; }

        /// <summary>
        /// Gets or sets the location type name
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
    }
}
