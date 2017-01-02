using System;
using System.Data;
using System.Xml;
using CambridgeSoft.COE.Framework.COESearchService;
using CambridgeSoft.COE.Framework.COESecurityService;
using CambridgeSoft.COE.Framework.Common;
using NUnit.Framework;
using System.Data.Common;
using CambridgeSoft.COE.Framework.COEHitListService;
using System.Reflection;
using CambridgeSoft.COE.Framework.NUnitTests.Helpers;
using CambridgeSoft.COE.Framework.COEDataViewService;

namespace SearchUnitTests
{
    /// <summary>
    /// Summary description for COESearchTest
    /// </summary>
    [TestFixture]
    public class COESearchTest
    {
        private COESearch _searchService;
        private SearchCriteria _searchCriteria;
        private ResultsCriteria _resultsCriteria;
        private PagingInfo _pagingInfo;
        private COEDataView _dataView;
        private string _useRealTableNames;
        private OrderByCriteria _orderByCriteria;
        private string pathToXmls;
        private static DALFactory _df;
        private static CambridgeSoft.COE.Framework.COESearchService.DAL _dal;
        private COEDataViewBO theCOEDataViewBO = null;
        private HitListInfo theHitListInfo = null;
        public COESearchTest()

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
            ExecuteCommand("delete from coetest.moltable where id in(401, 402)");
            ExecuteCommand("insert into coetest.moltable(ID, MOL_ID, MOLNAME, BASE64_CDX) values(401, 401, 'butanedioic acid, 2,3-dihydroxy-, [R-(R*,R*)]-', '')");
            ExecuteCommand("insert into coetest.moltable(ID, MOL_ID, MOLNAME, BASE64_CDX) values(402, 402, 'butanedioic acid, 2,3-dihydroxy- (R*,R*)-(+-)-', '')");
        }

        private static void LoadDAL()
        {
            if (_df == null)
                _df = new DALFactory();
            if (_dal == null)
                _df.GetDAL<CambridgeSoft.COE.Framework.COESearchService.DAL>(ref _dal, "COESearch", "COEDB", true);
        }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        [TestFixtureTearDown]
        public static void MyClassCleanup()
        {
            ExecuteCommand("delete from coetest.moltable where id in(401, 402)");

        }
        //
        // Use TestInitialize to run code before running each test 
        [SetUp]
        public void MyTestInitialize()
        {
            Login();
            _pagingInfo = GetFullPagingInfo();
            pathToXmls = SearchHelper.GetExecutingTestResultsBasePath(SearchHelper._COESearchTestpathToXml);
            _searchCriteria = GetSearchCriteria();
            _resultsCriteria = GetResultsCriteria();
            _dataView = GetDataView();
            _useRealTableNames = "YES";
            _orderByCriteria = GetOrderByCriteria();
            theCOEDataViewBO = CreatDataView();
            theHitListInfo = GetHitListInfo();
        }

        //
        // Use TestCleanup to run code after each test has run
        // [TearDown]
        // public void MyTestCleanup() { }
        //
        #endregion

        #region Test Methods
        [Test]
        public void COETEST_OrderedByMolWeight_ASC_Search()
        {
            COETEST_OrderedByMolWeight_ASC(false, false);
        }

        [Test]
        public void COETEST_OrderedByMolWeight_ASC_NoHitlist()
        {
            COETEST_OrderedByMolWeight_ASC(true, false);
        }

        [Test]
        public void COETEST_OrderedByMolWeight_ASC_NoHitlistNoPaging()
        {
            COETEST_OrderedByMolWeight_ASC(true, true);
        }

        private void COETEST_OrderedByMolWeight_ASC(bool noHitlist, bool noPaging)
        {
            DataSet ds = GetDataSet(noHitlist, noPaging);
            Assert.IsTrue(ds != null);

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                Console.WriteLine(row["MolWeight"].ToString());
            }
        }

        [Test]
        public void COETEST_DataviewHandling_Merge_Search()
        {
            COETEST_DataviewHandling_Merge(false, false);
        }

        [Test]
        public void COETEST_DataviewHandling_Merge_NoHitlist()
        {
            COETEST_DataviewHandling_Merge(true, false);
        }

        [Test]
        public void COETEST_DataviewHandling_Merge_NoHitlistNoPaging()
        {
            COETEST_DataviewHandling_Merge(true, true);
        }

        private void COETEST_DataviewHandling_Merge(bool noHitlist, bool noPaging)
        {
            _dataView.DataViewID = 5002;
            _pagingInfo.RecordCount = 10;
            _dataView.DataViewHandling = COEDataView.DataViewHandlingOptions.MERGE_CLIENT_AND_SERVER_DATAVIEW;
            _dataView.Basetable = 220;
            _searchCriteria = this.GetSearchCriteria(@"\SearchCriteria1.xml");
            _resultsCriteria = this.GetResultsCriteria(@"\ResultsCriteria1.xml");

            DataSet ds = GetDataSet(noHitlist, noPaging);

            Assert.IsTrue(ds != null);

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                Console.WriteLine(row["MOL_ID"].ToString());
            }
        }

        [Test]
        public void COETEST_SearchOrderedById_ASC_Search()
        {
            COETEST_SearchOrderedById_ASC(false, false);
        }

        [Test]
        public void COETEST_SearchOrderedById_ASC_NoHitlist()
        {
            COETEST_SearchOrderedById_ASC(true, false);
        }

        [Test]
        public void COETEST_SearchOrderedById_ASC_NoHitlistNoPaging()
        {
            COETEST_SearchOrderedById_ASC(true, true);
        }

        private void COETEST_SearchOrderedById_ASC(bool noHitlist, bool noPaging)
        {

            pathToXmls = SearchHelper.GetExecutingTestResultsBasePath(@"\Search Tests\COESearchTest XML");
            //pathToXmls = AppDomain.CurrentDomain.BaseDirectory.Substring(0, AppDomain.CurrentDomain.BaseDirectory.IndexOf("CambridgeSoft.COE.Framework.NUnitTests")) + @"CambridgeSoft.COE.Framework.NUnitTests" + @"\Search Tests\COESearchTest XML";
            _dataView = GetDataView();
            _searchCriteria = GetSearchCriteria();
            _resultsCriteria = GetResultsCriteria();
            _pagingInfo.RecordCount = 25;
            _pagingInfo.Start = 1;

            DataSet ds = GetDataSet(noHitlist, noPaging);

            Assert.IsTrue(ds.Tables[0] != null, "Null table returned by COETestSearchOrderedById");
            Assert.AreEqual(_pagingInfo.RecordCount, ds.Tables[0].Rows.Count, "Incorrect number of rows returned by COETestSearchOrderedById");
            if (noPaging)
                Assert.AreEqual(293, _pagingInfo.RecordCount, "Incorrect number of rows returned by COETestSearchOrderedById");
            else
                Assert.AreEqual(25, _pagingInfo.RecordCount, "Incorrect number of rows returned by COETestSearchOrderedById");
        }

        [Test]
        public void SearchOverGrandChild_Search()
        {
            SearchOverGrandChild(false, false);
        }

        [Test]
        public void SearchOverGrandChild_NoHitlist()
        {
            SearchOverGrandChild(true, false);
        }

        [Test]
        public void SearchOverGrandChild_NoHitlistNoPaging()
        {
            SearchOverGrandChild(true, true);
        }

        private void SearchOverGrandChild(bool noHitlist, bool noPaging)
        {
            pathToXmls = SearchHelper.GetExecutingTestResultsBasePath(@"\Search Tests\COESearchTest XML");
            _dataView = GetDataViewWithOuterJoins();
            _searchCriteria = GetSearchCriteria();
            SearchCriteria.NumericalCriteria molidcriteria = new SearchCriteria.NumericalCriteria();
            molidcriteria.Operator = SearchCriteria.COEOperators.GTE;
            molidcriteria.Value = "1";
            SearchCriteria.SearchCriteriaItem item = new SearchCriteria.SearchCriteriaItem();
            item.TableId = 220;
            item.FieldId = 222;
            item.ID = 2;
            item.Criterium = molidcriteria;
            _searchCriteria.Items.Add(item);

            _resultsCriteria = GetResultsCriteria();

            ResultsCriteria.ResultsCriteriaTable propsTable = new ResultsCriteria.ResultsCriteriaTable(220);
            ResultsCriteria.Field clogP = new ResultsCriteria.Field(223);
            propsTable.Criterias.Add(clogP);
            _resultsCriteria.Tables.Add(propsTable);

            _pagingInfo.RecordCount = 25;
            _pagingInfo.Start = 1;
            _useRealTableNames = "NO";
            DataSet ds = GetDataSet(noHitlist, noPaging); ;

            Assert.IsTrue(ds.Tables[2] != null, "Null table returned by DoSearchTestOverGrandChild");
            Assert.IsTrue(ds.Tables[2].Rows.Count > 0, "No rows returned in grandchild table by DoSearchTestOverGrandChild");
            Assert.AreEqual(_pagingInfo.RecordCount, ds.Tables[0].Rows.Count, "Incorrect number of rows returned by DoSearchTestOverGrandChild");
            if (noPaging)
                Assert.AreEqual(226, _pagingInfo.RecordCount, "Incorrect number of rows returned by DoSearchTestOverGrandChild");
            else
                Assert.AreEqual(25, _pagingInfo.RecordCount, "Incorrect number of rows returned by DoSearchTestOverGrandChild");
            Assert.IsTrue(ds.Relations.Count == 2, "There is an incorrect number of relations in the resulting dataset");
            Assert.IsTrue(ds.Relations[0].ParentTable.TableName == "Table_203", "Wrong relation was created");
            Assert.IsTrue(ds.Relations[0].ChildTable.TableName == "Table_210", "Wrong relation was created");
            Assert.IsTrue(ds.Relations[1].ParentTable.TableName == "Table_210", "Wrong relation was created");
            Assert.IsTrue(ds.Relations[1].ChildTable.TableName == "Table_220", "Wrong relation was created");
        }

        [Test]
        public void SearchWithNullGrandChildValues_Search()
        {
            SearchTestWithNullGrandChildValues(false, false);
        }

        [Test]
        public void SearchWithNullGrandChildValues_NoHitlist()
        {
            SearchTestWithNullGrandChildValues(true, false);
        }

        [Test]
        public void SearchWithNullGrandChildValues_NoHitlistNoPaging()
        {
            SearchTestWithNullGrandChildValues(true, true);
        }

        private void SearchTestWithNullGrandChildValues(bool noHitlist, bool noPaging)
        {
            /*
             * select mol_id, syn_id from coetest.moltable, coetest.synonyms_r
             * where moltable.mol_id = synonyms_r.syn_id(+)
             * and syn_id is null;
             */

            pathToXmls = SearchHelper.GetExecutingTestResultsBasePath(@"\Search Tests\COESearchTest XML");
            _dataView = GetDataViewWithOuterJoins();
            _searchCriteria = GetSearchCriteria();
            _resultsCriteria = GetResultsCriteria();

            ResultsCriteria.ResultsCriteriaTable propsTable = new ResultsCriteria.ResultsCriteriaTable(220);
            ResultsCriteria.Field clogP = new ResultsCriteria.Field(223);
            propsTable.Criterias.Add(clogP);
            _resultsCriteria.Tables.Add(propsTable);

            _pagingInfo.RecordCount = 25;
            _pagingInfo.Start = 1;
            _useRealTableNames = "NO";

            DataSet ds = GetDataSet(noHitlist, noPaging);

            Assert.IsTrue(ds.Tables[2] != null, "Null table returned by DoSearchTestOverGrandChild");
            Assert.IsTrue(ds.Tables[2].Rows.Count > 0, "No rows returned in grandchild table by DoSearchTestOverGrandChild");
            Assert.IsTrue(HasNullValues(ds.Tables[2].Rows), "There are no null values, the test is invalid"); //Null values are not retrieved
            Assert.AreEqual(_pagingInfo.RecordCount, ds.Tables[0].Rows.Count, "Incorrect number of rows returned by DoSearchTestOverGrandChild");
            Assert.IsTrue(ds.Relations.Count == 2, "There is an incorrect number of relations in the resulting dataset");
            Assert.IsTrue(ds.Relations[0].ParentTable.TableName == "Table_203", "Wrong relation was created");
            Assert.IsTrue(ds.Relations[0].ChildTable.TableName == "Table_210", "Wrong relation was created");
            Assert.IsTrue(ds.Relations[1].ParentTable.TableName == "Table_210", "Wrong relation was created");
            Assert.IsTrue(ds.Relations[1].ChildTable.TableName == "Table_220", "Wrong relation was created");
        }

        [Test]
        public void WhereClauseInParsing_Search()
        {
            WhereClauseInParsing(false, false);
        }

        [Test]
        public void WhereClauseInParsing_NoHitlist()
        {
            WhereClauseInParsing(true, false);
        }

        [Test]
        public void WhereClauseInParsing_NoHitlistNoPaging()
        {
            WhereClauseInParsing(true, true);
        }

        private void WhereClauseInParsing(bool noHitlist, bool noPaging)
        {
            pathToXmls = SearchHelper.GetExecutingTestResultsBasePath(@"\Search Tests\COESearchTest XML");
            _dataView = GetDataView();
            _searchCriteria = GetSearchCriteria();
            SearchCriteria.NumericalCriteria incriteria = new SearchCriteria.NumericalCriteria();
            incriteria.Operator = SearchCriteria.COEOperators.IN;
            incriteria.Value = "1 , 2 ,3 , 4,5,6";
            SearchCriteria.SearchCriteriaItem item = new SearchCriteria.SearchCriteriaItem();
            item.TableId = 203;
            item.FieldId = 206;
            item.ID = 2;
            item.Criterium = incriteria;
            _searchCriteria.Items.Add(item);
            _resultsCriteria = GetResultsCriteria();
            _pagingInfo = GetFullPagingInfo();

            DataSet ds = GetDataSet(noHitlist, noPaging);
            Assert.AreEqual(6, ds.Tables[0].Rows.Count, "WhereClauseInParsing did not return the expected count");

            _pagingInfo = GetFullPagingInfo();
            _searchCriteria.Items[1].GetSearchCriteriaItem(2).Criterium.Value = "1-3";
            ds = GetDataSet(noHitlist, noPaging);
            Assert.AreEqual(3, ds.Tables[0].Rows.Count, "WhereClauseInParsing did not return the expected count");

            _pagingInfo = GetFullPagingInfo();
            _searchCriteria.Items[1].GetSearchCriteriaItem(2).Criterium.Value = "1-3;4,5";
            ds = GetDataSet(noHitlist, noPaging);
            Assert.AreEqual(5, ds.Tables[0].Rows.Count, "WhereClauseInParsing did not return the expected count");

            _pagingInfo = GetFullPagingInfo();
            _searchCriteria.Items[1].GetSearchCriteriaItem(2).Criterium.Value = "-3--1;4,5";
            ds = GetDataSet(noHitlist, noPaging);
            Assert.AreEqual(2, ds.Tables[0].Rows.Count, "WhereClauseInParsing did not return the expected count");

            _pagingInfo = GetFullPagingInfo();
            _searchCriteria = GetSearchCriteria();
            SearchCriteria.TextCriteria inTextCriteria = new SearchCriteria.TextCriteria();
            inTextCriteria.Operator = SearchCriteria.COEOperators.IN;
            inTextCriteria.CaseSensitive = SearchCriteria.COEBoolean.No;
            inTextCriteria.Value = "pYriDInE, iNdeNe";
            SearchCriteria.SearchCriteriaItem item1 = new SearchCriteria.SearchCriteriaItem();
            item1.TableId = 203;
            item1.FieldId = 207;
            item1.ID = 2;
            item1.Criterium = inTextCriteria;
            _searchCriteria.Items.Add(item1);
            ds = GetDataSet(noHitlist, noPaging);
            Assert.AreEqual(2, ds.Tables[0].Rows.Count, "WhereClauseInParsing did not return the expected count");
        }



        //To test CSBR-152027
        [Test]
        public void UpdateCriteriaRange_Search()
        {
            UpdateCriteriaRange(false, false);
        }

        [Test]
        public void UpdateCriteriaRange_NoHitlist()
        {
            UpdateCriteriaRange(true, false);
        }

        [Test]
        public void UpdateCriteriaRange_NoHitlistNoPaging()
        {
            UpdateCriteriaRange(true, true);
        }

        private void UpdateCriteriaRange(bool noHitlist, bool noPaging)
        {
            pathToXmls = SearchHelper.GetExecutingTestResultsBasePath(@"\Search Tests\COESearchTest XML");
            _dataView = GetDataView();
            _searchCriteria = GetSearchCriteria();
            SearchCriteria.NumericalCriteria incriteria = new SearchCriteria.NumericalCriteria();
            incriteria.Operator = SearchCriteria.COEOperators.IN;
            incriteria.Value = "-1 , 2 ,3 , -4,5,6";
            SearchCriteria.SearchCriteriaItem item = new SearchCriteria.SearchCriteriaItem();
            item.TableId = 203;
            item.FieldId = 206;
            item.ID = 2;
            item.Criterium = incriteria;
            _searchCriteria.Items.Add(item);
            _resultsCriteria = GetResultsCriteria();
            _pagingInfo = GetFullPagingInfo();

            DataSet ds = GetDataSet(noHitlist, noPaging);

            string[] inputCriteriaArray = { "1,2 ,3 , 4,5,6", "-3--1;4,5", "1-3;4,5", "-4-2", "-4--2;5,-7", "1-3", "5;-7-5;2", "1 , 2 ,3 , -4,5,-6", "-2, -4, 7", "2,-4,-5" };
            // string[] inputCriteriaArray = { "1 , 2 ,3 , -4,5,-6", "-2, -4, 7", "2,-4,-5","-2-6","-5--2" };

            foreach (string inputString in inputCriteriaArray)
            {
                _searchCriteria.Items[1].GetSearchCriteriaItem(2).Criterium.Value = inputString;
                ds = GetDataSet(noHitlist, noPaging);
                ds.Clear();
            }

        }


        [Test]
        public void SearchChemicalNames_Search()
        {
            SearchChemicalNames(false, false);
        }

        [Test]
        public void SearchChemicalNames_NoHitlist()
        {
            SearchChemicalNames(true, false);
        }

        [Test]
        public void SearchChemicalNames_NoHitlistNoPaging()
        {
            SearchChemicalNames(true, true);
        }

        private void SearchChemicalNames(bool noHitlist, bool noPaging)
        {
            /*
                =butanedioic acid, 2,3-dihydroxy-, [R-(R*,R*)]-  finds itself OK
                =butanedioic acid, 2,3-dihydroxy- (R*,R*)-(+-)- doesn't find itself, and should
            */

            pathToXmls = SearchHelper.GetExecutingTestResultsBasePath(@"\Search Tests\COESearchTest XML");
            _dataView = GetDataView();
            _searchCriteria = GetSearchCriteria();
            SearchCriteria.TextCriteria chemnamecriteria = new SearchCriteria.TextCriteria();
            chemnamecriteria.NormalizedChemicalName = SearchCriteria.COEBoolean.No;
            chemnamecriteria.Operator = SearchCriteria.COEOperators.LIKE;
            chemnamecriteria.Value = "butanedioic acid, 2,3-dihydroxy-, [R-(R*,R*)]-";
            SearchCriteria.SearchCriteriaItem item = new SearchCriteria.SearchCriteriaItem();
            item.TableId = 203; //Moltable
            item.FieldId = 207; //Molname
            item.ID = 2;
            item.Criterium = chemnamecriteria;
            _searchCriteria.Items.Add(item);
            _resultsCriteria = GetResultsCriteria();
            _pagingInfo = GetFullPagingInfo();

            DataSet ds = GetDataSet(noHitlist, noPaging);

            Assert.IsTrue(ds.Tables[0].Rows.Count == 1);

            _searchCriteria = GetSearchCriteria();
            chemnamecriteria = new SearchCriteria.TextCriteria();
            chemnamecriteria.NormalizedChemicalName = SearchCriteria.COEBoolean.No;
            chemnamecriteria.Operator = SearchCriteria.COEOperators.LIKE;
            chemnamecriteria.Value = "butanedioic acid, 2,3-dihydroxy- (R*,R*)-(+-)-";
            item = new SearchCriteria.SearchCriteriaItem();
            item.TableId = 203; //Moltable
            item.FieldId = 207; //Molname
            item.ID = 2;
            item.Criterium = chemnamecriteria;
            _searchCriteria.Items.Add(item);
            _resultsCriteria = GetResultsCriteria();
            _pagingInfo = GetFullPagingInfo();

            ds = GetDataSet(noHitlist, noPaging);

            Assert.IsTrue(ds.Tables[0].Rows.Count == 1);

            _searchCriteria = GetSearchCriteria();
            chemnamecriteria = new SearchCriteria.TextCriteria();
            chemnamecriteria.NormalizedChemicalName = SearchCriteria.COEBoolean.No;
            chemnamecriteria.Operator = SearchCriteria.COEOperators.EQUAL;
            chemnamecriteria.Value = "butanedioic acid, 2,3-dihydroxy-, [R-(R*,R*)]-";
            item = new SearchCriteria.SearchCriteriaItem();
            item.TableId = 203; //Moltable
            item.FieldId = 207; //Molname
            item.ID = 2;
            item.Criterium = chemnamecriteria;
            _searchCriteria.Items.Add(item);
            _resultsCriteria = GetResultsCriteria();
            _pagingInfo = GetFullPagingInfo();

            ds = GetDataSet(noHitlist, noPaging);

            Assert.IsTrue(ds.Tables[0].Rows.Count == 1);

            _searchCriteria = GetSearchCriteria();
            chemnamecriteria = new SearchCriteria.TextCriteria();
            chemnamecriteria.NormalizedChemicalName = SearchCriteria.COEBoolean.No;
            chemnamecriteria.Operator = SearchCriteria.COEOperators.EQUAL;
            chemnamecriteria.Value = "butanedioic acid, 2,3-dihydroxy- (R*,R*)-(+-)-";
            item = new SearchCriteria.SearchCriteriaItem();
            item.TableId = 203; //Moltable
            item.FieldId = 207; //Molname
            item.ID = 2;
            item.Criterium = chemnamecriteria;
            _searchCriteria.Items.Add(item);
            _resultsCriteria = GetResultsCriteria();
            _pagingInfo = GetFullPagingInfo();

            ds = GetDataSet(noHitlist, noPaging);

            Assert.IsTrue(ds.Tables[0].Rows.Count == 1);
        }

        [Test]
        public void DoSearchTestChemicalNamesNoHitlist()
        {
            /*
                =butanedioic acid, 2,3-dihydroxy-, [R-(R*,R*)]-  finds itself OK
                =butanedioic acid, 2,3-dihydroxy- (R*,R*)-(+-)- doesn't find itself, and should
            */
            DataSet ds = null;
            pathToXmls = SearchHelper.GetExecutingTestResultsBasePath(@"\Search Tests\COESearchTest XML");
            _dataView = GetDataView();
            _searchCriteria = GetSearchCriteria();
            SearchCriteria.TextCriteria chemnamecriteria = new SearchCriteria.TextCriteria();
            chemnamecriteria.NormalizedChemicalName = SearchCriteria.COEBoolean.No;
            chemnamecriteria.Operator = SearchCriteria.COEOperators.LIKE;
            chemnamecriteria.Value = "butanedioic acid, 2,3-dihydroxy-, [R-(R*,R*)]-";
            SearchCriteria.SearchCriteriaItem item = new SearchCriteria.SearchCriteriaItem();
            item.TableId = 203; //Moltable
            item.FieldId = 207; //Molname
            item.ID = 2;
            item.Criterium = chemnamecriteria;
            _searchCriteria.Items.Add(item);
            _resultsCriteria = GetResultsCriteria();
            _pagingInfo = GetFullPagingInfo();

            ds = _searchService.GetData(_resultsCriteria, _pagingInfo, _dataView, "NO", null, _searchCriteria);

            Assert.IsTrue(ds.Tables[0].Rows.Count == 1);

            _searchCriteria = GetSearchCriteria();
            chemnamecriteria = new SearchCriteria.TextCriteria();
            chemnamecriteria.NormalizedChemicalName = SearchCriteria.COEBoolean.No;
            chemnamecriteria.Operator = SearchCriteria.COEOperators.LIKE;
            chemnamecriteria.Value = "butanedioic acid, 2,3-dihydroxy- (R*,R*)-(+-)-";
            item = new SearchCriteria.SearchCriteriaItem();
            item.TableId = 203; //Moltable
            item.FieldId = 207; //Molname
            item.ID = 2;
            item.Criterium = chemnamecriteria;
            _searchCriteria.Items.Add(item);
            _resultsCriteria = GetResultsCriteria();
            _pagingInfo = GetFullPagingInfo();

            ds = _searchService.GetData(_resultsCriteria, _pagingInfo, _dataView, "NO", null, _searchCriteria);

            Assert.IsTrue(ds.Tables[0].Rows.Count == 1);

            _searchCriteria = GetSearchCriteria();
            chemnamecriteria = new SearchCriteria.TextCriteria();
            chemnamecriteria.NormalizedChemicalName = SearchCriteria.COEBoolean.No;
            chemnamecriteria.Operator = SearchCriteria.COEOperators.EQUAL;
            chemnamecriteria.Value = "butanedioic acid, 2,3-dihydroxy-, [R-(R*,R*)]-";
            item = new SearchCriteria.SearchCriteriaItem();
            item.TableId = 203; //Moltable
            item.FieldId = 207; //Molname
            item.ID = 2;
            item.Criterium = chemnamecriteria;
            _searchCriteria.Items.Add(item);
            _resultsCriteria = GetResultsCriteria();
            _pagingInfo = GetFullPagingInfo();

            ds = _searchService.GetData(_resultsCriteria, _pagingInfo, _dataView, "NO", null, _searchCriteria);

            Assert.IsTrue(ds.Tables[0].Rows.Count == 1);

            _searchCriteria = GetSearchCriteria();
            chemnamecriteria = new SearchCriteria.TextCriteria();
            chemnamecriteria.NormalizedChemicalName = SearchCriteria.COEBoolean.No;
            chemnamecriteria.Operator = SearchCriteria.COEOperators.EQUAL;
            chemnamecriteria.Value = "butanedioic acid, 2,3-dihydroxy- (R*,R*)-(+-)-";
            item = new SearchCriteria.SearchCriteriaItem();
            item.TableId = 203; //Moltable
            item.FieldId = 207; //Molname
            item.ID = 2;
            item.Criterium = chemnamecriteria;
            _searchCriteria.Items.Add(item);
            _resultsCriteria = GetResultsCriteria();
            _pagingInfo = GetFullPagingInfo();

            ds = _searchService.GetData(_resultsCriteria, _pagingInfo, _dataView, "NO", null, _searchCriteria);

            Assert.IsTrue(ds.Tables[0].Rows.Count == 1);
        }


        [Test]
        public void GetDataFromSavedThenGetDataFromTemp_Search()
        {
            GetDataFromSavedThenGetDataFromTemp(false, false);
        }

        [Test]
        public void GetDataFromSavedThenGetDataFromTemp_NoHitlist()
        {
            GetDataFromSavedThenGetDataFromTemp(true, false);
        }

        [Test]
        public void GetDataFromSavedThenGetDataFromTemp_NoHitlistNoPaging()
        {
            GetDataFromSavedThenGetDataFromTemp(true, true);
        }

        /// <summary>
        /// A bug has been reported that the search gets the cached version of the hitlist table to search. So if you first get data from a 
        /// saved hitlist then the regular searches fails due to the fact that the cached version do not have the temp hitlist table.
        /// </summary>
        public void GetDataFromSavedThenGetDataFromTemp(bool noHitlist, bool noPaging)
        {
            pathToXmls = SearchHelper.GetExecutingTestResultsBasePath(@"\Search Tests\COESearchTest XML");

            _dataView = GetDataView();
            _dataView.DataViewHandling = COEDataView.DataViewHandlingOptions.MERGE_CLIENT_AND_SERVER_DATAVIEW;
            _searchCriteria = GetSearchCriteria();
            _resultsCriteria = GetResultsCriteria();
            _pagingInfo.RecordCount = 25;
            _pagingInfo.Start = 1;


            COEHitListBO markedHL = COEHitListBO.GetMarkedHitList("COEDB", "CSSADMIN", _dataView.DataViewID);
            _pagingInfo.HitListID = markedHL.HitListID;
            _pagingInfo.HitListType = markedHL.HitListType;
            DataSet ds = _searchService.GetData(_resultsCriteria, _pagingInfo, _dataView); // forcing the fist search to be based on a saved HL

            _pagingInfo = GetFullPagingInfo();
            _pagingInfo.RecordCount = 25;
            _pagingInfo.Start = 1;
            ds = GetDataSet(noHitlist, noPaging);

            Assert.IsTrue(ds.Tables[0] != null, "Null table returned by COETestSearchOrderedById");
            Assert.AreEqual(_pagingInfo.RecordCount, ds.Tables[0].Rows.Count, "Incorrect number of rows returned by COETestSearchOrderedById");
            if (noPaging)
                Assert.AreEqual(293, _pagingInfo.RecordCount, "Incorrect number of rows returned by COETestSearchOrderedById");
            else
                Assert.AreEqual(25, _pagingInfo.RecordCount, "Incorrect number of rows returned by COETestSearchOrderedById");
        }

        #region search with dataviews that have tags
        [Test]
        public void Tags_COETEST_OrderedByMolWeight_ASC_Search()
        {
            Tags_COETEST_OrderedByMolWeight_ASC(false, false);
        }

        [Test]
        public void Tags_COETEST_OrderedByMolWeight_ASC_NoHitlist()
        {
            Tags_COETEST_OrderedByMolWeight_ASC(true, false);
        }

        [Test]
        public void Tags_COETEST_OrderedByMolWeight_ASC_NoHitlistNoPaging()
        {
            Tags_COETEST_OrderedByMolWeight_ASC(true, true);
        }

        private void Tags_COETEST_OrderedByMolWeight_ASC(bool noHitlist, bool noPaging)
        {
            pathToXmls = SearchHelper.GetExecutingTestResultsBasePath(@"\Search Tests\COESearchTest XML\TaggedDvSearch XML");
            _dataView = GetDataView();

            DataSet ds = GetDataSet(noHitlist, noPaging);
            Assert.IsTrue(ds != null);

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                Console.WriteLine(row["MolWeight"].ToString());
            }
        }


        [Test]
        public void Tags_SearchOverGrandChild_Search()
        {
            Tags_SearchOverGrandChild(false, false);
        }

        [Test]
        public void Tags_SearchOverGrandChild_NoHitlist()
        {
            Tags_SearchOverGrandChild(true, false);
        }

        [Test]
        public void Tags_SearchOverGrandChild_NoHitlistNoPaging()
        {
            Tags_SearchOverGrandChild(true, true);
        }

        private void Tags_SearchOverGrandChild(bool noHitlist, bool noPaging)
        {
            pathToXmls = SearchHelper.GetExecutingTestResultsBasePath(@"\Search Tests\COESearchTest XML");
            _searchCriteria = GetSearchCriteria();
            SearchCriteria.NumericalCriteria molidcriteria = new SearchCriteria.NumericalCriteria();
            molidcriteria.Operator = SearchCriteria.COEOperators.GTE;
            molidcriteria.Value = "1";
            SearchCriteria.SearchCriteriaItem item = new SearchCriteria.SearchCriteriaItem();
            item.TableId = 220;
            item.FieldId = 222;
            item.ID = 2;
            item.Criterium = molidcriteria;
            _searchCriteria.Items.Add(item);

            _resultsCriteria = GetResultsCriteria();

            ResultsCriteria.ResultsCriteriaTable propsTable = new ResultsCriteria.ResultsCriteriaTable(220);
            ResultsCriteria.Field clogP = new ResultsCriteria.Field(223);
            propsTable.Criterias.Add(clogP);
            _resultsCriteria.Tables.Add(propsTable);

            _pagingInfo.RecordCount = 25;
            _pagingInfo.Start = 1;
            _useRealTableNames = "NO";

            pathToXmls = SearchHelper.GetExecutingTestResultsBasePath(@"\Search Tests\COESearchTest XML\TaggedDvSearch XML");
            _dataView = GetDataViewWithOuterJoins();

            DataSet ds = GetDataSet(noHitlist, noPaging); ;

            Assert.IsTrue(ds.Tables[2] != null, "Null table returned by DoSearchTestOverGrandChild");
            Assert.IsTrue(ds.Tables[2].Rows.Count > 0, "No rows returned in grandchild table by DoSearchTestOverGrandChild");
            Assert.AreEqual(_pagingInfo.RecordCount, ds.Tables[0].Rows.Count, "Incorrect number of rows returned by DoSearchTestOverGrandChild");
            if (noPaging)
                Assert.AreEqual(226, _pagingInfo.RecordCount, "Incorrect number of rows returned by DoSearchTestOverGrandChild");
            else
                Assert.AreEqual(25, _pagingInfo.RecordCount, "Incorrect number of rows returned by DoSearchTestOverGrandChild");
            Assert.IsTrue(ds.Relations.Count == 2, "There is an incorrect number of relations in the resulting dataset");
            Assert.IsTrue(ds.Relations[0].ParentTable.TableName == "Table_203", "Wrong relation was created");
            Assert.IsTrue(ds.Relations[0].ChildTable.TableName == "Table_210", "Wrong relation was created");
            Assert.IsTrue(ds.Relations[1].ParentTable.TableName == "Table_210", "Wrong relation was created");
            Assert.IsTrue(ds.Relations[1].ChildTable.TableName == "Table_220", "Wrong relation was created");
        }
        #endregion
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

        private OrderByCriteria GetOrderByCriteria()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(pathToXmls + @"\OrderByCriteria.xml");
            OrderByCriteria obc = new OrderByCriteria(doc);
            return obc;
        }

        private COEDataView GetDataView(string fileName)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(pathToXmls + "\\" + fileName);
            COEDataView dataview = new COEDataView(doc);
            return dataview;
        }
        private COEDataView GetDataView()
        {
            return GetDataView("DataView.xml");
        }

        private COEDataView GetDataViewWithOuterJoins()
        {
            return GetDataView("DataViewWithOuterJoins.xml");
        }

        private ResultsCriteria GetResultsCriteria()
        {
            return this.GetResultsCriteria(@"ResultsCriteria.xml");
        }

        private ResultsCriteria GetResultsCriteria(string filename)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(pathToXmls + "\\" + filename);
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

            doc.Load(pathToXmls + fileName);
            SearchCriteria sc = new SearchCriteria(doc);
            return sc;
        }

        private bool HasNullValues(DataRowCollection dataRowCollection)
        {
            foreach (DataRow row in dataRowCollection)
            {
                if (row[0] is System.DBNull)
                    return true;
            }
            return false;
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
                ds = _searchService.GetData(_resultsCriteria, _pagingInfo, _dataView, _useRealTableNames, _orderByCriteria, _searchCriteria);
            }
            else
            {
                HitListInfo info = _searchService.GetHitList(_searchCriteria, _dataView);
                _pagingInfo.HitListID = info.HitListID;
                _pagingInfo.HitListType = info.HitListType;


                ds = _searchService.GetData(_resultsCriteria, _pagingInfo, _dataView, _useRealTableNames, _orderByCriteria);
            }

            return ds;
        }

        private static void ExecuteCommand(string command)
        {
            try
            {
                if (!string.IsNullOrEmpty(command))
                {
                    if (_dal == null)
                    {
                        LoadDAL();
                    }
                    DbCommand cmd = _dal.DALManager.Database.GetSqlStringCommand(command);
                    _dal.DALManager.Database.ExecuteNonQuery(cmd);
                }
            }
            catch
            {
                throw;
            }
        }
        private COEDataViewBO CreatDataView()
        {
            return SearchHelper.CreateDataView(SearchHelper._COESearchTestpathToXml + "\\" + "DataView.xml");
        }
        private HitListInfo GetHitListInfo()
        {
            return _searchService.GetHitList(_searchCriteria, _dataView);
        }
        private ConnStringType GetConnStringType(int dataViewID)
        {
            return ConnStringType.OWNERPROXY;
        }
        #endregion

        #region GetData

        /// <summary>
        /// Unit Test method of   GetData(ResultsCriteria resultsCriteria, PagingInfo pagingInfo, int dataViewID)
        /// </summary>
        [Test]
        public void GetDataUsingdvIDparamTest()
        {
            //COEDataViewBO theCOEDataViewBO = CreatDataView();
            DataSet ds = _searchService.GetData(_resultsCriteria, _pagingInfo, theCOEDataViewBO.COEDataView.DataViewID);
            Assert.IsNotNull(ds, "COESearch.GetData is not returning expected value");
        }
        /// <summary>
        /// Unit test method for GetData(ResultsCriteria resultsCriteria, PagingInfo pagingInfo, int dataViewID, string useRealTableNames)
        /// </summary>
        [Test]
        public void GetDataUsingdvIDTabNameparamTest()
        {
            // COEDataViewBO theCOEDataViewBO = CreatDataView();
            DataSet ds = _searchService.GetData(_resultsCriteria, _pagingInfo, theCOEDataViewBO.COEDataView.DataViewID, null);
            Assert.IsNotNull(ds, "COESearch.GetData is not returning expected value");
        }
        /// <summary>
        /// Unit Test method for  GetData(ResultsCriteria resultsCriteria, PagingInfo pagingInfo, int dataViewID, string useRealTableNames, OrderByCriteria orderByCriteria)
        /// </summary>
        [Test]
        public void GetDataUsingdvIDTabNameorderbyparamTest()
        {
            //COEDataViewBO theCOEDataViewBO = CreatDataView();
            DataSet ds = _searchService.GetData(_resultsCriteria, _pagingInfo, theCOEDataViewBO.COEDataView.DataViewID, null, _orderByCriteria);
            Assert.IsNotNull(ds, "COESearch.GetData is not returning expected value");
        }
        /// <summary>
        /// Unit Test for GetData(ResultsCriteria resultsCriteria, PagingInfo pagingInfo, COEDataView dataView)
        /// </summary>
        [Test]
        public void GetDataUsingdataviewparamTest()
        {
            //COEDataViewBO theCOEDataViewBO = CreatDataView();
            DataSet ds = _searchService.GetData(_resultsCriteria, _pagingInfo, theCOEDataViewBO.COEDataView);
            Assert.IsNotNull(ds, "COESearch.GetData is not returning expected value");
        }

        /// <summary>
        /// Unit Test for  GetData(ResultsCriteria resultsCriteria, PagingInfo pagingInfo, COEDataView dataView, string useRealTableNames)
        /// </summary>
        [Test]
        public void GetDataUsingdvTabNameparamTest()
        {
            //COEDataViewBO theCOEDataViewBO = CreatDataView();
            DataSet ds = _searchService.GetData(_resultsCriteria, _pagingInfo, theCOEDataViewBO.COEDataView, null);
            Assert.IsNotNull(ds, "COESearch.GetData is not returning expected value");
        }
        /// <summary>
        /// Unit Test for GetData(ResultsCriteria resultsCriteria, PagingInfo pagingInfo, COEDataView dataView, string useRealTableNames, OrderByCriteria orderByCriteria)
        /// </summary>
        [Test]
        public void GetDataUsingdvTabNameorderbyparamTest()
        {
            //COEDataViewBO theCOEDataViewBO = CreatDataView();
            DataSet ds = _searchService.GetData(_resultsCriteria, _pagingInfo, theCOEDataViewBO.COEDataView, null, _orderByCriteria);
            Assert.IsNotNull(ds, "COESearch.GetData is not returning expected value");
        }

        /// <summary>
        /// Unit Test For GetData(ResultsCriteria resultsCriteria, PagingInfo pagingInfo, COEDataView dataView, string useRealTableNames, OrderByCriteria orderByCriteria, SearchCriteria searchCriteria)
        /// </summary>
        [Test]
        public void GetDataUsingdvTabNameorderbSearchyparamTest()
        {
            // COEDataViewBO theCOEDataViewBO = CreatDataView();
            DataSet ds = _searchService.GetData(_resultsCriteria, _pagingInfo, theCOEDataViewBO.COEDataView, null, _orderByCriteria, _searchCriteria);
            Assert.IsNotNull(ds, "COESearch.GetData is not returning expected value");
        }
        /// <summary>
        /// Unit Test For GetData(ResultsCriteria resultsCriteria, PagingInfo pagingInfo, int dataViewID, string useRealTableNames, OrderByCriteria orderByCriteria, SearchCriteria searchCriteria)
        /// </summary>
        [Test]
        public void GetDataUsingdvIDTabNameorderbSearchyparamTest()
        {
            // COEDataViewBO theCOEDataViewBO = CreatDataView();
            DataSet ds = _searchService.GetData(_resultsCriteria, _pagingInfo, theCOEDataViewBO.COEDataView.DataViewID, null, _orderByCriteria, _searchCriteria);
            Assert.IsNotNull(ds, "COESearch.GetData is not returning expected value");
        }
        #endregion

        #region GetPartialHitList

        /// <summary>
        /// Unit Test for HitListInfo GetPartialHitList(SearchCriteria searchCriteria, int dataViewID)
        /// </summary>
        [Test]
        public void GetPartialHitListusingdvidparamTest()
        {
            HitListInfo theHitListInfo = _searchService.GetPartialHitList(_searchCriteria, theCOEDataViewBO.COEDataView.DataViewID);
            Assert.IsNotNull(theHitListInfo, "COESearch.GetPartialHitList does not retruning expected value");
        }
        /// <summary>
        /// Unit Test for public HitListInfo GetPartialHitList(SearchCriteria searchCriteria, int dataViewID, HitListInfo refineHitlist)
        /// </summary>
        [Test]
        public void GetPartialHitListusingdvidHitListparamTest()
        {
            HitListInfo theHitListInfo = _searchService.GetPartialHitList(_searchCriteria, theCOEDataViewBO.COEDataView.DataViewID, null);
            Assert.IsNotNull(theHitListInfo, "COESearch.GetPartialHitList does not retruning expected value");
        }
        /// <summary>
        /// Unit Test For public HitListInfo GetPartialHitList(SearchCriteria searchCriteria, int dataViewID, int commitSize)
        /// </summary>
        [Test]
        public void GetPartialHitListusingdvidcomsizparamTest()
        {
            HitListInfo theHitListInfo = _searchService.GetPartialHitList(_searchCriteria, theCOEDataViewBO.COEDataView.DataViewID, 1);
            Assert.IsNotNull(theHitListInfo, "COESearch.GetPartialHitList does not retruning expected value");
        }
        /// <summary>
        /// Unit Test For  GetPartialHitList(SearchCriteria searchCriteria, int dataViewID, int commitSize, HitListInfo refineHitlist)
        /// </summary>
        [Test]
        public void GetPartialHitListusingdvidcomsizeHitListparamTest()
        {
            HitListInfo theHitListInfo = _searchService.GetPartialHitList(_searchCriteria, theCOEDataViewBO.COEDataView.DataViewID, 1, null);
            Assert.IsNotNull(theHitListInfo, "COESearch.GetPartialHitList does not retruning expected value");
        }
        /// <summary>
        /// Unit Test for GetPartialHitList(SearchCriteria searchCriteria, COEDataView dataView)
        /// </summary>
        [Test]
        public void GetPartialHitListusingdvparamTest()
        {
            HitListInfo theHitListInfo = _searchService.GetPartialHitList(_searchCriteria, theCOEDataViewBO.COEDataView);
            Assert.IsNotNull(theHitListInfo, "COESearch.GetPartialHitList does not retruning expected value");
        }

        /// <summary>
        /// GetPartialHitList(SearchCriteria searchCriteria, COEDataView dataView, HitListInfo refineHitlist)
        /// </summary>
        [Test]
        public void GetPartialHitListusingdvHitListparamTest()
        {
            HitListInfo theHitListInfo = _searchService.GetPartialHitList(_searchCriteria, theCOEDataViewBO.COEDataView, null);
            Assert.IsNotNull(theHitListInfo, "COESearch.GetPartialHitList does not retruning expected value");
        }
        /// <summary>
        /// Unit Test For GetPartialHitList(SearchCriteria searchCriteria, COEDataView dataView, int commitSize)
        /// </summary>
        [Test]
        public void GetPartialHitListusingComitSizeparamTest()
        {
            HitListInfo theHitListInfo = _searchService.GetPartialHitList(_searchCriteria, theCOEDataViewBO.COEDataView, 0);
            Assert.IsNotNull(theHitListInfo, "COESearch.GetPartialHitList does not retruning expected value");
        }

        /// <summary>
        /// GetPartialHitList(SearchCriteria searchCriteria, COEDataView dataView, int commitSize, HitListInfo refineHitlist)
        /// </summary>
        [Test]
        public void GetPartialHitListusingComitSizHitListeparamTest()
        {
            HitListInfo theHitListInfo = _searchService.GetPartialHitList(_searchCriteria, theCOEDataViewBO.COEDataView, 0, null);
            Assert.IsNotNull(theHitListInfo, "COESearch.GetPartialHitList does not retruning expected value");
        }

        /// <summary>
        /// Unit Test For GetPartialHitList(SearchCriteria searchCriteria, COEDataView dataView, ConnStringType connStringType, HitListInfo refineHitlist) 
        /// </summary>
        [Test]
        public void GetPartialHitListusingComitSizHitListConparamTest()
        {
            // HitListInfo theHitListInfo = _searchService.GetPartialHitList(_searchCriteria, theCOEDataViewBO.COEDataView, GetConnStringType(theCOEDataViewBO.COEDataView), null);
            //Assert.IsNotNull(theHitListInfo, "COESearch.GetPartialHitList does not retruning expected value");
        }

        [Test]
        public void GetPartialHitListusingComitSizHitListConparamTest1()
        {
        }
        //private HitListInfo GetPartialHitList(SearchCriteria searchCriteria, COEDataView dataView, ConnStringType connStringType, int commitSize, HitListInfo refineHitlist)
        #endregion

        #region GetGlobalSearchData
        //public DataSet[] GetGlobalSearchData(ResultsCriteria resultsCriteria, PagingInfo pagingInfo, COEDataView[] dataView)
        /// <summary>
        /// Unit Test For GetGlobalSearchData(ResultsCriteria resultsCriteria, PagingInfo pagingInfo, COEDataView[] dataView)
        /// </summary>
        [Test]
        public void GetGlobalSearchDataTest()
        {
            DataSet[] ds = _searchService.GetGlobalSearchData(_resultsCriteria, _pagingInfo, new COEDataView[1] { theCOEDataViewBO.COEDataView });
            Assert.IsNotNull(ds, "COESearch.GetGlobalSearchData is not returning expected value");
            Assert.IsNotNull(ds[0], "COESearch.GetGlobalSearchData is not returning expected value");

        }
        #endregion
    }
}
