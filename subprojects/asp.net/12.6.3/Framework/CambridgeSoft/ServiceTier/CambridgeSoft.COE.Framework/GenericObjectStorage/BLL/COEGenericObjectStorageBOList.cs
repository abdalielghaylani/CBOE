using System;
using System.Data;
using Csla;
using Csla.Data;
using Csla.Validation;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.Properties;
using CambridgeSoft.COE.Framework.COELoggingService;


namespace CambridgeSoft.COE.Framework.COEGenericObjectStorageService
{
    /// <summary>
    /// This class provides access to several generic objects at once.
    /// </summary>
    [Serializable()]
    public class COEGenericObjectStorageBOList : Csla.BusinessListBase<COEGenericObjectStorageBOList, COEGenericObjectStorageBO>
    {
        #region Variables
        [NonSerialized]
        private DAL _coeDAL = null;
        [NonSerialized]
        private DALFactory _dalFactory = new DALFactory();
        private string _serviceName = "COEGenericObjectStorage";
        private string _databaseName = string.Empty;
        [NonSerialized]
        static COELog _coeLog = COELog.GetSingleton("COEGenericObjectStorage");

        #endregion

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

        private COEGenericObjectStorageBOList()
        { /* require use of factory method */ }

        /// <summary>
        /// Instanciates a new <see cref="COEGenericObjectStorageBOList"/> for a given database.
        /// </summary>
        /// <param name="databaseName">The database name.</param>
        /// <returns>The <see cref="COEGenericObjectStorageBOList"/></returns>
        public static COEGenericObjectStorageBOList NewList(string databaseName)
        {
            SetDatabaseName();

            if(!CanAddObject())
                throw new System.Security.SecurityException(Resources.UserNotAuthorizedForAddObject + " COEGenericObjectStorageBOList");
            return new COEGenericObjectStorageBOList();
        }

        /// <summary>
        /// Gets the entire list of generic object for which the logged user has permission. (Whether because the user has been granted permissions of because the record is public)
        /// </summary>
        /// <returns>A <see cref="COEGenericObjectStorageBOList"/> with all the records that matched the filter</returns>
        public static COEGenericObjectStorageBOList GetList()
        {
            SetDatabaseName();
            if(!CanGetObject())
                throw new System.Security.SecurityException(Resources.UserNotAuthorizedForViewObject + " COEGenericObjectStorageBOList");
            return DataPortal.Fetch<COEGenericObjectStorageBOList>(new Criteria());
        }

        /// <summary>
        /// Gets all the generic objects owned by the given user for which the logged user has permission. (Whether because the user has been granted permissions of because the record is public)
        /// </summary>
        /// <param name="userName">The user</param>
        /// <returns>The <see cref="COEGenericObjectStorageBOList"/> with all the records that matched the filter</returns>
        public static COEGenericObjectStorageBOList GetList(string userName)
        {
            SetDatabaseName();

            if(!CanGetObject())
                throw new System.Security.SecurityException(Resources.UserNotAuthorizedForViewObject + " COEGenericObjectStorageBOList");
            return DataPortal.Fetch<COEGenericObjectStorageBOList>(new GetByUserCriteria(userName));
        }

        /// <summary>
        /// Gets all the generic objects owned by the given user which has its public state set as the given, for which the logged user has permission. (Whether because the user has been granted permissions of because the record is public)
        /// </summary>
        /// <param name="userName">The user name</param>
        /// <param name="isPublic">A flag indicating if public records will be retrieved or not</param>
        /// <returns>The matching <see cref="COEGenericObjectStorageBOList"/></returns>
        public static COEGenericObjectStorageBOList GetList(string userName, bool isPublic)
        {
            SetDatabaseName();

            if(!CanGetObject())
                throw new System.Security.SecurityException(Resources.UserNotAuthorizedForViewObject + " COEGenericObjectStorageBOList");
            return DataPortal.Fetch<COEGenericObjectStorageBOList>(new GetByUserAndPublicCriteria(userName, isPublic));
        }

        /// <summary>
        /// Gets all the generic objects that matches the User Name and Form Group parameters, for which the logged user has permission. (Whether because the user has been granted permissions of because the record is public)
        /// </summary>
        /// <param name="userName">The user name</param>
        /// <param name="formGroupId">The form group id</param>
        /// <returns>The matching <see cref="COEGenericObjectStorageBOList"/></returns>
        public static COEGenericObjectStorageBOList GetList(string userName, int formGroupId)
        {
            SetDatabaseName();

            if(!CanGetObject())
                throw new System.Security.SecurityException(Resources.UserNotAuthorizedForViewObject + " COEGenericObjectStorageBOList");
            return DataPortal.Fetch<COEGenericObjectStorageBOList>(new GetByUserAndFormGroupCriteria(userName, formGroupId));
        }

        /// <summary>
        /// Gets all the generic objects that matches the User Name and Form Group parameters, for which the logged user has permission. (Whether because the user has been granted permissions of because the record is public)
        /// If retrieveAllData is set to true, the actual data is retrieved, otherwise it is not (Lighter objects, Best suit for listing only and saving network bandwidth)
        /// </summary>
        /// <param name="userName">The user name</param>
        /// <param name="formGroupId">The form group id</param>
        /// <param name="retrieveAllData">Determines if the actual data string must be filled in</param>
        /// <returns>The matching <see cref="COEGenericObjectStorageBOList"/></returns>
        public static COEGenericObjectStorageBOList GetList(string userName, int formGroupId, bool retrieveAllData)
        {
            SetDatabaseName();

            if(!CanGetObject())
                throw new System.Security.SecurityException(Resources.UserNotAuthorizedForViewObject + " COEGenericObjectStorageBOList");
            return DataPortal.Fetch<COEGenericObjectStorageBOList>(new GetByUserAndFormGroupCriteria(userName, formGroupId, retrieveAllData));
        }

        /// <summary>
        /// <para>
        /// Gets all the generic objects that matches the Is Public parameter, excluding the records owned by the given <paramref name="excludeUser"/>, for which the logged user has permission. (Whether because the user has been granted permissions of because the record is public)
        /// </para>
        /// </summary>
        /// <param name="isPublic">A flag indicating if public records will be retrieved or not</param>
        /// <param name="excludeUser">The name of the user whose records will be excluded</param>
        /// <returns>The matching <see cref="COEGenericObjectStorageBOList"/></returns>
        public static COEGenericObjectStorageBOList GetList(bool isPublic, string excludeUser)
        {
            SetDatabaseName();

            if(!CanGetObject())
                throw new System.Security.SecurityException(Resources.UserNotAuthorizedForViewObject + " COEGenericObjectStorageBOList");
            return DataPortal.Fetch<COEGenericObjectStorageBOList>(new GetByPublicCriteria(isPublic, excludeUser));
        }

        /// <summary>
        /// Gets all the generic objects that matches the User Name, Is Public and Form Group parameters, for which the logged user has permission. (Whether because the user has been granted permissions of because the record is public)
        /// </summary>
        /// <param name="userName">The user name</param>
        /// <param name="isPublic">A flag indicating if public records will be retrieved or not</param>
        /// <param name="formGroupId">The form group id</param>
        /// <returns>The matching <see cref="COEGenericObjectStorageBOList"/></returns>
        public static COEGenericObjectStorageBOList GetList(string userName, bool isPublic, int formGroupId)
        {
            SetDatabaseName();

            if(!CanGetObject())
                throw new System.Security.SecurityException(Resources.UserNotAuthorizedForViewObject + " COEGenericObjectStorageBOList");
            return DataPortal.Fetch<COEGenericObjectStorageBOList>(new GetByUserAndPublicAndFormgroupCriteria(userName, isPublic, formGroupId));
        }

        /// <summary>
        /// Gets all the generic objects that are in the given database that matches the User Name, Is Public and Form Group parameters, for which the logged user has permission. (Whether because the user has been granted permissions of because the record is public)
        /// If retrieveAllData is set to true, the actual data is retrieved, otherwise it is not (Lighter objects, Best suit for listing only and saving network bandwidth)
        /// </summary>
        /// <param name="userName">The user name</param>
        /// <param name="isPublic">A flag indicating if public records will be retrieved or not</param>
        /// <param name="formGroupId">The form group id</param>
        /// <param name="retrieveAllData">Determines if the actual data string must be filled in</param>
        /// <returns>The matching <see cref="COEGenericObjectStorageBOList"/></returns>
        public static COEGenericObjectStorageBOList GetList(string userName, bool isPublic, int formGroupId, bool retrieveAllData)
        {
            SetDatabaseName();

            if(!CanGetObject())
                throw new System.Security.SecurityException(Resources.UserNotAuthorizedForViewObject + " COEGenericObjectStorageBOList");
            return DataPortal.Fetch<COEGenericObjectStorageBOList>(new GetByUserAndPublicAndFormgroupCriteria(userName, isPublic, formGroupId, retrieveAllData));
        }

        /// <summary>
        /// Determines if an object can be added.
        /// </summary>
        /// <returns>True if the object can be added, false otherwise</returns>
        public static bool CanAddObject()
        {
            //TO DO:  need to add authorization access code
            return true;
        }

        /// <summary>
        /// Determines if an object can be retrieved.
        /// </summary>
        /// <returns>True if the object can be retrieved, false otherwise</returns>
        public static bool CanGetObject()
        {
            //TO DO:  need to add authorization access code
            return true;
        }

        /// <summary>
        /// Determines if an object can be edited.
        /// </summary>
        /// <returns>True if the object can be edited, false otherwise</returns>
        public static bool CanEditObject()
        {
            //TO DO:  need to add authorization access code
            return true;
        }

        /// <summary>
        /// Determines if an object can be deleted.
        /// </summary>
        /// <returns>True if the object can be deleted, false otherwise</returns>
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
            //constructors
            public Criteria() { }
        }

        [Serializable()]
        private class GetByUserCriteria
        {
            #region Variables
            public string _userName;
            public bool _excludeLoggedInUser;
            #endregion

            #region Constructors
            public GetByUserCriteria(string userName)
            {
                _userName = userName;
            }
            #endregion
        }

        [Serializable()]
        private class GetByUserAndFormGroupCriteria
        {
            #region Variables
            public string _userName;
            public int _formGroup;
            public bool _retrieveAllData;
            #endregion

            #region Constructors
            public GetByUserAndFormGroupCriteria(string userName, int formGroup)
            {
                _userName = userName;
                _formGroup = formGroup;
            }

            public GetByUserAndFormGroupCriteria(string userName, int formGroup, bool retrieveAllData)
            {
                _userName = userName;
                _formGroup = formGroup;
                _retrieveAllData = retrieveAllData;
            }
            #endregion
        }

        [Serializable()]
        private class GetByUserAndPublicCriteria
        {
            #region Variables
            public string _userName;
            public bool _isPublic;
            #endregion

            #region Constructors
            public GetByUserAndPublicCriteria(string userName, bool isPublic)
            {
                _userName = userName;
                _isPublic = isPublic;
            }
            #endregion
        }

        [Serializable()]
        private class GetByPublicCriteria
        {
            #region Variables
            public bool _isPublic;
            public string _excludeUser;
            #endregion

            #region Constructors
            public GetByPublicCriteria(bool isPublic, string excludeUser)
            {
                _isPublic = isPublic;
                _excludeUser = excludeUser;
            }
            #endregion
        }

        [Serializable()]
        private class GetByUserAndPublicAndFormgroupCriteria
        {
            #region Variables
            public string _excludedUser;
            public bool _isPublic;
            public int _formGroup;
            public bool _retrieveAllData;
            #endregion

            #region Constructors
            public GetByUserAndPublicAndFormgroupCriteria(string excludedUser, bool isPublic, int formGroup)
            {
                _excludedUser = excludedUser;
                _isPublic = isPublic;
                _formGroup = formGroup;
            }
            public GetByUserAndPublicAndFormgroupCriteria(string excludedUser, bool isPublic, int formGroup, bool retrieveAllData)
            {
                _excludedUser = excludedUser;
                _isPublic = isPublic;
                _formGroup = formGroup;
                _retrieveAllData = retrieveAllData;
            }
            #endregion
        }
        #endregion //Filter Criteria

        #region Data Access - Fetch

        //get all the saved records in the table
        private void DataPortal_Fetch(Criteria criteria)
        {
            RaiseListChangedEvents = false;
            SafeDataReader dr = null;
            if(_coeDAL == null) { LoadDAL(); }
             // Coverity Fix CID - 11547 
            if (_coeDAL != null)
            {
                dr = _coeDAL.GetAll();
                Fetch(dr);
            }
            else
                throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));

            RaiseListChangedEvents = true;
        }

        private void DataPortal_Fetch(GetByUserCriteria getByUserCriteria)
        {
            RaiseListChangedEvents = false;
            SafeDataReader dr = null;
            if(_coeDAL == null) { LoadDAL(); }
            // Coverity Fix CID - 11552
            if (_coeDAL != null)
            {
                dr = _coeDAL.GetAll(getByUserCriteria._userName);
                Fetch(dr);
            }
            else
                throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));

            RaiseListChangedEvents = true;
        }

        private void DataPortal_Fetch(GetByUserAndFormGroupCriteria getByUserAndFormGroupCriteria)
        {
            RaiseListChangedEvents = false;
            SafeDataReader dr = null;
            if(_coeDAL == null) { LoadDAL(); }
            // Coverity Fix CID - 11549 
            if (_coeDAL != null)
            {
                if (getByUserAndFormGroupCriteria._retrieveAllData)
                    dr = _coeDAL.GetAllCompleteData(getByUserAndFormGroupCriteria._userName, getByUserAndFormGroupCriteria._formGroup);
                else
                    dr = _coeDAL.GetAll(getByUserAndFormGroupCriteria._userName, getByUserAndFormGroupCriteria._formGroup);

                Fetch(dr);
            }
            else
                throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));

            RaiseListChangedEvents = true;
        }

        private void DataPortal_Fetch(GetByUserAndPublicCriteria getByUserAndPublicCriteria)
        {
            RaiseListChangedEvents = false;
            SafeDataReader dr = null;
            if(_coeDAL == null) { LoadDAL(); }
            // Coverity Fix CID - 11551
            if (_coeDAL != null)
            {
                dr = _coeDAL.GetAll(getByUserAndPublicCriteria._userName, getByUserAndPublicCriteria._isPublic);
                Fetch(dr);
            }
            else
                throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));

            RaiseListChangedEvents = true;
        }

        private void DataPortal_Fetch(GetByUserAndPublicAndFormgroupCriteria getByUserAndPublicAndFormgroupCriteria)
        {
            RaiseListChangedEvents = false;
            SafeDataReader dr = null;
            if(_coeDAL == null) { LoadDAL(); }
            // Coverity Fix CID - 11550
            if (_coeDAL != null)
            {
                if (getByUserAndPublicAndFormgroupCriteria._retrieveAllData)
                    dr = _coeDAL.GetAllCompleteData(getByUserAndPublicAndFormgroupCriteria._excludedUser, getByUserAndPublicAndFormgroupCriteria._isPublic, getByUserAndPublicAndFormgroupCriteria._formGroup);
                else
                    dr = _coeDAL.GetAll(getByUserAndPublicAndFormgroupCriteria._excludedUser, getByUserAndPublicAndFormgroupCriteria._isPublic, getByUserAndPublicAndFormgroupCriteria._formGroup);
                Fetch(dr);
            }
            else
                throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));

            RaiseListChangedEvents = true;
        }

        private void DataPortal_Fetch(GetByPublicCriteria getByPublicCriteria)
        {
            RaiseListChangedEvents = false;
            SafeDataReader dr = null;
            if(_coeDAL == null) { LoadDAL(); }

            // Coverity Fix CID - 11548
            if (_coeDAL != null)
            {
                dr = _coeDAL.GetAll(getByPublicCriteria._isPublic, getByPublicCriteria._excludeUser);
                Fetch(dr);
            }
            else
                throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));

            RaiseListChangedEvents = true;
        }

        protected void Fetch(SafeDataReader dr)
        {
            while(dr.Read())
            {
                string data = string.Empty;
                if(dr.GetSchemaTable().Select("[ColumnName]='COEGENERICOBJECT'").Length > 0)
                {
                    data = dr.GetString("COEGENERICOBJECT");
                }

                COEGenericObjectStorageBO genericObjectData = new COEGenericObjectStorageBO(
                    int.Parse(dr.GetValue("ID").ToString()), //weird but needed.
                    dr.GetString("NAME"),
                    dr.GetString("DESCRIPTION"),
                    dr.GetString("USER_ID"),
                    dr.GetChar("IS_PUBLIC") == '1' ? true : false,
                    int.Parse(dr.GetValue("FORMGROUP").ToString()), //weird but needed.
                    dr.GetSmartDate("DATE_CREATED"),
                    data,
                    dr.GetString("DATABASE"),
                    true,
                    false,
                    false, dr.GetString("ASSOCIATEDDATAVIEWID")); //if we want to support database specific storage then we will need to use the databaseName parameter
                                
                this.Add(genericObjectData);

            }
            dr.Close();
        }

        #endregion //Data Access - Fetch

        #region Data Access - Update
        protected override void DataPortal_Update()
        {
            RaiseListChangedEvents = false;
            if(_coeDAL == null) { LoadDAL(); }

            // loop through each deleted child object
            foreach(COEGenericObjectStorageBO deletedChild in DeletedList)
                deletedChild.DeleteSelf(_coeDAL);
            DeletedList.Clear();

            // loop through each non-deleted child object
            foreach(COEGenericObjectStorageBO child in this)
            {
                if(child.IsNew)
                    child.Insert(_coeDAL);
                else
                    child.Update(_coeDAL);

                RaiseListChangedEvents = true;
            }
        }

        #endregion //Data Access - Update

        private void LoadDAL()
        {
            if(_dalFactory == null) { _dalFactory = new DALFactory(); }

            _dalFactory.GetDAL<DAL>(ref _coeDAL, _serviceName, COEDatabaseName.Get(), true);
        }

        #endregion //Data Access
    }
}


