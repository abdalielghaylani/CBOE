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
using CambridgeSoft.COE.Framework.COEConfigurationService;

namespace CambridgeSoft.COE.Framework.COESecurityService
{

    [Serializable()]
    public class GroupUser : ReadOnlyBase<GroupUser>
    {
        #region member variables

        // TODO: add your own fields, properties and methods
        private int _personID;
        private string _userID;
        private int _supervisorID;
        private int _siteID;
        private bool _active = false;
        private string _userCode = string.Empty;
        private string _firstName = string.Empty;
        private string _lastName = string.Empty;
        private string _middleName = string.Empty;
        private string _title = string.Empty;
        private string _department = string.Empty;
        private string _telephone = string.Empty;
        private string _address = string.Empty;
        private string _email = string.Empty;
        private RoleList _groupGrantedRoles= null;
        private RoleList _directGrantedRoles = null;
        private RoleList _directAvailableRoles = null;
        private bool _isleader = false;
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
        internal GroupUser(int personID, string userCode, string userID, int supervisorID,
                       string title, string firstName, string middleName, string lastName, int siteID, string department, string telephone, string email, string address, bool active,
            RoleList groupGrantedRoles, RoleList directGrantedRoles, RoleList directAvailableRoles, bool isleader)
        {
            _personID = personID;
            _userCode = userCode;
            _userID = userID;
            _supervisorID = supervisorID;
            _title = title;
            _firstName = firstName;
            _middleName = middleName;
            _lastName = lastName;
            _siteID = siteID;
            _department = department;
            _telephone = telephone;
            _email = email;
            _address = address;
            _active = active;
            _groupGrantedRoles = groupGrantedRoles;
            _directGrantedRoles = directGrantedRoles;
            _directAvailableRoles = directAvailableRoles;
            _isleader = isleader;
        }

        #endregion

        #region properties
        public int PersonID
        {
            get { return _personID; }
        }
        public string UserID
        {
            get { return _userID; }
        }
        public int SupervisorID
        {
            get { return _supervisorID; }
        }
        public bool Active
        {
            get { return _active; }
        }

        public string UserCode
        {
            get { return _userCode; }
        }

        public string FirstName
        {
            get { return _firstName; }
        }

        public string LastName
        {
            get { return _lastName; }
        }

        public string MiddleName
        {
            get { return _middleName; }
        }

        public string Title
        {
            get { return _title; }
        }

        public string Department
        {
            get { return _department; }
        }

        public string Telephone
        {
            get { return _telephone; }
        }

        public string Address
        {
            get { return _address; }
        }


        public string Email
        {
            get { return _email; }
        }

        public RoleList GroupGrantedRoles
        {
            get { return _groupGrantedRoles; }
        }
       
        public RoleList DirectGrantedRoles
        {
            get { return _directGrantedRoles; }
        }

        public RoleList DirectAvailableRoles
        {
            get { return _directAvailableRoles; }
        }

        protected override object GetIdValue()
        {
            return _personID;
        }


        public bool isleader
        {
            get { return _isleader; }
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

      

        private GroupUser()
        { /* require use of factory methods */ }



        #endregion

        #region Criteria

        [Serializable()]
        private class Criteria
        {

            internal int _personID;

            //constructors
            public Criteria(int personID)
            {
                _personID = personID;
            }
        }
        #endregion //Criteria

        #region data access
        private void DataPortal_Fetch(Criteria criteria)
        {

            //DataTable dt = _coeDAL.GetUser(criteria._userName);
            //FetchObject(dt);

            //_coeLog.LogEnd("Fetching users in GroupUser", 1);

        }


        private void FetchObject(DAL _coeDAL, DataTable dt)
        {
            try
            {

            //    _personID = System.Convert.ToInt32(dt.Rows[0]["PERSONID"]);
            //    _userCode = System.Convert.ToString(dt.Rows[0]["USERCODE"]);
            //    _userID = System.Convert.ToString(dt.Rows[0]["USERNAME"]);
            //    _supervisorID = System.Convert.ToInt32(dt.Rows[0]["SUPERVISORID"]);
            //    _title = System.Convert.ToString(dt.Rows[0]["TITLE"]);
            //    _firstName = System.Convert.ToString(dt.Rows[0]["FIRSTNAME"]);
            //    _middleName = System.Convert.ToString(dt.Rows[0]["MIDDLENAME"]);
            //    _lastName = System.Convert.ToString(dt.Rows[0]["LASTNAME"]);
            //    _siteID = System.Convert.ToInt32(dt.Rows[0]["SITEID"]);
            //    _department = System.Convert.ToString(dt.Rows[0]["DEPARTMENT"]);
            //    _telephone = System.Convert.ToString(dt.Rows[0]["TELEPHONE"]);
            //    _email = System.Convert.ToString(dt.Rows[0]["EMAIL"]);
            //    _active = System.Convert.ToBoolean(dt.Rows[0]["ISACTIVE"]);
            //    _groupGrantedRoles = _coeDAL.GetGroupPrivSets(//net to get these
            //    _directGrantedRoles = //net to get these
            //    _directAvailableRoles = //net to get these




            }
            catch (System.Exception ex)
            {

            }

        }




        private void LoadDAL()
        {
            ////Load DAL for database calls. The database is actually alwas set to the centralizedDataStorageDB in the resources. However, I'm leaving
            ////the code untouced just in case we need to go back to non-centralized.
            //if (_dalFactory == null) { _dalFactory = new DALFactory(); }
            //_dalFactory.GetDAL<DAL>(ref _coeDAL, _serviceName, COEDatabaseName.Get().ToString(), true);
        }
        #endregion

    }
	
}     
