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
    public class PrivilegeList :
      NameValueListBase<string, bool>
    {
        #region member variables

        private static PrivilegeList _list;

        //variables data access
        [NonSerialized]
        private DAL _coeDAL = null;
        [NonSerialized]
        private DALFactory _dalFactory = new DALFactory();
        private string _serviceName = "COESecurity";

        [NonSerialized]
        static COELog _coeLog = COELog.GetSingleton("COESecurity");


        #endregion

        #region Business Methods



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



        public static PrivilegeList GetList(string roleName)
        {
            SetDatabaseName();
            return DataPortal.Fetch<PrivilegeList>(new CriteriaByRole(roleName));
        }
        public static PrivilegeList GetDefaultListForApp(string appName)
        {
            SetDatabaseName();
            return DataPortal.Fetch<PrivilegeList>(new CriteriaByApp(appName));
        }
      

        


        /// <summary>
        /// Clears the in-memory RoleList cache
        /// so the list of roles is reloaded on
        /// next request.
        /// </summary>
        public static void InvalidateCache()
        {
            _list = null;
        }

        private PrivilegeList()
        { /* require use of factory methods */ }

        #endregion

        #region critiera
       

        [Serializable()]
        private class CriteriaByRole
        {
            internal string _roleName;
           
            //constructors
            public CriteriaByRole(string roleName)
            {
                _roleName = roleName;
               
            }
        }

        [Serializable()]
        private class CriteriaByApp
        {
            internal string _appName;

            //constructors
            public CriteriaByApp(string appName)
            {
                _appName = appName;

            }
        }

        #endregion

        #region Data Access


        private void DataPortal_Fetch(CriteriaByRole criteria)
        {
            this.RaiseListChangedEvents = false;
            try
            {
                _coeLog.LogStart(string.Empty, 1);
                if (_coeDAL == null) { LoadDAL(); }
                // Coverity Fix CID - 11638
                if (_coeDAL != null)
                {
                    //get all roles regardless of app or user
                    DataTable dt = _coeDAL.GetPrivilegesForRole(criteria._roleName);
                    FetchObject(dt);
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


        private void DataPortal_Fetch(CriteriaByApp criteria)
        {
            this.RaiseListChangedEvents = false;
            try
            {
                _coeLog.LogStart(string.Empty, 1);
                if (_coeDAL == null) { LoadDAL(); }
                // Coverity Fix CID - 11637
                if (_coeDAL != null)
                {
                    //get all roles regardless of app or user
                    DataTable dt = _coeDAL.GetDefaultPrivileges(criteria._appName);
                    FetchObject(dt);
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


        private void FetchObject(DataTable dt)
        {
            try
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        // for (int j = 1; j < dt.Rows[i].ItemArray.Length-1; j++)
                        //removed the -1 from above. this was missing the last privilege in any table
                        for (int j = 1; j < dt.Rows[i].ItemArray.Length; j++)
                        {
                            IsReadOnly = false;
                            DataRow dr = dt.Rows[i];
                            this.Add(new NameValuePair(dt.Rows[i].Table.Columns[j].ToString(), dt.Rows[i].ItemArray[j].ToString() == "1"));

                        }
                    }

                IsReadOnly = true;

                
            }
            catch (Exception ex)
            {

                throw ex;
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

