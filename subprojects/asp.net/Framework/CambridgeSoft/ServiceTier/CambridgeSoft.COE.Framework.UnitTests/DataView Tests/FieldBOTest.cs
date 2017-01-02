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
    /// FieldBOTest class contains unit test methods for FieldBO class
    /// </summary>
    [TestClass]
    public class FieldBOTest
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
        /// Test method for Creating new FIELDBO object with Database
        /// </summary>
        [TestMethod]
        public void NewField_CreatingNewFieldBOObjectWithDatabase()
        {
            COEDataView.Field theField = new COEDataView.Field();
            FieldBO theFieldBO = FieldBO.NewField(theField, "COEDB");
            Assert.IsNotNull(theFieldBO, "FieldBO.NewField(theField, 'COEDB') did not return the expected value.");
        }

        /// <summary>
        /// Test method for Creating new and clean FIELDBO object with Database
        /// </summary>
        [TestMethod]
        public void NewField_CreatingNewAndCleanFieldBOObjectWithDatabase()
        {
            COEDataView.Field theField = new COEDataView.Field();
            FieldBO theFieldBO = FieldBO.NewField(theField, "COEDB", true);
            Assert.IsNotNull(theFieldBO, "FieldBO.NewField(theField, 'COEDB', true) did not return the expected value.");
        }

        /// <summary>
        /// Test method for Creating new and dirty FIELDBO object with Database
        /// </summary>
        [TestMethod]
        public void NewField_CreatingNewAndDirtyFieldBOObjectWithDatabase()
        {
            COEDataView.Field theField = new COEDataView.Field();
            FieldBO theFieldBO = FieldBO.NewField(theField, "COEDB", false);
            Assert.IsNotNull(theFieldBO, "FieldBO.NewField(theField, 'COEDB', false) did not return the expected value.");
        }

        /// <summary>
        /// Test method for Generating FIELDBO xml using ToString() method
        /// </summary>
        [TestMethod]
        public void ToString_GeneratingFIELDBOXML()
        {
            COEDataView.Field theField = new COEDataView.Field();
            FieldBO theFieldBO = FieldBO.NewField(theField, "COEDB", false);
            string strFieldsXML =  theFieldBO.ToString();
            Assert.IsTrue(!string.IsNullOrEmpty(strFieldsXML), "FieldBO.ToString() did not return the expected value.");
        }

        /// <summary>
        /// Test method for Verifying ToString() method whether it maintains the variables value or not by IsDefaultQuery property.
        /// </summary>
        [TestMethod]
        public void ToString_VerifyingDataPersistedOrNotWithToStringMethodByIsDefaultQuery()
        {
            COEDataView.Field theField = new COEDataView.Field();
            FieldBO theFieldBO = FieldBO.NewField(theField, "COEDB", false);
            theFieldBO.IsDefaultQuery = true;
            string strFieldsXML = theFieldBO.ToString();

            XmlDocument theXMLDocument = new XmlDocument();
            theXMLDocument.LoadXml(strFieldsXML);
            XmlNode theXmlNode = theXMLDocument.SelectSingleNode("./fields");

            COEDataView.Field theResult = new COEDataView.Field(theXmlNode);

            Assert.IsTrue(theResult.IsDefaultQuery, "FieldBO.ToString() does not maintain the IsDefaultQuery value which was set to 'TRUE'.");
        }

        /// <summary>
        /// Test method for Verifying ToString() method whether it maintains the variables value or not by IsIndexed property.
        /// </summary>
        [TestMethod]
        public void ToString_VerifyingDataPersistedOrNotWithToStringMethodByIsIndexed()
        {
            COEDataView.Field theField = new COEDataView.Field();
            FieldBO theFieldBO = FieldBO.NewField(theField, "COEDB", false);
            theFieldBO.IsIndexed = true;
            string strFieldsXML = theFieldBO.ToString();

            XmlDocument theXMLDocument = new XmlDocument();
            theXMLDocument.LoadXml(strFieldsXML);
            XmlNode theXmlNode = theXMLDocument.SelectSingleNode("./fields");

            COEDataView.Field theResult = new COEDataView.Field(theXmlNode);

            Assert.IsTrue(theResult.IsIndexed, "FieldBO.ToString() does not maintain the IsIndexed value which was set to 'TRUE'.");
        }

        /// <summary>
        /// Test method for Verifying ToString() method whether it maintains the variables value or not by IndexName property.
        /// </summary>
        [TestMethod]
        public void ToString_VerifyingDataPersistedOrNotWithToStringMethodByIndexName()
        {
            COEDataView.Field theField = new COEDataView.Field();
            FieldBO theFieldBO = FieldBO.NewField(theField, "COEDB", false);
            String Expected = "TestIndexName_IX";
            theFieldBO.IndexName = Expected;
            string strFieldsXML = theFieldBO.ToString();

            XmlDocument theXMLDocument = new XmlDocument();
            theXMLDocument.LoadXml(strFieldsXML);
            XmlNode theXmlNode = theXMLDocument.SelectSingleNode("./fields");

            COEDataView.Field theResult = new COEDataView.Field(theXmlNode);

            Assert.IsTrue(theResult.IndexName == Expected, "FieldBO.ToString() does not maintain the IndexName value.");
        }

        #endregion
    }
}
