using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using System.Data;
using Oracle.DataAccess.Client;
using Oracle.DataAccess.Types;
using CambridgeSoft.COE.Framework.Properties;
using Csla.Data;

namespace CambridgeSoft.COE.Framework.COEPageControlSettingsService
{
    public class OracleDataAccessClientDAL : DAL
    {
        #region Data Access Methods

        /// <summary>
        /// Get a xml from DB by given application name and type.
        /// </summary>
        /// <param name="appName">application name</param>
        /// <param name="type">Custom/Master/Privileges</param>
        /// <returns></returns>
        internal override string GetConfigurationXML(string appName, COEPageControlSettings.Type type)
        {
            string xml = string.Empty;
            string sql = "SELECT CONFIGURATIONXML FROM " + Resources.SecurityDatabaseName + ".COEPAGECONTROL " +
                         "WHERE UPPER(APPLICATION) = '" + appName.ToUpper() +
                         "' AND UPPER(TYPE) = '" + type.ToString().ToUpper() + "'";
            DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sql);
            try
            {
                xml = Convert.ToString(DALManager.ExecuteScalar(dbCommand));
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return xml;
        }

        /// <summary>
        /// Get a xml from DB by given application name and type.
        /// </summary>
        /// <param name="appName">application name</param>
        /// <param name="type">Custom/Master/Privileges</param>
        /// <returns></returns>
        internal override string GetConfigurationXMLFromPrivsPage(string appName, COEPageControlSettings.Type type)
        {
            StringBuilder builder = new StringBuilder("<?xml version='1.0' encoding='utf-8'?><Privileges>");
            string tableNames = string.Empty;
            string sql = "SELECT CONFIGURATIONXML FROM COEPAGECONTROL " +
                         "WHERE UPPER(APPLICATION) = " + DALManager.BuildSqlStringParameterName("pAppName") +
                         " AND UPPER(TYPE) = " + DALManager.BuildSqlStringParameterName("pType");
            DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sql);
            dbCommand.Parameters.Add(new OracleParameter(DALManager.BuildSqlStringParameterName("pAppName"), OracleDbType.Varchar2, appName.ToUpper(), ParameterDirection.Input));
            dbCommand.Parameters.Add(new OracleParameter(DALManager.BuildSqlStringParameterName("pType"), OracleDbType.Varchar2, type.ToString().ToUpper(), ParameterDirection.Input));
            try
            {
                tableNames = Convert.ToString(DALManager.ExecuteScalar(dbCommand));
                //Now we have the table name, we need to get the priviliges from the columns in the given table.
                if (!string.IsNullOrEmpty(tableNames))
                {
                    builder.Append("<ID>FullListPrivilges_" + appName.ToUpper() + "</ID>");
                    //Can be more that one, e.g. Manager = Security + DVMgr
                    List<string> privileges = this.GetPrivilegesNames(tableNames.Split("|".ToCharArray()));
                    foreach (string privName in privileges)
                        builder.Append(string.Format("<Privilege><ID>{0}</ID><Description></Description><FriendlyName></FriendlyName></Privilege>", privName));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            builder.Append("</Privileges>");
            return builder.ToString();
        }

        /// <summary>
        /// Returns a list of privileges for the given TableName minus the PK column
        /// </summary>
        /// <param name="tables">List of tables to check columns</param>
        /// <returns></returns>
        private List<string> GetPrivilegesNames(string[] tables)
        {
            List<string> retVal = new List<string>(); //To store the table names in case two tables have some privileges names in common (we don't want to add them twice)
            foreach (string table in tables)
            {
                string sql = "SELECT COLUMN_NAME PRIVILEGE FROM USER_TAB_COLUMNS WHERE TABLE_NAME = '" + table.ToUpper() + "'" +
                                " MINUS " +
                                "SELECT AIC.COLUMN_NAME FROM ALL_IND_COLUMNS AIC LEFT JOIN ALL_CONSTRAINTS ALC " +
                                "ON AIC.INDEX_NAME = ALC.CONSTRAINT_NAME AND AIC.TABLE_NAME = ALC.TABLE_NAME " +
                                "AND AIC.TABLE_OWNER = ALC.OWNER WHERE AIC.TABLE_NAME = '" + table.ToUpper() + "' ORDER BY 1";

                DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sql);
                IDataReader reader = DALManager.ExecuteReader(dbCommand);
                while (reader.Read())
                {
                    string privName = reader.GetString(0);
                    if (!retVal.Contains(privName)) //Check the privileges doesn't already exist.
                        retVal.Add(privName);
                }
                reader.Dispose();
                dbCommand.Dispose();
            }
            return retVal;
        }


        /// <summary>
        /// Update the custom xml by given application name.
        /// </summary>
        /// <param name="appName">application name</param>
        /// <param name="type">Custom/Master/Privileges</param>
        /// <param name="xml">Custom.xml</param>
        internal override void UpdateConfigurationXML(string appName, COEPageControlSettings.Type type, string xml)
        {
            string sql = "UPDATE " + Resources.SecurityDatabaseName + ".COEPAGECONTROL " +
                         "SET CONFIGURATIONXML = " + DALManager.BuildSqlStringParameterName("pXml") +
                         " WHERE UPPER(APPLICATION) = '" + appName.ToUpper() +
                         "' AND TYPE = '" + type.ToString().ToUpper() + "'";
            DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sql);
            DALManager.Database.AddParameter(dbCommand, "pXml", DbType.AnsiString, Int32.MaxValue, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, xml);
            int recordsAffected = -1;
            try
            {
                recordsAffected = Convert.ToInt32(DALManager.ExecuteNonQuery(dbCommand));
                if (recordsAffected == 0)
                    InsertConfigurationXML(appName, type, xml);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Insert the custom xml into DB.
        /// </summary>
        /// <param name="appName"></param>
        /// <param name="type"></param>
        /// <param name="xml"></param>
        private void InsertConfigurationXML(string appName, COEPageControlSettings.Type type, string xml)
        {
            string sql = "INSERT INTO " + Resources.SecurityDatabaseName + ".COEPAGECONTROL (APPLICATION,TYPE,CONFIGURATIONXML)" +
                         "VALUES ('" + appName.ToUpper() + "','" + type.ToString().ToUpper() + "'," +
                         DALManager.BuildSqlStringParameterName("pXml") + ")";
            DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sql);
            DALManager.Database.AddParameter(dbCommand, "pXml", DbType.AnsiString, Int32.MaxValue, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, xml);
            DALManager.ExecuteNonQuery(dbCommand);
        }

        #endregion
    }
}
