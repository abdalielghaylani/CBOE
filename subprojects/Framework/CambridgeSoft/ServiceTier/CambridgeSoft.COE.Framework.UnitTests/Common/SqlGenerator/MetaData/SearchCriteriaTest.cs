using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CambridgeSoft.COE.Framework.Common.SqlGenerator.MetaData;
using CambridgeSoft.COE.Framework.UnitTests.Helpers;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries;
using System.Xml;

namespace CambridgeSoft.COE.Framework.UnitTests.Common.SqlGenerator.MetaData
{
    /// <summary>
    /// Summary description for SearchCriteriaTest
    /// </summary>
    [TestClass]
    public class SearchCriteriaTest
    {
        COEDataView _dataview = null;
        DataView theDataView = null;
        CambridgeSoft.COE.Framework.Common.SqlGenerator.MetaData.SearchCriteria _SearchCriteria = null;
        XmlDocument _dataviewDoc = null;
        public SearchCriteriaTest()
        {
            _dataview = new COEDataView();
            theDataView = new DataView();
            _dataviewDoc = new XmlDocument();
            GetDataView();
            _SearchCriteria = new Framework.Common.SqlGenerator.MetaData.SearchCriteria();
            LoadSearchCriteriaFromXML();
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
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion
        /// <summary>
        /// Get dataview.
        /// </summary>
        /// <returns></returns>
        private void LoadSearchCriteriaFromXML()
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(SearchHelper.GetExecutingTestResultsBasePath(string.Empty) + @"\Search Tests\COESearchTest XML\OrderedSearchTestXml\SearchCriteria.xml");
                _SearchCriteria.LoadFromXML(doc.InnerXml);
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
        [TestMethod]
        public void IsStructureListCriteriaTableTest()
        {
            try
            {
                bool actual = _SearchCriteria.IsStructureListCriteriaTable(_dataview.Tables[0].Id);
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// Indicates if a criteria has to be aggregated while searching, by using the having clause.
        /// </summary>
        [TestMethod]
        public void ContainsAggregatedFunctionsTest()
        {
            try
            {
                bool actual = _SearchCriteria.ContainsAggregatedFunctions();
                Assert.AreEqual(false, actual, "SearchCriteria.ContainsAggregatedFunctions did not return the expected value");
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        ///  If there are lookup fields specified to be used to search, a list of pairs is returned indicating the relationships between the 
        /// source field and the lookup field id.
        /// </summary>
        [TestMethod]
        public void GetLookupFieldsTest()
        {
            try
            {
                Dictionary<int, int> actual = _SearchCriteria.GetLookupFields(theDataView);
                Assert.IsNotNull(actual, "SearchCriteria.GetLookupFields did not return the expected value");
            }
            catch
            {
                throw;
            }
        }
        [TestMethod]
        public void GetTableIdsTest()
        {
            try
            {
                List<int> actual = _SearchCriteria.GetTableIds();
                Assert.IsTrue(actual.Count > 0, "SearchCriteria.ContainsAggregatedFunctions did not return the expected value");
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// Gets all the table ids involved in the select clause Overload which takes dataView as parameter The parameter is superflous.  It is here for backwards compatibility
        /// </summary>
        [TestMethod]
        public void GetTableIds_DataViewTest()
        {
            try
            {
                List<int> actual = _SearchCriteria.GetTableIds(theDataView);
                Assert.IsTrue(actual.Count > 0, "SearchCriteria.ContainsAggregatedFunctions did not return the expected value");
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        ///  Gets a where clause from the underlying xml definition.
        /// </summary>
        [TestMethod]
        public void GetWhereClause_DataViewTest()
        {
            try
            {
                WhereClause actual = _SearchCriteria.GetWhereClause(theDataView);
                Assert.IsTrue(actual.Items.Count > 0, "SearchCriteria.GetWhereClause did not return the expected value");
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// Gets a where clause from the underlying xml definition.
        /// </summary>
        [TestMethod]
        public void GetWhereClause_XmlDocTest()
        {
            try
            {
                WhereClause actual = _SearchCriteria.GetWhereClause(_dataviewDoc);
                Assert.IsTrue(actual.Items.Count > 0, "SearchCriteria.GetWhereClause did not return the expected value");
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Gets a where clause from the underlying xml definition.
        /// </summary>
        [TestMethod]
        public void GetWhereClause_XmlStringTest()
        {
            try
            {
                WhereClause actual = _SearchCriteria.GetWhereClause(theDataView.ToString());
                Assert.IsTrue(actual.Items.Count > 0, "SearchCriteria.GetWhereClause did not return the expected value");
            }
            catch
            {
                throw;
            }
        }
    }

}
