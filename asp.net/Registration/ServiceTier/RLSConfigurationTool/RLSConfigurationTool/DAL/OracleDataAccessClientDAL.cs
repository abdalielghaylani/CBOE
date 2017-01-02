using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Csla.Validation;
using Oracle.DataAccess.Client;
using Oracle.DataAccess.Types;
using System.Data.Common;
using CambridgeSoft.COE.Framework.COEConfigurationService;
using System.Xml;

namespace CambridgeSoft.COE.RLSConfigurationTool
{
    class OracleDataAccessClientDAL
    {
        internal void ChangeRLS(string value, string orclInstance, string userName, string password)  
        {
            string connectionString = string.Format("Data Source = {0};User Id = {1};Password = {2};", orclInstance, userName, password);
            OracleConnection conn = null;
            OracleCommand command = null;
            try
            {
                conn = new OracleConnection(connectionString);
                command = conn.CreateCommand();
                conn.Open();
                command.CommandText = "RegDB.RegistrationRLS.ActivateRLS";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new OracleParameter("AState",value));
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                //Coverity fix - CID 19664
                if (command != null)
                    command.Dispose();
                if(conn.State == ConnectionState.Open)
                    conn.Close();
            }
        }
    }
}