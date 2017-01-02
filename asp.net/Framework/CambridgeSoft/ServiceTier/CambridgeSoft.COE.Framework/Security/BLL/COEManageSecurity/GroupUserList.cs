using System;
using System.Data;
using Csla;
using Csla.Data;
using Csla.Validation;
using System.Collections.Generic;
using System.Configuration;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.COELoggingService;
using CambridgeSoft.COE.Framework.Properties;

namespace CambridgeSoft.COE.Framework.COESecurityService
{

    [Serializable()]
    public class GroupUserList : ReadOnlyListBase<GroupUserList, GroupUser>
    {
        #region member variables

        RoleList _groupGrantedRoles = null;
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

        public static bool CanGetObject()
        {
            // TODO: customize to check user role
            //return ApplicationContext.User.IsInRole("");
            return true;
        }

        #endregion

        #region Factory Methods
        public GroupUser GetUserByID(int personID)
        {
            foreach (GroupUser user in this)
                if (user.PersonID == personID)
                    return user;
            return null;
        }

        public GroupUser GetUserByUserID(string userID)
        {
            foreach (GroupUser user in this)
                if (user.UserID == userID)
                    return user;
            return null;
        }


        public void AddUser(GroupUser user)
        {
            if (!Contains(user.PersonID))
            {
                this.AllowNew = true;
                this.IsReadOnly = false;
                this.Add(user);
            }
            else
            {
                throw new InvalidOperationException("user already exists");
            }

        }

        public void RemoveUser(GroupUser user)
        {
            foreach (GroupUser hasUser in this)
            {
                if (hasUser.PersonID == user.PersonID)
                {
                    this.AllowRemove = true;
                    this.IsReadOnly = false;
                    Remove(user);
                    break;
                }
            }
        }

        public bool Contains(int personID)
        {
            bool boolReturn = false;
            foreach (GroupUser hasUser in this)
            {
                if (hasUser.PersonID == personID)
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

        public static GroupUserList Get(int groupID)
        {
            SetDatabaseName();
            return DataPortal.Fetch<GroupUserList>(new Criteria(groupID));
        }




        public GroupUserList()
        { /* require use of factory methods */ }

        #endregion

       

        

        #region Data Access

        [Serializable()]
        private class Criteria
        {
            internal int _groupID;
           
            public Criteria(int groupID)
            {
                _groupID = groupID;
                
            }
        }
        private void DataPortal_Fetch(Criteria criteria)
        {

            RaiseListChangedEvents = false;
            IsReadOnly = false;
            if (_coeDAL == null) { LoadDAL(); }
             // Coverity Fix CID - 11635
            if (_coeDAL != null)
            {
                DataTable dt = null;
                dt = _coeDAL.GetGroupUsers(criteria._groupID);
                Fetch(dt, criteria._groupID);
            }
            else
                throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));

            IsReadOnly = true;
            RaiseListChangedEvents = true;
        }


        #endregion

        protected void Fetch(DataTable dt, int groupID)
        {

            
                try
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        DataRow dr = dt.Rows[i];
                        int supervisorID = 0;
                        int siteID = 0;
                        int personID = System.Convert.ToInt32(dt.Rows[i]["PERSON_ID"]);
                        if (_groupGrantedRoles == null)
                        { //you only need to get this one time
                            _groupGrantedRoles = RoleList.GetGroupRoleList(groupID);
                        }
                        if (dt.Rows[i]["SUPERVISOR_INTERNAL_ID"] == System.DBNull.Value)
                        {
                            supervisorID = 0;
                        }
                        else
                        {
                            supervisorID = System.Convert.ToInt32(dt.Rows[i]["SUPERVISOR_INTERNAL_ID"]);
                        }

                        if (dt.Rows[i]["SITE_ID"] == System.DBNull.Value)
                        {
                            siteID = 0;
                        }
                        else
                        {
                            siteID = System.Convert.ToInt32(dt.Rows[i]["SITE_ID"]);
                        }


                        RoleList directGrantedRoles = RoleList.GetGroupPersonDirectRoleList(groupID, personID);
                        RoleList directAvailableRoles = RoleList.GetGroupPersonDirectAvailableRoleList(personID);
                        GroupUser GroupUser = new GroupUser(
                            personID,
                            System.Convert.ToString(dt.Rows[i]["USER_CODE"]),
                            System.Convert.ToString(dt.Rows[i]["USER_ID"]),
                            supervisorID,
                            System.Convert.ToString(dt.Rows[i]["TITLE"]),
                            System.Convert.ToString(dt.Rows[i]["FIRST_NAME"]),
                            System.Convert.ToString(dt.Rows[i]["MIDDLE_NAME"]),
                            System.Convert.ToString(dt.Rows[i]["LAST_NAME"]),
                            siteID,
                            System.Convert.ToString(dt.Rows[i]["DEPARTMENT"]),
                            System.Convert.ToString(dt.Rows[i]["TELEPHONE"]),
                            System.Convert.ToString(dt.Rows[i]["EMAIL"]),
                            System.Convert.ToString(dt.Rows[i]["INT_ADDRESS"]),
                            System.Convert.ToBoolean(dt.Rows[i]["ACTIVE"]),
                             _groupGrantedRoles,
                             directGrantedRoles,
                             directAvailableRoles,
                             System.Convert.ToBoolean(dt.Rows[i]["IS_LEADER"]));
                        this.Add(GroupUser);
                    }
                }
                catch (Exception e)
                {

                    throw e;
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


