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
    public class COEPrivilegeBO : BusinessBase<COEPrivilegeBO>
    {
        #region member variables

        // TODO: add your own fields, properties and methods
        private int _privilegeID;
        private string _privilegeName = string.Empty;
        private string _nonPrivilegeName = string.Empty;
        private string _nonPrivilegeValue = string.Empty;
        private bool _enabled = false;
        //variables data access
        [NonSerialized]
        private DAL _coeDAL = null;
        [NonSerialized]
        private DALFactory _dalFactory = new DALFactory();
        private string _serviceName = "COESecurity";
        internal bool _isGranted = false;

        [NonSerialized]
        static COELog _coeLog = COELog.GetSingleton("COESecurity");

        #endregion

        #region constructors
        //constructor to be called from  as well as any other services that needs to construct this object
        internal COEPrivilegeBO(int privilegeID, string privilegeName, bool enabled, string nonPrivilegeName, string nonPrivilegeValue)
        {
            _privilegeID = privilegeID;
            _privilegeName = privilegeName;
            _enabled = enabled;
            _nonPrivilegeName = nonPrivilegeName;
            _nonPrivilegeValue= nonPrivilegeValue;
            MarkAsChild();
        }
        internal void Dirty()
        {
         MarkDirty();
        }

        internal void Deleted ()
        {
         MarkDeleted();
        }

        #endregion

        #region properties
        public int PrivilegeID
        {
            get { return _privilegeID; }
        }
        public string PrivilegeName
        {
            get { return _privilegeName; }
            set { _privilegeName = value; }
        }
        public string NonPrivilegeName
        {
            get { return _nonPrivilegeName; }
            set { _nonPrivilegeName = value; }
        }

        public string NonPrivilegeValue
        {
            get { return _nonPrivilegeValue; }
            set { _nonPrivilegeValue = value; }
        }
        public bool Enabled
        {
            get { return _enabled; }
            set { _enabled = value; }
        }
        protected override object GetIdValue()
        {
            return _privilegeID;
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

        public static COERoleReadOnlyBO Get(string appName, string privilegeName, string roleName)
        {
            SetDatabaseName();
            return DataPortal.Fetch<COERoleReadOnlyBO>(new Criteria(appName, privilegeName, roleName));
        }

       

        private COEPrivilegeBO()
        { /* require use of factory methods */ }


        public static COEPrivilegeBO New()
        {
            if (!CanAddObject())
                throw new System.Security.SecurityException(
                  "User not authorized to add a privilege");
            return DataPortal.Create<COEPrivilegeBO>();
        }

         public static void Delete(int privilegeID)
            {
                SetDatabaseName();
                if (!CanDeleteObject())
                    throw new System.Security.SecurityException(Resources.UserNotAuthorizedForDeleteObject + " COEPrivilegeBO");
                DataPortal.Delete(new CriteriaByID(privilegeID));
            }



            public override COEPrivilegeBO Save()
            {
                SetDatabaseName();
                if (IsDeleted && !CanDeleteObject())
                    throw new System.Security.SecurityException(Resources.UserNotAuthorizedForDeleteObject + " COEUserBO");
                else if (IsNew && !CanAddObject())
                    throw new System.Security.SecurityException(Resources.UserNotAuthorizedForAddObject + " COEUserBO");
                else if (!CanEditObject())
                    throw new System.Security.SecurityException(Resources.UserNotAuthorizedForEditObject + " COEUserBO");
                return base.Save();
            }
           

        #endregion

        #region Validation Rules

        protected override void AddBusinessRules()
        {
            ValidationRules.AddRule(
              Csla.Validation.CommonRules.StringRequired, "Name");
        }

        protected override void AddInstanceBusinessRules()
        {
            ValidationRules.AddInstanceRule(NoDuplicates, "UserID");
        }

        private bool NoDuplicates(object target, Csla.Validation.RuleArgs e)
        {
            COEPrivilegeBOList parent = (COEPrivilegeBOList)this.Parent;
            foreach (COEPrivilegeBO item in parent)
                if (item.PrivilegeID== _privilegeID && !ReferenceEquals(item, this))
                {
                    e.Description = "Privilege must be unique";
                    return false;
                }
            return true;
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

            internal string _privilegeName;
            internal string _appName;
            internal string _roleName;

            //constructors
            public Criteria(string appName, string privilegeName, string roleName)
            {
                _privilegeName = privilegeName;
                _appName = appName;
                _roleName = roleName;
            }
        }

        [Serializable()]
        private class CriteriaByID
        {

            internal int _id;

            //constructors
            public CriteriaByID(int id)
            {
                _id = id;
            }
        }
        #endregion //Criteria

        #region data access


        [RunLocal()]
        protected override void DataPortal_Create()
        {
            _privilegeID= 0;
            _privilegeName = string.Empty;
            _enabled = false;

            //ValidationRules.CheckRules();
        } 
        private void DataPortal_Fetch(Criteria criteria)
        {
            //_coeLog.LogStart(string.Empty, 1);
            //if (_coeDAL == null) { LoadDAL(); }
            //SafeDataReader dr = _coeDAL.GetPrivilege(criteria._appName, criteria._roleName, criteria._privilegeName);
            //FetchObject(dr);

            //_coeLog.LogEnd(string.Empty, 1);

        }

       

        private void FetchObject(SafeDataReader dr)
        {
            //try
            //{
            //    dr.Read();
            //         _privilegeID = dr.GetInt16("ROWNUM");
            //         _privilegeName = dr.GetString("PRIVILEGE_NAME");
            //         _enabled = dr.GetString("ENABLED").Equals("1");
            //    dr.Close();
            //}
            //catch (System.Exception ex)
            //{

            //}

        }

            protected override void DataPortal_Update()
            {

             }

            internal void Update(DAL coeDAL)
            {

            }


          
            protected override void DataPortal_Insert()
            {

             }

            internal void Insert(DAL coeDAL)
            {

             }


            private void DataPortal_Delete(CriteriaByID criteria)
             {

             }
             protected override void DataPortal_DeleteSelf()
             {
                 if (_coeDAL == null) { LoadDAL(); }
                 //_coeDAL.Delete(_id);
             }


             internal void DeleteSelf(DAL coeDAL)
             {

             }
           

        private void LoadDAL()
        {
            if (_dalFactory == null) { _dalFactory = new DALFactory(); }
            _dalFactory.GetDAL<DAL>(ref _coeDAL, _serviceName, COEDatabaseName.Get().ToString(), true);
        }
        #endregion

    }

}
