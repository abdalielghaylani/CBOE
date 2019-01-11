using Newtonsoft.Json;
using System;

namespace PerkinElmer.COE.Inventory.Model
{
    public class LocationData
    {
        /// <summary>
        /// Gets or sets the internal identifier of the location.
        /// </summary>
        [JsonProperty(PropertyName = "ID")]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the internal identifier of the parent location.
        /// </summary>
        [JsonProperty(PropertyName = "Parent ID")]
        public int? ParentId { get; set; }

        /// <summary>
        /// Gets or sets the name of the location.
        /// </summary>
        [JsonProperty(PropertyName = "Name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description of the location.
        /// </summary>
        [JsonProperty(PropertyName = "Description")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the type of the location.
        /// </summary>
        [JsonProperty(PropertyName = "Type")]
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the barcode of the location.
        /// </summary>
        [JsonProperty(PropertyName = "Barcode")]
        public string Barcode { get; set; }
    }
}