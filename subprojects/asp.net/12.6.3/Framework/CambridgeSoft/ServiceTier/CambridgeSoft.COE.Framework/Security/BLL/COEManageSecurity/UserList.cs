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
    public class UserList :
      NameValueListBase<string, string>
    {
        #region member variables

        private static UserList _list;

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
            COEDatabaseName.Set(DALUtils.GetDefaultQualifyDbName(Resources.CentralizedStorageDB));
        }


        internal static void SetDatabaseName(string databaseName)
        {
            COEDatabaseName.Set(DALUtils.GetDefaultQualifyDbName(Resources.CentralizedStorageDB));

        }
        /// <summary>
        /// Returns a list of roles.
        /// </summary>
        public static UserList GetDBMSList()
        {
            SetDatabaseName();
            _list = DataPortal.Fetch<UserList>
              (new DBMSUserCriteria());
            return _list;
        }


        /// <summary>
        /// Returns a list of roles.
        /// </summary>
        public static UserList GetCOEList()
        {
            SetDatabaseName();
            _list = DataPortal.Fetch<UserList>
              (new COEUserCriteria(string.Empty));
            return _list;
        }


        /// <summary>
        /// Returns a list of roles.
        /// </summary>
        public static UserList GetCOEList(string _coeIdentifier)
        {
            SetDatabaseName();
            _list = DataPortal.Fetch<UserList>
              (new COEUserCriteria(_coeIdentifier));
            return _list;
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

        private UserList()
        { /* require use of factory methods */ }

        #endregion

        #region critiera
        [Serializable()]
        private class DBMSUserCriteria
        {

            //constructors
            public DBMSUserCriteria()
            {
                
            }
        }

        [Serializable()]
        private class COEUserCriteria
        {
            internal string _coeIdentifier;
            //constructors
           

            public COEUserCriteria(string coeIdentifier)
            {
                _coeIdentifier = coeIdentifier;
            }
        }
        #endregion

        #region Data Access

        private void DataPortal_Fetch(DBMSUserCriteria criteria)
        {
            this.RaiseListChangedEvents = false;
            try
            {
                _coeLog.LogStart(string.Empty, 1);
                if (_coeDAL == null) { LoadDAL(); }
                // Coverity Fix CID - 11650
                if (_coeDAL != null)
                {
                    DataTable dt = _coeDAL.GetAllDBMSUsers();
                    FetchObject1(dt);
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

        private void DataPortal_Fetch(COEUserCriteria criteria)
        {
            this.RaiseListChangedEvents = false;
            try
            {
                _coeLog.LogStart(string.Empty, 1);
                if (_coeDAL == null) { LoadDAL(); }
                // Coverity Fix CID - 11649
                if (_coeDAL != null)
                {
                    DataTable dt = _coeDAL.GetAllCOEUsers(criteria._coeIdentifier);
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
                    IsReadOnly = false;
                    DataRow dr = dt.Rows[i];
                    string realName = string.Empty;
                    string userName = dr["USERNAME"].ToString();
			        string fName = dr["FirstName"].ToString();
                    string lName = dr["LastName"].ToString();
			        if (fName.Length + lName.Length > 0) {
                        if (lName.Length > 0 && fName.Length > 0)
                        {

                            lName = lName + ",";
                            realName = userName +  " (" + lName + fName + ")";
                        }
                        else
                        {
                            if (lName.Length > 0 && fName.Length == 0)
                            {
                                if (lName.ToUpper() == userName.ToUpper())
                                {
                                    realName = userName;
                                }
                                else
                                {
                                    realName = userName + " (" + lName + ")";
                                }
                            }
                            
                        }
				        
                    }else{
                        realName = userName;
                    }
                    if (dr["USERNAME"].ToString() != string.Empty)
                    {
                        this.Add(new NameValuePair(dr["USERNAME"].ToString(), realName));
                    }


                }

                IsReadOnly = true;
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void FetchObject1(DataTable dt)
        {
            try
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    IsReadOnly = false;
                    DataRow dr = dt.Rows[i];
                    this.Add(new NameValuePair(dr["USERNAME"].ToString(), dr["USERNAME"].ToString()));


                }

                IsReadOnly = true;
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
        #endregion

    }
}

