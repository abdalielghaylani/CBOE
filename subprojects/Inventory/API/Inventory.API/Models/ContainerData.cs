using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PerkinElmer.COE.Inventory.API.Models
{
    public class ContainerData
    {
        /// <summary>
        /// Gets or sets the internal identifier of the container.
        /// </summary>
        [JsonProperty(PropertyName = "ID")]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the container id of the container.
        /// </summary>
        [JsonProperty(PropertyName = "Container ID")]
        public string ContainerId { get; set; }

        /// <summary>
        /// Gets or sets the name of the container.
        /// </summary>
        [JsonProperty(PropertyName = "Name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the location of the container.
        /// </summary>
        [JsonProperty(PropertyName = "Location")]
        public string Location { get; set; }

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
        public string ContainerSize { get; set; }

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
        public string QuantityAvailable { get; set; }
    }
}