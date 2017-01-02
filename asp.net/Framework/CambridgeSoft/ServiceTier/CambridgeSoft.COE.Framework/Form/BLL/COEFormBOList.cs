using System;
using System.Data;
using Csla;
using Csla.Data;
using Csla.Validation;
using System.Configuration;
using System.Collections.Generic;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.Properties;
using CambridgeSoft.COE.Framework.COELoggingService;
using CambridgeSoft.COE.Framework.Common.Messaging;


namespace CambridgeSoft.COE.Framework.COEFormService
{
    [Serializable()]
    public class COEFormBOList : Csla.BusinessListBase<COEFormBOList, COEFormBO>
    {
        //variables data access
        [NonSerialized]
        private DAL _coeDAL = null;
        [NonSerialized]
        private DALFactory _dalFactory = new DALFactory();
        private string _serviceName = "COEForm";
        private string _databaseName = string.Empty;
        [NonSerialized]
         static COELog _coeLog = COELog.GetSingleton("COEForm");
        #region Factory Methods
        private COEFormBOList()
        { /* require use of factory method */ }

        //this method must be called prior to any other method inorder to set the database that the dal will use
        internal static void SetDatabaseName()
        {
            COEDatabaseName.Set(DALUtils.GetDefaultQualifyDbName(Resources.CentralizedStorageDB));
        }

        internal static void SetDatabaseName(string databaseName)
        {
            COEDatabaseName.Set(DALUtils.GetDefaultQualifyDbName(databaseName));
        } 
        
        public static COEFormBOList NewList(string databaseName)
        {
            SetDatabaseName();

            if (!CanAddObject())
                throw new System.Security.SecurityException(Resources.UserNotAuthorizedForAddObject + " COEFormBOList");
            return new COEFormBOList();
        }
        public static COEFormBOList GetCOEFormBOList()
        {
            SetDatabaseName();

            if (!CanGetObject())
                throw new System.Security.SecurityException(Resources.UserNotAuthorizedForViewObject + " COEFormBOList");
            return DataPortal.Fetch<COEFormBOList>(new Criteria());
        }
        public static COEFormBOList GetCOEFormBOListbyUser(string userName)
        {
            SetDatabaseName();

            if (!CanGetObject())
                throw new System.Security.SecurityException(Resources.UserNotAuthorizedForViewObject + " COEFormBOList");
            return DataPortal.Fetch<COEFormBOList>(new GetByUserCriteria(userName));
        }
        public static COEFormBOList GetCOEFormBOListbyDatabase(string databaseName)
        {
            SetDatabaseName();

            if (!CanGetObject())
                throw new System.Security.SecurityException(Resources.UserNotAuthorizedForViewObject + " COEFormBOList");
            return DataPortal.Fetch<COEFormBOList>(new GetByDatabaseNameCriteria(databaseName));
        }

        public static COEFormBOList GetCOEFormBOList(string username, string databaseName, string application, string formtype, bool getPublicForms) {
            SetDatabaseName();

            if(!CanGetObject())
                throw new System.Security.SecurityException(Resources.UserNotAuthorizedForViewObject + " COEFormBOList");
            return DataPortal.Fetch<COEFormBOList>(new GetByLongCriteria(username, databaseName, application, formtype,getPublicForms));
        }

        public static COEFormBOList GetCOEFormBOList(string username, string databaseName, string application, int formtypeid, bool getPublicForms) {
            SetDatabaseName();

            if(!CanGetObject())
                throw new System.Security.SecurityException(Resources.UserNotAuthorizedForViewObject + " COEFormBOList");
            return DataPortal.Fetch<COEFormBOList>(new GetByLongCriteria(username, databaseName, application, formtypeid, getPublicForms));
        }

        public static COEFormBOList GetCOEFormBOListbyAllDatabases()
        {
            SetDatabaseName();

            if (!CanGetObject())
                throw new System.Security.SecurityException(Resources.UserNotAuthorizedForViewObject + " COEFormBOList");
            return DataPortal.Fetch<COEFormBOList>(new GetByAllDatabaseNameCriteria());
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
            public string _databaseName;


            //constructors
            public GetByDatabaseNameCriteria(string databaseName)
            {
               
                _databaseName = databaseName;
            }
        }
        [Serializable()]
        private class GetByAllDatabaseNameCriteria
        {
            //constructors
            public GetByAllDatabaseNameCriteria()
            {

            }
        }

        [Serializable()]
        private class GetByLongCriteria {
            public string DatabaseName;
            public string UserName;
            public string Application;
            public string FormType;
            public int FormTypeId = -1;
            public bool GetPublicForms;

            //constructors
            public GetByLongCriteria(string username, string databaseName, string application, string formtype, bool getPublicForms) {
                this.UserName = username;
                this.DatabaseName = databaseName;
                this.Application = application;
                this.FormType = formtype;
                this.GetPublicForms = getPublicForms;
            }

            public GetByLongCriteria(string username, string databaseName, string application, int formtypeid, bool getPublicForms) {
                this.UserName = username;
                this.DatabaseName = databaseName;
                this.Application = application;
                this.FormTypeId = formtypeid;
                this.GetPublicForms = getPublicForms;
            }
        }
        #endregion //Filter Criteria

        #region Data Access - Fetch
        private void DataPortal_Fetch(Criteria criteria)
        {
            RaiseListChangedEvents = false;
            if (_coeDAL == null) { LoadDAL(); }
             // Coverity Fix CID - 11535
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
            // Coverity Fix CID - 11539
            if (_coeDAL != null)
            {
                using (SafeDataReader dr = _coeDAL.GetUserForms(criteria._userName))
                {
                    Fetch(dr);
                }
            }
            else
                throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));

            RaiseListChangedEvents = true;
        }
        private void DataPortal_Fetch(GetByDatabaseNameCriteria criteria)
        {
            RaiseListChangedEvents = false;
            if (_coeDAL == null) { LoadDAL(); }
            // Coverity Fix CID - 11537
            if (_coeDAL != null)
            {
                using (SafeDataReader dr = _coeDAL.GetForms(null, criteria._databaseName, null, 0, true))
                {
                    Fetch(dr);
                }
            }
            else
                throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));

            RaiseListChangedEvents = true;
        }
        private void DataPortal_Fetch(GetByAllDatabaseNameCriteria criteria)
        {
            RaiseListChangedEvents = false;
            if(_coeDAL == null) { LoadDAL(); }
            // Coverity Fix CID - 11536
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
            SafeDataReader dr = null;
            _dalFactory = new DALFactory();
            if(_coeDAL == null) { this.LoadDAL(); }

            // Coverity Fix CID - 11538
            if (_coeDAL != null)
            {
                try
                {
                    if (criteria.FormTypeId >= 0)
                        dr = _coeDAL.GetForms(criteria.UserName, criteria.DatabaseName, criteria.Application, criteria.FormTypeId, criteria.GetPublicForms);
                    else
                        dr = _coeDAL.GetForms(criteria.UserName, criteria.DatabaseName, criteria.Application, criteria.FormType, criteria.GetPublicForms);

                    Fetch(dr);
                }
                finally
                {
                    dr.Close();
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
                try
                {
                    COEFormBO formBOData = new COEFormBO(
                        dr.GetInt32("ID"),
                        dr.GetString("NAME"),
                        dr.GetString("DESCRIPTION"),
                        dr.GetString("USER_ID"),
                        (dr.GetString("IS_PUBLIC").Equals("1") ? true : false),
                        dr.GetInt32("FORMGROUP"),
                        dr.GetSmartDate("DATE_CREATED"),
                        null,
                        //FormGroup.GetFormGroup(dr.GetString("COEFORM")),
                        dr.GetString("DATABASE"),
                        dr.GetString("APPLICATION"),
                        dr.GetInt16("FORMTYPEID"),
                        dr.GetString("FORMTYPE")); //if we want to support database specific storage then we will need to use the databaseName parameter
                    this.Add(formBOData);
                }
                catch (Exception e)
                {
                   
                    //Loopthrough
                }
            }
        }

        #endregion //Data Access - Fetch

        #region Data Access - Update
        protected override void DataPortal_Update()
        {
            RaiseListChangedEvents = false;
            if (_coeDAL == null) { LoadDAL(); }

            // loop through each deleted child object
            foreach (COEFormBO deletedChild in DeletedList)
                deletedChild.DeleteSelf(_coeDAL);
            DeletedList.Clear();

            // loop through each non-deleted child object
            foreach (COEFormBO child in this)
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
    

