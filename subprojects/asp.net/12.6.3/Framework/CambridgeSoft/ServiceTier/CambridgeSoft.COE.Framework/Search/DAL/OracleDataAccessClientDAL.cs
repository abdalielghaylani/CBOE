using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using CambridgeSoft.COE.Framework.Common.SqlGenerator;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.Properties;
using CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries;
using CambridgeSoft.COE.Framework.Common.SqlGenerator.NonQueries;
using Oracle.DataAccess.Client;
using Oracle.DataAccess.Types;
using CambridgeSoft.COE.Framework.COELoggingService;
using System.Reflection;
using System.Text.RegularExpressions;
using CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries.SelectItems;


namespace CambridgeSoft.COE.Framework.COESearchService
{
    /// <summary>
    /// The Data Access class in charge of communicating against oracle database for searching data.
    /// </summary>
    public class OracleDataAccessClientDAL : CambridgeSoft.COE.Framework.COESearchService.DAL
    {
        /// <summary>
        /// Executes a Query against a database and returns a DataSet.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>A DataSet containing the results.</returns>
        public override DataSet ExecuteDataSet(Query query)
        {
            _coeLog.LogStart("ExecuteDataSet", 1, System.Diagnostics.SourceLevels.All);
            try
            {
                DataSet dataSet = new DataSet("SearchResults");
                dataSet.Tables.Add(this.ExecuteDataTable(query));
                return dataSet;
            }
            catch (Exception exception)
            {
                _coeLog.Log("ExecuteDataSet: " + exception.Message, 1, System.Diagnostics.SourceLevels.Error);
                throw exception;
            }
            finally
            {
                _coeLog.LogEnd("ExecuteDataSet", 1, System.Diagnostics.SourceLevels.All);
            }
        }


        /// <summary>
        /// Executes a query agains database and Returns a DataTable.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="tableName">The desired name of the resulting table.</param>
        /// <returns>The resulting rows in a DataTable.</returns>
        public override DataTable ExecuteDataTable(Query query, string tableName)
        {
            try
            {
                DataTable dataTable = ExecuteDataTable(query);
                dataTable.TableName = tableName;
                return dataTable;
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// Executes a query agains database and Returns a DataTable.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>The resulting rows in a DataTable.</returns>
        public override DataTable ExecuteDataTable(Query query)
        {
            string methodSignature = MethodBase.GetCurrentMethod().DeclaringType.Name + "->" + MethodBase.GetCurrentMethod().Name.ToUpper() + "  ";
            _coeLog.LogStart(methodSignature + query.ToString(), 1, System.Diagnostics.SourceLevels.All);
            try
            {
                //CBOE-779/CBOE-1026
                //(REG: Records cannot be searched in CBOE Build 12.6.0.2505)
               List<SelectClauseItem> tempCollection = new List<SelectClauseItem>();
               if (query != null)
               {
                   foreach (var item in query.GetSelectClauseItems())
                   {
                       if (item!=null && item.Alias.Trim() == "" && item.Name.Trim() == "" && item.DataField.FieldId == -1)
                       {
                           tempCollection.Add(item);
                       }
                   }
                   foreach (var item in tempCollection)
                   {
                       if (item != null)
                       {
                           query.RemoveSelectItem(item);
                       }
                   }
               }

                DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(query.ToString());
                dbCommand.Connection = (DALManager.DbConnection != null) ? DALManager.DbConnection : DALManager.Database.CreateConnection();
                dbCommand.Transaction = DALManager.DbTransaction;
                
                for (int i = 0; i < query.ParamValues.Count; i++)
                {
                    OracleParameter param = new OracleParameter(DALManager.BuildSqlStringParameterName(i.ToString()), query.ParamValues[i].Val);
                    param.DbType = query.ParamValues[i].Type;
                    dbCommand.Parameters.Add(param);
                    _coeLog.Log("Parameter :" + i + " = " + query.ParamValues[i].Val + "  --  Param Type: " + param.DbType);
                }

                ((OracleCommand)dbCommand).FetchSize *= 16;
                ((OracleCommand)dbCommand).InitialLOBFetchSize = -1;
                var adapter = new OracleDataAdapter((OracleCommand)dbCommand);

                DataTable dataTable = new DataTable();
                // For avoid the DateTime Parsing issue, ignore the culture infor.
                dataTable.Locale = System.Globalization.CultureInfo.InvariantCulture;
                adapter.FillSchema(dataTable, SchemaType.Mapped);
                
                adapter.SafeMapping.Add("*", typeof(string));

                foreach (DataColumn col in dataTable.Columns)
                {
                    switch (col.DataType.Name)
                    {
                        case "Int16":
                        case "Int32":
                            col.DataType = typeof(Int64);
                            break;
                    }
                }
                adapter.Fill(dataTable);
                foreach (DataColumn col in dataTable.Columns)
                {
                    col.ReadOnly = false;
                    //For sql generation purposes we truncated the alias to be at most 30 characters, 
                    //and if trimmed we ended the alias with "|hash" where hash is got from the method GetHashCode()
                    //We need to make that transparent to the end user and thus re-fill the column name with the FullAlias.
                    Regex regex = new Regex(@"\|\d+$");
                    if (regex.IsMatch(col.ColumnName))
                    {
                        string hashStr = col.ColumnName.Substring(col.ColumnName.LastIndexOf("|") + 1);
                        foreach (SelectClauseItem item in query.GetSelectClauseItems())
                        {
                            if (item.GetHashCode().ToString() == hashStr)
                            {
                                col.ColumnName = item.FullAlias;
                                break;
                            }
                        }
                    }
                }
                adapter.Dispose();
                return dataTable;
            }
            catch (Exception exception)
            {
                _coeLog.Log(methodSignature + exception.Message, 1, System.Diagnostics.SourceLevels.Error);
                throw exception;
            }
            finally
            {
                _coeLog.LogEnd(methodSignature);
            }
        }        

        /// <summary>
        /// Executes a Non Query statement like insert and update.
        /// </summary>
        /// <param name="statement">The Statement.</param>
        /// <returns>The number of records affected.</returns>
        public override int ExecuteNonQuery(NonQuery statement)
        {
            string methodSignature = MethodBase.GetCurrentMethod().DeclaringType.Name + "->" + MethodBase.GetCurrentMethod().Name.ToUpper() + "  ";
            
            _coeLog.LogStart(methodSignature + statement.ToString(), 1, System.Diagnostics.SourceLevels.All);
            
            try
            {
                DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(statement.ToString());
                for (int i = 0; i < statement.ParamValues.Count; i++)
                {
                    OracleParameter param = new OracleParameter(DALManager.BuildSqlStringParameterName(i.ToString()), statement.ParamValues[i].Val);

                    if(statement.ParamValues[i].Type == DbType.Object)
                        param.OracleDbType = OracleDbType.Clob;
                    else if(statement.ParamValues[i].Type == DbType.Boolean)
                        param.OracleDbType = OracleDbType.Varchar2;
                    else
                        param.DbType = statement.ParamValues[i].Type;
                    
                    _coeLog.Log("Parameter :" + i + " = " + statement.ParamValues[i].Val + "  --  Param Type: " + param.DbType);

                    dbCommand.Parameters.Add(param);
                }
               
                int recordCount = DALManager.ExecuteNonQuery(dbCommand);

                _coeLog.Log(methodSignature + "RecordCount = " + recordCount);
                return recordCount;
            }
            catch (Exception exception)
            {
                _coeLog.Log(methodSignature + exception.Message);
                throw exception;
            }
            finally
            {
                _coeLog.LogEnd(methodSignature);
            }
        }

        /// <summary>
        /// Executes a query and keeps the connection alive.
        /// </summary>
        /// <param name="tableName">The table name to be filled.</param>
        /// <param name="pagingInfo">Info about paging.</param>
        /// <param name="holder">The keep alive holder.</param>
        /// <returns>A DataSet named SearchResults.</returns>
        public override DataSet ExecuteDataSetKeepAlive(string tableName, PagingInfo pagingInfo, KeepAliveHolder holder)
        {
            _coeLog.LogStart("ExecuteDataSetKeepAlive", 1, System.Diagnostics.SourceLevels.All);
            try
            {
                DataSet ds = new DataSet();
                // Fix Coverity: CID-28974 Resource Leak
                using (OracleDataAdapter da = new OracleDataAdapter())
                {
                    da.Fill(ds, pagingInfo.Start - 1, pagingInfo.RecordCount, tableName, holder.RefCursor);

                    _coeLog.LogEnd("ExecuteDataSetKeepAlive", 1, System.Diagnostics.SourceLevels.All);
                }

                return ds;
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// Creates a cursor and returns its holder.
        /// </summary>
        /// <param name="query">The query to fill the cursor.</param>
        /// <param name="timerPolicy">Determines if a timer policy will be applied to release the resources.</param>
        /// <returns>The holder.</returns>
        public override KeepAliveHolder CreateKeepAliveHolder(Query query, bool timerPolicy, string key)
        {
            _coeLog.LogStart("CreateKeepAliveHolder", 1, System.Diagnostics.SourceLevels.All);
            OracleConnection connection = null;
            KeepAliveHolder holder = null;
            try
            {

                if (DALManager.DbConnection != null)
                    connection = (OracleConnection)DALManager.DbConnection;
                else
                    connection = (OracleConnection)DALManager.Database.CreateConnection();

                //Fix required for avoiding sporadic error by Database Group.
                string refCurName = "c" + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + DateTime.Now.Millisecond.ToString();
                OracleParameter paramCursor = new OracleParameter(refCurName, OracleDbType.RefCursor, ParameterDirection.Output);

                // Coverity Fix CID - 11819
                using (OracleCommand cmd = new OracleCommand("begin open " + ":" + refCurName + " for " + query.ToString() + "; end;", connection))
                {
                    _coeLog.Log("SQL: " + cmd.CommandText);
                    cmd.Parameters.Add(paramCursor);
                    for (int i = 0; i < query.ParamValues.Count; i++)
                    {
                        OracleParameter param = null;
                        if (query.ParamValues[i].Type == DbType.Boolean)
                        {
                            bool value = false;
                            char charValue = 'F';
                            string paramVal = (query.ParamValues[i].Val.ToUpper() == "T") ? "true" : "false";
                            if (bool.TryParse(paramVal, out value))
                            {
                                charValue = value ? 'T' : 'F';
                            }
                            param = new OracleParameter(DALManager.BuildSqlStringParameterName(i.ToString()), charValue);
                            param.OracleDbType = OracleDbType.Char;
                        }
                        else
                        {
                            param = new OracleParameter(DALManager.BuildSqlStringParameterName(i.ToString()), query.ParamValues[i].Val);
                            param.DbType = query.ParamValues[i].Type;
                        }
                        cmd.Parameters.Add(param);
                        _coeLog.Log("Parameter :" + i + " = " + query.ParamValues[i].Val + "  --  Param Type: " + param.DbType);
                    }

                    if (connection.State != ConnectionState.Open)
                        connection.Open();

                    cmd.ExecuteNonQuery();
                    OracleRefCursor refCursor = (OracleRefCursor)cmd.Parameters[0].Value;

                    holder = new KeepAliveHolder(connection, refCursor, key, timerPolicy);
                    _coeLog.LogEnd("CreateKeepAliveHolder", 1, System.Diagnostics.SourceLevels.All);                    
                }
                return holder;
            }
            catch (Exception exception)
            {
                if (connection != null && connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
                _coeLog.LogEnd("CreateKeepAliveHolder", 1, System.Diagnostics.SourceLevels.All);
                throw exception;
            }
        }

        /// <summary>
        /// Inserts into HitList Table from a ref cursor, the amount of commitSize records.
        /// </summary>
        /// <param name="refCursor">The ref cursor.</param>
        /// <param name="commitSize">The commit size.</param>
        /// <returns></returns>
        public override int InsertHitListIncrementally(KeepAliveHolder holder, int commitSize, out bool hasEnded)
        {
            hasEnded = false;


            int recordCount = 0;

            OracleRefCursor refCursor = holder.RefCursor;

            // Coverity Fix CID - 11820
            using (OracleCommand command = new OracleCommand(Resources.CentralizedStorageDB + ".CoeDBLibrary.inserthitlist"))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new OracleParameter(("refcursor"), OracleDbType.RefCursor, refCursor, ParameterDirection.Input));
                command.Parameters.Add(new OracleParameter(("commitSize"), OracleDbType.Int32, commitSize, ParameterDirection.Input));
                command.Parameters.Add(new OracleParameter(("recordCount"), OracleDbType.Int32, recordCount, ParameterDirection.Output));
                command.Parameters.Add(new OracleParameter(("hasEnded"), OracleDbType.Int32, ParameterDirection.Output));
                command.Connection = refCursor.Connection;

                command.ExecuteNonQuery();
                recordCount = int.Parse(command.Parameters[2].Value.ToString());

                if (int.Parse(command.Parameters[3].Value.ToString()) == 1)
                {
                    holder.Close();
                    hasEnded = true;
                }
            }

            return recordCount;
        }

        /// <summary>
        /// Refreshes the user registry record count
        /// </summary>
        /// <param name="hitListInfo">The Hitlist info</param>
        /// <param name="tableName">contains the table name for the query</param>
        /// <param name="owner">table owner</param>
        /// <returns>updated hitlistinfo with record count</returns>
        public override HitListInfo RefreshDatabaseRecordCount(HitListInfo hitListInfo, string tableName, string owner)
        {
            try
            {
                if (owner.ToUpper() == Resources.RegistrationDatabaseName && tableName.ToUpper() == "VW_MIXTURE_REGNUMBER")
                {
                    string ClientIdValue = Csla.ApplicationContext.User.Identity.Name;
                    string returnValue = "0";
                    DbCommand dbCommand = DALManager.Database.GetStoredProcCommand(Resources.RegistrationDatabaseName + ".refresh_User_regnum_count");
                    dbCommand.Parameters.Add(new OracleParameter("RETURN_VALUE", OracleDbType.Int32, 0, ParameterDirection.ReturnValue, true, 8, 0, String.Empty, DataRowVersion.Current, returnValue));
                    dbCommand.Parameters.Add(new OracleParameter("i_proj", OracleDbType.Int32, DBNull.Value, ParameterDirection.Input));
                    dbCommand.Parameters.Add(new OracleParameter("i_user_id", OracleDbType.Varchar2, ClientIdValue.ToUpper(), ParameterDirection.Input));
                    DALManager.ExecuteNonQuery(dbCommand);
                    returnValue = Convert.ToString(dbCommand.Parameters[0].Value);
                    hitListInfo.RecordCount = Convert.ToInt32(returnValue);
                    hitListInfo.IsExactRecordCount = true;
                }
                return hitListInfo;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// Override: Get RowCount via statisstics if table size is less then a threshold
        /// </summary>
        /// <param name="hitListInfo"></param>
        /// <param name="tableName"></param>
        /// <param name="owner"></param>
        /// <returns></returns>
        public override HitListInfo GetFastRecordCount(HitListInfo hitListInfo, string tableName, string owner)
        {
            try
            {
                DataSet dataset;
                if (owner.ToUpper() == Resources.RegistrationDatabaseName && tableName.ToUpper() == "VW_MIXTURE_REGNUMBER")
                {
                    string ClientIdValue = Csla.ApplicationContext.User.Identity.Name;
                    string sql = "select reg_num_cnt as ExactRecordCount from " + Resources.RegistrationDatabaseName + ".user_regnum_count where upper(user_id) = " + DALManager.BuildSqlStringParameterName("pUserID");
                    DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sql);

                    DALManager.Database.AddInParameter(dbCommand, "pUserID", DbType.AnsiString, ClientIdValue.ToUpper());
                    dataset = DALManager.ExecuteDataSet(dbCommand);
                    if (dataset != null && dataset.Tables.Count > 0 && dataset.Tables[0].Rows.Count > 0)
                    {
                        DataRow dr = dataset.Tables[0].Rows[0];
                        hitListInfo.RecordCount = Convert.ToInt32(dr["ExactRecordCount"]);
                        hitListInfo.IsExactRecordCount = true;
                    }
                }
                else
                {
                    int pThreshold1 = 1000000;
                    int pThreshold2 = pThreshold1;
                    int approx_count = 0;
                    string pTableName = tableName;
                    string pOwner = owner;

                    string sql = "select num_rows from dba_tables where table_name = " + DALManager.BuildSqlStringParameterName("pTableName") +
                                 " and owner= " + DALManager.BuildSqlStringParameterName("pOwner");
                    //create stored procedure command type
                    DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sql);

                    DALManager.Database.AddInParameter(dbCommand, "pTableName", DbType.AnsiString, tableName);
                    DALManager.Database.AddInParameter(dbCommand, "pOwner", DbType.AnsiString, owner);
                    dataset = DALManager.ExecuteDataSet(dbCommand);
                    DataRow dr;
                    if (dataset != null && dataset.Tables.Count > 0 && dataset.Tables[0].Rows.Count > 0)
                    {
                        dr = dataset.Tables[0].Rows[0];
                        approx_count = Convert.ToInt32(dr["num_rows"]);
                    }
                    if (approx_count > pThreshold1)
                    {
                        hitListInfo.RecordCount = approx_count;
                        hitListInfo.IsExactRecordCount = false;
                    }
                    else
                    {
                        sql = "select count(*) as ExactRecordCount from " + pOwner + "." + pTableName;
                        //create stored procedure command type
                        dbCommand = DALManager.Database.GetSqlStringCommand(sql);

                        dataset = DALManager.ExecuteDataSet(dbCommand);
                        if (dataset != null && dataset.Tables.Count > 0 && dataset.Tables[0].Rows.Count > 0)
                        {
                            dr = dataset.Tables[0].Rows[0];
                            hitListInfo.RecordCount = Convert.ToInt32(dr["ExactRecordCount"]);
                            hitListInfo.IsExactRecordCount = true;
                        }
                    }
                }
                dataset.Dispose();
                return hitListInfo;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
                
        

    }
}
