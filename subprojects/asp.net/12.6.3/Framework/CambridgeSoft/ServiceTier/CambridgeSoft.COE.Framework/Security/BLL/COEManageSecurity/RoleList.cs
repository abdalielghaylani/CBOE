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

namespace CambridgeSoft.COE.Framework.COESecurityService
{
    [Serializable()]
    public class RoleList :
      NameValueListBase<string, string>
    {
        #region member variables

        private static RoleList _list;

        //variables data access
        [NonSerialized]
        private DAL _coeDAL = null;
        [NonSerialized]
        private DALFactory _dalFactory = new DALFactory();
        private string _serviceName = "COESecurity";

        [NonSerialized]
        static COELog _coeLog = COELog.GetSingleton("COESecurity");


        #endregion

        #region Business Methods



        #endregion

        #region Factory Methods

        //this method must be called prior to any other method inorder to set the database that the dal will use
        internal static void SetDatabaseName()
        {
            COEDatabaseName.Set(DALUtils.GetDefaultQualifyDbName(Resources.CentralizedStorageDB));
        }

        internal static void SetDatabaseName(string databaseName)
        {
            COEDatabaseName.Set(DALUtils.GetDefaultQualifyDbName(Resources.CentralizedStorageDB));

        }
       
        public static RoleList GetFromList(NameValueListBase<string, string> list)
        {
            return DataPortal.Fetch<RoleList>(new NameValueList(list));
        }

        public static RoleList GetList()
        {
            SetDatabaseName();
            return DataPortal.Fetch<RoleList>();
        }

        public static RoleList GetListByApplication(string appName)
        {
            SetDatabaseName();
            return DataPortal.Fetch<RoleList>(new CriteriaByApp(appName));
        }


        public static RoleList GetListByUser(string userName)
        {
            SetDatabaseName();
            return DataPortal.Fetch<RoleList>(new CriteriaByUser(string.Empty, userName));
        }

         public static RoleList GetListByUser(string appName, string userName)
        {
            SetDatabaseName();
            return DataPortal.Fetch<RoleList>(new CriteriaByUser(appName, userName));
        }

        public static RoleList GetListByRole(string roleName)
        {
            SetDatabaseName();
            return DataPortal.Fetch<RoleList>(new CriteriaByRole(roleName));
        }
        public static RoleList GetDBMSList()
        {
            SetDatabaseName();
            return DataPortal.Fetch<RoleList>(new DBMSCriteria());
        }

        public static RoleList GetGroupRoleList(int groupID)
        {
            SetDatabaseName();
            return DataPortal.Fetch<RoleList>(new GroupRoleListCriteria(groupID));
        }

        public static RoleList GetGroupRoleAvailableList(int groupID)
        {
            SetDatabaseName();
            return DataPortal.Fetch<RoleList>(new GroupRoleListAvailCriteria(groupID));
        }

        public static RoleList GetGroupPersonDirectRoleList(int groupID, int personID)
        {
            SetDatabaseName();
            return DataPortal.Fetch<RoleList>(new PersonDirectRoleCriteria(groupID, personID));
        }

        public static RoleList GetGroupPersonDirectAvailableRoleList(int personID)
        {
            SetDatabaseName();
            return DataPortal.Fetch<RoleList>(new PersonDirectAvailRoleCriteria(personID));
        }

        /// <summary>
        /// Clears the in-memory RoleList cache
        /// so the list of roles is reloaded on
        /// next request.
        /// </summary>
        public static void InvalidateCache()
        {
            _list = null;
        }

        private RoleList()
        { /* require use of factory methods */ }

        #endregion

        #region critiera
        [Serializable()]
        private class GroupRoleListCriteria
        {
            internal int _groupID;
            //constructors
            public GroupRoleListCriteria(int groupID)
            {
                _groupID = groupID;
            }
        }
        [Serializable()]
        private class GroupRoleListAvailCriteria
        {
            internal int _groupID;
            //constructors
            public GroupRoleListAvailCriteria(int groupID)
            {
                _groupID = groupID;
            }
        }

        [Serializable()]
        private class PersonDirectRoleCriteria
        {

            internal int _personID;
            internal int _groupID;
            //constructors
            public PersonDirectRoleCriteria(int groupID, int personID)
            {

                _personID = personID;
                _groupID = groupID;
            }
        }

        [Serializable()]
        private class PersonDirectAvailRoleCriteria
        {

            internal int _personID;
            //constructors
            public PersonDirectAvailRoleCriteria(int personID)
            {
                _personID = personID;
            }
        }
        [Serializable()]

        private class CriteriaByApp
        {
            internal string _appName;
            //constructors
            public CriteriaByApp(string appName)
            {
                _appName = appName;
            }
        }

        [Serializable()]
        private class CriteriaByUser
        {
            internal string _userName;
            internal string _appName;
            //constructors
            public CriteriaByUser(string appName, string userName)
            {
                _userName = userName;
                _appName = appName;
            }
        }

        [Serializable()]
        private class CriteriaByRole
        {
            internal string _roleName;
           
            //constructors
            public CriteriaByRole(string roleName)
            {
                _roleName = roleName;
               
            }
        }

        [Serializable()]
        private class DBMSCriteria
        {

            //constructors
            public DBMSCriteria()
            {

            }
        }

        [Serializable()]
        private class NameValueList
        {
            internal NameValueListBase<string, string> _list;

            //constructors
            public NameValueList(NameValueListBase<string, string> list)
            {
                _list = list;
            }
        }

        #endregion

        #region Data Access
        private void DataPortal_Fetch(NameValueList criteria)
        {
            this.RaiseListChangedEvents = false;
            try
            {
                IsReadOnly = false;
                foreach (NameValueListBase<string, string>.NameValuePair listItem in criteria._list)
                {
                    this.Add(new NameValuePair(listItem.Value, listItem.Key));
                }

            }
            catch (Exception)
            {

                throw;
            }
            IsReadOnly = true;
            this.RaiseListChangedEvents = true;
        }
        private void DataPortal_Fetch(GroupRoleListCriteria criteria)
        {
            this.RaiseListChangedEvents = false;
            try
            {
                _coeLog.LogStart(string.Empty, 1);
                if (_coeDAL == null) { LoadDAL(); }
                 // Coverity Fix CID - 11643
                if (_coeDAL != null)
                {
                    DataTable dt = _coeDAL.GetGroupPrivSets(criteria._groupID);
                    FetchGroupObject(dt);
                }
                else
                    throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));

                _coeLog.LogEnd(string.Empty, 1);
            }
            catch (Exception)
            {

                throw;
            }
            this.RaiseListChangedEvents = true;
        }

        private void DataPortal_Fetch(GroupRoleListAvailCriteria criteria)
        {
            this.RaiseListChangedEvents = false;
            try
            {
                _coeLog.LogStart(string.Empty, 1);
                if (_coeDAL == null) { LoadDAL(); }
                 // Coverity Fix CID - 11643
                if (_coeDAL != null)
                {
                    DataTable dt = _coeDAL.GetGroupAvailablePrivSets(criteria._groupID);
                    FetchGroupObject(dt);
                }
                else
                    throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));

                _coeLog.LogEnd(string.Empty, 1);
            }
            catch (Exception)
            {

                throw;
            }
            this.RaiseListChangedEvents = true;
        }

        private void DataPortal_Fetch(PersonDirectRoleCriteria criteria)
        {
            this.RaiseListChangedEvents = false;
            try
            {
                _coeLog.LogStart(string.Empty, 1);
                if (_coeDAL == null) { LoadDAL(); }
                 // Coverity Fix CID - 11645
                if (_coeDAL != null)
                {
                    DataTable dt = _coeDAL.GetPersonNonGSRoles(criteria._groupID, criteria._personID);
                    FetchGroupObject(dt);
                }
                else
                    throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));

                _coeLog.LogEnd(string.Empty, 1);
            }
            catch (Exception)
            {

                throw;
            }
            this.RaiseListChangedEvents = true;
        }
        private void DataPortal_Fetch(PersonDirectAvailRoleCriteria criteria)
        {
            this.RaiseListChangedEvents = false;
            try
            {
                _coeLog.LogStart(string.Empty, 1);
                if (_coeDAL == null) { LoadDAL(); }
              

                _coeLog.LogEnd(string.Empty, 1);
            }
            catch (Exception)
            {

                throw;
            }
            this.RaiseListChangedEvents = true;
        }

        private void DataPortal_Fetch(DBMSCriteria criteria)
        {
            this.RaiseListChangedEvents = false;
            try
            {
                _coeLog.LogStart(string.Empty, 1);
                if (_coeDAL == null) { LoadDAL(); }
                // Coverity Fix CID - 11642
                if (_coeDAL != null)
                {
                    DataTable dt = _coeDAL.GetDBMSRoles();
                    FetchObject(dt);
                }
                else
                    throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));

                _coeLog.LogEnd(string.Empty, 1);
            }
            catch (Exception)
            {

                throw;
            }
            this.RaiseListChangedEvents = true;
        }

        private void DataPortal_Fetch(Criteria criteria)
        {
            this.RaiseListChangedEvents = false;
            try
            {
                _coeLog.LogStart(string.Empty, 1);
                if (_coeDAL == null) { LoadDAL(); }
                // Coverity Fix CID - 11646
                if (_coeDAL != null)
                {
                    //get all roles regardless of app or user
                    DataTable dt = _coeDAL.GetAllCOERoles(string.Empty);
                    FetchObject(dt);
                }
                else
                    throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));

                _coeLog.LogEnd(string.Empty, 1);
            }
            catch (Exception)
            {

                throw;
            }
            this.RaiseListChangedEvents = true;
        }

        private void DataPortal_Fetch(CriteriaByApp criteria)
        {
            this.RaiseListChangedEvents = false;
            try
            {
                _coeLog.LogStart(string.Empty, 1);
                if (_coeDAL == null) { LoadDAL(); }
                // Coverity Fix CID - 11639
                if (_coeDAL != null)
                {
                    DataTable dt = _coeDAL.GetAllCOERoles(criteria._appName);
                    FetchObject(dt);
                }

                _coeLog.LogEnd(string.Empty, 1);
            }
            catch (Exception)
            {

                throw;
            }
            this.RaiseListChangedEvents = true;
        }


        private void DataPortal_Fetch(CriteriaByUser criteria)
        {
            this.RaiseListChangedEvents = false;
            try
            {
                _coeLog.LogStart(string.Empty, 1);
                if (_coeDAL == null) { LoadDAL(); }
                  // Coverity Fix CID - 11641
                if (_coeDAL != null)
                {
                    DataTable dt = _coeDAL.GetAllCOEUserRoles(criteria._appName, criteria._userName);
                    FetchObject(dt);
                }
                else
                    throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));

                _coeLog.LogEnd(string.Empty, 1);
            }
            catch (Exception)
            {

                throw;
            }
            this.RaiseListChangedEvents = true;
        }

        private void DataPortal_Fetch(CriteriaByRole criteria)
        {
            this.RaiseListChangedEvents = false;
            try
            {
                _coeLog.LogStart(string.Empty, 1);
                if (_coeDAL == null) { LoadDAL(); }
                 // Coverity Fix CID - 11640
                if (_coeDAL != null)
                {
                    DataTable dt = _coeDAL.GetRoleRoles(criteria._roleName);
                    FetchObject(dt);
                }
                else
                    throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));

                _coeLog.LogEnd(string.Empty, 1);
            }
            catch (Exception)
            {

                throw;
            }
            this.RaiseListChangedEvents = true;
        }

        private void FetchObject(DataTable dt)
        {
            try
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    IsReadOnly = false;
                    DataRow dr = dt.Rows[i];
                    if (dr["ROLE_NAME"].ToString() != string.Empty)
                    {
                        this.Add(new NameValuePair(dr["ROLE_NAME"].ToString(), dr["ROLE_NAME"].ToString()));
                    }


                }

                IsReadOnly = true;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private void FetchGroupObject(DataTable dt)
        {
            try
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    IsReadOnly = false;
                    DataRow dr = dt.Rows[i];
                    this.Add(new NameValuePair(dr["ROLE_ID"].ToString(), dr["ROLE_NAME"].ToString()));
                }

                IsReadOnly = true;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }



        private void LoadDAL()
        {
            if (_dalFactory == null) { _dalFactory = new DALFactory(); }
            _dalFactory.GetDAL<DAL>(ref _coeDAL, _serviceName, COEDatabaseName.Get().ToString(), true);
        }
        #endregion

    }
}

