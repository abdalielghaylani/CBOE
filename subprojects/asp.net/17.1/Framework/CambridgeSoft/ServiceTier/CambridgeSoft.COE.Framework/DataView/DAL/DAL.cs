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
using CambridgeSoft.COE.Framework.COELoggingService;
using CambridgeSoft.COE.Framework.Caching;
using CambridgeSoft.COE.Framework.ExceptionHandling;

namespace CambridgeSoft.COE.Framework.COEDataViewService
{
    public class DAL : DALBase
    {
        [NonSerialized]
        static COELog _coeLog = COELog.GetSingleton("COEDataView");
        protected string _coeDataViewTableName = string.Empty;
        protected string _dvAlias = string.Empty;

        /// <summary>
        /// to get the name of the table
        /// </summary>
        public override void SetServiceSpecificVariables()
        {
            try
            {
                string owner = this.DALManager.DatabaseData.OriginalOwner.ToString();
                COEDataViewUtilities.BuildDataViewTableName(owner, ref _coeDataViewTableName);
                _dvAlias = "dv1";
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleDALException(ex,null);//At this scenario, ExceptionContext object can be null.
            }                                                      // We don't have dbcommand parameter used.
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
        public virtual int Insert(int formgroup, string name, bool isPublic, string description, string userId, COEDataView coeDataView, string databaseName, int id, string application)
        {
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
            DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sql);

            DALManager.Database.AddInParameter(dbCommand, "pId", DbType.Int32, id);
            DALManager.Database.AddParameter(dbCommand, "pName", DbType.AnsiString, 1000, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, name);
            DALManager.Database.AddParameter(dbCommand, "pDescription", DbType.AnsiString, 1000, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, description);
            DALManager.Database.AddParameter(dbCommand, "pIsPublic", DbType.AnsiString, 1, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, Convert.ToInt16(isPublic));
            DALManager.Database.AddParameter(dbCommand, "pUserId", DbType.AnsiString, 1000, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, userId);
            DALManager.Database.AddParameter(dbCommand, "pFormGroup", DbType.AnsiString, 1000, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, formgroup);
            DALManager.Database.AddParameter(dbCommand, "pCOEDataView", DbType.AnsiString, Int32.MaxValue, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, serializedCOEDataView);//Changed by Sumeet, (1000 -> Int32.MaxValue)So that test XML can be used
            DALManager.Database.AddInParameter(dbCommand,"pDateCreated", DbType.Date, DateTime.Now);
            DALManager.Database.AddParameter(dbCommand, "pDatabaseName", DbType.AnsiString, 1000, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, databaseName);
            DALManager.Database.AddParameter(dbCommand, "pApplication", DbType.AnsiString, 1000, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, application);


            int recordsAffected = -1;
            try
            {
               recordsAffected = DALManager.ExecuteNonQuery(dbCommand);
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.DispatchDALException(ex, dbCommand);//At this scenario, as we required ExceptionContext object 
            }                                                              //and as we have to pass dbcommand parameter, which is used here
            return id;                                                     //As suggested i have moved the code to DispatchDALException mehod.
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
        public virtual int Insert(int formgroup, string name, bool isPublic, string description, string userId, COEDataViewManagerBO coeDataView, string databaseName, int id, string application)
        {
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
            DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sql);

            DALManager.Database.AddInParameter(dbCommand, "pId", DbType.Int32, id);
            DALManager.Database.AddParameter(dbCommand, "pName", DbType.AnsiString, 1000, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, name);
            DALManager.Database.AddParameter(dbCommand, "pDescription", DbType.AnsiString, 1000, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, description);
            DALManager.Database.AddParameter(dbCommand, "pIsPublic", DbType.AnsiString, 1, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, Convert.ToInt16(isPublic));
            DALManager.Database.AddParameter(dbCommand, "pUserId", DbType.AnsiString, 1000, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, userId);
            DALManager.Database.AddParameter(dbCommand, "pFormGroup", DbType.AnsiString, 1000, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, formgroup);
            DALManager.Database.AddParameter(dbCommand, "pCOEDataView", DbType.AnsiString, Int32.MaxValue, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, serializedCOEDataView);//Changed by Sumeet, (1000 -> Int32.MaxValue)So that test XML can be used
            DALManager.Database.AddInParameter(dbCommand, "pDateCreated", DbType.Date, DateTime.Now);
            DALManager.Database.AddParameter(dbCommand, "pDatabaseName", DbType.AnsiString, 1000, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, databaseName);
            DALManager.Database.AddParameter(dbCommand, "pApplication", DbType.AnsiString, 1000, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, application);


            int recordsAffected = -1;
            try
            {
                recordsAffected = DALManager.ExecuteNonQuery(dbCommand);
            }
            catch (Exception ex)
            {
               COEExceptionDispatcher.DispatchDALException(ex, dbCommand);
            }
            return id;
        }

        /// <summary>
        /// deletes a dataview record containing a coeDataView
        /// </summary>
        /// <param name="dataViewID">a dataview id</param>
        public void Delete(int id)
        {
            string sql = null;
            DbCommand dbCommand = null;
            sql = "DELETE FROM " + _coeDataViewTableName +
            " WHERE ID =" + DALManager.BuildSqlStringParameterName("pId");

            dbCommand = DALManager.Database.GetSqlStringCommand(sql);

            DALManager.Database.AddInParameter(dbCommand, "pId", DbType.Int32, id);
           
            int recordsAffected = -1;
            try
            {
                recordsAffected = DALManager.ExecuteNonQuery(dbCommand);
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.DispatchDALException(ex, dbCommand);
            }
        }

        /// <summary>
        /// empties the table in the database.
        /// </summary>
        public void DeleteAll()
        {
            string sql = "DELETE FROM " + _coeDataViewTableName;
            DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sql);
            int recordsAffected = -1;
            try
            {
                recordsAffected = DALManager.ExecuteNonQuery(dbCommand);
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.DispatchDALException(ex, dbCommand);
            }
        }

        /// <summary>
        /// deletes all dataviews for a user
        /// </summary>
        /// <param name="userName">the user name</param>
        public void DeleteUserDataView(string userName)
        {
            string sql = "DELETE FROM " + _coeDataViewTableName +
                " WHERE LTRIM(RTRIM(USER_ID))=LTRIM(RTRIM(" + DALManager.BuildSqlStringParameterName("pUserName") + "))";
            DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sql);
            DALManager.Database.AddParameter(dbCommand, "pUserName", DbType.AnsiString, 1000, ParameterDirection.Input, false, 0, 0, string.Empty, DataRowVersion.Current, userName.ToUpper());
            int recordsAffected = -1;
            try
            {
                recordsAffected = DALManager.ExecuteNonQuery(dbCommand);
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.DispatchDALException(ex, dbCommand);
            }
        }


        /// <summary>
        /// gets all dataview data for a user
        /// </summary>
        /// <param name="userName">the user name</param>
        /// <returns>a safedatareader containing all dataviews for a user </returns>
        public virtual SafeDataReader GetUserDataViews(string userName)
        {
            DbCommand dbCommand = null;
            string sql = " select dv1.* from " + Resources.CentralizedStorageDB + ".coedataview dv1 where LTRIM(RTRIM(USER_ID))=LTRIM(RTRIM(" + DALManager.BuildSqlStringParameterName("pUserNameInParam") + ")) and" +
                        AccessPermissionsSQL(_dvAlias, _coeDataViewTableName)  +
                        GetApplicationFilter(_dvAlias);
            dbCommand = DALManager.Database.GetSqlStringCommand(sql);
            DALManager.Database.AddParameter(dbCommand, "pUserNameInParam", DbType.AnsiString, 1000, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, userName);
            DALManager.Database.AddInParameter(dbCommand, "pPersonID", DbType.Int32, COEUser.ID);
            DALManager.Database.AddParameter(dbCommand, "pUserName", DbType.AnsiString, 100, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, COEUser.Name.ToUpper());
            DALManager.Database.AddParameter(dbCommand, "pUserName1", DbType.AnsiString, 100, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, COEUser.Name.ToUpper());

            SafeDataReader safeReader=null;// I have moved this inorder to maintain consistency.and to get build.
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

        /// <summary>
        /// returns dataviews filtered by isPublic flag
        /// </summary>
        /// <param name="isPublic">whether the dataview is public or private</param>
        /// <returns>a safedatareader containing the dataviews</returns>
        public virtual SafeDataReader GetAll(bool isPublic)
        {
            //TODO: need to limit out put to datviews that COEUser.ID has access to.
            string sql = " select dv1.* from " + Resources.CentralizedStorageDB + ".coedataview dv1 where dv1.is_public=" + DALManager.BuildSqlStringParameterName("pIsPublic") +
                GetApplicationFilter(_dvAlias) +
                " and " + AccessPermissionsSQL(_dvAlias, _coeDataViewTableName);

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
        /// <summary>
        /// gets all dataview for a user
        /// </summary>
        /// <param name="userName">the user name</param>
        /// <returns>a safedatareader containing all dataviews for a user </returns>
        public virtual SafeDataReader GetAll(string userName)
        {          
            DbCommand dbCommand = null;
            string sql = " select dv1.* from " + Resources.CentralizedStorageDB + ".coedataview dv1 where LTRIM(RTRIM(USER_ID))=LTRIM(RTRIM(" + DALManager.BuildSqlStringParameterName("pUserNameInParam") + ")) " +
                GetApplicationFilter(_dvAlias) +
                "  and" + AccessPermissionsSQL(_dvAlias, _coeDataViewTableName);

            dbCommand = DALManager.Database.GetSqlStringCommand(sql);
            DALManager.Database.AddParameter(dbCommand, "pUserNameInParam", DbType.AnsiString, 1000, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, userName);
            DALManager.Database.AddInParameter(dbCommand,"pPersonID", DbType.Int32, COEUser.ID);
            DALManager.Database.AddParameter(dbCommand, "pUserName", DbType.AnsiString, 100, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, COEUser.Name.ToUpper());
            DALManager.Database.AddParameter(dbCommand, "pUserName1", DbType.AnsiString, 100, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, COEUser.Name.ToUpper());
            SafeDataReader safeReader=null;
            try
            {
                safeReader = new SafeDataReader(DALManager.Database.ExecuteReader(dbCommand));
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.DispatchDALException(ex,dbCommand);
            }
            return safeReader;
        }

        /// <summary>
        /// Get all dataviews that are not application specific
        /// </summary>
        /// <returns>a safedatareader containing the dataviews</returns>
        public virtual SafeDataReader GetAll()
        {
            string sql = "select dv1.* from " + Resources.CentralizedStorageDB + ".coedataview dv1 where dv1.is_public=1 " +
                GetApplicationFilter(_dvAlias) +
                " or "  + AccessPermissionsSQL(_dvAlias, _coeDataViewTableName);

            DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sql);
            DALManager.Database.AddInParameter(dbCommand, "pPersonID", DbType.Int32, COEUser.ID);
            DALManager.Database.AddParameter(dbCommand, "pUserName", DbType.AnsiString, 100, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, COEUser.Name.ToUpper());
            DALManager.Database.AddParameter(dbCommand, "pUserName1", DbType.AnsiString, 100, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, COEUser.Name.ToUpper());
            SafeDataReader safeReader = null;
            try
            {
                safeReader = new SafeDataReader(DALManager.ExecuteReader(dbCommand));
            }
            catch(Exception ex)
            {
                COEExceptionDispatcher.DispatchDALException(ex, dbCommand);
            }
            return safeReader;
        }

        internal virtual SafeDataReader GetAllNoMaster()
        {
            string sql = "select dv1.* from " + Resources.CentralizedStorageDB + ".coedataview dv1 where dv1.id > 0 and (dv1.is_public=1 " +
                GetApplicationFilter(_dvAlias) +
                " or " + AccessPermissionsSQL(_dvAlias, _coeDataViewTableName) + ")";

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
                throw ex;
            }
            return safeReader;
        }

        /// <summary>
        /// Get all dataviews for a given application
        /// </summary>
        /// <param name="appName">The application name</param>
        /// <returns>a safedatareader containing the dataviews</returns>
        public virtual SafeDataReader GetApplicationDataviews(string appName)
        {
            //TODO: need to limit out put to datviews that COEUser.ID has access to.
            string sql = " select dv1.* from " + Resources.CentralizedStorageDB + ".coedataview dv1 where dv1.is_public=1 and dv1.application = " +
                DALManager.BuildSqlStringParameterName("pAppName") + " " + GetApplicationNotNullFilter(_dvAlias) +
                " or " + AccessPermissionsSQL(_dvAlias, _coeDataViewTableName);


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
        /// get all dataviews without getting the actual COEDATAVIEW object
        /// </summary>
        /// <returns>a safedatareader containing the dataviews</returns>
        public virtual SafeDataReader GetAllLightWeight()
        {
            string sql = "select dv1.id, dv1.name, dv1.description, dv1.user_id, dv1.is_public, dv1.formgroup, dv1.date_created, dv1.database, dv1.application" +
                        " from " + Resources.CentralizedStorageDB + ".coedataview dv1 where dv1.is_public=1 " +
                        GetApplicationFilter(_dvAlias) +
                        " or " + AccessPermissionsSQL(_dvAlias, _coeDataViewTableName);

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

        internal virtual SafeDataReader GetAllNoMasterLightWeight()
        {
            string sql = "select dv1.id, dv1.name, dv1.description, dv1.user_id, dv1.is_public,dv1.formgroup, dv1.date_created, dv1.database, dv1.application" +
                        " from " + Resources.CentralizedStorageDB + ".coedataview dv1 where dv1.id > 0 and (dv1.is_public=1 " +
                        GetApplicationFilter(_dvAlias) +
                        " or " + AccessPermissionsSQL(_dvAlias, _coeDataViewTableName) + ")";

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
                throw ex;
            }
            return safeReader;
        }

        /// <summary>
        /// update a dataview
        /// </summary>
        /// <param name="dataViewId">dataviewid for which update in done</param>
        /// <param name="dataView">the updated xml form of the dataview</param>
        /// <param name="dataViewName">the updated name of the dataview</param>
        /// <param name="description">the updated description of the dataview</param>
        /// <param name="isPublic">whether the dataview is made public or not</param>
        public virtual void Update(int id, string serializedCOEDataView, string name, string description, bool isPublic, string databaseName, string application)
        {
            string sql="UPDATE " + _coeDataViewTableName +
                " SET NAME= " + DALManager.BuildSqlStringParameterName("pName") + "," +
                " DESCRIPTION=" + DALManager.BuildSqlStringParameterName("pDescription") + "," +
                " IS_PUBLIC= " + DALManager.BuildSqlStringParameterName("pIsPublic") + "," +
                " DATABASE= " + DALManager.BuildSqlStringParameterName("pDatabase") + "," +
                " COEDATAVIEW=" + DALManager.BuildSqlStringParameterName("pCOEDataView") + "," +
                " APPLICATION=" + DALManager.BuildSqlStringParameterName("pApplication") +
                " WHERE ID=" + DALManager.BuildSqlStringParameterName("pId");
            
            DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sql);


            //now setting the values for the parameters.
            DALManager.Database.AddParameter(dbCommand, "pName", DbType.AnsiString, 1000, ParameterDirection.Input, false, 0, 0, string.Empty, DataRowVersion.Current, name);
            DALManager.Database.AddParameter(dbCommand, "pDescription", DbType.AnsiString, 1000, ParameterDirection.Input, false, 0, 0, string.Empty, DataRowVersion.Current, description);
            DALManager.Database.AddParameter(dbCommand, "pIsPublic", DbType.AnsiString, 1, ParameterDirection.Input, false, 0, 0, string.Empty, DataRowVersion.Current, Convert.ToInt16(isPublic));
            DALManager.Database.AddParameter(dbCommand, "pDatabase", DbType.AnsiString, 1000, ParameterDirection.Input, false, 0, 0, string.Empty, DataRowVersion.Current, databaseName);
            DALManager.Database.AddParameter(dbCommand, "pCOEDataView", DbType.AnsiString, Int32.MaxValue, ParameterDirection.Input, false, 0, 0, string.Empty, DataRowVersion.Current, serializedCOEDataView);
            DALManager.Database.AddParameter(dbCommand, "pApplication", DbType.AnsiString, 1000, ParameterDirection.Input, false, 0, 0, string.Empty, DataRowVersion.Current, application);
            DALManager.Database.AddInParameter(dbCommand, "pId", DbType.Int32, id);
            int recordsAffected = -1;
            try
            {
                recordsAffected = DALManager.ExecuteNonQuery(dbCommand);
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.DispatchDALException(ex, dbCommand);
            }
        }

        /// <summary>
        /// get a dataview
        /// </summary>
        /// <param name="id">the id for the dataview</param>
        /// <returns>a safedatareader containing the dataview if the logged in users has access privileges for the dataview</returns>
        public virtual SafeDataReader Get(int id)
        {

            string sql = "select dv1.id, dv1.name, dv1.description, dv1.user_id, dv1.is_public, dv1.date_created,dv1.database, dv1.formgroup, dv1.coedataview, dv1.application " +
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

        internal bool CanGetDataview(int id)
        {
            string sql = "select count(dv1.id) " +
                        " from " + Resources.CentralizedStorageDB + ".coedataview dv1 where dv1.id=" + DALManager.BuildSqlStringParameterName("pId") + " AND (dv1.is_public=1 or " + AccessPermissionsSQL(_dvAlias, _coeDataViewTableName) + ")";

            DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sql);
            DALManager.Database.AddInParameter(dbCommand, "pId", DbType.Int32, id);
            DALManager.Database.AddInParameter(dbCommand, "pPersonID", DbType.Int32, COEUser.ID);
            DALManager.Database.AddParameter(dbCommand, "pUserName", DbType.AnsiString, 100, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, COEUser.Name.ToUpper());
            DALManager.Database.AddParameter(dbCommand, "pUserName1", DbType.AnsiString, 100, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, COEUser.Name.ToUpper());
            int count =0;
            try
            {
                count = Convert.ToInt32(DALManager.ExecuteScalar(dbCommand));
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.DispatchDALException(ex, dbCommand);
            }
            return count > 0;
        }

        /// <summary>
        /// get a dataview
        /// </summary>
        /// <param name="id">the id for the dataview</param>
        /// <returns>a safedatareader containing the dataview if the logged in users has access privileges for the dataview</returns>
        public virtual SafeDataReader Get(string dataviewName)
        {
            string sql = "select dv1.id, dv1.name, dv1.description, dv1.user_id, dv1.is_public, dv1.date_created,dv1.database, dv1.formgroup, dv1.coedataview, dv1.application " +
                        " from " + Resources.CentralizedStorageDB + ".coedataview dv1 where dv1.name=" + DALManager.BuildSqlStringParameterName("pName");

            DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sql);
            DALManager.Database.AddParameter(dbCommand, DALManager.BuildSqlStringParameterName("pName"), DbType.AnsiString, 255, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, dataviewName);

            SafeDataReader safeReader = null;
            try
            {
                safeReader = new SafeDataReader(DALManager.ExecuteReader(dbCommand));
            }
            catch(Exception ex)
            {
                COEExceptionDispatcher.DispatchDALException(ex, dbCommand);
            }
            return safeReader;
        }

        /// <summary>
        /// Base method meant to build a dependecy to be used with the caching mechanism. The dependency should be built against a single record in database.
        /// Must be overriden.
        /// </summary>
        /// <param name="id">Id used to filter the record in database</param>
        /// <returns>A COECacheDependency</returns>
        public virtual COECacheDependency GetCacheDependency(int id) { return null; }

        /// <summary>
        /// Method is supposed to get all the tags used in all of the dataviews/tables
        /// </summary>
        /// <returns>The list of tags</returns>
        internal virtual List<string> GetAllTags()
        {
            return new List<string>();
        }

        /// <summary>
        /// Gets list of fields from table which are of type date/CLOB/BLOB. 
        /// The user can not assign these fields as primary key while creating or modifying dataview
        /// </summary>
        /// <param name="strTableName">Table name</param>
        /// <returns></returns>
        public virtual List<string> GetInvalidPrimaryKeyFields(string strTableName)
        {
            return new List<string>();
        }

        /// <summary>
        /// Returns the datatable with primary & unique key contraint column information
        /// </summary>
        /// <param name="database">Name of the database, used as database owner.</param>
        /// <param name="table">Name of the table to find the index information</param>
        /// <returns>Datatable</returns>
        public virtual DataTable GetUniqueFields(string DataBaseName, string TableName)
        {
            //Bug Fixing : CBOE-242
            return new DataTable();
        }

        /// <summary>
        /// Returns the datatable which contains the NOTNULL string/number columns
        /// </summary>
        /// <param name="database">Name of the database, used as database owner.</param>
        /// <param name="table">Name of the table to find the index information</param>
        /// <returns>Datatable</returns>
        public virtual DataTable GetPrimaryKeyFieldNotNullCols(string DataBaseName, string TableName)
        {
            //Bug Fixing : CBOE-767
            return new DataTable();
        }
        /// <summary>
        /// Gets the list of BioAssay View Names.
        /// </summary>
        /// <returns></returns>
        public virtual List<string> GetELNSchemaName()
        {
            // JiraID: CBOE-1021
            return new List<string>();
        }
        #region COEDataViewAsDataSet
        internal virtual DataSet GetMasterDataViewTables()
        {
            // Needs to be implemented in the specific dal, as in Oracle it is based on a view made over an XMLType column. I don't think that would be
            // the way on other dbms
            return new DataSet();
        }

        internal virtual DataSet GetPublishedDataViewTables()
        {
            // Needs to be implemented in the specific dal, as in Oracle it is based on a view made over an XMLType column. I don't think that would be
            // the way on other dbms
            return new DataSet();
        }
        #endregion
        
        #region valid cartridge

        /// <summary>
        /// Get data reader to query valid cartridge
        /// </summary>
        /// <returns>data reader</returns>
        internal virtual SafeDataReader GetValidCartridge()
        {
            var sql = "select CARTRIDGE_ID, CARTRIDGE_NAME, CARTRIDGE_SCHEMA from " + Resources.CentralizedStorageDB + ".VALID_CARTRIDGE";
            var dbCommand = this.DALManager.Database.GetSqlStringCommand(sql);

            return new SafeDataReader(this.DALManager.Database.ExecuteReader(dbCommand));
        }

        #endregion

    }
}
