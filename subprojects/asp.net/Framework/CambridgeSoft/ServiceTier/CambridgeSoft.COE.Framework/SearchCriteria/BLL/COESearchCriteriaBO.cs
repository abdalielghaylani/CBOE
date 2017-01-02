using System;
using System.Data;
using System.Data.SqlClient;
using Csla;
using Csla.Data;
using Csla.Validation;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.Properties;
using CambridgeSoft.COE.Framework.COELoggingService;
using CambridgeSoft.COE.Framework.COEConfigurationService;
using System.Xml;
using CambridgeSoft.COE.Framework.Common.Messaging;
using CambridgeSoft.COE.Framework.Caching;

namespace CambridgeSoft.COE.Framework.COESearchCriteriaService
{
    [Serializable()]
    public class COESearchCriteriaBO : Csla.BusinessBase<COESearchCriteriaBO>, ICacheable
    {
        #region Member variables
        //declare members
        private int _id = 0;
        private string _name = string.Empty;
        private SmartDate _dateCreated = new SmartDate(true);
        private string _description = string.Empty;
        private bool _isPublic = false;
        private string _userName = string.Empty;
        private int _formGroup = 0;
        private int _numHits = 0;
        private SearchCriteria _coeSearchCriteria;
        private string _databaseName = string.Empty;
        private string _serviceName = "COESearchCriteria";
        private int _dataViewId;
        private SearchCriteriaType _searchCriteriaType;

        public const int MAX_LENGTH_OF_NAME = 50;

        //variables data access
        [NonSerialized]
        private DAL _coeDAL = null;
        [NonSerialized]
        private DALFactory _dalFactory = new DALFactory();

        [NonSerialized]
        static COELog _coeLog = COELog.GetSingleton("COESearchCriteria");
        [NonSerialized]
        private string _specialFolder = COEConfigurationBO.ConfigurationBaseFilePath + @"SimulationFolder\Framework\";
        #endregion

        #region Properties

        public string DatabaseName
        {
            get
            {
                CanReadProperty(true);
                return _databaseName;
            }
            set
            {
                CanWriteProperty(true);
                if (_databaseName != value)
                {
                    _databaseName = value;
                    PropertyHasChanged();
                }
            }
        }

        [System.ComponentModel.DataObjectField(true, false)]
        public int ID
        {
            get
            {
                CanReadProperty(true);
                return _id;
            }
        }

        public string Name
        {
            get
            {
                CanReadProperty(true);
                return _name;
            }
            set
            {
                CanWriteProperty(true);
                if (value == null)
                    value = string.Empty;
                if (_name != value)
                {
                    _name = value;
                    PropertyHasChanged();
                }
            }
        }

        public DateTime DateCreated
        {
            get
            {
                CanReadProperty(true);
                return _dateCreated.Date;
            }
        }

        public string Description
        {
            get
            {
                CanReadProperty(true);
                return _description;
            }
            set
            {
                CanWriteProperty(true);
                if (value == null)
                    value = string.Empty;
                if (_description != value)
                {
                    _description = value;
                    PropertyHasChanged();
                }
            }
        }

        public bool IsPublic
        {
            get
            {
                CanReadProperty(true);
                return _isPublic;
            }
            set
            {
                CanWriteProperty(true);
                if (_isPublic != value)
                {
                    _isPublic = value;
                    PropertyHasChanged();
                }
            }
        }

        public string UserName
        {
            get
            {
                CanReadProperty(true);
                return _userName;
            }
            set
            {
                CanWriteProperty(true);
                if (value == null) value = string.Empty;
                if (_userName != value)
                {
                    _userName = value;
                    PropertyHasChanged();
                }
            }
        }

        public int FormGroup
        {
            get
            {
                CanReadProperty(true);
                return _formGroup;
            }
            set
            {
                CanWriteProperty(true);
                if (_formGroup != value)
                {
                    _formGroup = value;
                    PropertyHasChanged();
                }
            }
        }


        public SearchCriteria SearchCriteria
        {
            get
            {
                CanReadProperty(true);
                return _coeSearchCriteria;
            }
            set
            {
                CanWriteProperty(true);
                if (_coeSearchCriteria == null || !_coeSearchCriteria.Equals(value))
                {
                    _coeSearchCriteria = value;
                    PropertyHasChanged();
                }
            }
        }
        public int NumberOfHits
        {
            get
            {
                CanReadProperty(true);
                return _numHits;
            }
            set
            {
                CanWriteProperty(true);
                if (_numHits != value)
                {
                    _numHits = value;
                    PropertyHasChanged();
                }
            }
        }

        protected override object GetIdValue()
        {
            return _id;
        }

        public int DataViewId
        {
            get
            {
                CanReadProperty(true);
                return _dataViewId;
            }
            set
            {
                CanWriteProperty(true);
                if (_dataViewId != value)
                {
                    _dataViewId = value;
                    PropertyHasChanged();
                }
            }
        }

        public SearchCriteriaType SearchCriteriaType
        {
            get
            {
                CanReadProperty(true);
                return _searchCriteriaType;
            }
        }

        [NonSerialized]
        private static ApplicationData _appConfigData;
        private static ApplicationData AppConfigData
        {
            get
            {
                if (_appConfigData == null && !string.IsNullOrEmpty(COEAppName.Get()))
                    _appConfigData = ConfigurationUtilities.GetApplicationData(COEAppName.Get());

                return _appConfigData;
            }
        }

        private static CacheItemData CacheConfig
        {
            get
            {
                try
                {
                    if (AppConfigData != null && AppConfigData.CachingData != null && AppConfigData.CachingData.SearchCriteria != null)
                    {
                        return AppConfigData.CachingData.SearchCriteria;
                    }
                }
                catch (Exception)
                {
                }
                return new CacheItemData();
            }
        }
        #endregion

        #region Constructors

        private COESearchCriteriaBO()
        { /* require use of factory method */ }

        //constructor to be called from queryCriteriaList as well as any other services that needs to construct this object
        internal COESearchCriteriaBO(int id, string name, string description, string userID, bool isPublic, int formGroup, SmartDate dateCreated, SearchCriteria coeSearchCriteria, int numHits, string databaseName, int dataViewId, SearchCriteriaType searchCriteriaType, bool IsOld)
        {
            _id = id;
            _name = name;
            _description = description;
            _userName = userID;
            _dateCreated = dateCreated;
            _isPublic = isPublic;
            _formGroup = formGroup;
            _coeSearchCriteria = coeSearchCriteria;
            _numHits = numHits;
            _databaseName = databaseName;
            _dataViewId = dataViewId;
            _searchCriteriaType = searchCriteriaType;
            if (IsOld)
                MarkOld();

        }

        internal COESearchCriteriaBO(SearchCriteria coeSearchCriteria, int numHits, string databaseName, int dataViewId)
        {
            _coeSearchCriteria = coeSearchCriteria;
            _numHits = numHits;
            _dataViewId = dataViewId;

        }

        #endregion

        #region Validation Rules

        private void AddCommonRules()
        {
            //
            // QueryName
            //
            ValidationRules.AddRule(CommonRules.StringRequired, "Name");
            ValidationRules.AddRule(CommonRules.StringMaxLength, new CommonRules.MaxLengthRuleArgs("Name", MAX_LENGTH_OF_NAME));
            //
            // DateCreated
            //
            ValidationRules.AddRule(CommonRules.MinValue<SmartDate>, new CommonRules.MinValueRuleArgs<SmartDate>("DateCreated", new SmartDate("1/1/2005")));
            ValidationRules.AddRule(CommonRules.RegExMatch, new CommonRules.RegExRuleArgs("DateCreated", @"(0[1-9]|1[012])[- /.](0[1-9]|[12][0-9]|3[01])[- /.](19|20)\d\d"));
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
            COEDatabaseName.Set(DALUtils.GetDefaultQualifyDbName(Resources.CentralizedStorageDB));
        }


        internal static void SetDatabaseName(string databaseName)
        {
            COEDatabaseName.Set(DALUtils.GetDefaultQualifyDbName(databaseName));
        }

        public static COESearchCriteriaBO New(string databaseName)
        {
            SetDatabaseName();
            if (!CanAddObject())
                throw new System.Security.SecurityException(Resources.UserNotAuthorizedForAddObject + " COESearchCriteriaBO");
            return DataPortal.Create<COESearchCriteriaBO>(new CreateNewCriteria(databaseName));
        }

        private static string BuildKey(int id, SearchCriteriaType type)
        {
            return id.ToString() + "_" + type.ToString();
        }

        public static COESearchCriteriaBO Get(SearchCriteriaType searchCriteriaType, int id)
        {
            string idString = BuildKey(id, searchCriteriaType);
          //  SetDatabaseName(); Jason: it seems this is client side, before calling web service, no need to use database name for creating DAL
            COESearchCriteriaBO result = null;
            if (!CanGetObject())
                throw new System.Security.SecurityException(Resources.UserNotAuthorizedForViewObject + " COESearchCriteriaBO");

            switch (CacheConfig.Cache)
            {
                case CacheType.Disabled:
                    result = DataPortal.Fetch<COESearchCriteriaBO>(new Criteria(searchCriteriaType, id));
                    break;
                case CacheType.ClientCache:
                    result = LocalCache.Get(idString, typeof(COESearchCriteriaBO)) as COESearchCriteriaBO;
                    if (result == null)
                    {
                        result = DataPortal.Fetch<COESearchCriteriaBO>(new Criteria(searchCriteriaType, id));
                        LocalCache.Add(idString, typeof(COESearchCriteriaBO), result, CacheConfig.AbsoluteExpiration, CacheConfig.SlidingExpiration, CacheConfig.DefaultPriority);
                    }
                    break;
                case CacheType.ServerCache:
                    result = ServerCache.Get(idString, typeof(COESearchCriteriaBO)) as COESearchCriteriaBO;
                    if (result == null)
                    {
                        result = DataPortal.Fetch<COESearchCriteriaBO>(new Criteria(searchCriteriaType, id));
                    }
                    break;
                case CacheType.ServerAndClientCache:
                    result = LocalCache.Get(idString, typeof(COESearchCriteriaBO)) as COESearchCriteriaBO;
                    if (!ServerCache.Exists(idString, typeof(COESearchCriteriaBO)) && result == null) //Exists is first to refresh the sliding time
                    {
                        result = DataPortal.Fetch<COESearchCriteriaBO>(new Criteria(searchCriteriaType, id));
                        LocalCache.Add(idString, typeof(COESearchCriteriaBO), result, CacheConfig.AbsoluteExpiration, CacheConfig.SlidingExpiration, CacheConfig.DefaultPriority);
                    }
                    else if (result == null)
                    {
                        result = ServerCache.Get(idString, typeof(COESearchCriteriaBO)) as COESearchCriteriaBO;
                        LocalCache.Add(idString, typeof(COESearchCriteriaBO), result, CacheConfig.AbsoluteExpiration, CacheConfig.SlidingExpiration, CacheConfig.DefaultPriority);
                    }
                    break;
            }

            return result;
        }

        public static void Delete(int id)
        {
            SetDatabaseName();

            if (!CanDeleteObject())
                throw new System.Security.SecurityException(Resources.UserNotAuthorizedForDeleteObject + " COESearchCriteriaBO");
            DataPortal.Delete(new Criteria(SearchCriteriaType.TEMP, id));
        }

        public static void Delete(SearchCriteriaType type, int id)
        {
            SetDatabaseName();

            if (!CanDeleteObject())
                throw new System.Security.SecurityException(Resources.UserNotAuthorizedForDeleteObject + " COESearchCriteriaBO");
            DataPortal.Delete(new Criteria(type, id));
        }

        public override COESearchCriteriaBO Save()
        {
            SetDatabaseName();
            if (IsDeleted && !CanDeleteObject())
                throw new System.Security.SecurityException(Resources.UserNotAuthorizedForDeleteObject + " COESearchCriteriaBO");
            else if (IsNew && !CanAddObject())
                throw new System.Security.SecurityException(Resources.UserNotAuthorizedForAddObject + " COESearchCriteriaBO");
            else if (!CanEditObject())
                throw new System.Security.SecurityException(Resources.UserNotAuthorizedForEditObject + " COESearchCriteriaBO");
            if (this.SearchCriteriaType == SearchCriteriaType.TEMP)
            {
                SavedSearchCriteriaCommand result;
                result = DataPortal.Execute<SavedSearchCriteriaCommand>(new SavedSearchCriteriaCommand(this));
                return DataPortal.Fetch<COESearchCriteriaBO>(new Criteria(SearchCriteriaType.SAVED, result.SavedSearchCriteriaId));

            }
            else
            {
                throw new Exception("The Search Criteria Type is " + SearchCriteriaType.SAVED.ToString());
            }
        }

        public COESearchCriteriaBO Update()
        {
            return base.Save();
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
        private class Criteria
        {

            internal int _id;
            internal SearchCriteriaType _searchCriteriaType;

            //constructors
            public Criteria(SearchCriteriaType searchCriteriaType, int id)
            {
                _searchCriteriaType = searchCriteriaType;
                _id = id;
            }
            public int SearchCriteriaId
            {
                get
                {
                    return _id;
                }
            }
            public SearchCriteriaType SearchCriteriaType
            {
                get
                {
                    return _searchCriteriaType;
                }
            }
        }
        #endregion //Criteria

        #region CreateNewCriteria

        [Serializable()]
        private class CreateNewCriteria
        {



            internal string _database;
            public CreateNewCriteria(string database)
            {
                _database = database;
            }


        }
        #endregion //CreateNewCriteria

        [Serializable()]
        public class SavedSearchCriteriaCommand : CommandBase
        {
            private COESearchCriteriaBO _searchCriteriaBO;
            private int _savedSearchCriteriaId;

            [NonSerialized]
            private DAL _coeDAL = null;
            [NonSerialized]
            private DALFactory _dalFactory = new DALFactory();
            private string _serviceName = "COESearchCriteria";

            public SavedSearchCriteriaCommand(COESearchCriteriaBO searchCriteriaBO)
            {
                this._searchCriteriaBO = searchCriteriaBO;
            }

            public int SavedSearchCriteriaId
            {
                get
                {
                    return _savedSearchCriteriaId;
                }
            }

            protected override void DataPortal_Execute()
            {
                if (_dalFactory == null) { _dalFactory = new DALFactory(); }
                _dalFactory.GetDAL<DAL>(ref _coeDAL, _serviceName, COEDatabaseName.Get().ToString(), true);
                _savedSearchCriteriaId = _coeDAL.NewSavedSearchCriteriaFromTemp(_searchCriteriaBO.Name, _searchCriteriaBO.Description, _searchCriteriaBO.UserName, _searchCriteriaBO.NumberOfHits, _searchCriteriaBO.IsPublic,
                    _searchCriteriaBO.FormGroup, _searchCriteriaBO.SearchCriteria, _searchCriteriaBO.DatabaseName, _searchCriteriaBO._dataViewId);
            }
        }

        #region Data Access - Create
        [RunLocal]
        private void DataPortal_Create(CreateNewCriteria createNewCriteria)
        {
            try
            {
                _databaseName = createNewCriteria._database;
                _coeSearchCriteria = null;

            }
            catch (Exception)
            {

                throw;
            }
        }

        #endregion //Data Access - Create

        #region Data Access - Fetch
        private void DataPortal_Fetch(Criteria criteria)
        {
            try
            {
                if (COE.Framework.Common.Utilities.SimulationMode())
                {
                    try
                    {
                        _id = criteria._id;

                        XmlDocument xmlDocument = new XmlDocument();
                        xmlDocument.Load(_specialFolder + this.GetType().Name + @".xml");
                        this._coeSearchCriteria = new SearchCriteria(xmlDocument);
                    }
                    catch
                    {
                        if (_coeDAL == null) { LoadDAL(); }
                        SafeDataReader dr = _coeDAL.Get(criteria.SearchCriteriaId, criteria.SearchCriteriaType);
                        FetchObject(dr, criteria.SearchCriteriaType);
                    }
                }
                else
                {
                    if (_coeDAL == null) { LoadDAL(); }
                    // Coverity Fix CID - 11586 
                    if (_coeDAL != null)
                    {
                        using (SafeDataReader dr = _coeDAL.Get(criteria.SearchCriteriaId, criteria.SearchCriteriaType))
                        {
                            FetchObject(dr, criteria.SearchCriteriaType);
                        }
                    }
                    else
                        throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private void FetchObject(SafeDataReader dr, SearchCriteriaType searchCriteriaType)
        {
            if (dr.Read())
            {
                _id = dr.GetInt32("ID");
                _name = dr.GetString("NAME");
                _dateCreated = dr.GetSmartDate("DATE_CREATED", _dateCreated.EmptyIsMin);
                _description = dr.GetString("DESCRIPTION");
                _isPublic = dr.GetChar("IS_PUBLIC") == '1';
                _userName = dr.GetString("USER_ID");
                _formGroup = dr.GetInt32("FORMGROUP");
                _searchCriteriaType = searchCriteriaType;
                _coeSearchCriteria = (SearchCriteria)COESearchCriteriaUtilities.DeserializeCOESearchCriteria(dr.GetString("COESEARCHCRITERIA"));
                _numHits = dr.GetInt32("NUM_HITS");
                _databaseName = dr.GetString("DATABASE");
                _dataViewId = dr.GetInt32("DATAVIEW_ID");

                if (CacheConfig.Cache == CacheType.ServerCache || CacheConfig.Cache == CacheType.ServerAndClientCache)
                {
                    // As dependencies are not serializable they are created on the dataportal_fetch calls (on server side)
                    _cacheDependency = _coeDAL.GetCacheDependency(_id, _searchCriteriaType);
                    // Moreover and due to the same fact, we dont want to use the server cache because that involves a CSLA command and serialization
                    // and would anyway lead to the current tier.
                    // So, local cache is used.
                    LocalCache.Add(BuildKey(_id, _searchCriteriaType), this.GetType(), this, CacheConfig.AbsoluteExpiration, CacheConfig.SlidingExpiration, CacheConfig.DefaultPriority);
                }
            }
        }

        #endregion //Data Access - Fetch

        #region Data Access - Insert

        //when already on the server, this can be called
        internal void Insert(DAL coeSearchCriteriaDAL)
        {
            try
            {
                if (!IsDirty) return;
                _id = coeSearchCriteriaDAL.Insert(_formGroup, _name, _isPublic, _description, _userName, _coeSearchCriteria, _numHits, _databaseName, _dataViewId);
                MarkOld();
            }
            catch (Exception)
            {

                throw;
            }
        }

        protected override void DataPortal_Insert()
        {
            try
            {
                if (_coeDAL == null) { LoadDAL(); }
                // Coverity Fix CID - 1587 
                if (_coeDAL != null)
                {
                    _id = _coeDAL.Insert(_formGroup, _name, _isPublic, _description, _userName, _coeSearchCriteria, _numHits, _databaseName, _dataViewId);
                }
                else
                    throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));
            }
            catch (Exception)
            {

                throw;
            }
        }

        internal void InsertTempQueryCriteria()
        {
            try
            {
                _name = "TEMP";
                _description = "TEMP";
                _userName = "TEMP"; //need to get this from Identiy
                _dateCreated = new SmartDate(DateTime.Now);
                _isPublic = false;
                _formGroup = 0;//not sure where this will comefrom
                if (_coeDAL == null) { LoadDAL(); }
                // Coverity Fix CID - 11590
                if (_coeDAL != null)                
                    _coeDAL.Insert(_formGroup, _name, _isPublic, _description, _userName, _coeSearchCriteria, _numHits, _databaseName, _dataViewId);
                else
                    throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));                
            }
            catch (Exception)
            {

                throw;
            }

        }


        #endregion

        #region Data Access - Update
        //called by other services
        internal void Update(DAL coeSearchCriteriaDAL)
        {
            try
            {
                if (!IsDirty) return;
                string serializedCOESearchCriteria = COESearchCriteriaUtilities.SerializeCOESearchCriteria(_coeSearchCriteria);
                coeSearchCriteriaDAL.Update(_id, serializedCOESearchCriteria, _name, _description, _isPublic, _numHits, _databaseName, _searchCriteriaType, _formGroup, _dataViewId);
                MarkOld();
            }
            catch (Exception)
            {

                throw;
            }
        }

        protected override void DataPortal_Update()
        {
            try
            {
                if (_coeDAL == null) { LoadDAL(); }

                // Coverity Fix CID - 11588
                if (_coeDAL != null)
                {
                    if (base.IsDirty)
                    {
                        string serializedCOESearchCriteria = COESearchCriteriaUtilities.SerializeCOESearchCriteria(_coeSearchCriteria);
                        _coeDAL.Update(_id, serializedCOESearchCriteria, _name, _description, _isPublic, _numHits, _databaseName, _searchCriteriaType, _formGroup, _dataViewId);
                    }
                }
                else
                    throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));
            }
            catch (Exception)
            {

                throw;
            }

        }
        #endregion //Data Access - Update

        #region Data Access - Delete
        //called by other services
        internal void DeleteSelf(DAL _coeDAL)
        {
            try
            {
                if (_coeDAL == null) { LoadDAL(); }
                // Coverity Fix CID - 11589 
                if (_coeDAL != null)
                    _coeDAL.Delete(_id);
                else
                    throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));
            }
            catch (Exception)
            {

                throw;
            }
        }

        protected override void DataPortal_DeleteSelf()
        {
            try
            {
                DataPortal_Delete(new Criteria(SearchCriteriaType.TEMP, _id));

            }
            catch (Exception)
            {

                throw;
            }
        }

        private void DataPortal_Delete(Criteria criteria)
        {
            try
            {
                if (_coeDAL == null) { LoadDAL(); }

                // Coverity Fix CID - 11585
                if (_coeDAL != null)
                {
                    if (criteria._searchCriteriaType == SearchCriteriaType.SAVED)
                        _coeDAL.DeleteSaved(criteria._id);
                    else
                        _coeDAL.Delete(criteria._id);
                }
                else
                    throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));
            }
            catch (Exception)
            {

                throw;
            }
        }


        #endregion //Data Access - Delete

        private void LoadDAL()
        {
            try
            {
                if (_dalFactory == null) { _dalFactory = new DALFactory(); }
                _dalFactory.GetDAL<DAL>(ref _coeDAL, _serviceName,  COEDatabaseName.Get().ToString(), true); ;

            }
            catch (Exception)
            {

                throw;
            }
        }

        #endregion //Data Access

        #region ICacheable Members
        [NonSerialized]
        private COECacheDependency _cacheDependency;
        /// <summary>
        /// Cache dependency that is build from the dal at dataportal_fetch time. Is the mechanism to get the cache updated when the underlying
        /// record changed in database.
        /// </summary>
        public COECacheDependency CacheDependency
        {
            get
            {
                return _cacheDependency;
            }
            set
            {
                _cacheDependency = value;
            }
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
    }
}

