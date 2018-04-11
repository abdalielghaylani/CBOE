using System;
using System.Data;
using Csla;
using Csla.Data;
using Csla.Validation;
using System.Collections.Generic;
using System.Configuration;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.COELoggingService;
using CambridgeSoft.COE.Framework.COEDataViewService;
using CambridgeSoft.COE.Framework.Properties;
using System.ComponentModel;

namespace CambridgeSoft.COE.Framework.COEDatabasePublishingService
{
    [Serializable()]
    public class COEDatabaseBOList : Csla.BusinessListBase<COEDatabaseBOList, COEDatabaseBO>
    {
        //variables data access
        [NonSerialized]
        private DAL _coeDAL = null;
        [NonSerialized]
        private DALFactory _dalFactory = new DALFactory();
        private string _serviceName = "COEDatabasePublishing";
        private string _databaseName;
        static COELog _coeLog = COELog.GetSingleton("COEDatabasePublishing");
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

        private COEDatabaseBOList()
        { /* require use of factory method */ }



        public static COEDatabaseBOList NewList()
        {
            SetDatabaseName();
            if (!CanAddObject())
                throw new System.Security.SecurityException(Resources.UserNotAuthorizedForAddObject + " COEDatabaseBOList");
            return new COEDatabaseBOList();
        }
        public static COEDatabaseBOList GetList(bool isPublished)
        {
            SetDatabaseName();

            if (!CanGetObject())
                throw new System.Security.SecurityException(Resources.UserNotAuthorizedForViewObject + " COEDatabaseBOList");
            return DataPortal.Fetch<COEDatabaseBOList>(new CriteriaPublished(isPublished));
        }

        public static COEDatabaseBOList GetList()
        {
            SetDatabaseName();

            if (!CanGetObject())
                throw new System.Security.SecurityException(Resources.UserNotAuthorizedForViewObject + " COEDatabaseBOList");
            return DataPortal.Fetch<COEDatabaseBOList>(new Criteria());
        }

        public COEDatabaseBO GetDatabase(string name)
        {
            foreach (COEDatabaseBO database in this)
                if (database.Name == name)
                    return database;

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


            //constructors
            public Criteria()
            {
            }
        }
        [Serializable()]
        private class CriteriaPublished
        {
            internal bool _isPublished = false;
            //constructors
            public CriteriaPublished(bool isPublished)
            {
                _isPublished = isPublished;
            }
        }

        #endregion //Filter Criteria

        #region Data Access - Fetch
        private void DataPortal_Fetch(Criteria criteria)
        {
            RaiseListChangedEvents = false;
            LoadDAL();

            using (SafeDataReader dr = _coeDAL.GetPublishedDatabases())
            {
                Fetch(dr);
            }
            _coeDAL = null;
            _dalFactory = null;
            LoadSecurityDAL();
            DataTable dt = _coeDAL.GetUnPublishedDatabases();
            Fetch(dt);
            RaiseListChangedEvents = true;
        }

        private void DataPortal_Fetch(CriteriaPublished criteria)
        {
            RaiseListChangedEvents = false;

            if (criteria._isPublished == true)
            {
                LoadDAL();
                using (SafeDataReader dr = _coeDAL.GetPublishedDatabases())
                {
                    Fetch(dr);
                }
            }
            else
            {
                _coeDAL = null;
                _dalFactory = null;
                LoadSecurityDAL();
                DataTable dt = _coeDAL.GetUnPublishedDatabases();
                Fetch(dt);
            }
            RaiseListChangedEvents = true;
        }

        protected void Fetch(SafeDataReader dr)
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
                        true);

                    this.Add(databaseData);
                }
                catch (Exception e)
                {

                }
            }
        }

        protected void Fetch(DataTable dt)
        {
            for (int i = 0; i < dt.Rows.Count - 1; i++)
            {

                DataRow dr = dt.Rows[i];
                COEDatabaseBO databaseData = new COEDatabaseBO(
                        0,
                        dr["OWNER"].ToString(),
                         new SmartDate(),
                        null,
                        false);

                this.Add(databaseData);

            }

        }



        #endregion //Data Access - Fetch


        #region Data Access - Update
        protected override void DataPortal_Update()
        {
            RaiseListChangedEvents = false;
            if (_coeDAL == null) { LoadDAL(); }

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


        private void LoadDAL()
        {
            if (_dalFactory == null) { _dalFactory = new DALFactory(); }
            _dalFactory.GetDAL<DAL>(ref _coeDAL, _serviceName, COEDatabaseName.Get().ToString(), true);
        }
        private void LoadSecurityDAL()
        {

            if (_dalFactory == null) { _dalFactory = new DALFactory(); }
            _dalFactory.GetDAL<CambridgeSoft.COE.Framework.COEDatabasePublishingService.DAL>(ref _coeDAL, _serviceName, Resources.SecurityDatabaseName.ToString(), true);
        }
        #endregion //Data Access


    }
}