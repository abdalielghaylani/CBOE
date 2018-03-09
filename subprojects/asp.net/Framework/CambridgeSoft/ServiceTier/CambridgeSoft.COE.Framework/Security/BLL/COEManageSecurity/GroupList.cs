using System;
using System.Data;
using System.Collections.Generic;
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
    public class  GroupList:
      NameValueListBase<int, string>
    {
        #region member variables

            private static GroupList _list;

            //variables data access
            [NonSerialized]
            private DAL _coeDAL = null;
            [NonSerialized]
            private DALFactory _dalFactory = new DALFactory();
            private string _serviceName = "COESecurity";

            [NonSerialized]
            static COELog _coeLog = COELog.GetSingleton("COESecurity");

        
        #endregion
        #region Business Methods



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

        /// <summary>
        /// Returns a list of groups.
        /// </summary>
        public static GroupList GetList(int groupOrgID, int groupID)
        {
                 SetDatabaseName();
                _list = DataPortal.Fetch<GroupList>
                  (new GroupOrgCriteria(groupOrgID, groupID));
            return _list;
        }

        /// <summary>
        /// Clears the in-memory GroupList cache
        /// so the list of roles is reloaded on
        /// next request.
        /// </summary>
        public static void InvalidateCache()
        {
            _list = null;
        }

        private GroupList()
        { /* require use of factory methods */ }

        #endregion
        #region critiera
        [Serializable()]
        private class GroupOrgCriteria
        {
            internal int _groupOrgID = -1;
            internal int _groupID = -1;
            //constructors
            public GroupOrgCriteria(int groupOrgID, int groupID)
            {
                _groupOrgID = groupOrgID;
                _groupID = groupID;
            }
        }
        #endregion
        #region Data Access

        private void DataPortal_Fetch(GroupOrgCriteria criteria)
        {
            this.RaiseListChangedEvents = false;
            try
            {
                _coeLog.LogStart(string.Empty, 1);
                if (_coeDAL == null) { LoadDAL(); }
                // Coverity Fix CID - 11634 
                if (_coeDAL != null)
                {
                    DataTable dt = _coeDAL.GetGroupList(criteria._groupOrgID, criteria._groupID);
                    FetchObject(dt);
                }
                else
                    throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));

                _coeLog.LogEnd(string.Empty, 1);
            }
            catch (Exception)
            {

                throw;
            }
            this.RaiseListChangedEvents = true;
        }

         private void FetchObject(DataTable dt){
             IsReadOnly = false;

             try
             {
                 for (int i = 0; i < dt.Rows.Count; i++)
                 {
                     IsReadOnly = false;
                     DataRow dr = dt.Rows[i];
                     this.Add(new NameValuePair(Convert.ToInt32(dr["GROUP_ID"].ToString()), dr["GROUP_NAME"].ToString()));

                 }
             }
             catch (System.Exception ex)
             {

             }
             
             IsReadOnly = true;
         }

        
          private void LoadDAL()
            {
                if (_dalFactory == null) { _dalFactory = new DALFactory(); }
                _dalFactory.GetDAL<DAL>(ref _coeDAL, _serviceName, COEDatabaseName.Get().ToString(), true);
            }	
            #endregion
        
    }
}

