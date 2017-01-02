using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text;
using System.Collections.Generic;
using CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries.SelectItems;
using CambridgeSoft.COE.Framework.Common;
using System.Xml;
using CambridgeSoft.COE.Framework.UnitTests.Helpers;
using CambridgeSoft.COE.Framework.Common.SqlGenerator.MetaData;
using CambridgeSoft.COE.Framework.Common.SqlGenerator;
using CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries;
using CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries.AggregateItems;
using CambridgeSoft.COE.Framework.Types.Exceptions;

namespace CambridgeSoft.COE.Framework.UnitTests.Common.SqlGenerator.Queries.AggregateItems
{
    /// <summary>
    /// Summary description for GroupByClauseItemTest
    /// </summary>
    [TestClass]
    public class GroupByClauseItemTest
    {
        public GroupByClauseItemTest()
        {
            //
            // TODO: Add constructor logic here
            //
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

        [TestMethod]
        public void GetDependantStringTest()
        {

            try
            {
                GroupByClauseItem theClause = new GroupByClauseItem();
                theClause.Item = GetSelectClauseItem();
                theClause.GroupById = 1;
                string expected = "CSCARTRIDGE.Formula(\"MOLTABLE\".\"BASE64_CDX\", '')";
                string actual = theClause.GetDependantString(DBMSType.ORACLE);
                Assert.AreEqual(expected, actual, "GroupByClauseItem.GetDependantString did not return expected value");
            }
            catch
            {

                throw;
            }
        }
        #region IComparable Members

        [TestMethod]
        public void CompareToTest()
        {
            try
            {
                GroupByClauseItem theClause = new GroupByClauseItem(GetSelectClauseItem());
                theClause.GroupById = 1;
                int expected = 0;
                int actual = theClause.CompareTo(theClause);
                Assert.AreEqual(expected, actual, "GroupByClauseItem.CompareTo did not return expected value");
            }
            catch
            {
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(UnsupportedDataTypeException))]
        public void CompareTo_ExceptionTest()
        {

            GroupByClauseItem theClause = new GroupByClauseItem(GetSelectClauseItem());
            theClause.GroupById = 1;
            int expected = 0;
            int actual = theClause.CompareTo(new object());
            Assert.AreEqual(expected, actual, "GroupByClauseItem.CompareTo did not return expected value");

        }

        #endregion

        [TestMethod]
        public void CloneTest()
        {
            GroupByClauseItem theClause = new GroupByClauseItem(GetSelectClauseItem());
            GroupByClauseItem theCloneObj = (GroupByClauseItem)theClause.Clone();
            Assert.AreEqual(theClause.Item, theCloneObj.Item, "GroupByClauseItem.Clone did not return expected value");
        }
        private SelectClauseItem GetSelectClauseItem()
        {
            SelectClauseItem theSelectClauseItem = null;
            try
            {
                XmlNode resultNode = null;
                XmlDocument doc = new XmlDocument();
                string pathToXmls = SearchHelper.GetExecutingTestResultsBasePath(SearchHelper._COEExportToExcel);
                doc.Load(pathToXmls + @"\ResultsCriteria.xml");
                DataView theDataView = GetDataView();
                XmlNodeList personNodes = doc.GetElementsByTagName("Formula");
                foreach (XmlNode item in personNodes)
                {
                    resultNode = item;
                    break;
                }
                if (resultNode != null && theDataView != null)
                {
                    SelectClauseFormula theClause = new SelectClauseFormula();
                    theSelectClauseItem = theClause.CreateInstance(resultNode, theDataView);
                }
            }
            catch
            {
                throw;
            }
            return theSelectClauseItem;
        }



        private DataView GetDataView()
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                string pathToXmls = SearchHelper.GetExecutingTestResultsBasePath(SearchHelper._COEExportToExcel);
                doc.Load(pathToXmls + @"\DataView.xml");
                DataView dataView = new DataView();
                dataView.LoadFromXML(doc);
                return dataView;
            }
            catch (Exception)
            {
                throw;
            }

        }
    }
}
