using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerkinElmer.COE.Inventory.Model
{
    public class SearchContainerData
    {
        /// <summary>
        /// Gets or sets the internal identifier of the compound.
        /// </summary>
        [JsonProperty(PropertyName = "compoundId")]
        public int? CompoundId { get; set; }

        /// <summary>
        /// Gets or sets the substance name.
        /// </summary>
        [JsonProperty(PropertyName = "substanceName")]
        public string SubstanceName { get; set; }

        /// <summary>
        /// Gets or sets the CAS registry number
        /// </summary>
        [JsonProperty(PropertyName = "casRegistryNumber")]
        public string CASRegistryNumber { get; set; }

        /// <summary>
        /// Gets or sets the internal identifier of the container status.
        /// </summary>
        [JsonProperty(PropertyName = "containerStatusId")]
        public int? ContainerStatusId { get; set; }

        /// <summary>
        /// Gets or sets the ACX number.
        /// </summary>
        [JsonProperty(PropertyName = "acxNumber")]
        public string ACXNumber { get; set; }

        /// <summary>
        /// Gets or sets the internal identifier of the location.
        /// </summary>
        [JsonProperty(PropertyName = "locationId")]
        public int? LocationId { get; set; }

        /// <summary>
        /// Gets or sets the barcode of the location.
        /// </summary>
        [JsonProperty(PropertyName = "locationBarcode")]
        public string LocationBarcode { get; set; }

        /// <summary>
        /// Gets or sets the LOT number.
        /// </summary>
        [JsonProperty(PropertyName = "lotNumber")]
        public string LotNumber { get; set; }

        /// <summary>
        /// Gets or sets the current user.
        /// </summary>
        [JsonProperty(PropertyName = "currentUser")]
        public string CurrentUser { get; set; }

        /// <summary>
        /// Gets or sets the remaining quantity of the container.
        /// </summary>
        [JsonProperty(PropertyName = "remainingQuantity")]
        public int? RemainingQuantity { get; set; }

        /// <summary>
        /// Gets or sets the internal identifier of the unit of measure of the container.
        /// </summary>
        [JsonProperty(PropertyName = "unitOfMeasureId")]
        public int? UnitOfMeasureId { get; set; }
    }
}
