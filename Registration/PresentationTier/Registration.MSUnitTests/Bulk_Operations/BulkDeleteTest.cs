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
    /// BulkDeleteTest class contains unit test methods for BulkDelete
    /// </summary>
    [TestClass]
    public class BulkDeleteTest
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

        #region BulkDelete Test Methods

        /// <summary>
        /// Deleting Temparary registry record in bulk order
        /// </summary>
        [TestMethod]
        public void Execute_DeletingTempararyRegistryReocrdInBulk() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            StringBuilder cmdString = new StringBuilder();
            cmdString.Append("Select DISTINCT COEDB.COESAVEDHITLIST.HITLISTID From REGDB.vw_temporarybatch, COEDB.COESAVEDHITLIST, COEDB.COESAVEDHITLISTID, REGDB.VW_MIXTURE, REGDB.VW_REGISTRYNUMBER ");
            cmdString.Append("WHERE COEDB.COESAVEDHITLIST.HITLISTID = COEDB.COESAVEDHITLISTID.ID AND ");
            cmdString.Append("REGDB.vw_temporarybatch.TEMPBATCHID = COEDB.COESAVEDHITLIST.ID AND ");
            cmdString.Append("COEDB.COESAVEDHITLIST.ID = REGDB.VW_MIXTURE.MIXTUREID ");
            cmdString.Append("AND REGDB.VW_MIXTURE.REGID = REGDB.VW_REGISTRYNUMBER.REGID AND ");
            cmdString.Append("COEDB.COESAVEDHITLISTID.SEARCH_CRITERIA_TYPE = 'TEMP'");

            DataTable dtHitListInfo = DALHelper.ExecuteQuery(cmdString.ToString());

            if (dtHitListInfo != null && dtHitListInfo.Rows.Count > 0)
            {
                try
                {
                    BulkDelete theBulkDelete = BulkDelete.Execute(Convert.ToInt32(Convert.ToString(dtHitListInfo.Rows[0]["HITLISTID"])), true, "Unit testing Method");

                    Assert.IsNotNull(theBulkDelete, "BulkDelete.Execute(Convert.ToInt32(Convert.ToString(dtHitListInfo.Rows[0]['HITLISTID'])), true, 'Unit testing Method') did not return the expected value.");
                }
                catch (Exception ex)
                {
                    Assert.Fail(ex.Message);
                }
            }
            else
            {
                Assert.IsTrue((dtHitListInfo == null && dtHitListInfo.Rows.Count == 0), "HitList information does not exist.");
            }
        }

        /// <summary>
        /// Deleting Permanant registry record in bulk order
        /// </summary>
        [TestMethod]
        public void Execute_DeletingPermanantRegistryReocrdInBulk() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            StringBuilder cmdString = new StringBuilder();
            cmdString.Append("Select DISTINCT COEDB.COESAVEDHITLIST.HITLISTID From COEDB.COESAVEDHITLIST, COEDB.COESAVEDHITLISTID, REGDB.VW_MIXTURE, REGDB.VW_REGISTRYNUMBER ");
            cmdString.Append("WHERE COEDB.COESAVEDHITLIST.HITLISTID = COEDB.COESAVEDHITLISTID.ID AND ");
            cmdString.Append("COEDB.COESAVEDHITLIST.ID = REGDB.VW_MIXTURE.MIXTUREID ");
            cmdString.Append("AND REGDB.VW_MIXTURE.REGID = REGDB.VW_REGISTRYNUMBER.REGID AND ");
            cmdString.Append("COEDB.COESAVEDHITLISTID.SEARCH_CRITERIA_TYPE = 'SAVED'");

            DataTable dtHitListInfo = DALHelper.ExecuteQuery(cmdString.ToString());

            if (dtHitListInfo != null && dtHitListInfo.Rows.Count > 0)
            {
                try
                {
                    BulkDelete theBulkDelete = BulkDelete.Execute(Convert.ToInt32(Convert.ToString(dtHitListInfo.Rows[0]["HITLISTID"])), false, "Unit testing Method");

                    Assert.IsNotNull(theBulkDelete, "BulkDelete.Execute(Convert.ToInt32(Convert.ToString(dtHitListInfo.Rows[0]['HITLISTID'])), false, 'Unit testing Method') did not return the expected value.");
                }
                catch (Exception ex)
                {
                    Assert.Fail(ex.Message);
                }
            }
            else
            {
                Assert.IsTrue((dtHitListInfo == null && dtHitListInfo.Rows.Count == 0), "HitList information does not exist.");
            }
        }

        /// <summary>
        /// Deleting Temparary registry record in bulk order with Log ID
        /// </summary>
        [TestMethod]
        public void Execute_DeletingTempararyRegistryReocrdInBulkWithLogID() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            StringBuilder cmdString = new StringBuilder();
            cmdString.Append("Select DISTINCT COEDB.COESAVEDHITLIST.HITLISTID From REGDB.vw_temporarybatch, COEDB.COESAVEDHITLIST, COEDB.COESAVEDHITLISTID, REGDB.VW_MIXTURE, REGDB.VW_REGISTRYNUMBER ");
            cmdString.Append("WHERE COEDB.COESAVEDHITLIST.HITLISTID = COEDB.COESAVEDHITLISTID.ID AND ");
            cmdString.Append("REGDB.vw_temporarybatch.TEMPBATCHID = COEDB.COESAVEDHITLIST.ID AND ");
            cmdString.Append("COEDB.COESAVEDHITLIST.ID = REGDB.VW_MIXTURE.MIXTUREID ");
            cmdString.Append("AND REGDB.VW_MIXTURE.REGID = REGDB.VW_REGISTRYNUMBER.REGID AND ");
            cmdString.Append("COEDB.COESAVEDHITLISTID.SEARCH_CRITERIA_TYPE = 'TEMP'");

            DataTable dtHitListInfo = DALHelper.ExecuteQuery(cmdString.ToString());

            if (dtHitListInfo != null && dtHitListInfo.Rows.Count > 0)
            {
                try
                {
                    int ilogID = 0;
                    bool blnResult = BulkDelete.Execute(Convert.ToInt32(Convert.ToString(dtHitListInfo.Rows[0]["HITLISTID"])), true, "Unit testing Method", out ilogID);

                    Assert.IsTrue(blnResult, "BulkDelete.Execute(Convert.ToInt32(Convert.ToString(dtHitListInfo.Rows[0]['HITLISTID'])), true, 'Unit testing Method', out ilogID) did not return the expected value.");
                }
                catch (Exception ex)
                {
                    Assert.Fail(ex.Message);
                }
            }
            else
            {
                Assert.IsTrue((dtHitListInfo == null && dtHitListInfo.Rows.Count == 0), "HitList information does not exist.");
            }
        }

        /// <summary>
        /// Deleting Permanant registry record in bulk order with Log ID
        /// </summary>
        [TestMethod]
        public void Execute_DeletingPermanantRegistryReocrdInBulkWithLogID() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            StringBuilder cmdString = new StringBuilder();
            cmdString.Append("Select DISTINCT COEDB.COESAVEDHITLIST.HITLISTID From COEDB.COESAVEDHITLIST, COEDB.COESAVEDHITLISTID, REGDB.VW_MIXTURE, REGDB.VW_REGISTRYNUMBER ");
            cmdString.Append("WHERE COEDB.COESAVEDHITLIST.HITLISTID = COEDB.COESAVEDHITLISTID.ID AND ");
            cmdString.Append("COEDB.COESAVEDHITLIST.ID = REGDB.VW_MIXTURE.MIXTUREID ");
            cmdString.Append("AND REGDB.VW_MIXTURE.REGID = REGDB.VW_REGISTRYNUMBER.REGID AND ");
            cmdString.Append("COEDB.COESAVEDHITLISTID.SEARCH_CRITERIA_TYPE = 'SAVED'");

            DataTable dtHitListInfo = DALHelper.ExecuteQuery(cmdString.ToString());

            if (dtHitListInfo != null && dtHitListInfo.Rows.Count > 0)
            {
                try
                {
                    int ilogID = 0;
                    bool blnResult = BulkDelete.Execute(Convert.ToInt32(Convert.ToString(dtHitListInfo.Rows[0]["HITLISTID"])), false, "Unit testing Method", out ilogID);

                    Assert.IsTrue(blnResult, "BulkDelete.Execute(Convert.ToInt32(Convert.ToString(dtHitListInfo.Rows[0]['HITLISTID'])), false, 'Unit testing Method', out ilogID) did not return the expected value.");
                }
                catch (Exception ex)
                {
                    Assert.Fail(ex.Message);
                }
            }
            else
            {
                Assert.IsTrue((dtHitListInfo == null && dtHitListInfo.Rows.Count == 0), "HitList information does not exist.");
            }
        }

        /// <summary>
        /// Getting data in all the properties of BulkDelete class
        /// </summary>
        [TestMethod]
        public void Execute_GettingDataInAllProperties() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            StringBuilder cmdString = new StringBuilder();
            cmdString.Append("Select DISTINCT COEDB.COESAVEDHITLIST.HITLISTID From COEDB.COESAVEDHITLIST, COEDB.COESAVEDHITLISTID, REGDB.VW_MIXTURE, REGDB.VW_REGISTRYNUMBER ");
            cmdString.Append("WHERE COEDB.COESAVEDHITLIST.HITLISTID = COEDB.COESAVEDHITLISTID.ID AND ");
            cmdString.Append("COEDB.COESAVEDHITLIST.ID = REGDB.VW_MIXTURE.MIXTUREID ");
            cmdString.Append("AND REGDB.VW_MIXTURE.REGID = REGDB.VW_REGISTRYNUMBER.REGID AND ");
            cmdString.Append("COEDB.COESAVEDHITLISTID.SEARCH_CRITERIA_TYPE = 'SAVED'");

            DataTable dtHitListInfo = DALHelper.ExecuteQuery(cmdString.ToString());

            if (dtHitListInfo != null && dtHitListInfo.Rows.Count > 0)
            {
                try
                {
                    BulkDelete theBulkDelete = BulkDelete.Execute(Convert.ToInt32(Convert.ToString(dtHitListInfo.Rows[0]["HITLISTID"])), false, "Unit testing Method");

                    string[] strFailedRecords = theBulkDelete.FailedRecords;
                    bool blnResult = theBulkDelete.Result;
                    bool blnTemporary = theBulkDelete.IsTemporary;
                    int iLogID = theBulkDelete.LogId;
                    string strLogDescription = theBulkDelete.LogDescription;

                    Assert.IsNotNull(theBulkDelete, "BulkDelete.Execute(Convert.ToInt32(Convert.ToString(dtHitListInfo.Rows[0]['HITLISTID'])), false, 'Unit testing Method') did not return the expected value.");
                }
                catch (Exception ex)
                {
                    Assert.Fail(ex.Message);
                }
            }
            else
            {
                Assert.IsTrue((dtHitListInfo == null && dtHitListInfo.Rows.Count == 0), "HitList information does not exist.");
            }
        }

        #endregion
    }
}
