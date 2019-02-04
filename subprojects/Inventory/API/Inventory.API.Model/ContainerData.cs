using Newtonsoft.Json;
using System;

namespace PerkinElmer.COE.Inventory.Model
{
    public class ContainerData
    {
        /// <summary>
        /// Gets or sets the internal identifier of the container.
        /// </summary>
        [JsonProperty(PropertyName = "containerId")]
        public int ContainerId { get; set; }

        /// <summary>
        /// Gets or sets the barcode of the container.
        /// </summary>
        [JsonProperty(PropertyName = "barcode")]
        public string Barcode { get; set; }

        /// <summary>
        /// Gets or sets the name of the container.
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the supplier of the container.
        /// </summary>
        [JsonProperty(PropertyName = "supplier")]
        public string Supplier { get; set; }

        /// <summary>
        /// Gets or sets the current user.
        /// </summary>
        [JsonProperty(PropertyName = "currentUser")]
        public string CurrentUser { get; set; }

        /// <summary>
        /// Gets or sets the size of the container.
        /// </summary>
        [JsonProperty(PropertyName = "containerSize")]
        public decimal ContainerSize { get; set; }

        /// <summary>
        /// Gets or sets the qualtity available.
        /// </summary>
        [JsonProperty(PropertyName = "quantityAvailable")]
        public decimal? QuantityAvailable { get; set; }

        /// <summary>
        /// Gets or sets the Unit Of Measure
        /// </summary>
        [JsonProperty(PropertyName = "unitOfMeasure")]
        public string UnitOfMeasure { get; set; }

        /// <summary>
        /// Gets or sets the date created.
        /// </summary>
        [JsonProperty(PropertyName = "dateCreated")]
        public DateTime DateCreated { get; set; }

        /// <summary>
        /// Gets or sets the type of the container.
        /// </summary>
        [JsonProperty(PropertyName = "containerType")]
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the status of the container.
        /// </summary>
        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }        

        /// <summary>
        /// Gets or sets the concentration
        /// </summary>
        [JsonProperty(PropertyName = "concentration")]
        public decimal? Concentration { get; set; }

        /// <summary>
        /// Gets or sets the Unit Of Concentration
        /// </summary>
        [JsonProperty(PropertyName = "unitOfConcentration")]
        public string UnitOfConcentration { get; set; }

        /// <summary>
        /// Gets or sets the Unit Of Weight
        /// </summary>
        [JsonProperty(PropertyName = "unitOfWeight")]
        public string UnitOfWeight { get; set; }

        /// <summary>
        /// Gets or sets the density
        /// </summary>
        [JsonProperty(PropertyName = "density")]
        public decimal? Density { get; set; }

        /// <summary>
        /// Gets or sets the Purity
        /// </summary>
        [JsonProperty(PropertyName = "purity")]
        public decimal? Purity { get; set; }

        /// <summary>
        /// Gets or sets the Unit Of Purity
        /// </summary>
        [JsonProperty(PropertyName = "unitOfPurity")]
        public string UnitOfPurity { get; set; }

        /// <summary>
        /// Gets or sets the compound
        /// </summary>
        [JsonProperty(PropertyName = "compound")]
        public CompoundData Compound { get; set; }

        /// <summary>
        /// Gets or sets the location
        /// </summary>
        [JsonProperty(PropertyName = "location")]
        public LocationData Location { get; set; }
    }
}