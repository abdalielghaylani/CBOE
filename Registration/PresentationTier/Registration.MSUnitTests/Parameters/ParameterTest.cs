using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CambridgeSoft.COE.Registration.Services.BLL.Command;
using CambridgeSoft.COE.Registration.Services.Types;
using CambridgeSoft.COE.Registration.Services.BLL;

namespace CambridgeSoft.COE.Registration.MSUnitTests
{
    /// <summary>
    /// ParameterTest class contains unit test methods for Parameter
    /// </summary>
    [TestClass]
    public class ParameterTest
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
        /// Generating update xml with new parameter
        /// </summary>
        [TestMethod]
        public void UpdateSelfConfig_GeneratingUpdateXMLWithNewParam() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            Parameter theParameter = Parameter.NewParameter("TestParameter", "Param Value", true);

            string strResult = theParameter.UpdateSelfConfig(false);

            Assert.IsTrue(!string.IsNullOrEmpty(strResult), "Parameter.UpdateSelfConfig(false) did not save the registry record.");
        }

        /// <summary>
        /// Generating update xml with deleted parameter
        /// </summary>
        [TestMethod]
        public void UpdateSelfConfig_GeneratingUpdateXMLWithDeletedParam() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            Parameter theParameter = Parameter.NewParameter("TestParameter", "Param Value", false);
            theParameter.Delete();
            string strResult = theParameter.UpdateSelfConfig(false);

            Assert.IsTrue(!string.IsNullOrEmpty(strResult), "Parameter.UpdateSelfConfig(false) did not save the registry record.");
        }

        /// <summary>
        /// Generating update xml with no new and deleted parameter
        /// </summary>
        [TestMethod]
        public void UpdateSelfConfig_GeneratingUpdateXMLWithNoNewAndDeletedParam() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            Parameter theParameter = Parameter.NewParameter("TestParameter", "Param Value", false);
            string strResult = theParameter.UpdateSelfConfig(false);

            Assert.IsTrue(!string.IsNullOrEmpty(strResult), "Parameter.UpdateSelfConfig(false) did not save the registry record.");
        }

        /// <summary>
        /// Verifying Name property
        /// </summary>
        [TestMethod]
        public void Name_VerifyingNameProperty() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            Parameter theParameter = Parameter.NewParameter("TestParameter", "Param Value", true);

            string strResult = theParameter.Name;

            Assert.IsTrue(!string.IsNullOrEmpty(strResult), "Parameter.Name did not save the registry record.");
        }

        /// <summary>
        /// Verifying Value property
        /// </summary>
        [TestMethod]
        public void Value_VerifyingValueProperty() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            Parameter theParameter = Parameter.NewParameter("TestParameter", "Param Value", true);

            string strResult = theParameter.Value;

            Assert.IsTrue(!string.IsNullOrEmpty(strResult), "Parameter.Value did not save the registry record.");
        }

        /// <summary>
        /// Fetching new ID value
        /// </summary>
        [TestMethod]
        public void GetIdValue_FetchingNewIDValue() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            Parameter theParameter = Parameter.NewParameter("TestParameter", "Param Value", true);

            PrivateObject thePrivateObject = new PrivateObject(theParameter);
            object theobject = thePrivateObject.Invoke("GetIdValue", new object[] { });

            Assert.IsNotNull(theobject, "Parameter.GetIdValue did not save the registry record.");
        }

        #endregion
    }
}
