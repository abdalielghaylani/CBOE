using System;
using System.Runtime.Serialization;
using System.Xml;
using CambridgeSoft.COE.Framework;
using CambridgeSoft.COE.Framework.Common;
using Spotfire.Dxp.Framework.Persistence;
using System.Collections.Generic;
using System.Linq;

namespace SpotfireIntegration.Common
{
    /// <summary>
    /// Contains metadata about a COE hitlist and a ResultsCriteria.
    /// </summary>
    [Serializable]
    [PersistenceVersion(1, 0)]
    // FIXME: This needs a better name.
    public sealed class COEHitList : ISerializable
    {
        private int hitListID;
        private readonly HitListType hitListType;
        private readonly int numHits;
        private readonly int dataViewID;
        private /*readonly*/ string resultsCriteriaXml;
        //variable to hold the query criteria xml
        private string searchCriteriaXml;

        [NonSerialized]
        private ResultsCriteria resultsCriteria;

        [NonSerialized]
        private SearchCriteria searchCriteria;

        public COEHitList()
        {
            this.hitListID = 0;
            this.hitListType = CambridgeSoft.COE.Framework.HitListType.TEMP;
            this.numHits = 0;
            this.dataViewID = 0;
            this.resultsCriteriaXml = string.Empty;
            this.resultsCriteria = new ResultsCriteria();
            this.searchCriteriaXml = string.Empty;
            this.searchCriteria = new SearchCriteria();
        }

        public COEHitList(int hitListID, HitListType hitListType, int numHits, int dataViewID, ResultsCriteria resultsCriteria)
            : this()
        {
            this.hitListID = hitListID;
            this.hitListType = hitListType;
            this.numHits = numHits;
            this.dataViewID = dataViewID;
            this.resultsCriteriaXml = resultsCriteria.ToString();
            this.resultsCriteria = resultsCriteria;
        }

        public COEHitList(int hitListID, HitListType hitListType, int numHits, int dataViewID, string resultsCriteriaXml)
            : this()
        {
            this.hitListID = hitListID;
            this.hitListType = hitListType;
            this.numHits = numHits;
            this.dataViewID = dataViewID;
            this.resultsCriteriaXml = resultsCriteriaXml;
            this.resultsCriteria = null;
        }

        public COEHitList(int hitListID, HitListType hitListType, int numHits, int dataViewID, ResultsCriteria resultsCriteria, SearchCriteria searchCriteria)
            : this(hitListID, hitListType, numHits, dataViewID, resultsCriteria)
        {
            this.searchCriteriaXml = searchCriteria.ToString();
            this.searchCriteria = searchCriteria;
        }

        public COEHitList(int hitListID, HitListType hitListType, int numHits, int dataViewID, string resultsCriteriaXml, string searchCriteriaXml)
            : this(hitListID, hitListType, numHits, dataViewID, resultsCriteriaXml)
        {
            this.searchCriteriaXml = searchCriteriaXml;
            this.searchCriteria = null;
        }

        public COEHitList(HitListType hitListType, int dataViewID, ResultsCriteria resultsCriteria, SearchCriteria searchCriteria)
            : this(0, hitListType, 0, dataViewID, resultsCriteria)
        {
            this.searchCriteria = searchCriteria;
            this.searchCriteriaXml = searchCriteria.ToString();
        }

        public COEHitList(SerializationInfo info, StreamingContext context)
        {
            this.hitListID = info.GetInt32("hitListID");
            this.hitListType = (HitListType)info.GetInt32("hitListType");
            this.numHits = info.GetInt32("numHits");
            this.dataViewID = info.GetInt32("dataViewID");
            this.resultsCriteriaXml = info.GetString("resultsCriteriaXml");
            this.resultsCriteria = null;
            //to make compatible with 12.5.2 we need to check the number of members present in the 
            //datacontract. In 12.5.2 the CBVN has only 5 arguments present in the data contract, hence, default he search criteria to empty.
            if (info.MemberCount > 5)
            {
                this.searchCriteriaXml = info.GetString("searchCriteriaXml");
            }
            else
            {
                this.searchCriteriaXml = string.Empty;
            }
            this.searchCriteria = null;
        }

        public int HitListID
        {
            get
            {
                return this.hitListID;
            }
            set
            {
                this.hitListID = value;
            }
        }

        public HitListType HitListType
        {
            get
            {
                return this.hitListType;
            }
        }

        public int NumHits
        {
            get
            {
                return this.numHits;
            }
        }

        public int DataViewID
        {
            get
            {
                return this.dataViewID;
            }
        }

        public String ResultsCriteriaXML
        {
            get { return resultsCriteriaXml; }
        }

        public ResultsCriteria ResultsCriteria
        {
            get
            {
                if (this.resultsCriteria == null)
                {
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(this.resultsCriteriaXml);
                    this.resultsCriteria = new ResultsCriteria(doc);
                }

                return this.resultsCriteria;
            }
            set // added by JD; needed to update RC after sort
            {
                this.resultsCriteria = value;
                this.resultsCriteriaXml = this.resultsCriteria.ToString();
            }
        }

        public String SearchCriteriaXml
        {
            get { return searchCriteriaXml; }
        }

        public SearchCriteria SearchCriteria
        {
            get
            {
                if (this.searchCriteria == null && !string.IsNullOrEmpty(this.searchCriteriaXml))
                {
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(this.searchCriteriaXml);
                    this.searchCriteria = new SearchCriteria(doc);
                }

                return this.searchCriteria;
            }
            set // added by JD; needed to update RC after sort
            {
                this.searchCriteria = value;
                this.searchCriteriaXml = this.searchCriteria.ToString();
            }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("hitListID", this.hitListID);
            info.AddValue("hitListType", (int)this.hitListType);
            info.AddValue("numHits", this.numHits);
            info.AddValue("dataViewID", this.dataViewID);
            info.AddValue("resultsCriteriaXml", this.resultsCriteriaXml);
            info.AddValue("searchCriteriaXml", this.searchCriteriaXml);
        }

        public override bool Equals(object obj)
        {
            if (obj is COEHitList)
            {
                COEHitList other = (COEHitList)obj;
                return (this.hitListID == other.hitListID
                    && this.hitListType == other.hitListType
                    && this.dataViewID == other.dataViewID
                    && CambridgeSoft.COE.Framework.Common.Utilities.GetMD5Hash(other.ResultsCriteriaXML) == CambridgeSoft.COE.Framework.Common.Utilities.GetMD5Hash(this.ResultsCriteriaXML)
                    && CambridgeSoft.COE.Framework.Common.Utilities.GetMD5Hash(other.SearchCriteriaXml) == CambridgeSoft.COE.Framework.Common.Utilities.GetMD5Hash(this.SearchCriteriaXml));
            }

            return false;
        }

        public override int GetHashCode()
        {
            return (this.hitListID + " " + this.dataViewID + "-" + this.ResultsCriteriaXML + "-" + this.SearchCriteriaXml).GetHashCode();
        }
    }

    /// <summary>
    /// Contains the metadata about the search criteria field order for persisting
    /// </summary>
    [Serializable]
    [PersistenceVersion(1, 0)]
    public class SearchCriteriaFieldOrder : ISerializable
    {
        #region Variables
        string searchFieldsOrder;

        [NonSerialized]
        SearchFields searchFieldsCollection; 
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the search criteria fields collection
        /// </summary>
        public SearchFields SearchFieldsCollection
        {
            get
            {
                if (searchFieldsOrder != null)
                {
                    searchFieldsCollection.DeserializeCollection(searchFieldsOrder);
                }
                return searchFieldsCollection;
            }
            set
            {
                searchFieldsCollection = value;
                searchFieldsOrder = searchFieldsCollection.ToString();
            }
        }

        /// <summary>
        /// Gets the search criteria fields order in xml string format
        /// </summary>
        public string SearchFieldsOrder
        {
            get
            {
                return searchFieldsOrder;
            }
        } 
        #endregion


        #region Constructor

        public SearchCriteriaFieldOrder()
        {
            searchFieldsCollection = new SearchFields();
        }

        public SearchCriteriaFieldOrder(string searchCriteriaFieldOrder)
            : this()
        {
            this.searchFieldsOrder = searchCriteriaFieldOrder;
        }


        public SearchCriteriaFieldOrder(SerializationInfo info, StreamingContext context)
        {
            searchFieldsOrder = info.GetString("searchFieldsOrder");
        }
 
        #endregion

        /// <summary>
        /// Populates a System.Runtime.Serialization.SerializationInfo with the data needed to serialize the target object.
        /// </summary>
        /// <param name="info">The System.Runtime.Serialization.SerializationInfo to populate with data.</param>
        /// <param name="context">The destination (see System.Runtime.Serialization.StreamingContext) for this serialization.</param>
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("searchFieldsOrder", searchFieldsOrder);
        }

        /// <summary>
        /// Add search criteria field to collection
        /// </summary>
        /// <param name="parentTableId">parent table id of the fieldBo object</param>
        /// <param name="tableId">table id to add</param>
        /// <param name="fieldId">field id to add</param>
        public void AddSearchField(int parentTableId, int fieldId, int tableId, Type fieldCriteriaType, string fldValue, string coeOperator)
        {
            searchFieldsCollection.AddSearchField(parentTableId, tableId, fieldId, fieldCriteriaType, fldValue, coeOperator);
        }

        /// <summary>
        /// Removes the search field from collection
        /// </summary>
        /// <param name="parentTableId">parent table id of the fieldBo object</param>
        /// <param name="tableId">table id to remove</param>
        /// <param name="fieldId">field id from selected TableBO to remove</param>
        public void RemoveSearchField(int parentTableId, int fieldId,int tableId)
        {
            searchFieldsCollection.RemoveSearchField(parentTableId, tableId, fieldId);
        }

        /// <summary>
        /// Clears the search field collection
        /// </summary>
        public void ClearSearchFields()
        {
            searchFieldsCollection.ClearSearchFields();
        }

        /// <summary>
        /// Generates the xml string representation of the search field collection
        /// </summary>
        /// <returns>returns the xml string of the search fields collection</returns>
        public override string ToString()
        {
            return searchFieldsCollection.ToString();
        }
    }
}
