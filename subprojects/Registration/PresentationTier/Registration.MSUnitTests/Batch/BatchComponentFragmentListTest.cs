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
    /// BatchComponentFragmentListTest class contains unit test methods for BatchComponentFragmentList
    /// </summary>
    [TestClass]
    public class BatchComponentFragmentListTest
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

        #region BatchComponentFragment Test Methods

        /// <summary>
        /// Creating new and clean Batch Component FragmentList
        /// </summary>
        [TestMethod]
        public void NewBatchComponentFragmentList_CreatingNewAndCleanBatchComponentFragmentList() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            XmlDocument theXmlDocument = new XmlDocument();
            theXmlDocument.Load(string.Format(@"{0}\{1}", Assembly.GetExecutingAssembly().Location.Substring(0, Assembly.GetExecutingAssembly().Location.IndexOf("TestResults")), GlobalVariables.RegistryRecordXml));
            XmlNode theXmlNode = theXmlDocument.SelectSingleNode("./MultiCompoundRegistryRecord/BatchList/Batch/BatchComponentList/BatchComponent/BatchComponentFragmentList");

            BatchComponentFragmentList theBatchComponentFragmentList = BatchComponentFragmentList.NewBatchComponentFragmentList(theXmlNode.OuterXml, true, true);

            Assert.IsNotNull(theBatchComponentFragmentList, "BatchComponentFragmentList.NewBatchComponentFragmentList(theXmlNode.OuterXml, true, true) did not return the expected value.");
        }

        /// <summary>
        /// Creating new and clean Batch Component FragmentList
        /// </summary>
        [TestMethod]
        public void NewBatchComponentFragmentList_CreatingBatchComponentFragmentList() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            XmlDocument theXmlDocument = new XmlDocument();
            theXmlDocument.Load(string.Format(@"{0}\{1}", Assembly.GetExecutingAssembly().Location.Substring(0, Assembly.GetExecutingAssembly().Location.IndexOf("TestResults")), GlobalVariables.RegistryRecordXml));
            XmlNode theXmlNode = theXmlDocument.SelectSingleNode("./MultiCompoundRegistryRecord/BatchList/Batch/BatchComponentList/BatchComponent/BatchComponentFragmentList");

            BatchComponentFragmentList theBatchComponentFragmentList = BatchComponentFragmentList.NewBatchComponentFragmentList(theXmlNode.OuterXml, false, false);

            Assert.IsNotNull(theBatchComponentFragmentList, "BatchComponentFragmentList.NewBatchComponentFragmentList(theXmlNode.OuterXml, true, true) did not return the expected value.");
        }

        /// <summary>
        /// Loading Batch Component Fragment from BatchComponentFragmentList using Framgent ID
        /// </summary>
        [TestMethod]
        public void GetBatchComponentFragmentByFragmentID_FetchingBatchComponentFragmentByFragmentID() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            XmlDocument theXmlDocument = new XmlDocument();
            theXmlDocument.Load(string.Format(@"{0}\{1}", Assembly.GetExecutingAssembly().Location.Substring(0, Assembly.GetExecutingAssembly().Location.IndexOf("TestResults")), GlobalVariables.RegistryRecordXml));
            XmlNode theXmlNode = theXmlDocument.SelectSingleNode("./MultiCompoundRegistryRecord/BatchList/Batch/BatchComponentList/BatchComponent/BatchComponentFragmentList");

            BatchComponentFragmentList theBatchComponentFragmentList = BatchComponentFragmentList.NewBatchComponentFragmentList(theXmlNode.OuterXml, false, false);

            if (theBatchComponentFragmentList.Count > 0)
            {
                BatchComponentFragment theBatchComponentFragment =  theBatchComponentFragmentList.GetBatchComponentFragmentByFragmentID(theBatchComponentFragmentList[0].FragmentID);

                Assert.IsNotNull(theBatchComponentFragment, "theBatchComponentFragmentList.GetBatchComponentFragmentByFragmentID(theBatchComponentFragmentList[0].FragmentID) did not return the expected value.");
            }
            else
            {
                Assert.IsTrue(theBatchComponentFragmentList.Count > 0, "XML doest not have Batch Component Fragments");
            }
            
        }

        /// <summary>
        /// Loading Batch Component Fragment by Fragment ID
        /// </summary>
        [TestMethod]
        public void GetChildById_LoadingBatchComponentFragmentByBatchComponentFragmentID() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            XmlDocument theXmlDocument = new XmlDocument();
            theXmlDocument.Load(string.Format(@"{0}\{1}", Assembly.GetExecutingAssembly().Location.Substring(0, Assembly.GetExecutingAssembly().Location.IndexOf("TestResults")), GlobalVariables.RegistryRecordXml));
            XmlNode theXmlNode = theXmlDocument.SelectSingleNode("./MultiCompoundRegistryRecord/BatchList/Batch/BatchComponentList/BatchComponent/BatchComponentFragmentList");

            BatchComponentFragmentList theBatchComponentFragmentList = BatchComponentFragmentList.NewBatchComponentFragmentList(theXmlNode.OuterXml, false, false);
            if (theBatchComponentFragmentList.Count > 0)
            {
                PrivateObject thePrivateObject = new PrivateObject(theBatchComponentFragmentList);
                object theResult = thePrivateObject.Invoke("GetChildById", new object[] { theBatchComponentFragmentList[0].ID });

                Assert.IsNotNull(theResult, "BatchComponentFragmentList.GetChildById(theBatchComponentFragmentList[0].ID) did not return the expected value.");
            }
            else
            {
                Assert.IsTrue(theBatchComponentFragmentList.Count > 0, "XML doest not have Batch Component Fragments");
            }
        }

        /// <summary>
        /// Getting Batch Component Fragment Index from BatchComponentFragmentList using Batch Component Fragment
        /// </summary>
        [TestMethod]
        public void GetIndex_FetchingBatchComponentFragmentIndexFromBatchComponentFragmentListUsingBatchComponentFragment() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            XmlDocument theXmlDocument = new XmlDocument();
            theXmlDocument.Load(string.Format(@"{0}\{1}", Assembly.GetExecutingAssembly().Location.Substring(0, Assembly.GetExecutingAssembly().Location.IndexOf("TestResults")), GlobalVariables.RegistryRecordXml));
            XmlNode theXmlNode = theXmlDocument.SelectSingleNode("./MultiCompoundRegistryRecord/BatchList/Batch/BatchComponentList/BatchComponent/BatchComponentFragmentList");

            BatchComponentFragmentList theBatchComponentFragmentList = BatchComponentFragmentList.NewBatchComponentFragmentList(theXmlNode.OuterXml, false, false);
            if (theBatchComponentFragmentList.Count > 0)
            {
                int iBatchComponentFragmentIndex = theBatchComponentFragmentList.GetIndex(theBatchComponentFragmentList[0]);

                Assert.IsNotNull(iBatchComponentFragmentIndex > 0, "BatchComponentFragmentList.GetIndex(theBatchComponentFragmentList[0]) did not return the expected value.");
            }
            else
            {
                Assert.IsTrue(theBatchComponentFragmentList.Count > 0, "XML doest not have Batch Component Fragments");
            }
        }

        /// <summary>
        /// Generating XML from Batch Component Fragment object
        /// </summary>
        [TestMethod]
        public void UpdateSelf_GeneratingXMLFromBatchComponentFragmentObject() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            XmlDocument theXmlDocument = new XmlDocument();
            theXmlDocument.Load(string.Format(@"{0}\{1}", Assembly.GetExecutingAssembly().Location.Substring(0, Assembly.GetExecutingAssembly().Location.IndexOf("TestResults")), GlobalVariables.RegistryRecordXml));
            XmlNode theXmlNode = theXmlDocument.SelectSingleNode("./MultiCompoundRegistryRecord/BatchList/Batch/BatchComponentList/BatchComponent/BatchComponentFragmentList");

            BatchComponentFragmentList theBatchComponentFragmentList = BatchComponentFragmentList.NewBatchComponentFragmentList(theXmlNode.OuterXml, false, false);
            if (theBatchComponentFragmentList.Count > 0)
            {
                PrivateObject thePrivateObject = new PrivateObject(theBatchComponentFragmentList);
                object theResult = thePrivateObject.Invoke("UpdateSelf", new object[] { true });

                Assert.IsTrue(!string.IsNullOrEmpty(Convert.ToString(theResult)), "BatchComponentFragmentList.UpdateSelf() did not return the expected value.");
            }
            else
            {
                Assert.IsTrue(theBatchComponentFragmentList.Count > 0, "XML doest not have Batch Component Fragments");
            }
        }

        /// <summary>
        /// Updating Batch Component Fragment List object from XML
        /// </summary>
        [TestMethod]
        public void UpdateFromXml_UpdatingBatchComponentFragmentListObjectFromXML() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            XmlDocument theXmlDocument = new XmlDocument();
            theXmlDocument.Load(string.Format(@"{0}\{1}", Assembly.GetExecutingAssembly().Location.Substring(0, Assembly.GetExecutingAssembly().Location.IndexOf("TestResults")), GlobalVariables.RegistryRecordXml));
            XmlNode theXmlNode = theXmlDocument.SelectSingleNode("./MultiCompoundRegistryRecord/BatchList/Batch/BatchComponentList/BatchComponent/BatchComponentFragmentList");

            BatchComponentFragmentList theBatchComponentFragmentList = BatchComponentFragmentList.NewBatchComponentFragmentList(theXmlNode.OuterXml, false, false);
            if (theBatchComponentFragmentList.Count > 0)
            {
                theBatchComponentFragmentList[0].Equivalents = 100;

                PrivateObject thePrivateObject = new PrivateObject(theBatchComponentFragmentList);
                thePrivateObject.Invoke("UpdateFromXml", new object[] { theXmlNode });

                Assert.AreNotEqual(theBatchComponentFragmentList[0].Equivalents, Convert.ToDecimal(100), "theBatchComponentList[0].UpdateSelf() did not return the expected value.");
            }
            else
            {
                Assert.IsTrue(theBatchComponentFragmentList.Count > 0, "XML doest not have Batch Component Fragments");
            }
        }

        #endregion
    }
}
