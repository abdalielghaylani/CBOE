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
    public class COERoleReadOnlyBOList :
      ReadOnlyListBase<COERoleReadOnlyBOList, COERoleReadOnlyBO>
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
        public COERoleReadOnlyBO GetRoleByID(int roleID)
        {
            foreach (COERoleReadOnlyBO role in this)
                if (role.RoleID == roleID)
                    return role;
            return null;
        }
        public COERoleReadOnlyBO GetRoleByRoleName(string roleName)
        {
            foreach (COERoleReadOnlyBO role in this)
                if (role.RoleName == roleName)
                    return role;
            return null;
        }
        public void AddRole(COERoleReadOnlyBO role)
        {
            if (!Contains(role.RoleID))
            {
                this.AllowNew = true;
                this.IsReadOnly = false;
                this.Add(role);
            }else
            {
                throw new InvalidOperationException("role already exists for user");
            }

        }

        public void RemoveRole(COERoleReadOnlyBO role)
        {
            foreach (COERoleReadOnlyBO hasRole in this)
            {
                if (hasRole.RoleID == role.RoleID)
                {
                    this.AllowRemove = true;
                    this.IsReadOnly = false;
                    Remove(role);
                    break;
                }
            }
        }

        public bool Contains(int roleID)
        {
            bool boolReturn = false;
            foreach (COERoleReadOnlyBO hasRole in this)
            {
                if (hasRole.RoleID == roleID)
                {
                    boolReturn =  true;
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

        public static COERoleReadOnlyBOList GetList()
        {
            SetDatabaseName();
            return DataPortal.Fetch<COERoleReadOnlyBOList>();
        }

        public static COERoleReadOnlyBOList GetListByApplication(List<string> applicationList)
        {
            SetDatabaseName();
            return DataPortal.Fetch<COERoleReadOnlyBOList>(new Criteria(FilterType.Application, applicationList, null));
        }

        public static COERoleReadOnlyBOList GetListByUser(List<string> userList)
        {
            SetDatabaseName();
            return DataPortal.Fetch<COERoleReadOnlyBOList>(new Criteria(FilterType.User, userList, null));
        }

        public static COERoleReadOnlyBOList GetListByRoleID(List<int> roleList)
        {
            SetDatabaseName();
            return DataPortal.Fetch<COERoleReadOnlyBOList>(new Criteria(FilterType.Role, null, roleList));
        }



        public COERoleReadOnlyBOList()
        { /* require use of factory methods */ }

        #endregion

        #region public enums

        [Serializable()]
        private enum FilterType
        {
            Application,
            User,
            Role
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
            
            // Coverity Fix CID - 11598 
            if (_coeDAL != null)
            {
                SafeDataReader dr = null;
                dr = _coeDAL.GetAllRoles();
                Fetch(dr);
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
            
            // Coverity Fix CID - 11599 
            if (_coeDAL != null)
            {
                switch (criteria.FilterType)
                {
                    case FilterType.Application:
                        dr = _coeDAL.GetRolesByApplication(criteria.FilterList);
                        break;
                    case FilterType.User:
                        dr = _coeDAL.GetRolesByUser(criteria.FilterList);
                        break;
                    case FilterType.Role:
                        dr = _coeDAL.GetRolesByRoleIDS(criteria.IntFilterList);
                        break;
                }
            }
            else
                throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));

            if (dr != null) Fetch(dr);
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
                    COERoleReadOnlyBO coeRoleReadOnlyBO = new COERoleReadOnlyBO(
                     dr.GetInt16("ROLE_ID"),
                     dr.GetString("ROLE_NAME"));
                   
                    this.Add(coeRoleReadOnlyBO);
                }
                catch (Exception e)
                {
                    dr.Close();
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


