using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerkinElmer.COE.Inventory.Model
{
    public class CustomFieldData
    {
        /// <summary>
        /// Gets or sets the internal identifier of the custom field.
        /// </summary>
        [JsonProperty(PropertyName = "customFieldId")]
        public int CustomFieldId { get; set; }

        /// <summary>
        /// Gets or sets the name of the custom field.
        /// </summary>
        [JsonProperty(PropertyName = "customFieldName")]
        public string CustomFielName { get; set; }

        // <summary>
        /// Gets or sets the name of the custom field group.
        /// </summary>
        [JsonProperty(PropertyName = "customFieldGroupName")]
        public string CustomFielGroupName { get; set; }
    }
}
