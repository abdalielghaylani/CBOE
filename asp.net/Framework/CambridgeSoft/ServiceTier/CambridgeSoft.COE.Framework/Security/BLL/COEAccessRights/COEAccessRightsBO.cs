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
    public class COEAccessRightsBO : Csla.BusinessBase<COEAccessRightsBO>
    {
        #region member variables

        // TODO: add your own fields, properties and methods
        private string _serviceName;
        private int _objectID;
        private ObjectTypes _objectType;
        //variables data access
        [NonSerialized]
        private DAL _coeDAL = null;
        [NonSerialized]
        private DALFactory _dalFactory = new DALFactory();
        private COERoleReadOnlyBOList _roles = new COERoleReadOnlyBOList();
        private COEUserReadOnlyBOList _users = new COEUserReadOnlyBOList();


        [NonSerialized]
        static COELog _coeLog = COELog.GetSingleton("COESecurity");

        #endregion

        #region public enums

        [Serializable()]
        public enum ObjectTypes
        {
            COEDATAVIEW,
            COEFORM,
            COEGENERICOBJECT,
            COEHITLIST,
            COESEARCHCRITERIA
        }


        #region constructors
        //constructor to be called from  as well as any other services that needs to construct this object
        public COEAccessRightsBO(ObjectTypes objectType, int objectID)
        {
            _objectType = objectType;
            _objectID = objectID;
            
        }

        public COEAccessRightsBO(COEUserReadOnlyBOList usersList, COERoleReadOnlyBOList rolesList)
        {
            _users = usersList;
            _roles = rolesList;

        }
        

        #endregion

        #region properties

        public COEUserReadOnlyBOList Users
        {
            get { return _users; }
            set
            {
                _users = value;
            MarkDirty();
                }
        }



        public COERoleReadOnlyBOList Roles
        {
            get { return _roles; }
            set{_roles = value;
            MarkDirty();
                }
        }

        public ObjectTypes ObjectType
        {
            get { return _objectType; }
            set
            {
                _objectType = value;
                MarkDirty();
            }
        }

        public int ObjectID
        {
            get { return _objectID; }
            set
            {
                _objectID = value;
                MarkDirty();
            }
        }
        

        #endregion

        #region Factory Methods
        protected override object GetIdValue()
        {
            return _objectID;
        }

        //this method must be called prior to any other method inorder to set the database that the dal will use
        public static void SetDatabaseName()
        {
            COEDatabaseName.Set(DALUtils.GetDefaultQualifyDbName(Resources.CentralizedStorageDB));
        }


        public static void SetDatabaseName(string databaseName)
        {
            COEDatabaseName.Set(DALUtils.GetDefaultQualifyDbName(Resources.CentralizedStorageDB));

        }

        public static COEAccessRightsBO Get(ObjectTypes objectType, int objectID)
        {
            SetDatabaseName();
            return DataPortal.Fetch<COEAccessRightsBO>(new Criteria(objectType, objectID));
        }

        public static COEAccessRightsBO New()
        {
            SetDatabaseName();

            return DataPortal.Create<COEAccessRightsBO>(new CreateNewCriteria());
        }



        public static void Delete(ObjectTypes objectType, int objectID)
        {
            SetDatabaseName();

            if (!CanDeleteObject())
                throw new System.Security.SecurityException(Resources.UserNotAuthorizedForDeleteObject + " COEAccessRightsBO");
            DataPortal.Delete(new Criteria(objectType, objectID));
        }

        public override COEAccessRightsBO Save()
        {
            if (IsDeleted && !CanDeleteObject())
                throw new System.Security.SecurityException(Resources.UserNotAuthorizedForDeleteObject + " COEAccessRightsBO");
            else if (IsNew && !CanAddObject())
                throw new System.Security.SecurityException(Resources.UserNotAuthorizedForAddObject + " COEAccessRightsBO");
            else if (!CanEditObject())
                throw new System.Security.SecurityException(Resources.UserNotAuthorizedForEditObject + " COEAccessRightsBO");

            return base.Save();
        }

        internal static bool CanAddObject()
        {
            // return Csla.ApplicationContext.User.IsInRole("CanSearch");
            return true;
        }

        internal static bool CanGetObject()
        {
            // return Csla.ApplicationContext.User.IsInRole("CanSearch");
            return true;
        }

        internal static bool CanEditObject()
        {
            // return Csla.ApplicationContext.User.IsInRole("CanSearch");
            return true;
        }

        internal static bool CanDeleteObject()
        {
            // return Csla.ApplicationContext.User.IsInRole("CanSearch");
            return true;
        }

        internal COEAccessRightsBO()
        { /* require use of factory methods */ }



        #endregion
        #region Criteria

        [Serializable()]
        private class Criteria
        {

            internal int _objectID;
            internal ObjectTypes _objectType;

            //constructors
            public Criteria(ObjectTypes objectType, int objectID)
            {
                _objectType = objectType;
                _objectID = objectID;
            }
        }

        [Serializable()]
        private class CreateNewCriteria
        {

            
        }
        #endregion //Criteria

        #region data access
        private void DataPortal_Fetch(Criteria criteria)
        {
            _coeLog.LogStart("Fetching access rights", 1);
            SafeDataReader dr = null;
            SafeDataReader dr2 = null;
            if (_coeDAL == null) { LoadDAL(); }
            // Coverity Fix CID - 11594 
            if (_coeDAL != null)
            {
                using (dr = _coeDAL.GetAccessRights(criteria._objectType.ToString(), criteria._objectID, "USER"))
                {
                    BuildUsersFromReader(dr);
                }

                using (dr2 = _coeDAL.GetAccessRights(criteria._objectType.ToString(), criteria._objectID, "ROLE"))
                {
                    BuildRolesFromReader(dr2);
                }
            }
            else
                throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));

            _coeLog.LogEnd("Fetching access rights", 1);
        }

        private void BuildUsersFromReader(SafeDataReader dr)
        {
            List<int> personList = new List<int>();
            while (dr.Read())
            {
                try
                {
                    personList.Add(dr.GetInt16("PRINCIPALID"));
                }
                catch (Exception e)
                {

                    //Loopthrough
                }
            }

            COEUserReadOnlyBOList list = COEUserReadOnlyBOList.GetListByUserID(personList);
            //List<COEUserReadOnlyBO> userList = new List<COEUserReadOnlyBO>();
            //foreach (COEUserReadOnlyBO user in list)
            //{
            //    userList.Add(user);
            //}
            this.Users = list;

    
        }


        private void BuildRolesFromReader(SafeDataReader dr)
        {
            List<int> roleList = new List<int>();
            while (dr.Read())
            {
                try
                {
                    roleList.Add(dr.GetInt16("PRINCIPALID"));
                }
                catch (Exception e)
                {

                    //Loopthrough
                }
            }

            COERoleReadOnlyBOList list = COERoleReadOnlyBOList.GetListByRoleID(roleList);
            //List<COERoleReadOnlyBO> rolesList = new List<COERoleReadOnlyBO>();
            //foreach (COERoleReadOnlyBO role in list)
            //{
            //    rolesList.Add(role);
            //}
            this.Roles = list;

        }

#endregion 
        
        #region Data Access - Insert
        protected override void DataPortal_Insert()
        {
            if (_coeDAL == null) { LoadDAL(); }
            // Coverity Fix CID - 11595
            if (_coeDAL != null)
            {
                //delete them all
                _coeDAL.DeleteAccessPrivileges(this.ObjectID, this.ObjectType.ToString());
                if (this.Users != null)
                {
                    foreach (COEUserReadOnlyBO user in this.Users)
                    {
                        _coeDAL.InsertAccessPrivilege(this.ObjectID, this.ObjectType.ToString(), user.PersonID, "USER");
                    }
                }
                if (this.Roles != null)
                {
                    foreach (COERoleReadOnlyBO role in this.Roles)
                    {
                        _coeDAL.InsertAccessPrivilege(this.ObjectID, this.ObjectType.ToString(), role.RoleID, "ROLE");
                    }
                }

                MarkOld();
            }
            else
                throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));
        }
        #endregion //Data Access - insert

        #region Data Access - Update
        protected override void DataPortal_Update()
        {
            if (_coeDAL == null) { LoadDAL(); }
            // Coverity Fix CID - 11596
            if (_coeDAL != null)
            {
                //delete them all
                _coeDAL.DeleteAccessPrivileges(this.ObjectID, this.ObjectType.ToString());
                //insert them all. Not efficient but does the job for now
                if (this.Users != null)
                {
                    foreach (COEUserReadOnlyBO user in this.Users)
                        _coeDAL.InsertAccessPrivilege(this.ObjectID, this.ObjectType.ToString(), user.PersonID, "USER");
                }
                if (this.Roles != null)
                {
                    foreach (COERoleReadOnlyBO role in this.Roles)
                        _coeDAL.InsertAccessPrivilege(this.ObjectID, this.ObjectType.ToString(), role.RoleID, "ROLE");
                }

                MarkOld();
            }
            else
                throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));
        }
        #endregion //Data Access - Update

        #region Data Access - Delete
       
        protected override void DataPortal_DeleteSelf()
        {
            DataPortal_Delete(new Criteria(_objectType, _objectID));
        }

        private void DataPortal_Delete(Criteria criteria)
        {
            if (_coeDAL == null) { LoadDAL(); }
            // Coverity Fix CID - 11593 
            if (_coeDAL != null)
            {
                //delete them all
                _coeDAL.DeleteAccessPrivileges(criteria._objectID, criteria._objectType.ToString());
            }
            else
                throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));
        }
        #endregion //Data Access - Delete
        private void LoadDAL()
        {
            if (_dalFactory == null) { _dalFactory = new DALFactory(); }
            _dalFactory.GetDAL<DAL>(ref _coeDAL, "COESecurity", COEDatabaseName.Get().ToString(), true);
        }
        #endregion

    }

}
