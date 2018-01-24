using System;
using System.Collections.Generic;
using System.Text;
using CambridgeSoft.COE.Framework;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.Properties;
using System.Data;
using System.Data.Common;
using System.Xml;
using System.Xml.Serialization;
using Csla.Security;
using Csla.Data;
using Oracle.DataAccess.Client;
using CambridgeSoft.COE.Framework.COELoggingService;
using CambridgeSoft.COE.Framework.COELoggingService;
using CambridgeSoft.COE.Framework.Types.Exceptions;


namespace CambridgeSoft.COE.Framework.COETableEditorService
{
    /// <summary>
    /// Base class for data access.
    /// </summary>
    public class DAL : DALBase
    {
        #region Variables

        //The name of table which has been selected by user.
        public string _COETableEditorTableName = string.Empty;

        [NonSerialized]
        static COELog _coeLog = COELog.GetSingleton("COETableEditor");

        #endregion

        #region Override Methods
        /// <summary>
        /// to get/set the name of the table. This function need to be called by COETableEditorBOList in order to set the Table that has been selected by user.
        /// </summary>
        public override void SetServiceSpecificVariables(string tableNameString)
        {
            COETableEditorUtilities.BuildCOETableEditorTableName(tableNameString, ref _COETableEditorTableName);
        }

        /// <summary>
        /// LoadDAL() invoe DALFactory() invoe this method
        /// </summary>
        public override void SetServiceSpecificVariables()
        {
            string owner = this.DALManager.DatabaseData.OriginalOwner.ToString();
            COETableEditorUtilities.BuildCOETableEditorTableName("", ref _COETableEditorTableName);
        }

        #endregion

        #region Data Access Methods

        /// <summary>
        /// Get a new ID for inserting a new record.
        /// </summary>
        /// <returns>The new id generated</returns>
        public virtual int GetNewID()
        {
            //this has to be overridden so throw an error if there is not override
            return -1;

        }

        #endregion

        #region Data Access - Select

        /// <summary>
        /// Get a single record
        /// </summary>
        /// <param name="id">Record ID</param>
        /// <returns>a safedatareader containing the record</returns>
        public SafeDataReader Get(int id)
        {
            string sql = "SELECT * FROM " + _COETableEditorTableName +
                " WHERE " + COETableEditorUtilities.getIdFieldName(_COETableEditorTableName) + "=" + DALManager.BuildSqlStringParameterName("pId");

            DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sql);
            DALManager.Database.AddInParameter(dbCommand, "pId", DbType.Int32, id);
            SafeDataReader safeReader;
            try
            {
                safeReader = new SafeDataReader(DALManager.Database.ExecuteReader(dbCommand));
            }
            catch (Exception e)
            {
                throw e;
            }
            return safeReader;
        }
     
        /// <summary>
        /// Return all data into DataTable
        /// </summary>
        /// <returns>Datatable</returns>
        public DataTable GetParentTableData(string tableName)
        {
            List<Column> CList = COETableEditorUtilities.getColumnList(tableName);
            string sql = "select ";
            string sqlSelect = string.Empty;
            string sqlFrom = " from ";
            string sqlJoinOnList = tableName;
            foreach (Column col in CList)
            {
                if (COETableEditorUtilities.getLookupField(tableName, col.FieldName).Length == 0)
                {
                    sqlSelect = sqlSelect + tableName + "." + col.FieldName + ",";
                }
                else
                {
                    if (COETableEditorUtilities.getLookupLocation(_COETableEditorTableName, col.FieldName).ToLower() == "database")
                    {
                        string LookupTable = COETableEditorUtilities.getLookupTableName(_COETableEditorTableName, col.FieldName);
                        string LookupField = COETableEditorUtilities.getLookupField(_COETableEditorTableName, col.FieldName);
                        string LookupID = COETableEditorUtilities.getLookupID(_COETableEditorTableName, col.FieldName);
                        sqlSelect = sqlSelect + LookupTable + "." + LookupField + ", ";
                        if (sqlJoinOnList.Equals(tableName))
                        {
                            if (string.IsNullOrEmpty(LookupTable))
                            {
                                sqlJoinOnList = " ( " + sqlJoinOnList + ")";
                            }
                            else
                            {
                                sqlJoinOnList = sqlJoinOnList + " Left Join " + LookupTable + " on " + tableName + "." + col.FieldName + "=" + LookupTable + "." + LookupID;
                            }
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(LookupTable))
                            {
                                sqlJoinOnList = " ( " + sqlJoinOnList + ")";
                            }
                            else
                            {
                                sqlJoinOnList = " ( " + sqlJoinOnList + ")" + " Left Join " + LookupTable + " on " + tableName + "." + col.FieldName + "=" + LookupTable + "." + LookupID;
                            }
                        }
                    }
                    // lookupfieldlocation property of the lookupfield is xml file
                    else
                    {
                        sqlSelect = sqlSelect + col.FieldName + ",";
                    }
                }
            }
            sqlSelect = sqlSelect.Substring(0, sqlSelect.LastIndexOf(','));

            sql = sql + sqlSelect + sqlFrom + sqlJoinOnList;

            DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sql);
            DataTable dataTable = DALManager.Database.ExecuteDataSet(dbCommand).Tables[0];
            return dataTable;
        }

        /// <summary>
        /// Return true if SQL query is valid
        /// </summary>
        /// <returns>Boolean</returns>
        public bool IsValidPicklistDomain(string tableName, List<Column> cList)
        {
            bool retVal = false;
            string sql = string.Empty;
            string sqlSelect = string.Empty;
            string sqlFrom = string.Empty;
            string sqlWhere = string.Empty;
            string sqlOrderBy = string.Empty;

            foreach (Column col in cList)
            {
                switch (col.FieldName)
                {
                    case "EXT_TABLE":
                        sqlFrom = col.FieldValue.ToString();
                        if (sqlFrom.IndexOf('.') == -1)
                            throw new Exception(Resources.InvalidPicklistTable);
                        break;
                    case "EXT_ID_COL":
                    case "EXT_DISPLAY_COL": // Case statement common for both
                        sqlSelect += "{0}" + "." + col.FieldValue.ToString() + ",";
                        break;
                    case "EXT_SQL_FILTER": // Case statement common for both
                        sqlWhere = col.FieldValue.ToString();
                        break;
                    case "EXT_SQL_SORTORDER":
                        sqlOrderBy = col.FieldValue.ToString();
                        break;
                }
            }
            sqlSelect = sqlSelect.Substring(0, sqlSelect.LastIndexOf(','));
            sql = "Select " + sql 
                   + string.Format(sqlSelect,sqlFrom)
                   + " From " + sqlFrom + " " + sqlWhere +" " + sqlOrderBy;
            sql = FrameworkUtils.ReplaceSpecialTokens(sql, true);
            DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sql);
            try
            {
                SafeDataReader safeReader;
                safeReader = new SafeDataReader(DALManager.Database.ExecuteReader(dbCommand));
                retVal = true;
            }
            catch (Exception ex)
            {
                retVal = false;
                throw new Exception(Resources.InvalidSQLQuery);
            }
            return retVal;
        }

        /// <summary>
        /// Get lookup values of the lookup field
        /// </summary>
        /// <param name="fieldName">Lookup field name</param>
        /// <returns>a safedatareader containing the lookup values</returns>
        public SafeDataReader GetLookupData(string fieldName)
        {
            string lookupTable = COETableEditorUtilities.getLookupTableName(_COETableEditorTableName, fieldName);
            string lookupID = COETableEditorUtilities.getLookupID(_COETableEditorTableName, fieldName);
            string lookupField = COETableEditorUtilities.getLookupField(_COETableEditorTableName, fieldName);
            string lookupFilter = COETableEditorUtilities.getLookupFilter(_COETableEditorTableName, fieldName);
            string sql = "select " + lookupTable + "." + lookupID + "," + lookupTable + "." + lookupField + " from " + lookupTable + ((lookupFilter.Trim().Length > 0) ? " where " + lookupFilter : "") + " order by " + lookupID;

            DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sql);
            SafeDataReader safeReader;
            try
            {
                safeReader = new SafeDataReader(DALManager.Database.ExecuteReader(dbCommand));
            }
            catch (Exception e)
            {
                throw e;
            }
            return safeReader;
        }

        /// <summary>
        /// Return all child table data into DataTable
        /// </summary>
        /// <param name="flag">For judging which operator("IN" OR "NOT IN") is used.</param>
        /// <returns>Datatable</returns>
        public DataTable GetChildTableData(string id, bool flag)
        {
            DataTable childDataTable = new DataTable();

            if (COETableEditorUtilities.GetIsHasChildTable(_COETableEditorTableName))
            {

                COEChildTable childTable = COETableEditorUtilities.GetChildTable(_COETableEditorTableName, 0);

                string childTableName = childTable.Name;
                string linkTableName = COETableEditorUtilities.GetLinkTableName(childTable);
                string sql = "SELECT ";
                string sqlSelect = string.Empty;

                List<Column> CList = COETableEditorUtilities.GetChildTableColumnList(childTable);

                foreach (Column col in CList)
                {
                    sqlSelect = sqlSelect + childTableName + "." + col.FieldName + ",";

                }
                sqlSelect = sqlSelect.Substring(0, sqlSelect.LastIndexOf(','));

                sql += sqlSelect + " FROM " + childTableName;

                sql += " WHERE ";

                if(!string.IsNullOrEmpty(childTable.SqlFilter))
                    sql += "(" + childTable.SqlFilter + ") AND ";

                sql += childTable.PrimaryKey + (flag ? " IN " : " NOT IN ");
                sql += " ( SELECT " + childTable.ChildPK;
                sql += " FROM " + linkTableName + " WHERE ";
                sql += childTable.ParentPK + " = " + DALManager.BuildSqlStringParameterName("pId") + ")";

                if (!string.IsNullOrEmpty(childTable.SqlSortOrder))
                    sql += " ORDER BY " + childTable.SqlSortOrder;

                DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sql);
                DALManager.Database.AddInParameter(dbCommand, "pId", DbType.Int32, id);
                try
                {
                    childDataTable = DALManager.Database.ExecuteDataSet(dbCommand).Tables[0];
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return childDataTable;
        }

        #endregion

        #region Data Access - Insert

        /// <summary>
        /// Inserting a new record.
        /// </summary>
        /// <param name="CList"></param>
        /// <returns></returns>
        public virtual int Insert(List<Column> CList, string sequenceName)
        {
            //retrieving the id to be inserted
            int id = GetNewID();

            string idFieldName = COETableEditorUtilities.getIdFieldName(_COETableEditorTableName);

            string sql = string.Empty;
            sql = "INSERT INTO " + _COETableEditorTableName + "( ";
            foreach (Column col in CList)
            {
                sql = sql + col.FieldName + " , ";
            }
            sql = sql.Substring(0, sql.Length - 2);
            sql = sql + ") VALUES (";
            sql = sql + DALManager.BuildSqlStringParameterName("pId") + " , ";
            foreach (Column col in CList)
            {
                //updated on 2008/04/03 for table manager debug
                if (!idFieldName.ToLower().Equals(col.FieldName.ToLower()))
                {
                    sql = sql + DALManager.BuildSqlStringParameterName("p" + col.FieldName) + " , ";
                }
            }
            sql = sql.Substring(0, sql.Length - 2);
            sql = sql + ")";
            System.Diagnostics.Debug.WriteLine(sql);
            DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sql);
            if (id == 0)
            {
                DALManager.Database.AddInParameter(dbCommand, "pId", DbType.Int32, null);
            }
            else
            {
                DALManager.Database.AddInParameter(dbCommand, "pId", DbType.Int32, id);
            }

            foreach (Column col in CList)
            {
                //updated on 2008/04/03 for table manager debug
                if (!idFieldName.ToLower().Equals(col.FieldName.ToLower()))
                {
                    //modified by Jerry on 2008/07/24 for column datavalue is null
                    if (col.FieldValue != null)
                    {
                        DALManager.Database.AddParameter(dbCommand, "p" + col.FieldName, col.FieldType, Int32.MaxValue, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, Convert.ChangeType(col.FieldValue, Column.DataTypeCast[col.FieldType]));
                    }
                    else
                    {
                        DALManager.Database.AddParameter(dbCommand, "p" + col.FieldName, col.FieldType, Int32.MaxValue, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, col.FieldValue);
                    }
                }
            }
            DALManager.ExecuteNonQuery(dbCommand);

            //there is some cases that the last inserted id will be bigger than the id given by GeNewId method because database triggers.
            id = GetLastInsertedID(sequenceName, idFieldName);               
            
            return id;
        }

        /// <summary>
        /// Inserting all records.
        /// </summary>
        /// <param name="CList"></param>
        /// <returns></returns>
        public int Insert(List<Column> CList, List<int> childTableData, string sequenceName)
        {
            int id = 0;

            try
            {
                DALManager.BeginTransaction();
                //Insert records into ParentTable .
                id = Insert(CList, sequenceName);

                if (childTableData != null)
                {
                    //Insert records into LinkTable .
                    InsertIntoLinkTable(childTableData, id);
                }

                DALManager.CommitTransaction();
            }
            catch (Exception ex)
            {
                DALManager.RollbackTransaction();
                throw ex;
            }

            return id;
        }

        /// <summary>
        /// Inserting records into LinkTable.
        /// </summary>
        /// <param name="CList"></param>
        /// <returns></returns>
        private void InsertIntoLinkTable(List<int> childTableData, int parentid)
        {
            COEChildTable childTable = COETableEditorUtilities.GetChildTable(_COETableEditorTableName, 0);

            string linkTableName = COETableEditorUtilities.GetLinkTableName(childTable);

            foreach (int data in childTableData)
            {
                List<Column> childColumns = new List<Column>();

                Column childColumn = new Column(childTable.ParentPK.Substring(childTable.ParentPK.LastIndexOf(".") + 1), DbType.Int32);
                childColumn.FieldValue = parentid;
                childColumns.Add(childColumn);

                childColumn = new Column(childTable.ChildPK.Substring(childTable.ChildPK.LastIndexOf(".") + 1), DbType.Int32);
                childColumn.FieldValue = data;
                childColumns.Add(childColumn);

                string sql = string.Empty;
                sql = "INSERT INTO " + linkTableName + "( ";
                foreach (Column col in childColumns)
                {
                    sql += col.FieldName + " , ";
                }
                sql = sql.Substring(0, sql.Length - 2);
                sql += ") VALUES (";
                foreach (Column col in childColumns)
                {
                    sql = sql + DALManager.BuildSqlStringParameterName("p" + col.FieldName) + " , ";
                }
                sql = sql.Substring(0, sql.Length - 2);
                sql = sql + ")";
                System.Diagnostics.Debug.WriteLine(sql);
                DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sql);
                foreach (Column col in childColumns)
                {
                    DALManager.Database.AddInParameter(dbCommand, "p" + col.FieldName, DbType.Int32, col.FieldValue);
                }
                DALManager.ExecuteNonQuery(dbCommand);
            }
        }

        #endregion

        #region Data Access - Update

        /// <summary>
        /// Update a record.
        /// </summary>
        /// <param name="id">Id field of the Record</param>
        /// <param name="CList">List of Fields in the Record</param>
        public void Update(int id, List<Column> CList)
        {
            string sql = string.Empty;
            sql = "UPDATE " + _COETableEditorTableName + " SET ";

            foreach (Column col in CList)
            {
                sql = sql + col.FieldName + "=" + DALManager.BuildSqlStringParameterName("p" + col.FieldName) + ",";
            }

            sql = sql.Substring(0, sql.Length - 1);
            sql = sql + " WHERE " + COETableEditorUtilities.getIdFieldName(_COETableEditorTableName) + "=" + DALManager.BuildSqlStringParameterName("pId");

            DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sql);
            foreach (Column col in CList)
            {
                //modified by Jerry on 2008/07/24 for column datavalue is null
                if (col.FieldValue != null)
                {
                    DALManager.Database.AddParameter(dbCommand, "p" + col.FieldName, col.FieldType, Int32.MaxValue, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, Convert.ChangeType(col.FieldValue, Column.DataTypeCast[col.FieldType]));
                }
                else
                {
                    DALManager.Database.AddParameter(dbCommand, "p" + col.FieldName, col.FieldType, Int32.MaxValue, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, col.FieldValue);
                }
            }
            DALManager.Database.AddInParameter(dbCommand, "pId", DbType.Int32, id);
            DALManager.ExecuteNonQuery(dbCommand);
        }

        /// <summary>
        /// Update all record.
        /// </summary>
        /// <param name="id">Id field of the Record</param>
        /// <param name="CList">List of Fields in the Record</param>
        public void Update(int id, List<Column> CList, List<int> childTableData)
        {
            try
            {
                DALManager.BeginTransaction();
                //Update the parent table.
                if (!(_COETableEditorTableName.ToLower() == Resources.SecurityDatabaseName.ToLower() + ".people"))
                {
                    Update(id, CList);
                }

                COEChildTable childTable = COETableEditorUtilities.GetChildTable(_COETableEditorTableName, 0);
                string linkTableName = COETableEditorUtilities.GetLinkTableName(childTable);
                string sql = string.Empty;

                sql += "DELETE FROM " + linkTableName + " WHERE " + childTable.ParentPK + " = " + DALManager.BuildSqlStringParameterName("pId");

                DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sql);
                DALManager.Database.AddInParameter(dbCommand, "pId", DbType.Int32, id);
                DALManager.ExecuteNonQuery(dbCommand);

                if (childTableData != null)
                {
                    InsertIntoLinkTable(childTableData, id);
                }

                DALManager.CommitTransaction();
            }
            catch (Exception ex)
            {
                DALManager.RollbackTransaction();
                throw ex;
            }
        }

        #endregion

        #region Data Access - Delete
        /// <summary>
        /// Delete a record. 
        /// </summary>
        /// <param name="id">Record ID</param>
        public void Delete(int id)
        {
            DALManager.BeginTransaction();
            try
            {
                //delete the LinkTable
                if (COETableEditorUtilities.GetIsHasChildTable(_COETableEditorTableName))
                {
                    string sqlChildTable = null;
                    DbCommand dbCmdChildTable = null;
                    COEChildTable childTable = COETableEditorUtilities.GetChildTable(_COETableEditorTableName, 0);
                    string linkTableName = COETableEditorUtilities.GetLinkTableName(childTable);
                    sqlChildTable = "DELETE FROM " + linkTableName + " WHERE " + childTable.ParentPK + " =" + DALManager.BuildSqlStringParameterName("pId");
                    dbCmdChildTable = DALManager.Database.GetSqlStringCommand(sqlChildTable);
                    DALManager.Database.AddInParameter(dbCmdChildTable, "pId", DbType.Int32, id);
                }
                string sqlParentTable = null;
                DbCommand dbCmdParentTable = null;
                sqlParentTable = "DELETE FROM " + _COETableEditorTableName + " WHERE " + COETableEditorUtilities.getIdFieldName(_COETableEditorTableName) + " =" + DALManager.BuildSqlStringParameterName("pId");
                dbCmdParentTable = DALManager.Database.GetSqlStringCommand(sqlParentTable);
                DALManager.Database.AddInParameter(dbCmdParentTable, "pId", DbType.Int32, id);

                DALManager.ExecuteNonQuery(dbCmdParentTable);
                DALManager.CommitTransaction();
            }
            catch (Exception ex)
            {
                DALManager.RollbackTransaction();
                string message = "Deleting failed.";
                if (ex.Message.Contains("ORA-01031"))
                {
                    message = "Deleting failed - insufficient privileges.";
                }
                if (ex.Message.Contains("ORA-02292"))
                {
                    message = "The entry is being used and therefore should not be deleted.";
                }
                throw new Exception(message);
            }
        }
        #endregion

        #region Methods
        private int GetLastInsertedID(string sequenceName, string idFieldName)
        {
            string owner = COETableEditorUtilities.GetAppDataBase();
            int id = -1;
            string sql;
            DbCommand dbCommand;
            sql = "SELECT MAX(" + idFieldName + ") FROM " + _COETableEditorTableName;
            dbCommand = DALManager.Database.GetSqlStringCommand(sql);
            Object idQueryResult = DALManager.ExecuteScalar(dbCommand);
            if (idQueryResult == System.DBNull.Value)
                id = 1;
            else
                id = Convert.ToInt32(idQueryResult);
            return id;
        }
        public bool IsUnique(string currentTable, string fieldCoumn, string filedValue, string keyField, string keyFieldValue)
        {
            bool bReturnVal = true;
            string sqlCheck = "SELECT COUNT(" + fieldCoumn + ") FROM " + currentTable +
                              " WHERE Lower(\"" + fieldCoumn + "\") =Lower('" + filedValue + "')";
            if (!string.IsNullOrEmpty(keyFieldValue))
            {
                sqlCheck += " AND \"" + keyField + "\" !='" + keyFieldValue + "'";
            }
            DbCommand dbCommandCreate = DALManager.Database.GetSqlStringCommand(sqlCheck);
            object obCount = DALManager.ExecuteScalar(dbCommandCreate);
            int iMatchCount = 0;
            if (obCount != System.DBNull.Value)
                iMatchCount = Convert.ToInt32(obCount);
            if (iMatchCount > 0)
                bReturnVal = false;
            return bReturnVal;
        }
        public bool IsUsedCheck(string isUsedCheckMethod, string fieldValue)
        {
            bool bResult = false;
            try
             {
                int returnCount = 0;
                DbCommand dbCommandCreate = DALManager.Database.GetSqlStringCommand(isUsedCheckMethod);
                dbCommandCreate.CommandType = CommandType.StoredProcedure;
                dbCommandCreate.Parameters.Add(new OracleParameter(("FieldValue"), OracleDbType.Varchar2, fieldValue, ParameterDirection.Input));
                dbCommandCreate.Parameters.Add(new OracleParameter(("RecordCount"), OracleDbType.Int32, ParameterDirection.Output));
                DALManager.ExecuteNonQuery(dbCommandCreate);
                returnCount = int.Parse(dbCommandCreate.Parameters[1].Value.ToString());
                if (returnCount > 0)
                    bResult = true;
             }
            catch (Exception ex)
                {
                    bResult = false;
                }
            return bResult;
        }

        #endregion
    }
}
