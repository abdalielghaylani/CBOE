using CambridgeSoft.COE.RegistrationAdmin.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Data;
using System.Collections.Generic;
using System.IO;
using RegistrationAdmin.Services.MSUnitTests.Helpers;
using System.Xml;
using CambridgeSoft.COE.RegistrationAdmin.Access;
using System.Data.Common;
using CambridgeSoft.COE.Framework.Common.Messaging;
using Microsoft.Practices.EnterpriseLibrary.Data;
using CambridgeSoft.COE.Framework.Common;
using System.Configuration;
using System.Text;
using CambridgeSoft.COE.Registration.Services.Types;
using CambridgeSoft.COE.Framework.COEDataViewService;

namespace RegistrationAdmin.Services.MSUnitTests
{
    /// <summary>
    ///This is a test class for ConfigurationRegistryRecordTest and is intended
    ///to contain all ConfigurationRegistryRecordTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ConfigurationRegistryRecordTest
    {
        XmlDocument configurationRegistryRecordXmlDoc;
        XmlDocument registrationAppSettingsXmlDoc;
        XmlDocument customTableListXmlDoc;
        XmlDocument coeFormGroupXmlDoc;

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
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        [TestInitialize()]
        public void MyTestInitialize()
        {
            configurationRegistryRecordXmlDoc = XMLDataLoader.LoadXmlDocument("ConfigurationRegistryRecord.xml");
            if (configurationRegistryRecordXmlDoc == null)
            {
                Assert.Fail("ConfigurationRegistryRecord.xml document can not be loaded");
            }

            registrationAppSettingsXmlDoc = XMLDataLoader.LoadXmlDocument("RegAppSettings.xml");
            if (registrationAppSettingsXmlDoc == null)
            {
                Assert.Fail("RegAppSettings.xml document can not be loaded");
            }

            customTableListXmlDoc = XMLDataLoader.LoadXmlDocument("CustomTableList.xml");
            if (customTableListXmlDoc == null)
            {
                Assert.Fail("CustomTableList.xml document can not be loaded");
            }

            coeFormGroupXmlDoc = XMLDataLoader.LoadXmlDocument("FormGroup.xml");
            if (coeFormGroupXmlDoc == null)
            {
                Assert.Fail("FormGroup.xml document can not be loaded");
            }
        }

        //Use TestCleanup to run code after each test has run
        [TestCleanup()]
        public void MyTestCleanup()
        {
        }

        #endregion

        ///// <summary>
        /////A test for ImportIniFile
        /////</summary>
        //[TestMethod()]
        //public void ImportIniFileTest()
        //{
        //    ConfigurationRegistryRecord_Accessor target = new ConfigurationRegistryRecord_Accessor(); // TODO: Initialize to an appropriate value
        //    Stream streamCfserver = null; // TODO: Initialize to an appropriate value
        //    Stream streamReg = null; // TODO: Initialize to an appropriate value
        //    target.ImportIniFile(streamCfserver, streamReg);
        //    Assert.Inconclusive("A method that does not return a value cannot be verified.");
        //}

        #region Completed
        /// <summary>
        ///A test for NewConfigurationRegistryRecord with default constructor
        ///</summary>
        [TestMethod()]
        public void NewConfigurationRegistryRecord_DefaultConstructorTest()
        {
            ConfigurationRegistryRecord target;
            //create RegAdminOracleDAL object to avoid exceptions while running default constructor
            RegAdminOracleDAL regDal = null;
            Helpers.Helpers.GetRegDal(ref regDal, "");
            target = ConfigurationRegistryRecord.NewConfigurationRegistryRecord();
            Assert.IsNotNull(target, string.Format("{0}.NewConfigurationRegistryRecord() failed to create object", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));
        }

        /// <summary>
        ///A test for NewConfigurationRegistryRecord with configuration registry record xml
        ///</summary>
        [TestMethod()]
        public void NewConfigurationRegistryRecord_LoadFromConfigurationXMLTest()
        {
            string xml = string.Empty; // TODO: Initialize to an appropriate value
            XmlNodeList xmlNodeList = XMLDataLoader.PrepareXMLNodeList(configurationRegistryRecordXmlDoc); // TODO: Initialize to an appropriate value
            Assert.IsNotNull(xmlNodeList, "xml node list null");

            XmlNode configurationRegistryRecordXML = xmlNodeList[0];
            xml = configurationRegistryRecordXML.OuterXml;

            ConfigurationRegistryRecord target = ConfigurationRegistryRecord.NewConfigurationRegistryRecord(xml); // TODO: Initialize to an appropriate value
            Assert.IsNotNull(target, string.Format("{0}.NewConfigurationRegistryRecord(xml) failed to create object", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));
        }

        /// <summary>
        ///A test for GetConfigurationSettingsXml
        ///</summary>
        [TestMethod()]
        public void GetConfigurationSettingsXmlTest()
        {
            XmlNodeList xmlNodeList = XMLDataLoader.PrepareXMLNodeList(configurationRegistryRecordXmlDoc); // TODO: Initialize to an appropriate value
            Assert.IsNotNull(xmlNodeList, "xml node list null");

            XmlNode configurationRegistryRecordXML = xmlNodeList[0];

            ConfigurationRegistryRecord target = ConfigurationRegistryRecord.NewConfigurationRegistryRecord(configurationRegistryRecordXML.OuterXml); // TODO: Initialize to an appropriate value
            Assert.IsNotNull(target, string.Format("{0}.NewConfigurationRegistryRecord(configurationRegistryRecordXML.ToString()) failed to create object", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

            string actual;
            actual = target.GetConfigurationSettingsXml();
            Assert.IsTrue(!string.IsNullOrEmpty(actual), string.Format("{0}.GetConfigurationSettingsXml() failed to get configuration settings xml string", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));
        }

        /// <summary>
        ///A test for GetPickListValues
        ///</summary>
        [TestMethod()]
        public void GetPickListValuesTest()
        {
            //create RegAdminOracleDAL object to avoid exceptions while running default constructor
            RegAdminOracleDAL regDal = null;
            Helpers.Helpers.GetRegDal(ref regDal, "");

            string xml = string.Empty; // TODO: Initialize to an appropriate value
            XmlNodeList xmlNodeList = XMLDataLoader.PrepareXMLNodeList(configurationRegistryRecordXmlDoc); // TODO: Initialize to an appropriate value
            Assert.IsNotNull(xmlNodeList, "xml node list null");

            XmlNode configurationRegistryRecordXML = xmlNodeList[0];
            xml = configurationRegistryRecordXML.OuterXml;

            ConfigurationRegistryRecord target = ConfigurationRegistryRecord.NewConfigurationRegistryRecord(xml); // TODO: Initialize to an appropriate value
            Assert.IsNotNull(target, string.Format("{0}.NewConfigurationRegistryRecord(xml) failed to create object", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

            string tableName = "VW_PICKLISTDOMAIN";
            string tableDetails = target.GetTable(tableName);

            XmlDocument tableXml = new XmlDocument();
            tableXml.InnerXml = tableDetails;

            XmlNode prefixXmlNode = tableXml.SelectSingleNode("/VW_PICKLISTDOMAIN/VW_PICKLISTDOMAIN/DESCRIPTION[text()='Prefixes']");
            if (prefixXmlNode != null)
            {
                XmlNode idNode = prefixXmlNode.ParentNode.SelectSingleNode("ID");
                string pickListDomainId = idNode.InnerText;

                DataSet ds = target.GetPickListValues(pickListDomainId);
                Assert.IsNotNull(ds, string.Format("{0}.GetPickListValues(5) failed to create object", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));
                string expected = "AB";
                string actual = ds.Tables[0].Rows[0][1].ToString();
                Assert.AreEqual<string>(expected, actual, string.Format("{0}.GetPickListValues(5) did not returned expected value", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));
            }
        }

        /// <summary>
        ///A test for GetTable
        ///</summary>
        [TestMethod()]
        public void GetTableTest()
        {
            //create RegAdminOracleDAL object to avoid exceptions while running default constructor
            RegAdminOracleDAL regDal = null;
            Helpers.Helpers.GetRegDal(ref regDal, "");

            string xml = string.Empty; // TODO: Initialize to an appropriate value
            XmlNodeList xmlNodeList = XMLDataLoader.PrepareXMLNodeList(configurationRegistryRecordXmlDoc); // TODO: Initialize to an appropriate value
            Assert.IsNotNull(xmlNodeList, "xml node list null");

            XmlNode configurationRegistryRecordXML = xmlNodeList[0];
            xml = configurationRegistryRecordXML.OuterXml;

            ConfigurationRegistryRecord target = ConfigurationRegistryRecord.NewConfigurationRegistryRecord(xml); // TODO: Initialize to an appropriate value
            Assert.IsNotNull(target, string.Format("{0}.NewConfigurationRegistryRecord(xml) failed to create object", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

            string tableName = "VW_PICKLISTDOMAIN";
            string tableDetails = target.GetTable(tableName);
            Assert.IsNotNull(tableDetails, string.Format("{0}.GetTable(tableName) did not returned the expected result", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));
        }

        /// <summary>
        ///A test for GetTableList
        ///</summary>
        [TestMethod()]
        public void GetTableListTest()
        {
            //create RegAdminOracleDAL object to avoid exceptions while running default constructor
            RegAdminOracleDAL regDal = null;
            Helpers.Helpers.GetRegDal(ref regDal, "");

            string xml = string.Empty; // TODO: Initialize to an appropriate value
            XmlNodeList xmlNodeList = XMLDataLoader.PrepareXMLNodeList(configurationRegistryRecordXmlDoc); // TODO: Initialize to an appropriate value
            Assert.IsNotNull(xmlNodeList, "xml node list null");

            XmlNode configurationRegistryRecordXML = xmlNodeList[0];
            xml = configurationRegistryRecordXML.OuterXml;

            ConfigurationRegistryRecord target = ConfigurationRegistryRecord.NewConfigurationRegistryRecord(xml); // TODO: Initialize to an appropriate value
            Assert.IsNotNull(target, string.Format("{0}.NewConfigurationRegistryRecord(xml) failed to create object", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

            List<string> tableNameList = new List<string>(); // TODO: Initialize to an appropriate value
            tableNameList.Add("VW_PICKLISTDOMAIN");
            string actual;
            actual = target.GetTableList(tableNameList);
            Assert.IsNotNull(actual);
        }

        /// <summary>
        ///A test for ExportSearchPermForm - search form is having form id 4003
        ///</summary>
        [TestMethod()]
        public void ExportSearchPermForm_ExportSearchFormWithID4003Test()
        {
            string expectedRegistrySerachFormID = "4003";
            ConfigurationRegistryRecord target;
            //create RegAdminOracleDAL object to avoid exceptions while running default constructor
            RegAdminOracleDAL regDal = null;
            Helpers.Helpers.GetRegDal(ref regDal, "");
            target = ConfigurationRegistryRecord.NewConfigurationRegistryRecord();
            Assert.IsNotNull(target, string.Format("{0}.NewConfigurationRegistryRecord() failed to create object", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

            target.COEFormHelper.Load(COEFormHelper.COEFormGroups.SearchPermanent);

            string formGroupXmlString = target.ExportSearchPermForm();
            Assert.IsTrue(!string.IsNullOrEmpty(formGroupXmlString), string.Format("{0}.ExportSearchPermForm() failed to return xml string", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

            XmlDocument formGroupDocument = new XmlDocument();
            formGroupDocument.InnerXml = formGroupXmlString;
            string actualRegistrySerachFormID = formGroupDocument.DocumentElement.Attributes["id"].Value;

            Assert.AreEqual<string>(expectedRegistrySerachFormID, actualRegistrySerachFormID, string.Format("{0}.ExportSearchPermForm() didi not returned the expected value", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

        }

        /// <summary>
        ///A test for ExportComponentDuplicatesForm - Duplicates form is having form id 4003
        ///</summary>
        [TestMethod()]
        public void ExportComponentDuplicatesForm_DuplicatesFormID4013Test()
        {
            string expectedRegistrySerachFormID = "4013";
            ConfigurationRegistryRecord target;
            //create RegAdminOracleDAL object to avoid exceptions while running default constructor
            RegAdminOracleDAL regDal = null;
            Helpers.Helpers.GetRegDal(ref regDal, "");
            target = ConfigurationRegistryRecord.NewConfigurationRegistryRecord();
            Assert.IsNotNull(target, string.Format("{0}.NewConfigurationRegistryRecord() failed to create object", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

            target.COEFormHelper.Load(COEFormHelper.COEFormGroups.SearchPermanent);

            string formGroupXmlString = target.ExportComponentDuplicatesForm();
            Assert.IsTrue(!string.IsNullOrEmpty(formGroupXmlString), string.Format("{0}.ExportComponentDuplicatesForm() failed to return xml string", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

            XmlDocument formGroupDocument = new XmlDocument();
            formGroupDocument.InnerXml = formGroupXmlString;
            string actualRegistrySerachFormID = formGroupDocument.DocumentElement.Attributes["id"].Value;

            Assert.AreEqual<string>(expectedRegistrySerachFormID, actualRegistrySerachFormID, string.Format("{0}.ExportComponentDuplicatesForm() didi not returned the expected value", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));
        }

        /// <summary>
        ///A test for ExportDataLoaderForm - data loader form id is 4015
        ///</summary>
        [TestMethod()]
        public void ExportDataLoaderForm_DAtaLoaderFormID4015Test()
        {
            string expectedRegistrySerachFormID = "4015";
            ConfigurationRegistryRecord target;
            //create RegAdminOracleDAL object to avoid exceptions while running default constructor
            RegAdminOracleDAL regDal = null;
            Helpers.Helpers.GetRegDal(ref regDal, "");
            target = ConfigurationRegistryRecord.NewConfigurationRegistryRecord();
            Assert.IsNotNull(target, string.Format("{0}.NewConfigurationRegistryRecord() failed to create object", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

            target.COEFormHelper.Load(COEFormHelper.COEFormGroups.SearchPermanent);

            string formGroupXmlString = target.ExportDataLoaderForm();
            Assert.IsTrue(!string.IsNullOrEmpty(formGroupXmlString), string.Format("{0}.ExportDataLoaderForm() failed to return xml string", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

            XmlDocument formGroupDocument = new XmlDocument();
            formGroupDocument.InnerXml = formGroupXmlString;
            string actualRegistrySerachFormID = formGroupDocument.DocumentElement.Attributes["id"].Value;

            Assert.AreEqual<string>(expectedRegistrySerachFormID, actualRegistrySerachFormID, string.Format("{0}.ExportDataLoaderForm() didi not returned the expected value", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));
        }

        /// <summary>
        ///A test for ExportDeleteLogForm - delete log form id is 4007
        ///</summary>
        [TestMethod()]
        public void ExportDeleteLogForm_DeleteLogForm4007Test()
        {
            string expectedRegistrySerachFormID = "4007";
            ConfigurationRegistryRecord target;
            //create RegAdminOracleDAL object to avoid exceptions while running default constructor
            RegAdminOracleDAL regDal = null;
            Helpers.Helpers.GetRegDal(ref regDal, "");
            target = ConfigurationRegistryRecord.NewConfigurationRegistryRecord();
            Assert.IsNotNull(target, string.Format("{0}.NewConfigurationRegistryRecord() failed to create object", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

            target.COEFormHelper.Load(COEFormHelper.COEFormGroups.SearchPermanent);

            string formGroupXmlString = target.ExportDeleteLogForm();
            Assert.IsTrue(!string.IsNullOrEmpty(formGroupXmlString), string.Format("{0}.ExportDeleteLogForm() failed to return xml string", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

            XmlDocument formGroupDocument = new XmlDocument();
            formGroupDocument.InnerXml = formGroupXmlString;
            string actualRegistrySerachFormID = formGroupDocument.DocumentElement.Attributes["id"].Value;

            Assert.AreEqual<string>(expectedRegistrySerachFormID, actualRegistrySerachFormID, string.Format("{0}.ExportDeleteLogForm() didi not returned the expected value", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));
        }

        /// <summary>
        ///A test for ExportELNSearchPermForm - ELN search permanant form id s 4006
        ///</summary>
        [TestMethod()]
        public void ExportELNSearchPermForm_ELNSearchForm4004Test()
        {

            string expectedRegistrySerachFormID = "4006";
            ConfigurationRegistryRecord target;
            //create RegAdminOracleDAL object to avoid exceptions while running default constructor
            RegAdminOracleDAL regDal = null;
            Helpers.Helpers.GetRegDal(ref regDal, "");
            target = ConfigurationRegistryRecord.NewConfigurationRegistryRecord();
            Assert.IsNotNull(target, string.Format("{0}.NewConfigurationRegistryRecord() failed to create object", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

            target.COEFormHelper.Load(COEFormHelper.COEFormGroups.SearchPermanent);

            string formGroupXmlString = target.ExportELNSearchPermForm();
            Assert.IsTrue(!string.IsNullOrEmpty(formGroupXmlString), string.Format("{0}.ExportELNSearchPermForm() failed to return xml string", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

            XmlDocument formGroupDocument = new XmlDocument();
            formGroupDocument.InnerXml = formGroupXmlString;
            string actualRegistrySerachFormID = formGroupDocument.DocumentElement.Attributes["id"].Value;

            Assert.AreEqual<string>(expectedRegistrySerachFormID, actualRegistrySerachFormID, string.Format("{0}.ExportELNSearchPermForm() didi not returned the expected value", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

        }

        /// <summary>
        ///A test for ExportELNSearchTempForm - ELN serach temporary form id is 4005
        ///</summary>
        [TestMethod()]
        public void ExportELNSearchTempForm_ELNTempSerach4005Test()
        {
            string expectedRegistrySerachFormID = "4005";
            ConfigurationRegistryRecord target;
            //create RegAdminOracleDAL object to avoid exceptions while running default constructor
            RegAdminOracleDAL regDal = null;
            Helpers.Helpers.GetRegDal(ref regDal, "");
            target = ConfigurationRegistryRecord.NewConfigurationRegistryRecord();
            Assert.IsNotNull(target, string.Format("{0}.NewConfigurationRegistryRecord() failed to create object", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

            target.COEFormHelper.Load(COEFormHelper.COEFormGroups.SearchPermanent);

            string formGroupXmlString = target.ExportELNSearchTempForm();
            Assert.IsTrue(!string.IsNullOrEmpty(formGroupXmlString), string.Format("{0}.ExportELNSearchTempForm() failed to return xml string", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

            XmlDocument formGroupDocument = new XmlDocument();
            formGroupDocument.InnerXml = formGroupXmlString;
            string actualRegistrySerachFormID = formGroupDocument.DocumentElement.Attributes["id"].Value;

            Assert.AreEqual<string>(expectedRegistrySerachFormID, actualRegistrySerachFormID, string.Format("{0}.ExportELNSearchTempForm() didi not returned the expected value", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

        }

        /// <summary>
        ///A test for ExportRegistryDuplicatesForm - Registryduplicates form id is 4014
        ///</summary>
        [TestMethod()]
        public void ExportRegistryDuplicatesForm_RegistryDuplicates4014Test()
        {
            string expectedRegistrySerachFormID = "4014";
            ConfigurationRegistryRecord target;
            //create RegAdminOracleDAL object to avoid exceptions while running default constructor
            RegAdminOracleDAL regDal = null;
            Helpers.Helpers.GetRegDal(ref regDal, "");
            target = ConfigurationRegistryRecord.NewConfigurationRegistryRecord();
            Assert.IsNotNull(target, string.Format("{0}.NewConfigurationRegistryRecord() failed to create object", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

            target.COEFormHelper.Load(COEFormHelper.COEFormGroups.SearchPermanent);

            string formGroupXmlString = target.ExportRegistryDuplicatesForm();
            Assert.IsTrue(!string.IsNullOrEmpty(formGroupXmlString), string.Format("{0}.ExportRegistryDuplicatesForm() failed to return xml string", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

            XmlDocument formGroupDocument = new XmlDocument();
            formGroupDocument.InnerXml = formGroupXmlString;
            string actualRegistrySerachFormID = formGroupDocument.DocumentElement.Attributes["id"].Value;

            Assert.AreEqual<string>(expectedRegistrySerachFormID, actualRegistrySerachFormID, string.Format("{0}.ExportRegistryDuplicatesForm() didi not returned the expected value", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

        }

        /// <summary>
        ///A test for ExportReviewMixtureForm - review mixture form id is 4011
        ///</summary>
        [TestMethod()]
        public void ExportReviewMixtureForm_ReviewMixture4011Test()
        {
            string expectedRegistrySerachFormID = "4011";
            ConfigurationRegistryRecord target;
            //create RegAdminOracleDAL object to avoid exceptions while running default constructor
            RegAdminOracleDAL regDal = null;
            Helpers.Helpers.GetRegDal(ref regDal, "");
            target = ConfigurationRegistryRecord.NewConfigurationRegistryRecord();
            Assert.IsNotNull(target, string.Format("{0}.NewConfigurationRegistryRecord() failed to create object", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

            target.COEFormHelper.Load(COEFormHelper.COEFormGroups.SearchPermanent);

            string formGroupXmlString = target.ExportReviewMixtureForm();
            Assert.IsTrue(!string.IsNullOrEmpty(formGroupXmlString), string.Format("{0}.ExportReviewMixtureForm() failed to return xml string", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

            XmlDocument formGroupDocument = new XmlDocument();
            formGroupDocument.InnerXml = formGroupXmlString;
            string actualRegistrySerachFormID = formGroupDocument.DocumentElement.Attributes["id"].Value;

            Assert.AreEqual<string>(expectedRegistrySerachFormID, actualRegistrySerachFormID, string.Format("{0}.ExportReviewMixtureForm() didi not returned the expected value", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

        }

        /// <summary>
        ///A test for ExportSearchComponentsToAddForm - search components to add form id is 4016
        ///</summary>
        [TestMethod()]
        public void ExportSearchComponentsToAddForm_SearchComponentsToAdd4016Test()
        {
            string expectedRegistrySerachFormID = "4016";
            ConfigurationRegistryRecord target;
            //create RegAdminOracleDAL object to avoid exceptions while running default constructor
            RegAdminOracleDAL regDal = null;
            Helpers.Helpers.GetRegDal(ref regDal, "");
            target = ConfigurationRegistryRecord.NewConfigurationRegistryRecord();
            Assert.IsNotNull(target, string.Format("{0}.NewConfigurationRegistryRecord() failed to create object", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

            target.COEFormHelper.Load(COEFormHelper.COEFormGroups.SearchPermanent);

            string formGroupXmlString = target.ExportSearchComponentsToAddForm();
            Assert.IsTrue(!string.IsNullOrEmpty(formGroupXmlString), string.Format("{0}.ExportSearchComponentsToAddForm() failed to return xml string", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

            XmlDocument formGroupDocument = new XmlDocument();
            formGroupDocument.InnerXml = formGroupXmlString;
            string actualRegistrySerachFormID = formGroupDocument.DocumentElement.Attributes["id"].Value;

            Assert.AreEqual<string>(expectedRegistrySerachFormID, actualRegistrySerachFormID, string.Format("{0}.ExportSearchComponentsToAddForm() didi not returned the expected value", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

        }

        /// <summary>
        ///A test for ExportSearchComponentsToAddRRForm - form id is 4017
        ///</summary>
        [TestMethod()]
        public void ExportSearchComponentsToAddRRForm_SearchComponentsToAddRR4017Test()
        {
            string expectedRegistrySerachFormID = "4017";
            ConfigurationRegistryRecord target;
            //create RegAdminOracleDAL object to avoid exceptions while running default constructor
            RegAdminOracleDAL regDal = null;
            Helpers.Helpers.GetRegDal(ref regDal, "");
            target = ConfigurationRegistryRecord.NewConfigurationRegistryRecord();
            Assert.IsNotNull(target, string.Format("{0}.NewConfigurationRegistryRecord() failed to create object", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

            target.COEFormHelper.Load(COEFormHelper.COEFormGroups.SearchPermanent);

            string formGroupXmlString = target.ExportSearchComponentsToAddRRForm();
            Assert.IsTrue(!string.IsNullOrEmpty(formGroupXmlString), string.Format("{0}.ExportSearchComponentsToAddRRForm() failed to return xml string", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

            XmlDocument formGroupDocument = new XmlDocument();
            formGroupDocument.InnerXml = formGroupXmlString;
            string actualRegistrySerachFormID = formGroupDocument.DocumentElement.Attributes["id"].Value;

            Assert.AreEqual<string>(expectedRegistrySerachFormID, actualRegistrySerachFormID, string.Format("{0}.ExportSearchComponentsToAddRRForm() didi not returned the expected value", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));
        }

        /// <summary>
        ///A test for ExportSearchTempForm - form id is 4002
        ///</summary>
        [TestMethod()]
        public void ExportSearchTempForm_SearchTemp4002Test()
        {
            string expectedRegistrySerachFormID = "4002";
            ConfigurationRegistryRecord target;
            //create RegAdminOracleDAL object to avoid exceptions while running default constructor
            RegAdminOracleDAL regDal = null;
            Helpers.Helpers.GetRegDal(ref regDal, "");
            target = ConfigurationRegistryRecord.NewConfigurationRegistryRecord();
            Assert.IsNotNull(target, string.Format("{0}.NewConfigurationRegistryRecord() failed to create object", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

            target.COEFormHelper.Load(COEFormHelper.COEFormGroups.SearchPermanent);

            string formGroupXmlString = target.ExportSearchTempForm();
            Assert.IsTrue(!string.IsNullOrEmpty(formGroupXmlString), string.Format("{0}.ExportSearchTempForm() failed to return xml string", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

            XmlDocument formGroupDocument = new XmlDocument();
            formGroupDocument.InnerXml = formGroupXmlString;
            string actualRegistrySerachFormID = formGroupDocument.DocumentElement.Attributes["id"].Value;

            Assert.AreEqual<string>(expectedRegistrySerachFormID, actualRegistrySerachFormID, string.Format("{0}.ExportSearchTempForm() didi not returned the expected value", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));
        }

        /// <summary>
        ///A test for ExportSendToRegistrationForm - form id is 4002
        ///</summary>
        [TestMethod()]
        public void ExportSendToRegistrationForm_SendToRegistration4004Test()
        {
            string expectedRegistrySerachFormID = "4004";
            ConfigurationRegistryRecord target;
            //create RegAdminOracleDAL object to avoid exceptions while running default constructor
            RegAdminOracleDAL regDal = null;
            Helpers.Helpers.GetRegDal(ref regDal, "");
            target = ConfigurationRegistryRecord.NewConfigurationRegistryRecord();
            Assert.IsNotNull(target, string.Format("{0}.NewConfigurationRegistryRecord() failed to create object", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

            target.COEFormHelper.Load(COEFormHelper.COEFormGroups.SearchPermanent);

            string formGroupXmlString = target.ExportSendToRegistrationForm();
            Assert.IsTrue(!string.IsNullOrEmpty(formGroupXmlString), string.Format("{0}.ExportSendToRegistrationForm() failed to return xml string", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

            XmlDocument formGroupDocument = new XmlDocument();
            formGroupDocument.InnerXml = formGroupXmlString;
            string actualRegistrySerachFormID = formGroupDocument.DocumentElement.Attributes["id"].Value;

            Assert.AreEqual<string>(expectedRegistrySerachFormID, actualRegistrySerachFormID, string.Format("{0}.ExportSendToRegistrationForm() didi not returned the expected value", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

        }

        /// <summary>
        ///A test for ExportSubmitMixtureForm - form id is 4010
        ///</summary>
        [TestMethod()]
        public void ExportSubmitMixtureForm_SubmitMixture4010Test()
        {
            string expectedRegistrySerachFormID = "4010";
            ConfigurationRegistryRecord target;
            //create RegAdminOracleDAL object to avoid exceptions while running default constructor
            RegAdminOracleDAL regDal = null;
            Helpers.Helpers.GetRegDal(ref regDal, "");
            target = ConfigurationRegistryRecord.NewConfigurationRegistryRecord();
            Assert.IsNotNull(target, string.Format("{0}.NewConfigurationRegistryRecord() failed to create object", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

            target.COEFormHelper.Load(COEFormHelper.COEFormGroups.SearchPermanent);

            string formGroupXmlString = target.ExportSubmitMixtureForm();
            Assert.IsTrue(!string.IsNullOrEmpty(formGroupXmlString), string.Format("{0}.ExportSubmitMixtureForm() failed to return xml string", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

            XmlDocument formGroupDocument = new XmlDocument();
            formGroupDocument.InnerXml = formGroupXmlString;
            string actualRegistrySerachFormID = formGroupDocument.DocumentElement.Attributes["id"].Value;

            Assert.AreEqual<string>(expectedRegistrySerachFormID, actualRegistrySerachFormID, string.Format("{0}.ExportSubmitMixtureForm() didi not returned the expected value", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

        }

        /// <summary>
        ///A test for ExportViewMixtureForm - form id is 4012
        ///</summary>
        [TestMethod()]
        public void ExportViewMixtureForm_ViewMixture4012Test()
        {
            string expectedRegistrySerachFormID = "4012";
            ConfigurationRegistryRecord target;
            //create RegAdminOracleDAL object to avoid exceptions while running default constructor
            RegAdminOracleDAL regDal = null;
            Helpers.Helpers.GetRegDal(ref regDal, "");
            target = ConfigurationRegistryRecord.NewConfigurationRegistryRecord();
            Assert.IsNotNull(target, string.Format("{0}.NewConfigurationRegistryRecord() failed to create object", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

            target.COEFormHelper.Load(COEFormHelper.COEFormGroups.SearchPermanent);

            string formGroupXmlString = target.ExportViewMixtureForm();
            Assert.IsTrue(!string.IsNullOrEmpty(formGroupXmlString), string.Format("{0}.ExportViewMixtureForm() failed to return xml string", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

            XmlDocument formGroupDocument = new XmlDocument();
            formGroupDocument.InnerXml = formGroupXmlString;
            string actualRegistrySerachFormID = formGroupDocument.DocumentElement.Attributes["id"].Value;

            Assert.AreEqual<string>(expectedRegistrySerachFormID, actualRegistrySerachFormID, string.Format("{0}.ExportViewMixtureForm() didi not returned the expected value", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

        }

        /// <summary>
        ///A test for ImportConfigurationSettings
        ///</summary>
        ///<remarks>
        ///Kindly execute the query (DELETE FROM COEDB.COECONFIGURATION WHERE DESCRIPTION = 'Registration') on COEDB database before executing this test method
        ///</remarks>
        [TestMethod()]
        public void ImportConfigurationSettingsTest()
        {

            ConfigurationRegistryRecord target;
            //create RegAdminOracleDAL object to avoid exceptions while running default constructor
            RegAdminOracleDAL regDal = null;
            Helpers.Helpers.GetRegDal(ref regDal, "");

            //DbCommand cmd = regDal.DALManager.Database.GetSqlStringCommand("DELETE FROM COEDB.COECONFIGURATION WHERE DESCRIPTION = 'Registration'");
            //regDal.DALManager.ExecuteNonQuery(cmd);

            target = ConfigurationRegistryRecord.NewConfigurationRegistryRecord();
            Assert.IsNotNull(target, string.Format("{0}.NewConfigurationRegistryRecord() failed to create object", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

            string configSettings = string.Empty; // TODO: Initialize to an appropriate value
            XmlNodeList xmlNodeList = XMLDataLoader.PrepareXMLNodeList(registrationAppSettingsXmlDoc); // TODO: Initialize to an appropriate value
            Assert.IsNotNull(xmlNodeList, "xml node list null");

            XmlNode registrationAppSettingsyRecordXML = xmlNodeList[0];
            configSettings = registrationAppSettingsyRecordXML.OuterXml;

            //import default configuration xml settings
            target.ImportConfigurationSettings(configSettings);
            Assert.IsFalse(target.IsImported);
        }

        /// <summary>
        ///A test for ImportCustomization
        ///</summary>
        [TestMethod()]
        public void ImportCustomization_WithoutForceImportTest()
        {
            string appRootInstallPath = @"C:\CBOE\EN1250_development\ChemOfficeEnterprise\Registration"; // TODO: Initialize to an appropriate value
            string folderPath = @"C:\CBOE\EN1250_development\ChemOfficeEnterprise\Registration\Config\default"; // TODO: Initialize to an appropriate value
            bool forceImport = false; // TODO: Initialize to an appropriate value

            ConfigurationRegistryRecord target;
            //create RegAdminOracleDAL object to avoid exceptions while running default constructor
            RegAdminOracleDAL regDal = null;
            Helpers.Helpers.GetRegDal(ref regDal, "");
            target = ConfigurationRegistryRecord.NewConfigurationRegistryRecord();
            Assert.IsNotNull(target, string.Format("{0}.NewConfigurationRegistryRecord() failed to create object", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

            target.ImportCustomization(appRootInstallPath, folderPath, forceImport);

            Assert.IsTrue(target.IsImported, string.Format("{0}.ImportCustomization(appRootInstallPath, folderPath, forceImport) failed to import configuration", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));
        }

        /// <summary>
        ///A test for ImportCustomization
        ///</summary>
        [TestMethod()]
        public void ImportCustomization_WithForceImportTest()
        {
            string appRootInstallPath = @"C:\CBOE\EN1250_development\ChemOfficeEnterprise\Registration"; // TODO: Initialize to an appropriate value
            string folderPath = @"C:\CBOE\EN1250_development\ChemOfficeEnterprise\Registration\Config\default"; // TODO: Initialize to an appropriate value
            bool forceImport = true; // TODO: Initialize to an appropriate value

            ConfigurationRegistryRecord target;
            //create RegAdminOracleDAL object to avoid exceptions while running default constructor
            RegAdminOracleDAL regDal = null;
            Helpers.Helpers.GetRegDal(ref regDal, "");
            target = ConfigurationRegistryRecord.NewConfigurationRegistryRecord();
            Assert.IsNotNull(target, string.Format("{0}.NewConfigurationRegistryRecord() failed to create object", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

            target.ImportCustomization(appRootInstallPath, folderPath, forceImport);

            Assert.IsTrue(!target.IsImported, string.Format("{0}.ImportCustomization(appRootInstallPath, folderPath, forceImport) failed to import configuration", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));
        }

        /// <summary>
        ///A test for ImportTableList
        ///</summary>
        [TestMethod()]
        public void ImportTableList_ShouldImportTableListXMLTest()
        {
            ConfigurationRegistryRecord target;
            //create RegAdminOracleDAL object to avoid exceptions while running default constructor
            RegAdminOracleDAL regDal = null;
            Helpers.Helpers.GetRegDal(ref regDal, "");
            target = ConfigurationRegistryRecord.NewConfigurationRegistryRecord();
            Assert.IsNotNull(target, string.Format("{0}.NewConfigurationRegistryRecord() failed to create object", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

            string xmlTableList = customTableListXmlDoc.ChildNodes[1].OuterXml;
            target.ImportTableList(xmlTableList);

            List<string> tableNameList = new List<string>(); // TODO: Initialize to an appropriate value
            tableNameList.Add("VW_PICKLISTDOMAIN");
            tableNameList.Add("VW_PICKLIST");
            tableNameList.Add("FRAGMENTTYPE");
            tableNameList.Add("FRAGMENTS");
            tableNameList.Add("VW_SEQUENCE");
            tableNameList.Add("VW_IDENTIFIERTYPE");
            tableNameList.Add("VW_PROJECT");

            string[] tableNameArray = target.GetTableNames;
            Assert.IsNotNull(tableNameArray, string.Format("{0}.ImportTableList(xmlTableList) failed to import tables", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));
            Assert.IsTrue(tableNameArray.Length > 0, string.Format("{0}.GetTableNames failed to return expected value", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

            List<string> tableNameListFromDB = new List<string>(tableNameArray);

            for (int i = 0; i < tableNameList.Count; i++)
            {
                bool isContains = tableNameListFromDB.Contains(tableNameList[i]);
                Assert.IsTrue(isContains, string.Format("{0}.ImportTableList(xmlTableList) failed to import table {1}", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target), tableNameList[i]));
            }
        }

        /// <summary>
        ///A test for ImportForm
        ///</summary>
        [TestMethod()]
        public void ImportForm_ImportStandard4010FormTest()
        {
            ConfigurationRegistryRecord target;
            //create RegAdminOracleDAL object to avoid exceptions while running default constructor
            RegAdminOracleDAL regDal = null;
            Helpers.Helpers.GetRegDal(ref regDal, "");
            target = ConfigurationRegistryRecord.NewConfigurationRegistryRecord();
            Assert.IsNotNull(target, string.Format("{0}.NewConfigurationRegistryRecord() failed to create object", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

            PrivateObject obj = new PrivateObject(target);
            obj.SetField("_forceImport", true);

            string coeFormGroup = coeFormGroupXmlDoc.ChildNodes[1].InnerXml; // TODO: Initialize to an appropriate value
            target.ImportForm(coeFormGroup);

            FormGroup frmGroup = target.FormGroup;
            Assert.IsNotNull(frmGroup, string.Format("{0}.FormGroup failed to rturn expected value", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

            int expected = 4010;
            int actual = frmGroup.Id;

            Assert.AreEqual<int>(expected, actual, string.Format("{0}.ImportForm(coeFormGroup) failed to import form 4010", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));
        }

        /// <summary>
        ///A test for ImportForm
        ///</summary>
        [TestMethod()]
        public void ImportForm_ImportDummy4020FormTest()
        {
            int formId = 4020;

            ConfigurationRegistryRecord target;
            //create RegAdminOracleDAL object to avoid exceptions while running default constructor
            RegAdminOracleDAL regDal = null;
            Helpers.Helpers.GetRegDal(ref regDal, "");
            target = ConfigurationRegistryRecord.NewConfigurationRegistryRecord();
            Assert.IsNotNull(target, string.Format("{0}.NewConfigurationRegistryRecord() failed to create object", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

            PrivateObject obj = new PrivateObject(target);
            obj.SetField("_forceImport", true);

            XmlNode formNode = coeFormGroupXmlDoc.ChildNodes[1];
            formNode.ChildNodes[0].Attributes["id"].Value = formId.ToString();

            string coeFormGroup = formNode.InnerXml; // TODO: Initialize to an appropriate value
            target.ImportForm(coeFormGroup);

            //connect to COEDB direct, and delete the newly added form 4020
            Database ds = null;
            Helpers.Helpers.GetDatabase(ref ds, Helpers.Helpers.DbType.COEDB);

            StringBuilder strSql = new StringBuilder();
            strSql.AppendFormat("SELECT COUNT(*) FROM COEFORM WHERE ID = {0}", formId);

            object formCountObj = ds.ExecuteScalar(CommandType.Text, strSql.ToString());
            Assert.IsNotNull(formCountObj, string.Format("{0}.ImportForm(coeFormGroup) failed to import form object", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));
            Assert.IsTrue(Convert.ToInt32(formCountObj) > 0, string.Format("{0}.ImportForm(coeFormGroup) failed to import form object", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

            strSql.Length = 0;
            strSql.AppendFormat("DELETE FROM COEFORM WHERE ID = {0}", formId);
            ds.ExecuteNonQuery(CommandType.Text, strSql.ToString());
        }

        /// <summary>
        ///A test for DatabaseReservedWords
        ///</summary>
        [TestMethod()]
        public void DatabaseReservedWords_GetDatabaseReservedWordsTest()
        {
            ConfigurationRegistryRecord target;
            //create RegAdminOracleDAL object to avoid exceptions while running default constructor
            RegAdminOracleDAL regDal = null;
            Helpers.Helpers.GetRegDal(ref regDal, "");
            target = ConfigurationRegistryRecord.NewConfigurationRegistryRecord();
            Assert.IsNotNull(target, string.Format("{0}.NewConfigurationRegistryRecord() failed to create object", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

            List<string> actual;
            actual = target.DatabaseReservedWords;
            Assert.IsNotNull(actual, string.Format("{0}.DatabaseReservedWords is null", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));
            Assert.IsTrue(actual.Count == 1828, string.Format("{0}.DatabaseReservedWords did not returned expected value", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));
        }

        /// <summary>
        ///A test for AddInList
        ///</summary>
        [TestMethod()]
        public void AddInList_getAddInTest()
        {
            ConfigurationRegistryRecord target;
            //create RegAdminOracleDAL object to avoid exceptions while running default constructor
            RegAdminOracleDAL regDal = null;
            Helpers.Helpers.GetRegDal(ref regDal, "");
            target = ConfigurationRegistryRecord.NewConfigurationRegistryRecord();
            Assert.IsNotNull(target, string.Format("{0}.NewConfigurationRegistryRecord() failed to create object", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

            AddInList actual;
            actual = target.AddInList;
            Assert.IsNotNull(actual, string.Format("{0}.AddInList is null", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));
            Assert.IsTrue(actual.Count == 5, string.Format("{0}.AddInList did not returned expected value", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));
        }

        /// <summary>
        ///A test for BatchComponentColumnList
        ///</summary>
        [TestMethod()]
        public void BatchComponentColumnList_GetBatchComponentColumnsTest()
        {
            ConfigurationRegistryRecord target;
            //create RegAdminOracleDAL object to avoid exceptions while running default constructor
            RegAdminOracleDAL regDal = null;
            Helpers.Helpers.GetRegDal(ref regDal, "");
            target = ConfigurationRegistryRecord.NewConfigurationRegistryRecord();
            Assert.IsNotNull(target, string.Format("{0}.NewConfigurationRegistryRecord() failed to create object", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

            List<string> actual;
            actual = target.BatchComponentColumnList;
            Assert.IsNotNull(actual, string.Format("{0}.BatchComponentColumnList is null", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));
            Assert.IsTrue(actual.Count == 5, string.Format("{0}.BatchComponentColumnList did not returned expected value", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));
        }

        /// <summary>
        ///A test for BatchComponentForm
        ///</summary>
        [TestMethod()]
        public void BatchComponentForm_GetBatchComponentFormTest()
        {
            ConfigurationRegistryRecord target;
            //create RegAdminOracleDAL object to avoid exceptions while running default constructor
            RegAdminOracleDAL regDal = null;
            Helpers.Helpers.GetRegDal(ref regDal, "");
            target = ConfigurationRegistryRecord.NewConfigurationRegistryRecord();
            Assert.IsNotNull(target, string.Format("{0}.NewConfigurationRegistryRecord() failed to create object", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

            FormGroup.Form actual;
            actual = target.BatchComponentForm;
            Assert.IsNotNull(actual, string.Format("{0}.BatchComponentForm is null", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

            string expected = "Batch Component custom properties";
            Assert.AreEqual<string>(expected, actual.Title, string.Format("{0}.BatchComponentForm failed to return expected value", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

        }

        /// <summary>
        ///A test for BatchComponentList
        ///</summary>
        [TestMethod()]
        public void BatchComponentList_GetBatchComponentListTest()
        {
            ConfigurationRegistryRecord target;
            //create RegAdminOracleDAL object to avoid exceptions while running default constructor
            RegAdminOracleDAL regDal = null;
            Helpers.Helpers.GetRegDal(ref regDal, "");
            target = ConfigurationRegistryRecord.NewConfigurationRegistryRecord();
            Assert.IsNotNull(target, string.Format("{0}.NewConfigurationRegistryRecord() failed to create object", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

            PropertyList actual;
            actual = target.BatchComponentList;
            Assert.IsNotNull(actual, string.Format("{0}.BatchComponentColumnList is null", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

            string expected = "PERCENTAGE";
            Assert.AreEqual<string>(expected, actual[0].Name, string.Format("{0}.BatchComponentList failed to return expected value", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));
        }

        /// <summary>
        ///A test for BatchForm
        ///</summary>
        [TestMethod()]
        public void BatchFormTest()
        {
            ConfigurationRegistryRecord target;
            //create RegAdminOracleDAL object to avoid exceptions while running default constructor
            RegAdminOracleDAL regDal = null;
            Helpers.Helpers.GetRegDal(ref regDal, "");
            target = ConfigurationRegistryRecord.NewConfigurationRegistryRecord();
            Assert.IsNotNull(target, string.Format("{0}.NewConfigurationRegistryRecord() failed to create object", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

            FormGroup.Form actual;
            actual = target.BatchForm;
            Assert.IsNotNull(actual, string.Format("{0}.BatchForm is null", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

            string expected = "Batch Information";
            Assert.AreEqual<string>(expected, actual.Title, string.Format("{0}.BatchForm failed to return expected value", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));
        }

        /// <summary>
        ///A test for BatchPropertyColumnList
        ///</summary>
        [TestMethod()]
        public void BatchPropertyColumnList_GetBatchPropertyListTest()
        {
            ConfigurationRegistryRecord target;
            //create RegAdminOracleDAL object to avoid exceptions while running default constructor
            RegAdminOracleDAL regDal = null;
            Helpers.Helpers.GetRegDal(ref regDal, "");
            target = ConfigurationRegistryRecord.NewConfigurationRegistryRecord();
            Assert.IsNotNull(target, string.Format("{0}.NewConfigurationRegistryRecord() failed to create object", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

            List<string> actual;
            actual = target.BatchPropertyColumnList;
            Assert.IsNotNull(actual, string.Format("{0}.BatchPropertyColumnList is null", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));
            Assert.IsTrue(actual.Count > 0, string.Format("{0}.BatchPropertyColumnList did not returned expected value", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));
        }

        /// <summary>
        ///A test for BatchPropertyList
        ///</summary>
        [TestMethod()]
        public void BatchPropertyList_GetBatchPropertyListTest()
        {
            ConfigurationRegistryRecord target;
            //create RegAdminOracleDAL object to avoid exceptions while running default constructor
            RegAdminOracleDAL regDal = null;
            Helpers.Helpers.GetRegDal(ref regDal, "");
            target = ConfigurationRegistryRecord.NewConfigurationRegistryRecord();
            Assert.IsNotNull(target, string.Format("{0}.NewConfigurationRegistryRecord() failed to create object", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

            PropertyList actual;
            actual = target.BatchPropertyList;
            Assert.IsNotNull(actual, string.Format("{0}.BatchPropertyList is null", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));
            Assert.IsTrue(actual.Count > 0, string.Format("{0}.BatchPropertyList did not returned expected value", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));
        }

        /// <summary>
        ///A test for COEFormHelper
        ///</summary>
        [TestMethod()]
        public void COEFormHelperTest()
        {
            ConfigurationRegistryRecord target;
            //create RegAdminOracleDAL object to avoid exceptions while running default constructor
            RegAdminOracleDAL regDal = null;
            Helpers.Helpers.GetRegDal(ref regDal, "");
            target = ConfigurationRegistryRecord.NewConfigurationRegistryRecord();
            Assert.IsNotNull(target, string.Format("{0}.NewConfigurationRegistryRecord() failed to create object", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

            COEFormHelper actual;
            actual = target.COEFormHelper;
            Assert.IsNotNull(actual, string.Format("{0}.COEFormHelper is null", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));
        }

        /// <summary>
        ///A test for ComponentDuplicatesFormGroup
        ///</summary>
        [TestMethod()]
        public void ComponentDuplicatesFormGroup_GetComponentDuplicateFormGroupTest()
        {
            ConfigurationRegistryRecord target;
            //create RegAdminOracleDAL object to avoid exceptions while running default constructor
            RegAdminOracleDAL regDal = null;
            Helpers.Helpers.GetRegDal(ref regDal, "");
            target = ConfigurationRegistryRecord.NewConfigurationRegistryRecord();
            Assert.IsNotNull(target, string.Format("{0}.NewConfigurationRegistryRecord() failed to create object", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

            FormGroup.Form actual;
            actual = target.ComponentDuplicatesFormGroup;
            Assert.IsNotNull(actual, string.Format("{0}.ComponentDuplicatesFormGroup is null", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

            string expected = "Component Information";
            Assert.AreEqual<string>(expected, actual.Title, string.Format("{0}.ComponentDuplicatesFormGroup failed to return expected value", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));
        }

        /// <summary>
        ///A test for CompoundForm
        ///</summary>
        [TestMethod()]
        public void CompoundForm_GetCompoundFormTest()
        {
            ConfigurationRegistryRecord target;
            //create RegAdminOracleDAL object to avoid exceptions while running default constructor
            RegAdminOracleDAL regDal = null;
            Helpers.Helpers.GetRegDal(ref regDal, "");
            target = ConfigurationRegistryRecord.NewConfigurationRegistryRecord();
            Assert.IsNotNull(target, string.Format("{0}.NewConfigurationRegistryRecord() failed to create object", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

            FormGroup.Form actual;
            actual = target.CompoundForm;
            Assert.IsNotNull(actual, string.Format("{0}.CompoundForm is null", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

            string expected = "Component Information";
            Assert.AreEqual<string>(expected, actual.Title, string.Format("{0}.CompoundForm failed to return expected value", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));
        }

        /// <summary>
        ///A test for CompoundPropertyColumnList
        ///</summary>
        [TestMethod()]
        public void CompoundPropertyColumnList_GetCompoundPropertyColumnListTest()
        {
            ConfigurationRegistryRecord target;
            //create RegAdminOracleDAL object to avoid exceptions while running default constructor
            RegAdminOracleDAL regDal = null;
            Helpers.Helpers.GetRegDal(ref regDal, "");
            target = ConfigurationRegistryRecord.NewConfigurationRegistryRecord();
            Assert.IsNotNull(target, string.Format("{0}.NewConfigurationRegistryRecord() failed to create object", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

            List<string> actual;
            actual = target.CompoundPropertyColumnList;
            Assert.IsNotNull(actual, string.Format("{0}.CompoundPropertyColumnList is null", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));
            Assert.IsTrue(actual.Count > 0, string.Format("{0}.CompoundPropertyColumnList did not returned expected value", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));
        }

        /// <summary>
        ///A test for CompoundPropertyList
        ///</summary>
        [TestMethod()]
        public void CompoundPropertyList_GetCompoundPropertyListTest()
        {
            ConfigurationRegistryRecord target;
            //create RegAdminOracleDAL object to avoid exceptions while running default constructor
            RegAdminOracleDAL regDal = null;
            Helpers.Helpers.GetRegDal(ref regDal, "");
            target = ConfigurationRegistryRecord.NewConfigurationRegistryRecord();
            Assert.IsNotNull(target, string.Format("{0}.NewConfigurationRegistryRecord() failed to create object", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

            PropertyList actual;
            actual = target.CompoundPropertyList;
            Assert.IsNotNull(actual, string.Format("{0}.CompoundPropertyList is null", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));
            Assert.IsTrue(actual.Count > 0, string.Format("{0}.CompoundPropertyList did not returned expected value", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));
        }

        /// <summary>
        ///A test for ELNSearchPermBatchComponentDetailForm
        ///</summary>
        [TestMethod()]
        public void ELNSearchPermBatchComponentDetailFormTest()
        {
            ConfigurationRegistryRecord target;
            //create RegAdminOracleDAL object to avoid exceptions while running default constructor
            RegAdminOracleDAL regDal = null;
            Helpers.Helpers.GetRegDal(ref regDal, "");
            target = ConfigurationRegistryRecord.NewConfigurationRegistryRecord();
            Assert.IsNotNull(target, string.Format("{0}.NewConfigurationRegistryRecord() failed to create object", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

            FormGroup.Form actual;
            actual = target.ELNSearchPermBatchComponentDetailForm;
            Assert.IsNotNull(actual, string.Format("{0}.ELNSearchPermBatchComponentDetailForm is null", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));
        }

        /// <summary>
        ///A test for ELNSearchPermBatchComponentQueryForm
        ///</summary>
        [TestMethod()]
        public void ELNSearchPermBatchComponentQueryFormTest()
        {
            ConfigurationRegistryRecord target;
            //create RegAdminOracleDAL object to avoid exceptions while running default constructor
            RegAdminOracleDAL regDal = null;
            Helpers.Helpers.GetRegDal(ref regDal, "");
            target = ConfigurationRegistryRecord.NewConfigurationRegistryRecord();
            Assert.IsNotNull(target, string.Format("{0}.NewConfigurationRegistryRecord() failed to create object", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

            FormGroup.Form actual;
            actual = target.ELNSearchPermBatchComponentQueryForm;
            Assert.IsNotNull(actual, string.Format("{0}.ELNSearchPermBatchComponentQueryForm is null", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

            string expected = "Batch Component Information";
            Assert.AreEqual<string>(expected, actual.Title, string.Format("{0}.ELNSearchPermBatchComponentQueryForm did not returned expected value", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));
        }

        /// <summary>
        ///A test for ELNSearchPermBatchDetailForm
        ///</summary>
        [TestMethod()]
        public void ELNSearchPermBatchDetailFormTest()
        {
            ConfigurationRegistryRecord target;
            //create RegAdminOracleDAL object to avoid exceptions while running default constructor
            RegAdminOracleDAL regDal = null;
            Helpers.Helpers.GetRegDal(ref regDal, "");
            target = ConfigurationRegistryRecord.NewConfigurationRegistryRecord();
            Assert.IsNotNull(target, string.Format("{0}.NewConfigurationRegistryRecord() failed to create object", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

            FormGroup.Form actual;
            actual = target.ELNSearchPermBatchDetailForm;
            Assert.IsNotNull(actual, string.Format("{0}.ELNSearchPermBatchDetailForm is null", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));
        }

        /// <summary>
        ///A test for ELNSearchPermBatchQueryForm
        ///</summary>
        [TestMethod()]
        public void ELNSearchPermBatchQueryFormTest()
        {
            ConfigurationRegistryRecord target;
            //create RegAdminOracleDAL object to avoid exceptions while running default constructor
            RegAdminOracleDAL regDal = null;
            Helpers.Helpers.GetRegDal(ref regDal, "");
            target = ConfigurationRegistryRecord.NewConfigurationRegistryRecord();
            Assert.IsNotNull(target, string.Format("{0}.NewConfigurationRegistryRecord() failed to create object", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

            FormGroup.Form actual;
            actual = target.ELNSearchPermBatchQueryForm;
            Assert.IsNotNull(actual, string.Format("{0}.ELNSearchPermBatchQueryForm is null", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

            string expected = "Batch Information";
            Assert.AreEqual<string>(expected, actual.Title, string.Format("{0}.ELNSearchPermBatchQueryForm did not return expected value", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));
        }

        /// <summary>
        ///A test for ELNSearchPermCompoundDetailForm
        ///</summary>
        [TestMethod()]
        public void ELNSearchPermCompoundDetailFormTest()
        {
            ConfigurationRegistryRecord target;
            //create RegAdminOracleDAL object to avoid exceptions while running default constructor
            RegAdminOracleDAL regDal = null;
            Helpers.Helpers.GetRegDal(ref regDal, "");
            target = ConfigurationRegistryRecord.NewConfigurationRegistryRecord();
            Assert.IsNotNull(target, string.Format("{0}.NewConfigurationRegistryRecord() failed to create object", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

            FormGroup.Form actual;
            actual = target.ELNSearchPermCompoundDetailForm;
            Assert.IsNotNull(actual, string.Format("{0}.ELNSearchPermCompoundDetailForm is null", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

            string expected = "";
            Assert.AreEqual<string>(expected, actual.Title, string.Format("{0}.ELNSearchPermCompoundDetailForm did not return expected value", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));
        }

        /// <summary>
        ///A test for ELNSearchPermCompoundQueryForm
        ///</summary>
        [TestMethod()]
        public void ELNSearchPermCompoundQueryFormTest()
        {
            ConfigurationRegistryRecord target;
            //create RegAdminOracleDAL object to avoid exceptions while running default constructor
            RegAdminOracleDAL regDal = null;
            Helpers.Helpers.GetRegDal(ref regDal, "");
            target = ConfigurationRegistryRecord.NewConfigurationRegistryRecord();
            Assert.IsNotNull(target, string.Format("{0}.NewConfigurationRegistryRecord() failed to create object", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

            FormGroup.Form actual;
            actual = target.ELNSearchPermCompoundQueryForm;
            Assert.IsNotNull(actual, string.Format("{0}.ELNSearchPermCompoundQueryForm is null", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

            string expected = "Component Information";
            Assert.AreEqual<string>(expected, actual.Title, string.Format("{0}.ELNSearchPermCompoundQueryForm did not return expected value", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));
        }

        /// <summary>
        ///A test for ELNSearchPermMixtureDetailForm
        ///</summary>
        [TestMethod()]
        public void ELNSearchPermMixtureDetailFormTest()
        {
            ConfigurationRegistryRecord target;
            //create RegAdminOracleDAL object to avoid exceptions while running default constructor
            RegAdminOracleDAL regDal = null;
            Helpers.Helpers.GetRegDal(ref regDal, "");
            target = ConfigurationRegistryRecord.NewConfigurationRegistryRecord();
            Assert.IsNotNull(target, string.Format("{0}.NewConfigurationRegistryRecord() failed to create object", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

            FormGroup.Form actual;
            actual = target.ELNSearchPermMixtureDetailForm;
            Assert.IsNotNull(actual, string.Format("{0}.ELNSearchPermMixtureDetailForm is null", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

            string expected = "";
            Assert.AreEqual<string>(expected, actual.Title, string.Format("{0}.ELNSearchPermMixtureDetailForm did not return expected value", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));
        }

        /// <summary>
        ///A test for ELNSearchPermMixtureQueryForm
        ///</summary>
        [TestMethod()]
        public void ELNSearchPermMixtureQueryFormTest()
        {
            ConfigurationRegistryRecord target;
            //create RegAdminOracleDAL object to avoid exceptions while running default constructor
            RegAdminOracleDAL regDal = null;
            Helpers.Helpers.GetRegDal(ref regDal, "");
            target = ConfigurationRegistryRecord.NewConfigurationRegistryRecord();
            Assert.IsNotNull(target, string.Format("{0}.NewConfigurationRegistryRecord() failed to create object", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

            FormGroup.Form actual;
            actual = target.ELNSearchPermMixtureQueryForm;
            Assert.IsNotNull(actual, string.Format("{0}.ELNSearchPermMixtureQueryForm is null", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

            string expected = "Registry Information";
            Assert.AreEqual<string>(expected, actual.Title, string.Format("{0}.ELNSearchPermMixtureQueryForm did not return expected value", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));
        }

        /// <summary>
        ///A test for ELNSearchTempBaseQueryForm
        ///</summary>
        [TestMethod()]
        public void ELNSearchTempBaseQueryFormTest()
        {
            ConfigurationRegistryRecord target;
            //create RegAdminOracleDAL object to avoid exceptions while running default constructor
            RegAdminOracleDAL regDal = null;
            Helpers.Helpers.GetRegDal(ref regDal, "");
            target = ConfigurationRegistryRecord.NewConfigurationRegistryRecord();
            Assert.IsNotNull(target, string.Format("{0}.NewConfigurationRegistryRecord() failed to create object", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

            FormGroup.Form actual;
            actual = target.ELNSearchTempBaseQueryForm;
            Assert.IsNotNull(actual, string.Format("{0}.ELNSearchTempBaseQueryForm is null", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

            string expected = "Temporary Information";
            Assert.AreEqual<string>(expected, actual.Title, string.Format("{0}.ELNSearchTempBaseQueryForm did not return expected value", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));
        }

        /// <summary>
        ///A test for ELNSearchTempChildQueryForm
        ///</summary>
        [TestMethod()]
        public void ELNSearchTempChildQueryFormTest()
        {
            ConfigurationRegistryRecord target;
            //create RegAdminOracleDAL object to avoid exceptions while running default constructor
            RegAdminOracleDAL regDal = null;
            Helpers.Helpers.GetRegDal(ref regDal, "");
            target = ConfigurationRegistryRecord.NewConfigurationRegistryRecord();
            Assert.IsNotNull(target, string.Format("{0}.NewConfigurationRegistryRecord() failed to create object", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

            FormGroup.Form actual;
            actual = target.ELNSearchTempChildQueryForm;
            Assert.IsNotNull(actual, string.Format("{0}.ELNSearchTempChildQueryForm is null", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

            string expected = "Component Information";
            Assert.AreEqual<string>(expected, actual.Title, string.Format("{0}.ELNSearchTempChildQueryForm did not return expected value", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));
        }

        /// <summary>
        ///A test for ELNSearchTempDetailsBaseForm
        ///</summary>
        [TestMethod()]
        public void ELNSearchTempDetailsBaseFormTest()
        {
            ConfigurationRegistryRecord target;
            //create RegAdminOracleDAL object to avoid exceptions while running default constructor
            RegAdminOracleDAL regDal = null;
            Helpers.Helpers.GetRegDal(ref regDal, "");
            target = ConfigurationRegistryRecord.NewConfigurationRegistryRecord();
            Assert.IsNotNull(target, string.Format("{0}.NewConfigurationRegistryRecord() failed to create object", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

            FormGroup.Form actual;
            actual = target.ELNSearchTempDetailsBaseForm;
            Assert.IsNotNull(actual, string.Format("{0}.ELNSearchTempDetailsBaseForm is null", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

            string expected = "";
            Assert.AreEqual<string>(expected, actual.Title, string.Format("{0}.ELNSearchTempDetailsBaseForm did not return expected value", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));
        }

        /// <summary>
        ///A test for ELNSearchTempDetailsChildForm
        ///</summary>
        [TestMethod()]
        public void ELNSearchTempDetailsChildFormTest()
        {
            ConfigurationRegistryRecord target;
            //create RegAdminOracleDAL object to avoid exceptions while running default constructor
            RegAdminOracleDAL regDal = null;
            Helpers.Helpers.GetRegDal(ref regDal, "");
            target = ConfigurationRegistryRecord.NewConfigurationRegistryRecord();
            Assert.IsNotNull(target, string.Format("{0}.NewConfigurationRegistryRecord() failed to create object", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

            FormGroup.Form actual;
            actual = target.ELNSearchTempDetailsChildForm;
            Assert.IsNotNull(actual, string.Format("{0}.ELNSearchTempDetailsChildForm is null", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

            string expected = "";
            Assert.AreEqual<string>(expected, actual.Title, string.Format("{0}.ELNSearchTempDetailsChildForm did not return expected value", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));
        }

        /// <summary>
        ///A test for ELNSearchTempListForm
        ///</summary>
        [TestMethod()]
        public void ELNSearchTempListFormTest()
        {
            ConfigurationRegistryRecord target;
            //create RegAdminOracleDAL object to avoid exceptions while running default constructor
            RegAdminOracleDAL regDal = null;
            Helpers.Helpers.GetRegDal(ref regDal, "");
            target = ConfigurationRegistryRecord.NewConfigurationRegistryRecord();
            Assert.IsNotNull(target, string.Format("{0}.NewConfigurationRegistryRecord() failed to create object", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

            FormGroup.Form actual;
            actual = target.ELNSearchTempListForm;
            Assert.IsNotNull(actual, string.Format("{0}.ELNSearchTempListForm is null", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

            string expected = null;
            Assert.AreEqual<string>(expected, actual.Title, string.Format("{0}.ELNSearchTempListForm did not return expected value", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));
        }

        /// <summary>
        ///A test for ELNSerachPermanentListForm
        ///</summary>
        [TestMethod()]
        public void ELNSerachPermanentListFormTest()
        {
            ConfigurationRegistryRecord target;
            //create RegAdminOracleDAL object to avoid exceptions while running default constructor
            RegAdminOracleDAL regDal = null;
            Helpers.Helpers.GetRegDal(ref regDal, "");
            target = ConfigurationRegistryRecord.NewConfigurationRegistryRecord();
            Assert.IsNotNull(target, string.Format("{0}.NewConfigurationRegistryRecord() failed to create object", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

            FormGroup.Form actual;
            actual = target.ELNSerachPermanentListForm;
            Assert.IsNotNull(actual, string.Format("{0}.ELNSerachPermanentListForm is null", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

            string expected = null;
            Assert.AreEqual<string>(expected, actual.Title, string.Format("{0}.ELNSerachPermanentListForm did not return expected value", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));
        }

        /// <summary>
        ///A test for ELNSerachPermanentListForm
        ///</summary>
        [TestMethod()]
        public void DataLoaderFormTest()
        {
            ConfigurationRegistryRecord target;
            //create RegAdminOracleDAL object to avoid exceptions while running default constructor
            RegAdminOracleDAL regDal = null;
            Helpers.Helpers.GetRegDal(ref regDal, "");
            target = ConfigurationRegistryRecord.NewConfigurationRegistryRecord();
            Assert.IsNotNull(target, string.Format("{0}.NewConfigurationRegistryRecord() failed to create object", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

            FormGroup.Form actual;
            actual = target.DataLoaderForm;
            Assert.IsNotNull(actual, string.Format("{0}.DataLoaderForm is null", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

            string expected = "Registry Information";
            Assert.AreEqual<string>(expected, actual.Title, string.Format("{0}.DataLoaderForm did not return expected value", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));
        }

        /// <summary>
        ///A test for GetAssemblyList
        ///</summary>
        [TestMethod()]
        public void GetAssemblyListTest()
        {
            ConfigurationRegistryRecord target;
            //create RegAdminOracleDAL object to avoid exceptions while running default constructor
            RegAdminOracleDAL regDal = null;
            Helpers.Helpers.GetRegDal(ref regDal, "");
            target = ConfigurationRegistryRecord.NewConfigurationRegistryRecord();
            Assert.IsNotNull(target, string.Format("{0}.NewConfigurationRegistryRecord() failed to create object", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

            AssemblyList actual;
            actual = target.GetAssemblyList;
            Assert.IsNotNull(actual, string.Format("{0}.GetAssemblyList is null", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));
        }

        /// <summary>
        ///A test for GetSaveErrorMessage
        ///</summary>
        [TestMethod()]
        public void GetSaveErrorMessageTest()
        {
            ConfigurationRegistryRecord target;
            //create RegAdminOracleDAL object to avoid exceptions while running default constructor
            RegAdminOracleDAL regDal = null;
            Helpers.Helpers.GetRegDal(ref regDal, "");
            target = ConfigurationRegistryRecord.NewConfigurationRegistryRecord();
            Assert.IsNotNull(target, string.Format("{0}.NewConfigurationRegistryRecord() failed to create object", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

            string expected = string.Empty;
            string actual;
            actual = target.GetSaveErrorMessage;
            Assert.AreEqual<string>(expected, actual);
        }

        /// <summary>
        ///A test for GetSelectedPropertyList
        ///</summary>
        [TestMethod()]
        public void GetSelectedPropertyList_ByBatchTest()
        {
            ConfigurationRegistryRecord target;
            //create RegAdminOracleDAL object to avoid exceptions while running default constructor
            RegAdminOracleDAL regDal = null;
            Helpers.Helpers.GetRegDal(ref regDal, "");
            target = ConfigurationRegistryRecord.NewConfigurationRegistryRecord();
            Assert.IsNotNull(target, string.Format("{0}.NewConfigurationRegistryRecord() failed to create object", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

            PropertyList expected = null; // TODO: Initialize to an appropriate value
            XmlNode configRegistryRecordNode = configurationRegistryRecordXmlDoc.ChildNodes[1];
            XmlNode propertyListNode = configRegistryRecordNode.SelectSingleNode("/ConfigurationRegistryRecords/ConfigurationRegistryRecord/Batch/PropertyList");

            expected = PropertyList.NewPropertyList(propertyListNode.OuterXml, true);
            Assert.IsNotNull(expected, string.Format("PropertyList.NewPropertyList(propertyListNode.OuterXml, true) failed to create object"));

            PropertyList actual;
            target.SelectedPropertyList = ConfigurationRegistryRecord.PropertyListType.Batch;
            actual = target.GetSelectedPropertyList;
            Assert.IsNotNull(actual, string.Format("{0}.GetSelectedPropertyList did not returned expected value", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));
            Assert.AreEqual<int>(expected.Count, actual.Count, string.Format("expected and actual values do not match", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));
        }

        /// <summary>
        ///A test for GetSelectedPropertyList
        ///</summary>
        [TestMethod()]
        public void GetSelectedPropertyList_ByBatchComponentTest()
        {
            ConfigurationRegistryRecord target;
            //create RegAdminOracleDAL object to avoid exceptions while running default constructor
            RegAdminOracleDAL regDal = null;
            Helpers.Helpers.GetRegDal(ref regDal, "");
            target = ConfigurationRegistryRecord.NewConfigurationRegistryRecord();
            Assert.IsNotNull(target, string.Format("{0}.NewConfigurationRegistryRecord() failed to create object", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

            PropertyList expected = null; // TODO: Initialize to an appropriate value
            XmlNode configRegistryRecordNode = configurationRegistryRecordXmlDoc.ChildNodes[1];
            XmlNode propertyListNode = configRegistryRecordNode.SelectSingleNode("/ConfigurationRegistryRecords/ConfigurationRegistryRecord/BatchComponent/PropertyList");

            expected = PropertyList.NewPropertyList(propertyListNode.OuterXml, true);
            Assert.IsNotNull(expected, string.Format("PropertyList.NewPropertyList(propertyListNode.OuterXml, true) failed to create object"));

            PropertyList actual;
            target.SelectedPropertyList = ConfigurationRegistryRecord.PropertyListType.BatchComponent;
            actual = target.GetSelectedPropertyList;
            Assert.IsNotNull(actual, string.Format("{0}.GetSelectedPropertyList did not returned expected value", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));
            Assert.AreEqual<int>(expected.Count, actual.Count, string.Format("expected and actual values do not match", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));
        }

        /// <summary>
        ///A test for GetSelectedPropertyList
        ///</summary>
        [TestMethod()]
        public void GetSelectedPropertyList_ByCompoundTest()
        {
            ConfigurationRegistryRecord target;
            //create RegAdminOracleDAL object to avoid exceptions while running default constructor
            RegAdminOracleDAL regDal = null;
            Helpers.Helpers.GetRegDal(ref regDal, "");
            target = ConfigurationRegistryRecord.NewConfigurationRegistryRecord();
            Assert.IsNotNull(target, string.Format("{0}.NewConfigurationRegistryRecord() failed to create object", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

            PropertyList expected = null; // TODO: Initialize to an appropriate value
            XmlNode configRegistryRecordNode = configurationRegistryRecordXmlDoc.ChildNodes[1];
            XmlNode propertyListNode = configRegistryRecordNode.SelectSingleNode("/ConfigurationRegistryRecords/ConfigurationRegistryRecord/Compound/PropertyList");

            expected = PropertyList.NewPropertyList(propertyListNode.OuterXml, true);
            Assert.IsNotNull(expected, string.Format("PropertyList.NewPropertyList(propertyListNode.OuterXml, true) failed to create object"));

            PropertyList actual;
            target.SelectedPropertyList = ConfigurationRegistryRecord.PropertyListType.Compound;
            actual = target.GetSelectedPropertyList;
            Assert.IsNotNull(actual, string.Format("{0}.GetSelectedPropertyList did not returned expected value", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));
            Assert.AreEqual<int>(expected.Count, actual.Count, string.Format("expected and actual values do not match", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));
        }

        /// <summary>
        ///A test for GetSelectedPropertyList
        ///</summary>
        [TestMethod()]
        public void GetSelectedPropertyList_ByPropertyListTest()
        {
            ConfigurationRegistryRecord target;
            //create RegAdminOracleDAL object to avoid exceptions while running default constructor
            RegAdminOracleDAL regDal = null;
            Helpers.Helpers.GetRegDal(ref regDal, "");
            target = ConfigurationRegistryRecord.NewConfigurationRegistryRecord();
            Assert.IsNotNull(target, string.Format("{0}.NewConfigurationRegistryRecord() failed to create object", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

            PropertyList expected = null; // TODO: Initialize to an appropriate value
            XmlNode configRegistryRecordNode = configurationRegistryRecordXmlDoc.ChildNodes[1];
            XmlNode propertyListNode = configRegistryRecordNode.SelectSingleNode("/ConfigurationRegistryRecords/ConfigurationRegistryRecord/PropertyList");

            expected = PropertyList.NewPropertyList(propertyListNode.OuterXml, true);
            Assert.IsNotNull(expected, string.Format("PropertyList.NewPropertyList(propertyListNode.OuterXml, true) failed to create object"));

            PropertyList actual;
            target.SelectedPropertyList = ConfigurationRegistryRecord.PropertyListType.PropertyList;
            actual = target.GetSelectedPropertyList;
            Assert.IsNotNull(actual, string.Format("{0}.GetSelectedPropertyList did not returned expected value", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));
            Assert.AreEqual<int>(expected.Count, actual.Count, string.Format("expected and actual values do not match", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));
        }

        /// <summary>
        ///A test for GetSelectedPropertyList
        ///</summary>
        [TestMethod()]
        public void GetSelectedPropertyList_ByStructureTest()
        {
            ConfigurationRegistryRecord target;
            //create RegAdminOracleDAL object to avoid exceptions while running default constructor
            RegAdminOracleDAL regDal = null;
            Helpers.Helpers.GetRegDal(ref regDal, "");
            target = ConfigurationRegistryRecord.NewConfigurationRegistryRecord();
            Assert.IsNotNull(target, string.Format("{0}.NewConfigurationRegistryRecord() failed to create object", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

            PropertyList expected = null; // TODO: Initialize to an appropriate value
            XmlNode configRegistryRecordNode = configurationRegistryRecordXmlDoc.ChildNodes[1];
            XmlNode propertyListNode = configRegistryRecordNode.SelectSingleNode("/ConfigurationRegistryRecords/ConfigurationRegistryRecord/Structure/PropertyList");

            expected = PropertyList.NewPropertyList(propertyListNode.OuterXml, true);
            Assert.IsNotNull(expected, string.Format("PropertyList.NewPropertyList(propertyListNode.OuterXml, true) failed to create object"));

            PropertyList actual;
            target.SelectedPropertyList = ConfigurationRegistryRecord.PropertyListType.Structure;
            actual = target.GetSelectedPropertyList;
            Assert.IsNotNull(actual, string.Format("{0}.GetSelectedPropertyList did not returned expected value", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));
            Assert.AreEqual<int>(expected.Count, actual.Count, string.Format("expected and actual values do not match", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));
        }

        /// <summary>
        ///A test for GetTableNames
        ///</summary>
        [TestMethod()]
        public void GetTableNamesTest()
        {
            ConfigurationRegistryRecord target;
            //create RegAdminOracleDAL object to avoid exceptions while running default constructor
            RegAdminOracleDAL regDal = null;
            Helpers.Helpers.GetRegDal(ref regDal, "");
            target = ConfigurationRegistryRecord.NewConfigurationRegistryRecord();
            Assert.IsNotNull(target, string.Format("{0}.NewConfigurationRegistryRecord() failed to create object", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

            string[] actual;
            actual = target.GetTableNames;
            Assert.IsNotNull(actual, string.Format("{0}.GetTableNames did not returned expected value", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

            XmlNodeList tableNameXmlNodeList = customTableListXmlDoc.ChildNodes[1].ChildNodes;
            string[] expected;
            List<string> tableNamesList = new List<string>();
            foreach (XmlNode tableNameNode in tableNameXmlNodeList)
            {
                if (!tableNamesList.Contains(tableNameNode.Name))
                    tableNamesList.Add(tableNameNode.Name);
            }
            expected = tableNamesList.ToArray();

            Assert.AreEqual<int>(expected.Length, actual.Length, string.Format("expected and actual values do not match", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

        }

        /// <summary>
        ///A test for MixtureForm
        ///</summary>
        [TestMethod()]
        public void MixtureFormTest()
        {
            ConfigurationRegistryRecord target;
            //create RegAdminOracleDAL object to avoid exceptions while running default constructor
            RegAdminOracleDAL regDal = null;
            Helpers.Helpers.GetRegDal(ref regDal, "");
            target = ConfigurationRegistryRecord.NewConfigurationRegistryRecord();
            Assert.IsNotNull(target, string.Format("{0}.NewConfigurationRegistryRecord() failed to create object", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

            FormGroup.Form actual;
            actual = target.MixtureForm;
            Assert.IsNotNull(actual, string.Format("{0}.MixtureForm is null", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

            string expected = null;
            Assert.AreEqual<string>(expected, actual.Title, string.Format("{0}.MixtureForm did not return expected value", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

        }

        /// <summary>
        ///A test for PropertiesLabels
        ///</summary>
        [TestMethod()]
        public void PropertiesLabelsTest()
        {
            ConfigurationRegistryRecord target;
            //create RegAdminOracleDAL object to avoid exceptions while running default constructor
            RegAdminOracleDAL regDal = null;
            Helpers.Helpers.GetRegDal(ref regDal, "");
            target = ConfigurationRegistryRecord.NewConfigurationRegistryRecord();
            Assert.IsNotNull(target, string.Format("{0}.NewConfigurationRegistryRecord() failed to create object", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

            Dictionary<string, string>[] actual;
            actual = target.PropertiesLabels;
            Assert.AreEqual<int>(5, actual.Length);
        }

        /// <summary>
        ///A test for PropertyColumnList
        ///</summary>
        [TestMethod()]
        public void PropertyColumnListTest()
        {
            ConfigurationRegistryRecord target;
            //create RegAdminOracleDAL object to avoid exceptions while running default constructor
            RegAdminOracleDAL regDal = null;
            Helpers.Helpers.GetRegDal(ref regDal, "");
            target = ConfigurationRegistryRecord.NewConfigurationRegistryRecord();
            Assert.IsNotNull(target, string.Format("{0}.NewConfigurationRegistryRecord() failed to create object", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

            List<string> actual;
            actual = target.PropertyColumnList;
            Assert.IsNotNull(actual, string.Format("{0}.PropertyColumnList failed to create object", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

            List<string> expected = Helpers.Helpers.ColumList("Mixtures");
            Assert.IsNotNull(expected, string.Format("Helpers.Helpers.ColumList(Mixtures) failed to create object"));
            Assert.AreEqual<int>(expected.Count, actual.Count, string.Format("method failed to return expected value"));
        }

        /// <summary>
        ///A test for PropertyList
        ///</summary>
        [TestMethod()]
        public void PropertyListTest()
        {
            ConfigurationRegistryRecord target;
            //create RegAdminOracleDAL object to avoid exceptions while running default constructor
            RegAdminOracleDAL regDal = null;
            Helpers.Helpers.GetRegDal(ref regDal, "");
            target = ConfigurationRegistryRecord.NewConfigurationRegistryRecord();
            Assert.IsNotNull(target, string.Format("{0}.NewConfigurationRegistryRecord() failed to create object", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

            PropertyList actual;
            actual = target.PropertyList;
            Assert.IsNotNull(actual, string.Format("{0}.PropertyList failed to create object", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

            XmlNode customPropertyNode = configurationRegistryRecordXmlDoc.ChildNodes[1].SelectSingleNode("/ConfigurationRegistryRecords/ConfigurationRegistryRecord/PropertyList");
            Assert.IsNotNull(customPropertyNode, "Custom property node not exists");
            string expected = customPropertyNode.ChildNodes[0].Attributes["name"].Value;

            Assert.AreEqual<string>(expected, actual[0].Name, "Method did not returned the expected value");
        }

        /// <summary>
        ///A test for RegistryDuplicates
        ///</summary>
        [TestMethod()]
        public void RegistryDuplicatesTest()
        {
            ConfigurationRegistryRecord target;
            //create RegAdminOracleDAL object to avoid exceptions while running default constructor
            RegAdminOracleDAL regDal = null;
            Helpers.Helpers.GetRegDal(ref regDal, "");
            target = ConfigurationRegistryRecord.NewConfigurationRegistryRecord();
            Assert.IsNotNull(target, string.Format("{0}.NewConfigurationRegistryRecord() failed to create object", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

            FormGroup.Form actual;
            actual = target.RegistryDuplicates;
            Assert.IsNotNull(actual, string.Format("{0}.RegistryDuplicates failed to create object", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));
            
            string expected = "Duplicate Registry Information";
            Assert.AreEqual<string>(expected, actual.Title, string.Format("{0}.RegistryDuplicates did not returned expected value", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));
        }

        /// <summary>
        ///A test for SearchPermBatchComponentDetailForm
        ///</summary>
        [TestMethod()]
        public void SearchPermBatchComponentDetailFormTest()
        {
            ConfigurationRegistryRecord target;
            //create RegAdminOracleDAL object to avoid exceptions while running default constructor
            RegAdminOracleDAL regDal = null;
            Helpers.Helpers.GetRegDal(ref regDal, "");
            target = ConfigurationRegistryRecord.NewConfigurationRegistryRecord();
            Assert.IsNotNull(target, string.Format("{0}.NewConfigurationRegistryRecord() failed to create object", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

            FormGroup.Form actual;
            actual = target.SearchPermBatchComponentDetailForm;
            Assert.IsNotNull(actual, string.Format("{0}.SearchPermBatchComponentDetailForm failed to create object", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

            string expected = "Batch Component Information";
            Assert.AreEqual<string>(expected, actual.Title, string.Format("{0}.SearchPermBatchComponentDetailForm did not returned expected value", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

        }

        /// <summary>
        ///A test for SearchPermBatchComponentQueryForm
        ///</summary>
        [TestMethod()]
        public void SearchPermBatchComponentQueryFormTest()
        {
            ConfigurationRegistryRecord target;
            //create RegAdminOracleDAL object to avoid exceptions while running default constructor
            RegAdminOracleDAL regDal = null;
            Helpers.Helpers.GetRegDal(ref regDal, "");
            target = ConfigurationRegistryRecord.NewConfigurationRegistryRecord();
            Assert.IsNotNull(target, string.Format("{0}.NewConfigurationRegistryRecord() failed to create object", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

            FormGroup.Form actual;
            actual = target.SearchPermBatchComponentQueryForm;
            Assert.IsNotNull(actual, string.Format("{0}.SearchPermBatchComponentQueryForm failed to create object", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

            string expected = "Batch Component Information";
            Assert.AreEqual<string>(expected, actual.Title, string.Format("{0}.SearchPermBatchComponentQueryForm did not returned expected value", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));
        }

        /// <summary>
        ///A test for SearchPermBatchDetailForm
        ///</summary>
        [TestMethod()]
        public void SearchPermBatchDetailFormTest()
        {
            ConfigurationRegistryRecord target;
            //create RegAdminOracleDAL object to avoid exceptions while running default constructor
            RegAdminOracleDAL regDal = null;
            Helpers.Helpers.GetRegDal(ref regDal, "");
            target = ConfigurationRegistryRecord.NewConfigurationRegistryRecord();
            Assert.IsNotNull(target, string.Format("{0}.NewConfigurationRegistryRecord() failed to create object", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));
            
            FormGroup.Form actual;
            actual = target.SearchPermBatchDetailForm;
            Assert.IsNotNull(actual, string.Format("{0}.SearchPermBatchDetailForm failed to create object", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

            string expected = "Batch Component Information";
            Assert.AreEqual<string>(expected, actual.Title, string.Format("{0}.SearchPermBatchDetailForm did not returned expected value", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));
        }

        /// <summary>
        ///A test for SearchPermBatchQueryForm
        ///</summary>
        [TestMethod()]
        public void SearchPermBatchQueryFormTest()
        {
            ConfigurationRegistryRecord target;
            //create RegAdminOracleDAL object to avoid exceptions while running default constructor
            RegAdminOracleDAL regDal = null;
            Helpers.Helpers.GetRegDal(ref regDal, "");
            target = ConfigurationRegistryRecord.NewConfigurationRegistryRecord();
            Assert.IsNotNull(target, string.Format("{0}.NewConfigurationRegistryRecord() failed to create object", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

            FormGroup.Form actual;
            actual = target.SearchPermBatchQueryForm;
            Assert.IsNotNull(actual, string.Format("{0}.SearchPermBatchQueryForm failed to create object", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

            string expected = "Batch Information";
            Assert.AreEqual<string>(expected, actual.Title, string.Format("{0}.SearchPermBatchQueryForm did not returned expected value", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

        }

        /// <summary>
        ///A test for SearchPermCompoundDetailForm
        ///</summary>
        [TestMethod()]
        public void SearchPermCompoundDetailFormTest()
        {
            ConfigurationRegistryRecord target;
            //create RegAdminOracleDAL object to avoid exceptions while running default constructor
            RegAdminOracleDAL regDal = null;
            Helpers.Helpers.GetRegDal(ref regDal, "");
            target = ConfigurationRegistryRecord.NewConfigurationRegistryRecord();
            Assert.IsNotNull(target, string.Format("{0}.NewConfigurationRegistryRecord() failed to create object", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

            FormGroup.Form actual;
            actual = target.SearchPermCompoundDetailForm;
            Assert.IsNotNull(actual, string.Format("{0}.SearchPermCompoundDetailForm failed to create object", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

            string expected = "Batch Information";
            Assert.AreEqual<string>(expected, actual.Title, string.Format("{0}.SearchPermCompoundDetailForm did not returned expected value", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

        }

        /// <summary>
        ///A test for SearchPermCompoundQueryForm
        ///</summary>
        [TestMethod()]
        public void SearchPermCompoundQueryFormTest()
        {
            ConfigurationRegistryRecord target;
            //create RegAdminOracleDAL object to avoid exceptions while running default constructor
            RegAdminOracleDAL regDal = null;
            Helpers.Helpers.GetRegDal(ref regDal, "");
            target = ConfigurationRegistryRecord.NewConfigurationRegistryRecord();
            Assert.IsNotNull(target, string.Format("{0}.NewConfigurationRegistryRecord() failed to create object", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

            FormGroup.Form actual;
            actual = target.SearchPermCompoundQueryForm;
            Assert.IsNotNull(actual, string.Format("{0}.SearchPermCompoundQueryForm failed to create object", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

            string expected = "Component Information";
            Assert.AreEqual<string>(expected, actual.Title, string.Format("{0}.SearchPermCompoundQueryForm did not returned expected value", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

        }

        /// <summary>
        ///A test for SearchPermMixtureDetailForm
        ///</summary>
        [TestMethod()]
        public void SearchPermMixtureDetailFormTest()
        {
            ConfigurationRegistryRecord target;
            //create RegAdminOracleDAL object to avoid exceptions while running default constructor
            RegAdminOracleDAL regDal = null;
            Helpers.Helpers.GetRegDal(ref regDal, "");
            target = ConfigurationRegistryRecord.NewConfigurationRegistryRecord();
            Assert.IsNotNull(target, string.Format("{0}.NewConfigurationRegistryRecord() failed to create object", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

            FormGroup.Form actual;
            actual = target.SearchPermMixtureDetailForm;
            Assert.IsNotNull(actual, string.Format("{0}.SearchPermMixtureDetailForm failed to create object", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

            string expected = "Component Information";
            Assert.AreEqual<string>(expected, actual.Title, string.Format("{0}.SearchPermMixtureDetailForm did not returned expected value", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

        }

        /// <summary>
        ///A test for SearchPermMixtureQueryForm
        ///</summary>
        [TestMethod()]
        public void SearchPermMixtureQueryFormTest()
        {
            ConfigurationRegistryRecord target;
            //create RegAdminOracleDAL object to avoid exceptions while running default constructor
            RegAdminOracleDAL regDal = null;
            Helpers.Helpers.GetRegDal(ref regDal, "");
            target = ConfigurationRegistryRecord.NewConfigurationRegistryRecord();
            Assert.IsNotNull(target, string.Format("{0}.NewConfigurationRegistryRecord() failed to create object", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

            FormGroup.Form actual;
            actual = target.SearchPermMixtureQueryForm;
            Assert.IsNotNull(actual, string.Format("{0}.SearchPermMixtureQueryForm failed to create object", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

            string expected = "Registry Information";
            Assert.AreEqual<string>(expected, actual.Title, string.Format("{0}.SearchPermMixtureQueryForm did not returned expected value", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

        }

        /// <summary>
        ///A test for SearchTempBaseQueryForm
        ///</summary>
        [TestMethod()]
        public void SearchTempBaseQueryFormTest()
        {
            ConfigurationRegistryRecord target;
            //create RegAdminOracleDAL object to avoid exceptions while running default constructor
            RegAdminOracleDAL regDal = null;
            Helpers.Helpers.GetRegDal(ref regDal, "");
            target = ConfigurationRegistryRecord.NewConfigurationRegistryRecord();
            Assert.IsNotNull(target, string.Format("{0}.NewConfigurationRegistryRecord() failed to create object", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

            FormGroup.Form actual;
            actual = target.SearchTempBaseQueryForm;
            Assert.IsNotNull(actual, string.Format("{0}.SearchTempBaseQueryForm failed to create object", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

            string expected = "Temporary Information";
            Assert.AreEqual<string>(expected, actual.Title, string.Format("{0}.SearchTempBaseQueryForm did not returned expected value", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

        }

        /// <summary>
        ///A test for SearchTempChildQueryForm
        ///</summary>
        [TestMethod()]
        public void SearchTempChildQueryFormTest()
        {
            ConfigurationRegistryRecord target;
            //create RegAdminOracleDAL object to avoid exceptions while running default constructor
            RegAdminOracleDAL regDal = null;
            Helpers.Helpers.GetRegDal(ref regDal, "");
            target = ConfigurationRegistryRecord.NewConfigurationRegistryRecord();
            Assert.IsNotNull(target, string.Format("{0}.NewConfigurationRegistryRecord() failed to create object", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

            FormGroup.Form actual;
            actual = target.SearchTempChildQueryForm;
            Assert.IsNotNull(actual, string.Format("{0}.SearchTempChildQueryForm failed to create object", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

            string expected = "Component Information";
            Assert.AreEqual<string>(expected, actual.Title, string.Format("{0}.SearchTempChildQueryForm did not returned expected value", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

        }

        /// <summary>
        ///A test for SearchTempDetailsBaseForm
        ///</summary>
        [TestMethod()]
        public void SearchTempDetailsBaseFormTest()
        {
            ConfigurationRegistryRecord target;
            //create RegAdminOracleDAL object to avoid exceptions while running default constructor
            RegAdminOracleDAL regDal = null;
            Helpers.Helpers.GetRegDal(ref regDal, "");
            target = ConfigurationRegistryRecord.NewConfigurationRegistryRecord();
            Assert.IsNotNull(target, string.Format("{0}.NewConfigurationRegistryRecord() failed to create object", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

            FormGroup.Form actual;
            actual = target.SearchTempDetailsBaseForm;
            Assert.IsNotNull(actual, string.Format("{0}.SearchTempDetailsBaseForm failed to create object", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

            string expected = "";
            Assert.AreEqual<string>(expected, actual.Title, string.Format("{0}.SearchTempDetailsBaseForm did not returned expected value", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

        }

        /// <summary>
        ///A test for SearchTempDetailsChildForm
        ///</summary>
        [TestMethod()]
        public void SearchTempDetailsChildFormTest()
        {
            ConfigurationRegistryRecord target;
            //create RegAdminOracleDAL object to avoid exceptions while running default constructor
            RegAdminOracleDAL regDal = null;
            Helpers.Helpers.GetRegDal(ref regDal, "");
            target = ConfigurationRegistryRecord.NewConfigurationRegistryRecord();
            Assert.IsNotNull(target, string.Format("{0}.NewConfigurationRegistryRecord() failed to create object", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

            FormGroup.Form actual;
            actual = target.SearchTempDetailsChildForm;
            Assert.IsNotNull(actual, string.Format("{0}.SearchTempDetailsChildForm failed to create object", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

            string expected = "";
            Assert.AreEqual<string>(expected, actual.Title, string.Format("{0}.SearchTempDetailsChildForm did not returned expected value", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

        }

        /// <summary>
        ///A test for SearchTempListForm
        ///</summary>
        [TestMethod()]
        public void SearchTempListFormTest()
        {
            ConfigurationRegistryRecord target;
            //create RegAdminOracleDAL object to avoid exceptions while running default constructor
            RegAdminOracleDAL regDal = null;
            Helpers.Helpers.GetRegDal(ref regDal, "");
            target = ConfigurationRegistryRecord.NewConfigurationRegistryRecord();
            Assert.IsNotNull(target, string.Format("{0}.NewConfigurationRegistryRecord() failed to create object", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

            FormGroup.Form actual;
            actual = target.SearchTempListForm;
            Assert.IsNotNull(actual, string.Format("{0}.SearchTempListForm failed to create object", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

            string expected = null;
            Assert.AreEqual<string>(expected, actual.Title, string.Format("{0}.SearchTempListForm did not returned expected value", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

        }

        /// <summary>
        ///A test for SelectedPropertyList
        ///</summary>
        [TestMethod()]
        public void SelectedPropertyListTest()
        {
            ConfigurationRegistryRecord target;
            //create RegAdminOracleDAL object to avoid exceptions while running default constructor
            RegAdminOracleDAL regDal = null;
            Helpers.Helpers.GetRegDal(ref regDal, "");
            target = ConfigurationRegistryRecord.NewConfigurationRegistryRecord();
            Assert.IsNotNull(target, string.Format("{0}.NewConfigurationRegistryRecord() failed to create object", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

            ConfigurationRegistryRecord.PropertyListType expected = ConfigurationRegistryRecord.PropertyListType.Batch;
            ConfigurationRegistryRecord.PropertyListType actual;
            target.SelectedPropertyList = ConfigurationRegistryRecord.PropertyListType.Batch;
            actual = target.SelectedPropertyList;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for SelectedPropertyName
        ///</summary>
        [TestMethod()]
        public void SelectedPropertyNameTest()
        {
            ConfigurationRegistryRecord target;
            //create RegAdminOracleDAL object to avoid exceptions while running default constructor
            RegAdminOracleDAL regDal = null;
            Helpers.Helpers.GetRegDal(ref regDal, "");
            target = ConfigurationRegistryRecord.NewConfigurationRegistryRecord();
            Assert.IsNotNull(target, string.Format("{0}.NewConfigurationRegistryRecord() failed to create object", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            target.SelectedPropertyName = expected;
            actual = target.SelectedPropertyName;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for SerachPermanentListForm
        ///</summary>
        [TestMethod()]
        public void SerachPermanentListFormTest()
        {
            ConfigurationRegistryRecord target;
            //create RegAdminOracleDAL object to avoid exceptions while running default constructor
            RegAdminOracleDAL regDal = null;
            Helpers.Helpers.GetRegDal(ref regDal, "");
            target = ConfigurationRegistryRecord.NewConfigurationRegistryRecord();
            Assert.IsNotNull(target, string.Format("{0}.NewConfigurationRegistryRecord() failed to create object", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

            FormGroup.Form actual;
            actual = target.SerachPermanentListForm;
            Assert.IsNotNull(actual, string.Format("{0}.SerachPermanentListForm failed to create object", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

            string expected = null;
            Assert.AreEqual<string>(expected, actual.Title, string.Format("{0}.SerachPermanentListForm did not returned expected value", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

        }

        /// <summary>
        ///A test for StructurePropertyColumnList
        ///</summary>
        [TestMethod()]
        public void StructurePropertyColumnListTest()
        {
            ConfigurationRegistryRecord target;
            //create RegAdminOracleDAL object to avoid exceptions while running default constructor
            RegAdminOracleDAL regDal = null;
            Helpers.Helpers.GetRegDal(ref regDal, "");
            target = ConfigurationRegistryRecord.NewConfigurationRegistryRecord();
            Assert.IsNotNull(target, string.Format("{0}.NewConfigurationRegistryRecord() failed to create object", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

            List<string> actual;
            actual = target.StructurePropertyColumnList;

            List<string> expected = Helpers.Helpers.ColumList("Structures");
            Assert.IsNotNull(expected, string.Format("Helpers.Helpers.ColumList(Structures) failed to create object"));
            Assert.AreEqual<int>(expected.Count, actual.Count, string.Format("method failed to return expected value"));
        }

        /// <summary>
        ///A test for StructurePropertyList
        ///</summary>
        [TestMethod()]
        public void StructurePropertyListTest()
        {
            ConfigurationRegistryRecord target;
            //create RegAdminOracleDAL object to avoid exceptions while running default constructor
            RegAdminOracleDAL regDal = null;
            Helpers.Helpers.GetRegDal(ref regDal, "");
            target = ConfigurationRegistryRecord.NewConfigurationRegistryRecord();
            Assert.IsNotNull(target, string.Format("{0}.NewConfigurationRegistryRecord() failed to create object", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

            PropertyList actual;
            actual = target.StructurePropertyList;

            XmlNodeList structurePropertyNodeList = configurationRegistryRecordXmlDoc.ChildNodes[1].SelectNodes("/ConfigurationRegistryRecords/ConfigurationRegistryRecord/Structure/PropertyList/Property");
            Assert.IsNotNull(structurePropertyNodeList, "Custom structure property list not exists");
            List<string> expectedList = new List<string>();
            foreach (XmlNode structurePropertyNode in structurePropertyNodeList)
            {
                if(!expectedList.Contains(structurePropertyNode.Attributes["name"].Value))
                    expectedList.Add(structurePropertyNode.Attributes["name"].Value);
            }
            Assert.AreEqual<int>(expectedList.Count, actual.Count, string.Format("{0}.StructurePropertyList did not return expected value", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));
        }

        /// <summary>
        ///A test for CleanUpConfiguration
        ///</summary>
        [TestMethod()]
        public void CleanUpConfigurationTest()
        {
            ConfigurationRegistryRecord target;
            //create RegAdminOracleDAL object to avoid exceptions while running default constructor
            RegAdminOracleDAL regDal = null;
            Helpers.Helpers.GetRegDal(ref regDal, "");
            target = ConfigurationRegistryRecord.NewConfigurationRegistryRecord();
            Assert.IsNotNull(target, string.Format("{0}.NewConfigurationRegistryRecord() failed to create object", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

            target.CleanUpConfiguration();
            string expected = string.Empty;
            Assert.AreEqual<string>(expected, target.DefalutValue);
            Assert.AreEqual<string>(expected, target.SelectedPropertyName);

            PrivateObject obj = new PrivateObject(target);
            string propertyStatusLog = (string)obj.GetFieldOrProperty("PropertyStatusLog");
            Assert.AreEqual<string>(expected, propertyStatusLog);
        }

        /// <summary>
        ///A test for UpdateSelf
        ///</summary>
        [TestMethod()]
        public void UpdateSelfTest()
        {
            string xml = string.Empty; // TODO: Initialize to an appropriate value
            XmlNodeList xmlNodeList = XMLDataLoader.PrepareXMLNodeList(configurationRegistryRecordXmlDoc); // TODO: Initialize to an appropriate value
            Assert.IsNotNull(xmlNodeList, "xml node list null");

            XmlNode configurationRegistryRecordXML = xmlNodeList[0];
            xml = configurationRegistryRecordXML.OuterXml;

            ConfigurationRegistryRecord target = ConfigurationRegistryRecord.NewConfigurationRegistryRecord(xml); // TODO: Initialize to an appropriate value
            Assert.IsNotNull(target, string.Format("{0}.NewConfigurationRegistryRecord(xml) failed to create object", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

            //string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            actual = target.UpdateSelf();
            Assert.IsTrue(!string.IsNullOrEmpty(actual));
        }

        /// <summary>
        ///A test for ExportCustomizedProperties
        ///</summary>
        [TestMethod()]
        public void ExportCustomizedPropertiesTest()
        {
            string xml = string.Empty; // TODO: Initialize to an appropriate value
            XmlNodeList xmlNodeList = XMLDataLoader.PrepareXMLNodeList(configurationRegistryRecordXmlDoc); // TODO: Initialize to an appropriate value
            Assert.IsNotNull(xmlNodeList, "xml node list null");

            XmlNode configurationRegistryRecordXML = xmlNodeList[0];
            xml = configurationRegistryRecordXML.OuterXml;

            ConfigurationRegistryRecord target = ConfigurationRegistryRecord.NewConfigurationRegistryRecord(xml); // TODO: Initialize to an appropriate value
            Assert.IsNotNull(target, string.Format("{0}.NewConfigurationRegistryRecord(xml) failed to create object", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

            string expected = xml; // TODO: Initialize to an appropriate value
            string actual;
            actual = target.ExportCustomizedProperties();
            Assert.AreNotEqual<string>(expected, actual);
        }

        /// <summary>
        ///A test for Save
        ///</summary>
        [TestMethod()]
        public void SaveTest()
        {
            ConfigurationRegistryRecord target;
            //create RegAdminOracleDAL object to avoid exceptions while running default constructor
            RegAdminOracleDAL regDal = null;
            Helpers.Helpers.GetRegDal(ref regDal, "");
            target = ConfigurationRegistryRecord.NewConfigurationRegistryRecord();
            Assert.IsNotNull(target, string.Format("{0}.NewConfigurationRegistryRecord() failed to create object", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

            ConfigurationRegistryRecord expected = target; // TODO: Initialize to an appropriate value
            ConfigurationRegistryRecord actual;
            actual = target.Save();
            Assert.AreEqual<ConfigurationRegistryRecord>(expected, actual);
        }

        /// <summary>
        ///A test for ImportDataView
        ///</summary>
        [TestMethod()]
        public void ImportDataViewTest()
        {
            ConfigurationRegistryRecord target;
            //create RegAdminOracleDAL object to avoid exceptions while running default constructor
            RegAdminOracleDAL regDal = null;
            Helpers.Helpers.GetRegDal(ref regDal, "");
            target = ConfigurationRegistryRecord.NewConfigurationRegistryRecord();
            Assert.IsNotNull(target, string.Format("{0}.NewConfigurationRegistryRecord() failed to create object", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));

            XmlDocument dataViewXmlDoc = XMLDataLoader.LoadXmlDocument("DataView.xml");
            Assert.IsNotNull(dataViewXmlDoc, "failed to load DataView.xml");

            Database ds = null;
            Helpers.Helpers.GetDatabase(ref ds, Helpers.Helpers.DbType.COEDB);
            Assert.IsNotNull(ds, "Failed to create database object");

            StringBuilder strSql = new StringBuilder();
            strSql.AppendFormat("SELECT COEDATAVIEW_SEQ.NEXTVAL FROM DUAL"); //MAX(ID) FROM COEDB.COEDATAVIEW"); //
            object nextDVId = ds.ExecuteScalar(CommandType.Text, strSql.ToString());
            Assert.IsNotNull(nextDVId, "failed to retrieve new dataview id from sequence");
            int dataviewId = Convert.ToInt32(nextDVId); // TODO: Initialize to an appropriate value
            //set dataviewid attribute value
            dataViewXmlDoc.ChildNodes[1].Attributes["dataviewid"].Value = dataviewId.ToString();
            string coeDataView = dataViewXmlDoc.ChildNodes[1].OuterXml; // TODO: Initialize to an appropriate value
            target.ImportDataView(coeDataView, dataviewId);

            COEDataViewBO dataviewBO = COEDataViewBO.New();
            dataviewBO = COEDataViewBO.Get(dataviewId);
            Assert.IsNotNull(dataviewBO, "failed to retrive new dataview from database");

            strSql.Length = 0;
            strSql.AppendFormat("DELETE FROM COEDB.COEDATAVIEW WHERE ID={0}", dataviewId);
            int rowsDeleted = ds.ExecuteNonQuery(CommandType.Text, strSql.ToString());
            Assert.AreEqual<int>(1, rowsDeleted, "failed to delete the dataview from database table COEDB.COEDATAVIEW");
        }
        #endregion Completed
    }
}
