using NUnit.Framework;
using System;
using System.Text;
using System.Collections.Generic;
using CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries.SelectItems;
using CambridgeSoft.COE.Framework.Common;
using System.Xml;
using CambridgeSoft.COE.Framework.NUnitTests.Helpers;
using CambridgeSoft.COE.Framework.Common.SqlGenerator.MetaData;
using CambridgeSoft.COE.Framework.Common.SqlGenerator;
namespace CambridgeSoft.COE.Framework.NUnitTests.Common.SqlGenerator.Queries.AggregateItems
{
    /// <summary>
    /// Summary description for SelectClauseMinTest
    /// </summary>
    [TestFixture]
    public class SelectClauseMinTest
    {
        public SelectClauseMinTest()
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

        /// <summary>
        ///A test for GetDependantString (DBMSType : MSACCESS)
        ///</summary>
       // [DeploymentItem("CambridgeSoft.COE.Framework.dll")]
        [Test]
        public void GetDependantStringMSACCESSTest()
        {
            SelectClauseMin target = new SelectClauseMin();

            Field fld = new Field();
            fld.FieldId = 18;
            fld.FieldName = "COMPOUND_ID";
            fld.FieldType = System.Data.DbType.Binary;
            fld.Table = new Table();
            ((Table)fld.Table).TableName = "inv_compounds";
            target.DataField = fld;
            target.Alias = "New";
            DBMSType dataBaseType = DBMSType.ORACLE;

            string expected = "MIN(\"COMPOUND_ID\")";
            string actual = target.Execute(dataBaseType, new List<Value>());

            Assert.AreEqual(expected, actual, "SelectItems.SelectClauseMin.GetDependantString did not return the expected value.");
        }

        /// <summary>
        ///A test for GetDependantString (DBMSType : ORACLE)
        ///</summary>
       // [DeploymentItem("CambridgeSoft.COE.Framework.dll")]
        [Test]
        public void GetDependantStringORACLETest()
        {
            SelectClauseMin target = new SelectClauseMin();

            Field fld = new Field();
            fld.FieldId = 18;
            fld.FieldName = "COMPOUND_ID";
            fld.FieldType = System.Data.DbType.Decimal;
            fld.Table = new Table();
            ((Table)fld.Table).TableName = "inv_compounds";
            target.DataField = fld;
            target.Alias = "New";
            DBMSType dataBaseType = DBMSType.MSACCESS;

            string expected = "ROUND(MIN(\"COMPOUND_ID\"), 10)";
            string actual = target.Execute(dataBaseType, new List<Value>());

            Assert.AreEqual(expected, actual, "SelectItems.SelectClauseMin.GetDependantString did not return the expected value.");
        }

        /// <summary>
        ///A test for GetDependantString (DBMSType : SQLSERVER)
        ///</summary>
       // [DeploymentItem("CambridgeSoft.COE.Framework.dll")]
        [Test]
        public void GetDependantStringSQLSERVERTest()
        {
            SelectClauseMin target = new SelectClauseMin();

            Field fld = new Field();
            fld.FieldId = 18;
            fld.FieldName = "COMPOUND_ID";
            fld.FieldType = System.Data.DbType.Decimal;
            fld.Table = new Table();
            ((Table)fld.Table).TableName = "inv_compounds";
            target.DataField = fld;
            target.Alias = "New";
            DBMSType dataBaseType = DBMSType.SQLSERVER;

            string expected = "ROUND(MIN(\"COMPOUND_ID\"), 10)";
            string actual = target.Execute(dataBaseType, new List<Value>());

            Assert.AreEqual(expected, actual, "SelectItems.SelectClauseMin.GetDependantString did not return the expected value.");
        }


        [Test]
        public void CreateInstanceTest()
        {
            XmlNode resultNode = null;
            XmlDocument doc = new XmlDocument();
            string pathToXmls = SearchHelper.GetExecutingTestResultsBasePath(SearchHelper._COEExportToExcel);
            doc.Load(pathToXmls + @"\ResultsCriteria.xml");
            DataView theDataView = GetDataView();
            XmlNodeList personNodes = doc.GetElementsByTagName("field");
            foreach (XmlNode item in personNodes)
            {
                resultNode = item;
                break;
            }
            if (resultNode != null && theDataView != null)
            {
                SelectClauseMin theClause = new SelectClauseMin();
                SelectClauseItem theItem = theClause.CreateInstance(resultNode, theDataView);
                Assert.IsNotNull(theClause.DataField, "SelectClauseMin.CreateInstance did not return expected result");
               

            }

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
