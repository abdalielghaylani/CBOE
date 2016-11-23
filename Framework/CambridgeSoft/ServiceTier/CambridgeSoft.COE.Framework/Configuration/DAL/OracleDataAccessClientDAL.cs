using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using CambridgeSoft.COE.Framework.Common.SqlGenerator.NonQueries;
using CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries;
using System.Data.Common;
using CambridgeSoft.COE.Framework.COELoggingService;
using Oracle.DataAccess.Client;
using CambridgeSoft.COE.Framework.Properties;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.Caching;


namespace CambridgeSoft.COE.Framework.COEConfigurationService
{
    public class OracleDataAccessClientDAL : CambridgeSoft.COE.Framework.COEConfigurationService.DAL
    {
        [NonSerialized]
        static COELog _coeLog = COELog.GetSingleton("COEConfiguration");

        public override string GetSection(string description, ref string handlerClassName)
        {
            string result = null;
            DbCommand dbCommand = DALManager.Database.GetStoredProcCommand(Resources.SecurityDatabaseName + ".ConfigurationManager.RetrieveConfiguration");

            OracleParameter descriptionParam = new OracleParameter("ADESCRIPTION", OracleDbType.Varchar2, description, ParameterDirection.Input);
            OracleParameter handlerClassNameParam = new OracleParameter("ACLASSNAME", OracleDbType.Clob, ParameterDirection.Output);
            OracleParameter sectionParam = new OracleParameter("ACONFIGURATION", OracleDbType.Clob, ParameterDirection.Output);

            dbCommand.Parameters.Add(descriptionParam);
            dbCommand.Parameters.Add(handlerClassNameParam);
            dbCommand.Parameters.Add(sectionParam);

            try
            {
                //Not using DALManager.ExecuteNonQuery(dbCommand); because we need the connection opened.
                dbCommand.Connection = DALManager.Database.CreateConnection();
                dbCommand.Connection.Open();
                dbCommand.ExecuteNonQuery();

                handlerClassName = ((Oracle.DataAccess.Types.OracleClob)(dbCommand.Parameters["ACLASSNAME"].Value)).Value;
                result = ((Oracle.DataAccess.Types.OracleClob)(dbCommand.Parameters["ACONFIGURATION"].Value)).Value;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("no data found")||ex.Message.Contains("ORA-01403"))  // In DB have different language set that will not return no data found so add oracle error code
                {
                    handlerClassName = typeof(CambridgeSoft.COE.Framework.Common.ApplicationDataConfigurationSection).AssemblyQualifiedName;
                }
                else
                    throw;
            }

            finally
            {
                //destroy the parameters since the utility command does not.
                handlerClassNameParam.Dispose();
                handlerClassNameParam = null;
                sectionParam.Dispose();
                sectionParam = null;

                dbCommand.Connection.Close();
                
            }
            return result;
        }

        //PROCEDURE InsertConfiguration(ADescription IN VARCHAR2, AClassName IN VARCHAR2, AConfiguration IN CLOB) IS

        public override void AddSection(string description, string handlerClassName, string sectionXml)
        {
            DbCommand dbCommand = DALManager.Database.GetStoredProcCommand(Resources.SecurityDatabaseName + ".ConfigurationManager.InsertConfiguration");

            OracleParameter descriptionParam = new OracleParameter("ADESCRIPTION", OracleDbType.Varchar2, description, ParameterDirection.Input);
            OracleParameter handlerClassNameParam = new OracleParameter("ACLASSNAME", OracleDbType.Varchar2, handlerClassName, ParameterDirection.Input);
            OracleParameter sectionParam = new OracleParameter("ACONFIGURATION", OracleDbType.Clob, sectionXml, ParameterDirection.Input);

            dbCommand.Parameters.Add(descriptionParam);
            dbCommand.Parameters.Add(handlerClassNameParam);
            dbCommand.Parameters.Add(sectionParam);
            try
            {
                //Not using DALManager.ExecuteNonQuery(dbCommand); because we need the connection opened.
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
                //destroy the parameters since the utility command does not.
                handlerClassNameParam.Dispose();
                handlerClassNameParam = null;
                sectionParam.Dispose();
                sectionParam = null;
                DALUtils.DestroyCommandAndConnection(dbCommand);
            }
        }

        //PROCEDURE DeleteConfiguration(ADescription IN VARCHAR2);
        public override void RemoveSection(string sectionName)
        {
            DbCommand dbCommand = DALManager.Database.GetStoredProcCommand(Resources.SecurityDatabaseName + ".ConfigurationManager.DeleteConfiguration");

            OracleParameter descriptionParam = new OracleParameter("ADESCRIPTION", OracleDbType.Varchar2, sectionName, ParameterDirection.Input);
            /*OracleParameter handlerClassNameParam = new OracleParameter("ACLASSNAME", OracleDbType.Varchar2, handlerClassName, ParameterDirection.Input);
            OracleParameter sectionParam = new OracleParameter("ACONFIGURATION", OracleDbType.Clob, sectionXml, ParameterDirection.Input);*/

            dbCommand.Parameters.Add(descriptionParam);

            try{
                //Not using DALManager.ExecuteNonQuery(dbCommand); because we need the connection opened.
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

        internal override COECacheDependency GetDependency(string applicationName)
        {
            string sql = "select rowid from " + Resources.SecurityDatabaseName + ".COECONFIGURATION where upper(DESCRIPTION) = " + DALManager.BuildSqlStringParameterName("appName");
            OracleConnection conn = DALManager.Database.CreateConnection() as OracleConnection;

            OracleCommand cmd = new OracleCommand(sql, conn);
            cmd.Parameters.Add(new OracleParameter(DALManager.BuildSqlStringParameterName("appName"), OracleDbType.Varchar2, applicationName, ParameterDirection.Input));
            return new OracleCacheDependency(cmd);

        }

        internal override string GetConfigurationSettingsXml(string appName)
        {
            string sql = "select CONFIGURATIONXML from " + Resources.SecurityDatabaseName + ".COECONFIGURATION where upper(DESCRIPTION) = '" + appName.ToUpper() + "'";
            DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sql);
            return (string)DALManager.ExecuteScalar(dbCommand);
        }
    }
}
