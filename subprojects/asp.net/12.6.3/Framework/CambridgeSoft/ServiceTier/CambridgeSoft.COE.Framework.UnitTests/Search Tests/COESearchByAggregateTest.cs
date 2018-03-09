using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CambridgeSoft.COE.Framework.Common;
using System.Xml;
using CambridgeSoft.COE.Framework.COESecurityService;
using CambridgeSoft.COE.Framework.COESearchService;
using System.Data;
using CambridgeSoft.COE.Framework.UnitTests.Helpers;

namespace SearchUnitTests
{
    /// <summary>
    /// Summary description for COESearchByAggregateTest
    /// </summary>
    //TODO: Add tests for searching by aggregate a grandchild table.
    //TODO: Add a test that mixes search by aggregate 
    [TestClass]
    public class COESearchByAggregateTest
    {
        #region Variables
        private COESearch _searchService;
        private int _dataViewID = 0;
        private SearchCriteria _searchCriteria;
        private ResultsCriteria _resultsCriteria;
        private PagingInfo _pagingInfo;
        private string _pathToXmls;
        #endregion

        #region Constructor
        public COESearchByAggregateTest()
        {
            _searchService = new COESearch();
            _pathToXmls = SearchHelper.GetExecutingTestResultsBasePath(SearchHelper._COESearchByAggpathToXml);
        }
        #endregion

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        [TestInitialize()]
        public void MyTestInitialize()
        {
            _pagingInfo = GetFullPagingInfo();
            _searchCriteria = GetSearchCriteria();
            _resultsCriteria = GetResultsCriteria();
            Login();
            SearchHelper.CreateDataView(SearchHelper._COEAggpathOfDV1, SearchHelper._COEAggdataViewID1);
            _dataViewID = SearchHelper._COEAggdataViewID1;
        }
        //
        // Use TestCleanup to run code after each test has run
        [TestCleanup()]
        public void MyTestCleanup()
        {
            SearchHelper.DeleteDataView(SearchHelper._COEAggdataViewID1);
        }
        //
        #endregion

        #region Test Methods
        /// <summary>
        /// Search by HAVING count("AID629ERA"."PUBCHEM_CID) > 0
        /// This method pages the results and use hitlists.
        /// </summary>
        [TestMethod]
        public void CountGreaterThanZeroSearch()
        {
            CountGreaterThanZero(false, false);
        }

        /// <summary>
        /// Search by HAVING count("AID629ERA"."PUBCHEM_CID) > 0
        /// This method pages the results, but do not use hitlists.
        /// </summary>
        [TestMethod]
        public void CountGreaterThanZeroNoHitlist()
        {
            CountGreaterThanZero(true, false);
        }

        /// <summary>
        /// Search by HAVING count("AID629ERA"."PUBCHEM_CID) > 0
        /// This method don't pages the results and do not use hitlists.
        /// </summary>
        [TestMethod]
        public void CountGreaterThanZeroNoHitlistNoPaging()
        {
            //This takes really long...
            //CountGreaterThanZero(true, true);
        }

        private void CountGreaterThanZero(bool noHitlist, bool noPaging)
        {
            SearchCriteria.NumericalCriteria numericalCriteria = new SearchCriteria.NumericalCriteria();
            numericalCriteria.Operator = SearchCriteria.COEOperators.GT;
            numericalCriteria.Value = "0";
            SearchCriteria.SearchCriteriaItem item = new SearchCriteria.SearchCriteriaItem();
            item.TableId = 1901; //AID629ERA
            item.FieldId = 1906; //pubchemcid
            item.AggregateFunctionName = "COUNT"; //AGGREGATE THE FIELD BY COUNT
            item.Criterium = numericalCriteria;
            _searchCriteria.Items.Add(item);

            DataSet ds = GetDataSet(noHitlist, noPaging);
            Assert.IsTrue(ds != null);
            Assert.IsTrue(ds.Tables.Count == 1);
            if (!noPaging)
                Assert.IsTrue(ds.Tables[0].Rows.Count == 5000); //limit set by paging info.
            else
                Assert.IsTrue(ds.Tables[0].Rows.Count == 75618);
        }

        /// <summary>
        /// Search by HAVING count("AID629ERA"."PUBCHEM_CID) &lt; 3
        /// 19 Results should be returned, all with a zero count. That's because the returned records are those that has no child data associated.
        /// This method pages the results and use hitlists.
        /// </summary>
        [TestMethod]
        public void CountLowerThanThreeSearch()
        {
            CountLowerThanThree(false, false);
        }

        /// <summary>
        /// Search by HAVING count("AID629ERA"."PUBCHEM_CID) &lt; 3
        /// 19 Results should be returned, all with a zero count. That's because the returned records are those that has no child data associated.
        /// This method pages the results, but do not use hitlists.
        /// </summary>
        [TestMethod]
        public void CountLowerThanThreeNoHitlist()
        {
            CountLowerThanThree(true, false);
        }

        /// <summary>
        /// Search by HAVING count("AID629ERA"."PUBCHEM_CID) &lt; 3
        /// 19 Results should be returned, all with a zero count. That's because the returned records are those that has no child data associated.
        /// This method don't pages the results and do not use hitlists.
        /// </summary>
        [TestMethod]
        public void CountLowerThanThreeNoHitlistNoPaging()
        {
            CountLowerThanThree(true, true);
        }

        private void CountLowerThanThree(bool noHitlist, bool noPaging)
        {
            SearchCriteria.NumericalCriteria numericalCriteria = new SearchCriteria.NumericalCriteria();
            numericalCriteria.Operator = SearchCriteria.COEOperators.LT;
            numericalCriteria.Value = "3";
            SearchCriteria.SearchCriteriaItem item = new SearchCriteria.SearchCriteriaItem();
            item.TableId = 1901; //AID629ERA
            item.FieldId = 1906; //pubchemcid
            item.AggregateFunctionName = "COUNT"; //AGGREGATE THE FIELD BY COUNT
            item.Criterium = numericalCriteria;
            _searchCriteria.Items.Add(item);
            DataSet ds = GetDataSet(noHitlist, noPaging);
            Assert.IsTrue(ds != null);
            Assert.IsTrue(ds.Tables.Count == 1);
            Assert.IsTrue(ds.Tables[0].Rows.Count == 19);
            _pagingInfo = this.GetFullPagingInfo();

            this.AddAggregatedSelectClausesToRC();
            ds = GetDataSet(noHitlist, noPaging);
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                Assert.IsTrue(int.Parse(row["ERalpha Primary Screen Count"].ToString()) < 3);
            }
        }

        /// <summary>
        /// Search by HAVING AVG("AID629ERA"."% Inhibition") BETWEEN 15 AND 18
        /// 1482 Results should be returned
        /// This method pages the results and use hitlists.
        /// </summary>
        [TestMethod]
        public void AvgPctInhibitionBetween15And18Search()
        {
            AvgPctInhibitionBetween15And18(false, false);
        }

        /// <summary>
        /// Search by HAVING AVG("AID629ERA"."% Inhibition") BETWEEN 15 AND 18
        /// 1482 Results should be returned.
        /// This method pages the results, but do not use hitlists.
        /// </summary>
        [TestMethod]
        public void AvgPctInhibitionBetween15And18NoHitlist()
        {
            AvgPctInhibitionBetween15And18(true, false);
        }

        /// <summary>
        /// Search by HAVING AVG("AID629ERA"."% Inhibition") BETWEEN 15 AND 18
        /// 1482 Results should be returned.
        /// This method don't pages the results and do not use hitlists.
        /// </summary>
        [TestMethod]
        public void AvgPctInhibitionBetween15And18NoHitlistNoPaging()
        {
            AvgPctInhibitionBetween15And18(true, true);
        }

        private void AvgPctInhibitionBetween15And18(bool noHitlist, bool noPaging)
        {
            SearchCriteria.NumericalCriteria numericalCriteria = new SearchCriteria.NumericalCriteria();
            numericalCriteria.Operator = SearchCriteria.COEOperators.EQUAL;
            numericalCriteria.Value = "15-18";
            SearchCriteria.SearchCriteriaItem item = new SearchCriteria.SearchCriteriaItem();
            item.TableId = 1901; //AID629ERA
            item.FieldId = 1903; //& Inhibition
            item.AggregateFunctionName = "AVG"; //AGGREGATE THE FIELD BY COUNT
            item.Criterium = numericalCriteria;
            _searchCriteria.Items.Add(item);

            DataSet ds = this.GetDataSet(noHitlist, noPaging);
            Assert.IsTrue(ds != null);
            Assert.IsTrue(ds.Tables.Count == 1);
            Assert.IsTrue(ds.Tables[0].Rows.Count == 1482);
            _pagingInfo = this.GetFullPagingInfo();
            this.AddAggregatedSelectClausesToRC();
            ds = GetDataSet(noHitlist, noPaging);

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                Assert.IsTrue(double.Parse(row["Average % Inhibition"].ToString()) >= 15);
                Assert.IsTrue(double.Parse(row["Average % Inhibition"].ToString()) <= 18);
            }
        }

        /// <summary>
        /// Search by HAVING AVG("AID629ERA"."% Inhibition") BETWEEN 15 AND 18
        /// And substructure of CC1=CC=CC2=CC=CC=C21
        /// 19 Results should be returned, all with a zero count. That's because the returned records are those that has no child data associated.
        /// This method pages the results and use hitlists.
        /// </summary>
        [TestMethod]
        public void AvgPctInhibitionBetween15And18AndSubstructureSearch()
        {
            AvgPctInhibitionBetween15And18AndSubstructure(false, false);
        }

        /// <summary>
        /// Search by HAVING AVG("AID629ERA"."% Inhibition") BETWEEN 15 AND 18
        /// And substructure of CC1=CC=CC2=CC=CC=C21
        /// 19 Results should be returned, all with a zero count. That's because the returned records are those that has no child data associated.
        /// This method pages the results, but do not use hitlists.
        /// </summary>
        [TestMethod]
        public void AvgPctInhibitionBetween15And18AndSubstructureNoHitlist()
        {
            AvgPctInhibitionBetween15And18AndSubstructure(true, false);
        }

        /// <summary>
        /// Search by HAVING AVG("AID629ERA"."% Inhibition") BETWEEN 15 AND 18
        /// And substructure of CC1=CC=CC2=CC=CC=C21
        /// 19 Results should be returned, all with a zero count. That's because the returned records are those that has no child data associated.
        /// This method don't pages the results and do not use hitlists.
        /// </summary>
        [TestMethod]
        public void AvgPctInhibitionBetween15And18AndSubstructureNoHitlistNoPaging()
        {
            AvgPctInhibitionBetween15And18AndSubstructure(true, true);
        }

        private void AvgPctInhibitionBetween15And18AndSubstructure(bool noHitlist, bool noPaging)
        {
            SearchCriteria.NumericalCriteria numericalCriteria = new SearchCriteria.NumericalCriteria();
            numericalCriteria.Operator = SearchCriteria.COEOperators.EQUAL;
            numericalCriteria.Value = "15-18";
            SearchCriteria.SearchCriteriaItem item = new SearchCriteria.SearchCriteriaItem();
            item.TableId = 1901; //AID629ERA
            item.FieldId = 1903; //& Inhibition
            item.ID = 1;
            item.AggregateFunctionName = "AVG"; //AGGREGATE THE FIELD BY AVERAGE
            item.Criterium = numericalCriteria;
            _searchCriteria.Items.Add(item);

            SearchCriteria.StructureCriteria structureCriteria = new SearchCriteria.StructureCriteria();
            structureCriteria.FullSearch = SearchCriteria.COEBoolean.No;
            structureCriteria.Structure = "CC1=CC=CC2=CC=CC=C21";
            structureCriteria.Implementation = "CsCartridge";

            SearchCriteria.SearchCriteriaItem itemStructure = new SearchCriteria.SearchCriteriaItem();
            itemStructure.ID = 2;
            itemStructure.TableId = 1429; //ASSAYEDCOMPOUNDS
            itemStructure.FieldId = 1430; //BASE64_CDX
            itemStructure.Criterium = structureCriteria;
            _searchCriteria.Items.Add(itemStructure);
            DataSet ds = GetDataSet(noHitlist, noPaging);
            Assert.IsTrue(ds != null);
            Assert.IsTrue(ds.Tables.Count == 1);
            Assert.IsTrue(ds.Tables[0].Rows.Count == 8);
            _pagingInfo = this.GetFullPagingInfo();

            this.AddAggregatedSelectClausesToRC();

            ds = GetDataSet(noHitlist, noPaging);
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                Assert.IsTrue(double.Parse(row["Average % Inhibition"].ToString()) >= 15);
                Assert.IsTrue(double.Parse(row["Average % Inhibition"].ToString()) <= 18);
            }
        }

        /// <summary>
        /// Search by HAVING AVG("AID629ERA"."Score") = 100.00
        /// 5 Results should be returned, all with a zero count.
        /// This method is added to probe Aggregate function over integer fields works when double values are used
        /// This method pages the results and use hitlists.
        /// </summary>
        [TestMethod]
        public void AvgScoreEquals100Dot00Search()
        {
            AvgScoreEquals100Dot00(false, false);
        }

        /// <summary>
        /// Search by HAVING AVG("AID629ERA"."Score") = 100.00
        /// 5 Results should be returned, all with a zero count.
        /// This method is added to probe Aggregate function over integer fields works when double values are used
        /// This method pages the results, but do not use hitlists.
        /// </summary>
        [TestMethod]
        public void AvgScoreEquals100Dot00NoHitlist()
        {
            AvgScoreEquals100Dot00(true, false);
        }

        /// <summary>
        /// Search by HAVING AVG("AID629ERA"."Score") = 100.00
        /// 5 Results should be returned, all with a zero count.
        /// This method is added to probe Aggregate function over integer fields works when double values are used
        /// This method don't pages the results and do not use hitlists.
        /// </summary>
        [TestMethod]
        public void AvgScoreEquals100Dot00NoHitlistNoPaging()
        {
            AvgScoreEquals100Dot00(true, true);
        }

        private void AvgScoreEquals100Dot00(bool noHitlist, bool noPaging)
        {
            SearchCriteria.NumericalCriteria numericalCriteria = new SearchCriteria.NumericalCriteria();
            numericalCriteria.Operator = SearchCriteria.COEOperators.EQUAL;
            numericalCriteria.Value = "100.00";
            SearchCriteria.SearchCriteriaItem item = new SearchCriteria.SearchCriteriaItem();
            item.TableId = 1901; //AID629ERA
            item.FieldId = 1905; //Score
            item.ID = 1;
            item.AggregateFunctionName = "AVG"; //AGGREGATE THE FIELD BY Average
            item.Criterium = numericalCriteria;
            _searchCriteria.Items.Add(item);

            DataSet ds = GetDataSet(noHitlist, noPaging);
            Assert.IsTrue(ds != null);
            Assert.IsTrue(ds.Tables.Count == 1);
            Assert.IsTrue(ds.Tables[0].Rows.Count == 5);

        }
        #endregion

        #region Private Methods
        private void AddAggregatedSelectClausesToRC()
        {
            /*  <aggregateFunction alias="ERalpha Primary Screen Count" functionName="count">
                    <field fieldId="1906" /> <!-- "AID629ERA"."PUBCHEM_CID" -->
                </aggregateFunction>*/

            ResultsCriteria.AggregateFunction eralphaPSC = new ResultsCriteria.AggregateFunction();
            eralphaPSC.FunctionName = "COUNT";
            eralphaPSC.Alias = "ERalpha Primary Screen Count";
            ResultsCriteria.Field pubchemcid = new ResultsCriteria.Field();
            pubchemcid.Id = 1906;
            eralphaPSC.Parameters.Add(pubchemcid);
            _resultsCriteria.Tables[0].Criterias.Add(eralphaPSC);

            /*  <aggregateFunction alias="Average % Inhibition" functionName="avg">
                    <field fieldId="1903" /> <!-- "AID629ERA"."% Inhibition" -->
                </aggregateFunction>*/
            ResultsCriteria.AggregateFunction avgPctIn = new ResultsCriteria.AggregateFunction();
            avgPctIn.FunctionName = "AVG";
            avgPctIn.Alias = "Average % Inhibition";
            ResultsCriteria.Field pctIn = new ResultsCriteria.Field();
            pctIn.Id = 1903;
            avgPctIn.Parameters.Add(pctIn);
            _resultsCriteria.Tables[0].Criterias.Add(avgPctIn);
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
            pi.RecordCount = 5000;
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
            return result;
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
        #endregion
    }
}
