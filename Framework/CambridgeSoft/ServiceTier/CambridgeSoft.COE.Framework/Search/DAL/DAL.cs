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
using CambridgeSoft.COE.Framework.COELoggingService;
using CambridgeSoft.COE.Framework.COEDataViewService;
using Oracle.DataAccess.Client;

namespace CambridgeSoft.COE.Framework.COESearchService
{
    /// <summary>
    /// Base DAL class for search service
    /// </summary>
    public class DAL : DALBase
    {
        [NonSerialized]
        protected static COELog _coeLog = COELog.GetSingleton("COESearch");


        /// <summary>
        /// Executes a non query statement.
        /// </summary>
        /// <param name="statement">The statement.</param>
        /// <returns>The number of records affected.</returns>
        public virtual int ExecuteNonQuery(NonQuery statement)
        {
            try
            {
                DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(statement.ToString());
                int numAffected = DALManager.ExecuteNonQuery(dbCommand);
                return numAffected;
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// Clears the COEFullPageTable based on the user id.
        /// </summary>
        public virtual void ClearCOEFullPageTable()
        {
            try
            {
                string ClientIdValue = Csla.ApplicationContext.User.Identity.Name;
                DbCommand dbCommand = DALManager.Database.GetStoredProcCommand(Resources.CentralizedStorageDB + ".delete_coefullpage_table");
                dbCommand.Parameters.Add(new OracleParameter("i_user_id", OracleDbType.Varchar2, ClientIdValue.ToUpper(), ParameterDirection.Input));
                DALManager.ExecuteNonQuery(dbCommand);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Executes a query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>A DataSet named SearchResults.</returns>
        public virtual DataSet ExecuteDataSet(Query query)
        {
            try
            {
                DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(query.ToString());
                DataSet dataSet = new DataSet("SearchResults");
                dataSet = DALManager.ExecuteDataSet(dbCommand);
                return dataSet;
            }
            catch (Exception)
            {

                throw;
            }
        }


        public virtual DataSet ExecuteDataSet(string query)
        {
            try
            {
                DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(query);
                DataSet dataSet = new DataSet("SearchResults");
                dataSet = DALManager.ExecuteDataSet(dbCommand);
                return dataSet;
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// Executes a query and keeps the connection alive.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="pagingInfo">Info about paging.</param>
        /// <param name="tableName">The table name to be filled.</param>
        /// <returns>A DataSet named SearchResults.</returns>
        public virtual DataSet ExecuteDataSetKeepAlive(string tableName, PagingInfo pagingInfo, KeepAliveHolder holder) { return null; }

        /// <summary>
        /// Executes a query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="tableName">The desired name of the resulting data table.</param>
        /// <returns>A DataTable.</returns>
        public virtual DataTable ExecuteDataTable(Query query, string tableName)
        {
            try
            {
                DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(query.ToString());
                DataTable dataTable = new DataTable();
                IDataReader dbDataReader = DALManager.ExecuteReader(dbCommand);

                // Fix Coverity: CID-28933 Resource Leak
                using (DataReaderAdapter dbDataAdapter = new DataReaderAdapter())
                {
                    dbDataAdapter.FillFromReader(dataTable, dbDataReader);
                    dataTable.TableName = tableName;
                    dbDataReader.Close();
                }

                return dataTable;
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// Executes a query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>A DataTable.</returns>
        public virtual DataTable ExecuteDataTable(Query query)
        {

            try
            {
                DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(query.ToString());
                DataTable dataTable = new DataTable();

                if (query.GetAlias() != null)
                    dataTable.TableName = query.GetMainTable().GetAlias();

                IDataReader dbDataReader = DALManager.ExecuteReader(dbCommand);

                // Fix Coverity: CID-28932 Resource Leak
                using (DataReaderAdapter dbDataAdapter = new DataReaderAdapter())
                {
                    dbDataAdapter.FillFromReader(dataTable, dbDataReader);
                    dbDataReader.Close();
                }

                return dataTable;
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
        /// <param name="key">The identifier of the holder.</param>
        /// <returns>The holder.</returns>
        public virtual KeepAliveHolder CreateKeepAliveHolder(Query query, bool timerPolicy, string key) { return null; }


        /// <summary>
        /// Inserts into HitList Table from a ref cursor, the amount of commitSize records.
        /// </summary>
        /// <param name="refCursor">The ref cursor.</param>
        /// <param name="commitSize">The commit size.</param>
        /// <returns></returns>
        public virtual int InsertHitListIncrementally(KeepAliveHolder holder, int commitSize, out bool hasEnded)
        {
            hasEnded = true;
            return -1;
        }

        /// <summary>
        /// Get RowCount use standard count(*)sql
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public virtual int GetExactRecordCount(string tableName, string owner)
        {

            try
            {
                string sql = "select count(*) from " + owner + "." + tableName;
                DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sql);
                int numRecs = Convert.ToInt32(DALManager.ExecuteScalar(dbCommand));
                return numRecs;
            }
            catch (Exception)
            {

                throw;
            }
        }


        public virtual string GetUserName(string name, int hits, string sql)
        {
            try
            {
                DbCommand dbcommand = DALManager.Database.GetSqlStringCommand(sql);
                string username = Convert.ToString(DALManager.ExecuteScalar(dbcommand));
                if (username == "" && hits == 0)
                {
                    hits = Convert.ToInt16(System.Configuration.ConfigurationManager.AppSettings["HitsPerPage"]);                   
                    string insertsql = "INSERT INTO REGDB.HITSPERPAGE(HITS_ID, HITS_USER_CODE, HITS_USER_ID, HITS)" +
                                       "SELECT COEDB.PEOPLE.PERSON_ID AS HITS_ID, COEDB.PEOPLE.USER_CODE AS HITS_USER_CODE, COEDB.PEOPLE.USER_ID AS HITS_USER_ID, '" + hits + "'" + " AS HITS FROM COEDB.PEOPLE WHERE COEDB.PEOPLE.USER_ID = '" + name.ToUpper() + "'";
                    DbCommand insertdbcommand = DALManager.Database.GetSqlStringCommand(insertsql);
                    int insert = Convert.ToInt32(DALManager.ExecuteNonQuery(insertdbcommand));
                }
                else if(username != null && hits != 0)
                {
                    string updatesql = "UPDATE REGDB.HITSPERPAGE SET REGDB.HITSPERPAGE.HITS = '" + hits + "'" +" WHERE REGDB.HITSPERPAGE.HITS_USER_ID = '" + name.ToUpper() + "'";
                    DbCommand updatedbcommand = DALManager.Database.GetSqlStringCommand(updatesql);
                    int update = Convert.ToInt32(DALManager.ExecuteNonQuery(updatedbcommand));                    

                }

                return username;
            }
            catch (Exception)
            {

                throw;
            }

        }


        public virtual int GetUserID(int hitsID, string sql)
        {
            DbCommand dbcommand = DALManager.Database.GetSqlStringCommand(sql);
            int Hits = Convert.ToInt16(DALManager.ExecuteScalar(dbcommand));
            return Hits;
        }
        /// <summary>
        /// Get RowCount use a DBMS type specific method table
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public virtual HitListInfo GetFastRecordCount(HitListInfo hitListInfo, string tableName, string owner)
        {
            //this has to be overridden so throw an error if there is not override
            return null;
        }

        /// <summary>
        /// Refreshes the user registry record count
        /// </summary>
        /// <param name="hitListInfo">The Hitlist info</param>
        /// <param name="tableName">contains the table name for the query</param>
        /// <param name="owner">table owner</param>
        /// <returns>updated hitlistinfo with record count</returns>
        public virtual HitListInfo RefreshDatabaseRecordCount(HitListInfo hitListInfo, string tableName, string owner)
        {
            //this has to be overridden so throw an error if there is not override
            return null;
        }

        /// <summary>
        /// Get RowCount for a particular hitlist using standard count(*) sql
        /// </summary>
        /// <param name="hitListID">the hitlist id.</param>
        /// <param name="owner">the schema owner.</param>
        /// <returns></returns>
        public virtual int GetHitListRecordCount(int hitListID)
        {
            try
            {
                string sql = "select number_hits from " + Resources.CentralizedStorageDB + "." + Resources.COETempHitListIDTableName + " where id=" + DALManager.Database.BuildParameterName("pHitListID");
                DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sql);
                DALManager.Database.AddInParameter(dbCommand, DALManager.Database.BuildParameterName("pHitListID"), DbType.Int32, hitListID);
                int numRecs = Convert.ToInt32(DALManager.ExecuteScalar(dbCommand));
                return numRecs;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public virtual DataSet GetByBaseTable(int dataviewid, bool matchBaseTableOnly, JumpToListType jumpToListType)
        {
            try
            {
                StringBuilder sbSql = new StringBuilder();
                DbCommand dbCommand = null;
                string _coeDataViewTableName = string.Empty;
                string owner = this.DALManager.DatabaseData.OriginalOwner.ToString();
                COEDataViewUtilities.BuildDataViewTableName(owner, ref _coeDataViewTableName);

                switch (jumpToListType)
                {
                    case JumpToListType.Dataview:

                        if (matchBaseTableOnly)
                        {
                            /*matchBaseTableOnly = true*/
                            sbSql.Append(" select distinct sf.dvid As SourceDataViewId, sbt.name As SourceDataViewName, sf.TABLEID As SourceTableId, sf.TABLENAME As SourceTableName, sf.FIELDID As SourceFieldId, sf.FIELDNAME As SourceFieldName, ");
                            sbSql.Append(" tf.dvid As TargetDataViewId, tbt.name As TargetDataViewName, tf.TABLEID As TargetTableId, tf.TABLENAME As TargetTableName, tf.FIELDID As TargetFieldId, tf.FIELDNAME As TargetFieldName ");
                            sbSql.Append(" From  VW_DVBASETABLES sbt, VW_DVFIELDS sf, VW_DVBASETABLES tbt, VW_DVFIELDS tf ");
                            sbSql.Append(" where sbt.dvid = " + dataviewid + " and sbt.dvid=sf.dvid and sf.tablename=sbt.basetablename and sf.fieldname=sbt.basetablefieldname ");
                            sbSql.Append(" and tbt.dvid = tf.dvid and tf.tablename=tbt.basetablename and tf.fieldname=tbt.basetablefieldname ");
                            sbSql.Append(" and sbt.basetablename=tbt.basetablename and sbt.basetablefieldname = tbt.basetablefieldname ");
                            
                            //DataView Access Permissions
                            sbSql.Append(" and ( tbt.Is_Public = 1 Or ( ");
                            sbSql.Append(" tbt.dvid " + AccessPermissionsSQL("", _coeDataViewTableName).Substring(5));
                            sbSql.Append(" )) ");
                            
                            dbCommand = DALManager.Database.GetSqlStringCommand(sbSql.ToString());
                            //parameters for DataView Access Permissions
                            DALManager.Database.AddInParameter(dbCommand, DALManager.Database.BuildParameterName("pPersonID"), DbType.Int32, COEUser.ID);
                            DALManager.Database.AddParameter(dbCommand, DALManager.Database.BuildParameterName("pUserName"), DbType.AnsiString, 100, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, COEUser.Name.ToUpper());
                            DALManager.Database.AddParameter(dbCommand, DALManager.Database.BuildParameterName("pUserName1"), DbType.AnsiString, 100, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, COEUser.Name.ToUpper());
                        }
                        else
                        {
                            /*matchBaseTableOnly = false*/
                            sbSql.Append(" select distinct sf.dvid As SourceDataViewId, sbt.name As SourceDataViewName, sf.TABLEID As SourceTableId, sf.TABLENAME As SourceTableName, sf.FIELDID As SourceFieldId, sf.FIELDNAME As SourceFieldName, ");
                            sbSql.Append(" tf.dvid As TargetDataViewId, tbt.name As TargetDataViewName, tf.TABLEID As TargetTableId, tf.TABLENAME As TargetTableName, tf.FIELDID As TargetFieldId, tf.FIELDNAME As TargetFieldName ");
                            sbSql.Append(" From  VW_DVBASETABLES sbt, VW_DVFIELDS sf, VW_DVBASETABLES tbt, VW_DVFIELDS tf ");
                            sbSql.Append(" where sbt.dvid = " + dataviewid + " and sbt.dvid=sf.dvid and sf.tablename=sbt.basetablename and sf.fieldname=sbt.basetablefieldname ");
                            sbSql.Append(" and tbt.dvid = tf.dvid ");
                            sbSql.Append(" and sbt.basetablename=tf.tablename and sbt.basetablefieldname = tf.fieldname ");

                            //DataView Access Permissions
                            sbSql.Append(" and ( tbt.Is_Public = 1 Or ( ");
                            sbSql.Append(" tbt.dvid " + AccessPermissionsSQL("", _coeDataViewTableName).Substring(5));
                            sbSql.Append(" )) ");

                            dbCommand = DALManager.Database.GetSqlStringCommand(sbSql.ToString());
                            //parameters for DataView Access Permissions
                            DALManager.Database.AddInParameter(dbCommand, DALManager.Database.BuildParameterName("pPersonID"), DbType.Int32, COEUser.ID);
                            DALManager.Database.AddParameter(dbCommand, DALManager.Database.BuildParameterName("pUserName"), DbType.AnsiString, 100, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, COEUser.Name.ToUpper());
                            DALManager.Database.AddParameter(dbCommand, DALManager.Database.BuildParameterName("pUserName1"), DbType.AnsiString, 100, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, COEUser.Name.ToUpper());
                        }
                        break;
                    case JumpToListType.ClientForm:
                        break;
                    case JumpToListType.WebForm:
                        break;
                    case JumpToListType.All:
                        break;
                }


                return DALManager.ExecuteDataSet(dbCommand);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public virtual DataSet GetByFieldId(int dataviewid, int fieldId, JumpToListType jumpToListType)
        {
            try
            {

                StringBuilder sbSql = new StringBuilder();
                DbCommand dbCommand = null;
                string _coeDataViewTableName = string.Empty;
                string owner = this.DALManager.DatabaseData.OriginalOwner.ToString();
                COEDataViewUtilities.BuildDataViewTableName(owner, ref _coeDataViewTableName);

                switch (jumpToListType)
                {
                    case JumpToListType.Dataview:

                        sbSql.Append(" select distinct sf.dvid As SourceDataViewId, sbt.name As SourceDataViewName, sf.TABLEID As SourceTableId, sf.TABLENAME As SourceTableName, sf.FIELDID As SourceFieldId, sf.FIELDNAME As SourceFieldName, ");
                        sbSql.Append(" tf.dvid As TargetDataViewId, tbt.name As TargetDataViewName, tf.TABLEID As TargetTableId, tf.TABLENAME As TargetTableName, tf.FIELDID As TargetFieldId, tf.FIELDNAME As TargetFieldName ");
                        sbSql.Append(" From  VW_DVBASETABLES sbt, VW_DVFIELDS sf, VW_DVBASETABLES tbt, VW_DVFIELDS tf ");
                        sbSql.Append(" where sbt.dvid = " + dataviewid + " and sbt.dvid=sf.dvid and sf.fieldid = " + fieldId);
                        sbSql.Append(" and tbt.dvid = tf.dvid ");
                        sbSql.Append(" and sf.fieldname = tf.fieldname ");

                        //DataView Access Permissions
                        sbSql.Append(" and ( tbt.Is_Public = 1 Or ( ");
                        sbSql.Append(" tbt.dvid " + AccessPermissionsSQL("", _coeDataViewTableName).Substring(5));
                        sbSql.Append(" )) ");

                        dbCommand = DALManager.Database.GetSqlStringCommand(sbSql.ToString());
                        //parameters for DataView Access Permissions
                        DALManager.Database.AddInParameter(dbCommand, DALManager.Database.BuildParameterName("pPersonID"), DbType.Int32, COEUser.ID);
                        DALManager.Database.AddParameter(dbCommand, DALManager.Database.BuildParameterName("pUserName"), DbType.AnsiString, 100, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, COEUser.Name.ToUpper());
                        DALManager.Database.AddParameter(dbCommand, DALManager.Database.BuildParameterName("pUserName1"), DbType.AnsiString, 100, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, COEUser.Name.ToUpper());

                        break;
                    case JumpToListType.ClientForm:
                        break;
                    case JumpToListType.WebForm:
                        break;
                    case JumpToListType.All:
                        break;
                }


                return DALManager.ExecuteDataSet(dbCommand);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    public class DataReaderAdapter : DbDataAdapter
    {
        public int FillFromReader(DataTable dataTable, IDataReader dataReader)
        {
            return this.Fill(dataTable, dataReader);
        }

        protected override RowUpdatedEventArgs CreateRowUpdatedEvent(
            DataRow dataRow,
            IDbCommand command,
            StatementType statementType,
            DataTableMapping tableMapping
        ) { return null; }

        protected override RowUpdatingEventArgs CreateRowUpdatingEvent(
        DataRow dataRow,
        IDbCommand command,
        StatementType statementType,
        DataTableMapping tableMapping
        ) { return null; }

        protected override void OnRowUpdated(
        RowUpdatedEventArgs value
        ) { }
        protected override void OnRowUpdating(
        RowUpdatingEventArgs value
        ) { }
    }



}
