using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CambridgeSoft.COE.Registration.Services.BLL.Command;
using CambridgeSoft.COE.Registration.Services.Types;
using System.Xml;
using System.Reflection;
using System.Data;
using CambridgeSoft.COE.Framework.Common.Validation;

namespace CambridgeSoft.COE.Registration.MSUnitTests
{
    /// <summary>
    /// BatchComponentListTest class contains unit test methods for BatchComponentList
    /// </summary>
    [TestClass]
    public class BatchComponentListTest
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
            Helpers.Authentication.Logon();
        }

        /// <summary>
        /// Use TestCleanup to run code after each test has run 
        /// </summary>
        [TestCleanup()]
        public void MyTestCleanup()
        {
            Helpers.Authentication.Logoff();
        }

        #endregion

        #region BatchComponent Test Methods

        /// <summary>
        /// Creating new and clean Batch Component 
        /// </summary>
        [TestMethod]
        public void NewBatchComponent_CreatingNewAndCleanBatchComponent() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            XmlDocument theXmlDocument = new XmlDocument();
            theXmlDocument.Load(string.Format(@"{0}\{1}", Assembly.GetExecutingAssembly().Location.Substring(0, Assembly.GetExecutingAssembly().Location.IndexOf("TestResults")), GlobalVariables.RegistryRecordXml));
            XmlNode theXmlNode = theXmlDocument.SelectSingleNode("./MultiCompoundRegistryRecord/BatchList/Batch/BatchComponentList");

            BatchComponentList theBatchComponentList = BatchComponentList.NewBatchComponentList(theXmlNode.OuterXml, true, true);

            Assert.IsNotNull(theBatchComponentList, "BatchComponent.NewBatchComponentList(theXmlNode.OuterXml, true, true) did not return the expected value.");
        }

        /// <summary>
        /// Creating Batch Component 
        /// </summary>
        [TestMethod]
        public void NewBatchComponent_CreatingBatchComponent() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            XmlDocument theXmlDocument = new XmlDocument();
            theXmlDocument.Load(string.Format(@"{0}\{1}", Assembly.GetExecutingAssembly().Location.Substring(0, Assembly.GetExecutingAssembly().Location.IndexOf("TestResults")), GlobalVariables.RegistryRecordXml));
            XmlNode theXmlNode = theXmlDocument.SelectSingleNode("./MultiCompoundRegistryRecord/BatchList/Batch/BatchComponentList");

            BatchComponentList theBatchComponentList = BatchComponentList.NewBatchComponentList(theXmlNode.OuterXml, false, false);

            Assert.IsNotNull(theBatchComponentList, "BatchComponent.NewBatchComponentList(theXmlNode.OuterXml, false, false) did not return the expected value.");
        }

        /// <summary>
        /// Getting Batch Component Index from BatchComponentList using Batch Component
        /// </summary>
        [TestMethod]
        public void GetIndex_FetchingBatchComponentIndexInBatchComponentListUsingBatchComponent() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            XmlDocument theXmlDocument = new XmlDocument();
            theXmlDocument.Load(string.Format(@"{0}\{1}", Assembly.GetExecutingAssembly().Location.Substring(0, Assembly.GetExecutingAssembly().Location.IndexOf("TestResults")), GlobalVariables.RegistryRecordXml));
            XmlNode theXmlNode = theXmlDocument.SelectSingleNode("./MultiCompoundRegistryRecord/BatchList/Batch/BatchComponentList");

            BatchComponentList theBatchComponentList = BatchComponentList.NewBatchComponentList(theXmlNode.OuterXml, true, true);
            if (theBatchComponentList.Count > 0)
            {
                int iBatchIndex = theBatchComponentList.GetIndex(theBatchComponentList[0]);

                Assert.IsNotNull(iBatchIndex > 0, "BatchComponentList.GetIndex(theBatchComponentList[0]) did not return the expected value.");
            }
            else
            {
                Assert.IsTrue(theBatchComponentList.Count > 0, "XML doest not have Batch Components");
            }
        }

        /// <summary>
        /// Generating XML from Batch object
        /// </summary>
        [TestMethod]
        public void UpdateSelf_GeneratingXMLFromBatchObject() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            XmlDocument theXmlDocument = new XmlDocument();
            theXmlDocument.Load(string.Format(@"{0}\{1}", Assembly.GetExecutingAssembly().Location.Substring(0, Assembly.GetExecutingAssembly().Location.IndexOf("TestResults")), GlobalVariables.RegistryRecordXml));
            XmlNode theXmlNode = theXmlDocument.SelectSingleNode("./MultiCompoundRegistryRecord/BatchList/Batch/BatchComponentList");

            BatchComponentList theBatchComponentList = BatchComponentList.NewBatchComponentList(theXmlNode.OuterXml, true, true);
            if (theBatchComponentList.Count > 0)
            {
                PrivateObject thePrivateObject = new PrivateObject(theBatchComponentList);
                object theResult = thePrivateObject.Invoke("UpdateSelf", new object[] { true });

                Assert.IsTrue(!string.IsNullOrEmpty(Convert.ToString(theResult)), "BatchComponentList.UpdateSelf() did not return the expected value.");
            }
            else
            {
                Assert.IsTrue(theBatchComponentList.Count > 0, "XML doest not have Batch Components");
            }
        }

        /// <summary>
        /// Updating Batch Component List object from XML
        /// </summary>
        [TestMethod]
        public void UpdateFromXml_UpdatingBatchComponentListObjectFromXML() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            XmlDocument theXmlDocument = new XmlDocument();
            theXmlDocument.Load(string.Format(@"{0}\{1}", Assembly.GetExecutingAssembly().Location.Substring(0, Assembly.GetExecutingAssembly().Location.IndexOf("TestResults")), GlobalVariables.RegistryRecordXml));
            XmlNode theXmlNode = theXmlDocument.SelectSingleNode("./MultiCompoundRegistryRecord/BatchList/Batch/BatchComponentList");

            BatchComponentList theBatchComponentList = BatchComponentList.NewBatchComponentList(theXmlNode.OuterXml, true, true);
            if (theBatchComponentList.Count > 0)
            {
                theBatchComponentList[0].PropertyList["PERCENTAGE"].Value = "90";

                PrivateObject thePrivateObject = new PrivateObject(theBatchComponentList);
                thePrivateObject.Invoke("UpdateFromXml", new object[] { theXmlNode });

                Assert.AreNotEqual(theBatchComponentList[0].PropertyList["PERCENTAGE"].Value, "90", "theBatchComponentList[0].UpdateSelf() did not return the expected value.");
            }
            else
            {
                Assert.IsTrue(theBatchComponentList.Count > 0, "XML doest not have Batch Components");
            }
        }

        /// <summary>
        /// Validating Batch Component
        /// </summary>
        [TestMethod]
        public void CheckBatchComponents_UpdatingBatchComponentListObjectFromXML() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            XmlDocument theXmlDocument = new XmlDocument();
            theXmlDocument.Load(string.Format(@"{0}\{1}", Assembly.GetExecutingAssembly().Location.Substring(0, Assembly.GetExecutingAssembly().Location.IndexOf("TestResults")), GlobalVariables.RegistryRecordXml));
            XmlNode theXmlNode = theXmlDocument.SelectSingleNode("./MultiCompoundRegistryRecord/BatchList/Batch/BatchComponentList");

            BatchComponentList theBatchComponentList = BatchComponentList.NewBatchComponentList(theXmlNode.OuterXml, true, true);
            if (theBatchComponentList.Count > 0)
            {
                theBatchComponentList[0].PropertyList["PERCENTAGE"].Value = "90";

                PrivateObject thePrivateObject = new PrivateObject(theBatchComponentList);
                object theResult = thePrivateObject.Invoke("CheckBatchComponents", new object[] { });

                Assert.IsNotNull(theResult, "theBatchComponentList[0].CheckBatchComponents() did not return the expected value.");
            }
            else
            {
                Assert.IsTrue(theBatchComponentList.Count > 0, "XML doest not have Batch Components");
            }
        }

        /// <summary>
        /// Loading failed validation messages
        /// </summary>
        [TestMethod]
        public void GetBrokenRulesDescription_FetchingFailedValidationMessages() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            XmlDocument theXmlDocument = new XmlDocument();
            theXmlDocument.Load(string.Format(@"{0}\{1}", Assembly.GetExecutingAssembly().Location.Substring(0, Assembly.GetExecutingAssembly().Location.IndexOf("TestResults")), GlobalVariables.RegistryRecordXml));
            XmlNode theXmlNode = theXmlDocument.SelectSingleNode("./MultiCompoundRegistryRecord/BatchList/Batch/BatchComponentList");

            BatchComponentList theBatchComponentList = BatchComponentList.NewBatchComponentList(theXmlNode.OuterXml, true, true);
            if (theBatchComponentList.Count > 0)
            {
                List<BrokenRuleDescription> theBrokenRuleDescription = new List<BrokenRuleDescription>();
                theBatchComponentList.GetBrokenRulesDescription(theBrokenRuleDescription);

                Assert.IsTrue(theBrokenRuleDescription.Count > 0, "BatchComponentList.GetBrokenRulesDescription(theBrokenRuleDescription) did not return the expected value.");
            }
            else
            {
                Assert.IsTrue(theBatchComponentList.Count > 0, "XML doest not have Batch Components");
            }
        }

        /// <summary>
        /// Loading Batch Component by Component ID
        /// </summary>
        [TestMethod]
        public void GetChildById_LoadingBatchComponentByComponentID() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            XmlDocument theXmlDocument = new XmlDocument();
            theXmlDocument.Load(string.Format(@"{0}\{1}", Assembly.GetExecutingAssembly().Location.Substring(0, Assembly.GetExecutingAssembly().Location.IndexOf("TestResults")), GlobalVariables.RegistryRecordXml));
            XmlNode theXmlNode = theXmlDocument.SelectSingleNode("./MultiCompoundRegistryRecord/BatchList/Batch/BatchComponentList");

            BatchComponentList theBatchComponentList = BatchComponentList.NewBatchComponentList(theXmlNode.OuterXml, true, true);
            if (theBatchComponentList.Count > 0)
            {
                theBatchComponentList[0].ID = 101;
                PrivateObject thePrivateObject = new PrivateObject(theBatchComponentList);
                object theResult = thePrivateObject.Invoke("GetChildById", new object[] { theBatchComponentList[0].ID });

                Assert.IsNotNull(theResult, "BatchComponentList.GetChildById(theBatchComponentList[0].ID) did not return the expected value.");
            }
            else
            {
                Assert.IsTrue(theBatchComponentList.Count > 0, "XML doest not have Batch Components");
            }
        }

        #endregion
    }
}
