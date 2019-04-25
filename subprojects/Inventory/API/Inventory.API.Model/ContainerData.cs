using Newtonsoft.Json;
using System;
using System.Data;

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
        [JsonProperty(PropertyName = "containerName")]
        public string ContainerName { get; set; }

        /// <summary>
        /// Gets or sets the supplier of the container.
        /// </summary>
        [JsonProperty(PropertyName = "supplier")]
        public SupplierData Supplier { get; set; }

        /// <summary>
        /// Gets or sets the current user.
        /// </summary>
        [JsonProperty(PropertyName = "currentUser")]
        public string CurrentUser { get; set; }

        /// <summary>
        /// Gets or sets the qualtity available.
        /// </summary>
        [JsonProperty(PropertyName = "quantityAvailable")]
        public UnitData QuantityAvailable { get; set; }

        /// <summary>
        /// Gets or sets the date created.
        /// </summary>
        [JsonProperty(PropertyName = "dateCreated")]
        public DateTime DateCreated { get; set; }

        /// <summary>
        /// Gets or sets the status of the container.
        /// </summary>
        [JsonProperty(PropertyName = "status")]
        public ContainerStatusData Status { get; set; }

        /// <summary>
        /// Gets or sets the concentration
        /// </summary>
        [JsonProperty(PropertyName = "concentration")]
        public UnitData Concentration { get; set; }

        /// <summary>
        /// Gets or sets the Weight
        /// </summary>
        [JsonProperty(PropertyName = "weight")]
        public UnitData Weight { get; set; }

        /// <summary>
        /// Gets or sets the density
        /// </summary>
        [JsonProperty(PropertyName = "density")]
        public UnitData Density { get; set; }

        /// <summary>
        /// Gets or sets the Purity
        /// </summary>
        [JsonProperty(PropertyName = "purity")]
        public UnitData Purity { get; set; }

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

        /// <summary>
        /// Gets or sets the initial quantity
        /// </summary>
        [JsonProperty(PropertyName = "quantityInitial")]
        public UnitData QuantityInitial { get; set; }

        /// <summary>
        /// Gets or sets the max quantity
        /// </summary>
        [JsonProperty(PropertyName = "quantityMax")]
        public UnitData QuantityMax { get; set; }

        /// <summary>
        /// Gets or sets the remaining quantity
        /// </summary>
        [JsonProperty(PropertyName = "quantityRemaining")]
        public UnitData QuantityRemaining { get; set; }

        /// <summary>
        /// Gets or sets the container type
        /// </summary>
        [JsonProperty(PropertyName = "containerType")]
        public ContainerTypeData ContainerType { get; set; }

        /// <summary>
        /// Gets or sets the registration id
        /// </summary>
        [JsonProperty(PropertyName = "regId")]
        public int? RegId { get; set; }

        /// <summary>
        /// Gets or sets the batch number
        /// </summary>
        [JsonProperty(PropertyName = "batchNumber")]
        public int? BatchNumber { get; set; }

        /// <summary>
        /// Gets or sets the minimum stock quantity
        /// </summary>
        [JsonProperty(PropertyName = "minStockQty")]
        public decimal? MinStockQty { get; set; }

        /// <summary>
        /// Gets or sets the maximum stock quantity
        /// </summary>
        [JsonProperty(PropertyName = "maxStockQty")]
        public decimal? MaxStockQty { get; set; }

        /// <summary>
        /// Gets or sets the expiration date
        /// </summary>
        [JsonProperty(PropertyName = "expirationDate")]
        public DateTime? ExpirationDate { get; set; }

        /// <summary>
        /// Gets or sets the description
        /// </summary>
        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the tare weight
        /// </summary>
        [JsonProperty(PropertyName = "tareWeight")]
        public decimal? TareWeight { get; set; }

        /// <summary>
        /// Gets or sets the net weight
        /// </summary>
        [JsonProperty(PropertyName = "netWeight")]
        public decimal? NetWeight { get; set; }

        /// <summary>
        /// Gets or sets the final weight
        /// </summary>
        [JsonProperty(PropertyName = "finalWeight")]
        public decimal? FinalWeight { get; set; }

        /// <summary>
        /// Gets or sets the solvent id
        /// </summary>
        [JsonProperty(PropertyName = "solventId")]
        public string SolventId { get; set; }

        /// <summary>
        /// Gets or sets the grade
        /// </summary>
        [JsonProperty(PropertyName = "grade")]
        public string Grade { get; set; }

        /// <summary>
        /// Gets or sets the comments
        /// </summary>
        [JsonProperty(PropertyName = "comments")]
        public string Comments { get; set; }

        /// <summary>
        /// Gets or sets the storage conditions
        /// </summary>
        [JsonProperty(PropertyName = "storageConditions")]
        public string StorageConditions { get; set; }

        /// <summary>
        /// Gets or sets the handling procedures
        /// </summary>
        [JsonProperty(PropertyName = "handlingProcedures")]
        public string HandlingProcedures { get; set; }

        /// <summary>
        /// Gets or sets the lot number
        /// </summary>
        [JsonProperty(PropertyName = "lotNumber")]
        public string LotNumber { get; set; }

        /// <summary>
        /// Gets or sets the date produced
        /// </summary>
        [JsonProperty(PropertyName = "dateProduced")]
        public DateTime? DateProduced { get; set; }

        /// <summary>
        /// Gets or sets the date ordered
        /// </summary>
        [JsonProperty(PropertyName = "dateOrdered")]
        public DateTime? DateOrdered { get; set; }

        /// <summary>
        /// Gets or sets the date received
        /// </summary>
        [JsonProperty(PropertyName = "dateReceived")]
        public DateTime? DateReceived { get; set; }

        /// <summary>
        /// Gets or sets the containerCost
        /// </summary>
        [JsonProperty(PropertyName = "containerCost")]
        public UnitData ContainerCost { get; set; }

        /// <summary>
        /// Gets or sets the PO number
        /// </summary>
        [JsonProperty(PropertyName = "poNumber")]
        public string PONumber { get; set; }

        /// <summary>
        /// Gets or sets the PO line number
        /// </summary>
        [JsonProperty(PropertyName = "poLineNumber")]
        public string POLineNumber { get; set; }

        /// <summary>
        /// Gets or sets the request number
        /// </summary>
        [JsonProperty(PropertyName = "requestNumber")]
        public string RequestNumber { get; set; }

        /// <summary>
        /// Gets or sets the owner id
        /// </summary>
        [JsonProperty(PropertyName = "ownerId")]
        public string OwnerId { get; set; }

        /// <summary>
        /// Gets or sets the number of copies
        /// </summary>
        [JsonProperty(PropertyName = "numberOfCopies")]
        public int? NumberOfCopies { get; set; }

        /// <summary>
        /// Gets or sets the field 1
        /// </summary>
        [JsonProperty(PropertyName = "field1")]
        public string Field1 { get; set; }

        /// <summary>
        /// Gets or sets the field 2
        /// </summary>
        [JsonProperty(PropertyName = "field2")]
        public string Field2 { get; set; }

        /// <summary>
        /// Gets or sets the field 3
        /// </summary>
        [JsonProperty(PropertyName = "field3")]
        public string Field3 { get; set; }

        /// <summary>
        /// Gets or sets the field 4
        /// </summary>
        [JsonProperty(PropertyName = "field4")]
        public string Field4 { get; set; }

        /// <summary>
        /// Gets or sets the field 5
        /// </summary>
        [JsonProperty(PropertyName = "field5")]
        public string Field5 { get; set; }

        /// <summary>
        /// Gets or sets the field 6
        /// </summary>
        [JsonProperty(PropertyName = "field6")]
        public string Field6 { get; set; }

        /// <summary>
        /// Gets or sets the field 7
        /// </summary>
        [JsonProperty(PropertyName = "field7")]
        public string Field7 { get; set; }

        /// <summary>
        /// Gets or sets the field 8
        /// </summary>
        [JsonProperty(PropertyName = "field8")]
        public string Field8 { get; set; }

        /// <summary>
        /// Gets or sets the field 9
        /// </summary>
        [JsonProperty(PropertyName = "field9")]
        public string Field9 { get; set; }

        /// <summary>
        /// Gets or sets the field 10
        /// </summary>
        [JsonProperty(PropertyName = "field10")]
        public string Field10 { get; set; }

        /// <summary>
        /// Gets or sets the date 1
        /// </summary>
        [JsonProperty(PropertyName = "date1")]
        public DateTime? Date1 { get; set; }

        /// <summary>
        /// Gets or sets the date 2
        /// </summary>
        [JsonProperty(PropertyName = "date2")]
        public DateTime? Date2 { get; set; }

        /// <summary>
        /// Gets or sets the date 3
        /// </summary>
        [JsonProperty(PropertyName = "date3")]
        public DateTime? Date3 { get; set; }

        /// <summary>
        /// Gets or sets the date 4
        /// </summary>
        [JsonProperty(PropertyName = "date4")]
        public DateTime? Date4 { get; set; }

        /// <summary>
        /// Gets or sets the date 5
        /// </summary>
        [JsonProperty(PropertyName = "date5")]
        public DateTime? Date5 { get; set; }

        /// <summary>
        /// Gets or sets the principal id
        /// </summary>
        [JsonProperty(PropertyName = "principalId")]
        public int? PrincipalId { get; set; }

        /// <summary>
        /// Gets or sets the barcode description id
        /// </summary>
        [JsonProperty(PropertyName = "barcodeDescriptionId")]
        public int? BarcodeDescriptionId { get; set; }

        /// <summary>
        /// Gets or sets the approved date
        /// </summary>
        [JsonProperty(PropertyName = "dateApproved")]
        public DateTime? DateApproved { get; set; }
    }
}