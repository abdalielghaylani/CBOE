using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using CambridgeSoft.COE.Framework.Common.SqlGenerator.MetaData;
using CambridgeSoft.COE.Framework.NUnitTests.Helpers;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries;
using System.Xml;


namespace CambridgeSoft.COE.Framework.NUnitTests.Common.SqlGenerator.MetaData
{
    /// <summary>
    /// Summary description for ResultsCriteriaTest
    /// </summary>
    [TestFixture]
    public class ResultsCriteriaTest
    {

        COEDataView _dataview = null;
        DataView theDataView = null;
        CambridgeSoft.COE.Framework.Common.SqlGenerator.MetaData.ResultsCriteria resultsCriteria = null;
        XmlDocument _dataviewDoc = null;
        public ResultsCriteriaTest()
        {
            _dataview = new COEDataView();
            theDataView = new DataView();
            _dataviewDoc = new XmlDocument();
            GetDataView();
            resultsCriteria = new Framework.Common.SqlGenerator.MetaData.ResultsCriteria();
            LoadResultCriteriaFromXML();

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
        // Use ClassInitialize to run code before running the first test in the class
        // [TestFixtureSetUp]
        // public static void MyClassInitialize() { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [TestFixtureTearDown]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [SetUp]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TearDown]
        // public void MyTestCleanup() { }
        //
        #endregion

        /// <summary>
        /// Get dataview.
        /// </summary>
        /// <returns></returns>
        private void LoadResultCriteriaFromXML()
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(SearchHelper.GetExecutingTestResultsBasePath(string.Empty) + @"\Search Tests\COESearchTest XML\OrderedSearchTestXml\ResultsCriteria.xml");
                resultsCriteria.LoadFromXML(doc);
            }
            catch
            {
                throw;
            }
        }
        private void GetDataView()
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(SearchHelper.GetExecutingTestResultsBasePath(string.Empty) + @"\Search Tests\COESearchTest XML\OrderedSearchTestXml\DataView.xml");
                _dataviewDoc = doc;
                _dataview = SearchHelper.BuildCOEDataViewFromXML("Search Tests\\COESearchTest XML\\OrderedSearchTestXml\\DataView.xml");
                theDataView.LoadFromXML(_dataview.ToString());
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Gets a list of Select Clause, each clause of the list is for a different query.
        /// </summary>
        [Test]
        public void GetSelectClause_XmlStringTest()
        {
            try
            {
                SelectClause[] actual = resultsCriteria.GetSelectClause(_dataview.ToString());
                Assert.IsTrue(actual.Count() > 0, "ResultsCriteria.GetSelectClause did not return the expected value");
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        ///  Gets a list of Select Clause, each clause of the list is for a different query.
        /// </summary>
        [Test]
        public void GetSelectClause_DataViewTest()
        {
            try
            {
                SelectClause[] actual = resultsCriteria.GetSelectClause(theDataView);
                Assert.IsTrue(actual.Count() > 0, "ResultsCriteria.GetSelectClause did not return the expected value");
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Gets a list of Select Clause, each clause of the list is for a different query.
        /// </summary>
        [Test]
        public void GetSelectClause_XmlDocTest()
        {
            try
            {
                SelectClause[] actual = resultsCriteria.GetSelectClause(_dataviewDoc);
                Assert.IsTrue(actual.Count() > 0, "ResultsCriteria.GetSelectClause did not return the expected value");
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// Gets all the table ids involved in the select clause
        /// </summary>
        [Test]
        public void GetTableIdsTest()
        {
            List<int>[] actual = resultsCriteria.GetTableIds(theDataView);
            Assert.IsTrue(actual.Count() > 0, "ResultsCriteria.GetSelectClause did not return the expected value");
        }
        /// <summary>
        /// Gets the id of the main table of a query.
        /// </summary>
        [Test]
        public void GetMainTableIdTest()
        {
            try
            {
                int actual = resultsCriteria.GetMainTableId();
                Assert.IsTrue(actual > 0, "ResultsCriteria.GetMainTableId did not return the expected value");
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// Unit Test For GetSelectClause().
        /// </summary>
        [Test]
        public void GetGroupByClauseTest()
        {
            try
            {
                SelectClause[] theClause = resultsCriteria.GetSelectClause(theDataView);
                GroupByClause[] actual = resultsCriteria.GetGroupByClause(theClause, true);
                Assert.IsNotNull(actual, "ResultsCriteria.GetGroupByClause did not return the expected value");
            }
            catch
            {
                throw;
            }
        }
    }
}
