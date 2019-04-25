using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerkinElmer.COE.Inventory.Model
{
    public class UnitData
    {
        /// <summary>
        /// Gets or sets the id of the unit.
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }
        /// <summary>
        /// Gets or sets the value of the unit.
        /// </summary>
        [JsonProperty(PropertyName = "value")]

        public decimal? Value { get; set; }

        /// <summary>
        /// Gets or sets the unit.
        /// </summary>
        [JsonProperty(PropertyName = "unit")]
        public string Unit { get; set; }
    }
}
