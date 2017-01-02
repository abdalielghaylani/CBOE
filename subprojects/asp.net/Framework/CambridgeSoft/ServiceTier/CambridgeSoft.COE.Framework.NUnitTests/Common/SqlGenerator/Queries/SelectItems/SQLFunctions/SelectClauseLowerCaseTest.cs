using NUnit.Framework;
using System;
using System.Text;
using System.Collections.Generic;
using CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries.SelectItems;
using CambridgeSoft.COE.Framework.Common.SqlGenerator;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.COEConfigurationService;
using System.Xml;
using CambridgeSoft.COE.Framework.Common.SqlGenerator.MetaData;
using CambridgeSoft.COE.Framework.NUnitTests.Helpers;

namespace CambridgeSoft.COE.Framework.NUnitTests.Common.SqlGenerator.Queries.SelectItems.SQLFunctions
{
    /// <summary>
    /// Summary description for SelectClauseLowerCaseTest
    /// </summary>
    [TestFixture]
    public class SelectClauseLowerCaseTest
    {
        public SelectClauseLowerCaseTest()
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

       // [DeploymentItem("CambridgeSoft.COE.Framework.dll")]
        [Test]
        public void GetDependantStringTest()
        {
            SelectClauseLowerCase target = new SelectClauseLowerCase();
            Field field = new Field();
            field.FieldId = 20;
            field.FieldName = "BASE64_CDX";
            target.DataField = field;
            target.FunctionName = "LOWER";
            DBMSType dataBaseType = DBMSType.ORACLE;
            string expected = "LOWER(\"BASE64_CDX\")";
            string actual = target.Execute(dataBaseType, new List<Value>());
            Assert.AreEqual(expected, actual, "SelectItems.SelectClauseLowerCase.GetDependantString did not return the expected value.");
        }

        [Test]
        public void CreateInstanceTest()
        {
            XmlNode resultNode = null;
            XmlDocument doc = new XmlDocument();
            string body = "<?xml version=\"1.0\" encoding=\"utf-16\"?><SQLFunctions><SQLFunction visible=\"true\" alias=\"Mol Wt\" orderById=\"0\"      direction=\"asc\" functionName=\"LOWER\"/> </SQLFunctions>";
            doc.LoadXml(body);
            XmlDocument dv = new XmlDocument();
            string path = SearchHelper.GetExecutingTestResultsBasePath(@"\DataView Tests\COEDataViewTestXML");
            dv.Load(path + @"\COETESTGrandChildDataview.xml");
            DataView dataView = new DataView();
            dataView.LoadFromXML(dv);

            XmlNodeList personNodes = doc.GetElementsByTagName("SQLFunction");
            foreach (XmlNode item in personNodes)
            {
                resultNode = item;
                break;
            }
            if (resultNode != null && dataView != null)
            {
                SelectClauseSQLFunction theClause = new SelectClauseSQLFunction();
                SelectClauseItem theItem = theClause.CreateInstance(resultNode, dataView);
                Assert.AreEqual((theItem as SelectClauseSQLFunction).FunctionName, "LOWER", "SelectClauseLowerCase.CreateInstance did not return expected result");
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
