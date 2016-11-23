using System;
using System.Xml;
using System.Collections.Generic;
using CambridgeSoft.COE.Framework.Types.Exceptions;
using CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries.WhereItems;

namespace CambridgeSoft.COE.Framework.Common.SqlGenerator
{
    using System.Linq;

    /// <summary>
    /// Its responsibility is to build a QUERY. Usually taken info from 3 xmls, a search criteria, 
    /// a search result and a data view (a schema).
    /// </summary>
    public class QueryBuilder
    {
        #region Variables
        /// <summary>
        /// Object that encapsuletes the logic for the database schema.
        /// </summary>
        protected CambridgeSoft.COE.Framework.Common.SqlGenerator.MetaData.DataView dataView;
        /// <summary>
        /// Object that encapsuletes the logic for the results schema.
        /// </summary>
        protected CambridgeSoft.COE.Framework.Common.SqlGenerator.MetaData.ResultsCriteria resultsCriteria;
        /// <summary>
        /// Object that encapsuletes the logic for the search criterias.
        /// </summary>
        protected CambridgeSoft.COE.Framework.Common.SqlGenerator.MetaData.SearchCriteria searchCriteria;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes its members to its default values.
        /// </summary>
        public QueryBuilder()
        {
            this.dataView = new CambridgeSoft.COE.Framework.Common.SqlGenerator.MetaData.DataView();
            this.searchCriteria = new CambridgeSoft.COE.Framework.Common.SqlGenerator.MetaData.SearchCriteria();
            this.resultsCriteria = new CambridgeSoft.COE.Framework.Common.SqlGenerator.MetaData.ResultsCriteria();
        }

        /// <summary>
        /// Initializes its members from XMLs
        /// </summary>
        /// <param name="dataViewXmlDocument">The data view schema of the database in a XmlDocument.</param>
        /// <param name="searchCriteriaXmlDocument">The search criterias in a XmlDocument.</param>
        /// <param name="resultsCriteriaXmlDocument">The results in a XmlDocuemt.</param>
        public QueryBuilder(XmlDocument dataViewXmlDocument,
                            XmlDocument searchCriteriaXmlDocument,
                            XmlDocument resultsCriteriaXmlDocument)
        {
            this.dataView = new CambridgeSoft.COE.Framework.Common.SqlGenerator.MetaData.DataView();
            this.dataView.LoadFromXML(dataViewXmlDocument);
            this.resultsCriteria = new CambridgeSoft.COE.Framework.Common.SqlGenerator.MetaData.ResultsCriteria();
            this.resultsCriteria.LoadFromXML(resultsCriteriaXmlDocument);
            this.searchCriteria = new CambridgeSoft.COE.Framework.Common.SqlGenerator.MetaData.SearchCriteria();
            this.searchCriteria.LoadFromXML(searchCriteriaXmlDocument);
        }

        /// <summary>
        /// Initializes its members from XMLs
        /// </summary>
        /// <param name="dataViewXmlString">The xml data view schema of the database in a string.</param>
        /// <param name="searchCriteriaXmlString">The xml search criterias in a string.</param>
        /// <param name="resultsCriteriaXmlString">The xml results in a string.</param>
        public QueryBuilder(string dataViewXmlString,
                            string searchCriteriaXmlString,
                            string resultsCriteriaXmlString)
        {
            this.dataView = new CambridgeSoft.COE.Framework.Common.SqlGenerator.MetaData.DataView();
            this.dataView.LoadFromXML(dataViewXmlString);
            this.resultsCriteria = new CambridgeSoft.COE.Framework.Common.SqlGenerator.MetaData.ResultsCriteria();
            this.resultsCriteria.LoadFromXML(resultsCriteriaXmlString);
            this.searchCriteria = new CambridgeSoft.COE.Framework.Common.SqlGenerator.MetaData.SearchCriteria();
            this.searchCriteria.LoadFromXML(searchCriteriaXmlString);
        }

        /// <summary>
        /// Initialize from SQLGenerator.Metadata.Dataview
        /// </summary>
        /// <param name="dataViewXmlString">The xml data view schema of the database in a string.</param>
        /// <param name="searchCriteriaXmlString">The xml search criterias in a string.</param>
        /// <param name="resultsCriteriaXmlString">The xml results in a string.</param>
        public QueryBuilder(MetaData.DataView dataView,
                            string searchCriteriaXmlString,
                            string resultsCriteriaXmlString)
        {
            this.dataView = dataView;
            this.resultsCriteria = new CambridgeSoft.COE.Framework.Common.SqlGenerator.MetaData.ResultsCriteria();
            this.resultsCriteria.LoadFromXML(resultsCriteriaXmlString);
            this.searchCriteria = new CambridgeSoft.COE.Framework.Common.SqlGenerator.MetaData.SearchCriteria();
            this.searchCriteria.LoadFromXML(searchCriteriaXmlString);
        }


        /// <summary>
        /// Initialize from DataView, SearchCriteria, and ResultsCriteria objects
        /// </summary>
        /// <param name="dataViewXmlString">The xml data view schema of the database in a string.</param>
        /// <param name="searchCriteriaXmlString">The xml search criterias in a string.</param>
        /// <param name="resultsCriteriaXmlString">The xml results in a string.</param>
        public QueryBuilder(MetaData.DataView dataView,
                            MetaData.SearchCriteria searchCriteria,
                            MetaData.ResultsCriteria resultsCriteria)
        {
            this.dataView = dataView;
            this.resultsCriteria = resultsCriteria;
            this.searchCriteria = searchCriteria;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Build queries from xmls definitions.
        /// </summary>
        /// <param name="databaseType">The underlying database to generate queries.</param>
        /// <returns>A list of queries based on the xmls definitions.</returns>
        public Queries.Query[] BuildQuery(DBMSType databaseType)
        {
            List<int>[] tablesInSelect = resultsCriteria.GetTableIds(dataView);
            List<int> tablesInWhere = searchCriteria.GetTableIds(dataView);
            Dictionary<int, int> fieldsInLookupSearches = searchCriteria.GetLookupFields(dataView);

            Queries.Query[] resultQueries = new Queries.Query[tablesInSelect.Length];

            if(tablesInSelect.Length > 0)
            {
                Queries.SelectClause[] selectClauses = resultsCriteria.GetSelectClause(dataView);
                Queries.WhereClause whereClause = searchCriteria.GetWhereClause(dataView);
                Queries.GroupByClause[] groupByClause = resultsCriteria.GetGroupByClause(selectClauses, searchCriteria.ContainsAggregatedFunctions());
                Queries.OrderByClause[] orderByClause = resultsCriteria.GetOrderByClause(dataView, selectClauses);

                for(int i = 0; i < tablesInSelect.Length; i++)
                {
                    resultQueries[i] = new Queries.Query();
                    resultQueries[i].DataBaseType = databaseType;

                    List<int> currentQueryTables = new List<int>();

                    if(tablesInSelect[i] != null && tablesInSelect[i].Count > 0)
                    {
                        resultQueries[i].SetMainTable(dataView.GetTable(tablesInSelect[i][0]));
                        resultQueries[i].MainTableID = dataView.GetTable(tablesInSelect[i][0]).TableId;
                        currentQueryTables.AddRange(tablesInSelect[i]);
                    }
                    else
                    {
                        XmlDocument document = new XmlDocument();
                        document.LoadXml(resultsCriteria.ToString());

                        ResultsCriteria resultsCriteriaMessaging = new ResultsCriteria(document);
                        throw new SQLGeneratorException("ResultCriteria Table " + resultsCriteriaMessaging.Tables[i].Id + " does not contains columns");
                    }

                    foreach(int whereTableId in tablesInWhere)
                    {
                        if(!currentQueryTables.Contains(whereTableId))
                        {
                            currentQueryTables.Add(whereTableId);
                        }
                    }
                    
                    foreach(int lookupFieldId in fieldsInLookupSearches.Values)
                    {
                        int lookupTableId = dataView.GetParentTableId(lookupFieldId.ToString());
                        if(!currentQueryTables.Contains(lookupTableId))
                            currentQueryTables.Add(lookupTableId);
                    }

                    List<Relation> relationPath = new List<Relation>();
                    for(int currentTableIndex = 1; currentTableIndex < currentQueryTables.Count; currentTableIndex++)
                    {
                        if(currentQueryTables[0] != currentQueryTables[currentTableIndex])
                        {
                            List<Relation> relations = dataView.GetRelations(currentQueryTables[0], currentQueryTables[currentTableIndex]);

                            if(relations.Count == 0)
                            {
                                //It may happen there is a lookup
                                bool wasALookupTable = false;
                                foreach(int lookupFieldId in fieldsInLookupSearches.Values)
                                {
                                    int lookupTableId = dataView.GetParentTableId(lookupFieldId.ToString());
                                    if(lookupTableId == currentQueryTables[currentTableIndex])
                                    {
                                        wasALookupTable = true;
                                        // Create and add the relation for the lookup.
                                        foreach(int sourceFieldId in fieldsInLookupSearches.Keys)
                                        {
                                            if(fieldsInLookupSearches[sourceFieldId] == lookupFieldId)
                                            {
                                                Relation rel = new Relation();
                                                rel.LeftJoin = true;
                                                rel.InnerJoin = true;
                                                rel.Parent = dataView.GetField(sourceFieldId);
                                                rel.Child = dataView.GetField(fieldsInLookupSearches[sourceFieldId]);
                                                relationPath.Add(rel);
                                                break;
                                            }
                                        }
                                        break;
                                    }
                                }

                                if(!wasALookupTable)
                                {

                                    if (! searchCriteria.IsStructureListCriteriaTable(currentQueryTables[currentTableIndex]))
                                    {
                                        throw new SQLGeneratorException(string.Format("Can't join table {0} to table {1} - Missing dataview relations?", currentQueryTables[0], currentQueryTables[currentTableIndex]));
                                    }
                                }
                            }

                            //Force left outer join for aggregated searches.
                            foreach (WhereClauseItem wItem in whereClause.GetAggregatedWhereClauseItems())
                            {
                                Field dataField = null;
                                if (wItem is WhereClauseBinaryOperation)
                                    dataField = ((WhereClauseBinaryOperation)wItem).DataField;
                                else if (wItem is WhereClauseNAryOperation)
                                    dataField = ((WhereClauseNAryOperation)wItem).DataField;

                                if (dataField != null)
                                {
                                    foreach (Relation rel in relations)
                                    {
                                        if (rel.Child.Table.GetAlias() == dataField.Table.GetAlias())
                                        {
                                            rel.InnerJoin = false;
                                            rel.LeftJoin = true;
                                        }
                                    }
                                }
                            }
                            relationPath.AddRange(relations);
                        }
                    }

                    foreach(Relation currentRelation in relationPath)
                    {
                        resultQueries[i].AddJoinRelation(currentRelation);
                    }

                    resultQueries[i].SetSelectClause(selectClauses[i]);
                    resultQueries[i].SetOrderByClause(orderByClause[i]);
                    // Coverity Fix CID - 10856 (from local server)
                    if(groupByClause!= null && groupByClause[i] != null)
                        resultQueries[i].SetGroupByClause(groupByClause[i]);
                    resultQueries[i].SetWhereClause(whereClause.Clone());
                }
            }

            return resultQueries;
        }

        /// <summary>
        /// Build query from given table. Doesn't support lookup, group by and order by.
        /// </summary>
        /// <param name="databaseType"> The underlying database to generate queries. </param>
        /// <param name="tableId"> The table Id. </param>
        /// <returns> A list of queries based on the xmls definitions. </returns>
        public Queries.Query BuildTableQuery(DBMSType databaseType, int tableId)
        {
            // Get select clause for the given table
            var selectClause = resultsCriteria.GetTableSelectClause(dataView, tableId);

            // Cant't create query for a table if not existed in result criteria
            if (selectClause.Items.Count < 1)
            {
                return null;
            }

            // Set main talbe to base table to maintain relation
            int baseTableId = dataView.GetBaseTableId();

            var query = new Queries.Query { DataBaseType = databaseType, MainTableID = baseTableId };
            query.SetMainTable(dataView.GetTable(baseTableId));
            
            // Set select clause
            query.SetSelectClause(selectClause);

            // Set where clause
            var whereClause = searchCriteria.GetWhereClause(dataView);
            query.SetWhereClause(whereClause.Clone());


            // Tables will be included in query
            var queryTables = new List<int>();

            // Always add basetable id
            queryTables.Add(baseTableId);

            // Add current table
            if (!queryTables.Contains(tableId))
            {
                queryTables.Add(tableId);
            }

            // Add tables contain where clause
            List<int> tablesInWhere = searchCriteria.GetTableIds(dataView);
            foreach (int whereTableId in tablesInWhere)
            {
                if (!queryTables.Contains(whereTableId))
                {
                    queryTables.Add(whereTableId);
                }
            }

            // Get relation pathes from base table to each query table
            List<Relation> relationPath = new List<Relation>();
            for (int i = 1; i < queryTables.Count; i++)
            {
                if (baseTableId != queryTables[i])
                {
                    List<Relation> relations = dataView.GetRelations(baseTableId, queryTables[i], true);
                    relationPath.AddRange(relations);
                }
            }

            // Set join clause
            foreach (Relation r in relationPath)
            {
                query.AddJoinRelation(r);
            }

            // if contains lookup field search criteria need to add join relationship
            foreach (var item in searchCriteria.GetLookupFields(dataView))
            {
                var fieldId = item.Key;
                var lookupFieldId = item.Value;
                Relation relation = new Relation()
                {
                    Parent = dataView.GetField(fieldId),
                    Child = dataView.GetField(lookupFieldId)
                };

                query.AddJoinRelation(relation);
            }

            return query;
        }

        public Queries.Query[] BuildGetDataQuery(DBMSType databaseType, COEDataView coeDataView)
        {
            List<int>[] tablesInSelect = resultsCriteria.GetTableIds(dataView);
            List<int> tablesInWhere = coeDataView.Tables.Select(table => table.Id).ToList();

            Dictionary<int, int> fieldsInLookupSearches = searchCriteria.GetLookupFields(dataView);

            Queries.Query[] resultQueries = new Queries.Query[tablesInSelect.Length];

            if (tablesInSelect.Length > 0)
            {
                Queries.SelectClause[] selectClauses = resultsCriteria.GetSelectClause(dataView);
                Queries.WhereClause whereClause = searchCriteria.GetWhereClause(dataView);
                Queries.GroupByClause[] groupByClause = resultsCriteria.GetGroupByClause(selectClauses, searchCriteria.ContainsAggregatedFunctions());
                Queries.OrderByClause[] orderByClause = resultsCriteria.GetOrderByClause(dataView, selectClauses);

                for (int i = 0; i < tablesInSelect.Length; i++)
                {
                    resultQueries[i] = new Queries.Query();
                    resultQueries[i].DataBaseType = databaseType;

                    List<int> currentQueryTables = new List<int>();

                    if (tablesInSelect[i] != null && tablesInSelect[i].Count > 0)
                    {
                        resultQueries[i].SetMainTable(dataView.GetTable(tablesInSelect[i][0]));
                        resultQueries[i].MainTableID = dataView.GetTable(tablesInSelect[i][0]).TableId;
                        currentQueryTables.AddRange(tablesInSelect[i]);
                    }
                    else
                    {
                        XmlDocument document = new XmlDocument();
                        document.LoadXml(resultsCriteria.ToString());

                        ResultsCriteria resultsCriteriaMessaging = new ResultsCriteria(document);
                        throw new SQLGeneratorException("ResultCriteria Table " + resultsCriteriaMessaging.Tables[i].Id + " does not contains columns");
                    }

                    foreach (int whereTableId in tablesInWhere)
                    {
                        if (!currentQueryTables.Contains(whereTableId))
                        {
                            currentQueryTables.Add(whereTableId);
                        }
                    }

                    foreach (int lookupFieldId in fieldsInLookupSearches.Values)
                    {
                        int lookupTableId = dataView.GetParentTableId(lookupFieldId.ToString());
                        if (!currentQueryTables.Contains(lookupTableId))
                            currentQueryTables.Add(lookupTableId);
                    }

                    List<Relation> relationPath = new List<Relation>();
                    for (int currentTableIndex = 1; currentTableIndex < currentQueryTables.Count; currentTableIndex++)
                    {
                        if (currentQueryTables[0] != currentQueryTables[currentTableIndex])
                        {
                            List<Relation> relations = dataView.GetRelations(currentQueryTables[0], currentQueryTables[currentTableIndex], true);

                            if (relations.Count == 0)
                            {
                                //It may happen there is a lookup
                                bool wasALookupTable = false;
                                foreach (int lookupFieldId in fieldsInLookupSearches.Values)
                                {
                                    int lookupTableId = dataView.GetParentTableId(lookupFieldId.ToString());
                                    if (lookupTableId == currentQueryTables[currentTableIndex])
                                    {
                                        wasALookupTable = true;
                                        // Create and add the relation for the lookup.
                                        foreach (int sourceFieldId in fieldsInLookupSearches.Keys)
                                        {
                                            if (fieldsInLookupSearches[sourceFieldId] == lookupFieldId)
                                            {
                                                Relation rel = new Relation();
                                                rel.LeftJoin = true;
                                                rel.InnerJoin = true;
                                                rel.Parent = dataView.GetField(sourceFieldId);
                                                rel.Child = dataView.GetField(fieldsInLookupSearches[sourceFieldId]);
                                                relationPath.Add(rel);
                                                break;
                                            }
                                        }
                                        break;
                                    }
                                }

                                if (!wasALookupTable)
                                {

                                    if (!searchCriteria.IsStructureListCriteriaTable(currentQueryTables[currentTableIndex]))
                                    {
                                        throw new SQLGeneratorException(string.Format("Can't join table {0} to table {1} - Missing dataview relations?", currentQueryTables[0], currentQueryTables[currentTableIndex]));
                                    }
                                }
                            }

                            //Force left outer join for aggregated searches.
                            foreach (WhereClauseItem wItem in whereClause.GetAggregatedWhereClauseItems())
                            {
                                Field dataField = null;
                                if (wItem is WhereClauseBinaryOperation)
                                    dataField = ((WhereClauseBinaryOperation)wItem).DataField;
                                else if (wItem is WhereClauseNAryOperation)
                                    dataField = ((WhereClauseNAryOperation)wItem).DataField;

                                if (dataField != null)
                                {
                                    foreach (Relation rel in relations)
                                    {
                                        if (rel.Child.Table.GetAlias() == dataField.Table.GetAlias())
                                        {
                                            rel.InnerJoin = false;
                                            rel.LeftJoin = true;
                                        }
                                    }
                                }
                            }
                            relationPath.AddRange(relations);
                        }
                    }

                    foreach (Relation currentRelation in relationPath)
                    {
                        resultQueries[i].AddJoinRelation(currentRelation);
                    }

                    resultQueries[i].SetSelectClause(selectClauses[i]);
                    resultQueries[i].SetOrderByClause(orderByClause[i]);
                    // Coverity Fix CID - 10856 (from local server)
                    if (groupByClause != null && groupByClause[i] != null)
                        resultQueries[i].SetGroupByClause(groupByClause[i]);
                    resultQueries[i].SetWhereClause(whereClause.Clone());
                }
            }

            return resultQueries;
        }

        /// <summary>
        /// Build queries string from xmls definitions.
        /// </summary>
        /// <param name="databaseType">The underlying database to generate queries.</param>
        /// <returns>A list of string queries based on the xmls definitions.</returns>
        public string[] GetQueryString(DBMSType databaseType)
        {
            Queries.Query[] queries = this.BuildQuery(databaseType);
            string[] stringQueries = new string[queries.Length];

            for(int i = 0; i < queries.Length; i++)
            {
                stringQueries[i] = queries[i].ToString();
            }
            return stringQueries;
        }



        /// <summary>
        /// Build queries from xmls definitions.
        /// </summary>
        /// <param name="databaseType">The underlying database to generate queries.</param>
        /// <param name="fieldsInLookupSearches">The lookup fields dictionary</param>
        /// <returns>A list of queries based on the xmls definitions.</returns>
        public Queries.Query[] BuildQuery(DBMSType databaseType, Dictionary<int, int> fieldsInLookupSearches=null)
        {
            List<int>[] tablesInSelect = resultsCriteria.GetTableIds(dataView);
            List<int> tablesInWhere = searchCriteria.GetTableIds(dataView);
            if (fieldsInLookupSearches == null)
                fieldsInLookupSearches = searchCriteria.GetLookupFields(dataView);

            Queries.Query[] resultQueries = new Queries.Query[tablesInSelect.Length];

            if (tablesInSelect.Length > 0)
            {
                Queries.SelectClause[] selectClauses = resultsCriteria.GetSelectClause(dataView);
                Queries.WhereClause whereClause = searchCriteria.GetWhereClause(dataView);
                Queries.GroupByClause[] groupByClause = resultsCriteria.GetGroupByClause(selectClauses, searchCriteria.ContainsAggregatedFunctions());
                Queries.OrderByClause[] orderByClause = resultsCriteria.GetOrderByClause(dataView, selectClauses);

                for (int i = 0; i < tablesInSelect.Length; i++)
                {
                    resultQueries[i] = new Queries.Query();
                    resultQueries[i].DataBaseType = databaseType;

                    List<int> currentQueryTables = new List<int>();

                    if (tablesInSelect[i] != null && tablesInSelect[i].Count > 0)
                    {
                        resultQueries[i].SetMainTable(dataView.GetTable(tablesInSelect[i][0]));
                        resultQueries[i].MainTableID = dataView.GetTable(tablesInSelect[i][0]).TableId;
                        currentQueryTables.AddRange(tablesInSelect[i]);
                    }
                    else
                    {
                        XmlDocument document = new XmlDocument();
                        document.LoadXml(resultsCriteria.ToString());

                        ResultsCriteria resultsCriteriaMessaging = new ResultsCriteria(document);
                        throw new SQLGeneratorException("ResultCriteria Table " + resultsCriteriaMessaging.Tables[i].Id + " does not contains columns");
                    }

                    foreach (int whereTableId in tablesInWhere)
                    {
                        if (!currentQueryTables.Contains(whereTableId))
                        {
                            currentQueryTables.Add(whereTableId);
                        }
                    }

                    foreach (int lookupFieldId in fieldsInLookupSearches.Values)
                    {
                        int lookupTableId = dataView.GetParentTableId(lookupFieldId.ToString());
                        if (!currentQueryTables.Contains(lookupTableId))
                            currentQueryTables.Add(lookupTableId);
                    }

                    List<Relation> relationPath = new List<Relation>();
                    for (int currentTableIndex = 1; currentTableIndex < currentQueryTables.Count; currentTableIndex++)
                    {
                        if (currentQueryTables[0] != currentQueryTables[currentTableIndex])
                        {
                            List<Relation> relations = dataView.GetRelations(currentQueryTables[0], currentQueryTables[currentTableIndex]);

                            if (relations.Count == 0)
                            {
                                //It may happen there is a lookup
                                bool wasALookupTable = false;
                                foreach (int lookupFieldId in fieldsInLookupSearches.Values)
                                {
                                    int lookupTableId = dataView.GetParentTableId(lookupFieldId.ToString());
                                    if (lookupTableId == currentQueryTables[currentTableIndex])
                                    {
                                        wasALookupTable = true;
                                        // Create and add the relation for the lookup.
                                        foreach (int sourceFieldId in fieldsInLookupSearches.Keys)
                                        {
                                            if (fieldsInLookupSearches[sourceFieldId] == lookupFieldId)
                                            {
                                                Relation rel = new Relation();
                                                rel.LeftJoin = true;
                                                rel.InnerJoin = true;
                                                rel.Parent = dataView.GetField(sourceFieldId);
                                                rel.Child = dataView.GetField(fieldsInLookupSearches[sourceFieldId]);
                                                relationPath.Add(rel);
                                                break;
                                            }
                                        }
                                        break;
                                    }
                                }

                                if (!wasALookupTable)
                                {

                                    if (!searchCriteria.IsStructureListCriteriaTable(currentQueryTables[currentTableIndex]))
                                    {
                                        throw new SQLGeneratorException(string.Format("Can't join table {0} to table {1} - Missing dataview relations?", currentQueryTables[0], currentQueryTables[currentTableIndex]));
                                    }
                                }
                            }

                            //Force left outer join for aggregated searches.
                            foreach (WhereClauseItem wItem in whereClause.GetAggregatedWhereClauseItems())
                            {
                                Field dataField = null;
                                if (wItem is WhereClauseBinaryOperation)
                                    dataField = ((WhereClauseBinaryOperation)wItem).DataField;
                                else if (wItem is WhereClauseNAryOperation)
                                    dataField = ((WhereClauseNAryOperation)wItem).DataField;

                                if (dataField != null)
                                {
                                    foreach (Relation rel in relations)
                                    {
                                        if (rel.Child.Table.GetAlias() == dataField.Table.GetAlias())
                                        {
                                            rel.InnerJoin = false;
                                            rel.LeftJoin = true;
                                        }
                                    }
                                }
                            }
                            relationPath.AddRange(relations);
                        }
                    }

                    foreach (Relation currentRelation in relationPath)
                    {
                        resultQueries[i].AddJoinRelation(currentRelation);
                    }

                    resultQueries[i].SetSelectClause(selectClauses[i]);
                    resultQueries[i].SetOrderByClause(orderByClause[i]);
                    // Coverity Fix CID - 10493 (from local server)
                    if (groupByClause != null && groupByClause[i] != null)
                        resultQueries[i].SetGroupByClause(groupByClause[i]);
                    resultQueries[i].SetWhereClause(whereClause.Clone());
                }
            }

            return resultQueries;
        }

        /// <summary>
        /// Get lookup fields list those are used for searching and mapping with lookup tables
        /// </summary>
        /// <param name="dataView">The underlying dataview to find lookup fields.</param>
        /// <returns>A list of lookup fields (field id and lookup field id).</returns>
        public static Dictionary<int, int> GetLookupFields(COEDataView dataView)
        {
            Dictionary<int, int> fieldsInLookupSearches = new Dictionary<int, int>();
            foreach (COEDataView.DataViewTable dvTable in dataView.Tables)
            {
                foreach (COEDataView.Field fld in dvTable.Fields)
                {
                    if (fld.LookupFieldId > 0)
                    {
                        fieldsInLookupSearches.Add(fld.Id, fld.LookupFieldId);
                    }
                }
            }
            if (fieldsInLookupSearches.Count > 0)
                return fieldsInLookupSearches;
            else
                return null;
        }


        #endregion
    }
}
