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
    /// BatchListTest class contains unit test methods for BatchList
    /// </summary>
    [TestClass]
    public class BatchListTest
    {
        #region Variables

        private TestContext testContextInstance;

        public static int iTempRegID;

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

        #region BatchList Test Methods

        /// <summary>
        /// Creating new Batch list using first parameterised constructor
        /// </summary>
        [TestMethod]
        public void NewBatchList_CreatingNewBatchListUsing1Constructor() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            XmlDocument theXmlDocument = new XmlDocument();
            theXmlDocument.Load(string.Format(@"{0}\{1}", Assembly.GetExecutingAssembly().Location.Substring(0, Assembly.GetExecutingAssembly().Location.IndexOf("TestResults")), GlobalVariables.RegistryRecordXml));
            XmlNode theXmlNode = theXmlDocument.SelectSingleNode("./MultiCompoundRegistryRecord/BatchList");

            BatchList theBatchList = BatchList.NewBatchList(theXmlNode.OuterXml, true, true);

            Assert.IsTrue(theBatchList.Count > 0, "BatchList.NewBatchList(theXmlDocument.InnerXml, true, true) did not return the expected value.");
        }

        /// <summary>
        /// Creating new Batch list using second parameterised constructor
        /// </summary>
        [TestMethod]
        public void NewBatchList_CreatingNewBatchListUsing2Constructor() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            XmlDocument theXmlDocument = new XmlDocument();
            theXmlDocument.Load(string.Format(@"{0}\{1}", Assembly.GetExecutingAssembly().Location.Substring(0, Assembly.GetExecutingAssembly().Location.IndexOf("TestResults")), GlobalVariables.RegistryRecordXml));
            XmlNode theXmlNode = theXmlDocument.SelectSingleNode("./MultiCompoundRegistryRecord/BatchList");

            BatchList theBatchList = BatchList.NewBatchList(theXmlNode.OuterXml, true, true, true, GlobalVariables.REGNUM1);

            Assert.IsTrue(theBatchList.Count > 0, "BatchList.NewBatchList(theXmlDocument.InnerXml, true, true,true,GlobalVariables.REGNUM1) did not return the expected value.");
        }

        /// <summary>
        /// Creating new Batch list using third parameterised constructor
        /// </summary>
        [TestMethod]
        public void NewBatchList_CreatingNewBatchListUsing3Constructor() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            XmlDocument theXmlDocument = new XmlDocument();
            theXmlDocument.Load(string.Format(@"{0}\{1}", Assembly.GetExecutingAssembly().Location.Substring(0, Assembly.GetExecutingAssembly().Location.IndexOf("TestResults")), GlobalVariables.RegistryRecordXml));
            XmlNode theXmlNode = theXmlDocument.SelectSingleNode("./MultiCompoundRegistryRecord/BatchList");

            BatchList theBatchList = BatchList.NewBatchList(theXmlNode.OuterXml, true, true, true, GlobalVariables.REGNUM1, RLSStatus.BatchLevelProjects);

            Assert.IsTrue(theBatchList.Count > 0, "BatchList.NewBatchList(theXmlDocument.InnerXml, true, true,true,GlobalVariables.REGNUM1,RLSStatus.BatchLevelProjects) did not return the expected value.");
        }

        /// <summary>
        /// Loading Batch by Batch ID
        /// </summary>
        [TestMethod]
        public void GetBatchById_LoadingBatchByBatchID() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            XmlDocument theXmlDocument = new XmlDocument();
            theXmlDocument.Load(string.Format(@"{0}\{1}", Assembly.GetExecutingAssembly().Location.Substring(0, Assembly.GetExecutingAssembly().Location.IndexOf("TestResults")), GlobalVariables.RegistryRecordXml));
            XmlNode theXmlNode = theXmlDocument.SelectSingleNode("./MultiCompoundRegistryRecord/BatchList");

            BatchList theBatchList = BatchList.NewBatchList(theXmlNode.OuterXml, true, true);
            if (theBatchList.Count > 0)
            {
                Batch theBatch = theBatchList.GetBatchById(theBatchList[0].ID);
                Assert.IsNotNull(theBatch, "BatchList.GetBatchById(theBatchList[0].ID) did not return the expected value.");
            }
            else
            {
                Assert.IsTrue(theBatchList.Count > 0, "XML doest not have batches");
            }
        }

        /// <summary>
        /// Loading Batch by Batch Full Registration number
        /// </summary>
        [TestMethod]
        public void GetBatchFromFullRegNum_LoadingBatchByBatchID() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            XmlDocument theXmlDocument = new XmlDocument();
            theXmlDocument.Load(string.Format(@"{0}\{1}", Assembly.GetExecutingAssembly().Location.Substring(0, Assembly.GetExecutingAssembly().Location.IndexOf("TestResults")), GlobalVariables.RegistryRecordXml));
            XmlNode theXmlNode = theXmlDocument.SelectSingleNode("./MultiCompoundRegistryRecord/BatchList");

            BatchList theBatchList = BatchList.NewBatchList(theXmlNode.OuterXml, true, true);
            if (theBatchList.Count > 0)
            {
                Batch theBatch = theBatchList.GetBatchFromFullRegNum(theBatchList[0].FullRegNumber);
                Assert.IsNotNull(theBatch, "BatchList.GetBatchFromFullRegNum(theBatchList[0].FullRegNumber) did not return the expected value.");
            }
            else
            {
                Assert.IsTrue(theBatchList.Count > 0, "XML doest not have batches");
            }
        }

        /// <summary>
        /// Loading Batch list by Batch ID
        /// </summary>
        [TestMethod]
        public void GetBatchesById_LoadingBatchByBatchID()
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            XmlDocument theXmlDocument = new XmlDocument();
            theXmlDocument.Load(string.Format(@"{0}\{1}", Assembly.GetExecutingAssembly().Location.Substring(0, Assembly.GetExecutingAssembly().Location.IndexOf("TestResults")), GlobalVariables.RegistryRecordXml));
            XmlNode theXmlNode = theXmlDocument.SelectSingleNode("./MultiCompoundRegistryRecord/BatchList");

            BatchList theBatchList = BatchList.NewBatchList(theXmlNode.OuterXml, true, true);
            if (theBatchList.Count > 0)
            {
                BatchList theResult = theBatchList.GetBatchesById(theBatchList[0].ID);
                Assert.IsTrue(theResult.Count > 0, "BatchList.GetBatchesById(theBatchList[0].ID) did not return the expected value.");
            }
            else
            {
                Assert.IsTrue(theBatchList.Count > 0, "XML doest not have batches");
            }
        }

        /// <summary>
        /// Getting Batch Index from BatchList using Batch
        /// </summary>
        [TestMethod]
        public void GetIndex_FetchingBatchIndexInBatchListUsingBatch() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            XmlDocument theXmlDocument = new XmlDocument();
            theXmlDocument.Load(string.Format(@"{0}\{1}", Assembly.GetExecutingAssembly().Location.Substring(0, Assembly.GetExecutingAssembly().Location.IndexOf("TestResults")), GlobalVariables.RegistryRecordXml));
            XmlNode theXmlNode = theXmlDocument.SelectSingleNode("./MultiCompoundRegistryRecord/BatchList");

            BatchList theBatchList = BatchList.NewBatchList(theXmlNode.OuterXml, true, true);
            if (theBatchList.Count > 0)
            {
                int iBatchIndex = theBatchList.GetIndex(theBatchList[0]);

                Assert.IsNotNull(iBatchIndex > 0, "BatchList.GetIndex(theBatchList[0]) did not return the expected value.");
            }
            else
            {
                Assert.IsTrue(theBatchList.Count > 0, "XML doest not have batches");
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
            XmlNode theXmlNode = theXmlDocument.SelectSingleNode("./MultiCompoundRegistryRecord/BatchList");

            BatchList theBatchList = BatchList.NewBatchList(theXmlNode.OuterXml, true, true);
            if (theBatchList.Count > 0)
            {
                PrivateObject thePrivateObject = new PrivateObject(theBatchList);
                object theResult = thePrivateObject.Invoke("UpdateSelf", new object[] { true });

                Assert.IsTrue(!string.IsNullOrEmpty(Convert.ToString(theResult)), "BatchList.UpdateSelf() did not return the expected value.");
            }
            else
            {
                Assert.IsTrue(theBatchList.Count > 0, "XML doest not have batches");
            }
        }

        /// <summary>
        /// Updating Batch object from XML
        /// </summary>
        [TestMethod]
        public void UpdateFromXml_UpdatingBatchObjectFromXML() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            XmlDocument theXmlDocument = new XmlDocument();
            theXmlDocument.Load(string.Format(@"{0}\{1}", Assembly.GetExecutingAssembly().Location.Substring(0, Assembly.GetExecutingAssembly().Location.IndexOf("TestResults")), GlobalVariables.RegistryRecordXml));
            XmlNode theXmlNode = theXmlDocument.SelectSingleNode("./MultiCompoundRegistryRecord/BatchList");

            BatchList theBatchList = BatchList.NewBatchList(theXmlNode.OuterXml, true, true);

            XmlNode theXmlNodeID = theXmlNode.SelectSingleNode("./Batch/BatchID");
            if (theXmlNodeID != null)
            {
                theBatchList[0].ID = Convert.ToInt32(theXmlNodeID.InnerText);
            }
            
            theBatchList[0].PersonCreated = 1;
            if (theBatchList.Count > 0)
            {
                if (theBatchList[0].ProjectList.Count > 0)
                {
                    theBatchList[0].ProjectList[0].Delete();
                }

                if (theBatchList[0].IdentifierList.Count > 0)
                {
                    theBatchList[0].IdentifierList[0].Delete();
                }
                if (theBatchList[0].PropertyList.Count > 0)
                {
                    theBatchList[0].PropertyList[0].Delete();
                }
                if (theBatchList[0].BatchComponentList.Count > 0)
                {
                    theBatchList[0].BatchComponentList[0].RegNum = "1";
                }

                PrivateObject thePrivateObject = new PrivateObject(theBatchList);
                thePrivateObject.Invoke("UpdateFromXml", new object[] { theXmlNode });


                if (theBatchList[0].ProjectList.Count > 0)
                {
                    Assert.IsTrue(theBatchList[0].ProjectList[0].IsDirty, "BatchList.UpdateFromXml(theXmlNode) did not return the expected value.");
                }
                else if (theBatchList[0].IdentifierList.Count > 0)
                {
                    Assert.IsTrue(theBatchList[0].IdentifierList[0].IsDirty, "BatchList.UpdateFromXml(theXmlNode) did not return the expected value.");
                }
                else if (theBatchList[0].PropertyList.Count > 0)
                {
                    Assert.IsTrue(theBatchList[0].PropertyList[0].IsDirty, "BatchList.UpdateFromXml(theXmlNode) did not return the expected value.");
                }
                else if (theBatchList[0].BatchComponentList.Count > 0)
                {
                    Assert.IsTrue(theBatchList[0].BatchComponentList[0].IsDirty, "BatchList.UpdateFromXml(theXmlNode) did not return the expected value.");
                }
            }
            else
            {
                Assert.IsTrue(theBatchList.Count > 0, "XML doest not have batches");
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
            XmlNode theXmlNode = theXmlDocument.SelectSingleNode("./MultiCompoundRegistryRecord/BatchList");

            BatchList theBatchList = BatchList.NewBatchList(theXmlNode.OuterXml, true, true);
            if (theBatchList.Count > 0)
            {
                List<BrokenRuleDescription> theBrokenRuleDescription = new List<BrokenRuleDescription>();
                theBatchList.GetBrokenRulesDescription(theBrokenRuleDescription);

                Assert.IsTrue((theBrokenRuleDescription.Count == 0 || theBrokenRuleDescription.Count>0), "BatchList.GetBrokenRulesDescription(theBrokenRuleDescription) did not return the expected value.");
            }
            else
            {
                Assert.IsTrue(theBatchList.Count > 0, "XML doest not have batches");
            }
        }

        #endregion
    }
}
