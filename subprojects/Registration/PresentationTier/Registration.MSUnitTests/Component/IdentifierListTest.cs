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
    /// IdentifierListTest class contains unit test methods for IdentifierList
    /// </summary>
    [TestClass]
    public class IdentifierListTest
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

        #region IdentifierList Test Methods

        /// <summary>
        /// Creating new Identifier List 
        /// </summary>
        [TestMethod]
        public void NewIdentifierList_CreatingNewIdentifierList() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            IdentifierList theIdentifierList = IdentifierList.NewIdentifierList();

            Assert.IsNotNull(theIdentifierList, "IdentifierList.NewIdentifierList() did not return the expected value.");
        }

        /// <summary>
        /// Creating new Identifier List using XML
        /// </summary>
        [TestMethod]
        public void NewIdentifierList_CreatingNewIdentifierListUsingXML() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            XmlDocument theXmlDocument = new XmlDocument();
            theXmlDocument.Load(string.Format(@"{0}\{1}", Assembly.GetExecutingAssembly().Location.Substring(0, Assembly.GetExecutingAssembly().Location.IndexOf("TestResults")), GlobalVariables.RegistryRecordXml));
            XmlNode theXmlNode = theXmlDocument.SelectSingleNode("./MultiCompoundRegistryRecord/IdentifierList");

            IdentifierList theIdentifierList = IdentifierList.NewIdentifierList(theXmlNode.OuterXml, true, true);

            Assert.IsTrue(theIdentifierList.Count > 0, "IdentifierList.NewIdentifierList(theXmlNode.OuterXml, true, true) did not return the expected value.");
        }

        /// <summary>
        /// Creating Dirty or old Identifier List using XML
        /// </summary>
        [TestMethod]
        public void NewIdentifierList_CreatingDirtyIdentifierListUsingXML() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            XmlDocument theXmlDocument = new XmlDocument();
            theXmlDocument.Load(string.Format(@"{0}\{1}", Assembly.GetExecutingAssembly().Location.Substring(0, Assembly.GetExecutingAssembly().Location.IndexOf("TestResults")), GlobalVariables.RegistryRecordXml));
            XmlNode theXmlNode = theXmlDocument.SelectSingleNode("./MultiCompoundRegistryRecord/IdentifierList");

            IdentifierList theIdentifierList = IdentifierList.NewIdentifierList(theXmlNode.OuterXml, false, false);

            Assert.IsTrue(theIdentifierList.Count > 0, "IdentifierList.NewIdentifierList(theXmlNode.OuterXml, false, false) did not return the expected value.");
        }

        /// <summary>
        /// Loading Identifier which are applicable for all the modules such as Registry, Compound, BaseFragment etc.
        /// </summary>
        [TestMethod]
        public void GetIdentifierList_LoadingIdentifiersApplicableForAll() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            IdentifierList theIdentifierList = IdentifierList.GetIdentifierList();

            Assert.IsTrue(theIdentifierList.Count > 0, "IdentifierList.GetIdentifierList() did not return the expected value.");
        }

        /// <summary>
        /// Loading Identifiers as per Identifiers type i.e. Identifier which are applicable for all the modules such as Registry, Compound, BaseFragment etc.
        /// </summary>
        [TestMethod]
        public void GetIdentifierListByType_LoadingIdentifiersAsPerIdentifierType_A() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            IdentifierList theIdentifierList = IdentifierList.GetIdentifierListByType(IdentifierTypeEnum.A);

            Assert.IsTrue(theIdentifierList.Count > 0, "IdentifierList.GetIdentifierListByType(IdentifierTypeEnum.A) did not return the expected value.");
        }

        /// <summary>
        /// Loading Identifiers as per Identifiers type i.e. Identifier applicable for Registry
        /// </summary>
        [TestMethod]
        public void GetIdentifierListByType_LoadingIdentifiersAsPerIdentifierType_R() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            IdentifierList theIdentifierList = IdentifierList.GetIdentifierListByType(IdentifierTypeEnum.R);

            Assert.IsTrue(theIdentifierList.Count > 0, "IdentifierList.GetIdentifierListByType(IdentifierTypeEnum.R) did not return the expected value.");
        }

        /// <summary>
        /// Loading Identifiers as per Identifiers type i.e. Identifier applicable for Compound from Cache
        /// </summary>
        [TestMethod]
        public void GetIdentifierListByType_LoadingIdentifiersAsPerIdentifierType_C_AndCacheTrue() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            IdentifierList theIdentifierList = IdentifierList.GetIdentifierListByType(IdentifierTypeEnum.C, true);

            Assert.IsTrue(theIdentifierList.Count > 0, "IdentifierList.GetIdentifierListByType(IdentifierTypeEnum.C, true) did not return the expected value.");
        }

        /// <summary>
        /// Loading Identifiers as per Identifiers type i.e. Identifier applicable for Compound from Database
        /// </summary>
        [TestMethod]
        public void GetIdentifierListByType_LoadingIdentifiersAsPerIdentifierType_C_AndCacheFalse()
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            IdentifierList theIdentifierList = IdentifierList.GetIdentifierListByType(IdentifierTypeEnum.C, false);

            Assert.IsTrue(theIdentifierList.Count > 0, "IdentifierList.GetIdentifierListByType(IdentifierTypeEnum.C, true) did not return the expected value.");
        }


        /// <summary>
        /// Loading Shared Identifiers as per Identifiers type i.e. Identifier applicable for Compound from Database
        /// </summary>
        [TestMethod]
        public void GetIdentifierListByType_LoadingIdentifiersAsPerIdentifierType_C_AndCacheTrueWithSharedIdentifiers() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            IdentifierList theIdentifierList = IdentifierList.GetIdentifierListByType(IdentifierTypeEnum.C, true, true);

            Assert.IsTrue(theIdentifierList.Count > 0, "IdentifierList.GetIdentifierListByType(IdentifierTypeEnum.C, true, true) did not return the expected value.");
        }

        /// <summary>
        /// Loading non-Shared Identifiers as per Identifiers type i.e. Identifier applicable for Compound from Database
        /// </summary>
        [TestMethod]
        public void GetIdentifierListByType_LoadingIdentifiersAsPerIdentifierType_C_AndCacheTrueWithNonSharedIdentifiers() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            IdentifierList theIdentifierList = IdentifierList.GetIdentifierListByType(IdentifierTypeEnum.C, true, false);

            Assert.IsTrue(theIdentifierList.Count > 0, "IdentifierList.GetIdentifierListByType(IdentifierTypeEnum.C, true, false) did not return the expected value.");
        }

        /// <summary>
        /// Loading non-Shared Identifiers as per Identifiers type i.e. Identifier applicable for Compound from Database
        /// </summary>
        [TestMethod]
        public void GetIdentifierListByType_LoadingIdentifiersAsPerIdentifierType_C_InStringFormat_AndCacheTrueWithSharedIdentifiers() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            IdentifierList theIdentifierList = IdentifierList.GetIdentifierListByType(IdentifierTypeEnum.C.ToString(), true, true);

            Assert.IsTrue(theIdentifierList.Count > 0, "IdentifierList.GetIdentifierListByType(IdentifierTypeEnum.C, true, false) did not return the expected value.");
        }

        /// <summary>
        /// Getting Identifier Index from Identifier List
        /// </summary>
        [TestMethod]
        public void GetIndex_GettingIdentifierIndexInIdentifierList() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            IdentifierList theIdentifierList = IdentifierList.GetIdentifierListByType(IdentifierTypeEnum.C.ToString(), true, true);

            if (theIdentifierList.Count > 0)
            {
                int iIdentifierIndex = theIdentifierList.GetIndex(theIdentifierList[0]);

                Assert.IsTrue(iIdentifierIndex > 0, "IdentifierList.GetIndex(theIdentifierList[0]) did not return the expected value.");
            }
            else
            {
                Assert.IsTrue(theIdentifierList.Count == 0, "IdentifierList.GetIdentifierListByType(IdentifierTypeEnum.C, true, false) did not return the expected value.");
            }
        }
        
        /// <summary>
        /// Updating IdentifierList object from XML
        /// </summary>
        [TestMethod]
        public void UpdateFromXml_UpdatingIdentifierListObjectFromXml() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            XmlDocument theXmlDocument = new XmlDocument();
            theXmlDocument.Load(string.Format(@"{0}\{1}", Assembly.GetExecutingAssembly().Location.Substring(0, Assembly.GetExecutingAssembly().Location.IndexOf("TestResults")), GlobalVariables.RegistryRecordXml));
            XmlNode theXmlNode = theXmlDocument.SelectSingleNode("./MultiCompoundRegistryRecord/IdentifierList");

            IdentifierList theIdentifierList = IdentifierList.NewIdentifierList(theXmlNode.OuterXml, true, true);

            theIdentifierList[0].InputText = "InputText Chnaged";

            PrivateObject thePrivateObject = new PrivateObject(theIdentifierList);
            thePrivateObject.Invoke("UpdateFromXml", new object[] { theXmlNode });

            Assert.AreNotEqual(theIdentifierList[0].InputText, "InputText Chnaged", "IdentifierList.UpdateSelf() did not return the expected value.");
        }

        /// <summary>
        /// Generating XML from IdentifierList object
        /// </summary>
        [TestMethod]
        public void UpdateSelf_GeneratingXMLFromIdentifierListObject() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            XmlDocument theXmlDocument = new XmlDocument();
            theXmlDocument.Load(string.Format(@"{0}\{1}", Assembly.GetExecutingAssembly().Location.Substring(0, Assembly.GetExecutingAssembly().Location.IndexOf("TestResults")), GlobalVariables.RegistryRecordXml));
            XmlNode theXmlNode = theXmlDocument.SelectSingleNode("./MultiCompoundRegistryRecord/IdentifierList");

            IdentifierList theIdentifierList = IdentifierList.NewIdentifierList(theXmlNode.OuterXml, true, true);

            PrivateObject thePrivateObject = new PrivateObject(theIdentifierList);
            object theResult = thePrivateObject.Invoke("UpdateSelf", new object[] { true });

            Assert.IsTrue(!string.IsNullOrEmpty(Convert.ToString(theResult)), "IdentifierList.UpdateSelf() did not return the expected value.");
        }

        #endregion
    }
}
