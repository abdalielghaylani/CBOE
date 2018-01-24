using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace SpotfireIntegration.Common
{
    /// <summary>
    /// Class to hold the collection of search criteria fields for maintaining field order on query tab
    /// </summary>
    public class SearchFields
    {
        #region Variables

        List<SearchField> searchFieldCollection; 
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the collection of search criteria fields collection
        /// </summary>
        public List<SearchField> SearchFieldCollection
        {
            get { return searchFieldCollection; }
            set { searchFieldCollection = value; }
        }

        #endregion

        /// <summary>
        /// Initializes an instance of the SearchFields object
        /// </summary>
        public SearchFields()
        {
            searchFieldCollection = new List<SearchField>();
        }

        /// <summary>
        /// Adds a new search field to collection
        /// </summary>
        /// <param name="parentTableId">parent table id to which table vbelongs on UI</param>
        /// <param name="fieldId">field id to add</param>
        /// <param name="tableId">table id corresponding to field</param>
        /// <param name="fieldCriteriaType">type of the field criteria</param>
        /// <param name="fldValue">field value</param>
        /// <param name="coeOperator">coe operator value</param>
        public void AddSearchField(int parentTableId, int fieldId, int tableId, Type fieldCriteriaType, string fldValue, string coeOperator)
        {
            SearchField fld = new SearchField(parentTableId, fieldId, tableId, fieldCriteriaType.ToString(), fldValue, coeOperator);
            searchFieldCollection.Add(fld);
        }

        /// <summary>
        /// Removes the search field from collection
        /// </summary>
        /// <param name="parentTableId">parent table id of the field object</param>
        /// <param name="tableId">table id to remove</param>
        /// <param name="fieldId">field id to remove</param>
        public void RemoveSearchField(int parentTableId, int fieldId, int tableId)
        {
            SearchField fld = searchFieldCollection.FirstOrDefault(p => p.TableId == tableId && p.FieldId == fieldId && p.ParentTableId == parentTableId);
            if (fld != null)
            {
                searchFieldCollection.Remove(fld);
            }
        }

        /// <summary>
        /// Clears the search field collection
        /// </summary>
        public void ClearSearchFields()
        {
            searchFieldCollection.Clear();
        }

        /// <summary>
        /// Generates the xml output of the search fields collection for saving as datatable property
        /// </summary>
        /// <returns>returns xml representation of the search fields collection</returns>
        public override string ToString()
        {
            XmlDocument doc = new XmlDocument();
            XmlNode parentNode = doc.CreateNode(XmlNodeType.Element, "searchFieldOrder", "");

            foreach (SearchField fld in searchFieldCollection)
            {
                XmlNode searchFldNode = doc.CreateNode(XmlNodeType.Element, "searchField", "");
                XmlAttribute parentTableIdAttr = doc.CreateAttribute("parentTableId");
                parentTableIdAttr.Value = fld.ParentTableId.ToString();
                XmlAttribute tableIdAttr = doc.CreateAttribute("tableId");
                tableIdAttr.Value = fld.TableId.ToString();
                XmlAttribute fldIdAttr = doc.CreateAttribute("fieldId");
                fldIdAttr.Value = fld.FieldId.ToString();
                XmlAttribute fieldCriteriaTypeAttr = doc.CreateAttribute("fieldCriteriaType");
                fieldCriteriaTypeAttr.Value = fld.FieldCriteriaType.ToString();
                XmlAttribute fieldValueAttr = doc.CreateAttribute("fieldValue");
                fieldValueAttr.Value = fld.FieldValue.ToString();
                XmlAttribute fieldCoeOperatorAttr = doc.CreateAttribute("fieldCoeOperator");
                fieldCoeOperatorAttr.Value = fld.CoeOperator.ToString();
                searchFldNode.Attributes.Append(parentTableIdAttr);
                searchFldNode.Attributes.Append(tableIdAttr);
                searchFldNode.Attributes.Append(fldIdAttr);
                searchFldNode.Attributes.Append(fieldCriteriaTypeAttr);
                searchFldNode.Attributes.Append(fieldValueAttr);
                searchFldNode.Attributes.Append(fieldCoeOperatorAttr);
                parentNode.AppendChild(searchFldNode);
            }
            doc.AppendChild(parentNode);
            return doc.InnerXml;
        }

        /// <summary>
        /// Generates the search fields collection using the xml string
        /// </summary>
        /// <param name="xmlSearchOrder">xml string containing search fields collection</param>
        public void DeserializeCollection(string xmlSearchOrder)
        {
            searchFieldCollection.Clear();
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlSearchOrder);
            if (doc.HasChildNodes)
            {
                XmlNode parentNode = doc.SelectSingleNode("searchFieldOrder");
                if (parentNode != null)
                {
                    if (parentNode.HasChildNodes)
                    {
                        foreach (XmlNode childEle in parentNode.ChildNodes)
                        {
                            if (childEle.Attributes["fieldValue"] != null && childEle.Attributes["fieldCoeOperator"] != null)
                            {
                                SearchField fld = new SearchField(Convert.ToInt32(childEle.Attributes["parentTableId"].Value), Convert.ToInt32(childEle.Attributes["fieldId"].Value), Convert.ToInt32(childEle.Attributes["tableId"].Value), childEle.Attributes["fieldCriteriaType"].Value,
                                    childEle.Attributes["fieldValue"].Value, childEle.Attributes["fieldCoeOperator"].Value);
                                searchFieldCollection.Add(fld);
                            }
                        }
                    }
                }
            }
        }
    }


    /// <summary>
    /// Search criteria field model for query tab field order
    /// </summary>
    public class SearchField
    {
        #region Variables
        int parentTableId;
        int tableId;
        int fieldId; 
        string fieldCriteriaType;
        string coeOperator;
        string fieldValue;
        #endregion

        #region Properties
        ///<summary>
        /// gets coe operator value in string format
        ///</summary>
        public string CoeOperator
        {
            get { return coeOperator; }
        }

        /// <summary>
        /// gets the field value
        /// </summary>
        public string FieldValue
        {
            get { return fieldValue; }
        }
        /// <summary>
        /// Gets the table id
        /// </summary>
        public int TableId
        {
            get { return tableId; }
        }

        /// <summary>
        /// Gets the field id from tableBO
        /// </summary>
        public int FieldId
        {
            get { return fieldId; }
        }

        /// <summary>
        /// Gets the parent table id of the selected field 
        /// </summary>
        public int ParentTableId
        {
            get { return parentTableId; }
        }

        /// <summary>
        /// Gets the type of query criteria field
        /// </summary>
        public string FieldCriteriaType
        {
            get
            {
                return fieldCriteriaType;
            }
        }

        #endregion

        /// <summary>
        /// Initializes an instance of the SearchField object with specified table id and field id
        /// </summary>
        /// <param name="tableId">table id of TableBO</param>
        /// <param name="fieldId">field id from TableBO</param>
        public SearchField(int parentTableId, int fieldId, int tableId, string fieldCriteriaType, string fldValue, string coeOperator)
        {
            this.tableId = tableId;
            this.fieldId = fieldId;
            this.parentTableId = parentTableId;
            this.fieldCriteriaType = fieldCriteriaType;
            this.fieldValue = fldValue;
            this.coeOperator = coeOperator;
        }
    }
}
