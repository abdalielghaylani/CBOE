using System;
using System.Collections.Generic;
using System.Text;
using CambridgeSoft.COE.Framework;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.Types;
using CambridgeSoft.COE.Framework.Properties;
using System.Data;
using System.Data.Common;
using System.Xml;
using System.Xml.Serialization;
using Csla.Security;
using Csla.Data;
using CambridgeSoft.COE.Framework.COELoggingService;
using System.Reflection;


namespace CambridgeSoft.COE.Framework.COESearchCriteriaService
{
    public class DAL : DALBase
    {
        public string _coeSearchCriteriaTableName = string.Empty;
        public string _savedSearchCriteriaTableName = string.Empty;
        [NonSerialized]
        private string _serviceName = "COESearchCriteria";
        [NonSerialized]
        protected static COELog _coeLog = COELog.GetSingleton("COESearchCriteria");


        /// <summary>
        /// to get the name of the table
        /// </summary>
        public override void SetServiceSpecificVariables()
        {
            try
            {
                string owner = this.DALManager.DatabaseData.OriginalOwner.ToString();
                COESearchCriteriaUtilities.BuildSearchCriteriaTableName(owner, ref _coeSearchCriteriaTableName);
                COESearchCriteriaUtilities.BuildSavedSearchCriteriaTable(owner, ref _savedSearchCriteriaTableName);

            }
            catch (Exception)
            {

                throw;
            }
        }
        public virtual int NewSavedSearchCriteriaFromTemp(string name, string description, string userId, int numHits, bool isPublic, int formGroup, SearchCriteria coeSearchCriteria, string databaseName, int dataviewId)
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

                DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sql);

                DALManager.Database.AddInParameter(dbCommand, "pSavedSearchCriteriaId", DbType.Int32, savedSearchCriteriaId);
                DALManager.Database.AddParameter(dbCommand, "pName", DbType.AnsiString, 1000, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, name);
                DALManager.Database.AddParameter(dbCommand, "pDescription", DbType.AnsiString, 1000, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, description);
                DALManager.Database.AddParameter(dbCommand, "pUserId", DbType.AnsiString, 1000, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, userId.ToUpper());
                DALManager.Database.AddInParameter(dbCommand, "pNumHits", DbType.Int32, numHits);
                DALManager.Database.AddParameter(dbCommand, "pIsPublic", DbType.AnsiString, 1, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, Convert.ToInt16(isPublic));
                DALManager.Database.AddInParameter(dbCommand, "pFormGroup", DbType.Int32, formGroup);
                DALManager.Database.AddInParameter(dbCommand, "pDateCreated", DbType.Date, DateTime.Now);
                DALManager.Database.AddParameter(dbCommand, "pCOESearchCriteria", DbType.AnsiString, 1000, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, coeSearchCriteria);
                DALManager.Database.AddParameter(dbCommand, "pDatabaseName", DbType.AnsiString, 1000, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, databaseName);
                DALManager.Database.AddInParameter(dbCommand, "pDataViewId", DbType.Int32, dataviewId);
                

                int recordsAffected = DALManager.ExecuteNonQuery(dbCommand);

                return savedSearchCriteriaId;

            }
            catch
            {
                throw;
            }
        }


        /// <summary>
        /// Inserts a new record in the coedataview table and returns its unique id,
        /// </summary>
        /// <returns>the new id generated</returns>
        public virtual int GetNewID()
        {
            //this has to be overridden so throw an error if there is not override
            return -1;

        }

        public virtual int GetNewSavedSearchCriteriaId()
        {
            //this has to be overridden so throw an error if there is not override
            return -1;
        }

        /// <summary>
        /// creates a new record in the coedataview table
        /// </summary>
        /// <param name="formgroup">the id of the formgroup</param>
        /// <param name="name">the name of the dataview</param>
        /// <param name="description">the description of the dataview</param>
        /// <param name="isPublic">whether the dataview is available as public</param>
        /// <param name="userId">the userid of the current user logged in</param>
        /// <param name="serializedCOESearchCriteria">the serialized/xml form of the coedataview object</param>
        /// <returns>the dataview id</returns>
        public virtual int Insert(int formGroup, string name, bool isPublic, string description, string userId, SearchCriteria _coeSearchCriteria, int numHits, string databaseName, int dataviewId)
        {
            try
            {
                _coeLog.LogStart(MethodBase.GetCurrentMethod().Name.ToUpper() + " - " + MethodBase.GetCurrentMethod().DeclaringType.Name);

                _coeLog.LogStart(MethodBase.GetCurrentMethod().Name.ToUpper() + " - " + MethodBase.GetCurrentMethod().DeclaringType.Name + "Get new Id");
                //retrieving the id to be inserted
                int id = GetNewID();
                _coeLog.LogEnd(MethodBase.GetCurrentMethod().Name.ToUpper() + " - " + MethodBase.GetCurrentMethod().DeclaringType.Name + "Get new Id");

                _coeSearchCriteria.SearchCriteriaID = id;
                string serializedCOESearchCriteria = COESearchCriteriaUtilities.SerializeCOESearchCriteria(_coeSearchCriteria);


                //inserting the data in the database.
                string sql = "INSERT INTO " + _coeSearchCriteriaTableName +
                     "(ID,NAME,DESCRIPTION,IS_PUBLIC,USER_ID," +
                     "FORMGROUP,COESEARCHCRITERIA,NUM_HITS, DATE_CREATED,DATABASE)VALUES " +
                     "( " + DALManager.BuildSqlStringParameterName("pId") +
                     " ," + DALManager.BuildSqlStringParameterName("pName") +
                     " , " + DALManager.BuildSqlStringParameterName("pDescription") +
                     " , " + DALManager.BuildSqlStringParameterName("pIsPublic") +
                     " , " + DALManager.BuildSqlStringParameterName("pUserId") +
                     " , " + DALManager.BuildSqlStringParameterName("pFormGroup") +
                     " , " + DALManager.BuildSqlStringParameterName("pCOESearchCriteria") +
                      " ," + DALManager.BuildSqlStringParameterName("pNumHits") +
                     " , " + DALManager.BuildSqlStringParameterName("pDateCreated") +
                     " , " + DALManager.BuildSqlStringParameterName("pDatabaseName") +
                     " , " + DALManager.BuildSqlStringParameterName("pDataViewId") + ")";

                DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sql);

                DALManager.Database.AddInParameter(dbCommand, "pId", DbType.Int32, id);
                DALManager.Database.AddParameter(dbCommand, "pName", DbType.AnsiString, 1000, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, name);
                DALManager.Database.AddParameter(dbCommand, "pDescription", DbType.AnsiString, 1000, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, description);
                DALManager.Database.AddParameter(dbCommand, "pIsPublic", DbType.AnsiString, 1, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, Convert.ToInt16(isPublic));
                DALManager.Database.AddParameter(dbCommand, "pUserId", DbType.AnsiString, 1000, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, userId.ToUpper());
                DALManager.Database.AddInParameter(dbCommand, "pFormGroup", DbType.Int32, formGroup);
                DALManager.Database.AddParameter(dbCommand, "pCOESearchCriteria", DbType.AnsiString, 1000, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, serializedCOESearchCriteria);
                DALManager.Database.AddInParameter(dbCommand, "pNumHits", DbType.Int32, numHits);
                DALManager.Database.AddInParameter(dbCommand, "pDateCreated", DbType.Date, DateTime.Now);
                DALManager.Database.AddParameter(dbCommand, "pDatabaseName", DbType.AnsiString, 1000, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, databaseName);
                DALManager.Database.AddInParameter(dbCommand, "pDataViewId", DbType.Int32, dataviewId);
                _coeLog.LogStart(MethodBase.GetCurrentMethod().Name.ToUpper() + " - " + MethodBase.GetCurrentMethod().DeclaringType.Name + "insert searchCriteria: " + sql);
                int recordsAffected = DALManager.ExecuteNonQuery(dbCommand);
                _coeLog.LogEnd(MethodBase.GetCurrentMethod().Name.ToUpper() + " - " + MethodBase.GetCurrentMethod().DeclaringType.Name + "insert searchCriteria: " + sql);
                return id;
            }
            catch (Exception ex)
            {
                _coeLog.Log(MethodBase.GetCurrentMethod().Name.ToUpper() + " - " + MethodBase.GetCurrentMethod().DeclaringType.Name + " - " + ex.Message);

                throw;
            }
            finally
            {
                _coeLog.LogEnd(MethodBase.GetCurrentMethod().Name.ToUpper() + " - " + MethodBase.GetCurrentMethod().DeclaringType.Name);
            }
        }



        /// <summary>
        /// deletes a dataview record containing a coeSearchCriteria
        /// </summary>
        /// <param name="dataViewID">a dataview id</param>
        public void Delete(int id)
        {
            try
            {

                string sql = null;
                DbCommand dbCommand = null;

                sql = "DELETE FROM " + _coeSearchCriteriaTableName +
                " WHERE ID =" + DALManager.BuildSqlStringParameterName("pId");

                dbCommand = DALManager.Database.GetSqlStringCommand(sql);

                DALManager.Database.AddInParameter(dbCommand, "pId", DbType.Int32, id);
                DALManager.ExecuteNonQuery(dbCommand);
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// deletes a dataview record containing a coeSearchCriteria
        /// </summary>
        /// <param name="dataViewID">a dataview id</param>
        public void DeleteSaved(int id)
        {
            try
            {

                string sql = null;
                DbCommand dbCommand = null;

                sql = "DELETE FROM " + _savedSearchCriteriaTableName +
                " WHERE ID =" + DALManager.BuildSqlStringParameterName("pId");

                dbCommand = DALManager.Database.GetSqlStringCommand(sql);

                DALManager.Database.AddInParameter(dbCommand, "pId", DbType.Int32, id);
                DALManager.ExecuteNonQuery(dbCommand);
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// empties the table in the database.
        /// </summary>
        public void DeleteAll()
        {
            try
            {

                string sql = "DELETE FROM " + _coeSearchCriteriaTableName;
                DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sql);
                DALManager.ExecuteNonQuery(dbCommand);
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// deletes all dataviews for a user
        /// </summary>
        /// <param name="userName">the user name</param>
        public void DeleteUserSearchCriteria(string userName)
        {
            try
            {

                string sql = "DELETE FROM " + _coeSearchCriteriaTableName +
                    " WHERE LTRIM(RTRIM(USER_ID))=LTRIM(RTRIM(" + DALManager.BuildSqlStringParameterName("pUserName") + "))";
                DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sql);
                DALManager.Database.AddParameter(dbCommand, "pUserName", DbType.AnsiString, 1000, ParameterDirection.Input, false, 0, 0, string.Empty, DataRowVersion.Current, userName.ToUpper());
                DALManager.ExecuteNonQuery(dbCommand);
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// gets all dataview data for a user
        /// </summary>
        /// <param name="userName">the user name</param>
        /// <returns>a safedatareader containing all dataviews for a user </returns>
        public SafeDataReader GetUserSearchCriterias(string userName)
        {
            try
            {

                string sql = null;
                DbCommand dbCommand = null;
                sql = "SELECT * FROM " + _coeSearchCriteriaTableName +
                    " WHERE LTRIM(RTRIM(USER_ID))=LTRIM(RTRIM(" + DALManager.BuildSqlStringParameterName("pUserName") + "))";
                dbCommand = DALManager.Database.GetSqlStringCommand(sql);
                DALManager.Database.AddParameter(dbCommand, "pUserName", DbType.AnsiString, 1000, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, userName.ToUpper());

                SafeDataReader safeReader = new SafeDataReader(DALManager.Database.ExecuteReader(dbCommand));
                return safeReader;
            }
            catch (Exception)
            {

                throw;
            }

        }



        /// <summary>
        /// returns dataviews filtered by isPublic flag
        /// </summary>
        /// <param name="isPublic">whether the dataview is public or private</param>
        /// <returns>a safedatareader containing the dataviews</returns>
        public SafeDataReader GetAll(bool isPublic)
        {
            try
            {

                string sql = "SELECT * FROM " + _coeSearchCriteriaTableName +
                    " WHERE LTRIM(RTRIM(IS_PUBLIC))=LTRIM(RTRIM(" + DALManager.BuildSqlStringParameterName("pIsPublic") + "))";
                DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sql);
                DALManager.Database.AddParameter(dbCommand, "pIsPublic", DbType.AnsiString, 1, ParameterDirection.Input, false, 0, 0, string.Empty, DataRowVersion.Current, Convert.ToInt16(isPublic));

                SafeDataReader safeReader = new SafeDataReader(DALManager.Database.ExecuteReader(dbCommand));
                return safeReader;
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// get all dataviews
        /// </summary>
        /// <returns>a safedatareader containing the dataviews</returns>
        public SafeDataReader GetAll()
        {
            try
            {

                string sql = "SELECT * FROM " + _coeSearchCriteriaTableName;
                DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sql);
                SafeDataReader safeReader = new SafeDataReader(DALManager.Database.ExecuteReader(dbCommand));
                return safeReader;
            }
            catch (Exception)
            {

                throw;
            }
        }




        public virtual SafeDataReader GetSavedSearchCriteria(string userId, int dataViewId, string databaseName)
        {

            try
            {
                string sql = "SELECT * FROM " + _savedSearchCriteriaTableName +
                       " WHERE (USER_ID = " + DALManager.BuildSqlStringParameterName("pUserId") + " OR IS_PUBLIC =1)" +
                       " AND (DATAVIEW_ID = " + DALManager.BuildSqlStringParameterName("pDataViewId") +                     
                       " AND DATABASE = " + DALManager.BuildSqlStringParameterName("pDatabase") + ")" +
                       " ORDER BY ID DESC";

                DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sql);


                DALManager.Database.AddParameter(dbCommand, "pUserId", DbType.AnsiString, 1000, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, userId.ToUpper());
                DALManager.Database.AddInParameter(dbCommand, "pDataViewId", DbType.Int32, dataViewId);
                DALManager.Database.AddParameter(dbCommand, "pDatabase", DbType.AnsiString, 1000, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, databaseName);

                SafeDataReader safeReader = new SafeDataReader(DALManager.Database.ExecuteReader(dbCommand));
                return safeReader;
            }
            catch(Exception exception)
            {
                throw exception;
            }
        }

        public virtual SafeDataReader GetSavedSearchCriteria(string userId, int dataViewId, int formGroup, string databaseName)
        {            

            try
            {
                string sql = "SELECT * FROM " + _savedSearchCriteriaTableName +
                      " WHERE (USER_ID = " + DALManager.BuildSqlStringParameterName("pUserId") + " OR IS_PUBLIC =1)" +
                      " AND (DATAVIEW_ID = " + DALManager.BuildSqlStringParameterName("pDataViewId") +
                      " AND FORMGROUP = " + DALManager.BuildSqlStringParameterName("pFormGroup") +
                      " AND DATABASE = " + DALManager.BuildSqlStringParameterName("pDatabase") + ")" +                    
                      " ORDER BY ID DESC";

                DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sql);


                DALManager.Database.AddParameter(dbCommand, "pUserId", DbType.AnsiString, 1000, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, userId.ToUpper());
                DALManager.Database.AddInParameter(dbCommand, "pDataViewId", DbType.Int32, dataViewId);
                DALManager.Database.AddInParameter(dbCommand, "pFormGroup", DbType.Int32, formGroup);
                DALManager.Database.AddParameter(dbCommand, "pDatabase", DbType.AnsiString, 1000, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, databaseName);

                SafeDataReader safeReader = new SafeDataReader(DALManager.Database.ExecuteReader(dbCommand));
                return safeReader;
            }
            catch
            {
                throw;
            }
        }

        public virtual SafeDataReader GetTempSearchCriterias(string userId, int dataViewId, string databaseName, int quantityToRetrieve)
        {

            try
            {
                string sql = "SELECT * FROM (SELECT * FROM " + _coeSearchCriteriaTableName +
                            " WHERE (USER_ID = " + DALManager.BuildSqlStringParameterName("pUserId") + " OR IS_PUBLIC =1)" +
                            " AND (DATAVIEW_ID = " + DALManager.BuildSqlStringParameterName("pDataViewId") +
                            " AND DATABASE = " + DALManager.BuildSqlStringParameterName("pDatabase") + ")" +
                            " ORDER BY ID DESC)" + 
                            " WHERE ROWNUM <= " + DALManager.BuildSqlStringParameterName("pQuantityToRetrieve");


                DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sql);

                DALManager.Database.AddParameter(dbCommand, "pUserId", DbType.AnsiString, 1000, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, userId.ToUpper());
                DALManager.Database.AddInParameter(dbCommand, "pDataViewId", DbType.Int32, dataViewId);
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

        public virtual SafeDataReader GetTempSearchCriterias(string userId, int dataViewId, int formGroup, string databaseName, int quantityToRetrieve)
        {

            throw new Exception("Operation not implemented Exception");
        }

        /// <summary>
        /// update a dataview
        /// </summary>
        /// <param name="dataViewId">dataviewid for which update in done</param>
        /// <param name="dataView">the updated xml form of the dataview</param>
        /// <param name="dataViewName">the updated name of the dataview</param>
        /// <param name="description">the updated description of the dataview</param>
        /// <param name="isPublic">whether the dataview is made public or not</param>
        public virtual void Update(int id, string serializedCOESearchCriteria, string name, string description, bool isPublic, int numHits, string databaseName, SearchCriteriaType searchCriteriaType, int formGroup, int dataviewId)
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
                    " SET NAME= " + DALManager.BuildSqlStringParameterName("pName") + "," +
                    " DESCRIPTION=" + DALManager.BuildSqlStringParameterName("pDescription") + "," +
                    " NUM_HITS=" + DALManager.BuildSqlStringParameterName("pNumHits") + "," +
                    " IS_PUBLIC= " + DALManager.BuildSqlStringParameterName("pIsPublic") + "," +
                    " FORMGROUP=" + DALManager.BuildSqlStringParameterName("pFormGroup") + "," +
                    " COESEARCHCRITERIA=" + DALManager.BuildSqlStringParameterName("pCOESearchCriteria") + "," +
                    " DATABASE=" + DALManager.BuildSqlStringParameterName("pDatabase") + "," +
                    " DATAVIEW_ID=" + DALManager.BuildSqlStringParameterName("pDataViewId") + "," +
                    " WHERE ID=" + DALManager.BuildSqlStringParameterName("pId");

                DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sql);


                //now setting the values for the parameters.
                
                DALManager.Database.AddParameter(dbCommand, "pName", DbType.AnsiString, 1000, ParameterDirection.Input, false, 0, 0, string.Empty, DataRowVersion.Current, name);
                DALManager.Database.AddParameter(dbCommand, "pDescription", DbType.AnsiString, 1000, ParameterDirection.Input, false, 0, 0, string.Empty, DataRowVersion.Current, description);
                DALManager.Database.AddInParameter(dbCommand, "pNumHits", DbType.Int32, numHits);
                DALManager.Database.AddParameter(dbCommand, "pIsPublic", DbType.AnsiString, 1, ParameterDirection.Input, false, 0, 0, string.Empty, DataRowVersion.Current, Convert.ToInt16(isPublic));
                DALManager.Database.AddInParameter(dbCommand, "pFormGroup", DbType.Int32, formGroup);
                DALManager.Database.AddParameter(dbCommand, "pCOESearchCriteria", DbType.AnsiString, 1000, ParameterDirection.Input, false, 0, 0, string.Empty, DataRowVersion.Current, serializedCOESearchCriteria);
                DALManager.Database.AddParameter(dbCommand, "pDatabase", DbType.AnsiString, 1000, ParameterDirection.Input, false, 0, 0, string.Empty, DataRowVersion.Current, databaseName);
                DALManager.Database.AddInParameter(dbCommand, "pDataViewId", DbType.Int32, dataviewId);                
                DALManager.Database.AddInParameter(dbCommand, "pId", DbType.Int32, id);
                DALManager.ExecuteNonQuery(dbCommand);
            }
            catch (Exception)
            {

                throw;
            }


        }


        /// <summary>
        /// get a dataview
        /// </summary>
        /// <param name="id">the id for the dataview</param>
        /// <returns>a safedatareader containing the dataview</returns>
        public SafeDataReader Get(int id, SearchCriteriaType searchCriteriaType)
        {
            string tablename;
            switch (searchCriteriaType) 
            {
                case SearchCriteriaType.TEMP:
                    tablename = _coeSearchCriteriaTableName;
                    break;
                case SearchCriteriaType.SAVED:
                    tablename = _savedSearchCriteriaTableName;
                    break;
                default:
                    tablename = string.Empty;
                    break;
            }

            try
            {

                string sql = "SELECT * FROM " + tablename +
                    " WHERE ID=" + DALManager.BuildSqlStringParameterName("pId");

                DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sql);                
                DALManager.Database.AddInParameter(dbCommand, "pId", DbType.Int32, id);

                SafeDataReader safeReader = new SafeDataReader(DALManager.Database.ExecuteReader(dbCommand));
                return safeReader;
            }
            catch (Exception)
            {

                throw;
            }

        }



        internal virtual CambridgeSoft.COE.Framework.Caching.COECacheDependency GetCacheDependency(int _id, SearchCriteriaType searchCriteriaType)
        {
            return null;
        }
    }
}
