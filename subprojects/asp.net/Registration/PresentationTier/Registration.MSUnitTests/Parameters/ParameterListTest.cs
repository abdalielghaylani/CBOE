using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CambridgeSoft.COE.Registration.Services.BLL.Command;
using CambridgeSoft.COE.Registration.Services.Types;
using CambridgeSoft.COE.Registration.Services.BLL;
using System.Xml;

namespace CambridgeSoft.COE.Registration.MSUnitTests
{
    /// <summary>
    /// ParameterListTest class contains unit test methods for ParameterList
    /// </summary>
    [TestClass]
    public class ParameterListTest
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

        #region Parameter Test Methods

        /// <summary>
        /// Generating Parameter list with parameter
        /// </summary>
        [TestMethod]
        public void UpdateSelfConfig_GeneratingParameterListWithParameter() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            StringBuilder theStringBuilder = new StringBuilder();

            theStringBuilder.Append("<params>");
            Parameter theParameter = Parameter.NewParameter("TestParameter1", "Param Value1", true);
            theStringBuilder.Append(theParameter.UpdateSelfConfig(false));
            theParameter = Parameter.NewParameter("TestParameter2", "Param Value2", true);
            theStringBuilder.Append(theParameter.UpdateSelfConfig(false));
            theParameter = Parameter.NewParameter("TestParameter3", "Param Value3", true);
            theStringBuilder.Append(theParameter.UpdateSelfConfig(false));
            theStringBuilder.Append("</params>");

            XmlDocument theXmlDocument = new XmlDocument();
            theXmlDocument.LoadXml(theStringBuilder.ToString());

            ParameterList theParameterList = ParameterList.NewParameterList(theXmlDocument.SelectSingleNode("./params"));
            theParameterList.RemoveAt(1);

            string strResult = theParameterList.UpdateSelfConfig(false);

            Assert.IsTrue(!string.IsNullOrEmpty(strResult), "ParameterList.UpdateSelfConfig(false) did not save the registry record.");
        }

        /// <summary>
        /// Generating Parameter list without parameter
        /// </summary>
        [TestMethod]
        public void UpdateSelfConfig_GeneratingParameterListWithoutParameter() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            ParameterList theParameterList = ParameterList.NewParameterList();

            string strResult = theParameterList.UpdateSelfConfig(false);

            Assert.IsTrue(!string.IsNullOrEmpty(strResult), "ParameterList.UpdateSelfConfig(false) did not save the registry record.");
        }

        #endregion
    }
}
