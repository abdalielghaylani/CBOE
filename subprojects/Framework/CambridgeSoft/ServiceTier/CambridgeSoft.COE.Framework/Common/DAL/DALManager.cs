using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Instrumentation;
using Microsoft.Practices.EnterpriseLibrary.Common.Instrumentation;
using Microsoft.Practices.EnterpriseLibrary.Common;
using Microsoft.Practices.EnterpriseLibrary.Data.Oracle;
using Microsoft.Practices.EnterpriseLibrary.Data.OracleDataAccessClient;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using Microsoft.Practices.EnterpriseLibrary.Data.OleDb;
using CambridgeSoft.COE.Framework.Properties;
using CambridgeSoft.COE.Framework.COEConfigurationService;


namespace CambridgeSoft.COE.Framework.Common
{
    /// <summary>
    /// The DALManager object controls all aspects of connectiong to a database.
    /// </summary>
    public class DALManager
    {

        private Database database;
        private DbConnection dbConnection;
        private DbTransaction dbTransaction;
        private DatabaseData databaseData;
        private ServiceData serviceData;
        private DBMSTypeData dbmsTypeData;



        /// <summary>
        /// Creates DALManager object through which all request to the Database are made
        /// </summary>
        /// <param name="connStringType">Connection string type</param>
        /// <param name="databaseData">Database configuration information</param>
        /// <param name="serviceData">Service configuration information</param>
        public DALManager(DatabaseData databaseData, ServiceData serviceData, DBMSTypeData dbmsTypeData, bool adminOverride)
        {
            this.serviceData = serviceData;
            this.dbmsTypeData = dbmsTypeData;
            this.databaseData = databaseData;


            //fill databaseData.DataSource with default datasource for dbmstype if empty
            if (databaseData.DataSource.ToString() == string.Empty)
            {
                databaseData.DataSource = dbmsTypeData.DataSource;
            }

            string ConnString = DALUtils.GetConnString(databaseData, dbmsTypeData, adminOverride);

            switch (databaseData.ProviderName)
            {
                case "System.Data.OracleClient":
                case "Oracle.DataAccess.Client":
                    OracleDatabase oralceODPDatabase = new OracleDatabase(ConnString);
                    this.Database = oralceODPDatabase;
                    break;
                case "System.Data.SqlClient":
                    SqlDatabase sqlDatabase = new SqlDatabase(ConnString);
                    this.Database = sqlDatabase;
                    break;
                case "System.Data.Oledb":
                    OleDbDatabase oledbDatabase = new OleDbDatabase(ConnString);
                    this.Database = oledbDatabase;
                    break;
                case "System.Data.Odbc":
                    GenericDatabase odbcDatabase = new GenericDatabase(ConnString, DbProviderFactories.GetFactory("System.Data.Odbc"));
                    this.Database = odbcDatabase;
                    break;
            }

            //set up instrumentation listener is the instrumentation settings indicated to do so.
            //DataInstrumentationListener listener = new DataInstrumentationListener("COECore", true, true, true);
            //DataInstrumentationListenerBinder binder = new DataInstrumentationListenerBinder();
            //binder.Bind(this.Database.GetInstrumentationEventProvider(), new DataInstrumentationListener("COECore", true, true, true));

            AssociateSession();
        }


        /// <summary>
        /// Associatest the current request with an existing session or creates a new session. User for
        /// single sign on only. Configuraiton driven
        /// </summary>
        public void AssociateSession()
        {

            //TODO: need to hook up with the Identity object
        }



        /// <summary>
        /// Create the database object that reflects the database type
        /// </summary>
        public Database Database
        {
            get { return this.database; }
            set { this.database = value; }
        }

        /// <summary>
        /// Create the DBMSTypeData 
        /// </summary>
        public DBMSTypeData DBMSTypeData
        {
            get { return this.dbmsTypeData; }
            set { this.dbmsTypeData = value; }
        }

        /// <summary>
        /// Creates a transaction object for the current dal operation
        /// </summary>
        public DbTransaction DbTransaction
        {
            get { return this.dbTransaction; }
            set { this.dbTransaction = value; }
        }

        /// <summary>
        /// Get or set DbConnection object
        /// </summary>
        public DbConnection DbConnection
        {
            get { return this.dbConnection; }
            set { this.dbConnection = value; }
        }

        /// <summary>
        /// Begin a transcation
        /// </summary>
        public void BeginTransaction()
        {
            GetConnection();
            this.DbConnection.Open();

            //set the Dbtransaction property
            DbTransaction = this.DbConnection.BeginTransaction();
        }

        /// <summary>
        /// Gets the connection for the current DAL request
        /// </summary>
        public void GetConnection()
        {
            //sets the dbconnection property with a newly created connetion
            DbConnection = this.Database.CreateConnection();

        }

        /// <summary>
        /// Commit current transaction
        /// </summary>
        public void CommitTransaction()
        {

            DbTransaction.Commit();
            this.DbConnection.Close();
            this.DbConnection.Dispose();
            this.DbConnection = null;
            dbTransaction = null;
        }

        /// <summary>
        ///  Rollback current tranction
        /// </summary>
        public void RollbackTransaction()
        {
            //if calling this method, then an exception is likely to have occurred. If rollback fails, then this secondary exception will overwrite the original problem. 
            //that's why the empty catch.
            try
            {
                DbTransaction.Rollback();
                this.DbConnection.Close();
                this.DbConnection.Dispose();
                this.DbConnection = null;
                DbTransaction.Dispose();
                dbTransaction = null;
            }
            catch { }
        }

        /// <summary>
        /// Get or set the ServiceData object containing information about the targeted service
        /// </summary>
        /// <value>The ServiceData object containing the serice data</value>
        public ServiceData ServiceData
        {
            get { return this.serviceData; }
            set { this.serviceData = value; }
        }
        /// <summary>
        /// Get or set the DatabaseData object containing information about the database making the current DALManager call
        /// </summary>
        /// <value>The DatabaseData object containing the database data</value>
        public DatabaseData DatabaseData
        {
            get { return this.databaseData; }
            set { this.databaseData = value; }
        }

        /// <summary>
        /// Build the sql parameter with the approparite token for the DBMS type 
        /// </summary>
        /// <param name="Name">The parameter name</param>
        /// <returns>A sql parameter name with its proper token</returns>
        public string BuildSqlStringParameterName(string Name)
        {
            string returnName = "";
            switch (DatabaseData.ProviderName)
            {
                case "System.Data.OracleClient":
                    returnName = "?";
                    break;
                case "Oracle.DataAccess.Client":
                    returnName = this.Database.ParameterToken + Name;
                    break;
                case "System.Data.SqlClient":
                    returnName = this.Database.ParameterToken + Name;
                    break;
                case "System.Data.Oledb":
                    returnName = "?";
                    break;
                case "System.Data.Odbc":
                    returnName = "?";
                    break;
            }
            return returnName;
        }


        /// <summary>
        /// Start tracing depending on configuration setting in coeConfiguration
        /// </summary>
        public void SetTracing()
        {

            if (DatabaseData.Tracing || DatabaseData.OracleTracing)
            {
                switch (DatabaseData.ProviderName)
                {
                    case "System.Data.OracleClient":
                        if (DatabaseData.OracleTracing)
                        {
                            if (this.DbTransaction != null)
                            {
                                int myInt = this.Database.ExecuteNonQuery(this.DbTransaction, CommandType.Text, "Alter Session set sql_trace = true");
                            }
                            else
                            {
                                int myInt = this.Database.ExecuteNonQuery(CommandType.Text, "Alter Session set sql_trace = true");
                            }
                        }
                        break;
                    case "Oracle.DataAccess.Client":
                        if (DatabaseData.OracleTracing)
                        {
                            if (this.DbTransaction != null)
                            {
                                int myInt = this.Database.ExecuteNonQuery(this.DbTransaction, CommandType.Text, "Alter Session set sql_trace = true");
                            }
                            else
                            {
                                int myInt = this.Database.ExecuteNonQuery(CommandType.Text, "Alter Session set sql_trace = true");
                            }
                        }
                        break;
                    case "System.Data.SqlClient":
                        //
                        break;
                    case "System.Data.Oledb":
                        //
                        break;
                    case "System.Data.Odbc":
                        //
                        break;
                }
            }
        }

        /// <summary>
        /// <para>Executes the <paramref name="dbCommand"/> and returns the number of rows affected.</para>
        /// </summary>
        /// <param name="dbCommand">The command that contains the query to execute.</param>
        /// </param>       
        /// <seealso cref="IDbCommand.ExecuteScalar"/>
        public int ExecuteNonQuery(DbCommand dbCommand)
        {
            this.SetTracing();
            //if (dbCommand is Oracle.DataAccess.Client.OracleCommand)
            //{
            //    ((Oracle.DataAccess.Client.OracleCommand)dbCommand).InitialLOBFetchSize = -1;
            //}
            int recordsAffected;
            try
            {
                if (this.DbTransaction != null)
                {
                    recordsAffected = this.Database.ExecuteNonQuery(dbCommand, this.DbTransaction);
                }
                else
                {
                    recordsAffected = this.Database.ExecuteNonQuery(dbCommand);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return recordsAffected;
        }

        /// <summary>
        /// <para>Executes the <paramref name="dbCommand"/> and adds a new <see cref="DataTable"/> to the existing <see cref="DataSet"/>.</para>
        /// </summary>
        /// <param name="dbCommand">
        /// <param>The <see cref="DbCommand"/> to execute.</para>
        /// </param>
        /// <param name="dataSet">
        /// <para>The <see cref="DataSet"/> to load.</para>
        /// </param>
        /// <param name="tableName">
        /// <para>The name for the new <see cref="DataTable"/> to add to the <see cref="DataSet"/>.</para>
        /// </param>        
        /// <exception cref="System.ArgumentNullException">Any input parameter was <see langword="null"/> (<b>Nothing</b> in Visual Basic)</exception>
        /// <exception cref="System.ArgumentException">tableName was an empty string</exception>
        public void LoadDataSet(DbCommand dbCommand, DataSet dataSet, string tableName)
        {
            this.SetTracing();
            //if (dbCommand is Oracle.DataAccess.Client.OracleCommand)
            //{
            //    ((Oracle.DataAccess.Client.OracleCommand)dbCommand).InitialLOBFetchSize = -1;
            //}
            try
            {
                if (this.DbTransaction != null)
                {
                    this.Database.LoadDataSet(dbCommand, dataSet, new string[] { tableName }, this.DbTransaction);
                }
                else
                {
                    this.Database.LoadDataSet(dbCommand, dataSet, new string[] { tableName });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// <para>Executes the <paramref name="dbCommand"/> and returns an <see cref="IDataReader"/> through which the result can be read.
        /// It is the responsibility of the caller to close the connection and reader when finished.</para>
        /// </summary>
        /// <param name="dbCommand">
        /// <para>The command that contains the query to execute.</para>
        /// </param>
        /// <returns>
        /// <para>An <see cref="IDataReader"/> object.</para>
        /// </returns>      
        public virtual IDataReader ExecuteReader(DbCommand dbCommand)
        {
            this.SetTracing();
            //if (dbCommand is Oracle.DataAccess.Client.OracleCommand)
            //{
            //    ((Oracle.DataAccess.Client.OracleCommand)dbCommand).InitialLOBFetchSize = -1;
            //}
            try
            {
                if (this.DbTransaction != null)
                {
                    return this.Database.ExecuteReader(dbCommand, this.DbTransaction);
                }
                else
                {
                    return this.Database.ExecuteReader(dbCommand);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// <para>Executes the <paramref name="dbCommand"/> and returns the first column of the first row in the result set returned by the query. Extra columns or rows are ignored.</para>
        /// </summary>
        /// <param name="dbCommand">
        /// <para>The command that contains the query to execute.</para>
        /// </param>
        /// <returns>
        /// <para>The first column of the first row in the result set.</para>
        /// </returns>
        /// <seealso cref="IDbCommand.ExecuteScalar"/>
        public object ExecuteScalar(DbCommand dbCommand)
        {
            this.SetTracing();
            //if (dbCommand is Oracle.DataAccess.Client.OracleCommand)
            //{
            //    ((Oracle.DataAccess.Client.OracleCommand)dbCommand).InitialLOBFetchSize = -1;
            //}
            try
            {
                if (this.DbTransaction != null)
                {
                    return this.Database.ExecuteScalar(dbCommand, this.DbTransaction);
                }
                else
                {
                    return this.Database.ExecuteScalar(dbCommand);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// <para>Executes the <paramref name="dbCommand"/> and returns the results in a new <see cref="DataSet"/>.</para>
        /// </summary>
        /// <param name="dbCommand"><para>The <see cref="DbCommand"/> to execute.</para></param>
        /// <returns>A <see cref="DataSet"/> with the results of the <paramref name="dbCommand"/>.</returns>        
        public DataSet ExecuteDataSet(DbCommand dbCommand)
        {
            this.SetTracing();
            try
            {
                if (this.DbTransaction != null)
                {
                    return this.Database.ExecuteDataSet(dbCommand, this.DbTransaction);
                }
                else
                {
                    return this.Database.ExecuteDataSet(dbCommand);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
