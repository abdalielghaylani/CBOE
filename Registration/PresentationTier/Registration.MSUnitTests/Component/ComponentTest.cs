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
    /// ComponentTest class contains unit test methods for Component
    /// </summary>
    [TestClass]
    public class ComponentTest
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

        #region Component Test Methods

        /// <summary>
        /// Creating new component using default constructor
        /// </summary>
        [TestMethod]
        public void NewComponent_CreatingNewComponentUsingDefaultConstructor() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            Component theComponent = Component.NewComponent();

            Assert.IsNotNull(theComponent, "Component.NewComponent() did not return the expected value.");
        }

        /// <summary>
        /// Creating new component using 2 constructor
        /// </summary>
        [TestMethod]
        public void NewComponent_CreatingNewComponentUsing2Constructor() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            Component theComponent = Component.NewComponent(true);

            Assert.IsNotNull(theComponent, "Component.NewComponent(true) did not return the expected value.");
        }

        /// <summary>
        /// Creating dirty or old component using 2 constructor
        /// </summary>
        [TestMethod]
        public void NewComponent_CreatingDirtyComponentUsing2Constructor() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            Component theComponent = Component.NewComponent(false);

            Assert.IsNotNull(theComponent, "Component.NewComponent(true) did not return the expected value.");
        }

        /// <summary>
        /// Creating new component using 3 constructor
        /// </summary>
        [TestMethod]
        public void NewComponent_CreatingNewComponentUsing3Constructor() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            XmlDocument theXmlDocument = new XmlDocument();
            theXmlDocument.Load(string.Format(@"{0}\{1}", Assembly.GetExecutingAssembly().Location.Substring(0, Assembly.GetExecutingAssembly().Location.IndexOf("TestResults")), GlobalVariables.RegistryRecordXml));
            XmlNode theXmlNode = theXmlDocument.SelectSingleNode("./MultiCompoundRegistryRecord/ComponentList");

            Component theComponent = Component.NewComponent(theXmlNode.InnerXml, true, true);

            Assert.IsNotNull(theComponent, "Component.NewComponent(theXmlNode.InnerXml, true, true) did not return the expected value.");
        }

        /// <summary>
        /// Creating dirty or old component using 3 constructor
        /// </summary>
        [TestMethod]
        public void NewComponent_CreatingDirtyComponentUsing3tConstructor() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            XmlDocument theXmlDocument = new XmlDocument();
            theXmlDocument.Load(string.Format(@"{0}\{1}", Assembly.GetExecutingAssembly().Location.Substring(0, Assembly.GetExecutingAssembly().Location.IndexOf("TestResults")), GlobalVariables.RegistryRecordXml));
            XmlNode theXmlNode = theXmlDocument.SelectSingleNode("./MultiCompoundRegistryRecord/ComponentList");

            Component theComponent = Component.NewComponent(theXmlNode.InnerXml, false, false);

            Assert.IsNotNull(theComponent, "Component.NewComponent(theXmlNode.InnerXml, false, false) did not return the expected value.");
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
            XmlNode theXmlNode = theXmlDocument.SelectSingleNode("./MultiCompoundRegistryRecord/ComponentList");

            Component theComponent = Component.NewComponent(theXmlNode.InnerXml, true, true);

            PrivateObject thePrivateObject = new PrivateObject(theComponent);
            object theResult = thePrivateObject.GetFieldOrProperty("IsValid");

            List<BrokenRuleDescription> theBrokenRuleDescription = new List<BrokenRuleDescription>();
            theComponent.GetBrokenRulesDescription(theBrokenRuleDescription);

            Assert.IsNotNull(theBrokenRuleDescription.Count == 0, "Component.GetBrokenRulesDescription(theBrokenRuleDescription) did not return the expected value.");
        }

        /// <summary>
        /// Updating Component object from XML
        /// </summary>
        [TestMethod]
        public void UpdateFromXml_UpdatingComponentObjectFromXml() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            XmlDocument theXmlDocument = new XmlDocument();
            theXmlDocument.Load(string.Format(@"{0}\{1}", Assembly.GetExecutingAssembly().Location.Substring(0, Assembly.GetExecutingAssembly().Location.IndexOf("TestResults")), GlobalVariables.RegistryRecordXml));
            XmlNode theXmlNode = theXmlDocument.SelectSingleNode("./MultiCompoundRegistryRecord/ComponentList/Component/CompoundList");

            Component theComponent = Component.NewComponent(theXmlNode.InnerXml, true, true);

            theComponent.Percentage = 100;

            PrivateObject thePrivateObject = new PrivateObject(theComponent);
            thePrivateObject.Invoke("UpdateFromXml", new object[] { theXmlNode });

            Assert.AreNotEqual(theComponent.Percentage, 100, "Component.UpdateSelf() did not return the expected value.");
        }

        /// <summary>
        /// Generating XML from Component object
        /// </summary>
        [TestMethod]
        public void UpdateSelf_GeneratingXMLFromComponentObject() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            XmlDocument theXmlDocument = new XmlDocument();
            theXmlDocument.Load(string.Format(@"{0}\{1}", Assembly.GetExecutingAssembly().Location.Substring(0, Assembly.GetExecutingAssembly().Location.IndexOf("TestResults")), GlobalVariables.RegistryRecordXml));
            XmlNode theXmlNode = theXmlDocument.SelectSingleNode("./MultiCompoundRegistryRecord/ComponentList");

            Component theComponent = Component.NewComponent(theXmlNode.InnerXml, true, true);

            theComponent.ComponentIndex = 0;

            PrivateObject thePrivateObject = new PrivateObject(theComponent);
            object theResult = thePrivateObject.Invoke("UpdateSelf", new object[] { true });

            Assert.IsTrue(!string.IsNullOrEmpty(Convert.ToString(theResult)), "Component.UpdateSelf() did not return the expected value.");
        }

        #endregion
    }
}
