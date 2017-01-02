using System;
using System.Text;
using System.Collections.Generic;
using NUnit.Framework;
using CambridgeSoft.COE.Framework.COESecurityService;
using System.Xml;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.COESearchService;
using CambridgeSoft.COE.Framework.COEDataViewService;
using CambridgeSoft.COE.Framework.COEHitListService;
using System.Data;
using CambridgeSoft.COE.Framework.NUnitTests.Helpers;
//TODO: Add a unit tests for the complex case adding an extra criteria, IE: Molweight 58-60. Confirm the sql is correct still.
namespace SearchUnitTests
{
    /// <summary>
    /// Set of unit tests for COESearchByHitlistTests. Requires Dataviews 5010 and 5002 in db.
    /// </summary>
    [TestFixture]
    public class COESearchByHitlistTests
    {
        private COESearch _searchService;
        private SearchCriteria _sourceSearchCriteria;
        private SearchCriteria _searchByHitlistSearchCriteria;
        private ResultsCriteria _sourceResultsCriteria;
        private ResultsCriteria _targetResultsCriteria;
        private PagingInfo _pagingInfo;
        private static int _sourceDataviewID;
        private static int _targetDataviewID;
        private static string _pathToXmls = SearchHelper.GetExecutingTestResultsBasePath(@"\Search Tests\COESearchByHitlistTests XML\");

        public COESearchByHitlistTests()
        {
            _searchService = new COESearch();
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        [TestFixtureSetUp]
        public static void MyClassInitialize()
        {
            COEDataViewBO.Delete(95000);
            COEDataViewBO.Delete(95001);
            _sourceDataviewID = InsertDataview(GetDataView("SourceDV.xml"));
            _targetDataviewID = InsertDataview(GetDataView("TargetDV.xml"));
        }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        [TestFixtureTearDown]
        public static void MyClassCleanup()
        {
            COEDataViewBO.Delete(_sourceDataviewID);
            COEDataViewBO.Delete(_targetDataviewID);
        }
        //
        // Use TestInitialize to run code before running each test 
        [SetUp]
        public void MyTestInitialize()
        {
            _pagingInfo = GetFullPagingInfo();
            _sourceSearchCriteria = GetSearchCriteria("SourceSearchCriteria.xml");
            _searchByHitlistSearchCriteria = GetSearchCriteria("SearchByHitlistSearchCriteria.xml");
            _sourceResultsCriteria = GetResultsCriteria("SourceRC.xml");
            _targetResultsCriteria = GetResultsCriteria("TargetRC.xml");

            Login();
        }

        //Use TestCleanup to run code after each test has run
        //[TearDown]
        //public void MyTestCleanup() { }
        #endregion

        #region Test Temp Hitlist Methods
        /// <summary>
        /// DV1
        /// -table1 assayedcompounds
        ///  -field pubchem_compound_cid pk
        ///  -field base64_cdx
        ///  -field pubchem_molecular_formula
        ///  -field reg_batch
        /// 
        /// DV2
        /// -table1 AID741MRP
        ///  -field pubchem_sid pk
        ///  -field toxicity enhacement factor
        ///  -field pubhem_cid
        ///  -field ar_number
        /// 
        /// Search using dv1 for molweightcontains C17.
        /// Use that temporary hitlist to join with DV2 using dv1.reg_batch = dv2.ar_number
        /// 
        /// This method pages the results and use hitlists.
        /// </summary>
        [Test]
        public void JoinThroughBaseTablesTestSearchTest()
        {
            JoinThroughBaseTablesTest(false, false);
        }

        /// <summary>
        /// DV1
        /// -table1 assayedcompounds
        ///  -field pubchem_compound_cid pk
        ///  -field base64_cdx
        ///  -field pubchem_molecular_formula
        ///  -field reg_batch
        /// 
        /// DV2
        /// -table1 AID741MRP
        ///  -field pubchem_sid pk
        ///  -field toxicity enhacement factor
        ///  -field pubhem_cid
        ///  -field ar_number
        /// 
        /// Search using dv1 for molweightcontains C17.
        /// Use that temporary hitlist to join with DV2 using dv1.reg_batch = dv2.ar_number
        /// 
        /// This method pages the results, but do not use hitlists.
        /// </summary>
        [Test]
        public void JoinThroughBaseTablesTestNoHitlistTest()
        {
            JoinThroughBaseTablesTest(true, false);
        }

        /// <summary>
        /// DV1
        /// -table1 assayedcompounds
        ///  -field pubchem_compound_cid pk
        ///  -field base64_cdx
        ///  -field pubchem_molecular_formula
        ///  -field reg_batch
        /// 
        /// DV2
        /// -table1 AID741MRP
        ///  -field pubchem_sid pk
        ///  -field toxicity enhacement factor
        ///  -field pubhem_cid
        ///  -field ar_number
        /// 
        /// Search using dv1 for molweightcontains C17.
        /// Use that temporary hitlist to join with DV2 using dv1.reg_batch = dv2.ar_number
        /// 
        /// This method don't pages the results and do not use hitlists.
        /// </summary>
        [Test]
        public void JoinThroughBaseTablesTestNoHitlistNoPagingTest()
        {
            JoinThroughBaseTablesTest(true, true);
        }

        private void JoinThroughBaseTablesTest(bool noHitlist, bool noPaging)
        {
            SearchResponse sr = null;
            sr = _searchService.DoSearch(_sourceSearchCriteria, _sourceResultsCriteria, _pagingInfo, _sourceDataviewID);
            SearchCriteria.HitlistCriteria hlCriteria = ((SearchCriteria.SearchCriteriaItem)_searchByHitlistSearchCriteria.Items[0]).Criterium as SearchCriteria.HitlistCriteria;
            hlCriteria.HitlistType = sr.HitListInfo.HitListType;
            hlCriteria.Value = sr.HitListInfo.HitListID.ToString();

            _pagingInfo = GetFullPagingInfo();
            DataSet ds = GetDataSet(noHitlist, noPaging);
            Assert.IsTrue(ds != null && sr.ResultsDataSet.Tables.Count > 0);
            Assert.IsTrue(ds.Tables["Table_985"].Rows.Count == 18, "Expected 18 records, but got " + ds.Tables["Table_985"].Rows.Count);
        }

        /// <summary>
        /// DV1
        /// -table1 assayedcompounds
        ///  -field pubchem_compound_cid pk
        ///  -field base64_cdx
        ///  -field pubchem_molecular_formula
        ///  -field reg_batch
        /// 
        /// DV2
        /// -table1 AID741MRP
        ///  -field pubchem_sid pk
        ///  -field toxicity enhacement factor
        ///  -field pubhem_cid
        ///  -field ar_number
        /// 
        /// Search using dv1 for molweightcontains C17.
        /// Use that temporary hitlist to join with DV2 using no source field. That is hitlist.ID = dv2.pubchem_cid
        /// This is helpful for joining dataviews where the primary key of the source dv is present in the target dv.
        /// 
        /// This method pages the results and use hitlists.
        /// </summary>
        [Test]
        public void JoinThroughBaseTablesNoSourceFieldTestSearchTest()
        {
            JoinThroughBaseTablesNoSourceFieldTest(false, false);
        }

        /// <summary>
        /// DV1
        /// -table1 assayedcompounds
        ///  -field pubchem_compound_cid pk
        ///  -field base64_cdx
        ///  -field pubchem_molecular_formula
        ///  -field reg_batch
        /// 
        /// DV2
        /// -table1 AID741MRP
        ///  -field pubchem_sid pk
        ///  -field toxicity enhacement factor
        ///  -field pubhem_cid
        ///  -field ar_number
        /// 
        /// Search using dv1 for molweightcontains C17.
        /// Use that temporary hitlist to join with DV2 using no source field. That is hitlist.ID = dv2.pubchem_cid
        /// This is helpful for joining dataviews where the primary key of the source dv is present in the target dv.
        /// 
        /// This method pages the results, but do not use hitlists.
        /// </summary>
        [Test]
        public void JoinThroughBaseTablesNoSourceFieldTestNoHitlistTest()
        {
            JoinThroughBaseTablesNoSourceFieldTest(true, false);
        }

        /// <summary>
        /// DV1
        /// -table1 assayedcompounds
        ///  -field pubchem_compound_cid pk
        ///  -field base64_cdx
        ///  -field pubchem_molecular_formula
        ///  -field reg_batch
        /// 
        /// DV2
        /// -table1 AID741MRP
        ///  -field pubchem_sid pk
        ///  -field toxicity enhacement factor
        ///  -field pubhem_cid
        ///  -field ar_number
        /// 
        /// Search using dv1 for molweightcontains C17.
        /// Use that temporary hitlist to join with DV2 using no source field. That is hitlist.ID = dv2.pubchem_cid
        /// This is helpful for joining dataviews where the primary key of the source dv is present in the target dv.
        /// 
        /// This method don't pages the results and do not use hitlists.
        /// </summary>
        [Test]
        public void JoinThroughBaseTablesNoSourceFieldTestNoHitlistNoPagingTest()
        {
            JoinThroughBaseTablesNoSourceFieldTest(true, true);
        }

        private void JoinThroughBaseTablesNoSourceFieldTest(bool noHitlist, bool noPaging)
        {
            SearchResponse sr = null;
            _searchByHitlistSearchCriteria.GetFromXML(@"<searchCriteria xmlns=""COE.SearchCriteria"">
<searchCriteriaItem id=""1""
				 fieldid=""993""
				 modifier=""""
				 tableid=""985"">
<hitlistCriteria></hitlistCriteria>
</searchCriteriaItem>
<searchCriteriaID>1</searchCriteriaID>
</searchCriteria>");
            sr = _searchService.DoSearch(_sourceSearchCriteria, _sourceResultsCriteria, _pagingInfo, _sourceDataviewID);
            SearchCriteria.HitlistCriteria hlCriteria = ((SearchCriteria.SearchCriteriaItem)_searchByHitlistSearchCriteria.Items[0]).Criterium as SearchCriteria.HitlistCriteria;
            hlCriteria.HitlistType = sr.HitListInfo.HitListType;
            hlCriteria.Value = sr.HitListInfo.HitListID.ToString();

            _pagingInfo = GetFullPagingInfo();
            DataSet ds = GetDataSet(noHitlist, noPaging);
            Assert.IsTrue(ds != null && sr.ResultsDataSet.Tables.Count > 0);
            Assert.IsTrue(ds.Tables["Table_985"].Rows.Count == 18, "Expected 18 records, but got " + ds.Tables["Table_985"].Rows.Count);
        }

        /// <summary>
        /// DV1
        /// -table1 assayedcompounds
        ///  -field pubchem_compound_cid pk
        ///  -field base64_cdx
        ///  -field pubchem_molecular_formula
        ///  -field reg_batch
        /// 
        /// DV2
        /// -table1 AID741MRP
        ///  -field pubchem_sid pk
        ///  -field toxicity enhacement factor
        ///  -field pubhem_cid
        ///  -field ar_number
        /// 
        /// Search using dv1 for molweightcontains C17.
        /// Use that temporary hitlist to join with DV2 using no source field, cause source fied is the source PK.
        /// That is hitlist.ID = dv2.pubchem_cid
        /// 
        /// This method pages the results and use hitlists.
        /// </summary>
        [Test]
        public void JoinThroughBaseTablesSourceFieldIsPKTestSearchTest()
        {
            JoinThroughBaseTablesSourceFieldIsPKTest(false, false);
        }

        /// <summary>
        /// DV1
        /// -table1 assayedcompounds
        ///  -field pubchem_compound_cid pk
        ///  -field base64_cdx
        ///  -field pubchem_molecular_formula
        ///  -field reg_batch
        /// 
        /// DV2
        /// -table1 AID741MRP
        ///  -field pubchem_sid pk
        ///  -field toxicity enhacement factor
        ///  -field pubhem_cid
        ///  -field ar_number
        /// 
        /// Search using dv1 for molweightcontains C17.
        /// Use that temporary hitlist to join with DV2 using no source field, cause source fied is the source PK.
        /// That is hitlist.ID = dv2.pubchem_cid
        /// 
        /// This method pages the results, but do not use hitlists.
        /// </summary>
        [Test]
        public void JoinThroughBaseTablesSourceFieldIsPKTestNoHitlistTest()
        {
            JoinThroughBaseTablesSourceFieldIsPKTest(true, false);
        }

        /// <summary>
        /// DV1
        /// -table1 assayedcompounds
        ///  -field pubchem_compound_cid pk
        ///  -field base64_cdx
        ///  -field pubchem_molecular_formula
        ///  -field reg_batch
        /// 
        /// DV2
        /// -table1 AID741MRP
        ///  -field pubchem_sid pk
        ///  -field toxicity enhacement factor
        ///  -field pubhem_cid
        ///  -field ar_number
        /// 
        /// Search using dv1 for molweightcontains C17.
        /// Use that temporary hitlist to join with DV2 using no source field, cause source fied is the source PK.
        /// That is hitlist.ID = dv2.pubchem_cid
        /// 
        /// This method don't pages the results and do not use hitlists.
        /// </summary>
        [Test]
        public void JoinThroughBaseTablesSourceFieldIsPKTestNoHitlistNoPagingTest()
        {
            JoinThroughBaseTablesSourceFieldIsPKTest(true, true);
        }

        public void JoinThroughBaseTablesSourceFieldIsPKTest(bool noHitlist, bool noPaging)
        {
            SearchResponse sr = null;
            _searchByHitlistSearchCriteria.GetFromXML(@"<searchCriteria xmlns=""COE.SearchCriteria"">
<searchCriteriaItem id=""1""
				 fieldid=""993""
				 modifier=""""
				 tableid=""985"">
<hitlistCriteria sourceFieldId=""1431"" hitlistType=""TEMP""></hitlistCriteria>
</searchCriteriaItem>
<searchCriteriaID>1</searchCriteriaID>
</searchCriteria>");
            sr = _searchService.DoSearch(_sourceSearchCriteria, _sourceResultsCriteria, _pagingInfo, _sourceDataviewID);
            SearchCriteria.HitlistCriteria hlCriteria = ((SearchCriteria.SearchCriteriaItem)_searchByHitlistSearchCriteria.Items[0]).Criterium as SearchCriteria.HitlistCriteria;
            hlCriteria.HitlistType = sr.HitListInfo.HitListType;
            hlCriteria.Value = sr.HitListInfo.HitListID.ToString();

            _pagingInfo = GetFullPagingInfo();

            DataSet ds = GetDataSet(noHitlist, noPaging);
            Assert.IsTrue(ds != null && sr.ResultsDataSet.Tables.Count > 0);
            Assert.IsTrue(ds.Tables["Table_985"].Rows.Count == 18, "Expected 18 records, but got " + ds.Tables["Table_985"].Rows.Count);
        }

        /// <summary>
        /// DV1
        /// -table assayedcompounds
        ///  -field pubchem_compound_cid pk
        ///  -field base64_cdx
        ///  -field pubchem_molecular_formula
        ///  -field reg_batch
        /// -table AID629ERA
        ///  -field % Inhibition
        ///  -field pubchem_cid pk
        ///  -field pubchem_sid
        ///  -field ar_number
        /// 
        /// DV2
        /// -table1 AID741MRP
        ///  -field pubchem_sid pk
        ///  -field toxicity enhacement factor
        ///  -field pubchem_cid
        ///  -field ar_number
        /// 
        /// Search using dv1 for molweightcontains C17.
        /// Use that temporary hitlist to join with DV2 using dv1.AID629ERA.ar_number = dv2.AID741MRP.ar_number
        /// 
        /// This method pages the results and use hitlists.
        /// </summary>
        [Test]
        public void JoinChildSourceBaseTableTargetTestSearchTes()
        {
            JoinChildSourceBaseTableTargetTest(false, false);
        }

        /// <summary>
        /// DV1
        /// -table assayedcompounds
        ///  -field pubchem_compound_cid pk
        ///  -field base64_cdx
        ///  -field pubchem_molecular_formula
        ///  -field reg_batch
        /// -table AID629ERA
        ///  -field % Inhibition
        ///  -field pubchem_cid pk
        ///  -field pubchem_sid
        ///  -field ar_number
        /// 
        /// DV2
        /// -table1 AID741MRP
        ///  -field pubchem_sid pk
        ///  -field toxicity enhacement factor
        ///  -field pubchem_cid
        ///  -field ar_number
        /// 
        /// Search using dv1 for molweightcontains C17.
        /// Use that temporary hitlist to join with DV2 using dv1.AID629ERA.ar_number = dv2.AID741MRP.ar_number
        /// 
        /// This method pages the results, but do not use hitlists.
        /// </summary>
        [Test]
        public void JoinChildSourceBaseTableTargetTestNoHitlistTest()
        {
            JoinChildSourceBaseTableTargetTest(true, false);
        }

        /// <summary>
        /// DV1
        /// -table assayedcompounds
        ///  -field pubchem_compound_cid pk
        ///  -field base64_cdx
        ///  -field pubchem_molecular_formula
        ///  -field reg_batch
        /// -table AID629ERA
        ///  -field % Inhibition
        ///  -field pubchem_cid pk
        ///  -field pubchem_sid
        ///  -field ar_number
        /// 
        /// DV2
        /// -table1 AID741MRP
        ///  -field pubchem_sid pk
        ///  -field toxicity enhacement factor
        ///  -field pubchem_cid
        ///  -field ar_number
        /// 
        /// Search using dv1 for molweightcontains C17.
        /// Use that temporary hitlist to join with DV2 using dv1.AID629ERA.ar_number = dv2.AID741MRP.ar_number
        /// 
        /// This method don't pages the results and do not use hitlists.
        /// </summary>
        [Test]
        public void JoinChildSourceBaseTableTargetTestNoHitlistNoPagingTest()
        {
            JoinChildSourceBaseTableTargetTest(true, true);
        }

        private void JoinChildSourceBaseTableTargetTest(bool noHitlist, bool noPaging)
        {
            SearchResponse sr = null;
            _searchByHitlistSearchCriteria.GetFromXML(@"<searchCriteria xmlns=""COE.SearchCriteria"">
<searchCriteriaItem id=""1""
				 fieldid=""987""
				 modifier=""""
				 tableid=""985"">
<hitlistCriteria sourceFieldId=""1908"" hitlistType=""TEMP""></hitlistCriteria>
</searchCriteriaItem>
<searchCriteriaID>1</searchCriteriaID>
</searchCriteria>");
            sr = _searchService.DoSearch(_sourceSearchCriteria, _sourceResultsCriteria, _pagingInfo, _sourceDataviewID);
            SearchCriteria.HitlistCriteria hlCriteria = ((SearchCriteria.SearchCriteriaItem)_searchByHitlistSearchCriteria.Items[0]).Criterium as SearchCriteria.HitlistCriteria;
            hlCriteria.HitlistType = sr.HitListInfo.HitListType;
            hlCriteria.Value = sr.HitListInfo.HitListID.ToString();

            _pagingInfo = GetFullPagingInfo();
            DataSet ds = GetDataSet(noHitlist, noPaging);
            Assert.IsTrue(ds != null && sr.ResultsDataSet.Tables.Count > 0);
            Assert.IsTrue(ds.Tables["Table_985"].Rows.Count == 17, "Expected 17 records, but got " + ds.Tables["Table_985"].Rows.Count);
        }

        /// <summary>
        /// DV1
        /// -table assayedcompounds
        ///  -field pubchem_compound_cid pk
        ///  -field base64_cdx
        ///  -field pubchem_molecular_formula
        ///  -field reg_batch
        /// -table AID629ERA
        ///  -field % Inhibition
        ///  -field pubchem_cid pk
        ///  -field pubchem_sid
        ///  -field ar_number
        /// 
        /// DV2
        /// -table AID741MRP
        ///  -field pubchem_sid pk
        ///  -field toxicity enhacement factor
        ///  -field pubchem_cid
        ///  -field ar_number
        /// -table AID741MRP_LOB
        ///  -field pubchem_cid pk
        ///  -field blob_content
        ///  -field ar_number
        /// 
        /// Search using dv1 for molweightcontains C17.
        /// Use that temporary hitlist to join with DV2 using dv1.AID629ERA.ar_number = dv2.AID741MRP_LOB.ar_number
        /// 
        /// This method pages the results and use hitlists.
        /// </summary>
        [Test]
        public void JoinThroughChildTablesTestSearchTest()
        {
            JoinThroughChildTablesTest(false, false);
        }

        /// <summary>
        /// DV1
        /// -table assayedcompounds
        ///  -field pubchem_compound_cid pk
        ///  -field base64_cdx
        ///  -field pubchem_molecular_formula
        ///  -field reg_batch
        /// -table AID629ERA
        ///  -field % Inhibition
        ///  -field pubchem_cid pk
        ///  -field pubchem_sid
        ///  -field ar_number
        /// 
        /// DV2
        /// -table AID741MRP
        ///  -field pubchem_sid pk
        ///  -field toxicity enhacement factor
        ///  -field pubchem_cid
        ///  -field ar_number
        /// -table AID741MRP_LOB
        ///  -field pubchem_cid pk
        ///  -field blob_content
        ///  -field ar_number
        /// 
        /// Search using dv1 for molweightcontains C17.
        /// Use that temporary hitlist to join with DV2 using dv1.AID629ERA.ar_number = dv2.AID741MRP_LOB.ar_number
        /// 
        /// This method pages the results, but do not use hitlists.
        /// </summary>
        [Test]
        public void JoinThroughChildTablesTestNoHitlistTest()
        {
            JoinThroughChildTablesTest(true, false);
        }

        /// <summary>
        /// DV1
        /// -table assayedcompounds
        ///  -field pubchem_compound_cid pk
        ///  -field base64_cdx
        ///  -field pubchem_molecular_formula
        ///  -field reg_batch
        /// -table AID629ERA
        ///  -field % Inhibition
        ///  -field pubchem_cid pk
        ///  -field pubchem_sid
        ///  -field ar_number
        /// 
        /// DV2
        /// -table AID741MRP
        ///  -field pubchem_sid pk
        ///  -field toxicity enhacement factor
        ///  -field pubchem_cid
        ///  -field ar_number
        /// -table AID741MRP_LOB
        ///  -field pubchem_cid pk
        ///  -field blob_content
        ///  -field ar_number
        /// 
        /// Search using dv1 for molweightcontains C17.
        /// Use that temporary hitlist to join with DV2 using dv1.AID629ERA.ar_number = dv2.AID741MRP_LOB.ar_number
        /// 
        /// This method don't pages the results and do not use hitlists.
        /// </summary>
        [Test]
        public void JoinThroughChildTablesTestNoHitlistNoPagingTest()
        {
            JoinThroughChildTablesTest(true, true);
        }

        private void JoinThroughChildTablesTest(bool noHitlist, bool noPaging)
        {
            SearchResponse sr = null;

            _searchByHitlistSearchCriteria.GetFromXML(@"<searchCriteria xmlns=""COE.SearchCriteria"">
<searchCriteriaItem id=""1""
				 fieldid=""1017""
				 modifier=""""
				 tableid=""1016"">
<hitlistCriteria sourceFieldId=""1908"" hitlistType=""TEMP""></hitlistCriteria>
</searchCriteriaItem>
<searchCriteriaID>1</searchCriteriaID>
</searchCriteria>");
            sr = _searchService.DoSearch(_sourceSearchCriteria, _sourceResultsCriteria, _pagingInfo, _sourceDataviewID);
            SearchCriteria.HitlistCriteria hlCriteria = ((SearchCriteria.SearchCriteriaItem)_searchByHitlistSearchCriteria.Items[0]).Criterium as SearchCriteria.HitlistCriteria;
            hlCriteria.HitlistType = sr.HitListInfo.HitListType;
            hlCriteria.Value = sr.HitListInfo.HitListID.ToString();

            _pagingInfo = GetFullPagingInfo();
            DataSet ds = GetDataSet(noHitlist, noPaging);
            Assert.IsTrue(ds != null && sr.ResultsDataSet.Tables.Count > 0);
            Assert.IsTrue(ds.Tables["Table_985"].Rows.Count == 17, "Expected 17 records, but got " + ds.Tables["Table_985"].Rows.Count);
        }
        #endregion

        #region Test Saved Hitlist Methods
        /// <summary>
        /// DV1
        /// -table1 assayedcompounds
        ///  -field pubchem_compound_cid pk
        ///  -field base64_cdx
        ///  -field pubchem_molecular_formula
        ///  -field reg_batch
        /// 
        /// DV2
        /// -table1 AID741MRP
        ///  -field pubchem_sid pk
        ///  -field toxicity enhacement factor
        ///  -field pubhem_cid
        ///  -field ar_number
        /// 
        /// Search using dv1 for molweightcontains C17.
        /// Use that temporary hitlist to join with DV2 using dv1.reg_batch = dv2.ar_number
        /// 
        /// This method pages the results and use hitlists.
        /// </summary>
        [Test]
        public void JoinThroughBaseTablesSavedHitListTestSearchTest()
        {
            JoinThroughBaseTablesSavedHitListTest(false, false);
        }

        /// <summary>
        /// DV1
        /// -table1 assayedcompounds
        ///  -field pubchem_compound_cid pk
        ///  -field base64_cdx
        ///  -field pubchem_molecular_formula
        ///  -field reg_batch
        /// 
        /// DV2
        /// -table1 AID741MRP
        ///  -field pubchem_sid pk
        ///  -field toxicity enhacement factor
        ///  -field pubhem_cid
        ///  -field ar_number
        /// 
        /// Search using dv1 for molweightcontains C17.
        /// Use that temporary hitlist to join with DV2 using dv1.reg_batch = dv2.ar_number
        /// 
        /// This method pages the results, but do not use hitlists.
        /// </summary>
        [Test]
        public void JoinThroughBaseTablesSavedHitListTestNoHitlistTest()
        {
            JoinThroughBaseTablesSavedHitListTest(true, false);
        }

        /// <summary>
        /// DV1
        /// -table1 assayedcompounds
        ///  -field pubchem_compound_cid pk
        ///  -field base64_cdx
        ///  -field pubchem_molecular_formula
        ///  -field reg_batch
        /// 
        /// DV2
        /// -table1 AID741MRP
        ///  -field pubchem_sid pk
        ///  -field toxicity enhacement factor
        ///  -field pubhem_cid
        ///  -field ar_number
        /// 
        /// Search using dv1 for molweightcontains C17.
        /// Use that temporary hitlist to join with DV2 using dv1.reg_batch = dv2.ar_number
        /// 
        /// This method don't pages the results and do not use hitlists.
        /// </summary>
        [Test]
        public void JoinThroughBaseTablesSavedHitListTestNoHitlistNoPagingTest()
        {
            JoinThroughBaseTablesSavedHitListTest(true, true);
        }

        private void JoinThroughBaseTablesSavedHitListTest(bool noHitlist, bool noPaging)
        {
            SearchResponse sr = _searchService.DoSearch(_sourceSearchCriteria, _sourceResultsCriteria, _pagingInfo, _sourceDataviewID);
            SearchCriteria.HitlistCriteria hlCriteria = ((SearchCriteria.SearchCriteriaItem)_searchByHitlistSearchCriteria.Items[0]).Criterium as SearchCriteria.HitlistCriteria;
            COEHitListBO hlBO = COEHitListBO.Get(sr.HitListInfo.HitListType, sr.HitListInfo.HitListID);
            hlBO = hlBO.Save();
            hlCriteria.HitlistType = hlBO.HitListType;
            hlCriteria.Value = hlBO.HitListID.ToString();

            _pagingInfo = GetFullPagingInfo();

            DataSet ds = GetDataSet(noHitlist, noPaging);

            Assert.IsTrue(ds != null && ds.Tables.Count > 0);
            Assert.IsTrue(ds.Tables["Table_985"].Rows.Count == 18);
        }

        /// <summary>
        /// DV1
        /// -table1 assayedcompounds
        ///  -field pubchem_compound_cid pk
        ///  -field base64_cdx
        ///  -field pubchem_molecular_formula
        ///  -field reg_batch
        /// 
        /// DV2
        /// -table1 AID741MRP
        ///  -field pubchem_sid pk
        ///  -field toxicity enhacement factor
        ///  -field pubhem_cid
        ///  -field ar_number
        /// 
        /// Search using dv1 for molweightcontains C17.
        /// Use that temporary hitlist to join with DV2 using no source field. That is hitlist.ID = dv2.pubchem_cid
        /// This is helpful for joining dataviews where the primary key of the source dv is present in the target dv.
        /// 
        /// This method pages the results and use hitlists.
        /// </summary>
        [Test]
        public void JoinThroughBaseTablesNoSourceFieldSavedHitListTestSearchTest()
        {
            JoinThroughBaseTablesNoSourceFieldSavedHitListTest(false, false);
        }

        /// <summary>
        /// DV1
        /// -table1 assayedcompounds
        ///  -field pubchem_compound_cid pk
        ///  -field base64_cdx
        ///  -field pubchem_molecular_formula
        ///  -field reg_batch
        /// 
        /// DV2
        /// -table1 AID741MRP
        ///  -field pubchem_sid pk
        ///  -field toxicity enhacement factor
        ///  -field pubhem_cid
        ///  -field ar_number
        /// 
        /// Search using dv1 for molweightcontains C17.
        /// Use that temporary hitlist to join with DV2 using no source field. That is hitlist.ID = dv2.pubchem_cid
        /// This is helpful for joining dataviews where the primary key of the source dv is present in the target dv.
        /// 
        /// This method pages the results, but do not use hitlists.
        /// </summary>
        [Test]
        public void JoinThroughBaseTablesNoSourceFieldSavedHitListTestNoHitlistTest()
        {
            JoinThroughBaseTablesNoSourceFieldSavedHitListTest(true, false);
        }

        /// <summary>
        /// DV1
        /// -table1 assayedcompounds
        ///  -field pubchem_compound_cid pk
        ///  -field base64_cdx
        ///  -field pubchem_molecular_formula
        ///  -field reg_batch
        /// 
        /// DV2
        /// -table1 AID741MRP
        ///  -field pubchem_sid pk
        ///  -field toxicity enhacement factor
        ///  -field pubhem_cid
        ///  -field ar_number
        /// 
        /// Search using dv1 for molweightcontains C17.
        /// Use that temporary hitlist to join with DV2 using no source field. That is hitlist.ID = dv2.pubchem_cid
        /// This is helpful for joining dataviews where the primary key of the source dv is present in the target dv.
        /// 
        /// This method don't pages the results and do not use hitlists.
        /// </summary>
        [Test]
        public void JoinThroughBaseTablesNoSourceFieldSavedHitListTestNoHitlistNoPagingTest()
        {
            JoinThroughBaseTablesNoSourceFieldSavedHitListTest(true, true);
        }

        private void JoinThroughBaseTablesNoSourceFieldSavedHitListTest(bool noHitlist, bool noPaging)
        {
            SearchResponse sr = null;
            _searchByHitlistSearchCriteria.GetFromXML(@"<searchCriteria xmlns=""COE.SearchCriteria"">
<searchCriteriaItem id=""1""
				 fieldid=""993""
				 modifier=""""
				 tableid=""985"">
<hitlistCriteria></hitlistCriteria>
</searchCriteriaItem>
<searchCriteriaID>1</searchCriteriaID>
</searchCriteria>");
            sr = _searchService.DoSearch(_sourceSearchCriteria, _sourceResultsCriteria, _pagingInfo, _sourceDataviewID);
            SearchCriteria.HitlistCriteria hlCriteria = ((SearchCriteria.SearchCriteriaItem)_searchByHitlistSearchCriteria.Items[0]).Criterium as SearchCriteria.HitlistCriteria;
            COEHitListBO hlBO = COEHitListBO.Get(sr.HitListInfo.HitListType, sr.HitListInfo.HitListID);
            hlBO = hlBO.Save();
            hlCriteria.HitlistType = hlBO.HitListType;
            hlCriteria.Value = hlBO.HitListID.ToString();

            _pagingInfo = GetFullPagingInfo();
            DataSet ds = GetDataSet(noHitlist, noPaging);

            Assert.IsTrue(ds != null && ds.Tables.Count > 0);
            Assert.IsTrue(ds.Tables["Table_985"].Rows.Count == 18);
        }

        /// <summary>
        /// DV1
        /// -table1 assayedcompounds
        ///  -field pubchem_compound_cid pk
        ///  -field base64_cdx
        ///  -field pubchem_molecular_formula
        ///  -field reg_batch
        /// 
        /// DV2
        /// -table1 AID741MRP
        ///  -field pubchem_sid pk
        ///  -field toxicity enhacement factor
        ///  -field pubhem_cid
        ///  -field ar_number
        /// 
        /// Search using dv1 for molweightcontains C17.
        /// Use that temporary hitlist to join with DV2 using no source field, cause source fied is the source PK.
        /// That is hitlist.ID = dv2.pubchem_cid
        /// 
        /// This method pages the results and use hitlists.
        /// </summary>
        [Test]
        public void JoinThroughBaseTablesSourceFieldIsPKSavedHitListTestSearchTest()
        {
            JoinThroughBaseTablesSourceFieldIsPKSavedHitListTest(false, false);
        }

        /// <summary>
        /// DV1
        /// -table1 assayedcompounds
        ///  -field pubchem_compound_cid pk
        ///  -field base64_cdx
        ///  -field pubchem_molecular_formula
        ///  -field reg_batch
        /// 
        /// DV2
        /// -table1 AID741MRP
        ///  -field pubchem_sid pk
        ///  -field toxicity enhacement factor
        ///  -field pubhem_cid
        ///  -field ar_number
        /// 
        /// Search using dv1 for molweightcontains C17.
        /// Use that temporary hitlist to join with DV2 using no source field, cause source fied is the source PK.
        /// That is hitlist.ID = dv2.pubchem_cid
        /// 
        /// This method pages the results, but do not use hitlists.
        /// </summary>
        [Test]
        public void JoinThroughBaseTablesSourceFieldIsPKSavedHitListTestNoHitlistTest()
        {
            JoinThroughBaseTablesSourceFieldIsPKSavedHitListTest(true, false);
        }

        /// <summary>
        /// DV1
        /// -table1 assayedcompounds
        ///  -field pubchem_compound_cid pk
        ///  -field base64_cdx
        ///  -field pubchem_molecular_formula
        ///  -field reg_batch
        /// 
        /// DV2
        /// -table1 AID741MRP
        ///  -field pubchem_sid pk
        ///  -field toxicity enhacement factor
        ///  -field pubhem_cid
        ///  -field ar_number
        /// 
        /// Search using dv1 for molweightcontains C17.
        /// Use that temporary hitlist to join with DV2 using no source field, cause source fied is the source PK.
        /// That is hitlist.ID = dv2.pubchem_cid
        /// 
        /// This method don't pages the results and do not use hitlists.
        /// </summary>
        [Test]
        public void JoinThroughBaseTablesSourceFieldIsPKSavedHitListTestNoHitlistNoPagingTest()
        {
            JoinThroughBaseTablesSourceFieldIsPKSavedHitListTest(true, true);
        }

        private void JoinThroughBaseTablesSourceFieldIsPKSavedHitListTest(bool noHitlist, bool noPaging)
        {
            SearchResponse sr = null;
            _searchByHitlistSearchCriteria.GetFromXML(@"<searchCriteria xmlns=""COE.SearchCriteria"">
<searchCriteriaItem id=""1""
				 fieldid=""993""
				 modifier=""""
				 tableid=""985"">
<hitlistCriteria sourceFieldId=""1431"" hitlistType=""TEMP""></hitlistCriteria>
</searchCriteriaItem>
<searchCriteriaID>1</searchCriteriaID>
</searchCriteria>");
            sr = _searchService.DoSearch(_sourceSearchCriteria, _sourceResultsCriteria, _pagingInfo, _sourceDataviewID);
            SearchCriteria.HitlistCriteria hlCriteria = ((SearchCriteria.SearchCriteriaItem)_searchByHitlistSearchCriteria.Items[0]).Criterium as SearchCriteria.HitlistCriteria;
            COEHitListBO hlBO = COEHitListBO.Get(sr.HitListInfo.HitListType, sr.HitListInfo.HitListID);
            hlBO = hlBO.Save();
            hlCriteria.HitlistType = hlBO.HitListType;
            hlCriteria.Value = hlBO.HitListID.ToString();

            _pagingInfo = GetFullPagingInfo();
            DataSet ds = GetDataSet(noHitlist, noPaging);
            Assert.IsTrue(ds != null && ds.Tables.Count > 0);
            Assert.IsTrue(ds.Tables["Table_985"].Rows.Count == 18);
        }

        /// <summary>
        /// DV1
        /// -table assayedcompounds
        ///  -field pubchem_compound_cid pk
        ///  -field base64_cdx
        ///  -field pubchem_molecular_formula
        ///  -field reg_batch
        /// -table AID629ERA
        ///  -field % Inhibition
        ///  -field pubchem_cid pk
        ///  -field pubchem_sid
        ///  -field ar_number
        /// 
        /// DV2
        /// -table1 AID741MRP
        ///  -field pubchem_sid pk
        ///  -field toxicity enhacement factor
        ///  -field pubchem_cid
        ///  -field ar_number
        /// 
        /// Search using dv1 for molweightcontains C17.
        /// Use that temporary hitlist to join with DV2 using dv1.AID629ERA.ar_number = dv2.AID741MRP.ar_number
        /// 
        /// This method pages the results and use hitlists.
        /// </summary>
        [Test]
        public void JoinChildSourceBaseTableTargetSavedHitListTestSearchTest()
        {
            JoinChildSourceBaseTableTargetSavedHitListTest(false, false);
        }

        /// <summary>
        /// DV1
        /// -table assayedcompounds
        ///  -field pubchem_compound_cid pk
        ///  -field base64_cdx
        ///  -field pubchem_molecular_formula
        ///  -field reg_batch
        /// -table AID629ERA
        ///  -field % Inhibition
        ///  -field pubchem_cid pk
        ///  -field pubchem_sid
        ///  -field ar_number
        /// 
        /// DV2
        /// -table1 AID741MRP
        ///  -field pubchem_sid pk
        ///  -field toxicity enhacement factor
        ///  -field pubchem_cid
        ///  -field ar_number
        /// 
        /// Search using dv1 for molweightcontains C17.
        /// Use that temporary hitlist to join with DV2 using dv1.AID629ERA.ar_number = dv2.AID741MRP.ar_number
        /// 
        /// This method pages the results, but do not use hitlists.
        /// </summary>
        [Test]
        public void JoinChildSourceBaseTableTargetSavedHitListTestNoHitlistTest()
        {
            JoinChildSourceBaseTableTargetSavedHitListTest(true, false);
        }

        /// <summary>
        /// DV1
        /// -table assayedcompounds
        ///  -field pubchem_compound_cid pk
        ///  -field base64_cdx
        ///  -field pubchem_molecular_formula
        ///  -field reg_batch
        /// -table AID629ERA
        ///  -field % Inhibition
        ///  -field pubchem_cid pk
        ///  -field pubchem_sid
        ///  -field ar_number
        /// 
        /// DV2
        /// -table1 AID741MRP
        ///  -field pubchem_sid pk
        ///  -field toxicity enhacement factor
        ///  -field pubchem_cid
        ///  -field ar_number
        /// 
        /// Search using dv1 for molweightcontains C17.
        /// Use that temporary hitlist to join with DV2 using dv1.AID629ERA.ar_number = dv2.AID741MRP.ar_number
        /// 
        /// This method don't pages the results and do not use hitlists.
        /// </summary>
        [Test]
        public void JoinChildSourceBaseTableTargetSavedHitListTestNoHitlistNoPagingTest()
        {
            JoinChildSourceBaseTableTargetSavedHitListTest(true, true);
        }

        private void JoinChildSourceBaseTableTargetSavedHitListTest(bool noHitlist, bool noPaging)
        {
            SearchResponse sr = null;
            _searchByHitlistSearchCriteria.GetFromXML(@"<searchCriteria xmlns=""COE.SearchCriteria"">
<searchCriteriaItem id=""1""
				 fieldid=""987""
				 modifier=""""
				 tableid=""985"">
<hitlistCriteria sourceFieldId=""1908"" hitlistType=""TEMP""></hitlistCriteria>
</searchCriteriaItem>
<searchCriteriaID>1</searchCriteriaID>
</searchCriteria>");
            sr = _searchService.DoSearch(_sourceSearchCriteria, _sourceResultsCriteria, _pagingInfo, _sourceDataviewID);
            SearchCriteria.HitlistCriteria hlCriteria = ((SearchCriteria.SearchCriteriaItem)_searchByHitlistSearchCriteria.Items[0]).Criterium as SearchCriteria.HitlistCriteria;
            COEHitListBO hlBO = COEHitListBO.Get(sr.HitListInfo.HitListType, sr.HitListInfo.HitListID);
            hlBO = hlBO.Save();
            hlCriteria.HitlistType = hlBO.HitListType;
            hlCriteria.Value = hlBO.HitListID.ToString();

            _pagingInfo = GetFullPagingInfo();
            DataSet ds = GetDataSet(noHitlist, noPaging);
            Assert.IsTrue(ds != null && ds.Tables.Count > 0);
            Assert.IsTrue(ds.Tables["Table_985"].Rows.Count == 17, "Expected 17 records, but got " + ds.Tables["Table_985"].Rows.Count);
        }

        /// <summary>
        /// DV1
        /// -table assayedcompounds
        ///  -field pubchem_compound_cid pk
        ///  -field base64_cdx
        ///  -field pubchem_molecular_formula
        ///  -field reg_batch
        /// -table AID629ERA
        ///  -field % Inhibition
        ///  -field pubchem_cid pk
        ///  -field pubchem_sid
        ///  -field ar_number
        /// 
        /// DV2
        /// -table AID741MRP
        ///  -field pubchem_sid pk
        ///  -field toxicity enhacement factor
        ///  -field pubchem_cid
        ///  -field ar_number
        /// -table AID741MRP_LOB
        ///  -field pubchem_cid pk
        ///  -field blob_content
        ///  -field ar_number
        /// 
        /// Search using dv1 for molweightcontains C17.
        /// Use that temporary hitlist to join with DV2 using dv1.AID629ERA.ar_number = dv2.AID741MRP_LOB.ar_number
        /// 
        /// This method pages the results and use hitlists.
        /// </summary>
        [Test]
        public void JoinThroughChildTablesSavedHitListTestSearchTest()
        {
            JoinThroughChildTablesSavedHitListTest(false, false);
        }

        /// <summary>
        /// DV1
        /// -table assayedcompounds
        ///  -field pubchem_compound_cid pk
        ///  -field base64_cdx
        ///  -field pubchem_molecular_formula
        ///  -field reg_batch
        /// -table AID629ERA
        ///  -field % Inhibition
        ///  -field pubchem_cid pk
        ///  -field pubchem_sid
        ///  -field ar_number
        /// 
        /// DV2
        /// -table AID741MRP
        ///  -field pubchem_sid pk
        ///  -field toxicity enhacement factor
        ///  -field pubchem_cid
        ///  -field ar_number
        /// -table AID741MRP_LOB
        ///  -field pubchem_cid pk
        ///  -field blob_content
        ///  -field ar_number
        /// 
        /// Search using dv1 for molweightcontains C17.
        /// Use that temporary hitlist to join with DV2 using dv1.AID629ERA.ar_number = dv2.AID741MRP_LOB.ar_number
        /// 
        /// This method pages the results, but do not use hitlists.
        /// </summary>
        [Test]
        public void JoinThroughChildTablesSavedHitListTestoHitlistTest()
        {
            JoinThroughChildTablesSavedHitListTest(true, false);
        }

        /// <summary>
        /// DV1
        /// -table assayedcompounds
        ///  -field pubchem_compound_cid pk
        ///  -field base64_cdx
        ///  -field pubchem_molecular_formula
        ///  -field reg_batch
        /// -table AID629ERA
        ///  -field % Inhibition
        ///  -field pubchem_cid pk
        ///  -field pubchem_sid
        ///  -field ar_number
        /// 
        /// DV2
        /// -table AID741MRP
        ///  -field pubchem_sid pk
        ///  -field toxicity enhacement factor
        ///  -field pubchem_cid
        ///  -field ar_number
        /// -table AID741MRP_LOB
        ///  -field pubchem_cid pk
        ///  -field blob_content
        ///  -field ar_number
        /// 
        /// Search using dv1 for molweightcontains C17.
        /// Use that temporary hitlist to join with DV2 using dv1.AID629ERA.ar_number = dv2.AID741MRP_LOB.ar_number
        /// 
        /// This method don't pages the results and do not use hitlists.
        /// </summary>
        [Test]
        public void JoinThroughChildTablesSavedHitListTestNoHitlistNoPagingTest()
        {
            JoinThroughChildTablesSavedHitListTest(true, true);
        }

        private void JoinThroughChildTablesSavedHitListTest(bool noHitlist, bool noPaging)
        {
            SearchResponse sr = null;
            _searchByHitlistSearchCriteria.GetFromXML(@"<searchCriteria xmlns=""COE.SearchCriteria"">
<searchCriteriaItem id=""1""
				 fieldid=""1017""
				 modifier=""""
				 tableid=""1016"">
<hitlistCriteria sourceFieldId=""1908"" hitlistType=""TEMP""></hitlistCriteria>
</searchCriteriaItem>
<searchCriteriaID>1</searchCriteriaID>
</searchCriteria>");
            sr = _searchService.DoSearch(_sourceSearchCriteria, _sourceResultsCriteria, _pagingInfo, _sourceDataviewID);
            SearchCriteria.HitlistCriteria hlCriteria = ((SearchCriteria.SearchCriteriaItem)_searchByHitlistSearchCriteria.Items[0]).Criterium as SearchCriteria.HitlistCriteria;
            COEHitListBO hlBO = COEHitListBO.Get(sr.HitListInfo.HitListType, sr.HitListInfo.HitListID);
            hlBO = hlBO.Save();
            hlCriteria.HitlistType = hlBO.HitListType;
            hlCriteria.Value = hlBO.HitListID.ToString();

            _pagingInfo = GetFullPagingInfo();
            DataSet ds = GetDataSet(noHitlist, noPaging);
            Assert.IsTrue(ds != null && ds.Tables.Count > 0);
            Assert.IsTrue(ds.Tables["Table_985"].Rows.Count == 17, "Expected 17 records, but got " + ds.Tables["Table_985"].Rows.Count);
        }
        #endregion

        #region Private Methods
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

        private static COEDataView GetDataView(string fileName)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(_pathToXmls + "\\" + fileName);
            COEDataView dataview = new COEDataView(doc);
            return dataview;
        }

        private COEDataView GetDataView()
        {
            return GetDataView("DataView.xml");
        }

        private static int InsertDataview(COEDataView dataView)
        {
            COEDataViewBO dv = COEDataViewBO.New(dataView.Name, dataView.Description, dataView, null);
            dv.ID = dataView.DataViewID;
            dv.IsPublic = true;

            dv = dv.Save();
            return dv.ID;
        }

        private DataSet GetDataSet(bool noHitlist, bool noPaging)
        {
            DataSet ds = new DataSet();
            if (noPaging)
            {
                _pagingInfo.End = _pagingInfo.RecordCount = 0;
            }

            if (noHitlist)
            {
                ds = _searchService.GetData(_targetResultsCriteria, _pagingInfo, _targetDataviewID, "NO", null, _searchByHitlistSearchCriteria);
            }
            else
            {
                HitListInfo info = _searchService.GetHitList(_searchByHitlistSearchCriteria, _targetDataviewID);
                _pagingInfo.HitListID = info.HitListID;
                _pagingInfo.HitListType = info.HitListType;


                ds = _searchService.GetData(_targetResultsCriteria, _pagingInfo, _targetDataviewID);
            }

            return ds;
        }
        #endregion
    }
}
