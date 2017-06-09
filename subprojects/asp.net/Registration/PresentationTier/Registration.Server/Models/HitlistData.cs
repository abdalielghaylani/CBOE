using System.Collections.Generic;
using Csla;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using CambridgeSoft.COE.Framework;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.COEHitListService;
using System;

namespace PerkinElmer.COE.Registration.Server.Models
{
    /// <summary>
    /// The class for hit-list information
    /// </summary>
    public partial class HitlistData
    {
        /// <summary>
        /// Constructs the hit-list object from a <see cref="COEHitListBO" /> object 
        /// </summary>
        /// <param name="hitlistBO">The <see cref="COEHitListBO" /> object</param>
        public HitlistData(COEHitListBO hitlistBO)
        {
            ID = hitlistBO.ID;
            HitlistID = hitlistBO.HitListID;
            HitlistType = hitlistBO.HitListType;
            NumberOfHits = hitlistBO.NumHits;
            IsPublic = hitlistBO.IsPublic;
            SearchCriteriaID = hitlistBO.SearchCriteriaID;
            SearchCriteriaType = hitlistBO.SearchCriteriaType;
            Name = hitlistBO.Name;
            Description = hitlistBO.Description;
            MarkedHitIDs = hitlistBO.MarkedHitListIDs;
            var dateCreated = hitlistBO.DateCreated;
            if (!dateCreated.IsEmpty)
                DateCreated = dateCreated.Date;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HitlistData" /> class.
        /// </summary>
        /// <param name="id">ID</param>
        /// <param name="hitlistID">Hit-list ID</param>
        /// <param name="hitlistType">Hit-list type</param>
        /// <param name="numberOfHits">Number of hits</param>
        /// <param name="isPublic">Is public</param>
        /// <param name="searchCriteriaID">Search criteria ID</param>
        /// <param name="searchCriteriaType">Search criteria type</param>
        /// <param name="name">Name of this hit-list object</param>
        /// <param name="description">Description of this hit-list object</param>
        /// <param name="markedHitIDs">IDs of all marked hits</param>
        /// <param name="dateCreated">Date created</param>
        [JsonConstructor]
        public HitlistData(int? id = null, int? hitlistID = null, HitListType hitlistType = HitListType.ALL, int? numberOfHits = null, bool? isPublic = null, int? searchCriteriaID = null, SearchCriteriaType searchCriteriaType = SearchCriteriaType.TEMP, string name = null, string description = null, List<int> markedHitIDs = null, DateTime? dateCreated = null)
        {
            ID = id;
            HitlistID = hitlistID;
            HitlistType = hitlistType;
            NumberOfHits = numberOfHits;
            IsPublic = isPublic;
            SearchCriteriaID = searchCriteriaID;
            SearchCriteriaType = searchCriteriaType;
            Name = name;
            Description = description;
            MarkedHitIDs = markedHitIDs;
            DateCreated = dateCreated;
        }

        /// <summary>
        /// Gets or sets ID
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public int? ID { get; set; }

        /// <summary>
        /// Gets or sets the hit-list ID
        /// </summary>
        [JsonProperty(PropertyName = "hitlistId")]
        public int? HitlistID { get; set; }

        /// <summary>
        /// Gets or sets the hit-list type
        /// </summary>
        [JsonProperty(PropertyName = "hitlistType")]
        [JsonConverter(typeof(StringEnumConverter))]
        public HitListType HitlistType { get; set; }

        /// <summary>
        /// Gets or sets the number of hits
        /// </summary>
        [JsonProperty(PropertyName = "numberOfHits")]
        public int? NumberOfHits { get; set; }

        /// <summary>
        /// Gets or sets the indictor that it is public
        /// </summary>
        [JsonProperty(PropertyName = "isPublic")]
        public bool? IsPublic { get; set; }

        /// <summary>
        /// Gets or sets the search criteria ID
        /// </summary>
        [JsonProperty(PropertyName = "searchCriteriaId")]
        public int? SearchCriteriaID { get; set; }

        /// <summary>
        /// Gets or sets the search criteria type
        /// </summary>
        [JsonProperty(PropertyName = "searchCriteriaType")]
        [JsonConverter(typeof(StringEnumConverter))]
        public SearchCriteriaType SearchCriteriaType { get; set; }

        /// <summary>
        /// Gets or sets Name of this hit-list object
        /// </summary>
        /// <value>Name of this hit-list object</value>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description
        /// </summary>
        /// <value>Description of this hit-list object</value>
        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets IDs of all marked hits
        /// </summary>
        /// <value>IDs of all marked hits</value>
        [JsonProperty(PropertyName = "markedHitIds", NullValueHandling = NullValueHandling.Ignore)]
        public List<int> MarkedHitIDs { get; set; }

        /// <summary>
        /// Gets or sets the creation date
        /// </summary>
        [JsonProperty(PropertyName = "dateCreated")]
        public DateTime? DateCreated { get; set; }

    }
}
