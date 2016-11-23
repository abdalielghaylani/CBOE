using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CambridgeSoft.COE.Registration.Services.BLL.Command;
using CambridgeSoft.COE.Registration.Services.Types;
using CambridgeSoft.COE.Registration.Services.BLL;
using System.Data;

namespace CambridgeSoft.COE.Registration.MSUnitTests
{
    /// <summary>
    /// GetTempIDCommandTest class contains unit test methods for GetTempIDCommand
    /// </summary>
    [TestClass]
    public class GetTempIDCommandTest
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

        #region GetTempIDCommand Test Methods

        /// <summary>
        /// Generating new Temparay ID
        /// </summary>
        [TestMethod]
        public void CanExecuteCommand_GeneratingNewTempararyID() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            string strResult = string.Empty;
            if (GetTempIDCommand.CanExecuteCommand())
            {
                strResult = GetTempIDCommand.Execute();
                Assert.IsTrue(!string.IsNullOrEmpty(strResult), "GetTempIDCommand.Execute() did not return the exepcted value.");
            }
            else
            {
                Assert.IsTrue(!string.IsNullOrEmpty(strResult), "Command can not be execute.");
            }

        }

        #endregion
    }
}
