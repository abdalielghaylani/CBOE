using System;
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
    public class COERoleBO : BusinessBase<COERoleBO>
    {
        #region member variables

        // TODO: add your own fields, properties and methods
        private int _roleID;
        private string _roleName = string.Empty;
        private string _privilegeTableName = string.Empty;
        private COEPrivilegeBOList _originalPrivileges = null;
        private COEPrivilegeBOList _availablePrivileges = null;
        private COEPrivilegeBOList _privileges = null;
        private COERoleBOList _roleRoles = null;
        private COERoleBOList _originalRoleRoles = null;
        private COEUserBOList _roleUsers = null;
        private COEUserBOList _originalRoleUsers = null;
        private bool _isDBMSRole = false;
        private string _coeIdentifier = string.Empty;
        public static bool roleUser = false;




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
        //constructor to be called from  as well as any other services that needs to construct this object
        internal COERoleBO(int roleID, string roleName, string privilegeTableName)
        {
            _roleID = roleID;
            _roleName = roleName;
            _privilegeTableName = privilegeTableName;

        }

        //constructor to be called from  as well as any other services that needs to construct this object
        internal COERoleBO(string roleName)
        {

            _roleName = roleName;


        }

        internal COERoleBO(int roleID, string roleName, string privilegeTableName, COEPrivilegeBOList privileges, COERoleBOList roleRoles, COEUserBOList roleUsers)
        {
            _roleID = roleID;
            _roleName = roleName;
            _privileges = privileges;
            _privilegeTableName = privilegeTableName;
            _roleUsers = roleUsers;
            _roleRoles = roleRoles;

        }

        internal COERoleBO(int roleID, string roleName, string privilegeTableName, COEPrivilegeBOList privileges, COERoleBOList roleRoles, COEUserBOList roleUsers, string COEIdentifier)
        {
            _roleID = roleID;
            _roleName = roleName;
            _privileges = privileges;
            _privilegeTableName = privilegeTableName;
            _roleUsers = roleUsers;
            _roleRoles = roleRoles;
            _coeIdentifier = COEIdentifier;

        }

        #endregion

        #region properties


        public int RoleID
        {
            get { return _roleID; }
            set
            {
                CanWriteProperty(true);
                if (!_roleID.Equals(value))
                {
                    _roleID = value;
                    PropertyHasChanged();
                }
            }
        }

        public bool IsDBMSRole
        {
            get { return _isDBMSRole; }
            set
            {
                CanWriteProperty(true);
                if (!_isDBMSRole.Equals(value))
                {
                    _isDBMSRole = value;
                    PropertyHasChanged();
                }
            }
        }
        public string RoleName
        {
            get { return _roleName; }
            set
            {
                CanWriteProperty(true);
                if (value == null) value = string.Empty;
                if (!_roleName.Equals(value))
                {
                    _roleName = value;
                    PropertyHasChanged();
                }
            }
        }

        public string COEIdentifier
        {
            get { return _coeIdentifier; }
            set
            {
                CanWriteProperty(true);
                if (value == null) value = string.Empty;
                if (!_coeIdentifier.Equals(value))
                {
                    _coeIdentifier = value;
                    PropertyHasChanged();
                }
            }
        }

        internal string PrivilegeTableName
        {
            get { return _privilegeTableName; }
            set
            {
                CanWriteProperty(true);
                if (value == null) value = string.Empty;
                if (!_privilegeTableName.Equals(value))
                {
                    _privilegeTableName = value;
                    PropertyHasChanged();
                }
            }
        }


        public COEPrivilegeBOList OriginalPrivileges
        {
            get { return _originalPrivileges; }
            set
            {
                CanWriteProperty(true);

                _originalPrivileges = value;

                PropertyHasChanged();
            }
        }
        public COEPrivilegeBOList Privileges
        {
            get { return _privileges; }
            set
            {
                CanWriteProperty(true);

                _privileges = value;

                PropertyHasChanged();
            }
        }

        public COERoleBOList RoleRoles
        {
            get { return _roleRoles; }
            set
            {
                CanWriteProperty(true);

                _roleRoles = value;

                PropertyHasChanged();
            }
        }

        public COERoleBOList OriginalRoleRoles
        {
            get { return _roleRoles; }
            set
            {
                CanWriteProperty(true);

                _originalRoleRoles = value;

                PropertyHasChanged();
            }
        }

        public COEUserBOList RoleUsers
        {
            get { return _roleUsers; }
            set
            {
                CanWriteProperty(true);

                _roleUsers = value;

                PropertyHasChanged();
            }
        }


        protected override object GetIdValue()
        {
            return _roleID;
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


        public static COERoleBO Get(string roleName)
        {
            SetDatabaseName();
            return DataPortal.Fetch<COERoleBO>(new Criteria(roleName, false, false, false));
        }




        public static COERoleBO Get(string roleName, bool getPrivileges, bool getRoleRoles, bool getRoleUsers)
        {
            SetDatabaseName();
            return DataPortal.Fetch<COERoleBO>(new Criteria(roleName, getPrivileges, getRoleRoles, getRoleUsers));
        }

        private COERoleBO()
        { /* require use of factory methods */ }

        public static COERoleBO New(string appName)
        {
            COERoleBO roleBO = new COERoleBO();
            roleBO = DataPortal.Create<COERoleBO>(new PrivilegeCriteria(appName, roleBO));
            roleBO.MarkNew();
            return roleBO;
        }
        public static COERoleBO New()
        {
            if (!CanAddObject())
                throw new System.Security.SecurityException(
                  "User not authorized to add a role");
            return DataPortal.Create<COERoleBO>();
        }

        public static void Delete(string roleName)
        {

            SetDatabaseName();
            if (!CanDeleteObject())
                throw new System.Security.SecurityException(Resources.UserNotAuthorizedForDeleteObject + " COEUserBO");
            DataPortal.Delete(new CriteriaByName(roleName));
        }
        public void Delete()
        {

            SetDatabaseName();
            if (!CanDeleteObject())
                throw new System.Security.SecurityException(Resources.UserNotAuthorizedForDeleteObject + " COERoleBO");
            DataPortal.Delete(new CriteriaByName(this.RoleName));
        }


        public override COERoleBO Save()
        {
            SetDatabaseName();
            if (IsDeleted && !CanDeleteObject())
                throw new System.Security.SecurityException(Resources.UserNotAuthorizedForDeleteObject + " COERoleBO");
            else if (IsNew && !CanAddObject())
                throw new System.Security.SecurityException(Resources.UserNotAuthorizedForAddObject + " COERoleBO");
            else if (!CanEditObject())
                throw new System.Security.SecurityException(Resources.UserNotAuthorizedForEditObject + " COERoleBO");
            ValidationRules.CheckRules();
            return base.Save();
        }


        #endregion

        #region Validation Rules
        protected override void AddBusinessRules()
        {
            ValidationRules.AddRule(Csla.Validation.CommonRules.StringRequired, "RoleName");



        }




        #endregion

        #region Authorization Rules

        protected override void AddAuthorizationRules()
        {
            //AuthorizationRules.AllowWrite(
            //  "Id", "Administrator");
            //AuthorizationRules.AllowWrite(
            //  "Name", "Administrator");
        }


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

            internal string _roleName;
            internal bool _getPrivileges;
            internal bool _getRoleRoles;
            internal bool _getRoleUsers;

            //constructors
            public Criteria(string roleName, bool getPrivileges, bool getRoleRoles, bool getRoleUsers)
            {
                _roleName = roleName;
                _getPrivileges = getPrivileges;
                _getRoleRoles = getRoleRoles;
                _getRoleUsers = getRoleUsers;
            }
        }

        [Serializable()]
        private class CriteriaByName
        {

            internal string _roleName;

            //constructors
            public CriteriaByName(string roleName)
            {
                _roleName = roleName;
            }
        }
        [Serializable()]
        protected class PrivilegeCriteria
        {
            internal string _appName;
            internal COERoleBO _roleBO;
            //constructors
            public PrivilegeCriteria(string appName, COERoleBO roleBO)
            {
                _appName = appName;
                _roleBO = roleBO;
            }
        }
        #endregion //Criteria

        #region data access


        [RunLocal()]
        protected override void DataPortal_Create()
        {
            _roleID = 0;
            _roleName = string.Empty;
            this.MarkNew();

            //ValidationRules.CheckRules();
        }

        protected void DataPortal_Create(PrivilegeCriteria criteria)
        {
            _roleID = 0;
            _roleName = string.Empty;
            _privileges = COEPrivilegeBOList.GetDefaultListForApp(criteria._appName);
            _coeIdentifier = criteria._appName;
            this.MarkNew();
        }

        private void DataPortal_Fetch(Criteria criteria)
        {
            _coeLog.LogStart(string.Empty, 1);
            if (_coeDAL == null) { LoadDAL(); }
            roleUser = criteria._getRoleUsers;
            // Coverity Fix CID - 11615 
            if (_coeDAL != null)
            {
                using (SafeDataReader dr = _coeDAL.GetRole(criteria._roleName))
                {
                    FetchObject(dr, criteria._getPrivileges, criteria._getRoleRoles, criteria._getRoleUsers);
                }
            }
            else
                throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));

            _coeLog.LogEnd(string.Empty, 1);
        }

        private void FetchObject(SafeDataReader dr, bool getPrivileges, bool getRoleRoles, bool getRoleUsers)
        {
            if (dr.Read())
            {
                _roleID = dr.GetInt16("ROLE_ID");
                _roleName = dr.GetString("ROLE_NAME");
                _privilegeTableName = dr.GetString("PRIVILEGE_TABLE_NAME");
                _privileges = null;
                _roleRoles = null;
                _roleUsers = null;
                _availablePrivileges = null;
                _coeIdentifier = dr.GetString("COEIDENTIFIER").ToUpper();
                _isDBMSRole = false;
                if (getPrivileges)
                {
                    _privileges = COEPrivilegeBOList.GetList(dr.GetString("ROLE_NAME"));
                    _originalPrivileges = _privileges;
                }
                if (getRoleRoles)
                {
                    _roleRoles = COERoleBOList.GetRoleRoles(dr.GetString("ROLE_NAME"));
                    _originalRoleRoles = _roleRoles;
                }
                if (getRoleUsers)
                {
                    _roleUsers = COEUserBOList.GetListByRole(dr.GetString("ROLE_NAME"));
                    _originalRoleUsers = _roleUsers;
                }
            }
        }

        protected override void DataPortal_Update()
        {
            if (_coeDAL == null) { LoadDAL(); }
            // Coverity Fix CID - 11617 
            if (_coeDAL != null)
                Update(_coeDAL);
            else
                throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));
        }

        internal void Update(DAL coeDAL)
        {
            //only one of the lists, privileges roleroles or roleuser will/shoud be populated for an update
            //each describes a differnt user case in the manager roles interface
            if (this._privileges != null)
            {
                string privilegeValueList = GetPrivValueList(coeDAL.DALManager.DatabaseData.DBMSType, this._privileges);
                coeDAL.UpdateRole(this._roleName, this._privilegeTableName, privilegeValueList);

            }
            if (this._roleRoles != null)
            {   //figure out which roles are new and which are removed
                string rolesToGrant = GetRolesToGrant();
                string rolesToRevoke = GetRolesToRevoke();

                coeDAL.UpdateRolesGrantedToRole(this._roleName, rolesToGrant, rolesToRevoke);

            }
            if (this._roleUsers != null)
            {
                //figure out which users are new and which are removed
                string usersToGrant = GetUsersToGrant();
                string usersToRevoke = GetUsersToRevoke();
                coeDAL.UpdateUsersGrantedARole(this._roleName, usersToGrant, usersToRevoke);

            }
        }


        protected override void DataPortal_Insert()
        {
            if (_coeDAL == null) { LoadDAL(); }
            Insert(_coeDAL);
        }

        internal void Insert(DAL coeDAL)
        {

            string privilegeValueList = GetPrivValueList(coeDAL.DALManager.DatabaseData.DBMSType, this._privileges);
            if (this._privilegeTableName == string.Empty)
            {
                this._privilegeTableName = coeDAL.GetPrivilegeTableName(this.COEIdentifier);
            }

            coeDAL.CreateRole(this._roleName, this._privilegeTableName, this._isDBMSRole, privilegeValueList, this.COEIdentifier);
        }


        private void DataPortal_Delete(CriteriaByName criteria)
        {
            COERoleBO role = COERoleBO.Get(criteria._roleName);
            if (_coeDAL == null) { LoadDAL(); }
            try
            {
                // Coverity Fix CID - 11613 
                if (_coeDAL != null)
                    _coeDAL.DeleteRole(role._privilegeTableName, criteria._roleName);
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
            // Coverity Fix CID - 11614
            if (_coeDAL != null)
                DeleteSelf(_coeDAL);
            else
                throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));
        }


        internal void DeleteSelf(DAL coeDAL)
        {
            coeDAL.DeleteRole(this._privilegeTableName, this._roleName);
        }

        private void LoadDAL()
        {
            if (_dalFactory == null) { _dalFactory = new DALFactory(); }
            _dalFactory.GetDAL<DAL>(ref _coeDAL, _serviceName, COEDatabaseName.Get().ToString(), true);
        }


        private string GetPrivValueList(DBMSType dbmsType, COEPrivilegeBOList privileges)
        {
            string valueList = string.Empty;
            foreach (COEPrivilegeBO privilege in privileges)
            {
                if (valueList == string.Empty)
                {
                    if (privilege.NonPrivilegeName.Length > 0)
                    {
                        switch (dbmsType)
                        {
                            case DBMSType.ORACLE:
                                switch (privilege.NonPrivilegeName)
                                {
                                    case "System.DateTime":
                                        valueList = "sysdate";
                                        break;
                                    case "System.Int64":
                                        valueList = "''";
                                        break;
                                    default:
                                        valueList = "'" + Convert.ToString(privilege.NonPrivilegeValue) + "'";
                                        break;
                                }
                                break;
                        }

                    }
                    else
                    {
                        valueList = "'" + Convert.ToString(Convert.ToInt16(privilege.Enabled)) + "'";
                    }
                }
                else
                {
                    if (privilege.NonPrivilegeName.Length > 0)
                    {
                        switch (dbmsType)
                        {
                            case DBMSType.ORACLE:
                                switch (privilege.NonPrivilegeName)
                                {
                                    case "System.DateTime":
                                        valueList = valueList + ",sysdate";
                                        break;
                                    case "System.Int64":
                                        valueList = valueList + ",''";
                                        break;
                                    default:
                                        valueList = valueList + ",'" + Convert.ToString(privilege.NonPrivilegeValue) + "'";
                                        break;
                                }
                                break;
                        }

                    }

                    else
                    {
                        valueList = valueList + ",'" + Convert.ToInt16(privilege.Enabled) + "'";
                    }

                }


            }
            return valueList;
        }
        #endregion


        #region private methods

        private string GetRolesToGrant()
        {
            //first compare teh original list with the new list and remove items that are not found
            bool itemFound = true;
            string newRolesToGrant = string.Empty;
            if (_roleRoles != null)
            {
                foreach (COERoleBO role in _roleRoles)
                {
                    if (_originalRoleRoles != null)
                    {
                        if (!_originalRoleRoles.Contains(role.RoleName))
                        {
                            if (newRolesToGrant == string.Empty)
                            {
                                newRolesToGrant = role.RoleName;
                            }
                            else
                            {
                                newRolesToGrant = newRolesToGrant + "," + role.RoleName;
                            }
                        }
                    }
                    else
                    {
                        if (newRolesToGrant == string.Empty)
                        {
                            newRolesToGrant = role.RoleName;
                        }
                        else
                        {
                            newRolesToGrant = newRolesToGrant + "," + role.RoleName;
                        }
                    }
                }
            }
            else
            {
                newRolesToGrant = string.Empty;
            }
            return newRolesToGrant;
        }


        private string GetRolesToRevoke()
        {

            bool itemFound = true;
            string newRolesToRevoke = string.Empty;
            if (_originalRoleRoles != null)
            {
                foreach (COERoleBO originalRole in _originalRoleRoles)
                {
                    if (_roleRoles != null)
                    {
                        if (!_roleRoles.Contains(originalRole.RoleName))
                        {
                            if (newRolesToRevoke == string.Empty)
                            {
                                newRolesToRevoke = originalRole.RoleName;
                            }
                            else
                            {
                                newRolesToRevoke = newRolesToRevoke + "," + originalRole.RoleName;
                            }
                        }
                    }
                    else
                    {
                        if (newRolesToRevoke == string.Empty)
                        {
                            newRolesToRevoke = originalRole.RoleName;
                        }
                        else
                        {
                            newRolesToRevoke = newRolesToRevoke + "," + originalRole.RoleName;
                        }
                    }
                }
            }
            else
            {
                newRolesToRevoke = string.Empty;
            }
            return newRolesToRevoke;
        }


        private string GetUsersToGrant()
        {
            //first compare teh original list with the new list and remove items that are not found
            bool itemFound = true;
            string newUsersToGrant = string.Empty;

            if (_roleUsers != null)
            {
                foreach (COEUserBO user in _roleUsers)
                {
                    if (_originalRoleUsers != null)
                    {
                        if (!_originalRoleUsers.Contains(user.UserID))
                        {
                            if (newUsersToGrant == string.Empty)
                            {
                                newUsersToGrant = "\"" + user.UserID + "\"";
                            }
                            else
                            {
                                newUsersToGrant = newUsersToGrant + "," + "\"" + user.UserID + "\"";
                            }
                        }
                    }
                    else
                    {
                        if (newUsersToGrant == string.Empty)
                        {
                            newUsersToGrant = user.UserID;
                        }
                        else
                        {
                            newUsersToGrant = newUsersToGrant + "," + user.UserID;
                        }
                    }

                }
            }
            else
            {
                newUsersToGrant = string.Empty;
            }
            return newUsersToGrant;
        }
        private string GetUsersToRevoke()
        {
            //first compare teh original list with the new list and remove items that are not found
            bool itemFound = true;
            string newUsersToRevoke = string.Empty;

            if (_originalRoleUsers != null)
            {
                foreach (COEUserBO user in _originalRoleUsers)
                {
                    if (_roleUsers != null)
                    {
                        if (!_roleUsers.Contains(user.UserID))
                        {
                            if (newUsersToRevoke == string.Empty)
                            {
                                newUsersToRevoke = "\"" + user.UserID + "\"";
                            }
                            else
                            {
                                newUsersToRevoke = newUsersToRevoke + "," + "\"" + user.UserID + "\"";
                            }
                        }
                    }
                    else
                    {
                        if (newUsersToRevoke == string.Empty)
                        {
                            newUsersToRevoke = user.UserID;
                        }
                        else
                        {
                            newUsersToRevoke = newUsersToRevoke + "," + user.UserID;
                        }
                    }
                }
            }
            else
            {

            }
            return newUsersToRevoke;
        }

        #endregion
    }
}
