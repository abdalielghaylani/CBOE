﻿// The following code was generated by Microsoft Visual Studio 2005.
// The test owner should check each test for validity.
using NUnit.Framework;
using System;
using System.Text;
using System.Collections.Generic;
using CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries.SelectItems;
using CambridgeSoft.COE.Framework.Common.SqlGenerator;
using CambridgeSoft.COE.Framework.Common;
using System.Xml;
using CambridgeSoft.COE.Framework.Common.SqlGenerator.MetaData;
using CambridgeSoft.COE.Framework.NUnitTests.Helpers;
namespace CambridgeSoft.COE.Framework.Common.SqlGenerator.UnitTests
{
    /// <summary>
    ///This is a test class for CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries.SelectItems.SelectClauseCase and is intended
    ///to contain all CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries.SelectItems.SelectClauseCase Unit Tests
    ///</summary>
    [TestFixture]
    public class SelectClauseCaseTest
    {


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
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //
        //[TestFixtureSetUp]
        //public static void MyClassInitialize()
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //
        //[TestFixtureTearDown]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //
        //[SetUp]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //
        //[TearDown]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for GetDependantString (DBMSType:ORACLE)
        ///</summary>
       // [DeploymentItem("CambridgeSoft.COE.Framework.dll")]
        [Test]
        public void GetDependantStringORACLETest()
        {
            try
            {
                Table emp = new Table();
                emp.TableName = "emp";
                emp.Alias = "e";

                Field nameField = new Field();
                nameField.Table = emp;
                nameField.FieldName = "NAME";

                Field lastNameField = new Field();
                lastNameField.Table = emp;
                lastNameField.FieldName = "LASTNAME";

                SelectClauseField clause = new SelectClauseField();
                clause.DataField = nameField;

                SelectClauseLiteral caseLiteral = new SelectClauseLiteral();
                caseLiteral.Literal = "'Literal Name'";

                SelectClauseField caseField = new SelectClauseField();
                caseField.DataField = lastNameField;

                SelectClauseCase target = new SelectClauseCase();
                target.Clause = clause;
                target.Cases.Add("'JOHN'", caseLiteral);
                target.Cases.Add("'SMITH'", caseField);
                target.Default = caseField;

                string expected = "DECODE (\"e\".\"NAME\", 'JOHN', 'Literal Name', 'SMITH', \"e\".\"LASTNAME\", \"e\".\"LASTNAME\")";
                string actual = target.Execute(DBMSType.ORACLE, new List<Value>());

                Assert.AreEqual(expected, actual, "CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries.SelectItems.SelectClauseC" +
                        "ase.GetDependantString did not return the expected value.");
            }
            catch (Exception exception)
            {
                Assert.Fail(exception.Message);
            }
        }

        /// <summary>
        ///A test for GetDependantString (DBMSType :MSACCESS)
        ///</summary>
       // [DeploymentItem("CambridgeSoft.COE.Framework.dll")]
        [Test]
        [ExpectedException(typeof(Exception))]
        public void GetDependantStringMSACCESSTest()
        {

            Table emp = new Table();
            emp.TableName = "emp";
            emp.Alias = "e";

            Field nameField = new Field();
            nameField.Table = emp;
            nameField.FieldName = "NAME";

            Field lastNameField = new Field();
            lastNameField.Table = emp;
            lastNameField.FieldName = "LASTNAME";

            SelectClauseField clause = new SelectClauseField();
            clause.DataField = nameField;

            SelectClauseLiteral caseLiteral = new SelectClauseLiteral();
            caseLiteral.Literal = "'Literal Name'";

            SelectClauseField caseField = new SelectClauseField();
            caseField.DataField = lastNameField;

            SelectClauseCase target = new SelectClauseCase();
            target.Clause = clause;
            target.Cases.Add("'JOHN'", caseLiteral);
            target.Cases.Add("'SMITH'", caseField);
            target.Default = caseField;
            string actual = target.Execute(DBMSType.MSACCESS, new List<Value>());

        }

        /// <summary>
        ///A test for GetDependantString (DBMSType :SQLSERVER)
        ///</summary>
       // [DeploymentItem("CambridgeSoft.COE.Framework.dll")]
        [Test]
        public void GetDependantStringSQLSERVERTest()
        {
            try
            {
                Table emp = new Table();
                emp.TableName = "emp";
                emp.Alias = "e";

                Field nameField = new Field();
                nameField.Table = emp;
                nameField.FieldName = "NAME";

                Field lastNameField = new Field();
                lastNameField.Table = emp;
                lastNameField.FieldName = "LASTNAME";

                SelectClauseField clause = new SelectClauseField();
                clause.DataField = nameField;

                SelectClauseLiteral caseLiteral = new SelectClauseLiteral();
                caseLiteral.Literal = "'Literal Name'";

                SelectClauseField caseField = new SelectClauseField();
                caseField.DataField = lastNameField;

                SelectClauseCase target = new SelectClauseCase();
                target.Clause = clause;
                target.Cases.Add("'JOHN'", caseLiteral);
                target.Cases.Add("'SMITH'", caseField);
                target.Default = caseField;

                string expected = "case \"e\".\"NAME\" WHEN 'JOHN' THEN 'Literal Name' WHEN 'SMITH' THEN \"e\".\"LASTNAME\" ELSE \"e\".\"LASTNAME\" END";
                string actual = target.Execute(DBMSType.SQLSERVER, new List<Value>());

                Assert.AreEqual(expected, actual, "CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries.SelectItems.SelectClauseC" +
                        "ase.GetDependantString did not return the expected value.");
            }
            catch (Exception exception)
            {
                Assert.Fail(exception.Message);
            }
        }

        /// <summary>
        /// Unit Test for SelectClauseItem CreateInstance(System.Xml.XmlNode resultsXmlNode, INamesLookup dvnLookup)
        /// </summary>

        [Test]
        public void CreateInstanceTest()
        {
            XmlNode resultNode = null;
            XmlDocument doc = new XmlDocument();
            string pathToXmls = SearchHelper.GetExecutingTestResultsBasePath(@"Common\SqlGenerator\Queries\SelectItems");
            doc.Load(pathToXmls + @"\ResultsCriteria.xml");
            DataView theDataView = GetDataView("OrderByTest XML");

            string body = "<?xml version=\"1.0\" encoding=\"utf-16\"?><switch xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" visible=\"true\" alias=\"Greatests switch-case\" orderById=\"0\" direction=\"asc\" inputType=\"text\">  <field visible=\"true\" orderById=\"0\" direction=\"asc\" fieldId=\"3\" />  <conditions>    <condition visible=\"true\" orderById=\"0\" direction=\"asc\" value=\"José\" default=\"false\">      <field visible=\"true\" orderById=\"0\" direction=\"asc\" fieldId=\"1\" />    </condition>    <condition visible=\"true\" orderById=\"0\" direction=\"asc\" value=\"\" default=\"true\">      <concatenation visible=\"true\" orderById=\"0\" direction=\"asc\">        <field visible=\"true\" orderById=\"0\" direction=\"asc\" fieldId=\"2\" />        <literal visible=\"true\" orderById=\"0\" direction=\"asc\"> Honguito </literal>      </concatenation>    </condition>  </conditions></switch>";
            XmlDocument bodyDoc = new XmlDocument();
            bodyDoc.LoadXml(body);

            XmlNodeList personNodes = bodyDoc.GetElementsByTagName("switch");
            foreach (XmlNode item in personNodes)
            {
                resultNode = item;
                break;
            }
            if (resultNode != null && theDataView != null)
            {
                SelectClauseCase theClause = new SelectClauseCase();
                SelectClauseItem theItem = theClause.CreateInstance(resultNode, theDataView);
                Assert.AreEqual(theItem.Name, "Greatests switch-case", "SelectClauseCase.CreateInstance did not return expected result");
            }

        }

        private DataView GetDataView()
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                string pathToXmls = SearchHelper.GetExecutingTestResultsBasePath(SearchHelper._LookupSearchTestpathToXml);
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

        private DataView GetDataView(string FolderName)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                string pathToXmls = SearchHelper.GetExecutingTestResultsBasePath(FolderName);
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
