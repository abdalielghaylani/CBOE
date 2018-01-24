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
    public class COEApplicationReadOnlyBO : ReadOnlyBase<COEApplicationReadOnlyBO>
    {
        #region member variables

        // TODO: add your own fields, properties and methods
       
        private string _applicationName = string.Empty;
        private int _id = -1;

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
        internal COEApplicationReadOnlyBO(int id, string applicationName)
        {
            _id = id;
            _applicationName = applicationName;
        }

        #endregion

        #region properties
      
        public string ApplicationName
        {
            get { return _applicationName; }
        }

        public int ID
        {
            get { return _id; }
        }
        protected override object GetIdValue()
        {
            return _id;
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

        //public static COEApplicationReadOnlyBO Get(string UserName)
        //{
        //    SetDatabaseName();
        //    return DataPortal.Fetch<COERoleReadOnlyBO>(new Criteria());
        //}

        private COEApplicationReadOnlyBO()
        { /* require use of factory methods */ }



        #endregion

        #region Criteria

        [Serializable()]
        private class Criteria
        {

            
        }
        #endregion //Criteria
    }

}
