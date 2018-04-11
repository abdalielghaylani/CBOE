using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CambridgeSoft.COE.Framework.Common;
using System.Xml;
using CambridgeSoft.COE.Framework.COESearchService;
using CambridgeSoft.COE.Framework.COESecurityService;
using System.Data;
using CambridgeSoft.COE.Framework.UnitTests.Helpers;

namespace SearchUnitTests
{
    /// <summary>
    /// Summary description for FilterChildDataTest
    /// </summary>
    [TestClass]
    public class FilterChildDataTest
    {
        private COESearch _searchService;
        private int _dataViewID = 0;
        private ResultsCriteria _resultsCriteria;
        private PagingInfo _pagingInfo;
        private string _pathToXmls;

        public FilterChildDataTest()
        {
            _searchService = new COESearch();
            _pathToXmls = SearchHelper.GetExecutingTestResultsBasePath(SearchHelper._FilterChildDataTestpathToXml);
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{

        //}
        //
        // Use TestInitialize to run code before running each test 
        [TestInitialize()]
        public void MyTestInitialize()
        {
            _pagingInfo = GetPagingInfo();
            _resultsCriteria = GetResultsCriteria();
            Login();
            SearchHelper.CreateDataView(SearchHelper._FilterChildDataTestDvPath, SearchHelper._FilterChildDataTestDv);
            string saveQueryHistory = CambridgeSoft.COE.Framework.COEConfigurationService.ConfigurationUtilities.GetApplicationDefaultsData().SearchServiceData.SaveQueryHistory;
            _dataViewID = SearchHelper._FilterChildDataTestDv;
            if (string.IsNullOrEmpty(saveQueryHistory) || (saveQueryHistory.ToLower() != "yes" && saveQueryHistory != "true"))
                throw new Exception("SaveQueryHistory must be enabled in order to filter child data");
        }
        //
        // Use TestCleanup to run code after each test has run
        [TestCleanup()]
        public void MyTestCleanup()
        {
            SearchHelper.DeleteDataView(SearchHelper._FilterChildDataTestDv);
        }

        #endregion

        #region Test Methods
        [TestMethod]
        public void FilterChildDataSearchOverParent_Search()
        {
            FilterChildDataSearchOverParent(false, false);
        }

        [TestMethod]
        public void FilterChildDataSearchOverParent_NoHitlist()
        {
            FilterChildDataSearchOverParent(true, false);
        }

        [TestMethod]
        public void FilterChildDataSearchOverParent_NoHitlistNoPaging()
        {
            FilterChildDataSearchOverParent(true, true);
        }

        private void FilterChildDataSearchOverParent(bool noHitlist, bool noPaging)
        {
            SearchCriteria criteria = new SearchCriteria();
            criteria.SearchCriteriaID = 1;
            SearchCriteria.SearchCriteriaItem item = new SearchCriteria.SearchCriteriaItem();
            item.FieldId = 1430; //Structure
            item.TableId = 1429; //Samples
            SearchCriteria.StructureCriteria structure = new SearchCriteria.StructureCriteria();
            structure.Implementation = "CsCartridge";
            structure.Structure = "OC(C1CCC(CCCCC)CC1)=O";
            structure.FullSearch = SearchCriteria.COEBoolean.Yes;
            item.Criterium = structure;
            criteria.Items.Add(item);
            DataSet ds = GetDataSet(noHitlist, noPaging, criteria);
            string message = ds.ExtendedProperties["FilteredChildDataMessage"] as string;
            Assert.AreEqual(1, ds.Tables[0].Rows.Count, "Expected 1 records and returned " + ds.Tables[0].Rows.Count + " in FilterChildDataSearchOverParent");
            Assert.IsFalse(string.IsNullOrEmpty(message), "FilteredChildDataMessage not filled in when no search over child was provided in FilterChildDataSearchOverParent");
            Assert.IsTrue((bool)ds.ExtendedProperties["FilteredChildData"], "FilteredChildData was returned as false when no search over child was provided in FilterChildDataSearchOverParent");

            _pagingInfo = GetPagingInfo();
            _pagingInfo.FilterChildData = false;
            ds = GetDataSet(noHitlist, noPaging, criteria);
            message = ds.ExtendedProperties["FilteredChildDataMessage"] as string;
            Assert.AreEqual(1, ds.Tables[0].Rows.Count, "Expected 1 records and returned " + ds.Tables[0].Rows.Count + " in FilterChildDataSearchOverParent");
            Assert.IsTrue(string.IsNullOrEmpty(message), "FilteredChildDataMessage filled in when FilterChildData was not requested in FilterChildDataSearchOverParent");
            Assert.IsFalse((bool)ds.ExtendedProperties["FilteredChildData"], "FilteredChildData was returned as true when FilterChildData was not requested in FilterChildDataSearchOverParent");
        }

        [TestMethod]
        public void FilterChildDataSearchOverOneChild_Search()
        {
            FilterChildDataSearchOverOneChild(false, false);
        }

        [TestMethod]
        public void FilterChildDataSearchOverOneChild_NoHitlist()
        {
            FilterChildDataSearchOverOneChild(true, false);
        }

        [TestMethod]
        public void FilterChildDataSearchOverOneChild_NoHitlistNoPaging()
        {
            FilterChildDataSearchOverOneChild(true, true);
        }

        private void FilterChildDataSearchOverOneChild(bool noHitlist, bool noPaging)
        {
            SearchCriteria criteria = new SearchCriteria();
            criteria.SearchCriteriaID = 1;
            SearchCriteria.SearchCriteriaItem item = new SearchCriteria.SearchCriteriaItem();
            item.FieldId = 1903; //Pct Inhibition
            item.TableId = 1901; //ERalpha Primary Screen
            SearchCriteria.NumericalCriteria pctIn = new SearchCriteria.NumericalCriteria();
            pctIn.Operator = SearchCriteria.COEOperators.EQUAL;
            pctIn.Value = "11.11";
            item.Criterium = pctIn;
            criteria.Items.Add(item);
            DataSet ds = GetDataSet(noHitlist, noPaging, criteria);
            string message = ds.ExtendedProperties["FilteredChildDataMessage"] as string;

            Assert.AreEqual(39, ds.Tables[0].Rows.Count, "Expected 39 records and returned " + ds.Tables[0].Rows.Count + " in FilterChildDataSearchOverOneChild");
            Assert.AreEqual(0, ds.Tables["Table_1901"].Select("[Pct Inhibition] <> 11.11").Length, ds.Tables[0].Rows.Count, "Child data not filtered on " + ds.Tables[0].Rows.Count + " in FilterChildDataSearchOverOneChild");
            Assert.AreEqual(39, ds.Tables["Table_1901"].Select("[Pct Inhibition] = 11.11").Length, ds.Tables[0].Rows.Count, "Child data not filtered on " + ds.Tables[0].Rows.Count + " in FilterChildDataSearchOverOneChild");
            Assert.IsTrue(string.IsNullOrEmpty(message), "FilteredChildDataMessage was filled in when search over child was provided in FilterChildDataSearchOverOneChild");
            Assert.IsTrue((bool)ds.ExtendedProperties["FilteredChildData"], "FilteredChildData was returned as false when no search over child was provided in FilterChildDataSearchOverOneChild");

            _pagingInfo = GetPagingInfo();
            _pagingInfo.FilterChildData = false;
            ds = GetDataSet(noHitlist, noPaging, criteria);
            message = ds.ExtendedProperties["FilteredChildDataMessage"] as string;
            Assert.AreEqual(39, ds.Tables[0].Rows.Count, "Expected 39 records and returned " + ds.Tables[0].Rows.Count + " in FilterChildDataSearchOverOneChild");
            Assert.AreNotEqual(0, ds.Tables["Table_1901"].Select("[Pct Inhibition] <> 11.11").Length);
            Assert.AreEqual(39, ds.Tables["Table_1901"].Select("[Pct Inhibition] = 11.11").Length);
            Assert.IsTrue(string.IsNullOrEmpty(message), "FilteredChildDataMessage was filled in when FilterChildData was passed as false in FilterChildDataSearchOverOneChild");
            Assert.IsFalse((bool)ds.ExtendedProperties["FilteredChildData"], "FilteredChildData was returned as true when initially passed as false in FilterChildDataSearchOverOneChild");
        }

        [TestMethod]
        public void FilterChildDataSearchOverTwoChild_Search()
        {
            FilterChildDataSearchOverTwoChild(false, false);
        }

        [TestMethod]
        public void FilterChildDataSearchOverTwoChild_NoHitlist()
        {
            FilterChildDataSearchOverTwoChild(true, false);
        }

        [TestMethod]
        public void FilterChildDataSearchOverTwoChild_NoHitlistNoPaging()
        {
            FilterChildDataSearchOverTwoChild(true, true);
        }

        private void FilterChildDataSearchOverTwoChild(bool noHitlist, bool noPaging)
        {
            SearchCriteria criteria = new SearchCriteria();
            criteria.SearchCriteriaID = 1;
            SearchCriteria.SearchCriteriaItem pctInItem = new SearchCriteria.SearchCriteriaItem();
            pctInItem.FieldId = 1903; //Pct Inhibition
            pctInItem.TableId = 1901; //ERalpha Primary Screen
            SearchCriteria.NumericalCriteria pctIn = new SearchCriteria.NumericalCriteria();
            pctIn.Operator = SearchCriteria.COEOperators.LT;
            pctIn.Value = "91";
            pctInItem.Criterium = pctIn;
            criteria.Items.Add(pctInItem);
            SearchCriteria.SearchCriteriaItem ec50Item = new SearchCriteria.SearchCriteriaItem();
            ec50Item.FieldId = 1881; //EC50 (1 nM E2) 
            ec50Item.TableId = 1879; //ERalpha Dose Response
            SearchCriteria.NumericalCriteria ec50 = new SearchCriteria.NumericalCriteria();
            ec50.Operator = SearchCriteria.COEOperators.GTE;
            ec50.Value = "1.05";
            ec50Item.Criterium = ec50;
            criteria.Items.Add(ec50Item);
            DataSet ds = GetDataSet(noHitlist, noPaging, criteria);
            string message = ds.ExtendedProperties["FilteredChildDataMessage"] as string;

            Assert.AreEqual(2, ds.Tables[0].Rows.Count, "Expected 2 records and returned " + ds.Tables[0].Rows.Count + " in FilterChildDataSearchOverTwoChild");
            Assert.AreEqual(0, ds.Tables["Table_1901"].Select("[Pct Inhibition] >= 91").Length, ds.Tables[0].Rows.Count, "Child data not filtered on " + ds.Tables[0].Rows.Count + " in FilterChildDataSearchOverTwoChild");
            Assert.AreEqual(3, ds.Tables["Table_1901"].Select("[Pct Inhibition] < 91").Length, ds.Tables[0].Rows.Count, "Child data not filtered on " + ds.Tables[0].Rows.Count + " in FilterChildDataSearchOverTwoChild");
            Assert.AreEqual(0, ds.Tables["Table_1879"].Select("[EC50 (1 nM E2)] < 1.05").Length, ds.Tables[0].Rows.Count, "Child data not filtered on " + ds.Tables[0].Rows.Count + " in FilterChildDataSearchOverTwoChild");
            Assert.AreEqual(2, ds.Tables["Table_1879"].Select("[EC50 (1 nM E2)] >= 1.05").Length, ds.Tables[0].Rows.Count, "Child data not filtered on " + ds.Tables[0].Rows.Count + " in FilterChildDataSearchOverTwoChild");
            Assert.IsTrue(string.IsNullOrEmpty(message), "FilteredChildDataMessage was filled in when search over child was provided in FilterChildDataSearchOverTwoChild");
            Assert.IsTrue((bool)ds.ExtendedProperties["FilteredChildData"], "FilteredChildData was returned as false when no search over child was provided in FilterChildDataSearchOverTwoChild");

            _pagingInfo = GetPagingInfo();
            _pagingInfo.FilterChildData = false;
            ds = GetDataSet(noHitlist, noPaging, criteria);
            message = ds.ExtendedProperties["FilteredChildDataMessage"] as string;
            Assert.AreEqual(2, ds.Tables[0].Rows.Count, "Expected 2 records and returned " + ds.Tables[0].Rows.Count + " in FilterChildDataSearchOverTwoChild");
            Assert.AreEqual(3, ds.Tables["Table_1901"].Select("[Pct Inhibition] >= 91").Length, ds.Tables[0].Rows.Count);
            Assert.AreEqual(3, ds.Tables["Table_1901"].Select("[Pct Inhibition] < 91").Length, ds.Tables[0].Rows.Count);
            Assert.AreEqual(0, ds.Tables["Table_1879"].Select("[EC50 (1 nM E2)] < 1.05").Length, ds.Tables[0].Rows.Count);
            Assert.AreEqual(2, ds.Tables["Table_1879"].Select("[EC50 (1 nM E2)] >= 1.05").Length, ds.Tables[0].Rows.Count);
            Assert.IsTrue(string.IsNullOrEmpty(message), "FilteredChildDataMessage was filled in when FilterChildData was passed as false in FilterChildDataSearchOverTwoChild");
            Assert.IsFalse((bool)ds.ExtendedProperties["FilteredChildData"], "FilteredChildData was returned as true when initially passed as false in FilterChildDataSearchOverTwoChild");
        }

        [TestMethod]
        public void FilterChildDataSearchByAggregate_Search()
        {
            FilterChildDataSearchByAggregate(false, false);
        }

        [TestMethod]
        public void FilterChildDataSearchByAggregate_NoHitlist()
        {
            FilterChildDataSearchByAggregate(true, false);
        }

        [TestMethod]
        public void FilterChildDataSearchByAggregate_NoHitlistNoPaging()
        {
            FilterChildDataSearchByAggregate(true, true);
        }

        private void FilterChildDataSearchByAggregate(bool noHitlist, bool noPaging)
        {
            SearchCriteria criteria = new SearchCriteria();
            criteria.SearchCriteriaID = 1;
            SearchCriteria.SearchCriteriaItem item = new SearchCriteria.SearchCriteriaItem();
            SearchCriteria.NumericalCriteria numericalCriteria = new SearchCriteria.NumericalCriteria();
            numericalCriteria.Operator = SearchCriteria.COEOperators.EQUAL;
            numericalCriteria.Value = "15-18";
            item.TableId = 1901; //AID629ERA
            item.FieldId = 1903; //& Inhibition
            item.AggregateFunctionName = "AVG"; //AGGREGATE THE FIELD BY AVG
            item.Criterium = numericalCriteria;
            criteria.Items.Add(item);

            DataSet ds = GetDataSet(noHitlist, noPaging, criteria);
            string message = ds.ExtendedProperties["FilteredChildDataMessage"] as string;

            Assert.AreEqual(1482, ds.Tables[0].Rows.Count, "Expected 39 records and returned " + ds.Tables[0].Rows.Count + " in FilterChildDataSearchOverOneChild");
            Assert.AreEqual(1482, ds.Tables[0].Select("[Average % Inhibition] >= 15 and [Average % Inhibition] <= 18").Length, ds.Tables[0].Rows.Count, "Child data not filtered on " + ds.Tables[0].Rows.Count + " in FilterChildDataSearchOverOneChild");
            //Actually no search is specific to a child record
            Assert.IsFalse(string.IsNullOrEmpty(message), "FilteredChildDataMessage was filled in when search over child was provided in FilterChildDataSearchOverOneChild");
            Assert.IsTrue((bool)ds.ExtendedProperties["FilteredChildData"], "FilteredChildData was returned as false when no search over child was provided in FilterChildDataSearchOverOneChild");
        }

        [TestMethod]
        public void FilterChildDataSearchByTwoColumnsOnAChildTbl_Search()
        {
            FilterChildDataSearchByTwoColumnsOnAChildTbl(false, false);
        }

        [TestMethod]
        public void FilterChildDataSearchByTwoColumnsOnAChildTbl_NoHitlist()
        {
            FilterChildDataSearchByTwoColumnsOnAChildTbl(true, false);
        }

        [TestMethod]
        public void FilterChildDataSearchByTwoColumnsOnAChildTbl_NoHitlistNoPaging()
        {
            FilterChildDataSearchByTwoColumnsOnAChildTbl(true, true);
        }

        private void FilterChildDataSearchByTwoColumnsOnAChildTbl(bool noHitlist, bool noPaging)
        {
            //TODO: Implement this method.
            Assert.Inconclusive("TODO: Implement this method.");
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

        private PagingInfo GetPagingInfo()
        {
            PagingInfo pi = new PagingInfo();
            pi.Start = 1;
            pi.RecordCount = 5000;
            pi.FilterChildData = true;
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

        private DataSet GetDataSet(bool noHitlist, bool noPaging, SearchCriteria searchCriteria)
        {
            DataSet ds = new DataSet();
            if (noPaging)
            {
                _pagingInfo.End = _pagingInfo.RecordCount = 0;
            }

            if (noHitlist)
            {
                ds = _searchService.GetData(_resultsCriteria, _pagingInfo, _dataViewID, "NO", null, searchCriteria);
            }
            else
            {
                HitListInfo info = _searchService.GetHitList(searchCriteria, _dataViewID);
                _pagingInfo.HitListID = info.HitListID;
                _pagingInfo.HitListType = info.HitListType;


                ds = _searchService.GetData(_resultsCriteria, _pagingInfo, _dataViewID);
            }

            return ds;
        }
        #endregion
    }
}
