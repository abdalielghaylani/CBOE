using System;
using System.Data;
using Csla;
using Csla.Data;
using Csla.Validation;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.Configuration;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.COELoggingService;
using CambridgeSoft.COE.Framework.Properties;

namespace CambridgeSoft.COE.Framework.COESecurityService
{

    [Serializable()]
    public class COEApplicationReadOnlyBOList :
      ReadOnlyListBase<COEApplicationReadOnlyBOList, COEApplicationReadOnlyBO>
    {
        #region member variables
        //variables data access
        [NonSerialized]
        private DAL _coeDAL = null;
        [NonSerialized]
        private DALFactory _dalFactory = new DALFactory();
        private string _serviceName = "COESecurity";

        [NonSerialized]
        static COELog _coeLog = COELog.GetSingleton("COESecurity");
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

     
       

        public static COEApplicationReadOnlyBOList GetList()
        {
            SetDatabaseName();
            return DataPortal.Fetch<COEApplicationReadOnlyBOList>(new Criteria());
        }



        public COEApplicationReadOnlyBOList()
        { /* require use of factory methods */ }

        #endregion

        #region public enums

       
        #endregion

        #region Data Access

        [Serializable()]
        private class Criteria
        {
           
        }



        
        private void DataPortal_Fetch(Criteria criteria)
        {

            RaiseListChangedEvents = false;
            IsReadOnly = false;
            if (_coeDAL == null) { LoadDAL(); }
            // Coverity Fix CID - 11604 
            if (_coeDAL != null)
            {
                SafeDataReader dr = null;
                dr = _coeDAL.GetAllApplicationNames();
                if (dr != null) Fetch(dr);
            }
            else
                throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));

            IsReadOnly = true;
            RaiseListChangedEvents = true;
        }

        #endregion

        protected void Fetch(SafeDataReader dr)
        {
            //first add all app entry
            COEApplicationReadOnlyBO coeApplicationReadOnlyBO1 = new COEApplicationReadOnlyBO(
                     1,
                     "All CS Applications");

            this.Add(coeApplicationReadOnlyBO1);

            while (dr.Read())
            {
                try
                {
                    COEApplicationReadOnlyBO coeApplicationReadOnlyBO = new COEApplicationReadOnlyBO(
                     dr.GetInt16("RID"),
                     dr.GetString("COEIDENTIFIER").ToUpper());

                    if(!this.Contains(coeApplicationReadOnlyBO))
                        this.Add(coeApplicationReadOnlyBO);
                }
                catch (Exception e)
                {

                    //Loopthrough
                }
            }
            dr.Close();


        }

        private void LoadDAL()
        {
            //Load DAL for database calls. The database is actually alwas set to the centralizedDataStorageDB in the resources. However, I'm leaving
            //the code untouced just in case we need to go back to non-centralized.
            if (_dalFactory == null) { _dalFactory = new DALFactory(); }
            _dalFactory.GetDAL<DAL>(ref _coeDAL, _serviceName, COEDatabaseName.Get().ToString(), true);
        }
    }
}


