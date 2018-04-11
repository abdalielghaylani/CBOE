// --------------------------------------------------------------------------------------------------------------------
// <copyright file="COESearch.cs" company="PerkinElmer Inc.">
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

namespace CambridgeSoft.COE.Framework.COESearchService
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using Oracle.DataAccess.Client;

    using Csla;
    using CambridgeSoft.COE.Framework.Properties;
    using CambridgeSoft.COE.Framework.Common;
    using CambridgeSoft.COE.Framework.COELoggingService;
    using CambridgeSoft.COE.Framework.COEDataViewService;

    //this object is not  a business object since
    //we are not directly working with CRUD operations
    //Instead this object will be more for containing command objects which will create 
    //calls to the search manager to execute it's functionality
    //we could continue to refactor these classes to encapsulate the search manager
    //logic, but I'm not sure it that is necessary.
    /// <summary>
    /// COESearch is the class used for perfoming searches through the COEFramework.
    /// </summary>
    /// 
    [Serializable]
    public class COESearch
    {
        #region Variables
        [NonSerialized]
        static COELog _coeLog = COELog.GetSingleton("COESearch");

        private int _maxRecordCount = -1;
        private int _lastInsertedSearchCriteriaID = 0;

        #region Variables for simplified search methods
        private int _dataViewID;
        private COEDataViewBO _dataViewBO = null;
        #endregion
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a COESearch class with its defaults values. Not suitable for using the simplified API.
        /// </summary>
        public COESearch() {
        }

        /// <summary>
        /// Initializes a search object that suites best for simplified API.
        /// </summary>
        /// <param name="dataviewID">Dataview ID to be used in the searches using the simplified API</param>
        public COESearch(int dataviewID)
		{
			_dataViewID = dataviewID;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets of sets the maximum amount of records to retrieve in a search.
        /// </summary>
        public int MaxRecordCount
        {
            get {
                return _maxRecordCount;
            }
            set {
                _maxRecordCount = value;
            }
        }

        /// <summary>
        /// Gets the last inserted search criteria ID.
        /// </summary>
        public int LastInsertedSearchCriteriaID {
            get {
                return _lastInsertedSearchCriteriaID;
            }
        }
        #endregion

        #region DoSearch
        /// <summary>
        /// <para>General purpose search method, that based on the input parameters performs differents searches.</para>
        /// <para>Four parameters are needed:</para>
        /// <list type="bullet">
        ///   <item><see cref="SearchCriteria"/>: This parameter defines the restrictions of the search.</item>
        ///   <item><see cref="ResultsCriteria"/>: This parameter defines the desired result fields.</item>
        ///   <item><see cref="PagingInfo"/>: This parameter defines the paging to be done.</item>
        ///   <item><see cref="COEDataView"/>: This parameter defines an id that links to a particular view of the database and the search will be performed
        ///   over that view.</item>
        /// </list>
        /// </summary>
        /// <param name="searchCriteria"><see cref="SearchCriteria"/> Search restrictions.</param>
        /// <param name="resultsCriteria"><see cref="ResultsCriteria"/>Desired resulting fields and tables.</param>
        /// <param name="pagingInfo"><see cref="PagingInfo"/>Desired paging.</param>
        /// <param name="dataViewID"><seealso cref="COEDataView"/>A database view identified by id</param>
        /// <returns>A <see cref="SearchResponse"/> containing the resulting dataset and the modified <see cref="HitListInfo"/> and <see cref="PagingInfo"/>.</returns>
        public SearchResponse DoSearch(SearchCriteria searchCriteria, ResultsCriteria resultsCriteria, PagingInfo pagingInfo, int dataViewID) {
            try
            {
                return DoSearch(searchCriteria, resultsCriteria, pagingInfo, GetDataViewFromID(dataViewID), GetConnStringType(dataViewID), null, new OrderByCriteria());

            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }

        /// <summary>
        /// <para>General purpose search method, that based on the input parameters performs differents searches.</para>
        /// <para>Four parameters are needed:</para>
        /// <list type="bullet">
        ///   <item><see cref="SearchCriteria"/>: This parameter defines the restrictions of the search.</item>
        ///   <item><see cref="ResultsCriteria"/>: This parameter defines the desired result fields.</item>
        ///   <item><see cref="PagingInfo"/>: This parameter defines the paging to be done.</item>
        ///   <item><see cref="COEDataView"/>: This parameter defines an id that links to a particular view of the database and the search will be performed
        ///   over that view.</item>
        /// </list>
        /// </summary>
        /// <param name="searchCriteria">Search restrictions.</param>
        /// <param name="resultsCriteria">Desired resulting fields and tables.</param>
        /// <param name="pagingInfo">Desired paging.</param>
        /// <param name="dataViewID">a database view identified by id</param>
        /// <param name="useRealTableNames">string that specifies if real table names are supposed to be returned (yes/true), if table names in the form Table_tableid (no/false) or if this is up o the search service (null/empty)</param>
        /// <returns>A <see cref="SearchResponse"/> containing the resulting dataset and the modified <see cref="HitListInfo"/> and <see cref="PagingInfo"/>.</returns>
        public SearchResponse DoSearch(SearchCriteria searchCriteria, ResultsCriteria resultsCriteria, PagingInfo pagingInfo, int dataViewID, string useRealTableNames) {
            try {
                return DoSearch(searchCriteria, resultsCriteria, pagingInfo, GetDataViewFromID(dataViewID), GetConnStringType(dataViewID), useRealTableNames, new OrderByCriteria());

            } catch(Exception ex) {
                throw ex;
            }

        }

        /// <summary>
        /// <para>General purpose search method, that based on the input parameters performs differents searches.</para>
        /// <para>Four parameters are needed:</para>
        /// <list type="bullet">
        ///   <item><see cref="SearchCriteria"/>: This parameter defines the restrictions of the search.</item>
        ///   <item><see cref="ResultsCriteria"/>: This parameter defines the desired result fields.</item>
        ///   <item><see cref="PagingInfo"/>: This parameter defines the paging to be done.</item>
        ///   <item><see cref="COEDataView"/>: This parameter defines a particular view of the database and the search will be performed
        ///   over that view.</item>
        /// </list>
        /// </summary>
        /// <param name="searchCriteria">Search restrictions.</param>
        /// <param name="resultsCriteria">Desired resulting fields and tables.</param>
        /// <param name="pagingInfo">Desired paging.</param>
        /// <param name="dataView">A database view. This view is ad hoc and requires that the logged in user have permissions</param>
        /// <returns>A <see cref="SearchResponse"/> containing the resulting dataset and the modified <see cref="HitListInfo"/> and <see cref="PagingInfo"/>.</returns>
        public SearchResponse DoSearch(SearchCriteria searchCriteria, ResultsCriteria resultsCriteria, PagingInfo pagingInfo, COEDataView dataView)
        {
            this.ApplyDataviewHandling(ref dataView);
            return DoSearch(searchCriteria, resultsCriteria, pagingInfo, dataView, GetConnStringType(dataView), null, new OrderByCriteria());
        }

        /// <summary>
        /// <para>General purpose search method, that based on the input parameters performs differents searches.</para>
        /// <para>Four parameters are needed:</para>
        /// <list type="bullet">
        ///   <item><see cref="SearchCriteria"/>: This parameter defines the restrictions of the search.</item>
        ///   <item><see cref="ResultsCriteria"/>: This parameter defines the desired result fields.</item>
        ///   <item><see cref="PagingInfo"/>: This parameter defines the paging to be done.</item>
        ///   <item><see cref="COEDataView"/>: This parameter defines a particular view of the database and the search will be performed
        ///   over that view.</item>
        /// </list>
        /// </summary>
        /// <param name="searchCriteria">Search restrictions.</param>
        /// <param name="resultsCriteria">Desired resulting fields and tables.</param>
        /// <param name="pagingInfo">Desired paging.</param>
        /// <param name="dataView">A database view. This view is ad hoc and requires that the logged in user have permissions</param>
        /// <param name="useRealTableNames">string that specifies if real table names are supposed to be returned (yes/true), if table names in the form Table_tableid (no/false) or if this is up o the search service (null/empty)</param>
        /// <returns>A <see cref="SearchResponse"/> containing the resulting dataset and the modified <see cref="HitListInfo"/> and <see cref="PagingInfo"/>.</returns>
        public SearchResponse DoSearch(SearchCriteria searchCriteria, ResultsCriteria resultsCriteria, PagingInfo pagingInfo, COEDataView dataView, string useRealTableNames)
        {
            this.ApplyDataviewHandling(ref dataView);
            return DoSearch(searchCriteria, resultsCriteria, pagingInfo, dataView, GetConnStringType(dataView), useRealTableNames, new OrderByCriteria());
        }

        /// <summary>
        /// <para>General purpose search method, that based on the input parameters performs differents searches.</para>
        /// <para>Four parameters are needed:</para>
        /// <list type="bullet">
        ///   <item><see cref="SearchCriteria"/>: This parameter defines the restrictions of the search.</item>
        ///   <item><see cref="ResultsCriteria"/>: This parameter defines the desired result fields.</item>
        ///   <item><see cref="PagingInfo"/>: This parameter defines the paging to be done.</item>
        ///   <item><see cref="COEDataView"/>: This parameter defines a particular view of the database and the search will be performed
        ///   over that view.</item>
        /// </list>
        /// </summary>
        /// <param name="searchCriteria">Search restrictions.</param>
        /// <param name="resultsCriteria">Desired resulting fields and tables.</param>
        /// <param name="pagingInfo">Desired paging.</param>
        /// <param name="dataView">A database view. This view is ad hoc and requires that the logged in user have permissions</param>
        /// <param name="useRealTableNames">string that specifies if real table names are supposed to be returned (yes/true), if table names in the form Table_tableid (no/false) or if this is up o the search service (null/empty)</param>
        /// <param name="orderByCriteria">Client ordering criteria</param>
        /// <returns>A <see cref="SearchResponse"/> containing the resulting dataset and the modified <see cref="HitListInfo"/> and <see cref="PagingInfo"/>.</returns>
        public SearchResponse DoSearch(SearchCriteria searchCriteria, ResultsCriteria resultsCriteria, PagingInfo pagingInfo, COEDataView dataView, string useRealTableNames, OrderByCriteria orderByCriteria)
        {
            this.ApplyDataviewHandling(ref dataView);
            return DoSearch(searchCriteria, resultsCriteria, pagingInfo, dataView, GetConnStringType(dataView), useRealTableNames, orderByCriteria);
        }

        private SearchResponse DoSearch(SearchCriteria searchCriteria, ResultsCriteria resultsCriteria, PagingInfo pagingInfo, COEDataView dataView, ConnStringType connType, string useRealTableNames, OrderByCriteria orderByCriteria)
        {
            try
            {
                DoSearchCommand result = DataPortal.Execute<DoSearchCommand>(new DoSearchCommand(searchCriteria, resultsCriteria, pagingInfo, dataView,  connType, useRealTableNames, orderByCriteria));
                _lastInsertedSearchCriteriaID = result.SearchCriteriaID;
                return result.SearchResponse;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region DoPartialSearch
        /// <summary>
        /// <para>General purpose search method, that based on the input parameters performs differents searches.</para>
        /// <para>Four parameters are needed:</para>
        /// <list type="bullet">
        ///   <item><see cref="SearchCriteria"/>: This parameter defines the restrictions of the search.</item>
        ///   <item><see cref="ResultsCriteria"/>: This parameter defines the desired result fields.</item>
        ///   <item><see cref="PagingInfo"/>: This parameter defines the paging to be done.</item>
        ///   <item><see cref="COEDataView"/>: This parameter defines a particular view of the database and the search will be performed
        ///   over that view.</item>
        /// </list>
        /// </summary>
        /// <param name="searchCriteria">Search restrictions.</param>
        /// <param name="resultsCriteria">Desired resulting fields and tables.</param>
        /// <param name="pagingInfo">Desired paging.</param>
        /// <param name="dataViewID">A database view identified by ID</param>
        /// <returns>A <see cref="SearchResponse"/> containing the resulting dataset and the modified <see cref="HitListInfo"/> and <see cref="PagingInfo"/>.</returns>
        public SearchResponse DoPartialSearch(SearchCriteria searchCriteria, ResultsCriteria resultsCriteria, PagingInfo pagingInfo, int dataViewID) {
            try {
                return DoPartialSearch(searchCriteria, resultsCriteria, pagingInfo, GetDataViewFromID(dataViewID), true, GetConnStringType(dataViewID), -1, null);
            } catch(Exception) {
                throw;
            }
        }

        /// <summary>
        /// <para>General purpose search method, that based on the input parameters performs differents searches.</para>
        /// <para>Four parameters are needed:</para>
        /// <list type="bullet">
        ///   <item><see cref="SearchCriteria"/>: This parameter defines the restrictions of the search.</item>
        ///   <item><see cref="ResultsCriteria"/>: This parameter defines the desired result fields.</item>
        ///   <item><see cref="PagingInfo"/>: This parameter defines the paging to be done.</item>
        ///   <item><see cref="COEDataView"/>: This parameter defines a particular view of the database and the search will be performed
        ///   over that view.</item>
        /// </list>
        /// </summary>
        /// <param name="searchCriteria">Search restrictions.</param>
        /// <param name="resultsCriteria">Desired resulting fields and tables.</param>
        /// <param name="pagingInfo">Desired paging.</param>
        /// <param name="dataViewID">A database view identified by ID</param>
        /// <param name="useRealTableNames">string that specifies if real table names are supposed to be returned (yes/true), if table names in the form Table_tableid (no/false) or if this is up o the search service (null/empty)</param>
        /// <returns>A <see cref="SearchResponse"/> containing the resulting dataset and the modified <see cref="HitListInfo"/> and <see cref="PagingInfo"/>.</returns>
        public SearchResponse DoPartialSearch(SearchCriteria searchCriteria, ResultsCriteria resultsCriteria, PagingInfo pagingInfo, int dataViewID, string useRealTableNames) {
            try {
                return DoPartialSearch(searchCriteria, resultsCriteria, pagingInfo, GetDataViewFromID(dataViewID), true, GetConnStringType(dataViewID), -1, useRealTableNames);
            } catch(Exception) {
                throw;
            }
        }

        /// <summary>
        /// <para>General purpose search method, that based on the input parameters performs differents searches.</para>
        /// <para>Four parameters are needed:</para>
        /// <list type="bullet">
        ///   <item><see cref="SearchCriteria"/>: This parameter defines the restrictions of the search.</item>
        ///   <item><see cref="ResultsCriteria"/>: This parameter defines the desired result fields.</item>
        ///   <item><see cref="PagingInfo"/>: This parameter defines the paging to be done.</item>
        ///   <item><see cref="COEDataView"/>: This parameter defines a particular view of the database and the search will be performed
        ///   over that view.</item>
        /// </list>
        /// </summary>
        /// <param name="searchCriteria">Search restrictions.</param>
        /// <param name="resultsCriteria">Desired resulting fields and tables.</param>
        /// <param name="pagingInfo">Desired paging.</param>
        /// <param name="dataViewID">A database view identified by ID</param>
        /// <returns>A <see cref="SearchResponse"/> containing the resulting dataset and the modified <see cref="HitListInfo"/> and <see cref="PagingInfo"/>.</returns>
        public SearchResponse DoPartialSearch(SearchCriteria searchCriteria, ResultsCriteria resultsCriteria, PagingInfo pagingInfo, int dataViewID, int commitSize) {
            try {
                return DoPartialSearch(searchCriteria, resultsCriteria, pagingInfo, GetDataViewFromID(dataViewID), true, GetConnStringType(dataViewID), commitSize, null);
            } catch(Exception) {
                throw;
            }
        }

        /// <summary>
        /// <para>General purpose search method, that based on the input parameters performs differents searches.</para>
        /// <para>Four parameters are needed:</para>
        /// <list type="bullet">
        ///   <item><see cref="SearchCriteria"/>: This parameter defines the restrictions of the search.</item>
        ///   <item><see cref="ResultsCriteria"/>: This parameter defines the desired result fields.</item>
        ///   <item><see cref="PagingInfo"/>: This parameter defines the paging to be done.</item>
        ///   <item><see cref="COEDataView"/>: This parameter defines a particular view of the database and the search will be performed
        ///   over that view.</item>
        /// </list>
        /// </summary>
        /// <param name="searchCriteria">Search restrictions.</param>
        /// <param name="resultsCriteria">Desired resulting fields and tables.</param>
        /// <param name="pagingInfo">Desired paging.</param>
        /// <param name="dataViewID">A database view identified by ID</param>
        /// <param name="useRealTableNames">string that specifies if real table names are supposed to be returned (yes/true), if table names in the form Table_tableid (no/false) or if this is up o the search service (null/empty)</param>
        /// <returns>A <see cref="SearchResponse"/> containing the resulting dataset and the modified <see cref="HitListInfo"/> and <see cref="PagingInfo"/>.</returns>
        public SearchResponse DoPartialSearch(SearchCriteria searchCriteria, ResultsCriteria resultsCriteria, PagingInfo pagingInfo, int dataViewID, int commitSize, string useRealTableNames) {
            try {
                return DoPartialSearch(searchCriteria, resultsCriteria, pagingInfo, GetDataViewFromID(dataViewID), true, GetConnStringType(dataViewID), commitSize, useRealTableNames);
            } catch(Exception) {
                throw;
            }
        }

        /// <summary>
        /// <para>General purpose search method, that based on the input parameters performs differents searches.</para>
        /// <para>Four parameters are needed:</para>
        /// <list type="bullet">
        ///   <item><see cref="SearchCriteria"/>: This parameter defines the restrictions of the search.</item>
        ///   <item><see cref="ResultsCriteria"/>: This parameter defines the desired result fields.</item>
        ///   <item><see cref="PagingInfo"/>: This parameter defines the paging to be done.</item>
        ///   <item><see cref="COEDataView"/>: This parameter defines a particular view of the database and the search will be performed
        ///   over that view.</item>
        /// </list>
        /// </summary>
        /// <param name="searchCriteria">Search restrictions.</param>
        /// <param name="resultsCriteria">Desired resulting fields and tables.</param>
        /// <param name="pagingInfo">Desired paging.</param>
        /// <param name="dataView">A database view.</param>
        /// <returns>A <see cref="SearchResponse"/> containing the resulting dataset and the modified <see cref="HitListInfo"/> and <see cref="PagingInfo"/>.</returns>
        public SearchResponse DoPartialSearch(SearchCriteria searchCriteria, ResultsCriteria resultsCriteria, PagingInfo pagingInfo, COEDataView dataView)
        {
            try
            {
                this.ApplyDataviewHandling(ref dataView);
                return DoPartialSearch(searchCriteria, resultsCriteria, pagingInfo, dataView, true, GetConnStringType(dataView), -1, null);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// <para>General purpose search method, that based on the input parameters performs differents searches.</para>
        /// <para>Four parameters are needed:</para>
        /// <list type="bullet">
        ///   <item><see cref="SearchCriteria"/>: This parameter defines the restrictions of the search.</item>
        ///   <item><see cref="ResultsCriteria"/>: This parameter defines the desired result fields.</item>
        ///   <item><see cref="PagingInfo"/>: This parameter defines the paging to be done.</item>
        ///   <item><see cref="COEDataView"/>: This parameter defines a particular view of the database and the search will be performed
        ///   over that view.</item>
        /// </list>
        /// </summary>
        /// <param name="searchCriteria">Search restrictions.</param>
        /// <param name="resultsCriteria">Desired resulting fields and tables.</param>
        /// <param name="pagingInfo">Desired paging.</param>
        /// <param name="dataView">A database view.</param>
        /// <param name="useRealTableNames">string that specifies if real table names are supposed to be returned (yes/true), if table names in the form Table_tableid (no/false) or if this is up o the search service (null/empty)</param>
        /// <returns>A <see cref="SearchResponse"/> containing the resulting dataset and the modified <see cref="HitListInfo"/> and <see cref="PagingInfo"/>.</returns>
        public SearchResponse DoPartialSearch(SearchCriteria searchCriteria, ResultsCriteria resultsCriteria, PagingInfo pagingInfo, COEDataView dataView, string useRealTableNames) {
            try {
                this.ApplyDataviewHandling(ref dataView);
                return DoPartialSearch(searchCriteria, resultsCriteria, pagingInfo, dataView, true, GetConnStringType(dataView), -1, useRealTableNames);
            } catch(Exception) {
                throw;
            }
        }

        /// <summary>
        /// <para>General purpose search method, that based on the input parameters performs differents searches.</para>
        /// <para>Four parameters are needed:</para>
        /// <list type="bullet">
        ///   <item><see cref="SearchCriteria"/>: This parameter defines the restrictions of the search.</item>
        ///   <item><see cref="ResultsCriteria"/>: This parameter defines the desired result fields.</item>
        ///   <item><see cref="PagingInfo"/>: This parameter defines the paging to be done.</item>
        ///   <item><see cref="COEDataView"/>: This parameter defines a particular view of the database and the search will be performed
        ///   over that view.</item>
        /// </list>
        /// </summary>
        /// <param name="searchCriteria">Search restrictions.</param>
        /// <param name="resultsCriteria">Desired resulting fields and tables.</param>
        /// <param name="pagingInfo">Desired paging.</param>
        /// <param name="dataView">A database view.</param>
        /// <returns>A <see cref="SearchResponse"/> containing the resulting dataset and the modified <see cref="HitListInfo"/> and <see cref="PagingInfo"/>.</returns>
        public SearchResponse DoPartialSearch(SearchCriteria searchCriteria, ResultsCriteria resultsCriteria, PagingInfo pagingInfo, COEDataView dataView, int commitSize) {
            try {
                this.ApplyDataviewHandling(ref dataView);
                return DoPartialSearch(searchCriteria, resultsCriteria, pagingInfo, dataView, true, GetConnStringType(dataView), commitSize, null);
            } catch(Exception) {
                throw;
            }
        }

        /// <summary>
        /// <para>General purpose search method, that based on the input parameters performs differents searches.</para>
        /// <para>Four parameters are needed:</para>
        /// <list type="bullet">
        ///   <item><see cref="SearchCriteria"/>: This parameter defines the restrictions of the search.</item>
        ///   <item><see cref="ResultsCriteria"/>: This parameter defines the desired result fields.</item>
        ///   <item><see cref="PagingInfo"/>: This parameter defines the paging to be done.</item>
        ///   <item><see cref="COEDataView"/>: This parameter defines a particular view of the database and the search will be performed
        ///   over that view.</item>
        /// </list>
        /// </summary>
        /// <param name="searchCriteria">Search restrictions.</param>
        /// <param name="resultsCriteria">Desired resulting fields and tables.</param>
        /// <param name="pagingInfo">Desired paging.</param>
        /// <param name="dataView">A database view.</param>
        /// <param name="useRealTableNames">string that specifies if real table names are supposed to be returned (yes/true), if table names in the form Table_tableid (no/false) or if this is up o the search service (null/empty)</param>
        /// <returns>A <see cref="SearchResponse"/> containing the resulting dataset and the modified <see cref="HitListInfo"/> and <see cref="PagingInfo"/>.</returns>
        public SearchResponse DoPartialSearch(SearchCriteria searchCriteria, ResultsCriteria resultsCriteria, PagingInfo pagingInfo, COEDataView dataView, int commitSize, string useRealTableNames) {
            try {
                this.ApplyDataviewHandling(ref dataView);
                return DoPartialSearch(searchCriteria, resultsCriteria, pagingInfo, dataView, true, GetConnStringType(dataView), commitSize, useRealTableNames);
            } catch(Exception) {
                throw;
            }
        }

        private SearchResponse DoPartialSearch(SearchCriteria searchCriteria, ResultsCriteria resultsCriteria, PagingInfo pagingInfo, COEDataView dataView, bool partialSearch, ConnStringType connStringType, int commitSize, string useRealTableNames) {
            try {
                DoSearchCommand result = DataPortal.Execute<DoSearchCommand>(new DoSearchCommand(searchCriteria, resultsCriteria, pagingInfo, dataView, partialSearch, connStringType, commitSize, useRealTableNames));
                _lastInsertedSearchCriteriaID = result.SearchCriteriaID;
                return result.SearchResponse;
            } catch(Exception) {
                throw;
            }
        }
        #endregion

        #region GetData
        /// <summary>
        /// <para>Search method that will get the data for an existing HitList.</para>
        /// <para>Three parameters are needed:</para>
        /// <list type="bullet">
        ///   <item><see cref="ResultsCriteria"/>: This parameter defines the desired result fields.</item>
        ///   <item><see cref="PagingInfo"/>: This parameter defines the paging to be done and <b>the existing hitlistid</b>.</item>
        ///   <item><see cref="COEDataView"/>: This parameter defines a particular view of the database and the search will be performed
        ///   over that view.</item>
        /// </list>
        /// </summary>
        /// <param name="resultsCriteria">Desired resulting fields and tables.</param>
        /// <param name="pagingInfo">Desired paging and <b>the hitlistid</b>.</param>
        /// <param name="dataView">A database view identified by ID</param>
        /// <returns>The resulting dataset.</returns>
        public DataSet GetData(ResultsCriteria resultsCriteria, PagingInfo pagingInfo, int dataViewID)
        {
            try
            {
                return GetData(resultsCriteria, pagingInfo, GetDataViewFromID(dataViewID), GetConnStringType(dataViewID), null, new OrderByCriteria(), null);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// <para>Search method that will get the data for an existing HitList.</para>
        /// <para>Three parameters are needed:</para>
        /// <list type="bullet">
        ///   <item><see cref="ResultsCriteria"/>: This parameter defines the desired result fields.</item>
        ///   <item><see cref="PagingInfo"/>: This parameter defines the paging to be done and <b>the existing hitlistid</b>.</item>
        ///   <item><see cref="COEDataView"/>: This parameter defines a particular view of the database and the search will be performed
        ///   over that view.</item>
        /// </list>
        /// </summary>
        /// <param name="resultsCriteria">Desired resulting fields and tables.</param>
        /// <param name="pagingInfo">Desired paging and <b>the hitlistid</b>.</param>
        /// <param name="dataView">A database view identified by ID</param>
        /// <param name="useRealTableNames">string that specifies if real table names are supposed to be returned (yes/true), if table names in the form Table_tableid (no/false) or if this is up o the search service (null/empty)</param>
        /// <returns>The resulting dataset.</returns>
        public DataSet GetData(ResultsCriteria resultsCriteria, PagingInfo pagingInfo, int dataViewID, string useRealTableNames) {
            try {
                return GetData(resultsCriteria, pagingInfo, GetDataViewFromID(dataViewID), GetConnStringType(dataViewID), useRealTableNames, new OrderByCriteria(), null);
            } catch(Exception ex) {
                throw;
            }
        }

        /// <summary>
        /// <para>Search method that will get the data for an existing HitList.</para>
        /// <para>Three parameters are needed:</para>
        /// <list type="bullet">
        ///   <item><see cref="ResultsCriteria"/>: This parameter defines the desired result fields.</item>
        ///   <item><see cref="PagingInfo"/>: This parameter defines the paging to be done and <b>the existing hitlistid</b>.</item>
        ///   <item><see cref="COEDataView"/>: This parameter defines a particular view of the database and the search will be performed
        ///   over that view.</item>
        /// </list>
        /// </summary>
        /// <param name="resultsCriteria">Desired resulting fields and tables.</param>
        /// <param name="pagingInfo">Desired paging and <b>the hitlistid</b>.</param>
        /// <param name="dataView">A database view identified by ID</param>
        /// <param name="useRealTableNames">string that specifies if real table names are supposed to be returned (yes/true), if table names in the form Table_tableid (no/false) or if this is up o the search service (null/empty)</param>
        /// <param name="dataViewID">Data view ID</param>
        /// <param name="orderByCriteria">Client ordering criteria</param>
        /// <returns>The resulting dataset.</returns>
        public DataSet GetData(ResultsCriteria resultsCriteria, PagingInfo pagingInfo, int dataViewID, string useRealTableNames, OrderByCriteria orderByCriteria)
        {
            try
            {
                return GetData(resultsCriteria, pagingInfo, GetDataViewFromID(dataViewID), GetConnStringType(dataViewID), useRealTableNames, orderByCriteria, null);
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// <para>Search method that will get the data for an existing HitList.</para>
        /// <para>Three parameters are needed:</para>
        /// <list type="bullet">
        ///   <item><see cref="ResultsCriteria"/>: This parameter defines the desired result fields.</item>
        ///   <item><see cref="PagingInfo"/>: This parameter defines the paging to be done and <b>the existing hitlistid</b>.</item>
        ///   <item><see cref="COEDataView"/>: This parameter defines a particular view of the database and the search will be performed
        ///   over that view.</item>
        /// </list>
        /// </summary>
        /// <param name="resultsCriteria">Desired resulting fields and tables.</param>
        /// <param name="pagingInfo">Desired paging and <b>the hitlistid</b>.</param>
        /// <param name="dataView">A database view.</param>
        /// <returns>The resulting dataset.</returns>
        public DataSet GetData(ResultsCriteria resultsCriteria, PagingInfo pagingInfo, COEDataView dataView)
        {
            try
            {
                this.ApplyDataviewHandling(ref dataView);
                return GetData(resultsCriteria, pagingInfo, dataView, GetConnStringType(dataView), null, new OrderByCriteria(), null);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// <para>Search method that will get the data for an existing HitList.</para>
        /// <para>Three parameters are needed:</para>
        /// <list type="bullet">
        ///   <item><see cref="ResultsCriteria"/>: This parameter defines the desired result fields.</item>
        ///   <item><see cref="PagingInfo"/>: This parameter defines the paging to be done and <b>the existing hitlistid</b>.</item>
        ///   <item><see cref="COEDataView"/>: This parameter defines a particular view of the database and the search will be performed
        ///   over that view.</item>
        /// </list>
        /// </summary>
        /// <param name="resultsCriteria">Desired resulting fields and tables.</param>
        /// <param name="pagingInfo">Desired paging and <b>the hitlistid</b>.</param>
        /// <param name="dataView">A database view.</param>
        /// <param name="useRealTableNames">string that specifies if real table names are supposed to be returned (yes/true), if table names in the form Table_tableid (no/false) or if this is up o the search service (null/empty)</param>
        /// <returns>The resulting dataset.</returns>
        public DataSet GetData(ResultsCriteria resultsCriteria, PagingInfo pagingInfo, COEDataView dataView, string useRealTableNames) {
            try {
                this.ApplyDataviewHandling(ref dataView);
                return GetData(resultsCriteria, pagingInfo, dataView, GetConnStringType(dataView), useRealTableNames, new OrderByCriteria(), null);
            } catch(Exception ex) {
                throw;
            }
        }

        /// <summary>
        /// <para>Search method that will get the data for an existing HitList.</para>
        /// <para>Three parameters are needed:</para>
        /// <list type="bullet">
        ///   <item><see cref="ResultsCriteria"/>: This parameter defines the desired result fields.</item>
        ///   <item><see cref="PagingInfo"/>: This parameter defines the paging to be done and <b>the existing hitlistid</b>.</item>
        ///   <item><see cref="COEDataView"/>: This parameter defines a particular view of the database and the search will be performed
        ///   over that view.</item>
        /// </list>
        /// </summary>
        /// <param name="resultsCriteria">Desired resulting fields and tables.</param>
        /// <param name="pagingInfo">Desired paging and <b>the hitlistid</b>.</param>
        /// <param name="dataView">A database view.</param>
        /// <param name="useRealTableNames">string that specifies if real table names are supposed to be returned (yes/true), if table names in the form Table_tableid (no/false) or if this is up o the search service (null/empty)</param>
        /// <param name="orderByCriteria">Client ordering Criteria</param>
        /// <returns>The resulting dataset.</returns>
        public DataSet GetData(ResultsCriteria resultsCriteria, PagingInfo pagingInfo, COEDataView dataView, string useRealTableNames, OrderByCriteria orderByCriteria)
        {
            try
            {
                this.ApplyDataviewHandling(ref dataView);
                return GetData(resultsCriteria, pagingInfo, dataView, GetConnStringType(dataView), useRealTableNames, orderByCriteria, null);
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        public DataSet GetData(ResultsCriteria resultsCriteria, PagingInfo pagingInfo, COEDataView dataView, string useRealTableNames, OrderByCriteria orderByCriteria, SearchCriteria searchCriteria)
        {
            try
            {
                this.ApplyDataviewHandling(ref dataView);
                return GetData(resultsCriteria, pagingInfo, dataView, GetConnStringType(dataView), useRealTableNames, orderByCriteria, searchCriteria);
            }
            catch { throw; }
        }

        
        public DataSet GetData(
            ResultsCriteria resultsCriteria,
            PagingInfo pagingInfo,
            int dataViewID,
            string useRealTableNames,
            OrderByCriteria orderByCriteria,
            SearchCriteria searchCriteria
            )
        {
            COEDataView dataView = GetDataViewFromID(dataViewID);
            return GetData(resultsCriteria, pagingInfo, dataView, GetConnStringType(dataView), useRealTableNames, orderByCriteria, searchCriteria);
        }

        /// <summary>
        /// <para>
        /// Search method that will get the data for an existing HitList.
        /// </para>
        /// <para>
        /// Three parameters are needed:
        /// </para>
        /// <list type="bullet">
        /// <item>
        /// <see cref="ResultsCriteria"/>: This parameter defines the desired result fields.
        /// </item>
        /// <item>
        /// <see cref="PagingInfo"/>: This parameter defines the paging to be done and <b>the existing hitlistid</b>.
        /// </item>
        /// <item>
        /// <see cref="COEDataView"/>: This parameter defines a particular view of the database and the search will be performed
        ///   over that view.
        /// </item>
        /// </list>
        /// </summary>
        /// <param name="resultsCriteria">
        /// Desired resulting fields and tables.
        /// </param>
        /// <param name="pagingInfo">
        /// Desired paging and <b>the hitlistid</b>.
        /// </param>
        /// <param name="dataView">
        /// A database view.
        /// </param>
        /// <param name="connStringType">
        /// Connection string type
        /// </param>
        /// <param name="useRealTableNames">
        /// string that specifies if real table names are supposed to be returned (yes/true), if table names in the form
        /// Table_tableid (no/false) or if this is up o the search service (null/empty)
        /// </param>
        /// <param name="orderByCriteria">
        /// Client ordering criteria
        /// </param>
        /// <param name="searchCriteria">
        /// The search Criteria.
        /// </param>        
        /// <returns>
        /// The resulting dataset.
        /// </returns>
        private DataSet GetData(
            ResultsCriteria resultsCriteria,
            PagingInfo pagingInfo,
            COEDataView dataView,
            ConnStringType connStringType,
            string useRealTableNames,
            OrderByCriteria orderByCriteria,
            SearchCriteria searchCriteria
            )
        {
            //this is returned the DoSearchCommand object so you can call methods within it
            GetDataCommand result = DataPortal.Execute(new GetDataCommand(resultsCriteria, pagingInfo, dataView, connStringType, useRealTableNames, orderByCriteria, searchCriteria));
            return result.ResultsDataSet;
        }
        #endregion

        #region GetPartialHitList
        /// <summary>
        /// <para>Performs a general purpose search and retrieves the associated <see cref="HitListInfo"/>.</para>
        /// <para>Two parameters are needed:</para>
        /// <list type="bullet">
        ///   <item><see cref="SearchCriteria"/>: This parameter defines the restrictions of the search.</item>
        ///   <item><see cref="COEDataView"/>: This parameter defines a particular view of the database and the search will be performed
        ///   over that view.</item>
        /// </list>
        /// </summary>
        /// <param name="searchCriteria">Search restrictions.</param>
        /// <param name="dataViewID">A database view identified by ID</param>
        /// <returns>The associeted <see cref="HitListInfo"/>.</returns>
        public HitListInfo GetPartialHitList(SearchCriteria searchCriteria, int dataViewID)
        {
            try
            {
                return GetPartialHitList(searchCriteria, GetDataViewFromID(dataViewID), GetConnStringType(dataViewID), null);

            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// <para>Performs a general purpose search and retrieves the associated <see cref="HitListInfo"/>.</para>
        /// <para>Two parameters are needed:</para>
        /// <list type="bullet">
        ///   <item><see cref="SearchCriteria"/>: This parameter defines the restrictions of the search.</item>
        ///   <item><see cref="COEDataView"/>: This parameter defines a particular view of the database and the search will be performed
        ///   over that view.</item>
        /// </list>
        /// </summary>
        /// <param name="searchCriteria">Search restrictions.</param>
        /// <param name="dataViewID">A database view identified by ID</param>
        /// <param name="refineHitlist">The hitlist to refine over</param>
        /// <returns>The associeted <see cref="HitListInfo"/>.</returns>
        public HitListInfo GetPartialHitList(SearchCriteria searchCriteria, int dataViewID, HitListInfo refineHitlist)
        {
            try
            {
                return GetPartialHitList(searchCriteria, GetDataViewFromID(dataViewID), GetConnStringType(dataViewID), refineHitlist);

            }
            catch(Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// <para>Performs a general purpose search and retrieves the associated <see cref="HitListInfo"/>.</para>
        /// <para>Two parameters are needed:</para>
        /// <list type="bullet">
        ///   <item><see cref="SearchCriteria"/>: This parameter defines the restrictions of the search.</item>
        ///   <item><see cref="COEDataView"/>: This parameter defines a particular view of the database and the search will be performed
        ///   over that view.</item>
        /// </list>
        /// </summary>
        /// <param name="searchCriteria">Search restrictions.</param>
        /// <param name="dataViewID">A database view identified by ID</param>
        /// <returns>The associeted <see cref="HitListInfo"/>.</returns>
        public HitListInfo GetPartialHitList(SearchCriteria searchCriteria, int dataViewID, int commitSize) {
            try {
                return GetPartialHitList(searchCriteria, GetDataViewFromID(dataViewID), GetConnStringType(dataViewID), commitSize, null);

            } catch(Exception) {
                throw;
            }
        }

        /// <summary>
        /// <para>Performs a general purpose search and retrieves the associated <see cref="HitListInfo"/>.</para>
        /// <para>Two parameters are needed:</para>
        /// <list type="bullet">
        ///   <item><see cref="SearchCriteria"/>: This parameter defines the restrictions of the search.</item>
        ///   <item><see cref="COEDataView"/>: This parameter defines a particular view of the database and the search will be performed
        ///   over that view.</item>
        /// </list>
        /// </summary>
        /// <param name="searchCriteria">Search restrictions.</param>
        /// <param name="dataViewID">A database view identified by ID</param>
        /// <param name="refineHitlist">The hitlist to refine over</param>
        /// <returns>The associeted <see cref="HitListInfo"/>.</returns>
        public HitListInfo GetPartialHitList(SearchCriteria searchCriteria, int dataViewID, int commitSize, HitListInfo refineHitlist)
        {
            try
            {
                return GetPartialHitList(searchCriteria, GetDataViewFromID(dataViewID), GetConnStringType(dataViewID), commitSize, refineHitlist);

            }
            catch(Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// <para>Performs a general purpose search and retrieves the associated <see cref="HitListInfo"/>.</para>
        /// <para>Two parameters are needed:</para>
        /// <list type="bullet">
        ///   <item><see cref="SearchCriteria"/>: This parameter defines the restrictions of the search.</item>
        ///   <item><see cref="COEDataView"/>: This parameter defines a particular view of the database and the search will be performed
        ///   over that view.</item>
        /// </list>
        /// </summary>
        /// <param name="searchCriteria">Search restrictions.</param>
        /// <param name="dataView">A database view.</param>
        /// <returns>The associeted <see cref="HitListInfo"/>.</returns>
        public HitListInfo GetPartialHitList(SearchCriteria searchCriteria, COEDataView dataView)
        {
            try
            {
                this.ApplyDataviewHandling(ref dataView);
                return GetPartialHitList(searchCriteria, dataView, GetConnStringType(dataView), null);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// <para>Performs a general purpose search and retrieves the associated <see cref="HitListInfo"/>.</para>
        /// <para>Two parameters are needed:</para>
        /// <list type="bullet">
        ///   <item><see cref="SearchCriteria"/>: This parameter defines the restrictions of the search.</item>
        ///   <item><see cref="COEDataView"/>: This parameter defines a particular view of the database and the search will be performed
        ///   over that view.</item>
        /// </list>
        /// </summary>
        /// <param name="searchCriteria">Search restrictions.</param>
        /// <param name="dataView">A database view.</param>
        /// <param name="refineHitlist">The hitlist to refine over</param>
        /// <returns>The associeted <see cref="HitListInfo"/>.</returns>
        public HitListInfo GetPartialHitList(SearchCriteria searchCriteria, COEDataView dataView, HitListInfo refineHitlist)
        {
            try
            {
                this.ApplyDataviewHandling(ref dataView);
                return GetPartialHitList(searchCriteria, dataView, GetConnStringType(dataView), refineHitlist);
            }
            catch(Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// <para>Performs a general purpose search and retrieves the associated <see cref="HitListInfo"/>.</para>
        /// <para>Two parameters are needed:</para>
        /// <list type="bullet">
        ///   <item><see cref="SearchCriteria"/>: This parameter defines the restrictions of the search.</item>
        ///   <item><see cref="COEDataView"/>: This parameter defines a particular view of the database and the search will be performed
        ///   over that view.</item>
        /// </list>
        /// </summary>
        /// <param name="searchCriteria">Search restrictions.</param>
        /// <param name="dataView">A database view.</param>
        /// <returns>The associeted <see cref="HitListInfo"/>.</returns>
        public HitListInfo GetPartialHitList(SearchCriteria searchCriteria, COEDataView dataView, int commitSize) {
            try {
                this.ApplyDataviewHandling(ref dataView);
                return GetPartialHitList(searchCriteria, dataView, GetConnStringType(dataView), commitSize, null);

            } catch(Exception) {
                throw;
            }
        }

        /// <summary>
        /// <para>Performs a general purpose search and retrieves the associated <see cref="HitListInfo"/>.</para>
        /// <para>Two parameters are needed:</para>
        /// <list type="bullet">
        ///   <item><see cref="SearchCriteria"/>: This parameter defines the restrictions of the search.</item>
        ///   <item><see cref="COEDataView"/>: This parameter defines a particular view of the database and the search will be performed
        ///   over that view.</item>
        /// </list>
        /// </summary>
        /// <param name="searchCriteria">Search restrictions.</param>
        /// <param name="dataView">A database view.</param>
        /// <param name="refineHitlist">The hitlist to refine over</param>
        /// <returns>The associeted <see cref="HitListInfo"/>.</returns>
        public HitListInfo GetPartialHitList(SearchCriteria searchCriteria, COEDataView dataView, int commitSize, HitListInfo refineHitlist)
        {
            try
            {
                this.ApplyDataviewHandling(ref dataView);
                return GetPartialHitList(searchCriteria, dataView, GetConnStringType(dataView), commitSize, refineHitlist);

            }
            catch(Exception)
            {
                throw;
            }
        }
        
        private HitListInfo GetPartialHitList(SearchCriteria searchCriteria, COEDataView dataView, ConnStringType connStringType, HitListInfo refineHitlist) {
            try
            {
                GetHitListCommand result = DataPortal.Execute<GetHitListCommand>(new GetHitListCommand(searchCriteria, dataView, true, connStringType, refineHitlist));
                _lastInsertedSearchCriteriaID = result.SearchCriteriaID;
                searchCriteria.SearchCriteriaID = result.SearchCriteriaID;
                return (HitListInfo) result.HitListInfo;
            }
            catch(Exception)
            {
                throw;
            }
        }

        private HitListInfo GetPartialHitList(SearchCriteria searchCriteria, COEDataView dataView, ConnStringType connStringType, int commitSize, HitListInfo refineHitlist)
        {
            try {
                GetHitListCommand result = DataPortal.Execute<GetHitListCommand>(new GetHitListCommand(searchCriteria, dataView, true, connStringType, commitSize, _maxRecordCount, refineHitlist));
                _lastInsertedSearchCriteriaID = result.SearchCriteriaID;
                searchCriteria.SearchCriteriaID = result.SearchCriteriaID;
                return (HitListInfo) result.HitListInfo;
            } catch(Exception) {
                throw;
            }
        }
        #endregion

        #region GetGlobalSearchData
        /// <summary>
        /// <para>Retrieves a dataset on several dataviews based on several hitlist, one per dataview.</para>
        /// <para>Three parameters are needed:</para>
        /// <list type="bullet">
        ///   <item><see cref="ResultsCriteria"/>: This parameter defines the desired result fields.</item>
        ///   <item><see cref="PagingInfo"/>[]: This parameter defines the paging to be done and <b>the existing hitlistid</b> for each dataview.</item>
        ///   <item><see cref="COEDataView"/>[]: This parameter defines a list of particular view of the database and the search will be performed
        ///   over those views.</item>
        /// </list>
        /// </summary>
        /// <param name="resultsCriteria">Desired resulting fields and tables.</param>
        /// <param name="pagingInfo">Desired paging and <b>the merged hitlistid</b>.</param>
        /// <param name="dataView">Several database views.</param>
        /// <returns>An array with the resulting datasets.</returns>
        public DataSet[] GetGlobalSearchData(ResultsCriteria resultsCriteria, PagingInfo pagingInfo, COEDataView[] dataView)
        {
            try
            {
                //this.ApplyDataviewHandling(ref dataView);
                return GetGlobalSearchData(resultsCriteria, pagingInfo, dataView, ConnStringType.PROXY);
            }
            catch(Exception ex)
            {
                throw;
            }

        }


        private DataSet[] GetGlobalSearchData(ResultsCriteria resultsCriteria, PagingInfo pagingInfo, COEDataView[] dataViews, ConnStringType connStringType)
        {
            try
            {
                GetGlobalSearchDataCommand result;
                result = DataPortal.Execute<GetGlobalSearchDataCommand>(new GetGlobalSearchDataCommand(resultsCriteria, pagingInfo, dataViews, connStringType));
                return (DataSet[]) result.ResultsGlobalSearchDataSet;
            }
            catch(Exception)
            {
                throw;
            }
        }
        #endregion

        #region GetHitList

        /// <summary>
        /// <para>Performs a general purpose search and retrieves the associated <see cref="HitListInfo"/>.</para>
        /// <para>Two parameters are needed:</para>
        /// <list type="bullet">
        ///   <item><see cref="SearchCriteria"/>: This parameter defines the restrictions of the search.</item>
        ///   <item><see cref="COEDataView"/>: This parameter defines a particular view of the database and the search will be performed
        ///   over that view.</item>
        /// </list>
        /// </summary>
        /// <param name="searchCriteria">Search restrictions.</param>
        /// <param name="dataViewID">A database view identified by ID</param>
        /// <returns>The associeted <see cref="HitListInfo"/>.</returns>
        public HitListInfo GetHitList(SearchCriteria searchCriteria, int dataViewID)
        {
            try
            {
                return GetHitList(searchCriteria, GetDataViewFromID(dataViewID), GetConnStringType(dataViewID));

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        

        /// <summary>
        /// <para>Performs a general purpose search and retrieves the associated <see cref="HitListInfo"/>.</para>
        /// <para>Two parameters are needed:</para>
        /// <list type="bullet">
        ///   <item><see cref="SearchCriteria"/>: This parameter defines the restrictions of the search.</item>
        ///   <item><see cref="COEDataView"/>: This parameter defines a particular view of the database and the search will be performed
        ///   over that view.</item>
        /// </list>
        /// </summary>
        /// <param name="searchCriteria">Search restrictions.</param>
        /// <param name="dataViewID">A database view identified by ID</param>
        /// <param name="refineHitlist">The hitlist to refine over</param>
        /// <returns>The associeted <see cref="HitListInfo"/>.</returns>
        public HitListInfo GetHitList(SearchCriteria searchCriteria, int dataViewID, HitListInfo refineHitlist)
        {
            try
            {
                return GetHitList(searchCriteria, GetDataViewFromID(dataViewID), GetConnStringType(dataViewID), refineHitlist);
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// <para>Performs a general purpose search and retrieves the associated <see cref="HitListInfo"/>.</para>
        /// <para>Two parameters are needed:</para>
        /// <list type="bullet">
        ///   <item><see cref="SearchCriteria"/>: This parameter defines the restrictions of the search.</item>
        ///   <item><see cref="COEDataView"/>: This parameter defines a particular view of the database and the search will be performed
        ///   over that view.</item>
        /// </list>
        /// </summary>
        /// <param name="searchCriteria">Search restrictions.</param>
        /// <param name="dataView">A database view.</param>
        /// <returns>The associeted <see cref="HitListInfo"/>.</returns>
        public HitListInfo GetHitList(SearchCriteria searchCriteria, COEDataView dataView)
        {
            try
            {
                this.ApplyDataviewHandling(ref dataView);
                return GetHitList(searchCriteria, dataView, GetConnStringType(dataView));
                
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// <para>Performs a general purpose search and retrieves the associated <see cref="HitListInfo"/> using a previous hitlist to
        /// refine over.</para>
        /// <para>Two parameters are needed:</para>
        /// <list type="bullet">
        ///   <item><see cref="SearchCriteria"/>: This parameter defines the restrictions of the search.</item>
        ///   <item><see cref="COEDataView"/>: This parameter defines a particular view of the database and the search will be performed
        ///   over that view.</item>
        ///   <item><see cref="HitListInfo"/>: This parameter defines the hitlist to refine over.</item>
        /// </list>
        /// </summary>
        /// <param name="searchCriteria">Search restrictions.</param>
        /// <param name="dataView">A database view.</param>
        /// <param name="refineHitlist">The hitlist to refine over</param>
        /// <returns>The associeted <see cref="HitListInfo"/>.</returns>
        public HitListInfo GetHitList(SearchCriteria searchCriteria, COEDataView dataView, HitListInfo refineHitlist)
        {
            try
            {
                this.ApplyDataviewHandling(ref dataView);
                return GetHitList(searchCriteria, dataView, GetConnStringType(dataView), refineHitlist);

            }
            catch(Exception ex)
            {
                throw;
            }
        }

        private HitListInfo GetHitList(SearchCriteria searchCriteria, COEDataView dataView, ConnStringType connStringType) {
            try {
                return GetHitList(searchCriteria, dataView, connStringType, null);
            } catch(Exception ex) {
                throw;
            }
        }

        private HitListInfo GetHitList(SearchCriteria searchCriteria, COEDataView dataView, ConnStringType connStringType, HitListInfo refineHitList)
        {
            try
            {
                GetHitListCommand result = DataPortal.Execute<GetHitListCommand>(new GetHitListCommand(searchCriteria, dataView, connStringType, refineHitList));
                _lastInsertedSearchCriteriaID = result.SearchCriteriaID;
                return (HitListInfo) result.HitListInfo;
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        #endregion

        #region GetHitListProgress
        /// <summary>
        /// <para>Gets an updated <see cref="HitListInfo"/>.</para>
        /// </summary>
        /// <param name="hitListInfo">The original HitListInfo with its ID.</param>
        /// <returns>The updated <see cref="HitListInfo"/>.</returns>
        public HitListInfo GetHitListProgress(HitListInfo hitListInfo, int dataViewID)
        {
            try
            {
                return GetHitListProgress(hitListInfo, GetDataViewFromID(dataViewID), GetConnStringType(dataViewID));
               
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// <para>Gets an updated <see cref="HitListInfo"/>.</para>
        /// </summary>
        /// <param name="hitListInfo">The original HitListInfo with its ID.</param>
        /// <returns>The updated <see cref="HitListInfo"/>.</returns>
        public HitListInfo GetHitListProgress(HitListInfo hitListInfo, COEDataView dataView)
        {
            try
            {
                this.ApplyDataviewHandling(ref dataView);
                return GetHitListProgress(hitListInfo, dataView, GetConnStringType(dataView));
               
            }
            catch (Exception)
            {
                throw;
            }
        }
        private HitListInfo GetHitListProgress(HitListInfo hitListInfo, COEDataView dv, ConnStringType connStringType) {
            try {
                GetHitListProgressCommand result;
                //this is returned the DoSearchCommand object so you can call methods within it
                result = DataPortal.Execute<GetHitListProgressCommand>(new GetHitListProgressCommand(hitListInfo, dv.Database, connStringType));
                return (HitListInfo) result.HitListInfo;
            } catch(Exception) {
                throw;
            }
        }
        #endregion

        #region GetExactRecordCount
        /// <summary>
        /// <para>Retrieves the total row count for the base table of the given dataview.</para>
        /// <para>One parameter is needed:</para>
        /// <list type="bullet">
        ///   <item><see cref="COEDataView"/>: This parameter defines a particular view of the database and the search will be performed
        ///   over that view.</item>
        /// </list>
        /// </summary>
        /// <param name="dataView">A database view identified by ID</param>
        /// <returns>The exact row count.</returns>
        public int GetExactRecordCount(int dataViewID)
        {
            try
            {
               return GetExactRecordCount(GetDataViewFromID(dataViewID), GetConnStringType(dataViewID));
               
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// <para>Retrieves the total row count for the base table of the given dataview.</para>
        /// <para>One parameter is needed:</para>
        /// <list type="bullet">
        ///   <item><see cref="COEDataView"/>: This parameter defines a particular view of the database and the search will be performed
        ///   over that view.</item>
        /// </list>
        /// </summary>
        /// <param name="dataView">A database view.</param>
        /// <returns>The exact row count.</returns>
        public int GetExactRecordCount(COEDataView dataView)
        {
            try
            {
                this.ApplyDataviewHandling(ref dataView);
                return GetExactRecordCount(dataView, GetConnStringType(dataView));
               
            }
            catch (Exception)
            {
                throw;
            }
        }

        public string GetUserName(string name, int hits)
        {
            try
            {
                string sql = "SELECT HITS_ID FROM REGDB.HITSPERPAGE WHERE HITS_USER_ID = '" + name.ToUpper() +"'";
                return GetUserName(name, hits, sql);
            }
            catch (Exception)
            {
                throw;
            }

        }

        public int GetHitsPerPage(int ID)
        {
            try
            {
                string sql = "SELECT HITS FROM REGDB.HITSPERPAGE WHERE HITS_ID = '" + ID + "'";
                return GetHitsPerPage(ID, sql);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private int GetExactRecordCount(COEDataView dataView, ConnStringType connStringType) {
            try {
                GetExactRecordCountCommand result;
                //this is returned the GetRecordCountCommand object so you can call methods within it
                result = DataPortal.Execute<GetExactRecordCountCommand>(new GetExactRecordCountCommand(dataView, connStringType));
                return result.RecordCount;
            } catch(Exception) {
                throw;
            }
        }

        private string GetUserName(string name, int hits, string sql)
        {
            try
            {
                GetUserNameCommand result;
                result = DataPortal.Execute<GetUserNameCommand>(new GetUserNameCommand(name, hits, sql));
                return result.UserName;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private int GetHitsPerPage(int ID, string sql)
        {
            try
            {
                GetUserIDCommand result;
                result = DataPortal.Execute<GetUserIDCommand>(new GetUserIDCommand(ID, sql));
                return result.Hits;
            }
            catch (Exception)
            {
                throw;
            }

        }

        /// <summary>
        /// <para>Refreshes the user registry record count.</para>
        /// </summary>
        /// <param name="dataView">A database view.</param>
        /// <returns>The refreshed record count.</returns>
        public int RefreshDatabaseRecordCount(COEDataView dataView)
        {
            try
            {
                this.ApplyDataviewHandling(ref dataView);
                RefreshDatabaseRecordCountCommand result;
                //this is returned the RefreshDatabaseRecordCountCommand object so you can call methods within it
                result = DataPortal.Execute<RefreshDatabaseRecordCountCommand>(new RefreshDatabaseRecordCountCommand(dataView, GetConnStringType(dataView)));
                return result.RecordCount;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// <para>Retrieves an approximate total row count for the base table of the given dataview.</para>
        /// <para>One parameter is needed:</para>
        /// <list type="bullet">
        ///   <item><see cref="COEDataView"/>: This parameter defines a particular view of the database and the search will be performed
        ///   over that view.</item>
        /// </list>
        /// </summary>
        /// <param name="dataView">A database view.</param>
        /// <returns>The approximate row count.</returns>
        public int GetFastRecordCount(COEDataView dataView) {
            try {
                this.ApplyDataviewHandling(ref dataView);
                GetFastRecordCountCommand result;
                //this is returned the GetRecordCountCommand object so you can call methods within it
                result = DataPortal.Execute<GetFastRecordCountCommand>(new GetFastRecordCountCommand(dataView, GetConnStringType(dataView)));
                return result.RecordCount;
            } catch(Exception) {
                throw;
            }
        }

        /// <summary>
        /// <para>Retrieves an approximate total row count for the table of the given dataview.</para>
        /// <para>One parameter is needed:</para>
        /// <list type="bullet">
        ///   <item><see cref="COEDataView"/>: This parameter defines a particular view of the database and the search will be performed
        ///   over that view.</item>
        /// </list>
        /// </summary>
        /// <param name="dataView">A database view.</param>
        /// <param name="tableId">id of the table in dataview to get the record count</param>
        /// <returns>The approximate row count.</returns>
        public int GetFastRecordCount(COEDataView dataView, int tableId)
        {
            try
            {
                this.ApplyDataviewHandling(ref dataView);
                GetFastRecordCountCommand result;
                //this is returned the GetRecordCountCommand object so you can call methods within it
                result = DataPortal.Execute<GetFastRecordCountCommand>(new GetFastRecordCountCommand(dataView, tableId, GetConnStringType(dataView)));
                return result.RecordCount;
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion        

        #region DataPortal Classes/Methods

        [Serializable]
        private class DoSearchCommand : CommandBase {
            private SearchManager _searchManager;
            private string _serviceName = "COESearch";
            //put private variables that will be sent by the factory method. don't need properties
            private SearchCriteria _searchCriteria;
            private ResultsCriteria _resultsCriteria;
            private SecurityInfo _securityInfo;
            private PagingInfo _pagingInfo;
            private COEDataView _dataView;
            private SearchResponse _searchResponse;
            private bool _searchResults;
            private bool _partialSearch;
            private ConnStringType _connStringType;
            private int _commitSize;
            private string _useRealTableNames;
            private OrderByCriteria _orderByCriteria;

            public DoSearchCommand(SearchCriteria searchCriteria, ResultsCriteria resultsCriteria, PagingInfo pagingInfo, COEDataView dataView, ConnStringType connStringType)
            {
                _searchCriteria = searchCriteria;
                _resultsCriteria = resultsCriteria;
                _pagingInfo = pagingInfo;
                _dataView = dataView;
                _partialSearch = false;
                _connStringType = connStringType;
                _commitSize = -1;
                _useRealTableNames = string.Empty;
            }

            public DoSearchCommand(SearchCriteria searchCriteria, ResultsCriteria resultsCriteria, PagingInfo pagingInfo, COEDataView dataView, ConnStringType connStringType, string useRealTableNames) {
                _searchCriteria = searchCriteria;
                _resultsCriteria = resultsCriteria;
                _pagingInfo = pagingInfo;
                _dataView = dataView;
                _partialSearch = false;
                _connStringType = connStringType;
                _commitSize = -1;
                _useRealTableNames = useRealTableNames;
            }

            public DoSearchCommand(SearchCriteria searchCriteria, ResultsCriteria resultsCriteria, PagingInfo pagingInfo, COEDataView dataView, ConnStringType connStringType, string useRealTableNames, OrderByCriteria orderByCriteria)
            {
                _searchCriteria = searchCriteria;
                _resultsCriteria = resultsCriteria;
                _pagingInfo = pagingInfo;
                _dataView = dataView;
                _partialSearch = false;
                _connStringType = connStringType;
                _commitSize = -1;
                _useRealTableNames = useRealTableNames;
                _orderByCriteria = orderByCriteria;
            }
            
            public DoSearchCommand(SearchCriteria searchCriteria, ResultsCriteria resultsCriteria, PagingInfo pagingInfo, COEDataView dataView, bool partialSearch, ConnStringType connStringType)
            {
                _searchCriteria = searchCriteria;
                _resultsCriteria = resultsCriteria;
                _pagingInfo = pagingInfo;
                _dataView = dataView;
                _partialSearch = partialSearch;
                _connStringType = connStringType;
                _commitSize = -1;
                _useRealTableNames = string.Empty;
            }

            public DoSearchCommand(SearchCriteria searchCriteria, ResultsCriteria resultsCriteria, PagingInfo pagingInfo, COEDataView dataView, bool partialSearch, ConnStringType connStringType, int commitSize) {
                _searchCriteria = searchCriteria;
                _resultsCriteria = resultsCriteria;
                _pagingInfo = pagingInfo;
                _dataView = dataView;
                _partialSearch = partialSearch;
                _connStringType = connStringType;
                _commitSize = commitSize;
                _useRealTableNames = string.Empty;
            }
            
            public DoSearchCommand(SearchCriteria searchCriteria, ResultsCriteria resultsCriteria, PagingInfo pagingInfo, COEDataView dataView, bool partialSearch, ConnStringType connStringType, int commitSize, OrderByCriteria orderByCriteria)
            {
                _searchCriteria = searchCriteria;
                _resultsCriteria = resultsCriteria;
                _pagingInfo = pagingInfo;
                _dataView = dataView;
                _partialSearch = partialSearch;
                _connStringType = connStringType;
                _commitSize = commitSize;
                _useRealTableNames = string.Empty;
                _orderByCriteria = orderByCriteria;
            }

            /// <param name="useRealTableNames">string that specifies if real table names are supposed to be returned (yes/true), if table names in the form Table_tableid (no/false) or if this is up o the search service (null/empty)</param>
            public DoSearchCommand(SearchCriteria searchCriteria, ResultsCriteria resultsCriteria, PagingInfo pagingInfo, COEDataView dataView, bool partialSearch, ConnStringType connStringType, int commitSize, string useRealTableNames) {
                _searchCriteria = searchCriteria;
                _resultsCriteria = resultsCriteria;
                _pagingInfo = pagingInfo;
                _dataView = dataView;
                _partialSearch = partialSearch;
                _connStringType = connStringType;
                _commitSize = commitSize;
                _useRealTableNames = useRealTableNames;
            }

            public DoSearchCommand(SearchCriteria searchCriteria, ResultsCriteria resultsCriteria, PagingInfo pagingInfo, COEDataView dataView, bool partialSearch, ConnStringType connStringType, int commitSize, string useRealTableNames, OrderByCriteria orderByCriteria)
            {
                _searchCriteria = searchCriteria;
                _resultsCriteria = resultsCriteria;
                _pagingInfo = pagingInfo;
                _dataView = dataView;
                _partialSearch = partialSearch;
                _connStringType = connStringType;
                _commitSize = commitSize;
                _useRealTableNames = useRealTableNames;
                _orderByCriteria = orderByCriteria;
            }

            public SearchResponse SearchResponse {
                get { return _searchResponse; }
                set { _searchResponse = value; }
            }

            public int SearchCriteriaID
            {
                get
                {
                    if (_searchCriteria != null)
                    {
                        return _searchCriteria.SearchCriteriaID;
                    }
                    else
                    {
                        return 0;
                    }
                }
            }

            [Transactional(TransactionalTypes.Manual)]
            protected override void DataPortal_Execute() {
                try {
                    SearchManager searchManager = new SearchManager();
                    if(_partialSearch) {
                        if(_commitSize > 0)
                            _searchResponse = searchManager.DoPartialSearch(ref _searchCriteria, _resultsCriteria, _pagingInfo, _dataView, _connStringType, _commitSize, _useRealTableNames, _orderByCriteria);
                        else
                            _searchResponse = searchManager.DoPartialSearch(ref _searchCriteria, _resultsCriteria, _pagingInfo, _dataView, _connStringType, _useRealTableNames, _orderByCriteria);
                    } else
                        _searchResponse = searchManager.DoSearch(ref _searchCriteria, _resultsCriteria, _pagingInfo, _dataView, _connStringType, _useRealTableNames, _orderByCriteria);
                } catch(Exception) {
                    throw;
                }
            }
        }

        [Serializable]
        private class GetHitListCommand : CommandBase {
            private SearchManager _searchManager;
            private string _appName = string.Empty;
            private string _serviceName = "COESearch";
            //put private variables that will be sent by the factory method. don't need properties
            private SearchCriteria _searchCriteria;
            private ResultsCriteria _resultsCriteria;
            private SecurityInfo _securityInfo;
            private PagingInfo _pagingInfo;
            private COEDataView _dataView;
            private HitListInfo _hitListInfo;
            private bool _searchResults;
            private bool _partialHitList;
            private ConnStringType _connStringType;
            private int _commitSize;
            private int _maxRecordCount;
            private HitListInfo _refineHitList;
            

            public GetHitListCommand(SearchCriteria searchCriteria, COEDataView dataView, ConnStringType connStringType)
            {
                _searchCriteria = searchCriteria;
                _dataView = dataView;
                _partialHitList = false;
                _connStringType = connStringType;
                _commitSize = -1;
                _maxRecordCount = -1;
                _refineHitList = null;
            }            

            public GetHitListCommand(SearchCriteria searchCriteria, COEDataView dataView, ConnStringType connStringType, HitListInfo refineHitlist)
                : this(searchCriteria, dataView, connStringType)
            {
                _refineHitList = refineHitlist;
            }
			
            public GetHitListCommand(SearchCriteria searchCriteria, COEDataView dataView, bool partialHitList, ConnStringType connStringType)
                : this(searchCriteria, dataView, connStringType)
            {
                _partialHitList = partialHitList;
            }

            public GetHitListCommand(SearchCriteria searchCriteria, COEDataView dataView, bool partialHitList, ConnStringType connStringType, HitListInfo refineHitlist)
                : this(searchCriteria, dataView, connStringType, refineHitlist)
            {
                _partialHitList = partialHitList;
            }

            public GetHitListCommand(SearchCriteria searchCriteria, COEDataView dataView, bool partialHitList, ConnStringType connStringType, int commitSize, int maxRecordCount)
                : this(searchCriteria, dataView, partialHitList, connStringType)
            {
                _commitSize = commitSize;
                _maxRecordCount = maxRecordCount;
            }

            public GetHitListCommand(SearchCriteria searchCriteria, COEDataView dataView, bool partialHitList, ConnStringType connStringType, int commitSize, int maxRecordCount, HitListInfo refineHitlist)
                : this(searchCriteria, dataView, partialHitList, connStringType, commitSize, maxRecordCount)
            {
                _refineHitList = refineHitlist;
            }

            public HitListInfo HitListInfo {
                get { return _hitListInfo; }
                set { _hitListInfo = value; }
            }

            public int SearchCriteriaID {
                get {
                    return _searchCriteria.SearchCriteriaID;
                }
            }
            
            [Transactional(TransactionalTypes.Manual)]
            protected override void DataPortal_Execute() {
                try
                {
                    SearchManager searchManager = new SearchManager();
                    searchManager.MaxRecordCount = _maxRecordCount;
                    //NOTE:  for performing the search we may need to use OwnerProxy otherwise the user
                    //needs permissions that only coedb has
                    _connStringType = ConnStringType.OWNERPROXY;
                    if (_partialHitList)
                    {
                        if (_commitSize > 0)
                        {
                            _hitListInfo = searchManager.GetPartialHitList(ref _searchCriteria, _dataView, _connStringType, _commitSize, _refineHitList);
                        }
                        else
                        {
                            _hitListInfo = searchManager.GetPartialHitList(ref _searchCriteria, _dataView, _connStringType, _refineHitList);
                        }
                    }
                    else
                    {
                        _hitListInfo = searchManager.GetHitList(
                           ref _searchCriteria, _dataView, _connStringType, _refineHitList);
                    }
                }
                catch (OracleException ex)
                {
                    throw new Exception(ex.Message);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }

        [Serializable]
        private class GetDataCommand : CommandBase {
            private SearchManager _searchManager;
            private string _serviceName = "COESearch";
            //put private variables that will be sent by the factory method. don't need properties
            private ResultsCriteria _resultsCriteria;
            private SecurityInfo _securityInfo;
            private PagingInfo _pagingInfo;
            private COEDataView _dataView;
            private DataSet _resultsDataSet;
            private bool _searchResults;
            private ConnStringType _connStringType;
            private string _useRealTableNames;
            private OrderByCriteria _orderByCriteria;
            private SearchCriteria _searchCriteria;
            
            

            public GetDataCommand(ResultsCriteria resultsCriteria, PagingInfo pagingInfo, COEDataView dataView,ConnStringType connStringType) {
                _resultsCriteria = resultsCriteria;
                _pagingInfo = pagingInfo;
                _dataView = dataView;
                _connStringType = connStringType;
                _useRealTableNames = string.Empty;
            }

            public GetDataCommand(ResultsCriteria resultsCriteria, PagingInfo pagingInfo, COEDataView dataView, ConnStringType connStringType, string useRealTableNames) {
                _resultsCriteria = resultsCriteria;
                _pagingInfo = pagingInfo;
                _dataView = dataView;
                _connStringType = connStringType;
                _useRealTableNames = useRealTableNames;
            }

            public GetDataCommand(ResultsCriteria resultsCriteria, PagingInfo pagingInfo, COEDataView dataView, ConnStringType connStringType, string useRealTableNames, OrderByCriteria orderByCriteria)
            {
                _resultsCriteria = resultsCriteria;
                _pagingInfo = pagingInfo;
                _dataView = dataView;
                _connStringType = connStringType;
                _useRealTableNames = useRealTableNames;
                _orderByCriteria = orderByCriteria;
            }

            public GetDataCommand(
                ResultsCriteria resultsCriteria,
                PagingInfo pagingInfo,
                COEDataView dataView,
                ConnStringType connStringType,
                string useRealTableNames,
                OrderByCriteria orderByCriteria,
                SearchCriteria searchCriteria
                )
            {
                _resultsCriteria = resultsCriteria;
                _pagingInfo = pagingInfo;
                _dataView = dataView;
                _connStringType = connStringType;
                _useRealTableNames = useRealTableNames;
                _orderByCriteria = orderByCriteria;
                _searchCriteria = searchCriteria;               
            }

            public DataSet ResultsDataSet {
                get { return _resultsDataSet; }
                set { _resultsDataSet = value; }
            }

            protected override void DataPortal_Execute()
            {
                try
                {
                    SearchManager searchManager = new SearchManager();
                    _resultsDataSet = searchManager.GetData(_resultsCriteria, _pagingInfo, _dataView, _connStringType, _useRealTableNames, _orderByCriteria, _searchCriteria);
                }
                catch (OracleException ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }

        [Serializable]
        private class GetGlobalSearchDataCommand : CommandBase
        {
            private SearchManager _searchManager;
            private string _serviceName = "COESearch";
            //put private variables that will be sent by the factory method. don't need properties

            private ResultsCriteria _resultsCriteria;
            private PagingInfo _pagingInfo;
            private COEDataView[] _dataView;
            private DataSet[] _resultsGlobalSearchDataSet;
            private ConnStringType _connStringType;

            public GetGlobalSearchDataCommand(ResultsCriteria resultsCriteria, PagingInfo pagingInfo, COEDataView[] dataView, ConnStringType connStringType)
            {
                _resultsCriteria = resultsCriteria;
                _pagingInfo = pagingInfo;
                _dataView = dataView;
                _connStringType = connStringType;

            }

            public DataSet[] ResultsGlobalSearchDataSet
            {
                get { return _resultsGlobalSearchDataSet; }
                set { _resultsGlobalSearchDataSet = value; }
            }

            protected override void DataPortal_Execute()
            {
                try
                {
                    SearchManager searchManager = new SearchManager();
                    _resultsGlobalSearchDataSet = searchManager.GetGlobalSearchData(_resultsCriteria, _pagingInfo, _dataView, _connStringType);
                }
                catch(Exception)
                {
                    throw;
                }
            }
        }

        [Serializable]
        private class GetExactRecordCountCommand : CommandBase {
            //what is returned from the command
            private int _recordCount;
            private COEDataView _dataView;
            private ConnStringType _connStringType;


            public GetExactRecordCountCommand(COEDataView dataView, ConnStringType connStringType)
            {
                //constructor to set properties that match input paramters
                _dataView = dataView;
                _connStringType =  connStringType;

            }

            public int RecordCount {
                get { return _recordCount; }
                set { _recordCount = value; }
            }

            protected override void DataPortal_Execute() {
                try {
                    SearchManager searchManager = new SearchManager();
                    _recordCount = searchManager.GetExactRecordCount(_dataView, _connStringType);
                } catch(Exception) {
                    throw;
                }
            }
        }

        [Serializable]
        private class GetUserNameCommand : CommandBase
        {
            private string _username;
            private string _name;
            private string _sql;
            private int _hits;
            public GetUserNameCommand( string name, int hits, string sql)
            {
                _name = name;
                _sql = sql;
                _hits = hits;
            }
            public string UserName {
                get { return _username; }
                set { _username = value; }
            }

            protected override void  DataPortal_Execute()
            {
 	         try {
                 SearchManager searchManager = new SearchManager();
                 _username = searchManager.GetUserName(_name, _hits, _sql);
             } catch(Exception) {
                 throw;
             }
            }            

        }

        [Serializable]
        private class GetUserIDCommand : CommandBase
        {
            private int _hits;
            private string _sql;
            private int _hitsID;
            public GetUserIDCommand(int hits_ID, string sql)
            {
               
                _sql = sql;
                _hitsID = hits_ID;
            }
            public int Hits
            {
                get { return _hits; }
                set { _hits = value; }
            }

            protected override void DataPortal_Execute()
            {
                try
                {
                    SearchManager searchManager = new SearchManager();
                    _hits = searchManager.GetUserID(_hitsID, _sql);
                }
                catch (Exception)
                {
                    throw;
                }
            }

        }

        [Serializable]
        private class GetFastRecordCountCommand : CommandBase {
            //what is returned from the command
            private int _recordCount;
            private int _tableId;
            private COEDataView _dataView;
            private ConnStringType _connStringType;


            public GetFastRecordCountCommand(COEDataView dataView, ConnStringType connStringType) {
                //constructor to set properties that match input paramters
                _dataView = dataView;
                _connStringType = connStringType;
                //assign base table id as default table id
                _tableId = _dataView.Basetable; 
            }

            public GetFastRecordCountCommand(COEDataView dataView, int tableId, ConnStringType connStringType)
            {
                //constructor to set properties that match input paramters
                _dataView = dataView;
                _connStringType = connStringType;
                _tableId = tableId;
            }

            public int RecordCount {
                get { return _recordCount; }
                set { _recordCount = value; }
            }

            protected override void DataPortal_Execute() {
                try {
                    SearchManager searchManager = new SearchManager();
                    HitListInfo info = new HitListInfo();
                    searchManager.GetFastRecordCount(info, _dataView, _tableId, _connStringType);
                    _recordCount = info.RecordCount;
                } catch(Exception) {
                    throw;
                }
            }
        }

        [Serializable]
        private class RefreshDatabaseRecordCountCommand : CommandBase
        {
            //what is returned from the command
            private int _recordCount;
            private int _tableId;
            private COEDataView _dataView;
            private ConnStringType _connStringType;


            public RefreshDatabaseRecordCountCommand(COEDataView dataView, ConnStringType connStringType)
            {
                //constructor to set properties that match input paramters
                _dataView = dataView;
                _connStringType = connStringType;
                //assign base table id as default table id
                _tableId = _dataView.Basetable; 
            }

            public int RecordCount
            {
                get { return _recordCount; }
                set { _recordCount = value; }
            }

            protected override void DataPortal_Execute()
            {
                try
                {
                    SearchManager searchManager = new SearchManager();
                    HitListInfo info = new HitListInfo();
                    searchManager.RefreshDatabaseRecordCount(info, _dataView, _tableId, _connStringType);
                    _recordCount = info.RecordCount;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        [Serializable]
        private class GetHitListProgressCommand : CommandBase {
            private HitListInfo _hitListInfo;
            private string _database;
            private ConnStringType _connStringType;

            public GetHitListProgressCommand(HitListInfo hitListInfo, string database, ConnStringType connStringType) {
                //constructor to set properties that match input paramters
                _hitListInfo = hitListInfo;
                _database = database;
                _connStringType = connStringType;
            }

            public HitListInfo HitListInfo {
                get { return _hitListInfo; }
                //set { _hitListInfo = value; }
            }

            [Transactional(TransactionalTypes.Manual)]
            protected override void DataPortal_Execute() {
                try {
                    SearchManager searchManager = new SearchManager();
                  
                    //NOTE:  for performing the search we may need to use OwnerProxy otherwise the user
                    //needs permissions that only coedb has
                    _connStringType = ConnStringType.OWNERPROXY;
                    _hitListInfo = searchManager.GetHitlistProgress(_hitListInfo, _database, _connStringType);
                } catch(Exception) {
                    throw;
                }
            }
        }

       

        #region dataView permissions

        /// <summary>
        /// Gets the ConnString Type based on the dataview being passed in
        /// </summary>
        /// <param name="dataView"></param>
        /// <returns>ConnStringType</returns>
        private ConnStringType GetConnStringType(COEDataView dataView)
        {

            if (dataView.DataViewHandling != COEDataView.DataViewHandlingOptions.USE_CLIENT_DATAVIEW && dataView.DataViewID > 0)
            {
                return ConnStringType.OWNERPROXY;
            }
            else
            {
                return ConnStringType.PROXY;
            }
        }

        /// <summary>
        /// Gets the ConnString Type based on the dataview being passed in
        /// </summary>
        /// <param name="dataView"></param>
        /// <returns>ConnStringType</returns>
        private ConnStringType GetConnStringType(int dataViewID)
        {
            return ConnStringType.OWNERPROXY;
        }

        /// <summary>
        /// gets the coedataview for a particular id  if the logged in user has permissions to the dataview
        /// </summary>
        /// <param name="dataViewID">valid id for stored dataview</param>
        /// <returns>coedataview</returns>
        private COEDataView GetDataViewFromID(int dataViewID)
        {

            COEDataViewBO coeDataViewBO = COEDataViewBO.Get(dataViewID);
            if (coeDataViewBO.COEDataView == null)
            {
                throw new Exception(Resources.DataViewPermissions);
            }
            else
            {
                return (COEDataView)coeDataViewBO.COEDataView;
            }
        }

        private void ApplyDataviewHandling(ref COEDataView dataView)
        {
            switch(dataView.DataViewHandling)
            {
                case COEDataView.DataViewHandlingOptions.USE_CLIENT_DATAVIEW:
                    break;
                case COEDataView.DataViewHandlingOptions.USE_SERVER_DATAVIEW:
                    dataView = this.GetDataViewFromID(dataView.DataViewID);
                    break;
                case COEDataView.DataViewHandlingOptions.MERGE_CLIENT_AND_SERVER_DATAVIEW:
                    // Enforce a no tables and rels in a merging dataview to avoid network traffic
                    //dataView.Tables = new COEDataView.DataViewTableList();
                    //dataView.Relationships = new List<COEDataView.Relationship>();
                    // Configure caching for better performance in the retrieval:
                    COEDataView serverDV = this.GetDataViewFromID(dataView.DataViewID);
                    serverDV.Basetable = dataView.Basetable;
                    serverDV.DataViewHandling = COEDataView.DataViewHandlingOptions.MERGE_CLIENT_AND_SERVER_DATAVIEW;
                    dataView = serverDV;
                    break;
            }
        }

        /// <summary>
        /// Uses the dataview service to determine if the incoming dataview has the same tables as the official stored dataview
        /// </summary>
        /// <param name="dataView"></param>
        /// <returns></returns>
        private bool DataViewsAreEquivalent(COEDataView dataView)
        {
            COEDataViewBO coeDataViewBO = COEDataViewBO.Get(dataView.DataViewID);
            return coeDataViewBO.DataViewAreEquivalent(dataView);
        }
        #endregion

        #region Simplified Access Methods
        /// <summary>
        /// Perform a query over data contained in a single DataView. For using this method, the constructor that receives a dataview id must be used.
        /// </summary>
        /// <remarks>
        /// <para>
        /// When a query is performed, a hit list is produced and cached on the server.  The result set is returned to the caller in pages.  The caller can determine how and when pages are generated and returned.
        /// </para>
        /// 
        /// <para>
        /// An initial call to the DoSearch() method defines: 1) the query criteria; 2) the contents of the result set; 3) the details of the initial page to be returned; and 4) whether partial results should be returned ahead of query completion.  The first page of the result set is returned by this call.
        /// </para>
        /// 
        /// <para>
        /// Additional pages of the result set can be retrived via calls to the GetDataPage() method.  Since only the hitlist is persisted on the server, the actual contents of each data page can be varied with each call to GetDataPage().  Pages can be returned out-of-sequence, but performance is best when they are returned in order.
        /// </para>
        /// 
        /// <para>
        /// The details for definining the query criteria and result set fields are covered in the corresponding object documentation.  The behavior of paging and partial results retrival is described below.
        /// For a "complete result set" the caller sends:
        /// </para>
        /// <code>
        /// SearchInput.ReturnPartialResults = false (or Null)
        /// ResultPageInfo.PageSize=100
        /// ResultPageInfo.Start=1
        /// </code>
        /// <para>
        /// The query will proceed to completion before any results are returned.  The requested page size (or a smaller page) will be returned to the caller upon completion of the query.  More pages can be returned with subsequent calls to GetDataPage() method.  The reasons for receiveing smaller than requested page size are:  1) query generated insufficient hits, 2) size of the resultset exceeds limits set on the server.  The limit can be configured differently for "narrow" resultsets (1 or 2 integer fields) than for "wide" multi field/fieldtype result sets.  The caller can ascertain why a smaller page size was returned by comparing the ResultSetInfo.TotalCount to the returned ResultPageInfo.PageSize.  ResultSetInfo.CurrentCount will always match ResultSetInfo.TotalCount.
        /// </para>
        /// <para>
        /// For a "partial result set" the caller sends:
        /// </para>
        /// <code>
        /// ResultPageInfo.PageSize=100
        /// ResultPageInfo.Start=1
        /// </code>
        /// <para>
        /// First page of results is returned as soon as available. The requested page size (or a smaller page) may be returned to the caller before completion of the query.   More pages can be returned with subsequent calls to GetResultPage() method, but the method may return "page not available" status.  A separate method can be called to obtain current count of results for a running query.  The reasons for receiveing smaller than requested page size are:  1) query generated insufficient hits, 2) size of the resultset exceeds limits set on the server, 3) results where returned before completion of the query.  The caller can ascertain why a smaller page size was returned by comparing the returned ResultSetInfo.TotalCount, ResultPageInfo.PageSize, and ResultSetInfo.CurrentCount.  A discrepancy between TotalCount and CurrentCount indicates that the query did not run to completion before the page was returned.  A discrepancy between ToalCount and PageSize indicates insufficient hits where found to fill the requested page.
        /// </para>
        /// <example>
        /// The following example shows a search against 
        /// <code language="C#">
        /// Search target = new COESearch(100);
        /// SearchInput searchInput = new SearchInput();
        /// searchInput.ReturnPartialResults = false;
        /// ResultPageInfo rpi = new ResultPageInfo();
        /// rpi.ResultSetID = 0;
        /// rpi.PageSize = pageSize;
        /// rpi.Start = start;
        /// rpi.End = start + pageSize - 1;
        /// 
        /// searchInput.FieldCriteria = new string[1] { "SUBSTANCE.STRUCTURE MOLWEIGHT 100-101" };
        /// 
        /// string[] resultFields = new string[] { "SUBSTANCE.ACX_ID", "SUBSTANCE.CSNUM", "STRUCTURE.MOL", "PRODUCT.CATALOGNUM", "PRODUCT.PRODNAME", "PRODUCT.SUPPLIERID", "PRODUCT.SUPPLIERNAME", "PRODUCT.PRODDESCRIP", "ACX_SYNONYM.NAME", "PACKAGE.PRODUCTID", "PACKAGE.SIZE", "PACKAGE.CONTAINER", "PACKAGE.PRICE", "PROPERTYLIST.PROPERTYNAME", "PROPERTYLIST.PROPERTYVALUE" };
        /// 
        /// DataResult dl = target.DoSearch(searchInput, resultFields, rpi);
        /// </code>
        /// </example>
        /// </remarks>
        /// <param name="searchInput"><see cref="SearchInput"/> The search input.</param>
        /// <param name="resultFields">
        /// A string array of full field names to be returned.
        /// Field names must be in the format of TABLENAME.FIELDNAME
        /// </param>
        /// <param name="resultPageInfo"><see cref="ResultPageInfo"/> The result page info.</param>
        /// <returns>
        /// Returns a <see cref="DataResult"/>DataResult object.
        /// </returns>
        public DataResult DoSearch(SearchInput searchInput, string[] resultFields, ResultPageInfo resultPageInfo) {
            return DoSearch(searchInput, resultFields, resultPageInfo, null);
        }

        /// <summary>
        /// Perform a query over data contained in a single DataView. For using this method, the constructor that receives a dataview id must be used.
        /// </summary>
        /// <remarks>
        /// <para>
        /// When a query is performed, a hit list is produced and cached on the server.  The result set is returned to the caller in pages.  The caller can determine how and when pages are generated and returned.
        /// </para>
        /// 
        /// <para>
        /// An initial call to the DoSearch() method defines: 1) the query criteria; 2) the contents of the result set; 3) the details of the initial page to be returned; and 4) whether partial results should be returned ahead of query completion.  The first page of the result set is returned by this call.
        /// </para>
        /// 
        /// <para>
        /// Additional pages of the result set can be retrived via calls to the GetDataPage() method.  Since only the hitlist is persisted on the server, the actual contents of each data page can be varied with each call to GetDataPage().  Pages can be returned out-of-sequence, but performance is best when they are returned in order.
        /// </para>
        /// 
        /// <para>
        /// The details for definining the query criteria and result set fields are covered in the corresponding object documentation.  The behavior of paging and partial results retrival is described below.
        /// For a "complete result set" the caller sends:
        /// </para>
        /// <code>
        /// SearchInput.ReturnPartialResults = false; //(or Null)
        /// ResultPageInfo.PageSize=100;
        /// ResultPageInfo.Start=1;
        /// </code>
        /// <para>
        /// The query will proceed to completion before any results are returned.  The requested page size (or a smaller page) will be returned to the caller upon completion of the query.  More pages can be returned with subsequent calls to GetDataPage() method.  The reasons for receiveing smaller than requested page size are:  1) query generated insufficient hits, 2) size of the resultset exceeds limits set on the server.  The limit can be configured differently for "narrow" resultsets (1 or 2 integer fields) than for "wide" multi field/fieldtype result sets.  The caller can ascertain why a smaller page size was returned by comparing the ResultSetInfo.TotalCount to the returned ResultPageInfo.PageSize.  ResultSetInfo.CurrentCount will always match ResultSetInfo.TotalCount.
        /// </para>
        /// <para>
        /// For a "partial result set" the caller sends:
        /// </para>
        /// <code>
        /// ResultPageInfo.PageSize=100
        /// ResultPageInfo.Start=1
        /// </code>
        /// <para>
        /// First page of results is returned as soon as available. The requested page size (or a smaller page) may be returned to the caller before completion of the query.   More pages can be returned with subsequent calls to GetResultPage() method, but the method may return "page not available" status.  A separate method can be called to obtain current count of results for a running query.  The reasons for receiveing smaller than requested page size are:  1) query generated insufficient hits, 2) size of the resultset exceeds limits set on the server, 3) results where returned before completion of the query.  The caller can ascertain why a smaller page size was returned by comparing the returned ResultSetInfo.TotalCount, ResultPageInfo.PageSize, and ResultSetInfo.CurrentCount.  A discrepancy between TotalCount and CurrentCount indicates that the query did not run to completion before the page was returned.  A discrepancy between ToalCount and PageSize indicates insufficient hits where found to fill the requested page.
        /// </para>
        /// <example>
        /// The following example shows a search against 
        /// <code language="C#">
        /// Search target = new COESearch(100);
        /// SearchInput searchInput = new SearchInput();
        /// searchInput.ReturnPartialResults = false;
        /// ResultPageInfo rpi = new ResultPageInfo();
        /// rpi.ResultSetID = 0;
        /// rpi.PageSize = pageSize;
        /// rpi.Start = start;
        /// rpi.End = start + pageSize - 1;
        /// 
        /// searchInput.FieldCriteria = new string[1] { "SUBSTANCE.STRUCTURE MOLWEIGHT 100-101" };
        /// 
        /// string[] resultFields = new string[] { "SUBSTANCE.ACX_ID", "SUBSTANCE.CSNUM", "STRUCTURE.MOL", "PRODUCT.CATALOGNUM", "PRODUCT.PRODNAME", "PRODUCT.SUPPLIERID", "PRODUCT.SUPPLIERNAME", "PRODUCT.PRODDESCRIP", "ACX_SYNONYM.NAME", "PACKAGE.PRODUCTID", "PACKAGE.SIZE", "PACKAGE.CONTAINER", "PACKAGE.PRICE", "PROPERTYLIST.PROPERTYNAME", "PROPERTYLIST.PROPERTYVALUE" };
        /// 
        /// DataResult dl = target.DoSearch(searchInput, resultFields, rpi, true);
        /// </code>
        /// </example>
        /// </remarks>
        /// <param name="searchInput"><see cref="SearchInput"/> The search input.</param>
        /// <param name="resultFields">
        /// A string array of full field names to be returned.
        /// Field names must be in the format of TABLENAME.FIELDNAME
        /// </param>
        /// <param name="resultPageInfo"><see cref="ResultPageInfo"/> The result page info.</param>
        /// <param name="useRealTableNames">string that specifies if real table names are supposed to be returned (yes/true), if table names in the form Table_tableid (no/false) or if this is up o the search service (null/empty)</param>
        /// <returns>
        /// Returns a <see cref="DataResult"/>DataResult object.
        /// </returns>
        public DataResult DoSearch(SearchInput searchInput, string[] resultFields, ResultPageInfo resultPageInfo, string useRealTableNames) {
            DataResult dataResult = new DataResult();
            try {
                //First thing needed will be a dataview from the id
                
                _dataViewBO = COEDataViewService.COEDataViewBO.Get(_dataViewID);
            } catch(Csla.DataPortalException ex) {
                dataResult.Status = "FAILURE: DataView with id " + _dataViewID + " was not found.\n" + ex.BusinessException.Message;
                return dataResult;
            }


            if((searchInput.FieldCriteria == null || searchInput.FieldCriteria.Length < 1) && (searchInput.Domain == null || searchInput.Domain.Length < 1)) {
                dataResult.Status = "FAILURE: No criteria and no domain entered. Please check your input and privide at least one of them.";
                return dataResult;
            }
            
            SearchCriteria searchCriteria = new SearchCriteria();
            try {
                searchCriteria = InputFieldsToSearchCriteria.GetSearchCriteria(searchInput.FieldCriteria, searchInput.SearchOptions, searchInput.Domain, searchInput.DomainFieldName, _dataViewBO.COEDataView);
                if(searchInput.AvoidHitList)
                    searchCriteria = null;
            } catch(Exception ex) {
                dataResult.Status = "FAILURE: Could not build the search criteria with the provided input.\n" + ex.Message;
                return dataResult;
            }

            if(resultPageInfo == null) {
                // To handle defaults
                resultPageInfo = new ResultPageInfo();
            }

            PagingInfo pagingInfo = new PagingInfo();
            pagingInfo.RecordCount = resultPageInfo.PageSize;
            pagingInfo.Start = resultPageInfo.Start;
            pagingInfo.End = resultPageInfo.End;


            //these will need to get looped and converted to results criteria
            //resultFields
            string[] fieldCriteriaWithoutSimilarity = searchInput.FieldCriteria;
            ResultsCriteria resultsCriteria = new ResultsCriteria();
            ResultsCriteria resultsCriteriaWithoutSimilarity = new ResultsCriteria();

            try {
                resultsCriteria = ResultFieldsToResultsCriteria.GetResultCriteria(resultFields, _dataViewBO.COEDataView);
                resultsCriteriaWithoutSimilarity = ResultFieldsToResultsCriteria.GetResultCriteria(resultFields, _dataViewBO.COEDataView);
                
                if(searchInput.ReturnSimilarityScores)
                    InputFieldsToSearchCriteria.AddSimilarityIfNecessary(searchInput.FieldCriteria, ref resultsCriteria, _dataViewBO.COEDataView);

            } catch(Exception ex) {
                dataResult.Status = "FAILURE: Could not build the result criteria with the provided result fields.\n" + ex.Message;
                return dataResult;
            }
            //resultpage info will need to be read and returned based on info of pagingInfo

            //this goes straight to the method call (DoSearch or DoParialSearch)
            //Search will return a SearchResponse object//which will need to get turned into a DataResult Object
            SearchResponse searchResponse = null;
            try {
                if(searchInput.ReturnPartialResults) {
                    searchResponse = this.DoPartialSearch(searchCriteria, resultsCriteria, pagingInfo, _dataViewID, useRealTableNames);
                } else {
                    searchResponse = this.DoSearch(searchCriteria, resultsCriteria, pagingInfo, _dataViewID, useRealTableNames);
                }
            } catch(Csla.DataPortalException ex) {
                if(ex.BusinessException.Message.Contains("\"SIMILARITY\": invalid identifier")) {
                    try {
                        if(searchInput.ReturnPartialResults) {
                            searchResponse = this.DoPartialSearch(searchCriteria, resultsCriteriaWithoutSimilarity, pagingInfo, _dataViewID, useRealTableNames);
                        } else {
                            searchResponse = this.DoSearch(searchCriteria, resultsCriteriaWithoutSimilarity, pagingInfo, _dataViewID, useRealTableNames);
                        }
                        dataResult.Status = "WARNING:  Similarity scores cannot be retried with this version of CS Cartridge.";
                    } catch (DataPortalException innerException) {
                        dataResult.Status = "FAILURE: The search failed.\n" + innerException.BusinessException.Message;
                        return dataResult;
                    }
                } else {
                    dataResult.Status = "FAILURE: The search failed.\n" + ex.BusinessException.Message;
                    return dataResult;
                }
            }
            //take search response and turn it into a dataResult
            int currentRecordCount = searchResponse.PagingInfo.RecordCount > searchResponse.HitListInfo.CurrentRecordCount ? searchResponse.HitListInfo.CurrentRecordCount : searchResponse.PagingInfo.RecordCount;
            ResultPageInfo rpi = new ResultPageInfo(searchResponse.PagingInfo.HitListID, searchResponse.PagingInfo.RecordCount, searchResponse.PagingInfo.Start, searchResponse.PagingInfo.End);
            ResultSetInfo rsi = new ResultSetInfo(searchResponse.HitListInfo.HitListID, 0, currentRecordCount, searchResponse.HitListInfo.RecordCount);


            dataResult.ResultSet = searchResponse.ResultsDataSet.GetXml();
            dataResult.resultPageInfo = rpi;
            if(string.IsNullOrEmpty(dataResult.Status))
                dataResult.Status = "SUCCESS";

            dataResult.resultSetInfo = rsi;


            return dataResult;
        }

        /// <summary>
        /// Perform a query over data contained in a single DataView. For using this method, the constructor that receives a dataview id must be used. This overload is intended to return only the primary key values that matches with the filter criterias.
        /// </summary>
        /// <remarks>
        /// <para>
        /// When a query is performed, a hit list is produced and cached on the server.  The result set is returned to the caller in pages.  The caller can determine how and when pages are generated and returned.
        /// </para>
        /// 
        /// <para>
        /// An initial call to this method defines: 1) the query criteria; 2) the primary key field tha would be the content of the result set; 3) the details of the initial page to be returned; and 4) whether partial results should be returned ahead of query completion.  The first page of the result set is returned by this call.
        /// </para>
        /// 
        /// <para>
        /// Additional pages of the result set can be retrived via calls to the GetDataPage() method.  Since only the hitlist is persisted on the server, the actual contents of each data page can be varied with each call to GetDataPage().  Pages can be returned out-of-sequence, but performance is best when they are returned in order.
        /// </para>
        /// 
        /// <para>
        /// The details for definining the query criteria and result set fields are covered in the corresponding object documentation.  The behavior of paging and partial results retrival is described below.
        /// For a "complete result set" the caller sends:
        /// </para>
        /// <code>
        /// SearchInput.ReturnPartialResults = false; //(or Null)
        /// ResultPageInfo.PageSize=100;
        /// ResultPageInfo.Start=1;
        /// </code>
        /// <para>
        /// The query will proceed to completion before any results are returned.  The requested page size (or a smaller page) will be returned to the caller upon completion of the query.  More pages can be returned with subsequent calls to GetDataPage() method.  The reasons for receiveing smaller than requested page size are:  1) query generated insufficient hits, 2) size of the resultset exceeds limits set on the server. The caller can ascertain why a smaller page size was returned by comparing the ResultSetInfo.TotalCount to the returned ResultPageInfo.PageSize.  ResultSetInfo.CurrentCount will always match ResultSetInfo.TotalCount.
        /// </para>
        /// <para>
        /// For a "partial result set" the caller sends:
        /// </para>
        /// <code>
        /// SearchInput.ReturnPartialResults = true;
        /// ResultPageInfo.PageSize=100;
        /// ResultPageInfo.Start=1;
        /// </code>
        /// <para>
        /// First page of results is returned as soon as available. The requested page size (or a smaller page) may be returned to the caller before completion of the query.   More pages can be returned with subsequent calls to GetResultPage() method, but the method may return "page not available" status.  A separate method can be called to obtain current count of results for a running query.  The reasons for receiveing smaller than requested page size are:  1) query generated insufficient hits, 2) size of the resultset exceeds limits set on the server, 3) results where returned before completion of the query.  The caller can ascertain why a smaller page size was returned by comparing the returned ResultSetInfo.TotalCount, ResultPageInfo.PageSize, and ResultSetInfo.CurrentCount.  A discrepancy between TotalCount and CurrentCount indicates that the query did not run to completion before the page was returned.  A discrepancy between ToalCount and PageSize indicates insufficient hits where found to fill the requested page.
        /// </para>
        /// <example>
        /// The following example shows a search against ACX database.
        /// <code language="C#">
        /// Search target = new COESearch(100);
        /// SearchInput searchInput = new SearchInput();
        /// searchInput.ReturnPartialResults = false;
        /// ResultPageInfo rpi = new ResultPageInfo();
        /// rpi.ResultSetID = 0;
        /// rpi.PageSize = pageSize;
        /// rpi.Start = start;
        /// rpi.End = start + pageSize - 1;
        /// 
        /// searchInput.FieldCriteria = new string[1] { "SUBSTANCE.STRUCTURE MOLWEIGHT 100-101" };
        /// 
        /// 
        /// DataListResult dlr = target.DoSearch(searchInput, "SUBSTANCE.ACX_ID", rpi);
        /// </code>
        /// </example>
        /// </remarks>
        /// <param name="searchInput"><see cref="SearchInput"/> The search input.</param>
        /// <param name="pkField">The primary key, which is the field to be retrieved in the DataListResult</param>
        /// <param name="resultPageInfo"><see cref="ResultPageInfo"/> The result page info.</param>
        /// <returns>
        /// Returns a <see cref="DataListResult"/>DataListResult object.
        /// </returns>
        public DataListResult DoSearch(SearchInput searchInput, string pkField, ResultPageInfo resultPageInfo) {
            return DoSearch(searchInput, pkField, resultPageInfo, null);
        }

        /// <summary>
        /// Perform a query over data contained in a single DataView. For using this method, the constructor that receives a dataview id must be used. This overload is intended to return only the primary key values that matches with the filter criterias.
        /// </summary>
        /// <remarks>
        /// <para>
        /// When a query is performed, a hit list is produced and cached on the server.  The result set is returned to the caller in pages.  The caller can determine how and when pages are generated and returned.
        /// </para>
        /// 
        /// <para>
        /// An initial call to this method defines: 1) the query criteria; 2) the primary key field tha would be the content of the result set; 3) the details of the initial page to be returned; and 4) whether partial results should be returned ahead of query completion.  The first page of the result set is returned by this call.
        /// </para>
        /// 
        /// <para>
        /// Additional pages of the result set can be retrived via calls to the GetDataPage() method.  Since only the hitlist is persisted on the server, the actual contents of each data page can be varied with each call to GetDataPage().  Pages can be returned out-of-sequence, but performance is best when they are returned in order.
        /// </para>
        /// 
        /// <para>
        /// The details for definining the query criteria and result set fields are covered in the corresponding object documentation.  The behavior of paging and partial results retrival is described below.
        /// For a "complete result set" the caller sends:
        /// </para>
        /// <code>
        /// SearchInput.ReturnPartialResults = false; //(or Null)
        /// ResultPageInfo.PageSize=100;
        /// ResultPageInfo.Start=1;
        /// </code>
        /// <para>
        /// The query will proceed to completion before any results are returned.  The requested page size (or a smaller page) will be returned to the caller upon completion of the query.  More pages can be returned with subsequent calls to GetDataPage() method.  The reasons for receiveing smaller than requested page size are:  1) query generated insufficient hits, 2) size of the resultset exceeds limits set on the server. The caller can ascertain why a smaller page size was returned by comparing the ResultSetInfo.TotalCount to the returned ResultPageInfo.PageSize.  ResultSetInfo.CurrentCount will always match ResultSetInfo.TotalCount.
        /// </para>
        /// <para>
        /// For a "partial result set" the caller sends:
        /// </para>
        /// <code>
        /// SearchInput.ReturnPartialResults = true;
        /// ResultPageInfo.PageSize=100;
        /// ResultPageInfo.Start=1;
        /// </code>
        /// <para>
        /// First page of results is returned as soon as available. The requested page size (or a smaller page) may be returned to the caller before completion of the query.   More pages can be returned with subsequent calls to GetResultPage() method, but the method may return "page not available" status.  A separate method can be called to obtain current count of results for a running query.  The reasons for receiveing smaller than requested page size are:  1) query generated insufficient hits, 2) size of the resultset exceeds limits set on the server, 3) results where returned before completion of the query.  The caller can ascertain why a smaller page size was returned by comparing the returned ResultSetInfo.TotalCount, ResultPageInfo.PageSize, and ResultSetInfo.CurrentCount.  A discrepancy between TotalCount and CurrentCount indicates that the query did not run to completion before the page was returned.  A discrepancy between ToalCount and PageSize indicates insufficient hits where found to fill the requested page.
        /// </para>
        /// <example>
        /// The following example shows a search against ACX database.
        /// <code language="C#">
        /// Search target = new COESearch(100);
        /// SearchInput searchInput = new SearchInput();
        /// searchInput.ReturnPartialResults = false;
        /// ResultPageInfo rpi = new ResultPageInfo();
        /// rpi.ResultSetID = 0;
        /// rpi.PageSize = pageSize;
        /// rpi.Start = start;
        /// rpi.End = start + pageSize - 1;
        /// 
        /// searchInput.FieldCriteria = new string[1] { "SUBSTANCE.STRUCTURE MOLWEIGHT 100-101" };
        /// 
        /// 
        /// DataListResult dlr = target.DoSearch(searchInput, "SUBSTANCE.ACX_ID", rpi, true);
        /// </code>
        /// </example>
        /// </remarks>
        /// <param name="searchInput"><see cref="SearchInput"/> The search input.</param>
        /// <param name="pkField">The primary key, which is the field to be retrieved in the DataListResult</param>
        /// <param name="resultPageInfo"><see cref="ResultPageInfo"/> The result page info.</param>
        /// /// <param name="useRealTableNames">string that specifies if real table names are supposed to be returned (yes/true), if table names in the form Table_tableid (no/false) or if this is up o the search service (null/empty)</param>
        /// <returns>
        /// Returns a <see cref="DataListResult"/>DataListResult object.
        /// </returns>
        public DataListResult DoSearch(SearchInput searchInput, string pkField, ResultPageInfo resultPageInfo, string useRealTableNames) {
            DataListResult dataListResult = new DataListResult();

            string[] resultFields = new string[] { pkField };
            try {
                DataResult dr = DoSearch(searchInput, resultFields, resultPageInfo, useRealTableNames);

                if(!(dr.Status == "SUCCESS")) {
                    dataListResult.Status = dr.Status;
                    if(dr.Status.StartsWith("FAILURE"))
                        return dataListResult;
                }

                dataListResult.resultPageInfo = dr.resultPageInfo;
                dataListResult.resultSetInfo = dr.resultSetInfo;

                DataSet ds = new DataSet();

                try {
                    //create a StringReader object to read our xml string
                    if(!string.IsNullOrEmpty(dr.ResultSet)) {
                        using(System.IO.StringReader stringReader = new System.IO.StringReader(dr.ResultSet)) {
                            //initialize our DataSet
                            ds = new DataSet();

                            //load the StringReader to our DataSet
                            ds.ReadXml(stringReader);
                        }
                    }
                } catch {
                    //return null
                    ds = null;
                }

                string[] results = new string[] { };
                List<string> similarityScores = new List<string>();
                if(ds != null && ds.Tables.Count > 0) {
                    Array.Resize(ref results, ds.Tables[0].Rows.Count);

                    for(int i = 0; i < ds.Tables[0].Rows.Count; i++) {
                        results.SetValue(ds.Tables[0].Rows[i][0].ToString(), i);
                        if(ds.Tables[0].Columns.Count > 1) {
                            similarityScores.Add(ds.Tables[0].Rows[i][1].ToString());
                        }
                    }

                }

                dataListResult.ResultList = results;
                dataListResult.SimilarityScores = similarityScores.ToArray();
                if(string.IsNullOrEmpty(dataListResult.Status))
                    dataListResult.Status = "SUCCESS";
            } catch(Csla.DataPortalException ex) {
                dataListResult.Status = "FAILURE: " + ex.BusinessException.Message;
            }
            return dataListResult;
        }

        /// <summary>
        /// Retrieves a page of results from the result set of a previous search.
        /// </summary>
        /// <param name="resultPageInfo"><see cref="ResultPageInfo"/> The result page info.</param>
        /// <param name="resultFields">
        /// A string array of full field names to be returned.
        /// Field names must be in the format of TABLENAME.FIELDNAME
        /// </param>
        /// <returns>
        ///		<see cref="DataResult"/> Returns a DataResult object. 
        /// </returns>
        public DataResult GetDataPage(ResultPageInfo resultPageInfo, string[] resultFields) {
            return GetDataPage(resultPageInfo, resultFields, null);
        }

        /// <summary>
        /// Retrieves a page of results from the result set of a previous search.
        /// </summary>
        /// <param name="resultPageInfo"><see cref="ResultPageInfo"/> The result page info.</param>
        /// <param name="resultFields">
        /// A string array of full field names to be returned.
        /// Field names must be in the format of TABLENAME.FIELDNAME
        /// </param>
        /// <param name="useRealTableNames">string that specifies if real table names are supposed to be returned (yes/true), if table names in the form Table_tableid (no/false) or if this is up o the search service (null/empty)</param>
        /// <returns>
        ///		<see cref="DataResult"/> Returns a DataResult object. 
        /// </returns>
        public DataResult GetDataPage(ResultPageInfo resultPageInfo, string[] resultFields, string useRealTableNames) {
            DataResult dataResult = new DataResult();

            try {
                //First thing needed will be a dataview from the id
                _dataViewBO = COEDataViewService.COEDataViewBO.Get(_dataViewID);
            } catch(Csla.DataPortalException ex) {
                dataResult.Status = "FAILURE: DataView with id " + _dataViewID + " was not found.\n" + ex.BusinessException.Message;
                return dataResult;
            }

            ResultsCriteria resultsCriteria = new ResultsCriteria();
            try {
                resultsCriteria = ResultFieldsToResultsCriteria.GetResultCriteria(resultFields, _dataViewBO.COEDataView);
            } catch(Exception ex) {
                dataResult.Status = "FAILURE: Could not build the result criteria with the provided result fields.\n" + ex.Message;
                return dataResult;
            }
            if(resultPageInfo == null) {
                dataResult.Status = "FAILURE: Result page info was not provided, and it is required for retrieving a page.";
                return dataResult;
            }
            PagingInfo pagingInfo = new PagingInfo();
            pagingInfo.RecordCount = resultPageInfo.PageSize;
            pagingInfo.Start = resultPageInfo.Start;
            pagingInfo.End = resultPageInfo.End;
            pagingInfo.HitListID = resultPageInfo.ResultSetID;

            dataResult.resultPageInfo = resultPageInfo;
            //DataSet ds = this.GetData(resultsCriteria, pagingInfo, _dataViewBO.COEDataView, useRealTableNames);
            DataSet ds = this.GetData(resultsCriteria, pagingInfo, _dataViewID , useRealTableNames);
            dataResult.ResultSet = ds.GetXml();
            int currentCount = 0;
            if(ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                currentCount = ds.Tables[0].Rows.Count;

            dataResult.resultSetInfo = new ResultSetInfo(resultPageInfo.ResultSetID, 0, currentCount, 0);
            if(string.IsNullOrEmpty(dataResult.Status))
            {
                dataResult.Status = "SUCCESS";
            }
            
            //            dataResult.resultSetInfo = new ResultSetInfo(_pagingInfo.HitListID, 0, _pagingInfo.
            return dataResult;
        }
        
        /// <summary>
        /// Retrieves information about a previously generated result set.  Used to check progress of an ongoing query.
        /// </summary>
        /// <param name="resultSetID">The handle to a previous result set.</param>
        /// 
        /// <returns>
        ///	<see cref="ResultSetInfo"/> Returns a ResultSetInfo object. Estimated Count would always be 0. Total Count would be 0 until
        /// the partial search is completely ready.
        /// </returns>
        public ResultSetInfo GetResultSetInfo(int resultSetID) {
            HitListInfo i = new HitListInfo();
            i.HitListID = resultSetID;
            
            try {
                //First thing needed will be a dataview from the id
                _dataViewBO = COEDataViewService.COEDataViewBO.Get(_dataViewID);
            } catch(Csla.DataPortalException ex) {
                throw new Exception("GetResultSetInfo Failes,  DataView with id " + _dataViewID + " was not found.\n" + ex.BusinessException.Message);
            }

            HitListInfo info = this.GetHitListProgress(i, _dataViewBO.COEDataView);
            ResultSetInfo resultSetInfo = new ResultSetInfo(resultSetID, 0, info.CurrentRecordCount, info.RecordCount);

            return resultSetInfo;
        }

        /// <summary>
        /// Updates a previous <see cref="ResultSetInfo"/>, filling in the CurrentRecordCount and the TotalCount if the search is complete.
        /// </summary>
        /// <param name="resultSetInfo">The previous result set info.</param>
        /// 
        /// <returns>
        ///		<see cref="ResultSetInfo"/> Returns a ResultSetInfo object. Estimated Count would always be 0. Total Count would be 0 until
        /// the partial search is completely ready.
        /// </returns>
        public ResultSetInfo GetUpdatedResultSetInfo(ResultSetInfo resultSetInfo) {
            HitListInfo i = new HitListInfo();
            i.HitListID = resultSetInfo.ID;
            i.RecordCount = resultSetInfo.TotalCount;
            i.CurrentRecordCount = resultSetInfo.CurrentCount;

            HitListInfo info = this.GetHitListProgress(i, _dataViewBO.COEDataView);
            resultSetInfo.CurrentCount = info.CurrentRecordCount;
            if(info.CurrentRecordCount == info.RecordCount) {
                resultSetInfo.TotalCount = info.RecordCount;
                resultSetInfo.IsComplete = true;
            }

            return resultSetInfo;
        }
        
        #endregion
        # endregion

    }
}