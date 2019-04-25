using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerkinElmer.COE.Inventory.Model
{
    public class ContainerUpdatedData
    {
        /// <summary>
        /// Gets or sets the name of the container property.
        /// </summary>
        [JsonProperty(PropertyName = "propertyName")]
        public string PropertyName { get; set; }

        /// <summary>
        /// Gets or sets the value of the container property.
        /// </summary>
        [JsonProperty(PropertyName = "propertyValue")]
        public string PropertyValue { get; set; }
    }
}
