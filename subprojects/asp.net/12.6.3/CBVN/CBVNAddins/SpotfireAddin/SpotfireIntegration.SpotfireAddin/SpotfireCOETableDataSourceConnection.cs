using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Spotfire.Dxp.Data;
using System.Data;
using Spotfire.Dxp.Data.Import;
using Spotfire.Dxp.Application;
using COEServiceLib;
using Spotfire.Dxp.Framework.ApplicationModel;
using CambridgeSoft.COE.Framework.COEDataViewService;
using CambridgeSoft.COE.Framework.Common;

namespace SpotfireIntegration.SpotfireAddin
{
    class SpotfireCOETableDataSourceConnection : DataSourceConnection
    {
        private readonly IEnumerable<object> promptModels;

        private DataSet dataSet = null;

        bool isDataViewExists = true;

        public SpotfireCOETableDataSourceConnection(SpotfireCOETableDataSource dataSource, IServiceProvider serviceProvider, IEnumerable<object> promptModels)
            : base(dataSource, serviceProvider)
        {
            this.promptModels = promptModels;
        }

        protected override IEnumerable<object> GetPromptModelsCore()
        {
            return this.promptModels;
        }

        protected override DataRowReader ExecuteQueryCore2()
        {
            DataRowReader reader = null;
            try
            {
                if (this.dataSet == null)
                {
                    this.dataSet = this.GetDataSet();
                }

                //Coverity Bug Fix CID 10853 (Local Analysis)
                if (this.dataSet != null)
                {
                    reader = DataRowReader.CreateReader(this.dataSet.CreateDataReader());
                    SpotfireCOETableDataSource dataSource = this.DataSource as SpotfireCOETableDataSource;
                    if (dataSource != null && reader != null && isDataViewExists)
                    {
                        COEMetadataTransformation transformation = new COEMetadataTransformation(dataSource.HitList, dataSource.TableID, dataSource.FilterChildHits);
                        AnalysisApplication application = (AnalysisApplication)this.ServiceProvider.GetService(typeof(AnalysisApplication));
                        DataTransformationConnection connection = transformation.Connect(application.ImportContext, reader);
                        reader = connection.ExecuteTransformation();
                    }
                }
            }
            catch
            {
                throw;
            }
            return reader;
        }

        private DataSet GetDataSet()
        {
            SpotfireCOETableDataSource dataSource = this.DataSource as SpotfireCOETableDataSource;
            COEService service = this.ServiceProvider.GetService(typeof(COEService)) as COEService;
            RemoveDeletedTablesFromResultsCriteria();
            if (!isDataViewExists)
            {
                DataSet childDataSet = new DataSet(dataSource.TableName);
                System.Data.DataTable copiedTable = childDataSet.Tables.Add(dataSource.TableName);
                return childDataSet;
            }
            try
            {
                //Coverity Bug Fix : CID 12917 
                if (dataSource != null && service != null)
                {
                    //check if hitlist id is present then get the dataset associated with the hit list id.
                    if (dataSource.HitListId > 0)
                    {
                        return service.GetDataSet(dataSource.HitList, dataSource.HitListId, dataSource.FilterChildHits
                            );
                    }
                    else if (dataSource.IsBaseTable)
                    {
                        return service.GetDataSet(dataSource.HitList, dataSource.FilterChildHits);
                    }
                    else
                    {
                        return service.GetDataSet(dataSource.HitList, dataSource.TableName, dataSource.TableID, dataSource.FilterChildHits);
                    }
                }
            }
            catch (System.Security.SecurityException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
            return null;
        }

        /// <summary>
        /// Removes the invalid/dropped tables in dataview from result criteria to avoid exceptions while loading the dxp file
        /// </summary>
        void RemoveDeletedTablesFromResultsCriteria()
        {
            SpotfireCOETableDataSource dataSource = this.DataSource as SpotfireCOETableDataSource;
            COEService service = this.ServiceProvider.GetService(typeof(COEService)) as COEService;
            TableNameIdMapper theTableNameIdMapper = TableNameIdMapper.Instance();
            service.ExecuteUsingCOEPrincipal(delegate()
            {
                COEDataViewBO dataviewBO = null;
                try
                {
                    //get dataview object from COEService
                    dataviewBO = service.LoadDataViewBO(dataSource.HitList);
                }
                catch(System.Security.SecurityException sEx)
                {
                    NotificationServiceHelper theNotificationServiceHelper = NotificationServiceHelper.Instance(this.ServiceProvider, string.Empty);
                    theNotificationServiceHelper.AddNotification("Dataview not found error", "Dataview not found",
                           sEx.Message, NotificationServiceHelper.NotificationType.Error);
                }
                //if dataview is null then throw dataview not exists exception
                if(dataviewBO == null)
                {
                    isDataViewExists = false;
                }
                else
                {
                    Dictionary<int, string> tableNameIdCollection = new Dictionary<int, string>();
                    //get the tables present in the dataview
                    TableListBO tablesCollection = dataviewBO.DataViewManager.Tables;
                    foreach (TableBO tableBO in tablesCollection)
                    {
                        tableNameIdCollection.Add(tableBO.ID, tableBO.Alias);
                    }
                    ResultsCriteria resultCriteria = dataSource.HitList.ResultsCriteria;
                    //get the tables missing from dataview
                    var missingTablesInDataview = theTableNameIdMapper.TableNameIdCollectionFromFile.Except(tableNameIdCollection, new TableNameIdComparer());
                    foreach (KeyValuePair<int, string> keyValPair in missingTablesInDataview)
                    {
                        //try to find the result criteria for missing tables
                        ResultsCriteria.ResultsCriteriaTable resultCriteriaTable = resultCriteria.Tables.FirstOrDefault(p => p.Id == keyValPair.Key);
                        if (resultCriteriaTable != null)
                        {
                            //remove the result criteria for missing tables and add it to notification
                            resultCriteria.Tables.Remove(resultCriteriaTable);
                            NotificationServiceHelper theNotificationServiceHelper = NotificationServiceHelper.Instance(this.ServiceProvider, dataviewBO.Name);
                            theNotificationServiceHelper.AddNotification("Table or field not present in dataview", "Table or field not present in dataview",
                                   string.Format("Table {0} not exists in dataview {1}", keyValPair.Value, dataviewBO.Name), NotificationServiceHelper.NotificationType.Error);
                        }
                    }
                    dataSource.HitList.ResultsCriteria = resultCriteria;
                }
            });
        }

        /// <summary>
        /// Compares the key value pair for tables from dataview
        /// </summary>
        class TableNameIdComparer : IEqualityComparer<KeyValuePair<int, string>>
        {
            /// <summary>
            /// Compares the key value pair
            /// </summary>
            /// <param name="x">table id value</param>
            /// <param name="y">table name value</param>
            /// <returns>returns true if table exists; otherwise false</returns>
            public bool Equals(KeyValuePair<int, string> x, KeyValuePair<int, string> y)
            {
                if (x.Key == y.Key || x.Value == y.Value)
                {
                    return true;
                }
                return false;
            }

            public int GetHashCode(KeyValuePair<int, string> obj)
            {
                return obj.GetHashCode();
            }
        }
    }

    /// <summary>
    /// Class for notification service
    /// </summary>
    class NotificationServiceHelper
    {
        #region Variables
        string _dataviewName;
        IServiceProvider _serviceProvider;
        static object lockObject = new object();
        static NotificationServiceHelper theNotificationServiceHelper;
        NotificationService _notitificationService;
        List<string> detailsMessageStrings;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes an instance of the NotificationServiceHelper class
        /// </summary>
        /// <param name="serviceProvider">IServiceProvider object</param>
        /// <param name="dataviewName">dataview name</param>
        private NotificationServiceHelper(IServiceProvider serviceProvider, string dataviewName)
        {
            detailsMessageStrings = new List<string>();
            this._serviceProvider = serviceProvider;
            this._dataviewName = dataviewName;
            this._notitificationService = (NotificationService)serviceProvider.GetService(typeof(NotificationService));
        }
        #endregion

        #region Methods
        /// <summary>
        /// Initializes an instanc eof NotificationServiceHelper class using singleton pattern
        /// </summary>
        /// <param name="serviceProvider">IServiceProvider object</param>
        /// <param name="dataviewName">dataview name</param>
        /// <returns>returns object of NotificationServiceHelper</returns>
        public static NotificationServiceHelper Instance(IServiceProvider serviceProvider, string dataviewName)
        {
            lock (lockObject)
            {
                if (theNotificationServiceHelper == null)
                {
                    theNotificationServiceHelper = new NotificationServiceHelper(serviceProvider, dataviewName);
                }
                return theNotificationServiceHelper;
            }
        }

        /// <summary>
        /// Adds the notification message to notification service
        /// </summary>
        /// <param name="title">title of message</param>
        /// <param name="message">message summary string</param>
        /// <param name="details">mesage details</param>
        /// <param name="notificationType">notification service type</param>
        public void AddNotification(string title, string summary, string details, NotificationType notificationType)
        {
            if (!isDetailsAlreadyAdded(details))
            {
                switch (notificationType)
                {
                    case NotificationType.Information:
                        _notitificationService.AddInformationNotification(title, summary, details);
                        break;
                    case NotificationType.Warning:
                        _notitificationService.AddWarningNotification(title, summary, details);
                        break;
                    case NotificationType.Error:
                        _notitificationService.AddErrorNotification(title, summary, details);
                        break;
                }
            }
        }

        bool isDetailsAlreadyAdded(string details)
        {
            if (!detailsMessageStrings.Contains(details))
            {
                detailsMessageStrings.Add(details);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Clears the notifications from queue
        /// </summary>
        public static void ClearNotifications()
        {
            if (theNotificationServiceHelper != null)
            {
                theNotificationServiceHelper.ClearAllNotifications();
            }
        }

        void ClearAllNotifications()
        {
            detailsMessageStrings.Clear();
        }
        #endregion

        /// <summary>
        /// Enum for notification type
        /// </summary>
        public enum NotificationType
        {
            Information,
            Error,
            Warning
        }
    }
}
