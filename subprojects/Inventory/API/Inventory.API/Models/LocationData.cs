using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PerkinElmer.COE.Inventory.API.Models
{
    public class LocationData
    {
        /// <summary>
        /// Gets or sets the internal identifier of the location.
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the internal identifier of the parent location.
        /// </summary>
        [JsonProperty(PropertyName = "parentId")]
        public int ParentId { get; set; }

        /// <summary>
        /// Gets or sets the name of the location.
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description of the location.
        /// </summary>
        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the type of the location.
        /// </summary>
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }
    }
}