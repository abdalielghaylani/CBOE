﻿using NUnit.Framework;
using System;
using System.Text;
using System.Collections.Generic;
using CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries.SelectItems;
using CambridgeSoft.COE.Framework.Common;
using System.Xml;
using CambridgeSoft.COE.Framework.NUnitTests.Helpers;
using CambridgeSoft.COE.Framework.Common.SqlGenerator.MetaData;
using CambridgeSoft.COE.Framework.Common.SqlGenerator;
using CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries;
using CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries.AggregateItems;
using CambridgeSoft.COE.Framework.Types.Exceptions;

namespace CambridgeSoft.COE.Framework.NUnitTests.Common.SqlGenerator.Queries.AggregateItems
{
    /// <summary>
    /// Summary description for OrderByClauseItemTest
    /// </summary>
    [TestFixture]
    public class OrderByClauseItemTest
    {
        public OrderByClauseItemTest()
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

        [Test]
        public void GetDependantStringTest()
        {

            try
            {
                OrderByClauseItem theClause = new OrderByClauseItem();
                theClause.Item = GetSelectClauseItem();
                theClause.OrderByID = 1;
                string expected = "CSCARTRIDGE.Formula(\"MOLTABLE\".\"BASE64_CDX\", '') ASC";
                string actual = theClause.GetDependantString(DBMSType.ORACLE,new List<Value>());
                Assert.AreEqual(expected, actual, "OrderByClauseItem.GetDependantString did not return expected value");
            }
            catch
            {

                throw;
            }
        }
        #region IComparable Members

        [Test]
        public void CompareToTest()
        {
            try
            {
                OrderByClauseItem theClause = new OrderByClauseItem(GetSelectClauseItem());
                theClause.OrderByID = 1;
                int expected = 0;
                int actual = theClause.CompareTo(theClause);
                Assert.AreEqual(expected, actual, "OrderByClauseItem.CompareTo did not return expected value");
            }
            catch
            {
                throw;
            }
        }

        [Test]
        [ExpectedException(typeof(UnsupportedDataTypeException))]
        public void CompareTo_ExceptionTest()
        {

            OrderByClauseItem theClause = new OrderByClauseItem(GetSelectClauseItem());
            theClause.OrderByID = 1;
            int expected = 0;
            int actual = theClause.CompareTo(new object());
            Assert.AreEqual(expected, actual, "OrderByClauseItem.CompareTo did not return expected value");

        }

        #endregion

        [Test]
        public void CloneTest()
        {
            OrderByClauseItem theClause = new OrderByClauseItem(GetSelectClauseItem());
            OrderByClauseItem theCloneObj = (OrderByClauseItem)theClause.Clone();
            Assert.AreEqual(theClause.Item, theCloneObj.Item, "OrderByClauseItem.Clone did not return expected value");
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
