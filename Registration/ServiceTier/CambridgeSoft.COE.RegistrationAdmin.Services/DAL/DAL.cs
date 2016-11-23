using System;
using CambridgeSoft.COE.Framework.Common;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Globalization;
using Oracle.DataAccess.Client;
using System.Collections.Generic;
using Csla.Data;

namespace CambridgeSoft.COE.RegistrationAdmin.Services
{
    public class DAL : DALBase
    {
        internal virtual string InsertConfigurationRegistryRecord(string xml)
        {
            return string.Empty;
        }

        internal virtual void UpdateConfigurationRegistryRecord(string xml)
        {
            return;
        }

        internal virtual void BeginTransaction()
        {
            return;
        }

        internal void CommitTransaction()
        {
            return;
        }

        internal void RollbackTransaction()
        {
            return;
        }
        internal virtual string GetConfigurationRegistryRecord()
        {
            return string.Empty;
        }

        internal DataSet GetTableList(List<string> tableNameList)
        {
            string sql = string.Empty;
            DataSet dsTables = new DataSet();
            DataSet tempDs = new DataSet();
            foreach (string tableName in tableNameList)
            {
                tempDs.Clear();
                sql = "SELECT * FROM " + tableName;
                DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sql);
                tempDs = DALManager.ExecuteDataSet(dbCommand);
                tempDs.Tables[0].TableName = tableName;
                dsTables.Merge(tempDs);
            }

            dsTables.DataSetName = "tableList";
            return dsTables;
        }

        internal DataSet GetTable(string tableName)
        {
            string sql = string.Empty;
            DataSet dsTable = new DataSet();
            sql = "SELECT * FROM " + tableName;
            DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sql);
            dsTable = DALManager.ExecuteDataSet(dbCommand);
            dsTable.Tables[0].TableName = tableName;
            dsTable.DataSetName = tableName;
            return dsTable;
        }

        internal virtual void ChangeRLS(string value)
        {

        }
        
        internal virtual void ImportTableList(string xmlTableList)
        {
            return;
        }


        /// <summary>
        /// Gets the Database reserved words
        /// </summary>
        /// <returns></returns>
        internal SafeDataReader GetDatabaseReservedWords()
        {
            string sql = string.Empty;                     
            sql = "SELECT KeyWord FROM REGDB.VW_ReseredWords";
            DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sql);
            SafeDataReader sdReservedWords = new SafeDataReader(DALManager.ExecuteReader(dbCommand));
            return sdReservedWords;
        }

    }
}
