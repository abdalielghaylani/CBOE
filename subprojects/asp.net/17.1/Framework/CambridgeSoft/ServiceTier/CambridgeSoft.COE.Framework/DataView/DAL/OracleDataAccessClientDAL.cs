using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using Oracle.DataAccess.Client;
using Oracle.DataAccess.Types;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.Properties;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using CambridgeSoft.COE.Framework.COELoggingService;
using CambridgeSoft.COE.Framework.Caching;
using Csla.Data;
using CambridgeSoft.COE.Framework.ExceptionHandling;


namespace CambridgeSoft.COE.Framework.COEDataViewService
{
    public class OracleDataAccessClientDAL : CambridgeSoft.COE.Framework.COEDataViewService.DAL
    {

        [NonSerialized]
        static COELog _coeLog = COELog.GetSingleton("COEDataView");

        /// <summary>
        /// to return the next id
        /// </summary>
        /// <param name="dataView"></param>
        /// <returns></returns>
        public override int GetNewID()
        {
            string sql = "SELECT " + _coeDataViewTableName + "_SEQ.NEXTVAL FROM DUAL";

            int id = -1;

            DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sql);

            try
            {
                id = Convert.ToInt32(DALManager.ExecuteScalar(dbCommand));
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.DispatchDALException(ex, dbCommand);
            }
            return id;
        }

        /// <summary>
        /// Builds an oracle dependecy to be used with the caching mechanism. The dependency is built against a single record in database.
        /// </summary>
        /// <param name="id">Id used to filter the record in database</param>
        /// <returns>An OracleCacheDependency</returns>
        public override COECacheDependency GetCacheDependency(int id)
        {
            OracleCacheDependency dependency = null;
            try
            {
                OracleConnection conn = DALManager.Database.CreateConnection() as OracleConnection;
                OracleCommand cmd = new OracleCommand("select dv1.rowid from coedb.coedataview dv1 where dv1.id=" + DALManager.BuildSqlStringParameterName("id"), conn);
                cmd.Parameters.Add(new OracleParameter(DALManager.BuildSqlStringParameterName("id"), OracleDbType.Int32, id, ParameterDirection.Input));
                dependency = new OracleCacheDependency(cmd);
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleDALException(ex, null);
            }
            return dependency;// I have moved this statement for code consistency and in-order to get build.
        }

        /// <summary>
        /// creates a new record in the coedataview table
        /// </summary>
        /// <param name="formgroup">the id of the formgroup</param>
        /// <param name="name">the name of the dataview</param>
        /// <param name="description">the description of the dataview</param>
        /// <param name="isPublic">whether the dataview is available as public</param>
        /// <param name="userId">the userid of the current user logged in</param>
        /// <param name="serializedCOEDataView">the serialized/xml form of the coedataview object</param>
        /// <returns>the dataview id</returns>
        public override int Insert(int formgroup, string name, bool isPublic, string description, string userId, COEDataView coeDataView, string databaseName, int id, string application)
        {

            OracleConnection conn = DALManager.Database.CreateConnection() as OracleConnection;
            //EnsureCOEDataViewTablesAndSequenceExists(); //we will no longer use this. if the tables aren't there errors will be thrown
            //add id to dataview object before serializing
            coeDataView.DataViewID = id;
            string serializedCOEDataView = COEDataViewUtilities.SerializeCOEDataView(coeDataView);
            //inserting the data in the database.
            string sql = "INSERT INTO " + _coeDataViewTableName +
                 "(ID,NAME,DESCRIPTION,IS_PUBLIC,USER_ID," +
                 "FORMGROUP,COEDATAVIEW,DATE_CREATED,DATABASE, APPLICATION)VALUES " +
                 "( " + DALManager.BuildSqlStringParameterName("pId") +
                 " ," + DALManager.BuildSqlStringParameterName("pName") +
                 " , " + DALManager.BuildSqlStringParameterName("pDescription") +
                 " , " + DALManager.BuildSqlStringParameterName("pIsPublic") +
                 " , " + DALManager.BuildSqlStringParameterName("pUserId") +
                 " , " + DALManager.BuildSqlStringParameterName("pFormGroup") +
                 " , " + DALManager.BuildSqlStringParameterName("pCOEDataView") +
                 " , " + DALManager.BuildSqlStringParameterName("pDateCreated") +
                 " , " + DALManager.BuildSqlStringParameterName("pDatabaseName") +
                 " , " + DALManager.BuildSqlStringParameterName("pApplication") + " )";

            OracleCommand dbCommand = new OracleCommand(sql, conn);

            dbCommand.Parameters.Add("pId", OracleDbType.Int32, id, ParameterDirection.Input);
            dbCommand.Parameters.Add("pName", OracleDbType.Varchar2, name, ParameterDirection.Input);
            dbCommand.Parameters.Add("pDescription", OracleDbType.Varchar2, description, ParameterDirection.Input);
            dbCommand.Parameters.Add("pIsPublic", OracleDbType.Varchar2, Convert.ToInt16(isPublic), ParameterDirection.Input);
            dbCommand.Parameters.Add("pUserId", OracleDbType.Varchar2, userId, ParameterDirection.Input);
            dbCommand.Parameters.Add("pFormGroup", OracleDbType.Varchar2, formgroup, ParameterDirection.Input);
            dbCommand.Parameters.Add("pCOEDataView", OracleDbType.XmlType, serializedCOEDataView, ParameterDirection.Input);
            dbCommand.Parameters.Add("pDateCreated", OracleDbType.Date, DateTime.Now, ParameterDirection.Input);
            dbCommand.Parameters.Add("pDatabaseName", OracleDbType.Varchar2, databaseName, ParameterDirection.Input);
            dbCommand.Parameters.Add("pApplication", OracleDbType.Varchar2, application, ParameterDirection.Input);

            int recordsAffected = -1;
            try
            {
                conn.Open();
                recordsAffected = dbCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.DispatchDALException(ex, dbCommand);
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }
            return id;
        }

        /// <summary>
        /// creates a new record in the coedataview table
        /// </summary>
        /// <param name="formgroup">the id of the formgroup</param>
        /// <param name="name">the name of the dataview</param>
        /// <param name="isPublic">whether the dataview is available as public</param>
        /// <param name="description">the description of the dataview</param>
        /// <param name="userId">the userid of the current user logged in</param>
        /// <param name="coeDataView">The coe data view.</param>
        /// <param name="databaseName">Name of the database.</param>
        /// <param name="id">The id.</param>
        /// <param name="application">The application like Registration, Inventory. used to Filter</param>
        /// <returns>the dataview id</returns>
        public override int Insert(int formgroup, string name, bool isPublic, string description, string userId, COEDataViewManagerBO coeDataView, string databaseName, int id, string application)
        {
            OracleConnection conn = DALManager.Database.CreateConnection() as OracleConnection;

            //EnsureCOEDataViewTablesAndSequenceExists(); //we will no longer use this. if the tables aren't there errors will be thrown
            //add id to dataview object before serializing
            coeDataView.DataViewId = id;
            string serializedCOEDataView = coeDataView.ToString(true);
            //inserting the data in the database.
            string sql = "INSERT INTO " + _coeDataViewTableName +
                 "(ID,NAME,DESCRIPTION,IS_PUBLIC,USER_ID," +
                 "FORMGROUP,COEDATAVIEW,DATE_CREATED,DATABASE, APPLICATION)VALUES " +
                 "( " + DALManager.BuildSqlStringParameterName("pId") +
                 " ," + DALManager.BuildSqlStringParameterName("pName") +
                 " , " + DALManager.BuildSqlStringParameterName("pDescription") +
                 " , " + DALManager.BuildSqlStringParameterName("pIsPublic") +
                 " , " + DALManager.BuildSqlStringParameterName("pUserId") +
                 " , " + DALManager.BuildSqlStringParameterName("pFormGroup") +
                 " , " + DALManager.BuildSqlStringParameterName("pCOEDataView") +
                 " , " + DALManager.BuildSqlStringParameterName("pDateCreated") +
                 " , " + DALManager.BuildSqlStringParameterName("pDatabaseName") +
                 " , " + DALManager.BuildSqlStringParameterName("pApplication") + " )";
            
            OracleCommand dbCommand = new OracleCommand(sql, conn);
                        
            dbCommand.Parameters.Add("pId", OracleDbType.Int32, id, ParameterDirection.Input);
            dbCommand.Parameters.Add("pName", OracleDbType.Varchar2, name, ParameterDirection.Input);
            dbCommand.Parameters.Add("pDescription", OracleDbType.Varchar2, description, ParameterDirection.Input);
            dbCommand.Parameters.Add("pIsPublic", OracleDbType.Varchar2, Convert.ToInt16(isPublic), ParameterDirection.Input);
            dbCommand.Parameters.Add("pUserId", OracleDbType.Varchar2, userId, ParameterDirection.Input);
            dbCommand.Parameters.Add("pFormGroup", OracleDbType.Varchar2, formgroup, ParameterDirection.Input);
            dbCommand.Parameters.Add("pCOEDataView", OracleDbType.XmlType, serializedCOEDataView, ParameterDirection.Input);
            dbCommand.Parameters.Add("pDateCreated", OracleDbType.Date, DateTime.Now, ParameterDirection.Input);
            dbCommand.Parameters.Add("pDatabaseName", OracleDbType.Varchar2, databaseName, ParameterDirection.Input);
            dbCommand.Parameters.Add("pApplication", OracleDbType.Varchar2, application, ParameterDirection.Input);

            int recordsAffected = -1;
            try
            {
                conn.Open();
                recordsAffected = DALManager.ExecuteNonQuery(dbCommand);
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.DispatchDALException(ex, dbCommand);
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }
            return id;
        }

        /// <summary>
        /// update a dataview
        /// </summary>
        /// <param name="dataViewId">dataviewid for which update in done</param>
        /// <param name="dataView">the updated xml form of the dataview</param>
        /// <param name="dataViewName">the updated name of the dataview</param>
        /// <param name="description">the updated description of the dataview</param>
        /// <param name="isPublic">whether the dataview is made public or not</param>
        public override void Update(int id, string serializedCOEDataView, string name, string description, bool isPublic, string databaseName, string application)
        {
            OracleConnection conn = DALManager.Database.CreateConnection() as OracleConnection;
            //EnsureCOEFormTablesAndSequenceExists(); //we will no longer use this. if the tables aren't there errors will be thrown

            string sql = "UPDATE " + _coeDataViewTableName +
                " SET NAME= " + DALManager.BuildSqlStringParameterName("pName") + "," +
                " DESCRIPTION=" + DALManager.BuildSqlStringParameterName("pDescription") + "," +
                " IS_PUBLIC= " + DALManager.BuildSqlStringParameterName("pIsPublic") + "," +
                " DATABASE= " + DALManager.BuildSqlStringParameterName("pDatabase") + "," +
                " COEDATAVIEW=" + DALManager.BuildSqlStringParameterName("pCOEDataView") + "," +
                " APPLICATION=" + DALManager.BuildSqlStringParameterName("pApplication") +
                " WHERE ID=" + DALManager.BuildSqlStringParameterName("pId");

            OracleCommand dbCommand = new OracleCommand(sql, conn);

            //now setting the values for the parameters.
            dbCommand.Parameters.Add("pName", OracleDbType.Varchar2, name, ParameterDirection.Input);
            dbCommand.Parameters.Add("pDescription", OracleDbType.Varchar2, description, ParameterDirection.Input);
            dbCommand.Parameters.Add("pIsPublic", OracleDbType.Varchar2, Convert.ToInt16(isPublic), ParameterDirection.Input);
            dbCommand.Parameters.Add("pDatabase", OracleDbType.Varchar2, databaseName, ParameterDirection.Input);
            dbCommand.Parameters.Add("pCOEDataView", OracleDbType.XmlType, serializedCOEDataView, ParameterDirection.Input);            
            dbCommand.Parameters.Add("pApplication", OracleDbType.Varchar2, application, ParameterDirection.Input);
            dbCommand.Parameters.Add("pId", OracleDbType.Int32, id, ParameterDirection.Input);
            int recordsAffected = -1;
            try
            {
                conn.Open();
                recordsAffected = DALManager.ExecuteNonQuery(dbCommand);
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.DispatchDALException(ex, dbCommand);
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }

        }

        public override SafeDataReader Get(int id)
        {
            string sql = "select dv1.id, dv1.name, dv1.description, dv1.user_id, dv1.is_public, dv1.date_created,dv1.database, dv1.formgroup, dv1.coedataview.getClobVal() as \"COEDATAVIEW\", dv1.application " +
                        " from " + Resources.CentralizedStorageDB + ".coedataview dv1 where dv1.id=" + DALManager.BuildSqlStringParameterName("pId");

            DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sql);
            DALManager.Database.AddInParameter(dbCommand, DALManager.BuildSqlStringParameterName("pId"), DbType.Int32, id);
            SafeDataReader safeReader = null;
            try
            {
                safeReader = new SafeDataReader(DALManager.ExecuteReader(dbCommand));
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.DispatchDALException(ex, dbCommand);
            }
            return safeReader;
        }

        public override SafeDataReader Get(string dataviewName)
        {
            string sql = "select dv1.id, dv1.name, dv1.description, dv1.user_id, dv1.is_public, dv1.date_created,dv1.database, dv1.formgroup, dv1.coedataview.getClobVal() as \"COEDATAVIEW\", dv1.application " +
                        " from " + Resources.CentralizedStorageDB + ".coedataview dv1 where dv1.name=" + DALManager.BuildSqlStringParameterName("pName");

            DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sql);
            DALManager.Database.AddParameter(dbCommand, DALManager.BuildSqlStringParameterName("pName"), DbType.AnsiString, 255, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, dataviewName);
            SafeDataReader safeReader = null;
            try
            {
                safeReader = new SafeDataReader(DALManager.ExecuteReader(dbCommand));
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.DispatchDALException(ex, dbCommand);
            }
            return safeReader;
        }

        public override SafeDataReader GetAll()
        {
            string sql = "select dv1.id, dv1.name, dv1.description, dv1.user_id, dv1.is_public, dv1.date_created,dv1.database, dv1.formgroup, dv1.coedataview.getClobVal() as \"COEDATAVIEW\", dv1.application from " + Resources.CentralizedStorageDB + ".coedataview dv1 where dv1.is_public=1 " +
                GetApplicationFilter(_dvAlias) +
                " or " + AccessPermissionsSQL(_dvAlias, _coeDataViewTableName) + "  Order By dv1.Name";


            DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sql);
            DALManager.Database.AddInParameter(dbCommand, "pPersonID", DbType.Int32, COEUser.ID);
            DALManager.Database.AddParameter(dbCommand, "pUserName", DbType.AnsiString, 100, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, COEUser.Name.ToUpper());
            DALManager.Database.AddParameter(dbCommand, "pUserName1", DbType.AnsiString, 100, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, COEUser.Name.ToUpper());
            SafeDataReader safeReader = null;
            try
            {
                safeReader = new SafeDataReader(DALManager.ExecuteReader(dbCommand));
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.DispatchDALException(ex, dbCommand);
            }
            return safeReader;
        }

        internal override SafeDataReader GetAllNoMaster()
        {
            string sql = "select dv1.id, dv1.name, dv1.description, dv1.user_id, dv1.is_public, dv1.date_created,dv1.database, dv1.formgroup, dv1.coedataview.getClobVal() as \"COEDATAVIEW\", dv1.application from " + Resources.CentralizedStorageDB + ".coedataview dv1 where dv1.id > 0 and (dv1.is_public=1 " +
                GetApplicationFilter(_dvAlias) +
                " or " + AccessPermissionsSQL(_dvAlias, _coeDataViewTableName) + ") Order By dv1.Name";
            
            DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sql);
            DALManager.Database.AddInParameter(dbCommand, "pPersonID", DbType.Int32, COEUser.ID);
            DALManager.Database.AddParameter(dbCommand, "pUserName", DbType.AnsiString, 100, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, COEUser.Name.ToUpper());
            DALManager.Database.AddParameter(dbCommand, "pUserName1", DbType.AnsiString, 100, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, COEUser.Name.ToUpper());
           
            SafeDataReader safeReader = null;

            try
            {
                safeReader = new SafeDataReader(DALManager.ExecuteReader(dbCommand));
            }
            catch (Exception ex)
            {
                // Handle the exception with exception policy.
                COEExceptionDispatcher.DispatchDALException(ex, dbCommand);
            }
            return safeReader;
        }        

        public override SafeDataReader GetAll(string userName)
        {
            DbCommand dbCommand = null;
            string sql = "select dv1.id, dv1.name, dv1.description, dv1.user_id, dv1.is_public, dv1.date_created,dv1.database, dv1.formgroup, dv1.coedataview.getClobVal() as \"COEDATAVIEW\", dv1.application from " + Resources.CentralizedStorageDB + ".coedataview dv1 where LTRIM(RTRIM(USER_ID))=LTRIM(RTRIM(" + DALManager.BuildSqlStringParameterName("pUserNameInParam") + ")) " +
                GetApplicationFilter(_dvAlias) +
                "  and" + AccessPermissionsSQL(_dvAlias, _coeDataViewTableName) + "  Order By dv1.Name";

            dbCommand = DALManager.Database.GetSqlStringCommand(sql);
            DALManager.Database.AddParameter(dbCommand, "pUserNameInParam", DbType.AnsiString, 1000, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, userName);
            DALManager.Database.AddInParameter(dbCommand, "pPersonID", DbType.Int32, COEUser.ID);
            DALManager.Database.AddParameter(dbCommand, "pUserName", DbType.AnsiString, 100, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, COEUser.Name.ToUpper());
            DALManager.Database.AddParameter(dbCommand, "pUserName1", DbType.AnsiString, 100, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, COEUser.Name.ToUpper());
            SafeDataReader safeReader=null;
            try
            {
                safeReader = new SafeDataReader(DALManager.Database.ExecuteReader(dbCommand));
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.DispatchDALException(ex, dbCommand);
            }
            return safeReader;
        }

        public override SafeDataReader GetAll(bool isPublic)
        {
            string sql = "select dv1.id, dv1.name, dv1.description, dv1.user_id, dv1.is_public, dv1.date_created,dv1.database, dv1.formgroup, dv1.coedataview.getClobVal() as \"COEDATAVIEW\", dv1.application from " + Resources.CentralizedStorageDB + ".coedataview dv1 where dv1.is_public=" + DALManager.BuildSqlStringParameterName("pIsPublic") +
                GetApplicationFilter(_dvAlias) +
                " and " + AccessPermissionsSQL(_dvAlias, _coeDataViewTableName) + "  Order By dv1.Name";

            DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sql);
            DALManager.Database.AddParameter(dbCommand, "pIsPublic", DbType.AnsiString, 1, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, Convert.ToInt16(isPublic));
            DALManager.Database.AddInParameter(dbCommand, "pPersonID", DbType.Int32, COEUser.ID);
            DALManager.Database.AddParameter(dbCommand, "pUserName", DbType.AnsiString, 100, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, COEUser.Name.ToUpper());
            DALManager.Database.AddParameter(dbCommand, "pUserName1", DbType.AnsiString, 100, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, COEUser.Name.ToUpper());
            SafeDataReader safeReader = null;
            try
            {
                safeReader = new SafeDataReader(DALManager.ExecuteReader(dbCommand));
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.DispatchDALException(ex, dbCommand);
            }

            return safeReader;
        }

        public override SafeDataReader GetUserDataViews(string userName)
        {
            DbCommand dbCommand = null;
            string sql = " select dv1.id, dv1.name, dv1.description, dv1.user_id, dv1.is_public, dv1.date_created,dv1.database, dv1.formgroup, dv1.coedataview.getClobVal() as \"COEDATAVIEW\", dv1.application from " + Resources.CentralizedStorageDB + ".coedataview dv1 where LTRIM(RTRIM(USER_ID))=LTRIM(RTRIM(" + DALManager.BuildSqlStringParameterName("pUserNameInParam") + ")) and" +
                        AccessPermissionsSQL(_dvAlias, _coeDataViewTableName) +
                        GetApplicationFilter(_dvAlias) + "  Order By dv1.Name";
            dbCommand = DALManager.Database.GetSqlStringCommand(sql);
            DALManager.Database.AddParameter(dbCommand, "pUserNameInParam", DbType.AnsiString, 1000, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, userName);
            DALManager.Database.AddInParameter(dbCommand, "pPersonID", DbType.Int32, COEUser.ID);
            DALManager.Database.AddParameter(dbCommand, "pUserName", DbType.AnsiString, 100, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, COEUser.Name.ToUpper());
            DALManager.Database.AddParameter(dbCommand, "pUserName1", DbType.AnsiString, 100, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, COEUser.Name.ToUpper());
            SafeDataReader safeReader = null;
            try
            {
                safeReader = new SafeDataReader(DALManager.Database.ExecuteReader(dbCommand));
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.DispatchDALException(ex, dbCommand);
            }
            return safeReader;
        }

        internal override List<string> GetAllTags()
        {
            List<string> result = new List<string>();
            DbCommand dbCommand = null;
            string sql = "SELECT DISTINCT TAG FROM " + Resources.CentralizedStorageDB + ".DVTableTags";

            dbCommand = DALManager.Database.GetSqlStringCommand(sql);
            using (SafeDataReader safeReader = new SafeDataReader(DALManager.Database.ExecuteReader(dbCommand)))
            {
                while (safeReader.Read())
                {
                    result.Add(safeReader.GetString(0));
                }
            }
            return result;
        }

        public override SafeDataReader GetApplicationDataviews(string appName)
        {
            string sql = " select dv1.id, dv1.name, dv1.description, dv1.user_id, dv1.is_public, dv1.date_created,dv1.database, dv1.formgroup, dv1.coedataview.getClobVal() as \"COEDATAVIEW\", dv1.application from " + Resources.CentralizedStorageDB + ".coedataview dv1 where dv1.is_public=1 and dv1.application = " +
                DALManager.BuildSqlStringParameterName("pAppName") + " " + GetApplicationNotNullFilter(_dvAlias) +
                " or " + AccessPermissionsSQL(_dvAlias, _coeDataViewTableName) + "  Order By dv1.Name";


            DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sql);
            DALManager.Database.AddParameter(dbCommand, "pAppName", DbType.AnsiString, 100, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, appName);
            DALManager.Database.AddInParameter(dbCommand, "pPersonID", DbType.Int32, COEUser.ID);
            DALManager.Database.AddParameter(dbCommand, "pUserName", DbType.AnsiString, 100, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, COEUser.Name.ToUpper());
            DALManager.Database.AddParameter(dbCommand, "pUserName1", DbType.AnsiString, 100, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, COEUser.Name.ToUpper());
            SafeDataReader safeReader = null;
            try
            {
                safeReader = new SafeDataReader(DALManager.ExecuteReader(dbCommand));
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.DispatchDALException(ex, dbCommand);
            }
            return safeReader;
        }

        /// <summary>
        /// Gets list of fields which are of type Date / BLOB / CLOB
        /// </summary>
        /// <param name="strTableName"></param>
        /// <returns></returns>
        public override List<string> GetInvalidPrimaryKeyFields(string strTableName)
        {
            List<string> lstFields = new List<string>();

            string sql = "select column_name,data_type from all_tab_columns where table_name =" + DALManager.BuildSqlStringParameterName("tName");
            DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sql);
            DALManager.Database.AddParameter(dbCommand, DALManager.BuildSqlStringParameterName("tName"), DbType.AnsiString, 255, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, strTableName);

            string strDataType = string.Empty;
            try
            {
                using (SafeDataReader safeReader = new SafeDataReader(DALManager.ExecuteReader(dbCommand)))  //COverity Fix CID 19430 ASV
                {
                    while (safeReader.Read())
                    {
                        strDataType = safeReader.GetString(1);
                        if (strDataType == "BLOB" || strDataType == "CLOB" || strDataType.ToUpperInvariant().Contains("DATE") || strDataType.ToUpperInvariant().Contains("TIME"))
                            lstFields.Add(safeReader.GetString(0));
                    }
                }
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.DispatchDALException(ex, dbCommand);
            }

            return lstFields;
        }

        /// <summary>
        /// Returns the datatable with primary & unique key contraint column information
        /// </summary>
        /// <param name="database">Name of the database, used as database owner.</param>
        /// <param name="table">Name of the table to find the index information</param>
        /// <returns>Datatable</returns>
        public override DataTable GetUniqueFields(string DataBaseName, string TableName)
        {
            DataTable returnDataTable = new DataTable();
            bool isBAViews = false;  
            try
            {
                StringBuilder sql = new StringBuilder();

                #region  Retrieve all the BioAssayView names
                List<string> lstBAViews = GetELNSchemaName();   //CBOE-1021
                if (lstBAViews != null && lstBAViews.Count > 0)
                {
                    foreach (string item in lstBAViews)
                    {
                        if (item.ToUpper().Equals(DataBaseName.ToUpper()))
                        {
                            isBAViews = true;
                            break;
                        }
                    }
                }
                #endregion

                if (isBAViews)
                {
                    sql.AppendFormat(" SELECT COLUMN_NAME , DATA_TYPE, NULLABLE  FROM ALL_TAB_COLUMNS WHERE COLUMN_NAME='ROW_ID' AND OWNER={0} AND TABLE_NAME={1}", DALManager.BuildSqlStringParameterName("database"), DALManager.BuildSqlStringParameterName("tableName"));
                }
                else
                {
                    sql.Append(" SELECT all_cons_columns.table_name, all_cons_columns.column_name,all_cons_columns.owner,all_cons_columns.constraint_name,all_constraints.constraint_type");
                    sql.Append(" FROM all_constraints ");
                    sql.Append(" INNER JOIN all_cons_columns ");
                    sql.Append(" ON  all_constraints.OWNER=all_cons_columns.OWNER ");
                    sql.AppendFormat(" WHERE all_constraints.owner= {0} AND all_constraints.table_name= {1} AND all_constraints.constraint_name=all_cons_columns.constraint_name AND ", DALManager.BuildSqlStringParameterName("database"), DALManager.BuildSqlStringParameterName("tableName"));
                    sql.Append(" UPPER( all_constraints.constraint_type) IN ('P')");
                }
                using (DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sql.ToString()))
                {
                    DALManager.Database.AddInParameter(dbCommand, DALManager.BuildSqlStringParameterName("database"), DbType.AnsiString, DataBaseName);
                    DALManager.Database.AddInParameter(dbCommand, DALManager.BuildSqlStringParameterName("tableName"), DbType.AnsiString, TableName);

                    DataSet ds = DALManager.ExecuteDataSet(dbCommand);
                    if (ds != null && ds.Tables.Count == 1 && ds.Tables[0].Rows.Count == 1)
                    {
                        returnDataTable = ds.Tables[0];
                        ds.Dispose();
                    }
                };
             }
            catch (Exception ex)
            {
                returnDataTable = null;
            }
            return returnDataTable;
        }

        /// <summary>
        /// Returns the datatable which contains the NOTNULL string/number columns
        /// </summary>
        /// <param name="database">Name of the database, used as database owner.</param>
        /// <param name="table">Name of the table to find the index information</param>
        /// <returns>Datatable</returns>
        public override DataTable GetPrimaryKeyFieldNotNullCols(string DataBaseName, string TableName)
        {
            //JiraID: CBOE-767 DVM Improvement
            DataTable returnDataTable = null; 
     
            //DbCommand dbCommand = null;
            //DataSet dataSet = null;
            //try
            //{
            //    StringBuilder sql = new StringBuilder();

            //    sql.Append(" SELECT  OWNER, COLUMN_NAME, DATA_TYPE, NULLABLE, DATA_LENGTH, DATA_PRECISION, DATA_SCALE FROM DBA_TAB_COLUMNS ");
            //    sql.AppendFormat(" WHERE UPPER(OWNER)={0} AND UPPER(TABLE_NAME)={1} ", DALManager.BuildSqlStringParameterName("database"), DALManager.BuildSqlStringParameterName("tableName"));
            //    sql.Append(" AND UPPER(DATA_TYPE) IN ('VARCHAR2','NVARCHAR2','NUMBER') AND UPPER(NULLABLE)='N' ");
                
            //    dbCommand = DALManager.Database.GetSqlStringCommand(sql.ToString());
                
            //    DALManager.Database.AddInParameter(dbCommand, DALManager.BuildSqlStringParameterName("database"), DbType.AnsiString, DataBaseName);
            //    DALManager.Database.AddInParameter(dbCommand, DALManager.BuildSqlStringParameterName("tableName"), DbType.AnsiString, TableName);


            //    dataSet = DALManager.ExecuteDataSet(dbCommand);
            //    if (dataSet != null && dataSet.Tables.Count == 1 && dataSet.Tables[0].Rows.Count == 1)
            //    {
            //        using (DataTable tempDt = dataSet.Tables[0])
            //        {
            //            for (int row = 0; row < tempDt.Rows.Count; )
            //            {
            //                DataRow item = dataSet.Tables[0].Rows[row];
            //                if (item["DATA_SCALE"] != DBNull.Value && Convert.ToInt32(item["DATA_SCALE"]) > 0)
            //                {
            //                    tempDt.Rows[row].Delete();
            //                    tempDt.AcceptChanges();
            //                    row = 0;
            //                }
            //                else
            //                    row++;
            //                item = null;
            //            }
            //            returnDataTable = tempDt;
            //        };
            //    }
            //}
            //catch (Exception ex)
            //{
            //    COEExceptionDispatcher.DispatchDALException(ex, dbCommand);
            //}
            //finally
            //{
            //    if (dbCommand != null)
            //        dbCommand.Dispose();
            //    if (dataSet != null)
            //        dataSet.Dispose();
            //}

            return returnDataTable;
        }

        /// <summary>
        /// Gets the list of BioAssay View Names.
        /// </summary>
        /// <returns></returns>
        public override List<string> GetELNSchemaName()
        {
            // JiraID: CBOE-1021
            List<string> ELN_schemList = new List<string>();
            DbCommand dbCommand = null;
            DataSet dataSet = null;
            try
            {
                StringBuilder sql = new StringBuilder();

                // The query returns the ELN schema names    AND OWNER LIKE 'EN%'
                sql.Append("SELECT   DISTINCT OWNER FROM DBA_OBJECTS  WHERE OWNER NOT IN('SYS', 'SYSTEM')  ORDER BY OWNER");
                dbCommand = DALManager.Database.GetSqlStringCommand(sql.ToString());
                dataSet = DALManager.ExecuteDataSet(dbCommand);
                if (dataSet != null && dataSet.Tables.Count > 0)
                {
                    StringBuilder tempSchemaName = new StringBuilder();
                    foreach (DataRow item in dataSet.Tables[0].Rows)
                    {
                        tempSchemaName.Append("'" + item["OWNER"].ToString() + "',");
                    }
                    string schemaName = tempSchemaName.ToString().Remove(tempSchemaName.ToString().Length - 1);

                    // Method returns the tables 'ELN_PARAMETERS' from different ELN schemas
                    using (DataTable dt = GetTablesFromELNSchemName(schemaName))
                    {
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            string tableName = string.Empty;
                            string owner = string.Empty;
                            string strBAViewName = string.Empty;

                            foreach (DataRow item in dt.Rows)
                            {
                                owner = item["OWNER"].ToString();
                                tableName = item["TABLE_NAME"].ToString();
                                if (!string.IsNullOrEmpty(owner) && !string.IsNullOrEmpty(tableName))
                                { 
                                    strBAViewName = GetBioAssayViewName(owner + "." + tableName);
                                    if (!string.IsNullOrEmpty(strBAViewName) && !ELN_schemList.Contains(strBAViewName))
                                        ELN_schemList.Add(strBAViewName);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.DispatchDALException(ex, dbCommand);
            }
            finally
            {
                if (dbCommand != null)
                    dbCommand.Dispose();
                if (dataSet != null)
                    dataSet.Dispose();
            }
            return ELN_schemList;
        }

        /// <summary>
        /// Gets the Bio Assay View Name
        /// </summary>
        /// <param name="query">Shema name</param>
        /// <returns></returns>
        private string GetBioAssayViewName(string query)
        {
            string strBioAssayViewName = string.Empty;
            if (!string.IsNullOrEmpty(query))
            {
                // JiraID: CBOE-1021               
                DbCommand dbCommand = null;
                DataSet dataSet = null;
                try
                {
                    StringBuilder sql = new StringBuilder();
                    sql.Append(" SELECT DATA AS BAVIEWNAME FROM  ");
                    sql.Append(query);
                    sql.Append(" WHERE UPPER(NAME)='BIOASSAY_VIEW_SCHEMA' ");

                    dbCommand = DALManager.Database.GetSqlStringCommand(sql.ToString());
                    dataSet = DALManager.ExecuteDataSet(dbCommand);
                    if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
                    {
                        strBioAssayViewName = dataSet.Tables[0].Rows[0]["BAVIEWNAME"].ToString();
                    }
                }
                catch (Exception ex)
                {
                    COEExceptionDispatcher.DispatchDALException(ex, dbCommand);
                }
                finally
                {
                    if (dbCommand != null)
                        dbCommand.Dispose();
                    if (dataSet != null)
                        dataSet.Dispose();
                }
            }
            return strBioAssayViewName;
        }

        /// <summary>
        /// Returns the datatable which contains the Enotebook database list
        /// </summary>
        /// <param name="schemaName">List of schema name seperated by comma like 'EN','EN2'</param>
        /// <returns></returns>
        private DataTable GetTablesFromELNSchemName(string schemaName)
        {
            DataTable returnDataTable = null;
            if (!string.IsNullOrEmpty(schemaName))
            {
                // JiraID: CBOE-1021               
                DbCommand dbCommand = null;
                DataSet dataSet = null;
                try
                {
                    StringBuilder sql = new StringBuilder();
                     
                    sql.Append(" SELECT DISTINCT OWNER, OBJECT_NAME AS TABLE_NAME FROM DBA_OBJECTS WHERE UPPER(OBJECT_TYPE) = 'TABLE' AND UPPER(OWNER) IN ");
                    sql.Append(" ( ");
                    sql.Append(schemaName);
                    sql.Append(" ) ");
                    sql.Append(" AND UPPER(OBJECT_NAME)='ELN_PARAMETERS'");

                    dbCommand = DALManager.Database.GetSqlStringCommand(sql.ToString());
                    dataSet = DALManager.ExecuteDataSet(dbCommand);
                    if (dataSet != null && dataSet.Tables.Count > 0)
                    {
                        returnDataTable = dataSet.Tables[0];
                    }
                }
                catch (Exception ex)
                {
                    COEExceptionDispatcher.DispatchDALException(ex, dbCommand);
                }
                finally
                {
                    if (dbCommand != null)
                        dbCommand.Dispose();
                    if (dataSet != null)
                        dataSet.Dispose();
                }
            }
            return returnDataTable;
        }

        #region COEDataViewAsDataSet
        internal override DataSet GetMasterDataViewTables()
        {
            DbCommand dbCommand = null;
            string sql = "SELECT DVID, TABLEID, TABLENAME, TABLEALIAS, TABLEDB, TABLEPK, ISVIEW FROM " + Resources.CentralizedStorageDB + ".MDVTables ORDER BY TABLEDB, TABLEALIAS";

            dbCommand = DALManager.Database.GetSqlStringCommand(sql);

            return DALManager.ExecuteDataSet(dbCommand);
        }

        internal override DataSet GetPublishedDataViewTables()
        {
            DbCommand dbCommand = null;
            string sql = "SELECT DVID, TABLEID, TABLENAME, TABLEALIAS, TABLEDB, TABLEPK, ISVIEW FROM " + Resources.CentralizedStorageDB + ".PublishedDVTables ORDER BY TABLEDB, TABLEALIAS";

            dbCommand = DALManager.Database.GetSqlStringCommand(sql);

            return DALManager.ExecuteDataSet(dbCommand);
        }
        #endregion
    }
}
