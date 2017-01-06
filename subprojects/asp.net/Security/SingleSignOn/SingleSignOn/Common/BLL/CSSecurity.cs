using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Oracle.DataAccess.Client;
using Oracle.DataAccess.Types;

namespace CambridgeSoft.COE.Security.Services
{
	public class CSSecurity : ICOESSO
	{	
		
		static readonly SSOConfigurationCSSConnectionsCollection cssconnections =  SSOConfigurationCSSecurity.GetConfig().CSSConnections;

		#region ICOESSO Members

		public bool checkUserExists(string userName)
		{
            OracleConnection conn = null;
            try
            {
                //Fix to CSBR:136249.(oracle session error)
                //making pooling to true will allow to maintain connection pooling concept. which will handle connection's to Db in a optimize way
                using (conn = new OracleConnection("User Id=" + cssconnections["ValidateUserConnection"].schemaName + ";Password=" + cssconnections["ValidateUserConnection"].password + ";Pooling=true;Data Source=" + cssconnections["ValidateUserConnection"].dataSource))
                {
                    //now if it is an Oracle User get the Person ID using the connection to make sure it is a cs_seucrity user
                    using (OracleCommand dbCommand = conn.CreateCommand())
                    {
                        dbCommand.AddToStatementCache = false;
                        dbCommand.CommandType = CommandType.StoredProcedure;
                        dbCommand.CommandText = cssconnections["ValidateUserConnection"].schemaName + ".LOGIN.GETPERSONID";

                        OracleParameter paramReturnValue = new OracleParameter();
                        paramReturnValue.ParameterName = "@RETURN_VALUE";
                        paramReturnValue.OracleDbType = OracleDbType.Int32;
                        paramReturnValue.Direction = ParameterDirection.ReturnValue;

                        OracleParameter pusername = new OracleParameter();
                        pusername.ParameterName = "@PUSERNAME";
                        pusername.OracleDbType = OracleDbType.NVarchar2;
                        pusername.Direction = ParameterDirection.Input;
                        pusername.Value = userName;

                        dbCommand.Parameters.Add(paramReturnValue); // must be added first, parameter 0
                        dbCommand.Parameters.Add(pusername); // parameter 1
                        conn.Open();
                        dbCommand.ExecuteNonQuery();
                        conn.Close();
                        conn.Dispose();
                        if (dbCommand.Parameters["@RETURN_VALUE"].Value.ToString() != string.Empty && dbCommand.Parameters["@RETURN_VALUE"].Value.ToString() != "0") //This check was wrong, when user doesn't exits it is returning '0' so modified the condition to check '0' as well
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
            catch
            {
                return false;
            }
            //Fix to CSBR:136249.(oracle session error)
            //In the finally block the connection will be closed, when a exception is occured.
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }

		}

		public bool ValidateUser(string userName, string password)
		{
            OracleConnection conn = null;
            try
            {
                //Fix to CSBR:136249.(oracle session error)
                //making pooling to true will allow to maintain connection pooling concept. which will handle connection's to Db in a optimize way
                using( conn = new OracleConnection("User Id=" + userName + ";Password=" + password + ";Pooling=false;Data Source=" + cssconnections["ValidateUserConnection"].dataSource))
                {
                    //now if it is an Oracle User get the Person ID using the connection to make sure it is a cs_seucrity user
                    using(OracleCommand dbCommand = conn.CreateCommand())
                    {
                        dbCommand.AddToStatementCache = false;
                        dbCommand.CommandType = CommandType.StoredProcedure;
                        dbCommand.CommandText = cssconnections["ValidateUserConnection"].schemaName + ".LOGIN.GETPERSONID";

                        OracleParameter paramReturnValue = new OracleParameter();
                        paramReturnValue.ParameterName = "@RETURN_VALUE";
                        paramReturnValue.OracleDbType = OracleDbType.Int32;
                        paramReturnValue.Direction = ParameterDirection.ReturnValue;

                        OracleParameter pusername = new OracleParameter();
                        pusername.ParameterName = "@PUSERNAME";
                        pusername.OracleDbType = OracleDbType.NVarchar2;
                        pusername.Direction = ParameterDirection.Input;
                        pusername.Value = userName;

                        dbCommand.Parameters.Add(paramReturnValue); // must be added first, parameter 0
                        dbCommand.Parameters.Add(pusername); // parameter 1
                        conn.Open();
                        dbCommand.ExecuteNonQuery();
                        conn.Close();
                        conn.Dispose();
                        if(dbCommand.Parameters["@RETURN_VALUE"].Value.ToString() != string.Empty)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                if (ex.Message.Contains("ORA-1017")) //Invalid username or password
                {
                    return false;
                }
                else if (ex.Message.Contains("ORA-28000")) //Locked account
                {
                    throw new Exception("The user account is locked, please contact your system administrator");
                }
                else
                    throw new Exception(ex.Message);
            }
            //Fix to CSBR:136249.(oracle session error)
            //In the finally block the connection will be closed, when a exception is occured.
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
		}

		public System.Xml.XmlDocument GetUserInfo(string userName)
		{
			throw new Exception("The method or operation is not implemented.");
		}

        public string GetCSSecurityPassword(string userName, string password)
        {
            return password;
        }

		#endregion
	}
}
