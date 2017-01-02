using System;
using System.Xml;
using System.Collections;
using CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries;
using CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries.WhereItems;
using System.Collections.Generic;
using CambridgeSoft.COE.Framework.Common.SqlGenerator.Utils;

namespace CambridgeSoft.COE.Framework.Common.SqlGenerator.MetaData {
    /// <summary>
    /// Representation of a search criteria to generate a query. Usaully loaded from an xml.
    /// </summary>
    public class SearchCriteria {
        #region Variables
        /// <summary>
        /// The source xml containing the search criteria.
        /// </summary>
        private XmlDocument searchCriteriaXML;
        [NonSerialized]
        private XmlNamespaceManager manager;
        private const string xmlNamespace = "COE";
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes its members to its default values.
        /// </summary>
        public SearchCriteria() {
            this.searchCriteriaXML = new XmlDocument();
            manager = new XmlNamespaceManager(searchCriteriaXML.NameTable);
            manager.AddNamespace(xmlNamespace, "COE.SearchCriteria");
        }
        #endregion

        #region Methods
        /// <summary>
        /// returns the xml representation of the searchCriteria
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            return this.searchCriteriaXML.OuterXml;
        }
        /// <summary>
        /// Loads the  values of this class from an xml.
        /// </summary>
        /// <param name="searchCriteriaXMLString">The schema xml in a string format.</param>
        public void LoadFromXML(string searchCriteriaXMLString) {
            XmlDocument xmlSearchCriteria = new XmlDocument();
            xmlSearchCriteria.LoadXml(searchCriteriaXMLString);
            LoadFromXML(xmlSearchCriteria);
        }

        /// <summary>
        /// Loads the  values of this class from an xml.
        /// </summary>
        /// <param name="searchCriteriaXMLDocument">The schema in a xml format.</param>
        public void LoadFromXML(XmlDocument searchCriteriaXMLDocument) {
            //this.searchCriteriaXML = (XmlDocument) searchCriteriaXMLDocument.Clone();
            this.searchCriteriaXML = XmlTranslation.Transform(searchCriteriaXMLDocument);
        }

        /// <summary>
        /// Gets a where clause from the underlying xml definition.
        /// </summary>
        /// <param name="dataViewXMLString">The database xml schema in a string format.</param>
        /// <returns>The Where Clause.</returns>
        public WhereClause GetWhereClause(string dataViewXMLString) {
            DataView dataView = new DataView();
            dataView.LoadFromXML(dataViewXMLString);

            return GetWhereClause(dataView);
        }

        /// <summary>
        /// Gets a where clause from the underlying xml definition.
        /// </summary>
        /// <param name="dataViewXMLDocument">The database xml schema in a XmlDocument format.</param>
        /// <returns>The Where Clause.</returns>
        public WhereClause GetWhereClause(XmlDocument dataViewXMLDocument) {
            DataView dataView = new DataView();
            dataView.LoadFromXML(dataViewXMLDocument);

            return GetWhereClause(dataView);
        }

        /// <summary>
        /// Gets a where clause from the underlying xml definition.
        /// </summary>
        /// <param name="dataView">The object that encapsulates the database schema.</param>
        /// <returns>The Where Clause.</returns>
        public WhereClause GetWhereClause(DataView dataView)
        {
            WhereClause where = new WhereClause();

            XmlNode managerNode = searchCriteriaXML.SelectSingleNode("//" + xmlNamespace + ":searchCriteria", this.manager);   // Coverity Fix CID - 11727 ASV
            if (managerNode != null)
            {
                XmlNodeList criteriaNodeList = managerNode.ChildNodes;

                // Coverity Fix CID - 11727
                if (criteriaNodeList != null)
                {
                    foreach (XmlNode criteriaNode in criteriaNodeList)
                    {
                        if (criteriaNode.NodeType == XmlNodeType.Element)
                        {
                            var clauseItem = WhereClauseFactory.CreateWhereClause(dataView, criteriaNode);
                            if (clauseItem != null)
                            {
                                where.AddItem(clauseItem);
                            }
                        }
                    }
                }
            }
            return where;
        }

        /// <summary>
        /// Gets all the table ids involved in the select clause
        /// Overload which takes dataView as parameter
        /// The parameter is superflous.  It is here for backwards compatibility
        /// </summary>
        /// <returns>The table ids involved in the select clause</returns>
        public List<int> GetTableIds(DataView dataView) {
            return GetTableIds();
        }

        /// <summary>
        /// Gets all the table ids involved in the select clause
        /// </summary>
        /// <returns>The table ids involved in the select clause</returns>
        public List<int> GetTableIds()
        {
            List<int> results = new List<int>();

            XmlNodeList nodeList = this.searchCriteriaXML.SelectNodes("//" + xmlNamespace + ":searchCriteriaItem", this.manager);

            foreach (XmlNode node in nodeList)
            {
                int parentTableID = int.Parse(node.Attributes["tableid"].Value);

                // Ignore the NONE operator fields.
                if (parentTableID > 0 &&
                    !results.Contains(parentTableID) &&
                    !XmlTranslation.IsNoneOperator(node.FirstChild))
                {
                    results.Add(parentTableID);
                }
            }

            return results;
        }

        /// <summary>
        /// If there are lookup fields specified to be used to search, a list of pairs is returned indicating the relationships between the 
        /// source field and the lookup field id.
        /// </summary>
        /// <param name="dataView">The dataview being used</param>
        /// <returns>A dictionary of int,int, where the key is the sourcefieldid and the value the lookupfieldid.</returns>
        public Dictionary<int, int> GetLookupFields(DataView dataView)
        {
            Dictionary<int, int> result = new Dictionary<int, int>();
            
            XmlNodeList nodeList = searchCriteriaXML.SelectNodes("//" + xmlNamespace + ":searchCriteriaItem", this.manager);
            
            foreach(XmlNode node in nodeList)
            {
                bool searchLookupById = node.Attributes["searchLookupByID"] == null || string.IsNullOrEmpty(node.Attributes["searchLookupByID"].Value) || (node.Attributes["searchLookupByID"].Value.ToLower() != "false" && node.Attributes["searchLookupByID"].Value.ToLower() != "0");
                if (!searchLookupById)
                {
                    int fieldId = int.Parse(node.Attributes["fieldid"].Value);
                    IColumn column = dataView.GetColumn(fieldId);
                    //When search with structure lookup Molweight and Formula FieldId exists in 2 places
                    //Added the condition to resolve the key already exist exception.
                    if (column is Lookup &&
                        !result.ContainsKey(((Lookup)column).FieldId) &&
                        !XmlTranslation.IsNoneOperator(node))
                    {
                        result.Add(((Lookup)column).FieldId, ((Lookup)column).LookupFieldId);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Indicates if a criteria has to be aggregated while searching, by using the having clause.
        /// </summary>
        /// <returns></returns>
        public bool ContainsAggregatedFunctions()
        {
            XmlNodeList nodeList = searchCriteriaXML.SelectNodes("//" + xmlNamespace + ":searchCriteriaItem[@aggregateFunctionName != '']", this.manager);
            return nodeList.Count > 0;
        }


        public bool IsStructureListCriteriaTable(int tableId)
        {
            bool rc = false;
            XmlNodeList nodeList = searchCriteriaXML.SelectNodes("//" + xmlNamespace + ":structureListCriteria", this.manager);
            if (nodeList.Count == 1)
            {
                string searchCritriaTableId = nodeList[0].ParentNode.Attributes["tableid"].Value;
                if (searchCritriaTableId.Length > 0 && searchCritriaTableId == tableId.ToString())
                {
                    rc = true;
                }
            }
            return rc;
        }



        #endregion
    }
}
