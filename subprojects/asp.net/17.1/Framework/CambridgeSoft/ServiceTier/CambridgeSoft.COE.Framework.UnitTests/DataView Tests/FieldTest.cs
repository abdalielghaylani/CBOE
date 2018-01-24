using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CambridgeSoft.COE.Framework.UnitTests.Helpers;
using CambridgeSoft.COE.Framework.Common;
using System.Xml;

namespace CambridgeSoft.COE.Framework.COEDataViewService.UnitTests
{
    /// <summary>
    /// FieldTest class contains unit test methods for Field class
    /// </summary>
    [TestClass]
    public class FieldTest
    {
        #region Variables

        private TestContext testContextInstance;

        #endregion

        #region Properties

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

        #endregion

        #region Test Initialization

        /// <summary>
        /// Use TestInitialize to run code before running each test  
        /// </summary>
        [TestInitialize()]
        public void MyTestInitialize()
        {
            Authentication.Logon();
        }

        /// <summary>
        /// Use TestCleanup to run code after each test has run
        /// </summary>
        [TestCleanup()]
        public void MyTestCleanup()
        {
            Authentication.Logoff();
        }

        #endregion

        #region Field Test Methods

        /// <summary>
        /// Test method for verifying default Constructor
        /// </summary>
        [TestMethod]
        public void Field_VerifyingDefaultConstructor()
        {
            COEDataView.Field theField = new COEDataView.Field();
            Assert.IsNotNull(theField, "Field.Field() did not return the expected value.");
        }

        /// <summary>
        /// Test method for verifying IsDefaultQuery property is set to 'FALSE' in default constructor or not
        /// </summary>
        [TestMethod]
        public void Field_CheckingIsDefaultQuery()
        {
            COEDataView.Field theField = new COEDataView.Field();
            Assert.IsFalse(theField.IsDefaultQuery, "In Default Constructor IsDefaultqQuery proeprty must be set to 'False'.");
        }

        /// <summary>
        /// Test method for verifying IsIndexed property is set to 'FALSE' in default constructor or not
        /// </summary>
        [TestMethod]
        public void Field_CheckingIsIndexed()
        {
            COEDataView.Field theField = new COEDataView.Field();
            Assert.IsFalse(theField.IsIndexed, "In Default Constructor IsIndexed proeprty must be set to 'False'.");
        }

        /// <summary>
        /// Test method for verifying IsDefaultQuery property is set to 'TRUE' in Parameterized constructor.
        /// </summary>
        [TestMethod]
        public void ToString_CheckingToStringMethod()
        {
            COEDataView.Field theField = new COEDataView.Field();

            string strFieldInformation = theField.ToString();

            Assert.IsTrue(!string.IsNullOrEmpty(strFieldInformation), "COEDataView.Field.ToString() does not returns expected value.");
        }

        /// <summary>
        /// Test method for verifying IsDefaultQuery property is set to 'TRUE' in Parameterized constructor.
        /// </summary>
        [TestMethod]
        public void Field_VerifyingConstructorWithFieldInformationXMLNodeByIsDefaultQuery()
        {
            COEDataView.Field theField = new COEDataView.Field();
            theField.IsDefaultQuery = true;
            string strFieldInformation = theField.ToString();

            XmlDocument theXMLDocument = new XmlDocument();
            theXMLDocument.LoadXml(strFieldInformation);
            XmlNode theXmlNode = theXMLDocument.SelectSingleNode("./fields");

            COEDataView.Field theResult = new COEDataView.Field(theXmlNode);

            Assert.IsTrue(theResult.IsDefaultQuery, "COEDataView.Field(theXmlNode) does not returns expected value.");
        }

        /// <summary>
        /// Test method for verifying IsIndexed property is set to 'TRUE' in Parameterized constructor.
        /// </summary>
        [TestMethod]
        public void Field_VerifyingConstructorWithFieldInformationXMLNodeByIsIndexed()
        {
            COEDataView.Field theField = new COEDataView.Field();
            theField.IsIndexed = true;
            string strFieldInformation = theField.ToString();

            XmlDocument theXMLDocument = new XmlDocument();
            theXMLDocument.LoadXml(strFieldInformation);
            XmlNode theXmlNode = theXMLDocument.SelectSingleNode("./fields");

            COEDataView.Field theResult = new COEDataView.Field(theXmlNode);

            Assert.IsTrue(theResult.IsIndexed, "COEDataView.Field(theXmlNode) does not returns expected value.");
        }

        /// <summary>
        /// Test method for verifying IndexName property is set to 'EMPTY' in default constructor or not
        /// </summary>
        [TestMethod]
        public void Field_CheckingIndexName()
        {
            COEDataView.Field theField = new COEDataView.Field();
            Assert.IsTrue(theField.IndexName == String.Empty, "In Default Constructor IsDefaultqQuery proeprty must be set to 'False'.");
        }


        /// <summary>
        /// Test method for verifying indexname property is set to empty in Parameterized constructor.
        /// </summary>
        [TestMethod]
        public void Field_VerifyingConstructorWithFieldInformationXMLNodeByIndexName()
        {
            COEDataView.Field theField = new COEDataView.Field();
            String Expected = "TestIndexName_IX";
            theField.IndexName = Expected;
            string strFieldInformation = theField.ToString();

            XmlDocument theXMLDocument = new XmlDocument();
            theXMLDocument.LoadXml(strFieldInformation);
            XmlNode theXmlNode = theXMLDocument.SelectSingleNode("./fields");

            COEDataView.Field theResult = new COEDataView.Field(theXmlNode);

            Assert.IsTrue(theResult.IndexName == Expected, "COEDataView.Field(theXmlNode) does not returns expected value.");
        }
        #endregion
    }
}
