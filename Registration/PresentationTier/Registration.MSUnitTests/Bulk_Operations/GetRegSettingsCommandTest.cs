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
using RegistrationWebApp.Code;
using CambridgeSoft.COE.Registration.Services;

namespace CambridgeSoft.COE.Registration.MSUnitTests
{
    /// <summary>
    /// GetRegSettingsCommandTest class contains unit test methods for GetRegSettingsCommand
    /// </summary>
    [TestClass]
    public class GetRegSettingsCommandTest
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

        #region GetRegSettingsCommand Test Methods

        /// <summary>
        /// Loading configuration value from Config file
        /// </summary>
        [TestMethod]
        public void Execute_LoadingConfigurationValueFromConfigFile() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            try
            {
                string strResult = GetRegSettingsCommand.Execute("REGISTRATION", "REGADMIN", "LockingEnabled");

                Assert.IsTrue(!string.IsNullOrEmpty(strResult), "BulkLock.Execute(Convert.ToInt32(Convert.ToString(dtHitListInfo.Rows[0]['HITLISTID']))) did not return the expected value.");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        #endregion
    }
}
