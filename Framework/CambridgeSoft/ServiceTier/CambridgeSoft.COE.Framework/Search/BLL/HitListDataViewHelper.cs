// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HitListDataViewHelper.cs" company="PerkinElmer Inc.">
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
    using CambridgeSoft.COE.Framework.Common;
    using CambridgeSoft.COE.Framework.Common.SqlGenerator;
    using CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries;
    using CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries.WhereItems;
    using CambridgeSoft.COE.Framework.Properties;
    using Oracle.DataAccess.Client;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Data.SqlClient;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Helper class provides methods to add hitlist table in a data view.
    /// </summary>
    public class HitListDataViewHelper
    {
        #region Methods to add old HITLIST table. Before Datalytix 6.2

        /// <summary>
        /// Adds the HitlistTable to the given dataview. This override accept the hitlist table name.
        /// </summary>
        /// <param name="dataView">
        /// The base dataview used to combine
        /// </param>
        /// <param name="manager">
        /// The manager.
        /// </param>
        public static void AddHitListTable(COEDataView dataView, SearchManager manager)
        {
            var hitListTable = GenerateHitListTable(dataView, manager);

            dataView.Tables.Add(hitListTable);

            var relationship = new COEDataView.Relationship
                                   {
                                       Parent = dataView.Basetable,
                                       Child = hitListTable.Id,
                                       ParentKey = int.Parse(dataView.BaseTablePrimaryKey),
                                       ChildKey = hitListTable.Fields.First().Id,
                                       JoinType = COEDataView.JoinTypes.INNER
                                   };

            dataView.Relationships.Add(relationship);
        }

        /// <summary>
        /// Generate hitlist table
        /// </summary>
        /// <param name="dataView"> The base dataview used to combine </param>
        /// <param name="manager"> The manager. </param>
        /// <returns> the HITLIST table </returns>
        private static COEDataView.DataViewTable GenerateHitListTable(COEDataView dataView, SearchManager manager)
        { 
            var table = new COEDataView.DataViewTable
                            {
                                Id = GetHighestTableID(dataView) + 1,
                                Name = manager.HitListTableName,
                                Alias = "hitlist",
                                Database = manager.HitListSchemaName
                            };

            manager.HitListTableId = table.Id;

            int fieldId = GetHighestFieldID(dataView);
            AddField(table, "ID", COEDataView.AbstractTypes.Integer, ++fieldId);
            AddField(table, "HITLISTID", COEDataView.AbstractTypes.Integer, ++fieldId);
            AddField(table, "SORTORDER", COEDataView.AbstractTypes.Integer, ++fieldId);

            manager.HitListSortOrderFieldId = fieldId;

            return table;
        }

        #endregion

        #region Methods to add new HITLIST table for Cross Instance query. Since Datalytix 6.2
        
        public static COEDataView GenerateDataViewForFilterChildHits(COEDataView dataView, string dataBaseName, int baseTableId)
        {
            var tempTableID = GetHighestTableID(dataView) + 1;
            var tempDataView = new COEDataView();
            tempDataView.Basetable = tempTableID;
            ////tempDataview.DataViewHandling = COEDataView.DataViewHandlingOptions.USE_CLIENT_DATAVIEW;
            tempDataView.Database = dataBaseName;
            tempDataView.Name = "TEMP_DV";
            tempDataView.Description = "DV for FilterChildHits";
            tempDataView.Application = dataView.Application;

            var tempPrimaryKey = GetHighestFieldID(dataView) + 1;
            var tempBaseTable = new COEDataView.DataViewTable()
            {
                Id = tempTableID,
                Database = dataBaseName,
                Alias = Resources.COETempChildHitListTableName,
                Name = Resources.COETempChildHitListTableName,
                PrimaryKey = tempPrimaryKey.ToString()
            };

            tempBaseTable.Fields.Add(new COEDataView.Field()
            {
                Id = tempPrimaryKey,
                Alias = GetHitlistColumnName(dataView, baseTableId),
                Name = GetHitlistColumnName(dataView, baseTableId),
                ParentTableId = tempTableID
            });

            tempBaseTable.Fields.Add(new COEDataView.Field()
            {
                Id = tempPrimaryKey + 1,
                Alias = "CHILDHITLISTID",
                Name = "CHILDHITLISTID",
                ParentTableId = tempTableID,
                DataType = COEDataView.AbstractTypes.Integer
            });

            tempDataView.Tables.Add(tempBaseTable);

            foreach (var childTable in dataView.Tables)
            {
                tempDataView.Tables.Add(childTable);
            }

            tempDataView.Relationships.AddRange(dataView.Relationships.ToArray());

            tempDataView.Relationships.Add(new CambridgeSoft.COE.Framework.Common.COEDataView.Relationship()
            {
                Parent = tempTableID,
                ParentKey = tempPrimaryKey,
                Child = baseTableId,
                ChildKey = dataView.GetTableHitListFieldId(baseTableId),
                JoinType = COEDataView.JoinTypes.INNER
            });

            return tempDataView;
        }

        public static SearchCriteria GenerateSearchCriteriaForFilterChildHits(COEDataView newDataView, int childHitListIdValue)
        {
            var newSearchCriteria = new SearchCriteria();

            newSearchCriteria.Items.Add(new CambridgeSoft.COE.Framework.Common.SearchCriteria.SearchCriteriaItem()
            {
                TableId = newDataView.Basetable,
                FieldId = newDataView.Tables.getById(newDataView.Basetable).Fields["CHILDHITLISTID"].Id,
                SearchLookupByID = false,
                Criterium = new SearchCriteria.NumericalCriteria()
                {
                    Operator = SearchCriteria.COEOperators.EQUAL,
                    Value = string.Format("{0}", childHitListIdValue)
                }
            });

            return newSearchCriteria;
        }

        /// <summary>
        /// Insert hit list to specific database
        /// </summary>
        /// <param name="multipleDataSet">All hit list table</param>
        /// <param name="resultCriteria">result criteria</param>
        /// <param name="dataView">coe data view</param>
        /// <param name="hitListId">hit list id</param>
        /// <returns>the max record rows</returns>
        public static int InsertHitListToDatabase(
            Dictionary<int, DataSet> multipleDataSet,
            ResultsCriteria resultCriteria,
            COEDataView dataView,
            int hitListId)
        {
            Task[] insertTask = new Task[resultCriteria.Tables.Count];
            var exceptionList = new List<Exception>();
            int maxNum = 0;
            for (var i = 0; i < resultCriteria.Tables.Count; i++)
            {
                var table = resultCriteria.Tables[i];
                insertTask[i] = Task.Factory.StartNew(() =>
                {
                    try
                    {
                        var dataSet = multipleDataSet[table.Id];
                        var tempNum = InsertHits(dataSet, dataView, dataView.Tables.getById(table.Id), hitListId);
                        maxNum = Math.Max(tempNum, maxNum);
                    }
                    catch (Exception e)
                    {
                        exceptionList.Add(e);
                    }
                });
            }

            Task.WaitAll(insertTask);

            if (exceptionList.Count != 0)
            {
                var errorMessage = new StringBuilder("One or more errors occurred.");
                foreach (var e in exceptionList)
                {
                    errorMessage.AppendLine(e.Message);
                }

                throw new Exception(errorMessage.ToString());
            }

            return maxNum;
        }

        /// <summary>
        /// Insert hitlist into instance hitlist table
        /// </summary>
        /// <param name="dataSet"> The hit list dataset </param>
        /// <param name="dataTable"> the table of hit list generated for </param>
        /// <param name="hitListId"> the hit list id</param>
        /// <param name="manager">the search manager </param>
        /// <returns> rows are inserted</returns>
        private static int InsertHits(DataSet dataSet, COEDataView dataView, COEDataView.DataViewTable dataTable, int hitListId)
        {
            var instanceName = ConfigurationUtilities.GetInstanceNameByDatabaseName(dataTable.Database);
            DALFactory dalFactory = new DALFactory();
            dalFactory.SetAllConfigDataExceptDatabase(instanceName, "COESearch", COEAppName.Get());
            var instanceDAL = dalFactory.GetDAL<DAL>(ConfigurationUtilities.GetDatabaseGlobalUser(dataTable.Database), true);

            // Genereate child hitlist for current table
            int childHitListId = CrossInstanceHitListHelper.GetNewChildHitListId(instanceDAL);

            // Insert CHILDHITLISTID table
            CrossInstanceHitListHelper.InsertChildHitListRecord(instanceDAL, hitListId, dataTable.Id, childHitListId);

            // Insert in child table  
            var columnToKeep = dataView.GetTableHitListField(dataTable.Id).Alias;

            for (var i = 0; i < dataSet.Tables[0].Columns.Count; i++)
            {
                DataColumn column = dataSet.Tables[0].Columns[i];
                if (!column.ColumnName.Equals(columnToKeep))
                {
                    dataSet.Tables[0].Columns.Remove(column.ColumnName);
                    i--;
                }
            }

            DataColumn hitlistIdColumn = new DataColumn("CHILDHITLISTID", typeof(int));
            hitlistIdColumn.DefaultValue = childHitListId;  

            dataSet.Tables[0].Columns.Add(hitlistIdColumn);

            dataSet.Tables[0].Columns.Add(new DataColumn("SORTORDER", typeof(int)));

            for (var i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {
                DataRow row = dataSet.Tables[0].Rows[i];
                row["SORTORDER"] = i;
                row.AcceptChanges();
            }

            dataSet.Tables[0].AcceptChanges();
            dataSet.AcceptChanges();

            // Insert the hitlist into child hitlist table
            BulkInsertChildHitListTable(
                instanceDAL, dataSet.Tables[0], columnToKeep, GetHitlistColumnName(dataView, dataTable.Id));

            int rowNum = dataSet.Tables[0].Rows.Count;

            return rowNum;
        }

        /// <summary>
        /// Builk insert the hitlist into child hitlist table (COETEMHITLIST)
        /// </summary>
        /// <param name="dal">the database access interface</param>
        /// <param name="table">the hitlist to be inserted</param>
        /// <param name="userTableColumn">The column name of the user table</param>
        /// <param name="hitlistColumn">the column name of the child hitlist table</param>
        public static void BulkInsertChildHitListTable(DAL dal, DataTable table, string userTableColumn, string hitlistColumn)
        {
            try
            {
                string hitListTableName = HitListUtilities.GetTempChildHitListTableName(dal.DALManager.DatabaseData.Owner);
                var ids = from row in table.AsEnumerable() select new { hitListId = row["CHILDHITLISTID"], id = row[userTableColumn] };

                if (ids.Count() <= 0)
                {
                    return;
                }

                dal.DALManager.BeginTransaction();

                string sql = "BEGIN " +
                                            "FOR I IN 1 .. " + dal.DALManager.BuildSqlStringParameterName("rowNum") + " LOOP " +
                                                 "INSERT /*+Append*/ INTO " + hitListTableName + " (CHILDHITLISTID," + hitlistColumn + ",SORTORDER) VALUES " +
                                                   "(" + dal.DALManager.BuildSqlStringParameterName("hitListIDs") + "," + dal.DALManager.BuildSqlStringParameterName("pIDs") + "(I),I);" +
                                            " END LOOP; " +
                                         "END;";

                OracleCommand dbCommand = (OracleCommand)dal.DALManager.Database.GetSqlStringCommand(sql);

                dbCommand.Parameters.Add(new OracleParameter("rowNum", OracleDbType.Int32, table.Rows.Count, ParameterDirection.Input));
                dbCommand.Parameters.Add(new OracleParameter("hitListIDs", OracleDbType.Int32, ids.First().hitListId, ParameterDirection.Input));
                dbCommand.Parameters.Add(new OracleParameter("pIDs", GetHitlistColumnOracleTypeByName(hitlistColumn), ids.Select(o => o.id).ToArray(), ParameterDirection.Input));
                dbCommand.Parameters["pIDs"].CollectionType = OracleCollectionType.PLSQLAssociativeArray;

                dal.DALManager.ExecuteNonQuery(dbCommand);

                dal.DALManager.CommitTransaction();
            }
            catch (Exception e)
            {
                dal.DALManager.RollbackTransaction();
                throw e;
            }
        }

        // This configuration specifies the number of seconds allowed for the bulk copy operation to complete before it is aborted.
        private static int BuilKCopyTimeoutSeconds
        {
            get
            {
                int result = 100;

                try
                {
                    result = int.Parse(System.Configuration.ConfigurationManager.AppSettings["BulkInsertHitListTimeoutSeconds"]);
                }
                catch
                {

                }

                return result;
            }
        }

        // This const specifies the number of rows to be sent as a batch to the database.
        private static int BuilkCopyBatchSize
        {
            get
            {
                // Juset set to 5000. We didn't do much investigation to decide the best value.
                return 5000;
            }
        }

        /// <summary>
        /// Create dataview by join data table and hit list tables
        /// </summary>
        /// <param name="dataTable"> the data table </param>
        /// <returns> COE data view </returns>
        public static COEDataView CreateHiltListDataViewForCrossInstance(COEDataView.DataViewTable dataTable, List<COEDataView.DataViewTable> lookupTables)
        {
            COEDataView dataView = new COEDataView();
            dataView.Tables.Add(dataTable);

            if (lookupTables != null)
            {
                dataView.Tables.AddRange(lookupTables);
            }
            dataView.Basetable = dataTable.Id;

            AddHitListTableForCrossInstance(dataView);

            return dataView;
        }

        /// <summary>
        /// Add HITLIST table in a dataview
        /// </summary>
        /// <param name="dataView"> the dataview to add table </param>
        public static void AddHitListTableForCrossInstance(COEDataView dataView)
        {
            int hitListTableId = AddInstanceHitListTable(dataView);
            AddHitListIdTable(dataView, hitListTableId);
        }

        // Get hitlist data type for the given table. Convert COEPK to NUMBER or STRING.
        private static COEDataView.HitListDataTypes GetFinalHitlistDataType(COEDataView dataView, int tableId)
        {
            var type = dataView.GetTableHitListDataType(tableId);
            if (type == COEDataView.HitListDataTypes.COEPK)
            {
                type = GetFinalHitListDataTypeByCOEPK(dataView, tableId);
            }

            return type;
        }

        // Get hitlist data type according to the COE table primary key
        private static COEDataView.HitListDataTypes GetFinalHitListDataTypeByCOEPK(COEDataView dataView, int tableId)
        {
            // Decide the finial hitlist datatype according to primary key data type
            var pkField = dataView.GetFieldById(int.Parse(dataView.GetTablePrimaryKeyById(tableId)));
            var type = Utilities.GetHitListDataTypeByFiledDataType(pkField.DataType);

            // We should save COE PK in string column if it is not Integer/Text type
            // since Oracle view may not have ROWID. For example if COE PK is Real type
            // it will be saved into TEMPCHITLIST.VAR_ID column.
            if (type == COEDataView.HitListDataTypes.ROWID)
            {
                type = COEDataView.HitListDataTypes.STRING;
            }

            return type;
        }

        // Datalytix 6.2 hitlist table can store 3 types of unique key
        private const string HitListColumnNameNumber = "ID";
        private const string HitListColumnNameVarchar2 = "VAR_ID";
        private const string HitListColumnNameRowId = "ROW_ID";

        /// <summary>
        /// Get proper column name for TEMPCHITLIST table to join with users data table
        /// </summary>
        /// <param name="dataView"></param>
        /// <param name="tableId"></param>
        /// <returns></returns>
        public static string GetHitlistColumnName(COEDataView.HitListDataTypes type)
        {
            // Default to save in TEMPCHITLIST.ID column
            string name = HitListColumnNameNumber;
            switch (type)
            {
                case COEDataView.HitListDataTypes.NUMBER:
                    // name = HitListColumnNameNumber;
                    break;
                case COEDataView.HitListDataTypes.STRING:
                    name = HitListColumnNameVarchar2;
                    break;
                case COEDataView.HitListDataTypes.ROWID:
                    name = HitListColumnNameRowId;
                    break;
                case COEDataView.HitListDataTypes.COEPK:
                    throw new Exception("Can't get hitlist column name for COEPK type. Please call ");
            }

            return name;
        }

        /// <summary>
        /// Get proper column name for TEMPCHITLIST table to join with users data table
        /// </summary>
        /// <param name="dataView"></param>
        /// <param name="tableId"></param>
        /// <returns></returns>
        public static string GetHitlistColumnName(COEDataView dataView, int tableId)
        {
            // Get final hitlist data type
            var type = GetFinalHitlistDataType(dataView, tableId);
            return GetHitlistColumnName(type);
        }

        // Get Oracle data type according to the given hitlist column name
        private static OracleDbType GetHitlistColumnOracleTypeByName(string hitlistColumn)
        {
            switch (hitlistColumn)
            {
                case HitListColumnNameNumber:
                    return OracleDbType.Int32;
                case HitListColumnNameVarchar2:
                case HitListColumnNameRowId:
                    return OracleDbType.Varchar2;
                default:
                    return OracleDbType.Int32;
            }
        }

        // Get DB data type according to the given hitlist column name
        public static DbType GetHitlistColumnDbTypeByName(string hitlistColumn)
        {
            switch (hitlistColumn)
            {
                case HitListColumnNameNumber:
                    return DbType.Int32;
                case HitListColumnNameVarchar2:
                case HitListColumnNameRowId:
                    return DbType.String;
                default:
                    return DbType.Int32;
            }
        }

        /// <summary>
        /// The add instance hit list table.
        /// </summary>
        /// <param name="dataView"> The data view. </param>
        /// <returns> The <see cref="int"/>. </returns>
        private static int AddInstanceHitListTable(COEDataView dataView)
        {
            var hitListTable = CreateInstanceHitListTable(dataView);
            dataView.Tables.Add(hitListTable);

            var hitColName = GetHitlistColumnName(dataView, dataView.Basetable);

            var relation = new COEDataView.Relationship
                               {
                                   Parent = dataView.Basetable,
                                   Child = hitListTable.Id,
                                   ParentKey = dataView.GetTableHitListFieldId(dataView.Basetable),
                                   ChildKey = hitListTable.Fields[hitColName].Id,
                                   JoinType = COEDataView.JoinTypes.INNER
                               };

            dataView.Relationships.Add(relation);

            return hitListTable.Id;
        }

        /// <summary>
        /// The add HITLISTID table.
        /// </summary>
        /// <param name="dataView"> The data view. </param>
        /// <param name="hitListTableId"> The hit list table id. </param>
        /// <param name="notFromCache"> The not From Cache. </param>
        private static void AddHitListIdTable(COEDataView dataView, int hitListTableId)
        {
            var hitListIdTable = CreateChildHitListIdTable(dataView);
            dataView.Tables.Add(hitListIdTable);

            var relation = new COEDataView.Relationship
                               {
                                   Parent = hitListTableId,
                                   Child = hitListIdTable.Id,
                                   ParentKey = dataView.Tables.getById(hitListTableId).Fields["CHILDHITLISTID"].Id,
                                   ChildKey = hitListIdTable.Fields["CHILDHITLISTID"].Id,
                                   JoinType = COEDataView.JoinTypes.INNER
                               };

            dataView.Relationships.Add(relation);
        }

        /// <summary>
        /// Generate hitlist table with new column TABLEID.
        /// </summary>
        /// <param name="dataView"> The base dataview used to combine </param>
        /// <returns> the HITLIST table </returns>
        private static COEDataView.DataViewTable CreateInstanceHitListTable(COEDataView dataView)
        {
            // full name: schema.tablename
            
            var table = new COEDataView.DataViewTable
            {
                Id = GetHighestTableID(dataView) + 1,
                Name = Resources.COETempChildHitListTableName,
                Alias = "hitlist",
                // we don't know the database name of hitlist table
                Database =  GetGlobalUserTagNameForBaseTable(dataView)
            };

            int fieldId = GetHighestFieldID(dataView);
            AddField(table, "CHILDHITLISTID", COEDataView.AbstractTypes.Integer, ++fieldId);
            AddField(table, HitListColumnNameNumber, COEDataView.AbstractTypes.Integer, ++fieldId);
            AddField(table, HitListColumnNameVarchar2, COEDataView.AbstractTypes.Text, ++fieldId);
            AddField(table, HitListColumnNameRowId, COEDataView.AbstractTypes.Text, ++fieldId);
            AddField(table, "SORTORDER", COEDataView.AbstractTypes.Integer, ++fieldId);

            return table;
        }

        /// <summary>
        /// Generate hitlist table with new column TABLEID.
        /// </summary>
        /// <param name="dataView"> The base dataview used to combine </param>
        /// <returns> the HITLIST table </returns>
        private static COEDataView.DataViewTable CreateChildHitListIdTable(COEDataView dataView)
        {
            // full name: schema.tablename
            var table = new COEDataView.DataViewTable
            {
                Id = GetHighestTableID(dataView) + 1,
                Name = Resources.COETempChildHitListIdTableName,
                Alias = "childhitlistid",
                // we don't know the database name of childhitlist table
                Database = GetGlobalUserTagNameForBaseTable(dataView)
            };

            int fieldId = GetHighestFieldID(dataView);
            AddField(table, "CHILDHITLISTID", COEDataView.AbstractTypes.Integer, ++fieldId, true);
            AddField(table, "HITLISTID", COEDataView.AbstractTypes.Integer, ++fieldId);
            AddField(table, "TABLEID", COEDataView.AbstractTypes.Integer, ++fieldId);

            return table;
        }

        /// <summary>
        /// Add hit list criteria to query
        /// </summary>
        /// <param name="query"> the query object to be modifed </param>
        /// <param name="dataView"> the data view to be queried </param>
        /// <param name="hitlistId"> the hit list id </param>
        /// <param name="tableId"> the table id </param>
        public static void AddHitListCriteriaToQuery(Query query, COEDataView dataView, int hitlistId, int tableId)
        {
            AddWhereCauseToQuery(query, dataView, hitlistId, tableId);
            AddRelationToQuery(query, dataView);
        }

        /// <summary>
        /// Get query combine query table and hitlist table
        /// </summary>
        /// <param name="query"> no paging query  </param>
        /// <param name="dataView"> dataview to query </param>
        private static void AddRelationToQuery(Query query, COEDataView dataView)
        {
            foreach (var coeRel in dataView.Relationships)
            {
                // Get the join field for parent table
                Table parentTable = GetSqlTableById(dataView, coeRel.Parent);
                var coeField = dataView.Tables.getById(coeRel.Parent).Fields.getById(coeRel.ParentKey);
                Field parentField = new Field(coeField.Name, ConvertToDbType(coeField.DataType), parentTable);

                // Get the join field for child table
                Table childTable = GetSqlTableById(dataView, coeRel.Child);
                coeField = dataView.Tables.getById(coeRel.Child).Fields.getById(coeRel.ChildKey);
                Field childField = new Field(coeField.Name, ConvertToDbType(coeField.DataType), childTable);

                // Add the SQL relation
                Relation sqlRel = new Relation { Parent = parentField, Child = childField, InnerJoin = true };
                query.AddJoinRelation(sqlRel);
            }
        }

        /// <summary>
        /// Get SQL table from COE dataview by table id
        /// </summary>
        /// <param name="dataView">the COE dataview</param>
        /// <param name="id">table id</param>
        /// <returns>SQL table</returns>
        private static Table GetSqlTableById(COEDataView dataView, int id)
        {
            COEDataView.DataViewTable coeTable = dataView.Tables.getById(id);
            var database = string.IsNullOrEmpty(coeTable.Database)
                               ? string.Empty
                               : ConfigurationUtilities.GetDatabaseData(coeTable.Database).Owner;
            return new Table(coeTable.Id, coeTable.Name, coeTable.Alias, database);
        }

        /// <summary>
        /// Add conditions on CHILDHITLISTID table and the relation between HITLIST table and CHILDHITLISTID table.
        /// </summary>
        /// <param name="query"> The query. </param>
        /// <param name="dataView"> The data view. </param>
        /// <param name="hitlistId"> The hitlist id. </param>
        /// <param name="tableId"> The table id. </param>
        private static void AddWhereCauseToQuery(Query query, COEDataView dataView, int hitlistId, int tableId)
        {
            // Get CHILDHITLISTID table name
            var tableName = Resources.COETempChildHitListIdTableName;

            // Get CHILDHITLISTID table
            COEDataView.DataViewTable coeTable = dataView.Tables[tableName];
            Table sqlTable = new Table(coeTable.Id, coeTable.Name, coeTable.Alias, coeTable.Database);

            // Get CHILDHITLISTID.HITLISTID field
            var coeField = coeTable.Fields["HITLISTID"];
            Field hitListIdField = new Field(coeField.Name, ConvertToDbType(coeField.DataType), sqlTable);

            // Add where cluase for CHILDHITLISTID.HITLISTID field
            var hitListIdClause = new WhereClauseEqual
                                      {
                                          DataField = hitListIdField,
                                          Val = new Value(hitlistId.ToString(CultureInfo.InvariantCulture), DbType.Int32)
                                      };
            query.AddWhereItem(hitListIdClause);

            // Get CHILDHITLISTID.TABLEID field
            coeField = coeTable.Fields["TABLEID"];
            Field tableIdField = new Field(coeField.Name, ConvertToDbType(coeField.DataType), sqlTable);

            // Add where clause for CHILDHITLISTID.TABLEID field
            var tableIdClause = new WhereClauseEqual
                                    {
                                        DataField = tableIdField,
                                        Val = new Value(tableId.ToString(CultureInfo.InvariantCulture), DbType.Int32)
                                    };
            query.AddWhereItem(tableIdClause);
        }

        /// <summary>
        /// Convert COE data type to system type
        /// </summary>
        /// <param name="type">the COE data type</param>
        /// <returns>the system type is transfered</returns>
        private static DbType ConvertToDbType(COEDataView.AbstractTypes type)
        {
            return Common.SqlGenerator.Utils.TypesConversor.GetType(type.ToString());
        }

        #endregion

        #region Common methods

        /// <summary>
        /// Gets the highest field id in a given dataview
        /// </summary>
        /// <param name="dataView">The dataview</param>
        /// <returns>The highest field id</returns>
        private static int GetHighestFieldID(COEDataView dataView)
        {
            int result = 0;

            foreach (COEDataView.DataViewTable table in dataView.Tables)
            {
                foreach (COEDataView.Field field in table.Fields)
                {
                    if (field.Id > result)
                    {
                        result = field.Id;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Gets the highest table id in a dataview
        /// </summary>
        /// <param name="dataView">The dataview</param>
        /// <returns>The highest table id</returns>
        private static int GetHighestTableID(COEDataView dataView)
        {
            int result = 0;

            foreach (COEDataView.DataViewTable table in dataView.Tables)
            {
                if (table.Id > result)
                {
                    result = table.Id;
                }
            }

            return result;
        }

        /// <summary>
        /// The add field.
        /// </summary>
        /// <param name="table"> The table. </param>
        /// <param name="name"> The name. </param>
        /// <param name="type"> The type. </param>
        /// <param name="id"> The id. </param>
        /// <param name="isPK"> Is primary key or not. </param>
        private static void AddField(
            COEDataView.DataViewTable table,
            string name,
            COEDataView.AbstractTypes type,
            int id,
            bool isPK = false)
        {
            var field = new COEDataView.Field
                            {
                                DataType = type,
                                Name = name,
                                Id = id,
                                ParentTableId = table.Id
                            };

            table.Fields.Add(field);

            if (isPK)
            {
                table.PrimaryKey = field.Id.ToString(CultureInfo.InvariantCulture);
            }
        }

        /// <summary>
        /// Get global user database tag name for base table.
        /// </summary>
        /// <param name="dataView"> the data view </param>
        /// <returns>the global user name </returns>
        private static string GetGlobalUserTagNameForBaseTable(COEDataView dataView)
        {
            var dataBaseName = dataView.GetDatabaseNameById(dataView.Basetable);
            var dataBase = ConfigurationUtilities.GetDatabaseData(dataBaseName);

            var instance = ConfigurationUtilities.GetInstanceData(dataBase.InstanceId);

            // The database tag name format is instancename.globaluser if not COEInstance, otherwise it is same as global user name.
            var globalDbName = instance.IsCBOEInstance ? instance.DatabaseGlobalUser : instance.Name + "." + instance.DatabaseGlobalUser;

            return globalDbName;
        }

        #endregion
    }
}
