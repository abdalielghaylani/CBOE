using System;
using System.Collections.Generic;
using System.Data;
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
    public class COEGroupBO : BusinessBase<COEGroupBO>
    {
        #region member variables

        // TODO: add your own fields, properties and methods
        private int _groupID;
        private int _groupOrgID;
        private string _groupName = string.Empty;
        private int _parentGroupID;
        private int _leaderPersonID;
        private GroupUserList _userList = null;
        private RoleList _privSets = null;
        private RoleList _availPrivSets = null;
        private String _privSetsString = string.Empty;
        private String _userListString = string.Empty;





        //variables data access
        [NonSerialized]
        private DAL _coeDAL = null;
        [NonSerialized]
        private DALFactory _dalFactory = new DALFactory();
        private string _serviceName = "COESecurity";

        [NonSerialized]
        static COELog _coeLog = COELog.GetSingleton("COESecurity");

        #endregion

        #region constructors


        internal COEGroupBO(int groupID, int groupOrgID, string groupName, int parentGroupID, int leaderPersonID, GroupUserList userList, RoleList privSets, RoleList availPrivSets)
        {
            _groupID = groupID;
            _groupOrgID = groupOrgID;
            _groupName = groupName;
            _parentGroupID = parentGroupID;
            _leaderPersonID = leaderPersonID;
            _userList = userList;
            _privSets = privSets;
            _availPrivSets = availPrivSets;

        }




        #endregion

        #region properties


        public int GroupID
        {
            get { return _groupID; }
            set
            {
                CanWriteProperty(true);
                if (!_groupID.Equals(value))
                {
                    _groupID = value;
                    PropertyHasChanged();
                }
            }
        }

        public int GroupOrgID
        {
            get { return _groupOrgID; }
            set
            {
                CanWriteProperty(true);
                if (!_groupOrgID.Equals(value))
                {
                    _groupOrgID = value;
                    PropertyHasChanged();
                }
            }
        }

        public int ParentGroupID
        {
            get { return _parentGroupID; }
            set
            {
                CanWriteProperty(true);
                if (!_parentGroupID.Equals(value))
                {
                    _parentGroupID = value;
                    PropertyHasChanged();
                }
            }
        }

        public int LeaderPersonID
        {
            get { return _leaderPersonID; }
            set
            {
                CanWriteProperty(true);
                if (!_leaderPersonID.Equals(value))
                {
                    _leaderPersonID = value;
                    PropertyHasChanged();
                }
            }
        }


        public string GroupName
        {
            get { return _groupName; }
            set
            {
                CanWriteProperty(true);
                if (value == null) value = string.Empty;
                if (!_groupName.Equals(value))
                {
                    _groupName = value;
                    PropertyHasChanged();
                }
            }
        }

        public GroupUserList UserList
        {
            get { return _userList; }
            set
            {
                CanWriteProperty(true);
                _userList = value;
                PropertyHasChanged();
            }
        }

        public RoleList PrivSets
        {
            get { return _privSets; }
            set
            {
                CanWriteProperty(true);
                _privSets = value;
                PropertyHasChanged();
            }
        }

        public String PrivSetsString
        {
            get
            {



                return _privSetsString;
            }
            set
            {
                CanWriteProperty(true);
                _privSetsString = value;
                PropertyHasChanged();
            }
        }


        public String UserListString
        {
            get { return _userListString; }
            set
            {
                CanWriteProperty(true);
                _userListString = value;
                PropertyHasChanged();
            }
        }
        public RoleList AvailablePrivSets
        {
            get { return _availPrivSets; }
            set
            {
                CanWriteProperty(true);
                _availPrivSets = value;
                PropertyHasChanged();
            }
        }

        protected override object GetIdValue()
        {
            return _groupID;
        }



        #endregion

        #region Factory Methods
        //this method must be called prior to any other method inorder to set the database that the dal will use
        internal static void SetDatabaseName()
        {
            COEDatabaseName.Set(Resources.CentralizedStorageDB);
        }


        internal static void SetDatabaseName(string databaseName)
        {
            COEDatabaseName.Set(Resources.CentralizedStorageDB);

        }


        public static COEGroupBO Get(int groupID)
        {
            SetDatabaseName();
            return DataPortal.Fetch<COEGroupBO>(new Criteria(groupID));
        }

        public static COEGroupBO SetGroupUsers(int groupID, string userString)
        {

            try
            {
                SetGroupUsersCommand result;
                result = DataPortal.Execute<SetGroupUsersCommand>(new SetGroupUsersCommand(groupID, userString));
                return COEGroupBO.Get(groupID);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }



        private COEGroupBO()
        { /* require use of factory methods */ }


        public static COEGroupBO New()
        {
            if (!CanAddObject())
                throw new System.Security.SecurityException(
                  "User not authorized to add a role");
            return DataPortal.Create<COEGroupBO>();
        }

        public static void Delete(int groupID)
        {

            SetDatabaseName();
            if (!CanDeleteObject())
                throw new System.Security.SecurityException(Resources.UserNotAuthorizedForDeleteObject + " COEUserBO");
            DataPortal.Delete(new Criteria(groupID));
        }


        public override COEGroupBO Save()
        {
            SetDatabaseName();
            if (IsDeleted && !CanDeleteObject())
                throw new System.Security.SecurityException(Resources.UserNotAuthorizedForDeleteObject + " COEUserBO");
            else if (IsNew && !CanAddObject())
                throw new System.Security.SecurityException(Resources.UserNotAuthorizedForAddObject + " COEUserBO");
            else if (!CanEditObject())
                throw new System.Security.SecurityException(Resources.UserNotAuthorizedForEditObject + " COEUserBO");
            ValidationRules.CheckRules();
            return base.Save();
        }


        #endregion

        #region Validation Rules

        protected override void AddBusinessRules()
        {
            ValidationRules.AddRule(Csla.Validation.CommonRules.StringRequired, "GroupName");
            ValidationRules.AddRule(Csla.Validation.CommonRules.StringMaxLength, new Csla.Validation.CommonRules.MaxLengthRuleArgs("GroupName", 20));
            ValidationRules.AddRule(Csla.Validation.CommonRules.IntegerMinValue, new Csla.Validation.CommonRules.IntegerMinValueRuleArgs("LeaderPersonID", 1));

        }



        #endregion

        #region Authorization Rules


        #endregion

        #region Authorization Rules

        public static bool CanAddObject()
        {
            return true;
            //return Csla.ApplicationContext.User.IsInRole("Administrator");
        }

        public static bool CanGetObject()
        {
            return true;
        }

        public static bool CanDeleteObject()
        {
            return true;

            //return Csla.ApplicationContext.User.IsInRole("Administrator");
        }

        public static bool CanEditObject()
        {
            return true;
            //return Csla.ApplicationContext.User.IsInRole("Administrator");
        }

        #endregion

        #region Criteria

        [Serializable()]
        private class Criteria
        {

            internal int _groupID;


            //constructors
            public Criteria(int groupID)
            {
                _groupID = groupID;

            }
        }


        #endregion //Criteria

        #region data access


        [RunLocal()]
        protected override void DataPortal_Create()
        {
            //_roleID = 0;
            //_roleName = string.Empty;
            this.MarkNew();
            //for now hard code to 1
            _availPrivSets = RoleList.GetGroupRoleAvailableList(1);
            //_availPrivSets = RoleList.GetGroupRoleAvailableList(_groupID);

            //ValidationRules.CheckRules();
        }
        private void DataPortal_Fetch(Criteria criteria)
        {
            _coeLog.LogStart(string.Empty, 1);
            if (_coeDAL == null) { LoadDAL(); }
            // Coverity Fix CID - 11608
            if (_coeDAL != null)
            {
                DataTable dt = _coeDAL.GetGroup(criteria._groupID);
                FetchObject(dt);
            }
            else
                throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));

            _coeLog.LogEnd(string.Empty, 1);
        }

        private void FetchObject(DataTable dt)
        {
            try
            {

                _groupID = System.Convert.ToInt32(dt.Rows[0]["GROUP_ID"]);
                _groupName = System.Convert.ToString(dt.Rows[0]["GROUP_NAME"]);
                _groupOrgID = System.Convert.ToInt32(dt.Rows[0]["GROUPORG_ID"]);
                if (dt.Rows[0]["PARENT_GROUP_ID"] == System.DBNull.Value)
                {
                    _parentGroupID = 0;
                }
                else
                {
                    _parentGroupID = System.Convert.ToInt32(dt.Rows[0]["PARENT_GROUP_ID"]);

                }
                if (dt.Rows[0]["LEADER_PERSON_ID"] == System.DBNull.Value)
                {
                    _leaderPersonID = 0;
                }
                else
                {
                    _leaderPersonID = System.Convert.ToInt32(dt.Rows[0]["LEADER_PERSON_ID"]);

                }
                _userList = GroupUserList.Get(_groupID);
                _privSets = RoleList.GetGroupRoleList(_groupID);
                _availPrivSets = RoleList.GetGroupRoleAvailableList(_groupID);

                String returnList = string.Empty;
                if (this._privSets != null)
                {
                    foreach (Csla.NameValueListBase<string, string>.NameValuePair nameValuePair in this._privSets)
                    {

                        returnList = returnList + nameValuePair.Key.ToString() + ",";
                    }
                }
                _privSetsString = returnList;

                String returnUserList = string.Empty;
                if (this._userList != null)
                {
                    foreach (GroupUser user in _userList)
                    {
                        if (returnUserList.Length > 0)
                        {
                            returnUserList = returnUserList + "," + user.PersonID.ToString();
                        }
                        else
                        {
                            returnUserList = user.PersonID.ToString();
                        }
                    }
                }
                _userListString = returnUserList;

            }

            catch (System.Exception ex)
            {

            }

        }

        protected override void DataPortal_Update()
        {
            if (_coeDAL == null) { LoadDAL(); }
            // Coverity Fix CID - 11610 
            if (_coeDAL != null)
                Update(_coeDAL);
            else
                throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));
        }

        internal void Update(DAL coeDAL)
        {
            string usersID = this.UserListString + "," + this.LeaderPersonID;
            int[] userList = CreatePersonIDString(this.UserList, usersID);
            int[] privSetList = CreatePrivSetString(this.PrivSets, this.PrivSetsString);          
            coeDAL.UpdateGroup(this._groupID, this._groupName, this._parentGroupID, this._leaderPersonID, userList, privSetList);          
        }

        private int[] CreatePersonIDString(GroupUserList UserList, string userListString)
        {
            int[] newList = null;
            if (UserList != null || userListString != String.Empty)
            {
                if (userListString.Length > 0 && userListString != string.Empty)
                {
                    string theString = userListString.TrimEnd(',');
                    string[] stringArray = theString.Split(',');
                    newList = new int[stringArray.Length];
                    for (int i = 0; i < stringArray.Length; i++)
                    {
                        newList[i] = System.Convert.ToInt32(stringArray[i]);
                    }
                }
                else
                {
                    // Coverity Fix CID - 19044 
                    if (UserList != null && UserList.Count > 0)
                    {
                        newList = new int[UserList.Count];
                        for (int i = 0; i < UserList.Count; i++)
                        {
                            newList[i] = System.Convert.ToInt32(UserList[i].PersonID);
                        }
                    }
                }

            }
            return newList;

        }

        private int[] CreatePrivSetString(RoleList privSetList, string privSetsString)
        {
            int[] newList = null;
            if (privSetList != null || privSetsString != String.Empty)
            {
                if (privSetsString.Length > 0)
                {//use the string instead 
                    string theString = privSetsString.TrimEnd(',');
                    string[] stringArray = theString.Split(',');
                    newList = new int[stringArray.Length];
                    for (int i = 0; i < stringArray.Length; i++)
                    {
                        if (stringArray[i]!="")
                        newList[i] = System.Convert.ToInt32(stringArray[i]);
                    }
                }
                else
                {
                    // Coverity Fix CID - 19045 
                    if (privSetList != null && privSetList.Count > 0)
                    {
                        newList = new int[privSetList.Count];
                        for (int i = 0; i < privSetList.Count; i++)
                        {
                            newList[i] = System.Convert.ToInt32(privSetList[i].Key);
                        }
                    }
                }
            }
            return newList;

        }

        protected override void DataPortal_Insert()
        {
            if (_coeDAL == null) { LoadDAL(); }
            // Coverity Fix CID - 11609 
            if (_coeDAL != null)
                Insert(_coeDAL);
            else
                throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));        
        }

        internal void Insert(DAL coeDAL)
        {
            //int[] userList = CreatePersonIDString(this.UserList, this.UserList);
            int[] userList = CreatePersonIDString(this.UserList, this.LeaderPersonID.ToString());
            int[] privSetList = CreatePrivSetString(this.PrivSets, this.PrivSetsString);
            //userList[0] = this.LeaderPersonID;
            int groupID = coeDAL.AddGroup(this._groupName, this._groupOrgID, this._parentGroupID, this._leaderPersonID, userList, privSetList);
            _groupID = groupID;
        }

        private void DataPortal_Delete(Criteria criteria)
        {

            if (_coeDAL == null) { LoadDAL(); }
            try
            {
                // Coverity Fix CID - 11606
                if (_coeDAL != null)
                    _coeDAL.DeleteGroup(criteria._groupID);
                else
                    throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));            
            }
            catch (Exception)
            {
                throw;
            }
        }


        protected override void DataPortal_DeleteSelf()
        {
            if (_coeDAL == null) { LoadDAL(); }
            // Coverity Fix CID - 11607 
            if (_coeDAL != null)
                DeleteSelf(_coeDAL);
            else
                throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));
        }


        internal void DeleteSelf(DAL coeDAL)
        {
            coeDAL.DeleteGroup(this._groupID);
        }



        private void LoadDAL()
        {
            if (_dalFactory == null) { _dalFactory = new DALFactory(); }
            _dalFactory.GetDAL<DAL>(ref _coeDAL, _serviceName, COEDatabaseName.Get().ToString(), true);
        }



        #endregion




        #region command objectss

        [Serializable]
        private class SetGroupUsersCommand : CommandBase
        {
            private int _groupID = -1;
            private string _usersToAdd = string.Empty;
            
            
            [NonSerialized]
            private DAL _coeDAL = null;
            [NonSerialized]
            private DALFactory _dalFactory = new DALFactory();
            private string _serviceName = "COESecurity";

            private bool _success = false;

            public SetGroupUsersCommand(int groupID, string usersToAdd)
            {
                _groupID = groupID;
                _usersToAdd = usersToAdd;

            }
            


            public SetGroupUsersCommand()
            {


            }

            public bool Success
            {
                get { return _success; }
                set { _success = value; }
            }

            protected override void DataPortal_Execute()
            {
                try
                {
                    COEDatabaseName.Set(Resources.CentralizedStorageDB);
                    if (_coeDAL == null) { LoadDAL(); }
                    // Coverity Fix CID - 11605 
                    if (_coeDAL != null)
                    {
                        try
                        {
                            _coeDAL.SetGroupUsers(_groupID, CreatePersonArray(_usersToAdd));
                            Success = true;
                        }
                        catch (System.Exception ex)
                        {
                            throw;
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


            private void LoadDAL()
            {
                if (_dalFactory == null) { _dalFactory = new DALFactory(); }
                _dalFactory.GetDAL<DAL>(ref _coeDAL, _serviceName, COEDatabaseName.Get().ToString(), true);
            }

            private int[] CreatePersonArray(string userListString)
            {
                int[] newList = null;
                if ( userListString != String.Empty)
                {


                    if (userListString.Length > 0 && userListString != string.Empty)
                    {
                        string[] stringArray = userListString.Split(',');
                        newList = new int[stringArray.Length];
                        for (int i = 0; i < stringArray.Length; i++)
                        {
                            newList[i] = System.Convert.ToInt32(stringArray[i]);
                        }

                    }


                }

                return newList;
            }
        }
        #endregion

    }
}
