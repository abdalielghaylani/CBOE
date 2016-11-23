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
    /// CompoundListTest class contains unit test methods for CompoundList
    /// </summary>
    [TestClass]
    public class CompoundListTest
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

        #region ComponentList Test Methods

        /// <summary>
        /// Creating new CompundList using Default Constructor
        /// </summary>
        [TestMethod]
        public void NewCompoundList_CreatingNewCompoundListUsingDefaultConstructor() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            CompoundList theCompoundList = CompoundList.NewCompoundList();

            Assert.IsNotNull(theCompoundList, "CompoundList.NewCompoundList() did not return the expected value.");
        }

        /// <summary>
        /// Creating new CompundList using XML
        /// </summary>
        [TestMethod]
        public void NewCompoundList_CreatingNewCompoundListUsingXml() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            XmlDocument theXmlDocument = new XmlDocument();
            theXmlDocument.Load(string.Format(@"{0}\{1}", Assembly.GetExecutingAssembly().Location.Substring(0, Assembly.GetExecutingAssembly().Location.IndexOf("TestResults")), GlobalVariables.RegistryRecordXml));
            XmlNode theXmlNode = theXmlDocument.SelectSingleNode("./MultiCompoundRegistryRecord/ComponentList/Component/CompoundList");

            CompoundList theCompoundList = CompoundList.NewCompoundList(theXmlNode.OuterXml, true, true);

            Assert.IsTrue(theCompoundList.Count > 0, "CompoundList.NewCompoundList(theXmlNode.OuterXml, true, true) did not return the expected value.");
        }

        /// <summary>
        /// Creating Dirty or old CompundList using XML
        /// </summary>
        [TestMethod]
        public void NewCompoundList_CreatingDirtyCompoundListUsingXml() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            XmlDocument theXmlDocument = new XmlDocument();
            theXmlDocument.Load(string.Format(@"{0}\{1}", Assembly.GetExecutingAssembly().Location.Substring(0, Assembly.GetExecutingAssembly().Location.IndexOf("TestResults")), GlobalVariables.RegistryRecordXml));
            XmlNode theXmlNode = theXmlDocument.SelectSingleNode("./MultiCompoundRegistryRecord/ComponentList/Component/CompoundList");

            CompoundList theCompoundList = CompoundList.NewCompoundList(theXmlNode.OuterXml, false, false);

            Assert.IsTrue(theCompoundList.Count > 0, "CompoundList.NewCompoundList(theXmlNode.OuterXml, false, false) did not return the expected value.");
        }

        /// <summary>
        /// Loading Compound List using Registration Number
        /// </summary>
        [TestMethod]
        public void GetCompoundList_LoadingCompoudListUdingRegNumber() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            RegistryRecord theRegistryRecord = RegistryHelper.LoadRegistryRecord(GlobalVariables.REGNUM1, GlobalVariables.RegistryLoadType.REGNUM);

            CompoundList theResult = CompoundList.GetCompoundList(theRegistryRecord.Xml);

            Assert.IsNotNull(theResult, "CompoundList.GetCompoundList(theCompoundList[0].RegNumber.RegNum) did not return the expected value.");
        }

        /// <summary>
        /// Fetching Compound Index in Compound List using Compund object
        /// </summary>
        [TestMethod]
        public void GetIndex_FetchingCompoundIndexInListUsingCompound() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            XmlDocument theXmlDocument = new XmlDocument();
            theXmlDocument.Load(string.Format(@"{0}\{1}", Assembly.GetExecutingAssembly().Location.Substring(0, Assembly.GetExecutingAssembly().Location.IndexOf("TestResults")), GlobalVariables.RegistryRecordXml));
            XmlNode theXmlNode = theXmlDocument.SelectSingleNode("./MultiCompoundRegistryRecord/ComponentList/Component/CompoundList");

            CompoundList theCompoundList = CompoundList.NewCompoundList(theXmlNode.OuterXml, true, true);

            theXmlNode = theXmlDocument.SelectSingleNode("./MultiCompoundRegistryRecord/ComponentList/Component/CompoundList/Compound");
            Compound theCompound = Compound.NewCompound(theXmlNode.OuterXml, true, true);

            int iCompoundIndex = -1;
            iCompoundIndex = theCompoundList.GetIndex(theCompound);

            Assert.IsTrue(iCompoundIndex > -1, "CompoundList.GetIndex(theCompound) did not return the expected value.");
        }

        /// <summary>
        /// Removing Compound from Compound List
        /// </summary>
        [TestMethod]
        public void Remove_RemovingCompoundFromCompoundList() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            XmlDocument theXmlDocument = new XmlDocument();
            theXmlDocument.Load(string.Format(@"{0}\{1}", Assembly.GetExecutingAssembly().Location.Substring(0, Assembly.GetExecutingAssembly().Location.IndexOf("TestResults")), GlobalVariables.RegistryRecordXml));
            XmlNode theXmlNode = theXmlDocument.SelectSingleNode("./MultiCompoundRegistryRecord/ComponentList/Component/CompoundList");

            CompoundList theCompoundList = CompoundList.NewCompoundList(theXmlNode.OuterXml, true, true);
            int iPrevCompoundCOunt = theCompoundList.Count;
            theXmlNode = theXmlDocument.SelectSingleNode("./MultiCompoundRegistryRecord/ComponentList/Component/CompoundList/Compound");
            Compound theCompound = Compound.NewCompound(theXmlNode.OuterXml, true, true);

            theCompoundList.Remove(theCompound);

            Assert.AreNotEqual(iPrevCompoundCOunt, theCompoundList.Count, "CompoundList.Remove(theCompound) did not return the expected value.");
        }

        /// <summary>
        /// Generating XML from Compound List object
        /// </summary>
        [TestMethod]
        public void UpdateSelf_GeneratingXMLFromCompoundListObject() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            XmlDocument theXmlDocument = new XmlDocument();
            theXmlDocument.Load(string.Format(@"{0}\{1}", Assembly.GetExecutingAssembly().Location.Substring(0, Assembly.GetExecutingAssembly().Location.IndexOf("TestResults")), GlobalVariables.RegistryRecordXml));
            XmlNode theXmlNode = theXmlDocument.SelectSingleNode("./MultiCompoundRegistryRecord/ComponentList/Component/CompoundList");

            CompoundList theCompoundList = CompoundList.NewCompoundList(theXmlNode.OuterXml, true, true);

            PrivateObject thePrivateObject = new PrivateObject(theCompoundList);
            object theResult = thePrivateObject.Invoke("UpdateSelf", new object[] { true });

            Assert.IsTrue(!string.IsNullOrEmpty(Convert.ToString(theResult)), "CompoundList.UpdateSelf(true) did not return the expected value.");
        }
       

        #endregion
    }
}
