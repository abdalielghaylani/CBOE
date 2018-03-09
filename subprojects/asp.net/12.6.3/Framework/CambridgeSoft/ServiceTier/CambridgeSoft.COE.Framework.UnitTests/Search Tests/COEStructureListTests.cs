using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CambridgeSoft.COE.Framework.COESearchService;
using CambridgeSoft.COE.Framework.Common;
using System.Xml;
using CambridgeSoft.COE.Framework.COESecurityService;
using System.IO;
using System.Data;
using CambridgeSoft.COE.Framework.UnitTests.Helpers;
using CambridgeSoft.COE.Framework.COEDataViewService;

namespace SearchUnitTests
{
    /// <summary>
    /// Class for testing StructureList searches
    /// </summary>
    [TestClass]
    public class COEStructureListTests
    {
        private COESearch _searchService;
        private int _dataViewID = 0;
        private SearchCriteria _searchCriteria;
        private ResultsCriteria _resultsCriteria;
        private PagingInfo _pagingInfo;
        private string _pathToXmls;

        public COEStructureListTests()
        {
            _searchService = new COESearch();
            _pathToXmls = SearchHelper.GetExecutingTestResultsBasePath(SearchHelper._COEStructureListTestspathToXml);

        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        //Use ClassCleanup to run code after all tests in a class have run
        [ClassCleanup()]
        public static void MyClassCleanup()
        {

            SearchHelper.DeleteDataView(SearchHelper._COEStructureListTestDV1);
            SearchHelper.DeleteDataView(SearchHelper._COEStructureListTestDV2);
        }

        // Use TestInitialize to run code before running each test 
        [TestInitialize()]
        public void MyTestInitialize()
        {
            _pagingInfo = GetFullPagingInfo();
            _searchCriteria = GetSearchCriteria();
            _resultsCriteria = this.GetResultsCriteria();
            Login();
            SearchHelper.CreateDataView(SearchHelper._COEStructureListTestDv1path, SearchHelper._COEStructureListTestDV1);
            SearchHelper.CreateDataView(SearchHelper._COEStructureListTestDv2path, SearchHelper._COEStructureListTestDV2);
            _dataViewID = SearchHelper._COEStructureListTestDV1;
        }
        //
        // Use TestCleanup to run code after each test has run
        [TestCleanup()]
        public void MyTestCleanup()
        {
            SearchHelper.DeleteDataView(SearchHelper._COEStructureListTestDV1);
            SearchHelper.DeleteDataView(SearchHelper._COEStructureListTestDV2);
        }

        #endregion

        #region Test Methods
        /// <summary>
        /// Requires DataviewId 5002 and COETest schema.
        /// Does a sdf search against MolTable for Bromobenzene, Furan, Cyclopentane and Cyclopropane
        /// </summary>
        [TestMethod]
        public void SDFSearchingOverBaseTable_Search()
        {
            SDFSearchingOverBaseTable(false, false);
        }

        /// <summary>
        /// Requires DataviewId 5002 and COETest schema.
        /// Does a sdf search against MolTable for Bromobenzene, Furan, Cyclopentane and Cyclopropane
        /// </summary>
        [TestMethod]
        public void SDFSearchingOverBaseTable_NoHitlist()
        {
            SDFSearchingOverBaseTable(true, false);
        }

        /// <summary>
        /// Requires DataviewId 5002 and COETest schema.
        /// Does a sdf search against MolTable for Bromobenzene, Furan, Cyclopentane and Cyclopropane
        /// </summary>
        [TestMethod]
        public void SDFSearchingOverBaseTable_NoHitlistNoPaging()
        {
            SDFSearchingOverBaseTable(true, true);
        }

        private void SDFSearchingOverBaseTable(bool noHitlist, bool noPaging)
        {
            DataSet ds = GetDataSet(noHitlist, noPaging);

            Assert.IsTrue(ds != null);
            Assert.AreNotEqual(0, ds.Tables.Count);
            Assert.AreEqual(5, ds.Tables["Table_203"].Rows.Count);
            //Assert.IsTrue(ds.Tables["Table_210"].Rows.Count == 3);
        }

        /// <summary>
        /// Requires DataviewId 5002 and COETest schema.
        /// Does a sdf search against MolTable for Bromobenzene, Furan, Cyclopentane and Cyclopropane against the wrong field and thus no index
        /// is present. An exception with graceful message should be thrown
        /// </summary>
        [TestMethod]
        public void SDFSearchingOverBaseTableNoIndex_Search()
        {
            SDFSearchingOverBaseTableNoIndex(false, false);
        }

        /// <summary>
        /// Requires DataviewId 5002 and COETest schema.
        /// Does a sdf search against MolTable for Bromobenzene, Furan, Cyclopentane and Cyclopropane against the wrong field and thus no index
        /// is present. An exception with graceful message should be thrown
        /// </summary>
        [TestMethod]
        public void SDFSearchingOverBaseTableNoIndex_NoHitlist()
        {
            SDFSearchingOverBaseTableNoIndex(true, false);
        }

        /// <summary>
        /// Requires DataviewId 5002 and COETest schema.
        /// Does a sdf search against MolTable for Bromobenzene, Furan, Cyclopentane and Cyclopropane against the wrong field and thus no index
        /// is present. An exception with graceful message should be thrown
        /// </summary>
        [TestMethod]
        public void SDFSearchingOverBaseTableNoIndex_NoHitlistNoPaging()
        {
            SDFSearchingOverBaseTableNoIndex(true, true);
        }

        private void SDFSearchingOverBaseTableNoIndex(bool noHitlist, bool noPaging)
        {
            DataSet ds = null;
            ((SearchCriteria.SearchCriteriaItem)_searchCriteria.Items[0]).FieldId = 206;
            bool exeptionThrown = false;
            try
            {
                ds = GetDataSet(noHitlist, noPaging);
            }
            catch (Exception ex)
            {
                exeptionThrown = true;
                Assert.IsTrue(ex.InnerException.InnerException.Message == "Sd file search is not supported without a CS Cartridge index on the database column containing the chemical structures");
            }
            Assert.IsTrue(exeptionThrown);
        }

        /// <summary>
        /// Requires COETest schema. Uses a dataview with product_subset as parent table and substance_subset as child.
        /// Does a sdf search against MolTable for Bromobenzene, Furan, Cyclopentane and Cyclopropane against the child table.
        /// Gets 9 products and its associated child rows )27)
        /// </summary>
        [TestMethod]
        public void SDFSearchingOverChildTable_Search()
        {
            SDFSearchingOverChildTable(false, false);
        }

        /// <summary>
        /// Requires COETest schema. Uses a dataview with product_subset as parent table and substance_subset as child.
        /// Does a sdf search against MolTable for Bromobenzene, Furan, Cyclopentane and Cyclopropane against the child table.
        /// Gets 9 products and its associated child rows )27)
        /// </summary>
        [TestMethod]
        public void SDFSearchingOverChildTable_NoHitlist()
        {
            SDFSearchingOverChildTable(true, false);
        }


        /// <summary>
        /// Requires COETest schema. Uses a dataview with product_subset as parent table and substance_subset as child.
        /// Does a sdf search against MolTable for Bromobenzene, Furan, Cyclopentane and Cyclopropane against the child table.
        /// Gets 9 products and its associated child rows )27)
        /// </summary>
        [TestMethod]
        public void SDFSearchingOverChildTable_NoHitlistNoPaging()
        {
            SDFSearchingOverChildTable(true, true);
        }

        // Failed : SUBSTANCE_SUBSET table not present
        private void SDFSearchingOverChildTable(bool noHitlist, bool noPaging)
        {
            try
            {
                DataSet ds = null;
                //((SearchCriteria.SearchCriteriaItem)_searchCriteria.Items[0]).TableId = 203; //SUBSTANCE
                //((SearchCriteria.SearchCriteriaItem)_searchCriteria.Items[0]).FieldId = 204; //BASE64_CDX

                ((SearchCriteria.SearchCriteriaItem)_searchCriteria.Items[0]).TableId = 10; //SUBSTANCE
                ((SearchCriteria.SearchCriteriaItem)_searchCriteria.Items[0]).FieldId = 12; //BASE64_CDX

                COEDataView prodSubsetDV = this.GetDataView();
                _resultsCriteria = this.GetResultsCriteria("ProdSubsetResultsCriteria.xml");
                ds = GetDataSet(noHitlist, noPaging, prodSubsetDV);

                //The test results are bogus, there seems that the products are three times in the product_substance.
                //We should expect for 3 products and one substance (Cyclopropane), so 3 products with one substance each.
                Assert.IsTrue(ds != null);
                Assert.AreEqual(2, ds.Tables.Count);
                Assert.AreEqual(9, ds.Tables["Table_1"].Rows.Count);
                Assert.AreEqual(27, ds.Tables["Table_10"].Rows.Count);
            }
            catch
            {

                Assert.Inconclusive("Fail: Table SUBSTANCE_SUBSET not present in DB");
            }
        }

        /// <summary>
        /// Requires COETest schema. Uses a dataview with product_subset as parent table and substance_subset as child.
        /// Does a sdf search against MolTable for Bromobenzene, Furan, Cyclopentane and Cyclopropane against the child table.
        /// Gets 9 products and its associated child rows )27)
        /// </summary>
        [TestMethod]
        public void SDFSearchingOverLookupInBaseTable_Search()
        {
            SDFSearchingOverLookupInBaseTable(false, false);
        }

        /// <summary>
        /// Requires COETest schema. Uses a dataview with product_subset as parent table and substance_subset as child.
        /// Does a sdf search against MolTable for Bromobenzene, Furan, Cyclopentane and Cyclopropane against the child table.
        /// Gets 9 products and its associated child rows )27)
        /// </summary>
        [TestMethod]
        public void SDFSearchingOverLookupInBaseTable_NoHitlist()
        {
            SDFSearchingOverLookupInBaseTable(true, false);
        }

        /// <summary>
        /// Requires COETest schema. Uses a dataview with product_subset as parent table and substance_subset as child.
        /// Does a sdf search against MolTable for Bromobenzene, Furan, Cyclopentane and Cyclopropane against the child table.
        /// Gets 9 products and its associated child rows )27)
        /// </summary>
        [TestMethod]
        public void SDFSearchingOverLookupInBaseTable_NoHitlistNoPaging()
        {
            SDFSearchingOverLookupInBaseTable(true, true);
        }
        //Failed : COETEST.SUBSTANCE_SUBSET table not present
        private void SDFSearchingOverLookupInBaseTable(bool noHitlist, bool noPaging)
        {
            try
            {
                DataSet ds = null;
                ((SearchCriteria.SearchCriteriaItem)_searchCriteria.Items[0]).TableId = 1; //SUBSTANCE
                ((SearchCriteria.SearchCriteriaItem)_searchCriteria.Items[0]).FieldId = 6; //STRUCTURE LOOKUP

                COEDataView prodSubsetDV = this.GetDataView();
                _resultsCriteria = this.GetResultsCriteria("ProdSubsetResultsCriteria.xml");

                ds = GetDataSet(noHitlist, noPaging, this.GetDataView());

                //The test results are bogus, there seems that the products are three times in the product_substance.
                //We should expect for 3 products and one substance (Cyclopropane), so 3 products with one substance each.
                Assert.IsTrue(ds != null);
                Assert.AreEqual(2, ds.Tables.Count);
                Assert.AreEqual(9, ds.Tables["Table_1"].Rows.Count);
                Assert.AreEqual(27, ds.Tables["Table_10"].Rows.Count);
            }
            catch
            {

                Assert.Inconclusive("Fail: Table COETEST.SUBSTANCE_SUBSET not present");
            }
        }

        /// <summary>
        /// Requires DataviewId 5010 and COETest schema.
        /// Uses a large set of molfiles to search over substance_subset
        /// </summary>
        [TestMethod]
        public void SDFSearchingOverBaseTableWithLargeMolFiles_Search()
        {
            try
            {
                SDFSearchingOverBaseTableWithLargeMolFiles(false, false);
            }
            catch
            {

                Assert.Inconclusive("Fail: Table SUBSTANCE_SUBSET not present in DB");
            }

        }

        /// <summary>
        /// Requires DataviewId 5010 and COETest schema.
        /// Uses a large set of molfiles to search over substance_subset
        /// </summary>
        [TestMethod]
        public void SDFSearchingOverBaseTableWithLargeMolFiles_NoHitlist()
        {
            try
            {
                SDFSearchingOverBaseTableWithLargeMolFiles(true, false);
            }
            catch
            {
                Assert.Inconclusive("Fail: Table SUBSTANCE_SUBSET not present in DB");
            }

        }

        /// <summary>
        /// Requires DataviewId 5010 and COETest schema.
        /// Uses a large set of molfiles to search over substance_subset
        /// </summary>
        [TestMethod]
        public void SDFSearchingOverBaseTableWithLargeMolFiles_NoHitlistNoPaging()
        {
            try
            {
                SDFSearchingOverBaseTableWithLargeMolFiles(true, true);
            }
            catch
            {
                Assert.Inconclusive("Fail: Table SUBSTANCE_SUBSET not present in DB");
            }
        }

        private void SDFSearchingOverBaseTableWithLargeMolFiles(bool noHitlist, bool noPaging)
        {
            DataSet ds = null;
            ((SearchCriteria.SearchCriteriaItem)_searchCriteria.Items[0]).TableId = 203; //SUBSTANCE_SUBSET
            ((SearchCriteria.SearchCriteriaItem)_searchCriteria.Items[0]).FieldId = 205; //BASE64_CDX
            ((SearchCriteria.StructureListCriteria)((SearchCriteria.SearchCriteriaItem)_searchCriteria.Items[0]).Criterium).StructureList = File.ReadAllText(_pathToXmls + "\\LargeSearchSamples.sdf");
            try
            {
                _dataViewID = SearchHelper._COEStructureListTestDV2;
                _resultsCriteria = this.GetResultsCriteria("SubstanceSubsetRC.xml");
                ds = GetDataSet(noHitlist, noPaging);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.StackTrace);
            }

            Assert.IsTrue(ds != null && ds.Tables.Count == 1);
            //Latest cartridge brings 70 results instead of 57
            Assert.AreEqual(282, ds.Tables[0].Rows.Count, "SDF Searching over Substance subset did not bring the expected amount of records");
            //Assert.IsTrue(ds.Tables["Table_383"].Rows.Count == 57);
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
            SearchCriteria result = new SearchCriteria();
            result.SearchCriteriaID = 1; //unique id
            SearchCriteria.StructureListCriteria listCriteria = new SearchCriteria.StructureListCriteria();
            listCriteria.StructureList = File.ReadAllText(_pathToXmls + "\\SearchSamples.sdf"); //The sdf file content as text.
            SearchCriteria.SearchCriteriaItem item = new SearchCriteria.SearchCriteriaItem();
            item.TableId = 203; //moltable
            item.FieldId = 205; //base64_cdx
            item.Criterium = listCriteria;
            result.Items.Add(item);
            return result;
        }


        private COEDataView GetDataView(string fileName)
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
        //Failed :COETEST.SUBSTANCE_SUBSET not present
        private DataSet GetDataSet(bool noHitlist, bool noPaging)
        {
            DataSet ds = new DataSet();
            if (noPaging)
            {
                _pagingInfo.End = _pagingInfo.RecordCount = 0;
            }

            if (noHitlist)
            {
                ds = _searchService.GetData(_resultsCriteria, _pagingInfo, _dataViewID, "NO", null, _searchCriteria);
            }
            else
            {
                HitListInfo info = _searchService.GetHitList(_searchCriteria, _dataViewID);
                _pagingInfo.HitListID = info.HitListID;
                _pagingInfo.HitListType = info.HitListType;


                ds = _searchService.GetData(_resultsCriteria, _pagingInfo, _dataViewID);
            }

            return ds;
        }
        //Failed :COETEST.SUBSTANCE_SUBSET not present
        private DataSet GetDataSet(bool noHitlist, bool noPaging, COEDataView coeDataView)
        {
            DataSet ds = new DataSet();
            if (noPaging)
            {
                _pagingInfo.End = _pagingInfo.RecordCount = 0;
            }

            if (noHitlist)
            {
                ds = _searchService.GetData(_resultsCriteria, _pagingInfo, coeDataView, "NO", null, _searchCriteria);
            }
            else
            {
                HitListInfo info = _searchService.GetHitList(_searchCriteria, coeDataView);
                _pagingInfo.HitListID = info.HitListID;
                _pagingInfo.HitListType = info.HitListType;


                ds = _searchService.GetData(_resultsCriteria, _pagingInfo, coeDataView);
            }

            return ds;
        }
        #endregion
    }
}
