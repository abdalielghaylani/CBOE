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
using System.Reflection;
using Csla.Data;
using CambridgeSoft.COE.Framework.Caching;

namespace CambridgeSoft.COE.Framework.COESearchCriteriaService
{
    /// <summary>
    /// 
    /// </summary>
    public class OracleDataAccessClientDAL : CambridgeSoft.COE.Framework.COESearchCriteriaService.DAL
    {
        /// <summary>
        /// to return the next id
        /// </summary>
        /// <param name="dataView"></param>
        /// <returns></returns>
        public override int GetNewID()
        {
            string methodSignature = MethodBase.GetCurrentMethod().DeclaringType.Name + "->" + MethodBase.GetCurrentMethod().Name.ToUpper() + "  ";

            try
            {
                string sql = "SELECT " + _coeSearchCriteriaTableName + "_SEQ.NEXTVAL FROM DUAL";
                _coeLog.LogStart(methodSignature + sql);
                DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sql);
                int id = Convert.ToInt32(DALManager.Database.ExecuteScalar(dbCommand));
                return id;
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

        public override int GetNewSavedSearchCriteriaId()
        {
            string methodSignature = MethodBase.GetCurrentMethod().DeclaringType.Name + "->" + MethodBase.GetCurrentMethod().Name.ToUpper() + "  ";

            try
            {
                string sql = "SELECT " + _coeSearchCriteriaTableName + "_SEQ.NEXTVAL FROM DUAL";
                _coeLog.LogStart(methodSignature + sql);
                DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sql);
                int id = Convert.ToInt32(DALManager.Database.ExecuteScalar(dbCommand));
                return id;
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


        public override int Insert(int formgroup, string name, bool isPublic, string description, string userId, SearchCriteria _coeSearchCriteria, int numHits, string databaseName, int dataviewId)
        {
            string methodSignature = MethodBase.GetCurrentMethod().DeclaringType.Name + "->" + MethodBase.GetCurrentMethod().Name.ToUpper() + "  ";

            try
            {

                _coeLog.LogStart(methodSignature);

                //retrieving the id to be inserted
                int id = GetNewID();

                _coeSearchCriteria.SearchCriteriaID = id;
                string serializedCOESearchCriteria = COESearchCriteriaUtilities.SerializeCOESearchCriteria(_coeSearchCriteria);


                //inserting the data in the database.
                string sql = "INSERT INTO " + _coeSearchCriteriaTableName +
                     "(ID,NAME,DESCRIPTION,IS_PUBLIC,USER_ID," +
                     "FORMGROUP,COESEARCHCRITERIA,NUM_HITS, DATE_CREATED,DATABASE,DATAVIEW_ID)VALUES " +
                     "( " + DALManager.BuildSqlStringParameterName("pId") +
                     " , " + DALManager.BuildSqlStringParameterName("pName") +
                     " , " + DALManager.BuildSqlStringParameterName("pDescription") +
                     " , " + DALManager.BuildSqlStringParameterName("pIsPublic") +
                     " , " + DALManager.BuildSqlStringParameterName("pUserId") +
                     " , " + DALManager.BuildSqlStringParameterName("pFormGroup") +
                     " , " + DALManager.BuildSqlStringParameterName("pCOESearchCriteria") +
                     " , " + DALManager.BuildSqlStringParameterName("pNumHits") +
                     " , " + DALManager.BuildSqlStringParameterName("pDateCreated") +
                     " , " + DALManager.BuildSqlStringParameterName("pDatabaseName") +
                     " , " + DALManager.BuildSqlStringParameterName("pDataViewId") + ")";

                OracleCommand dbCommand = (OracleCommand)DALManager.Database.GetSqlStringCommand(sql);

                dbCommand.Parameters.Add(new OracleParameter("pId", OracleDbType.Int32, id, ParameterDirection.Input));
                dbCommand.Parameters.Add(new OracleParameter("pName", OracleDbType.Varchar2, 1000, name, ParameterDirection.Input));
                dbCommand.Parameters.Add(new OracleParameter("pDescription", OracleDbType.Varchar2, 1000, description, ParameterDirection.Input));
                dbCommand.Parameters.Add(new OracleParameter("pIsPublic", OracleDbType.Varchar2, 1, Convert.ToInt16(isPublic), ParameterDirection.Input));
                dbCommand.Parameters.Add(new OracleParameter("pUserId", OracleDbType.Varchar2, 1000, userId.ToUpper(), ParameterDirection.Input));
                dbCommand.Parameters.Add(new OracleParameter("pFormGroup", OracleDbType.Int32, formgroup, ParameterDirection.Input));
                dbCommand.Parameters.Add(new OracleParameter("pCOESearchCriteria", OracleDbType.Clob, serializedCOESearchCriteria, ParameterDirection.Input));
                dbCommand.Parameters.Add(new OracleParameter("pNumHits", OracleDbType.Int32, numHits, ParameterDirection.Input));
                dbCommand.Parameters.Add(new OracleParameter("pDateCreated", OracleDbType.Date, DateTime.Now, ParameterDirection.Input));
                dbCommand.Parameters.Add(new OracleParameter("pDatabaseName", OracleDbType.Varchar2, 1000, databaseName, ParameterDirection.Input));
                dbCommand.Parameters.Add(new OracleParameter("pDataViewId", OracleDbType.Int32, dataviewId, ParameterDirection.Input));                

                _coeLog.LogStart(methodSignature + "insert searchCriteria: " + sql +
                                        "\npId: " + id.ToString() +
                                        "\npName: " + name +
                                        "\npDescription: " + description +
                                        "\npIsPublic: " + Convert.ToInt16(isPublic).ToString() +
                                        "\npUserId: " + userId.ToUpper() +
                                        "\npFormGroup: " + formgroup.ToString() +
                                        "\npCOESearchCriteria: " + serializedCOESearchCriteria +
                                        "\npNumHits: " + numHits.ToString() +
                                        "\npDateCreated: " + DateTime.Now.ToString() +
                                        "\npDatabaseName: " + databaseName +
                                        "\npDataViewId: " + dataviewId.ToString());

                int recordsAffected = DALManager.ExecuteNonQuery(dbCommand);
                _coeLog.LogEnd(methodSignature + "insert searchCriteria: " + sql);
                return id;
            }
            catch (Exception ex)
            {
                _coeLog.Log(methodSignature + ex.Message, 1, System.Diagnostics.SourceLevels.Error);

                throw;
            }
            finally
            {
                _coeLog.LogEnd(methodSignature);
            }
        }
        public override int NewSavedSearchCriteriaFromTemp(string name, string description, string userId, int numHits, bool isPublic, int formGroup, SearchCriteria coeSearchCriteria, string databaseName, int dataviewId)
        {
            try
            {
                int savedSearchCriteriaId = GetNewSavedSearchCriteriaId();

                if (name == string.Empty) 
                name = "SAVED";
                if (description == string.Empty) 
                description = "SAVED Search Criteria";
                string serializedCOESearchCriteria = COESearchCriteriaUtilities.SerializeCOESearchCriteria(coeSearchCriteria);

                string sql = "INSERT INTO " + _savedSearchCriteriaTableName +
                            "(ID,NAME,DESCRIPTION,USER_ID,NUM_HITS,IS_PUBLIC,FORMGROUP,DATE_CREATED,COESEARCHCRITERIA,DATABASE,DATAVIEW_ID) VALUES " +
                            "(" + DALManager.BuildSqlStringParameterName("pSavedSearchCriteriaId") +
                            "," + DALManager.BuildSqlStringParameterName("pName") +
                            "," + DALManager.BuildSqlStringParameterName("pDescription") +
                            "," + DALManager.BuildSqlStringParameterName("pUserId") +
                            "," + DALManager.BuildSqlStringParameterName("pNumHits") +
                            "," + DALManager.BuildSqlStringParameterName("pIsPublic") +
                            "," + DALManager.BuildSqlStringParameterName("pFormGroup") +
                            "," + DALManager.BuildSqlStringParameterName("pDateCreated") +
                            "," + DALManager.BuildSqlStringParameterName("pCOESearchCriteria") +
                            "," + DALManager.BuildSqlStringParameterName("pDatabaseName") +
                            "," + DALManager.BuildSqlStringParameterName("pDataViewId") + ")";

                OracleCommand dbCommand = (OracleCommand)DALManager.Database.GetSqlStringCommand(sql);

                dbCommand.Parameters.Add(new OracleParameter("pSavedSearchCriteriaId", OracleDbType.Int32, savedSearchCriteriaId, ParameterDirection.Input));
                dbCommand.Parameters.Add(new OracleParameter("pName", OracleDbType.Varchar2, 1000, name, ParameterDirection.Input));
                dbCommand.Parameters.Add(new OracleParameter("pDescription", OracleDbType.Varchar2, 1000, description, ParameterDirection.Input));
                dbCommand.Parameters.Add(new OracleParameter("pUserId", OracleDbType.Varchar2, 1000, userId.ToUpper(), ParameterDirection.Input));
                dbCommand.Parameters.Add(new OracleParameter("pNumHits", OracleDbType.Int32, numHits, ParameterDirection.Input));
                dbCommand.Parameters.Add(new OracleParameter("pIsPublic", OracleDbType.Varchar2, 1, Convert.ToInt16(isPublic), ParameterDirection.Input));
                dbCommand.Parameters.Add(new OracleParameter("pFormGroup", OracleDbType.Int32, formGroup, ParameterDirection.Input));
                dbCommand.Parameters.Add(new OracleParameter("pDateCreated", OracleDbType.Date, DateTime.Now, ParameterDirection.Input));
                dbCommand.Parameters.Add(new OracleParameter("pCOESearchCriteria", OracleDbType.Clob, serializedCOESearchCriteria, ParameterDirection.Input));
                dbCommand.Parameters.Add(new OracleParameter("pDatabaseName", OracleDbType.Varchar2, 1000, databaseName, ParameterDirection.Input));
                dbCommand.Parameters.Add(new OracleParameter("pDataViewId", OracleDbType.Int32, dataviewId, ParameterDirection.Input));

                int recordsAffected = DALManager.ExecuteNonQuery(dbCommand);

                return savedSearchCriteriaId;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public override void Update(int id, string serializedCOESearchCriteria, string name, string description, bool isPublic, int numHits, string databaseName, SearchCriteriaType searchCriteriaType, int formGroup, int dataviewId)
        {
            string tableName;


            if (searchCriteriaType == SearchCriteriaType.SAVED)
            {
                tableName = _savedSearchCriteriaTableName;
            }
            else
            {
                tableName = _coeSearchCriteriaTableName;
            }

            try
            {

                string sql = "UPDATE " + tableName +
                    " SET NAME = " + DALManager.BuildSqlStringParameterName("pName") + "," +
                    " DESCRIPTION = " + DALManager.BuildSqlStringParameterName("pDescription") + "," +
                    " NUM_HITS = " + DALManager.BuildSqlStringParameterName("pNumHits") + "," +
                    " IS_PUBLIC = " + DALManager.BuildSqlStringParameterName("pIsPublic") + "," +
                    " FORMGROUP = " + DALManager.BuildSqlStringParameterName("pFormGroup") + "," +
                    " COESEARCHCRITERIA = " + DALManager.BuildSqlStringParameterName("pCOESearchCriteria") + "," +
                    " DATABASE = " + DALManager.BuildSqlStringParameterName("pDatabase") + "," +
                    " DATAVIEW_ID = " + DALManager.BuildSqlStringParameterName("pDataViewId") +
                    " WHERE ID = " + DALManager.BuildSqlStringParameterName("pId");

                OracleCommand dbCommand = (OracleCommand)DALManager.Database.GetSqlStringCommand(sql);

                dbCommand.Parameters.Add(new OracleParameter("pName", OracleDbType.Varchar2, 1000, name, ParameterDirection.Input));
                dbCommand.Parameters.Add(new OracleParameter("pDescription", OracleDbType.Varchar2, 1000, description, ParameterDirection.Input));
                dbCommand.Parameters.Add(new OracleParameter("pNumHits", OracleDbType.Int32, numHits, ParameterDirection.Input));
                dbCommand.Parameters.Add(new OracleParameter("pIsPublic", OracleDbType.Varchar2, 1, Convert.ToInt16(isPublic), ParameterDirection.Input));
                dbCommand.Parameters.Add(new OracleParameter("pFormGroup", OracleDbType.Int32, formGroup, ParameterDirection.Input));
                dbCommand.Parameters.Add(new OracleParameter("pCOESearchCriteria", OracleDbType.Clob, serializedCOESearchCriteria, ParameterDirection.Input));
                dbCommand.Parameters.Add(new OracleParameter("pDatabaseName", OracleDbType.Varchar2, 1000, databaseName, ParameterDirection.Input));
                dbCommand.Parameters.Add(new OracleParameter("pDataViewId", OracleDbType.Int32, dataviewId, ParameterDirection.Input));
                dbCommand.Parameters.Add(new OracleParameter("pId", OracleDbType.Int32, id, ParameterDirection.Input));

                int recordsAffected = DALManager.ExecuteNonQuery(dbCommand);


            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public override SafeDataReader GetTempSearchCriterias(string userId, int dataViewId, int formGroup, string databaseName, int quantityToRetrieve)
        {
            try
            {
                string sql = "SELECT * FROM (SELECT * FROM " + _coeSearchCriteriaTableName +
                       " WHERE (USER_ID = " + DALManager.BuildSqlStringParameterName("pUserId") + " OR IS_PUBLIC =1)" +
                       " AND (DATAVIEW_ID = " + DALManager.BuildSqlStringParameterName("pDataViewId") +
                       " AND FORMGROUP = " + DALManager.BuildSqlStringParameterName("pFormGroup") + 
                       " AND DATABASE = " + DALManager.BuildSqlStringParameterName("pDatabase") + ")" +
                       " ORDER BY ID DESC) WHERE ROWNUM <= " + DALManager.BuildSqlStringParameterName("pQuantityToRetrieve");


                DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sql);

                DALManager.Database.AddParameter(dbCommand, "pUserId", DbType.AnsiString, 1000, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, userId.ToUpper());
                DALManager.Database.AddInParameter(dbCommand, "pDataViewId", DbType.Int32, dataViewId);
                DALManager.Database.AddInParameter(dbCommand, "pFormGroup", DbType.Int32, formGroup);
                DALManager.Database.AddParameter(dbCommand, "pDatabase", DbType.AnsiString, 1000, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, databaseName);
                DALManager.Database.AddInParameter(dbCommand, "pQuantityToRetrieve", DbType.Int32, quantityToRetrieve);

                SafeDataReader safeReader = new SafeDataReader(DALManager.Database.ExecuteReader(dbCommand));
                return safeReader;
            }
            catch
            {
                throw;
            }
        }

        internal override COECacheDependency GetCacheDependency(int id, SearchCriteriaType searchCriteriaType)
        {
            string tablename;
            switch (searchCriteriaType)
            {
                case SearchCriteriaType.SAVED:
                    tablename = _savedSearchCriteriaTableName;
                    break;
                default:
                    tablename = _coeSearchCriteriaTableName;
                    break;
            }

            OracleConnection conn = DALManager.Database.CreateConnection() as OracleConnection;

            string sql = "SELECT rowid FROM " + tablename +
            " WHERE ID=" + DALManager.BuildSqlStringParameterName("pId");
            OracleCommand cmd = new OracleCommand(sql, conn);
            cmd.Parameters.Add(new OracleParameter(DALManager.BuildSqlStringParameterName("pId"), OracleDbType.Int32, id, ParameterDirection.Input));

            OracleCacheDependency dependency = new OracleCacheDependency(cmd);
            return dependency;

        }
    }
}
