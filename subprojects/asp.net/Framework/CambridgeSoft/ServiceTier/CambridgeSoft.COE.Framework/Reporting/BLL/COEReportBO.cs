using System;
using CambridgeSoft.COE.Framework.COESecurityService;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.Properties;
using Csla;
using Csla.Data;


namespace CambridgeSoft.COE.Framework.COEReportingService
{
    /// <summary>
    /// Report CSLA based business object. Used for CRUD operation on database stored reports.
    /// </summary>
    [Serializable]
    public class COEReportBO : BusinessBase<COEReportBO>
    {
        #region Member Variables

        //declare members
        private int _coeReportId = -1;
        private string _name = string.Empty;
        private SmartDate _dateCreated = new SmartDate(true);
        private string _description = string.Empty;
        private bool _isPublic = false;
        private string _userId = string.Empty;
        private string _databaseName = string.Empty;
        private string _application;
        COEReport _coeReportDefinition;
        private int _dataViewId;
        private COEAccessRightsBO _coeAccessRights;
        private const string CLASSNAME = "COEReportTemplateBO";
        private byte[] _coeReportTemplateHash = null;
        private COEReportType _type;
        private string _category;

        //variable data access
        [NonSerialized]
        private DAL _coeDAL = null;
        [NonSerialized]
        private DALFactory _dalFactory = new DALFactory();
        private string _serviceName = Resources.ReportingServiceName;

        #endregion

        #region Properties

        /// <summary>
        /// Unique identifier.
        /// </summary>
        // Get more info about lambda expressions on http://msdn.microsoft.com/en-us/library/bb397687.aspx
        protected override object GetIdValue()
        {
            return ID;
        }

        public int ID
        {
            get 
            {
                CanReadProperty();
                return _coeReportId; 
            }
            set { 
                CanWriteProperty();

                if (_coeReportId != value)
                {
                    _coeReportId = value;
                    PropertyHasChanged();
                }
            }
        }

        /// <summary>
        /// A name for the report being stored.
        /// </summary>
        public string Name
        {
            get {
                CanReadProperty();
                return _name; 
            }
            set {
                CanWriteProperty();
                if (!_name.Equals(value))
                {
                    _name = value;
                    PropertyHasChanged();
                }
            }
        }

        /// <summary>
        /// The database name that is stored with the object.
        /// </summary>
        public string DataBaseName
        {
            get
            {
                CanReadProperty();
                return _databaseName;
            }
            set
            {
                CanWriteProperty();
                if (!_databaseName.Equals(value))
                {
                    _databaseName = value;
                    PropertyHasChanged();
                }
            }
        }

        /// <summary>
        /// The access rights object
        /// </summary>
        public COEAccessRightsBO COEAccessRights
        {
            get {
                CanReadProperty();
                return _coeAccessRights; 
            }
            set {
                CanWriteProperty();

                if (_coeAccessRights != value)
                {
                    _coeAccessRights = value;
                    PropertyHasChanged();
                }
            }
        }

        /// <summary>
        /// The name of the aplication
        /// </summary>
        public string Aplication
        {
            get {
                CanReadProperty();
                return _application; 
            }
            set 
            {
                CanWriteProperty();
                if (!_application.Equals(value))
                {
                    _application = value;
                    PropertyHasChanged();
                }

            }
        }

        /// <summary>
        /// The date when the data was originally stored.
        /// </summary>
        public DateTime DateCreated
        {
            get {
                CanReadProperty();
                return _dateCreated.Date; 
            }
        }

        /// <summary>
        /// A description for the report.
        /// </summary>
        public string Description
        {
            get {
                CanReadProperty();
                return _description; 
            }
            set {
                CanWriteProperty();
                if (!_description.Equals(value))
                {
                    _description = value;
                    PropertyHasChanged();
                }
            }
        }

        /// <summary>
        /// The template of the report
        /// </summary>
        public COEReport ReportDefinition 
        {
            get 
            {
                CanReadProperty();
                return _coeReportDefinition;
            }
            set { 
                CanWriteProperty();

                if (!CompareByteArray(_coeReportTemplateHash, CambridgeSoft.COE.Framework.Common.Utilities.ComputeMD5(value.ToString())))
                {
                    _coeReportDefinition = value;
                CreateReportTemplateHash();

                    PropertyHasChanged();
            }
        }
        }

        /// <summary>
        /// The user name storing/accessing the data.
        /// </summary>
        public string UserId
        {
            get {
                CanReadProperty();
                return _userId; 
            }
            set {
                CanWriteProperty();

                if (!_userId.Equals(value))
                {
                    _userId = value;
                    PropertyHasChanged();
                }
            }
        }

        /// <summary>
        /// Indicates if only the creator can access this record or if everyone can.
        /// </summary>
        public bool IsPublic
        {
            get {
                CanReadProperty();
                return _isPublic; }
            set {
                CanWriteProperty();

                if (_isPublic != value)
                {
                    _isPublic = value;
                    PropertyHasChanged();
                }
            }
        }

        /// <summary>
        /// The id of the selected dataview
        /// </summary>
        public int DataViewId
        {
            get {
				CanReadProperty();

                if (this._coeReportDefinition != null && this._coeReportDefinition.DataViewId > 0)
                    _dataViewId = _coeReportDefinition.DataViewId;
                
                return _dataViewId; 
            }
            set {
                CanWriteProperty();

                if (_dataViewId != value)
                {
                    _dataViewId = value;
                    PropertyHasChanged();
                }
            }
        }

/// <summary>
        /// Report type: List or Label.
        /// </summary>
        public COEReportType ReportType
        {
            get 
            { 
                CanReadProperty();
                return _type; 
            }
            set 
            { 
                CanWriteProperty();

                if(_type == null || !_type.Equals(value))
                {
                    _type = value;
                    PropertyHasChanged();
                }
            }
        }

        public string Category
        {
            get 
            {
                CanReadProperty();
                return _category; 
            }
            set 
            { 
                CanWriteProperty();

                if(_category == null || !_category.Equals(value))
                {
                    _category = value; 
                    PropertyHasChanged();
                }
            }
        }
        #endregion

        #region Constructors

        /// <summary>
        /// a constructor of the COEReportTemplateBO
        /// </summary>
        /// <param name="coeReportId">the id of the new COEReportTemplateBO</param>
        /// <param name="name">the name of the new COEReportTemplateBO</param>
        /// <param name="description">the description of the new COEReportTemplateBO</param>
        /// <param name="userID">the user id that creates the new COEReportTemplateBO</param>
        /// <param name="isPublic">determines whether a new COEReportTemplateBO is public or not</param>
        /// <param name="dateCreated">creation date of the new COEReportTemplateBO</param>
        /// <param name="coeReportTemplate">the report template of the new COEReportTemplateBO</param>
        /// <param name="databaseName">the database name of the new COEReportTemplateBO</param>
        /// <param name="application">application where the the new COEReportTemplateBO is created</param>
        /// <param name="dataViewId">the dataview id related to the new COEReportTemplateBO</param>
        internal COEReportBO(int coeReportId, string name, string description, string userID, bool isPublic, SmartDate dateCreated, COEReport coeReportTemplate, string databaseName, string application, int dataViewId, COEReportType type, string category)
        {
            _coeReportId = coeReportId;
            _name = name;
            _description = description;
            _userId = userID;
            _dateCreated = dateCreated;
            _isPublic = isPublic;
            _databaseName = databaseName;
            _application = application;
            _coeReportDefinition = coeReportTemplate;
            if (_coeReportDefinition != null)
                _coeReportDefinition.ID = coeReportId;
            _dataViewId = dataViewId;
            _type = type;
            _category = category;
        }

        internal COEReportBO() { }

        #endregion

        #region Factory Methods

        /// <summary>
        /// this method must be called prior to any other method inorder 
        /// to set the database that the dal will use
        /// </summary>
        internal static void SetDatabaseName()
        {
            COEDatabaseName.Set(DALUtils.GetDefaultQualifyDbName(Resources.CentralizedStorageDB));
        }

        /// <summary>
        /// this method must be called prior to any other method inorder to set the database that the dal will use
        /// </summary>
        /// <param name="databaseName">the database name to be set</param>
        internal static void SetDatabaseName(string databaseName)
        {
            COEDatabaseName.Set(databaseName);
        }

        public static COEReportBO New(string name, string description, string userId, bool isPublic, SmartDate dateCreated, COEReport coeReportTemplate, string databaseName, string application, int dataViewId)
        {
            return New(name, description, userId, isPublic, dateCreated, coeReportTemplate, databaseName, application, dataViewId, 0);
        }

        /// <summary>
        /// creates a new instance of COEReportBO object 
        /// </summary>
        /// <param name="name">the name of the new COEReportBO</param>
        /// <param name="description">the description of the new COEReportBO</param>
        /// <param name="userId">the user id that creates the new COEReportBO</param>
        /// <param name="isPublic">determines whether a new COEReportBO is public or not</param>
        /// <param name="dateCreated">creation date of the new COEReportBO</param>
        /// <param name="coeReportTemplate">the report template of the new COEReportBO</param>
        /// <param name="databaseName">the database name of the new COEReportBO</param>
        /// <param name="application">application where the the new COEReportBO is created</param>
        /// <param name="dataViewId">the dataview id related to the new COEReportBO</param>
        /// <returns>a new COEReportBO</returns>
        
        public static COEReportBO New(string name, string description, string userId, bool isPublic, SmartDate dateCreated, COEReport coeReportDefinition, string databaseName, string application, int dataViewId, COEReportType type)
        {
            return New(name, description, userId, isPublic, dateCreated, coeReportDefinition, databaseName, application, dataViewId, type, coeReportDefinition == null ? null : coeReportDefinition.Category);

        }

        public static COEReportBO New(string name, string description, string userId, bool isPublic, SmartDate dateCreated, COEReport coeReportDefinition, string databaseName, string application, int dataViewId, COEReportType type, string category)
        {
            SetDatabaseName();

            if (!CanAddObject())
                throw new System.Security.SecurityException(Resources.UserNotAuthorizedForAddObject + " " + CLASSNAME);

            return DataPortal.Create<COEReportBO>(new CreateNewCriteria(0, name, description, userId, application, databaseName, dateCreated, dataViewId, type, coeReportDefinition, isPublic, category));

        }

        /// <summary>
        /// creates a new instance of COEReportTemplateBO object 
        /// </summary>
        /// <param name="databaseName"></param>
        /// <returns>a new COEReportBO</returns>
        public static COEReportBO New(string databaseName)
        {
            return New(null, null, null, true, new SmartDate(DateTime.Today), null, COEDatabaseName.Get(), COEAppName.Get(), 0, 0);
        }

        public static COEReportBO New(string name, string description, COEReport coeReportDefinition)
        {
            return New(name, description, COEUser.Name, true, new SmartDate(DateTime.Today), coeReportDefinition, COEDatabaseName.Get(), COEAppName.Get(), 0, 0);
        }

        /// <summary>
        /// to get a COEReportBO from object id
        /// </summary>
        /// <param name="coeReportId">the id of the stored COEReportTemplateBO</param>
        /// <returns>a new COEReportTemplateBO</returns>
        public static COEReportBO Get(int coeReportId)
        {
            SetDatabaseName();

            if (!CanGetObject())
                throw new System.Security.SecurityException(Resources.UserNotAuthorizedForViewObject + " " + CLASSNAME);
            return DataPortal.Fetch<COEReportBO>(new Criteria(coeReportId));
        }

        /// <summary>
        /// to get a COEReportBO from object id 
        /// </summary>
        /// <param name="coeReportId">the id of the stored COEReportTemplateBO</param>
        /// <param name="includeAccessRights"></param>
        /// <returns></returns>
        public static COEReportBO Get(int coeReportId, bool includeAccessRights)
        {
            SetDatabaseName();

            if (!CanGetObject())
                throw new System.Security.SecurityException(Resources.UserNotAuthorizedForViewObject + " " + CLASSNAME);
            return DataPortal.Fetch<COEReportBO>(new Criteria(coeReportId, includeAccessRights));
        }

        /// <summary>
        /// to delete the COEReportBO with id = <paramref name="coeReportId"/>
        /// </summary>
        /// <param name="coeReportId">the id of the COEReportBO to be deleted</param>
        public static void Delete(int coeReportId)
        {
            SetDatabaseName();

            if (!CanDeleteObject())
                throw new System.Security.SecurityException(Resources.UserNotAuthorizedForDeleteObject + " " + CLASSNAME);
            DataPortal.Delete(new Criteria(coeReportId));
        }


        public override COEReportBO Save()
        {
            if (IsDeleted && !CanDeleteObject())
                throw new System.Security.SecurityException(Resources.UserNotAuthorizedForDeleteObject + " " + CLASSNAME);
            else if (IsNew && !CanAddObject())
                throw new System.Security.SecurityException(Resources.UserNotAuthorizedForAddObject + " " + CLASSNAME);
            else if (!CanEditObject())
                throw new System.Security.SecurityException(Resources.UserNotAuthorizedForEditObject + " " + CLASSNAME);
            if (!this.CompareByteArray(_coeReportTemplateHash, CambridgeSoft.COE.Framework.Common.Utilities.ComputeMD5(this.ReportDefinition.ToString())))
                this.MarkDirty();

            return base.Save();
        }

        private void CreateReportTemplateHash()
        {
            _coeReportTemplateHash = CambridgeSoft.COE.Framework.Common.Utilities.ComputeMD5(this.ReportDefinition.ToString());
        }

        private bool CompareByteArray(byte[] operand1, byte[] operand2)
        {
            //if both are the same reference or null return true.
            if (operand1 == operand2)
                return true;

            if (operand1 == null || operand2 == null)
                return false;

            if (operand1.Length == operand2.Length)
                for (int index = 0; index < operand1.Length; index++)
                    if (operand1[index] != operand2[index])
                        return false;

            return true;
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

        #endregion

        #region Data Access

        #region Data Access - Create
        [RunLocal]
        private void DataPortal_Create(CreateNewCriteria criteria)
        {
            //COEReportTemplateBO(0, name, description, userId, isPublic, dateCreated, coeReportTemplate, databaseName, application, dataViewId, reportTypeId)
            _coeReportId = criteria._coeReportId;
            _name = criteria._name;
            _description = criteria._description;
            _userId = criteria._userID;
            _dateCreated = criteria._dateCreated;
            _isPublic = criteria._isPublic;
            _databaseName = criteria._database;
            _application = criteria._application;
            _coeReportDefinition = criteria._coeReportDefinition;
            
            if (_coeReportDefinition != null)
                _coeReportDefinition.ID = criteria._coeReportId;
            else
                _coeReportDefinition = new COEReport();

            _dataViewId = criteria._dataViewId;
            _type = criteria._reportType;
            _category = criteria._category;
        }

        #endregion

        #region Data Access - Fetch

        private void DataPortal_Fetch(Criteria criteria)
        {
            if (_coeDAL == null) { LoadDAL(); }
            // Coverity Fix CID - 11570 
            if (_coeDAL != null)
            {
                using (SafeDataReader dr = _coeDAL.Get(criteria._coeReportId))
                {
                    FetchObject(dr);
                }
            }
            else
                throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));

            if (criteria._includeAccessRights == true)
            {
                this.COEAccessRights = COEAccessRightsBO.Get(COEAccessRightsBO.ObjectTypes.COEGENERICOBJECT, this._coeReportId);
            }
            CreateReportTemplateHash();
        }

        internal void FetchObject(SafeDataReader dr)
        {
            try
            {
                if (dr.Read())
                    LoadObjectFromReader(dr);
                else
                    throw new Exception(Resources.ReadingReportTemplateBO_ErrorMessage);
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }
        internal void LoadObjectFromReader(SafeDataReader dr)
        {
            _coeReportId = dr.GetInt32("ID");
            _name = dr.GetString("NAME");
            _dateCreated = dr.GetSmartDate("DATE_CREATED", _dateCreated.EmptyIsMin);
            _description = dr.GetString("DESCRIPTION");
            _isPublic = (dr.GetString("IS_PUBLIC").Equals("1") ? true : false);
            _userId = dr.GetString("USER_ID");
            _application = dr.GetString("APPLICATION");
            _coeReportDefinition = COEReport.GetCOEReport(dr.GetString("REPORT_TEMPLATE"));
            _databaseName = dr.GetString("DATABASE");
            _dataViewId = dr.GetInt32("DATAVIEW_ID");
            _type = (COEReportType)Enum.Parse(typeof(COEReportType), dr.GetString("TYPE"));
            _category = dr.GetString("CATEGORY");
        }

        #endregion

        #region Data Access - Insert

        //called by other services
        internal void Insert(DAL coeDAL)
        {
            if (!IsDirty)
                return;
            if (_coeDAL == null)
                LoadDAL();
            if (_coeAccessRights != null)
                _isPublic = false;

            _coeReportId = coeDAL.Insert(_name, _isPublic, _description, _userId, _coeReportDefinition, _databaseName, _application, DataViewId, _type, _category);

            if (_coeAccessRights != null)
            {
                _coeAccessRights.ObjectID = _coeReportId;
                _coeAccessRights.ObjectType = COEAccessRightsBO.ObjectTypes.COEGENERICOBJECT;
                _coeAccessRights.Save();
            }
            MarkOld();
            MarkClean();
        }

        protected override void DataPortal_Insert()
        {
            if (_coeDAL == null) { LoadDAL(); }
            // Coverity Fix CID - 11571
            if (_coeDAL != null)
            {
                if (_coeAccessRights != null)
                    _isPublic = false;

                _coeReportId = _coeDAL.Insert(_name, _isPublic, _description, _userId, _coeReportDefinition, _databaseName, _application, DataViewId, _type, _category);
                if (_coeAccessRights != null)
                {
                    _coeAccessRights.ObjectID = _coeReportId;
                    _coeAccessRights.ObjectType = COEAccessRightsBO.ObjectTypes.COEGENERICOBJECT;
                    _coeAccessRights.Save();
                }
                MarkOld();
                MarkClean();
            }
            else
                throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));
        }
        #endregion

        #region Data Access - Update

        //called by other services
        internal void Update(DAL coeDAL)
        {
            if (!IsDirty) return;
            coeDAL.Update(_coeReportId, _name, _isPublic, _description, _userId, _coeReportDefinition.ToString(), _databaseName, _application, DataViewId, _type, _category);
            MarkOld();
        }

        protected override void DataPortal_Update()
        {
            if (_coeDAL == null) { LoadDAL(); }
            // Coverity Fix CID - 11572
            if (_coeDAL != null)
            {
                if (IsDirty)
                {
                    string serializedCOEReportTemplate = _coeReportDefinition.ToString();
                    _coeDAL.Update(_coeReportId, _name, _isPublic, _description, _userId, _coeReportDefinition.ToString(), _databaseName, _application, DataViewId, _type, _category);
                }
            }
            else
                throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));
        }
        #endregion

        #region Data Access - Delete

        //called by other services
        internal void DeleteSelf(DAL _coeDAL)
        {
            if (_coeDAL == null) { LoadDAL(); }
            // Coverity Fix CID - 11573 
            if (_coeDAL != null)
                _coeDAL.Delete(_coeReportId);
            else
                throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));
        }

        protected override void DataPortal_DeleteSelf()
        {
            DataPortal_Delete(new Criteria(_coeReportId));
        }

        private void DataPortal_Delete(Criteria criteria)
        {
            if (_coeDAL == null) { LoadDAL(); }
            // Coverity Fix CID - 11569 
            if (_coeDAL != null)
                _coeDAL.Delete(criteria._coeReportId);
            else
                throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));
        }
        #endregion

        #endregion

        #region Criteria

        [Serializable()]
        private class Criteria
        {
            #region Variables
            internal int _coeReportId;
            internal bool _includeAccessRights;
            #endregion

            #region Constructors
            public Criteria(int coeReportId)
            {
                _coeReportId = coeReportId;
                _includeAccessRights = false;
            }

            public Criteria(int coeReportId, bool includeAccessRights)
            {
                _coeReportId = _coeReportId;
                _includeAccessRights = includeAccessRights;
            }
            #endregion
        }
        #endregion

        #region CreateNewCriteria classes

        [Serializable()]
        protected class CreateNewCriteria
        {
            internal int _coeReportId;
            internal string _name;
            internal string _description;
            internal string _userID;
            internal bool _isPublic;
            internal SmartDate _dateCreated;
            internal COEReport _coeReportDefinition;
            internal string _application;
            internal int _dataViewId;
            internal COEReportType _reportType;
            internal string _database;
            internal string _category;


            public CreateNewCriteria(int coeReportid, string name, string description, string userId, string application, string database, SmartDate dateCreated, int dataviewId, COEReportType reportType, COEReport coeReportDefinition, bool isPublic, string category)
            {
                _coeReportId = coeReportid;
                _name = name;
                _description = description;
                _userID = userId;
                _application = application;
                _database = database;
                _dateCreated = dateCreated;
                _dataViewId = dataviewId;
                _reportType = reportType;
                _coeReportDefinition = coeReportDefinition;
                _isPublic = isPublic;
                _database = database;
                _category = category;
            }

            public CreateNewCriteria(string database)
            {
                _database = database;
            }
        }

        #endregion

        #region LoadDAL

        private void LoadDAL()
        {
            if (_dalFactory == null) { _dalFactory = new DALFactory(); }
            _dalFactory.GetDAL<DAL>(ref _coeDAL, _serviceName, COEDatabaseName.Get(), true);
        }

        #endregion
    }
}
