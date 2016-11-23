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

namespace CambridgeSoft.COE.Framework.COESecurityService
{

    [Serializable()]
    public class COEUserReadOnlyBOList : ReadOnlyListBase<COEUserReadOnlyBOList, COEUserReadOnlyBO>
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
        #region Authorization Rules

        public static bool CanGetObject()
        {
            // TODO: customize to check user role
            //return ApplicationContext.User.IsInRole("");
            return true;
        }

        #endregion

        #region Factory Methods
        public COEUserReadOnlyBO GetUserByID(int personID)
        {
            foreach (COEUserReadOnlyBO user in this)
                if (user.PersonID == personID)
                    return user;
            return null;
        }

        public COEUserReadOnlyBO GetUserByUserID(string userID)
        {
            foreach (COEUserReadOnlyBO user in this)
                if (user.UserID == userID)
                    return user;
            return null;
        }
     

        public void AddUser(COEUserReadOnlyBO user)
        {
            if (!Contains(user.PersonID))
            {
                this.AllowNew = true;
                this.IsReadOnly = false;
                this.Add(user);
            }
            else
            {
                throw new InvalidOperationException("user already exists");
            }

        }

        public void RemoveUser(COEUserReadOnlyBO user)
        {
            foreach (COEUserReadOnlyBO hasUser in this)
            {
                if (hasUser.PersonID == user.PersonID)
                {
                    this.AllowRemove = true;
                    this.IsReadOnly = false;
                    Remove(user);
                    break;
                }
            }
        }

        public bool Contains(int personID)
        {
            bool boolReturn = false;
            foreach (COEUserReadOnlyBO hasUser in this)
            {
                if (hasUser.PersonID == personID)
                {
                    boolReturn = true;
                    break;
                }

            }
            return boolReturn;
        }
       
        //this method must be called prior to any other method inorder to set the database that the dal will use
        internal static void SetDatabaseName()
        {
            COEDatabaseName.Set(DALUtils.GetDefaultQualifyDbName(Resources.CentralizedStorageDB));
        }


        internal static void SetDatabaseName(string databaseName)
        {

            COEDatabaseName.Set(DALUtils.GetDefaultQualifyDbName(Resources.CentralizedStorageDB));

        }

        public static COEUserReadOnlyBOList GetList()
        {
            SetDatabaseName();
            return DataPortal.Fetch<COEUserReadOnlyBOList>();
        } 

        public static COEUserReadOnlyBOList GetListByApplication(List<string> applicationList)
        {
            SetDatabaseName();
            return DataPortal.Fetch<COEUserReadOnlyBOList>(new Criteria(FilterType.Application, applicationList, null));
        }

        public static COEUserReadOnlyBOList GetListByRole(List<string> roleList)
        {
            SetDatabaseName();
            return DataPortal.Fetch<COEUserReadOnlyBOList>(new Criteria(FilterType.Role, roleList, null));
        }

        public static COEUserReadOnlyBOList GetListByUserID(List<int> userIDList)
        {
            SetDatabaseName();
            return DataPortal.Fetch<COEUserReadOnlyBOList>(new Criteria(FilterType.User, null, userIDList));
        }
       
       

        public COEUserReadOnlyBOList()
        { /* require use of factory methods */ }

        #endregion

        #region public enums

        [Serializable()]
        private enum FilterType
        {
            Application,
            Role,
            User
        }
        #endregion

        #region Data Access

        [Serializable()]
        private class Criteria
        {
            private FilterType _filterType;
            private List<string> _filterList;
            private List<int> _intFilterList;
            public List<int> IntFilterList
            {
                get { return _intFilterList; }
            }

            public List<string> FilterList
            {
                get { return _filterList; }
            }

            public FilterType FilterType
            {
                get { return _filterType; }
            }
            public Criteria(FilterType filter, List<string> filterList, List<int> intFilterList)
            {
                _filterType = filter;
                _filterList = filterList;
                _intFilterList = intFilterList;
            }
        }
        private void DataPortal_Fetch()
        {

            RaiseListChangedEvents = false;
            IsReadOnly = false;
            if (_coeDAL == null) { LoadDAL(); }
            // Coverity Fix CID - 11601 
            if (_coeDAL != null)
            {
                using (SafeDataReader dr = _coeDAL.GetAllUsers())
                {
                    Fetch(dr);
                }
            }
            else
                throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));

            IsReadOnly = true;
            RaiseListChangedEvents = true;
        }

        private void DataPortal_Fetch(Criteria criteria)
        {
          
            RaiseListChangedEvents = false;
            IsReadOnly = false;
            if (_coeDAL == null) { LoadDAL(); }
            SafeDataReader dr = null;

            try
            {
                // Coverity Fix CID - 11602 
                if (_coeDAL != null)
                {
                    switch (criteria.FilterType)
                    {
                        case FilterType.Application:
                            dr = _coeDAL.GetUsersByApplication(criteria.FilterList);
                            break;
                        case FilterType.Role:
                            dr = _coeDAL.GetUsersByRole(criteria.FilterList);
                            break;
                        case FilterType.User:
                            dr = _coeDAL.GetUsersByID(criteria.IntFilterList);
                            break;
                    }
                }
                else
                    throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));

                if (dr != null)
                {
                    Fetch(dr);
                    dr.Close();
                }
            }
            catch (Exception) { throw; }           
            
            IsReadOnly = true;
            RaiseListChangedEvents = true;
        }

        #endregion

        protected void Fetch(SafeDataReader dr)
        {

            while (dr.Read())
            {
                try
                {
                    COEUserReadOnlyBO coeUserReadOnlyBO = new COEUserReadOnlyBO(
                     dr.GetInt16("PERSON_ID"),
                     dr.GetString("USER_CODE"),
                     dr.GetString("USER_ID"),
                     dr.GetInt16("SUPERVISOR_INTERNAL_ID"),
                     dr.GetString("TITLE"),
                     dr.GetString("FIRST_NAME"),
                     dr.GetString("MIDDLE_NAME"),
                     dr.GetString("LAST_NAME"),
                     dr.GetInt16("SITE_ID"),
                     dr.GetString("DEPARTMENT"),
                     dr.GetString("TELEPHONE"),
                     dr.GetString("EMAIL"),
                     dr.GetInt16("ACTIVE").Equals("1"));

                    this.Add(coeUserReadOnlyBO);
                }
                catch (Exception e)
                {

                    //Loopthrough
                }
            }
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
    

