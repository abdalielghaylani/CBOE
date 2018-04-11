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
    /// BatchComponentTest class contains unit test methods for BatchComponent
    /// </summary>
    [TestClass]
    public class BatchComponentTest
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

        #region BatchComponent Test Methods

        /// <summary>
        /// Creating Batch Component using Default constructor
        /// </summary>
        [TestMethod]
        public void NewBatchComponent_CreatingBatchComponentUsingDefaultConstructor() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            BatchComponent theBatchComponent = BatchComponent.NewBatchComponent();

            Assert.IsNotNull(theBatchComponent, "BatchComponent.NewBatchComponent() did not return the expected value.");
        }

        /// <summary>
        /// Creating Batch Component using parameterised constructor
        /// </summary>
        [TestMethod]
        public void NewBatchComponent_CreatingNewAndCleanBatchComponentUsing2Constructor() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            XmlDocument theXmlDocument = new XmlDocument();
            theXmlDocument.Load(string.Format(@"{0}\{1}", Assembly.GetExecutingAssembly().Location.Substring(0, Assembly.GetExecutingAssembly().Location.IndexOf("TestResults")), GlobalVariables.RegistryRecordXml));
            XmlNode theXmlNode = theXmlDocument.SelectSingleNode("./MultiCompoundRegistryRecord/BatchList/Batch/BatchComponentList");

            BatchComponent theBatchComponent = BatchComponent.NewBatchComponent(theXmlNode.InnerXml, true, true);

            Assert.IsNotNull(theBatchComponent, "BatchComponent.NewBatchComponent(theXmlNode.InnerXml, true, true) did not return the expected value.");
        }

        /// <summary>
        /// Creating Batch Component using parameterised constructor
        /// </summary>
        [TestMethod]
        public void NewBatchComponent_CreatingBatchComponentUsing2Constructor() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            XmlDocument theXmlDocument = new XmlDocument();
            theXmlDocument.Load(string.Format(@"{0}\{1}", Assembly.GetExecutingAssembly().Location.Substring(0, Assembly.GetExecutingAssembly().Location.IndexOf("TestResults")), GlobalVariables.RegistryRecordXml));
            XmlNode theXmlNode = theXmlDocument.SelectSingleNode("./MultiCompoundRegistryRecord/BatchList/Batch/BatchComponentList");

            BatchComponent theBatchComponent = BatchComponent.NewBatchComponent(theXmlNode.InnerXml, false, false);

            Assert.IsNotNull(theBatchComponent, "BatchComponent.NewBatchComponent(theXmlNode.InnerXml, false, false) did not return the expected value.");
        }

        /// <summary>
        /// Validating Component percentage value
        /// </summary>
        [TestMethod]
        public void AreValidPercentages_ValidatingComponentPercantageValue() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            XmlDocument theXmlDocument = new XmlDocument();
            theXmlDocument.Load(string.Format(@"{0}\{1}", Assembly.GetExecutingAssembly().Location.Substring(0, Assembly.GetExecutingAssembly().Location.IndexOf("TestResults")), GlobalVariables.RegistryRecordXml));
            XmlNode theXmlNode = theXmlDocument.SelectSingleNode("./MultiCompoundRegistryRecord/BatchList/Batch/BatchComponentList");

            BatchComponent theBatchComponent = BatchComponent.NewBatchComponent(theXmlNode.InnerXml, true, true);

            theBatchComponent.PropertyList["PERCENTAGE"].Value = "100";

            PrivateObject thePrivateObject = new PrivateObject(theBatchComponent);
            object theResult = thePrivateObject.Invoke("AreValidPercentages");

            Assert.IsNotNull(theResult, "BatchComponent.AreValidPercentages() did not return the expected value.");
        }

        /// <summary>
        /// Getting failed validation messages
        /// </summary>
        [TestMethod]
        public void GetBrokenRulesDescription_FetchingFailedValidationMessages() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            XmlDocument theXmlDocument = new XmlDocument();
            theXmlDocument.Load(string.Format(@"{0}\{1}", Assembly.GetExecutingAssembly().Location.Substring(0, Assembly.GetExecutingAssembly().Location.IndexOf("TestResults")), GlobalVariables.RegistryRecordXml));
            XmlNode theXmlNode = theXmlDocument.SelectSingleNode("./MultiCompoundRegistryRecord/BatchList/Batch/BatchComponentList");

            BatchComponent theBatchComponent = BatchComponent.NewBatchComponent(theXmlNode.InnerXml, true, true);

            theBatchComponent.PropertyList["PERCENTAGE"].Value = "100";
            
            PrivateObject thePrivateObject = new PrivateObject(theBatchComponent);
            object theResult = thePrivateObject.Invoke("AreValidPercentages");

            List<BrokenRuleDescription> theBrokenRuleDescription = new List<BrokenRuleDescription>();
            theBatchComponent.GetBrokenRulesDescription(theBrokenRuleDescription);

            Assert.IsNotNull(theBrokenRuleDescription.Count == 0, "BatchComponent.GetBrokenRulesDescription(theBrokenRuleDescription) did not return the expected value.");
        }

        /// <summary>
        /// Fetching Bindable properties
        /// </summary>
        [TestMethod]
        public void BindeablePropertiesDT_FetchingBindableProperties() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            XmlDocument theXmlDocument = new XmlDocument();
            theXmlDocument.Load(string.Format(@"{0}\{1}", Assembly.GetExecutingAssembly().Location.Substring(0, Assembly.GetExecutingAssembly().Location.IndexOf("TestResults")), GlobalVariables.RegistryRecordXml));
            XmlNode theXmlNode = theXmlDocument.SelectSingleNode("./MultiCompoundRegistryRecord/BatchList/Batch/BatchComponentList");

            BatchComponent theBatchComponent = BatchComponent.NewBatchComponent(theXmlNode.InnerXml, true, true);

            DataTable dtBindableProperties = theBatchComponent.BindeablePropertiesDT;

            Assert.IsNotNull(dtBindableProperties, "BatchComponent.BindeablePropertiesDT did not return the expected value.");
        }

        /// <summary>
        /// Updating Batch Component object from XML
        /// </summary>
        [TestMethod]
        public void UpdateFromXml_UpdatingBatchComponentObjectFromXml() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            XmlDocument theXmlDocument = new XmlDocument();
            theXmlDocument.Load(string.Format(@"{0}\{1}", Assembly.GetExecutingAssembly().Location.Substring(0, Assembly.GetExecutingAssembly().Location.IndexOf("TestResults")), GlobalVariables.RegistryRecordXml));
            XmlNode theXmlNode = theXmlDocument.SelectSingleNode("./MultiCompoundRegistryRecord/BatchList/Batch/BatchComponentList");

            BatchComponent theBatchComponent = BatchComponent.NewBatchComponent(theXmlNode.InnerXml, true, true);

            theBatchComponent.PropertyList["PERCENTAGE"].Value = "90";

            PrivateObject thePrivateObject = new PrivateObject(theBatchComponent);
            thePrivateObject.Invoke("UpdateFromXml", new object[] { theXmlNode });

            Assert.AreNotEqual(theBatchComponent.PropertyList["PERCENTAGE"].Value, "90", "BatchComponent.UpdateSelf() did not return the expected value.");
        }

        #endregion
    }
}
