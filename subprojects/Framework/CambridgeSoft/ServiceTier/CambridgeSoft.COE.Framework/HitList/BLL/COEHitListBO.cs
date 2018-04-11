using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using Csla;
using Csla.Data;
using System.Data;
using CambridgeSoft.COE.Framework.Properties;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.COEConfigurationService;
using CambridgeSoft.COE.Framework.COELoggingService;
using CambridgeSoft.COE.Framework.COEDataViewService;



namespace CambridgeSoft.COE.Framework.COEHitListService
{

    [Serializable()]
    public class COEHitListBO : BusinessBase<COEHitListBO>
    {

        #region Member variables

        protected int _id;
        protected int _hitlistID;
        protected int _numHits;
        protected string _name = string.Empty;
        protected string _description = string.Empty;
        protected string _userID = string.Empty;
        protected bool _isPublic = false;
        protected List<Hit> _hits = null;
        protected List<int> _markedHitsIDs = null;
        private int[] _hitIDs = null;
        private double[] _sortIDs = null;
        private List<int> _hitIDsList = null;
        private List<double> _sortIDsList = null;
        private int _parentHitListID;
        private int _dataViewID;
        private HitListInfo _hitListInfo;
        private int _searchCriteriaId;
        private SearchCriteriaType _searchCriteriaType;
        private bool _associatingCriteria = false;

        protected SmartDate _dateCreated;
        protected HitListType _hitListType;

        private HitListType _originalHitListType = HitListType.TEMP;

        private int tempID = 0;

        protected string _tempHitListTableName = string.Empty;
        protected string _tempHitListSchemaName = string.Empty;
        protected string _savedHitListTableName = string.Empty;
        protected string _savedHitListSchemaName = string.Empty;
        protected string _tempHitListIDTableName = string.Empty;
        protected string _savedHitListIDTableName = string.Empty;

        //this is not right, but i don't understand where to get password at this time

        //variables data access
        [NonSerialized]
        private DAL _coeDAL = null;
        [NonSerialized]
        private DALFactory _dalFactory = new DALFactory();
        private string _serviceName = "COEHitList";
        private string _databaseName = null;

        [NonSerialized]
        static COELog _coeLog = COELog.GetSingleton("COEHitList");
        #endregion

        #region Constructors

        internal COEHitListBO()
        {
            GetHitListTableNames();
        }

        internal COEHitListBO(int id, string name, int hitlistid, string description, string userID, bool isPublic, int numHits, SmartDate dateCreated, HitListType hitListType, string databaseName, int dataViewID, int searchCriteriaId, string searchCriteriaType)
        {
            _id = id;
            _name = name;
            _hitlistID = hitlistid;
            _description = description;
            _userID = userID;
            _isPublic = isPublic;
            _numHits = numHits;
            _dateCreated = dateCreated;
            _hitListType = hitListType;
            _databaseName = databaseName;
            _dataViewID = dataViewID;
            _searchCriteriaId = searchCriteriaId;
            if(!string.IsNullOrEmpty(searchCriteriaType))
                _searchCriteriaType = (SearchCriteriaType) Enum.Parse(typeof(SearchCriteriaType), searchCriteriaType);
        }

        #endregion

        #region Internal properties
        internal HitListType OriginalHitListType
        {
            get
            {
                return _originalHitListType;
            }
        }
        #endregion

        #region Public properties

        public List<int> MarkedHitListIDs
        {
            get
            {
                CanReadProperty(true);
                return _markedHitsIDs;
            }
        }

        public int DataViewID
        {
            get
            {
                CanReadProperty(true);
                return _dataViewID;
            }
            set
            {
                CanWriteProperty(true);
                _dataViewID = value;
                PropertyHasChanged();
            }
        }

        public int ParentHitListID
        {
            get
            {
                CanReadProperty(true);
                return _parentHitListID;
            }
        }

        public HitListInfo HitListInfo
        {
            get
            {
                if (_hitListInfo == null)
                {
                    _hitListInfo = new HitListInfo();
                    _hitListInfo.HitListID = _hitlistID;
                    _hitListInfo.HitListType = _hitListType;
                    _hitListInfo.Database = _databaseName;
                    _hitListInfo.CurrentRecordCount = _hitListInfo.RecordCount = _numHits;
                    CanReadProperty(true);
                    return _hitListInfo;
                }
                else
                {
                    CanReadProperty(true);
                    return _hitListInfo;
                }
            }
        }

        public string DatabaseName
        {
            get { return _databaseName; }
            set { _databaseName = value; }
        }

        [System.ComponentModel.DataObjectField(true, true)]
        public virtual int ID
        {
            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
            get
            {
                CanReadProperty(true);
                return _id;
            }
            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
            set
            {
                CanWriteProperty(true);
                _id = value;
            }
        }

        [System.ComponentModel.DataObjectField(true, true)]
        public virtual int HitListID
        {
            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
            get
            {
                CanReadProperty(true);
                return _hitlistID;
            }
            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
            set
            {
                CanWriteProperty(true);
                _hitlistID = value;
                PropertyHasChanged();
            }
        }

        public virtual string Name
        {
            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
            get
            {
                CanReadProperty(true);
                return _name;
            }
            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
            set
            {
                CanWriteProperty(true);
                _name = value;
                PropertyHasChanged();
            }
        }

        public virtual string Description
        {
            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
            get
            {
                CanReadProperty(true);
                return _description;
            }
            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
            set
            {
                CanWriteProperty(true);
                _description = value;
                PropertyHasChanged();
            }
        }

        public virtual string UserID
        {
            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
            get
            {
                CanReadProperty(true);
                return _userID;
            }
            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
            set
            {
                CanWriteProperty(true);
                _userID = value;
                PropertyHasChanged();
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
                _isPublic = value;
                PropertyHasChanged();
            }
        }

        public HitListType HitListType
        {
            get
            {
                CanReadProperty(true);
                return _hitListType;
            }
            set
            {
                CanWriteProperty(true);
                _hitListType = value;
                PropertyHasChanged();
            }
        }

        public int SearchCriteriaID
        {
            get
            {
                CanReadProperty(true);
                return _searchCriteriaId;
            }
            set
            {
                CanWriteProperty(true);
                if(_searchCriteriaId != value)
                {
                    _searchCriteriaId = value;
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
            set
            {
                CanWriteProperty(true);
                if(_searchCriteriaType != value)
                {
                    _searchCriteriaType = value;
                    PropertyHasChanged();
                }
            }
        }

        public virtual int NumHits
        {
            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
            get
            {
                CanReadProperty(true);
                return _numHits;
            }
            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
            set
            {
                CanWriteProperty(true);
                _numHits = value;
                PropertyHasChanged();
            }
        }

        public virtual List<Hit> Hits
        {
            get
            {
                CanReadProperty(true);
                return _hits;
            }
            set
            {
                CanWriteProperty(true);
                _hits = value;
                PropertyHasChanged();
            }
        }

        public int[] HitIds
        {

            get
            {
                CanReadProperty(true);
                _hitIDs = null;
                _hitIDsList = new List<int>();
                foreach (Hit hit in this.Hits)
                {
                    _hitIDsList.Add(hit.HitID);
                }
                _hitIDs = _hitIDsList.ToArray();
                return _hitIDs;
            }
            set
            {
                CanWriteProperty(true);
                Hits = new List<Hit>();
                foreach (int val in value)
                {
                    Hits.Add(new Hit(val));
                }
            }
        }

        private double[] SortIds
        {

            get
            {
                CanReadProperty(true);
                _sortIDs = null;
                _sortIDsList = new List<double>();
                foreach (Hit hit in this.Hits)
                {
                    _sortIDsList.Add(hit.SortID);
                }
                _sortIDs = _sortIDsList.ToArray();
                return _sortIDs;
            }
        }

        public virtual SmartDate DateCreated
        {
            get
            {
                CanReadProperty(true);
                return _dateCreated;
            }
            set
            {
                CanWriteProperty(true);
                if (_dateCreated != value)
                {
                    _dateCreated = value;
                    PropertyHasChanged();
                }
            }
        }
        public string TempHitListIDTableName
        {
            get
            {
                return _tempHitListIDTableName;
            }

        }

        public string TempHitListTableName
        {
            get
            {
                return _tempHitListTableName;
            }

        }

        public string TempHitListSchemaName
        {
            get
            {
                return _tempHitListSchemaName;
            }
        }

        public string SavedHitListIDTableName
        {
            get
            {
                return _savedHitListIDTableName;
            }

        }       

        public string SavedHitListTableName
        {
            get
            {
                return _savedHitListTableName;
            }

        }

        public string SavedHitListSchemaName
        {
            get
            {
                return _savedHitListSchemaName;
            }

        }
       
        #endregion

        #region Business base requirements

        protected override object GetIdValue()
        {
            return _id;
        }

        #endregion

        #region Factory methods

        //this method must be called prior to any other method inorder to set the database that the dal will use
        internal static void SetDatabaseName()
        {
            COEDatabaseName.Set(Resources.CentralizedStorageDB);
        }

        internal static void SetDatabaseName(string databaseName)
        {
            COEDatabaseName.Set(databaseName);
        }

        /// <summary>
        /// Creates a new COEHitListBO object with default vaules
        /// </summary>
        /// <param name="hitListType">specify the type of hitlist to create</param>
        /// <returns>a COEHitListBO object with default values</returns>
        public static COEHitListBO New(string databaseName, HitListType hitListType)
        {
            SetDatabaseName();
            //this creates a new hitlist object with default values.
            return DataPortal.Create<COEHitListBO>(new CreateNewCriteria(hitListType, databaseName));
        }

        /// <summary>
        /// Creates a new COEHitListBO object with default vaules
        /// </summary>
        /// <param name="hitListType">specify the type of hitlist to create</param>
        /// <returns>a COEHitListBO object with default values</returns>
        public static COEHitListBO New(string databaseName, HitListType hitListType, int hitListID, int dataViewID)
        {
            SetDatabaseName();
            //this creates a new hitlist object with default values.
            return DataPortal.Create<COEHitListBO>(new CreateNewCriteria(hitListType, databaseName, hitListID, dataViewID));
        }


        /// <summary>
        /// Overload. Creates a new COEHitListBO object set to a particular hitlistid
        /// </summary>
        /// <param name="hitListType">specify the type of hitlist to create</param>
        /// <returns>a COEHitListBO object with default values</returns>
        public static COEHitListBO New(string databaseName, HitListType hitListType, int hitListID)
        {
            SetDatabaseName();
            return DataPortal.Create<COEHitListBO>(new CreateNewCriteria(hitListType, databaseName, hitListID));
        }


        /// <summary>
        /// Gets a COEHitListBO object from the coe hitlist tables
        /// </summary>
        /// <param name="hitListType">specify the type of hitlist to be returned</param>
        /// <returns>the COEHitListBO object for the specified id and list type/returns>
        public static COEHitListBO Get(HitListType hitListType, int id)
        {
            SetDatabaseName();
            if (!COEHitListBO.CanGetHitList())
                throw new System.Security.SecurityException("User not authorized to view a hitlist.");
            return DataPortal.Fetch<COEHitListBO>(new Criteria(hitListType, id));
        }

        public static COEHitListBO GetMarkedHitList(string databaseName, string userName, int dataViewID)
        {
            SetDatabaseName();
            if (!COEHitListBO.CanGetHitList())
                throw new System.Security.SecurityException("User not authorized to view a hitlist.");
            return DataPortal.Fetch<COEHitListBO>(new MarkedCriteria(databaseName, userName, dataViewID));
        }

        /// <summary>
        /// Deletes a COEHitListBO object from the coe hitlist tables
        /// </summary>
        /// <param name="hitListType">specify the type of hitlist to be deleted</param>
        public static void Delete(HitListType hitListType, int id)
        {
            SetDatabaseName();
            if (!COEHitListBO.CanDeleteHitList())
                throw new System.Security.SecurityException("User not authorized to delete a hitlist.");
            DataPortal.Delete(new Criteria(hitListType, id));
        }

        /// <summary>
        /// Stores the current associated searchcriteria into the database.
        /// </summary>
        public void AssociateSearchCriteria()
        {
            _associatingCriteria = true;
            base.Save();
            _associatingCriteria = false;
        }

        /// <summary>
        /// As Save is intended for moving a temp hitlist to be saved, this methods works as CSLA's Save. That is, insert, delete, or update 
        /// would be performed based on the BusinessObject state.
        /// </summary>
        public COEHitListBO Update()
        {
            return base.Save();
        }


        /// <summary>
        /// This method creates a new SAVED hitlist from a given one. To performing CSLA Save() method, use Update() method
        /// </summary>
        /// <returns></returns>
        public override COEHitListBO Save()
        {
            //if (this.HitListType != HitListType.SAVED)
            //{
                if (!COEHitListBO.CanEditHitList())
                    throw new System.Security.SecurityException("User not authorized to edit a hitlist.");
                SetDatabaseName();
                SavedHitListCommand result;
                result = DataPortal.Execute<SavedHitListCommand>(new SavedHitListCommand(this));
                return DataPortal.Fetch<COEHitListBO>(new Criteria(HitListType.SAVED, result.SavedHitListID));
            //}
            //else
            //{
            //    throw new Exception("The HitList Type is " + HitListType.SAVED.ToString());
            //}
        }

        public virtual COEHitListBO AddToRecent()
        {
            if (!COEHitListBO.CanEditHitList())
                throw new System.Security.SecurityException("User not authorized to edit a hitlist.");

            SetDatabaseName();
            AddToRecentHitListCommand result;
            result = DataPortal.Execute<AddToRecentHitListCommand>(new AddToRecentHitListCommand(this));
            return DataPortal.Fetch<COEHitListBO>(new Criteria(HitListType.TEMP, result.NewHitListID));
        }

        public virtual int SynchronizeRecordCount()
        {
            if (!COEHitListBO.CanEditHitList())
                throw new System.Security.SecurityException("User not authorized to edit a hitlist.");

            SetDatabaseName();
            SynchronizeRecordCountCommand result;

            COEDataViewBO dataView = COEDataViewBO.Get(this.DataViewID);

            string baseTableName = dataView.DataViewManager.BaseTable;
            if (baseTableName.IndexOf("(") > 0)
                baseTableName = baseTableName.Remove(baseTableName.IndexOf("(")).Trim();

            string primaryKeyName = dataView.DataViewManager.Tables[baseTableName].PrimaryKeyName;
            if (primaryKeyName.IndexOf("(") > 0)
                primaryKeyName = primaryKeyName.Remove(primaryKeyName.IndexOf("(")).Trim();

            baseTableName = baseTableName.Insert(0, dataView.DataViewManager.DataBase + ".");
            //primaryKeyName = primaryKeyName.Insert(0, baseTableName + ".");

            result = DataPortal.Execute<SynchronizeRecordCountCommand>(new SynchronizeRecordCountCommand(this.HitListID, this.HitListType, baseTableName, primaryKeyName));

            return result.RecordCount;
        }
        #endregion

        #region Hits
        [Serializable()]
        public class Hit
        {
            private int _hitID;
            private double _sortID;


            public Hit(int hitID, double sortID)
            {
                _hitID = hitID;
                _sortID = sortID;



            }

            public Hit(int hitID)
            {
                _hitID = hitID;
                _sortID = -1;
            }

            public int HitID
            {
                get { return _hitID; }
                set { _hitID = value; }
            }

            public double SortID
            {
                get { return _sortID; }
                set { _sortID = value; }
            }


        }



        #endregion

        #region Criteria classes

        [Serializable()]
        protected class Criteria
        {
            private int _id;
            private HitListType _hitListType;


            public Criteria(HitListType hitListType, int id)
            {
                _id = id;
                _hitListType = hitListType;
            }


            public int ID
            {
                get
                {
                    return _id;
                }
            }

            public HitListType HitListType
            {
                get
                {
                    return _hitListType;
                }
            }


        }

        #region CreateNewCriteria classes

        [Serializable()]
        protected class CreateNewCriteria
        {
            private HitListType _hitListType;
            private string _databaseName;
            private int _dataViewID;
            private int _hitListID;

            public CreateNewCriteria(HitListType hitListType, string databaseName)
            {
                _hitListType = hitListType;
                _databaseName = databaseName;
            }

            public CreateNewCriteria(HitListType hitListType, string databaseName, int hitListID)
            {
                _hitListType = hitListType;
                _databaseName = databaseName;
                _hitListID = hitListID;
            }

            public CreateNewCriteria(HitListType hitListType, string databaseName, int hitListID, int dataViewID)
            {
                _hitListType = hitListType;
                _databaseName = databaseName;
                _hitListID = hitListID;
                _dataViewID = dataViewID;
            }

            public int DataViewID
            {
                get
                {
                    return _dataViewID;
                }
            }

            public int HitListID
            {
                get
                {
                    return _hitListID;
                }
            }

            public HitListType HitListType
            {
                get
                {
                    return _hitListType;
                }
            }

            public string DatabaseName
            {
                get
                {
                    return _databaseName;
                }
            }

        }


        [Serializable()]
        protected class SaveTempCriteria
        {
            private int _tempID;

            public SaveTempCriteria(int tempID)
            {
                _tempID = tempID;
            }

            public int TempID
            {
                get
                {
                    return _tempID;
                }
            }
        }
        #endregion

        #region MarkedCriteria
        [Serializable()]
        protected class MarkedCriteria
        {
            private string _databaseName;
            private string _userName;
            private int _dataViewID;


            public MarkedCriteria(string databaseName, string userName, int dataViewID)
            {
                _databaseName = databaseName;
                _userName = userName;
                _dataViewID = dataViewID;
            }

            public string DatabaseName
            {
                get { return _databaseName; }
            }

            public string UserName
            {
                get { return _userName; }
            }

            public int DataViewID
            {
                get { return _dataViewID; }
            }
        }
        #endregion

        #endregion

        #region Data access
        //this can only be called from this assembly or child classes
        protected void DataPortal_Create(CreateNewCriteria criteria)
        {

            switch (criteria.HitListType)
            {
                case HitListType.TEMP:
                    _userID = COEUser.Get().ToString();
                    _name = "TEMP";
                    _description = "TEMP";
                    break;
                case HitListType.SAVED:
                    _userID = COEUser.Get().ToString();
                    _name = string.Empty;
                    _description = string.Empty;
                    break;
                case HitListType.MARKED:
                    _userID = COEUser.Get().ToString();
                    _name = "USERMARKED";
                    _description = "USERMARKED";
                    break;

            }
            _isPublic = false;
            _numHits = 0;
            _hitListType = criteria.HitListType;
            _databaseName = criteria.DatabaseName;
            _dataViewID = criteria.DataViewID;
            _hitlistID = criteria.HitListID;
            _originalHitListType = _hitListType;
        }

        //here we are cheating and using fetch to save a temp hitlist to be a saved hitlist. I could have done this with a command object.
        protected void DataPortal_Fetch(SaveTempCriteria criteria)
        {
            //we are creating the complete new record here
            if (_coeDAL == null) { LoadDAL(); }
            _hitListType = HitListType.SAVED;
            // Coverity Fix CID - 11468 
            if (_coeDAL != null)
                this._id = _coeDAL.CreateNewSavedHitListFromTemp(criteria.TempID, _name, _description, _isPublic);
            else
                throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));
        }

        protected void DataPortal_Fetch(Criteria criteria)
        {
            try
            {
                if (_coeDAL == null) { LoadDAL(); }
                 // Coverity Fix CID - 11557
                if (_coeDAL != null)
                {
                    using (SafeDataReader dr = (SafeDataReader)_coeDAL.GetHitList(criteria.ID, criteria.HitListType))
                    {
                        Fetch(criteria.HitListType, dr);
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

        protected void DataPortal_Fetch(MarkedCriteria criteria)
        {
            try
            {
                if (_coeDAL == null) { LoadDAL(); }
                // Coverity Fix CID - 11556
                if (_coeDAL != null)
                {
                    using (SafeDataReader dr = (SafeDataReader)_coeDAL.GetMarkedHitList(criteria.DatabaseName, criteria.UserName, criteria.DataViewID))
                    {
                        Fetch(HitListType.MARKED, dr);
                        _hitListType = HitListType.MARKED;
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

        protected void Fetch(HitListType hitListType, SafeDataReader dr)
        {
            if (dr.Read())
            {
                _id = dr.GetInt32("id");
                _name = dr.GetString("name");
                _description = dr.GetString("description");
                _userID = dr.GetString("user_id");
                if (dr.GetString("is_public") == "1")
                {
                    _isPublic = true;
                }
                else
                {
                    _isPublic = false;
                }
                _hitlistID = dr.GetInt32("hitlistid");
                _numHits = dr.GetInt32("number_hits");
                _dateCreated = dr.GetSmartDate("Date_Created");
                _hitListType = hitListType;
                _databaseName = dr.GetString("database");
                _dataViewID = dr.GetInt32("dataview_id");
                _parentHitListID = dr.GetInt32("parent_hitlist_id");
                if (!string.IsNullOrEmpty(dr.GetString("type")))
                    _originalHitListType = (HitListType)Enum.Parse(typeof(HitListType), dr.GetString("type"));
                else
                    _originalHitListType = _hitListType;
                
                _searchCriteriaId = dr.GetInt32("search_criteria_id");
                if(!string.IsNullOrEmpty(dr.GetString("search_criteria_type")))
                    _searchCriteriaType = (SearchCriteriaType) Enum.Parse(typeof(SearchCriteriaType), dr.GetString("search_criteria_type"));
                else
                    _searchCriteriaType = SearchCriteriaType.TEMP;

            }
        }

        protected override void DataPortal_Insert()
        {
            //we are creating the complete new record here
            if (_coeDAL == null) { LoadDAL(); }

            if (_hits != null)
            {
                _numHits = _hits.Count;
            }

            // Coverity Fix CID - 11558 
            if (_coeDAL != null)
            {
                switch (_hitListType)
                {
                    case HitListType.TEMP:
                        this._id = _coeDAL.CreateNewTempHitList(_name, _isPublic, _description, _userID, _numHits, _databaseName, _hitlistID, _dataViewID, _originalHitListType, _searchCriteriaId, _searchCriteriaType.ToString());
                        break;
                    case HitListType.SAVED:
                        this._id = _coeDAL.CreateNewSavedHitList(_name, _isPublic, _description, _userID, _numHits, _databaseName, _hitlistID, _dataViewID, _searchCriteriaId, _searchCriteriaType.ToString());
                        break;
                    case HitListType.MARKED:
                        this._id = _coeDAL.CreateNewSavedHitList(_name, _isPublic, _description, _userID, _numHits, _databaseName, _hitlistID, _dataViewID, _searchCriteriaId, _searchCriteriaType.ToString());
                        break;
                }

                if (_hits != null)
                {
                    //add hits to hitlisttable
                    _coeDAL.InsertHits(HitIds, SortIds, this._id, _hitListType);
                }
            }
            else
                throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));
        }

        protected override void DataPortal_Update()
        {
            try
            {
                if (_coeDAL == null) { LoadDAL(); }

                // Coverity Fix CID - 11559 
                if (_coeDAL != null)
                {
                    if (_hits != null)
                    {
                        //add hits to hitlisttable
                        _coeDAL.UpdateHits(HitIds, SortIds, this._id, HitListType.TEMP);
                        _numHits = _numHits + _hits.Count;
                    }

                    switch (_hitListType)
                    {
                        case HitListType.TEMP:
                            if (!_associatingCriteria)
                                _coeDAL.UpdateTempHitList(_id, _name, _description, _isPublic, _numHits, _hitlistID, _dataViewID, _parentHitListID, _searchCriteriaId, _searchCriteriaType.ToString());
                            else
                                _coeDAL.AssociateSearchCriteriaWithTempHitList(_id, _searchCriteriaId, _searchCriteriaType.ToString());
                            break;
                        case HitListType.SAVED:
                            if (!_associatingCriteria)
                                _coeDAL.UpdateSavedHitList(_id, _name, _description, _isPublic, _numHits, _hitlistID, _dataViewID, _parentHitListID, _searchCriteriaId, _searchCriteriaType.ToString());
                            else
                                _coeDAL.AssociateSearchCriteriaWithSavedHitList(_id, _searchCriteriaId, _searchCriteriaType.ToString());
                            break;
                        case HitListType.MARKED:
                            if (!_associatingCriteria)
                                _coeDAL.UpdateSavedHitList(_id, _name, _description, _isPublic, _numHits, _hitlistID, _dataViewID, _parentHitListID, _searchCriteriaId, _searchCriteriaType.ToString());
                            else
                                _coeDAL.AssociateSearchCriteriaWithSavedHitList(_id, _searchCriteriaId, _searchCriteriaType.ToString());
                            break;
                    }
                }
                else
                    throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        protected override void DataPortal_DeleteSelf()
        {
            try
            {
                DataPortal_Delete(new Criteria(_hitListType, _id));
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
                // Coverity Fix CID - 11554
                if (_coeDAL != null)
                    _coeDAL.DeleteHitList(criteria.HitListType, criteria.ID);
                else
                    throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));
            }
            catch (Exception)
            {
                throw;
            }
        }



        [Serializable]
        private class SavedHitListCommand : CommandBase
        {
            private COEHitListBO _originalHitList;
            private int _savedHitListID;

            [NonSerialized]
            private DAL _coeDAL = null;
            [NonSerialized]
            private DALFactory _dalFactory = new DALFactory();
            private string _serviceName = "COEHitList";

            public int SavedHitListID
            {
                get
                {
                    return _savedHitListID;
                }
            }

            public SavedHitListCommand(COEHitListBO tempHitList)
            {
                this._originalHitList = tempHitList;
            }

            protected override void DataPortal_Execute()
            {
                try
                {
                    //the heart of the command. In this case it is simply calling a command in ConfigurationUlities, but it could also call a Manager method, or be
                    //a completely new method that calls nothing elase
                    if (_dalFactory == null) { _dalFactory = new DALFactory(); }
                    _dalFactory.GetDAL<DAL>(ref _coeDAL, _serviceName, COEDatabaseName.Get().ToString(), true);
                    
                    _savedHitListID = _coeDAL.CreateNewSavedHitListFromExisting(_originalHitList.ID, _originalHitList.HitListType, _originalHitList.Name, _originalHitList.Description, _originalHitList.UserID, _originalHitList.IsPublic, _originalHitList.NumHits, _originalHitList.DatabaseName, _originalHitList.DataViewID, _originalHitList.SearchCriteriaID, _originalHitList.SearchCriteriaType.ToString());
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }

        [Serializable]
        private class AddToRecentHitListCommand : CommandBase
        {
            private COEHitListBO _originalHitList;
            private int _newHitListID;

            [NonSerialized]
            private DAL _coeDAL = null;
            [NonSerialized]
            private DALFactory _dalFactory = new DALFactory();
            private string _serviceName = "COEHitList";

            public int NewHitListID
            {
                get
                {
                    return _newHitListID;
                }
            }



            public AddToRecentHitListCommand(COEHitListBO originalHitList)
            {
                _originalHitList = originalHitList;
            }


            protected override void DataPortal_Execute()
            {
                try
                {
                    //the heart of the command. In this case it is simply calling a command in ConfigurationUlities, but it could also call a Manager method, or be
                    //a completely new method that calls nothing elase
                    if (_dalFactory == null) { _dalFactory = new DALFactory(); }
                    _dalFactory.GetDAL<DAL>(ref _coeDAL, _serviceName, COEDatabaseName.Get().ToString(), true);
                    _newHitListID = _coeDAL.AddToRecent(_originalHitList.ID, _originalHitList.HitListID, _originalHitList.HitListType, _originalHitList.Name, _originalHitList.Description, _originalHitList.UserID, _originalHitList.IsPublic, _originalHitList.NumHits, _originalHitList.DatabaseName, _originalHitList.DataViewID, _originalHitList.ParentHitListID, _originalHitList.OriginalHitListType.ToString(), _originalHitList.SearchCriteriaID, _originalHitList.SearchCriteriaType.ToString());



                }
                catch (Exception ex)
                {

                    throw;
                }
            }
        }

        [Serializable]
        private class SynchronizeRecordCountCommand : CommandBase
        {
            private string _baseTable;
            private string _primaryKeyName;
            private int _hitlistID;
            private HitListType _hitlistType;
            private int _recordCount;

            [NonSerialized]
            private DAL _coeDAL = null;
            [NonSerialized]
            private DALFactory _dalFactory = new DALFactory();
            private string _serviceName = "COEHitList";

            public int RecordCount
            {
                get
                {
                    return _recordCount;
                }
            }



            public SynchronizeRecordCountCommand(int hitlistID, HitListType hitlistType, string baseTable, string primaryKeyName)
            {
                _hitlistID = hitlistID;
                _hitlistType = hitlistType;
                _baseTable = baseTable;
                _primaryKeyName = primaryKeyName;
            }


            protected override void DataPortal_Execute()
            {
                try
                {
                    if (_dalFactory == null) { _dalFactory = new DALFactory(); }
                    _dalFactory.GetDAL<DAL>(ref _coeDAL, _serviceName, COEDatabaseName.Get().ToString(), true);
                    _recordCount = _coeDAL.SynchronizeRecordCount(_hitlistID, _hitlistType, _baseTable, _primaryKeyName);
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }
        #endregion

        #region Marked HitList

        public void AddMarkedHits(int[] hitsIDs)
        {
            if (_markedHitsIDs == null)
            {
                _markedHitsIDs = new List<int>();
                foreach (int hit in hitsIDs)
                {
                    _markedHitsIDs.Add(hit);
                }
            }
            else
            {
                foreach (int hit in hitsIDs)
                {
                    if (!_markedHitsIDs.Contains(hit)) ;
                    _markedHitsIDs.Add(hit);
                }
            }
        }

        public void MarkHit(int hitID)
        {
            if (this.HitListType != HitListType.MARKED)
                throw new Exception("The hitlisttype is not MARKED.");
            SetDatabaseName();
            MarkHitCommand result = DataPortal.Execute<MarkHitCommand>(new MarkHitCommand(this.ID, hitID));
            this.NumHits += result.RecordsAffected;
        }

        public void UnMarkHit(int hitID)
        {
            if (this.HitListType != HitListType.MARKED)
                throw new Exception("The hitlisttype is not MARKED.");
            SetDatabaseName();
            UnMarkHitCommand result = DataPortal.Execute<UnMarkHitCommand>(new UnMarkHitCommand(this.ID, hitID));
            this.NumHits -= result.RecordsAffected;
        }

        public void UnMarkAllHits()
        {
            if (this.HitListType != HitListType.MARKED)
                throw new Exception("The hitlisttype is not MARKED.");
            SetDatabaseName();
            UnMarkAllHitsCommand result = DataPortal.Execute<UnMarkAllHitsCommand>(new UnMarkAllHitsCommand(this.UserID, this._dataViewID));
            this.NumHits = 0;
        }

        /// <summary>
        /// Marks all hits in a given hitlist
        /// </summary>
        /// <param name="hitlistId">Hitlist containing the hits to mark</param>
        /// <param name="markedHitListId">User's marked hitlist</param>
        /// <param name="hitListType">Hitlist type</param>
        public void MarkAllHits(int hitlistId, int markedHitListId, HitListType hitListType)
        {

            SetDatabaseName();
            MarkAllHitsCommand result = DataPortal.Execute<MarkAllHitsCommand>(new MarkAllHitsCommand(hitlistId, markedHitListId, hitListType, _dataViewID));
            this.NumHits += result.RecordsAffected;
        }

        /// <summary>
        /// Marks all hits in a given hitlist
        /// </summary>
        /// <param name="hitlistId">Hitlist containing the hits to mark</param>
        /// <param name="markedHitListId">User's marked hitlist</param>
        /// <param name="hitListType">Hitlist type</param>
        /// <param name="markedHitsMax">Maximum size of the marked hitlist.</param>
        public int MarkAllHits(int hitlistId, int markedHitListId, HitListType hitListType, int markedHitsMax)
        {

            SetDatabaseName();
            MarkAllHitsCommand result = DataPortal.Execute<MarkAllHitsCommand>(new MarkAllHitsCommand(hitlistId, markedHitListId, hitListType, _dataViewID, markedHitsMax));
            this.NumHits += result.RecordsAffected;
            return result.RecordsAffected;
        }

        [Serializable]
        private class MarkHitCommand : CommandBase
        {
            private int _hitToBeMarked;
            private int _markedHitListID;
            private int _recordsAffected;

            public MarkHitCommand(int id, int hitID)
            {
                _markedHitListID = id;
                _hitToBeMarked = hitID;
            }

            [NonSerialized]
            private DAL _coeDAL = null;
            [NonSerialized]
            private DALFactory _dalFactory = new DALFactory();
            private string _serviceName = "COEHitList";

            public int MarkedHitListID
            {
                get { return _markedHitListID; }
            }

            public int HitToBeMarked
            {
                get { return _hitToBeMarked; }
            }

            public int RecordsAffected
            {
                get { return _recordsAffected; }
            }

            protected override void DataPortal_Execute()
            {
                try
                {
                    if (_dalFactory == null) { _dalFactory = new DALFactory(); }
                    _dalFactory.GetDAL<DAL>(ref _coeDAL, _serviceName, COEDatabaseName.Get().ToString(), true);
                    _markedHitListID = _coeDAL.AddToMarkedHitList(_markedHitListID, new int[1] { _hitToBeMarked }, out _recordsAffected);
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }

        [Serializable]
        private class UnMarkHitCommand : CommandBase
        {
            private int _hitToBeUnMarked;
            private int _markedHitListID;
            private int _recordsAffected;

            public UnMarkHitCommand(int id, int hitID)
            {
                _markedHitListID = id;
                _hitToBeUnMarked = hitID;
            }

            [NonSerialized]
            private DAL _coeDAL = null;
            [NonSerialized]
            private DALFactory _dalFactory = new DALFactory();
            private string _serviceName = "COEHitList";

            public int MarkedHitListID
            {
                get { return _markedHitListID; }
            }

            public int HitToBeUnMarked
            {
                get { return _hitToBeUnMarked; }
            }

            public int RecordsAffected
            {
                get { return _recordsAffected; }
            }

            protected override void DataPortal_Execute()
            {
                try
                {
                    if (_dalFactory == null) { _dalFactory = new DALFactory(); }
                    _dalFactory.GetDAL<DAL>(ref _coeDAL, _serviceName, COEDatabaseName.Get().ToString(), true);
                    _markedHitListID = _coeDAL.RemoveFromMarkedHitList(_markedHitListID, new int[1] { _hitToBeUnMarked }, out _recordsAffected);
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }

        [Serializable]
        private class UnMarkAllHitsCommand : CommandBase
        {
            private string _userId;
            private int _formGroupId;

            public UnMarkAllHitsCommand(string userId, int formGroupId)
            {
                _userId = userId;
                _formGroupId = formGroupId;
            }

            [NonSerialized]
            private DAL _coeDAL = null;
            [NonSerialized]
            private DALFactory _dalFactory = new DALFactory();
            private string _serviceName = "COEHitList";

            public string UserId
            {
                get { return _userId; }
            }

            public int FormGroupId
            {
                get { return _formGroupId; }
            }

            protected override void DataPortal_Execute()
            {
                try
                {
                    if (_dalFactory == null) { _dalFactory = new DALFactory(); }
                    _dalFactory.GetDAL<DAL>(ref _coeDAL, _serviceName, COEDatabaseName.Get().ToString(), true);
                    _coeDAL.UnMarkAllMarked(_userId, _formGroupId);
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }

        [Serializable]
        private class MarkAllHitsCommand : CommandBase
        {
            private HitListType _hitListType;
            private int _formGroupId;
            private int _hitListId;
            private int _markedHitListId;
            private int _recordsAffected;
            private int _markedHitsMax;

            public MarkAllHitsCommand(int hitListId, int markedHitList, HitListType hitListType, int formGroupId)
            {
                _hitListType = hitListType;
                _formGroupId = formGroupId;
                _hitListId = hitListId;
                _markedHitListId = markedHitList;
                _markedHitsMax = int.MaxValue;
            }

            public MarkAllHitsCommand(int hitListId, int markedHitList, HitListType hitListType, int formGroupId, int markedHitsMax)
            {
                _hitListType = hitListType;
                _formGroupId = formGroupId;
                _hitListId = hitListId;
                _markedHitListId = markedHitList;
                _markedHitsMax = markedHitsMax;
            }

            [NonSerialized]
            private DAL _coeDAL = null;
            [NonSerialized]
            private DALFactory _dalFactory = new DALFactory();
            private string _serviceName = "COEHitList";

            public int RecordsAffected
            {
                get { return _recordsAffected; }
            }

            protected override void DataPortal_Execute()
            {
                try
                {
                    if (_dalFactory == null) { _dalFactory = new DALFactory(); }
                    _dalFactory.GetDAL<DAL>(ref _coeDAL, _serviceName, COEDatabaseName.Get().ToString(), true);
                    _recordsAffected = _coeDAL.MarkAllHits(_hitListId, _markedHitListId, _hitListType, _formGroupId, _markedHitsMax);
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }

        #endregion

        #region Exists

        public static bool Exists(int hitListID)
        {
            try
            {
                ExistsCommand result = DataPortal.Execute<ExistsCommand>(new ExistsCommand(hitListID));
                return result.Exists;

            }
            catch (Exception)
            {

                throw;
            }
        }

        [Serializable()]
        private class ExistsCommand : CommandBase
        {
            private int hitListID;
            private bool _Exists;

            public bool Exists
            {
                get { return _Exists; }
            }

            public ExistsCommand(int hitListID)
            {
                this.hitListID = hitListID;
            }

            protected override void DataPortal_Execute()
            {
                //call DAL with this.hitlistID
            }
        }

        #endregion

        #region Validation rules

        protected override void AddBusinessRules()
        {
            ValidationRules.AddRule(Csla.Validation.CommonRules.StringRequired, "Name");
            ValidationRules.AddRule(Csla.Validation.CommonRules.StringMaxLength, new Csla.Validation.CommonRules.MaxLengthRuleArgs("Name", 50));
            ValidationRules.AddRule(Csla.Validation.CommonRules.StringRequired, "Description");
            ValidationRules.AddRule(Csla.Validation.CommonRules.StringMaxLength, new Csla.Validation.CommonRules.MaxLengthRuleArgs("Description", 250));
            ValidationRules.AddRule(Csla.Validation.CommonRules.StringRequired, "UserID");
            //ValidationRules.AddRule(Csla.Validation.CommonRules.st, "IsPublic");
            //ValidationRules.AddRule(Csla.Validation.CommonRules.StringMaxLength, new Csla.Validation.CommonRules.MaxLengthRuleArgs("IsPublic", 1));
            ValidationRules.AddRule(Csla.Validation.CommonRules.StringRequired, "DateCreated");
            //need to add custom rules to the customRules class
        }

        #endregion

        #region Authorization rules
        //For authorization rules
        protected override void AddAuthorizationRules()
        {

            //we really don't need authorization of hitlisa mangement
        }

        public static bool CanAddHitList()
        {   //for now we will send true for everything
            return true;
            //return Csla.ApplicationContext.User.IsInRole("CanSearch");
        }

        public static bool CanGetHitList()
        {
            //for now we will send true for everything
            return true;
            //return Csla.ApplicationContext.User.IsInRole("CanSearch");
        }

        public static bool CanDeleteHitList()
        {
            //for now we will send true for everything
            return true;
            //return Csla.ApplicationContext.User.IsInRole("CanSearch");
        }

        public static bool CanEditHitList()
        {
            //for now we will send true for everything
            return true;
            //return Csla.ApplicationContext.User.IsInRole("CanSearch");
        }

        #endregion

        #region Utility function this may belong in a shared class
        /// <summary>
        /// Utiltiy function to get the fully qualified hitlist table names
        /// </summary>
        public void GetHitListTableNames()
        {
            try
            {
                DatabaseData db = ConfigurationUtilities.GetDatabaseData(COEDatabaseName.Get().ToString());
                string owner = db.Owner;
                HitListUtilities.BuildTempHitListIDTableName(owner, ref _tempHitListIDTableName);
                HitListUtilities.BuildTempHitListTableName(owner, ref _tempHitListTableName);
                HitListUtilities.BuildSavedHitListIDTableName(owner, ref _savedHitListIDTableName);
                HitListUtilities.BuildSavedHitListTableName(owner, ref _savedHitListTableName);

                _tempHitListTableName = Resources.COETempHitListTableName;
                _tempHitListSchemaName = owner;
                _savedHitListTableName = Resources.COESavedHitListTableName;
                _savedHitListSchemaName = owner;

            }
            catch (Exception e)
            {

                throw;
            }
        }





        private void LoadDAL()
        {
            try
            {
                if (_dalFactory == null) { _dalFactory = new DALFactory(); }
                _dalFactory.GetDAL<DAL>(ref _coeDAL, _serviceName, COEDatabaseName.Get().ToString(), true);

            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion

    }

}
