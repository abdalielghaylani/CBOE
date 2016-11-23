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


namespace CambridgeSoft.COE.Framework.COEGenericObjectStorageService
{
    /// <summary>
    /// Base Data access logic layer class for supporting COEGenericObjectStorage Service.
    /// </summary>
    public class DAL : DALBase
    {
        protected string _coeGenericObjectStorageTableName = string.Empty;
        private string _objAlias = "genObj";
        [NonSerialized]
        static COELog _coeLog = COELog.GetSingleton("COEGenericObjectStorage");


        /// <summary>
        /// Gets the name of the table.
        /// </summary>
        public override void SetServiceSpecificVariables()
        {
            try
            {
                string owner = this.DALManager.DatabaseData.OriginalOwner.ToString();
                COEGenericObjectStorageUtilities.BuildCOEGenericObjectStorageTableName(owner, ref _coeGenericObjectStorageTableName);

            }
            catch(Exception)
            {

                throw;
            }
        }



        /// <summary>
        /// Gets the new available id to be used as id for a record in COEGenericObjectStorage.
        /// </summary>
        /// <returns>The new id generated</returns>
        public virtual int GetNewID()
        {
            //this has to be overridden so throw an error if there is not override
            return -1;
        }

        /// <summary>
        /// Creates a new record in the COEGenericObjectStorage table.
        /// </summary>
        /// <param name="name">The name of the form.</param>
        /// <param name="description">The description of the form.</param>
        /// <param name="userId">The user that is making the request.</param>
        /// <param name="coeGenericObjectStorage">The string version of the object to be stored.</param>
        /// <param name="formgroup">The form group.</param>
        /// <param name="isPublic">Is the object public?.</param>
        /// <returns>The form's id.</returns>
        public virtual int Insert(int formgroup, string name, bool isPublic, string description, string userId, string coeGenericObjectStorage, string databaseName, int id, string associatedDataviewID = "")
        {
            try
            {
                //inserting the data in the database.
                string sql = "INSERT INTO " + _coeGenericObjectStorageTableName +
                     "(ID,NAME,DESCRIPTION,USER_ID," +
                     "COEGENERICOBJECT,DATE_CREATED,ASSOCIATEDDATAVIEWID,DATABASE)VALUES " +
                     "( " + DALManager.BuildSqlStringParameterName("pId") +
                     " ," + DALManager.BuildSqlStringParameterName("pName") +
                     " , " + DALManager.BuildSqlStringParameterName("pDescription") +
                     " , " + DALManager.BuildSqlStringParameterName("pIsPublic") +
                     " , " + DALManager.BuildSqlStringParameterName("pUserId") +
                     " , " + DALManager.BuildSqlStringParameterName("pFormGroup") +
                     " , " + DALManager.BuildSqlStringParameterName("pCOEGenericObject") +
                     " , " + DALManager.BuildSqlStringParameterName("pDateCreated") +
                     " , " + DALManager.BuildSqlStringParameterName("pAssociatedDataviewID") +
                     " , " + DALManager.BuildSqlStringParameterName("pDatabaseName") + " )";

                DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sql);

                DALManager.Database.AddInParameter(dbCommand, "pId", DbType.Int32, id);
                DALManager.Database.AddParameter(dbCommand, "pName", DbType.AnsiString, 1000, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, name);
                DALManager.Database.AddParameter(dbCommand, "pDescription", DbType.AnsiString, 1000, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, description);
                DALManager.Database.AddParameter(dbCommand, "pIsPublic", DbType.AnsiString, 1, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, Convert.ToInt16(isPublic));
                DALManager.Database.AddParameter(dbCommand, "pUserId", DbType.AnsiString, 1000, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, userId.ToUpper());
                DALManager.Database.AddParameter(dbCommand, "pFormGroup", DbType.AnsiString, 1000, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, formgroup);
                DALManager.Database.AddParameter(dbCommand, "pCOEGenericObject", DbType.AnsiString, 10000, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, coeGenericObjectStorage);
                DALManager.Database.AddInParameter(dbCommand, "pDateCreated", DbType.Date, DateTime.Now);
                DALManager.Database.AddParameter(dbCommand, "pAssociatedDataviewID", DbType.AnsiString, 1000, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, associatedDataviewID);
                DALManager.Database.AddParameter(dbCommand, "pDatabaseName", DbType.AnsiString, 1000, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, databaseName);


                int recordsAffected = -1;

                recordsAffected = DALManager.ExecuteNonQuery(dbCommand);

                return id;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// Deletes a form record containing a coeDataView.
        /// </summary>
        /// <param name="dataViewID">A form id.</param>
        public virtual void Delete(int id)
        {
            string sql = string.Empty;
            try
            {
                DbCommand dbCommand = null;

                sql = "DELETE FROM " + _coeGenericObjectStorageTableName +
                    " WHERE ID =" + DALManager.BuildSqlStringParameterName("pId");

                dbCommand = DALManager.Database.GetSqlStringCommand(sql);

                DALManager.Database.AddInParameter(dbCommand, "pId", DbType.Int32, id);
                int recordsAffected = -1;

                recordsAffected = DALManager.ExecuteNonQuery(dbCommand);
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// Empties the table in the database.
        /// </summary>
        public virtual void DeleteAll()
        {
            try
            {

                string sql = "DELETE FROM " + _coeGenericObjectStorageTableName;
                DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sql);
                DALManager.ExecuteNonQuery(dbCommand);
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// Deletes all dataviews for a user.
        /// </summary>
        /// <param name="userName">The user name.</param>
        public virtual void DeleteUserGenericObjects(string userName)
        {
            string sql = string.Empty;
            try
            {

                sql = "DELETE FROM " + _coeGenericObjectStorageTableName +
                    " WHERE LTRIM(RTRIM(USER_ID))=LTRIM(RTRIM(" + DALManager.BuildSqlStringParameterName("pUserName") + "))";
                DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sql);
                DALManager.Database.AddParameter(dbCommand, "pUserName", DbType.AnsiString, 1000, ParameterDirection.Input, false, 0, 0, string.Empty, DataRowVersion.Current, userName.ToUpper());
                DALManager.ExecuteNonQuery(dbCommand);
            }
            catch(Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Gets all stored generic objects.
        /// </summary>
        /// <returns>A safedatareader containing the generic objects.</returns>
        public virtual SafeDataReader GetAll()
        {
            SafeDataReader safeReader = null;
            string sql = string.Empty;
            try
            {
                sql = "SELECT ID, NAME, DESCRIPTION, USER_ID, IS_PUBLIC, FORMGROUP, DATE_CREATED, DATABASE, ASSOCIATEDDATAVIEWID FROM " + _coeGenericObjectStorageTableName + " " + _objAlias + " where " + _objAlias + ".is_public=1 or " + AccessPermissionsSQL(_objAlias, _coeGenericObjectStorageTableName) + " order by " + _objAlias +".name";
                DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sql);
                DALManager.Database.AddInParameter(dbCommand, "pPersonID", DbType.Int32, COEUser.ID);
                DALManager.Database.AddParameter(dbCommand, "pUserName", DbType.AnsiString, 100, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, COEUser.Name.ToUpper());
                DALManager.Database.AddParameter(dbCommand, "pUserName1", DbType.AnsiString, 100, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, COEUser.Name.ToUpper());
                safeReader = new SafeDataReader(DALManager.Database.ExecuteReader(dbCommand));
                return safeReader;
            }
            catch(Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Gets all form data for a user named <paramref name="userName"/>.
        /// </summary>
        /// <param name="userName">The user name.</param>
        /// <returns>A safedatareader containing all dataviews for a user.</returns>
        public virtual SafeDataReader GetAll(string userName)
        {
            SafeDataReader safeReader = null;
            string sql = string.Empty;

            try
            {
                DbCommand dbCommand = null;
                sql = "SELECT ID, NAME, DESCRIPTION, USER_ID, IS_PUBLIC, FORMGROUP, DATE_CREATED, DATABASE, ASSOCIATEDDATAVIEWID FROM " + _coeGenericObjectStorageTableName +
                    " " + _objAlias + " WHERE LTRIM(RTRIM(" + _objAlias + ".USER_ID))=LTRIM(RTRIM(" + DALManager.BuildSqlStringParameterName("pUserNameInParam") + ")) OR " + _objAlias + ".IS_PUBLIC='1' OR " + AccessPermissionsSQL(_objAlias, _coeGenericObjectStorageTableName) + " order by " + _objAlias + ".name";
                dbCommand = DALManager.Database.GetSqlStringCommand(sql);

                DALManager.Database.AddParameter(dbCommand, "pUserNameInParam", DbType.AnsiString, 1000, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, userName.ToUpper());
                DALManager.Database.AddInParameter(dbCommand, "pPersonID", DbType.Int32, COEUser.ID);
                DALManager.Database.AddParameter(dbCommand, "pUserName", DbType.AnsiString, 100, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, COEUser.Name.ToUpper());
                DALManager.Database.AddParameter(dbCommand, "pUserName1", DbType.AnsiString, 100, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, COEUser.Name.ToUpper());

                safeReader = new SafeDataReader(DALManager.Database.ExecuteReader(dbCommand));
                return safeReader;
            }
            catch(Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Gets all form data for a user and a form group.
        /// </summary>
        /// <param name="userName">The user name.</param>
        /// <returns>A safedatareader containing all dataviews for a user.</returns>
        public virtual SafeDataReader GetAll(string userName, int formGroup)
        {

            string sql = null;
            DbCommand dbCommand = null;
            sql = "SELECT ID, NAME, DESCRIPTION, USER_ID, IS_PUBLIC, FORMGROUP, DATE_CREATED, DATABASE, ASSOCIATEDDATAVIEWID FROM " + _coeGenericObjectStorageTableName +
                " " + _objAlias + " WHERE (LTRIM(RTRIM(" + _objAlias + ".USER_ID))=LTRIM(RTRIM(" + DALManager.BuildSqlStringParameterName("pUserNameInParam") + "))" +
                " OR " + AccessPermissionsSQL(_objAlias, _coeGenericObjectStorageTableName) + ")" +
                " AND " + _objAlias + ".FORMGROUP=" + DALManager.BuildSqlStringParameterName("pFormGroup") + " order by " + _objAlias + ".Name";
            dbCommand = DALManager.Database.GetSqlStringCommand(sql);

            DALManager.Database.AddParameter(dbCommand, "pUserNameInParam", DbType.AnsiString, 1000, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, userName.ToUpper());
            DALManager.Database.AddInParameter(dbCommand, "pPersonID", DbType.Int32, COEUser.ID);
            DALManager.Database.AddParameter(dbCommand, "pUserName", DbType.AnsiString, 100, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, COEUser.Name.ToUpper());
            DALManager.Database.AddParameter(dbCommand, "pUserName1", DbType.AnsiString, 100, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, COEUser.Name.ToUpper());
            DALManager.Database.AddInParameter(dbCommand, "pFormGroup", DbType.Int32, formGroup);
            SafeDataReader safeReader = new SafeDataReader(DALManager.Database.ExecuteReader(dbCommand));

            return safeReader;
        }
        /// <summary>
        /// Gets all form data for a user and a form group.
        /// </summary>
        /// <param name="userName">The user name.</param>
        /// <returns>A safedatareader containing all dataviews for a user.</returns>
        public virtual SafeDataReader GetAllCompleteData(string userName, int formGroup)
        {

            string sql = null;
            DbCommand dbCommand = null;
            sql = "SELECT ID, NAME, DESCRIPTION, USER_ID, IS_PUBLIC, FORMGROUP, DATE_CREATED, DATABASE, COEGENERICOBJECT, ASSOCIATEDDATAVIEWID FROM " + _coeGenericObjectStorageTableName +
                " " + _objAlias + " WHERE (LTRIM(RTRIM(" + _objAlias + ".USER_ID))=LTRIM(RTRIM(" + DALManager.BuildSqlStringParameterName("pUserNameInParam") + "))" +
                " OR " + AccessPermissionsSQL(_objAlias, _coeGenericObjectStorageTableName) + ")" +
                " AND " + _objAlias + ".FORMGROUP=" + DALManager.BuildSqlStringParameterName("pFormGroup")+ " order by " + _objAlias + ".name";
            dbCommand = DALManager.Database.GetSqlStringCommand(sql);
            DALManager.Database.AddParameter(dbCommand, "pUserNameInParam", DbType.AnsiString, 1000, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, userName.ToUpper());
            DALManager.Database.AddInParameter(dbCommand, "pPersonID", DbType.Int32, COEUser.ID);
            DALManager.Database.AddParameter(dbCommand, "pUserName", DbType.AnsiString, 100, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, COEUser.Name.ToUpper());
            DALManager.Database.AddParameter(dbCommand, "pUserName1", DbType.AnsiString, 100, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, COEUser.Name.ToUpper());
            DALManager.Database.AddInParameter(dbCommand, "pFormGroup", DbType.Int32, formGroup);
            SafeDataReader safeReader = new SafeDataReader(DALManager.Database.ExecuteReader(dbCommand));

            return safeReader;
        }

        /// <summary>
        /// Gets all form data for a user.
        /// </summary>
        /// <param name="userName">The user name.</param>
        /// <returns>A safedatareader containing all dataviews for a user.</returns>
        public virtual SafeDataReader GetAll(bool isPublic, string excludeUser)
        {
            SafeDataReader safeReader = null;
            try
            {
                DbCommand dbCommand = null;
                string sql = "SELECT * FROM " + _coeGenericObjectStorageTableName + " " + _objAlias +
                    " WHERE (" + _objAlias + ".IS_PUBLIC = '" + (isPublic ? "1" : "0") + "'" +
                    " OR " + AccessPermissionsSQL(_objAlias, _coeGenericObjectStorageTableName) + ") order by " + _objAlias + ".name";

                if(!string.IsNullOrEmpty(excludeUser))
                {
                    sql += " AND LTRIM(RTRIM(" + _objAlias + ".USER_ID))<>LTRIM(RTRIM(" + DALManager.BuildSqlStringParameterName("pExcludeUserName") + "))";
                }

                dbCommand = DALManager.Database.GetSqlStringCommand(sql);

                DALManager.Database.AddInParameter(dbCommand, "pPersonID", DbType.Int32, COEUser.ID);
                DALManager.Database.AddParameter(dbCommand, "pUserName", DbType.AnsiString, 100, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, COEUser.Name.ToUpper());
                DALManager.Database.AddParameter(dbCommand, "pUserName1", DbType.AnsiString, 100, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, COEUser.Name.ToUpper());
                if(!string.IsNullOrEmpty(excludeUser))
                    DALManager.Database.AddParameter(dbCommand, "pExcludeUserName", DbType.AnsiString, 1000, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, excludeUser);
                safeReader = new SafeDataReader(DALManager.Database.ExecuteReader(dbCommand));
                return safeReader;
            }
            catch(Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Gets public or private records for a user.
        /// </summary>
        /// <param name="userName">The user name.</param>
        /// <param name="isPublic">A flag that indicates if public or private records are to be retrieved</param>
        /// <returns>A safedatareader containing all dataviews for a user.</returns>
        public virtual SafeDataReader GetAll(string userName, bool isPublic)
        {
            SafeDataReader safeReader = null;
            string sql = string.Empty;

            try
            {
                DbCommand dbCommand = null;
                sql = "SELECT ID, NAME, DESCRIPTION, USER_ID, IS_PUBLIC, FORMGROUP, DATE_CREATED, DATABASE, ASSOCIATEDDATAVIEWID FROM " + _coeGenericObjectStorageTableName + " " + _objAlias +
                    " WHERE (" + _objAlias + ".IS_PUBLIC='" + System.Convert.ToInt16(isPublic) + "'" +
                    " OR " + AccessPermissionsSQL(_objAlias, _coeGenericObjectStorageTableName) + ") order by " + _objAlias + ".name";

                if(!string.IsNullOrEmpty(userName))
                    sql += " AND LTRIM(RTRIM(" + _objAlias + ".USER_ID))=LTRIM(RTRIM(" + DALManager.BuildSqlStringParameterName("pUserNameInParam") + "))";

                dbCommand = DALManager.Database.GetSqlStringCommand(sql);
                DALManager.Database.AddInParameter(dbCommand, "pPersonID", DbType.Int32, COEUser.ID);
                DALManager.Database.AddParameter(dbCommand, "pUserName", DbType.AnsiString, 100, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, COEUser.Name.ToUpper());
                DALManager.Database.AddParameter(dbCommand, "pUserName1", DbType.AnsiString, 100, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, COEUser.Name.ToUpper());
                if(!string.IsNullOrEmpty(userName))
                    DALManager.Database.AddParameter(dbCommand, "pUserNameInParam", DbType.AnsiString, 1000, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, userName.ToUpper());
                safeReader = new SafeDataReader(DALManager.Database.ExecuteReader(dbCommand));

                return safeReader;
            }
            catch(Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Gets all form data for all users except for excluded user.
        /// </summary>
        /// <param name="excludedUser">The user to be excluded.</param>
        /// <returns>A safedatareader containing all dataviews for all users except for excluded user.</returns>
        public virtual SafeDataReader GetAll(string excludedUser, bool isPublic, int formgroup)
        {
            try
            {
                string sql = null;
                DbCommand dbCommand = null;
                sql = "SELECT ID, NAME, DESCRIPTION, USER_ID, IS_PUBLIC, FORMGROUP, DATE_CREATED, DATABASE, ASSOCIATEDDATAVIEWID FROM " + _coeGenericObjectStorageTableName + " " + _objAlias +
                    " WHERE " + _objAlias + ".FORMGROUP=" + formgroup + " AND " + _objAlias + ".IS_PUBLIC='" + (isPublic ? "1" : "0") + "'" +
                    " AND( " + AccessPermissionsSQL(_objAlias, _coeGenericObjectStorageTableName) + " OR " + _objAlias + ".IS_PUBLIC='1')" +
                    " AND LTRIM(RTRIM(" + _objAlias + ".USER_ID))<>LTRIM(RTRIM(" + DALManager.BuildSqlStringParameterName("pUserNameInParam") + ")) order by " + _objAlias + ".name";

                dbCommand = DALManager.Database.GetSqlStringCommand(sql);
                DALManager.Database.AddInParameter(dbCommand, "pPersonID", DbType.Int32, COEUser.ID);
                DALManager.Database.AddParameter(dbCommand, "pUserName", DbType.AnsiString, 100, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, COEUser.Name.ToUpper());
                DALManager.Database.AddParameter(dbCommand, "pUserName1", DbType.AnsiString, 100, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, COEUser.Name.ToUpper());
                DALManager.Database.AddParameter(dbCommand, "pUserNameInParam", DbType.AnsiString, 1000, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, excludedUser.ToUpper());
                SafeDataReader safeReader = new SafeDataReader(DALManager.Database.ExecuteReader(dbCommand));
                return safeReader;
            }
            catch(Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Gets all form data for all users except for excluded user.
        /// </summary>
        /// <param name="excludedUser">The user to be excluded.</param>
        /// <returns>A safedatareader containing all dataviews for all users except for excluded user.</returns>
        public virtual SafeDataReader GetAllCompleteData(string excludedUser, bool isPublic, int formgroup)
        {
            try
            {
                string sql = null;
                DbCommand dbCommand = null;
                sql = "SELECT ID, NAME, DESCRIPTION, USER_ID, IS_PUBLIC, FORMGROUP, DATE_CREATED, DATABASE, COEGENERICOBJECT, ASSOCIATEDDATAVIEWID FROM " + _coeGenericObjectStorageTableName + " " + _objAlias +
                    " WHERE " + _objAlias + ".FORMGROUP=" + formgroup + " AND " + _objAlias + ".IS_PUBLIC='" + (isPublic ? "1" : "0") + "'" +
                    " AND( " + AccessPermissionsSQL(_objAlias, _coeGenericObjectStorageTableName) + " OR " + _objAlias + ".IS_PUBLIC='1')" +
                    " AND LTRIM(RTRIM(" + _objAlias + ".USER_ID))<>LTRIM(RTRIM(" + DALManager.BuildSqlStringParameterName("pUserNameInParam") + ")) order by " + _objAlias + ".name";

                dbCommand = DALManager.Database.GetSqlStringCommand(sql);
                DALManager.Database.AddInParameter(dbCommand, "pPersonID", DbType.Int32, COEUser.ID);
                DALManager.Database.AddParameter(dbCommand, "pUserName", DbType.AnsiString, 100, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, COEUser.Name.ToUpper());
                DALManager.Database.AddParameter(dbCommand, "pUserName1", DbType.AnsiString, 100, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, COEUser.Name.ToUpper());
                DALManager.Database.AddParameter(dbCommand, "pUserNameInParam", DbType.AnsiString, 1000, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, excludedUser.ToUpper());
                SafeDataReader safeReader = new SafeDataReader(DALManager.Database.ExecuteReader(dbCommand));
                return safeReader;
            }
            catch(Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Updates a generic object.
        /// </summary>
        /// <param name="id">ID of the generic object.</param>
        /// <param name="coeGenericObject">The updated xml form of the form.</param>
        /// <param name="name">The updated name of the form.</param>
        /// <param name="description">The updated description of the form.</param>
        /// <param name="isPublic">The is public updated property.</param>
        public virtual void Update(int id, string coeGenericObject, string name, string description, bool isPublic, string databaseName)
        {
            string sql = string.Empty;
            try
            {

                sql = "UPDATE " + _coeGenericObjectStorageTableName +
                    " SET NAME= " + DALManager.BuildSqlStringParameterName("pName") + "," +
                    " DESCRIPTION=" + DALManager.BuildSqlStringParameterName("pDescription") + "," +
                    " IS_PUBLIC= " + DALManager.BuildSqlStringParameterName("pIsPublic") + "," +
                    " DATABASE= " + DALManager.BuildSqlStringParameterName("pDatabase") + "," +
                    " COEGENERICOBJECT=" + DALManager.BuildSqlStringParameterName("pCOEGenericObject") + "," +
                    " WHERE ID=" + DALManager.BuildSqlStringParameterName("pId");

                DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sql);

                //now setting the values for the parameters.
                DALManager.Database.AddParameter(dbCommand, "pName", DbType.AnsiString, 1000, ParameterDirection.Input, false, 0, 0, string.Empty, DataRowVersion.Current, name);
                DALManager.Database.AddParameter(dbCommand, "pDescription", DbType.AnsiString, 1000, ParameterDirection.Input, false, 0, 0, string.Empty, DataRowVersion.Current, description);
                DALManager.Database.AddParameter(dbCommand, "pIsPublic", DbType.AnsiString, 1, ParameterDirection.Input, false, 0, 0, string.Empty, DataRowVersion.Current, Convert.ToInt16(isPublic));
                DALManager.Database.AddParameter(dbCommand, "pDatabase", DbType.AnsiString, 1000, ParameterDirection.Input, false, 0, 0, string.Empty, DataRowVersion.Current, databaseName);
                DALManager.Database.AddParameter(dbCommand, "pCOEGenericObject", DbType.AnsiString, 10000, ParameterDirection.Input, false, 0, 0, string.Empty, DataRowVersion.Current, coeGenericObject);
                DALManager.Database.AddInParameter(dbCommand, "pId", DbType.Int32, id);
                DALManager.ExecuteNonQuery(dbCommand);
            }
            catch(Exception ex)
            {
                throw;
            }
        }


        /// <summary>
        /// Gets a form.
        /// </summary>
        /// <param name="id">The id for the form.</param>
        /// <returns>A safedatareader containing the form.</returns>
        public SafeDataReader Get(int id)
        {
            SafeDataReader safeReader = null;
            string sql = string.Empty;

            try
            {
                sql = "SELECT * FROM " + _coeGenericObjectStorageTableName + " " + _objAlias +
                    " WHERE ID=" + DALManager.BuildSqlStringParameterName("pId") +
                    " AND (" + _objAlias + ".IS_PUBLIC = '1' OR " + AccessPermissionsSQL(_objAlias, _coeGenericObjectStorageTableName) +
                    " OR LTRIM(RTRIM(" + _objAlias + ".USER_ID))=LTRIM(RTRIM(" + DALManager.BuildSqlStringParameterName("pUserNameInParam") + ")))";

                DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sql);
                DALManager.Database.AddInParameter(dbCommand, "pId", DbType.Int32, id);
                DALManager.Database.AddInParameter(dbCommand, "pPersonID", DbType.Int32, COEUser.ID);
                DALManager.Database.AddParameter(dbCommand, "pUserName", DbType.AnsiString, 100, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, COEUser.Name.ToUpper());
                DALManager.Database.AddParameter(dbCommand, "pUserName1", DbType.AnsiString, 100, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, COEUser.Name.ToUpper());
                DALManager.Database.AddParameter(dbCommand, "pUserNameInParam", DbType.AnsiString, 1000, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, COEUser.Name.ToUpper());
                safeReader = new SafeDataReader(DALManager.Database.ExecuteReader(dbCommand));
                return safeReader;
            }
            catch(Exception ex)
            {
                throw;
            }
        }
    }
}
