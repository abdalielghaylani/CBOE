using Csla;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.Properties;
using System;
using Csla.Data;


namespace CambridgeSoft.COE.Framework.COEReportingService
{
    [Serializable]
    public class COEReportBOList : BusinessListBase<COEReportBOList, COEReportBO>
    {
        #region Variables

        //variables data access
        [NonSerialized]
        private DAL _coeDAL = null;
        [NonSerialized]
        private DALFactory _dalFactory = new DALFactory();
        private string _serviceName = Resources.ReportingServiceName;
        private string _databaseName = string.Empty;
        private const string CLASSNAME = "COEReportTemplateBOList";

        #endregion

        #region Factory Methods

        private COEReportBOList()
        { /* require use of factory method */ }

        /// <summary>
        /// this method must be called prior to any other method inorder to set the database that the dal will use
        /// </summary>
        internal static void SetDatabaseName()
        {
            COEDatabaseName.Set(DALUtils.GetDefaultQualifyDbName(Resources.CentralizedStorageDB));
        }

        /// <summary>
        /// this method must be called prior to any other method inorder to set the database that the dal will use
        /// </summary>
        /// <param name="databaseName">the database name to be set</param>
        internal static void SetDatabaseName(string databaseName)
        {
            COEDatabaseName.Set(databaseName);
        }                   

        /// <summary>
        /// Gets all COEReportBO´s from the database.
        /// </summary>
        /// <returns>a list of all COEReportTemplateBO´s stored on database</returns>
        public static COEReportBOList GetCOEReportBOList()
        {
            SetDatabaseName();

            if (!CanGetObject())
                throw new System.Security.SecurityException(Resources.UserNotAuthorizedForViewObject + " " + CLASSNAME);
            return DataPortal.Fetch<COEReportBOList>(new Criteria());
        }

        /// <summary>
        /// Gets all COEReportBO´s of the user with id = <paramref name="userId"/>
        /// </summary>
        /// <param name="userId">the owner of the report list</param>
        /// <returns></returns>
        public static COEReportBOList GetCOEReportBOListByUser(string userId)
        {
            return GetCOEReportBOList(userId, null, null, true, int.MinValue, null);
        }

        /// <summary>
        /// Gets all COEReportBO´s of the user with id = <paramref name="userId"/> and dataview id = <paramref name="dataViewId"/>
        /// </summary>
        /// <param name="userId">the user id of the reports owner</param>
        /// <param name="dataViewId">the dataview of the report list</param>
        /// <returns>a COEReportBOList filtered by user id and dataview id</returns>
        public static COEReportBOList GetCOEReportBOListByDataViewId(string userId,int dataViewId)
        {
            return GetCOEReportBOList(userId, null, null, true, int.MinValue, null);
        }

        /// <summary>
        /// Gets all COEReportBO´s with database name = <paramref name="databaseName"/>
        /// </summary>
        /// <param name="databaseName"></param>
        /// <returns>a COEReportBOList filtered by database name</returns>
        public static COEReportBOList GetCOEReportBOListByDatabase(string databaseName)
        {
            return GetCOEReportBOList(null, databaseName, null, true, int.MinValue, null);
        }

        /// <summary>
        /// Gets a COEReportBOList by long criteria
        /// </summary>
        /// <param name="userId">the user id of the COEReportBOList to get. Ignored if null.</param>
        /// <param name="databaseName">the database name of the COEReportBOList to get. Ignored if null</param>
        /// <param name="application">the aplication name of the COEReportBOList to get. Ignored if null</param>
        /// <param name="getPublicReports">boolean value indicating to retrieve only public reports or everything.</param>
        /// <param name="dataViewId">the dataview id of the COEReportBOList to get. Ignored if negative</param>
        /// <returns>a COEReportBOList filtered by long criteria. Ignored if negative.</returns>
        public static COEReportBOList GetCOEReportBOList(string userId, string databaseName, string application, bool getPublicReports, int dataViewId)
        {
            return GetCOEReportBOList(userId, databaseName, application, getPublicReports, dataViewId, null);
        }

        public static COEReportBOList GetCOEReportBOList(string userId, string databaseName, string application, bool getPublicReports, int dataViewId, COEReportType? type)
        {
            return GetCOEReportBOList(userId, databaseName, application, getPublicReports, dataViewId, type, null);
        }

        public static COEReportBOList GetCOEReportBOList(string userId, string databaseName, string application, bool getPublicReports, int dataViewId, COEReportType? type, string category)
        {
            SetDatabaseName();

            if (!CanGetObject())
                throw new System.Security.SecurityException(Resources.UserNotAuthorizedForViewObject + " " + CLASSNAME);
            return DataPortal.Fetch<COEReportBOList>(new GetByLongCriteria(userId, databaseName, application, getPublicReports, dataViewId, type, category));
        }

        public static COEReportBOList GetCOEReportBOList(string name, string description)
        {
            SetDatabaseName();

            if (!CanGetObject())
                throw new System.Security.SecurityException(Resources.UserNotAuthorizedForViewObject + " " + CLASSNAME);
            return DataPortal.Fetch<COEReportBOList>(new GetByLongCriteria(name, description));
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
        private class GetByLongCriteria
        {

            public string _databaseName;
            public string _userId;
            public string _application;            
            public bool _getPublicReports = true;
            public int _dataViewId = -1;
            public string _name;
            public string _description;
            public COEReportType? _type;
            public string _category = null;

            //constructors
            public GetByLongCriteria(string userId, string databaseName, string application, bool getPublicReports, int dataViewId, COEReportType? type, string category)
            {
                this._userId = userId;
                this._databaseName = databaseName;
                this._application = application;                
                this._getPublicReports = getPublicReports;
                this._dataViewId = dataViewId;
                this._type = type;
                _category = category;
            }
            public GetByLongCriteria(string name, string description)
            {
                _name = name;
                _description = description;
            }
        }
        #endregion //Filter Criteria

        #region Data Access - Fetch

        private void DataPortal_Fetch(Criteria criteria)
        {
            RaiseListChangedEvents = false;
            if (_coeDAL == null) { LoadDAL(); }
            // Coverity Fix CID - 11574
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

        private void DataPortal_Fetch(GetByLongCriteria criteria) 
        {
            RaiseListChangedEvents = false;
            if (_coeDAL == null) { LoadDAL(); }
            
            // Coverity Fix CID - 11575
            if (_coeDAL != null)
            {
                using (SafeDataReader dr = _coeDAL.GetReports(criteria._userId, criteria._name, criteria._description, criteria._databaseName, criteria._application, criteria._getPublicReports, criteria._dataViewId, criteria._type, criteria._category))
                {
                    Fetch(dr);
                }
            }
            else
                throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));

            RaiseListChangedEvents = true;
        }

        protected void Fetch( SafeDataReader dr)
        {
            while (dr.Read())
            {
                COEReportBO reportData = new COEReportBO();
                reportData.LoadObjectFromReader(dr);
                
                this.Add(reportData);                    
            }
        }

        #endregion //Data Access - Fetch

        #region Data Access - Update

        protected override void DataPortal_Update()
        {
            RaiseListChangedEvents = false;
            if (_coeDAL == null) { LoadDAL(); }

            // loop through each deleted child object
            foreach (COEReportBO deletedChild in DeletedList)
                deletedChild.DeleteSelf(_coeDAL);
            DeletedList.Clear();

            // loop through each non-deleted child object
            foreach (COEReportBO child in this)
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
        private void LoadDAL(string  databaseName)
        {
            if (_dalFactory == null) { _dalFactory = new DALFactory(); }

            _dalFactory.GetDAL<DAL>(ref _coeDAL, _serviceName, databaseName, true);
        }
        #endregion //Data Access
    }
}
