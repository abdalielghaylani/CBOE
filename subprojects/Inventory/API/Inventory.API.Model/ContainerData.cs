using Newtonsoft.Json;
using System;

namespace PerkinElmer.COE.Inventory.Model
{
    public class ContainerData
    {
        /// <summary>
        /// Gets or sets the internal identifier of the container.
        /// </summary>
        [JsonProperty(PropertyName = "ContainerId")]
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
        [JsonProperty(PropertyName = "CurrentUser")]
        public string CurrentUser { get; set; }

        /// <summary>
        /// Gets or sets the size of the container.
        /// </summary>
        [JsonProperty(PropertyName = "ContainerSize")]
        public decimal ContainerSize { get; set; }

        /// <summary>
        /// Gets or sets the qualtity available.
        /// </summary>
        [JsonProperty(PropertyName = "QuantityAvailable")]
        public decimal? QuantityAvailable { get; set; }

        /// <summary>
        /// Gets or sets the Unit Of Measure
        /// </summary>
        [JsonProperty(PropertyName = "UnitOfMeasure")]
        public string UnitOfMeasure { get; set; }

        /// <summary>
        /// Gets or sets the date created.
        /// </summary>
        [JsonProperty(PropertyName = "DateCreated")]
        public DateTime DateCreated { get; set; }

        /// <summary>
        /// Gets or sets the type of the container.
        /// </summary>
        [JsonProperty(PropertyName = "ContainerType")]
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the status of the container.
        /// </summary>
        [JsonProperty(PropertyName = "Status")]
        public string Status { get; set; }        

        /// <summary>
        /// Gets or sets the concentration
        /// </summary>
        [JsonProperty(PropertyName = "Concentration")]
        public decimal? Concentration { get; set; }

        /// <summary>
        /// Gets or sets the Unit Of Concentration
        /// </summary>
        [JsonProperty(PropertyName = "UnitOfConcentration")]
        public string UnitOfConcentration { get; set; }

        /// <summary>
        /// Gets or sets the Unit Of Weight
        /// </summary>
        [JsonProperty(PropertyName = "UnitOfWeight")]
        public string UnitOfWeight { get; set; }

        /// <summary>
        /// Gets or sets the Purity
        /// </summary>
        [JsonProperty(PropertyName = "Purity")]
        public decimal? Purity { get; set; }

        /// <summary>
        /// Gets or sets the Unit Of Purity
        /// </summary>
        [JsonProperty(PropertyName = "UnitOfPurity")]
        public string UnitOfPurity { get; set; }

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