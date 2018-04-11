using System;
using System.Text;
using System.Collections.Generic;
using NUnit.Framework;
using CambridgeSoft.COE.Framework.COESearchService;
using CambridgeSoft.COE.Framework.Common;
using System.Data;
using System.Xml;
using CambridgeSoft.COE.Framework.COESecurityService;
using System.Reflection;
using CambridgeSoft.COE.Framework.NUnitTests.Helpers;
using CambridgeSoft.COE.Framework.COEDataViewService;

//TODO: Add test for sorting child tables through aggregate functions
namespace SearchUnitTests
{
    /// <summary>
    /// Set of unit tests for testing aggregate function clauses.
    /// </summary>
    [TestFixture]
    public class COEAggregationResultsTests
    {
        private COESearch _searchService;
        private int _dataViewID = 0;
        private SearchCriteria _searchCriteria;
        private ResultsCriteria _resultsCriteria;
        private PagingInfo _pagingInfo;

        private string _pathToXmls;
        private COEDataViewBO theCOEDataViewBO = null;
        public COEAggregationResultsTests()
        {
            _searchService = new COESearch();
            _pathToXmls = SearchHelper.GetExecutingTestResultsBasePath(SearchHelper._COEAggpathToXml);
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [TestFixtureSetUp]
        // public static void MyClassInitialize() { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [TestFixtureTearDown]
        // public static void MyClassCleanup() { }
        //
        /// <summary>
        /// Use TestInitialize to run code before running each test 
        /// </summary>
        [SetUp]
        public void MyTestInitialize()
        {
            _pagingInfo = GetFullPagingInfo();
            _searchCriteria = GetSearchCriteria();
            Login();
            _dataViewID = SearchHelper._COEAggdataViewID1;
            theCOEDataViewBO = SearchHelper.CreateDataView(SearchHelper._COEAggpathOfDV1, SearchHelper._COEAggdataViewID1);
            SearchHelper.CreateDataView(SearchHelper._COEAggpathOfDV2, SearchHelper._COEAggdataViewID2);
            //_dataViewID = SearchHelper._COEAggdataViewID2;
        }

        //Use TestCleanup to run code after each test has run
        [TearDown]
        public void MyTestCleanup()
        {

            SearchHelper.DeleteDataView(SearchHelper._COEAggdataViewID1);
            SearchHelper.DeleteDataView(SearchHelper._COEAggdataViewID2);
        }

        #endregion

        #region Using Hitlist
        /// <summary>
        /// Requires DataviewId 5003 and COETest schema.
        /// Searches for assayedcompounds with Formula =C14H14O4 (Full search). 6 records should be returned.
        /// And for those records 4 should have positive average pct of inhibition and 2 should be negative.
        /// Each of the records should have 3 child records, giving a total of 6 * 3 = 18.
        /// This method pages the results and use hitlists.
        /// </summary>
        [Test]
        public void AggregateChildToParentSearch()
        {
            AggregateChildToParent(false, false);
        }

        /// <summary>
        /// Requires DataviewId 5003 and COETest schema.
        /// Searches for assayedcompounds with Formula =C14H14O4 (Full search). 6 records should be returned.
        /// And for those records 4 should have positive average pct of inhibition and 2 should be negative.
        /// Each of the records should have 3 child records, giving a total of 6 * 3 = 18.
        /// This method pages the results, but do not use hitlists.
        /// </summary>
        [Test]
        public void AggregateChildToParentNoHitlist()
        {
            AggregateChildToParent(true, false);
        }

        /// <summary>
        /// Requires DataviewId 5003 and COETest schema.
        /// Searches for assayedcompounds with Formula =C14H14O4 (Full search). 6 records should be returned.
        /// And for those records 4 should have positive average pct of inhibition and 2 should be negative.
        /// Each of the records should have 3 child records, giving a total of 6 * 3 = 18.
        /// This method don't pages the results and do not use hitlists.
        /// </summary>
        [Test]
        public void AggregateChildToParentNoHitlistNoPaging()
        {
            AggregateChildToParent(true, true);
        }

        private void AggregateChildToParent(bool noHitlist, bool noPaging)
        {
            _resultsCriteria = GetResultsCriteria("ChildToParentRC.xml");
            _pagingInfo = GetFullPagingInfo();
            _searchCriteria = GetSearchCriteria(); // Where Formula =C14H14O4 (Full search)

            DataSet ds = this.GetDataSet(noHitlist, noPaging);

            Assert.IsTrue(ds.Tables[0] != null, "Null table returned by " + MethodInfo.GetCurrentMethod().Name);
            Assert.IsTrue(ds.Tables[0].Rows.Count == 6, "Expected 6 records and got " + ds.Tables[0].Rows.Count + " in " + MethodInfo.GetCurrentMethod().Name);

            Assert.IsTrue(ds.Tables[0].Select("[Average % Inhibition] > 0").Length == 4, "Expected 4 records with positive Average % Inhibition and got " + ds.Tables[0].Select("[Average % Inhibition] > 0").Length + " in method " + MethodInfo.GetCurrentMethod().Name);
            Assert.IsTrue(ds.Tables[0].Select("[Average % Inhibition] < 0").Length == 2, "Expected 2 records with negative Average % Inhibition and got " + ds.Tables[0].Select("[Average % Inhibition] < 0") + " in method " + MethodInfo.GetCurrentMethod().Name);
            Assert.IsTrue(Convert.ToDouble(ds.Tables[0].Compute("Sum([Average % Inhibition])", "")) == 127.21333333333333333333333332, "The sum of the Average % Inhibition was not the expected in method " + MethodInfo.GetCurrentMethod().Name);

            Assert.IsTrue(ds.Tables[0].Select("[ERalpha Primary Screen Count] > 0").Length == 6, "Expected 6 records with positive ERalpha Primary Screen Count and got different result in method " + MethodInfo.GetCurrentMethod().Name);
            Assert.IsTrue(Convert.ToInt32(ds.Tables[0].Compute("Avg([ERalpha Primary Screen Count])", "")) == 3, "The average of the ERalpha Primary Screen Count was not the expected in method " + MethodInfo.GetCurrentMethod().Name);
            Assert.IsTrue(Convert.ToInt32(ds.Tables[0].Compute("Sum([ERalpha Primary Screen Count])", "")) == 18, "The sum of the ERalpha Primary Screen Count was not the expected in method " + MethodInfo.GetCurrentMethod().Name);
        }

        /// <summary>
        /// This is not a true unit test.  Instead it just tries a bunch
        /// of functions and tells you which work and which don't
        /// oracle 11 or greater: collect, median.  Collect currently fails ...maybe because we are in a number column.
        /// </summary>
        [Test]
        public void AggregateChildToParentToSeeWhatAggsWork()
        {
            String[] strs =  
            {"avg","collect","corr","count","covar_pop","covar_samp","cume_dist",
            "dense_rank","first","group_id","grouping","grouping_id",
            "last","max","median","min","percentile_cont","percentile_disc","percent_rank",
            "rank","stddev","stddev_pop","stddev_samp",
            "sum","var_pop","var_samp","variance"};

            _resultsCriteria = GetResultsCriteria("ChildToParentRC.xml");
            _searchCriteria = GetSearchCriteria(); // Where Formula =C14H14O4 (Full search)

            HitListInfo info = _searchService.GetHitList(_searchCriteria, _dataViewID);
            _pagingInfo.HitListID = info.HitListID;
            _pagingInfo.HitListType = info.HitListType;


            _resultsCriteria.Tables[0].Criterias.Remove(_resultsCriteria.Tables[0].Criterias[0]);

            string currentFunc = string.Empty;

            string returnValues = string.Empty;

            List<string> smessages = new List<string>();
            List<string> fmessages = new List<string>();

            for (int i = 0; i < strs.Length; i++)
            {
                currentFunc = strs[i];
                _resultsCriteria.Tables[0].Criterias[3].Alias = currentFunc + " % Inhibition";
                ((ResultsCriteria.AggregateFunction)_resultsCriteria.Tables[0].Criterias[3]).FunctionName = currentFunc;
                ((ResultsCriteria.AggregateFunction)_resultsCriteria.Tables[0].Criterias[3]).Alias = currentFunc + " % Inhibition";
                returnValues = string.Empty;

                try
                {
                    DataSet ds = _searchService.GetData(_resultsCriteria, _pagingInfo, _dataViewID);
                    int j = 1;
                    foreach (DataRow r in ds.Tables[0].Rows)
                    {

                        returnValues += " " + r[currentFunc + " % Inhibition"].ToString();
                        if (j < ds.Tables[0].Rows.Count)
                        {
                            returnValues += ",";
                            j++;
                        }
                    }

                    if (ds.Tables[0].Rows.Count == 6)
                    {
                        smessages.Add(currentFunc + " works and returns (" + returnValues + ")");
                    }
                    else
                    {
                        fmessages.Add(currentFunc + " returned " + ds.Tables[0].Rows.Count + " hits and returns (" + returnValues + ")");
                    }
                }
                catch (Exception ex)
                {
                    fmessages.Add(string.Format("{0} does not work: {1}", currentFunc, ex.InnerException.InnerException.Message));
                }
            }


            Console.WriteLine("These seem to work:");
            foreach (string message in smessages)
            {
                Console.WriteLine(message);
            }

            Console.WriteLine("\r\n");
            Console.WriteLine("These don't seem to work:");
            foreach (string message in fmessages)
            {
                Console.WriteLine(message);
            }
        }

        /// <summary>
        /// Requires DataviewId 5003 and COETest schema.
        /// Searches for assayedcompounds with Formula =C14H14O4 (Full search). 6 records should be returned.
        /// And for those records 4 should have positive average pct of inhibition and 2 should be negative.
        /// Each of the records should have 3 child records, giving a total of 6 * 3 = 18.
        /// The big difference between this test and the previous test is the order the RC appear in the xml.  
        /// The current status is that the sql generation fails when the aggregate criteria is first.
        /// This method pages the results and use hitlists.
        /// </summary>
        [Test]
        public void AggregateChildToParentAggregateFirstSearch()
        {
            AggregateChildToParentAggregateFirst(false, false);
        }

        /// <summary>
        /// Requires DataviewId 5003 and COETest schema.
        /// Searches for assayedcompounds with Formula =C14H14O4 (Full search). 6 records should be returned.
        /// And for those records 4 should have positive average pct of inhibition and 2 should be negative.
        /// Each of the records should have 3 child records, giving a total of 6 * 3 = 18.
        /// The big difference between this test and the previous test is the order the RC appear in the xml.  
        /// The current status is that the sql generation fails when the aggregate criteria is first.
        /// This method pages the results, but do not use hitlists.
        /// </summary>
        [Test]
        public void AggregateChildToParentAggregateFirstNoHitlist()
        {
            AggregateChildToParentAggregateFirst(true, false);
        }

        /// <summary>
        /// Requires DataviewId 5003 and COETest schema.
        /// Searches for assayedcompounds with Formula =C14H14O4 (Full search). 6 records should be returned.
        /// And for those records 4 should have positive average pct of inhibition and 2 should be negative.
        /// Each of the records should have 3 child records, giving a total of 6 * 3 = 18.
        /// The big difference between this test and the previous test is the order the RC appear in the xml.  
        /// The current status is that the sql generation fails when the aggregate criteria is first.
        /// This method don't pages the results and do not use hitlists.
        /// </summary>
        [Test]
        public void AggregateChildToParentAggregateFirstNoHitlistNoPaging()
        {
            AggregateChildToParentAggregateFirst(true, true);
        }

        private void AggregateChildToParentAggregateFirst(bool noHitlist, bool noPaging)
        {
            _resultsCriteria = GetResultsCriteria("ChildToParentRCAggFirst.xml");
            _pagingInfo = GetFullPagingInfo();
            _searchCriteria = GetSearchCriteria(); // Where Formula =C14H14O4 (Full search)

            DataSet ds = GetDataSet(noHitlist, noPaging);

            Assert.IsTrue(ds.Tables[0] != null, "Null table returned by " + MethodInfo.GetCurrentMethod().Name);
            Assert.IsTrue(ds.Tables[0].Rows.Count == 6, "Expected 6 records and got " + ds.Tables[0].Rows.Count + " in " + MethodInfo.GetCurrentMethod().Name);

            Assert.IsTrue(ds.Tables[0].Select("[Average % Inhibition] > 0").Length == 4, "Expected 4 records with positive Average % Inhibition and got " + ds.Tables[0].Select("[Average % Inhibition] > 0").Length + " in method " + MethodInfo.GetCurrentMethod().Name);
            Assert.IsTrue(ds.Tables[0].Select("[Average % Inhibition] < 0").Length == 2, "Expected 2 records with negative Average % Inhibition and got " + ds.Tables[0].Select("[Average % Inhibition] < 0") + " in method " + MethodInfo.GetCurrentMethod().Name);
            Assert.IsTrue(Convert.ToDouble(ds.Tables[0].Compute("Sum([Average % Inhibition])", "")) == 127.21333333333333333333333332, "The sum of the Average % Inhibition was not the expected in method " + MethodInfo.GetCurrentMethod().Name);

            Assert.IsTrue(ds.Tables[0].Select("[ERalpha Primary Screen Count] > 0").Length == 6, "Expected 6 records with positive ERalpha Primary Screen Count and got different result in method " + MethodInfo.GetCurrentMethod().Name);
            Assert.IsTrue(Convert.ToInt32(ds.Tables[0].Compute("Avg([ERalpha Primary Screen Count])", "")) == 3, "The average of the ERalpha Primary Screen Count was not the expected in method " + MethodInfo.GetCurrentMethod().Name);

        }

        /// <summary>
        /// Requires DataviewId 5003 and COETest schema.
        /// Searches for assayedcompounds with Formula =C14H14O4 (Full search). 6 records should be returned.
        /// Table ERalpha Dose Response (AID1078ERA) is being aggregated, by a sum of its EC50 (1 nM E2) column.
        /// Only one of those records should have a Ralpha Dose associated record, with a EC50 (1 nM E2) value of 13.7, and thus the aggregated
        /// sum should also be 13.7.
        /// Also the table ERalpha Primary Screen (AID629ERA) is being aggregated, by a sum of its Pct of Inhibition column.
        /// The 6 records should have values, 4 should have positve sums and 2 negative sums.
        /// This method pages the results and use hitlists.
        /// </summary>
        [Test]
        public void AggregateTwoChildToParentSearch()
        {
            AggregateTwoChildToParent(false, false);
        }

        /// <summary>
        /// Requires DataviewId 5003 and COETest schema.
        /// Searches for assayedcompounds with Formula =C14H14O4 (Full search). 6 records should be returned.
        /// Table ERalpha Dose Response (AID1078ERA) is being aggregated, by a sum of its EC50 (1 nM E2) column.
        /// Only one of those records should have a Ralpha Dose associated record, with a EC50 (1 nM E2) value of 13.7, and thus the aggregated
        /// sum should also be 13.7.
        /// Also the table ERalpha Primary Screen (AID629ERA) is being aggregated, by a sum of its Pct of Inhibition column.
        /// The 6 records should have values, 4 should have positve sums and 2 negative sums.
        /// This method pages the results, but do not use hitlists.
        /// </summary>
        [Test]
        public void AggregateTwoChildToParentNoHitlist()
        {
            AggregateTwoChildToParent(true, false);
        }

        /// <summary>
        /// Requires DataviewId 5003 and COETest schema.
        /// Searches for assayedcompounds with Formula =C14H14O4 (Full search). 6 records should be returned.
        /// Table ERalpha Dose Response (AID1078ERA) is being aggregated, by a sum of its EC50 (1 nM E2) column.
        /// Only one of those records should have a Ralpha Dose associated record, with a EC50 (1 nM E2) value of 13.7, and thus the aggregated
        /// sum should also be 13.7.
        /// Also the table ERalpha Primary Screen (AID629ERA) is being aggregated, by a sum of its Pct of Inhibition column.
        /// The 6 records should have values, 4 should have positve sums and 2 negative sums.
        /// This method don't pages the results and do not use hitlists.
        /// </summary>
        [Test]
        public void AggregateTwoChildToParentNoHitlistNoPaging()
        {
            AggregateTwoChildToParent(true, true);
        }

        private void AggregateTwoChildToParent(bool noHitlist, bool noPaging)
        {
            _resultsCriteria = GetResultsCriteria("TwoChildToParentRC.xml");
            _pagingInfo = GetFullPagingInfo();
            _searchCriteria = GetSearchCriteria(); // Where Formula =C14H14O4 (Full search)

            DataSet ds = this.GetDataSet(noHitlist, noPaging);

            Assert.IsTrue(ds.Tables[0] != null, "Null table returned by " + MethodInfo.GetCurrentMethod().Name);
            Assert.IsTrue(ds.Tables[0].Rows.Count == 6, "Expected 6 recrods and got " + ds.Tables[0].Rows.Count + " in " + MethodInfo.GetCurrentMethod().Name);

            Assert.IsTrue(ds.Tables[0].Select("[Max EC50 (1 nM E2)] > 0").Length == 1, "Expected 1 record with Max EC50 (1 nM E2) and got " + ds.Tables[0].Select("[Max EC50 (1 nM E2)] > 0").Length + " in method " + MethodInfo.GetCurrentMethod().Name);
            Assert.IsTrue(ds.Tables[0].Select("[Max EC50 (1 nM E2)] is null").Length == 5, "Expected 5 records with null Max EC50 (1 nM E2) and got " + ds.Tables[0].Select("[Max EC50 (1 nM E2)] is null").Length + " in method " + MethodInfo.GetCurrentMethod().Name);
            Assert.IsTrue(Convert.ToDouble(ds.Tables[0].Compute("Max([Max EC50 (1 nM E2)])", "")) == 13.7, "Expected to have Max([Max EC50 (1 nM E2)]) == 13.7 and got " + Convert.ToDouble(ds.Tables[0].Compute("Max([Max EC50 (1 nM E2)])", "")) + " in method " + MethodInfo.GetCurrentMethod().Name);

            Assert.IsTrue(ds.Tables[0].Select("[Sum % Inhibition] > 0").Length == 4, "Expected 4 records with positive Average % Inhibition and got " + ds.Tables[0].Select("[Sum % Inhibition] > 0").Length + " in method " + MethodInfo.GetCurrentMethod().Name);
            Assert.IsTrue(ds.Tables[0].Select("[Sum % Inhibition] < 0").Length == 2, "Expected 2 records with negative Average % Inhibition and got " + ds.Tables[0].Select("[Sum % Inhibition] < 0").Length + " in method " + MethodInfo.GetCurrentMethod().Name);
            Assert.IsTrue(Convert.ToDouble(ds.Tables[0].Compute("Sum([Sum % Inhibition])", "")) == 381.64, "The sum of the Average % Inhibition was not the expected in method " + MethodInfo.GetCurrentMethod().Name);

        }

        [Test]
        public void AggregateTwoChildToParentSearchForceNoPaging()
        {
            _resultsCriteria = GetResultsCriteria("TwoChildToParentRC.xml");
            _pagingInfo = GetFullPagingInfo();
            _searchCriteria = GetSearchCriteria(); // Where Formula =C14H14O4 (Full search)

            DataSet ds = new DataSet();

            HitListInfo info = _searchService.GetHitList(_searchCriteria, _dataViewID);
            _pagingInfo.HitListID = info.HitListID;
            _pagingInfo.HitListType = info.HitListType;
            _pagingInfo.End = _pagingInfo.RecordCount = 0;

            ds = _searchService.GetData(_resultsCriteria, _pagingInfo, _dataViewID);

            Assert.IsTrue(ds.Tables[0] != null, "Null table returned by " + MethodInfo.GetCurrentMethod().Name);
            Assert.IsTrue(ds.Tables[0].Rows.Count == 6, "Expected 6 recrods and got " + ds.Tables[0].Rows.Count + " in " + MethodInfo.GetCurrentMethod().Name);

            Assert.IsTrue(ds.Tables[0].Select("[Max EC50 (1 nM E2)] > 0").Length == 1, "Expected 1 record with Max EC50 (1 nM E2) and got " + ds.Tables[0].Select("[Max EC50 (1 nM E2)] > 0").Length + " in method " + MethodInfo.GetCurrentMethod().Name);
            Assert.IsTrue(ds.Tables[0].Select("[Max EC50 (1 nM E2)] is null").Length == 5, "Expected 5 records with null Max EC50 (1 nM E2) and got " + ds.Tables[0].Select("[Max EC50 (1 nM E2)] is null").Length + " in method " + MethodInfo.GetCurrentMethod().Name);
            Assert.IsTrue(Convert.ToDouble(ds.Tables[0].Compute("Max([Max EC50 (1 nM E2)])", "")) == 13.7, "Expected to have Max([Max EC50 (1 nM E2)]) == 13.7 and got " + Convert.ToDouble(ds.Tables[0].Compute("Max([Max EC50 (1 nM E2)])", "")) + " in method " + MethodInfo.GetCurrentMethod().Name);

            Assert.IsTrue(ds.Tables[0].Select("[Sum % Inhibition] > 0").Length == 4, "Expected 4 records with positive Average % Inhibition and got " + ds.Tables[0].Select("[Sum % Inhibition] > 0").Length + " in method " + MethodInfo.GetCurrentMethod().Name);
            Assert.IsTrue(ds.Tables[0].Select("[Sum % Inhibition] < 0").Length == 2, "Expected 2 records with negative Average % Inhibition and got " + ds.Tables[0].Select("[Sum % Inhibition] < 0").Length + " in method " + MethodInfo.GetCurrentMethod().Name);
            Assert.IsTrue(Convert.ToDouble(ds.Tables[0].Compute("Sum([Sum % Inhibition])", "")) == 381.64, "The sum of the Average % Inhibition was not the expected in method " + MethodInfo.GetCurrentMethod().Name);

        }


        /// <summary>
        /// Requires DataviewId 5003 and COETest schema.
        /// Searches for assayedcompounds with Formula =C14H14O4 (Full search). 6 records should be returned.
        /// And for those records 4 should have positive average pct of inhibition and 2 should be negative.
        /// The average is rounded for 2 decimal places
        /// Each of the records should have 3 child records, giving a total of 6 * 3 = 18.
        /// This method pages the results and use hitlists.
        /// </summary>
        [Test]
        public void AggregateChildToParentWrappedWithSQLFunctionSearch()
        {
            AggregateChildToParentWrappedWithSQLFunction(false, false);
        }

        /// <summary>
        /// Requires DataviewId 5003 and COETest schema.
        /// Searches for assayedcompounds with Formula =C14H14O4 (Full search). 6 records should be returned.
        /// And for those records 4 should have positive average pct of inhibition and 2 should be negative.
        /// The average is rounded for 2 decimal places
        /// Each of the records should have 3 child records, giving a total of 6 * 3 = 18.
        /// This method pages the results, but do not use hitlists.
        /// </summary>
        [Test]
        public void AggregateChildToParentWrappedWithSQLFunctionNoHitlist()
        {
            AggregateChildToParentWrappedWithSQLFunction(true, false);
        }

        /// <summary>
        /// Requires DataviewId 5003 and COETest schema.
        /// Searches for assayedcompounds with Formula =C14H14O4 (Full search). 6 records should be returned.
        /// And for those records 4 should have positive average pct of inhibition and 2 should be negative.
        /// The average is rounded for 2 decimal places
        /// Each of the records should have 3 child records, giving a total of 6 * 3 = 18.
        /// This method don't pages the results and do not use hitlists.
        /// </summary>
        [Test]
        public void AggregateChildToParentWrappedWithSQLFunctionNoHitlistNoPaging()
        {
            AggregateChildToParentWrappedWithSQLFunction(true, true);
        }

        private void AggregateChildToParentWrappedWithSQLFunction(bool noHitlist, bool noPaging)
        {
            _resultsCriteria = GetResultsCriteria("ChildToParentWrappedWithSQLFunctionRC.xml");
            _pagingInfo = GetFullPagingInfo();
            _searchCriteria = GetSearchCriteria(); // Where Formula =C14H14O4 (Full search)

            DataSet ds = GetDataSet(noHitlist, noPaging);

            Assert.IsTrue(ds.Tables[0] != null, "Null table returned by " + MethodInfo.GetCurrentMethod().Name);
            Assert.IsTrue(ds.Tables[0].Rows.Count == 6, "Expected 6 recrods and got " + ds.Tables[0].Rows.Count + " in " + MethodInfo.GetCurrentMethod().Name);

            Assert.IsTrue(ds.Tables[0].Select("[Average % Inhibition] > 0").Length == 4, "Expected 4 records with positive Average % Inhibition and got " + ds.Tables[0].Select("[Average % Inhibition] > 0").Length + " in method " + MethodInfo.GetCurrentMethod().Name);
            Assert.IsTrue(ds.Tables[0].Select("[Average % Inhibition] < 0").Length == 2, "Expected 2 records with negative Average % Inhibition and got " + ds.Tables[0].Select("[Average % Inhibition] < 0") + " in method " + MethodInfo.GetCurrentMethod().Name);
            Assert.IsTrue(Convert.ToDouble(ds.Tables[0].Compute("Sum([Average % Inhibition])", "")) == 127.2, "The sum of the Average % Inhibition was not the expected in method " + MethodInfo.GetCurrentMethod().Name);

            // (999.99 - 999).ToString() --> 0.99M
            // Then the Length of the string is 5 at most. (0.9M and 0M)
            // So enforcing there are no strings with a leng bigger than that assures there is no record with more decimal places.
            Assert.IsTrue(ds.Tables[0].Select("Len(Convert(([Average % Inhibition] - Convert([Average % Inhibition], System.Int32)), System.String)) > 5").Length == 0, "There are some values that have more tan 2 decimal places for Average % Inhibition in method " + MethodInfo.GetCurrentMethod().Name);

            Assert.IsTrue(ds.Tables[0].Select("[ERalpha Primary Screen Count] > 0").Length == 6, "Expected 6 records with positive ERalpha Primary Screen Count and got different result in method " + MethodInfo.GetCurrentMethod().Name);
            Assert.IsTrue(Convert.ToInt32(ds.Tables[0].Compute("Avg([ERalpha Primary Screen Count])", "")) == 3, "The average of the ERalpha Primary Screen Count was not the expected in method " + MethodInfo.GetCurrentMethod().Name);
            Assert.IsTrue(Convert.ToInt32(ds.Tables[0].Compute("Sum([ERalpha Primary Screen Count])", "")) == 18, "The sum of the ERalpha Primary Screen Count was not the expected in method " + MethodInfo.GetCurrentMethod().Name);
        }

        /// <summary>
        /// Requires DataviewId 5003 and COETest schema.
        /// Searches for assayedcompounds with Formula =C14H14O4 (Full search). 6 records should be returned.
        /// And for those records 1 should have more than 5 chars in average pct of inhibition and 1 should have
        /// less than 4 chars in average pct of inhibition.
        /// Each of the records should have 3 child records, giving a total of 6 * 3 = 18.
        /// This method pages the results and use hitlists.
        /// </summary>
        [Test]
        public void AggregateASQLFunctionFromChildToParentSearch()
        {
            AggregateASQLFunctionFromChildToParent(false, false);
        }

        /// <summary>
        /// Requires DataviewId 5003 and COETest schema.
        /// Searches for assayedcompounds with Formula =C14H14O4 (Full search). 6 records should be returned.
        /// And for those records 1 should have more than 5 chars in average pct of inhibition and 1 should have
        /// less than 4 chars in average pct of inhibition.
        /// Each of the records should have 3 child records, giving a total of 6 * 3 = 18.
        /// This method pages the results, but do not use hitlists.
        /// </summary>
        [Test]
        public void AggregateASQLFunctionFromChildToParentNoHitlist()
        {
            AggregateASQLFunctionFromChildToParent(true, false);
        }

        /// <summary>
        /// Requires DataviewId 5003 and COETest schema.
        /// Searches for assayedcompounds with Formula =C14H14O4 (Full search). 6 records should be returned.
        /// And for those records 1 should have more than 5 chars in average pct of inhibition and 1 should have
        /// less than 4 chars in average pct of inhibition.
        /// Each of the records should have 3 child records, giving a total of 6 * 3 = 18.
        /// This method don't pages the results and do not use hitlists.
        /// </summary>
        [Test]
        public void AggregateASQLFunctionFromChildToParentNoHitlistNoPaging()
        {
            AggregateASQLFunctionFromChildToParent(true, true);
        }

        private void AggregateASQLFunctionFromChildToParent(bool noHitlist, bool noPaging)
        {
            _resultsCriteria = GetResultsCriteria("ChildToParentWithSQLFunctionRC.xml");
            _pagingInfo = GetFullPagingInfo();
            _searchCriteria = GetSearchCriteria(); // Where Formula =C14H14O4 (Full search)

            DataSet ds = GetDataSet(noHitlist, noPaging);

            Assert.IsTrue(ds.Tables[0] != null, "Null table returned by " + MethodInfo.GetCurrentMethod().Name);
            Assert.IsTrue(ds.Tables[0].Rows.Count == 6, "Expected 6 recrods and got " + ds.Tables[0].Rows.Count + " in " + MethodInfo.GetCurrentMethod().Name);

            Assert.IsTrue(ds.Tables[0].Select("[Average Length % Inhibition] > 5").Length == 1, "Expected 4 records with positive Average % Inhibition and got " + ds.Tables[0].Select("[Average Length % Inhibition] > 0").Length + " in method " + MethodInfo.GetCurrentMethod().Name);
            Assert.IsTrue(ds.Tables[0].Select("[Average Length % Inhibition] < 4").Length == 1, "Expected 2 records with negative Average % Inhibition and got " + ds.Tables[0].Select("[Average Length % Inhibition] < 0") + " in method " + MethodInfo.GetCurrentMethod().Name);
            Assert.IsTrue(Convert.ToDouble(ds.Tables[0].Compute("Sum([Average Length % Inhibition])", "")) == 28.333333333333336, "The sum of the Average % Inhibition was not the expected in method " + MethodInfo.GetCurrentMethod().Name);

            Assert.IsTrue(ds.Tables[0].Select("[ERalpha Primary Screen Count] > 0").Length == 6, "Expected 6 records with positive ERalpha Primary Screen Count and got different result in method " + MethodInfo.GetCurrentMethod().Name);
            Assert.IsTrue(Convert.ToInt32(ds.Tables[0].Compute("Avg([ERalpha Primary Screen Count])", "")) == 3, "The average of the ERalpha Primary Screen Count was not the expected in method " + MethodInfo.GetCurrentMethod().Name);
            Assert.IsTrue(Convert.ToInt32(ds.Tables[0].Compute("Sum([ERalpha Primary Screen Count])", "")) == 18, "The sum of the ERalpha Primary Screen Count was not the expected in method " + MethodInfo.GetCurrentMethod().Name);
        }

        /// <summary>
        /// Requires DataviewId 5103 and COETest schema 12.5.0.11 or later.
        /// Uses the ACX subset data including packages and cleaned up products subset
        /// Searches for ACX records with CAS= 111-%. 4 records should be returned.
        /// It aggregates the packages table twice treating as a separate table each time.  
        /// It checks the returned values for each aggregated column
        /// This method pages the results and use hitlists.
        /// </summary>
        [Test]
        public void AggregateTwoGrandChildrenToParentSearch()
        {
            AggregateTwoGrandChildrenToParent(false, false);
        }

        /// <summary>
        /// Requires DataviewId 5103 and COETest schema 12.5.0.11 or later.
        /// Uses the ACX subset data including packages and cleaned up products subset
        /// Searches for ACX records with CAS= 111-%. 4 records should be returned.
        /// It aggregates the packages table twice treating as a separate table each time.  
        /// It checks the returned values for each aggregated column
        /// This method uses a hitlist but does not page the results.
        /// </summary>
        [Test]
        public void AggregateTwoGrandChildrenToParentNoHitlist()
        {
            AggregateTwoGrandChildrenToParent(true, false);
        }


        /// <summary>
        /// Requires DataviewId 5103 and COETest schema 12.5.0.11 or later.
        /// Uses the ACX subset data including packages and cleaned up products subset
        /// Searches for ACX records with CAS= 111-%. 4 records should be returned.
        /// It aggregates the packages table twice treating as a separate table each time.  
        /// It checks the returned values for each aggregated column
        /// This method does not use hitlists or paging
        /// </summary>
        [Test]
        public void AggregateTwoGrandChildrenToParentNoHitlistNoPaging()
        {
            AggregateTwoGrandChildrenToParent(true, true);
        }

        /// <summary>
        /// Requires DataviewId 5103 and COETest schema :(Fail SUBSTANCE_SUBSET table not present in db
        /// </summary>
        public void AggregateTwoGrandChildrenToParent(bool noHitlist, bool noPaging)
        {
            try
            {
                _dataViewID = SearchHelper._COEAggdataViewID2;
                _resultsCriteria = GetResultsCriteria("GrandChildToParentRC.xml");
                _pagingInfo = GetFullPagingInfo();
                _searchCriteria = GetSearchCriteria("GrandChildSearchCriteriaACX.xml");

                DataSet ds = GetDataSet(noHitlist, noPaging, _dataViewID);

                Assert.IsTrue(ds.Tables[0] != null, "Null table returned by " + MethodInfo.GetCurrentMethod().Name);
                Assert.IsTrue(ds.Tables[0].Rows.Count == 4, "Expected 4 records and got " + ds.Tables[0].Rows.Count + " in " + MethodInfo.GetCurrentMethod().Name);

                Assert.IsTrue(ds.Tables[0].Select("[CsNum] = 3059")[0][2].ToString() == "209", "Expected the value of 209  got " + ds.Tables[0].Select("[CsNum] = 3059")[0][2].ToString() + " in method " + MethodInfo.GetCurrentMethod().Name);
                Assert.IsTrue(ds.Tables[0].Select("[CsNum] = 3059")[0][3].ToString() == "200.90368421052631578947368421", "Expected the value of 200.90368421052631578947368421  got " + ds.Tables[0].Select("[CsNum] = 3059")[0][2].ToString() + " in method " + MethodInfo.GetCurrentMethod().Name);

                Assert.IsTrue(ds.Tables[0].Select("[CsNum] = 7519")[0][2].ToString() == "56", "Expected the value of 56 got " + ds.Tables[0].Select("[CsNum] = 7519")[0][2].ToString() + " in method " + MethodInfo.GetCurrentMethod().Name);
                Assert.IsTrue(ds.Tables[0].Select("[CsNum] = 7519")[0][3].ToString() == "67.744464285714285714285714286", "Expected the value of 67.744464285714285714285714286  got " + ds.Tables[0].Select("[CsNum] = 7519")[0][2].ToString() + " in method " + MethodInfo.GetCurrentMethod().Name);

                Assert.IsTrue(ds.Tables[0].Select("[CsNum] = 7601")[0][2].ToString() == "32", "Expected the value of 32  got " + ds.Tables[0].Select("[CsNum] = 7601")[0][2].ToString() + " in method " + MethodInfo.GetCurrentMethod().Name);
                Assert.IsTrue(ds.Tables[0].Select("[CsNum] = 7601")[0][3].ToString() == "98.7309375", "Expected the value of 98.7309375  got " + ds.Tables[0].Select("[CsNum] = 7601")[0][2].ToString() + " in method " + MethodInfo.GetCurrentMethod().Name);

                Assert.IsTrue(ds.Tables[0].Select("[CsNum] = 930")[0][2].ToString() == "87", "Expected the value of 87 got " + ds.Tables[0].Select("[CsNum] = 930")[0][2].ToString() + " in method " + MethodInfo.GetCurrentMethod().Name);
                Assert.IsTrue(ds.Tables[0].Select("[CsNum] = 930")[0][3].ToString() == "180.42770114942528735632183908", "Expected the value of 180.42770114942528735632183908  got " + ds.Tables[0].Select("[CsNum] = 930")[0][2].ToString() + " in method " + MethodInfo.GetCurrentMethod().Name);
            }
            catch
            {
                Assert.Inconclusive("Fail:SUBSTANCE_SUBSET table not present in DB");
            }

        }

        #endregion

        #region Avoiding hitlist
        /// <summary>
        /// Requires DataviewId 5003 and COETest schema.
        /// Performs a browse of the assayedcompounds getting back the first 500 records.
        /// 497 should have values, and for those records 187 should have positive average pct of inhibition and 310 should be negative
        /// and none should be 0.
        /// Each of the records should have 3 child records, giving a total of 6 * 3 = 18.
        /// </summary>
        [Test]
        public void AggregateChildToParentAvoidHitlist()
        {
            _resultsCriteria = GetResultsCriteria("ChildToParentRC.xml");
            _pagingInfo = new PagingInfo();
            _pagingInfo.Start = 1;
            _pagingInfo.RecordCount = 500;

            DataSet ds = _searchService.GetData(_resultsCriteria, _pagingInfo, _dataViewID);

            Assert.IsTrue(ds.Tables[0] != null, "Null table returned by " + MethodInfo.GetCurrentMethod().Name);
            Assert.IsTrue(ds.Tables[0].Rows.Count == 500, "Expected 50000 recrods and got " + ds.Tables[0].Rows.Count + " in " + MethodInfo.GetCurrentMethod().Name);

            Assert.IsTrue(ds.Tables[0].Select("[Average % Inhibition] > 0").Length == 187, "Expected 187 records with positive Average % Inhibition and got " + ds.Tables[0].Select("[Average % Inhibition] > 0").Length + " in method " + MethodInfo.GetCurrentMethod().Name);
            Assert.IsTrue(ds.Tables[0].Select("[Average % Inhibition] < 0").Length == 310, "Expected 310 records with negative Average % Inhibition and got " + ds.Tables[0].Select("[Average % Inhibition] < 0").Length + " in method " + MethodInfo.GetCurrentMethod().Name);
            Assert.IsTrue(ds.Tables[0].Select("[Average % Inhibition] = 0").Length == 0, "Expected 0 records with a zero Average % Inhibition and got " + ds.Tables[0].Select("[Average % Inhibition] = 0").Length + " in method " + MethodInfo.GetCurrentMethod().Name);
            Assert.IsTrue(Convert.ToDouble(ds.Tables[0].Compute("Sum([Average % Inhibition])", "")) == -585.70333333333338, "The sum of the Average % Inhibition was not the expected in method " + MethodInfo.GetCurrentMethod().Name);

            Assert.IsTrue(ds.Tables[0].Select("[ERalpha Primary Screen Count] > 0").Length == 497, "Expected 497 records with positive ERalpha Primary Screen Count and got " + ds.Tables[0].Select("[ERalpha Primary Screen Count] > 0").Length + " in method " + MethodInfo.GetCurrentMethod().Name);
            Assert.IsTrue(Math.Round(Convert.ToDouble(ds.Tables[0].Compute("Avg([ERalpha Primary Screen Count])", "")), 5) == 2.988, "Rounded avg expected is 2.988 for ERalpha Primary Screen Count, but got " + Math.Round(Convert.ToDouble(ds.Tables[0].Compute("Avg([ERalpha Primary Screen Count])", "")), 5) + " instead, in method " + MethodInfo.GetCurrentMethod().Name);
            Assert.IsTrue(Convert.ToInt32(ds.Tables[0].Compute("Sum([ERalpha Primary Screen Count])", "")) == 1494, "The 1494 as sum of the ERalpha Primary Screen Count and got " + Convert.ToInt32(ds.Tables[0].Compute("Sum([ERalpha Primary Screen Count])", "")) + " in method " + MethodInfo.GetCurrentMethod().Name);
        }


        /// <summary>
        /// Requires DataviewId 5003 and COETest schema.
        /// Performs a browse of the assayedcompounds getting back the first 2000 records.
        /// Table ERalpha Dose Response (AID1078ERA) is being aggregated, by a sum of its EC50 (1 nM E2) column.
        /// Only one of those records should have a Ralpha Dose associated record, with a EC50 (1 nM E2) value of 13.7, and thus the aggregated
        /// sum should also be 13.7.
        /// Also the table ERalpha Primary Screen (AID629ERA) is being aggregated, by a sum of its Pct of Inhibition column.
        /// 2000 records should have values, and for those records 791 should have positive average pct of inhibition and 1203 should be negative
        /// and none should be 0.
        /// </summary>
        [Test]
        public void AggregateTwoChildToParentAvoidHitlist()
        {
            _resultsCriteria = GetResultsCriteria("TwoChildToParentRC.xml");
            _pagingInfo = new PagingInfo();
            _pagingInfo.Start = 1;
            _pagingInfo.RecordCount = 2000;

            DataSet ds = _searchService.GetData(_resultsCriteria, _pagingInfo, _dataViewID);

            Assert.IsTrue(ds.Tables[0] != null, "Null table returned by " + MethodInfo.GetCurrentMethod().Name);
            Assert.IsTrue(ds.Tables[0].Rows.Count == 2000, "Expected 2000 recrods and got " + ds.Tables[0].Rows.Count + " in " + MethodInfo.GetCurrentMethod().Name);

            Assert.IsTrue(ds.Tables[0].Select("[Max EC50 (1 nM E2)] > 0").Length == 1, "Expected 1 record with Max EC50 (1 nM E2) and got " + ds.Tables[0].Select("[Max EC50 (1 nM E2)] > 0").Length + " in method " + MethodInfo.GetCurrentMethod().Name);
            Assert.IsTrue(ds.Tables[0].Select("[Max EC50 (1 nM E2)] is null").Length == 1999, "Expected 1999 records with null Max EC50 (1 nM E2) and got " + ds.Tables[0].Select("[Max EC50 (1 nM E2)] is null").Length + " in method " + MethodInfo.GetCurrentMethod().Name);
            Assert.IsTrue(Convert.ToDouble(ds.Tables[0].Compute("Max([Max EC50 (1 nM E2)])", "")) == 13.7, "Expected to have Max([Max EC50 (1 nM E2)]) == 13.7 and got " + Convert.ToDouble(ds.Tables[0].Compute("Max([Max EC50 (1 nM E2)])", "")) + " in method " + MethodInfo.GetCurrentMethod().Name);

            Assert.IsTrue(ds.Tables[0].Select("[Sum % Inhibition] > 0").Length == 791, "Expected 791 records with positive Sum % Inhibition and got " + ds.Tables[0].Select("[Sum % Inhibition] > 0").Length + " in method " + MethodInfo.GetCurrentMethod().Name);
            Assert.IsTrue(ds.Tables[0].Select("[Sum % Inhibition] < 0").Length == 1203, "Expected 1203 records with negative Sum % Inhibition and got " + ds.Tables[0].Select("[Sum % Inhibition] < 0").Length + " in method " + MethodInfo.GetCurrentMethod().Name);
            Assert.IsTrue(ds.Tables[0].Select("[Sum % Inhibition] = 0").Length == 0, "Expected 0 records with a zero Sum % Inhibition and got " + ds.Tables[0].Select("[Sum % Inhibition] = 0").Length + " in method " + MethodInfo.GetCurrentMethod().Name);
            Assert.IsTrue(Convert.ToDouble(ds.Tables[0].Compute("Count([Sum % Inhibition])", "")) == 1994.0, "Expected 1994 records with Sum % Inhibition and got " + Convert.ToDouble(ds.Tables[0].Compute("Count([Sum % Inhibition])", "")) + " in method " + MethodInfo.GetCurrentMethod().Name);
            Assert.IsTrue(Convert.ToDouble(ds.Tables[0].Compute("Sum([Sum % Inhibition])", "")) == -5422.86, "The sum of the Sum % Inhibition was not the expected in method " + MethodInfo.GetCurrentMethod().Name);
        }

        /// <summary>
        /// Requires DataviewId 5003 and COETest schema.
        /// Performs a browse of the assayedcompounds getting back the first 500 records.
        /// 497 should have values, and for those records 187 should have positive average pct of inhibition and 310 should be negative
        /// and none should be 0.
        /// The average is rounded for 2 decimal places
        /// Each of the records should have 3 child records, giving a total of 6 * 3 = 18.
        /// </summary>
        [Test]
        public void AggregateChildToParentWrappedWithSQLFunctionAvoidHitlist()
        {
            _resultsCriteria = GetResultsCriteria("ChildToParentWrappedWithSQLFunctionRC.xml");
            _pagingInfo = new PagingInfo();
            _pagingInfo.Start = 1;
            _pagingInfo.RecordCount = 500;

            DataSet ds = _searchService.GetData(_resultsCriteria, _pagingInfo, _dataViewID);

            Assert.IsTrue(ds.Tables[0] != null, "Null table returned by " + MethodInfo.GetCurrentMethod().Name);
            Assert.IsTrue(ds.Tables[0].Rows.Count == 500, "Expected 500 recrods and got " + ds.Tables[0].Rows.Count + " in " + MethodInfo.GetCurrentMethod().Name);

            Assert.IsTrue(ds.Tables[0].Select("[Average % Inhibition] > 0").Length == 187, "Expected 187 records with positive Average % Inhibition and got " + ds.Tables[0].Select("[Average % Inhibition] > 0").Length + " in method " + MethodInfo.GetCurrentMethod().Name);
            Assert.IsTrue(ds.Tables[0].Select("[Average % Inhibition] < 0").Length == 310, "Expected 310 records with negative Average % Inhibition and got " + ds.Tables[0].Select("[Average % Inhibition] < 0") + " in method " + MethodInfo.GetCurrentMethod().Name);
            Assert.IsTrue(ds.Tables[0].Select("[Average % Inhibition] = 0").Length == 0, "Expected 0 records with a zero Average % Inhibition and got " + ds.Tables[0].Select("[Average % Inhibition] = 0").Length + " in method " + MethodInfo.GetCurrentMethod().Name);
            Assert.IsTrue(Convert.ToDouble(ds.Tables[0].Compute("Sum([Average % Inhibition])", "")) == -585.74, "The sum of the Average % Inhibition was not the expected in method " + MethodInfo.GetCurrentMethod().Name);

            Assert.IsTrue(ds.Tables[0].Select("Len(Convert(([Average % Inhibition] - Convert([Average % Inhibition], System.Int32)), System.String)) > 5").Length == 0, "There are some values that have more than 2 decimal places for Average % Inhibition in method " + MethodInfo.GetCurrentMethod().Name);

            Assert.IsTrue(ds.Tables[0].Select("[ERalpha Primary Screen Count] > 0").Length == 497, "Expected 497 records with positive ERalpha Primary Screen Count and got different result in method " + MethodInfo.GetCurrentMethod().Name);
            Assert.IsTrue(Math.Round(Convert.ToDouble(ds.Tables[0].Compute("Avg([ERalpha Primary Screen Count])", "")), 5) == 2.988, "Rounded avg expected is 2.988 for ERalpha Primary Screen Count, but got " + Math.Round(Convert.ToDouble(ds.Tables[0].Compute("Avg([ERalpha Primary Screen Count])", "")), 5) + " instead, in method " + MethodInfo.GetCurrentMethod().Name);
            Assert.IsTrue(Convert.ToInt32(ds.Tables[0].Compute("Sum([ERalpha Primary Screen Count])", "")) == 1494, "The 1494 as sum of the ERalpha Primary Screen Count and got " + Convert.ToInt32(ds.Tables[0].Compute("Sum([ERalpha Primary Screen Count])", "")) + " in method " + MethodInfo.GetCurrentMethod().Name);
        }

        /// <summary>
        /// Requires DataviewId 5103 and COETest schema 12.5.0.11 or later.
        /// Uses the ACX subset data including packages and cleaned up products subset
        /// Searches for ACX records with CAS= 111-%. 4 records should be returned for the
        /// base table.  433 for the child table.
        /// It aggregates the packages table twice treating as a separate table each time.  
        /// It checks the returned values for each aggregated column
        /// This method pages the results and use hitlists.
        /// </summary>
        [Test]
        public void AggregateTwoGrandChildrenToChildSearch()
        {
            AggregateTwoGrandChildrenToChild(false, false);
        }

        /// <summary>
        /// Requires DataviewId 5103 and COETest schema 12.5.0.11 or later.
        /// Uses the ACX subset data including packages and cleaned up products subset
        /// Searches for ACX records with CAS= 111-%. 4 records should be returned for the
        /// base table.  433 for the child table.
        /// It aggregates the packages table twice treating as a separate table each time.  
        /// It checks the returned values for each aggregated column
        /// This method uses a hitlist but does not page the results.
        /// </summary>
        [Test]
        public void AggregateTwoGrandChildrenToChildNoHitlist()
        {
            AggregateTwoGrandChildrenToChild(true, false);
        }


        /// <summary>
        /// Requires DataviewId 5103 and COETest schema 12.5.0.11 or later.
        /// Uses the ACX subset data including packages and cleaned up products subset
        /// Searches for ACX records with CAS= 111-%. 4 records should be returned for the
        /// base table.  433 for the child table.
        /// It aggregates the packages table twice treating as a separate table each time.  
        /// It checks the returned values for each aggregated column
        /// This method does not use hitlists or paging
        /// </summary>
        [Test]
        public void AggregateTwoGrandChildrenToChildNoHitlistNoPaging()
        {
            AggregateTwoGrandChildrenToChild(true, true);
        }


        /// <summary>
        /// Requires DataviewId 5103 and COETest schema : COETEST.SUBSTANCE_SUBSET not present
        /// </summary>
        public void AggregateTwoGrandChildrenToChild(bool noHitlist, bool noPaging)
        {

            try
            {
                _resultsCriteria = GetResultsCriteria("GrandChildToChildRC.xml");
                _pagingInfo = GetFullPagingInfo();
                _searchCriteria = GetSearchCriteria("GrandChildSearchCriteriaACX.xml");

                DataSet ds = GetDataSet(noHitlist, noPaging, SearchHelper._COEAggdataViewID2);

                Assert.IsTrue(ds.Tables[0] != null, "Null table returned by " + MethodInfo.GetCurrentMethod().Name);
                Assert.IsTrue(ds.Tables[0].Rows.Count == 4, "Expected 4 records and got " + ds.Tables[0].Rows.Count + " in " + MethodInfo.GetCurrentMethod().Name);
                Assert.IsTrue(ds.Tables[1].Rows.Count == 433, "Expected 433 records and got " + ds.Tables[0].Rows.Count + " in " + MethodInfo.GetCurrentMethod().Name);


                //spot check some values
                Assert.IsTrue(ds.Tables[1].Select("[Product Id] = 383564")[0][2].ToString() == "3", "Expected the value of 3  got " + ds.Tables[1].Select("[Product Id] = 383564")[0][2].ToString() + " in method " + MethodInfo.GetCurrentMethod().Name);
                Assert.IsTrue(ds.Tables[1].Select("[Product Id] = 1096650")[0][2].ToString() == "0", "Expected the value of 0  got " + ds.Tables[1].Select("[Product Id] = 1096650")[0][2].ToString() + " in method " + MethodInfo.GetCurrentMethod().Name);
                Assert.IsTrue(ds.Tables[1].Select("[Product Id] = 2062079")[0][2].ToString() == "2", "Expected the value of 2  got " + ds.Tables[1].Select("[Product Id] = 2062079")[0][2].ToString() + " in method " + MethodInfo.GetCurrentMethod().Name);

                Assert.IsTrue(ds.Tables[1].Select("[Product Id] = 805582")[0][3].ToString() == "63.5", "Expected the value of 63.5  got " + ds.Tables[1].Select("[Product Id] = 805582")[0][3].ToString() + " in method " + MethodInfo.GetCurrentMethod().Name);
                Assert.IsTrue(ds.Tables[1].Select("[Product Id] = 2125494")[0][3].ToString() == "511.2", "Expected the value of 511.2  got " + ds.Tables[1].Select("[Product Id] = 2125494")[0][3].ToString() + " in method " + MethodInfo.GetCurrentMethod().Name);
                Assert.IsTrue(ds.Tables[1].Select("[Product Id] = 1536897")[0][3].ToString() == "81.725", "Expected the value of 81.725  got " + ds.Tables[1].Select("[Product Id] = 1536897")[0][3].ToString() + " in method " + MethodInfo.GetCurrentMethod().Name);

            }
            catch
            {
                Assert.Inconclusive("Fail:SUBSTANCE_SUBSET table not present in DB");
            }

        }


        #endregion

        #region Sorting
        /// <summary>
        /// Requires DataviewId 5003 and COETest schema.
        /// Searches for assayedcompounds with Formula =C14H14O4 (Full search). 6 records should be returned.
        /// And for those records 4 should have positive average pct of inhibition and 2 should be negative.
        /// Each of the records should have 3 child records, giving a total of 6 * 3 = 18.
        /// Orders by [Average % Inhibition]
        /// This method pages the results and use hitlists.
        /// </summary>
        [Test]
        public void AggregateChildToParentSortByAVGSearch()
        {
            AggregateChildToParentSortByAVG(false, false);
        }

        /// <summary>
        /// Requires DataviewId 5003 and COETest schema.
        /// Searches for assayedcompounds with Formula =C14H14O4 (Full search). 6 records should be returned.
        /// And for those records 4 should have positive average pct of inhibition and 2 should be negative.
        /// Each of the records should have 3 child records, giving a total of 6 * 3 = 18.
        /// Orders by [Average % Inhibition]
        /// This method pages the results, but do not use hitlists.
        /// </summary>
        [Test]
        public void AggregateChildToParentSortByAVGNoHitlist()
        {
            AggregateChildToParentSortByAVG(true, false);
        }

        /// <summary>
        /// Requires DataviewId 5003 and COETest schema.
        /// Searches for assayedcompounds with Formula =C14H14O4 (Full search). 6 records should be returned.
        /// And for those records 4 should have positive average pct of inhibition and 2 should be negative.
        /// Each of the records should have 3 child records, giving a total of 6 * 3 = 18.
        /// Orders by [Average % Inhibition]
        /// This method don't pages the results and do not use hitlists.
        /// </summary>
        [Test]
        public void AggregateChildToParentSortByAVGNoHitlistNoPaging()
        {
            AggregateChildToParentSortByAVG(true, true);
        }

        private void AggregateChildToParentSortByAVG(bool noHitlist, bool noPaging)
        {
            _resultsCriteria = GetResultsCriteria("ChildToParentRCSortByAVG.xml");
            _pagingInfo = GetFullPagingInfo();
            _searchCriteria = GetSearchCriteria(); // Where Formula =C14H14O4 (Full search)

            DataSet ds = GetDataSet(noHitlist, noPaging);

            Assert.IsTrue(ds.Tables[0] != null, "Null table returned by " + MethodInfo.GetCurrentMethod().Name);
            Assert.IsTrue(ds.Tables[0].Rows.Count == 6, "Expected 6 records and got " + ds.Tables[0].Rows.Count + " in " + MethodInfo.GetCurrentMethod().Name);

            Assert.IsTrue(ds.Tables[0].Select("[Average % Inhibition] > 0").Length == 4, "Expected 4 records with positive Average % Inhibition and got " + ds.Tables[0].Select("[Average % Inhibition] > 0").Length + " in method " + MethodInfo.GetCurrentMethod().Name);
            Assert.IsTrue(ds.Tables[0].Select("[Average % Inhibition] < 0").Length == 2, "Expected 2 records with negative Average % Inhibition and got " + ds.Tables[0].Select("[Average % Inhibition] < 0") + " in method " + MethodInfo.GetCurrentMethod().Name);
            Assert.IsTrue(Convert.ToDouble(ds.Tables[0].Compute("Sum([Average % Inhibition])", "")) == 127.21333333333333333333333332, "The sum of the Average % Inhibition was not the expected in method " + MethodInfo.GetCurrentMethod().Name);

            Assert.IsTrue(ds.Tables[0].Select("[ERalpha Primary Screen Count] > 0").Length == 6, "Expected 6 records with positive ERalpha Primary Screen Count and got different result in method " + MethodInfo.GetCurrentMethod().Name);
            Assert.IsTrue(Convert.ToInt32(ds.Tables[0].Compute("Avg([ERalpha Primary Screen Count])", "")) == 3, "The average of the ERalpha Primary Screen Count was not the expected in method " + MethodInfo.GetCurrentMethod().Name);
            Assert.IsTrue(Convert.ToInt32(ds.Tables[0].Compute("Sum([ERalpha Primary Screen Count])", "")) == 18, "The sum of the ERalpha Primary Screen Count was not the expected in method " + MethodInfo.GetCurrentMethod().Name);
        }

        /// <summary>
        /// Requires DataviewId 5003 and COETest schema.
        /// Searches for assayedcompounds with Formula =C14H14O4 (Full search). 6 records should be returned.
        /// And for those records 4 should have positive average pct of inhibition and 2 should be negative.
        /// Each of the records should have 3 child records, giving a total of 6 * 3 = 18.
        /// Orders by [MolWt]
        /// </summary>
        /// /// <summary>
        /// Requires DataviewId 5003 and COETest schema.
        /// Searches for assayedcompounds with Formula =C14H14O4 (Full search). 6 records should be returned.
        /// And for those records 4 should have positive average pct of inhibition and 2 should be negative.
        /// Each of the records should have 3 child records, giving a total of 6 * 3 = 18.
        /// Orders by [MolWt] and [Formula]
        /// This method pages the results and use hitlists.
        /// </summary>
        [Test]
        public void AggregateChildToParentSortByMolWtAndFormulaSearch()
        {
            AggregateChildToParentSortByMolWtAndFormula(false, false);
        }

        /// <summary>
        /// Requires DataviewId 5003 and COETest schema.
        /// Searches for assayedcompounds with Formula =C14H14O4 (Full search). 6 records should be returned.
        /// And for those records 4 should have positive average pct of inhibition and 2 should be negative.
        /// Each of the records should have 3 child records, giving a total of 6 * 3 = 18.
        /// Orders by [MolWt] and [Formula]
        /// This method pages the results, but do not use hitlists.
        /// </summary>
        [Test]
        public void AggregateChildToParentSortByMolWtAndFormulaNoHitlist()
        {
            AggregateChildToParentSortByMolWtAndFormula(true, false);
        }

        /// <summary>
        /// Requires DataviewId 5003 and COETest schema.
        /// Searches for assayedcompounds with Formula =C14H14O4 (Full search). 6 records should be returned.
        /// And for those records 4 should have positive average pct of inhibition and 2 should be negative.
        /// Each of the records should have 3 child records, giving a total of 6 * 3 = 18.
        /// Orders by [MolWt] and [Formula]
        /// This method don't pages the results and do not use hitlists.
        /// </summary>
        [Test]
        public void AggregateChildToParentSortByMolWtAndFormulaNoHitlistNoPaging()
        {
            AggregateChildToParentSortByMolWtAndFormula(true, true);
        }

        private void AggregateChildToParentSortByMolWtAndFormula(bool noHitlist, bool noPaging)
        {
            _resultsCriteria = GetResultsCriteria("ChildToParentRCSortByMolWtAndFormula.xml");
            _pagingInfo = GetFullPagingInfo();
            _searchCriteria = GetSearchCriteria(); // Where Formula =C14H14O4 (Full search)

            DataSet ds = GetDataSet(noHitlist, noPaging);

            Assert.IsTrue(ds.Tables[0] != null, "Null table returned by " + MethodInfo.GetCurrentMethod().Name);
            Assert.IsTrue(ds.Tables[0].Rows.Count == 6, "Expected 6 records and got " + ds.Tables[0].Rows.Count + " in " + MethodInfo.GetCurrentMethod().Name);

            Assert.IsTrue(ds.Tables[0].Select("[Average % Inhibition] > 0").Length == 4, "Expected 4 records with positive Average % Inhibition and got " + ds.Tables[0].Select("[Average % Inhibition] > 0").Length + " in method " + MethodInfo.GetCurrentMethod().Name);
            Assert.IsTrue(ds.Tables[0].Select("[Average % Inhibition] < 0").Length == 2, "Expected 2 records with negative Average % Inhibition and got " + ds.Tables[0].Select("[Average % Inhibition] < 0") + " in method " + MethodInfo.GetCurrentMethod().Name);
            Assert.IsTrue(Convert.ToDouble(ds.Tables[0].Compute("Sum([Average % Inhibition])", "")) == 127.21333333333333333333333332, "The sum of the Average % Inhibition was not the expected in method " + MethodInfo.GetCurrentMethod().Name);

            Assert.IsTrue(ds.Tables[0].Select("[ERalpha Primary Screen Count] > 0").Length == 6, "Expected 6 records with positive ERalpha Primary Screen Count and got different result in method " + MethodInfo.GetCurrentMethod().Name);
            Assert.IsTrue(Convert.ToInt32(ds.Tables[0].Compute("Avg([ERalpha Primary Screen Count])", "")) == 3, "The average of the ERalpha Primary Screen Count was not the expected in method " + MethodInfo.GetCurrentMethod().Name);
            Assert.IsTrue(Convert.ToInt32(ds.Tables[0].Compute("Sum([ERalpha Primary Screen Count])", "")) == 18, "The sum of the ERalpha Primary Screen Count was not the expected in method " + MethodInfo.GetCurrentMethod().Name);
        }
        /// <summary>
        /// Requires DataviewId 5003 and COETest schema.
        /// Searches for assayedcompounds with Formula =C14H14O4 (Full search). 6 records should be returned.
        /// And for those records 4 should have positive average pct of inhibition and 2 should be negative.
        /// Each of the records should have 3 child records, giving a total of 6 * 3 = 18.
        /// Orders by [Average % Inhibition] Descending
        /// This method pages the results and use hitlists.
        /// </summary>
        [Test]
        public void AggregateChildToParentSortByAVGDescSearch()
        {
            AggregateChildToParentSortByAVGDesc(false, false);
        }

        /// <summary>
        /// Requires DataviewId 5003 and COETest schema.
        /// Searches for assayedcompounds with Formula =C14H14O4 (Full search). 6 records should be returned.
        /// And for those records 4 should have positive average pct of inhibition and 2 should be negative.
        /// Each of the records should have 3 child records, giving a total of 6 * 3 = 18.
        /// Orders by [Average % Inhibition] Descending
        /// This method pages the results, but do not use hitlists.
        /// </summary>
        [Test]
        public void AggregateChildToParentSortByAVGDescNoHitlist()
        {
            AggregateChildToParentSortByAVGDesc(true, false);
        }

        /// <summary>
        /// Requires DataviewId 5003 and COETest schema.
        /// Searches for assayedcompounds with Formula =C14H14O4 (Full search). 6 records should be returned.
        /// And for those records 4 should have positive average pct of inhibition and 2 should be negative.
        /// Each of the records should have 3 child records, giving a total of 6 * 3 = 18.
        /// Orders by [Average % Inhibition] Descending
        /// This method don't pages the results and do not use hitlists.
        /// </summary>
        [Test]
        public void AggregateChildToParentSortByAVGDescNoHitlistNoPaging()
        {
            AggregateChildToParentSortByAVGDesc(true, true);
        }

        private void AggregateChildToParentSortByAVGDesc(bool noHitlist, bool noPaging)
        {
            _resultsCriteria = GetResultsCriteria("ChildToParentRCSortByAVGDesc.xml");
            _pagingInfo = GetFullPagingInfo();
            _searchCriteria = GetSearchCriteria(); // Where Formula =C14H14O4 (Full search)

            DataSet ds = GetDataSet(noHitlist, noPaging);

            Assert.IsTrue(ds.Tables[0] != null, "Null table returned by " + MethodInfo.GetCurrentMethod().Name);
            Assert.IsTrue(ds.Tables[0].Rows.Count == 6, "Expected 6 records and got " + ds.Tables[0].Rows.Count + " in " + MethodInfo.GetCurrentMethod().Name);

            Assert.IsTrue(ds.Tables[0].Select("[Average % Inhibition] > 0").Length == 4, "Expected 4 records with positive Average % Inhibition and got " + ds.Tables[0].Select("[Average % Inhibition] > 0").Length + " in method " + MethodInfo.GetCurrentMethod().Name);
            Assert.IsTrue(ds.Tables[0].Select("[Average % Inhibition] < 0").Length == 2, "Expected 2 records with negative Average % Inhibition and got " + ds.Tables[0].Select("[Average % Inhibition] < 0") + " in method " + MethodInfo.GetCurrentMethod().Name);
            Assert.IsTrue(Convert.ToDouble(ds.Tables[0].Compute("Sum([Average % Inhibition])", "")) == 127.21333333333333333333333332, "The sum of the Average % Inhibition was not the expected in method " + MethodInfo.GetCurrentMethod().Name);

            Assert.IsTrue(ds.Tables[0].Select("[ERalpha Primary Screen Count] > 0").Length == 6, "Expected 6 records with positive ERalpha Primary Screen Count and got different result in method " + MethodInfo.GetCurrentMethod().Name);
            Assert.IsTrue(Convert.ToInt32(ds.Tables[0].Compute("Avg([ERalpha Primary Screen Count])", "")) == 3, "The average of the ERalpha Primary Screen Count was not the expected in method " + MethodInfo.GetCurrentMethod().Name);
            Assert.IsTrue(Convert.ToInt32(ds.Tables[0].Compute("Sum([ERalpha Primary Screen Count])", "")) == 18, "The sum of the ERalpha Primary Screen Count was not the expected in method " + MethodInfo.GetCurrentMethod().Name);

            double lastAvg = double.MaxValue;
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                if (Convert.ToDouble(row["Average % Inhibition"]) <= lastAvg)
                    lastAvg = Convert.ToDouble(row["Average % Inhibition"]);
                else
                    throw new Exception("AggregateChildToParentSortByAVGDesc did not sort properly");

            }
        }

        /// <summary>
        /// Requires DataviewId 5003 and COETest schema.
        /// Searches for assayedcompounds with eralpha childs with average between 15 and 18. 1482 records should be returned.
        /// Then the first page of 15 records is fetched.
        /// Each of the records should have 3 child records, giving a total of 6 * 15 = 45.
        /// Orders by [ERalpha Primary Screen Count] and [PUBCHEM_CID]
        /// This method pages the results and use hitlists.
        /// </summary>
        [Test]
        public void AggregateChildToParentSortByCOUNTAndFIELDSearch()
        {
            AggregateChildToParentSortByCOUNTAndFIELD(false, false);
        }

        /// <summary>
        /// Requires DataviewId 5003 and COETest schema.
        /// Searches for assayedcompounds with eralpha childs with average between 15 and 18. 1482 records should be returned.
        /// Then the first page of 15 records is fetched.
        /// Each of the records should have 3 child records, giving a total of 6 * 15 = 45.
        /// Orders by [ERalpha Primary Screen Count] and [PUBCHEM_CID]
        /// This method pages the results, but do not use hitlists.
        /// </summary>
        [Test]
        public void AggregateChildToParentSortByCOUNTAndFIELDNoHitlist()
        {
            AggregateChildToParentSortByCOUNTAndFIELD(true, false);
        }

        /// <summary>
        /// Requires DataviewId 5003 and COETest schema.
        /// Searches for assayedcompounds with eralpha childs with average between 15 and 18. 1482 records should be returned.
        /// Then the first page of 15 records is fetched.
        /// Each of the records should have 3 child records, giving a total of 6 * 15 = 45.
        /// Orders by [ERalpha Primary Screen Count] and [PUBCHEM_CID]
        /// This method don't pages the results and do not use hitlists.
        /// </summary>
        [Test]
        public void AggregateChildToParentSortByCOUNTAndFIELDNoHitlistNoPaging()
        {
            AggregateChildToParentSortByCOUNTAndFIELD(true, true);
        }

        private void AggregateChildToParentSortByCOUNTAndFIELD(bool noHitlist, bool noPaging)
        {
            _resultsCriteria = GetResultsCriteria("ChildToParentRCSortByCountAndField.xml");
            _pagingInfo = GetFullPagingInfo();

            _searchCriteria = new SearchCriteria();
            SearchCriteria.NumericalCriteria numericalCriteria = new SearchCriteria.NumericalCriteria();
            numericalCriteria.Operator = SearchCriteria.COEOperators.EQUAL;
            numericalCriteria.Value = "15-18";
            SearchCriteria.SearchCriteriaItem item = new SearchCriteria.SearchCriteriaItem();
            item.TableId = 1901; //AID629ERA
            item.FieldId = 1903; //& Inhibition
            item.AggregateFunctionName = "AVG"; //AGGREGATE THE FIELD BY COUNT
            item.Criterium = numericalCriteria;
            _searchCriteria.Items.Add(item);

            if (!noHitlist)
            {
                HitListInfo info = _searchService.GetHitList(_searchCriteria, _dataViewID);
                Assert.IsTrue(info.RecordCount == 1482, "Expected 1482 records and got " + info.RecordCount + " in " + MethodInfo.GetCurrentMethod().Name);
            }
            else
            {
                _pagingInfo.RecordCount = 15; //First page of 15 so we can compare pages in parent and child table
                DataSet ds = GetDataSet(noHitlist, noPaging);
                int expectedCount = noPaging ? 1482 : 15;
                decimal averageSum = noPaging ? 24297.796666666666666666666666M : 225.17333333333333333333333332M;
                int countSum = 3 * expectedCount;

                Assert.IsTrue(ds.Tables[0] != null, "Null table returned by " + MethodInfo.GetCurrentMethod().Name);
                Assert.IsTrue(ds.Tables[0].Rows.Count == expectedCount, "Expected " + expectedCount + " records and got " + ds.Tables[0].Rows.Count + " in " + MethodInfo.GetCurrentMethod().Name);

                Assert.IsTrue(ds.Tables[0].Select("[Average % Inhibition] >= 15").Length == expectedCount, "Expected " + expectedCount + "records with Average % Inhibition >= expectedCount and got " + ds.Tables[0].Select("[Average % Inhibition] >= 15").Length + " in method " + MethodInfo.GetCurrentMethod().Name);
                Assert.IsTrue(ds.Tables[0].Select("[Average % Inhibition] <= 18").Length == expectedCount, "Expected " + expectedCount + "records with Average % Inhibition <=18 and got " + ds.Tables[0].Select("[Average % Inhibition] <= 18") + " in method " + MethodInfo.GetCurrentMethod().Name);
                Assert.IsTrue(Convert.ToDecimal(ds.Tables[0].Compute("Sum([Average % Inhibition])", "")) == averageSum, "The sum of the Average % Inhibition was not the expected in method " + MethodInfo.GetCurrentMethod().Name);

                Assert.IsTrue(ds.Tables[0].Select("[ERalpha Primary Screen Count] > 0").Length == expectedCount, "Expected " + expectedCount + "records with positive ERalpha Primary Screen Count and got different result in method " + MethodInfo.GetCurrentMethod().Name);
                Assert.IsTrue(Convert.ToInt32(ds.Tables[0].Compute("Avg([ERalpha Primary Screen Count])", "")) == 3, "The average of the ERalpha Primary Screen Count was not the expected in method " + MethodInfo.GetCurrentMethod().Name);
                Assert.IsTrue(Convert.ToInt32(ds.Tables[0].Compute("Sum([ERalpha Primary Screen Count])", "")) == countSum, "The sum of the ERalpha Primary Screen Count was not the expected in method " + MethodInfo.GetCurrentMethod().Name);

                //check all the child table records have a corresponding record in the parent to assure the right page:
                foreach (DataRow row in ds.Tables[1].Rows)
                {
                    DataRow[] parentRows = row.GetParentRows(ds.Relations[0]);
                    Assert.IsTrue(parentRows.Length > 0, "Some records in the child table has no parent. Are the right records being retrieved for child tables?");
                }
            }
        }

        /// <summary>
        /// Requires DataviewId 5003 and COETest schema.
        /// Browses assayedcompounds with eralpha childs.
        /// Then the first page of 15 records is fetched.
        /// Each of the records should have 3 child records, giving a total of 6 * 15 = 45.
        /// Orders by [ERalpha Primary Screen Count] and [PUBCHEM_CID]
        /// </summary>
        [Test]
        public void AggregateChildToParentSortByCOUNTAndFIELDAvoidHitlist()
        {
            _resultsCriteria = GetResultsCriteria("ChildToParentRCSortByCountAndField.xml");
            _pagingInfo = GetFullPagingInfo();
            _pagingInfo.RecordCount = 15; //First page of 15 so we can compare pages in parent and child table

            DataSet ds = _searchService.GetData(_resultsCriteria, _pagingInfo, _dataViewID);

            Assert.IsTrue(ds.Tables[0] != null, "Null table returned by " + MethodInfo.GetCurrentMethod().Name);
            Assert.IsTrue(ds.Tables[0].Rows.Count == 15, "Expected 15 records and got " + ds.Tables[0].Rows.Count + " in " + MethodInfo.GetCurrentMethod().Name);

            Assert.IsTrue(ds.Tables[0].Select("[ERalpha Primary Screen Count] > 0").Length == 15, "Expected 15 records with positive ERalpha Primary Screen Count and got different result in method " + MethodInfo.GetCurrentMethod().Name);
            Assert.IsTrue(Convert.ToInt32(ds.Tables[0].Compute("Avg([ERalpha Primary Screen Count])", "")) == 3, "The average of the ERalpha Primary Screen Count was not the expected in method " + MethodInfo.GetCurrentMethod().Name);
            Assert.IsTrue(Convert.ToInt32(ds.Tables[0].Compute("Sum([ERalpha Primary Screen Count])", "")) == 45, "The sum of the ERalpha Primary Screen Count was not the expected in method " + MethodInfo.GetCurrentMethod().Name);

            //check all the child table records have a corresponding record in the parent to assure the right page:
            foreach (DataRow row in ds.Tables[1].Rows)
            {
                DataRow[] parentRows = row.GetParentRows(ds.Relations[0]);
                Assert.IsTrue(parentRows.Length > 0, "Some records in the child table has no parent. Are the right records being retrieved for child tables?");
            }
        }
        #endregion

        #region old Test
        /// <summary>
        /// This test has only meaning with registration installed, if you create your own data, and then test "manually".
        /// No good assert is provided, it was meant for debugging purposes and will be removed when we figure out a scenario in the coetest
        /// schema for grandchild.
        /// </summary>
        [Test]
        public void COETestSearchAggregateFunctionWithBatches()
        {
            COEDataView dataView = GetDataView("BatchesWithIdentifiersAndProjects.xml");
            _resultsCriteria = GetResultsCriteria("ResultsCriteriaForBatches.xml");
            _pagingInfo = new PagingInfo();
            _pagingInfo.Start = 1;
            _pagingInfo.RecordCount = 100;

            DataSet ds = _searchService.GetData(_resultsCriteria, _pagingInfo, dataView);

            Assert.IsTrue(ds.Tables["Table_13"] != null, "Batches table wasn't returned by " + MethodInfo.GetCurrentMethod().Name);
            Assert.AreEqual(_pagingInfo.RecordCount, ds.Tables[0].Rows.Count, "Incorrect number of rows returned by " + MethodInfo.GetCurrentMethod().Name);
            //Assert.IsTrue(ds.Tables[0].Select("[Average EC50] = 13.7").Length == 1, "No row returned Average EC50 in method COETestSearchAggregateFunction");
        }

        /// <summary>
        /// This test has only meaning with registration installed, if you create your own data, and then test "manually".
        /// No good assert is provided, it was meant for debugging purposes and will be removed when we figure out a scenario in the coetest
        /// schema for grandchild.
        /// The main focus of this is to prove that Child Tables can be sorted by aggregated clauses
        /// </summary>
        [Test]
        public void COETestSearchAggregateFunctionWithBatchesChildSortedByIdentifiersCount()
        {
            COEDataView dataView = GetDataView("BatchesWithIdentifiersAndProjects.xml");
            _resultsCriteria = GetResultsCriteria("ResultsCriteriaForBatches.xml");
            _resultsCriteria.Tables[1].Criterias[5].OrderById = 1;
            _pagingInfo = new PagingInfo();
            _pagingInfo.Start = 1;
            _pagingInfo.RecordCount = 100;

            DataSet ds = _searchService.GetData(_resultsCriteria, _pagingInfo, dataView);

            Assert.IsTrue(ds.Tables["Table_13"] != null, "Batches table wasn't returned by " + MethodInfo.GetCurrentMethod().Name);
            Assert.AreEqual(_pagingInfo.RecordCount, ds.Tables[0].Rows.Count, "Incorrect number of rows returned by " + MethodInfo.GetCurrentMethod().Name);
            //Assert.IsTrue(ds.Tables[0].Select("[Average EC50] = 13.7").Length == 1, "No row returned Average EC50 in method COETestSearchAggregateFunction");
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
            return this.GetSearchCriteria(@"SearchCriteria.xml");
        }

        private SearchCriteria GetSearchCriteria(string fileName)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(_pathToXmls + "\\" + fileName);
            SearchCriteria sc = new SearchCriteria(doc);
            return sc;
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

        private DataSet GetDataSet(bool noHitlist, bool noPaging, COEDataView dataview)
        {
            DataSet ds = new DataSet();
            if (noPaging)
            {
                _pagingInfo.End = _pagingInfo.RecordCount = 0;
            }

            if (noHitlist)
            {
                ds = _searchService.GetData(_resultsCriteria, _pagingInfo, dataview, "NO", null, _searchCriteria);
            }
            else
            {
                HitListInfo info = _searchService.GetHitList(_searchCriteria, dataview);
                _pagingInfo.HitListID = info.HitListID;
                _pagingInfo.HitListType = info.HitListType;


                ds = _searchService.GetData(_resultsCriteria, _pagingInfo, dataview);
            }

            return ds;
        }

        private DataSet GetDataSet(bool noHitlist, bool noPaging, Int32 dataviewID)
        {
            DataSet ds = new DataSet();
            if (noPaging)
            {
                _pagingInfo.End = _pagingInfo.RecordCount = 0;
            }

            if (noHitlist)
            {
                ds = _searchService.GetData(_resultsCriteria, _pagingInfo, dataviewID, "NO", null, _searchCriteria);
            }
            else
            {
                HitListInfo info = _searchService.GetHitList(_searchCriteria, dataviewID);
                _pagingInfo.HitListID = info.HitListID;
                _pagingInfo.HitListType = info.HitListType;


                ds = _searchService.GetData(_resultsCriteria, _pagingInfo, dataviewID);
            }

            return ds;
        }
        #endregion

        #region GetHitList


        /// <summary>
        /// Unit Test Method for GetHitList(SearchCriteria searchCriteria, int dataViewID, HitListInfo refineHitlist)
        /// </summary>
        [Test]
        public void GetHitListusingdvID()
        {
            HitListInfo info = _searchService.GetHitList(_searchCriteria, _dataViewID);
            Assert.IsNotNull(info, "COESearch.GetHitListusingdvID is not returning expected value");
        }
        /// <summary>
        /// Unit Test Method for HitListInfo GetHitList(SearchCriteria searchCriteria, COEDataView dataView)
        /// </summary>
        [Test]
        public void GetHitListusingdvIDHitList()
        {
            HitListInfo info = _searchService.GetHitList(_searchCriteria, theCOEDataViewBO.COEDataView);
            Assert.IsNotNull(info, "COESearch.GetHitListusingdvIDHitList is not returning expected value");
        }
        /// <summary>
        /// Unit Test Method for GetHitList(SearchCriteria searchCriteria, COEDataView dataView, HitListInfo refineHitlist)
        /// </summary>
        [Test]
        public void GetHitListusingdvHitList()
        {
            HitListInfo info = _searchService.GetHitList(_searchCriteria, theCOEDataViewBO.COEDataView, null);
            Assert.IsNotNull(info, "COESearch.GetHitListusingdvHitList is not returning expected value");
        }
        #endregion

        #region GetHitListProgress

        /// <summary>
        /// Unit Test Method For GetHitListProgress(HitListInfo hitListInfo, int dataViewID)
        /// </summary>
        [Test]
        public void GetHitListProgressusingdvIDHitList()
        {
            HitListInfo info = _searchService.GetPartialHitList(_searchCriteria, theCOEDataViewBO.COEDataView, null);
            Assert.IsNotNull(info, "COESearch.GetHitListusingdvHitList is not returning expected value");
        }
        /// <summary>
        /// Unit Test for GetHitListProgress(HitListInfo hitListInfo, COEDataView dataView)
        /// </summary>
        [Test]
        public void GetHitListProgressusingdvHitList()
        {
            HitListInfo info = _searchService.GetPartialHitList(_searchCriteria, theCOEDataViewBO.COEDataView);
            Assert.IsNotNull(info, "COESearch.GetHitListusingdvHitList is not returning expected value");
        }
        #endregion

        #region GetExactRecordCount

        /// <summary>
        /// Unit Test For int GetExactRecordCount(int dataViewID)
        /// </summary>
        [Test]
        public void GetExactRecordCountUnigDvIDTest()
        {
            int expectedvalue = 75637;
            int actual = _searchService.GetExactRecordCount(_dataViewID);
            Assert.AreEqual(expectedvalue, actual, "COESearch.GetExactRecordCountUnigDvIDTest is not returning expected value");

        }

        /// <summary>
        /// Unit Test For int GetExactRecordCount(COEDataView dataView)
        /// </summary>
        [Test]
        public void GetExactRecordCountUsingDvTest()
        {
            int expectedvalue = 75637;
            int actual = _searchService.GetExactRecordCount(theCOEDataViewBO.COEDataView);
            Assert.AreEqual(expectedvalue, actual, "COESearch.GetExactRecordCountUnigDvIDTest is not returning expected value");
        }

        #endregion
    }
}
