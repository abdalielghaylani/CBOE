using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Xml;
using System.Xml.XPath;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.Properties;
using Csla;
using Csla.Data;
using Csla.Validation;
using CambridgeSoft.COE.Framework.COELoggingService;
using CambridgeSoft.COE.Framework.COEDisplayDataBrokerService;
using System.Collections;
using CambridgeSoft.COE.Framework.Caching;
using System.Reflection;

namespace CambridgeSoft.COE.Framework.COEPickListPickerService
{
    /// <summary>
    /// <para>
    /// Main class of the service COEPickListPicker, which provides the ability to retrieve name value list values from database.
    /// </para>
    /// <para>
    /// This service is mainly intended for those typical cases of retriveing all the allowed status for a particular object or even
    /// more typical for retrieving all the countries/states.
    /// </para>
    /// <para>
    /// For this service to work you have to configure two tables in the schema you are working with.
    /// </para>
    /// <list type="bullet">
    ///   <item>PickList: This table contains a list of values for an specific domain. For instance all the values for the domain Country.</item>
    ///   <item>PickListDomain: This table contains the list of known domains.</item>
    /// </list>
    /// <para>Once the tables are set you can start using this service for retrieven the NameValueList for each domain you require.</para>
    /// </summary>
    [Serializable()]
    public class PickListNameValueList : NameValueListBase<string, string>, IKeyValueListHolder, IPicklistService
    {
        #region Variables data access
        [NonSerialized, NotUndoable]
        private DAL _coeDAL = null;
        [NonSerialized, NotUndoable]
        private DALFactory _dalFactory = new DALFactory();
        private string _serviceName = "COEPickListPicker";
        #endregion

        #region Factory Methods
        private int _domainId;
        private string _domainName;
        private static PickListNameValueList _list;
        [NonSerialized]
         static COELog _coeLog = COELog.GetSingleton("COEPickListPicker");


        #region Variables private
        private string _table;
        private string _IDColumn;
        private string _displayColumn;
        private string _whereFilter;
        private string _orderByFilter;
        private List<string> _columns;
        private string _description = null;
        private string _sqlQuery = string.Empty; 
        private List<string> _whereClauseList;
        private List<string> _orderClauseList;
        private PickListStatus _pickListStatus = CambridgeSoft.COE.Framework.COEPickListPickerService.PickListStatus.Active;
        #endregion

        #region Properties public 

        /// <summary>
        /// Returns sql query
        /// </summary>
        private  string SqlQuery
        {
            get
            {
                return _sqlQuery;
            }
            set
            {
                _sqlQuery = value;
            }
        }
       
        /// <summary>
        /// Identifier of the database column
        /// </summary>
        public List<string> Columns
        {
            get
            {
                if (_columns == null)
                    _columns = new List<string>();
                return _columns;
            }
            set
            {
                _columns = value;
            }
        }

        /// <summary>
        /// The name of the database table or view from which the picklist's values
        /// will be derived.
        /// </summary>
        public string Table
        {
            get
            {
                return _table;
            }
            set
            {
                _table = value;
            }
        }

        /// <summary>
        /// The column that will contain the value members for the picklist.
        /// </summary>
        public string IdColumn
        {
            get
            {
                return _IDColumn;
            }
            set
            {
                _IDColumn = value;
                if (!string.IsNullOrEmpty(_IDColumn))
                    this.Columns.Add(_IDColumn.Trim());
            }
        }

        /// <summary>
        /// The column that will contain the display members for the picklist.
        /// </summary>
        public string DisplayColumn
        {
            get
            {
                return _displayColumn;
            }
            set
            {
                _displayColumn = value;
                Columns.Add(_displayColumn);
            }
        }                     

        /// <summary>
        /// Contains columns used in whereclause
        /// </summary>
        public List<string> WhereClauseList
        {
            get
            {
                if (_whereClauseList == null)
                    _whereClauseList = new List<string>();
                return _whereClauseList;
            }
            set
            {
                _whereClauseList = value;
            }
        }

        /// <summary>
        /// Contains columns used in order by grouping
        /// </summary>
        public List<string> OrderClauseList
        {
            get
            {
                if (_orderClauseList == null)
                    _orderClauseList = new List<string>();
                return _orderClauseList;
            }
            set
            {
                _orderClauseList = value;
            }
        }

        /// <summary>
        /// Any filter statement (where clause) required to derive the proper subset
        /// of records serving as this picklist's item-set.
        /// </summary>
        public string WhereFilter
        {
            set
            {
                _whereFilter = value;
                if (!string.IsNullOrEmpty(_whereFilter))
                    this.WhereClauseList.Add(_whereFilter.Trim());
            }
        }

         /// <summary>
        /// Any filter statement (order clause) required to derive the proper subset
        /// of records serving as this picklist's item-set.
        /// </summary>
        public string OrderByFilter
        {
            set
            {
                _orderByFilter = value;
                if (!string.IsNullOrEmpty(_orderByFilter))
                    this.OrderClauseList.Add(_orderByFilter.Trim());
            }
        }
        
        /// <summary>
        /// General-purpose description of the picklist.
        /// </summary>
        public string Description
        {
            get
            {
                return _description;
            }
            protected set
            {
                _description = value;
                return;
            }
        }

        /// <summary>
        /// Related domain for the list.
        /// </summary>
        public int DomainID
        {
            get
            {
                return _domainId;
            }
            set
            {
                _domainId = value;
            }
        }

        /// <summary>
        /// Implements IPicklistService.
        /// </summary>
        public string GetQuery
        {
            get
            {
                return "";
            }
        }

        /// <summary>
        /// Returns status of picklist to be fetched from SQL.
        /// </summary>
        public CambridgeSoft.COE.Framework.COEPickListPickerService.PickListStatus PickListStatus
        {
            get
            {
                return _pickListStatus;
            }
            set
            {
                _pickListStatus = value;
            }
        }

        #endregion
       

        /// <summary>
        /// Retrieves all the values allowed for the given <paramref name="domainName"/> over the provided <paramref name="databaseName"/>
        /// </summary>
        /// <param name="databaseName">Database schema name.</param>
        /// <param name="domainName">The domain name.</param>
        /// <returns>The list of values for the given domain.</returns>
        public static PickListNameValueList GetPickListNameValueList(string databaseName, string domainName)
        {
            string methodSignature = MethodBase.GetCurrentMethod().DeclaringType.Name + "->" + MethodBase.GetCurrentMethod().Name.ToUpper() + " ";
            _coeLog.LogStart(methodSignature, 1, System.Diagnostics.SourceLevels.All);
            SetDatabaseName(databaseName);
            _list = DataPortal.Fetch<PickListNameValueList>(new DomainNameCriteria(domainName));
            _coeLog.LogEnd(methodSignature);
            return _list;
        }

        /// <summary>
        /// Retrieves all the values allowed for the given <paramref name="domainId"/> over the provided <paramref name="databaseName"/>
        /// </summary>
        /// <param name="databaseName">Database schema name.</param>
        /// <param name="domainId">The domain ID.</param>
        /// <returns>The list of values for the given domain.</returns>
        public static PickListNameValueList GetPickListNameValueList(string databaseName, int domainId)
        {
            string methodSignature = MethodBase.GetCurrentMethod().DeclaringType.Name + "->" + MethodBase.GetCurrentMethod().Name.ToUpper() + " ";
            _coeLog.LogStart(methodSignature, 1, System.Diagnostics.SourceLevels.All);
            SetDatabaseName(databaseName);
            _list = DataPortal.Fetch<PickListNameValueList>(new DomainIdCriteria(domainId));
            _coeLog.LogEnd(methodSignature);
            return _list;
        }

        public static PickListNameValueList GetPickListNameValueList(string sql)
        {
            string methodSignature = MethodBase.GetCurrentMethod().DeclaringType.Name + "->" + MethodBase.GetCurrentMethod().Name.ToUpper() + " ";
            _coeLog.LogStart(methodSignature, 1, System.Diagnostics.SourceLevels.All);
            _list = DataPortal.Fetch<PickListNameValueList>(new SQLCriteria(sql));
            _coeLog.LogEnd(methodSignature);
            return _list;
        }

        /// <summary>
        /// Retrieves all the picklist domain values available
        /// </summary>
        /// <returns>ID,Name list of </returns>
        public static PickListNameValueList GetAllPickListDomains(string databaseName)
        {
            string methodSignature = MethodBase.GetCurrentMethod().DeclaringType.Name + "->" + MethodBase.GetCurrentMethod().Name.ToUpper() + " ";
            _coeLog.LogStart(methodSignature, 1, System.Diagnostics.SourceLevels.All);
            SetDatabaseName(databaseName);
            _list = DataPortal.Fetch<PickListNameValueList>();
            _coeLog.LogEnd(methodSignature);
            return _list;
        }

        /// <summary>
        /// Retrieves all the picklist domain values available
        /// </summary>
        /// <returns>ID,Name list of </returns>
        public static PickListNameValueList GetAllPickListDomains()
        {
            string methodSignature = MethodBase.GetCurrentMethod().DeclaringType.Name + "->" + MethodBase.GetCurrentMethod().Name.ToUpper() + " ";
            _coeLog.LogStart(methodSignature, 1, System.Diagnostics.SourceLevels.All);
            _list = DataPortal.Fetch<PickListNameValueList>();
            _coeLog.LogEnd(methodSignature);
            return _list;
        }

        /// <summary>
        /// Retrieves all the values allowed for the given <paramref name="domainId"/> over the provided <paramref name="active"/> <paramref name="includeValue"/>
        /// </summary>
        /// <param name="domainId">PickListDomain Id.</param>
        /// <param name="active">Picklist status.</param>
        /// <param name="includeValue">IncludeValue picklist value.</param>
        /// <returns>The list of values for the given domain.</returns>
        public static PickListNameValueList GetPickListNameValueList(int domainId, object pickListStatus, object includeValue)
        {
            string methodSignature = MethodBase.GetCurrentMethod().DeclaringType.Name + "->" + MethodBase.GetCurrentMethod().Name.ToUpper() + " ";
            _coeLog.LogStart(methodSignature, 1, System.Diagnostics.SourceLevels.All);

            if (ServerCache.Exists(domainId.ToString(), typeof(PickListNameValueList)))
            {
                _list = ServerCache.Get(domainId.ToString(), typeof(PickListNameValueList)) as PickListNameValueList;
            }
            else
            {
                _list = DataPortal.Fetch<PickListNameValueList>(new SQLCriteria(GetDropdownSql(domainId, pickListStatus, includeValue)));
                ServerCache.Add(domainId.ToString(), typeof(PickListNameValueList), _list, LocalCache.NoAbsoluteExpiration, TimeSpan.FromMinutes(60), COECacheItemPriority.Normal);
            }
            if (_list == null)
                _list = DataPortal.Fetch<PickListNameValueList>(new SQLCriteria(GetDropdownSql(domainId, pickListStatus, includeValue)));
            _coeLog.LogEnd(methodSignature);
            return _list;
        }       

        /// <summary>
        /// Retrieves the constructed sql query<paramref name="domainId"/> over the provided <paramref name="active"/> <paramref name="includeValue"/>
        /// </summary>
        /// <param name="domainId">PickListDomain Id.</param>
        /// <param name="active">Picklist status.</param>
        /// <param name="includeValue">IncludeValue picklist value.</param>
        /// <returns>The sql query for the given domain.</returns>
        public static string GetDropdownSql(int domainId, object pickListStatus, object includeValue)
        {
            return DataPortal.Fetch<PickListNameValueList>(new PicklistServiceCriteria(domainId, pickListStatus, includeValue)).SqlQuery;
        }

        /// <summary>
        /// Build , Get, Set picklist query on provided picklistServiceCriteria communicates with querybuilder to construct sql query
        /// </summary>
        /// <param name="PicklistServiceCriteria">PicklistServiceCriteria</param>
        private void BuildSql(PicklistServiceCriteria picklistServiceCriteria)
        {
            QueryBuilderList queryBuilderList = QueryBuilderList.NewQueryBuilderList();
            queryBuilderList.AddNewQueryBuilder(this);
                        
            if (!string.IsNullOrEmpty(picklistServiceCriteria.IncludeValue))
            {
                IPicklistService pickListNameValueList = new PickListNameValueList();
                pickListNameValueList = (IPicklistService)this.Clone();
                pickListNameValueList.Columns.Clear();
                pickListNameValueList.WhereClauseList.Clear();
                pickListNameValueList.Columns.AddRange(queryBuilderList[0].SelectClause.Columns);
                if(picklistServiceCriteria.IncludeValue != null)
                    pickListNameValueList.WhereClauseList.Add(" WHERE " + this.IdColumn + @" IN (" + picklistServiceCriteria.IncludeValue + ")");
                queryBuilderList.AddNewQueryBuilder(pickListNameValueList);
            }
            this.SqlQuery = queryBuilderList.GetQuery;
        }

        /// <summary>
        /// Picklist status to return
        /// returns the status of picklist ot be used in SQL.
        /// </summary>
        private string GetPickListStatus(PickListStatus pickListStatus)
        {
            string retVal = string.Empty;
            const string ACTIVE = "ACTIVE";

            pickListStatus = this._coeDAL.ValidateSqlColumn(this.Table, ACTIVE) ? pickListStatus : PickListStatus.None; 
            
            switch (pickListStatus)
            {
                case COEPickListPickerService.PickListStatus.Active:
                    retVal = " WHERE ACTIVE = 'T'";
                    break;
                case COEPickListPickerService.PickListStatus.InActive:
                    retVal = " WHERE ACTIVE = 'F'";
                    break;
                case COEPickListPickerService.PickListStatus.All:
                    this.WhereClauseList.RemoveAll(clause=>clause.ToUpper().Contains("ACTIVE")); //Fix CBOE-1881 need to clear only ACTIVE status clause
                    retVal = " WHERE (ACTIVE = 'T' OR ACTIVE = 'F')";
                    break;
                case COEPickListPickerService.PickListStatus.None:
                    retVal = "";
                    break;
                default:
                    retVal = " WHERE ACTIVE = 'T'";
                    break;
            }
            return retVal;
        }

        /// <summary>
        /// Cleans previously cached values.
        /// </summary>
        public static void InvalidateCache()
        {
            string domainID = string.Empty;
            PickListNameValueList lstCol = PickListNameValueList.GetAllPickListDomains();
            if (lstCol != null)
            {
                foreach (Csla.NameValueListBase<string, string>.NameValuePair pair in lstCol)
                {
                    domainID = pair.Key;
                    if (ServerCache.Exists(domainID, typeof(PickListNameValueList)))
                    {
                        ServerCache.Remove(domainID, typeof(PickListNameValueList));
                    }
                }
            }
            _list = null;
        }

        //constructor
        private PickListNameValueList()
        {
        }
        
        #endregion

        #region DataAccess

        //this method must be called prior to any other method inorder to set the database that the dal will use
        internal static void SetDatabaseName(string databaseName)
        {
            if(!string.IsNullOrEmpty(databaseName))
                COEDatabaseName.Set(databaseName);
        }      

        private void DataPortal_Fetch(DomainIdCriteria criteria)
        {
            //to fetch id and Value in datareader.
            RaiseListChangedEvents = false;
            IsReadOnly = false;
            
            if(_coeDAL == null) { LoadDAL(); }
            // Coverity Fix CID - 11565 
            if (_coeDAL != null)
            {
                using (SafeDataReader dr = _coeDAL.GetPickListNameValueList(criteria._domainId))
                {
                    while (dr.Read())
                    {
                        Add(new NameValueListBase<string, string>.NameValuePair(dr.GetInt32(0).ToString(), System.Web.HttpUtility.HtmlDecode(dr.GetString(1))));
                    }
                }
            }
            else
                throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));

            IsReadOnly = true;
            RaiseListChangedEvents = true;
           
        }

        private void DataPortal_Fetch(DomainNameCriteria criteria) {
            //to fetch id and Value in datareader.
            RaiseListChangedEvents = false;
            IsReadOnly = false;

            if (_coeDAL == null) { LoadDAL(); }
            // Coverity Fix CID - 11566
            if (_coeDAL != null)
            {
                _domainId = _coeDAL.GetDomainPickListId(criteria._domainName);
                using (SafeDataReader dr = _coeDAL.GetPickListNameValueList(this._domainId))
                {
                    while (dr.Read())
                    {
                        Add(new NameValueListBase<string, string>.NameValuePair(dr.GetInt32(0).ToString(), System.Web.HttpUtility.HtmlDecode(dr.GetString(1))));
                    }
                }
            }
            else
                throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));

            IsReadOnly = true;
            RaiseListChangedEvents = true;

        }

        private void DataPortal_Fetch(SQLCriteria criteria)
        {
            RaiseListChangedEvents = false;
            IsReadOnly = false;
            if(_coeDAL == null) { LoadDAL(); }
            // Coverity Fix CID - 11568
            if (_coeDAL != null)
            {
                using (SafeDataReader dr = _coeDAL.GetPickListNameValueList(criteria.SqlStatement))
                {
                    while (dr.Read())
                    {
                        string val = string.Empty;
                        string key = dr.GetValue(0).ToString();
                        if (dr.GetValue(1) != null)
                        {
                            val = System.Web.HttpUtility.HtmlDecode(dr.GetValue(1).ToString());
                        }
                        Add(new NameValueListBase<string, string>.NameValuePair(key, val));
                    }
                }
            }
            else
                throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));
            
            IsReadOnly = true; 
            RaiseListChangedEvents = true;
        }

        private void DataPortal_Fetch()
        {
            //to fetch id and Value in datareader.
            RaiseListChangedEvents = false;
            IsReadOnly = false;

            if (_coeDAL == null) { LoadDAL(); }
            // Coverity Fix CID - 11564
            if (_coeDAL != null)
            {
                using (SafeDataReader dr = _coeDAL.GetAllPickListDomains())
                {
                    while (dr.Read())
                    {
                        Add(new NameValueListBase<string, string>.NameValuePair(dr.GetInt32(0).ToString(), System.Web.HttpUtility.HtmlDecode(dr.GetString(1))));
                    }
                }
            }
            else
                throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));

            IsReadOnly = true;
            RaiseListChangedEvents = true;
        }

        private void DataPortal_Fetch(PicklistServiceCriteria picklistServiceCriteria)
        { 
            RaiseListChangedEvents = false;
            IsReadOnly = false;
            SafeDataReader dr = null;
            try
            {
                if (_coeDAL == null) { LoadDAL(); }
                dr = _coeDAL.GetPicklistDomain(picklistServiceCriteria.DomainId);
                while (dr.Read())
                {
                    if (!string.IsNullOrEmpty(dr.GetString("EXT_TABLE")))
                    {
                        this.Table = dr.GetString("EXT_TABLE");
                        this.IdColumn = dr.GetString("EXT_ID_COL");
                        this.DisplayColumn = dr.GetString("EXT_DISPLAY_COL");
                        this.WhereFilter = dr.GetString("EXT_SQL_FILTER");
                        this.OrderByFilter = dr.GetString("EXT_SQL_SORTORDER");
                        this.WhereFilter = GetPickListStatus(picklistServiceCriteria.PickListStatus);
                    }
                    else
                    {
                        this.Table = "REGDB.PICKLIST";
                        this.IdColumn = "ID";
                        this.DisplayColumn = "PICKLISTVALUE";
                        this.WhereFilter = "WHERE PICKLISTDOMAINID = " + Convert.ToString(picklistServiceCriteria.DomainId);
                        this.OrderByFilter = " ORDER BY SORTORDER ASC";
                        this.WhereFilter = GetPickListStatus(picklistServiceCriteria.PickListStatus);
                    }
                   
                }
            }
            finally
            {
                dr.Close();
            }
            this.BuildSql(picklistServiceCriteria);
            IsReadOnly = true;
            RaiseListChangedEvents = true;
        }

        #endregion
          
        #region DomainIdCriteria
        [Serializable()]
        private class DomainIdCriteria {
            public int _domainId;

            //constructors
            public DomainIdCriteria(int domainId) {
                _domainId = domainId;
            }
        }
        #endregion //DomainIdCriteria

        #region DomainNameCriteria
        [Serializable()]
        private class DomainNameCriteria {
            public string _domainName;

            //constructors
            public DomainNameCriteria(string domainName) {
                _domainName = domainName;
            }
        }
        #endregion //DomainNameCriteria

        #region SQLCriteria
        [Serializable()]
        private class SQLCriteria
        {
            public string SqlStatement;

            public SQLCriteria(string sql)
            {
                sql = FrameworkUtils.ReplaceSpecialTokens(sql, true);
                this.SqlStatement = sql;
            }
        }
        #endregion //SQLCriteria

        #region PicklistServiceCriteria
        [Serializable()]
        private class PicklistServiceCriteria
        {
            private int _domainId;
            private PickListStatus _pickListStatus;
            private string _includeValue;

            public int DomainId
            {
                get
                {
                    return _domainId;
                }
                set
                {
                    _domainId = value;
                    return;
                }
            }

            public PickListStatus PickListStatus
            {
                get
                {
                    return _pickListStatus;
                }
                set
                {
                    _pickListStatus = value;
                    return;
                }
            }

            public string IncludeValue
            {
                get
                {
                    return _includeValue;
                }
                set
                {
                    _includeValue = value;
                    return;
                }
            }
                       
            //constructors
            public PicklistServiceCriteria(int domainId, object active, object includeValue)
            {
                DomainId = domainId;
                PickListStatus = CastActive(active);
                IncludeValue = CastIncludeValue(includeValue);
            }

            private PickListStatus CastActive(object obj)
            {
                PickListStatus retVal = PickListStatus.Active;
                if (obj is string)
                {
                    retVal = (Convert.ToString(obj).ToUpper() == "T") ? PickListStatus.Active : ((Convert.ToString(obj).ToUpper() == "TRUE") ? PickListStatus.Active : PickListStatus.InActive);
                }
                if (obj is bool)
                {
                    retVal = (Convert.ToBoolean(obj) == true) ? PickListStatus.Active : PickListStatus.InActive;
                }
                if (obj is int)
                {
                    retVal = (Convert.ToInt32(obj) == 1) ? PickListStatus.Active : PickListStatus.InActive;
                }
                if (obj is PickListStatus)
                {
                    retVal = ((PickListStatus)obj);
                }
                return retVal;
            }

            private string CastIncludeValue(object obj)
            {
                string retVal = string.Empty;

                if (obj != null)
                {
                    retVal = Convert.ToString(obj);
                }
                return retVal;
            }

        }
        #endregion //PicklistServiceCriteria

        #region DALLoader 
        private void LoadDAL()
        {
            if (_dalFactory == null) { _dalFactory = new DALFactory(); }
            string databaseName = string.IsNullOrEmpty(COEDatabaseName.Get()) ? Resources.RegistrationDatabaseName : COEDatabaseName.Get();
            _dalFactory.GetDAL<DAL>(ref _coeDAL, _serviceName, databaseName, true);

        }
        #endregion

        #region IKeyValueListHolder Members

        public IDictionary KeyValueList
        {
            get 
            {
                System.Collections.Specialized.OrderedDictionary dictionary = new System.Collections.Specialized.OrderedDictionary(this.Count);
                
                foreach(PickListNameValueList.NameValuePair pair in this)
                {
                    if(!dictionary.Contains(pair.Key))
                        dictionary.Add(pair.Key, pair.Value);
                }
                return dictionary;
            }
        }

        #endregion

    }
}
