using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Xml;

using CambridgeSoft.COE.Framework.COEConfigurationService;
using CambridgeSoft.COE.Framework.COEDataViewService;
using CambridgeSoft.COE.Framework.COELoggingService;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.Properties;

using Csla;
using Csla.Data;
using Csla.Validation;

namespace CambridgeSoft.COE.Framework.COEDatabasePublishingService
{
    using Utilities = CambridgeSoft.COE.Framework.Common.Utilities;

    [Serializable()]
    public class COEDatabaseBO : Csla.BusinessBase<COEDatabaseBO>
    {
        #region Member variables
        //declare members (Fields inside Database Tables)
        private int _id = -1;
        private string _name = string.Empty;
        private string _owner = string.Empty;
        private SmartDate _dateCreated = new SmartDate(true);
        private COEDataView _coeDataView = null;
        private bool _isPublished = false;
        private string _password = string.Empty;
        private string _databaseId = null;
        private string _instanceName = null;

        // A field data type which match to ROWID hitlist data type
        private const COEDataView.AbstractTypes RowIdDataType = COEDataView.AbstractTypes.CLob;

        //variables data access
        [NonSerialized]
        internal DAL _coeDAL = null;

        [NonSerialized]
        internal DAL _globalDAL = null;

        [NonSerialized]
        internal DALFactory _dalFactory = null;
        internal string _serviceName = "COEDatabasePublishing";

        [NonSerialized]
        static COELog _coeLog = COELog.GetSingleton("COEDatabasePublishing");

        //variable to hold the value for publishing relationships
        //defaulted to true so that relationships will be included while publishing a schema
        private bool _isPublishRelationships = true;

        #endregion

        #region Properties

        [System.ComponentModel.DataObjectField(true, false)]
        public int ID
        {
            get
            {
                CanReadProperty("ID", true);
                return _id;
            }
        }

        public string Name
        {
            get
            {
                CanReadProperty("Name", true);
                return _name;
            }
        }

        public string Owner
        {
            get
            {
                if (string.IsNullOrEmpty(Name) || !Name.Contains("."))
                {
                    return Name;
                }
                else
                {
                    return Name.Split(new char[] { '.' })[1];
                }
            }
        }

        public string Instance
        {
            get
            {
                return _instanceName;
            }

            set
            {
                _instanceName = value;
            }
        }

        public string DatabaseId
        {
            get
            {
                return _databaseId;
            }

            set
            {
                _databaseId = value;
            }
        }

        public string Password
        {
            get
            {
                return _password;
            }

            set
            {
                _password = value;
            }
        }

        public bool IsPublished
        {
            get
            {
                return _isPublished;
            }
        }
        
        public DateTime DateCreated
        {
            get
            {
                CanReadProperty("DateCreated", true);
                return _dateCreated.Date;
            }
        }

        public COEDataView COEDataView
        {
            get
            {
                CanReadProperty("COEDataView", true);
                return _coeDataView;
            }
            set
            {
                _coeDataView = value;
                PropertyHasChanged("COEDataView");
            }
        }

        public override bool IsValid
        {
            get
            {
                this.ValidationRules.CheckRules();

                return base.IsValid;
            }
        }

        public bool IsMainInstance
        {
            get
            {
                bool isMainInstance = false;
                InstanceData mainInstance = ConfigurationUtilities.GetMainInstance();

                if (!string.IsNullOrEmpty(_instanceName))
                {
                    isMainInstance = _instanceName.Equals(mainInstance.Name, StringComparison.InvariantCultureIgnoreCase);
                }
                else if (string.IsNullOrEmpty(_name))
                {
                    isMainInstance = !this._name.Contains(".") ||
                        this._name.Split(new char[] { '.' })[0].Equals(mainInstance.Name, StringComparison.InvariantCultureIgnoreCase);
                }

                return isMainInstance;
            }
        }

        public string InstanceSchemaName
        {
            get
            {
                return IsMainInstance ? _name : _instanceName + "." + _name;
            }
        }

        protected override object GetIdValue()
        {
            return _id;
        }

        /// <summary>
        /// Gets or sets if relationships publishing is selected from UI
        /// </summary>
        public bool IsPublishRelationships
        {
            get
            {
                return _isPublishRelationships;
            }
            set
            {
                CanWriteProperty("IsPublishRelationships");
                _isPublishRelationships = value;
                PropertyHasChanged("IsPublishRelationships");
            }
        }

        #endregion

        #region Constructors

        internal COEDatabaseBO(string name)
        {
            _name = name;
        }

        internal COEDatabaseBO()
        {
        }

        //constructor to be called from queryCriteriaList as well as any other services that needs to construct this object
        internal COEDatabaseBO(int id, string name, SmartDate dateCreated, COEDataView coeDataView, bool isPublished, string instanceName, string databaseId)
        {
            _id = id;
            _name = name;
            _dateCreated = dateCreated;
            _coeDataView = coeDataView;
            _isPublished = isPublished;
            _instanceName = instanceName;
            _databaseId = databaseId;
            MarkAsChild();
        }
        
        internal COEDatabaseBO(COEDataView coeDataView)
        {
            _coeDataView = coeDataView;
            MarkAsChild();
        }

        #endregion

        #region Validation Rules

        private void AddCommonRules()
        {
            //
            // QueryName
            //
            ValidationRules.AddRule(CommonRules.StringRequired, "Name");
            ValidationRules.AddRule(CommonRules.StringMaxLength, new CommonRules.MaxLengthRuleArgs("Name", 50));
            //Description
            ValidationRules.AddRule(CommonRules.StringMaxLength, new CommonRules.MaxLengthRuleArgs("Name", 255));
            //
            // DateCreated
            //
            //ValidationRules.AddRule(CommonRules.MinValue<DateTime>, new CommonRules.MinValueRuleArgs<DateTime>("DateCreated", new DateTime(2005, 1, 1)));
            //ValidationRules.AddRule(CommonRules.RegExMatch, new CommonRules.RegExRuleArgs("DateCreated", @"(0[1-9]|1[012])[- /.](0[1-9]|[12][0-9]|3[01])[- /.](19|20)\d\d"));
        }

        protected override void AddBusinessRules()
        {
            AddCommonRules();
        }

        protected override void AddInstanceBusinessRules()
        {
            base.AddInstanceBusinessRules();
            ValidationRules.AddInstanceRule(this.UniquePublishedDB, new RuleArgs("Name"));
            ValidationRules.AddInstanceRule(this.IsNotEmpty, new RuleArgs("COEDataView"));
        }

        private bool IsNotEmpty(object target, RuleArgs args)
        {
            try
            {
                COEDataView dview = new COEDataView();
                if (this.COEDataView == null)
                    dview.GetFromXML(this.BuildDataViewForSchema(_name, _instanceName));
                else
                    dview = this.COEDataView;

                if (dview == null || dview.Tables.Count <= 0)
                {
                    args.Description = Resources.EmptySchema;
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                args.Description = ex.Message;
                return false;
            }
        }

        private bool UniquePublishedDB(object target, RuleArgs e)
        {
            if (!this.CheckNameIsUnique())
            {
                e.Description = Resources.AlreadyPublishedDatabase;
                return false;
            }
            else
                return true;
        }

        private bool CheckNameIsUnique()
        {
            bool isUnique = true;
            if (!_isPublished)
            {
                SetDatabaseName();
                if (_coeDAL == null) { LoadDAL(); }
                // Coverity Fix CID - 11502
                if (_coeDAL != null)
                {
                    SafeDataReader reader = _coeDAL.GetPublishedDatabases();

                    while (reader.Read())
                    {
                        if (reader.GetString("NAME") == this.InstanceSchemaName && Convert.ToInt32(reader.GetInt64("ID")) != _id)
                        {
                            isUnique = false;
                        }
                    }
                }
                else
                {
                    throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));
                }
            }
            return isUnique;
        }
        #endregion //Validation Rules

        #region Factory Methods

        //this method must be called prior to any other method inorder to set the database that the dal will use
        internal static void SetDatabaseName()
        {
            COEDatabaseName.Set(DALUtils.GetDefaultQualifyDbName(Resources.CentralizedStorageDB));
        }

        internal static void SetDatabaseName(string databaseName)
        {
            COEDatabaseName.Set(DALUtils.GetDefaultQualifyDbName(Resources.CentralizedStorageDB));
        }

        public static COEDatabaseBO New()
        {
            SetDatabaseName();
            if (!CanAddObject())
                throw new System.Security.SecurityException(Resources.UserNotAuthorizedForAddObject + " COEDataViewBO");

            return DataPortal.Create<COEDatabaseBO>(new CreateNewCriteria());
        }

        public static COEDatabaseBO New(string name)
        {
            COEDatabaseBO result = New();
            result._name = name;
            return result;
        }

        public static COEDatabaseBO Get(string name)
        {
            SetDatabaseName();
            if (!CanGetObject())
                throw new System.Security.SecurityException(Resources.UserNotAuthorizedForViewObject + " COEDatabaseBO");
            return DataPortal.Fetch<COEDatabaseBO>(new Criteria(name));
        }

        public static COEDatabaseBO Get(string instanceName, string schemaName)
        {
            var instanceBO = ConfigurationUtilities.GetInstanceData(instanceName);

            if (instanceBO != null && instanceBO.IsCBOEInstance)
            {
                return Get(schemaName);
            }
            else
            {
                var qualifyDbName = instanceName + "." + schemaName;
                return Get(qualifyDbName);
            }
        }

        public COEDatabaseBO Publish(string password)
        {
            try
            {
                SetDatabaseName();
                if (this._isPublished == false)
                {
                    if (!CanAddObject())
                        throw new System.Security.SecurityException(Resources.UserNotAuthorizedForEditObject + " COEDatabaseBO");
                    COEDatabaseBO returnBO = DataPortal.Create<COEDatabaseBO>(
                        new CreateBasedOnCriteria(this.Name, password, _isPublishRelationships, this.Instance, this.Owner, false));

                    returnBO._isPublished = true;
                    return returnBO;
                }
                else
                {
                    return this;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public COEDatabaseBO TryPublish(string password)
        {
            try
            {
                SetDatabaseName();
                if (this._isPublished == false)
                {
                    if (!CanAddObject())
                        throw new System.Security.SecurityException(Resources.UserNotAuthorizedForEditObject + " COEDatabaseBO");
                    COEDatabaseBO returnBO = DataPortal.Create<COEDatabaseBO>(
                        new CreateBasedOnCriteria(this.Name, password, _isPublishRelationships, this.Instance, this.Owner, true));

                    returnBO._isPublished = true;
                    return returnBO;
                }
                else
                {
                    return this;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public COEDatabaseBO PublishOnlyWithoutValidation(string schemaName)
        {
            var publishCriteria = new PublishOnlyCriteria(schemaName, this.COEDataView.ToString());
            return DataPortal.Create<COEDatabaseBO>(publishCriteria);
        }

        public COEDatabaseBO RefreshPublish()
        {
            try
            {
                if (this._isPublished == true)
                {
                    SetDatabaseName();
                    if (!CanEditObject())
                        throw new System.Security.SecurityException(Resources.UserNotAuthorizedForEditObject + " COEDatabaseBO");
                    this.COEDataView.GetFromXML(this.BuildDataViewForSchema(_owner, _instanceName));
                    return DataPortal.Update<COEDatabaseBO>(this);
                }
                else
                {
                    return this;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public COEDatabaseBO UnPublish()
        {
            try
            {
                SetDatabaseName();
                if (!CanDeleteObject())
                    throw new System.Security.SecurityException(Resources.UserNotAuthorizedForDeleteObject + " COEDatabaseBO");
                DataPortal.Delete(new Criteria(this._name));
                this._isPublished = false;
                return this;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static bool CanAddObject()
        {
            // return Csla.ApplicationContext.User.IsInRole("CanSearch");
            return true;
        }

        public static bool CanGetObject()
        {
            // return Csla.ApplicationContext.User.IsInRole("CanSearch");
            return true;
        }

        public static bool CanEditObject()
        {
            // return Csla.ApplicationContext.User.IsInRole("CanSearch");
            return true;
        }

        public static bool CanDeleteObject()
        {
            // return Csla.ApplicationContext.User.IsInRole("CanSearch");
            return true;
        }

        #endregion //Factory Methods

        #region Data Access
        protected override void DataPortal_OnDataPortalException(DataPortalEventArgs e, Exception ex)
        {
            throw ex;
        }

        #region Criteria

        [Serializable()]
        protected class CreateNewCriteria
        {
            public CreateNewCriteria()
            {
            }
        }

        [Serializable()]
        protected class Criteria
        {
            internal string _name = string.Empty;

            public Criteria(string name)
            {
                _name = name;
            }
        }

        [Serializable()]
        protected class CreateBasedOnCriteria
        {
            internal string _password = String.Empty;
            internal string _name = String.Empty;
            internal string _instanceName = string.Empty;
            internal string _ownerName = string.Empty;
            internal bool _addRelationships = true;
            internal bool _isTestMode = false;

            public CreateBasedOnCriteria(string name, string password, string instanceName, string ownerName)
            {
                _name = name;
                _password = password;
                _instanceName = instanceName;
                _ownerName = ownerName;
            }

            public CreateBasedOnCriteria(string name, string password, bool addRelationships, string instanceName, string ownerName, bool isTestMode = false)
                : this(name, password, instanceName, ownerName)
            {
                _addRelationships = addRelationships;
                _isTestMode = isTestMode;
            }
        }

        [Serializable()]
        protected class PublishOnlyCriteria
        {
            internal string _schemaName = string.Empty;
            internal string _dataviewXml = string.Empty;

            public PublishOnlyCriteria(string schemaName, string dataviewXml)
            {
                _schemaName = schemaName;
                _dataviewXml = dataviewXml;
            }
        }

        #endregion //Criteria

        #region Data Access - Create

        internal static void UpdateTablePermissions(COEDataViewManagerBO coeDataViewManagerBO)
        {
            COEDatabaseBO GrantingDatabaseBO = new COEDatabaseBO();
            GrantingDatabaseBO.DoGrantsAndRevokes(coeDataViewManagerBO);
        }

        private void DoGrantsAndRevokes(COEDataViewManagerBO coeDataViewManagerBO)
        {
            //get a list of the publishes schemas
            InstanceData mainInstance = ConfigurationUtilities.GetMainInstance();
            COEDatabaseBOList coeDatabaseBOList = COEDatabaseBOList.GetList(true, mainInstance.Name);

            //loop through database list
            COEDataViewBO dvBO = COEDataViewBO.GetMasterSchema();
            foreach (COEDatabaseBO coeDatabaseBO in coeDatabaseBOList)
            {
                string database = coeDatabaseBO.Name;
                List<TableStatus> tableStatusAll = new List<TableStatus>();
                List<TableStatus> filteredTableStatus = new List<TableStatus>();
                tableStatusAll = coeDataViewManagerBO.GetMasterDataViewTablesStatus(coeDataViewManagerBO.Tables, dvBO.DataViewManager.Tables);
                //get filtered list for a single database
                filteredTableStatus = tableStatusAll.FindAll(delegate(TableStatus currentTableStatus) { return currentTableStatus.Table.DataBase == database; });
                //loop through tables for that singe database, check status and grant or revoke as appropriate
                if (filteredTableStatus.Count > 0)
                {
                    bool isMainInstance = coeDatabaseBO.Instance == ConfigurationUtilities.GetCOEInstance().InstanceName;

                    LoadDAL();
                    foreach (TableStatus tableStatus in filteredTableStatus)
                    {
                        switch (tableStatus.Status)
                        {
                            case TableStatus.TableStatusOpt.Added:
                                _coeDAL.GrantTable(database, tableStatus.Table.Name.ToString());
                                break;
                            case TableStatus.TableStatusOpt.Removed:
                                _coeDAL.RevokeTable(database, tableStatus.Table.Name.ToString());
                                break;
                        }
                    }
                }
            }
        }

        [RunLocal]
        private void DataPortal_Create(CreateNewCriteria criteria)
        {
            _coeDataView = null;
        }

        private void DataPortal_Create(CreateBasedOnCriteria criteria)
        {
            _password = criteria._password;
            if (_coeDAL == null) { LoadDAL(); }
            Insert2(_coeDAL, criteria);
        }

        private void DataPortal_Create(PublishOnlyCriteria criteria)
        {
            Insert(criteria);
        }

        #endregion //Data Access - Create

        #region Data Access - Fetch

        private void DataPortal_Fetch(Criteria criteria)
        {
            _coeLog.LogStart("Fetching COEDatabaseDataView", 1);
            if (_coeDAL == null) { LoadDAL(); }
            // Coverity Fix CID - 11504
            if (_coeDAL != null)
            {
                using (SafeDataReader dr = _coeDAL.GetCOEDatabaseDataView(criteria._name))
                {
                    FetchObject(dr);
                }
            }
            else
            {
                throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));
            }

            _coeLog.LogEnd("Fetching COEDatabaseDataView");
        }

        private void FetchObject(SafeDataReader dr)
        {
            try
            {
                InstanceData mainInstance = ConfigurationUtilities.GetMainInstance();

                if (dr.Read())
                {
                    _id = dr.GetInt16("ID");
                    _name = dr.GetString("NAME");
                    _instanceName = (string.IsNullOrEmpty(_name) || !_name.Contains(".")) ? mainInstance.Name : _name.Split(new char[] { '.' })[0];
                    _owner = (string.IsNullOrEmpty(_name) || !_name.Contains(".")) ? _name : _name.Split(new char[] { '.' })[1];
                    _dateCreated = dr.GetSmartDate("DATE_CREATED", _dateCreated.EmptyIsMin);
                    _coeDataView = (COEDataView)COEDataViewUtilities.DeserializeCOEDataView(dr.GetString("COEDATAVIEW"));
                    _isPublished = true;
                    // _coeAccessRights
                }
            }
            catch (System.Exception ex)
            {
            }
        }

        #endregion //Data Access - Fetch

        #region Data Access - Insert

        protected void Insert(PublishOnlyCriteria criteria)
        {
            if (_coeDAL == null)
            {
                LoadDAL();
            }

            var dbName = criteria._schemaName.ToUpper();
            var owner = dbName.Contains(".") ? dbName.Split('.')[1] : dbName;
            var instance = dbName.Contains(".") ? ConfigurationUtilities.GetInstanceData(dbName.Split('.')[0]) : ConfigurationUtilities.GetCOEInstance();

            _id = _coeDAL.InsertCOEDatabaseDataView(dbName, criteria._dataviewXml);
            // populate return object
            _coeDataView = (COEDataView)COEDataViewUtilities.DeserializeCOEDataView(criteria._dataviewXml);
            _dateCreated = new SmartDate(DateTime.Now);
            _isPublished = true;
            MarkOld();

            // create base roles and add to security role and generic privilege table
            // Only when its coe instance db, grant permission.
            if (!dbName.Contains("."))
            {
                AddToCOESecurity(criteria._schemaName);
            }

            //add to coeframeworkconfig
            AddToConfig(dbName, owner, instance.Id);
        }

        protected void Insert(DAL _coeDAL, CreateBasedOnCriteria criteria)
        {
            Insert2(_coeDAL, null);
        }

        protected void Insert2(DAL _coeDAL, CreateBasedOnCriteria criteria)
        {
            var dbInstance = ConfigurationUtilities.GetInstanceData(criteria._instanceName);
            var globalDbName = dbInstance.IsCBOEInstance ? dbInstance.DatabaseGlobalUser : dbInstance.InstanceName + "." + dbInstance.DatabaseGlobalUser;

            LoadGlobalDAL(globalDbName);

            if (criteria != null && _globalDAL.AuthenticateUser(criteria._name, criteria._password, dbInstance.HostName, dbInstance.Port, dbInstance.SID))
            {
                try
                {
                    //grant select through proxy 
                    GrantProxyAccess(criteria._name, criteria._password, dbInstance.DatabaseGlobalUser);

                    //dal must be reloaded because grant proxy uses the security dal
                    LoadDAL();
                    _name = criteria._name;
                    //get the dataview representation for the schema
                    string dataview = (this.COEDataView != null && this.COEDataView.Tables.Count > 0) ? this.COEDataView.ToString() : BuildDataViewForSchema(criteria);
                    //insert dataview in coeschema

                    var dbName = dbInstance.IsCBOEInstance ? criteria._name : criteria._instanceName + "." + criteria._name;

                    // If this is test mode, just test the security, do not save into database.
                    if (!criteria._isTestMode)
                    {
                        _id = _coeDAL.InsertCOEDatabaseDataView(dbName, dataview);

                        // create base roles and add to security role and generic privilege table
                        // Only when its coe instance db, grant permission.
                        if (dbInstance.IsCBOEInstance)
                        {
                            AddToCOESecurity(criteria._name);
                        }

                        //add to coeframeworkconfig
                        AddToConfig(dbName, criteria._ownerName, dbInstance.Id);
                    }

                    // populate return object
                    _coeDataView = (COEDataView)COEDataViewUtilities.DeserializeCOEDataView(dataview);
                    _dateCreated = new SmartDate(DateTime.Now);
                    _isPublished = true;
                    MarkOld();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            else
            {
                throw new Exception("Owner/Password is invalid");
            }
        }

        protected override void DataPortal_Insert()
        {
            if (_coeDAL == null) { LoadDAL(); }
            {
                //Insert(_coeDAL);
            }
        }

        #endregion

        #region Data Access - Update

        internal void Update(DAL _coeDAL)
        {
            if (_coeDAL == null) { LoadDAL(); }

            //get the dataview from the back end database. no password should be needed

            string dataview = (this.COEDataView != null && this.COEDataView.Tables.Count > 0) ? this.COEDataView.ToString() : BuildDataViewForSchema(_owner, _instanceName);

            //update dataview in coeschema
            DateTime dateModified = DateTime.Now;
            _dateCreated = new SmartDate(dateModified);
            // Coverity Fix CID - 11509 
            if (_coeDAL != null)
            {
                _coeDAL.UpdateCOEDatabaseDataView(_name, dataview, dateModified);
            }
            else
                throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));

            //populate return object
            _coeDataView = (COEDataView)COEDataViewUtilities.DeserializeCOEDataView(dataview);
            MarkOld();
        }

        protected override void DataPortal_Update()
        {
            Update(_coeDAL);
        }

        #endregion //Data Access - Update

        #region Data Access - Delete
        //called by other services
        internal void DeleteSelf(DAL _coeDAL)
        {
            if (_coeDAL == null) { LoadDAL(); }
            // Coverity Fix CID - 11505 
            if (_coeDAL != null)
            {
                //get the dataview representation for the schema
                _coeDAL.DeleteCOEDatabaseDataView(this._name);
            }
            else
                throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));

            InstanceData mainInstance = ConfigurationUtilities.GetMainInstance();
            var instanceName = mainInstance.Name;
            var schemNameWithoutInstance = this._name;

            if (this._name.Contains("."))
            {
                var array = this._name.Split(new char[] { '.' });
                if (array.Length == 2)
                {
                    instanceName = array[0];
                    schemNameWithoutInstance = array[1];
                }
            }

            var instance = ConfigurationUtilities.GetInstanceData(instanceName);

            if (instance.IsCBOEInstance)
            {
                LoadGlobalDAL(instance.DatabaseGlobalUser);
            }
            else
            {
                LoadGlobalDAL(instance.Name + "." + instance.DatabaseGlobalUser);
            }

            //grant select through proxy 
            RevokeProxyAccess(schemNameWithoutInstance, instance.DatabaseGlobalUser);

            //create base roles and add to security role and generic privilege table
            if (instance.IsCBOEInstance)
            {
                RemoveFromCOESecurity(this._name);
            }

            //add to coeframeworkconfig
            RemoveFromConfig(this._name);
        }

        protected override void DataPortal_DeleteSelf()
        {
            DataPortal_Delete(new Criteria(_name));
        }

        private void DataPortal_Delete(Criteria criteria)
        {
            if (_coeDAL == null) { LoadDAL(); }

            _name = criteria._name;
            DeleteSelf(_coeDAL);
        }
        
        #endregion //Data Access - Delete

        private void LoadDAL()
        {
            _dalFactory = new DALFactory();
            _dalFactory.GetDAL<CambridgeSoft.COE.Framework.COEDatabasePublishingService.DAL>(ref _coeDAL, _serviceName, COEDatabaseName.Get().ToString(), true);
        }

        private void LoadGlobalDAL(string globalDatabaseName)
        {
            _dalFactory = new DALFactory();
            _dalFactory.GetDAL<CambridgeSoft.COE.Framework.COEDatabasePublishingService.DAL>(ref _globalDAL, _serviceName, globalDatabaseName, true);
        }

        #endregion //Data Access

        #region publishing methods

        protected string BuildDataViewForSchema(string owner, string instanceName)
        {
            InstanceData mainInstance = ConfigurationUtilities.GetMainInstance();
            bool isMainInstance = (string.IsNullOrWhiteSpace(instanceName) ||
                instanceName.Equals(mainInstance.Name, StringComparison.InvariantCultureIgnoreCase));
            var instance = ConfigurationUtilities.GetInstanceData(instanceName);

            if (isMainInstance)
            {
                LoadGlobalDAL(instance.DatabaseGlobalUser);
            }
            else
            {
                LoadGlobalDAL(instance.Name + "." + instance.DatabaseGlobalUser);
            }

            COEDataView coeDataView = new COEDataView();
            DataSet DatabaseSchema = new DataSet();
            DataSet definedDatabaseSchema = new DataSet();
            string qualifiedDatabase = isMainInstance ? owner : instanceName + "." + owner;

            DatabaseSchema = this.GetDataSet(_globalDAL, owner);
            definedDatabaseSchema = AddIdToTables(_globalDAL, owner, DatabaseSchema);

            PublishTableDataView(qualifiedDatabase, coeDataView, definedDatabaseSchema.Tables["DataTables"]);
            if (_isPublishRelationships)
                PublishRelationDataView(coeDataView, definedDatabaseSchema.Tables["RelationTables"]);

            coeDataView.Database = qualifiedDatabase;

            return COEDataViewUtilities.SerializeCOEDataView(coeDataView);
        }

        protected string BuildDataViewForSchema(CreateBasedOnCriteria criteria)
        {
            _isPublishRelationships = criteria._addRelationships;
            return BuildDataViewForSchema(criteria._name, criteria._instanceName);
        }

        private void AddToCOESecurity(string ownerName)
        {
            //LoadSecurityDAL();
            _coeDAL.InsertPrivileges(ownerName);
        }

        private void RemoveFromCOESecurity(string ownerName)
        {
            _globalDAL.RemovePrivileges(ownerName);
        }

        private void GrantProxyAccess(string ownerName, string password, string globalOwnerName)
        {
            _globalDAL.GrantProxy(ownerName, globalOwnerName);
        }

        private void RevokeProxyAccess(string ownerName, string globalOwnerName)
        {
            _globalDAL.RevokeProxy(ownerName, globalOwnerName);
        }

        private void RemoveFromConfig(string ownerName)
        {
            //not sure how to do this
        }

        private void AddToConfig(string dbName, string ownerName, Guid instanceId)
        {
            string dataBase = dbName;
            string provider = "Oracle.DataAccess.Client"; //we need or more general way to deal withthis.

            var databasesData = new DatabaseData();

            //Allocation of values to dataBaseData
            databasesData.Name = dataBase.ToUpper();
            databasesData.Owner = ownerName.ToUpper();
            databasesData.InstanceId = instanceId;

            databasesData.Password = string.Empty;
            databasesData.ProviderName = provider;
            databasesData.OracleTracing = false;
            databasesData.Tracing = false;

            string COEConfigPath = COEConfigurationManager.GetDefaultConfigurationFilePath();

            var xmlDoc = new System.Xml.XmlDocument();
            xmlDoc.Load(COEConfigPath);

            try
            {
                UpdateOrInsertDatabaseElement(xmlDoc, databasesData);
                File.SetAttributes(COEConfigPath, FileAttributes.Normal);
                xmlDoc.Save(COEConfigPath);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void UpdateOrInsertDatabaseElement(XmlDocument xmlDoc, DatabaseData databaseData)
        {
            var stringBuilder = new StringBuilder();
            var stringWriter = new StringWriter(stringBuilder);
            var xmlWriter = new XmlTextWriter(stringWriter);
            var connSettings = new COEConfigurationSettings();

            const string databaseEelmentPath = "configuration/coeConfiguration/databases";

            connSettings.Databases.Add(databaseData);

            connSettings.WriteXml(xmlWriter);
            stringBuilder.Replace("<clear />", "");

            var tempXmlDoc = new XmlDocument();
            tempXmlDoc.LoadXml(stringBuilder.ToString());

            var instanceXml = tempXmlDoc.SelectSingleNode("SerializableConfigurationSection/databases").InnerXml;

            var tempEelment = xmlDoc.CreateElement("temp");
            tempEelment.InnerXml = instanceXml;

            var existingDatabaseNode = xmlDoc.SelectSingleNode(databaseEelmentPath);

            XmlNode existingNode = null;
            for (int i = 0; i < existingDatabaseNode.ChildNodes.Count; i++)
            {
                if (existingDatabaseNode.ChildNodes[i].Attributes["name"].Value.Equals(
                    databaseData.Name, StringComparison.InvariantCultureIgnoreCase))
                {
                    existingNode = existingDatabaseNode.ChildNodes[i];
                    break;
                }
            }

            if (existingNode != null)
            {
                existingDatabaseNode.RemoveChild(existingNode);
            }

            xmlDoc.SelectSingleNode(databaseEelmentPath).AppendChild(tempEelment.FirstChild);
        }

        /// <summary>
        /// Fill COEDataView object with tables in the database
        /// </summary>
        /// <param name="DbTablesStructure">Tables Schema</param>
        private void PublishTableDataView(string dataBase, COEDataView dataView, DataTable DbTablesStructure)
        {
            //int baseTableID = -1;
            Int32 TableId = 0;
            Int32 ColumnId = 0;
            Int32 RowCount = -1;
            string PrimaryKeys = string.Empty;
            var PKDataType = RowIdDataType;
            int PKDataLength = 0;
            COEDataView.DataViewTable SchemaTable = null;

            foreach (DataRow drTables in DbTablesStructure.Rows)
            {
                RowCount++;

                if (TableId != (int)drTables["TableId"])
                {
                    SchemaTable = new COEDataView.DataViewTable();
                    TableId = (int)drTables["TableId"];
                    SchemaTable.Id = TableId;
                    //  if (RowCount==0)
                    //    baseTableID = TableId;

                    SchemaTable.Name = (string)drTables["TableName"];
                    SchemaTable.Database = dataBase;
                }
                COEDataView.Field SchemaTableFields = new COEDataView.Field();

                SchemaTableFields.Id = (int)drTables["ColumnId"];
                SchemaTableFields.Name = (string)drTables["ColumnName"];

                if (COEDataView.IndexTypes.CS_CARTRIDGE.ToString() == drTables["IndexType"].ToString())
                    SchemaTableFields.IndexType = COEDataView.IndexTypes.CS_CARTRIDGE;

                //assign isIndexed field value of SchemaTableFields - PP on 29Jan2013
                if (!string.IsNullOrEmpty(Convert.ToString(drTables["IndexName"])))
                {
                    SchemaTableFields.IsIndexed = true;
                }

                // Coverity Fix CID - 11508
                SchemaTableFields.DataType = GetDataTypes(Convert.ToString(drTables["DataType"]), Convert.ToInt32(drTables["DataPrecision"]), Convert.ToInt32(drTables["DataScale"]));
                // Coverity Fix CID - 10902, 10319 (from local server)                               
                if (SchemaTable != null)
                {
                    SchemaTable.Fields.Add(SchemaTableFields);

                    if ((bool)drTables["IsPrimaryKey"] == true)
                    {
                        if (PrimaryKeys == string.Empty)
                        {
                            PrimaryKeys = drTables["ColumnId"].ToString();
                            PKDataType = SchemaTableFields.DataType;
                            PKDataLength = Convert.ToInt32(drTables["DataLength"]);
                        }
                        else
                        {
                            PrimaryKeys += "," + drTables["ColumnId"];
                            PKDataType = RowIdDataType;
                            PKDataLength = 0;
                        }
                    }

                    if (drTables["TableType"].ToString().Contains("VIEW"))
                    {
                        SchemaTable.IsView = true;
                    }

                    // Is not one of the last 2 rows
                    if (RowCount < DbTablesStructure.Rows.Count - 1)
                    {
                        if ((int)DbTablesStructure.Rows[RowCount + 1]["TableId"] != TableId)
                        {
                            SchemaTable.PrimaryKey = PrimaryKeys;
                            SchemaTable.HitListDataType = this.GetHitListDataType(PKDataType, PKDataLength, SchemaTable.IsView);
                            dataView.Tables.Add(SchemaTable);
                            PrimaryKeys = string.Empty;
                            PKDataType = RowIdDataType;
                            PKDataLength = 0;
                        }
                    }
                    else
                    {
                        SchemaTable.PrimaryKey = PrimaryKeys;
                        SchemaTable.HitListDataType = this.GetHitListDataType(PKDataType, PKDataLength, SchemaTable.IsView);
                        dataView.Tables.Add(SchemaTable);
                        PrimaryKeys = string.Empty;
                        PKDataType = RowIdDataType;
                        PKDataLength = 0;
                    }
                }
            }
            // return baseTableID;
        }

        // Get hitlist data type
        private COEDataView.HitListDataTypes GetHitListDataType(COEDataView.AbstractTypes PrimaryKeyType, int dataLength, bool isView)
        {
            var type = Utilities.GetHitListDataTypeByFiledDataType(PrimaryKeyType, dataLength);

            // A view may don't have PK filed. But we can't store ROWID in hitlist
            // since jointed views don't have ROWID. (ref ORA-01445)
            if (type == COEDataView.HitListDataTypes.ROWID && isView)
            {
                // The hitlist data type will be decided according to COE DataView PK filed
                type = COEDataView.HitListDataTypes.COEPK;
            }

            return type;
        }

        /// <summary>
        /// Fill COEDataView object with relations in the database
        /// </summary>
        /// <param name="DBRelationStructure">Relations Schema</param>
        private void PublishRelationDataView(COEDataView dataView, DataTable DBRelationStructure)
        {
            foreach (DataRow drRelation in DBRelationStructure.Rows)
            {
                COEDataView.Relationship RelationshipTable = new COEDataView.Relationship();

                RelationshipTable.Parent = (int)drRelation["ParentTableId"];
                RelationshipTable.ParentKey = (int)drRelation["ParentKey"];
                RelationshipTable.Child = (int)drRelation["ChildTableId"];
                RelationshipTable.ChildKey = (int)drRelation["ChildKey"];
                dataView.Relationships.Add(RelationshipTable);
            }
        }

        /// <summary>
        /// Check Application from COEConfig file
        /// </summary>
        /// <param name="OwnerName">Owner Name</param>
        /// <returns>dataset, containing the </returns>
        protected DataSet AddIdToTables(DAL coeDAL, string dataBase, DataSet DbTables)
        {
            Int32 TableId = 0;
            Int32 ColumnId = 0;

            string IndexTypeClobValue = COEDataView.IndexTypes.CS_CARTRIDGE.ToString();

            string SchemaTableName = "TABLE_NAME";
            string SchemaColumnName = "COLUMN_NAME";
            string SchemaDataType = "DATA_TYPE";
            string SchemaDataLength = "DATA_LENGTH";
            string SchemaDataPrecision = "DATA_PRECISION";
            string SchemaDataScale = "DATA_SCALE";
            string SchemaFieldIsPrimaryKey = "IS_PRIMARY_KEY";
            string SchemaTableType = "TYPE";

            string RelationFieldPkTableName = "PK_Table_Name";
            string RelationFieldPkColumnName = "PK_Column";
            string RelationFieldFkTableName = "FK_Table_Name";
            string RelationFieldFkColumnName = "FK_Column";

            DataTable SchemaDataTable = new DataTable();
            SchemaDataTable.TableName = "DataTables";
            SchemaDataTable.Columns.Add(new DataColumn("TableId", Type.GetType("System.Int32")));
            SchemaDataTable.Columns.Add(new DataColumn("TableName", Type.GetType("System.String")));
            SchemaDataTable.Columns.Add(new DataColumn("ColumnId", Type.GetType("System.Int32")));
            SchemaDataTable.Columns.Add(new DataColumn("ColumnName", Type.GetType("System.String")));
            SchemaDataTable.Columns.Add(new DataColumn("DataType", Type.GetType("System.String")));
            SchemaDataTable.Columns.Add(new DataColumn("DataLength", Type.GetType("System.Int32")));
            SchemaDataTable.Columns.Add(new DataColumn("DataPrecision", Type.GetType("System.Int32")));
            SchemaDataTable.Columns.Add(new DataColumn("DataScale", Type.GetType("System.Int32")));
            SchemaDataTable.Columns.Add(new DataColumn("IsPrimaryKey", Type.GetType("System.Boolean")));
            SchemaDataTable.Columns.Add(new DataColumn("TableType", Type.GetType("System.String")));
            SchemaDataTable.Columns.Add(new DataColumn("IndexType", Type.GetType("System.String")));
            //add new column to hold the index name - PP on 29Jan2013
            SchemaDataTable.Columns.Add(new DataColumn("IndexName", Type.GetType("System.String")));

            DataTable RelationDataTable = new DataTable();
            RelationDataTable.TableName = "RelationTables";
            RelationDataTable.Columns.Add(new DataColumn("ParentTableId", Type.GetType("System.Int32")));
            RelationDataTable.Columns.Add(new DataColumn("ChildTableId", Type.GetType("System.Int32")));
            RelationDataTable.Columns.Add(new DataColumn("ParentKey", Type.GetType("System.Int32")));
            RelationDataTable.Columns.Add(new DataColumn("ChildKey", Type.GetType("System.Int32")));

            string tableName = string.Empty;
            string tableType = string.Empty;

            DataTable indexTypeDataTable = new DataTable("indexTypeDataTable");
            indexTypeDataTable = coeDAL.GetIndexTypeFields(dataBase);
            DataView indexTypeDataView = null;
            if (indexTypeDataTable != null)
            {
                indexTypeDataView = new DataView(indexTypeDataTable, "", "table_name, column_name", DataViewRowState.CurrentRows);
            }

            //get the index names of selected database from all_indexes table - PP on 29Jan2013
            DataTable indexNameDataTable = new DataTable("indexNameDataTable");
            indexNameDataTable = coeDAL.GetIndexFields(dataBase);
            DataView indexNameDataView = null;
            if (indexNameDataTable != null)
            {
                indexNameDataView = new DataView(indexNameDataTable, "", "table_name, column_name", DataViewRowState.CurrentRows);
            }

            try
            {
                foreach (DataRow drSchema in DbTables.Tables[0].Rows)
                {
                    if (tableName + tableType != drSchema[SchemaTableName].ToString() + drSchema[SchemaTableType].ToString())
                    {
                        tableName = drSchema[SchemaTableName].ToString();
                        tableType = drSchema[SchemaTableType].ToString();
                        TableId = ++ColumnId;
                    }
                    ColumnId++;
                    DataRow rowSchema = SchemaDataTable.NewRow();
                    rowSchema["TableId"] = TableId;
                    rowSchema["TableName"] = tableName;
                    rowSchema["ColumnId"] = ColumnId;
                    rowSchema["ColumnName"] = drSchema[SchemaColumnName].ToString();
                    rowSchema["DataType"] = drSchema[SchemaDataType].ToString().Trim();
                    rowSchema["DataLength"] = drSchema[SchemaDataLength] is System.DBNull ? 0 : Convert.ToInt32(drSchema[SchemaDataLength]);
                    rowSchema["DataPrecision"] = drSchema[SchemaDataPrecision] is System.DBNull ? 0 : Convert.ToInt32(drSchema[SchemaDataPrecision]);
                    rowSchema["DataScale"] = drSchema[SchemaDataScale] is System.DBNull ? 0 : Convert.ToInt32(drSchema[SchemaDataScale]);
                    rowSchema["TableType"] = drSchema[SchemaTableType].ToString().Trim();
                    if (indexTypeDataView != null)
                    {
                        // indexTypeLookup
                        DataRowView[] indexTypeLookupRows = indexTypeDataView.FindRows(new object[] { tableName, drSchema[SchemaColumnName].ToString() });

                        if (indexTypeLookupRows.Length > 0)
                        {
                            rowSchema["IndexType"] = IndexTypeClobValue;
                        }
                    }

                    //add index name to the schema table row
                    if (indexNameDataView != null)
                    {
                        //index name lookup
                        DataRowView[] indexNameLookupRows = indexNameDataView.FindRows(new object[] { tableName, drSchema[SchemaColumnName].ToString() });

                        if (indexNameLookupRows.Length > 0)
                        {
                            rowSchema["IndexName"] = indexNameLookupRows[0]["indexName"];
                        }
                    }

                    if (drSchema[SchemaFieldIsPrimaryKey].ToString().ToUpper() == "TRUE")
                        rowSchema["IsPrimaryKey"] = true;
                    else
                        rowSchema["IsPrimaryKey"] = false;

                    SchemaDataTable.Rows.Add(rowSchema);
                }

                DataView DataTablesDataView = new DataView(SchemaDataTable, "", "TableName, ColumnName", DataViewRowState.CurrentRows);

                foreach (DataRow drRelation in DbTables.Tables[1].Rows)
                {
                    DataRowView[] ParentKeysLookupRows = DataTablesDataView.FindRows(new object[] { drRelation[RelationFieldPkTableName].ToString(), drRelation[RelationFieldPkColumnName].ToString() });
                    DataRowView[] ChildKeysLookupRows = DataTablesDataView.FindRows(new object[] { drRelation[RelationFieldFkTableName].ToString(), drRelation[RelationFieldFkColumnName].ToString() });

                    if (ParentKeysLookupRows.Length > 0 && ChildKeysLookupRows.Length > 0)
                    {
                        DataRow rowRelation = RelationDataTable.NewRow();

                        foreach (DataRowView TablesDataRowView in ParentKeysLookupRows)
                        {
                            rowRelation["ParentTableId"] = (int)TablesDataRowView["TableId"];
                            rowRelation["ParentKey"] = (int)TablesDataRowView["ColumnId"];
                        }
                        foreach (DataRowView RelationDataRowView in ChildKeysLookupRows)
                        {
                            rowRelation["ChildTableId"] = (int)RelationDataRowView["TableId"];
                            rowRelation["ChildKey"] = (int)RelationDataRowView["ColumnId"];
                        }
                        RelationDataTable.Rows.Add(rowRelation);
                    }
                }
            }
            catch (Exception ex)
            {
            }

            DataSet FormatedDataSet = new DataSet();
            FormatedDataSet.Tables.Add(RelationDataTable);
            FormatedDataSet.Tables.Add(SchemaDataTable);

            return FormatedDataSet;
        }

        /// <summary>
        /// Check the Database columns Data type and Concvert it to COEDataView's AbstractTypes  
        /// </summary>
        /// <param name="DataTypes">Data Types</param>
        /// <returns>Datatype Compatible to COEDataView class</returns>
        private COEDataView.AbstractTypes GetDataTypes(string dataTypes, int dataPrecision, int dataScale)
        {
            COEDataView.AbstractTypes AbstractDataType = new COEDataView.AbstractTypes();
            switch (dataTypes.Trim())
            {
                case "BLOB":
                    AbstractDataType = Common.COEDataView.AbstractTypes.BLob;
                    break;
                case "CLOB":
                    AbstractDataType = Common.COEDataView.AbstractTypes.CLob;
                    break;
                case "CHAR":
                case "VARCHAR2":
                    AbstractDataType = COEDataView.AbstractTypes.Text;
                    break;
                case "DATE":
                    AbstractDataType = COEDataView.AbstractTypes.Date;
                    break;
                case "NUMBER":
                    if (dataScale > 0)
                        AbstractDataType = COEDataView.AbstractTypes.Real;
                    else
                    {
                        if (dataPrecision > 0)
                            AbstractDataType = COEDataView.AbstractTypes.Integer;
                        else
                            AbstractDataType = COEDataView.AbstractTypes.Real;
                    }
                    break;
                case "FLOAT":
                    AbstractDataType = COEDataView.AbstractTypes.Real;
                    break;
                case "INTEGER":
                case "LONG":
                    AbstractDataType = COEDataView.AbstractTypes.Integer;
                    break;
                default:
                    if (dataTypes.Contains("TIMESTAMP"))
                    {
                        AbstractDataType = COEDataView.AbstractTypes.Date;
                    }
                    else
                    {
                        AbstractDataType = COEDataView.AbstractTypes.Text;
                    }

                    break;
            }

            return AbstractDataType;
        }

        /// <summary>
        /// load DAL and Populate Tables Schema and Relations from the database
        /// </summary>
        /// <returns>dataset contains tables schame and relations</returns>
        private DataSet GetDataSet(DAL coeDAL, string dataBase)
        {
            DataSet DatabaseSchema = new DataSet();
            DatabaseSchema = coeDAL.Get_DatabaseSchema(dataBase);
            return DatabaseSchema;
        }

        /// <summary>
        /// Validates and Creates default dataview for and application
        /// </summary>
        /// <param name="tables">array of tables to be published</param>
        /// <param name="publishedDatabaseName">name of database that the tables are in</param>
        public void PublishTables(string[] tables, string publishedDatabaseName)
        {
            return;
        }

        #endregion

        /// <summary>
        /// Retrieves the index information from the selected database and table.
        /// </summary>
        /// <param name="database">Name of the database, used as database owner.</param>
        /// <param name="table">Name of the table to find the index information</param>
        /// <returns>Returns datatable containing information about the index fields and columns on the selected database.</returns>
        public static DataTable GetFieldIndexes(string database, string tableName)
        {
            try
            {
                COEDatabaseBO theDatabaseBO = new COEDatabaseBO();
                return theDatabaseBO.RetrieveFieldIndexInfoTable(database, tableName);
            }
            catch
            {
                throw;
            }
        }

        private DataTable RetrieveFieldIndexInfoTable(string database, string tableName)
        {
            try
            {
                // var databaseData = ConfigurationUtilities.GetDatabaseData(database);
                var instanceName = database.Contains(".") ? database.Split('.')[0] : ConfigurationUtilities.GetCOEInstance().InstanceName;
                var databaseOwner = database.Contains(".") ? database.Split('.')[1] : database;
                var instanceData = ConfigurationUtilities.GetInstanceData(instanceName);

                _globalDAL = CrossInstanceDALFactory.GetInstanceGlobalDAL<DAL>(instanceData.Name, _serviceName);
                return _globalDAL.GetIndexFields(databaseOwner, tableName);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Function for create index on database
        /// </summary>
        /// <param name="DatabaseName">Name of the database</param>
        /// <param name="TableName">Name of Table</param>
        /// <param name="ColumnName">Name of column on which index to be created</param>
        /// <returns>success of index creation</returns>
        public static Boolean CreateIndex(string DataBaseName, string TableName, string FieldName)
        {
            try
            {
                COEDatabaseBO theDatabaseBO = new COEDatabaseBO();
                return theDatabaseBO._CreateIndex(DataBaseName, TableName, FieldName);
            }
            catch
            {
                throw;
            }
        }

        private Boolean _CreateIndex(string DataBaseName, string TableName, string FieldName)
        {
            try
            {
                var databaseData = ConfigurationUtilities.GetDatabaseData(DataBaseName);
                var instanceData = ConfigurationUtilities.GetInstanceData(databaseData.InstanceId);

                _globalDAL = CrossInstanceDALFactory.GetInstanceGlobalDAL<DAL>(instanceData.Name, _serviceName);
                return _globalDAL.CreateIndex(databaseData.Owner, TableName, FieldName);
            }
            catch
            {
                throw;
            }
        }
    }
}
