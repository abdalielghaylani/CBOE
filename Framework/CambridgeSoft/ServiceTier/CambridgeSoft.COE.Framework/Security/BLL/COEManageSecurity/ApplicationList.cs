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
    public class ApplicationList :
      NameValueListBase<string, string>
    {
        #region member variables

        private static ApplicationList _list;

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

        #region critiera
        [Serializable()]
        private class Criteria
        {
            internal bool _addAllIdentifier;
            //constructors
            public Criteria(bool addAllIdentifier)
            {
                _addAllIdentifier = addAllIdentifier;
            }
        }
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
        public static ApplicationList GetList(bool addAllIdentifier)
        {
            SetDatabaseName();
            _list = DataPortal.Fetch<ApplicationList>
              (new Criteria(addAllIdentifier));
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

        private ApplicationList()
        { /* require use of factory methods */ }

        #endregion

        #region Data Access

        private void DataPortal_Fetch(Criteria criteria)
        {
            this.RaiseListChangedEvents = false;
            try
            {
                _coeLog.LogStart(string.Empty, 1);
                if (_coeDAL == null) { LoadDAL(); }
                 // Coverity Fix CID - 11603
                if (_coeDAL != null)
                {
                    using (SafeDataReader dr = _coeDAL.GetAllApplicationNames())
                    {
                        FetchObject(dr, criteria._addAllIdentifier);
                    }
                }
                else
                    throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));

                _coeLog.LogEnd(string.Empty, 1);
            }
            catch (Exception ex )
            {
                throw ex;
            }
            this.RaiseListChangedEvents = true;
        }

        private void FetchObject(SafeDataReader dr, bool addAllIdentifier)
        {
            IsReadOnly = false;
            if (addAllIdentifier)
            {
                this.Add(new NameValuePair("All CS Applications", "All CS Applications"));
            }
            while (dr.Read())
            {
                    IsReadOnly = false;
                    if (dr.GetString("COEIDENTIFIER") != string.Empty)
                    {
                        if(!this.ContainsKey(dr.GetString("COEIDENTIFIER").ToUpper()))
                            this.Add(new NameValuePair(dr.GetString("COEIDENTIFIER").ToUpper(), dr.GetString("COEIDENTIFIER").ToUpper()));
                    }
            }
            IsReadOnly = true;
        }


        private void LoadDAL()
        {
            if (_dalFactory == null) { _dalFactory = new DALFactory(); }
            _dalFactory.GetDAL<DAL>(ref _coeDAL, _serviceName, COEDatabaseName.Get().ToString(), true);
        }
        #endregion

    }
}

