using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using Csla.Data;
using CambridgeSoft.COE.Framework.Common.SqlGenerator;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.Properties;
using CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries;
using CambridgeSoft.COE.Framework.Common.SqlGenerator.NonQueries;
using CambridgeSoft.COE.Framework.COEDatabasePublishingService;
using CambridgeSoft.COE.Framework.COELoggingService;
using CambridgeSoft.COE.Framework.COEConfigurationService;
using CambridgeSoft.COE.Framework.COEDataViewService;

namespace CambridgeSoft.COE.Framework.COEDatabasePublishingService
{
    public class DAL : DALBase
    {
        [NonSerialized]
         static COELog _coeLog = COELog.GetSingleton("COEDatabasePublishing");
        public string _coeSchemaTableName = string.Empty;
        public string _coeDataViewTableName = string.Empty;

        public virtual int GetNewID()
        {
            throw new Exception("not implemented");
        }
        /// <summary>
        /// to get the name of the table
        /// </summary>
        public override void SetServiceSpecificVariables()
        {           
            string owner = this.DALManager.DatabaseData.OriginalOwner.ToString();
            COEDatabasePublishingUtilities.BuildTableName(owner, ref _coeSchemaTableName);
            COEDataViewUtilities.BuildDataViewTableName(owner, ref _coeDataViewTableName);
        }

        /// <summary>
        /// get Index type fields containing chemical structure for data view
        /// <returns>table of table and column name containing chemical structure</returns>
        /// </summary>
        /// <param name="database">Name of the data base</param>
        public virtual DataTable GetIndexTypeFields(string database)
        {
            throw new NotImplementedException("Method not implemented.");
        }

        /// <summary>
        /// Retrieves the index information from the selected database.
        /// </summary>
        /// <param name="database">Name of the database, used as database owner.</param>
        /// <returns>Returns datatable containing information about the index fields and columns on the selected database.</returns>
        public virtual DataTable GetIndexFields(string database)
        {
            throw new NotImplementedException("Method not implemented");
        }

        /// <summary>
        /// Retrieves the index information from the selected database and table.
        /// </summary>
        /// <param name="database">Name of the database, used as database owner.</param>
        /// <param name="table">Name of the table to find the index information</param>
        /// <returns>Returns datatable containing information about the index fields and columns on the selected database.</returns>
        public virtual DataTable GetIndexFields(string database, string table)
        {
            throw new NotImplementedException("Method not implemented");
        }

        /// <summary>
        /// Function for create index on database
        /// </summary>
        /// <param name="DatabaseName">Name of the database</param>
        /// <param name="TableName">Name of Table</param>
        /// <param name="ColumnName">Name of column on which index to be created</param>
        /// <returns>success of index creation</returns>
        public virtual Boolean CreateIndex(string DatabaseName, string TableName, string ColumnName)
        {
            throw new NotImplementedException("Method not implemented");
        }

        /// <summary>
        /// Insert values to security table, create role for the selected schema 
        /// </summary>
        /// <param name="schemaName">Name of the schema</param>
        public virtual int InsertPrivileges(string schemaName)
        {
            string sql = string.Empty;
            int recordsAffected = -1;
            sql = "CREATE ROLE " + schemaName + "_BROWSER NOT IDENTIFIED";
            try
            {
                DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sql);
                recordsAffected = DALManager.ExecuteNonQuery(dbCommand);
            }
            catch (Exception ex)
            {
                
            }

            sql = "GRANT 'CONNECT' TO " + schemaName + "_BROWSER";
            try
            {
                DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sql);
                recordsAffected = DALManager.ExecuteNonQuery(dbCommand);
            }
            catch (Exception ex)
            {  }

            sql = "GRANT CSS_USER to " + schemaName + "_BROWSER";
            try
            {
                DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sql);
                recordsAffected = DALManager.ExecuteNonQuery(dbCommand);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            sql = "GRANT " + schemaName + "_BROWSER TO CSS_ADMIN";
            try
            {
                DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sql);
                recordsAffected = DALManager.ExecuteNonQuery(dbCommand);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            sql = "INSERT INTO "  + Resources.SecurityDatabaseName  +".SECURITY_ROLES (ROLE_ID,PRIVILEGE_TABLE_INT_ID,ROLE_NAME,COEIDENTIFIER) ";
            sql += "values(" + Resources.SecurityDatabaseName + ".SECURITY_ROLES_SEQ.NEXTVAL,(select privilege_table_id from " + Resources.SecurityDatabaseName + ".privilege_tables where privilege_table_name='COE_PRIVILEGES'),'" + schemaName + "_BROWSER','"+ schemaName +"')";
          
            try
            {
                DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sql);
                recordsAffected = DALManager.ExecuteNonQuery(dbCommand);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            sql = "INSERT INTO " + Resources.SecurityDatabaseName + ".COE_PRIVILEGES(ROLE_INTERNAL_ID,CAN_BROWSE, CAN_UPDATE,CAN_INSERT,CAN_DELETE)";
            sql += "values((select max(role_id) from " + Resources.SecurityDatabaseName + ".security_roles),'1','0','0','0')";

            try
            {
                DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sql);
                recordsAffected = DALManager.ExecuteNonQuery(dbCommand);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            sql = "CREATE ROLE " + schemaName + "_ADMIN NOT IDENTIFIED";
            try
            {
                DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sql);
                recordsAffected = DALManager.ExecuteNonQuery(dbCommand);
            }
            catch (Exception ex)
            {
                
            }

            sql = "GRANT 'CONNECT' TO " + schemaName + "_ADMIN";
            try
            {
                DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sql);
                recordsAffected = DALManager.ExecuteNonQuery(dbCommand);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            sql = "GRANT CSS_USER to " + schemaName + "_ADMIN";
            try
            {
                DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sql);
                recordsAffected = DALManager.ExecuteNonQuery(dbCommand);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            sql = "GRANT " + schemaName + "_ADMIN TO CSS_ADMIN";
            try
            {
                DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sql);
                recordsAffected = DALManager.ExecuteNonQuery(dbCommand);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            sql = "INSERT INTO " + Resources.SecurityDatabaseName + ".SECURITY_ROLES (ROLE_ID,PRIVILEGE_TABLE_INT_ID,ROLE_NAME,COEIDENTIFIER) ";
            sql += "values(" + Resources.SecurityDatabaseName + ".SECURITY_ROLES_SEQ.NEXTVAL,(select privilege_table_id from " + Resources.SecurityDatabaseName + ".privilege_tables where privilege_table_name='COE_PRIVILEGES'),'" + schemaName + "_ADMIN','" + schemaName + "')";
            
            try
            {
                DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sql);
                recordsAffected = DALManager.ExecuteNonQuery(dbCommand);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            sql = "INSERT INTO " + Resources.SecurityDatabaseName + ".COE_PRIVILEGES(ROLE_INTERNAL_ID,CAN_BROWSE, CAN_UPDATE,CAN_INSERT,CAN_DELETE)";
            sql += "values((select max(role_id) from " + Resources.SecurityDatabaseName + ".security_roles),'1','1','1','1')";

            try
            {
                DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sql);
                recordsAffected = DALManager.ExecuteNonQuery(dbCommand);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return recordsAffected;
        }

        /// <summary>
        /// Insert values to security table, create role for the selected schema 
        /// </summary>
        /// <param name="schemaName">Name of the schema</param>
        public virtual int RemovePrivileges(string schemaName)
        {
            string sql = string.Empty;
            int recordsAffected = -1;
            sql = "DROP ROLE " + schemaName + "_BROWSER";
            try
            {
                DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sql);
                recordsAffected = DALManager.ExecuteNonQuery(dbCommand);
            }
            catch (Exception ex)
            {
                
            }


            sql = "DELETE FROM " + Resources.SecurityDatabaseName + ".COE_PRIVILEGES WHERE ROLE_INTERNAL_ID =(select ROLE_ID FROM " + Resources.SecurityDatabaseName + ".security_roles where role_name = '" + schemaName + "_BROWSER')";
            
            try
            {
                DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sql);
                recordsAffected = DALManager.ExecuteNonQuery(dbCommand);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            sql = "DELETE FROM " + Resources.SecurityDatabaseName + ".SECURITY_ROLES WHERE ROLE_NAME= '" + schemaName + "_BROWSER'";
              try
            {
                DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sql);
                recordsAffected = DALManager.ExecuteNonQuery(dbCommand);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            sql = "DROP ROLE " + schemaName + "_ADMIN";
            try
            {
                DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sql);
                recordsAffected = DALManager.ExecuteNonQuery(dbCommand);
            }
            catch (Exception ex)
            {
                
            }


            sql = "DELETE FROM " + Resources.SecurityDatabaseName + ".COE_PRIVILEGES WHERE ROLE_INTERNAL_ID=(select ROLE_ID FROM " + Resources.SecurityDatabaseName + ".security_roles where role_name = '" + schemaName + "_ADMIN')";

            try
            {
                DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sql);
                recordsAffected = DALManager.ExecuteNonQuery(dbCommand);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            sql = "DELETE FROM " + Resources.SecurityDatabaseName + ".SECURITY_ROLES WHERE ROLE_NAME= '" + schemaName + "_ADMIN'";
            try
            {
                DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sql);
                recordsAffected = DALManager.ExecuteNonQuery(dbCommand);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
            return recordsAffected;
        }

        /// <summary>
        /// Grant the Role for the created Schema.
        /// </summary>
        /// <param name="userList">List of users</param>        
        /// <param name="schemaName">Name of the schema</param>
        public virtual void GrantProxy(string schemaName, string globalSchemaName)
        {
            throw new NotImplementedException("Method not implemented.");
        }

        /// <summary>
        /// Grant the Role for the created Schema.
        /// </summary>
        /// <param name="userList">List of users</param>        
        /// <param name="schemaName">Name of the schema</param>
        public virtual void RevokeProxy(string schemaName, string globalSchemaName)
        {
            throw new NotImplementedException("Method not implemented.");
        }

        /// <summary>   
        /// Executes two queries defined in the method.
        /// <returns>DataSet Containing two tables, one containing the list of columns of each table in the database and the other containing the relationships </returns>
        /// </summary>
        public virtual DataSet Get_DatabaseSchema(string OwnerName)
        {
            throw new NotImplementedException("Method not implemented.");
        }

        /// <summary>
        /// Populate all the Owners in the database
        /// </summary>
        /// <returns>list of Owners </returns>
        public virtual SafeDataReader GetAllInstanceDatabases()
        {
            throw new NotImplementedException("Method not implemented.");
        }

        /// <summary>
        /// Populate all the Owners in the database
        /// </summary>
        /// <returns>list of Owners </returns>
        public virtual SafeDataReader GetPublishedDatabases()
        {
            DbCommand dbCommand = null;
            string sql = "SELECT * FROM " + _coeSchemaTableName;
            dbCommand = DALManager.Database.GetSqlStringCommand(sql);
            SafeDataReader safeReader = new SafeDataReader(DALManager.Database.ExecuteReader(dbCommand));
            return safeReader;
        }

        internal virtual int InsertCOEDatabaseDataView(string ownerName, string serializedCOEDataView)
        {
            //retrieving the id to be inserted
            int id = GetNewID();
            string sql = "INSERT INTO " + _coeSchemaTableName +
                 "(ID,NAME,COEDATAVIEW,DATE_CREATED)VALUES " +
                 "( " + DALManager.BuildSqlStringParameterName("pId") +
                 " ," + DALManager.BuildSqlStringParameterName("pName") +
                 " , " + DALManager.BuildSqlStringParameterName("pCOEDataView") +
                 " , " + DALManager.BuildSqlStringParameterName("pDateCreated") + ")";
            DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sql);

            DALManager.Database.AddInParameter(dbCommand, "pId", DbType.Int32, id);
            DALManager.Database.AddParameter(dbCommand, "pName", DbType.AnsiString, 1000, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, ownerName.ToUpper());
            DALManager.Database.AddParameter(dbCommand, "pCOEDataView", DbType.AnsiString, Int32.MaxValue, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, serializedCOEDataView);//Changed by Sumeet, (1000 -> Int32.MaxValue)So that test XML can be used
            DALManager.Database.AddInParameter(dbCommand, "pDateCreated", DbType.Date, DateTime.Now);
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

        internal virtual void UpdateCOEDatabaseDataView(string ownerNameWithInstance, string serializedCOEDataView, DateTime dateModified)
        {
            string sql = "UPDATE " + _coeSchemaTableName +
                 " SET COEDATAVIEW= " + DALManager.BuildSqlStringParameterName("pCOEDataView") +
                 " ,DATE_CREATED= " + DALManager.BuildSqlStringParameterName("pDateCreated") + 
                 " WHERE Name= " + DALManager.BuildSqlStringParameterName("pOwnerName");

            DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sql);

            DALManager.Database.AddParameter(dbCommand, "pCOEDataView", DbType.AnsiString, Int32.MaxValue, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, serializedCOEDataView);
            DALManager.Database.AddInParameter(dbCommand, "pDateCreated", DbType.DateTime, dateModified);
            DALManager.Database.AddParameter(dbCommand, "pOwnerName", DbType.AnsiString, 250, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, ownerNameWithInstance.ToUpper());

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
        /// update a dataview
        /// </summary>
        /// <param name="dataViewId">dataviewid for which update in done</param>
        /// <param name="dataView">the updated xml form of the dataview</param>
        /// <param name="dataViewName">the updated name of the dataview</param>
        /// <param name="description">the updated description of the dataview</param>
        /// <param name="isPublic">whether the dataview is made public or not</param>
        public virtual void UpdateCOEDataView(int id, string serializedCOEDataView, string name, string description, bool isPublic, string databaseName, string application)
        {
            throw new NotImplementedException("Method not implemented.");
        }

        /// <summary>
        /// Deletes the schemas under the instance.
        /// </summary>
        /// <param name="instanceName">The instance name.</param>
        public virtual void DeleteInstanceSchemas(string instanceName)
        {
            throw new NotImplementedException("Method not implemented.");
        }

        public virtual SafeDataReader GetCOEDatabaseDataView(string ownerName)
        {
            SafeDataReader safeReader = null;
            DbCommand dbCommand = null;
            string sql = "SELECT id, name, coedataview, date_created FROM " + _coeSchemaTableName + " WHERE name=" + DALManager.BuildSqlStringParameterName("pOwnerName");
            dbCommand = DALManager.Database.GetSqlStringCommand(sql);
            DALManager.Database.AddParameter(dbCommand, "pOwnerName", DbType.AnsiString, 200, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, ownerName);
            try
            {
                safeReader = new SafeDataReader(DALManager.Database.ExecuteReader(dbCommand));
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return safeReader;
        }

        public virtual void DeleteCOEDatabaseDataView(string ownerName)
        {
            DbCommand dbCommand = null;
            string sql = "DELETE FROM " + _coeSchemaTableName + " WHERE name=" + DALManager.BuildSqlStringParameterName("pOwnerName");
            dbCommand = DALManager.Database.GetSqlStringCommand(sql);
            DALManager.Database.AddParameter(dbCommand, "pOwnerName", DbType.AnsiString, 200, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, ownerName);
            DALManager.Database.ExecuteNonQuery(dbCommand);
        }

        /// <summary>   
        /// Check owner in the database
        /// <returns>true, if owner and password are correct</returns>
        /// </summary>
        public virtual bool AuthenticateUser(string ownerName, string password)
        {
            throw new Exception("not implemented");
        }

        public virtual bool AuthenticateUser(string ownerName, string password, string host, int port, string serviceName)
        {
            throw new Exception("not implemented");
        }

        internal virtual void GrantTable(string databaseName, string tableName)
        {
            throw new Exception("not implemented");
        }

        internal virtual void RevokeTable(string databaseName, string tableName)
        {
            throw new Exception("not implemented");
        }      
    }

    public class DataReaderAdapter : DbDataAdapter
    {
        public int FillFromReader(DataTable dataTable, IDataReader dataReader)
        {
            return this.Fill(dataTable, dataReader);
        }
      
        protected override RowUpdatedEventArgs CreateRowUpdatedEvent(
            DataRow dataRow,
            IDbCommand command,
            StatementType statementType,
            DataTableMapping tableMapping
        ) { return null; }

        protected override RowUpdatingEventArgs CreateRowUpdatingEvent(
        DataRow dataRow,
        IDbCommand command,
        StatementType statementType,
        DataTableMapping tableMapping
        ) { return null; }

        protected override void OnRowUpdated(
        RowUpdatedEventArgs value
        ) { }
        protected override void OnRowUpdating(
        RowUpdatingEventArgs value
        ) { }
    }

}
