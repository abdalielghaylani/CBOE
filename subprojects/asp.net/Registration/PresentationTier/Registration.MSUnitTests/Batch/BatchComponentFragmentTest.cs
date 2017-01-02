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
    /// BatchComponentFragmentTest class contains unit test methods for BatchComponentFragment
    /// </summary>
    [TestClass]
    public class BatchComponentFragmentTest
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
        /// Creating Batch Component Fragments using Default constructor
        /// </summary>
        [TestMethod]
        public void NewBatchComponent_CreatingBatchComponentFragmentUsingDefaultConstructor() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            BatchComponentFragment theBatchComponentFragment = BatchComponentFragment.NewBatchComponentFragment();

            Assert.IsNotNull(theBatchComponentFragment, "BatchComponentFragment.NewBatchComponentFragment() did not return the expected value.");
        }

        /// <summary>
        /// Creating Batch Component Fragments using Fragment object and Equivalent parameters
        /// </summary>
        [TestMethod]
        public void NewBatchComponent_CreatingBatchComponentFragmentUsingFragmentObjectAndEquivalent() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            Fragment theFragment = Fragment.NewFragment(true, true);

            BatchComponentFragment theBatchComponentFragment = BatchComponentFragment.NewBatchComponentFragment(theFragment, 111);

            Assert.IsNotNull(theBatchComponentFragment, "BatchComponentFragment.NewBatchComponentFragment(theFragment, 111) did not return the expected value.");
        }

        /// <summary>
        /// Creating Batch Component Fragments using Fragment ID and Equivalents parameters
        /// </summary>
        [TestMethod]
        public void NewBatchComponent_CreatingBatchComponentFragmentUsingFragmentIDAndEquivalent() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            Fragment theFragment = Fragment.NewFragment(true, true);

            BatchComponentFragment theBatchComponentFragment = BatchComponentFragment.NewBatchComponentFragment(theFragment.FragmentID, 111);

            Assert.IsNotNull(theBatchComponentFragment, "BatchComponentFragment.NewBatchComponentFragment(theFragment.FragmentID, 111) did not return the expected value.");
        }

        /// <summary>
        /// Creating new and clean Batch Component Fragments using Xml, New and Clean parameters
        /// </summary>
        [TestMethod]
        public void NewBatchComponent_CreatingNewAndCleanBatchComponentFragmentUsingXml() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            XmlDocument theXmlDocument = new XmlDocument();
            theXmlDocument.Load(string.Format(@"{0}\{1}", Assembly.GetExecutingAssembly().Location.Substring(0, Assembly.GetExecutingAssembly().Location.IndexOf("TestResults")), GlobalVariables.RegistryRecordXml));
            XmlNode theXmlNode = theXmlDocument.SelectSingleNode("./MultiCompoundRegistryRecord/BatchList/Batch/BatchComponentList/BatchComponent/BatchComponentFragmentList/BatchComponentFragment");

            BatchComponentFragment theBatchComponentFragment = BatchComponentFragment.NewBatchComponentFragment(theXmlNode.OuterXml, true, true);

            Assert.IsNotNull(theBatchComponentFragment, "BatchComponentFragment.NewBatchComponentFragment(theXmlNode.OuterXml, true, true) did not return the expected value.");
        }

        /// <summary>
        /// Creating new and clean Batch Component Fragments using Xml, New and Clean parameters
        /// </summary>
        [TestMethod]
        public void NewBatchComponent_CreatingBatchComponentFragmentUsingXml() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            XmlDocument theXmlDocument = new XmlDocument();
            theXmlDocument.Load(string.Format(@"{0}\{1}", Assembly.GetExecutingAssembly().Location.Substring(0, Assembly.GetExecutingAssembly().Location.IndexOf("TestResults")), GlobalVariables.RegistryRecordXml));
            XmlNode theXmlNode = theXmlDocument.SelectSingleNode("./MultiCompoundRegistryRecord/BatchList/Batch/BatchComponentList/BatchComponent/BatchComponentFragmentList/BatchComponentFragment");

            BatchComponentFragment theBatchComponentFragment = BatchComponentFragment.NewBatchComponentFragment(theXmlNode.OuterXml, false, false);

            Assert.IsNotNull(theBatchComponentFragment, "BatchComponentFragment.NewBatchComponentFragment(theXmlNode.OuterXml, false, false) did not return the expected value.");
        }

        /// <summary>
        /// Updating Batch Component Fragment object from XML
        /// </summary>
        [TestMethod]
        public void UpdateFromXml_UpdatingBatchComponentFragmentObjectFromXml() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            XmlDocument theXmlDocument = new XmlDocument();
            theXmlDocument.Load(string.Format(@"{0}\{1}", Assembly.GetExecutingAssembly().Location.Substring(0, Assembly.GetExecutingAssembly().Location.IndexOf("TestResults")), GlobalVariables.RegistryRecordXml));
            XmlNode theXmlNode = theXmlDocument.SelectSingleNode("./MultiCompoundRegistryRecord/BatchList/Batch/BatchComponentList/BatchComponent/BatchComponentFragmentList/BatchComponentFragment");

            BatchComponentFragment theBatchComponentFragment = BatchComponentFragment.NewBatchComponentFragment(theXmlNode.OuterXml, false, false);

            theBatchComponentFragment.Equivalents = 100;

            PrivateObject thePrivateObject = new PrivateObject(theBatchComponentFragment);
            thePrivateObject.Invoke("UpdateFromXml", new object[] { theXmlNode });

            Assert.AreNotEqual(theBatchComponentFragment.Equivalents, 100, "BatchComponentFragment.UpdateSelf() did not return the expected value.");
        }

        /// <summary>
        /// Getting old value of a property by changing proeprty value
        /// </summary>
        [TestMethod]
        public void GetOldValue_GettingOldValueOfPropertyByChangingIt() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            XmlDocument theXmlDocument = new XmlDocument();
            theXmlDocument.Load(string.Format(@"{0}\{1}", Assembly.GetExecutingAssembly().Location.Substring(0, Assembly.GetExecutingAssembly().Location.IndexOf("TestResults")), GlobalVariables.RegistryRecordXml));
            XmlNode theXmlNode = theXmlDocument.SelectSingleNode("./MultiCompoundRegistryRecord/BatchList/Batch/BatchComponentList/BatchComponent/BatchComponentFragmentList/BatchComponentFragment");

            BatchComponentFragment theBatchComponentFragment = BatchComponentFragment.NewBatchComponentFragment(theXmlNode.OuterXml, false, false);

            float OldEquivalentsValue = theBatchComponentFragment.Equivalents;
            theBatchComponentFragment.Equivalents = 100;
            object theResult = theBatchComponentFragment.GetOldValue("Equivalents");

            Assert.AreEqual(OldEquivalentsValue, theResult, "BatchComponentFragment.GetOldValue('Equivalents') did not return the expected value.");
        }

        /// <summary>
        /// Getting old value of a property without hcanging its value
        /// </summary>
        [TestMethod]
        public void GetOldValue_GettingOldValueOfPropertyByWithoutChangingIt() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            XmlDocument theXmlDocument = new XmlDocument();
            theXmlDocument.Load(string.Format(@"{0}\{1}", Assembly.GetExecutingAssembly().Location.Substring(0, Assembly.GetExecutingAssembly().Location.IndexOf("TestResults")), GlobalVariables.RegistryRecordXml));
            XmlNode theXmlNode = theXmlDocument.SelectSingleNode("./MultiCompoundRegistryRecord/BatchList/Batch/BatchComponentList/BatchComponent/BatchComponentFragmentList/BatchComponentFragment");

            BatchComponentFragment theBatchComponentFragment = BatchComponentFragment.NewBatchComponentFragment(theXmlNode.OuterXml, false, false);

            string strOldEquivalentsValue = theBatchComponentFragment.Equivalents.ToString();
            object theResult = theBatchComponentFragment.GetOldValue("Equivalents");

            Assert.IsNull(theResult, "BatchComponentFragment.GetOldValue('Equivalents') did not return the expected value.");
        }

        #endregion
    }
}
