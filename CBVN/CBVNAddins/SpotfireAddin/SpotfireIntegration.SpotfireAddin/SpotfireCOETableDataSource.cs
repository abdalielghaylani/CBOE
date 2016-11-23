namespace SpotfireIntegration.SpotfireAddin
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using Spotfire.Dxp.Application;
    using Spotfire.Dxp.Application.Extension;
    using Spotfire.Dxp.Data;
    using Spotfire.Dxp.Data.Exceptions;
    using Spotfire.Dxp.Framework.ApplicationModel;
    using Spotfire.Dxp.Framework.Persistence;
    using SpotfireIntegration.Common;
    using COEServiceLib;
    using Spotfire.Dxp.Framework.Preferences;

    [Serializable]
    [PersistenceVersion(1, 0)]
    public class SpotfireCOETableDataSource : CustomDataSource
    {
        #region Fields

        private bool isBaseTable;
        private COEHitList hitList;
        private string tableName;
        private string formName;
        private int tableID;
        private int hitListId;
        private bool filterChildHits;

        #endregion

        #region Constructors

        public SpotfireCOETableDataSource()
            : base()
        {
            this.isBaseTable = true;
        }

        protected SpotfireCOETableDataSource(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.isBaseTable = info.GetBoolean("isBaseTable");
            this.hitList = (COEHitList)info.GetValue("hitList", typeof(COEHitList));
            this.tableName = info.GetString("tableName");
            this.tableID = info.GetInt32("tableID");
            PopulateTableNameIdCollection(this.tableID, this.tableName);
            this.formName = info.GetString("documentTitle");//documentTitle is read GetObject method & assigned to formName
            //old saved files will not have this property and can fail hence set value to false
            try
            {
                this.filterChildHits = info.GetBoolean("filterChildHits");
            }
            catch (System.Runtime.Serialization.SerializationException)
            {
                this.filterChildHits = false;
            }
        }

        /// <summary>
        /// Populates the table name id collection from current datasource
        /// </summary>
        /// <param name="tableId">table id</param>
        /// <param name="tableName">table name</param>
        private void PopulateTableNameIdCollection(int tableId, string tableName)
        {
            TableNameIdMapper theTableNameIdMapper = TableNameIdMapper.Instance();
            theTableNameIdMapper.DataViewId = this.hitList.DataViewID;
            theTableNameIdMapper.AddEntry(tableId, tableName);
        }

        //CSBR:151920 formName parameter is added to set DocumentTitle property.
        public SpotfireCOETableDataSource(COEHitList hitList, string tableName, bool isBaseTable,
        string formName, bool filterChildHits)
            : this(hitList, tableName, isBaseTable, 0, filterChildHits)
        {
            this.formName = formName;
            this.filterChildHits = filterChildHits;
        }
        public SpotfireCOETableDataSource(COEHitList hitList, string tableName, bool isBaseTable, int tableID, bool filterChildHits)
            : base()
        {
            this.isBaseTable = isBaseTable;
            this.hitList = hitList;
            this.tableName = tableName;
            this.tableID = tableID;
            this.filterChildHits = filterChildHits;
        }
        public SpotfireCOETableDataSource(COEHitList hitList, string tableName, bool isBaseTable, int tableID, string formName, bool filterChildHits)
            : base()
        {
            this.isBaseTable = isBaseTable;
            this.hitList = hitList;
            this.tableName = tableName;
            this.tableID = tableID;
            this.formName = formName;
            this.filterChildHits = filterChildHits;
        }

        public SpotfireCOETableDataSource(COEHitList hitList, string tableName, bool isBaseTable, string formName, int hitListId, bool filterChildHits)
            : base()
        {
            this.isBaseTable = isBaseTable;
            this.hitList = hitList;
            this.tableName = tableName;
            this.formName = formName;
            this.hitListId = hitListId;
            if (hitListId > 0)
            {
                this.hitList.HitListID = hitListId;
            }
            this.filterChildHits = filterChildHits;
        }

        #endregion

        #region Properties

        public override bool IsLinkable
        {
            get { return true; }
        }

        public override string Name
        {
            get { return this.formName; }
        }

        internal bool IsBaseTable
        {
            get { return this.isBaseTable; }
        }

        internal COEHitList HitList
        {
            get { return this.hitList; }
        }

        internal string TableName
        {
            get { return this.tableName; }
        }

        internal int TableID
        {
            get { return this.tableID; }
        }

        internal int HitListId
        {
            get { return this.hitListId; }
        }

        private bool NeedsHitListMetadata
        {
            get
            {
                return (this.hitList == null || (this.tableName == null && !this.isBaseTable));
            }
        }

        private bool NeedsAuthentication(IServiceProvider serviceProvider)
        {
            COEService service = (COEService)serviceProvider.GetService(typeof(COEService));
            //Comment this code for resolving the login issue in 2-tier mode.
            //get analysis application and check if currently connected to same server as that of dxp file.
            AnalysisApplication application = (AnalysisApplication)serviceProvider.GetService(typeof(AnalysisApplication));
            CBOEConnectionPersistance theCboeConnectionPersistance = application.GetConnInfo();
            if (theCboeConnectionPersistance != null)
            {
                if (GetServerName(theCboeConnectionPersistance.TheConnInfo.ServerName) == service.ServerName)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            return !service.IsAuthenticated;
        }

        /// <summary>
        /// Get the server name only without connection mode information for comparison purpose
        /// </summary>
        /// <param name="inputServerName">The server name stored in connection information</param>
        /// <returns>returns the server name</returns>
        string GetServerName(string inputServerName)
        {
            string serverName = string.Empty;
            string twoTierMode = "2-Tier";
            if (inputServerName.StartsWith(twoTierMode, true, System.Globalization.CultureInfo.CurrentCulture))
            {
                inputServerName = inputServerName.Replace(twoTierMode, string.Empty);
                inputServerName = inputServerName.Replace('/', ' ');
                serverName = inputServerName.Trim();
            }
            else
            {
                int position = inputServerName.LastIndexOf('/');
                if (position > -1)
                {
                    serverName = inputServerName.Substring(position + 1);
                }

                if (string.IsNullOrEmpty(serverName))
                {
                    position = inputServerName.IndexOf('/');
                    if (position > -1)
                    {
                        serverName = inputServerName.Substring(0, position);
                    }
                }

                if (string.IsNullOrEmpty(serverName))
                {
                    serverName = inputServerName;
                }
            }
            return serverName;
        }

        public bool FilterChildHits
        {
            get { return this.filterChildHits; }
        }

        #endregion

        #region Public Methods

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("isBaseTable", this.isBaseTable);
            info.AddValue("hitList", this.hitList);
            info.AddValue("tableName", this.tableName);
            info.AddValue("tableID", this.tableID);
            //CSBR:151920 formName parameter is added to set DocumentTitle property.
            info.AddValue("documentTitle", this.formName);
            info.AddValue("filterChildHits", this.filterChildHits);
        }

        #endregion

        #region Methods

        protected override DataSourceConnection ConnectCore(IServiceProvider serviceProvider, DataSourcePromptMode promptMode)
        {
            AnalysisApplication application = (AnalysisApplication)serviceProvider.GetService(typeof(AnalysisApplication));
            application.Document.CustomNodes.AddNewIfNeeded<COETableLoadNode>();

            // Get the prompt service.
            //Coverity Bug Fix CID:12916 
            PromptService promptService = serviceProvider.GetService(typeof(PromptService)) as PromptService;
            if (promptService != null)
            {
                if (this.NeedsAuthentication(serviceProvider))
                {
                    if (!promptService.IsPromptingAllowed)
                    {
                        throw new ImportException("Cannot open data from COE since prompting is not allowed and user is not authenticated.");
                    }
                }

                // Check if we need to prompt and prompting is not allowed.
                if (this.NeedsHitListMetadata)
                {
                    if (!promptService.IsPromptingAllowed)
                    {
                        throw new ImportException("Cannot open data from COE since prompting is not allowed and no hitlist is selected.");
                    }
                }

                // First create an empty enumeration of prompt model, this will be used as the prompt model input
                // if prompting is not allowed.
                IEnumerable<object> promptModels;

                if (promptService.IsPromptingAllowed && promptMode != DataSourcePromptMode.None)
                {
                    promptModels = this.Prompt(serviceProvider, promptMode);
                }
                else
                {
                    promptModels = new List<object>();
                }

                return new SpotfireCOETableDataSourceConnection(this, serviceProvider, promptModels);
            }
            return null;
        }

        /// <summary>Handle prompting for this data source. This method is written using the
        /// yield return construct to be able to handle asynchronous prompting. After the first yeild
        /// return one can assume that the prompting has been successfully performed.
        /// </summary>
        /// <returns>The prompt models.</returns>
        private IEnumerable<object> Prompt(IServiceProvider serviceProvider, DataSourcePromptMode promptMode)
        {
            if (this.NeedsAuthentication(serviceProvider))
            {
                COEService service = (COEService)serviceProvider.GetService(typeof(COEService));
                //get the analysis application and retrieve the connection information present in the document property
                AnalysisApplication application = (AnalysisApplication)serviceProvider.GetService(typeof(AnalysisApplication));
                CBOEConnectionPersistance theCBOEConnectionPersistance = application.GetConnInfo();
                //if authenticatio nis required then read the prefenreces stored in custom preferences, 
                //this code will be used when a link dxp file is opened by the user and user needs to authenticate 
                //against the CBOE server
                SetUserGroupPreferences(service, application);

                bool success = false;
                while (!success)
                {
                    SpotfireCOEAuthenticationPromptModel promptModel = service.GetLoginModel();
                    //if connection information is present then set current MRU entry from the document property to login model
                    if (theCBOEConnectionPersistance != null)
                    {
                        promptModel.CurrentMRUEntry = theCBOEConnectionPersistance.TheConnInfo.CurrentMRUEntry;
                        promptModel.EnableServerSelectionCombo = false;
                    }
                    yield return promptModel;

                    if (!string.IsNullOrEmpty(promptModel.Username) &&
                        !string.IsNullOrEmpty(promptModel.Password) &&
                        !string.IsNullOrEmpty(promptModel.Server))
                    {
                        success = service.Login(promptModel.Username, promptModel.Password, promptModel.Server);
                        if (success)
                        {
                            promptModel.Save();
                        }
                    }
                }
            }

            if (this.NeedsHitListMetadata || promptMode == DataSourcePromptMode.All)
            {
                COEHitListPromptModel hitListPromptModel = new COEHitListPromptModel();
                yield return hitListPromptModel;

                COEService service = (COEService)serviceProvider.GetService(typeof(COEService));
                COEResultsCriteriaPromptModel resultsCriteriaPromptModel = new COEResultsCriteriaPromptModel();
                resultsCriteriaPromptModel.DataViews = service.GetDataViews();
                yield return resultsCriteriaPromptModel;

                this.hitList = new COEHitList(
                    hitListPromptModel.HitListID,
                    hitListPromptModel.HitListType,
                    0,
                    resultsCriteriaPromptModel.DataViewBO.ID,
                    resultsCriteriaPromptModel.ResultsCriteria,
                    resultsCriteriaPromptModel.SearchCriteria);

                this.isBaseTable = true;
                this.tableName = resultsCriteriaPromptModel.DataViewBO.BaseTable;
                this.tableID = resultsCriteriaPromptModel.DataViewBO.COEDataView.Basetable;
            }
        }

        /// <summary>
        /// Sets the user group level preferences before authentication
        /// </summary>
        /// <param name="service">COEService instance</param>
        /// <param name="application">AnalysisApplication object that is currently being used </param>
        private static void SetUserGroupPreferences(COEService service, AnalysisApplication application)
        {
            //Loading Spotfire Preferences values
            application.SetUserCustomPreferences(service);
        }

        #endregion
    }
}