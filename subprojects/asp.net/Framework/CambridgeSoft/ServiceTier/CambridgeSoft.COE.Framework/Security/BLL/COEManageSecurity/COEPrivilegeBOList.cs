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
    public class COEPrivilegeBOList :
      BusinessListBase<COEPrivilegeBOList, COEPrivilegeBO>
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
        public COEPrivilegeBO GetPrivilegeByID(int privilegeID)
        {
            foreach (COEPrivilegeBO privilege in this)
                if (privilege.PrivilegeID == privilegeID)
                    return privilege;
            return null;
        }
        public COEPrivilegeBO GetPrivilegeByName(string privilegeName)
        {
            foreach (COEPrivilegeBO privilege in this)
                if (privilege.PrivilegeName == privilegeName)
                    return privilege;
            return null;
        }

    

        public COEPrivilegeBOList AddPrivilege(COEPrivilegeBO privilege)
        {
            if (!Contains(privilege.PrivilegeID))
            {
              
                this.Add(privilege);
                privilege.Dirty();

            }else
            {
                throw new InvalidOperationException("privilege already exists for role");
            }
            return this;
        }

        public COEPrivilegeBOList RemovePrivilege(COEPrivilegeBO privilege)
        {
            foreach (COEPrivilegeBO hasPrivilege in this)
            {
                if (hasPrivilege.PrivilegeID == privilege.PrivilegeID)
                {

                   
                    this.Remove(privilege);
                    privilege.Dirty();
                    privilege.Deleted();
                    
                    break;
                }
            }
            return this;
        }


      
        public bool Contains(int privilegeID)
        {
            bool boolReturn = false;
            foreach (COEPrivilegeBO hasPrivilege in this)
            {
                if (hasPrivilege.PrivilegeID == privilegeID)
                {
                    boolReturn =  true;
                    break;
                }
                
            }
            return boolReturn;
        }

        //this method must be called prior to any other method inorder to set the database that the dal will use
        internal static void SetDatabaseName()
        {
            COEDatabaseName.Set(Resources.CentralizedStorageDB);
        }


        internal static void SetDatabaseName(string databaseName)
        {
            COEDatabaseName.Set(Resources.CentralizedStorageDB);

        }

        public static COEPrivilegeBOList GetList(string roleName)
        {
            SetDatabaseName();
            return DataPortal.Fetch<COEPrivilegeBOList>(new CriteriaByRoleName(roleName));
        }

        

        public static COEPrivilegeBOList GetList(string roleName, bool retrieveGrantedPrivileges)
        {
            SetDatabaseName();
            return DataPortal.Fetch<COEPrivilegeBOList>(new Criteria(roleName, retrieveGrantedPrivileges));
        }

        public static COEPrivilegeBOList GetDefaultListForApp(string appName)
        {
            SetDatabaseName();
            return DataPortal.Fetch<COEPrivilegeBOList>(new CriteriaByApp(appName));
        }
      

        internal COEPrivilegeBOList()
        {
            MarkAsChild();
            /* require use of factory methods */ }

        #endregion

        #region Data Access

        [Serializable()]
        private class Criteria
        {

            internal string _roleName;
            internal bool _isGranted;
            public Criteria(string roleName, bool isGranted)
            {
                _roleName = roleName;
                _isGranted = isGranted;
            }
        }

        [Serializable()]
        private class CriteriaByRoleName
        {

            internal string _roleName;
            public CriteriaByRoleName(string roleName)
            {
                _roleName = roleName;
            }
        }

        [Serializable()]
        private class CriteriaByApp
        {
            internal string _coeIdentifier;

            //constructors
            public CriteriaByApp(string coeIdentifier)
            {
                _coeIdentifier = coeIdentifier;

            }
        }

        private void DataPortal_Fetch(CriteriaByRoleName criteria)
        {
            RaiseListChangedEvents = false;
            if (_coeDAL == null) { LoadDAL(); }
            // Coverity Fix CID - 11612 
            if (_coeDAL != null)
            {
                DataTable dt = null;
                dt = _coeDAL.GetPrivilegesForRole(criteria._roleName);
                Fetch(dt);
            }
            else
                throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));

            RaiseListChangedEvents = true;
        }               

        #endregion

        private void DataPortal_Fetch(CriteriaByApp criteria)
        {
            this.RaiseListChangedEvents = false;
            try
            {
                _coeLog.LogStart(string.Empty, 1);
                if (_coeDAL == null) { LoadDAL(); }
                 // Coverity Fix CID - 11611
                if (_coeDAL != null)
                {
                    //get all roles regardless of app or user
                    DataTable dt = _coeDAL.GetDefaultPrivileges(criteria._coeIdentifier);
                    Fetch(dt);
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

        protected void Fetch(DataTable dt)
        {   
            
            try
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    for (int j = 1; j < dt.Rows[i].ItemArray.Length; j++)
                    {

                           if (dt.Rows[i].ItemArray[j].ToString().Length>1){
                               COEPrivilegeBO COEPrivilegeBO = new COEPrivilegeBO(
                               j,
                               string.Empty, false,
                               dt.Rows[i].Table.Columns[j].DataType.ToString(),
                               dt.Rows[i].ItemArray[j].ToString());
                               this.Add(COEPrivilegeBO);
                           }else{
                            COEPrivilegeBO COEPrivilegeBO2 = new COEPrivilegeBO(
                            j,
                            dt.Rows[i].Table.Columns[j].ToString(),
                            dt.Rows[i].ItemArray[j].ToString() == "1",
                            string.Empty, string.Empty);
                            this.Add(COEPrivilegeBO2);
                           }
                           
                    }
                }
            }
           catch(System.Exception)
            {

            }
        }

        
       
        protected override void DataPortal_Update()
        {
            RaiseListChangedEvents = false;
            if (_coeDAL == null) { LoadDAL(); }
            foreach (COEPrivilegeBO item in DeletedList)
                item.DeleteSelf(_coeDAL);
            DeletedList.Clear();

            foreach (COEPrivilegeBO item in this)
                if (item.IsNew)
                    item.Insert(_coeDAL);
                else
                    item.Update(_coeDAL);
            RaiseListChangedEvents = true;
        }
        private void LoadDAL()
        {
            if (_dalFactory == null) { _dalFactory = new DALFactory(); }
            _dalFactory.GetDAL<DAL>(ref _coeDAL, _serviceName, COEDatabaseName.Get().ToString(), true);
        }
    }
}
