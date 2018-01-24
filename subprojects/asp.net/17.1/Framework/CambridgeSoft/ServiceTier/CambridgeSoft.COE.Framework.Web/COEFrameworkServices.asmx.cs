using System;
using System.Web;
using System.Collections;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml;
using System.Data;
using System.Xml.XPath;
using System.IO;
using System.Security;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.COESecurityService;
using CambridgeSoft.COE.Framework.COESearchService;
using CambridgeSoft.COE.Framework.COEConfigurationService;
using CambridgeSoft.COE.Framework.COEExportService;
using CambridgeSoft.COE.Framework.COEFormService;
using CambridgeSoft.COE.Framework.COEGenericObjectStorageService;
using CambridgeSoft.COE.Framework.COEHitListService;
using CambridgeSoft.COE.Framework.COESearchCriteriaService;
using CambridgeSoft.COE.Framework.COETableEditorService;
using CambridgeSoft.COE.Framework.COEPickListPickerService;

using System.Configuration;
using CambridgeSoft.COE.Framework.COEDataViewService;
using System.Security.Authentication;
using System.Collections.Generic;
using CambridgeSoft.COE.Framework.Common.Utility;
using CambridgeSoft.COE.Framework.COEDatabasePublishingService;
using System.Xml.Serialization;

namespace CambridgeSoft.COE.Framework.Web
{
    /// <summary>
    /// Summary description for COEFrameworkServices
    /// </summary>
    [WebService(Namespace = "CambridgeSoft.COE.Framework.Web", Description = "Web Services for COE Framework Services", Name = "COEFrameworkServices")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class COEFrameworkServices : System.Web.Services.WebService
    {
        #region Public Variables
        /// <summary>
        /// Header credentials for authorizing a client. Required for searching.
        /// </summary>
        public COECredentials Credentials = new COECredentials();
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes the web service
        /// </summary>
        public COEFrameworkServices()
        {
        }
        #endregion

        #region Search Services
        /// <summary>
        /// Performs a search based on the input criterias
        /// </summary>
        /// <param name="searchCriteriaString">Search criteria</param>
        /// <param name="resultsCriteriaString">Result criteria</param>
        /// <param name="pagingInfoString">Paging information</param>
        /// <param name="dataViewString">Data view</param>
        /// <returns></returns>
        [WebMethod(false, Description = "Do A Search")]
        [SoapHeader("Credentials")]
        public string DoSearch(string searchCriteriaString, string resultsCriteriaString, string pagingInfoString, string dataViewString)
        {
            if(AuthenticateUserThroughHeader())
            {
                SearchCriteria searchCriteria = COEWebServiceUtilities.DeserializeCOESearchCriteria(searchCriteriaString);
                ResultsCriteria resultsCriteria = COEWebServiceUtilities.DeserializeCOEResultsCriteria(resultsCriteriaString);
                PagingInfo pagingInfo = COEWebServiceUtilities.DeserializeCOEPagingInfo(pagingInfoString);
                COEDataView dataView = COEWebServiceUtilities.DeserializeCOEDataView(dataViewString);

                COESearch coeSearch = new COESearch();

                SearchResponse searchResponse = coeSearch.DoSearch(searchCriteria, resultsCriteria, pagingInfo, dataView);

                string searchResponseString = COEWebServiceUtilities.SerializeCOESearchResponse(searchResponse);

                return searchResponseString;
            }

            return "NOT AUTHORIZED";
        }

        /// <summary>
        /// Performs a global search based on input criteria.
        /// </summary>
        /// <param name="searchCriteriaString">Search criteria</param>
        /// <param name="resultsCriteriaString">Result criteria</param>
        /// <param name="pagingInfoString"></param>
        /// <param name="dataViewString"></param>
        /// <returns></returns>
        [WebMethod(false, Description = "Do Search over several databases")]
        [SoapHeader("Credentials")]
        public string DoGlobalSearch(string searchCriteriaString, string resultsCriteriaString, string pagingInfoString, string dataViewString)
        {
            if(AuthenticateUserThroughHeader())
            {
                SearchCriteria searchCriteria = COEWebServiceUtilities.DeserializeCOESearchCriteria(searchCriteriaString);
                ResultsCriteria resultsCriteria = COEWebServiceUtilities.DeserializeCOEResultsCriteria(resultsCriteriaString);
                PagingInfo pagingInfo = COEWebServiceUtilities.DeserializeCOEPagingInfo(pagingInfoString);
                COEDataView dataView = COEWebServiceUtilities.DeserializeCOEDataView(dataViewString);

                COESearch coeSearch = new COESearch();

                SearchResponse searchResponse = coeSearch.DoSearch(searchCriteria, resultsCriteria, pagingInfo, dataView);

                string searchResponseString = COEWebServiceUtilities.SerializeCOESearchResponse(searchResponse);

                return searchResponseString;
            }

            return "NOT AUTHORIZED";
        }

        /// <summary>
        /// Gets data from a previous search.
        /// </summary>
        /// <param name="resultsCriteriaString">Result criteria</param>
        /// <param name="pagingInfoString">Paging information</param>
        /// <param name="dataViewString">Data view</param>
        /// <returns></returns>
        [WebMethod(false, Description = "Get Data from a previous search")]
        [SoapHeader("Credentials")]
        public string GetData(string resultsCriteriaString, string pagingInfoString, string dataViewString)
        {
            if(AuthenticateUserThroughHeader())
            {

                ResultsCriteria resultsCriteria = COEWebServiceUtilities.DeserializeCOEResultsCriteria(resultsCriteriaString);
                PagingInfo pagingInfo = COEWebServiceUtilities.DeserializeCOEPagingInfo(pagingInfoString);
                COEDataView dataView = COEWebServiceUtilities.DeserializeCOEDataView(dataViewString);

                COESearch coeSearch = new COESearch();

                DataSet resultDataSet = coeSearch.GetData(resultsCriteria, pagingInfo, dataView);

                return resultDataSet.GetXml();
            }

            return "NOT AUTHORIZED";
        }

        /// <summary>
        /// Gets filtered data. Paging info may or may not contain a hitlistid and hitlisttype. If provided the search criteria is used to
        /// filter the original hitlist. If not present a new search is performed. Note that this method would not create a new hitlist, but will
        /// only get the data filtered.
        /// </summary>
        /// <param name="searchCriteriaString">Search criteria</param>
        /// <param name="resultsCriteriaString">Result criteria</param>
        /// <param name="pagingInfoString">Paging information</param>
        /// <param name="dataViewString">Data view</param>
        /// <param name="useRealTableNames">Indicates if the underlying table name is to be retrieved or a name in the form "Table_"+"ID"</param>
        /// <returns></returns>
        [WebMethod(false, Description = @"Gets filtered data. Paging info may or may not contain a hitlistid and hitlisttype. If provided the search criteria is used to
        filter the original hitlist. If not present a new search is performed. Note that this method would not create a new hitlist, but will
        only get the data filtered.")]
        [SoapHeader("Credentials")]
        public string GetFilteredData(string searchCriteriaString, string resultsCriteriaString, string pagingInfoString, string dataViewString, bool useRealTableNames)
        {
            if (AuthenticateUserThroughHeader())
            {
                SearchCriteria searchCriteria = COEWebServiceUtilities.DeserializeCOESearchCriteria(searchCriteriaString);
                ResultsCriteria resultsCriteria = COEWebServiceUtilities.DeserializeCOEResultsCriteria(resultsCriteriaString);
                PagingInfo pagingInfo = COEWebServiceUtilities.DeserializeCOEPagingInfo(pagingInfoString);
                COEDataView dataView = COEWebServiceUtilities.DeserializeCOEDataView(dataViewString);

                COESearch coeSearch = new COESearch();

                DataSet resultDataSet = coeSearch.GetData(resultsCriteria, pagingInfo, dataView, (useRealTableNames ? "YES" : "NO"), null, searchCriteria);

                return resultDataSet.GetXml();
            }

            return "NOT AUTHORIZED";
        }

        /// <summary>
        /// Gets data from previous global search.
        /// </summary>
        /// <param name="resultsCriteriaString">Result criteria</param>
        /// <param name="pagingInfoString">Paging information</param>
        /// <param name="dataViewString">Data view</param>
        /// <returns></returns>
        [WebMethod(false, Description = "Get Data from previous global search")]
        [SoapHeader("Credentials")]
        public string GlobalSearchGetData(string resultsCriteriaString, string pagingInfoString, string dataViewString)
        {
            if(AuthenticateUserThroughHeader())
            {

                ResultsCriteria resultsCriteria = COEWebServiceUtilities.DeserializeCOEResultsCriteria(resultsCriteriaString);
                PagingInfo pagingInfo = COEWebServiceUtilities.DeserializeCOEPagingInfo(pagingInfoString);
                COEDataView dataView = COEWebServiceUtilities.DeserializeCOEDataView(dataViewString);

                COESearch coeSearch = new COESearch();

                DataSet resultDataSet = coeSearch.GetData(resultsCriteria, pagingInfo, dataView);

                return resultDataSet.ToString();
            }

            return "NOT AUTHORIZED";
        }

        #region Simple API
        /// <summary>
        /// Does a search with a simplified API
        /// </summary>
        /// <param name="searchInput"><see cref="SearchInput"/>Filters the data to be retrieved</param>
        /// <param name="resultFields">List of fields to be retrieved</param>
        /// <param name="pageInfo"><see cref="ResultPageInfo"/>Specifies which page is to be get</param>
        /// <param name="dataviewID">Data view to be used</param>
        /// <returns></returns>
        [WebMethod(false, Description = "Do a search with a simple API")]
        [SoapHeader("Credentials")]
        public DataResult DoSearchSimple(SearchInput searchInput, string[] resultFields, ResultPageInfo pageInfo, int dataviewID)
        {
            if(AuthenticateUserThroughHeader())
            {
                COESearch coeSearch = new COESearch(dataviewID);
                return coeSearch.DoSearch(searchInput, resultFields, pageInfo);
            }
            else
                throw new AuthenticationException("User could not be authenticated.");
        }

        /// <summary>
        /// Does a search that returns matching ids.
        /// </summary>
        /// <param name="searchInput"><see cref="SearchInput"/>Filters the data to be retrieved</param>
        /// <param name="pkField">Specifies which is the primary key field</param>
        /// <param name="pageInfo"><see cref="ResultPageInfo"/>Specifies which page is to be get</param>
        /// <param name="dataviewID">Data view to be used</param>
        /// <returns></returns>
        [WebMethod(false, Description = "Do a search with a simple API")]
        [SoapHeader("Credentials")]
        public DataListResult GetIdsSimple(SearchInput searchInput, string pkField, ResultPageInfo pageInfo, int dataviewID)
        {
            if(AuthenticateUserThroughHeader())
            {
                COESearch coeSearch = new COESearch(dataviewID);
                return coeSearch.DoSearch(searchInput, pkField, pageInfo);
            }
            else
                throw new AuthenticationException("User could not be authenticated.");
        }

        /// <summary>
        /// Gets a page with a simplified API.
        /// </summary>
        /// <param name="pageInfo"><see cref="ResultPageInfo"/>Specifies which page is to be get</param>
        /// <param name="resultFields">List of fields to be retrieved</param>
        /// <param name="dataviewID">Data view to be used</param>
        /// <returns></returns>
        [WebMethod(false, Description = "Gets a page with a simple API")]
        [SoapHeader("Credentials")]
        public DataResult GetDataPageSimple(ResultPageInfo pageInfo, string[] resultFields, int dataviewID)
        {
            if(AuthenticateUserThroughHeader())
            {
                COESearch coeSearch = new COESearch(dataviewID);
                return coeSearch.GetDataPage(pageInfo, resultFields);
            }
            else
                throw new AuthenticationException("User could not be authenticated.");
        }
        #endregion
        #endregion

        #region Export Services
        /// <summary>
        /// Exports data from a previous search to the specified format.
        /// </summary>
        /// <param name="resultsCriteriaString">Result criteria</param>
        /// <param name="pagingInfoString">Paging information</param>
        /// <param name="dataViewString">Data view</param>
        /// <param name="exportType">Export type</param>
        /// <returns></returns>
        [WebMethod(false, Description = "Export data from a previous search to the specified format")]
        [SoapHeader("Credentials")]
        public string ExportData(string resultsCriteriaString, string pagingInfoString, string dataViewString, string exportType)
        {
            if(AuthenticateUserThroughHeader())
            {

                ResultsCriteria resultsCriteria = COEWebServiceUtilities.DeserializeCOEResultsCriteria(resultsCriteriaString);
                PagingInfo pagingInfo = COEWebServiceUtilities.DeserializeCOEPagingInfo(pagingInfoString);
                COEDataView dataView = COEWebServiceUtilities.DeserializeCOEDataView(dataViewString);

                COEExport coeExport = new COEExport();

                string exportResults = coeExport.GetData(resultsCriteria, pagingInfo, dataView, exportType);

                return exportResults;
            }

            return "NOT AUTHORIZED";
        }
        #endregion

        #region Configuration Services
        //[WebMethod(false, Description = "Get Application Names in COEFrameworkConfig file")]
        //[SoapHeader("Credentials")]
        //public string GetAllAppNamesInConfig(){
        //    string results = string.Empty;
        //    if (AuthenticateUserThroughHeader())
        //    {

        //        return results;
        //    }

        //    return "NOT AUTHORIZED";
        //}

        //[WebMethod(false, Description = "Get Database Names in COEFrameworkConfig file")]
        //[SoapHeader("Credentials")]
        //public string GetAllDatabaseNamesInConfig()
        //{
        //    string results = string.Empty;
        //    if (AuthenticateUserThroughHeader())
        //    {

        //        return results;
        //    }

        //    return "NOT AUTHORIZED";
        //}

        //[WebMethod(false, Description = "Get Applications in COEFrameworkConfig file based on DBMS type")]
        //[SoapHeader("Credentials")]
        //public string GetAppByDatabase(string dbmsType)
        //{
        //    string results = string.Empty;
        //    if (AuthenticateUserThroughHeader())
        //    {

        //        return results;
        //    }

        //    return "NOT AUTHORIZED";
        //}

        //[WebMethod(false, Description = "Get Databases in COEFrameworkConfig file based on DBMS type")]
        //[SoapHeader("Credentials")]
        //public string GetDatabaseByDatabaseType(string dbmsType)
        //{
        //    string results = string.Empty;
        //    if (AuthenticateUserThroughHeader())
        //    {

        //        return results;
        //    }

        //    return "NOT AUTHORIZED";
        //}

        #endregion

        #region Dataview Services

        #region Single
        /// <summary>
        /// Gets a COEDataview by its ID.
        /// </summary>
        /// <param name="dataViewID">The record id</param>
        /// <returns><see cref="COEDataView"/>The dataview</returns>
        [WebMethod(false, Description = "Get COEDataView by ID")]
        [SoapHeader("Credentials")]
        [return: XmlRoot("COEDataView", Namespace = "COE.COEDataView")]
        public COEDataView GetDataView(int dataViewID)
        {
            if(AuthenticateUserThroughHeader())
            {

                return COEDataViewBO.Get(dataViewID).COEDataView;
            }

            return null;
        }

        /// <summary>
        /// Deletes the COEDataView with the given ID
        /// </summary>
        /// <param name="dataViewID">The record ID</param>
        /// <returns>The number of records affected</returns>
        [WebMethod(false, Description = "Deletes the COEDataView with the given ID")]
        [SoapHeader("Credentials")]
        public int DeleteDataView(int dataViewID)
        {
            if(AuthenticateUserThroughHeader())
            {
                COEDataViewBO.Delete(dataViewID);
                return 1;
            }

            return 0;
        }

        /// <summary>
        /// Updates the COEDataView specified by dataViewID with the given values
        /// </summary>
        /// <param name="dataViewID">The record ID</param>
        /// <param name="databaseName">Database name</param>
        /// <param name="name">Name</param>
        /// <param name="description">Description</param>
        /// <param name="isPublic">Is public or private</param>
        /// <param name="formgroup">Form group id</param>
        /// <param name="userName">Owner</param>
        /// <param name="coeDataView">The Dataview</param>
        /// <returns><see cref="COEDataView"/>The updated dataview</returns>
        [WebMethod(false, Description = "Updates the COEDataView specified by dataViewID with the given values")]
        [SoapHeader("Credentials")]
        [return: XmlRoot("COEDataView", Namespace = "COE.COEDataView")]
        public COEDataView UpdateDataView(int dataViewID, string databaseName, string name, string description, bool isPublic, int formgroup, string userName, COEDataView coeDataView)
        {
            if(AuthenticateUserThroughHeader())
            {
                COEDataViewBO dvBO = COEDataViewBO.Get(dataViewID);
                dvBO.COEDataView = coeDataView;
                dvBO.DatabaseName = databaseName;
                dvBO.Name = name;
                dvBO.Description = description;
                dvBO.IsPublic = isPublic;
                dvBO.FormGroup = formgroup;
                dvBO.UserName = userName;
                return dvBO.Save().COEDataView;
            }

            return null;
        }

        /// <summary>
        /// Updates the COEDataView specified by dataViewID with the given values
        /// </summary>
        /// <param name="dataViewID">The record ID</param>
        /// <param name="databaseName">Database name</param>
        /// <param name="name">Name</param>
        /// <param name="description">Description</param>
        /// <param name="isPublic">Is public or private</param>
        /// <param name="formgroup">Form group id</param>
        /// <param name="userName">Owner</param>
        /// <param name="coeDataView">The Dataview</param>
        /// <returns><see cref="COEDataView"/>The updated dataview</returns>
        [WebMethod(false, Description = "Updates the COEDataView specified by dataViewID with the given values")]
        [SoapHeader("Credentials")]
        [return: XmlRoot("COEDataView", Namespace = "COE.COEDataView")]
        public COEDataView UpdateDataViewStr(int dataViewID, string databaseName, string name, string description, bool isPublic, int formgroup, string userName, string coeDataView)
        {
            if (AuthenticateUserThroughHeader())
            {
                COEDataViewBO dvBO = COEDataViewBO.Get(dataViewID);
                dvBO.COEDataView = Framework.Common.Utilities.XmlDeserialize<COEDataView>(coeDataView);
                dvBO.DatabaseName = databaseName;
                dvBO.Name = name;
                dvBO.Description = description;
                dvBO.IsPublic = isPublic;
                dvBO.FormGroup = formgroup;
                dvBO.UserName = userName;
                return dvBO.Save().COEDataView;
            }

            return null;
        }

        /// <summary>
        /// Creates and stores a COEDataView
        /// </summary>
        /// <param name="databaseName">Database name</param>
        /// <param name="name">Name</param>
        /// <param name="description">Description</param>
        /// <param name="isPublic">Is public or private</param>
        /// <param name="formgroup">Form group id</param>
        /// <param name="userName">Owner</param>
        /// <param name="coeDataView">The Dataview</param>
        /// <returns></returns>
        [WebMethod(false, Description = "Creates and stores a COEDataView")]
        [SoapHeader("Credentials")]
        public int InsertDataView(string databaseName, string name, string description, bool isPublic, int formgroup, string userName, COEDataView coeDataView)
        {
            if(AuthenticateUserThroughHeader())
            {
                COEDataViewBO dvBO = COEDataViewBO.New();
                dvBO.COEDataView = coeDataView;
                dvBO.DatabaseName = databaseName;
                dvBO.Name = name;
                dvBO.Description = description;
                dvBO.IsPublic = isPublic;
                dvBO.FormGroup = formgroup;
                dvBO.UserName = userName;
                
                if (coeDataView.DataViewID > 0)
                    dvBO.ID = coeDataView.DataViewID;

                return dvBO.Save().ID;
            }

            return -1;
        }

        /// <summary>
        /// Creates and stores a COEDataView
        /// </summary>
        /// <param name="databaseName">Database name</param>
        /// <param name="name">Name</param>
        /// <param name="description">Description</param>
        /// <param name="isPublic">Is public or private</param>
        /// <param name="formgroup">Form group id</param>
        /// <param name="userName">Owner</param>
        /// <param name="coeDataView">The Dataview</param>
        /// <returns></returns>
        [WebMethod(false, Description = "Creates and stores a COEDataView")]
        [SoapHeader("Credentials")]
        public int InsertDataViewStr(string databaseName, string name, string description, bool isPublic, int formgroup, string userName, string coeDataView)
        {
            if (AuthenticateUserThroughHeader())
            {
                COEDataViewBO dvBO = COEDataViewBO.New();
                dvBO.COEDataView = Framework.Common.Utilities.XmlDeserialize<COEDataView>(coeDataView);
                dvBO.DatabaseName = databaseName;
                dvBO.Name = name;
                dvBO.Description = description;
                dvBO.IsPublic = isPublic;
                dvBO.FormGroup = formgroup;
                dvBO.UserName = userName;

                if (dvBO.COEDataView.DataViewID > 0)
                    dvBO.ID = dvBO.COEDataView.DataViewID;

                return dvBO.Save().ID;
            }

            return -1;
        }

        /// <summary>
        /// Publishes the table to the master dataview and the dataview that was passed.
        /// </summary>
        /// <param name="dataviewID">The dataview ID to add the table to.</param>
        /// <param name="fullTableName">The name of the table you want to publish in the format of SCHEMA.TABLENAME</param>
        /// <param name="primaryKeyFieldName">Name of the pimary key field. FIELDNAME or SCHEMA.TABLENAME.FIELDNAME of a field in the table being published (optional)</param>
        /// <param name="parentTableJoinFieldName">Name of the parent table join field in the format SCHEMA.TABLENAME.FIELDNAME. If not passed will use basetable primary key.</param>
        /// <param name="childTableJoinFieldName">Field in the table to be published that joins to the Parent Table in the format FIELDNAME or SCHEMA.TABLENAME.FIELDNAME</param>
        /// <param name="joinType">Type of the join. Inner | Outer</param>
        /// <returns>not really sure yet. Could return true or false. Or the tableid. I am open to suggestions.</returns>
        /// <example>PublishTableToDataview(10, "BIOASSAY.AV_1181_1206", "BIOASSAY.AV_1181_1206.ID", "REGDB.VW_REG_BATCHES.BATCHID", "BIOASSAY.AV_1181_1206.BATCHID", "INNER")</example>
        [WebMethod(false, Description = "Publishes the table to the master dataview and the dataview that was passed.")]
        [SoapHeader("Credentials")]
        public string PublishTableToDataview(int dataviewID, string fullTableName, string primaryKeyFieldName, string parentTableJoinFieldName, string childTableJoinFieldName, Framework.Common.COEDataView.JoinTypes joinType)
        {
            if (AuthenticateUserThroughHeader())
            {
                COEDataViewBO bo = COEDataViewBO.Get(dataviewID);
                if(bo.ID > 0)
                {
                    return bo.PublishTableToDataview(fullTableName, primaryKeyFieldName, parentTableJoinFieldName, childTableJoinFieldName, joinType);
                }
            }

            return "User was not authenticated";
        }

        #endregion
        #region list
        /// <summary>
        /// Gets a list of dataview owned by a user.
        /// </summary>
        /// <param name="userName">The user name</param>
        /// <returns>The matching list of <see cref="COEDataView"/></returns>
        [WebMethod(false, Description = "Get a List of DataViews for a user")]
        [SoapHeader("Credentials")]
        public COEDataView[] GetDataViewDataListByUser(string userName)
        {
            if(AuthenticateUserThroughHeader())
            {
                COEDataViewBOList list = COEDataViewBOList.GetDataViewDataListByUser(userName);
                COEDataView[] results = new COEDataView[list.Count];
                for(int i = 0; i < list.Count; i++)
                {
                    results[i] = list[i].COEDataView;
                }
                return results;
            }

            return null;
        }

        /// <summary>
        /// Gets a list of dataviews for a gived database
        /// </summary>
        /// <param name="databaseName">Database name</param>
        /// <returns>The matching list of <see cref="COEDataView"/></returns>
        [WebMethod(false, Description = "Get a List of DataViews for a database")]
        [SoapHeader("Credentials")]
        public COEDataView[] GetDataViewDataListByDatabase(string databaseName)
        {
            if(AuthenticateUserThroughHeader())
            {
                COEDataViewBOList list = COEDataViewBOList.GetDataViewListbyDatabase(databaseName);
                COEDataView[] results = new COEDataView[list.Count];
                for(int i = 0; i < list.Count; i++)
                {
                    results[i] = list[i].COEDataView;
                }
                return results;
            }

            return null;
        }

        /// <summary>
        /// Gets a list with all the dataviews
        /// </summary>
        /// <returns>The matching list of <see cref="COEDataView"/></returns>
        [WebMethod(false, Description = "Get a List of DataViews for all databases in COEFrameworkConfig")]
        [SoapHeader("Credentials")]
        public COEDataView[] GetDataViewListforAllDatabases()
        {
            if(AuthenticateUserThroughHeader())
            {
                COEDataViewBOList list = COEDataViewBOList.GetDataViewListforAllDatabases();
                COEDataView[] results = new COEDataView[list.Count];
                for(int i = 0; i < list.Count; i++)
                {
                    results[i] = list[i].COEDataView;
                }
                return results;
            }

            return null;
        }
        #endregion
        #endregion

        #region Form Services
        //    #region single

        //    [WebMethod(false, Description = "Get COEFormBO by ID")]
        //    [SoapHeader("Credentials")]
        //    public string GetForm(int formID)
        //    {
        //        string results = string.Empty;
        //        if (AuthenticateUserThroughHeader())
        //        {

        //            return results;
        //        }

        //        return "NOT AUTHORIZED";
        //    }

        //    [WebMethod(false, Description = "Delete COEFormBO by ID")]
        //    [SoapHeader("Credentials")]
        //    public string DeleteForm(int formID)
        //    {
        //        string results = string.Empty;
        //        if (AuthenticateUserThroughHeader())
        //        {


        //        }

        //        return "NOT AUTHORIZED";
        //    }


        //    [WebMethod(false, Description = "Update COEFormBO by ID")]
        //    [SoapHeader("Credentials")]
        //    public string UpdateForm(int formID, string databaseName, string name, string description, bool isPublic, int formgroup, string userName, string coeForm )
        //    {
        //        string results = string.Empty;
        //        if (AuthenticateUserThroughHeader())
        //        {


        //        }

        //        return "NOT AUTHORIZED";
        //    }

        //    [WebMethod(false, Description = "Insert COEFormBO ")]
        //    [SoapHeader("Credentials")]
        //    public string InsertForm(string databaseName, string name, string description, bool isPublic, int formgroup, string userName, string coeForm )
        //    {
        //        string results = string.Empty;
        //        if (AuthenticateUserThroughHeader())
        //        {


        //        }

        //        return "NOT AUTHORIZED";
        //    }
        //    #endregion
        //    #region list

        //    [WebMethod(false, Description = "Get a List of Forms for a user")]
        //    [SoapHeader("Credentials")]
        //    public string GetFormListByUser(string databaseName, string userName)
        //    {
        //        string results = string.Empty;
        //        if (AuthenticateUserThroughHeader())
        //        {


        //        }

        //        return "NOT AUTHORIZED";
        //    }

        //    [WebMethod(false, Description = "Get a List of Forms for a database")]
        //    [SoapHeader("Credentials")]
        //    public string GetCOEFormBOListByDatabase(string databaseName)
        //    {
        //        string results = string.Empty;
        //        if (AuthenticateUserThroughHeader())
        //        {


        //        }

        //        return "NOT AUTHORIZED";
        //    }



        //    [WebMethod(false, Description = "Get a List of Forms for all databases in COEFrameworkConfig")]
        //    [SoapHeader("Credentials")]
        //    public string GetFormListforAllDatabases()
        //    {
        //        string results = string.Empty;
        //        if (AuthenticateUserThroughHeader())
        //        {


        //        }

        //        return "NOT AUTHORIZED";
        //    }
        //    #endregion
        //    #endregion

        //    #region generic object storage  services
        //    #region single
        //    [WebMethod(false, Description = "Get GenericStorageObjectBO by ID")]
        //    [SoapHeader("Credentials")]
        //    public string GetGenericStorageObject(int objectID)
        //    {
        //        string results = string.Empty;
        //        if (AuthenticateUserThroughHeader())
        //        {

        //            return results;
        //        }

        //        return "NOT AUTHORIZED";
        //    }

        //    [WebMethod(false, Description = "Delete GenericStorageObjectBO by ID")]
        //    [SoapHeader("Credentials")]
        //    public string DeleteGenericStorageObject(int objectID)
        //    {
        //        string results = string.Empty;
        //        if (AuthenticateUserThroughHeader())
        //        {


        //        }

        //        return "NOT AUTHORIZED";
        //    }


        //    [WebMethod(false, Description = "Update GenericStorageObjectBO by ID")]
        //    [SoapHeader("Credentials")]
        //    public string UpdateGenericStorageObject(int objectID, string databaseName, string name, string description, bool isPublic, int formgroup, string userName, string coeGenericStorageObject )
        //    {
        //        string results = string.Empty;
        //        if (AuthenticateUserThroughHeader())
        //        {


        //        }

        //        return "NOT AUTHORIZED";
        //    }

        //    [WebMethod(false, Description = "Insert GenericStorageObjectBO ")]
        //    [SoapHeader("Credentials")]
        //    public string InsertGenericStorageObject(string databaseName, string name, string description, bool isPublic, int formgroup, string userName, string coeGenericStorageObject )
        //    {
        //        string results = string.Empty;
        //        if (AuthenticateUserThroughHeader())
        //        {


        //        }

        //        return "NOT AUTHORIZED";
        //    }
        //    #endregion
        //    #region list
        //        [WebMethod(false, Description = "Get a List of generic objects for a user")]
        //    [SoapHeader("Credentials")]
        //    public string GetGenericObjecListByUser(string databaseName, string userName)
        //    {
        //        string results = string.Empty;
        //        if (AuthenticateUserThroughHeader())
        //        {


        //        }

        //        return "NOT AUTHORIZED";
        //    }

        //    [WebMethod(false, Description = "Get a List of generic objects for a user and isPublic setting ")]
        //    [SoapHeader("Credentials")]
        //    public string GetGenericObjecListByUserAndIsPublic(string databaseName,string userName,string isPublic)
        //    {
        //        string results = string.Empty;
        //        if (AuthenticateUserThroughHeader())
        //        {


        //        }

        //        return "NOT AUTHORIZED";
        //    }



        //    [WebMethod(false, Description = "Get a List of generic objects exluding some users and isPublic setting ")]
        //    [SoapHeader("Credentials")]
        //    public string GetGenericObjecListIsPublicExludeUsers()
        //    {
        //        string results = string.Empty;
        //        if (AuthenticateUserThroughHeader())
        //        {


        //        }

        //        return "NOT AUTHORIZED";
        //    }
        //    #endregion
        //    #endregion

        //    #region hitlist services
        //   #region single

        //    [WebMethod(false, Description = "Get COEHitListBO by ID")]
        //    [SoapHeader("Credentials")]
        //    public string GetHitList(int hitListID, string database, string HitListType)
        //    {
        //        string results = string.Empty;
        //        if (AuthenticateUserThroughHeader())
        //        {

        //            return results;
        //        }

        //        return "NOT AUTHORIZED";
        //    }

        //    [WebMethod(false, Description = "Delete COEHitListBO by ID")]
        //    [SoapHeader("Credentials")]
        //    public string DeleteHitList(int hitListID, string database,  string HitListType)
        //    {
        //        string results = string.Empty;
        //        if (AuthenticateUserThroughHeader())
        //        {


        //        }

        //        return "NOT AUTHORIZED";
        //    }


        //    [WebMethod(false, Description = "Update COEHitListBO by ID")]
        //    [SoapHeader("Credentials")]
        //    public string UpdateHitList(int hitListID, string databaseName, string HitListType, string name, string description, bool isPublic, int formgroup, string userName, int[] hits)
        //    {
        //        string results = string.Empty;
        //        if (AuthenticateUserThroughHeader())
        //        {


        //        }

        //        return "NOT AUTHORIZED";
        //    }

        //    [WebMethod(false, Description = "Insert COEHitListBO with hits ")]
        //    [SoapHeader("Credentials")]
        //    public string InsertHitList(int hitListID, string databaseName, string HitListType,  string name, string description, bool isPublic, int formgroup, string userName, int[] hits)
        //    {
        //        string results = string.Empty;
        //        if (AuthenticateUserThroughHeader())
        //        {


        //        }

        //        return "NOT AUTHORIZED";
        //    }

        //    [WebMethod(false, Description = "Save a temp hitlist")]
        //    [SoapHeader("Credentials")]
        //    public string NewSavedFromTempHitList(int hitListID, string databaseName, string tempHitListID,  string name, string description, bool isPublic, int formgroup, string userName, int[] hits)
        //    {
        //        string results = string.Empty;
        //        if (AuthenticateUserThroughHeader())
        //        {


        //        }

        //        return "NOT AUTHORIZED";
        //    }
        #endregion

        #region Hitlist Operations
        //    [WebMethod(false, Description = "Create a new temp hitlist from substration of two hitlist")]
        //    [SoapHeader("Credentials")]
        //    public string NewTempFromSubtraction(int hitListID1, string databaseName1, string hitListType1,  int hitListID2, string databaseName2, string hitListType2)
        //    {
        //        string results = string.Empty;
        //        if (AuthenticateUserThroughHeader())
        //        {


        //        }

        //        return "NOT AUTHORIZED";
        //    }


        //    [WebMethod(false, Description = "Create a new temp hitlist from union of two hitlist")]
        //    [SoapHeader("Credentials")]
        //    public string NewTempFromUnion(int hitListID1, string databaseName1, string hitListType1,  int hitListID2, string databaseName2, string hitListType2)
        //    {
        //        string results = string.Empty;
        //        if (AuthenticateUserThroughHeader())
        //        {


        //        }

        //        return "NOT AUTHORIZED";
        //    }

        //    [WebMethod(false, Description = "Create a new temp hitlist from Intersection of two hitlist")]
        //    [SoapHeader("Credentials")]
        //    public string NewTempFromIntersect(int hitListID1, string databaseName1, string hitListType1,  int hitListID2, string databaseName2, string hitListType2)
        //    {
        //        string results = string.Empty;
        //        if (AuthenticateUserThroughHeader())
        //        {


        //        }

        //        return "NOT AUTHORIZED";
        //    }

        //#endregion
        //    #region list

        //    [WebMethod(false, Description = "Get a List of Forms for a user")]
        //    [SoapHeader("Credentials")]
        //    public string GetHitListList(string databaseName, string hitListType)
        //    {
        //        string results = string.Empty;
        //        if (AuthenticateUserThroughHeader())
        //        {


        //        }

        //        return "NOT AUTHORIZED";
        //    }


        //    #endregion
        #endregion

        #region PicklistPicker Services
        //    #endregion

        //    #region search criteria services
        //    #region single
        //    [WebMethod(false, Description = "Get COESearchCriteriaBO by ID")]
        //    [SoapHeader("Credentials")]
        //    public string GetSearchCriteria(int searchCriteriaID)
        //    {
        //        string results = string.Empty;
        //        if (AuthenticateUserThroughHeader())
        //        {

        //            return results;
        //        }

        //        return "NOT AUTHORIZED";
        //    }

        //    [WebMethod(false, Description = "Delete COESearchCriteriaBO by ID")]
        //    [SoapHeader("Credentials")]
        //    public string DeleteSearchCriteria(int searchCriteriaID)
        //    {
        //        string results = string.Empty;
        //        if (AuthenticateUserThroughHeader())
        //        {


        //        }

        //        return "NOT AUTHORIZED";
        //    }


        //    [WebMethod(false, Description = "Update COESearchCriteriaBO by ID")]
        //    [SoapHeader("Credentials")]
        //    public string UpdateSearchCriteria(int searchCriteriaID, string databaseName, string name, string description, bool isPublic, int formgroup, string userName, string searchCriteria)
        //    {
        //        string results = string.Empty;
        //        if (AuthenticateUserThroughHeader())
        //        {


        //        }

        //        return "NOT AUTHORIZED";
        //    }

        //    [WebMethod(false, Description = "Insert COESearchCriteriaBO ")]
        //    [SoapHeader("Credentials")]
        //    public string InsertSearchCriteria(string databaseName, string name, string description, bool isPublic, int formgroup, string userName, string searchCriteria)
        //    {
        //        string results = string.Empty;
        //        if (AuthenticateUserThroughHeader())
        //        {


        //        }

        //        return "NOT AUTHORIZED";
        //    }
        //    #endregion
        //    #region list

        //    [WebMethod(false, Description = "Get a List of SearchCriteria for a user")]
        //    [SoapHeader("Credentials")]
        //    public string GetSearchCriteriaListByUser(string databaseName, string userName)
        //    {
        //        string results = string.Empty;
        //        if (AuthenticateUserThroughHeader())
        //        {


        //        }

        //        return "NOT AUTHORIZED";
        //    }
        //    #endregion
        #endregion

        #region Security Services
        private bool AuthenticateUserThroughHeader()
        {
            if (string.IsNullOrEmpty(Credentials.UserName) && string.IsNullOrEmpty(Credentials.AuthenticationTicket))
            {
                // No credentials have been provided...apply 'testing' credentials (even if they don't exist)
                Credentials.UserName = ConfigurationManager.AppSettings["TestingUserName"];
                Credentials.Password = ConfigurationManager.AppSettings["TestingPassword"];
            }

            if(string.IsNullOrEmpty(Credentials.UserName))
            {
                if(string.IsNullOrEmpty(Credentials.AuthenticationTicket))
                    throw new SecurityException("Valid credentials were not provided");
                else if(!COEPrincipal.Login(Credentials.AuthenticationTicket, true))
                {
                    throw new SecurityException("Invalid Authentication Ticket");
                }
            }
            else
            {
                if(!COEPrincipal.Login(Credentials.UserName, Credentials.Password))
                {
                    throw new SecurityException("Invalid username or password");
                }
            }

            return true;
        }
        #endregion

        #region TableEditor Services
        //    [WebMethod(false, Description = "Get table row to by ID")]
        //    [SoapHeader("Credentials")]
        //    public string GetTableRow(string appName,int rowUniqueID)
        //    {
        //        string results = string.Empty;
        //        if (AuthenticateUserThroughHeader())
        //        {

        //            return results;
        //        }

        //        return "NOT AUTHORIZED";
        //    }

        //    [WebMethod(false, Description = "Delete table row by ID")]
        //    [SoapHeader("Credentials")]
        //    public string DeleteTableRow(string appName,int rowUniqueID)
        //    {
        //        string results = string.Empty;
        //        if (AuthenticateUserThroughHeader())
        //        {


        //        }

        //        return "NOT AUTHORIZED";
        //    }


        //    [WebMethod(false, Description = "Update table row  by ID")]
        //    [SoapHeader("Credentials")]
        //    public string UpdateTableRow(string appName, int searchCriteriaIDstring, string[] columnList)
        //    {
        //        string results = string.Empty;
        //        if (AuthenticateUserThroughHeader())
        //        {


        //        }

        //        return "NOT AUTHORIZED";
        //    }

        //[WebMethod(false, Description = "Insert table row ")]
        //    [SoapHeader("Credentials")]
        //    public string InsertTableRow(string appName,string[] columnList)
        //    {
        //        string results = string.Empty;
        //        if (AuthenticateUserThroughHeader())
        //        {


        //        }

        //        return "NOT AUTHORIZED";
        //    }



        //    [WebMethod(false, Description = "Get available tables for editting as specified for an applicaton")]
        //    [SoapHeader("Credentials")]
        //    public string GetEditableTablesList(string appName, string tableName)
        //    {
        //        string results = string.Empty;
        //        if (AuthenticateUserThroughHeader())
        //        {


        //        }

        //        return "NOT AUTHORIZED";
        //    }

        //    [WebMethod(false, Description = "Get table data for a table")]
        //    [SoapHeader("Credentials")]
        //    public string GetTableDataList(string appName, string tableName)
        //    {
        //        string results = string.Empty;
        //        if (AuthenticateUserThroughHeader())
        //        {


        //        }

        //        return "NOT AUTHORIZED";
        //    }

        #endregion

    }
}
