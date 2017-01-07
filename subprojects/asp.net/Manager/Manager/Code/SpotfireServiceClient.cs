// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SpotfireServiceClient.cs" company="PerkinElmer Inc.">
//   Copyright (c) 2013 PerkinElmer Inc.,
//   940 Winter Street, Waltham, MA 02451.
//   All rights reserved.
//   This software is the confidential and proprietary information
//   of PerkinElmer Inc. ("Confidential Information"). You shall not
//   disclose such Confidential Information and may not use it in any way,
//   absent an express written license agreement between you and PerkinElmer Inc.
//   that authorizes such use.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Manager.Code
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Globalization;
    using System.Linq;
    using System.Net;
    using System.Net.Security;
    using System.Security.Cryptography.X509Certificates;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Security;
    using System.Text;
    using System.Threading.Tasks;

    using CambridgeSoft.COE.Framework.COEConfigurationService;
    using CambridgeSoft.COE.Framework.COEDatabasePublishingService;
    using CambridgeSoft.COE.Framework.COEDataViewService;
    using CambridgeSoft.COE.Framework.Common;
    using SpotfireElementManagerService;
    using SpotfireLibraryService;
    using Utility = Utilities;

    /// <summary>
    /// Class for Spotfire service API
    /// </summary>
    public class SpotfireServiceClient : IDisposable
    {
        private readonly string spotfireServiceUrl = "{0}/spotfire/ws";
        private readonly string elementManagerServiceAddress = "{0}/ElementManagerService?";
        private readonly string libraryServiceAddress = "{0}/LibraryService?";
        private readonly string userName;
        private readonly string password;

        private ElementManagerServiceClient spotfireElementManagerServiceClient;
        private LibraryServiceClient spotfireLibraryServiceClient;

        private const string instanceDatasourcePathPattern = "DLDS_{0}_{1}";
        private const string dataviewFolderPattern = "Dataviews_{0}";

        /// <summary>
        /// Initializes a new instance of the <see cref="SpotfireServiceClient"/> class. 
        /// </summary>
        /// <param name="spotfireUrl"> The Spotfire server url. </param>
        /// <param name="userName"> The Spotfire administrator user name. </param>
        /// <param name="password"> The Spotfire administrator password. </param>
        public SpotfireServiceClient(string spotfireUrl, string userName, string password)
        {
            this.userName = userName;
            this.password = password;
            this.spotfireServiceUrl = string.Format(this.spotfireServiceUrl, spotfireUrl);
            this.elementManagerServiceAddress = string.Format(elementManagerServiceAddress, spotfireServiceUrl);
            this.libraryServiceAddress = string.Format(libraryServiceAddress, spotfireServiceUrl);

            ServicePointManager.ServerCertificateValidationCallback += ServerCertificateValidationCallback;

            // initialize element manager service client
            InitializeElementManagerServiceClient();

            // initialize library service client
            InitializeLibraryServiceClient();
        }

        /// <summary>
        /// Gets Spotfire element manager service client instance
        /// </summary>
        internal ElementManagerServiceClient SpotfireElementManagerServiceClient
        {
            get { return spotfireElementManagerServiceClient; }
        }

        /// <summary>
        /// Gets Spotfire library service client instance
        /// </summary>
        internal LibraryServiceClient SpotfireLibraryServiceClient
        {
            get { return spotfireLibraryServiceClient; }
        } 

        #region Static Methods

        /// <summary>
        /// Delete db instance.
        /// </summary>
        /// <param name="instanceBO"> The instance business object. </param>
        /// <returns> The <see cref="string"/>. </returns>
        public static string DeleteDBInstance(COEInstanceBO instanceBO)
        {
            var spotfire = COESpotFireSettingsBO.Get();
            string spotfireUrl = spotfire.SpotfireURL;
            string userName = spotfire.SpotfireUser;
            string password = DecryptPassword(spotfire.SpotfirePassword);

            var errorMsg = ActionWithExceptionHandling(() =>
            {
                // Publish database instance to Spotfire server
                using (var spotfireServiceClient = new SpotfireServiceClient(spotfireUrl, userName, password))
                {
                    var rootFolderId = spotfireServiceClient.GetDatalytixRootFolder();
                    spotfireServiceClient.DeleteDataSource(
                        rootFolderId,
                        string.Format(instanceDatasourcePathPattern, instanceBO.InstanceName.Trim().ToUpper(), instanceBO.Id));
                }
            });

            return errorMsg;
        }

        /// <summary>
        /// The publish db instance.
        /// </summary>
        /// <param name="instanceBO"> The instance business object. </param>
        /// <returns> The <see cref="string"/>. </returns>
        public static string PublishDBInstance(COEInstanceBO instanceBO)
        {
            var spotfire = COESpotFireSettingsBO.Get();
            string spotfireUrl = spotfire.SpotfireURL;
            string userName = spotfire.SpotfireUser;
            string password = DecryptPassword(spotfire.SpotfirePassword);

            var errorMsg = ActionWithExceptionHandling(() =>
            {
                // Publish database instance to Spotfire server
                using (var spotfireServiceClient = new SpotfireServiceClient(spotfireUrl, userName, password))
                {
                    var rootFolderId = spotfireServiceClient.GetDatalytixRootFolder();
                    spotfireServiceClient.CreateDataSource(
                        rootFolderId,
                        string.Format(instanceDatasourcePathPattern, instanceBO.InstanceName.Trim().ToUpper(), instanceBO.Id),
                        instanceBO.HostName.Trim(),
                        instanceBO.Port,
                        instanceBO.SID,
                        instanceBO.DatabaseGlobalUser.Trim(),
                        string.IsNullOrEmpty(instanceBO.Password) ? GetPassword(instanceBO) : DecryptPassword(instanceBO.Password),
                        true,
                        instanceBO.DriverType);
                }
            });

            return errorMsg;
        }

        /// <summary>
        /// Publish data view.
        /// </summary>
        /// <param name="dataview"> The dataview. </param>
        /// <returns> The <see cref="string"/>. </returns>
        public static string PublishDataView(COEDataViewBO dataview)
        {
            var spotfire = COESpotFireSettingsBO.Get();
            string spotfireUrl = spotfire.SpotfireURL;
            string userName = spotfire.SpotfireUser;
            string password = DecryptPassword(spotfire.SpotfirePassword);

            var errorMsg = ActionWithExceptionHandling(() =>
            {
                // Publish database instance to Spotfire server
                using (var spotfireServiceClient = new SpotfireServiceClient(spotfireUrl, userName, password))
                {
                    var defaultInstance = ConfigurationUtilities.GetMainInstance();
                    var rootFolderId = spotfireServiceClient.GetDatalytixRootFolder();

                    var instanceDataviewRootId = spotfireServiceClient.CreateFolder(
                        string.Format(dataviewFolderPattern, defaultInstance.Id), rootFolderId);

                    var coeDataView = dataview.COEDataView;
                    var dataviewFolderId =
                        spotfireServiceClient.CreateFolder(
                            coeDataView.DataViewID.ToString(CultureInfo.InvariantCulture),
                            instanceDataviewRootId);

                    foreach (var table in coeDataView.Tables)
                    {
                        var tableFolderId =
                            spotfireServiceClient.CreateFolder(
                                table.Id.ToString(CultureInfo.InvariantCulture),
                                dataviewFolderId);

                        // Create or find the exsiting data source from Spotfire server
                        var database = ConfigurationUtilities.GetDatabaseData(table.Database, true);

                        if (database == null)
                        {
                            throw new ApplicationException(string.Format("Database '{0}' is not found in configuration file.", table.Database));
                        }
                        if (database.InstanceData == null)
                        {
                            throw new ApplicationException(string.Format("Data source for database '{0}' is not found in configuration file", table.Database));
                        }

                        var instance = database.InstanceData;

                        var globalDbName = instance.IsCBOEInstance ? instance.DatabaseGlobalUser : instance.Name + "." + instance.DatabaseGlobalUser;
                        var globalDatabase = ConfigurationUtilities.GetDatabaseData(globalDbName, true);

                        if (globalDatabase == null)
                        {
                            throw new ApplicationException(string.Format("Global database for data source '{0}' is not found in configuration file.", instance.InstanceName));
                        }

                        var datasourceId = spotfireServiceClient.CreateDataSource(
                            rootFolderId,
                            string.Format(instanceDatasourcePathPattern, instance.InstanceName.Trim().ToUpper(), instance.Id),
                            instance.HostName,
                            instance.Port,
                            instance.SID,
                            instance.DatabaseGlobalUser,
                            DecryptPassword(globalDatabase.Password));

                        var publishingError = PublishTableFields(spotfireServiceClient, datasourceId, tableFolderId, database, dataview, table);

                        if (!string.IsNullOrEmpty(publishingError))
                        {
                            throw new System.ApplicationException(publishingError);
                        }
                    }
                }
            });

            return errorMsg;
        }

        /// <summary>
        /// Delete data view.
        /// </summary>
        /// <param name="dataview"> The dataview. </param>
        /// <returns> The <see cref="string"/>. </returns>
        public static string DeleteDataView(COEDataViewBO dataview)
        {
            var spotfire = COESpotFireSettingsBO.Get();
            string spotfireUrl = spotfire.SpotfireURL;
            string userName = spotfire.SpotfireUser;
            string password = DecryptPassword(spotfire.SpotfirePassword);

            var errorMsg = ActionWithExceptionHandling(() =>
            {
                // Delete dataview in Spotfire server
                using (var spotfireServiceClient = new SpotfireServiceClient(spotfireUrl, userName, password))
                {
                    var defaultInstance = ConfigurationUtilities.GetMainInstance();
                    var rootFolderId = spotfireServiceClient.GetDatalytixRootFolder();
                    var instanceDataviewRootId = spotfireServiceClient.CreateFolder(
                        string.Format(dataviewFolderPattern, defaultInstance.Id), rootFolderId);

                    var coeDataView = dataview.COEDataView;
                    var dataviewFolderId = spotfireServiceClient.CreateFolder(
                            coeDataView.DataViewID.ToString(CultureInfo.InvariantCulture),
                            instanceDataviewRootId);

                    spotfireServiceClient.DeleteFolder(dataviewFolderId);
                }
            });

            return errorMsg;
        }

        #endregion

        /// <summary>
        /// Dispose service clients
        /// </summary>
        public void Dispose()
        {
            ServicePointManager.ServerCertificateValidationCallback -= ServerCertificateValidationCallback;

            try
            {
                if (spotfireElementManagerServiceClient != null && spotfireElementManagerServiceClient.State != CommunicationState.Faulted)
                {
                    spotfireElementManagerServiceClient.Close();
                }
            }
            finally
            {
                if (spotfireElementManagerServiceClient != null && spotfireElementManagerServiceClient.State != CommunicationState.Closed)
                {
                    spotfireElementManagerServiceClient.Abort();
                }
            } 

            try
            {
                if (spotfireLibraryServiceClient != null && spotfireLibraryServiceClient.State != CommunicationState.Faulted)
                {
                    spotfireLibraryServiceClient.Close();
                }
            }
            finally
            {
                if (spotfireLibraryServiceClient != null && spotfireLibraryServiceClient.State != CommunicationState.Closed)
                {
                    spotfireLibraryServiceClient.Abort();
                }
            }
        }

        #region APIs

        /// <summary>
        /// Create Spotfire library item folder. If the folder is existed, will not create the new.
        /// </summary>
        /// <param name="name">The folder name.</param>
        /// <param name="parentId">The parent item id.</param>
        /// <param name="description">The description.</param>
        /// <returns>The created or existed folder id.</returns>
        public string CreateFolder(string name, string parentId = null, string description = "")
        {
            string parentPath = "/";
            ItemType[] types = spotfireLibraryServiceClient.loadTypes(new[] { "spotfire.folder" });

            if (parentId == null)
            {
                parentId = spotfireLibraryServiceClient.pathToGuid(parentPath, types[0]);
            }
            else
            {
                parentPath = spotfireLibraryServiceClient.guidToPath(parentId);
            }

            // Check folder exist in current parent folder
            var folderId = spotfireLibraryServiceClient.pathToGuid(parentPath + "/" + name, types[0]);
            if (folderId != null)
            {
                return folderId;
            }

            var folder = new Folder { parentId = parentId, name = name, description = description };
            folder = spotfireElementManagerServiceClient.addElement(folder, false) as Folder;

            return folder != null ? folder.id : null;
        }

        /// <summary>
        /// Get Datalytix root folder in Spotfire library
        /// </summary>
        /// <returns>The Datalytix root folder id.</returns>
        public string GetDatalytixRootFolder()
        {
            return CreateFolder("%Datalytix%", null, "Do not modify this folder manually");
        }

        /// <summary>
        /// Delete specific folder from Spotfire library.
        /// </summary>
        /// <param name="folderId">The folder id.</param>
        public void DeleteFolder(string folderId)
        {
            spotfireLibraryServiceClient.deleteItems(new[] { folderId });
        }

        /// <summary>
        /// Create Spotfire data source item
        /// </summary>
        /// <param name="folderId">The folder id.</param>
        /// <param name="instanceName">The database instance name.</param>
        /// <param name="serverUrl">The database server url.</param>
        /// <param name="port">The database server port.</param>
        /// <param name="sid">The database sid.</param>
        /// <param name="userName">The database user name.</param>
        /// <param name="password">The database password.</param>
        /// <param name="updateExisting">
        /// The value whether update the existing data source item in library, default value is False..
        /// </param>
        /// <returns>The created data source id.</returns>
        public string CreateDataSource(
            string folderId,
            string instanceName,
            string serverUrl,
            int port,
            string sid,
            string userName,
            string password,
            bool updateExisting = false,
            DriverType driverType = DriverType.Oracle)
        {
            var folder = spotfireElementManagerServiceClient.loadElement(folderId) as Folder;

            if (folder == null)
            {
                throw new ArgumentException(string.Format("Cannot retrieve folder via {0}", folderId), "folderId");
            }

            // Check datasource exist in current parent folder
            ItemType[] types = spotfireLibraryServiceClient.loadTypes(new[] { "spotfire.datasource" });
            var parentPath = spotfireLibraryServiceClient.guidToPath(folderId);
            var datasourceId = spotfireLibraryServiceClient.pathToGuid(parentPath + "/" + instanceName, types[0]);
            var connectionUrl = string.Format(GetConnectionUrlPattern(driverType), serverUrl, port, sid);
            var pingCmd = "SELECT 1 FROM DUAL";
            
            // Call USE_PASSWORD for JChem cartridge in Connection Initialization
            var initialCmd = string.Format("SELECT DATASOURCE_INITIALIZATION('{0}') FROM dual", password);

            // Data source existing.
            if (!string.IsNullOrEmpty(datasourceId))
            {
                // Update existing data source.
                if (updateExisting)
                {
                    var existingDataSource = spotfireElementManagerServiceClient.loadElement(datasourceId) as DataSource;
                    existingDataSource.connectionUrl = connectionUrl;
                    existingDataSource.userName = userName;
                    existingDataSource.password = password;
                    existingDataSource.pingCommand = pingCmd;
                    existingDataSource.dataSourceType = CambridgeSoft.COE.Framework.Common.Utility.COEConvert.ToString(driverType);
                    existingDataSource.initializationCommands = new[] { initialCmd };

                    var dataSource = spotfireElementManagerServiceClient.updateElement(existingDataSource, false);

                    if (dataSource == null ||
                        !datasourceId.Equals(dataSource.id, StringComparison.InvariantCultureIgnoreCase))
                    {
                        throw new ApplicationException("Data source updated but the id changed, this will cause the dataviews refered to this data source become non-queryable");
                    }

                    return datasourceId;
                }
            }
            else
            {
                // Create new data source.
                var newDataSource = new DataSource
                {
                    parentId = folder.id,
                    name = instanceName,
                    path = folder.path,
                    dataSourceType = CambridgeSoft.COE.Framework.Common.Utility.COEConvert.ToString(driverType),
                    writeAllowed = false,
                    fetchSize = 10000,
                    batchSize = 100,
                    dataSourceAuthentication = false,
                    connectionUrl = connectionUrl,
                    userName = userName,
                    password = password,
                    maxConnections = 30,
                    minConnections = 1,
                    pingCommand = pingCmd,
                    credentialsTimeout = 86400,
                    initializationCommands = new[] { initialCmd }
                };

                var dataSource = spotfireElementManagerServiceClient.addElement(newDataSource, false) as DataSource;
                datasourceId = dataSource != null ? dataSource.id : null;
            }

            return datasourceId;
        }

        private string GetConnectionUrlPattern(DriverType driverType)
        {
            var connectionUrlPattern = string.Empty;

            switch (driverType)
            {
                case DriverType.OracleDataDirect:
                    connectionUrlPattern = "jdbc:tibcosoftwareinc:oracle://{0}:{1};SID={2}";
                    break;
                    // Default connection url pattern is Oracle driver pattern.
                case DriverType.Oracle:
                default:
                    connectionUrlPattern = "jdbc:oracle:thin:@{0}:{1}:{2}";
                    break;
            }

            return connectionUrlPattern;
        }

        /// <summary> Delete data source from Spotfire server.
        /// </summary>
        /// <param name="folderId"> The parent folder id. </param>
        /// <param name="instanceName"> The instance name. </param>
        public void DeleteDataSource(string folderId, string instanceName)
        {
            ItemType[] types = spotfireLibraryServiceClient.loadTypes(new[] { "spotfire.datasource" });
            var parentPath = spotfireLibraryServiceClient.guidToPath(folderId);
            var datasourceId = spotfireLibraryServiceClient.pathToGuid(parentPath + "/" + instanceName, types[0]);
            spotfireLibraryServiceClient.deleteItems(new[] { datasourceId });
        }

        /// <summary>
        /// Create Spotfire column item
        /// </summary>
        /// <param name="dataSourceId">The data source item id.</param>
        /// <param name="folderId">The folder item id.</param>
        /// <param name="schema">The database schema.</param>
        /// <param name="table">The data table.</param>
        /// <param name="databaseColumnId">the database column identity.</param>
        /// <param name="dataType">The data type.</param>
        /// <param name="alias">The alias.</param>
        /// <param name="calculation">The calculation expression. Default value is "%1"</param>
        /// <param name="updateExisting">
        /// The value whether update the existing column item in library, default value is False..
        /// </param>
        /// <param name="properties">The properties will be set for the column</param>
        /// <returns>The created column item id.</returns>
        public string CreateColumn(
            string dataSourceId,
            string folderId,
            string schema,
            string table,
            string databaseColumnId,
            string dataType,
            string alias,
            string calculation = "%1",
            bool updateExisting = false,
            IDictionary<string,string> properties=null)
        {
            var dataSource = spotfireElementManagerServiceClient.loadElement(dataSourceId) as DataSource;

            if (dataSource == null)
            {
                throw new ArgumentException(string.Format("Cannot retrieve dataSource via {0}", dataSourceId), "dataSourceId");
            }

            // Check column exist in current parent folder
            ItemType[] types = spotfireLibraryServiceClient.loadTypes(new[] { "spotfire.column" });
            var parentPath = spotfireLibraryServiceClient.guidToPath(folderId);
            var columnId = spotfireLibraryServiceClient.pathToGuid(parentPath + "/" + alias, types[0]);
            if (columnId != null)
            {
                if (updateExisting)
                {
                    spotfireLibraryServiceClient.deleteItems(new[] { columnId });
                }
                else
                {
                    return columnId;
                }
            }

            var column = new Column
            {
                name = alias,
                parentId = folderId,
                calculation = calculation,
                dataType = dataType,
                drillable = false
            };

            // Set the property for column.
            if (properties != null && properties.Count > 0)
            {
                column.properties = properties.Select(p => new Property { key = p.Key, value = p.Value }).ToArray();
            }

            var dataSourceColumnElementPath = new DataSourceElementPath
            {
                dataSourceId = dataSource.id,
                dataSourceName = dataSource.name,
                schema = schema,
                table = table,
                column = databaseColumnId
            };

            column.sourceColumns = new[] { dataSourceColumnElementPath };
            column = spotfireElementManagerServiceClient.addElement(column, false) as Column;

            return column != null ? column.id : null;
        }

        public string CreateJoin(int fieldId, String fieldName, int lookupFieldId, String lookupFieldName, String tableName, String schema, String dataSourceId, String tableFolderId, String lookupSchema, String lookupTableName, bool updateExisting = false)
        {

            var dataSource = spotfireElementManagerServiceClient.loadElement(dataSourceId) as DataSource;
            
            if (dataSource == null)
            {
                throw new ArgumentException(string.Format("Cannot retrieve dataSource via {0}", dataSourceId), "dataSourceId");
            }

            string joinName = fieldId + "-" + lookupFieldId;

            // Check column exist in current parent folder
            ItemType[] types = spotfireLibraryServiceClient.loadTypes(new[] { "spotfire.join" });
            var parentPath = spotfireLibraryServiceClient.guidToPath(tableFolderId);
            var joinId = spotfireLibraryServiceClient.pathToGuid(parentPath + "/" + joinName, types[0]);
            if (joinId != null)
            {
                if (updateExisting)
                {
                    spotfireLibraryServiceClient.deleteItems(new[] { joinId });
                }
                else
                {
                    return joinId;
                }
            }

            Join join = new Join();
            join.parentId = tableFolderId;
            join.name = joinName;
            join.joinCondition = new JoinCondition();
            join.joinCondition.joinType = "LeftOuterJoin";
            join.joinCondition.joinColumns = new DataSourceElementPath[2];

            DataSourceElementPath columnToReplace = new DataSourceElementPath();
            DataSourceElementPath columLookup = new DataSourceElementPath();

            columnToReplace.dataSourceId = dataSource.id;
            columnToReplace.dataSourceName = dataSource.name;
            columnToReplace.schema = schema;
            columnToReplace.table = tableName;
            columnToReplace.column = fieldName;

            columLookup.dataSourceId = dataSource.id;
            columLookup.dataSourceName = dataSource.name;
            
            columLookup.schema = lookupSchema;
            columLookup.table = lookupTableName;
            columLookup.column = lookupFieldName;

            join.joinCondition.joinColumns[0] = columnToReplace;
            join.joinCondition.joinColumns[1] = columLookup;

            DataSourceElementPath tableToReplace = new DataSourceElementPath();
            DataSourceElementPath tablelookup = new DataSourceElementPath();

            tableToReplace.dataSourceId = columnToReplace.dataSourceId;
            tableToReplace.dataSourceName = columnToReplace.dataSourceName;
            tableToReplace.schema = columnToReplace.schema;
            tableToReplace.table = columnToReplace.table;

            tablelookup.dataSourceId = columLookup.dataSourceId;
            tablelookup.dataSourceName = columLookup.dataSourceName;
            tablelookup.schema = columLookup.schema;
            tablelookup.table = columLookup.table;

            join.targetTables = new DataSourceElementPath[2];
            join.targetTables[0] = tableToReplace;
            join.targetTables[1] = tablelookup;

            join = (Join)spotfireElementManagerServiceClient.addElement(join, false);
            return join.id;
        }
		
		#endregion

        #region private static methods

        private static string GetPassword(COEInstanceBO publishInstance)
        {
            var db = publishInstance.IsCBOEInstance ?
                ConfigurationUtilities.GetDatabaseData(publishInstance.DatabaseGlobalUser.ToUpper()) :
                ConfigurationUtilities.GetDatabaseData(publishInstance.InstanceName.ToUpper() + "." + publishInstance.DatabaseGlobalUser.ToUpper());

            return DecryptPassword(db.Password);
        }

        private static string DecryptPassword(string password)
        {
            return Utilities.IsRijndaelEncrypted(password) ? Utilities.DecryptRijndael(password) : password;
        }

        private static string ActionWithExceptionHandling(Action action, bool logException = true)
        {
            var errorMsg = string.Empty;
            var errorMsgWillLog = string.Empty;

            try
            {
                action();
            }
            catch (MessageSecurityException securityEx)
            {
                errorMsg = "Invalid username or password for Spotfire server";
                errorMsgWillLog = securityEx.Message;
            }
            catch (EndpointNotFoundException endpointNotFoundEx)
            {
                errorMsg = "Spotfire service cannot be reached. please check Spotfire service address.";
                errorMsgWillLog = endpointNotFoundEx.Message;
            }
            catch (FaultException<Manager.SpotfireElementManagerService.IMFaultInfo> faultEx)
            {
                if (faultEx.Message.Contains("Error(s) found during validation"))
                {
                    errorMsg = "Validation error, this maybe caused by invalid driver type or invalid data source username and password.";
                }
                else if (faultEx.Message.Contains("Conflicting name"))
                {
                    errorMsg = "Duplicate columns in dataview:" + faultEx.Message.Substring(16);
                }
                else
                {
                    errorMsg = "Unknown exception happend while calling Spotfire server. Please contact with Spotfire server administrator.";
                }

                errorMsgWillLog = faultEx.Message;
            }
            catch (SecurityNegotiationException negotiationEx)
            {
                errorMsg = "It is failed to validate the server certficate, please install the certificate or ignore the certification error by configuration setting";
                errorMsgWillLog = negotiationEx.Message;
            }
            catch (CommunicationException communicationEx)
            {
                errorMsg = "Error happend while communicating with Spotfire server, please try it later.";
                errorMsgWillLog = communicationEx.Message;
            }
            catch (ApplicationException appEx)
            {
                errorMsg = appEx.Message;
                errorMsgWillLog = appEx.Message;
            }
            catch (TimeoutException timeoutEx)
            {
                errorMsg = "Spotfire service timeout, please try it later.";
                errorMsgWillLog = timeoutEx.Message;
            }
            catch (Exception ex)
            {
                errorMsg = "Unknown exception happend while calling Spotfire server. Please contact with Spotfire server administrator.";
                errorMsgWillLog = ex.Message;
            }

            if (logException)
            {
                Utility.WriteToAppLog(CambridgeSoft.COE.Framework.GUIShell.GUIShellTypes.LogsCategories.Error, errorMsgWillLog);
            }

            return errorMsg;
        }

        private static string PublishTableFields(
            SpotfireServiceClient spotfireServiceClient,
            string datasourceId,
            string tableFolderId,
            DatabaseData database,
            COEDataViewBO dataview,
            COEDataView.DataViewTable table)
        {
            // Create a task list to execute creating fields asynchronously
            var tasks = new List<Task>();
            var exceptionMsgList = new List<string>();

            // Create columns
            foreach (var field in table.Fields)
            {
                // Avoid the closure.
                int fieldId = field.Id;
                string fieldName = field.Name;
                string fieldType = field.DataType.ToString();
                
                if (fieldType == COEDataView.AbstractTypes.Text.ToString())
                {
                    fieldType = "String";
                }
                else if (fieldType == COEDataView.AbstractTypes.Integer.ToString())
                {
                    fieldType = "LongInteger";
                }
                else if (fieldType == COEDataView.AbstractTypes.Date.ToString())
                {
                    fieldType = "DateTime";
                }

                // Set the content type for structure field.
                var contentProperty = new Dictionary<string, string>();
                if (COEDataView.IsStructureContentType(field.MimeType))
                {
                    contentProperty.Add("ContentType", GetMimeTypeFromDVMimeType(field.MimeType));

                    // direct cartridge structure need to be converted to molfile its data type is string not blob or clob, if user sets it, when publish to spotfire
                    // server, those column return data type need to be set to string.
                    if (field.IndexType == COEDataView.IndexTypes.DIRECT_CARTRIDGE && field.MimeType == COEDataView.MimeTypes.CHEMICAL_X_DATADIRECT_CTAB)
                    {
                        // this value return data type is string, field type need to be as string
                        fieldType = "String";
                    }
                }

                tasks.Add(Task.Factory.StartNew(() =>
                {
                    var errorMsg = ActionWithExceptionHandling(() =>
                    {
                        spotfireServiceClient.CreateColumn(
                            datasourceId,
                            tableFolderId,
                            database.Owner,
                            table.Name,
                            fieldName,
                            fieldType,
                            fieldId.ToString(),
                            "%1",
                            true,
                            contentProperty);
                    }, false);

                    if (!string.IsNullOrEmpty(errorMsg))
                    {
                        exceptionMsgList.Add(errorMsg);
                    }
                }));

                if (field.IndexType != COEDataView.IndexTypes.NONE)
                {
                    string formulaExpression = "%1", molWeightExpression = "%1";

                    switch (field.IndexType)
                    {
                        case COEDataView.IndexTypes.CS_CARTRIDGE:
                            formulaExpression = "CSCARTRIDGE.Formula(%1, '')";
                            molWeightExpression = "CSCARTRIDGE.MolWeight(%1)";
                            break;
                        case COEDataView.IndexTypes.DIRECT_CARTRIDGE:
                            formulaExpression = "mdlaux.molfmla(NULL, %1)";
                            molWeightExpression = "mdlaux.molwt(NULL, %1)";
                            break;
                        case COEDataView.IndexTypes.JCHEM_CARTRIDGE:
                            formulaExpression = "jc_formula(%1)";
                            molWeightExpression = "jc_molweight(%1)";
                            break;
                    }

                    tasks.Add(Task.Factory.StartNew(() =>
                    {
                        var errorMsg = ActionWithExceptionHandling(() =>
                        {
                            spotfireServiceClient.CreateColumn(
                                datasourceId,
                                tableFolderId,
                                database.Owner,
                                table.Name,
                                fieldName,
                                "String",
                                fieldId + "_formula",
                                formulaExpression,
                                true);
                        }, false);

                        if (!string.IsNullOrEmpty(errorMsg))
                        {
                            exceptionMsgList.Add(errorMsg);
                        }
                    }));

                    tasks.Add(Task.Factory.StartNew(() =>
                    {
                        var errorMsg = ActionWithExceptionHandling(() =>
                        {
                            spotfireServiceClient.CreateColumn(
                                datasourceId,
                                tableFolderId,
                                database.Owner,
                                table.Name,
                                fieldName,
                                "Real",
                                fieldId + "_molweight",
                                molWeightExpression,
                                true);
                        }, false);

                        if (!string.IsNullOrEmpty(errorMsg))
                        {
                            exceptionMsgList.Add(errorMsg);
                        }
                    }));
                }

                if (field.LookupFieldId > 0)
                {
                    COEDataView.Field dataViewLookupField = dataview.COEDataView.GetFieldById(field.LookupFieldId);
                    COEDataView.DataViewTable lookupTable = dataview.COEDataView.Tables.getById(dataViewLookupField.ParentTableId);
                    String lookupSchema = ConfigurationUtilities.GetDatabaseData(lookupTable.Database, true).Owner;

                    int lookupFieldId = field.LookupFieldId;
                    String lookupFieldName = dataViewLookupField.Name;
                    String tableName = table.Name;
                    String schema = database.Owner;

                    tasks.Add(Task.Factory.StartNew(() =>
                    {
                        var errorMsg = ActionWithExceptionHandling(() =>
                        {
                            spotfireServiceClient.CreateJoin(fieldId,
                                fieldName,
                                lookupFieldId,
                                lookupFieldName,
                                tableName,
                                schema,
                                datasourceId,
                                tableFolderId,
                                lookupSchema,
                                lookupTable.Name,
                                true);
                        }, false);

                        if (!string.IsNullOrEmpty(errorMsg))
                        {
                            exceptionMsgList.Add(errorMsg);
                        }
                    }));
                }
            }

            Task.WaitAll(tasks.ToArray());

            if (exceptionMsgList.Count > 0)
            {
                return string.Join(";", exceptionMsgList.Distinct().ToArray());
            }

            return string.Empty;
        }

        /// <summary>
        /// Translates COEDataView Mime type enumeration values to valid chemical mime type string
        /// </summary>
        /// <param name="dataViewMimeType">The COEDataView.Mime Types enumerate value to be translated</param>
        /// <returns>A supported chemical mime type string or blank</returns>
        private static string GetMimeTypeFromDVMimeType(COEDataView.MimeTypes dataViewMimeType)
        {
            switch (dataViewMimeType)
            {
                case COEDataView.MimeTypes.CHEMICAL_X_CDX:
                    return "chemical/x-cdx";
                case COEDataView.MimeTypes.CHEMICAL_X_MDLMOLFILE:
                case COEDataView.MimeTypes.CHEMICAL_X_DATADIRECT_CTAB:
                    return "chemical/x-mdl-molfile";
                case COEDataView.MimeTypes.CHEMICAL_X_SMILES:
                    return "chemical/x-daylight-smiles";
                case COEDataView.MimeTypes.CHEMICAL_X_MDL_CHIME:
                    return "chemical/x-mdl-chime";
                case COEDataView.MimeTypes.CHEMICAL_X_CML:
                    return "chemical/x-cml";
                default:
                    return string.Empty;
            }
        }

        #endregion

        #region initialize service clients

        private void InitializeElementManagerServiceClient()
        {
            var basicHttpbinding = new BasicHttpBinding
            {
                Name = "ElementManagerServiceSoapBinding",
                Security =
                {
                    Mode = this.elementManagerServiceAddress.StartsWith("https", StringComparison.InvariantCultureIgnoreCase) ? BasicHttpSecurityMode.Transport : BasicHttpSecurityMode.TransportCredentialOnly,
                    Transport = { ClientCredentialType = HttpClientCredentialType.Basic }
                },
                MaxReceivedMessageSize = 10000000
            };

            WebRequest.RegisterPrefix(this.elementManagerServiceAddress, new WebRequestCreater());
            var endpointAddress = new EndpointAddress(this.elementManagerServiceAddress);

            this.spotfireElementManagerServiceClient = new ElementManagerServiceClient(basicHttpbinding, endpointAddress);
            this.spotfireElementManagerServiceClient.ClientCredentials.UserName.UserName = userName;
            this.spotfireElementManagerServiceClient.ClientCredentials.UserName.Password = password;

            using (var scope = new OperationContextScope(spotfireElementManagerServiceClient.InnerChannel))
            {
                var httpRequestProperty = new HttpRequestMessageProperty();
                httpRequestProperty.Headers[HttpRequestHeader.Authorization] =
                    "Basic " + Convert.ToBase64String(Encoding.Unicode.GetBytes(userName + ":" + password));
                OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = httpRequestProperty;
            }
        }

        private bool ServerCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            // Good certification
            if (sslPolicyErrors == SslPolicyErrors.None)
            {
                return true;
            }

            // Ignore the certification error.
            if (string.Compare(ConfigurationManager.AppSettings["IgnoreSpotfireServerCertValidationError"], "true", true) == 0)
            {
                return true;
            }

            return false;
        }

        private void InitializeLibraryServiceClient()
        { 
            var basicHttpbinding = new BasicHttpBinding
            {
                Name = "LibraryServiceSoapBinding",
                Security =
                {
                    Mode = this.libraryServiceAddress.StartsWith("https", StringComparison.InvariantCultureIgnoreCase) ? BasicHttpSecurityMode.Transport : BasicHttpSecurityMode.TransportCredentialOnly,
                    Transport = { ClientCredentialType = HttpClientCredentialType.Basic }
                },
                MaxReceivedMessageSize = 10000000
            };

            WebRequest.RegisterPrefix(this.libraryServiceAddress, new WebRequestCreater());
            var endpointAddress = new EndpointAddress(this.libraryServiceAddress);

            this.spotfireLibraryServiceClient = new LibraryServiceClient(basicHttpbinding, endpointAddress);
            this.spotfireLibraryServiceClient.ClientCredentials.UserName.UserName = userName;
            this.spotfireLibraryServiceClient.ClientCredentials.UserName.Password = password;

            using (var scope = new OperationContextScope(spotfireLibraryServiceClient.InnerChannel))
            {
                var httpRequestProperty = new HttpRequestMessageProperty();
                httpRequestProperty.Headers[HttpRequestHeader.Authorization] =
                    "Basic " + Convert.ToBase64String(Encoding.Unicode.GetBytes(userName + ":" + password));
                OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = httpRequestProperty;
            }
        } 

        #endregion 

        /// <summary>
        /// TODO: Update summary.
        /// </summary>
        private class WebRequestCreater : IWebRequestCreate
        {
            public WebRequest Create(Uri uri)
            {
                return WebRequest.CreateDefault(uri);
            }
        }
    }
}
