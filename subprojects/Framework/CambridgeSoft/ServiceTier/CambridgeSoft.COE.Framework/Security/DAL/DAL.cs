using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Csla.Data;
using System.Data.Common;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.COELoggingService;
using CambridgeSoft.COE.Framework.COESecurityService;
using CambridgeSoft.COE.Framework.Properties;



namespace CambridgeSoft.COE.Framework.COESecurityService
{
    public class DAL : DALBase
    {
        protected string _globalsTableName = "COEGLOBALS";
        protected string _sessionTableName = "COESESSION";

        [NonSerialized]
        static COELog _coeLog = COELog.GetSingleton("COESecurity");

        #region DalBase required

         public override void SetServiceSpecificVariables()
        {

            try
            {
                string owner = this.DALManager.DatabaseData.OriginalOwner.ToString();
                COESecurityUtilities.BuildCOEGlobalsTableName(owner, ref _globalsTableName);
                COESecurityUtilities.BuildCOESessionTableName(owner, ref _sessionTableName);


            }
            catch (Exception ex)
            {

                throw;
            }
        }

        #endregion

        #region schema version

        internal virtual Version GetFrameworkSchemaVersion()
        {
            //must be overriden by provider
            return null;
        }

        #endregion

        #region session

        internal virtual int StartSession(int userID)
        {
            //must be overriden by provider
            return -1;
        }

        internal virtual void EndSession(int sessionID)
        {
            //must be overriden by provider
            return;
        }

        #endregion

        #region Users

        

        internal virtual int GetPersonID(string userID)
        {

            DbCommand dbCommand = null;
            string sql = string.Empty;
            DbCommand dbCommand2 = null;
            string sql2 = string.Empty;
            int returnValue = 0;
            int returnValue2 = 0;

            sql = "SELECT Person_ID from " + Resources.SecurityDatabaseName + ".people where upper(user_id) = Upper(" + DALManager.BuildSqlStringParameterName("pUserName") + ") AND Active <> -1";
            dbCommand = this.DALManager.Database.GetSqlStringCommand(sql);
            sql2 = "SELECT Active from " + Resources.SecurityDatabaseName + ".people where upper(user_id) = Upper(" + DALManager.BuildSqlStringParameterName("pUserName") + ")";
            dbCommand2 = this.DALManager.Database.GetSqlStringCommand(sql2);
            try
            {
                DALManager.Database.AddParameter(dbCommand, "pUserName", DbType.AnsiString, 40, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, userID.ToUpper());
                returnValue = System.Convert.ToInt16(DALManager.ExecuteScalar (dbCommand));
                if (returnValue <= 0)
                {
                    throw new InvalidCredentials("Login Failed. User was not found in " + Resources.SecurityDatabaseName + ".People table.");
                }
                else
                {
                    DALManager.Database.AddParameter(dbCommand2, "pUserName", DbType.AnsiString, 40, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, userID.ToUpper());
                    returnValue2 = System.Convert.ToInt16(DALManager.ExecuteScalar (dbCommand2));
                    if (returnValue2 <= 0)
                    {
                        throw new InvalidCredentials("Login Failed. User is set to not active in " + Resources.SecurityDatabaseName + ".People table.");
                    }
                }

            }
            catch (Exception ex)
            {
                throw;
            }
            return returnValue;
        }



       

        internal virtual SafeDataReader GetAllUsers()
        {
            SafeDataReader safeReader = null;
            try
            {
                //CSBR-133820
                string sql = "select * from " + Resources.SecurityDatabaseName + ".people where active =1 MINUS select * from " + Resources.SecurityDatabaseName + ".people where USER_ID = 'login_disabled' ORDER BY 3";
                DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sql);
                safeReader = new SafeDataReader(DALManager.Database.ExecuteReader(dbCommand));
            }
            catch (Exception ex)
            {

                throw;
            }
            return safeReader;

        }

        internal virtual DataTable GetAllCOEUsers()
        {
            return GetAllCOEUsers(string.Empty);
        }

        internal virtual DataTable GetAllCOEUsers(string coeIdentifier)
        {
            throw new Exception("must be implemented for specific provider");
        }

        internal virtual DataTable GetAllDBMSUsers()
        {
            throw new Exception("must be implemented for specific provider");
        }

        internal virtual SafeDataReader GetSiteList()
        {
             SafeDataReader safeReader = null;
            try
            {
                string sql = "SELECT Site_ID, Site_Name AS DisplayText FROM " + Resources.SecurityDatabaseName + ".Sites ORDER BY lower(Site_Name) ASC";
                DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sql);
                safeReader = new SafeDataReader(DALManager.Database.ExecuteReader(dbCommand));
            }
            catch (Exception ex)
            {

                throw;
            }
            return safeReader;

        }
        internal virtual SafeDataReader GetPersonList()
        {
            SafeDataReader safeReader = null;
            try
            {
                string sql = "SELECT Person_ID , Last_Name||DECODE(First_Name, NULL, '', ', '||First_Name) AS DisplayText FROM " + Resources.SecurityDatabaseName + ".People ORDER BY lower(Last_Name) ASC";
                DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sql);
                safeReader = new SafeDataReader(DALManager.Database.ExecuteReader(dbCommand));
            }
            catch (Exception ex)
            {

                throw;
            }
            return safeReader;

        }

        internal virtual SafeDataReader GetUsers(string privtablename)
        {
            //must be overriden by provider
            return null;

        }

        internal virtual SafeDataReader GetUsersByApplication(List<string> appNames)
        {
            string runningList = string.Empty;
            for (int i = 0; i < appNames.Count; i++)
            {
                if (i > 0)
                {
                    runningList = runningList + ",'" + Convert.ToString(appNames[i]) + "'";
                }
                else
                {
                    runningList = "'" + Convert.ToString(appNames[i]) + "'";
                }
            }

            SafeDataReader safeReader = null;
            try
            {
                string sql = "select distinct cp.user_id, cp.* from " + Resources.SecurityDatabaseName + ".people cp, " + Resources.SecurityDatabaseName + ".security_roles s, privilege_tables p, dba_role_privs rp" +
                     " where s.role_name = rp.granted_role " +
                     " and rp.granted_role" +
                     " in (select granted_role from dba_role_privs" +
                     " where  grantee = cp.user_id)" +
                     " and s.coeidentifier in(" + runningList + ")";
                DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sql);
                safeReader = new SafeDataReader(DALManager.Database.ExecuteReader(dbCommand));
            }
            catch (Exception ex)
            {

                throw;
            }
            return safeReader;

        }

        internal virtual SafeDataReader GetUsersByRole(List<string> roleNames)
        {
            SafeDataReader safeReader = null;
            if (roleNames.Count > 0)
            {
                string runningList = string.Empty;
                for (int i = 0; i < roleNames.Count; i++)
                {
                    if (i > 0)
                    {
                        runningList = runningList + ",'" + Convert.ToString(roleNames[i]) + "'";
                    }
                    else
                    {
                        runningList = "'" + Convert.ToString(roleNames[i]) + "'";
                    }
                }

                //SafeDataReader safeReader = null;
                try
                {
                    string sql = " select cp.* from " + Resources.SecurityDatabaseName + ".people cp, " + Resources.SecurityDatabaseName + ".security_roles s, " + Resources.SecurityDatabaseName + ".privilege_tables p, dba_role_privs rp" +
                         " where s.role_name = rp.granted_role " +
                         " and rp.granted_role" +
                         " in (select granted_role from dba_role_privs" +
                         " where  grantee = cp.user_id)" +
                         " and s.role_name in(" + runningList + ")";
                    DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sql);
                    safeReader = new SafeDataReader(DALManager.Database.ExecuteReader(dbCommand));
                }
                catch (Exception ex)
                {

                    throw;
                }
            }
            return safeReader;
        }

       

        internal virtual DataTable GetUser(string userName)
        {
            //must be overriden by provider
            return null;
        }

        internal virtual string GetUserNameById(int personId)
        {
            DataTable dt = null;
            DbCommand dbCommand = null;
            string sql = "SELECT user_id from " + Resources.SecurityDatabaseName + ".people where person_id = " + DALManager.BuildSqlStringParameterName("pPersonId");
            try
            {
                dbCommand = this.DALManager.Database.GetSqlStringCommand(sql);
                DALManager.Database.AddParameter(dbCommand, "pPersonId", DbType.Int32, ParameterDirection.Input, string.Empty, DataRowVersion.Current, personId);
            }
            catch (Exception ex) 
            {
                throw ex;
            }

            return (string)DALManager.Database.ExecuteScalar(dbCommand);
        }        

        internal virtual SafeDataReader GetUsersByID(List<int> userIDList)
        {
            SafeDataReader safeReader = null;
            if (userIDList.Count > 0)
            {
                string runningList = string.Empty;
                for (int i = 0; i < userIDList.Count; i++)
                {
                    if (i > 0)
                    {
                        runningList = runningList + "," + Convert.ToString(userIDList[i]);
                    }
                    else
                    {
                        runningList = Convert.ToString(userIDList[i]);
                    }
                }

                //SafeDataReader safeReader = null;
                DbCommand dbCommand = null;
                string sql = string.Empty;
                int returnValue = 0;
                //CSBR-133820
                sql = "SELECT person_id,user_code,user_id,supervisor_internal_id,title,first_name,middle_name,last_name,site_id,department,telephone,email,active from " + Resources.SecurityDatabaseName + ".people where person_id IN(" + runningList + ") ORDER BY user_id";
                dbCommand = this.DALManager.Database.GetSqlStringCommand(sql);
                try
                {
                    safeReader = new SafeDataReader(DALManager.Database.ExecuteReader(dbCommand));
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
            return safeReader;
        }

        internal virtual void DeleteUser(string user) 
        {
            throw new System.Exception("not implemented");

        }

        internal virtual void CreateUser(string userName, int isAlreadyInOracle, string password, string rolesGranted, string firstName, string middleName, string lastName, string telephone, string email, string address, string userCode, int supervisorID, int siteID, bool isActive, bool activatingUser)
        {
            throw new System.Exception("not implemented");

        }


        internal virtual void CreateLDAPUser(string userName,  string rolesGranted, string firstName, string middleName, string lastName, string telephone, string email, string address, string userCode, int supervisorID, int siteID, bool isActive)
        {
            throw new System.Exception("not implemented");

        }

        internal virtual void UpdateUser(string userName, string password, string rolesGranted, string rolesRevoked, string firstName, string middleName, string lastName, string telephone, string email, string address, string userCode, int supervisorID, int pSiteID, bool isActive)
        {
            throw new System.Exception("not implemented");

        }


        #endregion

        #region Roles

        internal virtual DataTable GetDBMSRoles()
        {
            throw new Exception("not implemented");
        }

        internal virtual SafeDataReader GetRolesByRoleIDS(List<int> roleIDList)
        {
            SafeDataReader safeReader = null;
            if (roleIDList.Count > 0)
            {
                string runningList = string.Empty;
                for (int i = 0; i < roleIDList.Count; i++)
                {
                    if (i > 0)
                    {
                        runningList = runningList + "," + Convert.ToString(roleIDList[i]);
                    }
                    else
                    {
                        runningList = Convert.ToString(roleIDList[i]);
                    }
                }
                
                DbCommand dbCommand = null;
                string sql = string.Empty;
                int returnValue = 0;

                //CSBR-133820
                sql = "SELECT * from " + Resources.SecurityDatabaseName + ".security_roles where role_id IN(" + runningList + ") ORDER BY ROLE_NAME";
                dbCommand = this.DALManager.Database.GetSqlStringCommand(sql);
                try
                {
                    safeReader = new SafeDataReader(DALManager.Database.ExecuteReader(dbCommand));
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
            return safeReader;
        }

        internal virtual DataSet GetUserRoleIds(string username, string privtablename)
        {
            DbCommand dbCommand = null;
            string sql = string.Empty;
            DataSet dataset = null;

            sql = "SELECT distinct s.role_id " +
                    " FROM security_roles s, privilege_tables p, dba_role_privs rp " +
                    " WHERE s.privilege_table_int_id = p.privilege_table_id " +
                    " AND s.role_name = rp.granted_role " +
                    " AND Upper(p.privilege_table_name) = " + DALManager.BuildSqlStringParameterName("pPrivilegeTableName") + 
                    " AND rp.granted_role IN (select granted_role " +
                    " from dba_role_privs " +
                    " WHERE  grantee = " + DALManager.BuildSqlStringParameterName("pUserName")  +
                    " UNION " +
                    " Select granted_role " +
                    " from dba_role_privs " +
                    " WHERE grantee IN (select granted_role " +
                    " from dba_role_privs " +
                    " WHERE  grantee = " + DALManager.BuildSqlStringParameterName("pUserName") + ") " +
                    " )";
            try
            {
            dbCommand = this.DALManager.Database.GetSqlStringCommand(sql);

            DALManager.Database.AddParameter(dbCommand, "pPrivilegeTableName", DbType.AnsiString, 100, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, privtablename.ToUpper());
            DALManager.Database.AddParameter(dbCommand, "pUserName", DbType.AnsiString, 40, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, username.ToUpper());


           
                dataset = DALManager.ExecuteDataSet(dbCommand);

            }
            catch (Exception ex)
            {
                
                    throw ex;
                
             }

            return dataset;
        }

        internal virtual DataTable GetAllCOERoles(string coeIdentifier)
        {
            throw new System.Exception("not implemented");
        }

        internal virtual DataTable GetAllCOEUserRoles(string coeIdentifier, string userName)
        {
            throw new System.Exception("not implemented");
        }
        internal virtual SafeDataReader GetAllRoles()
        {
            SafeDataReader safeReader = null;
            try
            {
                //CSBR-133820
                string sql = "select sr.*,pt.PRIVILEGE_TABLE_NAME from " + Resources.SecurityDatabaseName + ".security_roles sr," + Resources.SecurityDatabaseName + ".PRIVILEGE_TABLES pt where sr.PRIVILEGE_TABLE_INT_ID  =   pt.PRIVILEGE_TABLE_ID ORDER BY sr.ROLE_NAME";
                DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sql);
                safeReader = new SafeDataReader(DALManager.Database.ExecuteReader(dbCommand));
            }
            catch (Exception ex)
            {

                throw;
            }
            return safeReader;
        }


        internal virtual string GetPrivilegeTableName(string coeIdentifier){

                string sql = "select distinct pt.PRIVILEGE_TABLE_NAME from " + Resources.SecurityDatabaseName + ".security_roles sr," + Resources.SecurityDatabaseName + ".PRIVILEGE_TABLES pt where pt.Privilege_Table_id=sr.privilege_table_int_id and Upper(sr.COEIDENTIFIER)=" + DALManager.BuildSqlStringParameterName("pCOEIDENTIFIER");
                DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sql);
                DALManager.Database.AddParameter(dbCommand, "pCOEIDENTIFIER", DbType.AnsiString, 100, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, coeIdentifier.ToUpper());
                string pTName = (string)DALManager.Database.ExecuteScalar(dbCommand);
                return pTName;
        }


        internal virtual SafeDataReader GetRole(string roleName)
        {
            SafeDataReader safeReader = null;
            try
            {

                string sql = "select sr.*,pt.PRIVILEGE_TABLE_NAME from " + Resources.SecurityDatabaseName + ".security_roles sr," + Resources.SecurityDatabaseName + ".PRIVILEGE_TABLES pt where sr.PRIVILEGE_TABLE_INT_ID  =   pt.PRIVILEGE_TABLE_ID and ROLE_NAME =" + DALManager.BuildSqlStringParameterName("proleName");
                DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sql);
                DALManager.Database.AddParameter(dbCommand, "proleName", DbType.AnsiString, 100, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, roleName.ToUpper());

                safeReader = new SafeDataReader(DALManager.Database.ExecuteReader(dbCommand));
            }
            catch (Exception ex)
            {

                throw;
            }
            return safeReader;
        }

        internal virtual SafeDataReader GetRole(int roleID)
        {
            SafeDataReader safeReader = null;
            try
            {

                string sql = "select * from " + Resources.SecurityDatabaseName + ".roles where ROLE_ID =" + DALManager.BuildSqlStringParameterName("pRoleID");
                DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sql);
                DALManager.Database.AddInParameter(dbCommand, "pRoleID", DbType.Int32, roleID);

                safeReader = new SafeDataReader(DALManager.Database.ExecuteReader(dbCommand));
            }
            catch (Exception ex)
            {

                throw;
            }
            return safeReader;
        }

        internal virtual SafeDataReader GetRolesByApplication(List<string> appNames)
        {
            
            string runningList = string.Empty;
            for (int i = 0; i < appNames.Count; i++)
            {
                if (i > 0)
                {
                    runningList = runningList + ",'" + Convert.ToString(appNames[i]) + "'";
                }
                else
                {
                    runningList = "'" + Convert.ToString(appNames[i]) + "'";
                }
            }

            SafeDataReader safeReader = null;
            try
            {
                string sql = " select distinct s.role_id,s.role_name,(select privilege_table_name from " + Resources.SecurityDatabaseName + ".privilege_tables where  PRIVILEGE_TABLE_ID=s.PRIVILEGE_TABLE_INT_ID) as PRIVILEGE_TABLE_NAME  from " + Resources.SecurityDatabaseName + ".people cp, " + Resources.SecurityDatabaseName + ".security_roles s, " + Resources.SecurityDatabaseName + ".privilege_tables p, dba_role_privs rp" +
                     " where s.role_name = rp.granted_role " +
                      " and s.PRIVILEGE_TABLE_INT_ID =p.PRIVILEGE_TABLE_ID " +
                     " and rp.granted_role" +
                     " in (select granted_role from dba_role_privs" +
                     " where  grantee = cp.user_id)" +
                     " and s.coeidentifier in(" + runningList + ")";
                DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sql);
                safeReader = new SafeDataReader(DALManager.Database.ExecuteReader(dbCommand));
            }
            catch (Exception ex)
            {

                throw;
            }
            return safeReader;
        }

        internal virtual SafeDataReader GetRolesByUser(List<string> userNames)
        {
            string runningList = string.Empty;
            for (int i = 0; i < userNames.Count; i++)
            {
                if (i > 0)
                {
                    runningList = runningList + "," + Convert.ToString(userNames[i]);
                }
                else
                {
                    runningList = Convert.ToString(userNames[i]);
                }
            }

            SafeDataReader safeReader = null;
            try
            {
                string sql = " select distinct s.role_name, s.role_id, s.coeidentifier from " + Resources.SecurityDatabaseName + ".people cp, " + Resources.SecurityDatabaseName + ".security_roles s, " + Resources.SecurityDatabaseName + ".privilege_tables p, dba_role_privs rp" +
                     " where s.role_name = rp.granted_role " +
                     " and rp.grantee = cp.user_id " +
                     " and cp.user_id in (" + DALManager.BuildSqlStringParameterName("pUserName") +")";
                DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sql);
                DALManager.Database.AddParameter(dbCommand, "pUserName", DbType.AnsiString, 40, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, runningList.ToUpper());
                safeReader = new SafeDataReader(DALManager.Database.ExecuteReader(dbCommand));
            }
            catch (Exception ex)
            {

                throw;
            }
            return safeReader;
        }

        internal virtual void UpdateRole(string roleName, string privilegeTableName, string privValueList)
        {
            throw new System.Exception("not implemented");
        }

        internal virtual void DeleteRole(string privilegeTableName, string roleName)
        {
            throw new System.Exception("not implemented");

        }

        internal virtual void CreateRole(string roleName, string privilegeTableName, bool isInDBMS, string privValueList,string coeIdentifier)
        {
            throw new System.Exception("not implemented");

        }

        internal virtual DataTable GetRoleRoles(string roleName)
        {
            throw new System.Exception("not implemented");

        }

        internal virtual DataTable GetRoleAvailableRoles(string privilegeTableName, string roleName)
        {
            throw new System.Exception("not implemented");

        }


        internal virtual DataTable GetRoleAvailableUsers(string privilegeTableName, string roleName)
        {
            throw new System.Exception("not implemented");

        }

        internal virtual DataTable GetAvailableRoles(string userName, string privilegeTableName)
        {
            throw new System.Exception("not implemented");

        }

        internal virtual DataTable GetAvailableRoles(string userName)
        {
            throw new System.Exception("not implemented");

        }

        internal virtual DataTable GetAvaialableRoles(string userName)
        {
            throw new System.Exception("not implemented");

        }

        internal virtual DataTable GetRoleGrantees(string roleName)
        {
            throw new System.Exception("not implemented");

        }

        internal virtual void UpdateRolesGrantedToRole(string roleName, string rolesGranted, string rolesRevoked)
        {
            throw new System.Exception("not implemented");
        }

        internal virtual void UpdateUsersGrantedARole(string roleName, string usersGranted, string usersRevoked)
        {
            throw new System.Exception("not implemented");
        }

        #endregion

        #region password

        internal virtual bool ChangePassword(string userName, string oldPassword, string newPassword)
        {
            throw new System.Exception("not implemented");

        }

        #endregion

        #region privileges

        internal virtual Dictionary<string, List<string>> GetAllApplicationRoles(string userName)
        {
            return null;
        }

        internal virtual DataTable GetPrivilegesForRole(string rolename)
        {
            return null;
        }

         internal virtual DataTable GetDefaultPrivileges(string appName)
        {
            return null;
        }

        internal virtual DataSet GetPrivsByRoleId(string roleidlist, string privtablename)
        {

            DbCommand dbCommand = null;
            string sql = string.Empty;
            DataSet dataset = null;

            sql = "SELECT * FROM " + privtablename + " WHERE ROLE_INTERNAL_ID IN(" + roleidlist + ")";
            dbCommand = this.DALManager.Database.GetSqlStringCommand(sql);
            try
            {
                dataset = DALManager.ExecuteDataSet(dbCommand);

            }
            catch (Exception ex)
            {
                if (ex.Message=="ORA-00942: table or view does not exist")
                {
                    dataset = null;
                    return dataset;
                }
                else
                {
                    throw ex;
                }
            }
            return dataset;

        }

        internal virtual string GetPrivTablesName()
        {
            string columnName = "PRIVILEGE_TABLE_NAME";
           
            string sql = "SELECT PRIVILEGE_TABLE_NAME FROM PRIVILEGE_TABLES";
            string privsTables = null;
            DataSet dataSet = this.ExecuteSqlQuery(sql);
            
            foreach (DataRow dataRow in dataSet.Tables[0].Rows)
            {
                if (!string.IsNullOrEmpty(privsTables))
                    privsTables = privsTables + "," + dataRow[columnName].ToString();
                else
                    privsTables = dataRow[columnName].ToString();
            }
            return privsTables;
        }

        /// <summary>
        /// Method to execute a SQL query
        /// </summary>
        /// <param name="sql">The statment to run</param>
        /// <returns>The SQL response of the query</returns>
        private DataSet ExecuteSqlQuery(string sql)
        {
            DbCommand dbCommand = null;
            DataSet dataset = null;
            dbCommand = this.DALManager.Database.GetSqlStringCommand(sql);
            try
            {
                dataset = DALManager.ExecuteDataSet(dbCommand);
            }
            catch (Exception ex)
            {
                throw;
            }
            return dataset;
        }

        internal virtual DataSet GetRolePrivs(string rolename)
        {
            //must be overriden by provider
            return null;

        }



        #endregion

        #region access rights

        internal virtual SafeDataReader GetAccessRights(string objectType, int objectID, string principalType)
        {
            string sql ="select up.principalid " +
                " from " + Resources.SecurityDatabaseName + ".coepermissions up, coedb.coeobjecttype ot, coedb.coeprincipaltype pt " +
                " where up.objectypeid = ot.id " +
                " and ot.name = " + DALManager.BuildSqlStringParameterName("pObjectType") + 
                " and up.principaltypeid = pt.id " +
                " and pt.name = "+ DALManager.BuildSqlStringParameterName("pPrincipalType") +
                " and up.objectid= " + DALManager.BuildSqlStringParameterName("pObjectID");
            SafeDataReader safeReader = null;
            try
            {
                DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sql);
                DALManager.Database.AddParameter(dbCommand, "pObjectType", DbType.AnsiString, 1000, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, objectType);
                DALManager.Database.AddParameter(dbCommand, "pPrincipalType", DbType.AnsiString, 1000, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, principalType);
                DALManager.Database.AddInParameter(dbCommand, "pObjectID", DbType.Int32, objectID);
                safeReader = new SafeDataReader(DALManager.Database.ExecuteReader(dbCommand));
            }
            catch (Exception ex)
            {

            }
            return safeReader;
        }

        internal virtual void InsertAccessPrivilege(int objectID, string objectType, int principalID, string principalType)
        {

            string sql = "insert into " + Resources.SecurityDatabaseName + ".coepermissions(objectid,objectypeid,principalid,principaltypeid)" +
            "values(" + objectID + ",(select id from coeobjecttype where name = '" + objectType + "')," + 
            principalID + ",(select id from coeprincipaltype where name = '" + principalType + "'))";
            DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sql);
            int recordsAffected = DALManager.Database.ExecuteNonQuery(dbCommand);

        }

        internal virtual void DeleteAccessPrivileges(int objectID, string objectType)
        {

            string sql = "delete from " + Resources.SecurityDatabaseName + ".coepermissions where objectid = " + objectID + " and objectypeid = (select id from coeobjecttype where name = '" + objectType + "')";
            DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sql);
            int recordsAffected = DALManager.Database.ExecuteNonQuery(dbCommand);

        }

        #endregion

        #region applications/schemas

        internal virtual SafeDataReader GetAllApplicationNames()
        {
            SafeDataReader safeReader = null;
            try{
                string sql = "select  max(COEIDENTIFIER) COEIDENTIFIER, max(RID) RID from " + Resources.SecurityDatabaseName + ".security_roles where privilege_table_int_id is not null group by COEIDENTIFIER";
                DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sql);
                 safeReader = new SafeDataReader(DALManager.Database.ExecuteReader(dbCommand));
            }
            catch (Exception ex)
            {

            }
            return safeReader;

        }

        #endregion

        internal virtual string GetGroupHeirarchyXML(int groupOrgID)
        {

            //must be overriden by provider
            throw new Exception("The method or operation is not implemented.");

        }
        internal virtual DataTable GetGroupList(int groupOrgID, int groupID)
        {

            //must be overriden by provider
            throw new Exception("The method or operation is not implemented.");

        }
        internal virtual DataTable GetPeopleList()
        {

            //must be overriden by provider
            throw new Exception("The method or operation is not implemented.");

        }
        internal virtual void DeleteGroup(int groupID)
        {

            //must be overriden by provider
            throw new Exception("The method or operation is not implemented.");

        }
        internal virtual void SetGroupPrivSets(int groupID, int[] privsetList)
        {
            //must be overriden by provider
            throw new Exception("The method or operation is not implemented.");
        }

        internal virtual void SetGroupUsers(int groupID, int[] userList)
        {
            //must be overriden by provider
            throw new Exception("The method or operation is not implemented.");
        }

        internal virtual int AddGroup(string groupName, int groupOrgID, int parent_GroupID, int _leaderPersonID, int[] userList, int[] privsetList)
        {

            //must be overriden by provider
            throw new Exception("The method or operation is not implemented.");

        }

        internal virtual int AddGroup(string groupName, int groupOrgID, int parent_GroupID, int _leaderPersonID)
        {

            //must be overriden by provider
            throw new Exception("The method or operation is not implemented.");

        }

        internal virtual void UpdateGroup(int groupID, string groupName, int parent_GroupID, int _leaderPersonID, int[] userList, int[] privsetList)
        {

            //must be overriden by provider
            throw new Exception("The method or operation is not implemented.");

        }

        internal virtual void UpdateGroup(int groupID, string groupName, int parent_GroupID, int _leaderPersonID)
        {

            //must be overriden by provider
            throw new Exception("The method or operation is not implemented.");

        }
        internal virtual DataTable GetGroupUserDirectAvailPrivsets(int personID)
        {
            //must be overriden by provider
            throw new Exception("The method or operation is not implemented.");
        }
        internal virtual DataTable GetGroupUserDirectPrivsets(int personID)
        {
            //must be overriden by provider
            throw new Exception("The method or operation is not implemented.");
        }
        internal virtual DataTable GetGroup(int groupID)
        {
            //must be overriden by provider
            throw new Exception("The method or operation is not implemented.");
        }
        internal virtual DataTable GetUserGroupPrivsets(int groupID, int personID)
        {
            //must be overriden by provider
            throw new Exception("The method or operation is not implemented.");
        }
        internal virtual DataTable GetGroupPrivSets(int groupID)
        {
            //must be overriden by provider
            throw new Exception("The method or operation is not implemented.");
        }
        internal virtual DataTable GetPersonNonGSRoles(int groupID, int personID)
        {
            //must be overriden by provider
            throw new Exception("The method or operation is not implemented.");
        }
        internal virtual DataTable GetGroupAvailablePrivSets(int groupID)
        {
            //must be overriden by provider
            throw new Exception("The method or operation is not implemented.");
        }
        internal virtual DataTable GetGroupUsers(int groupID)
        {
            //must be overriden by provider
            throw new Exception("The method or operation is not implemented.");
        }
    }
}
