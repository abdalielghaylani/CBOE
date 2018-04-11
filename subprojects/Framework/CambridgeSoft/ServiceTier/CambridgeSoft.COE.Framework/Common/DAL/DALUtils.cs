using System;
using System.Collections.Generic;
using System.Text;
using CambridgeSoft.COE.Framework.COEConfigurationService;
using CambridgeSoft.COE.Framework.Properties;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data.Common;
using System.Data;


namespace CambridgeSoft.COE.Framework.Common
{
    /// <summary>
    /// Common Data Access Logic utils.
    /// </summary>
    public class DALUtils
    {
        /// <summary>
        /// <para>Gets a <see cref="DALManager"/> based on the configuration.</para>
        /// </summary>
        /// <param name="databaseData">Database configuration section</param>
        /// <param name="serviceData">Service configuration section</param>
        /// <param name="connStringType">Connection string type</param>
        /// <returns>A <see cref="DALManager"/></returns>
        public static DALManager GetDALManager(DatabaseData databaseData, ServiceData serviceData, DBMSTypeData dbmsTypeData, bool adminOverride)
        {
            DALManager dALManager = new DALManager(databaseData, serviceData, dbmsTypeData, adminOverride);
            return dALManager;
        }

        /// <summary>
        /// Build a connection string for calling the datastore
        /// </summary>
        /// <param name="databaseData">Database configuration section</param>
        /// <param name="connStringType">Connection string type</param>
        /// <returns>The connection string</returns>
        public static string GetConnString(DatabaseData databaseData, DBMSTypeData dbmsTypeData, bool adminOverride)
        {
            //set default
            ConnStringType connStringType = ConnStringType.OWNER;
            //check is this is a proxy
            bool useProxy = System.Convert.ToBoolean(dbmsTypeData.UseProxy);

            //determine connType for builing string
            switch (databaseData.DBMSType)
            {
                case DBMSType.ORACLE:
                    if (useProxy == true)
                    {
                        if (adminOverride == true)
                        {
                            connStringType = ConnStringType.OWNERPROXY;
                        }
                        else
                        {
                            connStringType = ConnStringType.PROXY;
                        }
                    }
                    else
                    {
                        connStringType = ConnStringType.OWNER;
                    }
                    break;
                case DBMSType.SQLSERVER:
                    connStringType = ConnStringType.OWNER;
                    break;
                case DBMSType.MSACCESS:
                    connStringType = ConnStringType.OWNER;
                    break;
            }

            string connString = AddCredentials(databaseData, connStringType);
            connString = AddDataSource(databaseData.DataSource, connString);
            connString = AddProviderOptions(databaseData.ProviderOptions, connString);

            return connString;
        }

        /// <summary>
        /// Adds credentials to the connection string.
        /// </summary>
        /// <param name="providerName">Fully calified class name of the data access provider</param>
        /// <param name="userName">The user name</param>
        /// <param name="password">The password</param>
        /// <param name="connStringType">The connection string type</param>
        /// <returns>The connection string with the added credentials</returns>
        public static string AddCredentials(DatabaseData databaseData, ConnStringType connStringType)
        {
            string providerName = databaseData.ProviderName;
            string userName = databaseData.Owner;
            string password = Utilities.IsRijndaelEncrypted(databaseData.Password) ? Utilities.DecryptRijndael(databaseData.Password) : databaseData.Password;
            string userNameKeyword = string.Empty;
            string passwordKeyword = string.Empty;
            string proxyuserNameKeyword = string.Empty;
            string proxypasswordKeyword = string.Empty;
            string connString = string.Empty;

            //get keywords based on provider 
            switch (providerName)
            {
                case "System.Data.OracleClient":
                    userNameKeyword = "user id=";
                    passwordKeyword = "password=";
                    proxyuserNameKeyword = "proxy user id=";
                    proxypasswordKeyword = "proxy password=";
                    break;
                case "Oracle.DataAccess.Client":
                    userNameKeyword = "user id=";
                    passwordKeyword = "password=";
                    proxyuserNameKeyword = "proxy user id=";
                    proxypasswordKeyword = "proxy password=";
                    break;
                case "System.Data.SqlClient":
                    userNameKeyword = "user id=";
                    passwordKeyword = "password=";
                    proxyuserNameKeyword = "proxy user id=";
                    proxypasswordKeyword = "proxy password=";
                    break;
                case "System.Data.Oledb":
                    userNameKeyword = "uid=";
                    passwordKeyword = "pwd=";
                    proxyuserNameKeyword = "puid=";
                    proxypasswordKeyword = "ppwd=";
                    break;
                case "System.Data.Odbc":
                    userNameKeyword = "uid=";
                    passwordKeyword = "pwd=";
                    proxyuserNameKeyword = "puid=";
                    proxypasswordKeyword = "ppwd=";
                    break;
            }

            //build credentials string based on connstringtype input
            switch (connStringType)
            {
                case ConnStringType.PROXY:
                    connString = userNameKeyword + COEUser.Get().ToString() + ";";
                    connString = connString + proxyuserNameKeyword + userName + ";";
                    connString = connString + proxypasswordKeyword + password;
                    break;
                case ConnStringType.OWNERPROXY:
                    connString = userNameKeyword + databaseData.OriginalOwner + ";";
                    connString = connString + proxyuserNameKeyword + userName + ";";
                    connString = connString + proxypasswordKeyword + password;
                    break;
                case ConnStringType.OWNER:
                    connString = userNameKeyword + userName + ";";
                    connString = connString + passwordKeyword + password;
                    break;
            }

            return connString;
        }

        /// <summary>
        /// Adds a datasource to the connection string
        /// </summary>
        /// <param name="dataSource">The datasource name</param>
        /// <param name="connString">The connection string without datasource</param>
        /// <returns>The connection string with the added datasource</returns>
        public static string AddDataSource(string dataSource, string connString)
        {
            return connString + ";Data Source=" + dataSource;
        }

        /// <summary>
        /// Adds specific provider options to the connection string
        /// </summary>
        /// <param name="providerOptions">The provider options</param>
        /// <param name="connString">The connection string</param>
        /// <returns>The connection string with the added provider options</returns>
        public static string AddProviderOptions(string providerOptions, string connString)
        {
            return connString + ";" + providerOptions;
        }

        /// <summary>
        /// Disposes a the command object in all cases.
        /// Disposes the connection object only when it is not part of the transaction.
        /// </summary>
        /// <param name="connection">The DBCommand object to destroy (connection is part of the command)</param>
        public static void DestroyCommandAndConnection(DbCommand command)
        {
            //Coverity Bug Fix :- CID : 11850  Jira Id :CBOE-194
            if (command != null && command.Connection != null && command.Transaction == null)
            {
                if (command.Connection.State != ConnectionState.Closed)
                {
                    command.Connection.Close();
                }
                command.Dispose();
            }

        }
    }
}
