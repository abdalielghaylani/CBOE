using System;
using System.Collections.Generic;
using System.Text;
using CambridgeSoft.COE.Framework.Common;
using Csla;
using Csla.Data;
using CambridgeSoft.COE.Framework.Export;
using CambridgeSoft.COE.Framework.Properties;

namespace CambridgeSoft.COE.Framework.COEExportService
{
    [Serializable()]
    public class COEExportTemplateBOList : Csla.BusinessListBase<COEExportTemplateBOList, COEExportTemplateBO>
    {
        // variables for data access
        [NonSerialized]
        private DAL _coeDAL = null;
        [NonSerialized]
        private DALFactory _dalFactory = new DALFactory();
        private string _serviceName = "COEExport";
        

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

        private COEExportTemplateBOList()
        {
            /* require use of factory method */
        }

        public static COEExportTemplateBOList NewList()
        {
            SetDatabaseName();

            try
            {
                if (!CanAddObject())
                    throw new System.Security.SecurityException(Resources.UserNotAuthorizedForAddObject + " COEExportTemplateBOList");
                return new COEExportTemplateBOList();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public static COEExportTemplateBOList NewList(string databaseName)
        {
            SetDatabaseName(databaseName);

            try
            {
                if (!CanAddObject())
                    throw new System.Security.SecurityException(Resources.UserNotAuthorizedForAddObject + " COEExportTemplateBOList");
                return new COEExportTemplateBOList();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public static COEExportTemplateBOList GetTemplates(bool isPublic)
        {
            SetDatabaseName();

            try
            {
                if (!CanGetObject())
                    throw new System.Security.SecurityException(Resources.UserNotAuthorizedForViewObject + " COEExportTemplateBOList");
                return DataPortal.Fetch<COEExportTemplateBOList>(new Criteria(isPublic));

            }
            catch (Exception)
            {

                throw;
            }
        }

        public static COEExportTemplateBOList GetTemplates()
        {
            SetDatabaseName();

            try
            {
                if (!CanGetObject())
                    throw new System.Security.SecurityException(Resources.UserNotAuthorizedForViewObject + " COEExportTemplateBOList");
                return DataPortal.Fetch<COEExportTemplateBOList>();

            }
            catch (Exception)
            {

                throw;
            }
        }

        public static COEExportTemplateBOList GetUserTemplatesByUserId(string userId)
        {
            SetDatabaseName();
            try
            {
                if (!CanGetObject())
                    throw new System.Security.SecurityException(Resources.UserNotAuthorizedForViewObject + " COEExportTemplateBOList");
                return DataPortal.Fetch<COEExportTemplateBOList>(new GetByUserIdCriteria(userId));
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static COEExportTemplateBOList GetUserTemplatesByDataViewId(int dataViewId, string userId, bool getPublicTemplates)
        {
            SetDatabaseName();
            try
            {
                if (!CanGetObject())
                    throw new System.Security.SecurityException(Resources.UserNotAuthorizedForViewObject + " COEExportTemplateBOList");
                return DataPortal.Fetch<COEExportTemplateBOList>(new GetByDataViewIdCriteria(dataViewId, userId, getPublicTemplates));
            }
            catch (Exception)
            {

                throw;
            }
        }

        public static COEExportTemplateBOList GetUserTemplatesByWebFormId(int webFormId)
        {
            SetDatabaseName();
            try
            {
                if (!CanGetObject())
                    throw new System.Security.SecurityException(Resources.UserNotAuthorizedForViewObject + " COEExportTemplateBOList");
                return DataPortal.Fetch<COEExportTemplateBOList>(new GetByWebFormIdCriteria(webFormId));
            }
            catch (Exception)
            {

                throw;
            }
        }

        public static COEExportTemplateBOList GetUserTemplatesByWinFormId(int winFormId)
        {
            SetDatabaseName();
            try
            {
                if (!CanGetObject())
                    throw new System.Security.SecurityException(Resources.UserNotAuthorizedForViewObject + " COEExportTemplateBOList");
                return DataPortal.Fetch<COEExportTemplateBOList>(new GetByWinFormIdCriteria(winFormId));
            }
            catch (Exception)
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

        #endregion

        #region Data Access

        #region Filter Criteria

        [Serializable()]
        private class Criteria
        {

            #region Members

            private string _name;
            private string _description;
            private ResultsCriteria _resultsCriteria;
            private bool _isPublic;
            private SmartDate _dateCreated;
            private int _dataViewID;
            private int _webFormID;
            private int _winFormID;
            private string _userID;

            #endregion

            #region Properties

            public string Name
            {
                get { return _name; }
                set { _name = value; }
            }

            public string Description
            {
                get { return _description; }
                set { _description = value; }
            }

            public ResultsCriteria ResultsCriteria
            {
                get { return _resultsCriteria; }
                set { _resultsCriteria = value; }
            }

            public SmartDate DateCreated
            {
                get { return _dateCreated; }
                set { _dateCreated = value; }
            }

            public bool IsPublic
            {
                get { return _isPublic; }
                set { _isPublic = value; }
            }

            public int DataViewId
            {
                get { return _dataViewID; }
                set { _dataViewID = value; }
            }

            public int WebFormId
            {
                get { return _webFormID; }
                set { _webFormID = value; }
            }

            public int WinFormId
            {
                get { return _winFormID; }
                set { _winFormID = value; }
            }

            public string UserId
            {
                get { return _userID; }
                set { _userID = value; }
            }

            #endregion

            #region Constructor

            public Criteria(bool isPublic)
            {
                this._isPublic = isPublic;
            }

            public Criteria(string name, string description, ResultsCriteria resultsCriteria, SmartDate dateCreated, bool isPublic, int dataViewId, int webFormId, int winFormId, string userId)
            {
                this._name = name;
                this._description = description;
                this._resultsCriteria = resultsCriteria;
                this._dateCreated = dateCreated;
                this._isPublic = isPublic;
                this._dataViewID = dataViewId;
                this._webFormID = webFormId;
                this._winFormID = winFormId;
                this._userID = userId;
            }

            #endregion

        }

        [Serializable()]
        private class GetByDataViewIdCriteria
        {

            #region Members
            private int _dataViewID;
            private string _userID;
            private bool _getPublicTemplates;
            #endregion

            #region Properties

            public int DataViewId
            {
                get { return _dataViewID; }
                set { _dataViewID = value; }
            }

            public string UserId
            {
                get { return _userID; }
                set { _userID = value; }
            }

            public bool GetPublicTemplates
            {
                get { return _getPublicTemplates; }
                set { _getPublicTemplates = value; }
            }
            #endregion

            #region Constructor

            public GetByDataViewIdCriteria(int dataViewId, string userId, bool getPublicTemplates)
            {
                this._dataViewID = dataViewId;
                this._userID = userId;
                this._getPublicTemplates = getPublicTemplates;
            }

            #endregion

        }

        [Serializable()]
        private class GetByWebFormIdCriteria
        {

            #region Members

            private int _webFormID;

            #endregion

            #region Properties

            public int WebFormId
            {
                get { return _webFormID; }
                set { _webFormID = value; }
            }

            #endregion

            #region Constructor

            public GetByWebFormIdCriteria(int webFormId)
            {
                this._webFormID = webFormId;
            }

            #endregion

        }

        [Serializable()]
        private class GetByWinFormIdCriteria
        {

            #region Members

            private int _winFormID;

            #endregion

            #region Properties

            public int WinFormID
            {
                get { return _winFormID; }
                set { _winFormID = value; }
            }

            #endregion

            #region Constructor

            public GetByWinFormIdCriteria(int winFormID)
            {
                this._winFormID = winFormID;
            }

            #endregion

        }

        [Serializable()]
        private class GetByUserIdCriteria
        {

            #region Members

            private string _userID;

            #endregion

            #region Properties

            public string UserID
            {
                get { return _userID; }
                set { _userID = value; }
            }

            #endregion

            #region Constructor

            public GetByUserIdCriteria(string userID)
            {
                this._userID = userID;
            }

            #endregion

        }

        #endregion //Filter Criteria

        #region Data Access - Fetch

        private void DataPortal_Fetch()
        {
            RaiseListChangedEvents = false;
            if (_coeDAL == null) { LoadDAL(); }
            // Coverity Fix CID - 11519
            if (_coeDAL != null)
            {
                using (SafeDataReader dr = _coeDAL.GetAll())
                {
                    Fetch(dr);
                }
            }
            else
                throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));

            RaiseListChangedEvents = true;
        }

        private void DataPortal_Fetch(Criteria criteria)
        {
            RaiseListChangedEvents = false;
            if (_coeDAL == null) { LoadDAL(); }
            // Coverity Fix CID - 11520 
            if (_coeDAL != null)
            {
                using (SafeDataReader dr = _coeDAL.GetAll(criteria.IsPublic))
                {
                    Fetch(dr);
                }
            }
            else
                throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));

            RaiseListChangedEvents = true;
        }

        private void DataPortal_Fetch(GetByUserIdCriteria criteria)
        {
            RaiseListChangedEvents = false;
            if (_coeDAL == null) { LoadDAL(); }
             // Coverity Fix CID - 11522
            if (_coeDAL != null)
            {
                using (SafeDataReader dr = _coeDAL.GetUserTemplates(criteria.UserID))
                {
                    Fetch(dr);
                }
            }
            else
                throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));

            RaiseListChangedEvents = true;
        }

        private void DataPortal_Fetch(GetByDataViewIdCriteria criteria)
        {
            RaiseListChangedEvents = false;
            if (_coeDAL == null) { LoadDAL(); }
            // Coverity Fix CID - 11521
            if (_coeDAL != null)
            {
                using (SafeDataReader dr = _coeDAL.GetUserTemplatesByDataViewId(criteria.DataViewId, criteria.UserId, criteria.GetPublicTemplates))
                {
                    Fetch(dr);
                }
            }
            else
                throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));

            RaiseListChangedEvents = true;
        }

        private void DataPortal_Fetch(GetByWebFormIdCriteria criteria)
        {
            RaiseListChangedEvents = false;
            if (_coeDAL == null) { LoadDAL(); }            
            // Coverity Fix CID - 11523
            if (_coeDAL != null)
            {
                using (SafeDataReader dr = _coeDAL.GetTemplatesByWebFormId(criteria.WebFormId))
                {
                    Fetch(dr);
                }
            }
            else
                throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));

            RaiseListChangedEvents = true;
        }

        private void DataPortal_Fetch(GetByWinFormIdCriteria criteria)
        {
            RaiseListChangedEvents = false;
            if (_coeDAL == null) { LoadDAL(); }
            // Coverity Fix CID - 11524
            if (_coeDAL != null)
            {
                using (SafeDataReader dr = _coeDAL.GetTemplatesByWinFormId(criteria.WinFormID))
                {
                    Fetch(dr);
                }
            }
            else
                throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));

            RaiseListChangedEvents = true;
        }

        protected void Fetch(SafeDataReader dr)
        {
            try
            {
                while (dr.Read())
                {
                    COEExportTemplateBO exportTemplateData = new COEExportTemplateBO(
                        dr.GetInt32("ID"),
                        dr.GetString("NAME"),
                        dr.GetString("DESCRIPTION"),
                        (ResultsCriteria)COEExportTemplateUtilities.DeserializeCOEResultCriteria(dr.GetString("COERESULTCRITERIA")),
                        (dr.GetString("IS_PUBLIC").Equals("1") ? true : false),
                        dr.GetInt32("DATAVIEW_ID"),
                        dr.GetInt32("WEBFORM_ID"),
                        dr.GetInt32("WINFORM_ID"),
                        dr.GetSmartDate("DATE_CREATED"),
                        dr.GetString("USER_ID"));

                    this.Add(exportTemplateData);

                }
                dr.Close();
            }
            catch (Exception)
            {
                dr.Close();
                throw;
            }
        }

        #endregion

        #region Data Access - Update
        protected override void DataPortal_Update()
        {
            RaiseListChangedEvents = false;
            if (_coeDAL == null) { LoadDAL(); }

            // loop through each deleted child object
            foreach (COEExportTemplateBO deletedChild in DeletedList)
                deletedChild.DeleteSelf(_coeDAL);
            DeletedList.Clear();

            // loop through each non-deleted child object
            foreach (COEExportTemplateBO child in this)
            {
                if (child.IsNew)
                    child.Insert(_coeDAL);
                else
                    child.Update(_coeDAL);
                RaiseListChangedEvents = true;
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

        #endregion

    }
}
