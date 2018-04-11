using System;
using System.Data;
using System.Collections.Generic;
using System.Data.SqlClient;
using Csla;
using Csla.Data;
using Csla.Validation;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.Properties;
using CambridgeSoft.COE.Framework.COELoggingService;
using CambridgeSoft.COE.Framework.COESecurityService;
using CambridgeSoft.COE.Framework.COEConfigurationService;
using CambridgeSoft.COE.Framework.COEDatabasePublishingService;
using System.Xml;
using CambridgeSoft.COE.Framework.Caching;
using System.Linq;
namespace CambridgeSoft.COE.Framework.COEDataViewService
{
    [Serializable()]
    public class COEDataViewBO : Csla.BusinessBase<COEDataViewBO>, IComparable<COEDataViewBO>, ICacheable
    {
        #region Member variables
        //declare members (Fields inside Database Tables)
        private int _id = -1;
        private string _databaseName = string.Empty;
        private string _name = string.Empty;
        private SmartDate _dateCreated = new SmartDate(true);
        private string _description = string.Empty;
        private bool _isPublic = false;
        private string _userName = string.Empty;
        private int _formGroup = 0;
        private COEDataView _coeDataView = null;
        private COEAccessRightsBO _coeAccessRights = null;
        private string _application = string.Empty;

        private COEDataViewManagerBO _dataViewManager = null;
        private bool _saveFromDataViewManager = false;

        //variables data access
        [NonSerialized]
        private DAL _coeDAL = null;
        [NonSerialized]
        private DALFactory _dalFactory = new DALFactory();
        private string _serviceName = "COEDataView";

        [NonSerialized]
        private string _specialFolder = COEConfigurationBO.ConfigurationBaseFilePath + @"SimulationFolder\Framework\";
        [NonSerialized]
        static COELog _coeLog = COELog.GetSingleton("COEDataView");
        [NonSerialized]
        private static ApplicationData _appConfigData;
        [NonSerialized]
        private static bool _forceLoad = false;
        #endregion

        #region Properties

        /// <summary>
        /// True - When existing dataview will be opened first time, then data will retrieved from Db, not from cache
        /// False - Dataview information will be retrived from cache
        /// </summary>
        public static bool ForceLoad
        {
            get { return _forceLoad; }
            set { _forceLoad = value; }
        }

        public COEAccessRightsBO COEAccessRights
        {
            get { return _coeAccessRights; }
            set
            {
                if (this.COEDataView != null && this._coeAccessRights != value)
                {
                    _coeAccessRights = value;
                    PropertyHasChanged("COEAccessRights");
                }
            }
        }

        public string BaseTable
        {
            get { return this.COEDataView.BaseTableName; }
        }

        public string DatabaseName
        {
            get { return _databaseName; }
            set
            {
                if (value == null)
                    value = string.Empty;
                if (!_databaseName.Equals(value))
                {
                    if (this.COEDataView != null && this.COEDataView.Database != value)
                    {
                        this.COEDataView.Database = value;
                        PropertyHasChanged("COEDataView");
                    }
                    if (this.DataViewManager != null)
                        this.DataViewManager.DataBase = value;

                    _databaseName = value;

                    PropertyHasChanged("DatabaseName");
                }
            }
        }

        [System.ComponentModel.DataObjectField(true, true)]
        public int ID
        {
            get
            {
                CanReadProperty("ID", true);
                return _id;
            }
            set
            {
                if (this.COEDataView != null)
                    this.COEDataView.DataViewID = value;
                if (this.DataViewManager != null)
                    this.DataViewManager.DataViewId = value;
                _id = value;
            }
        }

        public string Name
        {
            get
            {
                CanReadProperty("Name", true);
                if (this.COEDataView != null && !string.IsNullOrEmpty(this.COEDataView.Name) && _name != this.COEDataView.Name)
                    _name = this.COEDataView.Name;
                return _name;
            }
            set
            {
                if (value == null) value = string.Empty;
                if (!_name.Equals(value))
                {
                    if (this.COEDataView != null && this.COEDataView.Name != value)
                    {
                        this.COEDataView.Name = value;
                        PropertyHasChanged("COEDataView");
                    }
                    if (this.DataViewManager != null)
                        this.DataViewManager.Name = value;

                    _name = value;
                    PropertyHasChanged("Name");
                }
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

        public string Description
        {
            get
            {
                CanReadProperty("Description", true);
                if (this.COEDataView != null && !string.IsNullOrEmpty(this.COEDataView.Description) && _description != this.COEDataView.Description)
                    _description = this.COEDataView.Description;
                return _description;
            }
            set
            {
                if (value == null) value = string.Empty;
                if (!_description.Equals(value))
                {
                    if (this.COEDataView != null && this.COEDataView.Description != value)
                    {
                        this.COEDataView.Description = value;
                        PropertyHasChanged("COEDataView");
                    }
                    if (this.DataViewManager != null)
                        this.DataViewManager.Description = value;

                    _description = value;
                    PropertyHasChanged("Description");
                }
            }
        }

        public bool IsPublic
        {
            get
            {
                CanReadProperty("IsPublic", true);
                return _isPublic;
            }
            set
            {

                if (!_isPublic.Equals(value))
                {
                    _isPublic = value;
                    PropertyHasChanged("IsPublic");
                }
            }
        }

        public string UserName
        {
            get
            {
                CanReadProperty("UserName", true);
                return _userName;
            }
            set
            {
                if (value == null) value = string.Empty;
                if (!_userName.Equals(value))
                {
                    _userName = value;
                    PropertyHasChanged("UserName");
                }
            }
        }

        public int FormGroup
        {
            get
            {
                CanReadProperty("FormGroup", true);
                return _formGroup;
            }
            set
            {

                if (!_formGroup.Equals(value))
                {
                    _formGroup = value;
                    PropertyHasChanged("FormGroup");
                }
            }
        }

        public string Application
        {
            get
            {
                CanReadProperty("Application", true);
                if (this.COEDataView != null && !string.IsNullOrEmpty(this.COEDataView.Application) && _application != this.COEDataView.Application)
                    _application = this.COEDataView.Application;
                return _application;
            }
            set
            {
                if (value == null) value = string.Empty;
                if (!_application.Equals(value))
                {
                    if (this.COEDataView != null && this.COEDataView.Application != value)
                    {
                        this.COEDataView.Application = value;
                        PropertyHasChanged("COEDataView");
                    }
                    if (this.DataViewManager != null)
                        this.DataViewManager.Application = value;

                    _application = value;
                    PropertyHasChanged("Application");
                }
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

        public COEDataViewManagerBO DataViewManager
        {
            get
            {
                CanReadProperty("DataViewManager", true);
                return _dataViewManager;
            }
            set
            {
                _dataViewManager = value;
                PropertyHasChanged("DataViewManager");
            }
        }

        private static ApplicationData AppConfigData
        {
            get
            {
                if (_appConfigData == null && !string.IsNullOrEmpty(COEAppName.Get()))
                    _appConfigData = ConfigurationUtilities.GetApplicationData(COEAppName.Get());

                return _appConfigData;
            }
        }

        public static CacheItemData CacheConfig
        {
            get
            {
                try
                {
                    if (AppConfigData != null && AppConfigData.CachingData != null && AppConfigData.CachingData.Dataview != null)
                    {
                        return AppConfigData.CachingData.Dataview;
                    }
                }
                catch (Exception)
                {
                }
                return new CacheItemData();
            }
        }

        protected override object GetIdValue()
        {
            return _id;
        }

        public override bool IsDirty
        {
            get
            {
                return base.IsDirty || this.DataViewManager.IsDirty;
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
        #endregion

        #region Delegates

        public static Comparison<COEDataViewBO> IDComparison_ASC = delegate(COEDataViewBO dv1, COEDataViewBO dv2)
        {
            return dv1.ID.CompareTo(dv2.ID);
        };
        public static Comparison<COEDataViewBO> IDComparison_DESC = delegate(COEDataViewBO dv1, COEDataViewBO dv2)
        {
            return -1 * dv1.ID.CompareTo(dv2.ID);
        };

        public static Comparison<COEDataViewBO> DataBaseComparison_ASC = delegate(COEDataViewBO dv1, COEDataViewBO dv2)
        {
            return dv1.DatabaseName.CompareTo(dv2.DatabaseName);
        };

        public static Comparison<COEDataViewBO> DataBaseComparison_DESC = delegate(COEDataViewBO dv1, COEDataViewBO dv2)
        {
            return -1 * dv1.DatabaseName.CompareTo(dv2.DatabaseName);
        };

        public static Comparison<COEDataViewBO> NameComparison_ASC = delegate(COEDataViewBO dv1, COEDataViewBO dv2)
        {
            return dv1.Name.CompareTo(dv2.Name);
        };

        public static Comparison<COEDataViewBO> NameComparison_DESC = delegate(COEDataViewBO dv1, COEDataViewBO dv2)
        {
            return -1 * dv1.Name.CompareTo(dv2.Name);
        };

        public static Comparison<COEDataViewBO> BaseTableComparison_ASC = delegate(COEDataViewBO dv1, COEDataViewBO dv2)
        {
            return dv1.BaseTable.CompareTo(dv2.BaseTable);
        };

        public static Comparison<COEDataViewBO> BaseTableComparison_DESC = delegate(COEDataViewBO dv1, COEDataViewBO dv2)
        {
            return -1 * dv1.BaseTable.CompareTo(dv2.BaseTable);
        };

        #endregion

        #region Constructors

        private COEDataViewBO()
        { }

        //constructor to be called from queryCriteriaList as well as any other services that needs to construct this object
        internal COEDataViewBO(int id, string name, string description, string userID, bool isPublic, int formGroup, SmartDate dateCreated, COEDataView coeDataView, string databaseName)
        {
            _id = id;
            Name = name;
            Description = description;
            _userName = userID;
            _dateCreated = dateCreated;
            _isPublic = isPublic;
            _formGroup = formGroup;
            _coeDataView = coeDataView;
            _application = string.Empty;
            if (coeDataView != null && coeDataView.DataViewID < 0)
            {
                coeDataView.DataViewID = this.ID;
            }
            _databaseName = databaseName;
            if (_id > 0 && !_isPublic)
            {
                this.COEAccessRights = new COEAccessRightsBO();
                this.COEAccessRights = COEAccessRightsBO.Get(COEAccessRightsBO.ObjectTypes.COEDATAVIEW, this._id);
            }
            MarkAsChild();

        }

        //constructor to be called from queryCriteriaList as well as any other services that needs to construct this object
        internal COEDataViewBO(int id, string name, string description, string userID, bool isPublic, int formGroup, SmartDate dateCreated, COEDataView coeDataView, string databaseName, COEAccessRightsBO coeAccessRights)
        {
            _id = id;
            Name = name;
            Description = description;
            _userName = userID;
            _dateCreated = dateCreated;
            _isPublic = isPublic;
            _formGroup = formGroup;
            _coeDataView = coeDataView;
            _databaseName = databaseName;
            _coeAccessRights = COEAccessRights;
            _application = string.Empty;
        }

        internal COEDataViewBO(int id, string name, string description, string userID, bool isPublic, int formGroup, SmartDate dateCreated, COEDataView coeDataView, string databaseName, COEAccessRightsBO coeAccessRights, string application)
        {
            _id = id;
            Name = name;
            Description = description;
            _userName = userID;
            _dateCreated = dateCreated;
            _isPublic = isPublic;
            _formGroup = formGroup;
            _coeDataView = coeDataView;
            _databaseName = databaseName;
            _coeAccessRights = COEAccessRights;
            _application = application;
        }


        internal COEDataViewBO(COEDataView coeDataView)
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
            ValidationRules.AddRule(CommonRules.StringMaxLength, new CommonRules.MaxLengthRuleArgs("Description", 255));
            //
            // DateCreated
            //
            //ValidationRules.AddRule(CommonRules.MinValue<DateTime>, new CommonRules.MinValueRuleArgs<DateTime>("DateCreated", new DateTime(2005, 1, 1)));
            //ValidationRules.AddRule(CommonRules.RegExMatch, new CommonRules.RegExRuleArgs("DateCreated", @"(0[1-9]|1[012])[- /.](0[1-9]|[12][0-9]|3[01])[- /.](19|20)\d\d"));

            //Application
            ValidationRules.AddRule(CommonRules.StringMaxLength, new CommonRules.MaxLengthRuleArgs("Application", 50));
        }

        protected override void AddInstanceBusinessRules()
        {
            base.AddInstanceBusinessRules();
        }

        protected override void AddBusinessRules()
        {
            AddCommonRules();
        }

        #endregion //Validation Rules

        #region Factory Methods

        //this method must be called prior to any other method inorder to set the database that the dal will use
        internal static void SetDatabaseName()
        {
            COEDatabaseName.Set(Resources.CentralizedStorageDB);
        }


        internal static void SetDatabaseName(string databaseName)
        {
            COEDatabaseName.Set(Resources.CentralizedStorageDB);

        }
        public static COEDataViewBO New()
        {

            SetDatabaseName();
            if (!CanAddObject())
                throw new System.Security.SecurityException(Resources.UserNotAuthorizedForAddObject + " COEDataViewBO");

            return DataPortal.Create<COEDataViewBO>(new CreateNewCriteria());
        }


        public static COEDataViewBO New(string name, string description, COEDataView dataView, COEAccessRightsBO COEAccessRights)
        {
            SetDatabaseName();
            if (!CanAddObject())
                throw new System.Security.SecurityException(Resources.UserNotAuthorizedForAddObject + " COEDataViewBO");

            return DataPortal.Create<COEDataViewBO>(new CreateBasedOnCriteria(name, description, dataView, dataView.Database, COEAccessRights, string.Empty));

        }

        public static COEDataViewBO New(string name, string description, COEDataView dataView, COEAccessRightsBO COEAccessRights, string application)
        {
            SetDatabaseName();
            if (!CanAddObject())
                throw new System.Security.SecurityException(Resources.UserNotAuthorizedForAddObject + " COEDataViewBO");

            return DataPortal.Create<COEDataViewBO>(new CreateBasedOnCriteria(name, description, dataView, dataView.Database, COEAccessRights, application));
        }

        /// <summary>
        /// Method to get an object based on other (Same info just different id, name and Description)
        /// </summary>
        /// <param name="id">The id of the object that we based to create this</param>
        /// <param name="name">Name of the new object</param>
        /// <param name="description">Description of the new object</param>
        /// <returns></returns>
        public static COEDataViewBO Clone(int id, string name, string description)
        {
            SetDatabaseName();
            if (!CanAddObject())
                throw new System.Security.SecurityException(Resources.UserNotAuthorizedForAddObject + " COEDataViewBO");
            COEDataViewBO retVal = DataPortal.Fetch<COEDataViewBO>(new CloneCriteria(id, name, description, string.Empty));
            retVal.MarkNew(); //In case to call a Save, it will be inserted into the DB.
            return retVal;
        }

        /// <summary>
        /// Method to get an object based on other (Same info just different id, name and Description)
        /// </summary>
        /// <param name="id">The id of the object that we based to create this</param>
        /// <param name="name">Name of the new object</param>
        /// <param name="description">Description of the new object</param>
        /// <param name="application">The application the dataview belongs to</param>
        /// <returns></returns>
        public static COEDataViewBO Clone(int id, string name, string description, string application)
        {
            SetDatabaseName();
            if (!CanAddObject())
                throw new System.Security.SecurityException(Resources.UserNotAuthorizedForAddObject + " COEDataViewBO");
            COEDataViewBO retVal = DataPortal.Fetch<COEDataViewBO>(new CloneCriteria(id, name, description, application));
            retVal.MarkNew(); //In case to call a Save, it will be inserted into the DB.
            return retVal;
        }

        /// <summary>
        /// Method to get an object based on other (Same info just different id, name, Description, and fromMaster)
        /// </summary>
        /// <param name="id">The id of the object that we based to create this</param>
        /// <param name="name">Name of the new object</param>
        /// <param name="description">Description of the new object</param>
        /// <param name="fromMaster">Default value for the FromMaster attribute for tables. (checked or unchecked in dvmanager)</param>
        /// <returns></returns>
        public static COEDataViewBO Clone(int id, string name, string description, bool fromMaster)
        {
            COEDataViewBO retVal = Clone(id, name, description);
            foreach (TableBO table in retVal.DataViewManager.Tables)
            {
                table.FromMasterSchema = fromMaster;
            }
            return retVal;
        }


        public static COEDataViewBO Get(int id)
        {
            string idString = id.ToString();
            SetDatabaseName();
            COEDataViewBO result = null;
            if (!CanGetObject(id))
                throw new System.Security.SecurityException(Resources.DataViewPermissions);

            if (id == 0)
                return DataPortal.Fetch<COEDataViewBO>(new Criteria(id, false));

            if (_forceLoad == true)
            {
                //kill the local cache object
                LocalCache.Remove(idString, typeof(COEDataViewBO));
                result = DataPortal.Fetch<COEDataViewBO>(new Criteria(id, false));
                LocalCache.Add(idString, typeof(COEDataViewBO), result, CacheConfig.AbsoluteExpiration, CacheConfig.SlidingExpiration, CacheConfig.DefaultPriority);
            }
            else
            {
                switch (CacheConfig.Cache)
                {
                    case CacheType.Disabled:
                        result = DataPortal.Fetch<COEDataViewBO>(new Criteria(id, false));
                        break;
                    case CacheType.ClientCache:
                        result = LocalCache.Get(idString, typeof(COEDataViewBO)) as COEDataViewBO;
                        if (result == null)
                        {
                            result = DataPortal.Fetch<COEDataViewBO>(new Criteria(id, false));
                            LocalCache.Add(idString, typeof(COEDataViewBO), result, CacheConfig.AbsoluteExpiration, CacheConfig.SlidingExpiration, CacheConfig.DefaultPriority);
                        }
                        break;
                    case CacheType.ServerCache:
                        result = ServerCache.Get(idString, typeof(COEDataViewBO)) as COEDataViewBO;
                        if (result == null)
                        {
                            result = DataPortal.Fetch<COEDataViewBO>(new Criteria(id, false));
                        }
                        break;
                    case CacheType.ServerAndClientCache:

                        object localCacheTime = LocalCache.GetDate(idString, typeof(COEDataViewBO));
                        object serverCacheTime = ServerCache.GetDate(idString, typeof(COEDataViewBO));

                        //If the server cache isn't there then add it.
                        //We are checking for a date rather than the dataview but it probably doesn't matter.)
                        if (serverCacheTime == null)
                        {
                            //This will create the server cache entry if it wasn't there
                            result = DataPortal.Fetch<COEDataViewBO>(new Criteria(id, false));

                            //now get the timestamp for the server cache
                            serverCacheTime = ServerCache.GetDate(idString, typeof(COEDataViewBO));
                        }

                        if ((localCacheTime != null && ((DateTime)localCacheTime < (DateTime)serverCacheTime)) || localCacheTime == null)
                        {
                            //kill the local cache object
                            LocalCache.Remove(idString, typeof(COEDataViewBO));

                            //if you didn't have to create the server cache but need the server cache to set the local cache
                            if (result == null)
                            {
                                //This will create the server cache entry if it wasn't there
                                result = ServerCache.Get(idString, typeof(COEDataViewBO)) as COEDataViewBO;
                                serverCacheTime = ServerCache.GetDate(idString, typeof(COEDataViewBO));
                            }
                            LocalCache.Add(idString, typeof(COEDataViewBO), result, CacheConfig.AbsoluteExpiration, CacheConfig.SlidingExpiration, CacheConfig.DefaultPriority, (DateTime)serverCacheTime);
                        }

                        //since we now always have an updated localcache we can can always return it
                        result = LocalCache.Get(idString, typeof(COEDataViewBO)) as COEDataViewBO;

                        break;
                }
            }

            return result;
        }

        public static COEDataViewBO Get(int id, bool includeAccessRights)
        {
            COEDataViewBO result = Get(id);
            if (includeAccessRights)
            {
                result.COEAccessRights = COEAccessRightsBO.Get(COEAccessRightsBO.ObjectTypes.COEDATAVIEW, id);

            }

            return result;
        }

        /// <summary>
        /// Gets a dataview from a hitlist.
        /// </summary>
        /// <param name="hitlistType">The type of the hitlist, (TEMP|SAVED|MARKED) <see cref="HitListType"/></param>
        /// <param name="hitlistValue">The hitlist id</param>
        /// <returns>The dataview the hitlist was created from</returns>
        public static COEDataViewBO Get(HitListType hitlistType, int hitlistValue)
        {
            COEHitListService.COEHitListBO hlBO = COEHitListService.COEHitListBO.Get(hitlistType, hitlistValue);
            return Get(hlBO.DataViewID);
        }

        /// <summary>
        /// Returns the Master DataView
        /// </summary>
        /// <returns>Master DataView</returns>
        /// <remarks>This method assums the Master DataView Id = 0</remarks>
        public static COEDataViewBO GetMasterSchema()
        {
            return Get(0, false);
        }

        public static void Delete(int id)
        {
            SetDatabaseName();
            if (!CanDeleteObject())
                throw new System.Security.SecurityException(Resources.UserNotAuthorizedForDeleteObject + " COEDataViewBO");


            switch (CacheConfig.Cache)
            {
                case CacheType.ClientCache:
                    LocalCache.Remove(id.ToString(), typeof(COEDataViewBO));
                    break;
                case CacheType.ServerCache:
                    if (ServerCache.Exists(id.ToString(), typeof(COEDataViewBO)))
                        ServerCache.Remove(id.ToString(), typeof(COEDataViewBO));
                    break;
                case CacheType.ServerAndClientCache:
                    LocalCache.Remove(id.ToString(), typeof(COEDataViewBO));
                    if (ServerCache.Exists(id.ToString(), typeof(COEDataViewBO)))
                        ServerCache.Remove(id.ToString(), typeof(COEDataViewBO));
                    break;
            }

            DataPortal.Delete(new Criteria(id, false));
        }

        public void SetMasterDVID()
        {
            _id = 0;
        }

        public override COEDataViewBO Save()
        {
            SetDatabaseName();
            if (IsDeleted && !CanDeleteObject())
                throw new System.Security.SecurityException(Resources.UserNotAuthorizedForDeleteObject + " COEDataViewBO");
            else if (IsNew && !CanAddObject())
                throw new System.Security.SecurityException(Resources.UserNotAuthorizedForAddObject + " COEDataViewBO");
            else if (!CanEditObject())
                throw new System.Security.SecurityException(Resources.UserNotAuthorizedForEditObject + " COEDataViewBO");
            return base.Save();
        }

        public COEDataViewBO SaveFromDataViewManager()
        {
            _saveFromDataViewManager = true;
            if (GetMasterSchema().DataViewManager == null && _id == 0) //TODO: Avoid harcoded 0 - Master Schema.
                this.MarkNew();
            SetDatabaseName();
            if (IsDeleted && !CanDeleteObject())
                throw new System.Security.SecurityException(Resources.UserNotAuthorizedForDeleteObject + " COEDataViewBO");
            else if (IsNew && !CanAddObject())
                throw new System.Security.SecurityException(Resources.UserNotAuthorizedForAddObject + " COEDataViewBO");
            else if (!CanEditObject())
                throw new System.Security.SecurityException(Resources.UserNotAuthorizedForEditObject + " COEDataViewBO");

            EncodeFields();
            return base.Save();
        }

        /// <summary>
        /// Fields alias are updated in encoded format.//CBOE-1087
        /// </summary>
        private void EncodeFields()
        {
            IEnumerable<FieldBO> listTables = null;
            foreach (var item in this.DataViewManager.Tables)
            {
                listTables = from C in item.Fields
                             where (C.Alias.Contains("<") || C.Alias.Contains(">") || C.Alias.Contains("&") || C.Alias.Contains("%"))
                             select C;
                if (listTables != null)
                {
                    foreach (FieldBO fldItem in listTables)
                    {
                        if (fldItem != null)
                        {
                            fldItem.Alias = System.Web.HttpUtility.HtmlEncode(fldItem.Alias);

                        }
                    }
                }
            }
        }

        /// <summary>
        /// this method compares the tables in two dataview
        /// </summary>
        /// <param name="dataView"></param>
        /// <returns></returns>
        internal bool DataViewAreEquivalent(COEDataView dataView)
        {
            // This "hack" is because if we are on simulation mode the DataViewManager is never initialized
            // TODO: Check what to do when simulation mode is being used.
            if (!CambridgeSoft.COE.Framework.Common.Utilities.SimulationMode())
            {
                TableListBO storedTables = this.DataViewManager.Tables;
                COEDataViewManagerBO dvManager = new COEDataViewManagerBO(dataView);
                TableListBO inputTables = dvManager.Tables;

                foreach (TableBO table in inputTables)
                {
                    if (!storedTables.Exists(table))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public static bool CanAddObject()
        {
            // return Csla.ApplicationContext.User.IsInRole("CanSearch");
            //if (_id == -1)
            //    throw new Types.Exceptions.DataviewException(Resources.DataViewPermissions);
            return true;
        }

        public static bool CanGetObject(int id)
        {
            return new CanGetDataviewCommand(id).Execute();
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

        #region Criterias
        [Serializable()]
        private class Criteria
        {

            internal int _id;
            internal bool _includeAccessRights;

            //constructors
            public Criteria(int id, bool includeAccessRights)
            {
                _id = id;
                _includeAccessRights = includeAccessRights;
            }
        }

        [Serializable()]
        protected class CreateNewCriteria
        {

            public CreateNewCriteria()
            {
            }
        }
        [Serializable()]
        private class CloneCriteria
        {
            internal int _id;
            internal string _name = String.Empty;
            internal string _description = String.Empty;
            internal string _application = String.Empty;
            public CloneCriteria(int id, string name, string description, string application)
            {
                _id = id;
                _name = name;
                _description = description;
                _application = application;
            }
        }

        [Serializable()]
        protected class CreateBasedOnCriteria
        {
            internal string _name = String.Empty;
            internal string _database;
            internal string _description = String.Empty;
            internal string _application = String.Empty;
            internal COEDataView _dataView = null;
            internal COEAccessRightsBO _coeAccessRights;
            public CreateBasedOnCriteria(string name, string description, COEDataView dataView, string database, COEAccessRightsBO coeAccessRights, string application)
            {
                if (!string.IsNullOrEmpty(name))
                    _name = name;
                if (!string.IsNullOrEmpty(description))
                    _description = description;
                if (dataView != null)
                    _dataView = dataView;
                if (database != null)
                    _database = database;
                if (!string.IsNullOrEmpty(application))
                    _application = application;
                if (_coeAccessRights != null && ((_coeAccessRights.Users != null && _coeAccessRights.Users.Count > 0) || (_coeAccessRights.Roles != null && _coeAccessRights.Roles.Count > 0)))
                    _coeAccessRights = coeAccessRights;
            }
        }
        #endregion //Criterias

        #region Data Access - Create
        [RunLocal]
        private void DataPortal_Create(CreateNewCriteria criteria)
        {
            _coeDataView = new COEDataView();
            if (this.DataViewManager == null)
                this.DataViewManager = COEDataViewManagerBO.NewManager(this.COEDataView);
            this.MarkNew();
        }

        [RunLocal]
        private void DataPortal_Create(CreateBasedOnCriteria criteria)
        {
            _coeDataView = criteria._dataView == null ? new COEDataView() : criteria._dataView;

            Name = criteria._name;
            Description = criteria._description;
            Application = criteria._application;
            this.COEAccessRights = criteria._coeAccessRights;

            if (this.DataViewManager == null)
                this.DataViewManager = COEDataViewManagerBO.NewManager(this.COEDataView);
            this.MarkNew();
        }

        /// <summary>
        /// Function for create index on database
        /// </summary>
        /// <param name="DatabaseName">Name of the database</param>
        /// <param name="TableName">Name of Table</param>
        /// <param name="ColumnName">Name of column on which index to be created</param>
        /// <returns>success of index creation</returns>
        public Boolean CreateIndex(string DataBaseName, string TableName, string FieldName)
        {
            SetDatabaseName();
            if (_coeDAL == null) { LoadDAL(); }
            return COEDatabaseBO.CreateIndex(DataBaseName, TableName, FieldName);
        }

        #endregion //Data Access - Create

        #region Data Access - Fetch
        private void DataPortal_Fetch(Criteria criteria)
        {
            _coeLog.LogStart("Fetching Dataview", 1);
            if (CambridgeSoft.COE.Framework.Common.Utilities.SimulationMode())
            {
                XmlDocument xmlDocument = new XmlDocument();

                xmlDocument.Load(_specialFolder + this.GetType().Name + "_" + criteria._id + @".xml");
                _id = criteria._id;
                try
                {
                    _coeDataView = (COEDataView)COEDataViewUtilities.DeserializeCOEDataView(xmlDocument.OuterXml);
                }
                catch (Exception)
                {
                    // To allow the edition of invalid dataviews for fixing it.
                    try
                    {
                        _coeDataView = new COEDataView(xmlDocument);
                    }
                    catch (Exception)
                    {
                        _coeDataView = new COEDataView();
                    }
                }
            }
            else
            {
                if (_coeDAL == null) { LoadDAL(); }
                // Coverity Fix CID - 11485
                if (_coeDAL != null)
                {
                    using (SafeDataReader dr = _coeDAL.Get(criteria._id))
                    {
                        FetchObject(dr);
                    }
                }
                else
                    throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));
            }

            _coeLog.LogEnd("Fetching Dataview", 1);
        }

        private void DataPortal_Fetch(CloneCriteria criteria)
        {
            _coeLog.LogStart("Fetching Dataview", 1);
            if (_coeDAL == null) { LoadDAL(); }

            // Coverity Fix CID - 11484 
            if (_coeDAL != null)
            {
                using (SafeDataReader dr = _coeDAL.Get(criteria._id))
                {
                    FetchObject(dr);
                }

                if (criteria._id != 0)
                {
                    _coeAccessRights = new COEAccessRightsBO();
                    _coeAccessRights = COEAccessRightsBO.Get(COEAccessRightsBO.ObjectTypes.COEDATAVIEW, criteria._id);
                }
                Name = criteria._name;
                Description = criteria._description;
                Application = criteria._application;

                if (_coeDataView != null)
                {
                    _coeDataView.Name = criteria._name;
                    _coeDataView.Description = criteria._description;
                }
                ID = -1;
                _coeLog.LogEnd("Fetching Dataview", 1);
            }
            else
                throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));
        }

        private void FetchObject(SafeDataReader dr)
        {
            if (dr.Read())
            {
                _id = Convert.ToInt32(dr.GetInt64("ID"));
                _name = dr.GetString("NAME");
                _dateCreated = dr.GetSmartDate("DATE_CREATED", _dateCreated.EmptyIsMin);//CREATION_DATE changed to DATE_CREATED
                _description = dr.GetString("DESCRIPTION");
                _isPublic = dr.GetString("IS_PUBLIC").Equals("1");
                _userName = dr.GetString("USER_ID");
                _formGroup = dr.GetInt32("FORMGROUP");
                _application = dr.GetString("APPLICATION");
                try
                {
                    _coeDataView = (COEDataView)COEDataViewUtilities.DeserializeCOEDataView(dr.GetString("COEDATAVIEW"));
                }
                catch (Exception)
                {
                    // To allow the edition of invalid dataviews for fixing it.
                    try
                    {
                        XmlDocument doc = new XmlDocument();
                        doc.LoadXml(dr.GetString("COEDATAVIEW"));
                        _coeDataView = new COEDataView(doc);
                    }
                    catch (Exception)
                    {
                        _coeDataView = new COEDataView();
                    }
                }
                //If the dataview is fetched from server, the policy must be user server, even if was mistakenly stored as use_client
                _coeDataView.DataViewHandling = COEDataView.DataViewHandlingOptions.USE_SERVER_DATAVIEW;
                _coeDataView.Name = COEDataViewUtilities.ReplaceStringForSpecialCharToLoad(_coeDataView.Name);
                _coeDataView.Description = COEDataViewUtilities.ReplaceStringForSpecialCharToLoad(_coeDataView.Description);

                _databaseName = dr.GetString("DATABASE"); //if we want to support different storage locations then this would be set by the input paramter
                FillEmptyValuesToMessagingType();
                if (_coeDataView != null)
                    _dataViewManager = new COEDataViewManagerBO(_coeDataView, _id, true); //There is a bug in the Deserialize line that is not setting correctly the dataviewid.
                // _coeAccessRights

            }
            this.MarkClean();
            this.MarkOld();


            if (CacheConfig.Cache == CacheType.ServerCache || CacheConfig.Cache == CacheType.ServerAndClientCache)
            {
                // As dependencies are not serializable they are created on the dataportal_fetch calls (on server side)
                _cacheDependency = _coeDAL.GetCacheDependency(_id);
                // Moreover and due to the same fact, we dont want to use the server cache because that involves a CSLA command and serialization
                // and would anyway lead to the current tier.
                // So, local cache is used.
                //LocalCache.Add(this.ID.ToString(), this.GetType(), this, CacheConfig.AbsoluteExpiration, CacheConfig.SlidingExpiration, CacheConfig.DefaultPriority);
                LocalCache.Add(this.ID.ToString(), this.GetType(), this, CacheConfig.AbsoluteExpiration, CacheConfig.SlidingExpiration, CacheConfig.DefaultPriority);

            }
        }

        #endregion //Data Access - Fetch

        #region Data Access - Insert

        internal void Insert(DAL _coeDAL)
        {
            if (_coeDAL == null) { LoadDAL(); }
            // Coverity Fix CID - 11488 
            if (_coeDAL != null)
            {
                if (_coeAccessRights != null && ((_coeAccessRights.Users != null && _coeAccessRights.Users.Count > 0) || (_coeAccessRights.Roles != null && _coeAccessRights.Roles.Count > 0))) { _isPublic = false; }
                _id = this.GenerateID();
                FillEmptyValuesToMessagingType();

                _coeDAL.Insert(_formGroup, Name, _isPublic, Description, _userName, _coeDataView, _databaseName, _id, _application);
                //call internal methods of COEAccessRightsBO to update accessRights
                if (_coeAccessRights != null && ((_coeAccessRights.Users != null && _coeAccessRights.Users.Count > 0) || (_coeAccessRights.Roles != null && _coeAccessRights.Roles.Count > 0)))
                {
                    _coeAccessRights.ObjectID = _id;
                    _coeAccessRights.ObjectType = COEAccessRightsBO.ObjectTypes.COEDATAVIEW;
                    _coeAccessRights.Save();
                }
                //Call some operation to update selects for tables
                this.MarkOld();
                this.MarkClean();
            }
            else
                throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));

        }

        private int GenerateID()
        {
            if (_id > 0)
                return _id;
            else
                return _coeDAL.GetNewID();
        }

        protected override void DataPortal_Insert()
        {
            if (_coeDAL == null) { LoadDAL(); }
            if (_coeAccessRights != null && ((_coeAccessRights.Users != null && _coeAccessRights.Users.Count > 0) || (_coeAccessRights.Roles != null && _coeAccessRights.Roles.Count > 0))) { _isPublic = false; }
            _id = this.GenerateID();

            if (this.DataViewManager != null)
                this.DataViewManager.RemoveOrphanRelationships();

            EncodeFields();

            if (_saveFromDataViewManager)
            {
                _dataViewManager.Name = COEDataViewUtilities.ReplaceStringForSpecialCharToSave(this.COEDataView.Name);
                _dataViewManager.Description = COEDataViewUtilities.ReplaceStringForSpecialCharToSave(this.COEDataView.Description);
                this.COEDataView.GetFromXML(COEDataViewUtilities.SortCoeDataview(_dataViewManager.ToString(true)));
                this.DataViewManager = COEDataViewManagerBO.NewManager(this.COEDataView);
            }
            else
            {
                this.COEDataView.Name = COEDataViewUtilities.ReplaceStringForSpecialCharToSave(this.COEDataView.Name);
                this.COEDataView.Description = COEDataViewUtilities.ReplaceStringForSpecialCharToSave(this.COEDataView.Description);
                this.COEDataView.GetFromXML(COEDataViewUtilities.SortCoeDataview(this.COEDataView.ToString()));
            }

            FillEmptyValuesToMessagingType();
            this.CheckNameIsUnique();

            this.Name = COEDataViewUtilities.ReplaceStringForSpecialCharToSave(this.Name);
            this.Description = COEDataViewUtilities.ReplaceStringForSpecialCharToSave(this.Description);
            if (!_saveFromDataViewManager)
                _id = _coeDAL.Insert(_formGroup, this.Name, _isPublic, this.Description, _userName, _coeDataView, _databaseName, _id, _application);
            else
                _id = _coeDAL.Insert(_formGroup, this.Name, _isPublic, this.Description, _userName, _dataViewManager, _databaseName, _id, _application);
            //call internal methods of COEAccessRightsBO to update accessRights
            if (_coeAccessRights != null && ((_coeAccessRights.Users != null && _coeAccessRights.Users.Count > 0) || (_coeAccessRights.Roles != null && _coeAccessRights.Roles.Count > 0)))
            {
                _coeAccessRights.ObjectID = _id;
                _coeAccessRights.ObjectType = COEAccessRightsBO.ObjectTypes.COEDATAVIEW;
                _coeAccessRights.Save();
            }

            //here we will apply grants to tables that are added or removed. This is only done
            //for the master dataview which has a dataview ID =0;
            //I believe the master is always saved through a dataview manger, so we only have to look
            //at this case
            if (_saveFromDataViewManager == true && _dataViewManager.DataViewId == 0)
            {
                COEDatabaseBO.UpdateTablePermissions(_dataViewManager);
            }
            //Call some operation to update selects for tables
            this.MarkOld();
            this.MarkClean();
        }

        #endregion

        #region Data Access - Update


        internal void Update(DAL _coeDAL)
        {
            if (_coeDAL == null) { LoadDAL(); }
            // Coverity Fix CID - 11489
            if (_coeDAL != null)
            {
                //Coverity fix - CID 11486
                if (base.IsDirty && _coeDataView != null)
                {
                    string serializedCOEDataView = COEDataViewUtilities.SerializeCOEDataView(_coeDataView);
                    if (!string.IsNullOrEmpty(serializedCOEDataView))
                    {
                        if (_coeAccessRights != null && ((_coeAccessRights.Users != null && _coeAccessRights.Users.Count > 0) || (_coeAccessRights.Roles != null && _coeAccessRights.Roles.Count > 0))) { _isPublic = false; }
                        FillEmptyValuesToMessagingType();

                        _coeDAL.Update(_id, serializedCOEDataView, this.Name, this.Description, _isPublic, _databaseName, this.Application);
                        XmlDocument doc = new XmlDocument();
                        doc.LoadXml(serializedCOEDataView);
                        this.COEDataView = new COEDataView(doc);

                        //call internal methods of COEAccessRightsBO to update accessRights
                        if (_coeAccessRights != null && ((_coeAccessRights.Users != null && _coeAccessRights.Users.Count > 0) || (_coeAccessRights.Roles != null && _coeAccessRights.Roles.Count > 0)))
                        {
                            _coeAccessRights.ObjectID = _id;
                            _coeAccessRights.ObjectType = COEAccessRightsBO.ObjectTypes.COEDATAVIEW;
                            _coeAccessRights.Save();
                        }
                        //Call some operation to update selects for tables
                        this.MarkOld();
                        this.MarkClean();
                    }
                }
            }
            else
                throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));
        }

        protected override void DataPortal_Update()
        {
            if (_coeDAL == null) { LoadDAL(); }
            string serializedCOEDataView = String.Empty;
            FillEmptyValuesToMessagingType();
            if (_dataViewManager == null && _coeDataView != null) //Seems not to be properly persisted
                _dataViewManager = new COEDataViewManagerBO(_coeDataView, _id, true);


            if (this.DataViewManager != null)  // Coverity Fix CID - 11486
                this.DataViewManager.RemoveOrphanRelationships();

            EncodeFields();

            if (!_saveFromDataViewManager)
            {
                _coeDataView.Name = COEDataViewUtilities.ReplaceStringForSpecialCharToSave(this._coeDataView.Name);
                _coeDataView.Description = COEDataViewUtilities.ReplaceStringForSpecialCharToSave(this._coeDataView.Description);
                serializedCOEDataView = COEDataViewUtilities.SerializeCOEDataView(_coeDataView);
            }
            else
            {
                if (_dataViewManager != null)  // Coverity Fix CID - 11486
                {
                    _dataViewManager.Name = COEDataViewUtilities.ReplaceStringForSpecialCharToSave(_coeDataView.Name);
                    _dataViewManager.Description = COEDataViewUtilities.ReplaceStringForSpecialCharToSave(_coeDataView.Description);
                    serializedCOEDataView = _dataViewManager.ToString(true);
                }
            }
            if (_coeAccessRights != null && ((_coeAccessRights.Users != null && _coeAccessRights.Users.Count > 0) || (_coeAccessRights.Roles != null && _coeAccessRights.Roles.Count > 0)))
            {
                if (_coeAccessRights.Roles != null || _coeAccessRights.Users != null)
                    _isPublic = false;
            }

            //here we will apply grants to tables that are added or removed. This is only done
            //for the master dataview which has a dataview ID =0;
            //I believe the master is always saved through a dataview manger, so we only have to look
            //at this case
            if (_saveFromDataViewManager == true && _dataViewManager != null && _dataViewManager.DataViewId == 0) // Coverity Fix CID - 11486
            {
                COEDatabaseBO.UpdateTablePermissions(_dataViewManager);
            }

            this.CheckNameIsUnique();
            serializedCOEDataView = COEDataViewUtilities.SortCoeDataview(serializedCOEDataView);

            _coeDAL.Update(_id, serializedCOEDataView, this.Name, this.Description, _isPublic, _databaseName, this.Application);
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(serializedCOEDataView);
            this.COEDataView = new COEDataView(doc);

            //call internal methods of COEAccessRightsBO to update accessRights
            if (_coeAccessRights != null && ((_coeAccessRights.Users != null && _coeAccessRights.Users.Count > 0) || (_coeAccessRights.Roles != null && _coeAccessRights.Roles.Count > 0)))
            {
                _coeAccessRights.ObjectID = _id;
                _coeAccessRights.ObjectType = COEAccessRightsBO.ObjectTypes.COEDATAVIEW;
                _coeAccessRights.Save();
            }
            else
            {
                _isPublic = true;
                COEAccessRightsBO.Delete(COEAccessRightsBO.ObjectTypes.COEDATAVIEW, _id);
            }
            this.MarkOld();
            this.MarkClean();

            LocalCache.Remove(_id.ToString(), typeof(COEDataViewBO));
        }

        #endregion //Data Access - Update

        #region Data Access - Delete
        //called by other services
        internal void DeleteSelf(DAL _coeDAL)
        {
            if (_coeDAL == null) { LoadDAL(); }

            // Coverity Fix CID - 11487
            if (_coeDAL != null)
            {
                _coeDAL.Delete(_id);
            }
            else
                throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));
        }

        protected override void DataPortal_DeleteSelf()
        {
            DataPortal_Delete(new Criteria(_id, true));
        }

        private void DataPortal_Delete(Criteria criteria)
        {

            if (_coeDAL == null) { LoadDAL(); }
            // Coverity Fix CID - 11483
            if (_coeDAL != null)
            {
                _coeDAL.Delete(criteria._id);
                if (criteria._includeAccessRights == true)
                {
                    _coeAccessRights.Delete();
                }
            }
            else
                throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));
        }


        #endregion //Data Access - Delete

        private void LoadDAL()
        {

            if (_dalFactory == null) { _dalFactory = new DALFactory(); }
            _dalFactory.GetDAL<DAL>(ref _coeDAL, _serviceName, COEDatabaseName.Get().ToString(), true);
        }

        #endregion //Data Access

        #region IComparable<COEDataViewBO> Members

        public int CompareTo(COEDataViewBO other)
        {
            return this._id.CompareTo(other._id);
        }

        #endregion

        private void FillEmptyValuesToMessagingType()
        {
            if (this.COEDataView != null && string.IsNullOrEmpty(this.COEDataView.Name))
                this.COEDataView.Name = _name;
            if (this.COEDataView != null && string.IsNullOrEmpty(this.COEDataView.Description))
                this.COEDataView.Description = _description;
            if (this.COEDataView != null && this.COEDataView.DataViewID < 0)
                this.COEDataView.DataViewID = this.ID;


            if (this.COEDataView != null && (this.COEDataView.Basetable >= 0))
            {
                string baseTableDB = this.COEDataView.GetDatabaseNameById(this.COEDataView.Basetable);
                if (!string.IsNullOrEmpty(baseTableDB))
                {
                    this.COEDataView.Database = baseTableDB;
                    _databaseName = baseTableDB;
                }
            }
        }

        public bool AreNameAndDescriptionValid
        {
            get { return _name.Length > 0 && _description.Length > 0; }
        }

        public bool CheckNameIsUnique()
        {
            bool isUnique = true;
            if (_coeDAL == null) { LoadDAL(); }

            // Coverity Fix CID - 11482 
            if (_coeDAL != null)
            {
                FillEmptyValuesToMessagingType();
                SafeDataReader reader = _coeDAL.Get(_name);
                while (reader.Read())
                {
                    if (reader.GetString("NAME") == _name && Convert.ToInt32(reader.GetInt64("ID")) != _id)
                    {
                        isUnique = false;
                        throw new Types.Exceptions.DataviewException(Resources.DataviewNameDuplicated);
                    }
                }
            }
            else
                throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));

            return isUnique;
        }



        public string PublishTableToDataview(string fullTableName, string primaryKeyFieldName, string parentTableJoinFieldName, string childTableJoinFieldName, COEDataView.JoinTypes joinType)
        {
            #region Variables
            int parentFieldID = -1;
            int parentTableID = -1;
            int childFieldID = -1;
            int childTableID = -1;
            string tableSchema = string.Empty;
            string tableName = string.Empty;
            string primaryKeyName = string.Empty;
            string childFieldName = string.Empty;
            string parentFieldName = string.Empty;
            string parentTableName = string.Empty;
            string parentSchemaName = string.Empty;

            string[] tableSplit = fullTableName != null ? fullTableName.Split('.') : new string[0];
            string[] primaryKeySplit = primaryKeyFieldName != null ? primaryKeyFieldName.Split('.') : new string[0];
            string[] parentFieldSplit = parentTableJoinFieldName != null ? parentTableJoinFieldName.Split('.') : new string[0];
            string[] childFieldSplit = childTableJoinFieldName != null ? childTableJoinFieldName.Split('.') : new string[0];
            COEDataView dataView = null;
            COEDataView.DataViewTable tableToAdd = null;
            #endregion

            #region New Table Splits
            if (tableSplit.Length == 2)
            {
                tableSchema = tableSplit[0];
                tableName = tableSplit[1];
            }
            else if (tableSplit.Length == 1)
                tableName = tableSplit[0];

            if (primaryKeySplit.Length == 3)
            {
                if (string.IsNullOrEmpty(tableSchema))
                    tableSchema = primaryKeySplit[0];

                if (string.IsNullOrEmpty(tableName))
                    tableName = primaryKeySplit[1];

                primaryKeyName = primaryKeySplit[2];
            }
            else if (primaryKeySplit.Length == 2)
            {
                if (string.IsNullOrEmpty(tableName))
                    tableName = primaryKeySplit[0];

                primaryKeyName = primaryKeySplit[1];
            }
            else if (primaryKeySplit.Length == 1)
                primaryKeyName = primaryKeySplit[0];

            if (this.DataViewManager == null)
                this.DataViewManager = COEDataViewManagerBO.NewManager(this.COEDataView);

            if (string.IsNullOrEmpty(tableSchema))
                tableSchema = this.DataViewManager.DataBase;
            #endregion

            #region Join Splits
            if (parentFieldSplit.Length == 3)
            {
                parentSchemaName = parentFieldSplit[0];
                parentTableName = parentFieldSplit[1];
                parentFieldName = parentFieldSplit[2];
            }
            else if (parentFieldSplit.Length == 2)
            {
                parentTableName = parentFieldSplit[0];
                parentFieldName = parentFieldSplit[1];
            }
            else if (parentFieldSplit.Length == 1)
                parentFieldName = parentFieldSplit[0];

            if (childFieldSplit.Length == 3)
            {
                if (string.IsNullOrEmpty(tableSchema))
                    tableSchema = childFieldSplit[0];

                childFieldName = childFieldSplit[2];
            }
            else if (childFieldSplit.Length == 2)
                childFieldName = childFieldSplit[1];
            else if (childFieldSplit.Length == 1)
                childFieldName = childFieldSplit[0];
            #endregion

            COEDatabaseBO database = COEDatabaseBO.Get(tableSchema);
            if (database != null)
            {
                #region Refresh published schema and grab the right table

                if (database.COEDataView.Tables[tableName] == null)
                    database = database.RefreshPublish(); //Rebuilds the dataview in serverside.

                tableToAdd = database.COEDataView.Tables[tableName];
                #endregion

                if (tableToAdd != null)
                {
                    #region Add the table to the master dataview
                    // Lets add the table to the Master DV:
                    COEDataViewBO master = COEDataViewBO.GetMasterSchema();
                    dataView = master.COEDataView;
                    int highestID = master.DataViewManager.Tables.HighestID;
                    if (highestID < 1)
                        highestID = 1;

                    if (master.DataViewManager.Tables[tableName] == null)
                    {
                        //Update field ids:
                        tableToAdd.Id = ++highestID;
                        foreach (COEDataView.Field field in tableToAdd.Fields)
                        {
                            if (tableToAdd.PrimaryKey == field.Id.ToString())
                                tableToAdd.PrimaryKey = (field.Id = ++highestID).ToString();
                            else
                                field.Id = ++highestID;

                            if (field.Name == primaryKeyName)
                                tableToAdd.PrimaryKey = field.Id.ToString();
                        }

                        dataView.Tables.Add(tableToAdd);
                        master.COEDataView = dataView;
                        master = master.Save();
                    }
                    #endregion

                    #region Add the table to the specific dataview. Build the relationship and also add it.
                    dataView = this.COEDataView;
                    highestID = this.DataViewManager.Tables.HighestID;
                    if (highestID < 1)
                        highestID = 1;

                    if (this.DataViewManager.Tables[tableName] == null)
                    {
                        //Update field ids:
                        childTableID = tableToAdd.Id = ++highestID;
                        foreach (COEDataView.Field field in tableToAdd.Fields)
                        {
                            if (tableToAdd.PrimaryKey == field.Id.ToString())
                                tableToAdd.PrimaryKey = (field.Id = ++highestID).ToString();
                            else
                                field.Id = ++highestID;

                            if (childFieldName == field.Name)
                                childFieldID = field.Id;

                            if (field.Name == primaryKeyName)
                                tableToAdd.PrimaryKey = field.Id.ToString();
                        }

                        dataView.Tables.Add(tableToAdd);

                        //Get the parent table and field for the join
                        if (string.IsNullOrEmpty(parentSchemaName))
                            parentSchemaName = this.DataViewManager.DataBase;
                        if (string.IsNullOrEmpty(parentTableName))
                            parentTableName = this.BaseTable;
                        if (string.IsNullOrEmpty(parentFieldName))
                            parentFieldName = this.DataViewManager.Tables.GetField(int.Parse(dataView.BaseTablePrimaryKey)).Name;

                        TableBO parentTbl = this.DataViewManager.Tables[parentTableName];
                        if (parentTbl != null)
                        {
                            FieldBO existingField = parentTbl.Fields.GetField(parentSchemaName, parentFieldName);
                            if (existingField != null)
                            {
                                parentFieldID = existingField.ID;
                                parentTableID = parentTbl.ID;
                            }
                        }

                        // A child and parent Field were found, let's add the Join
                        if (childFieldID > -1 && parentFieldID > -1)
                        {
                            COEDataView.Relationship rel = new COEDataView.Relationship();
                            rel.JoinType = joinType;
                            rel.Parent = parentTableID;
                            rel.ParentKey = parentFieldID;
                            rel.Child = childTableID;
                            rel.ChildKey = childFieldID;

                            dataView.Relationships.Add(rel);
                        }
                        this.COEDataView = dataView;

                        this.Save();
                    }
                    #endregion
                    return "OK";
                }
                return string.Format("Table: {0} not found under schema {1}", tableName, tableSchema);
            }
            return string.Format("Schema: {0} was never published", tableSchema);
        }
        #region ICacheable Members
        // I was not able to make this class properly serializable
        [NonSerialized]
        private COECacheDependency _cacheDependency;
        /// <summary>
        /// Cache dependency that is build from the dal at dataportal_fetch time. Is the mechanism to get the cache updated when the underlying
        /// record changed in database.
        /// </summary>
        public COECacheDependency CacheDependency
        {
            get { return _cacheDependency; }
            set { _cacheDependency = value; }
        }

        /// <summary>
        /// Method triggered when the object is removed from cache. Currently display information in the debug console, if in debug mode.
        /// </summary>
        /// <param name="key">The object id</param>
        /// <param name="value">The actual dataviewbo</param>
        /// <param name="reason">The reason why it was removed from cache</param>
        public void ItemRemovedFromCache(string key, object value, COECacheItemRemovedReason reason)
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine("");
            System.Diagnostics.Debug.WriteLine("*****************************");
            System.Diagnostics.Debug.WriteLine("Item Removed from cache.");
            System.Diagnostics.Debug.WriteLine("Key: " + key);
            System.Diagnostics.Debug.WriteLine("Reason: " + reason.ToString());
            System.Diagnostics.Debug.WriteLine("Current Time: " + DateTime.Now);
            System.Diagnostics.Debug.WriteLine("*****************************");
#endif
        }

        #endregion

        public static List<string> GetAllTags()
        {
            return GetTagsCommand.Execute();
        }

        /// <summary>
        /// Gets list of fields from table which are of type date/CLOB/BLOB.
        /// </summary>
        /// <param name="strTableName"></param>
        /// <returns></returns>
        public List<string> GetInvalidPrimaryKeyFields(string strTableName)
        {
            try
            {
                SetDatabaseName();

                if (_coeDAL == null)
                    LoadDAL();
                if (_coeDAL != null)
                    return _coeDAL.GetInvalidPrimaryKeyFields(strTableName);
                else
                    throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));
            }
            catch (Exception ex)
            {
                throw;
            }
            return null;
        }

        /// <summary>
        /// Method will return a new FieldBO object with unique Id.
        /// </summary>
        /// <returns>Return new FieldBO object with unique Id</returns>
        public FieldBO GetNewfield(string fieldname)
        {
            COEDataView.Field dataViewField = new COEDataView.Field();
            FieldBO newField = FieldBO.NewField(dataViewField, this.DatabaseName, true);
            if (newField != null)
            {
                newField.ID = ++this._dataViewManager.Tables.HighestID;
                newField.Name = fieldname;
            }
            return newField;
        }
        /// <summary>
        /// overloaded Method will return a new FieldBO object with unique Id.
        /// </summary>
        /// <returns>Return new FieldBO object with unique Id</returns>
        public FieldBO GetNewfield(string fieldname, string databaseName)
        {
            COEDataView.Field dataViewField = new COEDataView.Field();
            FieldBO newField = FieldBO.NewField(dataViewField, databaseName, true);
            if (newField != null)
            {
                newField.ID = ++this._dataViewManager.Tables.HighestID;
                newField.Name = fieldname;
            }
            return newField;
        }

        private DataTable _GetUniqueFields(string DataBaseName, string TableName)
        {
            //Bug Fixing : CBOE-242
            SetDatabaseName();
            if (_coeDAL == null) { LoadDAL(); }
            if (_coeDAL != null)
            {
                return _coeDAL.GetUniqueFields(DataBaseName, TableName);
            }
            else
            {
                throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));
            }
        }

        /// <summary>
        /// Returns the datatable with primary & unique key contraint column information
        /// </summary>
        /// <param name="database">Name of the database, used as database owner.</param>
        /// <param name="table">Name of the table to find the index information</param>
        /// <returns>Datatable</returns>
        public DataTable GetUniqueFields(string DataBaseName, string TableName)
        {
            //Bug Fixing : CBOE-242
            COEDataViewBO theCOEDataViewBO = new COEDataViewBO();
            return theCOEDataViewBO._GetUniqueFields(DataBaseName, TableName);
        }

        /// <summary>
        /// Returns the datatable which contains the NOTNULL string/number columns
        /// </summary>
        /// <param name="database">Name of the database, used as database owner.</param>
        /// <param name="table">Name of the table to find the index information</param>
        /// <returns>Datatable</returns>
        public DataTable GetPrimaryKeyFieldNotNullCols(string DataBaseName, string TableName)
        {
            //Bug Fixing : CBOE-242
            COEDataViewBO theCOEDataViewBO = new COEDataViewBO();
            return theCOEDataViewBO._GetPrimaryKeyFieldNotNullCols(DataBaseName, TableName);
        }

        private DataTable _GetPrimaryKeyFieldNotNullCols(string DataBaseName, string TableName)
        {
            //Bug Fixing : CBOE-1021
            SetDatabaseName();
            if (_coeDAL == null) { LoadDAL(); }
            if (_coeDAL != null)
            {
                return _coeDAL.GetPrimaryKeyFieldNotNullCols(DataBaseName, TableName);
            }
            else
            {
                throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));
            }
        }

        /// <summary>
        /// Gets the list of BioAssay View Names.
        /// </summary>
        /// <returns></returns>
        public List<string> GetELNSchemaName()
        {
            //Bug Fixing : CBOE-1021
            COEDataViewBO theCOEDataViewBO = new COEDataViewBO();
            return theCOEDataViewBO._GetELNSchemaName();
        }

        private List<string> _GetELNSchemaName()
        {
            //Bug Fixing : CBOE-1021
            SetDatabaseName();
            if (_coeDAL == null) { LoadDAL(); }
            if (_coeDAL != null)
            {
                return _coeDAL.GetELNSchemaName();
            }
            else
            {
                throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));
            }
        }
    }


    [Serializable]
    internal class GetTagsCommand : CommandBase
    {
        private string _serviceName = "COEDataView";
        private string _database = Resources.CentralizedStorageDB;
        [NonSerialized]
        private DAL _coeDAL;
        [NonSerialized]
        private DALFactory _dalFactory;

        internal List<string> Tags;

        private GetTagsCommand() { Tags = new List<string>(); }

        protected override void DataPortal_Execute()
        {
            if (_coeDAL == null)
                LoadDAL();
            // Coverity Fix CID - 11491
            if (_coeDAL != null)
            {
                this.Tags = _coeDAL.GetAllTags();
            }
            else
                throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));
        }

        private void LoadDAL()
        {

            if (_dalFactory == null) { _dalFactory = new DALFactory(); }
            _dalFactory.GetDAL<DAL>(ref _coeDAL, _serviceName, _database, true);
        }

        internal static List<string> Execute()
        {
            return DataPortal.Execute<GetTagsCommand>(new GetTagsCommand()).Tags;
        }
    }

    [Serializable]
    internal class CanGetDataviewCommand : CommandBase
    {
        private int _id;
        private string _serviceName = "COEDataView";
        [NonSerialized]
        private DAL _coeDAL;
        [NonSerialized]
        private DALFactory _dalFactory;

        internal bool HasPermissions;
        public CanGetDataviewCommand(int id)
        {
            _id = id;
            HasPermissions = false;
        }


        protected override void DataPortal_Execute()
        {
            if (_coeDAL == null)
                LoadDAL();
            // Coverity Fix CID - 11490 
            if (_coeDAL != null)
            {
                HasPermissions = _coeDAL.CanGetDataview(_id);
            }
            else
                throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));
        }

        private void LoadDAL()
        {

            if (_dalFactory == null) { _dalFactory = new DALFactory(); }
            _dalFactory.GetDAL<DAL>(ref _coeDAL, _serviceName, COEDatabaseName.Get().ToString(), true);
        }

        internal bool Execute()
        {
            return DataPortal.Execute<CanGetDataviewCommand>(this).HasPermissions;
        }
    }
}
