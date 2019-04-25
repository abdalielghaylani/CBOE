using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerkinElmer.COE.Inventory.Model
{
    public class ContainerStatusData
    {
        /// <summary>
        /// Gets or sets the status id
        /// </summary>
        [JsonProperty(PropertyName = "statusId")]
        public short StatusId { get; set; }

        /// <summary>
        /// Gets or sets the status name
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
    }
}
