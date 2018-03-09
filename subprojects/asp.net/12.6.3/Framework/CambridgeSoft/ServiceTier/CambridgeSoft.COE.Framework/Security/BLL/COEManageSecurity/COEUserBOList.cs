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
    public class COEUserBOList :
      BusinessListBase<COEUserBOList, COEUserBO>
    {
        #region member variables
        //variables data access
        [NonSerialized]
        private DAL _coeDAL = null;
        [NonSerialized]
        private DALFactory _dalFactory = new DALFactory();
        private string _serviceName = "COESecurity";

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
        public COEUserBO GetUserByID(int personID)
        {
            foreach (COEUserBO user in this)
                if (user.PersonID == personID)
                    return user;
            return null;
        }

        public COEUserBO GetUserByUserID(string userID)
        {
            foreach (COEUserBO user in this)
                if (user.UserID == userID)
                    return user;
            return null;
        }


        public void AddUser(COEUserBO user)
        {
            if (!Contains(user.PersonID))
            {
               
                this.Add(user);
            }
            else
            {
                throw new InvalidOperationException("user already exists");
            }

        }

        public void RemoveUser(COEUserBO user)
        {
            foreach (COEUserBO hasUser in this)
            {
                if (hasUser.PersonID == user.PersonID)
                {
                    this.AllowRemove = true;
                   
                    Remove(user);
                    break;
                }
            }
        }
        /// <summary>
        /// check to see if a user is in the current list by personID
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public bool Contains(int personID)
        {
            bool boolReturn = false;
            foreach (COEUserBO hasUser in this)
            {
                if (hasUser.PersonID == personID)
                {
                    boolReturn = true;
                    break;
                }

            }
            return boolReturn;
        }


        /// <summary>
        /// check to see if a user is in the current list by user name (userID)
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public bool Contains(string userID)
        {
            bool boolReturn = false;
            foreach (COEUserBO hasUser in this)
            {
                if (hasUser.UserID == userID)
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

        public static COEUserBOList GetList()
        {
            SetDatabaseName();
            return DataPortal.Fetch<COEUserBOList>();
        }

        public static COEUserBOList GetListByApplication(List<string> applicationList)
        {
            SetDatabaseName();
            return DataPortal.Fetch<COEUserBOList>(new Criteria(FilterType.Application, applicationList, null));
        }

        public static COEUserBOList GetListByApplication(string applicationName)

        {
            List<string> applicationList = new List<string>();
            applicationList.Add(applicationName);
            SetDatabaseName();
            return DataPortal.Fetch<COEUserBOList>(new Criteria(FilterType.Application, applicationList, null));
        }

        public static COEUserBOList GetListByRole(List<string> roleList)
        {
            SetDatabaseName();
            return DataPortal.Fetch<COEUserBOList>(new Criteria(FilterType.Role, roleList, null));
        }

        
        public static COEUserBOList GetListByRole(string roleName)
        {
            
            SetDatabaseName();
            return DataPortal.Fetch<COEUserBOList>(new CriteriaRoleGrantees(roleName));
        }

        internal static COEUserBOList GetListByRole(string roleName, bool getUserRoles)
        {
            if (getUserRoles == true)
            {
                List<string> roleList = new List<string>();
                roleList.Add(roleName);
                SetDatabaseName();
                return DataPortal.Fetch<COEUserBOList>(new Criteria(FilterType.Role, roleList, null));
            }
            else
            {
                SetDatabaseName();
                return DataPortal.Fetch<COEUserBOList>(new CriteriaRoleGrantees(roleName));
            }
        }

        public static COEUserBOList GetListByUserID(List<int> userIDList)
        {
            SetDatabaseName();
            return DataPortal.Fetch<COEUserBOList>(new Criteria(FilterType.User, null, userIDList));
        }

        public static COEUserBOList GetRoleAvailableUsers(COERoleBO role)
        {
            string roleName = role.RoleName;
            SetDatabaseName();
            return DataPortal.Fetch<COEUserBOList>(new CriteriaAvailUsers(null, roleName));
        }

        public static COEUserBOList NewList(List<String> useList)
        {
            COEUserBOList newList = new COEUserBOList();
            COEUserBOList newList2 = newList.BuildFromListItems(newList, useList);
            return newList2;
        }


        public COEUserBOList BuildFromListItems(COEUserBOList newList, List<string> users)
        {
            if (users != null)
            {
                foreach (string userName in users)
                {
                    COEUserBO coeUserBO = new COEUserBO(userName);
                    newList.Add(coeUserBO);
                }
            }
            else
            {
                newList = null;
            }

            return newList;
        }

        internal COEUserBOList()
        { /* require use of factory methods */ }

        #endregion

        #region public enums

        [Serializable()]
        private enum FilterType
        {
            Application,
            Role,
            User
        }
        #endregion

        #region Data Access

        [Serializable()]
        private class CriteriaAvailUsers
        {
            internal string _privTableName = string.Empty;
            internal string _roleName = string.Empty;
            public CriteriaAvailUsers(string privTableName, string roleName)
            {
                _privTableName = privTableName;
                _roleName = roleName;

            }

        }

        

        [Serializable()]
        private class CriteriaRoleGrantees
        {
            internal string _roleName = string.Empty;
            public CriteriaRoleGrantees(string roleName)
            {

                _roleName = roleName;

            }

        }

        [Serializable()]
        private class Criteria
        {
            private FilterType _filterType;
            private List<string> _filterList;
            private List<int> _intFilterList;
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
            public Criteria(FilterType filter, List<string> filterList, List<int> intFilterList)
            {
                _filterType = filter;
                _filterList = filterList;
                _intFilterList = intFilterList;
            }
        }
        private void DataPortal_Fetch()
        {
            try
            {
                RaiseListChangedEvents = false;
                if (_coeDAL == null) { LoadDAL(); }
                // Coverity Fix CID - 11628 
                if (_coeDAL != null)
                {
                    SafeDataReader dr = null;
                    dr = _coeDAL.GetAllUsers();
                    Fetch(dr);
                    foreach (COEUserBO user in this)
                    {
                        user.GetUserRoles();
                    }
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

       

        private void DataPortal_Fetch(CriteriaRoleGrantees criteria)
        {
            try
            {
                RaiseListChangedEvents = false;
                if (_coeDAL == null) { LoadDAL(); }
                 // Coverity Fix CID - 11631
                if (_coeDAL != null)
                {
                    DataTable dr = _coeDAL.GetRoleGrantees(criteria._roleName);
                    Fetch2(dr);
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

        private void DataPortal_Fetch(Criteria criteria)
        {

            try
            {
                RaiseListChangedEvents = false;
                if (_coeDAL == null) { LoadDAL(); }
                SafeDataReader dr = null;
                // Coverity Fix CID - 10439 (from local server)
                if (_coeDAL != null)
                {
                    switch (criteria.FilterType)
                    {
                        case FilterType.Application:
                            dr = _coeDAL.GetUsersByApplication(criteria.FilterList);
                            break;
                        case FilterType.Role:
                            dr = _coeDAL.GetUsersByRole(criteria.FilterList);
                            break;
                        case FilterType.User:
                            dr = _coeDAL.GetUsersByID(criteria.IntFilterList);
                            break;
                    }
                }
                if(dr != null)
                    Fetch(dr);
                foreach (COEUserBO user in this)
                {
                    user.GetUserRoles();
                }
                RaiseListChangedEvents = true;
            }
            catch (Exception)
            {
                
                throw;
            }
        }

        private void DataPortal_Fetch(CriteriaAvailUsers criteria)
        {
            try
            {
                RaiseListChangedEvents = false;
                if (_coeDAL == null) { LoadDAL(); }
                 // Coverity Fix CID - 11630 
                if (_coeDAL != null)
                {
                    DataTable dt = null;
                    dt = _coeDAL.GetRoleAvailableUsers(criteria._privTableName, criteria._roleName);
                    Fetch2(dt);
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
        #endregion

        protected void Fetch2(DataTable dt)
        {
            try
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {

                    DataRow dr = dt.Rows[i];
                    COEUserBO userBO = COEUserBO.Get(dr["USERNAME"].ToString());
                    if(!string.IsNullOrEmpty(userBO.UserID))
                        this.Add(userBO);


                }
            }
            catch (Exception)
            {
                
                throw;
            }
        }

        protected void Fetch(SafeDataReader dr)
        {

            try
            {
                while (dr.Read())
                {
                    try
                    {
                        COEUserBO coeUserBO = new COEUserBO(
                         dr.GetInt16("PERSON_ID"),
                         dr.GetString("USER_CODE"),
                         dr.GetString("USER_ID"),
                         dr.GetInt16("SUPERVISOR_INTERNAL_ID"),
                         dr.GetString("TITLE"),
                         dr.GetString("FIRST_NAME"),
                         dr.GetString("MIDDLE_NAME"),
                         dr.GetString("LAST_NAME"),
                         dr.GetInt16("SITE_ID"),
                         dr.GetString("DEPARTMENT"),
                         dr.GetString("TELEPHONE"),
                         dr.GetString("EMAIL"),
                         dr.GetInt16("ACTIVE").Equals("1"),
                        dr.GetString("INT_ADDRESS"));

                        this.Add(coeUserBO);
                    }
                    catch (Exception e)
                    {

                        //Loopthrough
                    }
                }
                dr.Close();

            }
            catch (Exception)
            {
                dr.Close();
                throw;
            }

        }
         protected override void DataPortal_Update()
        {
            try
            {
                RaiseListChangedEvents = false;
                if (_coeDAL == null) { LoadDAL(); }
                // Coverity Fix CID - 11632 
                if (_coeDAL != null)
                {
                    foreach (COEUserBO item in DeletedList)
                        item.DeleteSelf(_coeDAL);
                    DeletedList.Clear();

                    foreach (COEUserBO item in this)
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
