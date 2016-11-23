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
    /// GetRegNumberListFromHitlistIDTest class contains unit test methods for GetRegNumberListFromHitlistID
    /// </summary>
    [TestClass]
    public class GetRegNumberListFromHitlistIDTest
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

        #region GetRegNumberListFromHitlistID Test Methods

        /// <summary>
        /// Fetching Registry number list by HitList ID
        /// </summary>
        [TestMethod]
        public void CanExecuteCommand_FetchingRegistryNumberListByHitListID() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            StringBuilder cmdString = new StringBuilder();
            cmdString.Append("Select HITLISTID, count(id) From COEDB.COESAVEDHITLIST Where Id in ");
            cmdString.Append("(SELECT  M.MIXTUREID FROM REGDB.VW_MIXTURE M JOIN REGDB.VW_REGISTRYNUMBER RN ON M.REGID = RN.REGID WHERE M.MIXTUREID IN ");
            cmdString.Append("(Select Id From COEDB.COESAVEDHITLIST))");
            cmdString.Append(" group by HITLISTID order by  count(id)");

            DataTable dtHitListIDs = DALHelper.ExecuteQuery(cmdString.ToString());

            if (dtHitListIDs != null && dtHitListIDs.Rows.Count > 0)
            {
                string strResult = string.Empty;
                GetRegNumberListFromHitlistID.HitListID = Convert.ToInt32(Convert.ToString(dtHitListIDs.Rows[0]["HITLISTID"]));
                if (GetRegNumberListFromHitlistID.CanExecuteCommand())
                {
                    strResult = GetRegNumberListFromHitlistID.Execute();
                    Assert.IsTrue(!string.IsNullOrEmpty(strResult), "GetRegNumberListFromHitlistID.Execute() did not return the exepcted value.");
                }
                else
                {
                    Assert.IsTrue(!string.IsNullOrEmpty(strResult), "Command can not be execute.");
                }
            }
            else
            {
                Assert.IsNotNull(dtHitListIDs, "HitList IDs not exist in database.");
            }
        }

        #endregion
    }
}
