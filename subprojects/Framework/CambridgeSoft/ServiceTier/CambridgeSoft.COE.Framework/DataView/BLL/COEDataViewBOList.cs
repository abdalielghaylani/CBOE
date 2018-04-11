using System;
using System.Data;
using Csla;
using Csla.Data;
using Csla.Validation;
using System.Collections.Generic;
using System.Configuration;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.COELoggingService;
using CambridgeSoft.COE.Framework.Properties;
using System.ComponentModel;
using System.Xml;
using CambridgeSoft.COE.Framework.ExceptionHandling;

namespace CambridgeSoft.COE.Framework.COEDataViewService
{
    [Serializable()]
    public class COEDataViewBOList : Csla.BusinessListBase<COEDataViewBOList, COEDataViewBO>
    {
        //variables data access
        [NonSerialized]
        private DAL _coeDAL = null;
        [NonSerialized]
        private DALFactory _dalFactory = new DALFactory();
        private string _serviceName = "COEDataView";
        private string _databaseName;
        static COELog _coeLog = COELog.GetSingleton("COEDataView");
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

        private COEDataViewBOList()
        { /* require use of factory method */ }



        public static COEDataViewBOList NewList()
        {
            SetDatabaseName();
            if (!CanAddObject())
                throw new System.Security.SecurityException(Resources.UserNotAuthorizedForAddObject + " COEDataViewBOList");
            return new COEDataViewBOList();
        }
        public static COEDataViewBOList GetDataViewDataList()
        {
            SetDatabaseName();

            if (!CanGetObject())
                throw new System.Security.SecurityException(Resources.UserNotAuthorizedForViewObject + " COEDataViewBOList");
            return DataPortal.Fetch<COEDataViewBOList>(new Criteria());
        }
        public static COEDataViewBOList GetDataViewDataListByUser(string userName)
        {
            SetDatabaseName();

            if (!CanGetObject())
                throw new System.Security.SecurityException(Resources.UserNotAuthorizedForViewObject + " COEDataViewBOList");
            return DataPortal.Fetch<COEDataViewBOList>(new GetByUserCriteria(userName));
        }
        public static COEDataViewBOList GetDataViewListbyDatabase(string databaseName)
        {
            SetDatabaseName();

            if (!CanGetObject())
                throw new System.Security.SecurityException(Resources.UserNotAuthorizedForViewObject + " COEDataViewBOList");
            return DataPortal.Fetch<COEDataViewBOList>(new GetByDatabaseNameCriteria(databaseName, false));
        }

        public static COEDataViewBOList GetDataViewListbyDatabase(string databaseName, bool lightWeight)
        {
            SetDatabaseName();
            if (!CanGetObject())
                throw new System.Security.SecurityException(Resources.UserNotAuthorizedForViewObject + " COEDataViewBOList");
            return DataPortal.Fetch<COEDataViewBOList>(new GetByDatabaseNameCriteria(databaseName, lightWeight));
        }

        public static COEDataViewBOList GetDataViewListforAllDatabases()
        {
            SetDatabaseName();

            if (!CanGetObject())
                throw new System.Security.SecurityException(Resources.UserNotAuthorizedForViewObject + " COEDataViewBOList");
            return DataPortal.Fetch<COEDataViewBOList>(new GetByAllDatabaseNameCriteria(false));
        }

        public static COEDataViewBOList GetDataviewListForApplication(string applicationName)
        {
            SetDatabaseName();

            if (!CanGetObject())
                throw new System.Security.SecurityException(Resources.UserNotAuthorizedForViewObject + " COEDataViewBOList");
            return DataPortal.Fetch<COEDataViewBOList>(new GetByApplication(false, applicationName));
        }

        /// <summary>
        /// Returns all the Dataviews in all the DBs but Master DataView;
        /// </summary>
        /// <returns>List of DataViews</returns>
        /// <remarks>This method assums the Master DV Id = 0</remarks>
        public static COEDataViewBOList GetDataViewListAndNoMaster()
        {
            SetDatabaseName();

            if (!CanGetObject())
                throw new System.Security.SecurityException(Resources.UserNotAuthorizedForViewObject + " COEDataViewBOList");

            return DataPortal.Fetch<COEDataViewBOList>(new GetByAllDatabaseNameCriteria(false, true));
        }

        private COEDataViewBO GetDataViewBOByID(int id)
        {
            Csla.FilteredBindingList<COEDataViewBO> filteredList = new FilteredBindingList<COEDataViewBO>(this);
            filteredList.FilterProvider = new Csla.FilterProvider(Filters.GetByID);
            filteredList.ApplyFilter(String.Empty, id);
            return filteredList.Count > 0 ? filteredList[0] : null;
        }

        public List<COEDataViewBO> GetByDatabaseName(string databaseName)
        {
            List<COEDataViewBO> list = new List<COEDataViewBO>();
            foreach (COEDataViewBO dv in this)
            {
                if (dv.DatabaseName == databaseName)
                    list.Add(dv);
            }
            return list;
        }

        public static COEDataViewBOList GetDataViewListforAllDatabases(bool lightWeight)
        {
            SetDatabaseName();

            if (!CanGetObject())
                throw new System.Security.SecurityException(Resources.UserNotAuthorizedForViewObject + " COEDataViewBOList");
            return DataPortal.Fetch<COEDataViewBOList>(new GetByAllDatabaseNameCriteria(lightWeight));
        }
        public static bool CanAddObject()
        {
            //TO DO:  need to add authorization access code
            return true;
        }

        public static bool CanGetObject()
        {
            //TO DO:  need to add authorization access code
            return true;
        }
        public static bool CanEditObject()
        {
            //TO DO:  need to add authorization access code
            return true;
        }
        public static bool CanDeleteObject()
        {
            //TO DO:  need to add authorization access code
            return true;
        }

        #endregion //Factory Methods

        #region Data Access

        #region Filter Criteria
        [Serializable()]
        private class Criteria
        {


            //constructors
            public Criteria()
            {

            }

        }

        [Serializable()]
        private class GetByUserCriteria
        {
            public string _userName;
            public bool _excludeLoggedInUser;

            //constructors
            public GetByUserCriteria(string userName)
            {
                _userName = userName;
            }

        }

        [Serializable()]
        private class GetByDatabaseNameCriteria
        {
            public bool _lightWeight;
            public string _databaseName;

            //constructors
            public GetByDatabaseNameCriteria(string databaseName, bool lightWeight)
            {
                _lightWeight = lightWeight;

            }
        }

        [Serializable()]
        private class GetByAllDatabaseNameCriteria
        {
            //LightWeight means that only the summary information would be retrieved, with no actual xml.
            public bool _lightWeight;
            //AvoidMaster is to add a filter: where id > 0. We expect the master to be way bigger than the rest and it is reasonable not to retrieve it when it is not absolutely necessary.
            public bool _avoidMaster;
            //constructors

            /// <summary>
            /// Initializes the criteria with the lightweight option. It means it WOULD retrieve the master dv, and depending on the lightWeight setting it may or may not retrieve the xml
            /// </summary>
            /// <param name="lightWeight">If true, the actual xml is not retrieved</param>
            public GetByAllDatabaseNameCriteria(bool lightWeight)
            {
                _lightWeight = lightWeight;
                _avoidMaster = false;
            }

            /// <summary>
            /// Initialize the criteria with both parameters.
            /// </summary>
            /// <param name="lightWeight">If true, the actual xml is not retrieved</param>
            /// <param name="avoidMaster">if true the master dataview is not fetched</param>
            public GetByAllDatabaseNameCriteria(bool lightWeight, bool avoidMaster)
            {
                _lightWeight = lightWeight;
                _avoidMaster = avoidMaster;
            }

        }

        [Serializable()]
        private class GetByApplication
        {
            public bool _lightWeight;
            public string _appName;
            //constructors
            public GetByApplication(bool lightWeight, string appName)
            {
                _lightWeight = lightWeight;
                _appName = appName;
            }
        }
        #endregion //Filter Criteria

        #region Data Access - Fetch
        private void DataPortal_Fetch(Criteria criteria)
        {
            RaiseListChangedEvents = false;
            if (_coeDAL == null) { LoadDAL(); }
            //COverity Bug Fix 11494 
            if (_coeDAL != null)
            {
                using (SafeDataReader dr = _coeDAL.GetAll())
                {
                    Fetch(dr);
                }
            }
            else
                throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));

            RaiseListChangedEvents = true;
        }

        private void DataPortal_Fetch(GetByUserCriteria criteria)
        {
            RaiseListChangedEvents = false;
            if (_coeDAL == null) { LoadDAL(); }
            // Coverity Fix CID - 11497 
            if (_coeDAL != null)
            {
                using (SafeDataReader dr = _coeDAL.GetUserDataViews(criteria._userName))
                {
                    Fetch(dr);
                }
            }
            else
                throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));
        }
        private void DataPortal_Fetch(GetByDatabaseNameCriteria criteria)
        {
            RaiseListChangedEvents = false;
            SafeDataReader dr = null;
            if (_coeDAL == null) { LoadDAL(); }

            try
            {
                // Coverity Fix CID - 11496 
                if (_coeDAL != null)
                {
                    if (criteria._lightWeight != true)
                    {
                        dr = _coeDAL.GetAll();
                        Fetch(dr);
                    }
                    else
                    {
                        dr = _coeDAL.GetAllLightWeight();
                        FetchLightWeight(dr);
                    }
                }
                else
                    throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));
            }
            finally
            {
                if (dr != null)
                    dr.Close();
            }

            RaiseListChangedEvents = true;
        }
        private void DataPortal_Fetch(GetByAllDatabaseNameCriteria criteria)
        {
            RaiseListChangedEvents = false;
            SafeDataReader dr = null;

            _dalFactory = new DALFactory();
            _coeDAL = null;
            LoadDAL();

            try
            {
                //COverity Bug Fix 11494
                if (_coeDAL != null)
                {
                    if (criteria._lightWeight != true)
                    {
                        if (criteria._avoidMaster)
                            dr = _coeDAL.GetAllNoMaster();
                        else
                            dr = _coeDAL.GetAll();

                        Fetch(dr);
                    }
                    else
                    {
                        if (criteria._avoidMaster)
                            dr = _coeDAL.GetAllNoMasterLightWeight();
                        else
                            dr = _coeDAL.GetAllLightWeight();

                        FetchLightWeight(dr);
                    }
                }
                else
                    throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));
            }
            finally
            {
                if (dr != null)
                    dr.Close();
            }

            RaiseListChangedEvents = true;
        }
        private void DataPortal_Fetch(GetByApplication criteria)
        {
            RaiseListChangedEvents = false;
            if (_coeDAL == null) { LoadDAL(); }
            // Coverity Fix CID - 11495
            if (_coeDAL != null)
            {
                using (SafeDataReader dr = _coeDAL.GetApplicationDataviews(criteria._appName))
                {
                    Fetch(dr);
                }
            }
            else
                throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));
            RaiseListChangedEvents = true;
        }
        protected void Fetch(SafeDataReader dr)
        {
            //COverity Bug Fix CID 11629 
            if (dr == null)
            {
                return;
            }

            try
            {
                while (dr.Read())
                {
                    COEDataView dataviewMessagingType = new COEDataView();
                    var dataView = dr.GetString("COEDATAVIEW");
                    try
                    {
                        dataviewMessagingType = (COEDataView)COEDataViewUtilities.DeserializeCOEDataView(dataView);
                    }
                    catch (Exception)
                    {
                        // To allow the edition of invalid dataviews for fixing it.
                        try
                        {
                            XmlDocument doc = new XmlDocument();
                            doc.LoadXml(dataView);
                            dataviewMessagingType = new COEDataView(doc);
                        }
                        catch (Exception)
                        {
                            dataviewMessagingType = new COEDataView();
                        }
                    }

                    COEDataViewBO dataViewData = new COEDataViewBO(
                        Convert.ToInt32(dr.GetInt64("ID")),
                        dr.GetString("NAME"),
                        dr.GetString("DESCRIPTION"),
                        dr.GetString("USER_ID"),
                        (dr.GetString("IS_PUBLIC").Equals("1") ? true : false),//: By Sumeet, during creation of sample application.
                        dr.GetInt32("FORMGROUP"),
                        dr.GetSmartDate("DATE_CREATED"),
                        dataviewMessagingType,
                        dr.GetString("DATABASE")); //if we want to support database specific storage then we will need to use the databaseName parameter

                    this.Add(dataViewData);
                }
            }
            catch (Oracle.DataAccess.Client.OracleException ex)
            {
                COEExceptionDispatcher.DispatchDALException(ex, null);
            }
        }

        protected void FetchLightWeight(SafeDataReader dr)
        {
            try
            {
                while (dr.Read())
                {
                    try
                    {
                        COEDataViewBO dataViewData = new COEDataViewBO(
                            dr.GetInt32("ID"),
                            dr.GetString("NAME"),
                            dr.GetString("DESCRIPTION"),
                            dr.GetString("USER_ID"),
                            (dr.GetString("IS_PUBLIC").Equals("1") ? true : false),//: By Sumeet, during creation of sample application.
                            dr.GetInt32("FORMGROUP"),
                            dr.GetSmartDate("DATE_CREATED"),
                            null,
                            dr.GetString("DATABASE")); //if we want to support database specific storage then we will need to use the databaseName parameter

                        this.Add(dataViewData);
                    }
                    catch (Exception e)
                    {
                        //Loopthrough
                    }
                }
            }
            catch (Oracle.DataAccess.Client.OracleException ex)
            {
                COEExceptionDispatcher.DispatchDALException(ex, null);
            }
        }

        #endregion //Data Access - Fetch


        #region Data Access - Update
        protected override void DataPortal_Update()
        {
            RaiseListChangedEvents = false;
            if (_coeDAL == null) { LoadDAL(); }

            // loop through each deleted child object
            foreach (COEDataViewBO deletedChild in DeletedList)
                deletedChild.DeleteSelf(_coeDAL);
            DeletedList.Clear();

            // loop through each non-deleted child object
            foreach (COEDataViewBO child in this)
            {
                if (child.IsNew)
                    child.Insert(_coeDAL);
                else
                    child.Update(_coeDAL);


                RaiseListChangedEvents = true;
            }
        }

        #endregion //Data Access - Update


        private void LoadDAL()
        {
            if (_dalFactory == null) { _dalFactory = new DALFactory(); }
            _dalFactory.GetDAL<DAL>(ref _coeDAL, _serviceName, COEDatabaseName.Get().ToString(), true);
        }
        /// <summary>
        /// Overloaded method of the LoadDAL, 
        /// </summary>
        /// <param name="appName">Name of the Application, It should be one from the provided in the Configuration.</param>
        private void LoadDAL(string databaseName)
        {
            if (_dalFactory == null) { _dalFactory = new DALFactory(); }
            _dalFactory.GetDAL<DAL>(ref _coeDAL, _serviceName, databaseName, true);
        }
        #endregion //Data Access

        #region Filtering class

        private class Filters
        {
            public static bool GetByID(object item, object filter)
            {
                int id = -1;
                COEDataViewBO dataViewBO = null;
                //Filter is a int
                //Coverity Bug Fix CID 11492 
                if (filter != null && filter is int)
                    id = Convert.ToInt32((string)filter.ToString());
                //assume item is the object it self. (See string.empty parameter)
                if (item != null && item is COEDataViewBO)
                    dataViewBO = ((COEDataViewBO)item);
                if (dataViewBO != null && dataViewBO.ID == id)
                    return true;
                return false;
            }
        }

        #endregion
    }
}