using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerkinElmer.COE.Inventory.Model
{
    public class ContainerTypeData
    {
        /// <summary>
        /// Gets or sets the container type id
        /// </summary>
        [JsonProperty(PropertyName = "containerTypeId")]
        public int ContainerTypeId { get; set; }

        /// <summary>
        /// Gets or sets the container type name
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
    }
}
