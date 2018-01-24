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
    public class COERoleReadOnlyBO : ReadOnlyBase<COERoleReadOnlyBO>
    {
        #region member variables

        // TODO: add your own fields, properties and methods
        private int _roleID;
        private string _roleName = string.Empty;

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
        internal COERoleReadOnlyBO(int roleID, string roleName)
        {
            _roleID = roleID;
            _roleName = roleName;
        }

        #endregion

        #region properties
        public int RoleID
        {
            get { return _roleID; }
        }
        public string RoleName
        {
            get { return _roleName; }
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

        public static COERoleReadOnlyBO Get(string UserName)
        {
            SetDatabaseName();
            return DataPortal.Fetch<COERoleReadOnlyBO>(new Criteria(UserName));
        }

        private COERoleReadOnlyBO()
        { /* require use of factory methods */ }



        #endregion
        #region Criteria

        [Serializable()]
        private class Criteria
        {

            internal string _roleName;

            //constructors
            public Criteria(string roleName)
            {
                _roleName = roleName;
            }
        }
        #endregion //Criteria

        #region data access
        private void DataPortal_Fetch(Criteria criteria)
        {
            _coeLog.LogStart("Fetching roles in COERoleReadOnlyBO", 1);
            if (_coeDAL == null) { LoadDAL(); }
            
            // Coverity Fix CID - 11597
            if (_coeDAL != null)
            {
                using (SafeDataReader dr = _coeDAL.GetRole(criteria._roleName))
                {
                    FetchObject(dr);
                }
            }
            else
                throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));

            _coeLog.LogEnd("Fetching roles in COERoleReadOnlyBO", 1);
        }

        private void FetchObject(SafeDataReader dr)
        {
            if (dr.Read())
            {
                _roleID = dr.GetInt16("ROLE_ID");
                _roleName = dr.GetString("ROLE_NAME");
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
