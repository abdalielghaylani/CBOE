using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CambridgeSoft.COE.Framework.COESearchService;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.COEDataViewService;
using CambridgeSoft.COE.Framework.COESecurityService;
using CambridgeSoft.COE.Framework.UnitTests.Helpers;
using System.Xml;
using System.Reflection;
using System.Data;
using CambridgeSoft.COE.Framework.Types.Exceptions;
using CambridgeSoft.COE.Framework.COEConfigurationService;

namespace CambridgeSoft.COE.Framework.UnitTests.Search_Tests
{
    /// <summary>
    /// Summary description for COESearchService
    /// </summary>
    [TestClass]
    public class COESearchService
    {
        private COESearch _searchService;
        private SearchManager _searchManager;
        private SearchCriteria _searchCriteria;
        private ResultsCriteria _resultsCriteria;
        private string _pathToXmls = SearchHelper.GetExecutingTestResultsBasePath(SearchHelper._COESearchByHitlistpathToXml);
        private COEDataView theCOEDataView = null;
        private string _useRealTableNames = string.Empty;
        private int _sourceDataviewID = 0;
        private PagingInfo _pagingInfo;
        public COESearchService()
        {
            _searchService = new COESearch();
            _searchManager = new SearchManager();
            COEDataViewBO.Delete(_sourceDataviewID);

        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }


        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{

        //}

        //Use ClassCleanup to run code after all tests in a class have run
        [ClassCleanup()]
        public static void MyClassCleanup() { }

        // Use TestInitialize to run code before running each test 
        [TestInitialize()]
        public void MyTestInitialize()
        {
            Login();
            _pagingInfo = GetFullPagingInfo();
            _searchCriteria = GetSearchCriteria("SourceSearchCriteria.xml");
            _resultsCriteria = GetResultsCriteria("SourceRC.xml");
            _useRealTableNames = "YES";
            theCOEDataView = GetDataViewObject("SourceDV.xml");
            _sourceDataviewID = InsertDataview(GetDataViewObject("SourceDV.xml"));

        }
        //  Use TestCleanup to run code after each test has run
        [TestCleanup()]
        public void MyTestCleanup()
        {
            COEDataViewBO.Delete(_sourceDataviewID);
        }

        #endregion

        #region  DoPartialSearch
        /// <summary>
        /// Unit Test for  SearchResponse DoPartialSearch(SearchCriteria searchCriteria, ResultsCriteria resultsCriteria, PagingInfo pagingInfo, int dataViewID)
        /// </summary>
        [TestMethod]
        public void DoPartialSearchUsingdvIdTest()
        {
            try
            {
                SearchResponse sr = _searchService.DoPartialSearch(_searchCriteria, _resultsCriteria, _pagingInfo, theCOEDataView.DataViewID);
                Assert.IsNotNull(sr, "COESearch.DoPartialSearch does not return expected value");
            }
            catch
            {
                Assert.Fail("DoPartialSearchUsingdvIdTest failed.");
            }
            finally
            {
                COEDataViewBO.Delete(_sourceDataviewID);
            }
        }


        ///// <summary>
        ///// UNit Test for SearchResponse DoPartialSearch(SearchCriteria searchCriteria, ResultsCriteria resultsCriteria, PagingInfo pagingInfo, int dataViewID, string useRealTableNames)
        ///// </summary>
        [TestMethod]
        public void DoPartialSearchUsingTabNameTest()
        {
            try
            {
                SearchResponse sr = _searchService.DoPartialSearch(_searchCriteria, _resultsCriteria, _pagingInfo, theCOEDataView.DataViewID, _useRealTableNames);
                Assert.IsNotNull(sr, "COESearch.DoPartialSearch does not return expected value");
            }
            catch
            {
                Assert.Fail("DoPartialSearchUsingTabNameTest failed.");
            }
            finally
            {
                COEDataViewBO.Delete(_sourceDataviewID);
            }

        }


        ///// <summary>
        ///// Unit Test For SearchResponse DoPartialSearch(SearchCriteria searchCriteria, ResultsCriteria resultsCriteria, PagingInfo pagingInfo, int dataViewID, int commitSize)
        ///// </summary>
        [TestMethod]
        public void DoPartialSearchUsingCommitSizeTest()
        {
            try
            {
                SearchResponse sr = _searchService.DoPartialSearch(_searchCriteria, _resultsCriteria, _pagingInfo, theCOEDataView.DataViewID, 1);
                Assert.IsNotNull(sr, "COESearch.DoPartialSearch does not return expected value");
            }
            catch
            {
                Assert.Fail("DoPartialSearchUsingCommitSizeTest failed.");
            }
            finally
            { COEDataViewBO.Delete(_sourceDataviewID); }
        }
        /// <summary>
        /// Unit Test for SearchResponse DoPartialSearch(SearchCriteria searchCriteria, ResultsCriteria resultsCriteria, PagingInfo pagingInfo, int dataViewID, int commitSize, string useRealTableNames) 
        /// </summary>
        [TestMethod]
        public void DoPartialSearchUsingCommitSizeTabNameTest()
        {
            try
            {
                SearchResponse sr = _searchService.DoPartialSearch(_searchCriteria, _resultsCriteria, _pagingInfo, theCOEDataView.DataViewID, 1, _useRealTableNames);
                Assert.IsNotNull(sr, "COESearch.DoPartialSearch does not return expected value");
            }
            catch
            {
                Assert.Fail("DoPartialSearchUsingCommitSizeTabNameTest failed.");
            }
            finally
            {
                COEDataViewBO.Delete(_sourceDataviewID);
            }
        }
        /// <summary>
        /// Unit Test For SearchResponse DoPartialSearch(SearchCriteria searchCriteria, ResultsCriteria resultsCriteria, PagingInfo pagingInfo, COEDataView dataView)
        /// </summary>
        [TestMethod]
        public void DoPartialSearchUsingDataviewTest()
        {
            try
            {
                SearchResponse sr = _searchService.DoPartialSearch(_searchCriteria, _resultsCriteria, _pagingInfo, theCOEDataView);
                Assert.IsNotNull(sr, "COESearch.DoPartialSearch does not return expected value");
            }
            catch
            {
                Assert.Fail("DoPartialSearchUsingDataviewTest failed.");
            }
            finally
            {
                COEDataViewBO.Delete(_sourceDataviewID);
            }
        }

        /// <summary>
        /// Unit Test For DoPartialSearch(SearchCriteria searchCriteria, ResultsCriteria resultsCriteria, PagingInfo pagingInfo, COEDataView dataView, string useRealTableNames)
        /// </summary>
        [TestMethod]
        public void DoPartialSearchUsingDataviewTabNameTest()
        {
            try
            {
                SearchResponse sr = _searchService.DoPartialSearch(_searchCriteria, _resultsCriteria, _pagingInfo, theCOEDataView, _useRealTableNames);
                Assert.IsNotNull(sr, "COESearch.DoPartialSearch does not return expected value");
            }
            catch
            {
                Assert.Fail("DoPartialSearchUsingDataviewTabNameTest failed.");
            }
            finally
            {
                COEDataViewBO.Delete(_sourceDataviewID);
            }

        }

        /// <summary>
        /// Unit Test ForSearchResponse DoPartialSearch(SearchCriteria searchCriteria, ResultsCriteria resultsCriteria, PagingInfo pagingInfo, COEDataView dataView, int commitSize) {
        /// </summary>
        [TestMethod]
        public void DoPartialSearchUsingDataviewComitSizeTest()
        {
            try
            {
                SearchResponse sr = _searchService.DoPartialSearch(_searchCriteria, _resultsCriteria, _pagingInfo, theCOEDataView, -1);
                Assert.IsNotNull(sr, "COESearch.DoPartialSearch does not return expected value");
            }
            catch
            {
                Assert.Fail("DoPartialSearchUsingDataviewComitSizeTest failed.");
            }
            finally
            {
                COEDataViewBO.Delete(_sourceDataviewID);
            }
        }

        /// <summary>
        /// Unit Test for SearchResponse DoPartialSearch(SearchCriteria searchCriteria, ResultsCriteria resultsCriteria, PagingInfo pagingInfo, COEDataView dataView, int commitSize, string useRealTableNames) 
        /// </summary>
        [TestMethod]
        public void DoPartialSearchUsingDVComitSizeTabNameTest()
        {
            try
            {
                SearchResponse sr = _searchService.DoPartialSearch(_searchCriteria, _resultsCriteria, _pagingInfo, theCOEDataView, -1, _useRealTableNames);
                Assert.IsNotNull(sr, "COESearch.DoPartialSearch does not return expected value");
            }
            catch
            {
                Assert.Fail("DoPartialSearchUsingDVComitSizeTabNameTest failed.");
            }
            finally
            {
                COEDataViewBO.Delete(_sourceDataviewID);
            }
        }
        #endregion

        #region DoSearch

        //SearchResponse DoSearch(SearchCriteria searchCriteria, ResultsCriteria resultsCriteria, PagingInfo pagingInfo, int dataViewID)

        /// <summary>
        /// Unit test for  SearchResponse DoSearch(ref SearchCriteria searchCriteria, ResultsCriteria resultsCriteria, PagingInfo pagingInfo, COEDataView dataView, bool directCall, ConnStringType connStringType)
        /// </summary>
        [TestMethod]
        public void DoSearchusingdirectCallTest()
        {
            try
            {
                SearchResponse sr = _searchManager.DoSearch(ref _searchCriteria, _resultsCriteria, _pagingInfo, theCOEDataView, true, GetConnStringType(theCOEDataView.DataViewID));
                Assert.IsNotNull(sr, "SearchManager.DoSearch does not return expected value");
            }
            catch
            {
                Assert.Fail("DoSearchusingdirectCallTest failed.");
            }
            finally
            {
                COEDataViewBO.Delete(_sourceDataviewID);
            }
        }
        /// <summary>
        /// Unit test for  SearchResponse DoSearch(ref SearchCriteria searchCriteria, ResultsCriteria resultsCriteria, PagingInfo pagingInfo, COEDataView dataView, bool directCall, ConnStringType connStringType)
        /// </summary>
        [TestMethod]
        public void DoSearchwithoutdirectCallTest()
        {
            try
            {
                SearchResponse sr = _searchManager.DoSearch(ref _searchCriteria, _resultsCriteria, _pagingInfo, theCOEDataView, false, GetConnStringType(theCOEDataView.DataViewID));
                Assert.IsNotNull(sr, "SearchManager.DoSearch does not return expected value");
            }
            catch
            {
                Assert.Fail("DoSearchwithoutdirectCallTest failed.");
            }
            finally
            {
                COEDataViewBO.Delete(_sourceDataviewID);
            }
        }
        //SearchResponse DoSearch(ref SearchCriteria searchCriteria, ResultsCriteria resultsCriteria, PagingInfo pagingInfo, COEDataView dataView, ConnStringType connStringType)
        [TestMethod]
        public void DoSearchusingdvTest()
        {
            try
            {
                SearchResponse sr = _searchManager.DoSearch(ref _searchCriteria, _resultsCriteria, _pagingInfo, theCOEDataView, GetConnStringType(theCOEDataView.DataViewID));
                Assert.IsNotNull(sr, "SearchManager.DoSearch does not return expected value");
            }
            catch
            {
                Assert.Fail("DoSearchusingdvTest failed.");
            }
            finally
            {
                COEDataViewBO.Delete(_sourceDataviewID);
            }
        }
        //SearchResponse DoSearch(ref SearchCriteria searchCriteria, ResultsCriteria resultsCriteria, PagingInfo pagingInfo, COEDataView dataView, ConnStringType connStringType, string useRealTableNames)
        [TestMethod]
        public void DoSearchusingTableNameTest()
        {
            try
            {
                SearchResponse sr = _searchManager.DoSearch(ref _searchCriteria, _resultsCriteria, _pagingInfo, theCOEDataView, GetConnStringType(theCOEDataView.DataViewID), _useRealTableNames);
                Assert.IsNotNull(sr, "SearchManager.DoSearch does not return expected value");
            }
            catch
            {
                Assert.Fail("DoSearchusingTableNameTest failed.");
            }
            finally
            {
                COEDataViewBO.Delete(_sourceDataviewID);
            }
        }
        //SearchResponse DoSearch(ref SearchCriteria searchCriteria, ResultsCriteria resultsCriteria, PagingInfo pagingInfo, COEDataView dataView, ConnStringType connStringType, string useRealTableNames, OrderByCriteria orderByCriteria)
        [TestMethod]
        public void DoSearchusingOrderByCriteriaTest()
        {
            try
            {
                SearchResponse sr = _searchManager.DoSearch(ref _searchCriteria, _resultsCriteria, _pagingInfo, theCOEDataView, GetConnStringType(theCOEDataView.DataViewID), _useRealTableNames, new OrderByCriteria());
                Assert.IsNotNull(sr, "SearchManager.DoSearch does not return expected value");
            }
            catch
            {
                Assert.Fail("DoSearchusingOrderByCriteriaTest failed.");
            }
            finally
            {
                COEDataViewBO.Delete(_sourceDataviewID);
            }
        }


        /// <summary>
        /// Unit test for public SearchResponse DoPartialSearch(ref SearchCriteria searchCriteria, ResultsCriteria resultsCriteria, PagingInfo pagingInfo, COEDataView dataView, ConnStringType connStringType)
        /// </summary>
        [TestMethod]
        public void DoPartialSearchusingconnStringTypeTest()
        {
            try
            {
                SearchResponse sr = _searchManager.DoPartialSearch(ref _searchCriteria, _resultsCriteria, _pagingInfo, theCOEDataView, GetConnStringType(theCOEDataView.DataViewID));
                Assert.IsNotNull(sr, "SearchManager.DoPartialSearch does not return expected value");
            }
            catch
            {
                Assert.Fail("DoPartialSearchusingconnStringTypeTest failed.");
            }
            finally
            {
                COEDataViewBO.Delete(_sourceDataviewID);
            }
        }
        /// <summary>
        /// UNit Test for public SearchResponse DoPartialSearch(ref SearchCriteria searchCriteria, ResultsCriteria resultsCriteria, PagingInfo pagingInfo, COEDataView dataView, ConnStringType connStringType, string useRealTableNames)
        /// </summary>
        [TestMethod]
        public void DoPartialSearchusinguseRealTableNamesTest()
        {
            try
            {
                SearchResponse sr = _searchManager.DoPartialSearch(ref _searchCriteria, _resultsCriteria, _pagingInfo, theCOEDataView, GetConnStringType(theCOEDataView.DataViewID), _useRealTableNames);
                Assert.IsNotNull(sr, "SearchManager.DoPartialSearch does not return expected value");
            }
            catch
            {
                Assert.Fail("DoPartialSearchusinguseRealTableNamesTest failed.");
            }
            finally
            {
                COEDataViewBO.Delete(_sourceDataviewID);
            }
        }
        /// <summary>
        /// Unit Test for public SearchResponse DoPartialSearch(ref SearchCriteria searchCriteria, ResultsCriteria resultsCriteria, PagingInfo pagingInfo, COEDataView dataView, ConnStringType connStringType, string useRealTableNames, OrderByCriteria orderByCriteria)
        /// </summary>
        [TestMethod]
        public void DoPartialSearchusingorderbycriteriaTest()
        {
            try
            {
                SearchResponse sr = _searchManager.DoPartialSearch(ref _searchCriteria, _resultsCriteria, _pagingInfo, theCOEDataView, GetConnStringType(theCOEDataView.DataViewID), _useRealTableNames, new OrderByCriteria());
                Assert.IsNotNull(sr, "SearchManager.DoPartialSearch does not return expected value");
            }
            catch
            {
                Assert.Fail("DoPartialSearchusingorderbycriteriaTest failed.");
            }
            finally
            {
                COEDataViewBO.Delete(_sourceDataviewID);
            }
        }
        /// <summary>
        /// Unit Test for public SearchResponse DoPartialSearch(ref SearchCriteria searchCriteria, ResultsCriteria resultsCriteria, PagingInfo pagingInfo, COEDataView dataView, ConnStringType connStringType, int commitSize)
        /// </summary>
        [TestMethod]
        public void DoPartialSearchusingcommitSizeTest()
        {
            try
            {
                SearchResponse sr = _searchManager.DoPartialSearch(ref _searchCriteria, _resultsCriteria, _pagingInfo, theCOEDataView, GetConnStringType(theCOEDataView.DataViewID), -1);
                Assert.IsNotNull(sr, "SearchManager.DoPartialSearch does not return expected value");
            }
            catch
            {
                Assert.Fail("DoPartialSearchusingcommitSizeTest failed.");
            }
            finally
            {
                COEDataViewBO.Delete(_sourceDataviewID);
            }
        }
        /// <summary>
        /// Unit Test For public SearchResponse DoPartialSearch(ref SearchCriteria searchCriteria, ResultsCriteria resultsCriteria, PagingInfo pagingInfo, COEDataView dataView, ConnStringType connStringType, int commitSize, string useRealTableNamesStr)
        /// </summary>
        [TestMethod]
        public void DoPartialSearchusingcommitSizeTabNameTest()
        {
            try
            {
                SearchResponse sr = _searchManager.DoPartialSearch(ref _searchCriteria, _resultsCriteria, _pagingInfo, theCOEDataView, GetConnStringType(theCOEDataView.DataViewID), -1, _useRealTableNames);
                Assert.IsNotNull(sr, "SearchManager.DoPartialSearch does not return expected value");
            }
            catch
            {
                Assert.Fail("DoPartialSearchusingcommitSizeTabNameTest failed.");
            }
            finally
            {
                COEDataViewBO.Delete(_sourceDataviewID);
            }
        }
        /// <summary>
        /// Unit Test For public SearchResponse DoPartialSearch(ref SearchCriteria searchCriteria, ResultsCriteria resultsCriteria, PagingInfo pagingInfo, COEDataView dataView, ConnStringType connStringType, int commitSize, string useRealTableNamesStr, OrderByCriteria orderByCriteria)
        /// </summary>
        [TestMethod]
        public void DoPartialSearchusingcommitSizeTabNameOrderbyTest()
        {
            try
            {
                SearchResponse sr = _searchManager.DoPartialSearch(ref _searchCriteria, _resultsCriteria, _pagingInfo, theCOEDataView, GetConnStringType(theCOEDataView.DataViewID), -1, _useRealTableNames, new OrderByCriteria());
                Assert.IsNotNull(sr, "SearchManager.DoPartialSearch does not return expected value");
            }
            catch
            {
                Assert.Fail("DoPartialSearchusingcommitSizeTabNameOrderbyTest failed.");
            }
            finally
            {
                COEDataViewBO.Delete(_sourceDataviewID);
            }
        }

        /// <summary>
        /// Unit Test For SearchResponse[] DoGlobalSearch(ref SearchCriteria searchCriteria, ResultsCriteria resultsCriteria, PagingInfo pagingInfo, COEDataView[] dataView, ConnStringType connStringType)
        /// </summary>
        [TestMethod]
        public void DoGlobalSearchTest()
        {
            try
            {
                SearchResponse[] sr = _searchManager.DoGlobalSearch(ref _searchCriteria, _resultsCriteria, _pagingInfo, new COEDataView[] { theCOEDataView, null }, GetConnStringType(theCOEDataView.DataViewID));
                Assert.IsNotNull(sr, "SearchManager.DoGlobalSearch does not return expected value");
                Assert.IsTrue(sr.Count() > 0, "SearchManager.DoGlobalSearch does not return expected value");
            }
            catch
            {
                Assert.Fail("DoGlobalSearchTest failed.");
            }
            finally
            {
                COEDataViewBO.Delete(_sourceDataviewID);
            }
        }
        /// <summary>
        /// Unit Test For GetGlobalSearchData(ResultsCriteria resultsCriteria, PagingInfo pagingInfo, COEDataView[] dataView, ConnStringType connStringType)
        /// </summary>
        [TestMethod]
        public void GetGlobalSearchDataTest()
        {
            try
            {
                DataSet[] ds = _searchManager.GetGlobalSearchData(_resultsCriteria, _pagingInfo, new COEDataView[] { theCOEDataView, null }, GetConnStringType(theCOEDataView.DataViewID));
                Assert.IsNotNull(ds, "SearchManager.GetGlobalSearchData does not return expected value");
                Assert.IsTrue(ds.Count() > 0, "SearchManager.GetGlobalSearchData does not return expected value");
            }
            catch
            {
                Assert.Fail("GetGlobalSearchDataTest failed.");
            }
            finally
            {
                COEDataViewBO.Delete(_sourceDataviewID);
            }
        }
        /// <summary>
        /// GetFastRecordCount(HitListInfo hitListInfo, COEDataView dataView, ConnStringType connStringType)
        /// </summary>
        [TestMethod]
        public void GetFastRecordCountTest()
        {
            try
            {
                HitListInfo ht = new HitListInfo();
                _searchManager.GetFastRecordCount(ht, theCOEDataView, GetConnStringType(theCOEDataView.DataViewID));
                Assert.IsNotNull(ht, "SearchManager.GetFastRecordCount does not return expected value");
            }
            catch
            {
                Assert.Fail("GetFastRecordCountTest failed.");
            }
            finally
            {
                COEDataViewBO.Delete(_sourceDataviewID);
            }
        }
        /// <summary>
        /// Unit Test For int GetExactRecordCount(COEDataView dataView, ConnStringType connStringType)
        /// </summary>
        [TestMethod]
        public void GetExactRecordCountTest()
        {
            try
            {
                int expected = 75637;
                int actual = _searchManager.GetExactRecordCount(theCOEDataView, GetConnStringType(theCOEDataView.DataViewID));
                Assert.AreEqual(expected, actual, "SearchManager.GetExactRecordCountTest does not return expected value");
            }
            catch
            {
                Assert.Fail("GetExactRecordCountTest failed.");
            }
            finally
            {
                COEDataViewBO.Delete(_sourceDataviewID);
            }
        }
        /// <summary>
        ///Unit Test For public DataSet GetData(ResultsCriteria resultsCriteria, PagingInfo pagingInfo, COEDataView dataView, ConnStringType connStringType) 
        /// </summary>
        [TestMethod]
        public void GetData_connStringTypeTest()
        {
            try
            {
                DataSet ds = _searchManager.GetData(_resultsCriteria, _pagingInfo, theCOEDataView, GetConnStringType(theCOEDataView.DataViewID));
                Assert.IsNotNull(ds, "SearchManager.GetData does not return expected value");
            }
            catch
            {
                Assert.Fail("GetData_connStringTypeTest failed.");
            }
            finally
            {
                COEDataViewBO.Delete(_sourceDataviewID);
            }
        }
        /// <summary>
        /// Unit Test For public DataSet GetData(ResultsCriteria resultsCriteria, PagingInfo pagingInfo, COEDataView dataView, ConnStringType connStringType, string useRealTableNamesStr)
        /// </summary>
        [TestMethod]
        public void GetData_useRealTableNamesStrTest()
        {
            try
            {
                DataSet ds = _searchManager.GetData(_resultsCriteria, _pagingInfo, theCOEDataView, GetConnStringType(theCOEDataView.DataViewID), _useRealTableNames);
                Assert.IsNotNull(ds, "SearchManager.GetData does not return expected value");
            }
            catch
            {
                Assert.Fail("GetData_useRealTableNamesStrTest failed.");
            }
            finally
            {
                COEDataViewBO.Delete(_sourceDataviewID);
            }
        }
        /// <summary>
        /// Unit Test For public DataSet GetData(ResultsCriteria resultsCriteria, PagingInfo pagingInfo, COEDataView dataView, ConnStringType connStringType, string useRealTableNamesStr, OrderByCriteria originalOrderByCriteria)
        /// </summary>
        [TestMethod]
        public void GetData_OrderByCriteriaTest()
        {
            try
            {
                DataSet ds = _searchManager.GetData(_resultsCriteria, _pagingInfo, theCOEDataView, GetConnStringType(theCOEDataView.DataViewID), _useRealTableNames, new OrderByCriteria());
                Assert.IsNotNull(ds, "SearchManager.GetData does not return expected value");
            }
            catch
            {
                Assert.Fail("GetData_OrderByCriteriaTest failed.");
            }
            finally
            {
                COEDataViewBO.Delete(_sourceDataviewID);
            }
        }
        /// <summary>
        /// Unit Test for public DataSet GetData(ResultsCriteria resultsCriteria, PagingInfo pagingInfo, COEDataView dataView, ConnStringType connStringType, string useRealTableNamesStr, OrderByCriteria originalOrderByCriteria, SearchCriteria searchCriteria)
        /// </summary>
        [TestMethod]

        public void GetData_SearchCriteriaTest()
        {
            try
            {
                DataSet ds = _searchManager.GetData(_resultsCriteria, _pagingInfo, theCOEDataView, GetConnStringType(theCOEDataView.DataViewID), _useRealTableNames, new OrderByCriteria(), _searchCriteria);
                Assert.IsNotNull(ds, "SearchManager.GetData does not return expected value");
            }
            catch
            {
                Assert.Fail("GetData_SearchCriteria failed.");
            }
            finally
            {
                COEDataViewBO.Delete(_sourceDataviewID);
            }
        }

        [TestMethod]

        public void GetData_HighlightSubStructuresTest()
        {
            try
            {
                _pagingInfo.HighlightSubStructures = true;
                DataSet ds = _searchManager.GetData(_resultsCriteria, _pagingInfo, theCOEDataView, GetConnStringType(theCOEDataView.DataViewID), _useRealTableNames, new OrderByCriteria(), _searchCriteria);
                Assert.IsNotNull(ds, "SearchManager.GetData does not return expected value");
                _pagingInfo.HighlightSubStructures = false;
            }
            catch
            {
                Assert.Fail("GetData_HighlightSubStructuresTest failed.");
            }
            finally
            {
                COEDataViewBO.Delete(_sourceDataviewID);
            }
        }

        [TestMethod]

        public void GetData__RealTableNamesNullTest()
        {
            try
            {
                _pagingInfo.HighlightSubStructures = true;
                DataSet ds = _searchManager.GetData(_resultsCriteria, _pagingInfo, theCOEDataView, GetConnStringType(theCOEDataView.DataViewID), null, new OrderByCriteria(), _searchCriteria);
                Assert.IsNotNull(ds, "SearchManager.GetData does not return expected value");
                _pagingInfo.HighlightSubStructures = false;
            }
            catch
            {
                Assert.Fail("GetData__RealTableNamesNullTest failed.");
            }
            finally
            {
                COEDataViewBO.Delete(_sourceDataviewID);
            }
        }

        [TestMethod]

        public void GetData__OrderByCriteriaNull()
        {
            try
            {
                _pagingInfo.HighlightSubStructures = true;
                DataSet ds = _searchManager.GetData(_resultsCriteria, _pagingInfo, theCOEDataView, GetConnStringType(theCOEDataView.DataViewID), _useRealTableNames, null, _searchCriteria);
                Assert.IsNotNull(ds, "SearchManager.GetData does not return expected value");
                _pagingInfo.HighlightSubStructures = false;
            }
            catch
            {
                Assert.Fail("GetData__OrderByCriteriaNull failed.");
            }
            finally
            {
                COEDataViewBO.Delete(_sourceDataviewID);
            }
        }
        [TestMethod]
        public void GetData__KeepAliveTest()
        {
            try
            {
                _pagingInfo.KeepAlive = KeepAliveModes.TRANSIENT;
                DataSet ds = _searchManager.GetData(_resultsCriteria, _pagingInfo, theCOEDataView, GetConnStringType(theCOEDataView.DataViewID), _useRealTableNames, null, _searchCriteria);
                Assert.IsNotNull(ds, "SearchManager.GetData does not return expected value");

            }
            catch
            {
                Assert.Fail("GetData__KeepAliveTest failed.");
            }
            finally
            {
                COEDataViewBO.Delete(_sourceDataviewID);
            }
        }
        [TestMethod]
        [ExpectedException(typeof(SQLGeneratorException))]
        public void GetData_BasetableNullTest()
        {
            DataSet ds = _searchManager.GetData(new ResultsCriteria(), _pagingInfo, theCOEDataView, GetConnStringType(theCOEDataView.DataViewID), _useRealTableNames, null, _searchCriteria);
            COEDataViewBO.Delete(_sourceDataviewID);
        }

        [TestMethod]
        public void GetData_addSortOrderTrueTest()
        {
            try
            {
                _pagingInfo.HitListID = 1;
                DataSet ds = _searchManager.GetData(_resultsCriteria, _pagingInfo, theCOEDataView, GetConnStringType(theCOEDataView.DataViewID), _useRealTableNames, null, _searchCriteria);
                Assert.IsNotNull(ds, "SearchManager.GetData does not return expected value");

            }
            catch
            {
                Assert.Fail("GetData_addSortOrderTrueTest failed.");
            }
            finally
            {
                COEDataViewBO.Delete(_sourceDataviewID);
            }
        }

        [TestMethod]
        public void GetData_FilterChildTrueDataTest()
        {
            try
            {
                _pagingInfo.FilterChildData = true;
                DataSet ds = _searchManager.GetData(_resultsCriteria, _pagingInfo, theCOEDataView, GetConnStringType(theCOEDataView.DataViewID), _useRealTableNames, null, _searchCriteria);
                Assert.IsNotNull(ds, "SearchManager.GetData does not return expected value");

            }
            catch
            {
                Assert.Fail("GetData_FilterChildDataTest failed.");
            }
            finally
            {
                COEDataViewBO.Delete(_sourceDataviewID);
            }
        }

        [TestMethod]
        public void GetData_FilterChildFalseDataTest()
        {
            try
            {
                _searchManager.AppConfigData =  ConfigurationUtilities.GetApplicationData(COEAppName.Get().ToString());
                _searchManager.AppConfigData.SaveQueryHistory = "";
                _pagingInfo.FilterChildData = true;
                _pagingInfo.HitListID = 1;
                DataSet ds = _searchManager.GetData(_resultsCriteria, _pagingInfo, theCOEDataView, GetConnStringType(theCOEDataView.DataViewID), _useRealTableNames, null, _searchCriteria);
                Assert.IsNotNull(ds, "SearchManager.GetData does not return expected value");

            }
            catch
            {
                Assert.Fail("GetData_FilterChildFalseDataTest failed.");
            }
            finally
            {
                COEDataViewBO.Delete(_sourceDataviewID);
            }
        }


        /// <summary>
        ///Unit Test Method For public HitListInfo GetHitList(ref SearchCriteria searchCriteria, COEDataView dataView, ConnStringType connStringType) 
        /// </summary>
        [TestMethod]
        public void GetHitList_connStringTypeTest()
        {
            try
            {
                HitListInfo ht = _searchManager.GetHitList(ref _searchCriteria, theCOEDataView, GetConnStringType(theCOEDataView.DataViewID));
                Assert.IsNotNull(ht, "SearchManager.GetHitList does not return expected value");
            }
            catch
            {
                Assert.Fail("GetHitList_connStringTypeTest failed.");
            }
            finally
            {
                COEDataViewBO.Delete(_sourceDataviewID);
            }
        }

        /// <summary>
        /// Unit Test For public HitListInfo GetHitList(ref SearchCriteria searchCriteria, COEDataView dataView, ConnStringType connStringType, HitListInfo refineHitlist)
        /// </summary>
        [TestMethod]
        public void GetHitList_refineHitlistTest()
        {
            try
            {
                HitListInfo ht = _searchManager.GetHitList(ref _searchCriteria, theCOEDataView, GetConnStringType(theCOEDataView.DataViewID), new HitListInfo());
                Assert.IsNotNull(ht, "SearchManager.GetHitList does not return expected value");
            }
            catch
            {
                Assert.Fail("GetHitList_refineHitlistTest failed.");
            }
            finally
            {
                COEDataViewBO.Delete(_sourceDataviewID);
            }
        }
        /// <summary>
        /// Unit Test for public HitListInfo GetPartialHitList(ref SearchCriteria searchCriteria, COEDataView dataView, ConnStringType connStringType, HitListInfo refineHitlist)
        /// </summary>
        [TestMethod]
        public void GetPartialHitList_refineHitlistTest()
        {
            try
            {
                HitListInfo ht = _searchManager.GetPartialHitList(ref _searchCriteria, theCOEDataView, GetConnStringType(theCOEDataView.DataViewID), new HitListInfo());
                Assert.IsNotNull(ht, "SearchManager.GetPartialHitList does not return expected value");
            }
            catch
            {
                Assert.Fail("GetPartialHitList_refineHitlistTest failed.");
            }
            finally
            {
                COEDataViewBO.Delete(_sourceDataviewID);
            }
        }
        /// <summary>
        /// Unit Test For public HitListInfo GetHitList(ref SearchCriteria searchCriteria, COEDataView dataView, bool partialHitList, ConnStringType connStringType, HitListInfo refineHitlist)
        /// </summary>
        [TestMethod]
        public void GetHitList_partialHitListTest()
        {
            try
            {
                HitListInfo ht = _searchManager.GetHitList(ref _searchCriteria, theCOEDataView, true, GetConnStringType(theCOEDataView.DataViewID), new HitListInfo());
                Assert.IsNotNull(ht, "SearchManager.GetHitList does not return expected value");
            }
            catch
            {
                Assert.Fail("GetHitList_partialHitListTest failed.");
            }
            finally
            {
                COEDataViewBO.Delete(_sourceDataviewID);
            }
        }
        /// <summary>
        /// Unit Test For  public HitListInfo GetHitlistProgress(HitListInfo originalHitListInfo, string database, ConnStringType connStringType)
        /// </summary>
        /// <param name="dataViewID"></param>
        /// <returns></returns>
        [TestMethod]
        public void GetHitlistProgressTest()
        {
            try
            {
                HitListInfo ht = _searchManager.GetHitlistProgress(new HitListInfo(), theCOEDataView.Database, GetConnStringType(theCOEDataView.DataViewID));
                Assert.IsNotNull(ht, "SearchManager.GetHitlistProgress does not return expected value");
            }
            catch
            {
                Assert.Fail("GetHitlistProgressTest failed.");
            }
            finally
            {
                COEDataViewBO.Delete(_sourceDataviewID);
            }
        }
        #endregion

        #region Private Methods
        private ConnStringType GetConnStringType(int dataViewID)
        {
            return ConnStringType.OWNERPROXY;
        }
        private static int InsertDataview(COEDataView dataView)
        {
            COEDataViewBO dv = COEDataViewBO.New(dataView.Name, dataView.Description, dataView, null);
            dv.ID = dataView.DataViewID;
            dv.IsPublic = true;

            dv = dv.Save();
            return dv.ID;
        }
        private COEDataView GetDataViewObject(string fileName)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(_pathToXmls + "\\" + fileName);
            COEDataView dataview = new COEDataView(doc);
            return dataview;
        }
        private void Login()
        {
            COEPrincipal.Logout();
            System.Security.Principal.IPrincipal user = Csla.ApplicationContext.User;

            string userName = "cssadmin";
            string password = "cssadmin";
            bool result = COEPrincipal.Login(userName, password);
        }

        private PagingInfo GetFullPagingInfo()
        {
            PagingInfo pi = new PagingInfo();
            pi.Start = 1;
            pi.RecordCount = 50000;
            return pi;
        }

        private ResultsCriteria GetResultsCriteria()
        {
            return this.GetResultsCriteria(@"ResultsCriteria.xml");
        }

        private ResultsCriteria GetResultsCriteria(string filename)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(_pathToXmls + "\\" + filename);
            ResultsCriteria rc = new ResultsCriteria(doc);
            return rc;
        }

        private SearchCriteria GetSearchCriteria()
        {
            return this.GetSearchCriteria(@"\SearchCriteria.xml");
        }

        private SearchCriteria GetSearchCriteria(string fileName)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(_pathToXmls + fileName);
            SearchCriteria sc = new SearchCriteria(doc);
            return sc;
        }

        #endregion
    }
}
