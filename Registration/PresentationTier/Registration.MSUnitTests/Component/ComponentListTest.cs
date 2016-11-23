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
    /// ComponentListTest class contains unit test methods for ComponentList
    /// </summary>
    [TestClass]
    public class ComponentListTest
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
        /// Creating new component list
        /// </summary>
        [TestMethod]
        public void NewComponentList_CreatingNewComponentList() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            XmlDocument theXmlDocument = new XmlDocument();
            theXmlDocument.Load(string.Format(@"{0}\{1}", Assembly.GetExecutingAssembly().Location.Substring(0, Assembly.GetExecutingAssembly().Location.IndexOf("TestResults")), GlobalVariables.RegistryRecordXml));
            XmlNode theXmlNode = theXmlDocument.SelectSingleNode("./MultiCompoundRegistryRecord/ComponentList");

            ComponentList theComponentList = ComponentList.NewComponentList(theXmlNode.OuterXml, true, true);

            Assert.IsNotNull(theComponentList, "ComponentList.NewComponentList(theXmlNode.OuterXml, true, true) did not return the expected value.");
        }

        /// <summary>
        /// Creating dirty or old component list
        /// </summary>
        [TestMethod]
        public void NewComponentList_CreatingNewComponentUsingDefaultConstructor() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            XmlDocument theXmlDocument = new XmlDocument();
            theXmlDocument.Load(string.Format(@"{0}\{1}", Assembly.GetExecutingAssembly().Location.Substring(0, Assembly.GetExecutingAssembly().Location.IndexOf("TestResults")), GlobalVariables.RegistryRecordXml));
            XmlNode theXmlNode = theXmlDocument.SelectSingleNode("./MultiCompoundRegistryRecord/ComponentList");

            ComponentList theComponentList = ComponentList.NewComponentList(theXmlNode.OuterXml, false, false);

            Assert.IsNotNull(theComponentList, "ComponentList.NewComponentList(theXmlNode.OuterXml, false, false) did not return the expected value.");
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

            ComponentList theComponentList = ComponentList.NewComponentList(theXmlNode.OuterXml, false, false);

            List<BrokenRuleDescription> theBrokenRuleDescription = new List<BrokenRuleDescription>();
            theComponentList.GetBrokenRulesDescription(theBrokenRuleDescription);

            Assert.IsTrue(theBrokenRuleDescription.Count == 0, "ComponentList.GetBrokenRulesDescription(theBrokenRuleDescription) did not return the expected value.");
        }

        /// <summary>
        /// Getting Batch Component usign Component
        /// </summary>
        [TestMethod]
        public void GetBatchComponentsForComponent_GettingBatchComponentByComponent() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            XmlDocument theXmlDocument = new XmlDocument();
            theXmlDocument.Load(string.Format(@"{0}\{1}", Assembly.GetExecutingAssembly().Location.Substring(0, Assembly.GetExecutingAssembly().Location.IndexOf("TestResults")), GlobalVariables.RegistryRecordXml));
            XmlNode theXmlNode = theXmlDocument.SelectSingleNode("./MultiCompoundRegistryRecord/ComponentList");

            ComponentList theComponentList = ComponentList.NewComponentList(theXmlNode.OuterXml, false, false);

            RegistryRecord theRegistryRecord = RegistryHelper.LoadRegistryRecord(GlobalVariables.REGNUM1, GlobalVariables.RegistryLoadType.REGNUM);
            if (theRegistryRecord == null)
            {
                theRegistryRecord = RegistryHelper.CreateRegistryObject(GlobalVariables.REGNUM1);
                theRegistryRecord = theRegistryRecord.Register(DuplicateAction.None);
            }

            theXmlDocument = new XmlDocument();
            theXmlDocument.Load(string.Format(@"{0}\{1}", Assembly.GetExecutingAssembly().Location.Substring(0, Assembly.GetExecutingAssembly().Location.IndexOf("TestResults")), GlobalVariables.RegistryRecordXml));
            theRegistryRecord.UpdateFromXml(theXmlDocument.InnerXml);
            theRegistryRecord = theRegistryRecord.Save();

            List<BatchComponent> theBatchComponentList = ComponentList.GetBatchComponentsForComponent(theRegistryRecord, theRegistryRecord.ComponentList[0].ComponentIndex);

            Assert.IsTrue(theBatchComponentList.Count > 0, "ComponentList.GetBatchComponentsForComponent(theRegistryRecord, theComponentList[0].ComponentIndex) did not return the expected value.");
        }

        /// <summary>
        /// Getting component by Reg number
        /// </summary>
        [TestMethod]
        public void GetComponentByRegNumber_GettingComponentByRegNumber() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            XmlDocument theXmlDocument = new XmlDocument();
            theXmlDocument.Load(string.Format(@"{0}\{1}", Assembly.GetExecutingAssembly().Location.Substring(0, Assembly.GetExecutingAssembly().Location.IndexOf("TestResults")), GlobalVariables.RegistryRecordXml));
            XmlNode theXmlNode = theXmlDocument.SelectSingleNode("./MultiCompoundRegistryRecord/ComponentList");

            ComponentList theComponentList = ComponentList.NewComponentList(theXmlNode.OuterXml, false, false);
            Component theComponent = theComponentList.GetComponentByRegNumber(theComponentList[0].Compound.RegNumber.RegNum);

            Assert.IsNotNull(theComponent, "ComponentList.GetComponentByRegNumber(theComponentList[0].Compound.RegNumber.RegNum) did not return the expected value.");
        }

        /// <summary>
        /// Getting Compund by Compund ID
        /// </summary>
        [TestMethod]
        public void GetCompoundByID_GettingCompoundByCompoundID() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            XmlDocument theXmlDocument = new XmlDocument();
            theXmlDocument.Load(string.Format(@"{0}\{1}", Assembly.GetExecutingAssembly().Location.Substring(0, Assembly.GetExecutingAssembly().Location.IndexOf("TestResults")), GlobalVariables.RegistryRecordXml));
            XmlNode theXmlNode = theXmlDocument.SelectSingleNode("./MultiCompoundRegistryRecord/ComponentList");

            ComponentList theComponentList = ComponentList.NewComponentList(theXmlNode.OuterXml, false, false);
            Compound theCompound = theComponentList.GetCompoundByID(theComponentList[0].Compound.ID);

            Assert.IsNotNull(theCompound, "ComponentList.GetCompoundByID(theComponentList[0].Compound.ID) did not return the expected value.");
        }

        /// <summary>
        /// Getting Component by Component Index
        /// </summary>
        [TestMethod]
        public void GetComponentByIndex_GettingComponentByIndex() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            XmlDocument theXmlDocument = new XmlDocument();
            theXmlDocument.Load(string.Format(@"{0}\{1}", Assembly.GetExecutingAssembly().Location.Substring(0, Assembly.GetExecutingAssembly().Location.IndexOf("TestResults")), GlobalVariables.RegistryRecordXml));
            XmlNode theXmlNode = theXmlDocument.SelectSingleNode("./MultiCompoundRegistryRecord/ComponentList");

            ComponentList theComponentList = ComponentList.NewComponentList(theXmlNode.OuterXml, false, false);

            PrivateObject thePrivateObject = new PrivateObject(theComponentList);
            object theResult = thePrivateObject.Invoke("GetComponentByIndex", new object[] { theComponentList[0].ComponentIndex });

            Assert.IsNotNull(theResult, "ComponentList.GetComponentByIndex(theComponentList[0].ComponentIndex ) did not return the expected value.");
        }

        /// <summary>
        /// Generating XML from Component List object
        /// </summary>
        [TestMethod]
        public void UpdateSelf_GeneratingXMLFromComponentListObject()
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            XmlDocument theXmlDocument = new XmlDocument();
            theXmlDocument.Load(string.Format(@"{0}\{1}", Assembly.GetExecutingAssembly().Location.Substring(0, Assembly.GetExecutingAssembly().Location.IndexOf("TestResults")), GlobalVariables.RegistryRecordXml));
            XmlNode theXmlNode = theXmlDocument.SelectSingleNode("./MultiCompoundRegistryRecord/ComponentList");

            ComponentList theComponentList = ComponentList.NewComponentList(theXmlNode.OuterXml, false, false);

            PrivateObject thePrivateObject = new PrivateObject(theComponentList);
            object theResult = thePrivateObject.Invoke("UpdateSelf", new object[] { true });

            Assert.IsTrue(!string.IsNullOrEmpty(Convert.ToString(theResult)), "ComponentList.UpdateSelf() did not return the expected value.");
        }


        #endregion
    }
}
