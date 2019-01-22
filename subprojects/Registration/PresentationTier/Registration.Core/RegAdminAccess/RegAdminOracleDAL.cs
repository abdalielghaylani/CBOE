using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Xml;

using Csla.Data;
using Csla.Validation;

using Oracle.DataAccess.Client;
using Oracle.DataAccess.Types;

using CambridgeSoft.COE.Framework.COEConfigurationService;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.ExceptionHandling;
using CambridgeSoft.COE.Registration.Core.Properties;
//using CambridgeSoft.COE.RegistrationAdmin.Services.Properties;

namespace CambridgeSoft.COE.RegistrationAdmin.Access
{
    public class RegAdminOracleDAL : DALBase
    {
        public string InsertConfigurationRegistryRecord(string xml)
        {
            string errorLog = string.Empty;
            DbCommand dbCommand = DALManager.Database.GetStoredProcCommand("REGDB.ConfigurationCompoundRegistry.SetCoeObjectConfig");
            dbCommand.Parameters.Add(new OracleParameter("ACoeObjectConfigXML", OracleDbType.Clob, xml, ParameterDirection.Input));
            dbCommand.Parameters.Add(new OracleParameter("AErrorsLog", OracleDbType.Clob, errorLog, ParameterDirection.Output));
            BeginTransaction();
            DALManager.ExecuteNonQuery(dbCommand);
            errorLog = ((OracleClob)dbCommand.Parameters["AErrorsLog"].Value).Value as string;
            CommitTransaction();
            return errorLog;
        }
        internal void BeginTransaction()
        {
            if (DALManager.DbTransaction == null)
            {
                DALManager.DbConnection = DALManager.Database.CreateConnection();
                DALManager.DbConnection.Open();
                DALManager.DbTransaction = DALManager.DbConnection.BeginTransaction();
            }
        }
        internal new void CommitTransaction()
        {
            if (DALManager.DbTransaction != null)
            {
                DALManager.DbTransaction.Commit();
                DALManager.DbTransaction = null;
                DALManager.DbConnection.Close();
                DALManager.DbConnection = null;
            }
        }
        public string GetConfigurationRegistryRecord()
        {
            string xml = string.Empty;
            DbCommand dbCommand = DALManager.Database.GetStoredProcCommand("REGDB.ConfigurationCompoundRegistry.GetCoeObjectConfig");
            dbCommand.Parameters.Add(new OracleParameter("AXml", OracleDbType.Clob, xml, ParameterDirection.Output));
            dbCommand.Connection = DALManager.Database.CreateConnection();
            dbCommand.Connection.Open();
            dbCommand.ExecuteNonQuery();
            xml = ((OracleClob)dbCommand.Parameters["AXml"].Value).Value as string;
            dbCommand.Connection.Close();
            return xml;
        }
        public void ImportTableList(string xmlTableList)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlTableList);
            StringBuilder sqlBuilder = new StringBuilder();
            OracleCommand command = null;
            //StringBuilder errorLog = new StringBuilder();
            List<OracleParameter> paramList = new List<OracleParameter>();
            string[] triggers = { "TRG_NOTEBOOKS", "TRG_PROJECTS", "TRG_SEQUENCE" };
            string sqlTriggers = string.Empty;

            foreach (string trg in triggers)
            {
                sqlTriggers = "ALTER TRIGGER " + trg + " DISABLE";
                command = (OracleCommand)DALManager.Database.GetSqlStringCommand(sqlTriggers);
                DALManager.Database.ExecuteNonQuery(command);
            }

            foreach (XmlNode table in xmlDoc.FirstChild.ChildNodes)
            {
                foreach (XmlNode column in table.ChildNodes)
                {

                    string sqlGetType = "SELECT DATA_TYPE, DATA_PRECISION FROM user_tab_columns WHERE TABLE_NAME = '" + table.Name + "' AND COLUMN_NAME = '" + column.Name + "'";

                    command = (OracleCommand)DALManager.Database.GetSqlStringCommand(sqlGetType);

                    DataSet columnInfo = DALManager.Database.ExecuteDataSet(command);

                    string columnType = columnInfo.Tables[0].Rows[0].ItemArray[0].ToString();

                    string columnPrecision = columnInfo.Tables[0].Rows[0].ItemArray[1].ToString();

                    OracleDbType oracleDbType = this.GetOracleDbType(columnType, columnPrecision);

                    switch (oracleDbType)
                    {
                        case OracleDbType.Date:
                            DateTime date = DateTime.Parse(column.InnerText);
                            paramList.Add(new OracleParameter("p" + column.Name, oracleDbType, date, ParameterDirection.Input));
                            break;
                        default:
                            paramList.Add(new OracleParameter("p" + column.Name, oracleDbType, column.InnerText, ParameterDirection.Input));
                            break;
                    }
                }

                sqlBuilder.Append("INSERT INTO " + table.Name + " (");

                foreach (XmlNode column in table)
                    sqlBuilder.Append(column.Name + ", ");


                sqlBuilder.Remove(sqlBuilder.Length - 2, 2);
                sqlBuilder.Append(") values (");

                foreach (XmlNode column in table)
                    sqlBuilder.Append(DALManager.BuildSqlStringParameterName("p" + column.Name) + ", ");

                sqlBuilder.Remove(sqlBuilder.Length - 2, 2);
                sqlBuilder.Append(")");

                command = (OracleCommand)DALManager.Database.GetSqlStringCommand(sqlBuilder.ToString());

                foreach (OracleParameter param in paramList)
                    command.Parameters.Add(param.Clone());

                //sqlBuilder.Remove(0, sqlBuilder.Length);


                try
                {
                    DALManager.Database.ExecuteNonQuery(command);
                }
                catch (Exception ex)
                {
                    //errorLog.Append(e.Message + errorLog.ToString());   
                }

                paramList.Clear();

                sqlBuilder = null;

                sqlBuilder = new StringBuilder();
            }

            foreach (string trg in triggers)
            {
                sqlTriggers = "ALTER TRIGGER " + trg + " ENABLE";
                command = (OracleCommand)DALManager.Database.GetSqlStringCommand(sqlTriggers);
                DALManager.Database.ExecuteNonQuery(command);
            }


        }

        private OracleDbType GetOracleDbType(string type, string precision)
        {
            OracleDbType oracleDbType = OracleDbType.Varchar2;

            switch (type)
            {
                case "NUMBER":
                    if (precision != string.Empty)
                    {
                        if (int.Parse(precision) <= 9)
                            oracleDbType = OracleDbType.Int32;
                        else
                            oracleDbType = OracleDbType.Int64;
                    }
                    else
                    {
                        oracleDbType = OracleDbType.Double;
                    }
                    break;
                default:
                    oracleDbType = (OracleDbType)Enum.Parse(typeof(OracleDbType), type, true);
                    break;
            }

            return oracleDbType;
        }

        public void ChangeRLS(string value)
        {
            DbCommand dbCommand = DALManager.Database.GetStoredProcCommand("RegDB.RegistrationRLS.ActivateRLS");
            dbCommand.Parameters.Add(new OracleParameter("AState", OracleDbType.Varchar2, value, ParameterDirection.Input));
            DALManager.ExecuteNonQuery(dbCommand);
        }

        public DataSet GetTableList(List<string> tableNameList)
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

        public DataSet GetTable(string tableName)
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

        /// <summary>
        /// Gets the columns in a given table.
        /// </summary>
        /// <returns>Rows of columnnames in a SafeDataReader</returns>
        public SafeDataReader GetColumns(string tableName)
        {
            string sql = string.Empty;
            string paramName = "tableName";
            SafeDataReader safeReader = null;
            DbCommand dbCommand = null;

            sql = "SELECT upper(column_name) as COLUMNNAME FROM sys.all_Tab_Columns"
                + " WHERE Table_Name = "  + DALManager.BuildSqlStringParameterName(paramName);

            try
            {
                dbCommand = DALManager.Database.GetSqlStringCommand(sql);
                DALManager.Database.AddInParameter(dbCommand, paramName, DbType.String, tableName.ToUpper());

                safeReader = new SafeDataReader(DALManager.ExecuteReader(dbCommand));
            }
            catch (Exception ex)
            {
                ExceptionContext currentContext = new ExceptionContext();
                currentContext.Command = dbCommand;
                COEExceptionDispatcher.HandleDALException(ex, currentContext);
            }
            return safeReader;
        }


        /// <summary>
        /// Gets the list of tables, columns which needs to be updated by a default value.
        /// </summary>
        /// <param name="propertyListType">Property type Ex: Batch, Mixture, Compound</param>
        /// <param name="PropertyName">Property name or column name to which user is trying to add validations</param>
        /// <returns>A xml format log contains details of property or column with its table and datatype </returns>
        public string GetPropertyStatusInDB(string propertyListType, string PropertyName, string ValidationType)
        {
            string errorLog = string.Empty;
            DbCommand dbCommand = DALManager.Database.GetStoredProcCommand("REGDB.ConfigurationCompoundRegistry.ValidateTables");
            try
            {
                dbCommand.Parameters.Add(new OracleParameter("AErrorsLog", OracleDbType.Clob, errorLog, ParameterDirection.Output));

                dbCommand.Parameters.Add(new OracleParameter("APropertyListType", OracleDbType.Varchar2, propertyListType, ParameterDirection.Input));
                dbCommand.Parameters.Add(new OracleParameter("AField", OracleDbType.Varchar2, PropertyName, ParameterDirection.Input));
                dbCommand.Parameters.Add(new OracleParameter("AValidationType", OracleDbType.Varchar2, ValidationType, ParameterDirection.Input));
                BeginTransaction();
                DALManager.ExecuteNonQuery(dbCommand);
                errorLog = ((OracleClob)dbCommand.Parameters["AErrorsLog"].Value).Value as string;
                CommitTransaction();
            }
            catch (Exception ex)
            {
                ExceptionContext currentContext = new ExceptionContext();
                currentContext.Command = dbCommand;
                COEExceptionDispatcher.HandleDALException(ex, currentContext);
            }
            return errorLog;
        }

        /// <summary>
        /// Update the default value for columns in DB using error log which contains table, column, value information.
        /// </summary>
        /// <param name="statusLog">Error log used in procedure</param>
        /// <returns>True string if updated succesfully, False if fails to update</returns>
        public bool UpdatePropertyStatusInDB(string statusLog)
        {
            string errorLog = string.Empty;
            bool retVal = false;
            DbCommand dbCommand = DALManager.Database.GetStoredProcCommand("REGDB.ConfigurationCompoundRegistry.UpdateTables");
            try
            {
                dbCommand.Parameters.Add(new OracleParameter("AUpdateLogXML", OracleDbType.Clob, statusLog, ParameterDirection.Input));
                dbCommand.Parameters.Add(new OracleParameter("AErrorsLog", OracleDbType.Varchar2, 200, errorLog, ParameterDirection.Output));
                BeginTransaction();
                DALManager.ExecuteNonQuery(dbCommand);
                errorLog = dbCommand.Parameters["AErrorsLog"].Value as string;
                CommitTransaction();
                retVal = true;
            }
            catch (Exception ex)
            {
                retVal = false;
                ExceptionContext currentContext = new ExceptionContext();
                currentContext.Command = dbCommand;
                COEExceptionDispatcher.HandleDALException(ex, currentContext);
            }
            return retVal;
        }

        /// <summary>
        /// Check the picklistDomain SQL Query .
        /// </summary>
        /// <returns>Boolean Is Valid SQL Statement</returns>
        public DataSet ExecuteSqlQuery(string sql)
        {
            DataSet dsTable = new DataSet();
            DbCommand dbCommand = null;
            try
            {
                dbCommand = DALManager.Database.GetSqlStringCommand(sql);
                dsTable = DALManager.ExecuteDataSet(dbCommand);
                dsTable.Tables[0].TableName = "PickList";
                dsTable.DataSetName = "PickList";
                DALManager.ExecuteNonQuery(dbCommand);
            }
            catch (Exception ex)
            {
                ExceptionContext currentContext = new ExceptionContext();
                currentContext.Command = dbCommand;
                COEExceptionDispatcher.HandleDALException(ex, currentContext);
            }
            return dsTable;
        }
       
        /// <summary>
        /// Gets the Database reserved words
        /// </summary>
        /// <returns></returns>
        public SafeDataReader GetDatabaseReservedWords()
        {
            string sql = string.Empty;
            sql = "SELECT KeyWord FROM REGDB.VW_ReseredWords";
            DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sql);
            SafeDataReader sdReservedWords = new SafeDataReader(DALManager.ExecuteReader(dbCommand));
            return sdReservedWords;
        }

        public SafeDataReader GetRegNumberSequencer(int sequenceId)
        {
            string paramName = "sequenceID";
            string sql = Resources.GetRegNumberSequencerSql
                + " WHERE s.sequence_id = " + DALManager.BuildSqlStringParameterName(paramName);
            DbCommand dbCommand = null;
            SafeDataReader safeReader = null;

            try
            {
                dbCommand = DALManager.Database.GetSqlStringCommand(sql);
                DALManager.Database.AddInParameter(dbCommand, paramName, DbType.Int32, sequenceId);

                safeReader = new SafeDataReader(DALManager.ExecuteReader(dbCommand));
            }
            catch (Exception ex)
            {
                ExceptionContext currentContext = new ExceptionContext();
                currentContext.Command = dbCommand;
                COEExceptionDispatcher.HandleDALException(ex, currentContext);
            }

            return safeReader;
        }

        public void ToggleAuditing(string value)
        {
            DbCommand cmd = DALManager.Database.GetStoredProcCommand("REGDB.Auditing.Toggle");
            cmd.Parameters.Add(new OracleParameter("status", OracleDbType.Varchar2, value, ParameterDirection.Input));
            DALManager.ExecuteNonQuery(cmd);
        }

       
    }
}
