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
    public class COERoleBOList :
      BusinessListBase<COERoleBOList, COERoleBO>
    {
        #region member variables
        //variables data access
        [NonSerialized]
        private DAL _coeDAL = null;
        [NonSerialized]
        private DALFactory _dalFactory = new DALFactory();
        private string _serviceName = "COESecurity";
        private string _privilegeTableName;

        [NonSerialized]
        static COELog _coeLog = COELog.GetSingleton("COESecurity");
        #endregion


        #region Authorization Rules

        public static bool CanAddObject()
        {
            // TODO: customize to check user role
            //return ApplicationContext.User.IsInRole("");
            return true;
        }

        public static bool CanGetObject()
        {
            // TODO: customize to check user role
            //return ApplicationContext.User.IsInRole("");
            return true;
        }

        public static bool CanEditObject()
        {
            // TODO: customize to check user role
            //return ApplicationContext.User.IsInRole("");
            return true;
        }

        public static bool CanDeleteObject()
        {
            // TODO: customize to check user role
            //return ApplicationContext.User.IsInRole("");
            return true;
        }

        #endregion

        #region Factory Methods
        public COERoleBO GetRoleByID(int roleID)
        {
            foreach (COERoleBO role in this)
                if (role.RoleID == roleID)
                    return role;
            return null;
        }
        public COERoleBO GetRoleByRoleName(string roleName)
        {
            foreach (COERoleBO role in this)
                if (role.RoleName == roleName)
                    return role;

            return null;
        }
        public void AddRole(COERoleBO role)
        {
            if (!Contains(role.RoleID) && !Contains(role.RoleName))     //CBOE-1793 COE Security-Add User-Observed available roles duplication for multiple times  ASV 05SEP13
            {

                this.Add(role);
            }
            else
            {
                throw new InvalidOperationException("role already exists for user");
            }

        }

        public void RemoveRole(COERoleBO role)
        {
            foreach (COERoleBO hasRole in this)
            {
                if (hasRole.RoleID == role.RoleID)
                {
                    this.AllowRemove = true;
                    this.DeletedList.Add(role);
                    break;
                }
            }

        }


        public bool Contains(int roleID)
        {
            bool boolReturn = false;
            foreach (COERoleBO hasRole in this)
            {
                if (hasRole.RoleID == roleID)
                {
                    boolReturn = true;
                    break;
                }

            }
            return boolReturn;
        }


        public bool Contains(string roleName)
        {
            bool boolReturn = false;
            foreach (COERoleBO hasRole in this)
            {
                if (hasRole.RoleName == roleName)
                {
                    boolReturn = true;
                    break;
                }

            }
            return boolReturn;
        }

        //this method must be called prior to any other method inorder to set the database that the dal will use
        internal static void SetDatabaseName()
        {
            COEDatabaseName.Set(DALUtils.GetDefaultQualifyDbName(Resources.CentralizedStorageDB));
        }


        internal static void SetDatabaseName(string databaseName)
        {
            COEDatabaseName.Set(DALUtils.GetDefaultQualifyDbName(Resources.CentralizedStorageDB));

        }
        public static COERoleBOList NewList(List<String> RoleList)
        {
            COERoleBOList newList = new COERoleBOList();
            COERoleBOList newList2 = newList.BuildFromListItems(newList, RoleList);
            return newList2;
        }



        public static List<string> GetRoleNameList(COERoleBOList roleList)
        {
            List<string> myRoleList = new List<string>();
            foreach (COERoleBO role in roleList)
            {
                if (!myRoleList.Contains(role.RoleName))           //CBOE-1793 COE Security-Add User-Observed available roles duplication for multiple times  ASV 05SEP13
                myRoleList.Add(role.RoleName);
            }
            return myRoleList;
        }

        public static COERoleBOList GetNewUserAvailableRoles()
        {
            //COERoleBOList newList = new COERoleBOList();
            //List<string> myRoleList = new List<string>();
            //myRoleList.Add("CSS_ADMIN");
            //myRoleList.Add("CSS_USER");
            //COERoleBOList newList2 = newList.BuildFromListItems(newList, myRoleList);
            //return newList2;
            SetDatabaseName();
            return DataPortal.Fetch<COERoleBOList>();

        }

        public COERoleBOList BuildFromListItems(COERoleBOList newList, List<string> roles)
        {
            if (roles != null)
            {
                foreach (string roleName in roles)
                {
                    if (!newList.Contains(roleName))                     //CBOE-1793 COE Security-Add User-Observed available roles duplication for multiple times  ASV 05SEP13
                    {
                    COERoleBO coeRoleBO = new COERoleBO(
                             0,
                             roleName,
                             null,
                             null,
                             null,
                             null
                             );
                    newList.Add(coeRoleBO);
                }
            }
            }
            else
            {
                newList = null;
            }

            return newList;
        }
        public static COERoleBOList GetList()
        {
            SetDatabaseName();
            return DataPortal.Fetch<COERoleBOList>();
        }

        public static COERoleBOList GetListByApplication(List<string> applicationList)
        {
            SetDatabaseName();
            return DataPortal.Fetch<COERoleBOList>(new Criteria(FilterType.Application, applicationList, null));
        }
        public static COERoleBOList GetListByApplication(string applicationName, bool getPrivileges, bool getRoleRoles, bool getRoleUsers)
        {
            List<string> applicationList = new List<string>();
            applicationList.Add(applicationName);
            SetDatabaseName();
            return DataPortal.Fetch<COERoleBOList>(new Criteria(FilterType.Application, applicationList, null, getPrivileges, getRoleRoles, getRoleUsers));
        }

        public static COERoleBOList GetListByUser(List<string> userList)
        {
            SetDatabaseName();
            return DataPortal.Fetch<COERoleBOList>(new Criteria(FilterType.User, userList, null));
        }

        public static COERoleBOList GetListByUser(List<string> userList, bool getPrivileges)
        {
            SetDatabaseName();
            return DataPortal.Fetch<COERoleBOList>(new Criteria(FilterType.User, userList, null, true, false, false));
        }

        public static COERoleBOList GetRoleAvailableRoles(COERoleBO role)
        {
            string privTableName = role.PrivilegeTableName;
            string roleName = role.RoleName;
            SetDatabaseName();
            return DataPortal.Fetch<COERoleBOList>(new CriteriaAvailRoles(privTableName, roleName));
        }

        public static COERoleBOList GetUserAvailableRoles(string userName)
        {
            string privTableName = string.Empty;
            SetDatabaseName();
            return DataPortal.Fetch<COERoleBOList>(new CriteriaUserAvailRoles(privTableName, userName));
        }



        public static COERoleBOList GetListByUser(string user)
        {
            List<string> userList = new List<string>();
            userList.Add(user);
            SetDatabaseName();
            return DataPortal.Fetch<COERoleBOList>(new Criteria(FilterType.User, userList, null));
        }

        public static COERoleBOList GetListByRoleID(List<int> roleList)
        {
            SetDatabaseName();
            return DataPortal.Fetch<COERoleBOList>(new Criteria(FilterType.Role, null, roleList));
        }

        public static COERoleBOList GetRoleRoles(string roleName)
        {
            SetDatabaseName();
            return DataPortal.Fetch<COERoleBOList>(new CriteriaByRoleName(roleName));

        }

        internal COERoleBOList()
        { /* require use of factory methods */ }

        #endregion

        #region public enums

        [Serializable()]
        private enum FilterType
        {
            Application,
            User,
            Role
        }
        #endregion

        #region Data Access
        [Serializable()]
        private class CriteriaAvailRoles
        {
            internal string _privTableName = string.Empty;
            internal string _roleName = string.Empty;
            public CriteriaAvailRoles(string privTableName, string roleName)
            {
                _privTableName = privTableName;
                _roleName = roleName;

            }

        }

        [Serializable()]
        private class CriteriaUserAvailRoles
        {
            internal string _privTableName = string.Empty;
            internal string _userName = string.Empty;

            public CriteriaUserAvailRoles(string privTableName, string userName)
            {
                _privTableName = privTableName;
                _userName = userName;

            }

        }



        [Serializable()]
        private class Criteria
        {
            private FilterType _filterType;
            private List<string> _filterList;
            private List<int> _intFilterList;
            private bool _getPrivileges = false;
            internal bool _getRoleRoles = false;
            internal bool _getRoleUsers = false;


            public List<int> IntFilterList
            {
                get { return _intFilterList; }
            }

            public List<string> FilterList
            {
                get { return _filterList; }
            }

            public FilterType FilterType
            {
                get { return _filterType; }
            }

            public bool GetPrivileges
            {
                get { return _getPrivileges; }
            }
            public Criteria(FilterType filter, List<string> filterList, List<int> intFilterList)
            {
                _filterType = filter;
                _filterList = filterList;
                _intFilterList = intFilterList;
            }

            public Criteria(FilterType filter, List<string> filterList, List<int> intFilterList, bool getPrivileges, bool getRoleRoles, bool getRoleUsers)
            {
                _filterType = filter;
                _filterList = filterList;
                _intFilterList = intFilterList;
                _getPrivileges = getPrivileges;
                _getRoleRoles = getRoleRoles;
                _getRoleUsers = getRoleUsers;
            }
        }

        [Serializable()]
        private class CriteriaByRoleName
        {
            internal string _roleName = string.Empty;
            public CriteriaByRoleName(string roleName)
            {
                _roleName = roleName;

            }

        }

        private void DataPortal_Fetch(CriteriaByRoleName criteria)
        {
            RaiseListChangedEvents = false;
            if (_coeDAL == null) { LoadDAL(); }
            DataTable dt = null;
            try
            {
                // Coverity Fix CID - 11621
                if (_coeDAL != null)
                {
                    dt = _coeDAL.GetRoleRoles(criteria._roleName);
                    Fetch(dt);
                }
                else
                    throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));
            }
            catch (Exception)
            {
                throw;
            }
            RaiseListChangedEvents = true;
        }

        private void DataPortal_Fetch()
        {
            RaiseListChangedEvents = false;
            if (_coeDAL == null) { LoadDAL(); }
            // Coverity Fix CID - 11618
            if (_coeDAL != null)
            {
                using (SafeDataReader dr = _coeDAL.GetAllRoles())
                {
                    Fetch(dr, false, false, false);
                }
            }
            else
                throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));

            RaiseListChangedEvents = true;
        }

        private void DataPortal_Fetch(CriteriaAvailRoles criteria)
        {
            RaiseListChangedEvents = false;
            if (_coeDAL == null) { LoadDAL(); }
            DataTable dt = null;
            //_privilegeTableName = criteria._privTableName;
            //get all available roles
            _privilegeTableName = string.Empty;
            try
            {
                // Coverity Fix CID - 11620 
                if (_coeDAL != null)
                {
                    dt = _coeDAL.GetRoleAvailableRoles(_privilegeTableName, criteria._roleName);
                    Fetch(dt);
                }
                else
                    throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));
            }
            catch (Exception)
            {
                throw;
            }            
            RaiseListChangedEvents = true;
        }

        private void DataPortal_Fetch(CriteriaUserAvailRoles criteria)
        {

            RaiseListChangedEvents = false;
            if (_coeDAL == null) { LoadDAL(); }
            DataTable dt = null;
            //_privilegeTableName = criteria._privTableName;
            //get all available roles
            _privilegeTableName = string.Empty;

            try
            {
                 // Coverity Fix CID - 11622
                if (_coeDAL != null)
                {
                    dt = _coeDAL.GetAvailableRoles(criteria._userName, _privilegeTableName);
                    Fetch(dt);
                }
                else
                    throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));
            }
            catch (Exception)
            {
                throw;
            }
            RaiseListChangedEvents = true;
        }



        private void DataPortal_Fetch(Criteria criteria)
        {
            try
            {
                RaiseListChangedEvents = false;
                if (_coeDAL == null) { LoadDAL(); }
                SafeDataReader dr = null;                
                try
                {
                    // Coverity Fix CID - 11619 
                    if (_coeDAL != null)
                    {
                        switch (criteria.FilterType)
                        {
                            case FilterType.Application:
                                dr = _coeDAL.GetRolesByApplication(criteria.FilterList);
                                break;
                            case FilterType.User:
                                dr = _coeDAL.GetRolesByUser(criteria.FilterList);

                                break;
                            case FilterType.Role:
                                dr = _coeDAL.GetRolesByRoleIDS(criteria.IntFilterList);
                                break;

                        }
                        // Coverity Fix CID - 10429 (from local server)
                        if(dr != null)
                            Fetch(dr, criteria.GetPrivileges, criteria._getRoleRoles, criteria._getRoleUsers);
                    }
                    else
                        throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));
                }
                finally
                {
                    if(dr != null && !dr.IsClosed)
                        dr.Close();
                }
            }
            catch (Exception)
            {
                throw;
            }

            RaiseListChangedEvents = true;
        }

        #endregion

        protected void Fetch(DataTable dt)
        {
            try
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {

                    DataRow dr = dt.Rows[i];
                    if (dr["ROLE_NAME"].ToString() != string.Empty)
                    {

                        COERoleBO RoleBO = COERoleBO.Get(dr["ROLE_NAME"].ToString());
                        if (!string.IsNullOrEmpty(RoleBO.RoleName) && !this.Contains(RoleBO.RoleName))                   //CBOE-1793 COE Security-Add User-Observed available roles duplication for multiple times  ASV 05SEP13
                        {
                            this.Add(RoleBO);
                        }
                    }


                }
            }
            catch (Exception)
            {

                throw;
            }
        }




        protected void Fetch(SafeDataReader dr, bool getPrivileges, bool getRoleRoles, bool getRoleUsers)
        {

            while (dr.Read())
            {
                int rid = dr.GetInt16("ROLE_ID");
                string rn = dr.GetString("ROLE_NAME");
                string coeidentifier = dr.GetString("COEIDENTIFIER").ToUpper();
                //string pn = dr.GetString("PRIVILEGE_TABLE_NAME");
                COEUserBOList roleUsers = null;
                COEPrivilegeBOList privs = null;
                COERoleBOList roleRoles = null;
                if (getPrivileges) { privs = COEPrivilegeBOList.GetList(rn); }
                if (getRoleRoles) { roleRoles = COERoleBOList.GetRoleRoles(rn); }
                if (getRoleUsers) { roleUsers = COEUserBOList.GetListByRole(rn, false); }

                if (!Contains(rn))                           //CBOE-1793 COE Security-Add User-Observed available roles duplication for multiple times  ASV 05SEP13
                {
                COERoleBO coeRoleBO = new COERoleBO(
                 rid,
                 rn,
                 _privilegeTableName,
                 privs,
                 roleRoles,
                 roleUsers,
                 coeidentifier);
                this.Add(coeRoleBO);
            }
        }
        }

        protected override void DataPortal_Update()
        {
            try
            {
                RaiseListChangedEvents = false;
                if (_coeDAL == null) { LoadDAL(); }

                // Coverity Fix CID - 11623 
                if (_coeDAL != null)
                {
                    foreach (COERoleBO item in DeletedList)
                        item.DeleteSelf(_coeDAL);
                    DeletedList.Clear();

                    foreach (COERoleBO item in this)
                        if (item.IsNew)
                            item.Insert(_coeDAL);
                        else
                            item.Update(_coeDAL);
                }
                else
                    throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));
            
                RaiseListChangedEvents = true;
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void LoadDAL()
        {
            //Load DAL for database calls. The database is actually alwas set to the centralizedDataStorageDB in the resources. However, I'm leaving
            //the code untouced just in case we need to go back to non-centralized.
            if (_dalFactory == null) { _dalFactory = new DALFactory(); }
            _dalFactory.GetDAL<DAL>(ref _coeDAL, _serviceName, COEDatabaseName.Get().ToString(), true);
        }
    }
}
