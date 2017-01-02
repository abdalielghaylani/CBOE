using System;
using System.Collections.Generic;
using System.Text;
using Csla;
using Csla.Data;
using System.Data;
using CambridgeSoft.COE.Framework.Properties;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.COELoggingService;


namespace CambridgeSoft.COE.Framework.COEHitListService
{
    [Serializable()]
    public class COEHitListBOList : BusinessListBase<COEHitListBOList, COEHitListBO>
    {
        #region Member variables



        //variables data access
        [NonSerialized]
        private DAL _coeDAL = null;
        [NonSerialized]
        private DALFactory _dalFactory = new DALFactory();
        private string _serviceName = "COEHitList";
        private string _databaseName = null;


        // load an admin dal for use for table creation and deleteion
        [NonSerialized]
        internal DAL _adminDAL = null;

        [NonSerialized]
        static COELog _coeLog = COELog.GetSingleton("COEHitList");


        #endregion

        #region Constructors

        private COEHitListBOList()
        {

        }


        #endregion

        #region Authorization rules

        public static bool CanGetHitListCollection()
        {
            return true;
        }

        #endregion

        #region Factory methods

        //this method must be called prior to any other method inorder to set the database that the dal will use
        internal static void SetDatabaseName()
        {
            COEDatabaseName.Set(DALUtils.GetDefaultQualifyDbName(Resources.CentralizedStorageDB));
        }

        internal static void SetDatabaseName(string databaseName)
        {
            COEDatabaseName.Set(DALUtils.GetDefaultQualifyDbName(databaseName));
        }

        public static COEHitListBOList GetSavedHitListList(string databaseName)
        {
            SetDatabaseName();
            try
            {
                return DataPortal.Fetch<COEHitListBOList>(new Criteria(HitListType.SAVED, databaseName));

            }
            catch (Exception)
            {

                throw;
            }
        }
        public static COEHitListBOList GetTempHitListList(string databaseName)
        {
            SetDatabaseName();
            try
            {
                return DataPortal.Fetch<COEHitListBOList>(new Criteria(HitListType.TEMP, databaseName));

            }
            catch (Exception)
            {

                throw;
            }
        }


        public static COEHitListBOList GetSavedHitListList(string databaseName, string userID, int dataviewID)
        {
            SetDatabaseName();
            try
            {
                return DataPortal.Fetch<COEHitListBOList>(new SavedCriteria(userID, databaseName, dataviewID));
            }
            catch (Exception)
            {

                throw;
            }
        }

        public static COEHitListBOList GetRecentHitLists(string databaseName, string userID, int dataviewID, int quantityToRetrieve)
        {
            SetDatabaseName();
            try
            {
                return DataPortal.Fetch<COEHitListBOList>(new RecentCriteria(userID, databaseName, dataviewID, quantityToRetrieve));

            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion

        #region Criteria classes
        [Serializable()]
        protected class RecentCriteria
        {
            protected int _dataViewID;
            protected string _userID;
            protected string _databaseName;
            protected int _quantityToRetrieve;

            public RecentCriteria(string userID, string databaseName, int dataviewID, int quantityToRetrieve)
            {
                _userID = userID;
                _databaseName = databaseName;
                _dataViewID = dataviewID;
                _quantityToRetrieve = quantityToRetrieve;
            }

            public string UserID
            {
                get { return _userID; }
            }

            public string DatabaseName
            {
                get { return _databaseName; }
            }

            public int QuantityToRetrieve
            {
                get { return _quantityToRetrieve; }
            }

            public int DataViewID
            {
                get { return _dataViewID; }
            }
        }

        [Serializable()]
        protected class SavedCriteria
        {
            protected int _dataViewID;
            protected string _userID;
            protected string _databaseName;

            public SavedCriteria(string userID, string databaseName, int dataviewID)
            {
                _userID = userID;
                _databaseName = databaseName;
                _dataViewID = dataviewID;
            }

            public string UserID
            {
                get { return _userID; }
            }

            public string DatabaseName
            {
                get { return _databaseName; }
            }

            public int DataViewID
            {
                get { return _dataViewID; }
            }
        }

        [Serializable()]
        protected class Criteria
        {
            protected HitListType hitListType;
            protected string _databaseName;

            public Criteria(HitListType hitListType, string databaseName)
            {
                this.hitListType = hitListType;
                this._databaseName = databaseName;
            }

            public HitListType HitListType
            {
                get
                {
                    return this.hitListType;
                }
            }

            public string DatabaseName
            {
                get
                {
                    return this._databaseName;
                }
            }
        }

        #endregion

        #region Data access

        protected void DataPortal_Fetch(Criteria criteria)
        {
            HitListType hitListType = criteria.HitListType;
            try
            {
                this.RaiseListChangedEvents = false;
                SafeDataReader dr = null;
                try
                {
                    if (_coeDAL == null) { LoadDAL(); }
                    // Coverity Fix CID - 11560
                    if (_coeDAL != null)
                    {
                        if (criteria.DatabaseName == string.Empty)
                        {
                            dr = (SafeDataReader)_coeDAL.GetHitLists(hitListType);
                        }
                        else
                        {
                            dr = (SafeDataReader)_coeDAL.GetHitLists(hitListType, criteria.DatabaseName);

                        }
                        Fetch(hitListType, dr);
                    }
                    else
                        throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));            
                }
                finally
                {
                    dr.Close();
                }
                this.RaiseListChangedEvents = true;
            }
            catch (Exception)
            {

                throw;
            }
        }

        protected void DataPortal_Fetch(RecentCriteria criteria)
        {
            try
            {
                this.RaiseListChangedEvents = false;

                if (_coeDAL == null) { LoadDAL(); }
                // Coverity Fix CID - 11561
                if (_coeDAL != null)
                {
                    using (SafeDataReader dr = (SafeDataReader)_coeDAL.GetRecentHitLists(criteria.UserID, criteria.DataViewID, criteria.DatabaseName, criteria.QuantityToRetrieve))
                    {
                        Fetch(HitListType.TEMP, dr);
                    }
                    this.RaiseListChangedEvents = true;
                }
                else
                    throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));
            }
            catch (Exception)
            {

                throw;
            }
        }

        protected void DataPortal_Fetch(SavedCriteria criteria)
        {
            try
            {
                this.RaiseListChangedEvents = false;

                if (_coeDAL == null) { LoadDAL(); }
                // Coverity Fix CID - 11562 
                if (_coeDAL != null)
                {
                    using (SafeDataReader dr = (SafeDataReader)_coeDAL.GetSavedHitLists(criteria.UserID, criteria.DataViewID, criteria.DatabaseName))
                    {
                        Fetch(HitListType.SAVED, dr);
                    }
                }
                else
                    throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));

                this.RaiseListChangedEvents = true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        protected void Fetch(HitListType hitlistType, SafeDataReader dr)
        {
            while (dr.Read())
            {
                COEHitListBO hitList = new COEHitListBO(
                    dr.GetInt32("id"),
                    dr.GetString("name"),
                    dr.GetInt32("hitlistid"),
                    dr.GetString("description"),
                    dr.GetString("user_id"),
                    (dr.GetString("is_public").Equals("1") ? true : false),
                    dr.GetInt32("number_hits"),
                    dr.GetSmartDate("Date_Created"),
                    string.IsNullOrEmpty(dr.GetString("TYPE")) ? hitlistType : (HitListType) Enum.Parse(typeof(HitListType), dr.GetString("TYPE")),
                    dr.GetString("database"),
                    dr.GetInt32("dataview_id"),
                    dr.GetInt32("search_criteria_id"),
                    dr.GetString("search_criteria_type"));
                this.Add(hitList);
            }
        }

        #endregion
        private void LoadDAL()
        {
            try
            {
                if (_dalFactory == null) { _dalFactory = new DALFactory(); }
                _dalFactory.GetDAL<DAL>(ref _coeDAL, _serviceName, COEDatabaseName.Get().ToString(), true); ;

            }
            catch (Exception)
            {

                throw;
            }
        }

        public HitListInfo Get(int hitListId)
        {
            HitListInfo resultHitListInfo = new HitListInfo();
            foreach (COEHitListBO hitList in this.Items)
            {
                if (hitList.ID == hitListId)
                {
                    resultHitListInfo.Database = hitList.DatabaseName;
                    resultHitListInfo.HitListID = hitListId;
                    resultHitListInfo.RecordCount = hitList.NumHits;
                    resultHitListInfo.HitListType = hitList.HitListType;
                    break;
                }
            }

            return resultHitListInfo;
        }
    }
}
