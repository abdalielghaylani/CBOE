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
    /// CompoundFragmentTest class contains unit test methods for CompoundFragment
    /// </summary>
    [TestClass]
    public class CompoundFragmentTest
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
        /// Creating new Compound Fragment using Fragment ID
        /// </summary>
        [TestMethod]
        public void NewCompoundFragment_CreatingNewCompoundFragmentUsingFragmentUD() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");
           
            FragmentList theFragmentList = FragmentList.GetFragmentList();
            CompoundFragment theCompoundFragment = CompoundFragment.NewCompoundFragment(theFragmentList[0].FragmentID);

            Assert.IsNotNull(theCompoundFragment, "CompoundFragment.NewCompoundFragment(theFragmentList[0].FragmentID) did not return the expected value.");
        }

        /// <summary>
        /// Creating new Compound Fragment using XML
        /// </summary>
        [TestMethod]
        public void NewCompoundFragment_CreatingNewCompoundFragmentUsingXML() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            XmlDocument theXmlDocument = new XmlDocument();
            theXmlDocument.Load(string.Format(@"{0}\{1}", Assembly.GetExecutingAssembly().Location.Substring(0, Assembly.GetExecutingAssembly().Location.IndexOf("TestResults")), GlobalVariables.RegistryRecordXml));
            XmlNode theXmlNode = theXmlDocument.SelectSingleNode("./MultiCompoundRegistryRecord/ComponentList/Component/CompoundList/Compound/FragmentList/Fragment");

            CompoundFragment theCompoundFragment = CompoundFragment.NewCompoundFragment(theXmlNode.OuterXml, true, true);

            Assert.IsNotNull(theCompoundFragment, "Compound.NewCompound(theXmlNode.OuterXml, true, true) did not return the expected value.");
        }

        /// <summary>
        /// Creating Dirty or old Compound Fragment using XML
        /// </summary>
        [TestMethod]
        public void NewCompoundFragment_CreatingDirtyCompoundFragmentUsingXML() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            XmlDocument theXmlDocument = new XmlDocument();
            theXmlDocument.Load(string.Format(@"{0}\{1}", Assembly.GetExecutingAssembly().Location.Substring(0, Assembly.GetExecutingAssembly().Location.IndexOf("TestResults")), GlobalVariables.RegistryRecordXml));
            XmlNode theXmlNode = theXmlDocument.SelectSingleNode("./MultiCompoundRegistryRecord/ComponentList/Component/CompoundList/Compound/FragmentList/Fragment");

            CompoundFragment theCompoundFragment = CompoundFragment.NewCompoundFragment(theXmlNode.OuterXml, false, false);

            Assert.IsNotNull(theCompoundFragment, "Compound.NewCompound(theXmlNode.OuterXml, false, false) did not return the expected value.");
        }

        #endregion
    }
}
