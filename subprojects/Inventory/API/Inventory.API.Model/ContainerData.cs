using Newtonsoft.Json;
using System;

namespace PerkinElmer.COE.Inventory.Model
{
    public class ContainerData
    {
        /// <summary>
        /// Gets or sets the internal identifier of the container.
        /// </summary>
        [JsonProperty(PropertyName = "Container ID")]
        public int ContainerId { get; set; }

        /// <summary>
        /// Gets or sets the barcode of the container.
        /// </summary>
        [JsonProperty(PropertyName = "Barcode")]
        public string Barcode { get; set; }

        /// <summary>
        /// Gets or sets the name of the container.
        /// </summary>
        [JsonProperty(PropertyName = "Name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the supplier of the container.
        /// </summary>
        [JsonProperty(PropertyName = "Supplier")]
        public string Supplier { get; set; }

        /// <summary>
        /// Gets or sets the current user.
        /// </summary>
        [JsonProperty(PropertyName = "Current User")]
        public string CurrentUser { get; set; }

        /// <summary>
        /// Gets or sets the size of the container.
        /// </summary>
        [JsonProperty(PropertyName = "Container Size")]
        public decimal ContainerSize { get; set; }

        /// <summary>
        /// Gets or sets the date created.
        /// </summary>
        [JsonProperty(PropertyName = "Date Created")]
        public DateTime DateCreated { get; set; }

        /// <summary>
        /// Gets or sets the type of the container.
        /// </summary>
        [JsonProperty(PropertyName = "Container Type")]
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the status of the container.
        /// </summary>
        [JsonProperty(PropertyName = "Status")]
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets the qualtity available.
        /// </summary>
        [JsonProperty(PropertyName = "Quantity Available")]
        public decimal QuantityAvailable { get; set; }

        /// <summary>
        /// Gets or sets the unit
        /// </summary>
        [JsonProperty(PropertyName = "Unit")]
        public string Unit { get; set; }

        /// <summary>
        /// Gets or sets the compound
        /// </summary>
        [JsonProperty(PropertyName = "Compound")]
        public CompoundData Compound { get; set; }

        /// <summary>
        /// Gets or sets the location
        /// </summary>
        [JsonProperty(PropertyName = "Location")]
        public LocationData Location { get; set; }
    }
}