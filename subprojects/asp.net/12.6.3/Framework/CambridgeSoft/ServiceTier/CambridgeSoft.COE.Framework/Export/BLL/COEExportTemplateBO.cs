using System;
using System.Collections.Generic;
using System.Text;
using Csla;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.COELoggingService;
using CambridgeSoft.COE.Framework.COEConfigurationService;
using Csla.Data;
using CambridgeSoft.COE.Framework.Export;
using Csla.Validation;
using CambridgeSoft.COE.Framework.Properties;

namespace CambridgeSoft.COE.Framework.COEExportService
{
    [Serializable()]
    public class COEExportTemplateBO : Csla.BusinessBase<COEExportTemplateBO>
    {
        #region Member variables

        // declare members
        private int _id = 0;
        private string _name = string.Empty;
        private string _description = string.Empty;
        private ResultsCriteria _coeResultCriteria;
        private SmartDate _dateCreated = new SmartDate(true);
        private bool _isPublic = false;
        private string _userName = string.Empty;
        private int _dataViewId = 0;
        private int _webFormId = 0;
        private int _winFormId = 0;
        private string _userId = string.Empty;
        private const string CLASSNAME = "COEExportTemplateBO";
        private string _databaseName = string.Empty;
        private string _serviceName = "COEExport";

        //variables data access
        [NonSerialized]
        private DAL _coeDAL = null;
        [NonSerialized]
        private DALFactory _dalFactory = new DALFactory();

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
            set
            {
                CanWriteProperty(true);
                if (value == null)
                    value = 0;
                if (_id != value)
                {
                    _id = value;
                    PropertyHasChanged();
                }
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

        public ResultsCriteria ResultCriteria
        {
            get
            {
                CanReadProperty(true);
                return _coeResultCriteria;
            }
            set
            {
                CanWriteProperty(true);
                if (_coeResultCriteria == null || !_coeResultCriteria.Equals(value))
                {
                    _coeResultCriteria = value;
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

        public DateTime DateCreated
        {
            get
            {
                CanReadProperty(true);
                return _dateCreated.Date;
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

        public int WebFormId
        {
            get
            {
                CanReadProperty(true);
                return _webFormId;
            }
            set
            {
                CanWriteProperty(true);
                if (_webFormId != value)
                {
                    _webFormId = value;
                    PropertyHasChanged();
                }
            }
        }



        public int WinFormId
        {
            get
            {
                CanReadProperty(true);
                return _winFormId;
            }
            set
            {
                CanWriteProperty(true);
                if (_winFormId != value)
                {
                    _winFormId = value;
                    PropertyHasChanged();
                }
            }
        }

        protected override object GetIdValue()
        {
            return _id;
        }

        #endregion

        #region Constructors

        public COEExportTemplateBO()
        {
            /* require use of factory method */
        }

        //constructor to be called from any other services that needs to construct this object
        internal COEExportTemplateBO(int id, string name, string description, ResultsCriteria coeResultCriteria, bool isPublic, int dataViewId, int webFormId, int winFormId, SmartDate dateCreated, string userID)
        {
            _id = id;
            _name = name;
            _description = description;
            _coeResultCriteria = coeResultCriteria;
            _isPublic = isPublic;
            _dateCreated = dateCreated;
            _dataViewId = dataViewId;
            _webFormId = webFormId;
            _winFormId = winFormId;
            _userName = userID;
        }

        #endregion

        #region Validation Rules

        private void AddCommonRules()
        {
            //
            // QueryName
            //
            ValidationRules.AddRule(CommonRules.StringRequired, "Name");
            ValidationRules.AddRule(CommonRules.StringMaxLength, new CommonRules.MaxLengthRuleArgs("Name", 100));
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

        public static COEExportTemplateBO New(string name, string description, ResultsCriteria resultsCriteria, bool isPublic, SmartDate dateCreated, int dataViewID, int webFormID, int winFormID, string userID)
        {
            SetDatabaseName();

            if (!CanAddObject())
                throw new System.Security.SecurityException(Resources.UserNotAuthorizedForAddObject + " " + CLASSNAME);

            return DataPortal.Create<COEExportTemplateBO>(new CreateNewCriteria(name, description, resultsCriteria, isPublic, dateCreated, dataViewID, webFormID, winFormID, userID));

        }

        public override COEExportTemplateBO Save()
        {
            SetDatabaseName();
            if (IsDeleted && !CanDeleteObject())
                throw new System.Security.SecurityException(Resources.UserNotAuthorizedForDeleteObject + " COEExportTemplateBO");
            else if (IsNew && !CanAddObject())
                throw new System.Security.SecurityException(Resources.UserNotAuthorizedForAddObject + " COEExportTemplateBO");
            else if (!CanEditObject())
                throw new System.Security.SecurityException(Resources.UserNotAuthorizedForEditObject + " COEExportTemplateBO");

            return base.Save();
        }

        public static COEExportTemplateBO Get(int id)
        {
            SetDatabaseName();


            if (!CanGetObject())
                throw new System.Security.SecurityException(Resources.UserNotAuthorizedForViewObject + " COEExportTemplateBO");
            return DataPortal.Fetch<COEExportTemplateBO>(new Criteria(id));
        }

        public static void Delete(string name)
        {
            SetDatabaseName();

            if (!CanDeleteObject())
                throw new System.Security.SecurityException(Resources.UserNotAuthorizedForDeleteObject + " COEExportTemplateBO");
            DataPortal.Delete(new NameCriteria(name));
        }

        public static void Delete(int id)
        {
            SetDatabaseName();

            if (!CanDeleteObject())
                throw new System.Security.SecurityException(Resources.UserNotAuthorizedForDeleteObject + " COEExportTemplateBO");
            DataPortal.Delete(new Criteria(id));
        }

        public COEExportTemplateBO Update()
        {
            if (!CanEditObject())
                throw new System.Security.SecurityException(Resources.UserNotAuthorizedForEditObject + " COEExportTemplateBO");


            return base.Save();
        }

        public static bool CanAddObject()
        {
            // 
            return true;
        }

        public static bool CanGetObject()
        {
            // 
            return true;
        }

        public static bool CanEditObject()
        {
            // 
            return true;
        }

        public static bool CanDeleteObject()
        {
            // 
            return true;
        }

        #endregion

        #region Data Access

        protected override void DataPortal_OnDataPortalException(DataPortalEventArgs e, Exception ex)
        {
            throw ex;
        }

        #region Data Access - Create

        [RunLocal]
        private void DataPortal_Create(CreateNewCriteria criteria)
        {
            //COEReportTemplateBO(0, name, description, userId, isPublic, dateCreated, coeReportTemplate, databaseName, application, dataViewId, reportTypeId)
            _name = criteria._name;
            _description = criteria._description;
            _coeResultCriteria = criteria._resultsCriteria;
            _isPublic = criteria._isPublic;
            _dateCreated = criteria._dateCreated;
            _dataViewId = criteria._dataViewID;
            _webFormId = criteria._webFormID;
            _winFormId = criteria._winFormID;
            _userId = criteria._userID;
        }

        #endregion

        #region Data Access - Fetch

        private void DataPortal_Fetch(Criteria criteria)
        {
            if (_coeDAL == null) { LoadDAL(); }
            // Coverity Fix CID - 11515 
            if (_coeDAL != null)
            {
                using (SafeDataReader dr = _coeDAL.GetTemplate(criteria._id))
                {
                    FetchObject(dr);
                }
            }
            else            
                throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));            
        }

        private void FetchObject(SafeDataReader dr)
        {
            if (dr.Read())
            {
                _id = dr.GetInt32("ID");
                _name = dr.GetString("NAME");
                _description = dr.GetString("DESCRIPTION");
                _coeResultCriteria = (ResultsCriteria)COEExportTemplateUtilities.DeserializeCOEResultCriteria(dr.GetString("COERESULTCRITERIA"));
                _dateCreated = dr.GetSmartDate("DATE_CREATED", _dateCreated.EmptyIsMin);
                _isPublic = (dr.GetString("IS_PUBLIC").Equals("1") ? true : false);
                _dataViewId = dr.GetInt32("DATAVIEW_ID");
                _webFormId = dr.GetInt32("WEBFORM_ID");
                _winFormId = dr.GetInt32("WINFORM_ID");
                _userName = dr.GetString("USER_ID");

            }
        }

        #endregion

        #region Data Access - Insert

        //called by other services
        internal void Insert(DAL coeExportTemplateDAL)
        {
            if (!IsDirty)
                return;
            if (_coeDAL == null)
                LoadDAL();

            if (this.CheckNameIsUnique())
                _id = coeExportTemplateDAL.Insert(_name, _description, _userName, _isPublic, _dataViewId, _webFormId, _winFormId, _coeResultCriteria);
            else
                throw new Exception(Resources.NameAlreadyExists);
        }

        protected override void DataPortal_Insert()
        {
            if (_coeDAL == null) { LoadDAL(); }
            // Coverity Fix CID - 11516
            if (_coeDAL != null)
            {
                if (this.CheckNameIsUnique())
                    _id = _coeDAL.Insert(_name, _description, _userName, _isPublic, _dataViewId, _webFormId, _winFormId, _coeResultCriteria);
                else
                    throw new Exception(Resources.NameAlreadyExists);
            }
            else
                throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));
        }

        #endregion

        #region Data Access - Update

        //called by other services
        internal void Update(DAL coeExportTemplateDAL)
        {
            try
            {
                if (!IsDirty) return;

                if (this.CheckNameIsUniqueForUpdate())
                    coeExportTemplateDAL.Update(_id, _name, _description, _userName, _isPublic, _dataViewId, _webFormId, _winFormId, _coeResultCriteria);
                else
                    throw new Exception(Resources.NameAlreadyExists);
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
                // Coverity Fix CID - 11517
                if (_coeDAL != null)
                {
                    if (base.IsDirty)
                    {
                        if (this.CheckNameIsUniqueForUpdate())
                            _coeDAL.Update(_id, _name, _description, _userName, _isPublic, _dataViewId, _webFormId, _winFormId, _coeResultCriteria);
                        else
                            throw new Exception(Resources.NameAlreadyExists);
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

        #endregion

        #region Data Access - Delete

        //called by other services
        internal void DeleteSelf(DAL coeExportTemplateDAL)
        {
            if (coeExportTemplateDAL == null) { LoadDAL(); }
            // Coverity Fix CID - 11518 
            if (coeExportTemplateDAL != null) //Coverity Fix CID 11518
                coeExportTemplateDAL.Delete(_name);
            else
                throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));
        }

        protected override void DataPortal_DeleteSelf()
        {
            DataPortal_Delete(new Criteria(_name));
        }

        private void DataPortal_Delete(Criteria criteria)
        {
            if (_coeDAL == null) { LoadDAL(); }
             // Coverity Fix CID - 11513
            if (_coeDAL != null)            
                _coeDAL.Delete(criteria._id);            
            else            
                throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));            
        }

        private void DataPortal_Delete(NameCriteria criteria)
        {
            if (_coeDAL == null) { LoadDAL(); }
            // Coverity Fix CID - 11514
            if (_coeDAL != null)            
                _coeDAL.Delete(criteria._name);            
            else            
                throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));            
        }

        #endregion

        #region Criteria

        [Serializable()]
        private class Criteria
        {

            internal int _id;
            internal string _name;

            //constructors
            public Criteria(int id)
            {
                _id = id;
            }
            public Criteria(string name)
            {
                _name = name;
            } // constructors

            public int ExportTemplateId
            {
                get
                {
                    return _id;
                }
            }
        }

        [Serializable()]
        private class NameCriteria
        {                       
            internal string _name;

            //constructors

            public NameCriteria(string name)
            {
                _name = name;
            } // constructors

            public string ExportTemplateName
            {
                get
                {
                    return _name;
                }
            }
        }

        #endregion //Criteria

        #region CreateNewCriteria

        [Serializable()]
        private class CreateNewCriteria
        {
            internal string _name;
            internal string _description;
            internal ResultsCriteria _resultsCriteria;
            internal bool _isPublic;
            internal SmartDate _dateCreated;
            internal int _dataViewID;
            internal int _webFormID;
            internal int _winFormID;
            internal string _userID;

            public CreateNewCriteria(string name, string description, ResultsCriteria resultsCriteria, bool isPublic, SmartDate dateCreated, int dataViewID, int webFormID, int winFormID, string userID)
            {
                _name = name;
                _description = description;
                _resultsCriteria = resultsCriteria;
                _isPublic = isPublic;
                _dateCreated = dateCreated;
                _dataViewID = dataViewID;
                _webFormID = webFormID;
                _winFormID = winFormID;
                _userID = userID;
            }
        }

        #endregion

        private void LoadDAL()
        {

            if (_dalFactory == null) { _dalFactory = new DALFactory(); }
            _dalFactory.GetDAL<DAL>(ref _coeDAL, _serviceName, COEDatabaseName.Get().ToString(), true);
        }

        public bool CheckNameIsUnique()
        {
            bool isUnique = true;
            int id = 0;

            if (_coeDAL == null) { LoadDAL(); }
            // Coverity Fix CID - 11511
            if (_coeDAL != null)
            {
                id = _coeDAL.GetId(_name);
                if (id != 0)
                {
                    isUnique = false;
                }
            }
            else            
                throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));            

            return isUnique;
        }

        public bool CheckNameIsUniqueForUpdate()
        {
            bool isUnique = true;
            int id = 0;

            if (_coeDAL == null) { LoadDAL(); }
              // Coverity Fix CID - 11512
            if (_coeDAL != null)
            {
                id = _coeDAL.GetId(_name);

                if (id != 0)
                {
                    if (id != _id)
                    {
                        isUnique = false;
                    }
                }
            }
            else           
                throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));           

            return isUnique;
        }

        #endregion

    }
}
