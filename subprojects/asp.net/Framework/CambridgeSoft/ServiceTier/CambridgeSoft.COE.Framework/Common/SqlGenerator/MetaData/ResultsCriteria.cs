using System;
using System.Xml;
using CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries;
using CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries.SelectItems;
using System.Collections.Generic;
using CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries.AggregateItems;
using CambridgeSoft.COE.Framework.Common.Utility;

namespace CambridgeSoft.COE.Framework.Common.SqlGenerator.MetaData
{
    /// <summary>
    /// Representation of the fields to display. Usaully loaded from an xml.
    /// </summary>
    public class ResultsCriteria
    {
        #region Variables
        private XmlDocument resultsCriteriaXML;
        [NonSerialized]
        private XmlNamespaceManager manager;
        private const string xmlNamespace = "COE.ResultsCriteria";
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes its members to its default values.
        /// </summary>
        public ResultsCriteria()
        {
            this.resultsCriteriaXML = new XmlDocument();
            manager = new XmlNamespaceManager(resultsCriteriaXML.NameTable);
            manager.AddNamespace(xmlNamespace, "COE.ResultsCriteria");
        }
        #endregion

        #region Methods
        /// <summary>
        /// Returns the xml representation of the dataview
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.resultsCriteriaXML.OuterXml;
        }

        /// <summary>
        /// Loads the  values of this class from an xml.
        /// </summary>
        /// <param name="resultsCriteriaXMLString">The schema xml in a string format.</param>
        public void LoadFromXML(string resultsCriteriaXMLString)
        {
            this.resultsCriteriaXML.LoadXml(resultsCriteriaXMLString);
        }

        /// <summary>
        /// Loads the  values of this class from an xml.
        /// </summary>
        /// <param name="resultsCriteriaXMLDocument">The schema xml in a XmlDocument format.</param>
        public void LoadFromXML(XmlDocument resultsCriteriaXMLDocument)
        {
            this.resultsCriteriaXML = (XmlDocument)resultsCriteriaXMLDocument.Clone();
        }

        /// <summary>
        /// Gets a list of Select Clause, each clause of the list is for a different query.
        /// </summary>
        /// <param name="dataViewXMLString">The database xml schema in a string format.</param>
        /// <returns>The list of Select Clauses.</returns>
        public SelectClause[] GetSelectClause(string dataViewXMLString)
        {
            DataView dataView = new DataView();
            dataView.LoadFromXML(dataViewXMLString);

            return this.GetSelectClause(dataView);
        }

        /// <summary>
        /// Gets a list of Select Clause, each clause of the list is for a different query.
        /// </summary>
        /// <param name="dataViewXMLDocument">The database xml schema in a XmlDocument format.</param>
        /// <returns>The list of Select Clauses.</returns>
        public SelectClause[] GetSelectClause(XmlDocument dataViewXMLDocument)
        {
            DataView dataView = new DataView();
            dataView.LoadFromXML(dataViewXMLDocument);

            return this.GetSelectClause(dataView);
        }

        /// <summary>
        /// Gets a list of Select Clause, each clause of the list is for a different query.
        /// </summary>
        /// <param name="dataView">The object that encapsulates the database schema.</param>
        /// <returns>The list of Select Clauses.</returns>
        public SelectClause[] GetSelectClause(DataView dataView)
        {
            XmlNodeList tableNodeList = resultsCriteriaXML.SelectNodes("//" + xmlNamespace + ":table", this.manager);

            SelectClause[] select = new SelectClause[tableNodeList.Count];

            // each node is a table node
            for (int i = 0; i < tableNodeList.Count; i++)
            {
                select[i] = new SelectClause();
                XmlNode tableNode = tableNodeList[i];
                //string tableName = dataView.GetTableName(int.Parse(tableNode.Attributes["id"].Value));
                XmlNodeList clauseList = tableNode.ChildNodes;
                foreach (XmlNode clauseNode in clauseList)
                {
                    if (clauseNode.NodeType != XmlNodeType.Comment)
                    {
                        SelectClauseItem item = SelectClauseFactory.CreateSelectClauseItem(clauseNode, dataView);

                        //CBOE-779
                        //Extra conditio i.e.  item.DataField != null is removed to fix the Issue CBOE-1026
                        //(REG: Records cannot be searched in CBOE Build 12.6.0.2505)
                        if (item != null)
                            select[i].AddItem(item);
                    }
                }
            }

            return select;
        }

        /// <summary>
        /// Gets the list of Order by clauses from the list of select clauses and the dataview.
        /// </summary>
        /// <param name="dataView">The dataview.</param>
        /// <param name="selectClauses">The list of select clauses.</param>
        /// <returns>The list of Order by clauses.</returns>
        public OrderByClause[] GetOrderByClause(DataView dataView, SelectClause[] selectClauses)
        {
            XmlNodeList tableNodeList = resultsCriteriaXML.SelectNodes("//" + xmlNamespace + ":table", this.manager);

            OrderByClause[] orderByClauses = null;
            if (tableNodeList.Count > 0)
            {
                orderByClauses = new OrderByClause[tableNodeList.Count];

                // each node is a table node
                for (int tableIndex = 0; tableIndex < tableNodeList.Count; tableIndex++)
                {
                    orderByClauses[tableIndex] = new OrderByClause();

                    XmlNode tableNode = tableNodeList[tableIndex];
                    //string tableName = dataView.GetTableName(int.Parse(tableNode.Attributes["id"].Value));

                    XmlNodeList clauseList = tableNode.ChildNodes;
                    for (int clauseIndex = 0; clauseIndex < clauseList.Count; clauseIndex++)
                    {
                        if (clauseList[clauseIndex].NodeType != XmlNodeType.Comment)
                        {
                            if (clauseList[clauseIndex].Attributes["orderById"] != null && clauseList[clauseIndex].Attributes["orderById"].Value.Trim() != "" && clauseList[clauseIndex].Attributes["orderById"].Value.Trim() != "0")
                            {
                                OrderByClauseItem item = new OrderByClauseItem();
                                item.Item = selectClauses[tableIndex].Items[clauseIndex];
                                item.OrderByID = int.Parse(clauseList[clauseIndex].Attributes["orderById"].Value.Trim());
                                if (clauseList[clauseIndex].Attributes["direction"] != null && clauseList[clauseIndex].Attributes["direction"].Value.Trim() != "")
                                    item.Direction = COEConvert.ToSortDirection(clauseList[clauseIndex].Attributes["direction"].Value);

                                orderByClauses[tableIndex].AddItem(item);
                            }
                        }
                    }
                    orderByClauses[tableIndex].Sort();
                }
            }

            return orderByClauses;
        }

        /// <summary>
        /// Gets all the table ids involved in the select clause
        /// </summary>
        /// <returns>The table ids involved in the select clause</returns>
        public List<int>[] GetTableIds(DataView dataView)
        {
            XmlNodeList tablesNodeList = resultsCriteriaXML.SelectNodes("//" + xmlNamespace + ":table", this.manager);
            List<int>[] results = new List<int>[tablesNodeList.Count];

            if (tablesNodeList.Count > 0)
            {
                for (int currentQueryIndex = 0; currentQueryIndex < tablesNodeList.Count; currentQueryIndex++)
                {
                    if (tablesNodeList[currentQueryIndex].ChildNodes.Count > 0)
                    {
                        results[currentQueryIndex] = new List<int>();

                        results[currentQueryIndex].Add(int.Parse(tablesNodeList[currentQueryIndex].Attributes["id"].Value));

                        XmlNodeList fieldNodes = tablesNodeList[currentQueryIndex].SelectNodes(".//" + xmlNamespace + ":*[@fieldId]", this.manager);
                        foreach (XmlNode currentFieldNode in fieldNodes)
                        {
                            try
                            {
                                if (currentFieldNode.NodeType != XmlNodeType.Comment && currentFieldNode.Attributes["fieldId"] != null)
                                {
                                    int toAddTable = dataView.GetParentTableId(currentFieldNode.Attributes["fieldId"].Value);

                                    if (!results[currentQueryIndex].Contains(toAddTable))
                                        results[currentQueryIndex].Add(toAddTable);
                                }
                            }
                            catch (Exception e)
                            {
                                string s = currentFieldNode.OuterXml.ToString();
                            }
                        }
                    }
                }
            }

            return results;
        }

        /// <summary>
        /// Gets the id of the main table of a query.
        /// </summary>
        /// <returns>The main table id for that query.</returns>
        public int GetMainTableId()
        {
            XmlNode tablesNode = resultsCriteriaXML.SelectSingleNode("//" + xmlNamespace + ":tables", this.manager);
            if (tablesNode != null && tablesNode.Attributes["basetable"] != null && tablesNode.Attributes["basetable"].Value != string.Empty)
                return int.Parse(tablesNode.Attributes["basetable"].Value);
            else
                return -1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="selectClauses"></param>
        /// <returns></returns>
        public GroupByClause[] GetGroupByClause(SelectClause[] selectClauses, bool isAggregate)
        {
            GroupByClause[] groupByClauses = null;

            if (selectClauses.Length > 0)
            {
                groupByClauses = new GroupByClause[selectClauses.Length];

                for (int selectIndex = 0; selectIndex < selectClauses.Length; selectIndex++)
                {
                    List<SelectClauseItem> items = new List<SelectClauseItem>();

                    foreach (SelectClauseItem selectItem in selectClauses[selectIndex].Items)
                    {
                        if (selectItem is SelectClauseAggregateFunction)
                        {
                            isAggregate = true;
                        }
                        else
                        {
                            items.Add(selectItem);
                        }
                    }

                    if (isAggregate)
                    {
                        groupByClauses[selectIndex] = new GroupByClause();
                        foreach (SelectClauseItem selectItem in items)
                        {
                            groupByClauses[selectIndex].AddItem(new GroupByClauseItem(selectItem));
                        }
                    }
                }

            }
            return groupByClauses;
        }
        #endregion
    }
}
