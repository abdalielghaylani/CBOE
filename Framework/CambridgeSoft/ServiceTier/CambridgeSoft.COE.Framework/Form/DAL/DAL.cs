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
using CambridgeSoft.COE.Framework.Common.Messaging;


namespace CambridgeSoft.COE.Framework.COEFormService
{
    public class DAL : DALBase
    {
        [NonSerialized]
        static COELog _coeLog = COELog.GetSingleton("COEForm");
        public string _coeFormTableName = string.Empty;
        public string _coeFormTypeTableName = string.Empty;
        private string _objAlias = "formTbl";

        /// <summary>
        /// to get the name of the table
        /// </summary>
        public override void SetServiceSpecificVariables()
        {
            string owner = this.DALManager.DatabaseData.OriginalOwner.ToString();
            _coeFormTableName = COEFormGroupUtilities.BuildCOEFormTableName(owner);
            _coeFormTypeTableName = COEFormGroupUtilities.BuildCOEFormTypeTableName(owner);
        }

        /// <summary>
        /// Inserts a new record in the COEForm table and returns the id
        /// </summary>
        /// <returns>the new id generated</returns>
        public virtual int GetNewID()
        {
            //this has to be overridden so throw an error if there is not override
            return -1;
        }

        /// <summary>
        /// creates a new record in the coeform table
        /// </summary>
        /// <param name="name">the name of the form</param>
        /// <param name="description">the description of the form</param>
        /// <param name="isPublic">is form available to users other then the one that created it</param>
        /// <param name="userId">the user that is making the request</param>
        /// <param name="formgroup">the id of the formgroup</param>
        /// <param name="serializedCOEForm">the serialized/xml form of the coeform object</param>
        /// <returns>the form id</returns>
        public virtual int Insert(int formgroup, string name, bool isPublic, string description, string userId, FormGroup coeForm, string databaseName, int id, int formtype, string application)
        {
            coeForm.Id = id;// this may need to be changed if the coeform is modified to make this an int
            string serializedCOEForm = coeForm.ToString();
            //inserting the data in the database.
            string sql = "INSERT INTO " + _coeFormTableName +
                 "(ID,NAME,DESCRIPTION,IS_PUBLIC,USER_ID," +
                 "FORMGROUP,COEFORM,DATE_CREATED,DATABASE,FORMTYPEID,APPLICATION)VALUES " +
                 "( " + DALManager.BuildSqlStringParameterName("pId") +
                 " ," + DALManager.BuildSqlStringParameterName("pName") +
                 " , " + DALManager.BuildSqlStringParameterName("pDescription") +
                 " , " + DALManager.BuildSqlStringParameterName("pIsPublic") +
                 " , " + DALManager.BuildSqlStringParameterName("pUserId") +
                 " , " + DALManager.BuildSqlStringParameterName("pFormGroup") +
                 " , " + DALManager.BuildSqlStringParameterName("pCOEForm") +
                 " , " + DALManager.BuildSqlStringParameterName("pDateCreated") +
                 " , " + DALManager.BuildSqlStringParameterName("pDatabaseName") +
                 " , " + DALManager.BuildSqlStringParameterName("pFormTypeId") +
                 " , " + DALManager.BuildSqlStringParameterName("pApplication") + " )";
            DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sql);

            DALManager.Database.AddInParameter(dbCommand, "pId", DbType.Int32, id);
            DALManager.Database.AddParameter(dbCommand, "pName", DbType.AnsiString, 1000, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, name);
            DALManager.Database.AddParameter(dbCommand, "pDescription", DbType.AnsiString, 1000, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, description);
            DALManager.Database.AddParameter(dbCommand, "pIsPublic", DbType.AnsiString, 1, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, Convert.ToInt16(isPublic));
            DALManager.Database.AddParameter(dbCommand, "pUserId", DbType.AnsiString, 1000, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, userId);
            DALManager.Database.AddParameter(dbCommand, "pFormGroup", DbType.AnsiString, 1000, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, formgroup);//By Sumeet from 1000 to Int32.MaxValue.
            DALManager.Database.AddParameter(dbCommand, "pCOEForm", DbType.AnsiString, Int32.MaxValue, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, serializedCOEForm);
            DALManager.Database.AddInParameter(dbCommand, "pDateCreated", DbType.Date, DateTime.Now);
            DALManager.Database.AddParameter(dbCommand, "pDatabaseName", DbType.AnsiString, 1000, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, databaseName);
            DALManager.Database.AddInParameter(dbCommand, "pFormTypeId", DbType.Int32, formtype > 0 ? formtype : 1);
            DALManager.Database.AddParameter(dbCommand, "pApplication", DbType.AnsiString, 1000, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, application);

            int recordsAffected = -1;
            try
            {
                recordsAffected = DALManager.ExecuteNonQuery(dbCommand);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return id;
        }

        /// <summary>
        /// deletes a form record containing a coeForm
        /// </summary>
        /// <param name="dataViewID">a form id</param>
        public void Delete(int id)
        {
            string sql = null;
            DbCommand dbCommand = null;
            sql = "DELETE FROM " + _coeFormTableName +
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
                throw ex;
            }
        }

        /// <summary>
        /// empties the table in the database.
        /// </summary>
        public void DeleteAll()
        {
            string sql = "DELETE FROM " + _coeFormTableName;
            DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sql);
            DALManager.ExecuteNonQuery(dbCommand);
        }

        /// <summary>
        /// deletes all forms for a user
        /// </summary>
        /// <param name="userName">the user name</param>
        public void DeleteUserForm(string userName)
        {
            string sql = "DELETE FROM " + _coeFormTableName +
                " WHERE LTRIM(RTRIM(USER_ID))=LTRIM(RTRIM(" + DALManager.BuildSqlStringParameterName("pUserName") + "))";
            DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sql);
            DALManager.Database.AddParameter(dbCommand, "pUserName", DbType.AnsiString, 1000, ParameterDirection.Input, false, 0, 0, string.Empty, DataRowVersion.Current, userName);
            DALManager.ExecuteNonQuery(dbCommand);
        }

        /// <summary>
        /// gets all form data for a user
        /// </summary>
        /// <param name="userName">the user name</param>
        /// <returns>a safedatareader containing all forms for a user </returns>
        public SafeDataReader GetUserForms(string userName)
        {
            string sql = null;
            DbCommand dbCommand = null;

            sql = "SELECT forms.ID, forms.NAME, forms.DESCRIPTION, forms.USER_ID, forms.IS_PUBLIC, forms.FORMGROUP, forms.DATE_CREATED, forms.DATABASE, forms.APPLICATION, forms.FORMTYPEID, formtypes.formtype " +
                        "FROM " + _coeFormTableName + " forms, " + _coeFormTypeTableName + " formtypes " +
                        "WHERE forms.formtypeid = formtypes.formtypeid" +
                " AND LTRIM(RTRIM(USER_ID))=LTRIM(RTRIM(" + DALManager.BuildSqlStringParameterName("pUserNameInParam") + "))" + 
                " AND (forms.IS_PUBLIC='1' OR " + AccessPermissionsSQL("forms", _coeFormTableName) + ")";
            dbCommand = DALManager.Database.GetSqlStringCommand(sql);
            DALManager.Database.AddParameter(dbCommand, "pUserNameInParam", DbType.AnsiString, 1000, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, userName);
            DALManager.Database.AddInParameter(dbCommand, "pPersonID", DbType.Int32, COEUser.ID);
            DALManager.Database.AddParameter(dbCommand, "pUserName", DbType.AnsiString, 100, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, COEUser.Name.ToUpper());
            DALManager.Database.AddParameter(dbCommand, "pUserName1", DbType.AnsiString, 100, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, COEUser.Name.ToUpper());
           
            SafeDataReader safeReader = new SafeDataReader(DALManager.Database.ExecuteReader(dbCommand));
            return safeReader;
        }

        /// <summary>
        /// returns forms filtered by isPublic flag
        /// </summary>
        /// <param name="isPublic">whether the form is public or private</param>
        /// <returns>a safedatareader containing the forms</returns>
        public SafeDataReader GetAll(bool isPublic)
        {
            string sql = "SELECT forms.*, formtypes.formtype " +
                        "FROM " + _coeFormTableName + " forms, " + _coeFormTypeTableName + " formtypes " +
                        "WHERE forms.formtypeid = formtypes.formtypeid" +
                " AND LTRIM(RTRIM(IS_PUBLIC))=LTRIM(RTRIM(" + DALManager.BuildSqlStringParameterName("pIsPublic") + "))" +
                " AND (forms.IS_PUBLIC='1' OR " + AccessPermissionsSQL("forms", _coeFormTableName) + ")";
            DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sql);
            DALManager.Database.AddParameter(dbCommand, "pIsPublic", DbType.AnsiString, 1, ParameterDirection.Input, false, 0, 0, string.Empty, DataRowVersion.Current, Convert.ToInt16(isPublic));
            DALManager.Database.AddInParameter(dbCommand, "pPersonID", DbType.Int32, COEUser.ID);
            DALManager.Database.AddParameter(dbCommand, "pUserName", DbType.AnsiString, 100, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, COEUser.Name.ToUpper());
            DALManager.Database.AddParameter(dbCommand, "pUserName1", DbType.AnsiString, 100, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, COEUser.Name.ToUpper());
            SafeDataReader safeReader = new SafeDataReader(DALManager.Database.ExecuteReader(dbCommand));
            return safeReader;
        }

        /// <summary>
        /// get all forms
        /// </summary>
        /// <returns>a safedatareader containing the forms</returns>
        public SafeDataReader GetAll()
        {
            string sql = "SELECT forms.ID, forms.NAME, forms.DESCRIPTION, forms.USER_ID, forms.IS_PUBLIC, forms.FORMGROUP, forms.DATE_CREATED, forms.DATABASE, forms.APPLICATION, forms.FORMTYPEID, formtypes.formtype " +
                        "FROM " + _coeFormTableName + " forms, " + _coeFormTypeTableName + " formtypes " +
                        "WHERE forms.formtypeid = formtypes.formtypeid" +
                        " AND (forms.IS_PUBLIC='1' OR " + AccessPermissionsSQL("forms", _coeFormTableName) + ")";
            DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sql);
            DALManager.Database.AddInParameter(dbCommand, "pPersonID", DbType.Int32, COEUser.ID);
            DALManager.Database.AddParameter(dbCommand, "pUserName", DbType.AnsiString, 100, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, COEUser.Name.ToUpper());
            DALManager.Database.AddParameter(dbCommand, "pUserName1", DbType.AnsiString, 100, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, COEUser.Name.ToUpper());
            SafeDataReader safeReader = new SafeDataReader(DALManager.Database.ExecuteReader(dbCommand));
            return safeReader;
        }

        /// <summary>
        /// update a form
        /// </summary>
        /// <param name="id">id of form</param>
        /// <param name="serializedCOEForm">the updated xml form of the form</param>
        /// <param name="name">the updated name of the form</param>
        /// <param name="description">the updated description of the form</param>
        /// <param name="isPublic">whether the form is made internal or not</param>
        public void Update(int id, string serializedCOEForm, string name, string description, bool isPublic, string databaseName)
        {
            string sql="UPDATE " + _coeFormTableName +
                " SET NAME= " + DALManager.BuildSqlStringParameterName("pName") + "," +
                " DESCRIPTION=" + DALManager.BuildSqlStringParameterName("pDescription") + "," +
                " IS_PUBLIC= " + DALManager.BuildSqlStringParameterName("pIsPublic") + "," +
                " DATABASE= " + DALManager.BuildSqlStringParameterName("pDatabase") + "," +
                " COEFORM=" + DALManager.BuildSqlStringParameterName("pCOEForm") +
                " WHERE ID=" + DALManager.BuildSqlStringParameterName("pId");
            
            DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sql);
            //now setting the values for the parameters.
            DALManager.Database.AddParameter(dbCommand, "pName", DbType.AnsiString, 1000, ParameterDirection.Input, false, 0, 0, string.Empty, DataRowVersion.Current, name);
            DALManager.Database.AddParameter(dbCommand, "pDescription", DbType.AnsiString, 1000, ParameterDirection.Input, false, 0, 0, string.Empty, DataRowVersion.Current, description);
            DALManager.Database.AddParameter(dbCommand, "pIsPublic", DbType.AnsiString, 1, ParameterDirection.Input, false, 0, 0, string.Empty, DataRowVersion.Current, Convert.ToInt16(isPublic));
            DALManager.Database.AddParameter(dbCommand, "pDatabase", DbType.AnsiString, 1000, ParameterDirection.Input, false, 0, 0, string.Empty, DataRowVersion.Current, databaseName);
            DALManager.Database.AddParameter(dbCommand, "pCOEForm", DbType.AnsiString, Int32.MaxValue, ParameterDirection.Input, false, 0, 0, string.Empty, DataRowVersion.Current, serializedCOEForm);
            DALManager.Database.AddInParameter(dbCommand, "pId", DbType.Int32, id);
            DALManager.ExecuteNonQuery(dbCommand);
        }

        /// <summary>
        /// get a form
        /// </summary>
        /// <param name="id">the id for the form</param>
        /// <returns>a safedatareader containing the form</returns>
        public SafeDataReader Get(int id)
        {
            string sql =    "SELECT forms.*, formtypes.formtype " +
                            "FROM 	" + _coeFormTableName + " forms, " + _coeFormTypeTableName + " formtypes " +
                            "WHERE 	forms.ID=" + DALManager.BuildSqlStringParameterName("pId") + " " +
                            "AND	forms.formtypeid = formtypes.formtypeid";

            DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sql);
            DALManager.Database.AddInParameter(dbCommand, "pId", DbType.Int32, id);
            SafeDataReader safeReader;
            try
            {
              safeReader = new SafeDataReader(DALManager.ExecuteReader(dbCommand));
            }
            catch (Exception e)
            {
                throw e;
            }
            return safeReader;
        }

        internal SafeDataReader GetForms(string username, string databaseName, string application, int formtypeid, bool getPublicForms) {
            string sql = String.Empty;
            if(!getPublicForms) {
                sql = "SELECT forms.ID, forms.NAME, forms.DESCRIPTION, forms.USER_ID, forms.IS_PUBLIC, forms.FORMGROUP, forms.DATE_CREATED, forms.DATABASE, forms.APPLICATION, forms.FORMTYPEID, formtypes.formtype " +
                        "FROM " + _coeFormTableName + " forms, " + _coeFormTypeTableName + " formtypes " +
                        "WHERE forms.formtypeid = formtypes.formtypeid";
                if(!string.IsNullOrEmpty(username))
                    sql += " AND forms.USER_ID=" + DALManager.BuildSqlStringParameterName("pUserId");
                
                        
            } else {
                sql = "SELECT forms.ID, forms.NAME, forms.DESCRIPTION, forms.USER_ID, forms.IS_PUBLIC, forms.FORMGROUP, forms.DATE_CREATED, forms.DATABASE, forms.APPLICATION, forms.FORMTYPEID, formtypes.formtype " +
                        "FROM " + _coeFormTableName + " forms, " + _coeFormTypeTableName + " formtypes " +
                        "WHERE forms.formtypeid = formtypes.formtypeid";
                if(!string.IsNullOrEmpty(username))
                    sql += " AND (forms.USER_ID=" + DALManager.BuildSqlStringParameterName("pUserId") + " " +
                        "OR forms.IS_PUBLIC=" + DALManager.BuildSqlStringParameterName("pIsPublic") + ")";
            }

            if(!string.IsNullOrEmpty(databaseName))
                sql += " AND forms.DATABASE=" + DALManager.BuildSqlStringParameterName("pDatabase");
            if(!string.IsNullOrEmpty(application))
                sql += " AND forms.APPLICATION=" + DALManager.BuildSqlStringParameterName("pApplication");
            if(formtypeid > 0)
                sql += " AND forms.FORMTYPEID=" + DALManager.BuildSqlStringParameterName("pFormTypeId");

            DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sql);
            if(!string.IsNullOrEmpty(username)) {
                DALManager.Database.AddParameter(dbCommand, DALManager.BuildSqlStringParameterName("pUserId"), DbType.AnsiString, 30, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, username);

                if(getPublicForms)
                    DALManager.Database.AddParameter(dbCommand, DALManager.BuildSqlStringParameterName("pIsPublic"), DbType.AnsiString, 1, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, "1");
            }

            if(!string.IsNullOrEmpty(databaseName))
                DALManager.Database.AddParameter(dbCommand, DALManager.BuildSqlStringParameterName("pDatabase"), DbType.AnsiString, 30, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, databaseName);
            if(!string.IsNullOrEmpty(application))
                DALManager.Database.AddParameter(dbCommand, DALManager.BuildSqlStringParameterName("pApplication"), DbType.AnsiString, 255, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, application);
            if(formtypeid > 0)
                DALManager.Database.AddParameter(dbCommand, DALManager.BuildSqlStringParameterName("pFormTypeId"), DbType.Int16, ParameterDirection.Input, string.Empty, DataRowVersion.Current, (short) formtypeid);
            
            SafeDataReader safeReader;
            try {
                safeReader = new SafeDataReader(DALManager.Database.ExecuteReader(dbCommand));
            } catch(Exception e) {
                throw e;
            }
            return safeReader;
        }

        internal SafeDataReader GetForms(string username, string databaseName, string application, string formtype, bool getPublicForms) {
            string sql = string.Empty;
            if(!getPublicForms) {
                sql = "SELECT forms.ID, forms.NAME, forms.DESCRIPTION, forms.USER_ID, forms.IS_PUBLIC, forms.FORMGROUP, forms.DATE_CREATED, forms.DATABASE, forms.APPLICATION, forms.FORMTYPEID, formtypes.formtype " +
                            "FROM " + _coeFormTableName + " forms, " + _coeFormTypeTableName + " formtypes " +
                            "WHERE forms.formtypeid = formtypes.formtypeid";
                if(!string.IsNullOrEmpty(username))
                    sql += " AND forms.USER_ID=" + DALManager.BuildSqlStringParameterName("pUserId");
            } else {
                sql = "SELECT forms.ID, forms.NAME, forms.DESCRIPTION, forms.USER_ID, forms.IS_PUBLIC, forms.FORMGROUP, forms.DATE_CREATED, forms.DATABASE, forms.APPLICATION, forms.FORMTYPEID, formtypes.formtype " +
                            "FROM " + _coeFormTableName + " forms, " + _coeFormTypeTableName + " formtypes " +
                            "WHERE forms.formtypeid = formtypes.formtypeid";
                if(!string.IsNullOrEmpty(username))
                    sql += " AND (forms.USER_ID=" + DALManager.BuildSqlStringParameterName("pUserId") + " " +
                           "OR forms.IS_PUBLIC=" + DALManager.BuildSqlStringParameterName("pIsPublic") + ")";
            }

            if(!string.IsNullOrEmpty(databaseName))
                sql += " AND forms.DATABASE=" + DALManager.BuildSqlStringParameterName("pDatabase");
            if(!string.IsNullOrEmpty(application))
                sql += " AND forms.APPLICATION=" + DALManager.BuildSqlStringParameterName("pApplication");
            if(!string.IsNullOrEmpty(formtype))
                sql += " AND formtypes.FORMTYPE=" + DALManager.BuildSqlStringParameterName("pFormType");

            DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sql);

            if(!string.IsNullOrEmpty(username)) {
                DALManager.Database.AddParameter(dbCommand, DALManager.BuildSqlStringParameterName("pUserId"), DbType.AnsiString, 30, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, username);

                if(getPublicForms)
                    DALManager.Database.AddParameter(dbCommand, DALManager.BuildSqlStringParameterName("pIsPublic"), DbType.AnsiString, 1, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, "1");
            }

            if(!string.IsNullOrEmpty(databaseName))
                DALManager.Database.AddParameter(dbCommand, DALManager.BuildSqlStringParameterName("pDatabase"), DbType.AnsiString, 30, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, databaseName);
            if(!string.IsNullOrEmpty(application))
                DALManager.Database.AddParameter(dbCommand, DALManager.BuildSqlStringParameterName("pApplication"), DbType.AnsiString, 255, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, application );
            if(!string.IsNullOrEmpty(formtype))
                DALManager.Database.AddParameter(dbCommand, DALManager.BuildSqlStringParameterName("pFormType"), DbType.AnsiString, 255, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, formtype);
            
            SafeDataReader safeReader;
            try {
                safeReader = new SafeDataReader(DALManager.Database.ExecuteReader(dbCommand));
            } catch(Exception e) {
                throw e;
            }
            return safeReader;
        }

        internal bool CanGetForm(int id)
        {
            string sql = "SELECT count(1) " +
                            "FROM 	" + _coeFormTableName + " forms " +
                            "WHERE 	forms.ID=" + DALManager.BuildSqlStringParameterName("pId") + " " +
                            "AND (forms.IS_PUBLIC='1' OR " + AccessPermissionsSQL("forms", _coeFormTableName) + " OR forms.USER_ID=" + DALManager.BuildSqlStringParameterName("pUserNameInParam") + ")";

            //DALManager.Database.CreateConnection();
            DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sql);
            DALManager.Database.AddInParameter(dbCommand, "pId", DbType.Int32, id);
            DALManager.Database.AddInParameter(dbCommand, "pPersonID", DbType.Int32, COEUser.ID);
            DALManager.Database.AddParameter(dbCommand, "pUserName", DbType.AnsiString, 100, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, COEUser.Name.ToUpper());
            DALManager.Database.AddParameter(dbCommand, "pUserName1", DbType.AnsiString, 100, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, COEUser.Name.ToUpper());
            DALManager.Database.AddParameter(dbCommand, "pUserNameInParam", DbType.AnsiString, 1000, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, COEUser.Name.ToUpper());

            int count = Convert.ToInt32(DALManager.ExecuteScalar(dbCommand));

            return count > 0;
        }

        internal virtual CambridgeSoft.COE.Framework.Caching.COECacheDependency GetCacheDependency(int id)
        {
            return null;
        }
    }
}
