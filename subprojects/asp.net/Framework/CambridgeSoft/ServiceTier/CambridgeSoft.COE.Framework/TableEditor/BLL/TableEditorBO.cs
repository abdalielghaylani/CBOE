using System;
using System.Data;
using System.Data.SqlClient;
using Csla;
using Csla.Data;
using Csla.Validation;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.Properties;
using System.Collections.Generic;
using CambridgeSoft.COE.Framework.COEConfigurationService;
using CambridgeSoft.COE.Framework.COELoggingService;


namespace CambridgeSoft.COE.Framework.COETableEditorService
{
    /// <summary>
    /// This object represents a record of a COETableEditor editable table.
    /// </summary>
    [Serializable()]
    public class COETableEditorBO : Csla.BusinessBase<COETableEditorBO>
    {
        #region Member variables
        //declare members (Fields inside Database Tables)
        private int _id = 0;
        private string _applicationName = string.Empty;
        private List<Column> _cList;
        private string _sequenceName;

        //variables data access
        private DAL _coeDAL = null;
        private DALFactory _dalFactory = new DALFactory();
        private string _serviceName = "COETableEditor";
        //this is not always the same as _applicationName so I creaed another vairalbe
        private string _appName = string.Empty;
        [NonSerialized]
        static COELog _coeLog = COELog.GetSingleton("COETableEditor");


        #endregion

        #region Member Methods

        /// <summary>
        /// Load DAL
        /// </summary>
        private void LoadDAL()
        {
            if (_dalFactory == null) { _dalFactory = new DALFactory(); }
            string _databaseName = ConfigurationUtilities.GetDatabaseNameFromAppName(COEAppName.Get().ToString());
            _dalFactory.GetDAL<DAL>(ref _coeDAL, _serviceName, _databaseName, true);
        }

        /// <summary>
        /// Delete DAL
        /// </summary>
        /// <param name="_coeDAL"></param>
        internal void DeleteSelf(DAL _coeDAL)
        {
            if (_coeDAL == null) { LoadDAL(); }
             // Coverity Fix CID - 11671
            if (_coeDAL != null)            
                _coeDAL.Delete(_id);
            else
                throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));
        }

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
        public string AppName
        {
            get
            {
                CanReadProperty("AppName", true);
                return _applicationName;
            }
            set
            {
                _applicationName = value;
            }
        }
        public List<Column> Columns
        {
            get
            {
                CanReadProperty("Columns", true);
                return _cList;
            }
            set
            {
                if (value == null) value = null;
                _cList = value;
                PropertyHasChanged("Columns");
            }
        }

        public string sequenceName
        {
            get
            {
                CanReadProperty("sequenceName", true);
                return _sequenceName;
            }
            set
            {
                _sequenceName = value;
                PropertyHasChanged("sequenceName");
            }
        }

        private List<int> _childTableData;
        public List<int> ChildTableData
        {
            get
            {
                CanReadProperty("ChildTableData", true);
                return _childTableData;
            }
            set
            {
                _childTableData = value;
                PropertyHasChanged("ChildTableData");
            }
        }

        protected override object GetIdValue()
        {
            return _id;
        }

        #endregion

        #region Constructors

        private COETableEditorBO()
        { 
            /* require use of factory method */ 
            _cList = null;
            _sequenceName = string.Empty;
        }

        /// <summary>
        /// constructor to be called from queryCriteriaList as well as any other services that needs to construct this object
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cList"></param>
        /// <param name="applicationName"></param>
        internal COETableEditorBO(int id, List<Column> cList, string applicationName, string sequenceName)
        {
            _id = id;
            _cList = cList;
            _applicationName = applicationName;
            _sequenceName = sequenceName;
        }

        internal COETableEditorBO(List<Column> cList)
        {
            _cList = cList;
        }

        internal COETableEditorBO(List<Column> cList, string sequenceName)
        {
            _cList = cList;
            _sequenceName = sequenceName;
        }

        #endregion

        #region Factory Methods
        /// <summary>
        /// this method must be called prior to any other method inorder to set the database that the dal will use
        /// </summary>
        internal static void SetDatabaseName()
        {
            COEDatabaseName.Set(Resources.CentralizedStorageDB);
        }

        public static COETableEditorBO New()
        {
            SetDatabaseName();
            if (!CanAddObject())
                throw new System.Security.SecurityException(Resources.UserNotAuthorizedForAddObject + " COETableEditorBO");

            return DataPortal.Create<COETableEditorBO>(new CreateNewCriteria());
        }

        /// <summary>
        /// Get one record.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static COETableEditorBO Get(int id)
        {
            SetDatabaseName();
            if (!CanGetObject())
                throw new System.Security.SecurityException(Resources.UserNotAuthorizedForViewObject + " COETableEditorBO");
            return DataPortal.Fetch<COETableEditorBO>(new Criteria(id));
        }

        /// <summary>
        /// Get record based on table name and column values.
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="cList"></param>
        /// <returns></returns>
        public static COETableEditorBO Get(string tableName, List<Column> cList)
        {
            SetDatabaseName();
            if (!CanGetObject())
                throw new System.Security.SecurityException(Resources.UserNotAuthorizedForViewObject + " COETableEditorBO");
            try
            {
                return DataPortal.Fetch<COETableEditorBO>(new SqlCriteria(tableName, cList));
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        /// <summary>
        /// Delete a record.
        /// </summary>
        /// <param name="id"></param>
        public static void Delete(int id)
        {
            SetDatabaseName();
            if (!CanDeleteObject())
                throw new System.Security.SecurityException(Resources.UserNotAuthorizedForDeleteObject + " COETableEditorBO");
            DataPortal.Delete(new Criteria(id));
        }

        /// <summary>
        /// save data of COETableEditorBO
        /// </summary>
        /// <returns></returns>
        public override COETableEditorBO Save()
        {
            SetDatabaseName();
            if (IsDeleted && !CanDeleteObject())
                throw new System.Security.SecurityException(Resources.UserNotAuthorizedForDeleteObject + " COEDataViewBO");
            else if (IsNew && !CanAddObject())
                throw new System.Security.SecurityException(Resources.UserNotAuthorizedForAddObject + " COETableEditorBO");
            else if (!CanEditObject())
                throw new System.Security.SecurityException(Resources.UserNotAuthorizedForEditObject + " COETableEditorBO");
           
            /*CSBR-153687
            Modified By DIVYA
            Checking for duplicate name for picklist domain under manage customizable table.
            */
            try
            {
                return base.Save();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            //End of CSBR-153687
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
            //constructors
            public Criteria(int id)
            {
                _id = id;
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
        private class SqlCriteria
        {
            internal string _tableName;
            internal List<Column> _cList;
            //constructors
            public SqlCriteria(string tableName, List<Column> cList)
            {
                _tableName = tableName;
                _cList = cList;
            }
        }

        #endregion //Criteria

        #region Data Access - Create
        /// <summary>
        /// get blank column list 
        /// </summary>
        /// <param name="criteria"></param>
        [RunLocal]
        private void DataPortal_Create(CreateNewCriteria criteria)
        {
            if (_coeDAL == null) { LoadDAL(); }
            // Coverity Fix CID - 11665
            if (_coeDAL != null)
            {
                _cList = COETableEditorUtilities.getColumnList(_coeDAL._COETableEditorTableName);
                _sequenceName = COETableEditorUtilities.getSequenceName(_coeDAL._COETableEditorTableName);
            }
            else
                throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));
        }

        #endregion //Data Access - Create

        #region Data Access - Fetch

        /// <summary>
        /// when already on the server, this can be called
        /// </summary>
        /// <param name="dataViewDAL"></param>
        /// <param name="dataViewID"></param>
        internal void Get(DAL dataViewDAL, int dataViewID)
        {
            if (!IsDirty) return;
            using (SafeDataReader dr = _coeDAL.Get(dataViewID))
            {
                FetchObject(dr);
            }
        }

        /// <summary>
        /// fetch a record from database by id
        /// </summary>
        /// <param name="criteria"></param>
        private void DataPortal_Fetch(Criteria criteria)
        {
            if (_coeDAL == null) { LoadDAL(); }
            // Coverity Fix CID - 11667
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

        /// <summary>
        /// perform fetch from database by sql statement
        /// </summary>
        /// <param name="criteria"></param>
        private void DataPortal_Fetch(SqlCriteria criteria)
        {
            try
            {
                if (_coeDAL == null) { LoadDAL(); }
                // Coverity Fix CID - 11668
	            if (_coeDAL != null)            
                    _coeDAL.IsValidPicklistDomain(criteria._tableName, criteria._cList);
                else
                    throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }

        /// <summary>
        /// format the safedatareader to columnlist
        /// </summary>
        /// <param name="dr"></param>
        private void FetchObject(SafeDataReader dr)
        {
            if (dr.Read())
            {
                _cList = COETableEditorUtilities.getColumnList(_coeDAL._COETableEditorTableName);
                _id = dr.GetInt32(COETableEditorUtilities.getIdFieldName(_coeDAL._COETableEditorTableName));
                for (int i = 0; i < _cList.Count; i++)
                {
                    switch (_cList[i].FieldType)
                    {
                        case DbType.DateTime:
                            _cList[i].FieldValue = dr.GetSmartDate(_cList[i].FieldName);
                            break;
                        case DbType.Boolean:
                            _cList[i].FieldValue = dr.GetBoolean(_cList[i].FieldName);
                            break;
                        case DbType.Int16:
                            //modified by Jerry on 2008/07/24 for column datavalue is null
                            _cList[i].FieldValue = dr.GetValue(_cList[i].FieldName);
                            break;
                        case DbType.Double:
                            //modified by Jerry on 2008/09/22 for column datavalue is null
                            _cList[i].FieldValue = dr.GetValue(_cList[i].FieldName);
                            break;
                        default:
                            _cList[i].FieldValue = dr.GetString(_cList[i].FieldName);
                            break;
                    }
                }
            }
            _applicationName = _appName;
        }

        #endregion //Data Access - Fetch

        #region Data Access - Insert

        /// <summary>
        /// when already on the server, this can be called
        /// </summary>
        /// <param name="coeTableEditorDAL"></param>
        internal void Insert(DAL coeTableEditorDAL)
        {
            if (!IsDirty) return;
            coeTableEditorDAL.Insert(_cList, _sequenceName);
            MarkOld();
        }

        /// <summary>
        /// insert a record
        /// </summary>
        protected override void DataPortal_Insert()
        {
            /*CSBR-153687
            Modified By DIVYA
            Checking for duplicate name for picklist domain under manage customizable table.
            */
            try
            {                
                if (_coeDAL == null) { LoadDAL(); }
                // Coverity Fix CID - 11669
	            if (_coeDAL != null)            
                {
                    if (COETableEditorUtilities.GetIsHasChildTable(_coeDAL._COETableEditorTableName))
                    {
                        _id = _coeDAL.Insert(_cList, _childTableData, _sequenceName);
                    }
                    else
                    {
                        _id = _coeDAL.Insert(_cList, _sequenceName);//Added ID:Sumeet
                    }
                }
                else
                    throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));
            }
            catch (Exception ex)
            {
                throw ex;
            }
            //End of CSBR-153687
        }
        #endregion

        #region Data Access - Update

        /// <summary>
        /// update a record
        /// </summary>
        /// <param name="coeDataViewDAL"></param>
        internal void Update(DAL coeDataViewDAL)
        {
            if (!IsDirty) return;
            coeDataViewDAL.Update(_id, _cList);
            MarkOld();
        }
       
        /// <summary>
        /// 2008-4-7 modify by david zhang
        /// update a record
        /// </summary>
        protected override void DataPortal_Update()
        {
            if (_coeDAL == null) { LoadDAL(); }
            // Coverity Fix CID - 11670
            if (_coeDAL != null)
            {
                if (base.IsDirty)
                {
                    if (COETableEditorUtilities.GetIsHasChildTable(_coeDAL._COETableEditorTableName))
                    {
                        _coeDAL.Update(_id, _cList, _childTableData);
                    }
                    else
                    {
                        _coeDAL.Update(_id, _cList);
                    }
                }
            }
            else
                throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));
        }
        #endregion //Data Access - Update

        #region Data Access - Delete
       
        /// <summary>
        /// 2008-4-7 modify by david zhang
        /// delete a record itself
        /// </summary>
        protected override void DataPortal_DeleteSelf()
        {
            DataPortal_Delete(new Criteria(_id));
        }
 
        /// <summary>
        /// 2008-4-7 modify by david zhang
        /// delete a record
        /// </summary>
        /// <param name="criteria"></param>
        private void DataPortal_Delete(Criteria criteria)
        {
            if (_coeDAL == null) { LoadDAL(); }
            // Coverity Fix CID - 11666
            if (_coeDAL != null)
                _coeDAL.Delete(criteria._id);
            else
                throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));
        }
        /// <summary>
        /// Method to check the vaue is used for any records
        /// </summary>
        /// <param name="isUsedCheckMethod"></param>
        /// <param name="fieldvalue"></param>
        /// <returns></returns>
        public bool IsUsedCheck(string isUsedCheckMethod, string fieldvalue)
        {
            if (_coeDAL == null) { LoadDAL(); }
            // Coverity Fix CID - 11664
            if (_coeDAL != null)
                return _coeDAL.IsUsedCheck(isUsedCheckMethod, fieldvalue);
            else
                throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));            
        }
        /// <summary>
        /// Method to check the value of the field is unique
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="fieldColumn"></param>
        /// <param name="fieldvalue"></param>
        /// <param name="keyField"></param>
        /// <param name="keyFieldValue"></param>
        /// <returns></returns>
        public bool IsUniqueCheck(string tableName, string fieldColumn,string fieldvalue,string keyField,string keyFieldValue)
        {
            if (_coeDAL == null) { LoadDAL(); }
            // Coverity Fix CID - 11663
            if (_coeDAL != null)
                return _coeDAL.IsUnique(tableName, fieldColumn, fieldvalue, keyField, keyFieldValue);
            else
                throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));            
        }
        #endregion 
        #endregion 
    }
}
