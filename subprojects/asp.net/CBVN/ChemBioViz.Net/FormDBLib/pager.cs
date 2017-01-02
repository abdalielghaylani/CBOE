using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using System.Collections.Generic;

using CambridgeSoft.COE.Framework;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.COEDataViewService;
using CambridgeSoft.COE.Framework.COESecurityService;
using CambridgeSoft.COE.Framework.COESearchService;
using CambridgeSoft.COE.Framework.COEHitListService;
using CambridgeSoft.COE.Framework.Common.Messaging;
using CambridgeSoft.COE.Framework.Caching;

using CBVUtilities;
using SharedLib;

namespace FormDBLib
{
    //---------------------------------------------------------------------
    public class Pager : IChemDataProvider
    {
        #region Enums
        public enum MoveType { kmFirst, kmNext, kmPrev, kmLast, kmCurr, kmGoto, kmGotoPageRow };
        #endregion

        #region Variables
        private int currRow, currRowInPage, rowsPerPage;
        private int m_listSize;
        private int m_hitListID;
        private HitListType m_hitListType;

        private FormDbMgr m_formDbMgr;
        private DataPage currPage;
        private PagingInfo m_pagingInfo;
        private SearchResponse m_searchResponse;
        private SearchCriteria m_searchCriteria;
        private List<DataPage> m_pages;
        #endregion

        #region Properties
        public HitListType _HitListType
        {
            get { return m_hitListType; }
            set { m_hitListType = value; }
        }
        //---------------------------------------------------------------------
        public int HitListID
        {
            get { return m_hitListID; }
            set { m_hitListID = value; }
        }
        //---------------------------------------------------------------------
        public int CurrRow
        {
            // this is the row most recently retrieved (as in grid fill), not necessarily the user-selected row
            get { return currRow; }
            set { currRow = value; }
        }
        //---------------------------------------------------------------------
        public int ListSize
        {
            get { return m_listSize; }
            set { m_listSize = value; }
        }
        //---------------------------------------------------------------------
        public int CurrRowInPage
        {
            get { return currRowInPage; }
            set { currRowInPage = value; }
        }
        //---------------------------------------------------------------------
        public DataPage CurrPage
        {
            get { return currPage; }
            set { currPage = value; }
        }
        //---------------------------------------------------------------------
        public DataSet CurrDataSet
        {
            get { return (currPage == null) ? null : currPage.DataSet; }
            set { currPage.DataSet = value; }
        }
        //---------------------------------------------------------------------
        public SearchResponse SearchResponse
        {
            get { return m_searchResponse; }
            set { m_searchResponse = value; }
        }
        //---------------------------------------------------------------------
        public int RowsPerPage
        {
            get { return rowsPerPage; }
            set { rowsPerPage = value; }
        }
        //---------------------------------------------------------------------
        #endregion

        #region Constructors
        public Pager(FormDbMgr formDbMgr)
        {
            m_pages = new List<DataPage>();
            rowsPerPage = CBVUtil.PageSize;
            m_listSize = 0;
            m_hitListID = 0;
            m_searchCriteria = null;
            m_formDbMgr = formDbMgr;
            Clear();
        }
        #endregion

        #region Methods
        public void Clear()
        {
            // remove all pages; required when list or RC changes
            currRow = currRowInPage = -1;
            currPage = null;
            m_pages.Clear();
        }
        //---------------------------------------------------------------------
        public DataPage FindPageContaining(int row)
        {
            foreach (DataPage page in m_pages)
            {
                if (page.ContainsRow(row))
                    return page;
            }
            return null;
        }
        //---------------------------------------------------------------------
        public static int GetMoveTargetRecno(MoveType how, int currRow, int listSize, int targetRec, int firstRowInPage)
        {
            int row = currRow;
            switch (how)
            {
                case MoveType.kmFirst: row = 0; break;
                case MoveType.kmNext: row += 1; break;
                case MoveType.kmPrev: row -= 1; break;
                case MoveType.kmLast: row = listSize - 1; break;
                case MoveType.kmGoto: row = targetRec; break;
                case MoveType.kmCurr: break;
                case MoveType.kmGotoPageRow:
                    row = firstRowInPage + targetRec; break;
            }
            if (row < 0) row = 0;
            if (row > listSize - 1) row = listSize - 1;
            return row;
        }
        //---------------------------------------------------------------------
        public bool CanMove(MoveType how)
        {
            // true if curr abs row is within bounds of full list
            // not valid for goto or gotoPageRow
            if (ListSize >= 0)
            {
                switch (how)
                {
                    case MoveType.kmFirst:
                    case MoveType.kmPrev: return currRow > 0;
                    case MoveType.kmLast:
                    case MoveType.kmNext: return currRow < (ListSize - 1);
                    case MoveType.kmCurr: return currRow != -1;
                }
            }
            return false;
        }
        //---------------------------------------------------------------------
        public void Move(MoveType how, int targetRecno)
        {
            int firstRow = (CurrPage == null) ? 0 : CurrPage.FirstRec;
            int row = GetMoveTargetRecno(how, currRow, ListSize, targetRecno, firstRow);
            MoveTo(row);
        }
        //---------------------------------------------------------------------
        public bool MoveTo(int row)
        {
            // position current pointers to given row; fetch new page if needed

            DataPage oldCurr = currPage;
            if (row >= 0 && (ListSize == 0 || row <= (ListSize - 1)))
            {
                if (currPage != null && !currPage.ContainsRow(row))
                {
                    currPage = FindPageContaining(row);
                }
                if (currPage == null)
                {
                    // CSBR-154473: We used to warn of big moves here,
                    //  possibly returning false.
                    // fetch page from server
                    currPage = FetchNewPage(row);
                    if (currPage == null)
                    {
                        currPage = oldCurr;
                        row = (currPage == null) ? 0 : currPage.FirstRec;
                        //throw new Exception("Fetch page failed"); .. nah, nobody catches this
                    }
                }
                currRow = row;
                currRowInPage = (currPage == null) ? 0 : row - currPage.FirstRec;
            }
            return true;
        }
        //---------------------------------------------------------------------
        public void AddSinglePage(DataSet dataSet)
        {
            // used for cached dataset
            DataPage dataPage = new DataPage(this);
            dataPage.FirstRec = 0;
            dataPage.LastRec = dataSet.Tables[0].Rows.Count - 1;
            dataPage.DataSet = dataSet;
            m_pages.Add(currPage);
            currPage = dataPage;
        }
        //---------------------------------------------------------------------
        private void SetHitlistData(int hitlistID, int nHits, HitListType hlType)
        {
            // set internal variables from incoming data
            HitListID = m_pagingInfo.HitListID = hitlistID;
            ListSize = m_pagingInfo.RecordCount = nHits;
            _HitListType = hlType;
        }
        //---------------------------------------------------------------------
        private void ProcessIncomingSearchResponse(SearchResponse sResponse)
        {
            // create page from incoming searchResponse and set hitlist info
            SetHitlistData(sResponse.HitListInfo.HitListID, sResponse.HitListInfo.RecordCount, sResponse.HitListInfo.HitListType);
            ProcessIncomingDataSet(sResponse.ResultsDataSet);
        }
        //---------------------------------------------------------------------
        private void ProcessIncomingDataSet(DataSet dataSet)
        {
            // create page from incoming dataset
            if (dataSet == null || dataSet.Tables == null || dataSet.Tables[0].Rows.Count == 0)
                throw new FormDBLib.Exceptions.NoHitsException();

            int nFirst = 0, nLast = this.RowsPerPage - 1;
            DataPage dataPage = MakeDataPage(nFirst, nLast, dataSet);
            currPage = dataPage;
            m_pages.Add(dataPage);

#if DEBUG
            bool bDumpPage = false;
            if (bDumpPage && dataPage != null)
            {
                FormDBLib.FormDbMgr.DebugDatasetNoXml(dataPage.DataSet);

                String fname = "C:\\dset_out.xml";
                dataPage.DataSet.WriteXml(fname, XmlWriteMode.WriteSchema);
                bDumpPage = false;  // dump only the first
            }
#endif
        }
        //---------------------------------------------------------------------
        private SearchResponse SearchWithDomain(COESearch coeSearch, SearchCriteria searchCriteria, HitListInfo domainHLInfo)
        {
            SearchResponse sResponse = new SearchResponse();

            HitListInfo hlResult = coeSearch.GetHitList(searchCriteria, m_formDbMgr.SelectedDataView, domainHLInfo);
            Debug.WriteLine(String.Format("Returned from domain search over hl={0}/hits={1}, result hl={2}/hits={3}",
                domainHLInfo.HitListID, domainHLInfo.RecordCount, hlResult.HitListID, hlResult.RecordCount));

            m_pagingInfo.HitListID = hlResult.HitListID;
            m_pagingInfo.HitListType = hlResult.HitListType;
            m_pagingInfo.RecordCount = hlResult.RecordCount;
            DataSet resultSet = coeSearch.GetData(m_formDbMgr.ResultsCriteria, m_pagingInfo, m_formDbMgr.SelectedDataView);

            sResponse = new SearchResponse();
            sResponse.HitListInfo = hlResult;
            sResponse.ResultsDataSet = resultSet;

            return sResponse;
        }
        //---------------------------------------------------------------------
        public void DoSearch(SearchCriteria searchCriteria, HitListInfo domainHLInfo, bool bFilterChildHits)
        {
            // perform search 
            // create and add page with returned dataset; throw if no hits
            m_searchCriteria = searchCriteria;
            m_pagingInfo = new PagingInfo();
            m_pagingInfo.RecordCount = rowsPerPage;
            m_pagingInfo.FilterChildData = bFilterChildHits;

            CBVTimer.StartTimer(true, "search", true);
            COESearch coeSearch = new COESearch();
            try
            {
                Clear();
                if (domainHLInfo != null)
                    m_searchResponse = SearchWithDomain(coeSearch, searchCriteria, domainHLInfo);
                else
                    m_searchResponse = coeSearch.DoSearch(searchCriteria, m_formDbMgr.ResultsCriteria, m_pagingInfo, m_formDbMgr.SelectedDataView);
            }
            catch (Exception ex)
            {
                CBVTimer.EndTimer();
                throw new FormDBLib.Exceptions.SearchException("Search error", ex);
            }
            CBVTimer.EndTimer();
            ProcessIncomingSearchResponse(m_searchResponse);    // throws if no hits
        }
        //---------------------------------------------------------------------
        public void DoRetrieveHitlist(int hitlistID, int listSize, bool bIsSaved, bool bFilterChildHits)
        {
            // create and add page with dataset retrieved from hitlistID
            // caller provides expected size of list
            if (hitlistID == 0)
            {
                DoRetrieveAll();
                return;
            }
            // retrieve the data
            m_pagingInfo = new PagingInfo();
            m_pagingInfo.RecordCount = rowsPerPage;
            m_pagingInfo.HitListID = hitlistID;
            m_pagingInfo.HitListType = bIsSaved ? HitListType.SAVED : HitListType.TEMP;
            m_pagingInfo.FilterChildData = bFilterChildHits;

            CBVTimer.StartTimer(true, "list retrieve", true);
            COESearch coeSearch = new COESearch();
            DataSet ds = null;
            Debug.WriteLine(String.Format("Retrieving hitlistID={0} {1}", hitlistID, bIsSaved ? "SAVED" : "temp"));
            try
            {
                Clear();
                ds = coeSearch.GetData(m_formDbMgr.ResultsCriteria, m_pagingInfo, m_formDbMgr.SelectedDataView);
            }
            catch (Exception ex)
            {
                CBVTimer.EndTimer();
                throw new FormDBLib.Exceptions.SearchException("Search error", ex);
            }
            CBVTimer.EndTimer();

            SetHitlistData(hitlistID, listSize, m_pagingInfo.HitListType);
            ProcessIncomingDataSet(ds);
        }
        //---------------------------------------------------------------------
        public void DoRetrieveAll()
        {
            m_pagingInfo = new PagingInfo();
            m_pagingInfo.RecordCount = rowsPerPage;
            m_pagingInfo.HitListID = 0;

            m_searchCriteria = null;

            int nRows = m_formDbMgr.BaseTableRecordCount;
            SetHitlistData(0, nRows, HitListType.TEMP);
            Clear();
            if (nRows > 0)
                MoveTo(0);
        }
        //---------------------------------------------------------------------
        public int DoMergeHitlists(HitListInfo hitlist1, HitListInfo hitlist2, MergeQuery.LogicChoice logic)
        {
            // create and add page with returned dataset; throw if no rows returned
            // return hitlistID of merged list
            m_pagingInfo = new PagingInfo();
            m_pagingInfo.RecordCount = rowsPerPage;
            int mergedHitListID = 0;

            CBVTimer.StartTimer(true, "list merge", true);
            COEHitListBO hlbo = null;
            try
            {
                switch (logic)
                {
                    case MergeQuery.LogicChoice.kmIntersect:
                        hlbo = COEHitListOperationManager.IntersectHitList(hitlist1, hitlist2);
                        break;
                    case MergeQuery.LogicChoice.kmSubtract:
                        hlbo = COEHitListOperationManager.SubtractHitLists(hitlist1, hitlist2);
                        break;
                    case MergeQuery.LogicChoice.kmSubtractFrom:
                        hlbo = COEHitListOperationManager.SubtractHitLists(hitlist2, hitlist1);
                        break;
                    case MergeQuery.LogicChoice.kmUnion:
                        hlbo = COEHitListOperationManager.UnionHitLists(hitlist1, hitlist2);
                        break;
                }
                //Coverity Bug Fix CID 13033 
                if (hlbo != null)
                {
                    mergedHitListID = hlbo.HitListID;
                    m_pagingInfo.HitListID = hlbo.HitListID;
                    m_pagingInfo.RecordCount = hlbo.NumHits;
                    m_pagingInfo.HitListType = hlbo.HitListType;

                    COESearch coeSearch = new COESearch();
                    DataSet dataSet = coeSearch.GetData(m_formDbMgr.ResultsCriteria, m_pagingInfo, m_formDbMgr.SelectedDataView);

                    SetHitlistData(hlbo.HitListID, hlbo.NumHits, hlbo.HitListType);
                    ProcessIncomingDataSet(dataSet);
                }
            }
            catch (Exception)
            {
                hlbo = null;
                mergedHitListID = 0;
            }
            CBVTimer.EndTimer();
            return mergedHitListID;
        }
        //---------------------------------------------------------------------
        public static DataSet GetOneRecord(ResultsCriteria resultsCriteria, COEDataView dataView)
        {
            // retrieve a single record for given RC
            // this gets us a small dataset we can use for binding

            PagingInfo pagingInfo = new PagingInfo();
            pagingInfo.HitListID = 0;
            pagingInfo.RecordCount = 1;
            pagingInfo.Start = 1;
            pagingInfo.End = 2;

            COESearch coeSearch = new COESearch();
            DataSet dataset = null;
            if (dataView != null)
            {
                try
                {
                    dataset = coeSearch.GetData(resultsCriteria, pagingInfo, dataView);
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                    dataset = null;
                }
            }
            return dataset;
        }
        //---------------------------------------------------------------------
        public static DataSet GetNRecords(ResultsCriteria resultsCriteria, COEDataView dataView,
                                           int maxRecs, int hitListID, bool bIsSaved)
        {
            return GetNRecordsEx(resultsCriteria, dataView, maxRecs, hitListID, bIsSaved, 0);
        }
        //---------------------------------------------------------------------
        public static DataSet GetNRecordsEx(ResultsCriteria resultsCriteria, COEDataView dataView,
                                            int maxRecs, int hitListID, bool bIsSaved, int startAt)
        {
            // retrieve all records for given RC, or up to given max
            // this gets us a dataset we can use for e.g. plotting

            PagingInfo pagingInfo = new PagingInfo();
            pagingInfo.HitListID = hitListID;
            pagingInfo.RecordCount = maxRecs;
            pagingInfo.Start = startAt;

            pagingInfo.End = -1; // 0; signal that we want no paging
            // THIS REQUIRES A FIX IN THE SERVER

            pagingInfo.HitListType = bIsSaved ? HitListType.SAVED : HitListType.TEMP;
            pagingInfo.KeepAlive = KeepAliveModes.NONE;

            COESearch coeSearch = new COESearch();
            DataSet dataset = null;
            if (dataView != null)
            {
                try
                {
                    dataset = coeSearch.GetData(resultsCriteria, pagingInfo, dataView);
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                    dataset = null;
                }
            }
            return dataset;
        }
        //---------------------------------------------------------------------
        public int MergeMultipleHitlists(List<HitListInfo> hitlistInfos)
        {
            Debug.Assert(hitlistInfos.Count > 1);

            HitListInfo hlMain = null;
            foreach (HitListInfo hli in hitlistInfos)
            {
                if (hlMain == null)
                {
                    hlMain = hli;
                }
                else
                {
                    try
                    {
                        COEHitListBO hlbo = COEHitListOperationManager.UnionHitLists(hlMain, hli);
                        hlMain = hlbo.HitListInfo;
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                    }
                }
            }
            //Coverity Bug Fix : CID 13034 
            int resultHitListID = (hlMain != null) ? hlMain.HitListID : 0;
            return resultHitListID;
        }
        //---------------------------------------------------------------------
        public DataPage FetchNewPage(int row)
        {
            // first see if we already have the page
            DataPage dataPage = FindPageContaining(row);
            if (dataPage != null)
                return dataPage;

            CBVStatMessage.Show("Retrieving data page");

            // no: retrieve dataset using GetData
            int nFirst = (row / rowsPerPage) * rowsPerPage;
            int nLast = nFirst + rowsPerPage - 1;

            // pagingInfo already contains hitlistID unless retrieve all
            m_pagingInfo.Start = nFirst + 1;
            m_pagingInfo.End = nLast + 1;
            m_pagingInfo.RecordCount = nLast - nFirst + 1;

            // fetch data
            try
            {
                CBVTimer.StartTimer(true, "fetch new data page", true);

                try
                {
                    if (ServerCache.Exists(m_formDbMgr.SelectedDataView.DataViewID.ToString(), typeof(COEDataViewBO)))
                        ServerCache.Remove(m_formDbMgr.SelectedDataView.DataViewID.ToString(), typeof(COEDataViewBO));
                    else
                        LocalCache.Remove(m_formDbMgr.SelectedDataView.DataViewID.ToString(), typeof(COEDataViewBO));
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
                COESearch coeSearch = new COESearch();
                DataSet dataSet = coeSearch.GetData(m_formDbMgr.ResultsCriteria, m_pagingInfo, m_formDbMgr.SelectedDataView);
                CBVTimer.EndTimer();

                dataPage = MakeDataPage(nFirst, nLast, dataSet);
                currPage = dataPage;
                m_pages.Add(dataPage);
            }
            catch (Exception ex)
            {
                // THIS IS LOUSY .. should throw or otherwise let caller know it failed
                // it gets here more than once
                CBVUtil.ReportError(ex, "Error retrieving data");

                //throw (ex);
            }
#if DEBUG
            // dump a typical data page for inspection
            bool bDumpPage = false, bDumpToFile = false;
            if (bDumpPage && dataPage != null)
            {
                FormDBLib.FormDbMgr.DebugDatasetNoXml(dataPage.DataSet);
                if (bDumpToFile)
                {
                    String fname = "C:\\dset_out.xml";
                    dataPage.DataSet.WriteXml(fname, XmlWriteMode.WriteSchema);
                }
                bDumpPage = false;  // dump only the first
            }
#endif
            CBVStatMessage.Hide();
            return dataPage;
        }
        //---------------------------------------------------------------------
        private DataPage MakeDataPage(int firstRec, int lastRec, DataSet dataSet)
        {
            FormDbMgr.PrepDataSet(dataSet, m_formDbMgr.SelectedDataView);

            if (dataSet != null)
            {
                String message = dataSet.ExtendedProperties["FilteredChildDataMessage"] as String;
                if (!String.IsNullOrEmpty(message))
                    Debug.WriteLine(String.Concat("FilteredChildDataMessage: ", message));
            }

            DataPage dataPage = new DataPage(this);
            dataPage.FirstRec = firstRec;
            dataPage.LastRec = lastRec;
            dataPage.DataSet = dataSet;
            return dataPage;
        }
        #endregion

        #region ChemDataProvider implementation
        bool IChemDataProvider.IsFinished()
        {
            return true;
        }
        //---------------------------------------------------------------------
        DataSet IChemDataProvider.GetSchema()
        {
            return CurrDataSet;
        }
        //---------------------------------------------------------------------
        void IChemDataProvider.Start()
        {
            if (currPage != null)
                MoveTo(0);
        }
        //---------------------------------------------------------------------
        int IChemDataProvider.GetRecordCount()
        {
            return ListSize;
        }
        //---------------------------------------------------------------------
        int IChemDataProvider.GetPageRow(int index)
        {
            return (currPage == null) ? 0 : index - currPage.FirstRec;
        }
        //---------------------------------------------------------------------
        #endregion
    }
    //---------------------------------------------------------------------
    public class DataPage
    {
        #region Variables
        private int firstRec, lastRec;
        private DataSet dataSet;
        private Pager pager;
        #endregion

        #region Properties
        public int FirstRec
        {
            get { return firstRec; }
            set { firstRec = value; }
        }
        //---------------------------------------------------------------------
        public int LastRec
        {
            get { return lastRec; }
            set { lastRec = value; }
        }
        //---------------------------------------------------------------------
        public DataSet DataSet
        {
            get { return dataSet; }
            set { dataSet = value; }
        }
        //---------------------------------------------------------------------
        public Pager Pager
        {
            get { return pager; }
            set { pager = value; }
        }
        #endregion

        #region Constructors
        public DataPage(Pager pager)
        {
            this.pager = pager;
            dataSet = null;
            firstRec = lastRec = -1;
        }
        #endregion

        #region Methods
        public bool ContainsRow(int r)
        {
            return r >= firstRec && r <= lastRec;
        }
        #endregion
    }
    //---------------------------------------------------------------------
}
