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
    /// CompoundTest class contains unit test methods for Compound
    /// </summary>
    [TestClass]
    public class CompoundTest
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
        /// Creating new Compund using XML
        /// </summary>
        [TestMethod]
        public void NewCompound_CreatingNewCompoundUsingXml() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            XmlDocument theXmlDocument = new XmlDocument();
            theXmlDocument.Load(string.Format(@"{0}\{1}", Assembly.GetExecutingAssembly().Location.Substring(0, Assembly.GetExecutingAssembly().Location.IndexOf("TestResults")), GlobalVariables.RegistryRecordXml));
            XmlNode theXmlNode = theXmlDocument.SelectSingleNode("./MultiCompoundRegistryRecord/ComponentList/Component/CompoundList/Compound");

            Compound theCompound = Compound.NewCompound(theXmlNode.OuterXml, true, true);

            Assert.IsNotNull(theCompound, "Compound.NewCompound(theXmlNode.OuterXml, true, true) did not return the expected value.");
        }

        /// <summary>
        /// Loading Compound using Compound ID
        /// </summary>
        [TestMethod]
        public void GetCompound_LoadingCompoundByCompoundID() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            XmlDocument theXmlDocument = new XmlDocument();
            theXmlDocument.Load(string.Format(@"{0}\{1}", Assembly.GetExecutingAssembly().Location.Substring(0, Assembly.GetExecutingAssembly().Location.IndexOf("TestResults")), GlobalVariables.RegistryRecordXml));
            XmlNode theXmlNode = theXmlDocument.SelectSingleNode("./MultiCompoundRegistryRecord/ComponentList/Component/CompoundList/Compound");

            Compound theCompound = Compound.NewCompound(theXmlNode.OuterXml, true, true);

            Compound result = Compound.GetCompound(theCompound.ID);

            Assert.IsNotNull(result, "Compound.GetCompound(theCompound.ID) did not return the expected value.");
        }

        /// <summary>
        /// Adding Identifier to the Compound using Identifier object
        /// </summary>
        [TestMethod]
        public void AddIdentifier_AddingIdentifierToTheCompoundUsingIdentifierObject() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            IdentifierList idList = IdentifierList.GetIdentifierListByType(IdentifierTypeEnum.C, true, true);
            if (idList.Count > 0)
            {
                Identifier theIdentifier = Identifier.NewIdentifier();
                theIdentifier.Active = true;
                theIdentifier.Description = "Test case Identifier";
                theIdentifier.InputText = "Test case Identifier";
                theIdentifier.Name = "TEST_CASE_IDENTIFIER";
                theIdentifier.IdentifierID = idList[0].IdentifierID;

                XmlDocument theXmlDocument = new XmlDocument();
                theXmlDocument.Load(string.Format(@"{0}\{1}", Assembly.GetExecutingAssembly().Location.Substring(0, Assembly.GetExecutingAssembly().Location.IndexOf("TestResults")), GlobalVariables.RegistryRecordXml));
                XmlNode theXmlNode = theXmlDocument.SelectSingleNode("./MultiCompoundRegistryRecord/ComponentList/Component/CompoundList/Compound");

                Compound theCompound = Compound.NewCompound(theXmlNode.OuterXml, true, true);
                int iPrevIdentifierCount = theCompound.IdentifierList.Count;
                theCompound.AddIdentifier(theIdentifier);

                Assert.AreEqual(iPrevIdentifierCount + 1, theCompound.IdentifierList.Count, "Compound.AddIdentifier(theIdentifier) did not return the expected value.");
            }
            else
            {
                Assert.AreEqual(idList.Count > 0, "Identifier list is empty.");
            }
        }

        /// <summary>
        /// Adding Identifier to the Compound using Identifier ID and Value
        /// </summary>
        [TestMethod]
        public void AddIdentifier_AddingIdentifierToTheCompoundUsingIdentifierIDAndValue() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            IdentifierList idList = IdentifierList.GetIdentifierListByType(IdentifierTypeEnum.C, true, true);
            if (idList.Count > 0)
            {
                Identifier theIdentifier = Identifier.NewIdentifier();
                theIdentifier.Active = true;
                theIdentifier.Description = "Test case Identifier";
                theIdentifier.Name = "TEST_CASE_IDENTIFIER";

                XmlDocument theXmlDocument = new XmlDocument();
                theXmlDocument.Load(string.Format(@"{0}\{1}", Assembly.GetExecutingAssembly().Location.Substring(0, Assembly.GetExecutingAssembly().Location.IndexOf("TestResults")), GlobalVariables.RegistryRecordXml));
                XmlNode theXmlNode = theXmlDocument.SelectSingleNode("./MultiCompoundRegistryRecord/ComponentList/Component/CompoundList/Compound");

                Compound theCompound = Compound.NewCompound(theXmlNode.OuterXml, true, true);
                int iPrevIdentifierCount = theCompound.IdentifierList.Count;
                theCompound.AddIdentifier(idList[0].IdentifierID, "Test case Identifier");

                Assert.AreEqual(iPrevIdentifierCount + 1, theCompound.IdentifierList.Count, "Compound.AddIdentifier(idList[0].IdentifierID, 'Test case Identifier') did not return the expected value.");
            }
            else
            {
                Assert.AreEqual(idList.Count > 0, "Identifier list is empty.");
            }
        }

        /// <summary>
        /// Adding Identifier to the Compound using Identifier Name and Value
        /// </summary>
        [TestMethod]
        public void AddIdentifier_AddingIdentifierToTheCompoundUsingIdentifierNameAndValue() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            IdentifierList idList = IdentifierList.GetIdentifierListByType(IdentifierTypeEnum.C, true, true);
            if (idList.Count > 0)
            {
                Identifier theIdentifier = Identifier.NewIdentifier();
                theIdentifier.Active = true;
                theIdentifier.Description = "Test case Identifier";
                theIdentifier.Name = "TEST_CASE_IDENTIFIER";

                XmlDocument theXmlDocument = new XmlDocument();
                theXmlDocument.Load(string.Format(@"{0}\{1}", Assembly.GetExecutingAssembly().Location.Substring(0, Assembly.GetExecutingAssembly().Location.IndexOf("TestResults")), GlobalVariables.RegistryRecordXml));
                XmlNode theXmlNode = theXmlDocument.SelectSingleNode("./MultiCompoundRegistryRecord/ComponentList/Component/CompoundList/Compound");

                Compound theCompound = Compound.NewCompound(theXmlNode.OuterXml, true, true);
                int iPrevIdentifierCount = theCompound.IdentifierList.Count;
                theCompound.AddIdentifier(idList[0].Name, "Test case Identifier");

                Assert.AreEqual(iPrevIdentifierCount + 1, theCompound.IdentifierList.Count, "Compound.AddIdentifier(idList[0].Name, 'Test case Identifier') did not return the expected value.");
            }
            else
            {
                Assert.AreEqual(idList.Count > 0, "Identifier list is empty.");
            }
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
            XmlNode theXmlNode = theXmlDocument.SelectSingleNode("./MultiCompoundRegistryRecord/ComponentList/Component/CompoundList/Compound");

            Compound theCompound = Compound.NewCompound(theXmlNode.OuterXml, true, true);

            bool result = theCompound.IsValid;

            List<BrokenRuleDescription> theBrokenRuleDescription = new List<BrokenRuleDescription>();
            theCompound.GetBrokenRulesDescription(theBrokenRuleDescription);

            Assert.IsNotNull(theBrokenRuleDescription.Count > 0, "Compound.GetBrokenRulesDescription(theBrokenRuleDescription) did not return the expected value.");
        }

        /// <summary>
        /// Updating Compound object from XML
        /// </summary>
        [TestMethod]
        public void UpdateFromXml_UpdatingCompoundObjectFromXml() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            XmlDocument theXmlDocument = new XmlDocument();
            theXmlDocument.Load(string.Format(@"{0}\{1}", Assembly.GetExecutingAssembly().Location.Substring(0, Assembly.GetExecutingAssembly().Location.IndexOf("TestResults")), GlobalVariables.RegistryRecordXml));
            XmlNode theXmlNode = theXmlDocument.SelectSingleNode("./MultiCompoundRegistryRecord/ComponentList/Component/CompoundList/Compound");

            Compound theCompound = Compound.NewCompound(theXmlNode.OuterXml, true, true);

            theCompound.PropertyList["CMP_COMMENTS"].Value = "10";

            PrivateObject thePrivateObject = new PrivateObject(theCompound);
            thePrivateObject.Invoke("UpdateFromXml", new object[] { theXmlNode });

            Assert.AreNotEqual(theCompound.PropertyList["CMP_COMMENTS"].Value, "50", "Compound.UpdateSelf() did not return the expected value.");
        }

        /// <summary>
        /// Generating XML from Compound object
        /// </summary>
        [TestMethod]
        public void UpdateSelf_GeneratingXMLFromCompoundObject() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            XmlDocument theXmlDocument = new XmlDocument();
            theXmlDocument.Load(string.Format(@"{0}\{1}", Assembly.GetExecutingAssembly().Location.Substring(0, Assembly.GetExecutingAssembly().Location.IndexOf("TestResults")), GlobalVariables.RegistryRecordXml));
            XmlNode theXmlNode = theXmlDocument.SelectSingleNode("./MultiCompoundRegistryRecord/ComponentList/Component/CompoundList/Compound");

            Compound theCompound = Compound.NewCompound(theXmlNode.OuterXml, true, true);

            theCompound.DateCreated = DateTime.Now;
            theCompound.PersonCreated = 1;
            theCompound.PersonRegistered = 1;
            theCompound.PersonApproved = 1;
            theCompound.DateLastModified = DateTime.Now;

            PrivateObject thePrivateObject = new PrivateObject(theCompound);
            object theResult = thePrivateObject.Invoke("UpdateSelf", new object[] { true });

            Assert.IsTrue(!string.IsNullOrEmpty(Convert.ToString(theResult)), "Compound.UpdateSelf(true) did not return the expected value.");
        }

        #endregion
    }
}
