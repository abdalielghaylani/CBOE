using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using System.Data;
using Oracle.DataAccess.Client;
using Oracle.DataAccess.Types;
using CambridgeSoft.COE.Framework.Properties;
using Csla.Data;
using CambridgeSoft.COE.Framework.COELoggingService;
using CambridgeSoft.COE.Framework.Common;



namespace CambridgeSoft.COE.Framework.COEPickListPickerService {
    /// <summary>
    /// Class for accessing Oracle's PickList and PickListDomain tables in order to serve to COEPickListPickerService.
    /// </summary>
    public class OracleDataAccessClientDAL : DAL {

        [NonSerialized]
         static COELog _coeLog = COELog.GetSingleton("COEPickListPicker");

        internal override Csla.Data.SafeDataReader GetPickListNameValueList(int domainId) {
            /* sql for retrieving the picklist of the given domainId */
            string sql = "SELECT ID, PICKLISTVALUE FROM " + Resources.RegistrationDatabaseName + ".PICKLIST WHERE PICKLISTDOMAINID=" + DALManager.BuildSqlStringParameterName("domainId");

            DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sql);
            DALManager.Database.AddInParameter(dbCommand, DALManager.BuildSqlStringParameterName("domainId"), DbType.Int32, domainId);
            SafeDataReader safeReader = new SafeDataReader(DALManager.ExecuteReader(dbCommand));
            return safeReader;
        }

        /// <summary>
        /// Gets the list of all picklist domains.
        /// </summary>
        /// <returns>List of picklist domains</returns>
        internal override Csla.Data.SafeDataReader GetAllPickListDomains()
        {
            /* sql for retrieving the picklist of the given domainId */
            string sql = "SELECT ID,DESCRIPTION FROM " + Resources.RegistrationDatabaseName + ".PICKLISTDOMAIN";
            DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sql);
            SafeDataReader safeReader = new SafeDataReader(DALManager.ExecuteReader(dbCommand));
            return safeReader;
        }

        internal override int GetDomainPickListId(string domainName) {
            int returnValue = 1;
            try {
                string sql = "SELECT ID FROM " + Resources.RegistrationDatabaseName + ".PICKLISTDOMAIN WHERE DESCRIPTION=" + DALManager.BuildSqlStringParameterName("domainName");

                DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sql);
                DALManager.Database.AddParameter(dbCommand, DALManager.BuildSqlStringParameterName("domainName"), DbType.AnsiString, 255, ParameterDirection.Input, false, 0, 0, string.Empty, DataRowVersion.Current, domainName);
                returnValue = Convert.ToInt32(DALManager.ExecuteScalar(dbCommand));
            } catch(Exception ex) {
                throw;
            }

            return returnValue;
        }

        internal override SafeDataReader GetPickListNameValueList(string vSql)
        {
            DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(vSql);
            SafeDataReader safeReader = new SafeDataReader(DALManager.ExecuteReader(dbCommand));
            return safeReader;
        }

        internal override SafeDataReader GetPicklistDomain(int vId)
        {
            string sql = @"Select * FROM " + Resources.RegistrationDatabaseName + ".PICKLISTDOMAIN WHERE ID = " + DALManager.BuildSqlStringParameterName("id");
            DbCommand dbCommand = null;
            SafeDataReader safeReader = null;

            try
            {
                dbCommand = DALManager.Database.GetSqlStringCommand(sql);
                DALManager.Database.AddInParameter(dbCommand, "id", DbType.Int32, vId);
                safeReader = new SafeDataReader(DALManager.ExecuteReader(dbCommand));
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return safeReader;
        }

        internal override bool ValidateSqlColumn(string table, string column)
        {
            try
            {
                int value = -1;
                string schema = string.Empty;
                string sqlUser = string.Empty;
                if (table.IndexOf(".") != -1)
                {
                    schema = table.Substring(0, table.IndexOf("."));
                    table = table.Replace(schema + ".", string.Empty);
                }

                if (DALManager.DatabaseData.OriginalOwner.ToUpper() == Resources.RegistrationDatabaseName.ToUpper())
                    sqlUser = "user_tab_columns";
                else
                    sqlUser = "DBA_tab_columns";

                string sql = @"select COUNT(ROWNUM)  from " + sqlUser + " where TABLE_NAME = '" + table.ToUpper() + "' and COLUMN_NAME = '" + column.ToUpper() + "'";
                DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sql);
                value = Convert.ToInt32(DALManager.ExecuteScalar(dbCommand));
                return (value > 0);
            }
            catch (Exception ex)
            {
                throw;
            }

        }
        
    }
}
