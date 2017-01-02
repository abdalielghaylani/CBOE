using System;
using System.Data;
using System.Data.SqlClient;
using Csla;
using Csla.Data;
using Csla.Validation;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.Properties;
using CambridgeSoft.COE.Framework.COELoggingService;
using CambridgeSoft.COE.Framework.COESecurityService;

namespace CambridgeSoft.COE.Framework.COEGenericObjectStorageService
{
    /// <summary>
    /// This class provides the ability to save/retrieve a generic object by receiving a string representation of it.
    /// </summary>
    [Serializable()]
    public class COEGenericObjectStorageBO : Csla.BusinessBase<COEGenericObjectStorageBO>
    {
        #region Member variables
        //declare members
        private int _id = 0;
        private string _name = string.Empty;
        private SmartDate _dateCreated = new SmartDate(true);
        private string _description = string.Empty;
        private bool _isPublic = false;
        private string _associatedDataviewID = string.Empty;
        private string _userName = string.Empty;
        private int _formGroup = 0;
        private string _coeGenericObject = string.Empty;
        private string _databaseName = string.Empty;
        private COEAccessRightsBO _coeAccessRights;
        //variables data access
        [NonSerialized]
        private DAL _coeDAL = null;
        [NonSerialized]
        private DALFactory _dalFactory = new DALFactory();
        private string _serviceName = "COEGenericObjectStorage";
        [NonSerialized]
        static COELog _coeLog = COELog.GetSingleton("COEGenericObjectStorage");


        #endregion

        #region Properties
        /// <summary>
        /// The database name that is stored with the object.
        /// </summary>
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
                if(_databaseName != value)
                {
                    _databaseName = value;
                    PropertyHasChanged();
                }
            }
        }

        /// <summary>
        /// Unique identifier.
        /// </summary>
        [System.ComponentModel.DataObjectField(true, false)]
        public int ID
        {
            get
            {
                CanReadProperty("ID", true);
                return _id;
            }
            set { _id = value; }
        }

        /// <summary>
        /// A name for the generic object being stored.
        /// </summary>
        public string Name
        {
            get
            {
                CanReadProperty("Name", true);
                return _name;
            }
            set
            {
                CanWriteProperty(true);
                if(value == null)
                    value = string.Empty;
                if(!_name.Equals(value))
                {
                    _name = value;
                    PropertyHasChanged();
                }
            }
        }

        /// <summary>
        /// The date when the data was originally stored.
        /// </summary>
        public DateTime DateCreated
        {
            get
            {
                CanReadProperty("DateCreated", true);
                return _dateCreated.Date;
            }
        }

        /// <summary>
        /// A description for the generic object.
        /// </summary>
        public string Description
        {
            get
            {
                CanReadProperty("Description", true);
                return _description;
            }
            set
            {
                CanWriteProperty(true);
                if(value == null)
                    value = string.Empty;
                if(!_description.Equals(value))
                {
                    _description = value;
                    PropertyHasChanged("Description");
                }
            }
        }

        /// <summary>
        /// Indicates if only the creator can access this record or if everyone can.
        /// </summary>
        public bool IsPublic
        {
            get
            {
                CanReadProperty("IsPublic", true);
                return _isPublic;
            }
            set
            {
                CanWriteProperty(true);
                if(!_isPublic.Equals(value))
                {
                    _isPublic = value;
                    PropertyHasChanged("IsPublic");
                }
            }
        }

        /// <summary>
        /// Indicates dataview ID from which this form is created.
        /// </summary>
        public string AssociatedDataviewID
        {
            get
            {
                CanReadProperty("AssociatedDataviewID", true);
                return _associatedDataviewID;
            }
            set
            {
                CanWriteProperty(true);
                if (value == null)
                    value = string.Empty;
                if (!_associatedDataviewID.Equals(value))
                {
                    _associatedDataviewID = value;
                    PropertyHasChanged("AssociatedDataviewID");
                }
            }
        }

        /// <summary>
        /// The user name storing/accessing the data.
        /// </summary>
        public string UserName
        {
            get
            {
                CanReadProperty("UserName", true);
                return _userName;
            }
            set
            {
                CanWriteProperty(true);
                if(value == null)
                    value = string.Empty;
                if(!_userName.Equals(value))
                {
                    _userName = value;
                    PropertyHasChanged("UserName");
                }
            }
        }

        /// <summary>
        /// Form group id associated with the current data.
        /// </summary>
        public int FormGroup
        {
            get
            {
                CanReadProperty("FormGroup", true);
                return _formGroup;
            }
            set
            {
                CanWriteProperty(true);
                if(!_formGroup.Equals(value))
                {
                    _formGroup = value;
                    PropertyHasChanged("FormGroup");
                }
            }
        }

        /// <summary>
        /// The string representation of the object being saved/accessed.
        /// </summary>
        public string COEGenericObject
        {
            get
            {
                CanReadProperty("COEGenericObject", true);
                return _coeGenericObject;
            }
            set
            {
                CanWriteProperty(true);
                if(_coeGenericObject != value)
                {
                    _coeGenericObject = value;
                    PropertyHasChanged("COEGenericObject");
                }
            }
        }


        protected override object GetIdValue()
        {
            return _id;
        }

        /// <summary>
        /// People and groups with permissions granted to manipulate this object.
        /// </summary>
        public COEAccessRightsBO COEAccessRights
        {
            get
            {
                return _coeAccessRights;
            }
            set
            {
                _coeAccessRights = value;
            }
        }
        #endregion

        #region Constructors

        private COEGenericObjectStorageBO()
        { /* require use of factory method */ }

        //constructor to be called from queryCriteriaList as well as any other services that needs to construct this object
        internal COEGenericObjectStorageBO(int id, string name, string description, string userID, bool isPublic, int formGroup, SmartDate dateCreated, string coeGenericObject, string databaseName)
        {
            _id = id;
            _name = name;
            _description = description;
            _userName = userID;
            _dateCreated = dateCreated;
            _isPublic = isPublic;
            _formGroup = formGroup;
            _coeGenericObject = coeGenericObject;
            _databaseName = databaseName;

        }

        internal COEGenericObjectStorageBO(int id, string name, string description, string userID, bool isPublic, int formGroup, SmartDate dateCreated, string coeGenericObject, string databaseName, bool markAsChild, bool isNew, bool isDirty, string associatedDataviewID = "")
        {
            _id = id;
            _name = name;
            _description = description;
            _userName = userID;
            _dateCreated = dateCreated;
            _isPublic = isPublic;
            _formGroup = formGroup;
            _coeGenericObject = coeGenericObject;
            _databaseName = databaseName;
            _associatedDataviewID = associatedDataviewID;
            if(markAsChild)
                this.MarkAsChild();
            if(!isNew)
                this.MarkOld();
            if(!isDirty)
                this.MarkClean();
        }

        internal COEGenericObjectStorageBO(string coeGenericObject)
        {
            _coeGenericObject = coeGenericObject;
        }

        #endregion

        #region Validation Rules

        private void AddCommonRules()
        {
            // Name
            ValidationRules.AddRule(CommonRules.StringRequired, "Name");
            ValidationRules.AddRule(CommonRules.StringMaxLength, new CommonRules.MaxLengthRuleArgs("Name", 255));

            //Description
            ValidationRules.AddRule(CommonRules.StringMaxLength, new CommonRules.MaxLengthRuleArgs("Description", 255));

            // DateCreated
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

        /// <summary>
        /// Instantiates a <see cref="COEGenericObjectStorageBO"/> to perform actions against the given database.
        /// </summary>
        /// <param name="databaseName">The database name</param>
        /// <returns>The <see cref="COEGenericObjectStorageBO"/></returns>
        public static COEGenericObjectStorageBO New(string databaseName)
        {
            SetDatabaseName();

            if(!CanAddObject())
                throw new System.Security.SecurityException(Resources.UserNotAuthorizedForAddObject + " COEGenericObjectStorageBO");
            return DataPortal.Create<COEGenericObjectStorageBO>(new CreateNewCriteria(databaseName));
        }

        /// <summary>
        /// Instantiates a <see cref="COEGenericObjectStorageBO"/> to perform actions against the given database and a given record id.
        /// </summary>
        /// <param name="id">The record id</param>
        /// <returns>The <see cref="COEGenericObjectStorageBO"/></returns>
        public static COEGenericObjectStorageBO Get(int id)
        {
            SetDatabaseName();

            if(!CanGetObject())
                throw new System.Security.SecurityException(Resources.UserNotAuthorizedForViewObject + " COEGenericObjectStorageBO");
            return DataPortal.Fetch<COEGenericObjectStorageBO>(new Criteria(id));
        }

        /// <summary>
        /// Instantiates a <see cref="COEGenericObjectStorageBO"/> to perform actions against the given database , user and fromgroupid
        /// </summary>
        /// <param name="userName">logged in user</param>
        /// <param name="formgroup">formgroupid</param>
        /// <returns></returns>
        public static COEGenericObjectStorageBO Get(string userName,int formgroup)
        {
            SetDatabaseName();

            if (!CanGetObject())
                throw new System.Security.SecurityException(Resources.UserNotAuthorizedForViewObject + " COEGenericObjectStorageBO");
            return DataPortal.Fetch<COEGenericObjectStorageBO>(new UserCriteria(userName,formgroup));
        }
        /// <summary>
        /// Instantiates a <see cref="COEGenericObjectStorageBO"/> to perform actions against the given database and a given record id and a flag that indicates if the access rights for the object should be retrieved.
        /// </summary>
        /// <param name="id">The record id</param>
        /// <param name="includeAccessRights">A flag that telling if access rights are to be retrieved</param>
        /// <returns>The <see cref="COEGenericObjectStorageBO"/></returns>
        public static COEGenericObjectStorageBO Get(int id, bool includeAccessRights)
        {
            SetDatabaseName();
            if(!CanGetObject())
                throw new System.Security.SecurityException(Resources.UserNotAuthorizedForViewObject + " COEGenericObjectStorageBO");

            return DataPortal.Fetch<COEGenericObjectStorageBO>(new Criteria(id, includeAccessRights));
        }

        /// <summary>
        /// Permanently removes the record with the given <paramref name="id"/> from the database named <paramref name="databaseName"/>.
        /// </summary>
        /// <param name="databaseName">The database name</param>
        /// <param name="id">The record id</param>
        public static void Delete(int id)
        {
            SetDatabaseName();

            if(!CanDeleteObject())
                throw new System.Security.SecurityException(Resources.UserNotAuthorizedForDeleteObject + " COEGenericObjectStorageBO");
            DataPortal.Delete(new Criteria(id));
        }

        /// <summary>
        /// Based on the object state decides if an insertion, deletion or update is needed.
        /// We strongly encourage using all save methods asigning the returned object back to the original one, as CSLA recommends. This is to avoid incoherencies between the object in the Client Side and the one in the Server Side caused by serialization mechanisms.
        /// 
        /// So, please do:
        /// 
        /// <code language="C#">
        /// GenericObjectStorageBO yourBO;
        /// // Fill in your bo as you need.
        /// yourBO = yourBO.Save();
        /// </code>
        /// </summary>
        /// <returns>The modified <see cref="COEGenericObjectStorageBO"/></returns>
        public override COEGenericObjectStorageBO Save()
        {
            if(IsDeleted && !CanDeleteObject())
                throw new System.Security.SecurityException(Resources.UserNotAuthorizedForDeleteObject + " COEGenericObjectStorageBO");
            else if(IsNew && !CanAddObject())
                throw new System.Security.SecurityException(Resources.UserNotAuthorizedForAddObject + " COEGenericObjectStorageBO");
            else if(!CanEditObject())
                throw new System.Security.SecurityException(Resources.UserNotAuthorizedForEditObject + " COEGenericObjectStorageBO");

            return base.Save();
        }

        /// <summary>
        /// Determines if the object can be added.
        /// </summary>
        /// <returns>True if the object can be added, false otherwise</returns>
        public static bool CanAddObject()
        {
            // return Csla.ApplicationContext.User.IsInRole("CanSearch");
            return true;
        }

        /// <summary>
        /// Determines if the object can be retrieved.
        /// </summary>
        /// <returns>True if the object can be retrieved, false otherwise</returns>
        public static bool CanGetObject()
        {
            // return Csla.ApplicationContext.User.IsInRole("CanSearch");
            return true;
        }

        /// <summary>
        /// Determines if the object can be edited.
        /// </summary>
        /// <returns>True if the object can be edited, false otherwise</returns>
        public static bool CanEditObject()
        {
            // return Csla.ApplicationContext.User.IsInRole("CanSearch");
            return true;
        }

        /// <summary>
        /// Determines if the object can be deleted.
        /// </summary>
        /// <returns>True if the object can be deleted, false otherwise</returns>
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

        /// <summary>
        /// Simple criteria that has only one parameter: The record id.
        /// </summary>
        [Serializable()]
        private class Criteria
        {
            #region Variables
            internal int _id;
            internal bool _includeAccessRights;
            #endregion

            #region Constructors
            /// <summary>
            /// Initializes the criteria with the given id. Using this constructor means access rights are not to be retrieved.
            /// </summary>
            /// <param name="id">The record id</param>
            public Criteria(int id)
            {
                _id = id;
                _includeAccessRights = false;
            }

            /// <summary>
            /// Initializes the criteria with the given id. The retrieval of access rights is indicated in the second parameter.
            /// </summary>
            /// <param name="id">The record id</param>
            /// <param name="includeAccessRights">A flag that tells if access rights should be retrieved or not</param>
            public Criteria(int id, bool includeAccessRights)
            {
                _id = id;
                _includeAccessRights = includeAccessRights;
            }
            #endregion
        }
        #endregion //Criteria

        #region CreateNewCriteria
        /// <summary>
        /// A simple criteria for creating a new object.
        /// </summary>
        [Serializable()]
        private class CreateNewCriteria
        {
            internal string _database;

            /// <summary>
            /// Initializes the criteria with the given database name.
            /// </summary>
            /// <param name="database"></param>
            public CreateNewCriteria(string database)
            {
                _database = database;
            }
        }
        #endregion //CreateNewCriteria

        /// <summary>
        /// UserCriteria that has the parameter username and formgroup
        /// </summary>
        [Serializable()]
        private class UserCriteria
        {
            #region Variables
            internal string _user;
            internal int _formGroup;
            internal bool _includeAccessRights;
            #endregion

            #region Constructors
            /// <summary>
            /// Initializes the UserCriteria with the given username. Using this constructor means access rights are not to be retrieved.
            /// </summary>
            /// <param name="id">The record id</param>
            public UserCriteria(string userName,int formgroup)
            {
                _user = userName;
                _formGroup =formgroup;
                _includeAccessRights = false;
            }

            /// <summary>
            /// Initializes the Usercriteria with the given username. The retrieval of access rights is indicated in the second parameter.
            /// </summary>
            /// <param name="userName">userid</param>
            /// <param name="includeAccessRights">A flag that tells if access rights should be retrieved or not</param>
            public UserCriteria(string userName, bool includeAccessRights)
            {
                _user = userName;
                _includeAccessRights = includeAccessRights;
            }
            #endregion
        }

        #region Data Access - Create
        [RunLocal]
        private void DataPortal_Create(CreateNewCriteria createNewCriteria)
        {
            _coeGenericObject = null;
            _databaseName = createNewCriteria._database;
            this.MarkNew();
            this.MarkClean();
        }
        #endregion //Data Access - Create

        #region Data Access - Fetch

        //when already on the server, this can be called
        internal void Get(DAL coeDAL, int genericObjectID)
        {
            if(!IsDirty)
                return;
            using(SafeDataReader dr = _coeDAL.Get(genericObjectID))
            {
                FetchObject(dr);
            }
        }

        private void DataPortal_Fetch(Criteria criteria)
        {
            if(_coeDAL == null) { LoadDAL(); }
            // Coverity Fix CID - 11543 
            if (_coeDAL != null)
            {
                using (SafeDataReader dr = _coeDAL.Get(criteria._id))
                {
                    if (!FetchObject(dr))
                        throw new Exception(string.Format("Unable to load object {0}", criteria._id));
                }
            }
            else
                throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));

            if(criteria._includeAccessRights == true)
            {
                this.COEAccessRights = COEAccessRightsBO.Get(COEAccessRightsBO.ObjectTypes.COEGENERICOBJECT, this._id);
            }
        }

        private void DataPortal_Fetch(UserCriteria criteria)
        {
            if (_coeDAL == null) { LoadDAL(); }
            if (_coeDAL != null)
            {
                using (SafeDataReader dr = _coeDAL.GetAllCompleteData(criteria._user,criteria._formGroup))
                {
                    FetchObject(dr);
                }
            }
            else
                throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));
            if (criteria._includeAccessRights == true)
            {
                this.COEAccessRights = COEAccessRightsBO.Get(COEAccessRightsBO.ObjectTypes.COEGENERICOBJECT, this._id);
            }
        }

        private bool FetchObject(SafeDataReader dr)
        {
            if(dr.Read())
            {
                _id = Convert.ToInt32(dr.GetInt64("ID"));
                _name = dr.GetString("NAME");
                _dateCreated = dr.GetSmartDate("DATE_CREATED", _dateCreated.EmptyIsMin);
                _description = dr.GetString("DESCRIPTION");
                _isPublic = dr.GetChar("IS_PUBLIC") == '1' ? true : false;
                _userName = dr.GetString("USER_ID");
                _formGroup = Convert.ToInt32(dr.GetInt64("FORMGROUP"));
                _coeGenericObject = (string) dr.GetString("COEGENERICOBJECT");
                _databaseName = dr.GetString("DATABASE"); //if we want to support different storage locations then this would be set by the input paramter
                this.MarkOld();
                this.MarkClean();

                return true;
            }

            return false;
        }

        #endregion //Data Access - Fetch

        #region Data Access - Insert

        //when already on the server, this can be called
        internal void Insert(DAL coeDAL)
        {
            if(!IsDirty)
                return;
            if(_coeDAL == null)
                LoadDAL();
            if(_coeAccessRights != null)
                _isPublic = false;

            _id = this.GenerateID();
            _id = coeDAL.Insert(_formGroup, _name, _isPublic, _description, _userName, _coeGenericObject, _databaseName, _id, _associatedDataviewID);

            //call internal methods of COEAccessRightsBO to update accessRights
            if(_coeAccessRights != null)
            {
                _coeAccessRights.ObjectID = _id;
                _coeAccessRights.ObjectType = COEAccessRightsBO.ObjectTypes.COEGENERICOBJECT;
                _coeAccessRights.Save();
            }
            this.MarkOld();
            this.MarkClean();
        }

        protected override void DataPortal_Insert()
        {
            if(_coeDAL == null) { LoadDAL(); }
            // Coverity Fix CID - 11544
            if (_coeDAL != null)
            {
                if (_coeAccessRights != null)
                    _isPublic = false;

                _id = this.GenerateID();

                _coeDAL.Insert(_formGroup, _name, _isPublic, _description, _userName, _coeGenericObject, _databaseName, _id, _associatedDataviewID);
                if (_coeAccessRights != null)
                {
                    _coeAccessRights.ObjectID = _id;
                    _coeAccessRights.ObjectType = COEAccessRightsBO.ObjectTypes.COEGENERICOBJECT;
                    _coeAccessRights.Save();
                }
                MarkOld();
                MarkClean();
            }
            else
                throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));
        }

        private int GenerateID()
        {
            if(_id > 0)
                return _id;
            else
            {
                if(_coeDAL == null)
                    LoadDAL();
                // Coverity Fix CID - 11541
                if (_coeDAL != null)            
                    return _coeDAL.GetNewID();
                else
                    throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));
            }
        }

        #endregion

        #region Data Access - Update
        //called by other services
        internal void Update(DAL coeDAL)
        {
            if(!IsDirty)
                return;
            coeDAL.Update(_id, _coeGenericObject, _name, _description, _isPublic, _databaseName);

        }

        protected override void DataPortal_Update()
        {
            if(_coeDAL == null) { LoadDAL(); }

             // Coverity Fix CID - 11545 
            if (_coeDAL != null)
            {
                if (base.IsDirty)
                {
                    _coeDAL.Update(_id, _coeGenericObject, _name, _description, _isPublic, _databaseName);
                }
                MarkOld();
                MarkClean();
            }
            else
                throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));
        }
        #endregion //Data Access - Update

        #region Data Access - Delete
        //called by other services
        internal void DeleteSelf(DAL _coeDAL)
        {
            if(_coeDAL == null) { LoadDAL(); }
            // Coverity Fix CID - 11546 
	        if (_coeDAL != null)            
                _coeDAL.Delete(_id);
            else
                throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));
        }

        protected override void DataPortal_DeleteSelf()
        {
            DataPortal_Delete(new Criteria(_id));
        }

        private void DataPortal_Delete(Criteria criteria)
        {
            if(_coeDAL == null) { LoadDAL(); }
            // Coverity Fix CID - 11542
            if (_coeDAL != null)
                _coeDAL.Delete(criteria._id);
            else
                throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));
        }
        #endregion //Data Access - Delete

        private void LoadDAL()
        {
            if(_dalFactory == null) { _dalFactory = new DALFactory(); }
            _dalFactory.GetDAL<DAL>(ref _coeDAL, _serviceName, COEDatabaseName.Get(), true);
        }

        #endregion //Data Access

    }
}

