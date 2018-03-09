using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using System.Data;
using Csla.Data;
using Oracle.DataAccess.Client;
using Oracle.DataAccess.Types;
using CambridgeSoft.COE.Framework.Properties;
using CambridgeSoft.COE.Framework.COELoggingService;
using CambridgeSoft.COE.Framework.Common;



namespace CambridgeSoft.COE.Framework.COESecurityService
{
    public class OracleDataAccessClientDAL : CambridgeSoft.COE.Framework.COESecurityService.DAL
    {
        [NonSerialized]
        static COELog _coeLog = COELog.GetSingleton("COESecurity");

        #region version info
        internal override Version GetFrameworkSchemaVersion()
        {
            //NOTE: If you initialize the COE Logger while the user is not authenticated, then it will always
            //  use some fallback user name in the logging folders and files. This is bad.
            //_coeLog.LogStart("GetFrameworkSchemaVersion");
            string version = string.Empty;
            Version versionReturn = null;
            string sql = "select VALUE from " + _globalsTableName + " WHERE UPPER(ID)='SCHEMAVERSION'";
            DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sql);
            try
            {
                version = (string)DALManager.ExecuteScalar(dbCommand);
                versionReturn = new Version(version);
            }
            catch (Exception ex)
            {
                throw new InvalidConnection(string.Format(Resources.InvalidConnectionServiceName, DALManager.DatabaseData.DBMSType.ToString(), DALManager.DBMSTypeData.DataSource.ToString()));
            }
            finally
            {
                //_coeLog.LogEnd("GetFrameworkSchemaVersion");
            }
            return versionReturn;

        }

        #endregion

        #region sessions
        internal override int StartSession(int user_id)
        {
            _coeLog.LogStart("StartSession");
            //retrieving the id to be inserted
            int id = GetNewID();
            string sql = "INSERT INTO " + _sessionTableName +
                 "(ID,USER_ID,STARTTIME)VALUES " +
                 "( " + DALManager.BuildSqlStringParameterName("pId") +
                 " , " + DALManager.BuildSqlStringParameterName("pUserId") +
                 " , " + DALManager.BuildSqlStringParameterName("pStartTime") + " )";
            DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sql);

            DALManager.Database.AddInParameter(dbCommand, "pId", DbType.Int32, id);
            DALManager.Database.AddInParameter(dbCommand, "pUserId", DbType.Int32, id);
            DALManager.Database.AddInParameter(dbCommand, "pStartTime", DbType.Date, DateTime.Now);
            int recordsAffected = -1;
            try
            {
                recordsAffected = DALManager.ExecuteNonQuery(dbCommand);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                _coeLog.LogEnd("StartSession");
            }
            return id;
        }

        internal override void EndSession(int sessionID)
        {
            _coeLog.LogStart("EndSession");
            string sql = "UPDATE " + _sessionTableName +
                " SET ENDTIME= " + DALManager.BuildSqlStringParameterName("pEndTime") +
                " WHERE ID=" + DALManager.BuildSqlStringParameterName("pId");

            DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sql);
            DALManager.Database.AddInParameter(dbCommand, "pEndTime", DbType.Date, DateTime.Now);
            DALManager.Database.AddInParameter(dbCommand, "pId", DbType.Int32, sessionID);
            int recordsAffected = -1;
            try
            {
                recordsAffected = DALManager.ExecuteNonQuery(dbCommand);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                _coeLog.LogEnd("EndSession");
            }
        }

        /// <summary>
        /// to return the next id
        /// </summary>
        /// <param name="dataView"></param>
        /// <returns></returns>
        public int GetNewID()
        {
            _coeLog.LogStart("GetNewID");
            string sql = "SELECT " + _sessionTableName + "_SEQ.NEXTVAL FROM DUAL";

            int id = -1;

            DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sql);

            try
            {
                id = Convert.ToInt32(DALManager.ExecuteScalar(dbCommand));
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                _coeLog.LogEnd("GetNewID");
            }
            return id;
        }

        #endregion

        #region Users
        internal override DataTable GetUser(string userName)
        {
            DataTable dt = new DataTable();
            OracleConnection connection = null;
            OracleRefCursor refCursor = null;
            OracleDataAdapter da = new OracleDataAdapter();
            OracleCommand dbCommand = new OracleCommand();
            try
            {
                connection = (OracleConnection)DALManager.Database.CreateConnection();

                dbCommand.Connection = connection;
                dbCommand.CommandType = CommandType.StoredProcedure;
                dbCommand.CommandText = Resources.SecurityDatabaseName + ".MANAGE_USERS.GETUSER";
                dbCommand.Parameters.Add(new OracleParameter("PUSERNAME", OracleDbType.Varchar2, userName.ToUpper(), ParameterDirection.Input));
                dbCommand.Parameters.Add(new OracleParameter("O_RS", OracleDbType.RefCursor, ParameterDirection.Output));
                connection.Open();
                dbCommand.ExecuteNonQuery();
                refCursor = (OracleRefCursor)dbCommand.Parameters[1].Value;

                da.Fill(dt, refCursor);
            }
            catch (Exception)
            {
                throw;
            }
            //make sure no matter what the adapters and connections are disposed
            finally
            {
                da.Dispose();
                refCursor.Dispose();
                DALUtils.DestroyCommandAndConnection(dbCommand);
            }
            if (dt == null)
            {

                throw new System.Exception("user does not exist");
            }
            return dt;
        }

        internal override DataTable GetAllCOEUsers()
        {
            return GetAllCOEUsers(string.Empty);
        }

        internal override DataTable GetAllCOEUsers(string coeIdentifier)
        {
            DataTable dt = new DataTable();
            OracleConnection connection = null;
            OracleDataAdapter da = null;
            OracleRefCursor refCursor = null;
            OracleCommand dbCommand = new OracleCommand();
            try
            {
                connection = (OracleConnection)DALManager.Database.CreateConnection();
                dbCommand.Connection = connection;
                dbCommand.CommandType = CommandType.StoredProcedure;
                dbCommand.CommandText = Resources.SecurityDatabaseName + ".MANAGE_USERS.GETCOEUSERS";
                dbCommand.Parameters.Add(new OracleParameter("pCOEIdentifier", OracleDbType.Varchar2, coeIdentifier, ParameterDirection.Input));
                dbCommand.Parameters.Add(new OracleParameter("O_RS", OracleDbType.RefCursor, ParameterDirection.Output));
                connection.Open();
                dbCommand.ExecuteNonQuery();
                refCursor = (OracleRefCursor)dbCommand.Parameters[1].Value;
                da = new OracleDataAdapter();
                da.Fill(dt, refCursor);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                da.Dispose();
                refCursor.Dispose();
                DALUtils.DestroyCommandAndConnection(dbCommand);
            }
            if (dt == null)
            {
                throw new System.Exception("user does not exist");
            }
            return dt;
        }

        internal override DataTable GetAllDBMSUsers()
        {
            OracleConnection connection = null;
            DataTable dt = new DataTable();
            OracleRefCursor refCursor = null;
            OracleDataAdapter da = new OracleDataAdapter();
            OracleCommand dbCommand = new OracleCommand();
            try
            {
                connection = (OracleConnection)DALManager.Database.CreateConnection();

                dbCommand.Connection = connection;
                dbCommand.CommandType = CommandType.StoredProcedure;
                dbCommand.CommandText = Resources.SecurityDatabaseName + ".MANAGE_USERS.GETALLUSERS";
                dbCommand.Parameters.Add(new OracleParameter("O_RS", OracleDbType.RefCursor, ParameterDirection.Output));
                connection.Open();
                dbCommand.ExecuteNonQuery();
                refCursor = (OracleRefCursor)dbCommand.Parameters[0].Value;

                da.Fill(dt, refCursor);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                da.Dispose();
                refCursor.Dispose();
                DALUtils.DestroyCommandAndConnection(dbCommand);
            }
            if (dt == null)
            {
                throw new System.Exception("user does not exist");
            }
            return dt;
        }

        internal override void DeleteUser(string userName)
        {
            try
            {
                string returnValue = "0";
                DbCommand dbCommand = DALManager.Database.GetStoredProcCommand(Resources.SecurityDatabaseName + ".DELETEUSER");
                dbCommand.Parameters.Add(new OracleParameter("RETURN_VALUE", OracleDbType.Int16, 0, ParameterDirection.ReturnValue, true, 8, 0, String.Empty, DataRowVersion.Current, returnValue));
                dbCommand.Parameters.Add(new OracleParameter("PUSERNAME", OracleDbType.Varchar2, userName.ToUpper(), ParameterDirection.Input));
                DALManager.ExecuteNonQuery(dbCommand);
                returnValue = System.Convert.ToString(dbCommand.Parameters[0].Value);
                if (returnValue != "1")
                {
                    throw new System.Exception(returnValue);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        internal override void CreateUser(string userName, int isAlreadyInOracle, string password, string rolesGranted, string firstName, string middleName, string lastName, string telephone, string email, string address, string userCode, int supervisorID, int siteID, bool isActive, bool activatingUser)
        {
            try
            {
                string returnValue = "0";
                DbCommand dbCommand = DALManager.Database.GetStoredProcCommand(Resources.SecurityDatabaseName + ".CREATEUSER");
                dbCommand.Parameters.Add(new OracleParameter("RETURN_VALUE", OracleDbType.Varchar2, 100, ParameterDirection.ReturnValue, true, 8, 0, String.Empty, DataRowVersion.Current, returnValue));
                dbCommand.Parameters.Add(new OracleParameter("PUSERNAME", OracleDbType.Varchar2, userName.ToUpper(), ParameterDirection.Input));
                dbCommand.Parameters.Add(new OracleParameter("pIsAlreadyInOracle", OracleDbType.Int16, isAlreadyInOracle, ParameterDirection.Input));
                if (password != null)
                {
                    dbCommand.Parameters.Add(new OracleParameter("pPassword", OracleDbType.Varchar2, (char)34 + password + (char)34, ParameterDirection.Input));
                }
                else
                {
                    dbCommand.Parameters.Add(new OracleParameter("pPassword", OracleDbType.Varchar2, null, ParameterDirection.Input));
                }
                dbCommand.Parameters.Add(new OracleParameter("pRolesGranted", OracleDbType.Varchar2, rolesGranted, ParameterDirection.Input));
                dbCommand.Parameters.Add(new OracleParameter("pFirstName", OracleDbType.Varchar2, firstName, ParameterDirection.Input));
                dbCommand.Parameters.Add(new OracleParameter("pMiddleName", OracleDbType.Varchar2, middleName, ParameterDirection.Input));
                dbCommand.Parameters.Add(new OracleParameter("pLastName", OracleDbType.Varchar2, lastName, ParameterDirection.Input));
                dbCommand.Parameters.Add(new OracleParameter("pTelephone", OracleDbType.Varchar2, telephone, ParameterDirection.Input));
                dbCommand.Parameters.Add(new OracleParameter("pEmail", OracleDbType.Varchar2, email, ParameterDirection.Input));
                dbCommand.Parameters.Add(new OracleParameter("pAddress", OracleDbType.Varchar2, address, ParameterDirection.Input));
                dbCommand.Parameters.Add(new OracleParameter("pUserCode", OracleDbType.Varchar2, userCode.ToUpper(), ParameterDirection.Input));
                if (supervisorID > 0)
                    dbCommand.Parameters.Add(new OracleParameter("pSupervisorID", OracleDbType.Int32, supervisorID, ParameterDirection.Input));
                else
                    dbCommand.Parameters.Add(new OracleParameter("pSupervisorID", OracleDbType.Int32, DBNull.Value, ParameterDirection.Input));
                if (siteID > 0)
                    dbCommand.Parameters.Add(new OracleParameter("pSiteID", OracleDbType.Int16, siteID, ParameterDirection.Input));
                else
                    dbCommand.Parameters.Add(new OracleParameter("pSiteID", OracleDbType.Int16, DBNull.Value, ParameterDirection.Input));
                dbCommand.Parameters.Add(new OracleParameter("pIsActive", OracleDbType.Int16, Convert.ToInt16(isActive), ParameterDirection.Input));
                dbCommand.Parameters.Add(new OracleParameter("pActivatingUser", OracleDbType.Char, 1, (activatingUser ? '1' : '0'), ParameterDirection.Input));
                DALManager.ExecuteNonQuery(dbCommand);

                returnValue = System.Convert.ToString(dbCommand.Parameters[0].Value);
                if (returnValue != "1")
                {
                    throw new Exception(returnValue);
                }
            }
            catch (OracleException ex)
            {
                switch (ex.Number)
                {
                    case 1711:
                        throw new Exception(Resources.Oracle1711);
                    case 911:
                        throw new Exception(Resources.OracleInvalidCharsUserName);
                    case 1935:
                        throw new Exception(Resources.OracleInvalidUserName);
                    default:
                        throw new Exception(ex.Message);
                }
            }
            catch (System.Exception exsys)
            {
                throw exsys;
            }
        }

        internal override void CreateLDAPUser(string userName, string rolesGranted, string firstName, string middleName, string lastName, string telephone, string email, string address, string userCode, int supervisorID, int siteID, bool isActive)
        {
            try
            {
                string returnValue = "0";
                DbCommand dbCommand = DALManager.Database.GetStoredProcCommand(Resources.SecurityDatabaseName + ".CREATEUSERFROMLDAP");
                dbCommand.Parameters.Add(new OracleParameter("RETURN_VALUE", OracleDbType.Varchar2, 100, ParameterDirection.ReturnValue, true, 8, 0, String.Empty, DataRowVersion.Current, returnValue));
                dbCommand.Parameters.Add(new OracleParameter("PUSERNAME", OracleDbType.Varchar2, userName.ToUpper(), ParameterDirection.Input));
                dbCommand.Parameters.Add(new OracleParameter("pRolesGranted", OracleDbType.Varchar2, rolesGranted, ParameterDirection.Input));
                dbCommand.Parameters.Add(new OracleParameter("pFirstName", OracleDbType.Varchar2, firstName, ParameterDirection.Input));
                dbCommand.Parameters.Add(new OracleParameter("pMiddleName", OracleDbType.Varchar2, middleName, ParameterDirection.Input));
                dbCommand.Parameters.Add(new OracleParameter("pLastName", OracleDbType.Varchar2, lastName, ParameterDirection.Input));
                dbCommand.Parameters.Add(new OracleParameter("pTelephone", OracleDbType.Varchar2, telephone, ParameterDirection.Input));
                dbCommand.Parameters.Add(new OracleParameter("pEmail", OracleDbType.Varchar2, email, ParameterDirection.Input));
                dbCommand.Parameters.Add(new OracleParameter("pAddress", OracleDbType.Varchar2, address, ParameterDirection.Input));
                dbCommand.Parameters.Add(new OracleParameter("pUserCode", OracleDbType.Varchar2, userCode.ToUpper(), ParameterDirection.Input));
                dbCommand.Parameters.Add(new OracleParameter("pSupervisorID", OracleDbType.Int32, supervisorID, ParameterDirection.Input));
                dbCommand.Parameters.Add(new OracleParameter("pSiteID", OracleDbType.Int16, siteID, ParameterDirection.Input));
                dbCommand.Parameters.Add(new OracleParameter("pIsActive", OracleDbType.Int16, Convert.ToInt16(isActive), ParameterDirection.Input));

                DALManager.ExecuteNonQuery(dbCommand);

                returnValue = System.Convert.ToString(dbCommand.Parameters[0].Value);
                if (returnValue != "1")
                {
                    throw new System.Exception(returnValue);
                }
            }
            catch (OracleException ex)
            {
                switch (ex.Number)
                {
                    case 1711:
                        throw new Exception(Resources.Oracle1711);
                    default:
                        throw new Exception(ex.Message);
                }
            }
            catch (System.Exception exsys)
            {
                throw exsys;
            }
        }

        internal override void UpdateUser(string userName, string password, string rolesGranted, string rolesRevoked, string firstName, string middleName, string lastName, string telephone, string email, string address, string userCode, int supervisorID, int siteID, bool isActive)
        {
            try
            {
                //Fixed Bug with ID:132972
                //This condition checks for the dummy text displayed at presention layer. if similar then null is send to SP
                if (password == "@@@@@@@@@@")
                {
                    password = null;
                }
                string returnValue = "0";
                DbCommand dbCommand = DALManager.Database.GetStoredProcCommand(Resources.SecurityDatabaseName + ".UPDATEUSER");
                dbCommand.Parameters.Add(new OracleParameter("RETURN_VALUE", OracleDbType.Int16, 0, ParameterDirection.ReturnValue, true, 8, 0, String.Empty, DataRowVersion.Current, returnValue));
                dbCommand.Parameters.Add(new OracleParameter("PUSERNAME", OracleDbType.Varchar2, userName.ToUpper(), ParameterDirection.Input));
                if (password != null)
                {
                    dbCommand.Parameters.Add(new OracleParameter("pPassword", OracleDbType.Varchar2, (char)34 + password + (char)34, ParameterDirection.Input));
                }
                else
                {
                    dbCommand.Parameters.Add(new OracleParameter("pPassword", OracleDbType.Varchar2, null, ParameterDirection.Input));
                }
                dbCommand.Parameters.Add(new OracleParameter("pRolesGranted", OracleDbType.Varchar2, rolesGranted, ParameterDirection.Input));
                dbCommand.Parameters.Add(new OracleParameter("pRolesRevoked", OracleDbType.Varchar2, rolesRevoked, ParameterDirection.Input));
                dbCommand.Parameters.Add(new OracleParameter("pFirstName", OracleDbType.Varchar2, firstName, ParameterDirection.Input));
                dbCommand.Parameters.Add(new OracleParameter("pMiddleName", OracleDbType.Varchar2, middleName, ParameterDirection.Input));
                dbCommand.Parameters.Add(new OracleParameter("pLastName", OracleDbType.Varchar2, lastName, ParameterDirection.Input));
                dbCommand.Parameters.Add(new OracleParameter("pTelephone", OracleDbType.Varchar2, telephone, ParameterDirection.Input));
                dbCommand.Parameters.Add(new OracleParameter("pEmail", OracleDbType.Varchar2, email, ParameterDirection.Input));
                dbCommand.Parameters.Add(new OracleParameter("pAddress", OracleDbType.Varchar2, address, ParameterDirection.Input));
                dbCommand.Parameters.Add(new OracleParameter("pUserCode", OracleDbType.Varchar2, userCode.ToUpper(), ParameterDirection.Input));
                if (supervisorID > 0)
                    dbCommand.Parameters.Add(new OracleParameter("pSupervisorID", OracleDbType.Int32, supervisorID, ParameterDirection.Input));
                else
                    dbCommand.Parameters.Add(new OracleParameter("pSupervisorID", OracleDbType.Int32, DBNull.Value, ParameterDirection.Input));
                if (siteID > 0)
                    dbCommand.Parameters.Add(new OracleParameter("pSiteID", OracleDbType.Int32, siteID, ParameterDirection.Input));
                else
                    dbCommand.Parameters.Add(new OracleParameter("pSiteID", OracleDbType.Int32, DBNull.Value, ParameterDirection.Input));
                dbCommand.Parameters.Add(new OracleParameter("pIsActive", OracleDbType.Int32, Convert.ToInt32(isActive), ParameterDirection.Input));

                DALManager.ExecuteNonQuery(dbCommand);
                returnValue = System.Convert.ToString(dbCommand.Parameters[0].Value);
                if (returnValue != "1")
                {
                    throw new System.Exception(returnValue);
                }
            }
            catch (OracleException ex)
            {
                switch (ex.Number)
                {
                    case 1711:
                        throw new Exception(Resources.Oracle1711);
                    default:
                        throw new Exception(ex.Message);
                }
            }
            catch (System.Exception exsys)
            {
                throw exsys;
            }
        }

        #endregion

        #region Privileges
        internal override DataTable GetDefaultPrivileges(string appName)
        {
            DataTable dt = new DataTable();
            OracleConnection connection = null;
            OracleCommand cmd = new OracleCommand();
            OracleRefCursor refCursor = null;
            OracleDataAdapter da = new OracleDataAdapter();
            try
            {
                connection = (OracleConnection)DALManager.Database.CreateConnection();

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = Resources.SecurityDatabaseName + ".MANAGE_ROLES.GETAPPDEFAULTPRIVS";
                cmd.Connection = connection;
                OracleParameter param = new OracleParameter("PCOEIdentifier", OracleDbType.Varchar2, appName, ParameterDirection.Input);
                cmd.Parameters.Add(param);
                OracleParameter paramCursor = new OracleParameter("O_RS", OracleDbType.RefCursor, ParameterDirection.Output);
                cmd.Parameters.Add(paramCursor);
                connection.Open();
                cmd.ExecuteNonQuery();
                refCursor = (OracleRefCursor)cmd.Parameters[1].Value;

                da.Fill(dt, refCursor);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                da.Dispose();
                if (refCursor != null)
                {
                    refCursor.Dispose();
                }
                DALUtils.DestroyCommandAndConnection(cmd);
            }
            return dt;
        }


        /// <summary>
        /// CReturns a single record whose fields contain the privilege names for that role  <br/><br/>
        /// </summary>
        /// <param name="rolename">role name</param>
        /// <returns>reader with privilege names</returns>
        internal override DataTable GetPrivilegesForRole(string rolename)
        {
            DataTable dt = new DataTable();
            OracleConnection connection = null;
            OracleCommand cmd = new OracleCommand();
            OracleRefCursor refCursor = null;
            OracleDataAdapter da = new OracleDataAdapter();
            try
            {
                connection = (OracleConnection)DALManager.Database.CreateConnection();

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = Resources.SecurityDatabaseName + ".MANAGE_ROLES.GETROLEPRIVS";
                cmd.Connection = connection;
                OracleParameter param = new OracleParameter("PROLENAME", OracleDbType.Varchar2, rolename, ParameterDirection.Input);
                cmd.Parameters.Add(param);
                OracleParameter paramCursor = new OracleParameter("O_RS", OracleDbType.RefCursor, ParameterDirection.Output);
                cmd.Parameters.Add(paramCursor);
                connection.Open();
                cmd.ExecuteNonQuery();
                refCursor = (OracleRefCursor)cmd.Parameters[1].Value;

                da.Fill(dt, refCursor);
            }
            catch (Exception ex)
            {
                if (ex.Message.StartsWith("ORA-01403")) //No data found, but no error
                    return dt;
                throw ex;
            }
            finally
            {
                da.Dispose();
                if (refCursor != null)
                {
                    refCursor.Dispose();
                }
                DALUtils.DestroyCommandAndConnection(cmd);
            }

            return dt;
        }

        internal override Dictionary<string, List<string>> GetAllApplicationRoles(string userName)
        {
            DataTable dt = new DataTable();
            Dictionary<string, List<string>> result = new Dictionary<string, List<string>>();
            OracleConnection connection = null;
            OracleCommand cmd = new OracleCommand();
            OracleRefCursor refCursor = null;
            OracleDataAdapter da = new OracleDataAdapter();
            try
            {
                connection = (OracleConnection)DALManager.Database.CreateConnection();

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = Resources.SecurityDatabaseName + ".Login.getPrivsByUser";
                cmd.Connection = connection;
                OracleParameter param = new OracleParameter("pUserName", OracleDbType.Varchar2, userName, ParameterDirection.Input);
                cmd.Parameters.Add(param);
                OracleParameter paramCursor = new OracleParameter("o_rs", OracleDbType.RefCursor, ParameterDirection.Output);
                cmd.Parameters.Add(paramCursor);
                connection.Open();
                cmd.ExecuteNonQuery();
                refCursor = (OracleRefCursor)cmd.Parameters[1].Value;

                da.Fill(dt, refCursor);

            }
            catch (Exception ex)
            {
                if (ex.Message.StartsWith("ORA-01403")) //No data found, but no error
                    return result;
                throw ex;
            }
            finally
            {
                da.Dispose();
                if (refCursor != null)
                {
                    refCursor.Dispose();
                }
                DALUtils.DestroyCommandAndConnection(cmd);
            }
            foreach (DataRow row in dt.Rows)
            {
                if (row["PRIV_VALUE"].ToString() == "1")
                {
                    string coeIdentifier = row["PRIV_SCOPE"].ToString().ToUpper();
                    if (result.ContainsKey(coeIdentifier))
                    {
                        if (!result[coeIdentifier].Contains(row["PRIV_NAME"].ToString()))
                            result[coeIdentifier].Add(row["PRIV_NAME"].ToString());
                    }
                    else
                    {
                        List<string> privs = new List<string>();
                        privs.Add(row["PRIV_NAME"].ToString());
                        result.Add(coeIdentifier, privs);
                    }
                }
            }
            return result;
        }
        #endregion

        #region roles
        internal override SafeDataReader GetRole(string roleName)
        {
            SafeDataReader safeReader = null;
            try
            {
                string sql = "select sr.*,pt.PRIVILEGE_TABLE_NAME from " + Resources.SecurityDatabaseName + ".security_roles sr," + Resources.SecurityDatabaseName + ".PRIVILEGE_TABLES pt where sr.PRIVILEGE_TABLE_INT_ID = pt.PRIVILEGE_TABLE_ID(+) and ROLE_NAME =" + DALManager.BuildSqlStringParameterName("proleName");
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

        internal override DataTable GetDBMSRoles()
        {
            DataTable dt = new DataTable();
            OracleConnection connection = null;
            OracleCommand cmd = new OracleCommand();
            OracleRefCursor refCursor = null;
            OracleDataAdapter da = new OracleDataAdapter();
            try
            {
                connection = (OracleConnection)DALManager.Database.CreateConnection();

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = Resources.SecurityDatabaseName + ".MANAGE_ROLES.GETALLROLES";
                cmd.Connection = connection;
                OracleParameter paramCursor = new OracleParameter("O_RS", OracleDbType.RefCursor, ParameterDirection.Output);
                cmd.Parameters.Add(paramCursor);
                connection.Open();
                cmd.ExecuteNonQuery();
                refCursor = (OracleRefCursor)cmd.Parameters[0].Value;

                da.Fill(dt, refCursor);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                da.Dispose();
                refCursor.Dispose();
                DALUtils.DestroyCommandAndConnection(cmd);
            }

            return dt;
        }

        internal override void UpdateRole(string roleName, string privilegeTableName, string privValueList)
        {
            string returnValue = "0";
            try
            {
                DbCommand dbCommand = DALManager.Database.GetStoredProcCommand(Resources.SecurityDatabaseName + ".UPDATEROLE");
                dbCommand.Parameters.Add(new OracleParameter("RETURN_VALUE", OracleDbType.Varchar2, 300, ParameterDirection.ReturnValue, true, 8, 0, String.Empty, DataRowVersion.Current, returnValue));
                dbCommand.Parameters.Add(new OracleParameter("pRoleName", OracleDbType.Varchar2, roleName.Length, roleName.ToUpper(), ParameterDirection.Input));
                dbCommand.Parameters.Add(new OracleParameter("pPrivilegeTableName", OracleDbType.Varchar2, privilegeTableName.Length, privilegeTableName.ToUpper(), ParameterDirection.Input));
                dbCommand.Parameters.Add(new OracleParameter("privValueList", OracleDbType.Varchar2, privValueList.Length, privValueList, ParameterDirection.Input));
                DALManager.ExecuteNonQuery(dbCommand);
                returnValue = System.Convert.ToString(dbCommand.Parameters[0].Value);
                if (returnValue != "1")
                {
                    throw new System.Exception(returnValue);
                }
            }
            catch (OracleException ex)
            {
                switch (ex.Number)
                {
                    case 1934:
                        throw new Exception(Resources.Oracle1934);
                    default:
                        throw new Exception(ex.Message);
                }
            }
            catch (System.Exception exsys)
            {
                throw exsys;
            }
        }

        internal override void DeleteRole(string privilegeTableName, string roleName)
        {
            try
            {
                string returnValue = "0";
                DbCommand dbCommand = DALManager.Database.GetStoredProcCommand(Resources.SecurityDatabaseName + ".DELETEROLE");
                dbCommand.Parameters.Add(new OracleParameter("RETURN_VALUE", OracleDbType.Varchar2, 0, ParameterDirection.ReturnValue, true, 8, 0, String.Empty, DataRowVersion.Current, returnValue));
                dbCommand.Parameters.Add(new OracleParameter("pRoleName", OracleDbType.Varchar2, roleName.Length, roleName.ToUpper(), ParameterDirection.Input));
                dbCommand.Parameters.Add(new OracleParameter("pPrivilegeTableName", OracleDbType.Varchar2, privilegeTableName.Length, privilegeTableName.ToUpper(), ParameterDirection.Input));
                DALManager.ExecuteNonQuery(dbCommand);
                returnValue = System.Convert.ToString(dbCommand.Parameters[0].Value);
                if (returnValue != "1")
                {
                    throw new System.Exception(returnValue);
                }
            }
            catch (OracleException ex)
            {
                switch (ex.Number)
                {
                    case 20000:
                        throw new Exception(Resources.Oracle20000);
                    case 6502:
                        //this is an artifact. The deletion is working fine.
                        break;
                    default:
                        throw new Exception(ex.Message);
                }
            }
            catch (System.Exception exsys)
            {
                throw exsys;
            }
        }

        internal override void CreateRole(string roleName, string privilegeTableName, bool isInDBMS, string privValueList, string coeIdentifier)
        {
            string returnValue = "0";
            //prepend empty value for roleid;

            try
            {
                DbCommand dbCommand = DALManager.Database.GetStoredProcCommand(Resources.SecurityDatabaseName + ".CREATECOEROLE");
                dbCommand.Parameters.Add(new OracleParameter("RETURN_VALUE", OracleDbType.Varchar2, 80, ParameterDirection.ReturnValue, true, 8, 0, String.Empty, DataRowVersion.Current, returnValue));
                dbCommand.Parameters.Add(new OracleParameter("pRoleName", OracleDbType.Varchar2, roleName.Length, roleName.ToUpper(), ParameterDirection.Input));
                dbCommand.Parameters.Add(new OracleParameter("pPrivilegeTableName", OracleDbType.Varchar2, privilegeTableName.Length, privilegeTableName.ToUpper(), ParameterDirection.Input));
                dbCommand.Parameters.Add(new OracleParameter("pIsAlreadyInOracle", OracleDbType.Int16, 1, System.Convert.ToInt16(isInDBMS), ParameterDirection.Input));
                dbCommand.Parameters.Add(new OracleParameter("privValueList", OracleDbType.Varchar2, privValueList.Length, privValueList, ParameterDirection.Input));
                dbCommand.Parameters.Add(new OracleParameter("pCOEIdentifier", OracleDbType.Varchar2, coeIdentifier.Length, coeIdentifier, ParameterDirection.Input));

                DALManager.ExecuteNonQuery(dbCommand);
                returnValue = System.Convert.ToString(dbCommand.Parameters[0].Value);
                if (returnValue != "1")
                {
                    throw new System.Exception(returnValue);
                }
            }
            catch (OracleException ex)
            {
                switch (ex.Number)
                {
                    case 1921:
                        throw new Exception(Resources.Oracle1921);
                    case 911:
                        throw new Exception(Resources.OracleInvalidCharsRoleName);
                    case 1935:
                        throw new Exception(Resources.OracleInvalidRoleName);
                    case 1937:
                        //throw new Exception(Resources.Oracle1937); 
                        /*Code modified by Krishna 29 Nov 10                        
                        oracle error number 1937 describes missing or invalid rolename.
                        Missing rolename validation is doing at business layer (COERoleBO)
                        Special characters validationand length validation is doing at client side.
                        The remaining posible cases might be starts with numbers or contains the oracle key word, etc.*/
                        if (char.IsNumber(roleName.ToCharArray()[0]))
                            throw new Exception(Resources.OracleInvalidRoleName);
                        else
                            throw new Exception(Resources.OracleInvalidStringRoleName);
                    default:
                        throw new Exception(ex.Message);
                }
            }
            catch (System.Exception exsys)
            {
                throw exsys;
            }
        }

        internal override DataTable GetRoleRoles(string roleName)
        {
            DataTable dt = new DataTable();
            OracleConnection connection = null;
            OracleCommand cmd = new OracleCommand();
            OracleRefCursor refCursor = null;
            OracleDataAdapter da = new OracleDataAdapter();
            try
            {
                connection = (OracleConnection)DALManager.Database.CreateConnection();

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = Resources.SecurityDatabaseName + ".MANAGE_ROLES.GETROLEROLES";
                cmd.Connection = connection;
                OracleParameter param = new OracleParameter("PROLENAME", OracleDbType.Varchar2, roleName, ParameterDirection.Input);
                cmd.Parameters.Add(param);
                OracleParameter paramCursor = new OracleParameter("O_RS", OracleDbType.RefCursor, ParameterDirection.Output);
                cmd.Parameters.Add(paramCursor);
                connection.Open();
                cmd.ExecuteNonQuery();
                refCursor = (OracleRefCursor)cmd.Parameters[1].Value;

                da.Fill(dt, refCursor);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                da.Dispose();
                refCursor.Dispose();
                DALUtils.DestroyCommandAndConnection(cmd);
            }
            return dt;
        }

        internal override DataTable GetRoleAvailableRoles(string privilegeTableName, string roleName)
        {
            DataTable dt = new DataTable();
            OracleConnection connection = null;
            OracleCommand cmd = new OracleCommand();
            OracleRefCursor refCursor = null;
            OracleDataAdapter da = new OracleDataAdapter();
            try
            {
                connection = (OracleConnection)DALManager.Database.CreateConnection();

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = Resources.SecurityDatabaseName + ".MANAGE_ROLES.GETROLEAVAILABLEROLES";
                cmd.Connection = connection;
                OracleParameter param = new OracleParameter("pPrivilegeTableName", OracleDbType.Varchar2, privilegeTableName, ParameterDirection.Input);
                cmd.Parameters.Add(param);
                OracleParameter param2 = new OracleParameter("pRoleName", OracleDbType.Varchar2, roleName, ParameterDirection.Input);
                cmd.Parameters.Add(param2);
                OracleParameter paramCursor = new OracleParameter("O_RS", OracleDbType.RefCursor, ParameterDirection.Output);
                cmd.Parameters.Add(paramCursor);
                connection.Open();
                cmd.ExecuteNonQuery();
                refCursor = (OracleRefCursor)cmd.Parameters[2].Value;

                da.Fill(dt, refCursor);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                da.Dispose();
                refCursor.Dispose();
                DALUtils.DestroyCommandAndConnection(cmd);
            }
            return dt;
        }

        internal override DataTable GetRoleAvailableUsers(string privilegeTableName, string roleName)
        {
            DataTable dt = new DataTable();
            OracleConnection connection = null;
            OracleCommand cmd = new OracleCommand();
            OracleRefCursor refCursor = null;
            OracleDataAdapter da = new OracleDataAdapter();
            try
            {
                connection = (OracleConnection)DALManager.Database.CreateConnection();

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = Resources.SecurityDatabaseName + ".MANAGE_USERS.GETROLEAVAILABLEUSERS";
                cmd.Connection = connection;
                OracleParameter param = new OracleParameter("pPrivilegeTableName", OracleDbType.Varchar2, privilegeTableName, ParameterDirection.Input);
                cmd.Parameters.Add(param);
                OracleParameter param2 = new OracleParameter("pRoleName", OracleDbType.Varchar2, roleName, ParameterDirection.Input);
                cmd.Parameters.Add(param2);
                OracleParameter paramCursor = new OracleParameter("O_RS", OracleDbType.RefCursor, ParameterDirection.Output);
                cmd.Parameters.Add(paramCursor);
                connection.Open();
                cmd.ExecuteNonQuery();
                refCursor = (OracleRefCursor)cmd.Parameters[2].Value;

                da.Fill(dt, refCursor);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                da.Dispose();
                refCursor.Dispose();
                DALUtils.DestroyCommandAndConnection(cmd);
            }
            return dt;
        }

        internal override DataTable GetRoleGrantees(string roleName)
        {
            DataTable dt = new DataTable();
            OracleConnection connection = null;
            OracleCommand cmd = new OracleCommand();
            OracleRefCursor refCursor = null;
            OracleDataAdapter da = new OracleDataAdapter();
            try
            {
                connection = (OracleConnection)DALManager.Database.CreateConnection();

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = Resources.SecurityDatabaseName + ".MANAGE_USERS.GETROLEGRANTEES";
                cmd.Connection = connection;
                OracleParameter param2 = new OracleParameter("pRoleName", OracleDbType.Varchar2, roleName, ParameterDirection.Input);
                cmd.Parameters.Add(param2);
                OracleParameter paramCursor = new OracleParameter("O_RS", OracleDbType.RefCursor, ParameterDirection.Output);
                cmd.Parameters.Add(paramCursor);
                connection.Open();
                cmd.ExecuteNonQuery();
                refCursor = (OracleRefCursor)cmd.Parameters[1].Value;

                da.Fill(dt, refCursor);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                da.Dispose();
                refCursor.Dispose();
                DALUtils.DestroyCommandAndConnection(cmd);
            }
            return dt;
        }

        internal override DataTable GetAvailableRoles(string userName)
        {
            DataTable dt = GetAvailableRoles(userName, "privileges");
            return dt;
        }

        internal override DataTable GetAvailableRoles(string userName, string privilegeTableName)
        {
            DataTable dt = new DataTable();
            OracleConnection connection = null;
            OracleCommand cmd = new OracleCommand();
            OracleRefCursor refCursor = null;
            OracleDataAdapter da = new OracleDataAdapter();
            if (privilegeTableName == string.Empty)
            {
                privilegeTableName = "";
            }
            try
            {
                connection = (OracleConnection)DALManager.Database.CreateConnection();

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = Resources.SecurityDatabaseName + ".MANAGE_ROLES.GETCOEAVAILABLEROLES";
                cmd.Connection = connection;
                OracleParameter param2 = new OracleParameter("pUserName", OracleDbType.Varchar2, userName, ParameterDirection.Input);
                cmd.Parameters.Add(param2);
                OracleParameter param = new OracleParameter("pPrivilegeTableName", OracleDbType.Varchar2, privilegeTableName, ParameterDirection.Input);
                cmd.Parameters.Add(param);
                OracleParameter paramCursor = new OracleParameter("O_RS", OracleDbType.RefCursor, ParameterDirection.Output);
                cmd.Parameters.Add(paramCursor);
                connection.Open();
                cmd.ExecuteNonQuery();
                refCursor = (OracleRefCursor)cmd.Parameters[2].Value;

                da.Fill(dt, refCursor);
            }
            catch (Exception ex)
            {
                connection.Close();
                throw ex;
            }
            finally
            {
                da.Dispose();
                refCursor.Dispose();
                DALUtils.DestroyCommandAndConnection(cmd);
            }
            return dt;
        }

        internal override void UpdateRolesGrantedToRole(string roleName, string rolesGranted, string rolesRevoked)
        {
            int roleGrantedLength = 0;
            int roleRevokedLength = 0;
            try
            {
                if (rolesRevoked.Length == 0)
                {
                    rolesRevoked = null;
                    roleRevokedLength = 0;
                }
                else
                {
                    roleRevokedLength = rolesRevoked.Length;
                }
                if (rolesGranted.Length == 0)
                {
                    rolesGranted = null;
                    roleGrantedLength = 0;
                }
                else
                {
                    roleGrantedLength = rolesGranted.Length;
                }
                string returnValue = "0";
                DbCommand dbCommand = DALManager.Database.GetStoredProcCommand(Resources.SecurityDatabaseName + ".MANAGE_ROLES.UPDATEROLESGRANTEDTOROLE");
                dbCommand.Parameters.Add(new OracleParameter("RETURN_VALUE", OracleDbType.Varchar2, 100, ParameterDirection.ReturnValue, true, 8, 0, String.Empty, DataRowVersion.Current, returnValue));
                dbCommand.Parameters.Add(new OracleParameter("pRoleName", OracleDbType.Varchar2, roleName.Length, roleName.ToUpper(), ParameterDirection.Input));
                dbCommand.Parameters.Add(new OracleParameter("pRolesGranted", OracleDbType.Varchar2, roleGrantedLength, rolesGranted, ParameterDirection.Input));
                dbCommand.Parameters.Add(new OracleParameter("pRolesRevoked", OracleDbType.Varchar2, roleRevokedLength, rolesRevoked, ParameterDirection.Input));

                DALManager.ExecuteNonQuery(dbCommand);
                returnValue = System.Convert.ToString(dbCommand.Parameters[0].Value);
                if (returnValue != "1")
                {
                    throw new System.Exception(returnValue);
                }
            }
            catch (OracleException ex)
            {
                switch (ex.Number)
                {
                    case 1934:
                        throw new Exception(Resources.Oracle1934);
                }
                throw new Exception(ex.Message);
            }
            catch (System.Exception exsys)
            {
                throw exsys;
            }
        }

        internal override void UpdateUsersGrantedARole(string roleName, string usersGranted, string usersRevoked)
        {
            int userGrantedLength = 0;
            int userRevokedLength = 0;

            try
            {
                if (usersRevoked.Length == 0)
                {
                    usersRevoked = null;
                    userRevokedLength = 0;
                }
                else
                {
                    userRevokedLength = usersRevoked.Length;
                }
                if (usersGranted.Length == 0)
                {
                    usersGranted = null;
                    userGrantedLength = 0;
                }
                else
                {
                    userGrantedLength = usersGranted.Length;
                }
                string returnValue = "0";
                DbCommand dbCommand = DALManager.Database.GetStoredProcCommand(Resources.SecurityDatabaseName + ".MANAGE_ROLES.UPDATEUSERSGRANTEDAROLE");
                dbCommand.Parameters.Add(new OracleParameter("RETURN_VALUE", OracleDbType.Varchar2, 100, ParameterDirection.ReturnValue, true, 8, 0, String.Empty, DataRowVersion.Current, returnValue));
                dbCommand.Parameters.Add(new OracleParameter("pRoleName", OracleDbType.Varchar2, roleName.Length, roleName.ToUpper(), ParameterDirection.Input));
                dbCommand.Parameters.Add(new OracleParameter("pUsersGranted", OracleDbType.Varchar2, userGrantedLength, usersGranted, ParameterDirection.Input));
                dbCommand.Parameters.Add(new OracleParameter("pUsersRevoked", OracleDbType.Varchar2, userRevokedLength, usersRevoked, ParameterDirection.Input));
                DALManager.ExecuteNonQuery(dbCommand);
                returnValue = System.Convert.ToString(dbCommand.Parameters[0].Value);
                if (returnValue != "1")
                {
                    throw new System.Exception(returnValue);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        internal override DataTable GetAllCOERoles(string coeIdentifier)
        {
            DataTable dt = new DataTable();
            OracleConnection connection = null;
            OracleCommand cmd = new OracleCommand();
            OracleRefCursor refCursor = null;
            OracleDataAdapter da = new OracleDataAdapter();
            try
            {
                connection = (OracleConnection)DALManager.Database.CreateConnection();

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = Resources.SecurityDatabaseName + ".MANAGE_ROLES.GETCOEROLES";
                cmd.Connection = connection;
                OracleParameter param2 = new OracleParameter("pCOEIdentifier", OracleDbType.Varchar2, coeIdentifier, ParameterDirection.Input);
                cmd.Parameters.Add(param2);
                OracleParameter paramCursor = new OracleParameter("O_RS", OracleDbType.RefCursor, ParameterDirection.Output);
                cmd.Parameters.Add(paramCursor);
                connection.Open();
                cmd.ExecuteNonQuery();
                refCursor = (OracleRefCursor)cmd.Parameters[1].Value;

                da.Fill(dt, refCursor);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                da.Dispose();
                refCursor.Dispose();
                DALUtils.DestroyCommandAndConnection(cmd);
            }
            return dt;
        }

        internal override DataTable GetAllCOEUserRoles(string coeIdentifier, string userName)
        {
            DataTable dt = new DataTable();
            OracleConnection connection = null;
            OracleCommand cmd = new OracleCommand();
            OracleRefCursor refCursor = null;
            OracleDataAdapter da = new OracleDataAdapter();
            try
            {
                connection = (OracleConnection)DALManager.Database.CreateConnection();

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = Resources.SecurityDatabaseName + ".MANAGE_ROLES.GETCOEUSERROLES";
                cmd.Connection = connection;
                OracleParameter param1 = new OracleParameter("pUserName", OracleDbType.Varchar2, userName, ParameterDirection.Input);
                cmd.Parameters.Add(param1);
                OracleParameter param2 = new OracleParameter("pCOEIdentifier", OracleDbType.Varchar2, coeIdentifier, ParameterDirection.Input);
                cmd.Parameters.Add(param2);
                OracleParameter paramCursor = new OracleParameter("O_RS", OracleDbType.RefCursor, ParameterDirection.Output);
                cmd.Parameters.Add(paramCursor);
                connection.Open();
                cmd.ExecuteNonQuery();
                refCursor = (OracleRefCursor)cmd.Parameters[2].Value;
                da.Fill(dt, refCursor);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                da.Dispose();
                refCursor.Dispose();
                DALUtils.DestroyCommandAndConnection(cmd);
            }
            return dt;
        }
        #endregion

        #region password
        internal override bool ChangePassword(string userName, string oldPassword, string newPassword)
        {
            try
            {
                string returnValue = "0";

                DbCommand dbCommand = DALManager.Database.GetStoredProcCommand(Resources.SecurityDatabaseName + ".CHANGEPWD ");
                dbCommand.Parameters.Add(new OracleParameter("RETURN_VALUE", OracleDbType.Varchar2, 100, ParameterDirection.ReturnValue, true, 8, 0, String.Empty, DataRowVersion.Current, returnValue));
                dbCommand.Parameters.Add(new OracleParameter("PUSERNAME", OracleDbType.Varchar2, userName.Length, userName.ToUpper(), ParameterDirection.Input));
                dbCommand.Parameters.Add(new OracleParameter("pPassword", OracleDbType.Varchar2, oldPassword.Length, "" + oldPassword + "", ParameterDirection.Input));
                dbCommand.Parameters.Add(new OracleParameter("pNewPassword", OracleDbType.Varchar2, newPassword.Length + 2, (char)34 + newPassword + (char)34, ParameterDirection.Input));
                DALManager.ExecuteNonQuery(dbCommand);
                returnValue = System.Convert.ToString(dbCommand.Parameters[0].Value);
                if (returnValue != "1")
                {
                    throw new System.Exception(returnValue);
                }
                else
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        internal override string GetGroupHeirarchyXML(int groupOrgID)
        {

            string xml = string.Empty;
            DbCommand dbCommand = null;
            OracleClob oraClob = null;
            if (groupOrgID > 0)
            {
                dbCommand = DALManager.Database.GetStoredProcCommand("COEDB.MANAGE_GROUPS.GETGROUPHIERARCHYXML");
                dbCommand.Parameters.Add(new OracleParameter("RETURN_VALUE", OracleDbType.Clob, xml, ParameterDirection.ReturnValue));
                dbCommand.Parameters.Add(new OracleParameter("PGROUPORGID", OracleDbType.Int32, groupOrgID, ParameterDirection.Input));
            }
            else
            {
                dbCommand = DALManager.Database.GetStoredProcCommand("COEDB.MANAGE_GROUPS.GETGROUPORGHIERARCHYXML");
                dbCommand.Parameters.Add(new OracleParameter("RETURN_VALUE", OracleDbType.Clob, xml, ParameterDirection.ReturnValue));
            }
            try
            {
                dbCommand.Connection = DALManager.Database.CreateConnection();
                dbCommand.Connection.Open();
                dbCommand.ExecuteNonQuery();
                oraClob = (OracleClob)dbCommand.Parameters[0].Value;
                xml = oraClob.Value as string;
            }
            catch
            {
                throw;
            }
            finally
            {
                //destory output clob
                oraClob.Dispose();
                oraClob = null;
                DALUtils.DestroyCommandAndConnection(dbCommand);
            }
            return xml;


        }
        internal override DataTable GetGroupList(int groupOrgID, int groupID)
        {

            DataTable dt = new DataTable();

            try
            {
                string sql = "select group_id, group_name from coedb.COEGroup where GROUP_ID <> " + groupID + "  and GROUPORG_ID=" + groupOrgID + " order by upper(group_name)";
                DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sql);
                DataSet ds = DALManager.Database.ExecuteDataSet(dbCommand);
                dt = ds.Tables[0];
                dbCommand.Connection.Close();

            }
            catch (Exception ex)
            {

                throw ex;
            }

            return dt;

        }
        internal override DataTable GetPeopleList()
        {

            DataTable dt = new DataTable();

            OracleConnection connection = null;
            OracleCommand cmd = new OracleCommand();
            OracleRefCursor refCursor = null;
            OracleDataAdapter da = new OracleDataAdapter();
            try
            {
                connection = (OracleConnection)DALManager.Database.CreateConnection();

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = Resources.SecurityDatabaseName + ".MANAGE_USERS.GETCOEUSERS";
                cmd.Connection = connection;
                OracleParameter param1 = new OracleParameter("pCOEIdentifier", OracleDbType.Varchar2, "", ParameterDirection.Input);
                cmd.Parameters.Add(param1);
                OracleParameter paramCursor = new OracleParameter("O_RS", OracleDbType.RefCursor, ParameterDirection.Output);
                cmd.Parameters.Add(paramCursor);
                connection.Open();
                cmd.ExecuteNonQuery();
                refCursor = (OracleRefCursor)cmd.Parameters[1].Value;

                da.Fill(dt, refCursor);

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                da.Dispose();
                refCursor.Dispose();
                DALUtils.DestroyCommandAndConnection(cmd);
            }
            return dt;

        }
        internal override void DeleteGroup(int groupID)
        {

            try
            {
                DbCommand dbCommand = DALManager.Database.GetStoredProcCommand(Resources.SecurityDatabaseName + ".MANAGE_GROUPS.DELETEGROUP");
                dbCommand.Parameters.Add(new OracleParameter("pRoleID", OracleDbType.Int32, 0, groupID, ParameterDirection.Input));
                DALManager.ExecuteNonQuery(dbCommand);
                dbCommand.Connection.Close();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        internal override void SetGroupPrivSets(int groupID, int[] privsetList)
        {
            if (privsetList != null)
            {
                DbCommand dbCommand = null;
                try
                {
                    dbCommand = DALManager.Database.GetStoredProcCommand(Resources.SecurityDatabaseName + ".MANAGE_GROUPS.SETGROUPROLES");
                    dbCommand.Parameters.Add(new OracleParameter("pGroupID", OracleDbType.Int32, groupID, ParameterDirection.Input));
                    OracleParameter collParam = new OracleParameter("pPrivSetList", OracleDbType.Int32, privsetList, ParameterDirection.Input);
                    collParam.CollectionType = OracleCollectionType.PLSQLAssociativeArray;
                    dbCommand.Parameters.Add(collParam);
                    dbCommand.Connection = DALManager.Database.CreateConnection();
                    dbCommand.Connection.Open();
                    dbCommand.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    DALUtils.DestroyCommandAndConnection(dbCommand);
                }
            }
        }

        internal override void SetGroupUsers(int groupID, int[] userList)
        {
            if (userList != null)
            {
                DbCommand dbCommand = null;
                try
                {
                    dbCommand = DALManager.Database.GetStoredProcCommand(Resources.SecurityDatabaseName + ".MANAGE_GROUPS.SETGROUPPEOPLE");
                    dbCommand.Parameters.Add(new OracleParameter("pGroupID", OracleDbType.Int32, groupID, ParameterDirection.Input));
                    OracleParameter collParam = new OracleParameter("puserList", OracleDbType.Int32, userList, ParameterDirection.Input);
                    collParam.CollectionType = OracleCollectionType.PLSQLAssociativeArray;
                    dbCommand.Parameters.Add(collParam);
                    dbCommand.Connection = DALManager.Database.CreateConnection();
                    dbCommand.Connection.Open();
                    dbCommand.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    DALUtils.DestroyCommandAndConnection(dbCommand);
                }
            }
            else
            {
                DbCommand dbCommand = null;
                try
                {
                    dbCommand = DALManager.Database.GetStoredProcCommand(Resources.SecurityDatabaseName + ".MANAGE_GROUPS.ClearGroupPeople");
                    dbCommand.Parameters.Add(new OracleParameter("pGroupID", OracleDbType.Int32, groupID, ParameterDirection.Input));
                    dbCommand.Connection = DALManager.Database.CreateConnection();
                    dbCommand.Connection.Open();
                    dbCommand.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    DALUtils.DestroyCommandAndConnection(dbCommand);
                }
            }
        }

        internal override int AddGroup(string groupName, int groupOrgID, int parentGroupID, int leaderPersonID, int[] userList, int[] privsetList)
        {
            int groupID = -1;
            string returnValue = string.Empty;
            DbCommand dbCommand = null;
            try
            {

                dbCommand = DALManager.Database.GetStoredProcCommand(Resources.SecurityDatabaseName + ".MANAGE_GROUPS.ADDGROUP");
                dbCommand.Parameters.Add(new OracleParameter("RETURN_VALUE", OracleDbType.Varchar2, 100, ParameterDirection.ReturnValue, true, 8, 0, String.Empty, DataRowVersion.Current, returnValue));
                dbCommand.Parameters.Add(new OracleParameter("pGroupName", OracleDbType.Varchar2, groupName.Length, groupName, ParameterDirection.Input));
                dbCommand.Parameters.Add(new OracleParameter("pParent_GroupID", OracleDbType.Int32, parentGroupID, ParameterDirection.Input));
                dbCommand.Parameters.Add(new OracleParameter("pGroupOrgID", OracleDbType.Int32, groupOrgID, ParameterDirection.Input));
                dbCommand.Parameters.Add(new OracleParameter("pLeaderPersonID", OracleDbType.Int32, leaderPersonID, ParameterDirection.Input));
                DALManager.ExecuteNonQuery(dbCommand);
                returnValue = System.Convert.ToString(dbCommand.Parameters[0].Value);
                groupID = System.Convert.ToInt32(returnValue);

                if (groupID > 0)
                {
                    SetGroupPrivSets(groupID, privsetList);
                    SetGroupUsers(groupID, userList);
                }
                dbCommand.Connection.Close();
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return groupID;


        }

        internal override int AddGroup(string groupName, int groupOrgID, int parentGroupID, int leaderPersonID)
        {
            int groupID = -1;
            string returnValue = string.Empty;
            try
            {

                DbCommand dbCommand = DALManager.Database.GetStoredProcCommand(Resources.SecurityDatabaseName + ".MANAGE_GROUPS.ADDGROUP");
                dbCommand.Parameters.Add(new OracleParameter("RETURN_VALUE", OracleDbType.Varchar2, 100, ParameterDirection.ReturnValue, true, 8, 0, String.Empty, DataRowVersion.Current, returnValue));
                dbCommand.Parameters.Add(new OracleParameter("pGroupName", OracleDbType.Varchar2, groupName.Length, groupName, ParameterDirection.Input));
                dbCommand.Parameters.Add(new OracleParameter("pParent_GroupID", OracleDbType.Int32, parentGroupID, ParameterDirection.Input));
                dbCommand.Parameters.Add(new OracleParameter("pGroupOrgID", OracleDbType.Int32, groupOrgID, ParameterDirection.Input));
                dbCommand.Parameters.Add(new OracleParameter("pLeaderPersonID", OracleDbType.Int32, leaderPersonID, ParameterDirection.Input));
                DALManager.ExecuteNonQuery(dbCommand);
                returnValue = System.Convert.ToString(dbCommand.Parameters[0].Value);
                groupID = System.Convert.ToInt32(returnValue);
                dbCommand.Connection.Close();

            }
            catch (Exception ex)
            {

                throw ex;
            }
            return groupID;

        }

        internal override void UpdateGroup(int groupID, string groupName, int parentGroupID, int leaderPersonID, int[] userList, int[] privsetList)
        {
            try
            {
                DbCommand dbCommand = DALManager.Database.GetStoredProcCommand(Resources.SecurityDatabaseName + ".MANAGE_GROUPS.UPDATEGROUP");
                dbCommand.Parameters.Add(new OracleParameter("pGroupID", OracleDbType.Int32, groupID, ParameterDirection.Input));
                dbCommand.Parameters.Add(new OracleParameter("pGroupName", OracleDbType.Varchar2, groupName, ParameterDirection.Input));
                dbCommand.Parameters.Add(new OracleParameter("pParentGroupID", OracleDbType.Varchar2, parentGroupID, ParameterDirection.Input));
                dbCommand.Parameters.Add(new OracleParameter("pLeaderPersonID", OracleDbType.Varchar2, leaderPersonID, ParameterDirection.Input));
                DALManager.ExecuteNonQuery(dbCommand);
                SetGroupUsers(groupID, userList);
                SetGroupPrivSets(groupID, privsetList);
                dbCommand.Connection.Close();

            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        internal override void UpdateGroup(int groupID, string groupName, int parentGroupID, int leaderPersonID)
        {

            try
            {
                DbCommand dbCommand = DALManager.Database.GetStoredProcCommand(Resources.SecurityDatabaseName + ".MANAGE_GROUPS.UPDATEGROUP");
                dbCommand.Parameters.Add(new OracleParameter("pGroupID", OracleDbType.Int32, groupID, ParameterDirection.Input));
                dbCommand.Parameters.Add(new OracleParameter("pGroupName", OracleDbType.Varchar2, groupName, ParameterDirection.Input));
                dbCommand.Parameters.Add(new OracleParameter("pParentGroupID", OracleDbType.Varchar2, parentGroupID, ParameterDirection.Input));
                dbCommand.Parameters.Add(new OracleParameter("pLeaderPersonID", OracleDbType.Varchar2, leaderPersonID, ParameterDirection.Input));
                DALManager.ExecuteNonQuery(dbCommand);
                dbCommand.Connection.Close();

            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        internal override DataTable GetGroupUserDirectAvailPrivsets(int personID)
        {
            DataTable dt = new DataTable();
            OracleConnection connection = null;
            OracleCommand cmd = new OracleCommand();
            OracleRefCursor refCursor = null;
            OracleDataAdapter da = new OracleDataAdapter();

            try
            {
                connection = (OracleConnection)DALManager.Database.CreateConnection();

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = Resources.SecurityDatabaseName + ".MANAGE_GROUPS.GETAVAILDIRECTPERSONPRIVSETS";
                cmd.Connection = connection;
                OracleParameter param1 = new OracleParameter("pPersonId", OracleDbType.Int32, personID, ParameterDirection.Input);
                cmd.Parameters.Add(param1);
                OracleParameter paramCursor = new OracleParameter("O_RS", OracleDbType.RefCursor, ParameterDirection.Output);
                cmd.Parameters.Add(paramCursor);
                connection.Open();
                cmd.ExecuteNonQuery();
                refCursor = (OracleRefCursor)cmd.Parameters[1].Value;

                da.Fill(dt, refCursor);

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                da.Dispose();
                refCursor.Dispose();
                DALUtils.DestroyCommandAndConnection(cmd);
            }

            return dt;
        }

        internal override DataTable GetGroupUserDirectPrivsets(int personID)
        {
            DataTable dt = new DataTable();
            OracleConnection connection = (OracleConnection)DALManager.Database.CreateConnection();
            OracleCommand cmd = new OracleCommand();
            OracleRefCursor refCursor = null;
            OracleDataAdapter da = new OracleDataAdapter();
            try
            {
                connection = (OracleConnection)DALManager.Database.CreateConnection();
                cmd = new OracleCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = Resources.SecurityDatabaseName + ".MANAGE_GROUPS.GETDIRECTPERSONPRIVSETS";
                cmd.Connection = connection;
                OracleParameter param1 = new OracleParameter("pPersonId", OracleDbType.Int32, personID, ParameterDirection.Input);
                cmd.Parameters.Add(param1);
                OracleParameter paramCursor = new OracleParameter("O_RS", OracleDbType.RefCursor, ParameterDirection.Output);
                cmd.Parameters.Add(paramCursor);
                connection.Open();
                cmd.ExecuteNonQuery();
                refCursor = (OracleRefCursor)cmd.Parameters[1].Value;

                da.Fill(dt, refCursor);

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                da.Dispose();
                refCursor.Dispose();
                DALUtils.DestroyCommandAndConnection(cmd);
            }

            return dt;
        }
        internal override DataTable GetGroup(int groupID)
        {
            DataTable dt = new DataTable();
            OracleConnection connection = null;
            OracleCommand cmd = new OracleCommand();
            OracleRefCursor refCursor = null;
            OracleDataAdapter da = new OracleDataAdapter();

            try
            {
                connection = (OracleConnection)DALManager.Database.CreateConnection();

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = Resources.SecurityDatabaseName + ".MANAGE_GROUPS.GETGROUP";
                cmd.Connection = connection;
                OracleParameter param1 = new OracleParameter("pGroupId", OracleDbType.Int32, groupID, ParameterDirection.Input);
                cmd.Parameters.Add(param1);
                OracleParameter paramCursor = new OracleParameter("O_RS", OracleDbType.RefCursor, ParameterDirection.Output);
                cmd.Parameters.Add(paramCursor);
                connection.Open();
                cmd.ExecuteNonQuery();
                refCursor = (OracleRefCursor)cmd.Parameters[1].Value;

                da.Fill(dt, refCursor);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                da.Dispose();
                refCursor.Dispose();
                DALUtils.DestroyCommandAndConnection(cmd);
            }

            return dt;

        }


        internal override DataTable GetUserGroupPrivsets(int groupID, int personID)
        {
            DataTable dt = new DataTable();
            OracleConnection connection = null;
            OracleCommand cmd = new OracleCommand();
            OracleRefCursor refCursor = null;
            OracleDataAdapter da = new OracleDataAdapter();

            try
            {
                connection = (OracleConnection)DALManager.Database.CreateConnection();

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = Resources.SecurityDatabaseName + ".MANAGE_GROUPS.GETPERSONPRIVSETSBYGROUP";
                cmd.Connection = connection;
                OracleParameter param1 = new OracleParameter("pGroupId", OracleDbType.Int32, groupID, ParameterDirection.Input);
                cmd.Parameters.Add(param1);
                OracleParameter param2 = new OracleParameter("pPersonID", OracleDbType.Int32, personID, ParameterDirection.Input);
                cmd.Parameters.Add(param2);
                OracleParameter paramCursor = new OracleParameter("O_RS", OracleDbType.RefCursor, ParameterDirection.Output);
                cmd.Parameters.Add(paramCursor);
                connection.Open();
                cmd.ExecuteNonQuery();
                refCursor = (OracleRefCursor)cmd.Parameters[2].Value;

                da.Fill(dt, refCursor);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                da.Dispose();
                refCursor.Dispose();
                DALUtils.DestroyCommandAndConnection(cmd);
            }
            return dt;
        }


        internal override DataTable GetPersonNonGSRoles(int groupID, int personID)
        {

            DataTable dt = new DataTable();
            OracleConnection connection = null;
            OracleCommand cmd = new OracleCommand();
            OracleRefCursor refCursor = null;
            OracleDataAdapter da = new OracleDataAdapter();

            try
            {
                connection = (OracleConnection)DALManager.Database.CreateConnection();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = Resources.SecurityDatabaseName + ".MANAGE_GROUPS.GetPersonNonGSRoles";
                cmd.Connection = connection;
                OracleParameter param1 = new OracleParameter("pGroupId", OracleDbType.Int32, groupID, ParameterDirection.Input);
                cmd.Parameters.Add(param1);
                OracleParameter param2 = new OracleParameter("pPersonID", OracleDbType.Int32, personID, ParameterDirection.Input);
                cmd.Parameters.Add(param2);
                OracleParameter paramCursor = new OracleParameter("O_RS", OracleDbType.RefCursor, ParameterDirection.Output);
                cmd.Parameters.Add(paramCursor);
                connection.Open();
                cmd.ExecuteNonQuery();
                refCursor = (OracleRefCursor)cmd.Parameters[2].Value;

                da.Fill(dt, refCursor);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                da.Dispose();
                refCursor.Dispose();
                DALUtils.DestroyCommandAndConnection(cmd);
            }

            return dt;
        }

        internal override DataTable GetGroupPrivSets(int groupID)
        {

            DataTable dt = new DataTable();
            OracleConnection connection = null;
            OracleCommand cmd = new OracleCommand();
            OracleRefCursor refCursor = null;
            OracleDataAdapter da = new OracleDataAdapter();

            try
            {
                connection = (OracleConnection)DALManager.Database.CreateConnection();

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = Resources.SecurityDatabaseName + ".MANAGE_GROUPS.GETGROUPROLES";
                cmd.Connection = connection;
                OracleParameter param1 = new OracleParameter("pGroupId", OracleDbType.Int32, groupID, ParameterDirection.Input);
                cmd.Parameters.Add(param1);
                OracleParameter paramCursor = new OracleParameter("O_RS", OracleDbType.RefCursor, ParameterDirection.Output);
                cmd.Parameters.Add(paramCursor);
                connection.Open();
                cmd.ExecuteNonQuery();
                refCursor = (OracleRefCursor)cmd.Parameters[1].Value;

                da.Fill(dt, refCursor);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                da.Dispose();
                refCursor.Dispose();
                DALUtils.DestroyCommandAndConnection(cmd);
            }

            return dt;

        }
        internal override DataTable GetGroupAvailablePrivSets(int groupID)
        {
            DataTable dt = new DataTable();
            OracleConnection connection = null;
            OracleCommand cmd = new OracleCommand();
            OracleRefCursor refCursor = null;
            OracleDataAdapter da = new OracleDataAdapter();

            try
            {
                connection = (OracleConnection)DALManager.Database.CreateConnection();

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = Resources.SecurityDatabaseName + ".MANAGE_GROUPS.GETAVAILGROUPROLES";
                cmd.Connection = connection;
                OracleParameter param1 = new OracleParameter("pGroupId", OracleDbType.Int32, groupID, ParameterDirection.Input);
                cmd.Parameters.Add(param1);
                OracleParameter paramCursor = new OracleParameter("O_RS", OracleDbType.RefCursor, ParameterDirection.Output);
                cmd.Parameters.Add(paramCursor);

                connection.Open();
                cmd.ExecuteNonQuery();
                refCursor = (OracleRefCursor)cmd.Parameters[1].Value;

                da.Fill(dt, refCursor);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                da.Dispose();
                refCursor.Dispose();
                DALUtils.DestroyCommandAndConnection(cmd);
            }

            return dt;
        }

        internal override DataTable GetGroupUsers(int groupID)
        {
            DataTable dt = new DataTable();
            OracleConnection connection = null;
            OracleCommand cmd = new OracleCommand();
            OracleRefCursor refCursor = null;
            OracleDataAdapter da = new OracleDataAdapter();

            try
            {
                connection = (OracleConnection)DALManager.Database.CreateConnection();

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = Resources.SecurityDatabaseName + ".MANAGE_GROUPS.GETGROUPPEOPLE";
                cmd.Connection = connection;
                OracleParameter param1 = new OracleParameter("pGroupId", OracleDbType.Int32, groupID, ParameterDirection.Input);
                cmd.Parameters.Add(param1);
                OracleParameter paramCursor = new OracleParameter("O_RS", OracleDbType.RefCursor, ParameterDirection.Output);
                cmd.Parameters.Add(paramCursor);
                connection.Open();
                cmd.ExecuteNonQuery();
                refCursor = (OracleRefCursor)cmd.Parameters[1].Value;

                da.Fill(dt, refCursor);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                da.Dispose();
                refCursor.Dispose();
                DALUtils.DestroyCommandAndConnection(cmd);
            }
            return dt;
        }
        #endregion
    }
}
