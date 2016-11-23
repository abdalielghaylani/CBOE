using System;
using System.Data;
using Csla;
using Csla.Data;
using Csla.Validation;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.Properties;
using CambridgeSoft.COE.Framework.COELoggingService;
using CambridgeSoft.COE.Framework.Common.Messaging;

namespace CambridgeSoft.COE.Framework.COESearchCriteriaService
{
    [Serializable()]
    public class COESearchCriteriaBOList : Csla.BusinessListBase<COESearchCriteriaBOList, COESearchCriteriaBO>
    {
        //variables data access
        [NonSerialized]
        private DAL _coeDAL = null;
        [NonSerialized]
        private DALFactory _dalFactory = new DALFactory();
        private string _serviceName = "COESearchCriteria";
        private string _databaseName;
        [NonSerialized]
        static COELog _coeLog = COELog.GetSingleton("COESearchCriteria");

        #region Factory Methods

        //this method must be called prior to any other method inorder to set the database that the dal will use
        internal static void SetDatabaseName()
        {
            COEDatabaseName.Set(DALUtils.GetDefaultQualifyDbName(Resources.CentralizedStorageDB));
        }

        internal static void SetDatabaseName(string databaseName)
        {
            COEDatabaseName.Set(databaseName);
        }

        private COESearchCriteriaBOList()
        { /* require use of factory method */ }

        public static COESearchCriteriaBOList NewList(string databaseName)
        {
            SetDatabaseName();

            try
            {
                if (!CanAddObject())
                    throw new System.Security.SecurityException(Resources.UserNotAuthorizedForAddObject + " COESearchCriteriaBOList");
                return new COESearchCriteriaBOList();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public static COESearchCriteriaBOList GetRecentSearchCriteria()
        {
            SetDatabaseName();

            try
            {
                if (!CanGetObject())
                    throw new System.Security.SecurityException(Resources.UserNotAuthorizedForViewObject + " COESearchCriteriaBOList");
                return DataPortal.Fetch<COESearchCriteriaBOList>(new Criteria(SearchCriteriaType.TEMP));

            }
            catch (Exception)
            {

                throw;
            }
        }

        public static COESearchCriteriaBOList GetSavedSearchCriteria(string userId, int dataviewId, string databaseName)
        {
            SetDatabaseName();
            try
            {
                if (!CanGetObject())
                    throw new System.Security.SecurityException(Resources.UserNotAuthorizedForViewObject + " COESearchCriteriaBOList");
                return DataPortal.Fetch<COESearchCriteriaBOList>(new Criteria(userId, dataviewId, SearchCriteriaType.SAVED, databaseName));
            }
            catch
            {
                throw;
            }
        }

        public static COESearchCriteriaBOList GetSavedSearchCriteria(string userId, int dataviewId, int formGroup, string databaseName)
        {
            SetDatabaseName();
            try
            {
                if (!CanGetObject())
                    throw new System.Security.SecurityException(Resources.UserNotAuthorizedForViewObject + " COESearchCriteriaBOList");
                return DataPortal.Fetch<COESearchCriteriaBOList>(new Criteria(userId, dataviewId, formGroup, SearchCriteriaType.SAVED, databaseName));
            }
            catch
            {
                throw;
            }
        }

        public static COESearchCriteriaBOList GetRecentSearchCriteria(string userId, int dataviewId, string databaseName, int quantityToRetrieve)
        {
            SetDatabaseName();
            try
            {
                if (!CanGetObject())
                    throw new System.Security.SecurityException(Resources.UserNotAuthorizedForViewObject + " COESearchCriteriaBOList");
                return DataPortal.Fetch<COESearchCriteriaBOList>(new Criteria(userId, dataviewId, SearchCriteriaType.TEMP, databaseName, quantityToRetrieve));
            }
            catch
            {
                throw;
            }

        }

        public static COESearchCriteriaBOList GetRecentSearchCriteria(string userId, int dataviewId, int formGroup, string databaseName, int quantityToRetrieve)
        {
            SetDatabaseName();
            try
            {
                if (!CanGetObject())
                    throw new System.Security.SecurityException(Resources.UserNotAuthorizedForViewObject + " COESearchCriteriaBOList");
                return DataPortal.Fetch<COESearchCriteriaBOList>(new Criteria(userId, dataviewId, formGroup, SearchCriteriaType.TEMP, databaseName, quantityToRetrieve));
            }
            catch
            {
                throw;
            }
        }

        public static bool CanAddObject()
        {
            //TO DO:  need to add authorization access code
            return true;
        }

        public static bool CanGetObject()
        {
            //TO DO:  need to add authorization access code
            return true;
        }

        public static bool CanEditObject()
        {
            //TO DO:  need to add authorization access code
            return true;
        }

        public static bool CanDeleteObject()
        {
            //TO DO:  need to add authorization access code
            return true;
        }

        #endregion //Factory Methods

        #region Data Access

        #region Filter Criteria
        [Serializable()]
        private class Criteria
        {
            #region Members
            private string _userId;
            private int _dataviewId;
            private SearchCriteriaType _searchCriteriaType;
            private int _quantityToRetrieve;
            private string _databaseName;
            private int _formGroup;
            private bool _byFromGroup = false;
            #endregion

            #region Constructors
            public Criteria(SearchCriteriaType searchCriteriaType)
            {
                this._searchCriteriaType = searchCriteriaType;
            }
            public Criteria(string userId, int dataviewId, SearchCriteriaType searchCriteriaType, string databaseName)
            {
                if (userId == null)
                {
                    throw new Exception("UserId can´t be null");
                }
                else
                {
                    this._userId = userId;
                    this._dataviewId = dataviewId;
                    this._searchCriteriaType = searchCriteriaType;
                    this._databaseName = databaseName;
                }
            }
            public Criteria(string userId, int dataviewId, int formGroup, SearchCriteriaType searchCriteriaType, string databaseName)
            {
                if (userId == null)
                {
                    throw new Exception("UserId can´t be null");
                }
                else
                {
                    this._userId = userId;
                    this._dataviewId = dataviewId;
                    this._searchCriteriaType = searchCriteriaType;
                    this._databaseName = databaseName;
                    this._formGroup = formGroup;
                    this._byFromGroup = true;
                }
            }
            public Criteria(string userId, int dataviewId, SearchCriteriaType searchCriteriaType, string databaseName, int quantityToRetrieve)
            {
                if (userId == null)
                {
                    throw new Exception("UserId can´t be null");
                }
                else
                {
                    this._userId = userId;
                    this._dataviewId = dataviewId;
                    this._searchCriteriaType = searchCriteriaType;
                    this._databaseName = databaseName;
                    this._quantityToRetrieve = quantityToRetrieve;
                }
            }
            public Criteria(string userId, int dataviewId, int formGroup, SearchCriteriaType searchCriteriaType, string databaseName, int quantityToRetrieve)
            {
                if (userId == null)
                {
                    throw new Exception("UserId can´t be null");
                }
                else
                {
                    this._userId = userId;
                    this._dataviewId = dataviewId;
                    this._searchCriteriaType = searchCriteriaType;
                    this._databaseName = databaseName;
                    this._quantityToRetrieve = quantityToRetrieve;
                    this._formGroup = formGroup;
                    this._byFromGroup = true;
                }
            }
            #endregion

            #region Properties

            public string UserId
            {
                get
                {
                    return _userId;
                }
            }
            public int DataViewId
            {
                get
                {
                    return _dataviewId;
                }
            }
            public SearchCriteriaType SearchCriteriaType
            {
                get
                {
                    return _searchCriteriaType;
                }
            }

            public int QuantityToRetrieve
            {
                get
                {
                    return _quantityToRetrieve;
                }
            }
            public string DataBaseName
            {
                get
                {
                    if (this._databaseName != null)
                    {
                        return this._databaseName;
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
            }
            public int FormGroup
            {
                get
                {
                    return _formGroup;
                }
            }
            public bool ByFormGroup
            {
                get
                {
                    return _byFromGroup;
                }
            }
            #endregion

        }

        #endregion //Filter Criteria

        #region Data Access - Fetch
        private void DataPortal_Fetch(Criteria criteria)
        {
            try
            {
                RaiseListChangedEvents = false;
                SafeDataReader dr = null;
                if (_coeDAL == null) { LoadDAL(); }

                // Coverity Fix CID - 11591
                if (_coeDAL != null)
                {
                    if (!criteria.ByFormGroup)
                    {
                        if (criteria.UserId == null)
                        {
                            dr = _coeDAL.GetAll();
                        }
                        else
                        {
                            switch (criteria.SearchCriteriaType)
                            {
                                case SearchCriteriaType.SAVED:
                                    dr = _coeDAL.GetSavedSearchCriteria(criteria.UserId, criteria.DataViewId, criteria.DataBaseName);
                                    break;
                                case SearchCriteriaType.TEMP:
                                    dr = _coeDAL.GetTempSearchCriterias(criteria.UserId, criteria.DataViewId, criteria.DataBaseName, criteria.QuantityToRetrieve);
                                    break;
                                default:
                                    throw new Exception("The Search Criteria Type is not correct");
                                    break;
                            }
                        }
                    }
                    else
                    {

                        switch (criteria.SearchCriteriaType)
                        {
                            case SearchCriteriaType.SAVED:
                                dr = _coeDAL.GetSavedSearchCriteria(criteria.UserId, criteria.DataViewId, criteria.FormGroup, criteria.DataBaseName);
                                break;
                            case SearchCriteriaType.TEMP:
                                dr = _coeDAL.GetTempSearchCriterias(criteria.UserId, criteria.DataViewId, criteria.FormGroup, criteria.DataBaseName, criteria.QuantityToRetrieve);
                                break;
                            default:
                                throw new Exception("The Search Criteria Type is not correct");
                                break;
                        }
                    }
                    Fetch(dr, criteria.SearchCriteriaType);
                    RaiseListChangedEvents = true;
                }
                else
                    throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));
            }
            catch (Exception)
            {

                throw;
            }
        }

        protected void Fetch(SafeDataReader dr, SearchCriteriaType searchCriteriaType)
        {
            try
            {
                while (dr.Read())
                {
                    COESearchCriteriaBO searchCriteriaData = new COESearchCriteriaBO(
                        dr.GetInt32("ID"),
                        dr.GetString("NAME"),
                        dr.GetString("DESCRIPTION"),
                        dr.GetString("USER_ID"),
                        (dr.GetString("IS_PUBLIC").Equals("1") ? true : false),
                        dr.GetInt32("FORMGROUP"),
                        dr.GetSmartDate("DATE_CREATED"),
                        (SearchCriteria)COESearchCriteriaUtilities.DeserializeCOESearchCriteria(dr.GetString("COESEARCHCRITERIA")),
                        dr.GetInt32("NUM_HITS"),
                        _databaseName = dr.GetString("DATABASE"), dr.GetInt32("DATAVIEW_ID"), searchCriteriaType,true); //if we want to support different storage locations then this would be set by the input paramter

                    
                    this.Add(searchCriteriaData);

                }
                dr.Close();
            }
            catch (Exception)
            {
                dr.Close();
                throw;
            }
        }

        #endregion //Data Access - Fetch

        #region Data Access - Update
        protected override void DataPortal_Update()
        {
            try
            {
                RaiseListChangedEvents = false;
                if (_coeDAL == null) { LoadDAL(); }

                // loop through each deleted child object
                foreach (COESearchCriteriaBO deletedChild in DeletedList)
                    deletedChild.DeleteSelf(_coeDAL);
                DeletedList.Clear();

                // loop through each non-deleted child object
                foreach (COESearchCriteriaBO child in this)
                {
                    if (child.IsNew)
                        child.Insert(_coeDAL);
                    else
                        child.Update(_coeDAL);


                    RaiseListChangedEvents = true;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        #endregion //Data Access - Update

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

        #endregion //Data Access

    }

}
    

