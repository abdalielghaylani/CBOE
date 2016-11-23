using Csla;
using Csla.Data;
using System;
using System.Linq;
using System.Data;

using CambridgeSoft.COE.Framework.COEConfigurationService;
using CambridgeSoft.COE.Framework.COEDataViewService;
using CambridgeSoft.COE.Framework.COELoggingService;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.Properties;

namespace CambridgeSoft.COE.Framework.COEDatabasePublishingService
{
    [Serializable()]
    public class COEDatabaseBOList : Csla.BusinessListBase<COEDatabaseBOList, COEDatabaseBO>
    {
        // COEDB data access.
        [NonSerialized]
        private DAL _coeDAL = null;

        [NonSerialized]
        private DALFactory _dalFactory = null;
        private string _serviceName = "COEDatabasePublishing";
        private string _databaseName;
        static COELog _coeLog = COELog.GetSingleton("COEDatabasePublishing");

        #region Factory Methods

        private COEDatabaseBOList()
        { /* require use of factory method */ }

        public static COEDatabaseBOList NewList()
        {
            if (!CanAddObject())
                throw new System.Security.SecurityException(Resources.UserNotAuthorizedForAddObject + " COEDatabaseBOList");
            return new COEDatabaseBOList();
        }

        public static COEDatabaseBOList GetList(bool isPublished, string instanceName)
        {
            if (!CanGetObject())
                throw new System.Security.SecurityException(Resources.UserNotAuthorizedForViewObject + " COEDatabaseBOList");

            if (isPublished)
            {
                return GetPublishedList(instanceName);
            }
            else
            {
                var allDb = GetList(instanceName);
                var published = GetPublishedList(instanceName);
                var nonPublished = COEDatabaseBOList.NewList();

                foreach (var db in allDb)
                {
                    var dbName = db.Name;
                    
                    // In COEDB, the dbname does not contain "Main." for default main instance.
                    InstanceData mainInstance = ConfigurationUtilities.GetMainInstance();
                    if (!string.IsNullOrEmpty(instanceName) && !instanceName.Equals(mainInstance.Name, StringComparison.InvariantCultureIgnoreCase))
                    {
                        dbName = instanceName + "." + dbName;
                    }

                    var publishedDb = published.Where(d => d.Name.Equals(dbName, StringComparison.InvariantCultureIgnoreCase))
                                               .FirstOrDefault();

                    if (publishedDb == null)
                    {
                        nonPublished.Add(db);
                    }
                }

                return nonPublished;
            }
        }

        private static COEDatabaseBOList GetPublishedList(string instanceName)
        {
            if (!CanGetObject())
                throw new System.Security.SecurityException(Resources.UserNotAuthorizedForViewObject + " COEDatabaseBOList");
            return DataPortal.Fetch<COEDatabaseBOList>(new CriteriaPublished(instanceName));
        }

        public static COEDatabaseBOList GetList(string instanceName)
        {
            if (!CanGetObject())
                throw new System.Security.SecurityException(Resources.UserNotAuthorizedForViewObject + " COEDatabaseBOList");
            return DataPortal.Fetch<COEDatabaseBOList>(new Criteria(instanceName));
        }

        public COEDatabaseBO GetDatabase(string name)
        {
            foreach (COEDatabaseBO database in this)
            {
                if (database.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
                {
                    return database;
                }
            }

            return null;
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
            internal string _instanceName;
            //constructors
            public Criteria(string instanceName)
            {
                _instanceName = instanceName;
            }
        }

        [Serializable()]
        private class CriteriaPublished
        {
            internal string _instanceName = null;
            //constructors
            public CriteriaPublished(string instanceName)
            {
                _instanceName = instanceName;
            }
        }

        #endregion //Filter Criteria

        #region Data Access - Fetch

        private void DataPortal_Fetch(CriteriaPublished criteria)
        {
            RaiseListChangedEvents = false;

            _coeDAL = null;
            _dalFactory = null;

            // For published schema, we always retrive from COEDB.
            LoadDAL(Resources.CentralizedStorageDB);
            using (SafeDataReader dr = _coeDAL.GetPublishedDatabases())
            {
                FetchCOETable(dr);
            }

            RaiseListChangedEvents = true;
        }

        private void DataPortal_Fetch(Criteria criteria)
        {
            RaiseListChangedEvents = false;

            _coeDAL = null;
            _dalFactory = null;

            InstanceData mainInstance = ConfigurationUtilities.GetMainInstance();
            var instanceName = string.IsNullOrEmpty(criteria._instanceName) ? mainInstance.Name : criteria._instanceName;
            InstanceData instanceData = ConfigurationUtilities.GetInstanceData(instanceName);

            if (instanceData.IsCBOEInstance)
            {
                LoadDAL(Resources.SecurityDatabaseName);
            }
            else
            {
                var qualifySecurityName = instanceData.InstanceName + "." + instanceData.DatabaseGlobalUser;
                LoadDAL(qualifySecurityName);
            }

            // For mainDb, use the string.Empty/NULL as the instance name.
            using (SafeDataReader dr = _coeDAL.GetAllInstanceDatabases())
            {
                FetchInstanceDb(dr, instanceName);
            }

            RaiseListChangedEvents = true;
        }

        protected void FetchCOETable(SafeDataReader dr)
        {
            while (dr.Read())
            {
                try
                {
                    COEDatabaseBO databaseData = new COEDatabaseBO(
                        dr.GetInt16("ID"),
                        dr.GetString("NAME"),
                        dr.GetSmartDate("DATE_CREATED"),
                        (COEDataView)COEDataViewUtilities.DeserializeCOEDataView(dr.GetString("COEDATAVIEW")),
                        true, null, null);

                    this.Add(databaseData);
                }
                catch (Exception e)
                {
                }
            }
        }

        protected void FetchInstanceDb(SafeDataReader dr, string instanceName)
        {
            while (dr.Read())
            {
                try
                {
                    // var qualifyDbName = string.IsNullOrEmpty(instanceName) ? dr.GetString("OWNER") : instanceName + "." + dr.GetString("OWNER");
                    var databaseData = new COEDatabaseBO(0, dr.GetString("OWNER"), new SmartDate(), null, false, instanceName, null);
                    this.Add(databaseData);
                }
                catch (Exception e)
                {
                }
            }
        }

        #endregion //Data Access - Fetch

        #region Data Access - Update
        protected override void DataPortal_Update()
        {
            RaiseListChangedEvents = false;
            if (_coeDAL == null) { LoadDAL(Resources.CentralizedStorageDB); }

            // loop through each deleted child object
            foreach (COEDatabaseBO deletedChild in DeletedList)
                deletedChild.DeleteSelf(_coeDAL);
            DeletedList.Clear();

            // loop through each non-deleted child object
            foreach (COEDatabaseBO child in this)
            {
                //if (child.IsNew)
                //child.Insert2(_coeDAL);
                //else
                //    child.Update(_coeDAL);


                //RaiseListChangedEvents = true;
            }
        }

        #endregion //Data Access - Update

        //private void LoadDAL()
        //{
        //    _dalFactory = new DALFactory();
        //    _dalFactory.GetDAL<DAL>(ref _coeDAL, _serviceName, Resources.CentralizedStorageDB, true);
        //}

        private void LoadDAL(string databaseName)
        {
            _dalFactory = new DALFactory();
            _dalFactory.GetDAL<CambridgeSoft.COE.Framework.COEDatabasePublishingService.DAL>(ref _coeDAL, _serviceName, databaseName, true);
        }

        #endregion //Data Access
    }
}
    

