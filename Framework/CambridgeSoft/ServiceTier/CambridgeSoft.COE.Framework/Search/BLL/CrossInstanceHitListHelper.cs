// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CrossInstanceHitListHelper.cs" company="PerkinElmer Inc.">
//   Copyright (c) 2013 PerkinElmer Inc.,
//   940 Winter Street, Waltham, MA 02451.
//   All rights reserved.
//   This software is the confidential and proprietary information
//   of PerkinElmer Inc. ("Confidential Information"). You shall not
//   disclose such Confidential Information and may not use it in any way,
//   absent an express written license agreement between you and PerkinElmer Inc.
//   that authorizes such use.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CambridgeSoft.COE.Framework.COESearchService
{
    using CambridgeSoft.COE.Framework.COEConfigurationService;
    using CambridgeSoft.COE.Framework.COEHitListService;
    using CambridgeSoft.COE.Framework.COESearchService.Processors;
    using CambridgeSoft.COE.Framework.Common;
    using CambridgeSoft.COE.Framework.Common.SqlGenerator;
    using CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries;
    using CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries.WhereItems;
    using Oracle.DataAccess.Client;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// A helper class provide methods for generating hit list for cross instance query.
    /// </summary>
    public class CrossInstanceHitListHelper
    {
        /// <summary>
        /// Check if tables in dataview are all from same instance
        /// </summary>
        /// <param name="dataView">The data view to judge</param>
        /// <returns>true for single instance, else return false</returns>
        public static bool IsSingleInstanceDataView(COEDataView dataView)
        {
            string instanceName = ConfigurationUtilities.GetInstanceNameByDatabaseName(dataView.Tables[0].Database);
            return dataView.Tables.All(table => ConfigurationUtilities.GetInstanceNameByDatabaseName(table.Database) == instanceName);
        }

        /// <summary>
        /// Get new child hit list id from sequence on the specified instance
        /// </summary>
        /// <param name="dal">the DAL for the database instance</param>
        /// <returns>new child hit list id</returns>
        public static int GetNewChildHitListId(DAL dal)
        {
            // Get CHILDHITLISTID
            string sql = "SELECT " + HitListUtilities.GetChildHitListIDSequenceName(dal.DALManager.DatabaseData.Owner) + ".NEXTVAL as HITLISTID FROM DUAL";
            DbCommand dbCommand = dal.DALManager.Database.GetSqlStringCommand(sql);
            return Convert.ToInt32(dal.DALManager.Database.ExecuteScalar(dbCommand));
        }

        /// <summary>
        /// Insert a child hitlist record into TEMPCHILDHITLISTID table
        /// </summary>
        /// <param name="dal">the DAL for the database instance</param>
        /// <param name="hitListId">the query hit list id</param>
        /// <param name="tableId">id of the table to generate hit list</param>
        /// <param name="childHitListId"> The table Hit List Id.</param>
        public static void InsertChildHitListRecord(DAL dal, int hitListId, int tableId, int childHitListId)
        {
            // Generate SQL string
            string sql = "INSERT INTO " + HitListUtilities.GetTempChildHitListIdTableName(dal.DALManager.DatabaseData.Owner) +
                        "(HITLISTID,TABLEID,CHILDHITLISTID) VALUES " +
                        "(" + hitListId +
                        " ,  " + tableId +
                        " ,  " + childHitListId + " )";

            // Eecute command
            try
            {
                dal.DALManager.BeginTransaction();

                DbCommand dbCommand = dal.DALManager.Database.GetSqlStringCommand(sql);
                int recordsAffected = dal.DALManager.ExecuteNonQuery(dbCommand);

                // Check result
                if (recordsAffected != 1)
                {
                    throw new Exception("Failed to insert COETEMPCHILDHITLISTID");
                }

                dal.DALManager.CommitTransaction();
            }
            catch (Exception e)
            {
                dal.DALManager.RollbackTransaction();
                throw e;
            }
        }

        #region Memory join for datalytix 6.2

        /// <summary>
        /// Generate joined table using memory join
        /// </summary>
        /// <param name="searchCriteria">search criteria</param>
        /// <param name="resultCriteria">result criteria</param>
        /// <param name="dataView">coe data view</param>
        /// <param name="manager">search manager</param>
        /// <returns>joined table data set for specific table id</returns>
        public static Dictionary<int, DataSet> GenerateJoinedTable(SearchCriteria searchCriteria, ResultsCriteria resultCriteria, COEDataView dataView, SearchManager manager)
        {
            List<int> tables = new List<int>();

            List<int> needToJoinTableIDs = new List<int>();

            Dictionary<int, SearchCriteria> multipleSearchCriteria = new Dictionary<int, SearchCriteria>();

            GenerateQueries(tables, needToJoinTableIDs, multipleSearchCriteria, searchCriteria, resultCriteria, dataView);

            Dictionary<int, DataSet> multipleDataSet = new Dictionary<int, DataSet>();

            Task[] tasks = new Task[tables.Count];
            var exceptionList = new List<Exception>();
            for (var i = 0; i < tables.Count; i++)
            {
                var tableId = tables[i];
                tasks[i] = Task.Factory.StartNew(() =>
                {
                    try
                    {
                        //Table t = query.GetMainTable() as Table;
                        SearchCriteria sc = new SearchCriteria();
                        if (multipleSearchCriteria.ContainsKey(tableId))
                        {
                            sc = multipleSearchCriteria[tableId];
                        }
                        var coeTable = dataView.Tables.getById(tableId);
                        var dataSet = GetDataSet(sc, dataView, coeTable, manager);
                        multipleDataSet[tableId] = dataSet;
                    }
                    catch (Exception e)
                    {
                        exceptionList.Add(e);
                    }
                });
            }

            Task.WaitAll(tasks);

            if (exceptionList.Count != 0)
            {
                var errorMessage = new StringBuilder("One or more errors occurred.");
                foreach (var e in exceptionList)
                {
                    errorMessage.AppendLine(e.Message);
                }

                throw new Exception(errorMessage.ToString());
            }

            var emptyCheck = multipleDataSet.Select(d => d.Value.Tables.Count == 0);

            if (emptyCheck.Contains(true))
            {
                // No hit list
                return null;
            }

            var notOnlyCritetiaOnBaseTable = searchCriteria.Items.Exists(item => ((CambridgeSoft.COE.Framework.Common.SearchCriteria.SearchCriteriaItem)item).TableId != dataView.Basetable);

            if (searchCriteria.Items.Count == 0 || !notOnlyCritetiaOnBaseTable)
            {
                JoinNoCriteria(dataView, multipleDataSet, dataView.Basetable);
            }
            else
            {
                var joinedTable = JoinWithCriteria(dataView, multipleDataSet, dataView.Basetable, searchCriteria);

                foreach (var tableID in needToJoinTableIDs)
                {
                    DataTable dataTable = new DataTable(dataView.GetTableAliasNameById(tableID));

                    var pk_name = dataView.GetTableHitListField(tableID).Alias;

                    DataColumn column = new DataColumn(pk_name);
                    dataTable.Columns.Add(column);
                    var rowIndex = joinedTable.Columns.IndexOf(tableID + "." + pk_name);
                    if (rowIndex == -1)
                    {
                        rowIndex = joinedTable.Columns.IndexOf(pk_name);
                    }

                    var pkList = new List<object>();

                    for (var i = 0 ; i < joinedTable.Rows.Count; i++)
                    {
                        DataRow row = joinedTable.Rows[i];
                 
                        var value = row.ItemArray[rowIndex];
                        if (value != null && !value.Equals(string.Empty))
                        {
                            pkList.Add(value);
                            
                        }
                    }

                    var distinctPkList = pkList.Distinct();

                    foreach(var value in distinctPkList)
                    {
                        var newRow = dataTable.NewRow();
                        newRow.ItemArray = new[] { value };
                        dataTable.Rows.Add(newRow);
                    }

                    dataTable.AcceptChanges();
                    multipleDataSet[tableID].Tables.Clear();
                    multipleDataSet[tableID].Tables.Add(dataTable);
                    multipleDataSet[tableID].AcceptChanges();
                }
            }

            return multipleDataSet;
        }

        private static DataSet GetDataSet(SearchCriteria searchCriteria, COEDataView dataView, COEDataView.DataViewTable table, SearchManager manager)
        {
            DataSet dataSet = null;
            var instanceName = ConfigurationUtilities.GetInstanceNameByDatabaseName(table.Database);
            DALFactory dalFactory = new DALFactory();
            dalFactory.SetAllConfigDataExceptDatabase(instanceName, "COESearch", COEAppName.Get());
            var instanceDAL = dalFactory.GetDAL<DAL>(ConfigurationUtilities.GetDatabaseGlobalUser(table.Database), true);

            // pre process

            try
            {
                instanceDAL.DALManager.BeginTransaction();

                // Get processors for all search criteria
                List<SearchProcessor> processors = manager.GetSearchProcessorList(searchCriteria.Items, dataView, instanceDAL);

                var query = GenerateQuery(table, dataView, searchCriteria);

                // for finally clause
                try
                {
                    // Update query by each processor

                    foreach (SearchProcessor processor in processors)
                    {
                        processor.Process(query);
                    }

                    DbCommand dbCommand = instanceDAL.DALManager.Database.GetSqlStringCommand(query.ToString());

                    for (int i = 0; i < query.ParamValues.Count; i++)
                    {
                        OracleParameter param = new OracleParameter(instanceDAL.DALManager.BuildSqlStringParameterName(i.ToString()), query.ParamValues[i].Val);
                        param.DbType = query.ParamValues[i].Type;
                        dbCommand.Parameters.Add(param);
                    }

                    dataSet = instanceDAL.DALManager.ExecuteDataSet(dbCommand);
                }
                finally
                {
                    // always run post process

                    foreach (SearchProcessor processor in processors)
                    {
                        processor.PostProcess(instanceDAL);
                    }
                }

                instanceDAL.DALManager.CommitTransaction();
            }
            catch (Oracle.DataAccess.Client.OracleException ex)
            {
                // Translate OracleException to Exception since client side 
                // maybe can't recognize OracleException class
                instanceDAL.DALManager.RollbackTransaction();
                throw new Exception(string.Format(ex.ToString()));
            }

            return dataSet;
        }

        private static void GenerateQueries(List<int> tables, List<int> needToJoinTableIDs, Dictionary<int, SearchCriteria> multipleSearchCriteria, SearchCriteria searchCriteria, ResultsCriteria resultCriteria, COEDataView dataView)
        {
            foreach (var item in searchCriteria.Items)
            {
                CambridgeSoft.COE.Framework.Common.SearchCriteria.SearchCriteriaItem s = item as CambridgeSoft.COE.Framework.Common.SearchCriteria.SearchCriteriaItem;
                if (!needToJoinTableIDs.Contains(s.TableId))
                {
                    needToJoinTableIDs.Add(s.TableId);
                }
            }

            foreach (var t in resultCriteria.Tables)
            {
                if (!needToJoinTableIDs.Contains(t.Id))
                {
                    needToJoinTableIDs.Add(t.Id);
                }
            }

            for (var i = 0; i < needToJoinTableIDs.Count; i++)
            {
                var id = needToJoinTableIDs[i];
                var result = from relationship in dataView.Relationships where relationship.Child == id select relationship.Parent;
                foreach (var pId in result)
                {
                    if (!needToJoinTableIDs.Contains(pId))
                    {
                        needToJoinTableIDs.Add(pId);
                    }
                }
            }

            if (searchCriteria.Items.Count == 0)
            {
                tables.AddRange(needToJoinTableIDs);
            }
            else
            {
                foreach (var tableID in needToJoinTableIDs)
                {
                    SearchCriteria s = new SearchCriteria();
                    multipleSearchCriteria[tableID] = s;
                }

                foreach (var item in searchCriteria.Items)
                {
                    CambridgeSoft.COE.Framework.Common.SearchCriteria.SearchCriteriaItem s = item as CambridgeSoft.COE.Framework.Common.SearchCriteria.SearchCriteriaItem;
                    if (!multipleSearchCriteria[s.TableId].Items.Contains(s))
                    {
                        multipleSearchCriteria[s.TableId].Items.Add(s);
                    }

                    foreach (var relationship in dataView.Relationships)
                    {
                        if (relationship.ParentKey == s.FieldId && multipleSearchCriteria.ContainsKey(relationship.Child) && !multipleSearchCriteria[relationship.Child].Items.Contains(s))
                        {
                            var copySearchCriteria = s.Clone() as CambridgeSoft.COE.Framework.Common.SearchCriteria.SearchCriteriaItem;
                            copySearchCriteria.FieldId = relationship.ChildKey;
                            copySearchCriteria.TableId = relationship.Child;
                            multipleSearchCriteria[relationship.Child].Items.Add(copySearchCriteria);
                        }

                        if (relationship.ChildKey == s.FieldId && multipleSearchCriteria.ContainsKey(relationship.Parent) && !multipleSearchCriteria[relationship.Parent].Items.Contains(s))
                        {
                            var copySearchCriteria = s.Clone() as CambridgeSoft.COE.Framework.Common.SearchCriteria.SearchCriteriaItem;
                            copySearchCriteria.FieldId = relationship.ParentKey;
                            copySearchCriteria.TableId = relationship.Parent;
                            multipleSearchCriteria[relationship.Parent].Items.Add(copySearchCriteria);
                        }
                    }
                }

                tables.AddRange(needToJoinTableIDs);
            }
        }

        private static Query GenerateQuery(CambridgeSoft.COE.Framework.Common.COEDataView.DataViewTable table, COEDataView dataView, SearchCriteria sc)
        {
            ResultsCriteria resultsCriteria = new ResultsCriteria();
            ResultsCriteria.ResultsCriteriaTable resultColumn = new ResultsCriteria.ResultsCriteriaTable();
            resultColumn.Id = table.Id;
            resultColumn.Criterias = new List<ResultsCriteria.IResultsCriteriaBase>();

            // Create result criteria for querytable unique field
            var hitListFieldId = dataView.GetTableHitListFieldId(table.Id);
            ResultsCriteria.Field pk = (hitListFieldId == 0) ? new ResultsCriteria.RowId() : new ResultsCriteria.Field();
            pk.Id = hitListFieldId;

            var primaryKey = int.Parse(table.PrimaryKey);
            resultColumn.Criterias.Add(pk);

            foreach (var relationship in dataView.Relationships)
            {
                if (table.Id == relationship.Parent)
                {
                    if (hitListFieldId != relationship.ParentKey)
                    {
                        ResultsCriteria.Field fk = new ResultsCriteria.Field();
                        fk.Id = relationship.ParentKey;
                        resultColumn.Criterias.Add(fk);
                    }
                }
                else if (table.Id == relationship.Child)
                {
                    if (hitListFieldId != relationship.ChildKey)
                    {
                        ResultsCriteria.Field fk = new ResultsCriteria.Field();
                        fk.Id = relationship.ChildKey;
                        resultColumn.Criterias.Add(fk);
                    }
                }
            }

            resultsCriteria.Add(resultColumn);

            // Create new dataview only contains single table for cross instance query.
            var coedataView = new COEDataView();
            coedataView.Tables.Add(table);
            coedataView.Basetable = table.Id;

            // Add tables for lookup fields (suppose all those tables are from single data source)
            AddLookupTables(ref coedataView, sc, dataView);

            var queryBuilder = new QueryBuilder(coedataView.ToString(), sc.ToString(), resultsCriteria.ToString());
            var qs = queryBuilder.BuildTableQuery(ConfigurationUtilities.GetDatabaseData(table.Database).DBMSType, table.Id);

            var sqlField = Common.SqlGenerator.MetaData.DataView.GetField(
                coedataView.GetFieldById(primaryKey),
                table);

            // Add PK is not null where clause
            var pkIsNotNull = new WhereClauseEqual
            {
                DataField = sqlField,
                Val = new Value("not null", DbType.String)
            };

            qs.AddWhereItem(pkIsNotNull);

            return qs;
        }
		
        // Adds tables needed to resolve lookup fields to the filtered dataview.
        private static void AddLookupTables(ref COEDataView newDataView, SearchCriteria searchCriteria, COEDataView fullDataView)
        {
            foreach (var sc in searchCriteria.Items)
            {
                var item = sc as SearchCriteria.SearchCriteriaItem;
                if (item != null)
                {
                    var fld = fullDataView.GetFieldById(item.FieldId);
                    if (fld != null && fld.LookupFieldId > 0)
                    {
                        COEDataView.Field lookupFld = fullDataView.GetFieldById(fld.LookupFieldId);
                        if (lookupFld != null)
                        {
                            COEDataView.DataViewTable lookupTbl = fullDataView.Tables.getById(lookupFld.ParentTableId);
                            if (!newDataView.Tables.Contains(lookupTbl))
                            {
                                newDataView.Tables.Add(lookupTbl);
                            }
                        }
                        
                    }
                }
            }
        }

        private static void JoinNoCriteria(COEDataView dataView, Dictionary<int, DataSet> multipleDataSet, int tableID)
        {
            var relatedBaseTable = from relationships in dataView.Relationships where relationships.Parent == tableID select relationships;

            foreach (var relationship in relatedBaseTable)
            {
                if (multipleDataSet.ContainsKey(relationship.Parent) && multipleDataSet.ContainsKey(relationship.Child))
                {
                    var parentColumnName = dataView.GetFieldById(relationship.ParentKey).Alias;
                    var childColumnName = dataView.GetFieldById(relationship.ChildKey).Alias;
                    var parentDataSet = multipleDataSet[relationship.Parent];
                    var childDataSet = multipleDataSet[relationship.Child];
                    DataTable parentTable = parentDataSet.Tables[0];
                    DataTable childTable = childDataSet.Tables[0];

                    var results = from parent in parentTable.AsEnumerable()
                                  join child in childTable.AsEnumerable()
                                      on parent[parentColumnName].ToString() equals child[childColumnName].ToString()
                                  select new { parent, child };

                    var array = results.ToArray();

                    DataTable newChildTable = new DataTable(dataView.GetTableAliasNameById(relationship.Child));
                    foreach (DataColumn column in childTable.Columns)
                    {
                        DataColumn c = new DataColumn(column.ColumnName);
                        newChildTable.Columns.Add(c);
                    }

                    var pkList = new List<DataRow>();

                    for (var i = 0 ; i < array.Length; i++)
                    {
                        var a = array[i];
                        pkList.Add(a.child);
                    }

                    var distinctPkList = pkList.Distinct();

                    foreach (var value in distinctPkList)
                    {
                        var newRow = newChildTable.NewRow();
                        newRow.ItemArray = value.ItemArray;
                        newChildTable.Rows.Add(newRow);
                    }

                    newChildTable.AcceptChanges();

                    parentTable.TableName = dataView.GetTableAliasNameById(relationship.Parent);
                    parentTable.AcceptChanges();

                    childDataSet.Tables.Clear();
                    childDataSet.Tables.Add(newChildTable);
                    childDataSet.AcceptChanges();

                    JoinNoCriteria(dataView, multipleDataSet, relationship.Child);
                }
            }
        }

        private static DataTable JoinWithCriteria(COEDataView dataView, Dictionary<int, DataSet> multipleDataSet, int tableID, SearchCriteria sc, DataTable joinedTable = null)
        {
            // Sort relationship
            List<COEDataView.Relationship> priorRelationships = new List<COEDataView.Relationship>();
            List<COEDataView.Relationship> laterRelationships = new List<COEDataView.Relationship>();

            var relatedBaseTable = from relationship in dataView.Relationships where relationship.Parent == tableID select relationship;

            foreach (var relationship in relatedBaseTable)
            {
                var result = sc.Items.Exists(item => ((CambridgeSoft.COE.Framework.Common.SearchCriteria.SearchCriteriaItem)item).TableId == relationship.Child);
                if (result)
                {
                    priorRelationships.Add(relationship);
                }
                else
                {
                    result = FindCriteria(dataView, relationship.Child, sc);
                    if (result)
                    {
                        priorRelationships.Add(relationship);
                    }
                    else
                    {
                        laterRelationships.Add(relationship);
                    }
                }
            }

            var relationships = priorRelationships.Concat(laterRelationships);

            foreach (var relationship in relationships)
            {
                // The relationship parent key column name.
                var parentColumnName = dataView.GetFieldById(relationship.ParentKey).Alias;

                // The relationship child key column name.
                var childColumnName = dataView.GetFieldById(relationship.ChildKey).Alias;
                if (multipleDataSet.ContainsKey(relationship.Parent) && multipleDataSet.ContainsKey(relationship.Child))
                {
                    var parentDataSet = multipleDataSet[relationship.Parent];
                    var childDataSet = multipleDataSet[relationship.Child];
                    DataTable parentTable = parentDataSet.Tables[0];
                    DataTable childTable = childDataSet.Tables[0];

                    if (joinedTable != null)
                    {
                        parentTable = joinedTable;
                    }

                    if (parentTable.Columns.Contains(relationship.Parent + "." + parentColumnName))
                    {
                        parentColumnName = relationship.Parent + "." + parentColumnName;
                    }

                    if (childTable.Columns.Contains(relationship.Child + "." + childColumnName))
                    {
                        childColumnName = relationship.Child + "." + childColumnName;
                    }

                    // Memory join the parent table and child table.
                    var results = from parent in parentTable.AsEnumerable()
                                  join child in childTable.AsEnumerable()
                                      on parent[parentColumnName].ToString() equals child[childColumnName].ToString()
                                  select new { parent, child };

                    DataTable table = new DataTable();

                    // Add the parent colums to new table.
                    for (var i = 0; i < parentTable.Columns.Count; i++)
                    {
                        var c = parentTable.Columns[i];
                        var pc = new DataColumn(c.ColumnName);

                        // If column name not existing, add it to table.
                        if (!table.Columns.Contains(pc.ColumnName))
                        {
                            table.Columns.Add(pc);
                        }
                        else
                        {
                            // If column already exists, then append the parent table id before the column name.
                            pc.ColumnName = relationship.Parent + "." + c.ColumnName;
                            table.Columns.Add(pc);
                        }
                    }

                    // Add the child columns to new table.
                    for (var i = 0; i < childTable.Columns.Count; i++)
                    {
                        var c = childTable.Columns[i];
                        var cc = new DataColumn(c.ColumnName);

                        // If column name not existing, add it to table.
                        if (!table.Columns.Contains(cc.ColumnName))
                        {
                            table.Columns.Add(cc);
                        }
                        else
                        {
                            // If column already exists, then append the child table id before the column name.
                            cc.ColumnName = relationship.Child + "." + c.ColumnName;
                            table.Columns.Add(cc);
                        }
                    }

                    var array = results.ToArray();

                    var containsCriteria = sc.Items.Exists(item => ((SearchCriteria.SearchCriteriaItem)item).TableId == relationship.Child);

                    if (!containsCriteria)
                    {
                        containsCriteria = FindCriteria(dataView, relationship.Child, sc);
                    }

                    if (containsCriteria)
                    {
                        // Add the join result to new table, every row contains the parent columns and child columns.
                        foreach (var a in array)
                        {
                            DataRow row = table.NewRow();
                            row.ItemArray = a.parent.ItemArray.Concat(a.child.ItemArray).ToArray();
                            table.Rows.Add(row);
                            row.AcceptChanges();
                        }

                        table.AcceptChanges();
                    }
                    else
                    {
                        foreach (DataRow pRow in parentTable.Rows)
                        {
                            var parentIndex = parentTable.Columns.IndexOf(parentColumnName);

                            var result = from a in array where a.parent.ItemArray[parentIndex].ToString() == pRow.ItemArray[parentIndex].ToString() select a.child;

                            if (result.Count() == 0)
                            {
                                DataRow row = table.NewRow();
                                object[] o = new object[childTable.Columns.Count];

                                // If no matched data, insert empty to child table columns.
                                for (var i = 0; i < o.Length; i++)
                                {
                                    o[i] = string.Empty;
                                }

                                row.ItemArray = pRow.ItemArray.Concat(o).ToArray();
                                table.Rows.Add(row);
                                row.AcceptChanges();
                            }
                            else
                            {
                                // Insert one row for every matched result. The parent columns are same
                                foreach (var r in result)
                                {
                                    DataRow row = table.NewRow();
                                    row.ItemArray = pRow.ItemArray.Concat(r.ItemArray).ToArray();
                                    table.Rows.Add(row);
                                    row.AcceptChanges();
                                }
                            }
                        }

                        table.AcceptChanges();
                    }

                    var tempChildJoinedTable = JoinWithCriteria(dataView, multipleDataSet, relationship.Child, sc, table);

                    joinedTable = tempChildJoinedTable ?? table;
                }
            }

            return joinedTable ?? multipleDataSet[dataView.Basetable].Tables[0];
        }

        private static bool FindCriteria(COEDataView dataView, int tableID, SearchCriteria sc)
        {
            var relatedTable = from relationship in dataView.Relationships where relationship.Parent == tableID select relationship;

            foreach (var r in relatedTable)
            {
                if (sc.Items.Exists(item => ((SearchCriteria.SearchCriteriaItem)item).TableId == r.Child))
                {
                    return true;
                }

                if (FindCriteria(dataView, r.Child, sc))
                {
                    return true;
                }
            }

            return false;
        }

        #endregion
    }
}
