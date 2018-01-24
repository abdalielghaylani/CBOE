// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SearchManager.cs" company="PerkinElmer Inc.">
//   Copyright (c) 2013 PerkinElmer Inc.,
//   940 Winter Street, Waltham, MA 02451.
//   All rights reserved.
//   This software is the confidential and proprietary information
//   of PerkinElmer Inc. ("Confidential Information"). You shall not
//   disclose such Confidential Information and may not use it in any way,
//   absent an express written license agreement between you and PerkinElmer Inc.
//   that authorizes such use.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Xml;
using System.IO;
using CambridgeSoft.COE.Framework.Properties;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.Common.SqlGenerator;
using CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries;
using CambridgeSoft.COE.Framework.COESearchService.Processors;
using CambridgeSoft.COE.Framework.Common.SqlGenerator.NonQueries;
using CambridgeSoft.COE.Framework.Types.Exceptions;
using CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries.SelectItems;
using CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries.AggregateItems;
using CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries.WhereItems;
using CambridgeSoft.COE.Framework.COEHitListService;
using CambridgeSoft.COE.Framework.COESearchCriteriaService;
using CambridgeSoft.COE.Framework.COEDataViewService;
using CambridgeSoft.COE.Framework.COEConfigurationService;
using CambridgeSoft.COE.Framework.COELoggingService;
using CambridgeSoft.COE.Framework.Types.Exceptions;
using System.Reflection;
using CambridgeSoft.COE.Framework.Caching;
using CambridgeSoft.COE.Framework.Search.Processors;

namespace CambridgeSoft.COE.Framework.COESearchService
{
        /// <summary>
        /// Class for managing tasks releated to Searching
        /// </summary>
        [Serializable]
        public class SearchManager : BLLBase
        {
            #region Variables
            [NonSerialized]
            static COELog _coeLog = COELog.GetSingleton("COESearch");

            [NonSerialized]
            private CambridgeSoft.COE.Framework.COESearchService.DAL searchDAL = null;
            [NonSerialized]
            private DALFactory dalFactory = new DALFactory();
            public delegate int InsertHitListAsyncDelegate(KeepAliveHolder holder,
                                            int commitSize);
            private HitListInfo hitListInfo = null;
            private QueryBuilder queryBuilder = null;
            private string hitListTableName;
            private string hitListIDTableName;
            private string hitListSchemaName;
            private int hitListTableID;
            private int hitListIDFieldID;
            private int hitListSortOrderFieldID;
            private KeepAliveHolder holder;
            private bool directCall = false; // by default, supposing a remote call.
            private bool keepAlive = false; // by default, supposing it's not in keep alive mode.
            private string _databaseName = null;

            private int _maxRecordCount;
            private bool _applyDataviewFilter = false;
            private bool commitTransaction = true;
            private const string FilteredChildData = "FilteredChildData";
            private const string FilteredChildDataMessage = "FilteredChildDataMessage";
            #endregion

            #region Properties
            public int MaxRecordCount
            {
                get
                {
                    return _maxRecordCount;
                }
                set
                {
                    if (value > 0)
                        _maxRecordCount = value;
                }
            }
            #endregion

            /// <summary>
            /// The DAL to access COEDB database.
            /// </summary>
            [NonSerialized]
            private DAL coeDAL = null;

            #region Manager implementation required by COE
            /// <summary>
            /// Default constructor for the Search Manager. This is used when
            /// instantiation via direct reference to this assembly
            /// </summary>
            public SearchManager()
            {
                this.ServiceName = "COESearch";

                if (ServiceConfigData == null)
                ServiceConfigData = CambridgeSoft.COE.Framework.COEConfigurationService.ConfigurationUtilities.GetServiceData(this.ServiceName);

                _maxRecordCount = this.ServiceConfigData.MaxRecordCount;
            }
            #endregion

            /*
		 * Methods that form the public API and call methods in the managers DAL classes to perform 
		 * storage operations
		 */
            #region Manager specific implementation
            /// <summary>
            /// General purpose search method
            /// </summary>
            /// <param name="searchCriteria">object containing fields and criteria for search</param>
            /// <param name="resultsCriteria">object containing fields and criteria for returning results dataset</param>
            /// <param name="pagingInfo">object containing paging info for getting results from a cached search</param>
            /// <param name="dataView">object containing the data view or an id of it</param>
            /// <param name="directCall">boolen value indicating if this is a direct call or a remote one.</param>
            /// <returns>SearchResponse object made up of hitlist object, paging info object and dataset object. One or more of these objects might be null
            /// depending on the initial search</returns>
            public SearchResponse DoSearch(ref SearchCriteria searchCriteria, ResultsCriteria resultsCriteria, PagingInfo pagingInfo, COEDataView dataView, bool directCall, ConnStringType connStringType)
            {
                this.directCall = directCall;
                return this.PerformSearch(ref searchCriteria, resultsCriteria, pagingInfo, dataView, false, connStringType);
            }

            /// <summary>
            /// General purpose search method
            /// </summary>
            /// <param name="searchCriteria">object containing fields and criteria for search, null searchCriteria will invoke a browse</param>
            /// <param name="resultsCriteria">object containing fields and criteria for returning results dataset</param>
            /// <param name="pagingInfo">object containing paging info for getting results from a cached search</param>
            /// <param name="dataView">object containing the data view or an id of it</param>
            /// <returns>SearchResponse object made up of hitlist object, paging info object and dataset object. One or more of these objects might be null
            /// depending on the initial search</returns>
            public SearchResponse DoSearch(ref SearchCriteria searchCriteria, ResultsCriteria resultsCriteria, PagingInfo pagingInfo, COEDataView dataView, ConnStringType connStringType)
            {
                return PerformSearch(ref searchCriteria, resultsCriteria, pagingInfo, dataView, false, connStringType);
            }


            /// <summary>
            /// General purpose search method
            /// </summary>
            /// <param name="searchCriteria">object containing fields and criteria for search, null searchCriteria will invoke a browse</param>
            /// <param name="resultsCriteria">object containing fields and criteria for returning results dataset</param>
            /// <param name="pagingInfo">object containing paging info for getting results from a cached search</param>
            /// <param name="dataView">object containing the data view or an id of it</param>
            /// <param name="useRealTableNames">string that specifies if real table names are supposed to be returned (yes/true), if table names in the form Table_tableid (no/false) or if this is up o the search service (null/empty)</param>
            /// <returns>SearchResponse object made up of hitlist object, paging info object and dataset object. One or more of these objects might be null
            /// depending on the initial search</returns>
            public SearchResponse DoSearch(ref SearchCriteria searchCriteria, ResultsCriteria resultsCriteria, PagingInfo pagingInfo, COEDataView dataView, ConnStringType connStringType, string useRealTableNames)
            {
                return PerformSearch(ref searchCriteria, resultsCriteria, pagingInfo, dataView, false, connStringType, useRealTableNames);
            }

            /// <summary>
            /// General purpose search method
            /// </summary>
            /// <param name="searchCriteria">object containing fields and criteria for search, null searchCriteria will invoke a browse</param>
            /// <param name="resultsCriteria">object containing fields and criteria for returning results dataset</param>
            /// <param name="pagingInfo">object containing paging info for getting results from a cached search</param>
            /// <param name="dataView">object containing the data view or an id of it</param>
            /// <param name="useRealTableNames">string that specifies if real table names are supposed to be returned (yes/true), if table names in the form Table_tableid (no/false) or if this is up o the search service (null/empty)</param>
            /// <returns>SearchResponse object made up of hitlist object, paging info object and dataset object. One or more of these objects might be null
            /// depending on the initial search</returns>
            public SearchResponse DoSearch(ref SearchCriteria searchCriteria, ResultsCriteria resultsCriteria, PagingInfo pagingInfo, COEDataView dataView, ConnStringType connStringType, string useRealTableNames, OrderByCriteria orderByCriteria)
            {
                return PerformSearch(ref searchCriteria, resultsCriteria, pagingInfo, dataView, false, connStringType, useRealTableNames, orderByCriteria);
            }

            /// <summary>
            /// General purpose search method that quickly returns the firsts records.
            /// </summary>
            /// <param name="searchCriteria">object containing fields and criteria for search, null searchCriteria will invoke a browse</param>
            /// <param name="resultsCriteria">object containing fields and criteria for returning results dataset</param>
            /// <param name="pagingInfo">object containing paging info for getting results from a cached search</param>
            /// <param name="dataView">object containing the data view or an id of it</param>
            /// <returns>SearchResponse object made up of hitlist object, paging info object and dataset object. One or more of these objects might be null
            /// depending on the initial search</returns>
            public SearchResponse DoPartialSearch(ref SearchCriteria searchCriteria, ResultsCriteria resultsCriteria, PagingInfo pagingInfo, COEDataView dataView, ConnStringType connStringType)
            {
                return PerformSearch(ref searchCriteria, resultsCriteria, pagingInfo, dataView, true, connStringType);
            }

            /// <summary>
            /// General purpose search method that quickly returns the firsts records.
            /// </summary>
            /// <param name="searchCriteria">object containing fields and criteria for search, null searchCriteria will invoke a browse</param>
            /// <param name="resultsCriteria">object containing fields and criteria for returning results dataset</param>
            /// <param name="pagingInfo">object containing paging info for getting results from a cached search</param>
            /// <param name="dataView">object containing the data view or an id of it</param>
            /// <param name="useRealTableNames">string that specifies if real table names are supposed to be returned (yes/true), if table names in the form Table_tableid (no/false) or if this is up o the search service (null/empty)</param>
            /// <returns>SearchResponse object made up of hitlist object, paging info object and dataset object. One or more of these objects might be null
            /// depending on the initial search</returns>
            public SearchResponse DoPartialSearch(ref SearchCriteria searchCriteria, ResultsCriteria resultsCriteria, PagingInfo pagingInfo, COEDataView dataView, ConnStringType connStringType, string useRealTableNames)
            {
                return PerformSearch(ref searchCriteria, resultsCriteria, pagingInfo, dataView, true, connStringType, useRealTableNames);
            }

            /// <summary>
            /// General purpose search method that quickly returns the firsts records.
            /// </summary>
            /// <param name="searchCriteria">object containing fields and criteria for search, null searchCriteria will invoke a browse</param>
            /// <param name="resultsCriteria">object containing fields and criteria for returning results dataset</param>
            /// <param name="pagingInfo">object containing paging info for getting results from a cached search</param>
            /// <param name="dataView">object containing the data view or an id of it</param>
            /// <param name="useRealTableNames">string that specifies if real table names are supposed to be returned (yes/true), if table names in the form Table_tableid (no/false) or if this is up o the search service (null/empty)</param>
            /// <param name="connStringType">Connection string type</param>
            /// <param name="orderByCriteria">Client ordering criteria</param>
            /// <returns>SearchResponse object made up of hitlist object, paging info object and dataset object. One or more of these objects might be null
            /// depending on the initial search</returns>
            public SearchResponse DoPartialSearch(ref SearchCriteria searchCriteria, ResultsCriteria resultsCriteria, PagingInfo pagingInfo, COEDataView dataView, ConnStringType connStringType, string useRealTableNames, OrderByCriteria orderByCriteria)
            {
                return PerformSearch(ref searchCriteria, resultsCriteria, pagingInfo, dataView, true, connStringType, useRealTableNames, orderByCriteria);
            }

            /// <summary>
            /// General purpose search method that quickly returns the firsts records.
            /// </summary>
            /// <param name="searchCriteria">object containing fields and criteria for search, null searchCriteria will invoke a browse</param>
            /// <param name="resultsCriteria">object containing fields and criteria for returning results dataset</param>
            /// <param name="pagingInfo">object containing paging info for getting results from a cached search</param>
            /// <param name="dataView">object containing the data view or an id of it</param>
            /// <returns>SearchResponse object made up of hitlist object, paging info object and dataset object. One or more of these objects might be null
            /// depending on the initial search</returns>
            public SearchResponse DoPartialSearch(ref SearchCriteria searchCriteria, ResultsCriteria resultsCriteria, PagingInfo pagingInfo, COEDataView dataView, ConnStringType connStringType, int commitSize)
            {
                return PerformSearch(ref searchCriteria, resultsCriteria, pagingInfo, dataView, true, connStringType, commitSize, null, new OrderByCriteria());
            }

            /// <summary>
            /// General purpose search method that quickly returns the firsts records.
            /// </summary>
            /// <param name="searchCriteria">object containing fields and criteria for search, null searchCriteria will invoke a browse</param>
            /// <param name="resultsCriteria">object containing fields and criteria for returning results dataset</param>
            /// <param name="pagingInfo">object containing paging info for getting results from a cached search</param>
            /// <param name="dataView">object containing the data view or an id of it</param>
            /// <param name="useRealTableNamesStr">string that specifies if real table names are supposed to be returned (yes/true), if table names in the form Table_tableid (no/false) or if this is up o the search service (null/empty)</param>
            /// <returns>SearchResponse object made up of hitlist object, paging info object and dataset object. One or more of these objects might be null
            /// depending on the initial search</returns>
            public SearchResponse DoPartialSearch(ref SearchCriteria searchCriteria, ResultsCriteria resultsCriteria, PagingInfo pagingInfo, COEDataView dataView, ConnStringType connStringType, int commitSize, string useRealTableNamesStr)
            {
                return PerformSearch(ref searchCriteria, resultsCriteria, pagingInfo, dataView, true, connStringType, commitSize, useRealTableNamesStr, new OrderByCriteria());
            }

            /// <summary>
            /// General purpose search method that quickly returns the firsts records.
            /// </summary>
            /// <param name="searchCriteria">object containing fields and criteria for search, null searchCriteria will invoke a browse</param>
            /// <param name="resultsCriteria">object containing fields and criteria for returning results dataset</param>
            /// <param name="pagingInfo">object containing paging info for getting results from a cached search</param>
            /// <param name="dataView">object containing the data view or an id of it</param>
            /// <param name="useRealTableNamesStr">string that specifies if real table names are supposed to be returned (yes/true), if table names in the form Table_tableid (no/false) or if this is up o the search service (null/empty)</param>
            /// <param name="commitSize">The size for commiting in each searching loop</param>
            /// <param name="connStringType">Connection string type</param>
            /// <param name="orderByCriteria">Client ordering criteria</param>
            /// <returns>SearchResponse object made up of hitlist object, paging info object and dataset object. One or more of these objects might be null
            /// depending on the initial search</returns>
            public SearchResponse DoPartialSearch(ref SearchCriteria searchCriteria, ResultsCriteria resultsCriteria, PagingInfo pagingInfo, COEDataView dataView, ConnStringType connStringType, int commitSize, string useRealTableNamesStr, OrderByCriteria orderByCriteria)
            {
                return PerformSearch(ref searchCriteria, resultsCriteria, pagingInfo, dataView, true, connStringType, commitSize, useRealTableNamesStr, orderByCriteria);
            }

            /// <summary>
            /// General purpose search method
            /// </summary>
            /// <param name="searchCriteria">object containing fields and criteria for search, null searchCriteria will invoke a browse</param>
            /// <param name="resultsCriteria">object containing fields and criteria for returning results dataset</param>
            /// <param name="pagingInfo">object containing paging info for getting results from a cached search</param>
            /// <param name="dataView">object containing the data view or an id of it</param>
            /// <param name="partialHitList">determines whether inserting partial hitlist or not</param>
            /// <returns>SearchResponse object made up of hitlist object, paging info object and dataset object. One or more of these objects might be null
            /// depending on the initial search</returns>
            private SearchResponse PerformSearch(ref SearchCriteria searchCriteria, ResultsCriteria resultsCriteria, PagingInfo pagingInfo, COEDataView dataView, bool partialHitList, ConnStringType connStringType)
            {
                return PerformSearch(ref searchCriteria, resultsCriteria, pagingInfo, dataView, partialHitList, connStringType, -1, null, new OrderByCriteria());
            }

            /// <summary>
            /// General purpose search method
            /// </summary>
            /// <param name="searchCriteria">object containing fields and criteria for search, null searchCriteria will invoke a browse</param>
            /// <param name="resultsCriteria">object containing fields and criteria for returning results dataset</param>
            /// <param name="pagingInfo">object containing paging info for getting results from a cached search</param>
            /// <param name="dataView">object containing the data view or an id of it</param>
            /// <param name="partialHitList">determines whether inserting partial hitlist or not</param>
            /// <param name="connStringType">Connection string type</param>
            /// <param name="orderByCriteria">Client ordering criteria</param>
            /// <returns>SearchResponse object made up of hitlist object, paging info object and dataset object. One or more of these objects might be null
            /// depending on the initial search</returns>
            private SearchResponse PerformSearch(ref SearchCriteria searchCriteria, ResultsCriteria resultsCriteria, PagingInfo pagingInfo, COEDataView dataView, bool partialHitList, ConnStringType connStringType, OrderByCriteria orderByCriteria)
            {
                return PerformSearch(ref searchCriteria, resultsCriteria, pagingInfo, dataView, partialHitList, connStringType, -1, null, orderByCriteria);
            }

            /// <summary>
            /// General purpose search method
            /// </summary>
            /// <param name="searchCriteria">object containing fields and criteria for search, null searchCriteria will invoke a browse</param>
            /// <param name="resultsCriteria">object containing fields and criteria for returning results dataset</param>
            /// <param name="pagingInfo">object containing paging info for getting results from a cached search</param>
            /// <param name="dataView">object containing the data view or an id of it</param>
            /// <param name="partialHitList">determines whether inserting partial hitlist or not</param>
            /// <param name="useRealTableNamesStr">string that specifies if real table names are supposed to be returned (yes/true), if table names in the form Table_tableid (no/false) or if this is up o the search service (null/empty)</param>
            /// <returns>SearchResponse object made up of hitlist object, paging info object and dataset object. One or more of these objects might be null
            /// depending on the initial search</returns>
            private SearchResponse PerformSearch(ref SearchCriteria searchCriteria, ResultsCriteria resultsCriteria, PagingInfo pagingInfo, COEDataView dataView, bool partialHitList, ConnStringType connStringType, string useRealTableNamesStr)
            {
                return PerformSearch(ref searchCriteria, resultsCriteria, pagingInfo, dataView, partialHitList, connStringType, -1, useRealTableNamesStr, new OrderByCriteria());
            }

            /// <summary>
            /// General purpose search method
            /// </summary>
            /// <param name="searchCriteria">object containing fields and criteria for search, null searchCriteria will invoke a browse</param>
            /// <param name="resultsCriteria">object containing fields and criteria for returning results dataset</param>
            /// <param name="pagingInfo">object containing paging info for getting results from a cached search</param>
            /// <param name="dataView">object containing the data view or an id of it</param>
            /// <param name="partialHitList">determines whether inserting partial hitlist or not</param>
            /// <param name="useRealTableNamesStr">string that specifies if real table names are supposed to be returned (yes/true), if table names in the form Table_tableid (no/false) or if this is up o the search service (null/empty)</param>
            /// <param name="connStringType">Connection string type</param>
            /// <param name="orderByCriteria">Client ordering criteria</param>
            /// <returns>SearchResponse object made up of hitlist object, paging info object and dataset object. One or more of these objects might be null
            /// depending on the initial search</returns>
            private SearchResponse PerformSearch(ref SearchCriteria searchCriteria, ResultsCriteria resultsCriteria, PagingInfo pagingInfo, COEDataView dataView, bool partialHitList, ConnStringType connStringType, string useRealTableNamesStr, OrderByCriteria orderByCriteria)
            {
                return PerformSearch(ref searchCriteria, resultsCriteria, pagingInfo, dataView, partialHitList, connStringType, -1, useRealTableNamesStr, orderByCriteria);
            }

            /// <summary>
            /// General purpose search method
            /// </summary>
            /// <param name="searchCriteria">object containing fields and criteria for search, null searchCriteria will invoke a browse</param>
            /// <param name="resultsCriteria">object containing fields and criteria for returning results dataset</param>
            /// <param name="pagingInfo">object containing paging info for getting results from a cached search</param>
            /// <param name="dataView">object containing the data view or an id of it</param>
            /// <param name="partialHitList">determines whether inserting partial hitlist or not</param>
            /// <param name="useRealTableNames">string that specifies if real table names are supposed to be returned (yes/true), if table names in the form Table_tableid (no/false) or if this is up o the search service (null/empty)</param>
            /// <returns>SearchResponse object made up of hitlist object, paging info object and dataset object. One or more of these objects might be null
            /// depending on the initial search</returns>
            private SearchResponse PerformSearch(ref SearchCriteria searchCriteria, ResultsCriteria resultsCriteria, PagingInfo pagingInfo, COEDataView dataView, bool partialHitList, ConnStringType connStringType, int commitSize, string useRealTableNames, OrderByCriteria orderByCriteria)
            {
                _databaseName = dataView.Database;

                // If partial search is required then we have to ensure that there is no ordering.
                if (partialHitList)
                {
                    if (orderByCriteria != null && orderByCriteria.Items.Count > 0)
                        throw new Exception(Resources.SortinNotAllowedForPartialSearch);

                    foreach (ResultsCriteria.ResultsCriteriaTable table in resultsCriteria.Tables)
                    {
                        foreach (ResultsCriteria.IResultsCriteriaBase rCriteria in table.Criterias)
                        {
                            if (rCriteria.OrderById > 0)
                            {
                                throw new Exception(Resources.SortinNotAllowedForPartialSearch);
                            }
                        }
                    }
                }

                try
                {
                    DataSet resultDataSet = null;
                    HitListInfo hitListInfo = null;
                    switch (searchCriteria == null)
                    {
                        case true:
                            _coeLog.LogStart("DoSearch case True", 1, System.Diagnostics.SourceLevels.All);
                            //this means that the search will not return a hitlist - it is a browse call and is opening the basetable
                            pagingInfo.HitListID = 0;
                            resultDataSet = GetData(resultsCriteria, pagingInfo, dataView, connStringType, useRealTableNames, orderByCriteria);
                            //create a hitListInfo to return
                            hitListInfo = new HitListInfo();
                            hitListInfo.HitListID = pagingInfo.HitListID;
                            hitListInfo.Database = dataView.Database;
                            if (resultDataSet != null && resultDataSet.Tables.Count > 0 && resultDataSet.Tables[0].Rows != null)
                                hitListInfo.CurrentRecordCount = resultDataSet.Tables[0].Rows.Count;
                            //populate the rowCount of the basetable in the hitlist rowcount
                            GetFastRecordCount(hitListInfo, dataView, connStringType);
                            _coeLog.LogEnd("DoSearch case True, RecordCount: " + hitListInfo.RecordCount, 1, System.Diagnostics.SourceLevels.All);
                            break;
                        case false:
                            if (searchCriteria.Items.Count == 1 &&
                                searchCriteria.Items[0] is SearchCriteria.SearchCriteriaItem &&
                                ((SearchCriteria.SearchCriteriaItem)searchCriteria.Items[0]).Criterium.GetType().Name == "DomainCriteria")
                            {

                                _coeLog.LogStart("DoSearch case false browse", 1, System.Diagnostics.SourceLevels.All);
                                SearchCriteria.DomainCriteria domainCriteria = (SearchCriteria.DomainCriteria)((SearchCriteria.SearchCriteriaItem)searchCriteria.Items[0]).Criterium;
                                pagingInfo.HitListID = System.Convert.ToInt32(domainCriteria.InnerText); //cast as int
                                pagingInfo.HitListType = HitListType.TEMP;
                                COEHitListBO domainHitList = COEHitListBO.Get(HitListType.TEMP, pagingInfo.HitListID);
                                resultDataSet = GetData(resultsCriteria, pagingInfo, dataView, connStringType, useRealTableNames, orderByCriteria);
                                //create a hitListInfo to return
                                hitListInfo = new HitListInfo();
                                hitListInfo.HitListID = pagingInfo.HitListID;
                                hitListInfo.Database = dataView.Database;
                                hitListInfo.RecordCount = domainHitList.NumHits;
                                hitListInfo.IsExactRecordCount = true;
                                _coeLog.LogEnd("DoSearch case false browse, RecordCount: " + hitListInfo.RecordCount, 1, System.Diagnostics.SourceLevels.All);
                            }
                            else
                            {
                                //this means that the search will return a hitlist
                                _coeLog.LogStart("DoSearch case false hitlist", 1, System.Diagnostics.SourceLevels.All);
                                //NOTE:  for performing the search we may need to use OwnerProxy otherwise the user
                                //needs permissions that only coedb has
                                connStringType = ConnStringType.OWNERPROXY;
                                if (commitSize <= 0)
                                    hitListInfo = GetHitList(ref searchCriteria, dataView, partialHitList, connStringType, null);
                                else
                                    hitListInfo = GetHitList(ref searchCriteria, dataView, partialHitList, connStringType, commitSize, null);
                                hitListInfo.Database = dataView.Database;
                                hitListInfo.IsExactRecordCount = true;
                                pagingInfo.HitListID = hitListInfo.HitListID;
                                pagingInfo.HitListType = hitListInfo.HitListType;
                                if (pagingInfo.RecordCount > hitListInfo.CurrentRecordCount)
                                    pagingInfo.RecordCount = hitListInfo.CurrentRecordCount;
                                resultDataSet = GetData(resultsCriteria, pagingInfo, dataView, connStringType, useRealTableNames, orderByCriteria);
                                _coeLog.LogEnd("DoSearch case false hitlist, RecordCount: " + hitListInfo.RecordCount, 1, System.Diagnostics.SourceLevels.All);
                            }
                            break;
                    }

                    SearchResponse searchResponse = new SearchResponse();
                    searchResponse.ResultsDataSet = resultDataSet;
                    searchResponse.PagingInfo = pagingInfo;
                    searchResponse.HitListInfo = hitListInfo;
                    return searchResponse;
                }
                catch (Exception ex)
                {

                    throw;
                }

            }

            /// <summary>
            /// Global search method
            /// </summary>
            /// <param name="searchCriteria">object containing fields and criteria for search, null searchCriteria will invoke a browse</param>
            /// <param name="resultsCriteria">object containing fields and criteria for returning results dataset</param>
            /// <param name="pagingInfo">object containing paging info for getting results from a cached search</param>
            /// <param name="dataView">array of dataview objects containing the databases(2) to search and the dataview to use</param>
            /// <param name="connStringType">Connection string type. Owner/OwnerProxy/Proxy</param>
            /// <returns>List of SearchResponse objects made up of hitlist object, paging info object and dataset object. One or more of these objects might be null
            /// depending on the initial search</returns>
            public SearchResponse[] DoGlobalSearch(ref SearchCriteria searchCriteria, ResultsCriteria resultsCriteria, PagingInfo pagingInfo, COEDataView[] dataView, ConnStringType connStringType)
            {
                _coeLog.LogStart("DoGlobalSearch", 1, System.Diagnostics.SourceLevels.All);
                int totalCount = dataView.GetLength(0);
                SearchResponse[] searchResponse = new SearchResponse[totalCount];
                for (int i = 0; i < totalCount - 1; i++)
                {
                    searchResponse[i] = DoSearch(ref searchCriteria, resultsCriteria, pagingInfo, dataView[i], connStringType);
                }
                _coeLog.LogEnd("DoGlobalSearch", 1, System.Diagnostics.SourceLevels.All);
                return searchResponse;
            }

            /// <summary>
            /// Global search Get Data
            /// </summary>
            /// <param name="resultsCriteria">object containing fields and criteria for returning results dataset</param>
            /// <param name="pagingInfo">an array of paging info one for each dataview </param>
            /// <param name="dataView">an array  of dataviews </param>
            /// <param name="connStringType">Connection string type. Owner/OwnerProxy/Proxy</param>
            /// <returns>List of DataSet containing results</returns>
            public DataSet[] GetGlobalSearchData(ResultsCriteria resultsCriteria, PagingInfo pagingInfo, COEDataView[] dataView, ConnStringType connStringType)
            {
                _coeLog.LogStart("GetGlobalSearchData", 1, System.Diagnostics.SourceLevels.All);
                int totalCount = dataView.GetLength(0);
                DataSet[] resultDataSet = new DataSet[totalCount];
                for (int i = 0; i < totalCount - 1; i++)
                {
                    resultDataSet[i] = GetData(resultsCriteria, pagingInfo, dataView[i], connStringType);
                }
                _coeLog.LogEnd("GetGlobalSearchData", 1, System.Diagnostics.SourceLevels.All);
                return resultDataSet;
            }

            /// <summary>
            /// Get row count
            /// </summary>
            /// <param name="hitListInfo">object containing hitlist info for getting results from a cached search</param>
            /// <param name="dataView">object containing the data view or an id of it</param>
            /// <param name="connStringType">Connection string type. Owner/OwnerProxy/Proxy</param>
            public void GetFastRecordCount(HitListInfo hitListInfo, COEDataView dataView, ConnStringType connStringType)
            {
                //to get the base table row count pass base table id to the GetFastRecordCount() method
                GetFastRecordCount(hitListInfo, dataView, dataView.Basetable, connStringType);
            }

            /// <summary>
            /// Get row count for specified table id in the dataview
            /// </summary>
            /// <param name="hitListInfo">object containing hitlist info for getting results from a cached search</param>
            /// <param name="dataView">object containing the data view or an id of it</param>
            /// <param name="tableId">id of the table in dataview to get the record count</param>
            /// <param name="connStringType">Connection string type. Owner/OwnerProxy/Proxy</param>
            public void GetFastRecordCount(HitListInfo hitListInfo, COEDataView dataView, int tableId, ConnStringType connStringType)
            {
                _coeLog.LogStart("GetFastRecordCount", 1, System.Diagnostics.SourceLevels.All);
                dataView = this.ApplyDataviewFilter(dataView, tableId);
                _databaseName = dataView.Database;
            if (searchDAL == null) { LoadDAL(connStringType); }
                // Coverity Fix CID - 11583
                if (searchDAL != null)
                {
                    string tableName = dataView.Tables.getById(tableId).Name;
                    hitListInfo = searchDAL.GetFastRecordCount(hitListInfo, tableName, dataView.GetDatabaseNameById(tableId));
                    _coeLog.LogEnd("GetFastRecordCount: " + hitListInfo.RecordCount, 1, System.Diagnostics.SourceLevels.All);
                }
                else
                    throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));
            }

            public void RefreshDatabaseRecordCount(HitListInfo hitListInfoPara, COEDataView dataView, int tableId, ConnStringType connStringType)
            {
                //coeLog.LogStart("RefreshDatabaseRecordCount", 1, System.Diagnostics.SourceLevels.All);

                dataView = this.ApplyDataviewFilter(dataView, tableId);

                var databaseName = dataView.GetDatabaseNameById(tableId);
            if (searchDAL == null) { LoadDAL(connStringType); }

                if (searchDAL != null)
                {
                    string tableName = dataView.Tables.getById(tableId).Name;
                    hitListInfoPara = searchDAL.RefreshDatabaseRecordCount(hitListInfoPara, tableName, ConfigurationUtilities.GetDatabaseData(databaseName).Owner);
                    //coeLog.LogEnd("RefreshDatabaseRecordCount: " + hitListInfoPara.RecordCount, 1, System.Diagnostics.SourceLevels.All);
                }
                else
                {
                    throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));
                }
            }

            /// <summary>
            /// Get exact row count
            /// </summary>
            /// <param name="dataView">object containing the data view </param>
            /// <param name="connStringType">Connection string type. Owner/OwnerProxy/Proxy</param>
            /// <returns>returns exact rowcount</returns>
            public int GetExactRecordCount(COEDataView dataView, ConnStringType connStringType)
            {
                _coeLog.LogStart("GetExactRecordCount", 1, System.Diagnostics.SourceLevels.All);
                dataView = this.ApplyDataviewFilter(dataView, dataView.Basetable);
                _databaseName = dataView.Database;
            if (searchDAL == null) { LoadDAL(connStringType); }
                // Coverity Fix CID - 11582
                if (searchDAL != null)
                {
                    int count = searchDAL.GetExactRecordCount(dataView.BaseTableName, dataView.GetDatabaseNameById(dataView.Basetable));
                    _coeLog.LogEnd("GetExactRecordCount: " + count, 1, System.Diagnostics.SourceLevels.All);
                    return count;
                }
                else
                    throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));
            }
            public string GetUserName(string name, int hits, string sql)
            {
                ConnStringType connStringType = ConnStringType.OWNERPROXY;
            LoadCOEDAL(connStringType);
             if (coeDAL != null)
            {
            string username = coeDAL.GetUserName(name, hits, sql);
            return username;
                }
                else
                {
                    throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));
                }
            }

            public int GetUserID(int hitsID, string sql)
            {
                ConnStringType connStringType = ConnStringType.OWNERPROXY;
            LoadCOEDAL(connStringType);
            if (coeDAL != null)
            {
                int Hits = coeDAL.GetUserID(hitsID, sql);
                return Hits;
            }
                else
                {
                    throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));
                }
            }

            /// <summary>
            /// Search method for return dataset
            /// </summary>
            /// <param name="resultsCriteria">object containing fields and criteria for returning results dataset</param>
            /// <param name="pagingInfo">object containing paging info for getting results from a cached search</param>
            /// <param name="dataView">object containing the data view or an id of it</param>
            /// <param name="connStringType">Connection string type. Owner/OwnerProxy/Proxy</param>
            /// <returns>DataSet containing results</returns>
            public DataSet GetData(ResultsCriteria resultsCriteria, PagingInfo pagingInfo, COEDataView dataView, ConnStringType connStringType)
            {
                return GetData(resultsCriteria, pagingInfo, dataView, connStringType, null, new OrderByCriteria());
            }

            /// <summary>
            /// Search method for return dataset
            /// </summary>
            /// <param name="resultsCriteria">object containing fields and criteria for returning results dataset</param>
            /// <param name="pagingInfo">object containing paging info for getting results from a cached search</param>
            /// <param name="dataView">object containing the data view or an id of it</param>
            /// <param name="connStringType">Connection string type. Owner/OwnerProxy/Proxy</param>
            /// <param name="useRealTableNamesStr">string that specifies if real table names are supposed to be returned (yes/true), if table names in the form Table_tableid (no/false) or if this is up o the search service (null/empty)</param>
            /// <returns>DataSet containing results</returns>
            public DataSet GetData(ResultsCriteria resultsCriteria, PagingInfo pagingInfo, COEDataView dataView, ConnStringType connStringType, string useRealTableNamesStr)
            {
                return GetData(resultsCriteria, pagingInfo, dataView, connStringType, useRealTableNamesStr, new OrderByCriteria());
            }

            /// <summary>
            /// Search method for return dataset
            /// </summary>
            /// <param name="resultsCriteria">object containing fields and criteria for returning results dataset</param>
            /// <param name="pagingInfo">object containing paging info for getting results from a cached search</param>
            /// <param name="dataView">object containing the data view or an id of it</param>
            /// <param name="useRealTableNamesStr">string that specifies if real table names are supposed to be returned (yes/true), if table names in the form Table_tableid (no/false) or if this is up o the search service (null/empty)</param>
            /// <param name="connStringType">Connection string type. Owner/OwnerProxy/Proxy</param>
            /// <param name="originalOrderByCriteria">Order by criteria</param>
            /// <returns>DataSet containing results</returns>
            public DataSet GetData(ResultsCriteria resultsCriteria, PagingInfo pagingInfo, COEDataView dataView, ConnStringType connStringType, string useRealTableNamesStr, OrderByCriteria originalOrderByCriteria)
            {
                return GetData(resultsCriteria, pagingInfo, dataView, connStringType, useRealTableNamesStr, originalOrderByCriteria, GetEmptySearchCriteria());
            }

            /// <summary>
            /// Search method for return dataset. This method is also used for searching without hitlist, by passing a hitlisid=0 in the paging info and providing searchcriteria to filter by.
            /// </summary>
            /// <param name="resultsCriteria">object containing fields and criteria for returning results dataset</param>
            /// <param name="pagingInfo">object containing paging info for getting results from a cached search. If hitlistid is 0, no hitlist joining is made</param>
            /// <param name="dataView">object containing the data view or an id of it</param>
            /// <param name="useRealTableNamesStr">string that specifies if real table names are supposed to be returned (yes/true), if table names in the form Table_tableid (no/false) or if this is up o the search service (null/empty)</param>
            /// <param name="connStringType">Connection string type. Owner/OwnerProxy/Proxy</param>
            /// <param name="originalOrderByCriteria">Order by criteria</param>
            /// <param name="searchCriteria">Search criteria to filter</param>
            /// <returns>DataSet containing results</returns>
            public DataSet GetData(ResultsCriteria resultsCriteria, PagingInfo pagingInfo, COEDataView dataView, ConnStringType connStringType, string useRealTableNamesStr, OrderByCriteria originalOrderByCriteria, SearchCriteria searchCriteria)
            {
                string methodSignature = MethodBase.GetCurrentMethod().DeclaringType.Name + "->" + MethodBase.GetCurrentMethod().Name.ToUpper() + " ";

                List<SearchProcessor>[] childFilterSearchPocessors = new List<SearchProcessor>[0];
                DataSet returnDataSet = new DataSet("SearchResults");
                //Initialize extended properties related to filtering of child data
                returnDataSet.ExtendedProperties.Add(FilteredChildData, pagingInfo.FilterChildData);
                returnDataSet.ExtendedProperties.Add(FilteredChildDataMessage, null);
                dataView = this.ApplyDataviewFilter(dataView, resultsCriteria);

                //If highlight substructure is specified the fields results criteria of the structures are going to be replaced with HighlightedStructure result criteria
                if (pagingInfo.HighlightSubStructures)
                {
                    this.TurnStructureCriteriaIntoHighligtStructure(ref resultsCriteria, dataView);
                }
                this.CheckPKIsPresent(dataView);
                bool useRealTableNames = false;
                bool avoidHitlistTable = pagingInfo.HitListID <= 0;
                bool avoidPaging = pagingInfo.RecordCount <= 0 && pagingInfo.End <= 0;

                //If not present the property, gather it form configurations.
                if (string.IsNullOrEmpty(useRealTableNamesStr))
                {
                    if (ServiceConfigData != null && ServiceConfigData.SearchServiceData != null)
                        if (!string.IsNullOrEmpty(ServiceConfigData.SearchServiceData.UseRealTableNames))
                            useRealTableNames = (ServiceConfigData.SearchServiceData.UseRealTableNames.ToLower() == "yes" || ServiceConfigData.SearchServiceData.UseRealTableNames.ToLower() == "true");
                }
                else
                    useRealTableNames = (useRealTableNamesStr.ToLower() == "yes" || useRealTableNamesStr.ToLower() == "true");

                if (originalOrderByCriteria == null)
                    originalOrderByCriteria = new OrderByCriteria();

                try
                {
                    _coeLog.LogStart(methodSignature, 1, System.Diagnostics.SourceLevels.All);
                    object[] hitListAppDomain = (object[])AppDomain.CurrentDomain.GetData(pagingInfo.HitListID.ToString());
                    if (hitListAppDomain == null
                        || ((int)hitListAppDomain[1]) > pagingInfo.End
                        || ((int)hitListAppDomain[1]) > (pagingInfo.Start + pagingInfo.RecordCount))
                    {
                        _databaseName = dataView.Database;

                        COEHitListBO hitList;
                        if (searchDAL == null)
                        {
                            LoadDAL(connStringType);
                            // Coverity Fix CID - 11581 
                            if (searchDAL == null)
                                throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));
                        }

                        hitList = COEHitListBO.Get(pagingInfo.HitListType, pagingInfo.HitListID);
                        if (hitList.OriginalHitListType == HitListType.TEMP)
                        {
                            hitListIDTableName = hitList.TempHitListIDTableName;
                            hitListTableName = hitList.TempHitListTableName;
                        }
                        else
                        {
                            hitListIDTableName = hitList.SavedHitListIDTableName;
                            hitListTableName = hitList.SavedHitListTableName;
                        }

                        if (pagingInfo.HitListID != hitList.HitListID)
                            pagingInfo.HitListID = hitList.HitListID;

                        DBMSType dbmsType = searchDAL.DALManager.DatabaseData.DBMSType;


                        InitKeepAlive(pagingInfo.KeepAlive);

                        /* Begin:
                         * Build the search criteria for the hitlistid 
                         */

                        // Move BaseTable to the beggining. FROM HERE ON IT'S ASSUMED BASETABLE IS THE FIRST ONE IN RESULTSCRITERIA, QUERIES AND RESULTSDATASET.TABLES
                        ResultsCriteria completeResultsCriteria = new ResultsCriteria();
                        ResultsCriteria parentResultsCriteria = new ResultsCriteria();
                        ResultsCriteria childrenResultsCriteria = new ResultsCriteria();


                        //Do this in a separate loop as collections are not to be modified while iterating, so it has to break after reinserting.
                        _coeLog.LogStart(methodSignature + "Forcing BaseTable to be the 1st.");

                        completeResultsCriteria = ResultsCriteria.GetResultsCriteria(resultsCriteria.ToString());
                        if (completeResultsCriteria[dataView.Basetable] == null)
                        {
                            ResultsCriteria.ResultsCriteriaTable parentTable = new ResultsCriteria.ResultsCriteriaTable();
                            parentTable.Id = dataView.Basetable;
                            parentResultsCriteria.Tables.Add(parentTable);
                            completeResultsCriteria.Tables.Insert(0, parentTable);
                        }

                        foreach (ResultsCriteria.ResultsCriteriaTable currentResultsCriteriaTable in completeResultsCriteria.Tables)
                        {
                            if (currentResultsCriteriaTable.Id == dataView.Basetable)
                            {
                                parentResultsCriteria.Tables.Add(currentResultsCriteriaTable);
                            }
                            else
                            {
                                childrenResultsCriteria.Add(currentResultsCriteriaTable);
                            }
                        }

                        _coeLog.LogEnd(methodSignature + "Forcing BaseTable to be the 1st.");

                        //The hitlist table must be added even when avoidHitlistTable, so that it gets in the cache properly built
                        dataView = this.GetDataViewWithHitlist(dataView);

                        if (!avoidHitlistTable)
                        {
                            if (completeResultsCriteria.SortByHitList == true)
                            {
                                ResultsCriteria.ResultsCriteriaTable hitListTableForSort = new ResultsCriteria.ResultsCriteriaTable();
                                hitListTableForSort.Id = hitListTableID;
                                completeResultsCriteria.Tables.Insert(1, hitListTableForSort);
                            }
                        }

                        if (searchCriteria == null)
                        {
                            searchCriteria = GetEmptySearchCriteria();
                        }

                        Common.SqlGenerator.MetaData.DataView dataViewLookUp = this.GetMetadataDataview(dataView, hitListTableName);
                        List<List<Relation>> relations = this.RetrieveRelations(completeResultsCriteria, dataViewLookUp);

                        //jhs add filter query to the outer query
                        //Filter Criterias are really just search criterias applied in GetData
                        //put it here so that it can muck with both filtering and the actual results criteria
                        SearchCriteria filterCriteria = new SearchCriteria();

                        //Create a transaction for the get data procedure. This is required for search processors thay might be used when filtering child data
                        _coeLog.LogStart(methodSignature + "Begin Transaction");
                        searchDAL.DALManager.BeginTransaction();
                        _coeLog.LogEnd(methodSignature + "Begin Transaction");

                        //Select processors of the parent table
                        List<SelectProcessor> parentProcessors = GetSelectProcessorList(parentResultsCriteria, pagingInfo, filterCriteria);

                        //Create the parent query
                        List<SearchProcessor> processors = GetSearchProcessorList(searchCriteria.Items, dataView);

                        _coeLog.LogStart(methodSignature + "Building Query for ParentTable Creation");
                        QueryBuilder queryBuilder = new QueryBuilder(dataViewLookUp, this.GetEmptySearchCriteria().ToString(), parentResultsCriteria.ToString());
                        Query parentQuery = queryBuilder.BuildQuery(dbmsType)[0];
                        //DGB later we need a copy of this query before the pagging is added to it
                        Query noPagingQuery = parentQuery;


                        //As partial searches are asynchronous and returning partial chunks, user sorting is not allowed and the sortin used in that case is hitlistsorting
                        if (hitListAppDomain != null && (parentQuery.GetOrderByClause().Count > 0 || originalOrderByCriteria.Items.Count > 0))
                            throw new Exception(Resources.SortinNotAllowedForPartialSearch);

                        OrderByCriteria parentOrderByCriteria = this.GetTableOrderByCriteria(dataView.Basetable, originalOrderByCriteria);
                        bool addSortOrder = (completeResultsCriteria.SortByHitList == true || (parentQuery.GetOrderByClause().Count <= 0 && parentOrderByCriteria.Items.Count <= 0)) && !avoidHitlistTable;

                        _coeLog.LogEnd(methodSignature + "Building Query for ParentTable Creation");

                        //if a sort by hitlist has been specified in the results criteria, the order by clauses need to be changed.
                        if (addSortOrder)
                        {
                            //next add a SelectClauseSortByHitList select clause
                            SelectClauseSortByHitList sortOrderCriteria = new SelectClauseSortByHitList(dataViewLookUp.GetColumn(hitListSortOrderFieldID));
                            //this value is gathered from a member variable set by GetSearchCriteriaFromHitList
                            sortOrderCriteria.Visible = false;

                            //Create an orderbyclause Item
                            OrderByClauseItem sortByHitlistOrderByItem = new OrderByClauseItem();
                            sortByHitlistOrderByItem.Direction = ResultsCriteria.SortDirection.ASC;
                            sortByHitlistOrderByItem.Item = sortOrderCriteria;
                            sortByHitlistOrderByItem.OrderByID = 1;

                            //create an orderbyclause and add the orderby item
                            OrderByClause orderByClause = new OrderByClause();
                            orderByClause.AddItem(sortByHitlistOrderByItem);

                            //set the orderbyclause for the parent query. This overwites all other order bys which is what we want.
                            noPagingQuery.SetOrderByClause(orderByClause);
                        }
                        else if (parentOrderByCriteria.Items.Count > 0)
                        {
                            OrderByClause orderByClause = new OrderByClause();

                            foreach (OrderByCriteria.OrderByCriteriaItem orderByCriteriaItem in parentOrderByCriteria.Items)
                            {
                                OrderByClauseItem sortByItem = new OrderByClauseItem();
                                sortByItem.Direction = orderByCriteriaItem.Direction == OrderByCriteria.OrderByDirection.ASC ? ResultsCriteria.SortDirection.ASC : ResultsCriteria.SortDirection.DESC;
                                sortByItem.OrderByID = orderByCriteriaItem.OrderIndex;
                                XmlDocument doc = new XmlDocument();
                                doc.LoadXml(orderByCriteriaItem.ResultCriteriaItem.ToString());
                                // Coverity Fix CID - 10863  (from local server)
                                if (doc.FirstChild != null)
                                {
                                    XmlNode siblingNode = doc.FirstChild.NextSibling;
                                    if (siblingNode != null)
                                        sortByItem.Item = SelectClauseFactory.CreateSelectClauseItem(siblingNode, dataViewLookUp);
                                }
                                orderByClause.AddItem(sortByItem);
                            }
                            noPagingQuery.SetOrderByClause(orderByClause);
                        }

                        //Fix pagingInfo information (End and RecordCount are redundant)
                        //If end and record count are both 0, paging is not requested.
                        if (pagingInfo.Start <= 0)
                            pagingInfo.Start = 1;

                        if (pagingInfo.RecordCount > 0)
                        {
                            pagingInfo.End = pagingInfo.Start + (pagingInfo.RecordCount - 1);
                            pagingInfo.RecordCount = 0;
                        }
                        if (pagingInfo.End > 0)
                        {
                            pagingInfo.RecordCount = pagingInfo.End + 1 - pagingInfo.Start;
                        }

                        Query fullPageQuery = null;
                        if (pagingInfo.Start >= 1)
                            fullPageQuery = this.BuildCOEFullPageQuery(dataViewLookUp, dataView, noPagingQuery, avoidHitlistTable, dbmsType, pagingInfo, resultsCriteria.SortByHitList, searchCriteria);
                        Query currentPageQuery = this.BuildCOECurrentPageQuery(pagingInfo);
#if DEBUG
                        System.Diagnostics.Debug.WriteLine("Parent Query (without paging)");
                        parentQuery.UseParameters = false;
                        System.Diagnostics.Debug.WriteLine(parentQuery.ToString());
                        System.Diagnostics.Debug.WriteLine("----------------------");
                        parentQuery.UseParameters = true;
#endif
                        _coeLog.Log(methodSignature + "Parent Query: " + parentQuery.ToString());

                        parentQuery = this.SurroundWithOuterQuery(parentQuery, dbmsType, dataView, this.GetEmptySearchCriteria());
                        parentQuery = this.RefactorQueryForAggregateFunctions(parentQuery, parentQuery.GetMainTable() as Query, dataViewLookUp, false);

                        if (pagingInfo.Start >= 1)
                        {
                            foreach (SearchProcessor processor in processors)
                            {
                                //Jordans suggest to figure out a mechanism to not call a processor that does not do anything on Pre/Post/Process
                                string type = processor.GetType().Name;
                                //coeLog.Log(type + " Process", 1, System.Diagnostics.SourceLevels.All);
                                processor.Process(fullPageQuery.GetMainTable() as Query);
                                //coeLog.Log(type + " Process", 1, System.Diagnostics.SourceLevels.All);
                            }
                            this.InsertIntoCOEFullPage(fullPageQuery);
                        }

                        this.InsertIntoCOECurrentPage(currentPageQuery);
                        _coeLog.LogStart(methodSignature + "Building Query for ChildTables Creation");

                        //If filter child data is specified, the original search is going to be retrived.
                        //Then only the criteria that filters child tables are going to be added to childFilterCriteria
                        SearchCriteria childFilterCriteria = new SearchCriteria();

                        if (pagingInfo.FilterChildData)
                        {
                            //Save query history needs to be enabled to get the original search criteria for the coesearchcriteria table
                            if (IsSaveQueryHistoryEnabled())
                            {
                                //If no hitlist id is specified that means a user requested browse (Retrieve All)
                                if (pagingInfo.HitListID > 0)
                                {
                                    COEHitListBO hlBO = COEHitListBO.Get(pagingInfo.HitListType, pagingInfo.HitListID);
                                    //If the hitlist do not have a search criteria, but savequeryhistory is enabled, that means the hl was created manually or by
                                    //performing hitlist operations (Union, Intersect...)
                                    if (hlBO != null && hlBO.SearchCriteriaID > 0)
                                    {
                                        SearchCriteria originalCriteria = COESearchCriteriaBO.Get(hlBO.SearchCriteriaType, hlBO.SearchCriteriaID).SearchCriteria;
                                        foreach (SearchCriteria.SearchExpression sc in originalCriteria.Items)
                                        {
                                            //Only add the criterium that filters child tables. No if the criteria is against a field in base table, should not be added.
                                            if (sc is SearchCriteria.SearchCriteriaItem && string.IsNullOrEmpty(((SearchCriteria.SearchCriteriaItem)sc).AggregateFunctionName) && ((SearchCriteria.SearchCriteriaItem)sc).TableId != dataView.Basetable)
                                            {
                                                childFilterCriteria.Items.Add(sc);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        returnDataSet.ExtendedProperties[FilteredChildDataMessage] = "No search criteria associated with the hitlist. Causes of this can be a) SaveQueryHistory was disabled at the time the search was performed, or b) The hitlist was not created with a search. No filter applied.";
                                        returnDataSet.ExtendedProperties[FilteredChildData] = false;
                                    }

                                    if (childFilterCriteria.Items.Count == 0)
                                    {
                                        returnDataSet.ExtendedProperties[FilteredChildDataMessage] = "There is no search against child tables.";
                                    }
                                }
                                else
                                {
                                    if (searchCriteria.Items.Count == 0)
                                        returnDataSet.ExtendedProperties[FilteredChildDataMessage] = "Retrieveing all the results. No filter applied.";
                                    else
                                    {
                                        foreach (SearchCriteria.SearchExpression sc in searchCriteria.Items)
                                        {
                                            //Only add the criterium that filters child tables. No if the criteria is against a field in base table, should not be added.
                                            if (sc is SearchCriteria.SearchCriteriaItem && string.IsNullOrEmpty(((SearchCriteria.SearchCriteriaItem)sc).AggregateFunctionName) && ((SearchCriteria.SearchCriteriaItem)sc).TableId != dataView.Basetable)
                                            {
                                                childFilterCriteria.Items.Add(sc);
                                            }
                                        }

                                        if (childFilterCriteria.Items.Count == 0)
                                        {
                                            returnDataSet.ExtendedProperties[FilteredChildDataMessage] = "There is no search against child tables.";
                                        }
                                    }
                                }

                            }
                            else
                            {
                                returnDataSet.ExtendedProperties[FilteredChildDataMessage] = "Save query history feature is disabled and thus the filtering cannot be applied. Turn SaveQueryHistory on for using this feature.";
                                returnDataSet.ExtendedProperties[FilteredChildData] = false;
                            }
                        }

                        //jhs child select processors
                        List<SelectProcessor> childProcessors = GetSelectProcessorList(childrenResultsCriteria, pagingInfo, childFilterCriteria);

                        // Create children queries. Parent query has already been filtered.
                        searchCriteria = new SearchCriteria();

                        queryBuilder = new QueryBuilder(dataViewLookUp, searchCriteria.ToString(), childrenResultsCriteria.ToString());

                        //Retrieve the lookup fields those are used in searching - CSBR-157359
                        Query[] childrenQueries = null;
                        if (pagingInfo.FilterChildData)
                        {
                            Dictionary<int, int> fieldsInLookupSearches = queryBuilder.GetLookupFields(dataView);

                            childrenQueries = queryBuilder.BuildQuery(dbmsType, fieldsInLookupSearches);
                        }
                        else
                        {
                            childrenQueries = queryBuilder.BuildQuery(dbmsType);
                        }

                        childFilterSearchPocessors = new List<SearchProcessor>[childrenQueries.Length];

                        //Loop over the child tables queries and append to each the corresponding filters.
                        for (int ix = 0; ix < childrenQueries.Length; ix++)
                        {
                            childFilterSearchPocessors[ix] = new List<SearchProcessor>();

                            //If there are filters to be applied to the child tables
                            if (childFilterCriteria.Items.Count > 0)
                            {
                                //Get a new searchcriteria that has only the items of current table.
                                SearchCriteria filterForTbl = childFilterCriteria.GetSubset(childrenQueries[ix].MainTableID);
                                if (filterForTbl.Items.Count > 0)
                                {
                                    //We found a filter to be added to the current query. So now we need to recreate the processors mechanism for the
                                    //SearchCriteria.

                                    //Pre process for the filter child criteria (subset)
                                    childFilterSearchPocessors[ix] = this.GetSearchProcessorList(filterForTbl.Items, dataView);

                                    //Get the search criteria metadata to build a where clause
                                    Common.SqlGenerator.MetaData.SearchCriteria sc = new Common.SqlGenerator.MetaData.SearchCriteria();
                                    sc.LoadFromXML(filterForTbl.ToString());
                                    WhereClause wc = sc.GetWhereClause(dataViewLookUp);
                                    foreach (WhereClauseItem item in wc.Items)
                                    {
                                        //Append the where clause item to the child query
                                        childrenQueries[ix].AddWhereItem(item);
                                    }
                                    //Continuing with the processor mechanims, call process.
                                    foreach (SearchProcessor processor in childFilterSearchPocessors[ix])
                                    {
                                        string type = processor.GetType().Name;
                                        _coeLog.Log(type + " Process", 1, System.Diagnostics.SourceLevels.All);
                                        processor.Process(childrenQueries[ix]);
                                        _coeLog.Log(type + " Process", 1, System.Diagnostics.SourceLevels.All);
                                    }
                                }
                            }
                        }

                        _coeLog.LogEnd(methodSignature + "Building Query for ChildTables Creation");

                        Query parentIdQuery = null;
                        if (childrenQueries.Length > 0)
                        {

                            //DGB moved the creation of parentIDResultsCriteria from above because it is not needed 
                            //unless there are child tables.  Also pass the parentQuery into the function because we need
                            //to reuse its order by information
                            //DGB  Pass the noPagingQuery instead because it is the one that has the sorting information.
                            ResultsCriteria parentIdResultsCriteria = this.BuildParentTableResultsCriteria(dataView, noPagingQuery, avoidPaging);


                            _coeLog.LogStart(methodSignature + "Building Query for ChildTables paging");
                            // Create the parentID query, that only contains an ID column - used for creating child queries
                            // DGB reworked to ensure that the parentIDQuery has the same order by clauses as the original parentQuery
                            queryBuilder = new QueryBuilder(dataViewLookUp, searchCriteria.ToString(), parentIdResultsCriteria.ToString());
                            parentIdQuery = queryBuilder.BuildQuery(dbmsType)[0];
                            _coeLog.LogEnd(methodSignature + "Building Query for ChildTables paging");

                            parentIdQuery = this.SurroundWithOuterQuery(parentIdQuery, dbmsType, dataView, searchCriteria);

                            parentIdQuery.Alias = "HITLISTQUERY";
                            parentIdQuery.SetMainTable(parentQuery.GetMainTable());
                            parentIdQuery.EncloseInParenthesis = true;

                            _coeLog.LogStart(methodSignature + "Adding relationships between each childtable and the parent to the QUERY");
                            for (int index = 0; index < childrenResultsCriteria.Tables.Count; index++)
                            {
                                Relation parentBaseRelation = new Relation();
                                parentBaseRelation.Child = new Field("ID", DbType.Int32, parentIdQuery);
                                parentBaseRelation.Parent = dataViewLookUp.GetField(int.Parse(dataView.BaseTablePrimaryKey));

                                List<Relation> relationPath = new List<Relation>();
                                relationPath.AddRange(dataViewLookUp.GetRelations(childrenResultsCriteria.Tables[index].Id, dataView.Basetable));

                                relationPath.Add(parentBaseRelation);

                                foreach (Relation currentRelation in relationPath)
                                {
                                    childrenQueries[index].AddJoinRelation(currentRelation);
                                }

                                foreach (OrderByCriteria.OrderByCriteriaItem item in this.GetTableOrderByCriteria(childrenResultsCriteria.Tables[index].Id, originalOrderByCriteria).Items)
                                {
                                    XmlDocument doc = new XmlDocument();
                                    doc.LoadXml(item.ResultCriteriaItem.ToString());
                                    OrderByClauseItem orderByClauseItem = new OrderByClauseItem();
                                    orderByClauseItem.Direction = item.Direction == OrderByCriteria.OrderByDirection.ASC ? ResultsCriteria.SortDirection.ASC : ResultsCriteria.SortDirection.DESC;
                                    orderByClauseItem.OrderByID = item.OrderIndex;
                                    orderByClauseItem.Item = SelectClauseFactory.CreateSelectClauseItem(doc.DocumentElement, dataViewLookUp);
                                    childrenQueries[index].AddOrderByItem(orderByClauseItem);
                                }

                                // Inject a hint into the parentIdQuery statement if provided in the SQLGenerator tag of COEFrameworkconfig.xml
                                SQLGeneratorData sqlGenerateData = ConfigurationUtilities.GetSQLGeneratorData(searchDAL.DALManager.DatabaseData);
                                if (sqlGenerateData.InnerQueryAliasHint != string.Empty)
                                {
                                    // The parentIdQuery is reused for each child table so we only need to add the hint once
                                    if (parentIdQuery.Hints.Count == 0)
                                    {
                                        parentIdQuery.Hints.Add(sqlGenerateData.InnerQueryAliasHint);
                                    }
                                }

                                childrenQueries[index] = this.RefactorQueryForAggregateFunctions(childrenQueries[index], parentIdQuery, dataViewLookUp, true);
                            }
                            _coeLog.LogEnd(methodSignature + "Adding relationships between each childtable and the parent to the QUERY");
#if DEBUG
                            foreach (Query currentQuery in childrenQueries)
                            {
                                currentQuery.UseParameters = false;
                                System.Diagnostics.Debug.WriteLine(currentQuery.ToString());
                                System.Diagnostics.Debug.WriteLine("----------------------");
                                currentQuery.UseParameters = true;
                            }
#endif
                        }
                        /* Execute the query and fill dataset to be returned */
                        if (this.keepAlive)
                        {
                            noPagingQuery.EncloseInParenthesis = false;

                            if (!directCall)
                            {
                                if (pagingInfo.TransactionId != null && pagingInfo.TransactionId != string.Empty)
                                    holder = (KeepAliveHolder)AppDomain.CurrentDomain.GetData(pagingInfo.TransactionId);
                                else
                                {
                                    Random random = new Random(System.DateTime.Now.Millisecond);
                                    //here we need to get the IdentityToken from the 
                                    pagingInfo.TransactionId = Csla.ApplicationContext.User.Identity.Name + random.Next();
                                }
                                if (holder == null)
                                {
                                    holder = searchDAL.CreateKeepAliveHolder(noPagingQuery, !directCall, pagingInfo.TransactionId);
                                    AppDomain.CurrentDomain.SetData(pagingInfo.TransactionId, holder);
                                }
                                else if (holder.LastRecord > pagingInfo.Start)
                                {
                                    AppDomain.CurrentDomain.SetData(pagingInfo.TransactionId, null);
                                    holder.Dispose();
                                    holder = searchDAL.CreateKeepAliveHolder(noPagingQuery, !directCall, pagingInfo.TransactionId);
                                    AppDomain.CurrentDomain.SetData(pagingInfo.TransactionId, holder);
                                }
                            }
                            else if (holder != null && holder.LastRecord >= pagingInfo.Start)
                            {
                                holder.Dispose();
                                holder = searchDAL.CreateKeepAliveHolder(noPagingQuery, !directCall, pagingInfo.TransactionId);
                            }
                            else if (holder == null)
                            {
                                holder = searchDAL.CreateKeepAliveHolder(noPagingQuery, !directCall, pagingInfo.TransactionId);
                            }

                            string baseTableName = string.Empty;

                            if (!useRealTableNames)
                            {
                                baseTableName = "Table_" + dataView.Basetable;
                            }
                            else
                                baseTableName = GetTableName(dataView, dataView.Basetable);


                            DataSet dataSet = searchDAL.ExecuteDataSetKeepAlive(baseTableName, pagingInfo, holder);
                            returnDataSet.Merge(dataSet);
                            pagingInfo.RecordCount = dataSet.Tables[0].Rows.Count;

                            for (int i = 0; i < childrenQueries.Length; i++)
                            {
                                childrenQueries[i].EncloseInParenthesis = false;
                                DataTable table = searchDAL.ExecuteDataTable(childrenQueries[i]);
                                if (!useRealTableNames)
                                {
                                    table.TableName = "Table_" + childrenQueries[i].MainTableID;
                                }
                                else
                                {
                                    table.TableName = GetTableName(dataView, childrenQueries[i].MainTableID);
                                }

                                if (table.Columns.CanRemove(table.Columns[Resources.SortOrderField]))
                                    table.Columns.Remove(Resources.SortOrderField);

                                returnDataSet.Tables.Add(table);
                            }
                        }
                        else
                        {
                            if (holder != null)
                            {
                                holder.Dispose();
                                holder = null;
                            }

#if DEBUG
                            System.Diagnostics.Debug.WriteLine("Parent Query (with paging)");
                            parentQuery.UseParameters = false;
                            System.Diagnostics.Debug.WriteLine(parentQuery.ToString());
                            System.Diagnostics.Debug.WriteLine("----------------------");
                            parentQuery.UseParameters = true;
#endif
                            _coeLog.Log("Parent Query (with paging): " + parentQuery.ToString(), 10);

                            parentQuery.EncloseInParenthesis = false;
                            DataTable table = searchDAL.ExecuteDataTable(parentQuery);

                            for (int j = 0; j < processors.Count; j++)
                            {
                                processors[j].PostProcess(searchDAL);
                            }

                            if (table.Columns.CanRemove(table.Columns[Resources.SortOrderField]))
                                table.Columns.Remove(Resources.SortOrderField);

                            if (!useRealTableNames)
                                table.TableName = "Table_" + dataView.Basetable;
                            else
                                table.TableName = GetTableName(dataView, dataView.Basetable);

                            returnDataSet.Tables.Add(table);
                            pagingInfo.RecordCount = table.Rows.Count;
                            //ljb 1/2008 refix the issue so that the start value is considered.
                            pagingInfo.End = pagingInfo.Start + pagingInfo.RecordCount - 1;

                            for (int i = 0; i < childrenQueries.Length; i++)
                            {
                                childrenQueries[i].EncloseInParenthesis = false;
#if DEBUG
                                System.Diagnostics.Debug.WriteLine("Child Query (with paging)");
                                childrenQueries[i].UseParameters = false;
                                System.Diagnostics.Debug.WriteLine(childrenQueries[i].ToString());
                                System.Diagnostics.Debug.WriteLine("----------------------");
                                childrenQueries[i].UseParameters = true;
#endif
                                table = searchDAL.ExecuteDataTable(childrenQueries[i]);
                                _coeLog.Log(methodSignature + "Child Query: " + childrenQueries[i].ToString());

                                //After the query was executed, if there where filters, the post process neeeds to be called to complete the processor mechanism
                                for (int j = 0; j < childFilterSearchPocessors[i].Count; j++)
                                {
                                    childFilterSearchPocessors[i][j].PostProcess(searchDAL);
                                }
                                if (!useRealTableNames)
                                {
                                    table.TableName = "Table_" + childrenQueries[i].MainTableID;
                                }
                                else
                                {
                                    table.TableName = GetTableName(dataView, childrenQueries[i].MainTableID);
                                }

                                //Remove not desired column sort order
                                if (table.Columns.CanRemove(table.Columns[Resources.SortOrderField]))
                                    table.Columns.Remove(Resources.SortOrderField);

                                if (!returnDataSet.Tables.Contains(table.TableName))
                                    returnDataSet.Tables.Add(table);
                            }
                        }

                        //ASUMES THE BASETABLE IS THE FIRST ONE IN THE RELATIONS AND RETURNDATASET.TABLES LISTS.
                        _coeLog.LogStart(methodSignature + "Building Relationships for Dataset");

                        //Add relationships between the datatables inside the dataset
                        returnDataSet.EnforceConstraints = false; // To allow null values in relationships
                        foreach (List<Relation> currentRelationGroup in relations)
                        {
                            foreach (Relation currentRelation in currentRelationGroup)
                            {
                                string parentColName = GetColumnName(dataViewLookUp, resultsCriteria, currentRelation.Parent);
                                string childColName = GetColumnName(dataViewLookUp, resultsCriteria, currentRelation.Child);

                                DataTable parentTable = null;
                                DataTable childTable = null;

                                if (!useRealTableNames)
                                {
                                    parentTable = returnDataSet.Tables["Table_" + ((Table)currentRelation.Parent.Table).TableId];
                                    childTable = returnDataSet.Tables["Table_" + ((Table)currentRelation.Child.Table).TableId];
                                }
                                else
                                {
                                    parentTable = returnDataSet.Tables[GetTableName(dataView, ((Table)currentRelation.Parent.Table).TableId)];
                                    childTable = returnDataSet.Tables[GetTableName(dataView, ((Table)currentRelation.Child.Table).TableId)];
                                }

                                if (parentTable != null && childTable != null)
                                {
                                    //Child table fields appear blank when an alias table is added to the dataview. CBOE-901
                                    // In case we have dupicate tables, then their names are same hence it will not get add into DataRelationCollection,hence realtions are identified using alias.
                                    string parentTableName = string.IsNullOrEmpty(dataView.Tables.getById(((Table)currentRelation.Parent.Table).TableId).Alias) ? dataView.Tables.getById(((Table)currentRelation.Parent.Table).TableId).Name : dataView.Tables.getById(((Table)currentRelation.Parent.Table).TableId).Alias;
                                    string childTableName = string.IsNullOrEmpty(dataView.Tables.getById(((Table)currentRelation.Child.Table).TableId).Alias) ? dataView.Tables.getById(((Table)currentRelation.Child.Table).TableId).Name : dataView.Tables.getById(((Table)currentRelation.Child.Table).TableId).Alias;

                                    string relationName = string.Format("{0}({1})->{2}({3})",
                                                                            parentTableName,
                                                                            parentColName,
                                                                            childTableName,
                                                                            childColName);

                                    DataRelation datarelation = new DataRelation(relationName,
                                                                parentTable.Columns[parentColName],

                                                                childTable.Columns[childColName],
                                                                false
                                                                );

                                    if (!returnDataSet.Relations.Contains(relationName))
                                    {

                                        datarelation.Nested = IsParentNestedWithChildTable(parentTable, childTable);//Fixed CSBR-152906
                                        returnDataSet.Relations.Add(datarelation);
                                    }
                                }
                            }
                        }

                        if (holder != null)
                            holder.LastRecord = pagingInfo.End;

                        if (pagingInfo.HitListID > 0 && pagingInfo.AddToRecent)
                            hitList.AddToRecent();
                    }
                    else
                    {
                        if (hitListAppDomain != null)
                            throw new Exception(Resources.PageNotAvailable);
                    }
                }
                catch (Exception exception)
                {
                    _coeLog.Log(methodSignature + "Exception: " + exception.Message, 1, System.Diagnostics.SourceLevels.Error);

                    _coeLog.LogStart(methodSignature + "Rollback Transaction");
                    searchDAL.DALManager.RollbackTransaction();
                    _coeLog.LogEnd(methodSignature + "Rollback Transaction");

                    string errorMessage = string.Empty;
                    Type exceptionType = exception.GetType();
                    switch (exceptionType.Name)
                    {
                        case "SQLGeneratorException":
                            errorMessage = Resources.SQLGeneratorErrorMessage;
                            break;
                        case "MolServerException":
                            errorMessage = Resources.MolServerErrorMessage;
                            break;
                        case "UnsupportedDataTypeException":
                            errorMessage = Resources.UnsupportedDataType;
                            break;
                        case "OracleException":
                            errorMessage = Resources.DatabaseErrorMessage;
                            // Conver OracleException to common exception.
                            // Eliminate the depdencies to OracleDataAccess for client side
                            exception = new Exception(exception.Message);
                            break;
                        case "ApplicationException":
                            errorMessage = exception.Message;
                            break;
                        default:
                            errorMessage = exception.Message;
                            break;
                    }

                    //add the query that is causing the issue to nofications
                    NotificationUtility.AddNotification(ref notifications, null, errorMessage);

                    //decide whether to throw the error and add a noficiation.
                    HandleError(exception);
                }
                finally
                {
                    _coeLog.LogEnd(methodSignature);
                }

                //Commit the transaction
                _coeLog.LogStart(methodSignature + "Commit Transaction");

                if (commitTransaction)
                    searchDAL.DALManager.CommitTransaction();

                _coeLog.LogEnd(methodSignature + "Commit Transaction");

                return returnDataSet;
            }

            /// <summary>
            /// Builds the current page query that later on will be used to insert into COEDB.COECurrentPage(BasePrimaryKey, SortOrder, ClientId).
            /// </summary>
            /// <param name="pagingInfo"></param>
            /// <returns>A Query</returns>
            private Query BuildCOECurrentPageQuery(PagingInfo pagingInfo)
            {
                var avoidPaging = pagingInfo.End <= 0 && pagingInfo.RecordCount <= 0;
                Query outerQuery = new Query();
                outerQuery.SetMainTable(new Table(Resources.CentralizedStorageDB + "." + Resources.COEFullPageTable));
                Field primaryKeyField = new Field(Resources.BaseTablePrimaryKeyField, DbType.Int32);
                Field sortOrderField = new Field(Resources.SortOrderField, DbType.Int32);
                Field clientIDField = new Field(Resources.ClientIdField, DbType.String);

                outerQuery.AddSelectItem(new SelectClauseField(primaryKeyField));
                outerQuery.AddSelectItem(new SelectClauseField(sortOrderField));

                if (!avoidPaging)
                {
                    string ClientIdValue = Csla.ApplicationContext.User.Identity.Name;
                    WhereClauseEqual equalConstraint = new WhereClauseEqual();
                    equalConstraint.DataField = clientIDField;
                    equalConstraint.Val = new Value(ClientIdValue.ToUpper(), DbType.String);
                    outerQuery.AddWhereItem(equalConstraint);

                    WhereClauseGreaterThan greaterEqual = new WhereClauseGreaterThan();
                    greaterEqual.GreaterEqual = true;
                    greaterEqual.DataField = sortOrderField;
                    greaterEqual.Val = new Value(pagingInfo.Start.ToString(), DbType.Int32);
                    outerQuery.AddWhereItem(greaterEqual);

                    WhereClauseLessThan lessEqual = new WhereClauseLessThan();
                    lessEqual.LessEqual = true;
                    lessEqual.DataField = sortOrderField;
                    lessEqual.Val = new Value(pagingInfo.End.ToString(), DbType.Int32);
                    outerQuery.AddWhereItem(lessEqual);
                }
                return outerQuery;
            }

            /// <summary>
            /// Builds the coe full page query that later on will be used to insert into COEDB.COEFullPage(BasePrimaryKey, SortOrder, ClientId). There is some
            /// extra step prior to actually inserting, which is executing processors. That is why insertion method is separated from query construction.
            /// </summary>
            /// <param name="dataviewLookup"></param>
            /// <param name="dataView"></param>
            /// <param name="noPagingQuery"></param>
            /// <param name="avoidHitlist"></param>
            /// <param name="dbmsType"></param>
            /// <param name="pagingInfo"></param>
            /// <param name="sortByHitList"></param>
            /// <returns>A Query</returns>
            private Query BuildCOEFullPageQuery(CambridgeSoft.COE.Framework.Common.SqlGenerator.MetaData.DataView dataviewLookup, COEDataView dataView, Query noPagingQuery, bool avoidHitlist, DBMSType dbmsType, PagingInfo pagingInfo, bool sortByHitList, SearchCriteria searchCriteria)
            {
            bool avoidPaging = pagingInfo.End <= 0 && pagingInfo.RecordCount <= 0;

                //Build a resultCriteria that only have the parent table id.
            ResultsCriteria parentIDRC = this.BuildParentTableResultsCriteria(dataView, noPagingQuery, avoidPaging);
            QueryBuilder builder = new QueryBuilder(dataviewLookup, searchCriteria.ToString(), parentIDRC.ToString());

                //The current page query only have the primary key and the search filters. There might be sorting and group bys.
                Query currentPageQuery = builder.BuildQuery(dbmsType)[0];

                // Remove where clause since it will reduce performance greatly
                currentPageQuery.RemoveWhereClause();

                //As Order By orders AFTER the query is filtered, it is no good for paging. So we are going to use a Analytical RowNum instead.
            SelectClauseAnalyticalRowNum analyticalSortOrder = new SelectClauseAnalyticalRowNum();
                analyticalSortOrder.Alias = Resources.SortOrderField;

                Field pkField = currentPageQuery.GetSelectClauseItems()[0].DataField as Field;
            SelectClauseField selectClausePK = new SelectClauseField(pkField);
                List<OrderByClauseItem> originalOrderBy = noPagingQuery.GetOrderByClause();

                // So, let's loop the order by clauses and put them into the analytical row num, and ensure that the tables involved are present
                // in the list of tables.
                if ((originalOrderBy.Count > 0) && !sortByHitList)
                {
                    analyticalSortOrder.Clauses = originalOrderBy;

                    foreach (OrderByClauseItem item in originalOrderBy)
                    {
                        // The DataField may be a simple Field or a Lookup.  
                        // Here we need to cast to IColumn because Lookup cannot be cast to IField
                        IColumn col = item.Item.DataField as IColumn;
                        //If the table is not the base table

                        if (col.Table.GetAlias() != pkField.Table.GetAlias())
                        {
                            //Build this fake relation to actually add the child table to the table list.
                            List<Relation> rels = dataviewLookup.GetRelations(dataviewLookup.GetBaseTableId(), dataviewLookup.GetParentTableId(col.FieldId.ToString()));
                            foreach (Relation rel in rels)
                            {
                                currentPageQuery.AddJoinRelation(rel);
                            }
                        }
                    }
                }
                else
                {
                    //Sort was not specified, but paging was requested, so we need to add a arbitrary sortorder. We have choosen the primary key
                    //as the sort in this case.
                    if (!avoidPaging)
                    {
                        //Add sorting for paging
                        OrderByClauseItem orderByPK = new OrderByClauseItem(selectClausePK);
                        orderByPK.Direction = ResultsCriteria.SortDirection.ASC;
                        orderByPK.Item = selectClausePK;
                        analyticalSortOrder.Clauses.Add(orderByPK);
                    }
                }

                // It is not a browse, a hitlist was already generated. Let's add a join to the hitlist then.
                if (!avoidHitlist)
                {
                    Table hitlistTable = new Table(dataView.Tables[dataView.Tables.Count - 1].Id, dataView.Tables[dataView.Tables.Count - 1].Name, dataView.Tables[dataView.Tables.Count - 1].Alias, dataView.Tables[dataView.Tables.Count - 1].Database);
                    Field hitList_HitListIDField = new Field(dataView.Tables[dataView.Tables.Count - 1].Fields[1].Name,
                                                      CambridgeSoft.COE.Framework.Common.SqlGenerator.Utils.TypesConversor.GetType(dataView.Tables[dataView.Tables.Count - 1].Fields[1].DataType.ToString()),
                                                      hitlistTable);
                    Field hitList_IDField = new Field(dataView.Tables[dataView.Tables.Count - 1].Fields[0].Name,
                                                      CambridgeSoft.COE.Framework.Common.SqlGenerator.Utils.TypesConversor.GetType(dataView.Tables[dataView.Tables.Count - 1].Fields[0].DataType.ToString()),
                                                      hitlistTable);
                    WhereClauseEqual hlClause = new WhereClauseEqual();
                    hlClause.DataField = hitList_HitListIDField;
                    hlClause.Val = new Value(pagingInfo.HitListID.ToString(), DbType.Int32);

                    currentPageQuery.AddWhereItem(hlClause);

                    Relation rel = new Relation();
                    rel.Parent = pkField;
                    rel.Child = hitList_IDField;
                    rel.InnerJoin = true;
                    currentPageQuery.AddJoinRelation(rel);
                }

                // Have we built a analytical row num?, then let's add it to the list of select clauses.
                if (analyticalSortOrder.Clauses.Count > 0)
                {
                    currentPageQuery.AddSelectItem(analyticalSortOrder);
                }

                // Now, we would create a group by clause. This clause has two puropses, if there are aggregated clauses, whether in the search criteria
                // or in the sorting, it is required. But, if there where no aggregate clauses, this serves as a "distinct" clause. Duplicated primary
                // keys are not allowed in COECURRENTPAGE.
                // We would always group by the primary key, and then we would group by all the sorting clauses (inside the analitycal row num) that
                // are not aggregated clauses.
                // GroupByClause groupByClause = new GroupByClause();
                // groupByClause.Items.Add(new GroupByClauseItem(selectClausePK));
                // foreach (OrderByClauseItem obClause in analyticalSortOrder.Clauses)
                // {
                // if (!obClause.Item.Equals(selectClausePK) && !(obClause.Item is SelectClauseAggregateFunction))

                // // For lookup fields, we cannot simply add the Lookup SelectClauseItem to the GroupBy clause because
                // // a subselect is not allowed in the group by.  Instead, we need to build a new SelectClauseField
                // // using the lookup field.  That is sufficient to ensure disticntnes of the results.
                // if (obClause.Item.DataField.GetType().ToString() == @"CambridgeSoft.COE.Framework.Common.SqlGenerator.Lookup")
                // {
                // // Get a reference to the lookup so that we can get the fieldName of the lookup field and
                // // and build a new field to pass to the GroupBy clause
                // Lookup luf = (Lookup)obClause.Item.DataField;
                // Field fld = new Field();
                // fld.FieldId = luf.FieldId;
                // fld.FieldName = luf.FieldName;
                // SelectClauseField scf = new SelectClauseField();
                // scf.DataField = fld;
                // groupByClause.Items.Add(new GroupByClauseItem(scf));
                // }
                // else
                // {
                // groupByClause.Items.Add(new GroupByClauseItem(obClause.Item));
                // }

                // }
                // currentPageQuery.SetGroupByClause(groupByClause);

                //As we have created the analytical row num, we are getting rid of the order by.
                currentPageQuery.SetOrderByClause(new OrderByClause());
                currentPageQuery.Alias = "InnerCurrentPageQuery";


                // The inner query at this point hold the ordered list of hits, now we want to get the right page from it.
                Query outerQuery = new Query();
                outerQuery.SetMainTable(currentPageQuery);

                Field primaryKeyField = new Field(Resources.ID, DbType.Int32, currentPageQuery);
                Field sortOrderField = new Field(Resources.SortOrderField, DbType.Int32, currentPageQuery);

                outerQuery.AddSelectItem(new SelectClauseField(primaryKeyField));
                // Outer query would select the base table primary key and the inner sort order. But if sort order was not requested AND paging was
                // not requested, the inner query has nothing but the id in its list. So in that particular case, in the outer query, we would insert the rownum
                // as sortorder.
                if (avoidPaging && analyticalSortOrder.Clauses.Count == 0)
                    outerQuery.AddSelectItem(new SelectClauseLiteral("rownum"));
                else
                    outerQuery.AddSelectItem(new SelectClauseField(sortOrderField));

                // Finally, let's add the where to get a single page.
                if (!avoidPaging)
                {
                    WhereClauseGreaterThan greaterEqual = new WhereClauseGreaterThan();
                    greaterEqual.GreaterEqual = true;
                    greaterEqual.DataField = sortOrderField;
                    greaterEqual.Val = new Value(pagingInfo.Start.ToString(), DbType.Int32);
                    outerQuery.AddWhereItem(greaterEqual);

                    WhereClauseLessThan lessEqual = new WhereClauseLessThan();
                    lessEqual.LessEqual = true;
                    lessEqual.DataField = sortOrderField;
                    lessEqual.Val = new Value(pagingInfo.End.ToString(), DbType.Int32);
                    outerQuery.AddWhereItem(lessEqual);
                }

                // Do not enclose the whole select in parenthesis, because this query is to be executed.
                // (ODP thows an error if the whole query is surrounded by parenthesis).
                outerQuery.EncloseInParenthesis = false;

                return outerQuery;
            }

            /// <summary>
            /// It inserts the current page query into database:
            /// INSERT INTO COEDB.COECURRENTPAGE(BASETABLEPRIMARYKEY, SORTORDER) (QUERY)
            /// </summary>
            /// <param name="query">The current page query to be inserted into the global temporary table (COEDB.COECURRENTPAGE)</param>
            private void InsertIntoCOECurrentPage(Query query)
            {
                Insert insert = new Insert();
                insert.MainTable = new Table(Resources.COECurrentPageTable);
                insert.MainTable.Database = Resources.CentralizedStorageDB;
                insert.Fields.Add(new Field(Resources.BaseTablePrimaryKeyField, DbType.Int32));
                insert.Fields.Add(new Field(Resources.SortOrderField, DbType.Double));
                insert.SelectStatement = query;

#if DEBUG
                System.Diagnostics.Debug.WriteLine("Current Page Insertion Query");
                insert.UseParameters = false;
                System.Diagnostics.Debug.WriteLine(insert.ToString());
                System.Diagnostics.Debug.WriteLine("----------------------");
                insert.UseParameters = true;
#endif
                searchDAL.ExecuteNonQuery(insert);
            }
            /// <summary>
            /// It inserts the full page query into database:
            /// INSERT INTO COEDB.COEFULLPAGE(BASETABLEPRIMARYKEY, SORTORDER) (QUERY)
            /// </summary>
            /// <param name="query">The full page query to be inserted into the COEFullPage table (COEDB.COEFULLPAGE)</param>
            private void InsertIntoCOEFullPage(Query query)
            {
                searchDAL.ClearCOEFullPageTable(); ; // Need to clear the current data in this table before inserting new set of search results.

                Insert insert = new Insert();
                insert.MainTable = new Table(Resources.COEFullPageTable);
                insert.MainTable.Database = Resources.CentralizedStorageDB;
                insert.Fields.Add(new Field(Resources.BaseTablePrimaryKeyField, DbType.Int32));
                insert.Fields.Add(new Field(Resources.SortOrderField, DbType.Double));
                insert.SelectStatement = query;
                searchDAL.ExecuteNonQuery(insert);
            }

            /// <summary>
            /// Looks for ResultCriteria.Field against the structure columns and turns them into HighlightedStructure criterias
            /// </summary>
            /// <param name="resultsCriteria">Original result criteria</param>
            /// <param name="dataView">Dataview messaging type</param>
            private void TurnStructureCriteriaIntoHighligtStructure(ref ResultsCriteria resultsCriteria, COEDataView dataView)
            {
                //Loop over the result criteria tables.
                for (int i = 0; i < resultsCriteria.Tables.Count; i++)
                {
                    //Loop each result criteria in the table
                    for (int j = 0; j < resultsCriteria.Tables[i].Criterias.Count; j++)
                    {
                        //Treat the criteria as field. If it was not a field, null was going to be returned.
                        ResultsCriteria.Field fld = resultsCriteria.Tables[i].Criterias[j] as ResultsCriteria.Field;
                        if (fld != null)
                        {
                            //Get the dataview field corresponding to the result criteria requested
                            //To see if the field is indexed by the cs cartridge, indicating a structure fild.
                            //A special case are lookup field, in which case the underlying field needs to be fetched to check if it is a structure field
                            COEDataView.Field dvField = dataView.GetFieldById(fld.Id);

                            if (dvField.LookupDisplayFieldId > 0)
                            {
                                //If a lookup, then fetch the underlying field to be displayed
                                dvField = dataView.GetFieldById(dvField.LookupDisplayFieldId);
                            }

                            //Now that we have the proper filed, let's see if it is a structure field
                            if (dvField.IndexType == COEDataView.IndexTypes.CS_CARTRIDGE)
                            {
                                //Create a new HighlightedStructure criteria and replace the field rc.
                                ResultsCriteria.HighlightedStructure hlStructure = new ResultsCriteria.HighlightedStructure();
                                hlStructure.Alias = fld.Alias;
                                hlStructure.Direction = fld.Direction;
                                hlStructure.Id = fld.Id;
                                hlStructure.OrderById = fld.OrderById;
                                hlStructure.Visible = fld.Visible;

                                resultsCriteria.Tables[i].Criterias[j] = hlStructure;
                            }
                        }
                    }
                }
            }

            /// <summary>
            /// Gets a metadatadataview from cache if the dataview handling option is not use the client dataview and if cached.
            /// </summary>
            /// <param name="dataView">The COEDataView</param>
            /// <param name="hitListTableName">The Hitlist table name. Used for looking at the cache.</param>
            /// <returns>A MetaData.DataView</returns>
            private Common.SqlGenerator.MetaData.DataView GetMetadataDataview(COEDataView dataView, string hitListTableName)
            {
                Common.SqlGenerator.MetaData.DataView result = null;
                switch (dataView.DataViewHandling)
                {
                    case COEDataView.DataViewHandlingOptions.USE_CLIENT_DATAVIEW:
                    case COEDataView.DataViewHandlingOptions.USE_SERVER_DATAVIEW:
                        result = new Common.SqlGenerator.MetaData.DataView(dataView);
                        break;
                    default:
                        string key = dataView.DataViewID.ToString() + "_" + dataView.Basetable.ToString() + "_" + dataView.Database + "_" + hitListTableName;
                        // If the dataviewbo was configured with server caching (or server and client) and the object was fetched already
                        // We would cache metadata.
                        if (LocalCache.Get(dataView.DataViewID.ToString(), typeof(COEDataViewBO)) != null)
                        {
                            result = LocalCache.Get(key, typeof(Common.SqlGenerator.MetaData.DataView)) as Common.SqlGenerator.MetaData.DataView;
                            if (result == null)
                            {
                                result = new Common.SqlGenerator.MetaData.DataView(dataView);
                                LocalCache.Add(key, typeof(Common.SqlGenerator.MetaData.DataView), result, LocalCache.NoAbsoluteExpiration, TimeSpan.FromMinutes(10), COECacheItemPriority.Normal);
                            }
                        }
                        // otherwise we won't because there won't be a way to invalidate the object
                        else
                        {
                            result = new Common.SqlGenerator.MetaData.DataView(dataView);
                        }
                        break;

                }
                return result;
            }

            /// <summary>
            /// Checks whether the dataview has a primary key, throws an exception if not.
            /// </summary>
            /// <param name="dataView"></param>
            private void CheckPKIsPresent(COEDataView dataView)
            {
                if (string.IsNullOrEmpty(dataView.BaseTablePrimaryKey))
                    throw new Exception(Resources.BaseTablePKNotSet);
            }

            /// <summary>
            /// Extracts all the order by criteria for a given tableid in a OrderByCriteria object.
            /// </summary>
            /// <param name="tableID">The table id we want to extract order bys for</param>
            /// <param name="orderByCriteria">The order by critaria</param>
            /// <returns>A new OrderByCriteria with is a subset of the original (For a single table)</returns>
            private OrderByCriteria GetTableOrderByCriteria(int tableID, OrderByCriteria orderByCriteria)
            {
                OrderByCriteria newCriteria = new OrderByCriteria();
                foreach (OrderByCriteria.OrderByCriteriaItem item in orderByCriteria.Items)
                {
                    if (item.TableID == tableID)
                    {
                        newCriteria.Items.Add(item);
                    }
                }
                return newCriteria;
            }

            /// <summary>
            /// Given a table id, looks for tha table name in a dataview.
            /// </summary>
            /// <param name="dataView">The dataview.</param>
            /// <param name="tableId">The tableid</param>
            /// <returns>The alias if present, the name otherwhise. If the table is no present in dataview it returns Table_{tableid}</returns>
            private string GetTableName(COEDataView dataView, int tableId)
            {
                string tableName = "Table_" + tableId;
                foreach (COEDataView.DataViewTable table in dataView.Tables)
                {
                    if (table.Id == tableId)
                    {
                        if (!string.IsNullOrEmpty(table.Alias))
                            tableName = table.Alias;
                        else
                            tableName = table.Name;
                    }
                }
                return tableName;
            }

            /// <summary>
            /// Gets the column name to be used, based on a field.
            /// </summary>
            /// <param name="dataViewLookUp">The dataview metadata</param>
            /// <param name="resultsCriteria">The results criteria</param>
            /// <param name="field">The field</param>
            /// <returns></returns>
            private string GetColumnName(CambridgeSoft.COE.Framework.Common.SqlGenerator.MetaData.DataView dataViewLookUp, ResultsCriteria resultsCriteria, Field field)
            {
                string methodSignature = MethodBase.GetCurrentMethod().DeclaringType.Name + "->" + MethodBase.GetCurrentMethod().Name.ToUpper() + " ";
                _coeLog.LogStart(methodSignature, 1, System.Diagnostics.SourceLevels.All);
                string result = dataViewLookUp.GetColumnAlias(field.FieldId);
                bool aliasFound = false;
                if (field.Table is Table)
                {
                    foreach (ResultsCriteria.ResultsCriteriaTable tbl in resultsCriteria.Tables)
                    {
                        if (tbl.Id == ((Table)field.Table).TableId)
                        {
                            foreach (ResultsCriteria.IResultsCriteriaBase rCritBase in tbl.Criterias)
                            {
                                if (rCritBase is ResultsCriteria.Field)
                                {
                                    if (((ResultsCriteria.Field)rCritBase).Id == field.FieldId && !string.IsNullOrEmpty(((ResultsCriteria.Field)rCritBase).Alias))
                                    {
                                        result = ((ResultsCriteria.Field)rCritBase).Alias;
                                        aliasFound = true;
                                        break;
                                    }
                                }
                            }
                            if (aliasFound)
                                break;
                        }
                    }
                }

                if (string.IsNullOrEmpty(result))
                    result = field.FieldName;

                _coeLog.LogEnd(methodSignature);
                return result;
            }

            /// <summary>
            /// Gets the column name to be used, based on a fieldid.
            /// </summary>
            /// <param name="dataViewLookUp">The dataview metadata</param>
            /// <param name="resultsCriteria">The Results Criteria</param>
            /// <param name="tableId">The table id</param>
            /// <param name="fieldId">The field id</param>
            /// <returns></returns>
            private string GetColumnName(CambridgeSoft.COE.Framework.Common.SqlGenerator.MetaData.DataView dataViewLookUp, ResultsCriteria resultsCriteria, int tableId, int fieldId)
            {
                string methodSignature = MethodBase.GetCurrentMethod().DeclaringType.Name + "->" + MethodBase.GetCurrentMethod().Name.ToUpper() + " ";
                _coeLog.LogStart(methodSignature, 1, System.Diagnostics.SourceLevels.All);
                string result = dataViewLookUp.GetColumnAlias(fieldId);
                bool aliasFound = false;

                foreach (ResultsCriteria.ResultsCriteriaTable tbl in resultsCriteria.Tables)
                {
                    if (tbl.Id == tableId)
                    {
                        foreach (ResultsCriteria.IResultsCriteriaBase rCritBase in tbl.Criterias)
                        {
                            if (rCritBase is ResultsCriteria.Field)
                            {
                                if (((ResultsCriteria.Field)rCritBase).Id == fieldId && !string.IsNullOrEmpty(((ResultsCriteria.Field)rCritBase).Alias))
                                {
                                    result = ((ResultsCriteria.Field)rCritBase).Alias;
                                    aliasFound = true;
                                    break;
                                }
                            }
                        }
                        if (aliasFound)
                            break;
                    }
                }


                if (string.IsNullOrEmpty(result))
                    result = dataViewLookUp.GetField(fieldId).FieldName;


                _coeLog.LogEnd(methodSignature);
                return result;
            }

            /// <summary>
            /// Adds the HitlistTable to the given dataview.
            /// </summary>
            /// <param name="dataView">A dataview</param>
            /// <returns>A dataview with the hitlist table added</returns>
            private COEDataView GetDataViewWithHitlist(COEDataView dataView)
            {
                return GetDataViewWithHitlist(dataView, this.hitListTableName);
            }

            /// <summary>
            /// Adds the HitlistTable to the given dataview. This override accept the hitlist table name..
            /// </summary>
            /// <param name="dataView">A dataview</param>
            /// <param name="tablename">Hitlist table name to be used.</param>
            /// <returns>A dataview with the hitlist table added</returns>
            private COEDataView GetDataViewWithHitlist(COEDataView dataView, string tablename)
            {
                _coeLog.LogStart("GetDataViewWithHitlist", 1, System.Diagnostics.SourceLevels.All);
                COEDataView resultDataView = new COEDataView();
                resultDataView.GetFromXML(dataView.ToString());
                COEDataView.DataViewTable hitListTable = new COEDataView.DataViewTable();
                hitListTable.Id = GetHighestTableID(dataView);
                hitListTableID = hitListTable.Id;
                hitListTable.Name = tablename;
                hitListTable.Alias = "hitlist";
                hitListTable.Database = string.Empty;
                COEDataView.Field id = new COEDataView.Field();
                id.DataType = COEDataView.AbstractTypes.Integer;
                id.Name = "ID";
                id.Id = GetHighestFieldID(dataView);
                id.ParentTableId = hitListTable.Id;
                hitListTable.Fields.Add(id);
                COEDataView.Field hitListId = new COEDataView.Field();
                hitListId.DataType = COEDataView.AbstractTypes.Integer;
                hitListId.Name = "HITLISTID";
                hitListId.Id = id.Id + 1;
                hitListId.ParentTableId = hitListTable.Id;
                hitListTable.Fields.Add(hitListId);
                COEDataView.Field sortOrderFieldId = new COEDataView.Field();
                sortOrderFieldId.DataType = COEDataView.AbstractTypes.Integer;
                sortOrderFieldId.Name = "SORTORDER";
                sortOrderFieldId.Id = hitListId.Id + 1;
                sortOrderFieldId.ParentTableId = hitListTable.Id;
                hitListSortOrderFieldID = sortOrderFieldId.Id;
                hitListTable.Fields.Add(sortOrderFieldId);

                resultDataView.Tables.Add(hitListTable);

                COEDataView.Relationship relationship = new COEDataView.Relationship();
                relationship.Parent = dataView.Basetable;
                relationship.Child = hitListTable.Id;
                relationship.ParentKey = int.Parse(dataView.BaseTablePrimaryKey);
                relationship.ChildKey = id.Id;
                relationship.JoinType = COEDataView.JoinTypes.INNER;

                resultDataView.Relationships.Add(relationship);
                _coeLog.LogEnd("GetDataViewWithHitlist", 1, System.Diagnostics.SourceLevels.All);
                return resultDataView;
            }

            /// <summary>
            /// Gets the highest field id in a given dataview
            /// </summary>
            /// <param name="dataView">The dataview</param>
            /// <returns>The highest field id</returns>
            private int GetHighestFieldID(COEDataView dataView)
            {
                int result = 0;
                foreach (COEDataView.DataViewTable table in dataView.Tables)
                {
                    foreach (COEDataView.Field field in table.Fields)
                    {
                        if (field.Id > result)
                            result = field.Id;
                    }
                }

                return result + 1;
            }

            /// <summary>
            /// Gets the highest table id in a dataview
            /// </summary>
            /// <param name="dataView">The dataview</param>
            /// <returns>The highest table id</returns>
            private int GetHighestTableID(COEDataView dataView)
            {
                int result = 0;
                foreach (COEDataView.DataViewTable table in dataView.Tables)
                {
                    if (table.Id > result)
                        result = table.Id;
                }

                return result + 1;
            }

            /// <summary>
            /// Gets an empty search criteria
            /// </summary>
            /// <returns>An empty search criteria</returns>
            private SearchCriteria GetEmptySearchCriteria()
            {
                _coeLog.LogStart("GetEmptySearchCriteria", 1, System.Diagnostics.SourceLevels.All);
                SearchCriteria searchCriteria = new SearchCriteria();
                searchCriteria.XmlNS = "COE.SearchCriteria";
                _coeLog.LogEnd("GetEmptySearchCriteria", 1, System.Diagnostics.SourceLevels.All);
                return searchCriteria;
            }

            /// <summary>
            /// Search method that performs a search and returns a hitlistInfo object
            /// </summary>
            /// <param name="searchCriteria">object containing query criteria</param>
            /// <param name="dataView">object containing the data view or an id of it</param>
            /// <param name="connStringType">the connection string type to be used</param>
            /// <returns>HitListInfo is an object containing hitlist info</returns>
            public HitListInfo GetHitList(ref SearchCriteria searchCriteria, COEDataView dataView, ConnStringType connStringType)
            {
                return GetHitList(ref searchCriteria, dataView, false, connStringType, null);
            }

            /// <summary>
            /// Search method that performs a search and returns a hitlistInfo object
            /// </summary>
            /// <param name="searchCriteria">object containing query criteria</param>
            /// <param name="dataView">object containing the data view or an id of it</param>
            /// <param name="connStringType">the connection string type to be used</param>
            /// <param name="refineHitlist">The hitlist to refine over</param>
            /// <returns>HitListInfo is an object containing hitlist info</returns>
            public HitListInfo GetHitList(ref SearchCriteria searchCriteria, COEDataView dataView, ConnStringType connStringType, HitListInfo refineHitlist)
            {
                return GetHitList(ref searchCriteria, dataView, false, connStringType, refineHitlist);
            }

            /// <summary>
            /// Search method that performs a search and returns a hitlistInfo object partially created, and then continues updating it in a
            /// background process.
            /// </summary>
            /// <param name="searchCriteria">object containing query criteria</param>
            /// <param name="dataView">object containing the data view or an id of it</param>
            /// <param name="connStringType">the connection string type to be used</param>
            /// <param name="refineHitlist">The hitlist to refine over</param>
            /// <returns>HitListInfo is an object containing hitlist info</returns>
            public HitListInfo GetPartialHitList(ref SearchCriteria searchCriteria, COEDataView dataView, ConnStringType connStringType, HitListInfo refineHitlist)
            {
                return GetHitList(ref searchCriteria, dataView, true, connStringType, refineHitlist);
            }

            /// <summary>
            /// Search method that performs a search and returns a hitlistInfo object partially created, and then continues updating it in a
            /// background process.
            /// </summary>
            /// <param name="searchCriteria">object containing query criteria</param>
            /// <param name="dataView">object containing the data view or an id of it</param>
            /// <param name="connStringType">the connection string type to be used</param>
            /// <param name="commitSize">the commit size to be used</param>
            /// <returns>HitListInfo is an object containing hitlist info</returns>
            public HitListInfo GetPartialHitList(ref SearchCriteria searchCriteria, COEDataView dataView, ConnStringType connStringType, int commitSize, HitListInfo refineHitlist)
            {
                return GetHitList(ref searchCriteria, dataView, true, connStringType, commitSize, refineHitlist);
            }

            /// <summary>
            /// Search method that performs a search and returns a hitlistInfo object
            /// </summary>
            /// <param name="searchCriteria">object containing query criteria</param>
            /// <param name="dataView">object containing the data view or an id of it</param>
            /// <param name="partialHitList">determines whether inserting partial hitlist or not</param>
            /// <param name="connStringType">the connection string type to be used</param>
            /// <param name="refineHitlist">The hitlist to refine over</param>
            /// <returns>HitListInfo is an object containing hitlist info</returns>
            public HitListInfo GetHitList(ref SearchCriteria searchCriteria, COEDataView dataView, bool partialHitList, ConnStringType connStringType, HitListInfo refineHitlist)
            {
                if (ServiceConfigData == null)
                {
                    try
                    {
                        ServiceConfigData = ConfigurationUtilities.GetServiceData(this.ServiceName);
                    }
                    catch (Exception)
                    {
                        ServiceConfigData = new ServiceData();
                    }
                }
                return GetHitList(ref searchCriteria, dataView, partialHitList, connStringType, this.ServiceConfigData.PartialHitlistCommitSize, refineHitlist);

            }
            /// <summary>
            /// Search method that performs a search and returns a hitlistInfo object
            /// </summary>
            /// <param name="searchCriteria">object containing query criteria</param>
            /// <param name="dataView">object containing the data view or an id of it</param>
            /// <param name="partialHitList">determines whether inserting partial hitlist or not</param>
            /// <param name="commitSize">if using partial hits, the commit size to be used</param>
            /// <param name="connStringType">the connection string type to be used</param>
            /// <param name="refineHitlist">The hitlist to refine over</param>
            /// <returns>HitListInfo is an object containing hitlist info</returns>
            private HitListInfo GetHitList(ref SearchCriteria searchCriteria, COEDataView dataView, bool partialHitList, ConnStringType connStringType, int commitSize, HitListInfo refineHitlist)
            {
                string methodSignature = MethodBase.GetCurrentMethod().DeclaringType.Name + "->" + MethodBase.GetCurrentMethod().Name.ToUpper() + "  ";
                dataView = this.ApplyDataviewFilter(dataView, searchCriteria);
                this.CheckPKIsPresent(dataView);
                //set application name since for searching this is variable.
                _coeLog.LogStart(methodSignature, 1, System.Diagnostics.SourceLevels.All);

                if (string.IsNullOrEmpty(dataView.Database))
                    throw new Exception("Missing DataView Attribute 'DATABASE'");

                _databaseName = dataView.Database;
                //end set application name

                ResultsCriteria resultsCriteria = new ResultsCriteria();

                _coeLog.LogStart(methodSignature + "Loading DAL");
                if (searchDAL == null)
                {
                    LoadDAL(connStringType);

                    // Coverity Fix CID - 11578 
                    if (searchDAL == null)
                        throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));
                }
                _coeLog.LogEnd(methodSignature + "Loading DAL");

                _coeLog.LogStart(methodSignature + "Creating COEHITLISTBO");

                //create hitlist object for creating new hitlist 
                COEHitListBO hitList = COEHitListBO.New(_databaseName, HitListType.TEMP, 0, dataView.DataViewID);
                hitListIDTableName = hitList.TempHitListIDTableName;
                hitListTableName = hitList.TempHitListTableName;
                //hitListSchemaName = hitList.TempHitListSchemaName;
                //get the hitlistID
                hitList = hitList.Update();

                _coeLog.LogEnd(methodSignature + "Creating COEHITLISTBO");

                //get coedataview
                if (dataView.Basetable == -1)
                    //ljb add new database property
                    dataView = COEDataViewBO.Get(dataView.DataViewID).COEDataView;

                resultsCriteria.XmlNS = "COE.ResultsCriteria";
                ResultsCriteria.ResultsCriteriaTable coeHitList = new ResultsCriteria.ResultsCriteriaTable();
                coeHitList.Id = dataView.Basetable;
                coeHitList.Criterias = new List<ResultsCriteria.IResultsCriteriaBase>();
                /*ResultsCriteria.Literal literal = new ResultsCriteria.Literal();
                literal.LiteralValue = hitList.ID.ToString();
                */
                ResultsCriteria.Field field = new ResultsCriteria.Field();
                field.Id = int.Parse(dataView.BaseTablePrimaryKey);

                //coeHitList.Criterias.Add(literal);
                coeHitList.Criterias.Add(field);
                resultsCriteria.Add(coeHitList);

                if (MaxRecordCount > 0)
                {
                    SearchCriteria.VerbatimCriteria maxRecordsSearchCriteria = new SearchCriteria.VerbatimCriteria();
                    maxRecordsSearchCriteria.Verbatim = "ROWNUM <= ?";
                    maxRecordsSearchCriteria.Parameter.Add(MaxRecordCount.ToString());

                    SearchCriteria.SearchCriteriaItem searchCriteriaItem = new SearchCriteria.SearchCriteriaItem();
                    searchCriteriaItem.Criterium = maxRecordsSearchCriteria;
                    searchCriteriaItem.ID = -1;

                    searchCriteria.Items.Add(searchCriteriaItem);
                }

                if (refineHitlist != null)
                {
                    string hitlistTableName = string.Empty;

                    if (refineHitlist.HitListType == HitListType.TEMP)
                        HitListUtilities.BuildTempHitListTableName(Resources.CentralizedStorageDB, ref hitlistTableName);
                    else
                        HitListUtilities.BuildSavedHitListTableName(Resources.CentralizedStorageDB, ref hitlistTableName);

                    dataView = GetDataViewWithHitlist(dataView, hitlistTableName);

                    SearchCriteria.NumericalCriteria hitlistSearchCriteria = new SearchCriteria.NumericalCriteria();
                    hitlistSearchCriteria.Operator = SearchCriteria.COEOperators.EQUAL;
                    hitlistSearchCriteria.Value = refineHitlist.HitListID.ToString();
                    SearchCriteria.SearchCriteriaItem searchCriteriaItem = new SearchCriteria.SearchCriteriaItem();
                    searchCriteriaItem.TableId = dataView.Tables[dataView.Tables.Count - 1].Id;
                    searchCriteriaItem.FieldId = dataView.Tables[dataView.Tables.Count - 1].Fields[1].Id;
                    searchCriteriaItem.Criterium = hitlistSearchCriteria;
                    searchCriteriaItem.ID = -2;

                    searchCriteria.Items.Add(searchCriteriaItem);
                }

                try
                {
                    _coeLog.LogStart(methodSignature + "Begin Transaction");
                    searchDAL.DALManager.BeginTransaction();
                    _coeLog.LogEnd(methodSignature + "Begin Transaction");

                    _coeLog.LogStart(methodSignature + "Processing hitlist query - template pattern");
                    List<SearchProcessor> processors = GetSearchProcessorList(searchCriteria.Items, dataView);

                    try
                    {
                        //we may need to add another parameter her to specifiy that there are ore then one searchcriteria for more then one table this requires DISTINCT be added
                        queryBuilder = new QueryBuilder(dataView.ToString(), searchCriteria.ToString(), resultsCriteria.ToString());
                        //this is where the select query for the hitlist in build. The hitlist table is not yet been added
                        Query query = this.queryBuilder.BuildQuery(searchDAL.DALManager.DatabaseData.DBMSType)[0];
                        foreach (SearchProcessor processor in processors)
                        {
                            //Jordans suggest to figure out a mechanism to not call a processor that does not do anything on Pre/Post/Process
                            string type = processor.GetType().Name;
                            _coeLog.Log(type + " Process", 1, System.Diagnostics.SourceLevels.All);
                            processor.Process(query);
                            _coeLog.Log(type + " Process", 1, System.Diagnostics.SourceLevels.All);
                        }
                        if (partialHitList)
                        {
                            SelectClauseLiteral literal = new SelectClauseLiteral(":0");
                            literal.Parameters.Add(new Value(hitList.ID.ToString(), DbType.Int32));
                            query.AddSelectItem(literal, 0);

                            query.EncloseInParenthesis = false;
                            hitList.NumHits = InsertHitListIncrementally(query, hitList.ID, commitSize);
                            query.EncloseInParenthesis = true;
                        }
                        else
                        {
                            SelectClauseLiteral literal = new SelectClauseLiteral("distinct :0");
                            literal.Parameters.Add(new Value(hitList.ID.ToString(), DbType.Int32));
                            query.AddSelectItem(literal, 0);

                            hitList.NumHits = InsertHitList(query);

                            // need to update the hitlist with the record count. Call Save since this will update when hitList has been added to after last save.
                            // When partial hits are being used, the stored procedure is in charge of updating the recordcount.
                            hitList = hitList.Update();
                        }
                    }
                    catch (SQLGeneratorException sqlException)
                    {
                        _coeLog.Log(methodSignature + sqlException.Message, 1, System.Diagnostics.SourceLevels.Error);
                        throw sqlException;
                    }
                    catch (MolServerException molServerException)
                    {
                        _coeLog.Log(methodSignature + molServerException.Message, 1, System.Diagnostics.SourceLevels.Error);
                        throw molServerException;
                    }
                    catch (UnsupportedDataTypeException unsupportedTypeException)
                    {
                        _coeLog.Log(methodSignature + unsupportedTypeException.Message, 1, System.Diagnostics.SourceLevels.Error);
                        throw unsupportedTypeException;
                    }
                    catch (Oracle.DataAccess.Client.OracleException oracleException)
                    {
                        _coeLog.Log(methodSignature + oracleException.Message, 1, System.Diagnostics.SourceLevels.Error);

                        //ORA-06502: PL/SQL: numeric or value error: NULL index table key value
                        if (oracleException.Message.Contains("ORA-06502"))
                            throw new Exception(string.Format("Can not insert null ID into hitlist table. Possible tables JOIN problem - review dataview relations. {0}", oracleException.Message));

                        throw new Exception(string.Format("Unhandled oracle exception: {0}", oracleException.Message));
                    }
                    catch (Exception exception)
                    {
                        _coeLog.Log(methodSignature + exception.Message, 1, System.Diagnostics.SourceLevels.Error);
                        throw exception;
                    }
                    finally
                    {
                        _coeLog.LogStart(methodSignature + "PostProcess");
                        for (int i = 0; i < processors.Count; i++)
                        {
                            processors[i].PostProcess(searchDAL);
                        }
                        _coeLog.LogEnd(methodSignature + "PostProcess");
                    }
                    _coeLog.LogStart(methodSignature + "Commit Transaction");
                    if (commitTransaction)
                        searchDAL.DALManager.CommitTransaction();
                    _coeLog.LogEnd(methodSignature + "Commit Transaction");

                }
                catch (Exception exception)
                {
                    _coeLog.Log(methodSignature + exception.Message, 1, System.Diagnostics.SourceLevels.Error);
                    searchDAL.DALManager.RollbackTransaction();
                    string errorMessage = string.Empty;
                    Type exceptionType = exception.GetType();
                    switch (exceptionType.Name)
                    {
                        case "SQLGeneratorException":
                            errorMessage = Resources.SQLGeneratorErrorMessage;
                            break;
                        case "MolServerException":
                            errorMessage = Resources.MolServerErrorMessage;
                            break;
                        case "UnsupportedDataTypeException":
                            errorMessage = Resources.UnsupportedDataType;
                            break;
                        case "OracleException":
                            errorMessage = Resources.DatabaseErrorMessage;
                            // Conver OracleException to common exception.
                            // Eliminate the depdencies to OracleDataAccess for client side
                            exception = new Exception(exception.Message);
                            break;
                        case "COENumericFormatConversionException":
                            errorMessage = exception.Message;
                            break;
                        default:
                            break;
                    }

                    //decide whether to throw the error and add a notification.
                    HandleError(exception);
                }
                finally
                {
                    _coeLog.LogEnd(methodSignature + "Processing hitlist query - template pattern");
                }

                _coeLog.LogStart(methodSignature + "SaveSearchCriteria");

                SaveSearchCriteria(ref searchCriteria, hitList, dataView.DataViewID);

                _coeLog.LogEnd(methodSignature + "SaveSearchCriteria");

                _coeLog.LogStart(methodSignature + "create hitlistInfo for returning ");
                //create hitlistInfo ojbect for returning information.  At some point we will likely use the hitlist object and return
                HitListInfo hitListInfo = new HitListInfo();
                if (!partialHitList)
                    hitListInfo.RecordCount = hitList.NumHits;

                hitListInfo.HitListID = hitList.ID;
                hitListInfo.HitListType = hitList.HitListType;
                hitListInfo.CurrentRecordCount = hitList.NumHits;

                _coeLog.LogEnd(methodSignature + "create hitlistInfo for returning");

                _coeLog.LogEnd(methodSignature, 1, System.Diagnostics.SourceLevels.All);

                return hitListInfo;
            }

            /// <summary>
            /// This methods updates a Hitlist that has retrieved using partial search.
            /// In the respose hitlist the CurrentRecordCount will be updated, and if the hitlist insertion process has finished you will get
            /// back a hitlist which RecordCount is equal to its CurrentRecordCount, otherwise the RecordCount will be cero.
            /// </summary>
            /// <param name="originalHitListInfo">The original hitlist, with its HitListID filled in.</param>
            /// <param name="database">The database. This usually comes from the Dataview attribute called Database.</param>
            /// <param name="connStringType">The conn string type.</param>
            /// <returns>A new HitList with CurrentRecordCount and RecordCount updated.</returns>
            public HitListInfo GetHitlistProgress(HitListInfo originalHitListInfo, string database, ConnStringType connStringType)
            {
                _coeLog.LogStart("GetHitlistProgress", 1, System.Diagnostics.SourceLevels.All);
                _databaseName = database;
                if (searchDAL == null) { LoadDAL(connStringType); }
                HitListInfo hitListInfo = originalHitListInfo;
                object[] appDomainInfo = (object[])AppDomain.CurrentDomain.GetData(hitListInfo.HitListID.ToString());
                if (appDomainInfo != null)
                    hitListInfo.CurrentRecordCount = (int)appDomainInfo[1];
                else
                {
                    // Coverity Fix CID - 11579 
                    if (searchDAL != null)
                        hitListInfo.CurrentRecordCount = hitListInfo.RecordCount = searchDAL.GetHitListRecordCount(hitListInfo.HitListID);
                    else
                        throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));
                }

                _coeLog.LogEnd("GetHitlistProgress", 1, System.Diagnostics.SourceLevels.All);

                return hitListInfo;
            }

            #endregion

            #region Private Methods
            /// <summary>
            /// Inserts into hitlist using a query.
            /// </summary>
            /// <param name="query">The query to be inserted.</param>
            /// <returns>The number of Hits for the query.</returns>
            private int InsertHitList(Query query)
            {
                try
                {
                    _coeLog.LogStart("InsertHitList", 1, System.Diagnostics.SourceLevels.All);
                    Query wrapperQuery = new Query();
                    query.Alias = "hitlist";
                    wrapperQuery.SetMainTable(query);
                    SelectClauseLiteral wildcard = new SelectClauseLiteral();
                    wildcard.Literal = "hitlist.*";
                    wrapperQuery.AddSelectItem(wildcard);

                    if (wrapperQuery.DataBaseType == DBMSType.ORACLE)
                    {
                        SelectClauseLiteral rowNum = new SelectClauseLiteral();
                        rowNum.Literal = "rownum";
                        wrapperQuery.AddSelectItem(rowNum);
                    }

                    Insert insert = new Insert();
                    insert.SelectStatement = wrapperQuery;
                    insert.Fields = new List<Field>();
                    Field hitListIDField = new Field("HITLISTID", DbType.Int16);
                    Field idField = new Field("ID", DbType.Int16);
                    insert.Fields.Add(hitListIDField);
                    insert.Fields.Add(idField);
                    if (query.DataBaseType == DBMSType.ORACLE)
                    {
                        Field sortOrderField = new Field("SORTORDER", DbType.Int32);
                        insert.Fields.Add(sortOrderField);
                    }
                    insert.MainTable = new Table(hitListTableName);
                    insert.DataBaseType = query.DataBaseType;

#if DEBUG
                    insert.UseParameters = false;
                    System.Diagnostics.Debug.WriteLine("HitList Insertion Query (no Threading):");
                    System.Diagnostics.Debug.WriteLine(insert.ToString());
                    System.Diagnostics.Debug.WriteLine("----------------------");
                    insert.UseParameters = true;
#endif
                    return searchDAL.ExecuteNonQuery(insert);
                }
                finally
                {
                    _coeLog.LogEnd("InsertHitList", 1, System.Diagnostics.SourceLevels.All);
                }
            }


            /// <summary>
            /// Inserts into hitlist using a query.
            /// </summary>
            /// <param name="query">The query to be inserted.</param>
            /// <returns>The number of Hits for the query.</returns>
            private int InsertHitListIncrementally(Query query, int hitListID, int commitSize)
            {
                string methodSignature = MethodBase.GetCurrentMethod().DeclaringType.Name + "->" + MethodBase.GetCurrentMethod().Name.ToUpper() + "  ";
                try
                {
                    _coeLog.LogStart(methodSignature, 1, System.Diagnostics.SourceLevels.All);
#if DEBUG
                    System.Diagnostics.Debug.WriteLine("HitList Cursor Query (using Threading):");
                    query.UseParameters = false;
                    System.Diagnostics.Debug.WriteLine(query.ToString());
                    System.Diagnostics.Debug.WriteLine("----------------------");
                    query.UseParameters = true;
#endif
                    KeepAliveHolder holder = searchDAL.CreateKeepAliveHolder(query, false, hitListID.ToString());

                    query.EncloseInParenthesis = false;

                    int firstCommitSize = commitSize;

                    if (AppConfigData == null)
                    {
                        try
                        {
                            AppConfigData = ConfigurationUtilities.GetApplicationData(COEAppName.Get().ToString());
                            if (AppConfigData.PartialHitlistFirstCommitSize > 0)
                                firstCommitSize = AppConfigData.PartialHitlistFirstCommitSize;
                        }
                        catch (Exception exception)
                        {
                            _coeLog.Log(exception.Message);
                        }
                    }

                    bool hasEnded;
                    int recordsAffected = searchDAL.InsertHitListIncrementally(holder, firstCommitSize, out hasEnded);

                    if (!hasEnded)
                    {
                        commitTransaction = false;
                        InsertHitListAsyncDelegate insertAsync = new InsertHitListAsyncDelegate(InsertHitListAsync);

                        AppDomain.CurrentDomain.SetData(holder.Key, new object[] { holder, recordsAffected });
                        IAsyncResult insertAsyncResult = insertAsync.BeginInvoke(holder,
                                               commitSize,
                                               new AsyncCallback(InsertHitListAsyncComplete),
                                               null);
                    }

                    query.EncloseInParenthesis = true;

                    return recordsAffected;
                }
                finally
                {
                    _coeLog.LogEnd(methodSignature);
                }
            }

            /// <summary>
            /// When partial hits are to be used, the first trunk is inserted syncrhonously using InsertHitListIncrementally, which then calls this
            /// method for the following insertions, in an asynchronous mode.
            /// </summary>
            /// <param name="holder">The keepalive holder</param>
            /// <param name="commitSize">The commit size</param>
            /// <returns>The keepalive unique key</returns>
            private int InsertHitListAsync(KeepAliveHolder holder, int commitSize)
            {
                string methodSignature = MethodBase.GetCurrentMethod().DeclaringType.Name + "->" + MethodBase.GetCurrentMethod().Name.ToUpper() + "  ";
                try
                {
                    _coeLog.LogStart(methodSignature, 1, System.Diagnostics.SourceLevels.All);
                    int recordsAffected = 1;
                    int lastRecAffected = 0;
                    bool hasEnded;
                    do
                    {
                        _coeLog.LogStart(methodSignature + "Inserting " + commitSize + " hits", 1, System.Diagnostics.SourceLevels.All);
                        lastRecAffected = recordsAffected;
                        recordsAffected = searchDAL.InsertHitListIncrementally(holder, commitSize, out hasEnded);
                        AppDomain.CurrentDomain.SetData(holder.Key, new object[] { holder, recordsAffected });

                        _coeLog.LogEnd(string.Format("{0} - Inserting {1} hits. Current Total {2} - {3}", methodSignature, commitSize, recordsAffected, (hasEnded ? "Finished!" : "More to come...")), 1, System.Diagnostics.SourceLevels.All);
                    } while (!hasEnded);
                    commitTransaction = true;
                    searchDAL.DALManager.CommitTransaction();
                    return int.Parse(holder.Key);
                }
                finally
                {
                    _coeLog.LogEnd("InsertHitListAsync", 1, System.Diagnostics.SourceLevels.All);
                }
            }

            /// <summary>
            /// This method is called when all the asynchronous calls to InsertHitListAsync are finished. Here resrouces and dictionaries are cleaned
            /// </summary>
            /// <param name="insertAsyncResult">The asyncResult, which should be of type: InsertHitListAsyncDelegate</param>
            private void InsertHitListAsyncComplete(IAsyncResult insertAsyncResult)
            {
                string methodSignature = MethodBase.GetCurrentMethod().DeclaringType.Name + "->" + MethodBase.GetCurrentMethod().Name.ToUpper() + "  ";

                _coeLog.LogStart(methodSignature, 1, System.Diagnostics.SourceLevels.All);

                // Retrieve the delegate that corresponds to the asynchronous method 
                InsertHitListAsyncDelegate ad = (InsertHitListAsyncDelegate)((System.Runtime.Remoting.Messaging.AsyncResult)insertAsyncResult).AsyncDelegate;

                // Get the result of the asynchronous method
                int result = ad.EndInvoke(insertAsyncResult);

                AppDomain.CurrentDomain.SetData(result.ToString(), null);
                _coeLog.LogEnd(methodSignature, 1, System.Diagnostics.SourceLevels.All);
            }

            /// <summary>
            /// Stores a recentily used search criteria, using the SearchCriteriaBO.
            /// </summary>
            /// <param name="searchCriteria">Search Criteria messaging type.</param>
            /// <param name="numHits">Number of hits for this search.</param>
            private void SaveSearchCriteria(ref SearchCriteria searchCriteria, COEHitListBO hitlistBO, int dataViewID)
            {
                if (this.IsSaveQueryHistoryEnabled())
                {
                    _coeLog.LogStart("SaveSearchCriteria", 1, System.Diagnostics.SourceLevels.All);
                    COESearchCriteriaBO coeSearchCriteriaBO = COESearchCriteriaBO.New(_databaseName);

                    coeSearchCriteriaBO.NumberOfHits = hitlistBO.NumHits;
                    coeSearchCriteriaBO.SearchCriteria = searchCriteria;
                    coeSearchCriteriaBO.UserName = COEUser.Name;
                    coeSearchCriteriaBO.DataViewId = dataViewID;

                    coeSearchCriteriaBO = coeSearchCriteriaBO.Update();
                    searchCriteria.SearchCriteriaID = coeSearchCriteriaBO.ID;

                    hitlistBO.SearchCriteriaID = coeSearchCriteriaBO.ID;
                    hitlistBO.SearchCriteriaType = coeSearchCriteriaBO.SearchCriteriaType;
                    hitlistBO.AssociateSearchCriteria();
                    _coeLog.LogEnd("SaveSearchCriteria", 1, System.Diagnostics.SourceLevels.All);
                }
                //These lines can be uncommented if you whish to allow caching of search criteria without storing it to the database.
                //else
                //{
                //    // It creates a key based on hitlistid  
                //    LocalCache.Add(hitlistBO.ID.ToString() + "_" + hitlistBO.HitListType.ToString(), typeof(SearchCriteria), searchCriteria, LocalCache.NoAbsoluteExpiration, TimeSpan.FromMinutes(5), COECacheItemPriority.Low);
                //}

            }

            /// <summary>
            /// Looks in configuration for savequery history config.
            /// </summary>
            /// <returns>True if save query history is enabled, false otherwise</returns>
            private bool IsSaveQueryHistoryEnabled()
            {
                if (AppConfigData == null)
                {
                    try
                    {
                        AppConfigData = ConfigurationUtilities.GetApplicationData(COEAppName.Get().ToString());
                    }
                    catch (Exception)
                    {
                    }
                }
                return (AppConfigData == null || AppConfigData.SaveQueryHistory == null || AppConfigData.SaveQueryHistory.ToLower() == "yes");
            }

            /// <summary>
            /// Outer query is required in at least this two scenarios: For Paging, For grouping when there's a Clob involved in the select clauses.
            /// Adds paging to the query when it is not a keep alive request.
            /// 
            /// Notice that this method is not called for a childtable outermost query, otherwise its child sorting would be lost.
            /// </summary>
            /// <param name="query">The query to add paging to.</param>
            /// <param name="pagingInfo">The object that has the paging info.</param>
            /// <param name="dbmsType">The underlying dbms.</param>
            /// <param name="dataview">The object that has table and relationship info</param>
            /// <returns>A query that has only a page as described in paging info.</returns>
        private Query SurroundWithOuterQuery(Query query, DBMSType dbmsType, COEDataView dataview, SearchCriteria searchCriteria)
        {
            string methodSignature = MethodBase.GetCurrentMethod().DeclaringType.Name + "->" + MethodBase.GetCurrentMethod().Name.ToUpper() + "  ";
            _coeLog.LogStart(methodSignature, 1, System.Diagnostics.SourceLevels.All);
            Query innerQuery = query; // inner query is initialized to the original query
            Query outerQuery = new Query();
            SelectClauseField primaryKeySelectClause = null;
            try
            {
                bool addGroupBy = false;
                SelectClauseItem[] items = new SelectClauseItem[innerQuery.GetSelectClauseItems().Count];
                innerQuery.GetSelectClauseItems().CopyTo(items);
                // Move all but primarykey select clauses to the outerquery
                for (int i = 0; i < items.Length; i++)
                {
                    innerQuery.RemoveSelectItem(items[i]);

                    if (items[i] is SelectClauseField && ((SelectClauseField)items[i]).DataField != null && ((SelectClauseField)items[i]).DataField.FieldId == int.Parse(dataview.BaseTablePrimaryKey))
                    {
                        primaryKeySelectClause = (SelectClauseField)items[i];
                    }
                    else
                        outerQuery.AddSelectItem(items[i]);
                }

                innerQuery.SetGroupByClause(new GroupByClause()); // Clean inner query group by clause.

                // Override the main table of the innerquery to ensure the main table is the base table.
                foreach (COEDataView.DataViewTable tbl in dataview.Tables)
                {
                    if (tbl.Id == dataview.Basetable)
                    {
                        innerQuery.SetMainTable(new Table(tbl.Id, tbl.Name, tbl.Alias, tbl.Database));
                        break;
                    }
                }

                Field pkfield = new Field();
                if (primaryKeySelectClause == null) // There was no primary key on the original query, then we will need to create it.
                {
                    // Coverity Fix CID - 10496 (from local server)
                    COEDataView.DataViewTable theDataViewTable = dataview.Tables[dataview.BaseTableName];
                    if (theDataViewTable != null)
                    {
                        COEDataView.Field dvField = theDataViewTable.Fields.getById(int.Parse(dataview.BaseTablePrimaryKey));
                        if (dvField != null)
                        {
                            pkfield = new Field(dvField.Id, dvField.Name, CambridgeSoft.COE.Framework.Common.SqlGenerator.Utils.TypesConversor.GetType(dvField.DataType.ToString()));
                            COEDataView.DataViewTable tbl = dataview.Tables.getById(dataview.Basetable);
                            pkfield.Table = new Table(tbl.Id, tbl.Name, tbl.Alias, tbl.Database);
                        }
                    }
                }
                else
                {
                    //Coverity Bug Fix :- CID : 11580  Jira Id :CBOE-194 
                    if (primaryKeySelectClause != null && primaryKeySelectClause.DataField != null)
                    {
                        Field fld = primaryKeySelectClause.DataField as Field;
                        if (fld != null)
                            pkfield = fld;
                    }
                   // pkfield = (primaryKeySelectClause != null && primaryKeySelectClause.DataField != null) ? primaryKeySelectClause.DataField as Field : pkfield;
                }

                // Will join to the current page table (COEDB.COECURRENTPAGE), which is a global temporary table holding a page of results
                // in the form (BaseTablePrimaryKey, SortOrder)
                Table currentPageTable = new Table(Resources.COECurrentPageTable);
                currentPageTable.Database = Resources.CentralizedStorageDB;
                currentPageTable.Alias = "currentPage";
                Field currentPage_IDField = new Field(Resources.BaseTablePrimaryKeyField, DbType.Int32, currentPageTable);
                SelectClauseField idSelectClause = new SelectClauseField(currentPage_IDField);
                idSelectClause.Alias = "ID";
                innerQuery.AddSelectItem(idSelectClause);

                //JHS 6/20/2012
                //Inject a hint into the queries that utilize the current page.  We will default to a hint of 100 rows, but this can be
                //overriden in COEFrameworkconfig.xml.  We do this since the page size should always be significantly smaller than the number
                //of rows in the base table and really prefer to use the index when possible.
                SQLGeneratorData sqlGenerateData = ConfigurationUtilities.GetSQLGeneratorData(searchDAL.DALManager.DatabaseData);
                if (sqlGenerateData.CurrentPageHint == string.Empty)
                {
                    //if currentPageTable.Alias above is changed then this string will need tobe changed
                    //it seems not worthwhile to build the string using the alias property or creating a new variable.
                    innerQuery.Hints.Add("OPT_ESTIMATE(table, \"currentPage\", rows=100)");
                }
                else
                {
                    innerQuery.Hints.Add(sqlGenerateData.CurrentPageHint);
                }

                Relation currentPageRel = new Relation();
                currentPageRel.Parent = pkfield;
                currentPageRel.Child = currentPage_IDField;
                currentPageRel.InnerJoin = true;
                innerQuery.AddJoinRelation(currentPageRel);


                // If a search criteria is provided, we need to check if any of those criteria are aggregated searches, in order to know
                // if a group by clause is to be added, later
                if (searchCriteria != null && searchCriteria.Items.Count > 0)
                {
                    //if there is an aggregate search, the child table must be added to the table lists
                    foreach (SearchCriteria.SearchExpression se in searchCriteria.Items)
                    {
                        SearchCriteria.SearchCriteriaItem sc = se as SearchCriteria.SearchCriteriaItem;
                        if (sc != null && !string.IsNullOrEmpty(sc.AggregateFunctionName))
                        {
                            List<COEDataView.Relationship> rels = FindRelations(dataview.Relationships, ((Table)pkfield.Table).TableId, sc.TableId);
                            // Ensuring the relationships between the field being searched and the base table are already there.
                            // addjoinrelation will not add them if they are already present.
                            if (rels.Count > 0)
                            {
                                Relation rel = new Relation();
                                rel.Parent = pkfield;
                                COEDataView.Field fld = dataview.GetFieldById(rels[0].ChildKey);
                                COEDataView.DataViewTable tbl = dataview.Tables.getById(rels[0].Child);
                                rel.Child = new Field(fld.Id, fld.Name, CambridgeSoft.COE.Framework.Common.SqlGenerator.Utils.TypesConversor.GetType(fld.DataType.ToString()));
                                rel.Child.Table = new Table(tbl.Id, tbl.Name, tbl.Alias, tbl.Database);
                                rel.LeftJoin = true;
                                rel.InnerJoin = false;
                                innerQuery.AddJoinRelation(rel);
                            }
                            addGroupBy = true;
                        }
                    }
                }


                switch (dbmsType)
                {
                    case DBMSType.ORACLE:
                            /*
                if it is not a browse:
                        
                SELECT   <BASETABLE.PK>, fld1, fld2, expr1, expr2,...  
                    FROM (
                    (SELECT "hitlist"."ID" ID,
                    ROW_NUMBER() over(order by <Orderbylist>) AS "SORTORDER"
                    FROM (BASETABLE "BASETABLEALIAS"),
                        COEDB.COETEMPHITLIST "hitlist"
                    WHERE <BASETABLE.PK> = "hitlist"."ID"
                    AND ("hitlist"."HITLISTID" = :hitlistID)) <InnerQueryAlias>), 
                    (BASETABLE "BASETABLEALIAS")
                WHERE
                    <InnerQueryAlias>.id = <BASETABLE.PK>
                AND     (<InnerQueryAlias>."SORTORDER" >= :pageStart) AND (<InnerQueryAlias>."SORTORDER" <= :PageEnd)
                ORDER BY <InnerQueryAlias>."SORTORDER";
                         
                else
                         
                SELECT   <BASETABLE.PK>, fld1, fld2, expr1, expr2,...  
                    FROM (
                    (SELECT <BASETABLE.PK> pkForSorting,
                    ROW_NUMBER() over(order by <Orderbylist>) AS "SORTORDER"
                    FROM (BASETABLE "BASETABLEALIAS"),
                        COEDB.COETEMPHITLIST "hitlist"
                    WHERE <BASETABLE.PK> = "hitlist"."ID"
                    AND ("hitlist"."HITLISTID" = :hitlistID)) <InnerQueryAlias>), 
                    (BASETABLE "BASETABLEALIAS")
                WHERE
                    <InnerQueryAlias>.pkForSorting = <BASETABLE.PK>
                AND     (<InnerQueryAlias>."SORTORDER" >= :pageStart) AND (<InnerQueryAlias>."SORTORDER" <= :PageEnd)
                ORDER BY <InnerQueryAlias>."SORTORDER";
                */

                // Sort order is already specified in COEDB.COECURRENTPAGE, using the column SortOrder. Let's add an order by clause
                // to the inner query to honor it.
                SelectClauseField sortOrder = new SelectClauseField(new Field(Resources.SortOrderField, DbType.Double, currentPageTable));
                sortOrder.Alias = Resources.SortOrderField;
                innerQuery.AddSelectItem(sortOrder);
                innerQuery.SetOrderByClause(new OrderByClause());

                if (innerQuery.Alias == null || innerQuery.Alias.Trim() == string.Empty)
                {
                    innerQuery.Alias = "InnerQueryAlias";
                }

                if (addGroupBy)
                {
                    //There where aggregated searches, and as we cleaned up the group by clause at the beginning, we need to recreate it.
                    GroupByClause groupByClause = new GroupByClause();
                    List<SelectClauseItem> sItems = innerQuery.GetSelectClauseItems();
                    for (int i = 0; i < sItems.Count; i++)
                    {
                        if (!(sItems[i] is SelectClauseAggregateFunction))
                        {
                            //A field that is not aggregated, then need a group by!
                            groupByClause.AddItem(new GroupByClauseItem(new SelectClauseField(sItems[i].DataField)));
                        }
                    }
                    innerQuery.SetGroupByClause(groupByClause);
                }

                // The outer query has the inner as main table.
                outerQuery.SetMainTable(innerQuery);
                // And also the primary key
                outerQuery.AddSelectItem(primaryKeySelectClause);

                // Both joined through the pk field
                Relation joinRelation = new Relation();
                joinRelation.Child = pkfield;
                joinRelation.Parent = new Field(Resources.ID, DbType.Int32, innerQuery);

                outerQuery.AddJoinRelation(joinRelation);

                Field sortOrderField = new Field(Resources.SortOrderField, DbType.Int32, innerQuery);
                // Outer query is also ordered by innerquery's SORTORDER (which in turns is the COECURRENTPAGE.SORTORDER
                outerQuery.AddOrderByItem(new OrderByClauseItem(new SelectClauseField(sortOrderField)));

                // Not to enclose the whole outer query in parenthesis as doing so disallows its execution.
                outerQuery.EncloseInParenthesis = false;
                        return outerQuery;
                    default:
                        /*
                        SELECT 
                            COMPOUND_ID 
                        FROM(
                        SELECT TOP STARTROW 
                                r.COMPOUND_ID
                        FROM (SELECT TOP ENDROW 
                                    IC.SUBSTANCE_NAME
                        IC.COMPOUND_ID,
                            FROM INV_COMPOUNDS IC INNER JOIN COEHITLIST CHL
                        ON CHL.ID = IC.COMPOUND_ID 
                        AND CHL.HITLISTID= @HiTLISTID
                        ORDER BY IC.COMPOUND_ID ASC, IC.SUBSTANCE_NAME DESC) r
                        ORDER BY r.COMPOUND_ID ASC DESC, r.SUBSTANCE_NAME DESC ASC)
                        ORDER BY IC.COMPOUND_ID ASC, IC.SUBSTANCE_NAME DESC
                        */
                        return null;
                }
            }
            finally
            {
                _coeLog.LogEnd(methodSignature, 1, System.Diagnostics.SourceLevels.All);
            }
        }

            /// <summary>
            /// At this point we have an outer query that may contain aggregated clauses, and an inner query with the current page, sorted.
            /// That also means the outer query, potentially may have a group by [structures], or any field. That is potentally dangerous because
            /// Fields that cannot be sorted, cannot be grouped by. That's the case of clobs.
            /// 
            /// So, we will be moving the aggregated clauses to the inner query, and in the outer query, refer to the inner item, then we know, that the
            /// inner clause do not select for clobs or non-sortable items, as it only holds the table primary key and a sort order.
            /// 
            /// And by doing so, we are allowed for selecting clobs (no aggregation in the outer query, no group by in the outer query).
            /// </summary>
            /// <param name="outerQuery">The outer query potentially holding aggregated clauses.</param>
            /// <param name="innerQuery">The inner query used for paging.</param>
            /// <param name="dvLookup">A dataview metadata</param>
            /// <param name="inChildTbl">True if the outer query is for filling in a child table, false if it is for the base table.</param>
            /// <returns></returns>
            private Query RefactorQueryForAggregateFunctions(Query outerQuery, Query innerQuery, Common.SqlGenerator.MetaData.DataView dvLookup, bool inChildTbl)
            {
                string methodSignature = MethodBase.GetCurrentMethod().DeclaringType.Name + "->" + MethodBase.GetCurrentMethod().Name.ToUpper() + "  ";
                _coeLog.LogStart(methodSignature, 1, System.Diagnostics.SourceLevels.All);
                List<SelectClauseItem> aggregateClauses = new List<SelectClauseItem>();
                List<WhereClauseItem> aggregateWhere = innerQuery.GetAggregatedWhereClauseItems();

                Dictionary<string, Query> childQueries = new Dictionary<string, Query>(); // TableAlias, InnerQuery

                try
                {
                    SelectClauseItem[] items = new SelectClauseItem[outerQuery.GetSelectClauseItems().Count];
                    outerQuery.GetSelectClauseItems().CopyTo(items);

                    //The inner query at this point holds the aggregation clauses, but also has the outer query.
                    //The outer query should instead refer to the inner query clauses that are aggregated.
                    //This loop is of building the list of queries and aggregated clauses present in the queries and also
                    //for changing the outer query select clause to refer to the inner one. See further comments for more details on this change.
                    for (int i = 0; i < items.Length; i++)
                    {
                        if (items[i] is SelectClauseAggregateFunction)
                        {
                            string key = items[i].DataField.Table.GetAlias();
                            //Build the dictionary that holds the list of child queries that have aggregate functions.
                            if (!childQueries.ContainsKey(key))
                            {
                                Query newInnerQuery = innerQuery.Clone() as Query;
                                if (childQueries.Count > 0)
                                    newInnerQuery.Alias += childQueries.Count;

                                //JHS 1/18/2011 - Force left outer join for the child table with an aggregate columns or rows with no child data disappear 
                                foreach (Relation rel in newInnerQuery.GetJoinClause().Relations)
                                {

                                    if (rel.Child.Table.GetAlias() == key)
                                    {
                                        rel.InnerJoin = false;
                                        rel.LeftJoin = true;
                                    }

                                }

                                //If in child table the alias would be: HITLISTQUERY, HITLISTQUERY1, ...
                                //In in parent table the alias would be: InnerQueryAlias, InnerQueryAlias1, ...
                                childQueries.Add(key, newInnerQuery);
                            }
                            //Remove original criteria, IE: COUNT("BATCH_IDENTIFIER"."ID") AS "Identifiers Count"
                            outerQuery.RemoveSelectItem(items[i]);

                            //Now add that item to the child query
                            childQueries[key].AddSelectItem(items[i]);

                            aggregateClauses.Add(items[i]);
                            Field dataField = new Field(items[i].Name, DbType.String, childQueries[key]);
                            SelectClauseField selectClauseFld = new SelectClauseField(dataField);

                            //Add a new criteria to refer to the innerquery select clause: HITLISTQUERY."Identifiers Count"
                            outerQuery.AddSelectItem(selectClauseFld);
                        }
                        else if (items[i] is SelectClauseSQLFunction)
                        {
                            //Do the same as in the previous block for aggregate clauses to which a function was applied, IE:
                            //Original Clause: ROUND("BATCHES"."FORMULA_WEIGHT", 2) AS "Weight"
                            //Clause to be used: ROUND(HITLISTQUERY."Weight", 2)
                            SelectClauseSQLFunction function = items[i] as SelectClauseSQLFunction;
                            for (int j = (function.Parameters.Count - 1); j >= 0; j--)
                            {
                                if (function.Parameters[j] is SelectClauseAggregateFunction)
                                {
                                    string key = function.Parameters[j].DataField.Table.GetAlias();
                                    if (!childQueries.ContainsKey(key))
                                    {
                                        Query newInnerQuery = innerQuery.Clone() as Query;
                                        if (childQueries.Count > 0)
                                            newInnerQuery.Alias += childQueries.Count;
                                        childQueries.Add(key, newInnerQuery);
                                    }

                                    childQueries[key].AddSelectItem(function.Parameters[j]);
                                    aggregateClauses.Add(function.Parameters[j]);
                                    Field dataField = new Field(function.Parameters[j].Name, DbType.String, childQueries[key]);
                                    SelectClauseField selectClauseFld = new SelectClauseField(dataField);
                                    //Remove original criteria, IE: ROUND("BATCHES"."FORMULA_WEIGHT", 2) AS "Weight"
                                    function.Parameters.Remove(function.Parameters[j]);
                                    //Add a new criteria to refer to the innerquery select clause: ROUND(HITLISTQUERY."Weight", 2)
                                    //As what we modify are parameters, the order matters.
                                    function.Parameters.Insert(j, selectClauseFld);
                                }
                            }
                        }
                    }

                    //JHS 5/24/2012
                    //We need to remove extraneous tables from the child/inner queries.  The inner query contins select clauses from all child tables orginally used in the aggregates
                    //that means as we add these inner queries we are adding them with extra join information for unrelated child tables
                    //we will now need to loop over each child query and remove any unneeded items.  
                    //If in parent table relationships should stay, first query in childqueries is the main table.

                    //we only need to do this if there is more than one child query because otherwise we don't end up with extraneous joins during the clone process above
                    if (childQueries.Count > 1)
                    {
                        //now we go through each child
                        foreach (Query q in childQueries.Values)
                        {
                            //We will build a list of tables involved in select items
                            //doing this once here will build a smaller, cheaper list to loop later
                            List<string> selectTableList = new List<string>();

                            foreach (SelectClauseItem qItem in q.GetSelectClauseItems())
                            {
                                string tableName = qItem.DataField.Table.GetAlias();

                                //if the name is not in the list then add it
                                if (!(selectTableList.BinarySearch(tableName) >= 0))
                                {
                                    selectTableList.Add(tableName);
                                }
                            }

                            //we need the main query
                            string queryMainTable = q.GetMainTable().GetAlias();

                            //you can't remove relations from a list that you are looping on so create an alternate list
                            List<Relation> listRelations = new List<Relation>(q.GetJoinClause().Relations);

                            //now for each relationship we need to look and see if they are appropriate
                            foreach (Relation childRel in listRelations)
                            {
                                string parentTable = childRel.Parent.Table.GetAlias();
                                string childTable = childRel.Child.Table.GetAlias();

                                //if the parent table is the query maintable then we need to make sure there is at least one select item from that table
                                //if not then we remove the relation
                                if ((parentTable == queryMainTable))
                                {
                                    //we know the table is joined to the main table so now we see if it is used in a select
                                    if (selectTableList.BinarySearch(childTable) < 0)
                                    {
                                        bool usedForJoin = false;
                                        //here things get interesting.  We have figured out that this table is not used for a select.  But it can be used as an intermediate
                                        //table for a grandchild.
                                        //we only remove the relation if it isn't used later in the list
                                        foreach (Relation childRelInner in listRelations)
                                        {

                                            if (childRelInner.Parent.ToString() == childTable.ToString())
                                            {
                                                usedForJoin = true;
                                            }

                                        }

                                        if (!usedForJoin)
                                        {
                                            q.RemoveJoinRelation(childRel);
                                        }
                                    }
                                }
                                else
                                {
                                    //we know the table is joined to the main table so now we see if it is used in a select
                                    if (selectTableList.BinarySearch(childTable) < 0)
                                    {
                                        q.RemoveJoinRelation(childRel);
                                    }
                                }

                            }
                        }
                    }

                    if (aggregateClauses.Count > 0 || aggregateWhere.Count > 0)
                    {
                        Query mainQuery = null;
                        if (inChildTbl)
                        {
                            //As we changed the clauses to refer to the inner query, no relationship to the original tables are needed.
                            //The only needed relationships are, to the table itself and to the innerquery.
                            //There is no support for grand childs so we dont need to look futher on what relation needs to stay and which shouldnt.
                            //So simple, clean them all with a new JoinClause.
                            outerQuery.SetJoinClause(new JoinClause());
                            outerQuery.SetMainTable(dvLookup.GetTable(outerQuery.MainTableID));
                            mainQuery = ((Query)innerQuery.GetMainTable());

                            //If in childTable, when ordering an order by is added, and if that orderby is based on an aggregated clause we need to do some changes
                            foreach (OrderByClauseItem orderItem in outerQuery.GetOrderByClause())
                            {
                                //Only change the clause for the aggregate functions being sorted
                                if (orderItem.Item is SelectClauseAggregateFunction)
                                {
                                    //The order by do not care about the type of the clause, it originally writes the full field name as in
                                    //"BATCH_IDENTIFIER"."ID"
                                    //The clause for the previous sample, in the innerquery, would be COUNT("BATCH_IDENTIFIER"."ID") AS "Identifiers Count"
                                    foreach (SelectClauseAggregateFunction aggClause in aggregateClauses)
                                    {
                                        //But if an aggregated field is sorted, what we actually want is to use the innerquery clause, as in:
                                        //HITLISTQUERY."Identifiers Count"
                                        if (aggClause == orderItem.Item)
                                        {
                                            SelectClauseField fieldClause = new SelectClauseField();
                                            fieldClause.DataField = new Field(aggClause.Alias, DbType.String, childQueries[orderItem.Item.DataField.Table.GetAlias()]);
                                            orderItem.Item = fieldClause;
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            //If in parent table relationships should stay, first query in childqueries is the main table.
                            foreach (Query q in childQueries.Values)
                            {
                                outerQuery.SetMainTable(q);
                                mainQuery = q;
                                break;
                            }
                        }

                        // We need to add relationship from the child table being aggregated to the main table being queried.
                        for (int i = 0; i < aggregateClauses.Count; i++)
                        {
                            if (aggregateClauses[i].DataField.Table is Table)
                            {
                                int mainTableId = mainQuery.MainTableID;
                                //Fetch the rels that joins the child aggregated table to the main table queried.
                                List<Relation> rels = dvLookup.GetRelations(mainTableId, ((Table)aggregateClauses[i].DataField.Table).TableId);
                                //get the key to be used in the dictionary to know to what query the rels are to be added.
                                string key = aggregateClauses[i].DataField.Table.GetAlias();
                                foreach (Relation rel in rels)
                                {
                                    if (inChildTbl)
                                    {
                                        //If in child table a join in the outer query is to be added between the innerquery and the outer
                                        if (dvLookup.GetParentTableId(rel.Parent.FieldId.ToString()) != mainTableId)
                                        {
                                            SelectClauseField childFK = new SelectClauseField(rel.Parent);
                                            childFK.Alias = "FKAggFor" + rel.Parent.FieldId;
                                            //Add the field to use in the join as a select clause, IE: "BATCHES"."BATCH_INTERNAL_ID" AS "FKAggFor19"
                                            childQueries[key].AddSelectItem(childFK);

                                            Relation newRel = new Relation();
                                            newRel.Parent = rel.Parent;
                                            newRel.Child = new Field(childFK.Alias, rel.Parent.FieldType, childQueries[key]);
                                            //Add the relationship, IE: "BATCHES"."BATCH_INTERNAL_ID" = HITLISTQUERY."FKAggFor19"
                                            outerQuery.AddJoinRelation(newRel);
                                        }
                                    }
                                    rel.InnerJoin = false;
                                    rel.LeftJoin = true;
                                    //Add the relation from the parent and child. We needed this relation to be outer also. That's another reason
                                    //Why we cleaned up rels before.
                                    childQueries[key].AddJoinRelation(rel);
                                }
                            }
                        }

                        #region AddGroupBy
                        int j = 0;
                        //Time to build the proper group by clause
                        foreach (Query q in childQueries.Values)
                        {
                            GroupByClause innerQueryGroupBy = new GroupByClause();
                            SelectClauseItem[] innerItems = new SelectClauseItem[q.GetSelectClauseItems().Count];
                            q.GetSelectClauseItems().CopyTo(innerItems);
                            for (int i = 0; i < innerItems.Length; i++)
                            {
                                //All fields that are not aggregated needs to be added to the groupby clause
                                if (!(innerItems[i] is SelectClauseAggregateFunction))
                                {
                                    //But if it is a selectclauseanalytical, that is our sorting mechanism, then we need to look if any of the
                                    //fields used for sorting is not an aggregate function to also add it to the group by
                                    if (innerItems[i] is SelectClauseAnalyticalRowNum)
                                    {
                                        foreach (OrderByClauseItem item in ((SelectClauseAnalyticalRowNum)innerItems[i]).Clauses)
                                        {
                                            if (!(item.Item is SelectClauseAggregateFunction))
                                            {
                                                //Found sorting though a field that is not aggregated, then need a group by!
                                                innerQueryGroupBy.AddItem(new GroupByClauseItem(item.Item));
                                            }
                                        }
                                    }
                                    else
                                    {
                                        //A field that is not aggregated, then need a group by!
                                        innerQueryGroupBy.AddItem(new GroupByClauseItem(new SelectClauseField(innerItems[i].DataField)));
                                    }
                                }
                            }

                            //Child queries adds a Order BY MainQueryAlias.SortOrder, then that needs to be added to the group by.
                            //Base table query on the other hand does not (uses the analyticalrownum function)
                            if (inChildTbl)
                            {
                                List<Relation> rels = dvLookup.GetRelations(((Table)outerQuery.GetMainTable()).TableId, dvLookup.GetBaseTableId());
                                if (rels.Count > 0)
                                {
                                    rels[0].Child = new Field("ID", DbType.Int32, q);
                                    outerQuery.AddJoinRelation(rels[0]);
                                }

                                if (innerQueryGroupBy.Items.Count > 0)
                                {
                                    Field fld = new Field("SORTORDER", DbType.String, mainQuery);
                                    innerQueryGroupBy.AddItem(new GroupByClauseItem(new SelectClauseField(fld)));
                                    outerQuery.SetGroupByClause(new GroupByClause());
                                }
                            }

                            q.SetGroupByClause(innerQueryGroupBy);

                            if (!inChildTbl && j > 0)
                            {
                                //If in parent child table, there are 3 nested query, we were working with outer and inner, but there another that is used
                                //for getting the same page as the parent query. 
                                //So we need to put that relationship back in (remember we cleaned up the relationship to force outer joins and to have
                                //only the relationships we need to have.
                                string fieldName = "ID";

                                Relation re = new Relation();
                                re.Parent = new Field(fieldName, DbType.Int32, mainQuery);
                                re.Child = new Field(fieldName, DbType.Int32, q);
                                re.InnerJoin = true;
                                //Add the relationship to the 
                                outerQuery.AddJoinRelation(re);
                            }
                            j++;
                        }
                        #endregion
                    }

                    return outerQuery;
                }
                finally
                {
                    _coeLog.LogEnd(methodSignature, 1, System.Diagnostics.SourceLevels.All);
                }
            }

            /// <summary>
            /// Build Parent Table ResultsCriteria
            /// </summary>
            /// <param name="dataView"></param>
            /// <param name="parentQuery"></param>
            /// <returns></returns>
            private ResultsCriteria BuildParentTableResultsCriteria(COEDataView dataView, Query parentQuery, bool avoidPaging)
            {
                _coeLog.LogStart("BuildParentTableResultsCriteria", 1, System.Diagnostics.SourceLevels.All);
                ResultsCriteria resultsCriteriaBaseTable = new ResultsCriteria();
                ResultsCriteria.ResultsCriteriaTable baseTable = new ResultsCriteria.ResultsCriteriaTable();
                baseTable.Id = dataView.Basetable;
                ResultsCriteria.Field baseTablePrimaryKey = new ResultsCriteria.Field();
                baseTablePrimaryKey.Id = int.Parse(dataView.BaseTablePrimaryKey);
                baseTablePrimaryKey.Alias = "ID";
                //Need to order by the PK when no ordering was requested, for paging purposes
                if (parentQuery.GetOrderByClause().Count == 0 && !avoidPaging)
                {
                    baseTablePrimaryKey.OrderById = 1;
                    baseTablePrimaryKey.Direction = ResultsCriteria.SortDirection.ASC;
                }

                baseTable.Criterias.Add(baseTablePrimaryKey);
                resultsCriteriaBaseTable.Tables.Add(baseTable);

                _coeLog.LogEnd("BuildParentTableResultsCriteria", 1, System.Diagnostics.SourceLevels.All);
                return resultsCriteriaBaseTable;
            }

            /// <summary>
            /// Retrieve Relations
            /// </summary>
            /// <param name="resultsCriteria"></param>
            /// <param name="dataViewXML"></param>
            /// <returns></returns>
            private List<List<Relation>> RetrieveRelations(ResultsCriteria resultsCriteria, Common.SqlGenerator.MetaData.DataView metaDataDataView)
            {
                _coeLog.LogStart("RetrieveRelations (Through QuickGraph)", 1, System.Diagnostics.SourceLevels.All);

                ResultsCriteria.ResultsCriteriaTable parentTable = resultsCriteria[metaDataDataView.GetBaseTableId()];

                List<List<Relation>> relationships = new List<List<Relation>>();

                ResultsCriteria loopingResultsCriteria = new ResultsCriteria();
                loopingResultsCriteria = ResultsCriteria.GetResultsCriteria(resultsCriteria.ToString());

                foreach (ResultsCriteria.ResultsCriteriaTable childTable in loopingResultsCriteria.Tables)
                {
                    if (childTable.Id != parentTable.Id)
                    {
                        List<Relation> linkingRelations;

                        if (childTable.Id <= 0)
                            linkingRelations = new List<Relation>();
                        else
                            linkingRelations = metaDataDataView.GetRelations(parentTable.Id, childTable.Id);

                        if (linkingRelations.Count == 0)
                            throw new Exception(string.Format("Can't join table {0} to table {1} - Missing dataview relations?", parentTable.Id, childTable.Id));

#if OnlyChildTables
                    else if(linkingRelations.Count > 1)
                        throw new SQLGeneratorException(string.Format("Only inmediate child tables of the base table are allowed in current implementation. Table {0} is {1} level descendant", childTable.Id, linkingRelations.Count));
#endif

                        relationships.Add(linkingRelations);

                        foreach (Relation currentRelation in linkingRelations)
                        {
                            int tableId = ((Table)currentRelation.Parent.Table).TableId;
                            /* No need to check parent table.
                             * if (resultsCriteria[tableId] == null)
                            {
                                ResultsCriteria.ResultsCriteriaTable newTable = new ResultsCriteria.ResultsCriteriaTable();
                                newTable.Id = tableId;
                                resultsCriteria.Add(newTable);
                            }*/

                            if (!ContainsField(resultsCriteria[tableId], currentRelation.Parent.FieldId))
                                resultsCriteria[tableId].Criterias.Add(new ResultsCriteria.Field(currentRelation.Parent.FieldId));

                            tableId = ((Table)currentRelation.Child.Table).TableId;
                            if (resultsCriteria[tableId] == null)
                            {
                                ResultsCriteria.ResultsCriteriaTable newTable = new ResultsCriteria.ResultsCriteriaTable();
                                newTable.Id = tableId;
                                resultsCriteria.Tables.Add(newTable);
                            }

                            if (!ContainsField(resultsCriteria[tableId], currentRelation.Child.FieldId))
                                resultsCriteria[tableId].Criterias.Add(new ResultsCriteria.Field(currentRelation.Child.FieldId));

                        }
                    }
                }

                _coeLog.LogEnd("RetrieveRelations (Thru QuickGraph)", 1, System.Diagnostics.SourceLevels.All);
                return relationships;
            }

            /// <summary>
            /// Contains Field
            /// </summary>
            /// <param name="Table"></param>
            /// <param name="fieldId"></param>
            /// <returns></returns>
            private bool ContainsField(ResultsCriteria.ResultsCriteriaTable Table, int fieldId)
            {
                foreach (ResultsCriteria.IResultsCriteriaBase criteria in Table.Criterias)
                {
                    if (criteria.GetType() == typeof(ResultsCriteria.Field))
                    {
                        if (((ResultsCriteria.Field)criteria).Id == fieldId)
                            return true;
                    }
                }
                return false;
            }

            /// <summary>
            /// Get Parent Table
            /// </summary>
            /// <param name="resultsCriteria"></param>
            /// <param name="dataView"></param>
            /// <returns></returns>
            private ResultsCriteria.ResultsCriteriaTable GetParentTable(ResultsCriteria resultsCriteria, COEDataView dataView)
            {
                foreach (ResultsCriteria.ResultsCriteriaTable table in resultsCriteria.Tables)
                {
                    if (table.Id == dataView.Basetable)
                    {
                        return table;
                    }
                }
                return null;
            }

            /// <summary>
            /// Initializes variables related to paging modes.
            /// </summary>
            /// <param name="keepAliveModes">The keep alive mode.</param>
            private void InitKeepAlive(KeepAliveModes keepAliveModes)
            {
                switch (keepAliveModes)
                {
                    case KeepAliveModes.NONE:
                        this.directCall = true;
                        this.keepAlive = false;
                        break;
                    case KeepAliveModes.TRANSIENT:
                        this.directCall = true;
                        this.keepAlive = true;
                        break;
                    case KeepAliveModes.PERSISTENT:
                        this.directCall = false;
                        this.keepAlive = true;
                        break;
                }
            }

            /// <summary>
            /// Load the DAL
            /// </summary>
            private void LoadDAL(ConnStringType connStringType)
            {
            if (dalFactory == null) { dalFactory = new DALFactory(); }
                switch (connStringType)
                {
                    case ConnStringType.PROXY:
                        //logged in user must have credential 
                        this.dalFactory.GetDAL<CambridgeSoft.COE.Framework.COESearchService.DAL>(ref this.searchDAL, this.ServiceName, Resources.CentralizedStorageDB, false);
                        break;

                    case ConnStringType.OWNERPROXY:
                        //coedb is used for search
                        this.dalFactory.GetDAL<CambridgeSoft.COE.Framework.COESearchService.DAL>(ref this.searchDAL, this.ServiceName, Resources.CentralizedStorageDB, true);
                        break;

                }
            }

            /// <summary>
            ///     Builds a string with the qualified field name by using either the 
            ///     table alias or the table name.  The alias if used preferentially.
            /// </summary>
            /// <param name="table"></param>
            /// <param name="field"></param>
            /// <returns></returns>
            private string GetQualifiedFieldName(COEDataView.DataViewTable table, COEDataView.Field field)
            {
                StringBuilder strBuild = new StringBuilder();
                if (!string.IsNullOrEmpty(table.Alias))
                {
                    strBuild.Append(table.Alias);
                }
                else
                {
                    strBuild.Append(table.Name);
                }
                strBuild.Append(@".");


                strBuild.Append(field.Name);
                return strBuild.ToString();
            }

            /// <summary>
            /// Get a list of processors and executes its PreProcess and Process methods.
            /// </summary>
            /// <param name="expressionList">The list of SearchExpressions</param>
            /// <returns>The processors list.</returns>
            private List<SearchProcessor> GetSearchProcessorList(List<SearchCriteria.SearchExpression> expressionList, COEDataView dataview)
            {
                _coeLog.LogStart("GetSearchProcessorList", 1, System.Diagnostics.SourceLevels.All);
                List<SearchProcessor> processorList = new List<SearchProcessor>();

                foreach (SearchCriteria.SearchExpression exp in expressionList)
                {
                    if (exp.GetType() == typeof(SearchCriteria.SearchCriteriaItem))
                    {
                        SearchProcessor processor = null;
                        string type = ((SearchCriteria.SearchCriteriaItem)exp).Criterium.GetType().Name;
                        switch (type)
                        {
                            case "FormulaCriteria":
                                processor = new FormulaProcessor(((SearchCriteria.SearchCriteriaItem)exp));
                                break;
                            case "MolWeightCriteria":
                                processor = new MolWeightProcessor(((SearchCriteria.SearchCriteriaItem)exp));
                                break;
                            case "StructureCriteria":
                                processor = new StructureProcessor(((SearchCriteria.SearchCriteriaItem)exp));
                                ((StructureProcessor)processor).TempTableName = ConfigurationUtilities.GetTempQueryTable(_databaseName);
                                break;
                            case "StructureListCriteria":
                                processor = new StructureListProcessor(((SearchCriteria.SearchCriteriaItem)exp));
                                ((StructureListProcessor)processor).TempTableName = ConfigurationUtilities.GetTempQueryTable(_databaseName);
                                break;
                            case "CustomCriteria":
                                processor = BaseCustomProcessor.CreateCustomProcessor((SearchCriteria.SearchCriteriaItem)exp);
                                break;
                            case "HitlistCriteria":
                                processor = new SearchByHitlistProcessor(((SearchCriteria.SearchCriteriaItem)exp));
                                break;
                            /*case "JChemFormulaCriteria":
                            //case "JChemMolWeightCriteria":
                            //case "JchemStructureCriteria":
                                //processor = new JChemSearchProcessor(((SearchCriteria.SearchCriteriaItem)exp));
                                break;*/
                            default:
                                processor = new GenericProcessor(((SearchCriteria.SearchCriteriaItem)exp));
                                break;
                        }
                        _coeLog.Log(type + " PreProcess", 1, System.Diagnostics.SourceLevels.All);
                        processor.PreProcess(searchDAL, dataview);
                        _coeLog.Log(type + " PreProcess", 1, System.Diagnostics.SourceLevels.All);
                        processorList.Add(processor);
                    }
                    else if (exp.GetType() == typeof(SearchCriteria.LogicalCriteria))
                        processorList.AddRange(GetSearchProcessorList(((SearchCriteria.LogicalCriteria)exp).Items, dataview));
                }

                _coeLog.LogEnd("GetSearchProcessorList", 1, System.Diagnostics.SourceLevels.All);
                return processorList;
            }

            /// <summary>
            /// Get a list of processors and executes its PreProcess and Process methods for Results Criteria.
            /// </summary>
            /// <param name="expressionList">The list of SearchExpressions</param>
            /// <returns>The processors list.</returns>
            private List<SelectProcessor> GetSelectProcessorList(ResultsCriteria rc, PagingInfo pagingInfo, SearchCriteria searchCriteria)
            {
                _coeLog.LogStart("GetSelectProcessorList", 1, System.Diagnostics.SourceLevels.All);
                List<SelectProcessor> processorList = new List<SelectProcessor>();

                //foreach (SearchCriteria.SearchExpression exp in expressionList)
                foreach (ResultsCriteria.ResultsCriteriaTable currentResultsCriteriaTable in rc.Tables)
                {
                    foreach (ResultsCriteria.IResultsCriteriaBase exp in currentResultsCriteriaTable.Criterias)
                    {
                        //if (exp.GetType() == typeof(ResultsCriteria.IResultsCriteriaBase))
                        //{
                        SelectProcessor processor = null;
                        string type = ((exp).GetType().Name);
                        switch (type)
                        {
                            case "HighlightedStructure":
                                processor = new HighlightProcessor(exp, pagingInfo, ConfigurationUtilities.GetTempQueryTable(_databaseName), searchCriteria);
                                break;
                            //case "JChemMolWeight":
                            //case "JChemFormula":
                            //    processor = new JChemSelectProcessor();
                            //    break;
                            default:
                                //processor = new GenericSelectProcessor(((SearchCriteria.SearchCriteriaItem)exp));
                                break;
                        }
                        if (processor != null)
                        {
                            _coeLog.Log(type + " PreProcess", 1, System.Diagnostics.SourceLevels.All);
                            processor.PreProcess(searchDAL);
                            _coeLog.Log(type + " PreProcess", 1, System.Diagnostics.SourceLevels.All);
                            processorList.Add(processor);
                            _coeLog.Log(type + " Process", 1, System.Diagnostics.SourceLevels.All);
                            processor.Process(exp);
                            _coeLog.Log(type + " Process", 1, System.Diagnostics.SourceLevels.All);
                        }
                    }
                    //else if (exp.GetType() == typeof(SearchCriteria.LogicalCriteria))
                    //    processorList.AddRange(GetSearchProcessorList(((SearchCriteria.LogicalCriteria)exp).Items));
                    //}
                }
                _coeLog.LogEnd("GetSelectProcessorList", 1, System.Diagnostics.SourceLevels.All);
                return processorList;
            }



            private COEDataView ApplyDataviewFilter(COEDataView dataView, ResultsCriteria resultCriteria)
            {
                if (!_applyDataviewFilter)
                    return dataView;

                COEDataView RetDataView = new COEDataView();
                RetDataView.Database = dataView.Database;
                RetDataView.Basetable = dataView.Basetable;
                RetDataView.DataViewID = dataView.DataViewID;
                RetDataView.DataViewHandling = dataView.DataViewHandling;
                RetDataView.Name = dataView.Name;

                // Add all tables referenced in the resultsCriteria.
                foreach (ResultsCriteria.ResultsCriteriaTable rcTbl in resultCriteria.Tables)
                {
                    // Add tables in rc fields
                    COEDataView.DataViewTable tbl = dataView.Tables.getById(rcTbl.Id);
                    if (!RetDataView.Tables.Contains(tbl)) RetDataView.Tables.Add(tbl);
                    // Add tables for lookup fields
                    this.AddTablesForLookupFields(ref RetDataView, tbl, dataView);
                    // Add tables in aggregatefunctions
                    this.AddTablesForAggregateFunctionFields(ref RetDataView, rcTbl, dataView);
                }

                this.AddRelationshipsToFileteredDataview(ref RetDataView, dataView);


                if (RetDataView.Tables.Count == 0)
                {
                    throw new DataviewException("Tables in resultCriteria not found in Dataview");
                }
                else
                {
                    return RetDataView;
                }
            }

            /// <summary>
            /// Filters (eliminates content) from COEDataview object.
            /// Removes all tables and relationships except the table indicated by tableId.
            /// The only remaining table becomes the base table 
            /// </summary>
            /// <param name="dataView">The dataview to filter</param>
            /// <param name="tableId">The table to keep</param>
            private COEDataView ApplyDataviewFilter(COEDataView dataView, int tableId)
            {
                if (!_applyDataviewFilter)
                    return dataView;

                COEDataView RetDataView = new COEDataView();
                RetDataView.Database = dataView.Database;
                RetDataView.Basetable = tableId;
                RetDataView.DataViewID = dataView.DataViewID;
                RetDataView.DataViewHandling = dataView.DataViewHandling;
                RetDataView.Name = dataView.Name;

                // Add table indicated by tableId parameter.
                foreach (COEDataView.DataViewTable dvTbl in dataView.Tables)
                {
                    if (dvTbl.Id == tableId)
                    {
                        RetDataView.Tables.Add(dvTbl);
                    }
                }

                if (RetDataView.Tables.Count == 0)
                {
                    throw new DataviewException(string.Format("Table not found while filtering Dataview {0} by TableId {1}", dataView.DataViewID, tableId));
                }
                else
                {
                    return RetDataView;
                }
            }

            /// <summary>
            /// Filters (eliminates content) from COEDataview object.
            /// Removes all tables and relationships except those needed for the searchCriteria.
            /// The dataview includes tables needed for lookups. 
            /// </summary>
            /// <param name="dataView">The dataview to filter</param>
            /// <param name="tableId">The table to keep</param>
            private COEDataView ApplyDataviewFilter(COEDataView dataView, SearchCriteria searchCriteria)
            {
                if (!_applyDataviewFilter)
                    return dataView;

                COEDataView RetDataView = new COEDataView();
                RetDataView.Database = dataView.Database;
                RetDataView.Basetable = dataView.Basetable;
                RetDataView.DataViewID = dataView.DataViewID;
                RetDataView.DataViewHandling = dataView.DataViewHandling;
                RetDataView.Name = dataView.Name;

                // Add tables from searchCriteria items.
                CambridgeSoft.COE.Framework.Common.SqlGenerator.MetaData.SearchCriteria sc = new Common.SqlGenerator.MetaData.SearchCriteria();
                sc.LoadFromXML(searchCriteria.ToString());
                List<int> scTables = sc.GetTableIds();
                foreach (int tableId in scTables)
                {
                    COEDataView.DataViewTable dvTbl = dataView.Tables.getById(tableId);
                    if (!RetDataView.Tables.Contains(dvTbl)) RetDataView.Tables.Add(dvTbl);
                    // Add any tables involved in lookups
                    this.AddTablesForLookupFields(ref RetDataView, dvTbl, dataView);
                }

                this.AddRelationshipsToFileteredDataview(ref RetDataView, dataView);

                if (RetDataView.Tables.Count == 0)
                {
                    throw new DataviewException(string.Format("Table not found while filtering Dataview {0} for current search criteria", dataView.DataViewID));
                }
                else
                {
                    return RetDataView;
                }
            }

            /// <summary>
            /// Adds relationships to the filtered dataview.
            /// Used after tables have added to the new dataview.  It adds relationships present
            /// in the original dataview that are still valid for the new dataview.
            /// </summary> 
            /// <param name="filteredDV">The filtered dataview</param>
            /// <param name="dataView">The unfiltered dataview</param>
            private void AddRelationshipsToFileteredDataview(ref COEDataView filteredDV, COEDataView dataView)
            {
                foreach (COEDataView.Relationship rel in dataView.Relationships)
                {
                    if (filteredDV.Tables.getById(rel.Parent) != null && filteredDV.Tables.getById(rel.Child) != null)
                    {
                        if (!filteredDV.Relationships.Contains(rel)) filteredDV.Relationships.Add(rel);
                    }
                }
            }

            /// <summary>
            /// Adds tables needed to resolve lookup fields to the filtered dataview.
            /// </summary>
            /// <param name="filteredDV">The dataview being filtered</param>
            /// <param name="dataView">The orgininal dataview</param>
            private void AddTablesForLookupFields(ref COEDataView filteredDV, COEDataView.DataViewTable tbl, COEDataView dataView)
            {
                foreach (COEDataView.Field fld in tbl.Fields)
                {
                    if (fld.LookupFieldId != -1)
                    {
                        COEDataView.Field lookupFld = dataView.GetFieldById(fld.LookupFieldId);
                        COEDataView.DataViewTable lookupTbl = dataView.Tables.getById(lookupFld.ParentTableId);
                        if (!filteredDV.Tables.Contains(lookupTbl)) filteredDV.Tables.Add(lookupTbl);
                    }
                }
            }

            /// <summary>
            /// Adds tables needed to resolve the fileds in aggregate functions to the filtered dataview
            /// </summary>
            /// <param name="filteredDataview">The dataview being filtered</param>
            /// <param name="rcTbl">The results criteria</param>
            /// <param name="coeDataview">The unfiltered dataview</param>
            private void AddTablesForAggregateFunctionFields(ref COEDataView filteredDataview, ResultsCriteria.ResultsCriteriaTable rcTbl, COEDataView coeDataview)
            {
                foreach (ResultsCriteria.IResultsCriteriaBase rcb in rcTbl.Criterias)
                {
                    if (rcb.GetType().Name == "AggregateFunction")
                    {
                        ResultsCriteria.AggregateFunction af = (ResultsCriteria.AggregateFunction)rcb;
                        this.AddTablesForAggregateFunctionFields(ref filteredDataview, af, coeDataview);
                    }
                    else if (rcb.GetType().Name == "SQLFunction")
                    {
                        ResultsCriteria.SQLFunction sf = (ResultsCriteria.SQLFunction)rcb;
                        foreach (ResultsCriteria.IResultsCriteriaBase rcb2 in sf.Parameters)
                        {
                            if (rcb2.GetType().Name == "AggregateFuncion")
                            {
                                ResultsCriteria.AggregateFunction af = (ResultsCriteria.AggregateFunction)rcb2;
                                this.AddTablesForAggregateFunctionFields(ref filteredDataview, af, coeDataview);
                            }
                        }
                    }
                }
            }

            /// <summary>
            /// Given an AggregateFunction ResultsCritria add fields found in it to the filetered table
            /// </summary>
            /// <param name="filteredDataview">The dataview being filtered</param>
            /// <param name="af">The AggregateFunction</param>
            /// <param name="coeDataview">The unfiltered dataview</param>
            private void AddTablesForAggregateFunctionFields(ref COEDataView filteredDataview, ResultsCriteria.AggregateFunction af, COEDataView coeDataview)
            {
                foreach (ResultsCriteria.IResultsCriteriaBase rcb in af.Parameters)
                {
                    if (rcb.GetType().Name == "Field")
                    {
                        ResultsCriteria.Field rcfld = (ResultsCriteria.Field)rcb;
                        COEDataView.Field fld = coeDataview.GetFieldById(rcfld.Id);
                        COEDataView.DataViewTable dvTbl = coeDataview.Tables.getById(fld.ParentTableId);
                        if (!filteredDataview.Tables.Contains(dvTbl)) filteredDataview.Tables.Add(dvTbl);
                    }
                    //TODO: What if aggregating something different to a field, like molweight or a round...
                }
            }

            private static List<COEDataView.Relationship> FindRelations(List<COEDataView.Relationship> relations, int parentTableId, int childTableId)
            {
                return relations.FindAll(delegate(COEDataView.Relationship relation)
                {
                    return (relation.Parent == parentTableId && relation.Child == childTableId);
                });
            }


            /// <summary>
            /// If the TableName of the child table in the relation matches the ColumnName of a column in the parent table in the relation, then it return false
            /// </summary>
            /// <param name="parentTable"></param>
            /// <param name="childTable"></param>
            /// <returns></returns>
            private bool IsParentNestedWithChildTable(DataTable parentTable, DataTable childTable)
            {
                for (int pTblcounter = 0; pTblcounter < parentTable.Columns.Count; pTblcounter++)
                {
                    if (parentTable.Columns[pTblcounter].ToString().Trim().Equals(childTable.ToString().Trim(), StringComparison.OrdinalIgnoreCase))
                        return false;
                }
                return true;
            }

            #endregion
		/// <summary>
        /// Load the DAL
        /// </summary>
        private void LoadCOEDAL(ConnStringType connStringType)
        {
            if (this.coeDAL != null)
            {
                return;
            }

            if (dalFactory == null)
            {
                dalFactory = new DALFactory();
            }

            switch (connStringType)
            {
                case ConnStringType.PROXY:
                    //logged in user must have credential 
                    this.dalFactory.GetDAL<CambridgeSoft.COE.Framework.COESearchService.DAL>(ref this.coeDAL, this.ServiceName, Resources.CentralizedStorageDB, false);

                    break;

                case ConnStringType.OWNERPROXY:
                    //coedb is used for search
                    this.dalFactory.GetDAL<CambridgeSoft.COE.Framework.COESearchService.DAL>(ref this.coeDAL, this.ServiceName, Resources.CentralizedStorageDB, true);
                    break;
            }

            if (coeDAL == null)
            {
                throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));
            }
        }

    }
}
