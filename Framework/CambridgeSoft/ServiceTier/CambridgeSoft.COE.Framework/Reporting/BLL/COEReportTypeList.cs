using System;
using System.Collections.Generic;
using System.Text;
using Csla;
using Csla.Data;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.Properties;
using CambridgeSoft.COE.Framework.COEReportingService;

namespace CambridgeSoft.COE.Framework.Reporting.BLL
{
    [Serializable]
    internal class COEReportTypeList: NameValueListBase<int, string>
    {
        #region Variables
        //variables data access
        [NonSerialized]
        private DAL _coeDAL = null;
        [NonSerialized]
        private DALFactory _dalFactory = new DALFactory();
        private string _serviceName = Resources.ReportingServiceName;

        [NonSerialized]
        private const string CLASSNAME = "COEReportTemplateBOList";
        #endregion

        #region Factory Methods
        #region Constructors
        
        public static bool CanGetObject()
        {
            //TO DO:  need to add authorization access code
            return true;
        }

        private COEReportTypeList()
        {

        }
        #endregion

        public static COEReportTypeList Get()
        {
            SetDatabaseName();
            if (!CanGetObject())
                throw new System.Security.SecurityException(Resources.UserNotAuthorizedForViewObject + " " + CLASSNAME);

            return DataPortal.Fetch<COEReportTypeList>();
        }
        #endregion

        #region DataAccess
        protected void DataPortal_Fetch()
        {
            this.RaiseListChangedEvents = false;
            this.IsReadOnly = false;
            
            if (_coeDAL == null) { LoadDAL(); }
            /*using (SafeDataReader dataReader = _coeDAL.GetReportTypes())
            {
                this.Fetch(dataReader);
            }*/

            this.IsReadOnly = true;
            this.RaiseListChangedEvents = true;
        }
        
        protected void Fetch(SafeDataReader dataReader)
        {
            while (dataReader.Read())
            {
                NameValuePair namevaluePair = new NameValueListBase<int, string>.NameValuePair(
                    dataReader.GetInt32("ID"),
                    dataReader.GetString("TYPE")
                    );

                this.Add(namevaluePair);
            }
        }
        #endregion

        #region private methods
        private void LoadDAL()
        {
            if (_dalFactory == null) { _dalFactory = new DALFactory(); }

            _dalFactory.GetDAL<DAL>(ref _coeDAL, _serviceName, COEDatabaseName.Get().ToString(), true);
        }
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
        #endregion
    }
}
