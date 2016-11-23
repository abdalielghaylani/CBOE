using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml;
using CambridgeSoft.COE.Registration.Services.Types;
using System.Reflection;
using System.Xml.XPath;
using CambridgeSoft.COE.Framework.COEGenericObjectStorageService;
using CambridgeSoft.COE.Framework.Common.Validation;
using System.Data;

namespace CambridgeSoft.COE.Registration.MSUnitTests
{
    /// <summary>
    /// RegistryRecordListTest class contains unit test methods for RegistryRecordList
    /// </summary>
    [TestClass]
    public class RegistryRecordListTest
    {
        #region Variables

        private TestContext testContextInstance;

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
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");
        }

        /// <summary>
        /// Use TestCleanup to run code after each test has run 
        /// </summary>
        [TestCleanup()]
        public void MyTestCleanup()
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");
            Helpers.Authentication.Logoff();
        }

        #endregion

        #region RegistryRecordList Test Methods

        /// <summary>
        /// Creating new registry record List
        /// </summary>
        [TestMethod]
        public void NewList_CreatingNewRegistryRecordList() 
        {
            RegistryRecordList theRegistryRecordList = RegistryRecordList.NewList();

            Assert.IsNotNull(theRegistryRecordList, "RegistryRecordList.NewList() did not return the expected value.");
        }

        /// <summary>
        /// Loading registry record list by Registration number
        /// </summary>
        [TestMethod]
        public void GetList_LoadingRegistryRecordListByRegistrationNumber() 
        {
            StringBuilder theStringBuilder = new StringBuilder();
            theStringBuilder.Append("<REGISTRYLIST>");
            theStringBuilder.Append("<REGNUMBER>");
            theStringBuilder.Append(GlobalVariables.REGNUM1);
            theStringBuilder.Append("</REGNUMBER>");
            theStringBuilder.Append("</REGISTRYLIST>");

            RegistryRecordList theRegistryRecordList = RegistryRecordList.GetList(theStringBuilder.ToString());

            Assert.IsTrue(theRegistryRecordList.Count > 0, "RegistryRecordList.GetList(theStringBuilder.ToString()) did not return the expected value.");
        }

        /// <summary>
        /// Loading registry record list from XML
        /// </summary>
        [TestMethod]
        public void GetListFomXml_LoadingRegistryRecordListFromXML() 
        {
            XmlDocument theXmlDocument = new XmlDocument();
            theXmlDocument.Load(string.Format(@"{0}\{1}", Assembly.GetExecutingAssembly().Location.Substring(0, Assembly.GetExecutingAssembly().Location.IndexOf("TestResults")), GlobalVariables.RegistryRecordXml));

            StringBuilder theStringBuilder = new StringBuilder();
            theStringBuilder.Append("<MultiCompoundRegistryRecordList>");
            theStringBuilder.Append(theXmlDocument.OuterXml);
            theStringBuilder.Append("</MultiCompoundRegistryRecordList>");

            RegistryRecordList theResult = RegistryRecordList.GetListFomXml(theStringBuilder.ToString());

            Assert.IsTrue(theResult.Count > 0, "RegistryRecordList.GetListFomXml(theStringBuilder.ToString()) did not return the expected value.");
        }

        /// <summary>
        /// Loading registry record list using Strcuture ID
        /// </summary>
        [TestMethod]
        public void GetListFromLinkedStructure_LoadingRegistryRecordListUsingStrcutureID() 
        {
            RegistryRecord theRegistryRecord = RegistryHelper.LoadRegistryRecord(GlobalVariables.REGNUM1, GlobalVariables.RegistryLoadType.REGNUM);
            if (theRegistryRecord == null)
            {
                theRegistryRecord = RegistryHelper.CreateRegistryObject(GlobalVariables.REGNUM1);
                theRegistryRecord = theRegistryRecord.Register(DuplicateAction.None);
            }

            RegistryRecordList theResult = RegistryRecordList.GetListFromLinkedStructure(theRegistryRecord.ComponentList[0].Compound.BaseFragment.Structure.ID);

            Assert.IsTrue(theResult.Count > 0, "RegistryRecordList.GetListFromLinkedStructure(theRegistryRecord.ComponentList[0].Compound.BaseFragment.Structure.ID) did not return the expected value.");
        }

        /// <summary>
        /// Loading registry record list using Mixture Component
        /// </summary>
        [TestMethod]
        public void GetListFromSharableComponent_LoadingRegistryRecordListUsingMixtureComponent() 
        {
            RegistryRecord theRegistryRecord = RegistryHelper.LoadRegistryRecord(GlobalVariables.REGNUM1, GlobalVariables.RegistryLoadType.REGNUM);
            if (theRegistryRecord == null)
            {
                theRegistryRecord = RegistryHelper.CreateRegistryObject(GlobalVariables.REGNUM1);
                theRegistryRecord = theRegistryRecord.Register(DuplicateAction.None);
            }

            RegistryRecordList theResult = RegistryRecordList.GetListFromSharableComponent(theRegistryRecord.ComponentList[0]);

            Assert.IsTrue(theResult.Count > 0, "RegistryRecordList.GetListFromSharableComponent(theRegistryRecord.ComponentList[0]) did not return the expected value.");
        }

        /// <summary>
        /// Loading registry record list using Mixture Component ID
        /// </summary>
        [TestMethod]
        public void GetListFromSharableComponent_LoadingRegistryRecordListUsingMixtureComponentID() 
        {
            RegistryRecord theRegistryRecord = RegistryHelper.LoadRegistryRecord(GlobalVariables.REGNUM1, GlobalVariables.RegistryLoadType.REGNUM);
            if (theRegistryRecord == null)
            {
                theRegistryRecord = RegistryHelper.CreateRegistryObject(GlobalVariables.REGNUM1);
                theRegistryRecord = theRegistryRecord.Register(DuplicateAction.None);
            }

            RegistryRecordList theResult = RegistryRecordList.GetListFromSharableComponent(theRegistryRecord.ComponentList[0].Compound.ID);

            Assert.IsTrue(theResult.Count > 0, "RegistryRecordList.GetListFromSharableComponent(theRegistryRecord.ComponentList[0].ID) did not return the expected value.");
        }

        /// <summary>
        /// Loading Log informatino table using Log ID
        /// </summary>
        [TestMethod]
        public void GetLogInfoTable_LoadingLogInformationTableUsingLogID() 
        {
            RegistryRecordList theRegistryRecordList = RegistryRecordList.NewList();

            PrivateObject thePrivateObject = new PrivateObject(theRegistryRecordList);
            object theObject = thePrivateObject.GetFieldOrProperty("RegDal");

            if (theObject != null)
            {
                CambridgeSoft.COE.Registration.Access.RegistrationOracleDAL theRegistrationOracleDAL = theObject as CambridgeSoft.COE.Registration.Access.RegistrationOracleDAL;
                theRegistrationOracleDAL.DALManager.GetConnection();
                thePrivateObject.SetFieldOrProperty("_regDal", theRegistrationOracleDAL);
            }

            StringBuilder cmdString = new StringBuilder();
            cmdString.Append("SELECT DISTINCT LOG_ID FROM REGDB.LOG_BULKREGISTRATION_ID");

            DataTable dtLogInfo = DALHelper.ExecuteQuery(cmdString.ToString());

            if (dtLogInfo != null && dtLogInfo.Rows.Count > 0)
            {

                DataTable dtLogInFoTable = RegistryRecordList.GetLogInfoTable(Convert.ToInt32(Convert.ToString(dtLogInfo.Rows[0]["LOG_ID"])));

                Assert.IsNotNull(dtLogInFoTable, "RegistryRecordList.GetLogInfoTable(LOG_ID) did not return the expected value.");
            }
            else
            {
                Assert.IsTrue((dtLogInfo == null && dtLogInfo.Rows.Count == 0), "Log information does not exist.");
            }
        }

        /// <summary>
        /// Loading Registration record using Hit List ID
        /// </summary>
        [TestMethod]
        public void GetRegistryRecordList_LoadingRegistryRecordUsingHitLisTID() 
        {
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
                    RegistryRecordList theResult = RegistryRecordList.GetRegistryRecordList(Convert.ToInt32(Convert.ToString(dtHitListInfo.Rows[0]["HITLISTID"])));

                    Assert.IsTrue(theResult.Count > 0, "RegistryRecordList.GetRegistryRecordList(HitListID) did not return the expected value.");
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
        /// Loading Temparary Registration record using Hit List ID
        /// </summary>
        [TestMethod]
        public void GetRegistryRecordList_LoadingTemparayRegistryRecordUsingHitLisTID() 
        {
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
                    RegistryRecordList theResult = RegistryRecordList.GetRegistryRecordList(Convert.ToInt32(Convert.ToString(dtHitListInfo.Rows[0]["HITLISTID"])), true);

                    Assert.IsTrue(theResult.Count > 0, "RegistryRecordList.GetRegistryRecordList(HitListID,true) did not return the expected value.");
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
        /// Loading Permanant Registration record using Hit List ID
        /// </summary>
        [TestMethod]
        public void GetRegistryRecordList_LoadingPermanantRegistryRecordUsingHitLisTID() 
        {
            StringBuilder cmdString = new StringBuilder();
            cmdString.Append("Select DISTINCT COEDB.COESAVEDHITLIST.HITLISTID From COEDB.COESAVEDHITLIST, COEDB.COESAVEDHITLISTID, REGDB.VW_MIXTURE, REGDB.VW_REGISTRYNUMBER ");
            cmdString.Append("WHERE COEDB.COESAVEDHITLIST.HITLISTID = COEDB.COESAVEDHITLISTID.ID AND ");
            cmdString.Append("COEDB.COESAVEDHITLIST.ID = REGDB.VW_MIXTURE.MIXTUREID ");
            cmdString.Append("AND REGDB.VW_MIXTURE.REGID = REGDB.VW_REGISTRYNUMBER.REGID AND ");
            cmdString.Append("COEDB.COESAVEDHITLISTID.SEARCH_CRITERIA_TYPE = 'SAVED'");

            DataTable dtHitListInfo = DALHelper.ExecuteQuery(cmdString.ToString());

            if (dtHitListInfo != null && dtHitListInfo.Rows.Count > 0)
            {
                RegistryRecordList theResult = RegistryRecordList.GetRegistryRecordList(Convert.ToInt32(Convert.ToString(dtHitListInfo.Rows[0]["HITLISTID"])), false);

                Assert.IsTrue(theResult.Count > 0, "RegistryRecordList.GetRegistryRecordList(HitListID,false) did not return the expected value.");
            }
            else
            {
                Assert.IsTrue((dtHitListInfo == null && dtHitListInfo.Rows.Count == 0), "HitList information does not exist.");
            }
        }

        /// <summary>
        /// Getting Temparary record count using Hit List ID
        /// </summary>
        [TestMethod]
        public void GetTempRecordCount_GettingTemparyRecordCountUsingHitLisTID() 
        {
            StringBuilder cmdString = new StringBuilder();
            cmdString.Append("Select DISTINCT COEDB.COESAVEDHITLIST.HITLISTID From COEDB.COESAVEDHITLIST, COEDB.COESAVEDHITLISTID, REGDB.VW_MIXTURE, REGDB.VW_REGISTRYNUMBER ");
            cmdString.Append("WHERE COEDB.COESAVEDHITLIST.HITLISTID = COEDB.COESAVEDHITLISTID.ID AND ");
            cmdString.Append("COEDB.COESAVEDHITLIST.ID = REGDB.VW_MIXTURE.MIXTUREID ");
            cmdString.Append("AND REGDB.VW_MIXTURE.REGID = REGDB.VW_REGISTRYNUMBER.REGID AND ");
            cmdString.Append("COEDB.COESAVEDHITLISTID.SEARCH_CRITERIA_TYPE = 'SAVED'");

            DataTable dtHitListInfo = DALHelper.ExecuteQuery(cmdString.ToString());

            if (dtHitListInfo != null && dtHitListInfo.Rows.Count > 0)
            {
                int iTempRecourdCount = RegistryRecordList.GetTempRecordCount(Convert.ToInt32(Convert.ToString(dtHitListInfo.Rows[0]["HITLISTID"])));

                Assert.IsTrue(iTempRecourdCount > 0, "RegistryRecordList.GetTempRecordCount(HitListID) did not return the expected value.");
            }
            else
            {
                Assert.IsTrue((dtHitListInfo == null && dtHitListInfo.Rows.Count == 0), "HitList information does not exist.");
            }
        }

        /// <summary>
        /// Loading Temparay registry record using Temparayry ID
        /// </summary>
        [TestMethod]
        public void GetTemporaryList_LoadingTemparayRegistryRecordUsingTemparayID() 
        {
            StringBuilder cmdString = new StringBuilder();
            cmdString.Append("SELECT TEMPBATCHID FROM REGDB.VW_TEMPORARYBATCH WHERE ROWNUM < 11");

            DataTable dtHitListInfo = DALHelper.ExecuteQuery(cmdString.ToString());

            if (dtHitListInfo != null && dtHitListInfo.Rows.Count > 0)
            {
                RegistryRecordList theResult = RegistryRecordList.GetTemporaryList(Convert.ToString(dtHitListInfo.Rows[0]["TEMPBATCHID"]));

                Assert.IsTrue(theResult.Count > 0, "RegistryRecordList.GetTemporaryList(TEMPID) did not return the expected value.");
            }
            else
            {
                Assert.IsTrue((dtHitListInfo == null && dtHitListInfo.Rows.Count == 0), "HitList information does not exist.");
            }
        }

        /// <summary>
        /// Loading Temparay registry record using Temparayry ID
        /// </summary>
        [TestMethod]
        public void LoadRegistryRecordList_LoadingTemparayRegistryRecordUsingTemparayID() 
        {
            StringBuilder cmdString = new StringBuilder();
            cmdString.Append("Select DISTINCT COEDB.COESAVEDHITLIST.HITLISTID From COEDB.COESAVEDHITLIST, COEDB.COESAVEDHITLISTID, REGDB.VW_MIXTURE, REGDB.VW_REGISTRYNUMBER ");
            cmdString.Append("WHERE COEDB.COESAVEDHITLIST.HITLISTID = COEDB.COESAVEDHITLISTID.ID AND ");
            cmdString.Append("COEDB.COESAVEDHITLIST.ID = REGDB.VW_MIXTURE.MIXTUREID ");
            cmdString.Append("AND REGDB.VW_MIXTURE.REGID = REGDB.VW_REGISTRYNUMBER.REGID AND ");
            cmdString.Append("COEDB.COESAVEDHITLISTID.SEARCH_CRITERIA_TYPE = 'TEMP'");

            DataTable dtHitListInfo = DALHelper.ExecuteQuery(cmdString.ToString());

            if (dtHitListInfo != null && dtHitListInfo.Rows.Count > 0)
            {
                try
                {
                    RegRecordListInfo theRegRecordListInfo = RegistryRecordList.LoadRegistryRecordList(DuplicateAction.None, Convert.ToInt32(Convert.ToString(dtHitListInfo.Rows[0]["HITLISTID"])));

                    Assert.IsNotNull(theRegRecordListInfo, "RegistryRecordList.LoadRegistryRecordList(DuplicateAction.None, HITLISTID) did not return the expected value.");
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
        /// Loading registry record using XML
        /// </summary>
        [TestMethod]
        public void LoadRegistryRecordList_LoadingRegistryRecordUsingXML() 
        {
            XmlDocument theXmlDocument = new XmlDocument();
            theXmlDocument.Load(string.Format(@"{0}\{1}", Assembly.GetExecutingAssembly().Location.Substring(0, Assembly.GetExecutingAssembly().Location.IndexOf("TestResults")), GlobalVariables.RegistryRecordXml));

            try
            {
                RegRecordListInfo theRegRecordListInfo = RegistryRecordList.LoadRegistryRecordList(DuplicateAction.None, theXmlDocument.InnerXml);

                Assert.IsNotNull(theRegRecordListInfo, "RegistryRecordList.LoadRegistryRecordList(DuplicateAction.None, theXmlDocument.InnerXml) did not return the expected value.");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        /// <summary>
        /// Loading Temparay registry record using XML and Temparayry ID
        /// </summary>
        [TestMethod]
        public void LoadRegistryRecordList_LoadingTemparayRegistryRecordUsingXMLAndTemparayID() 
        {
            StringBuilder cmdString = new StringBuilder();
            cmdString.Append("Select DISTINCT COEDB.COESAVEDHITLIST.HITLISTID From COEDB.COESAVEDHITLIST, COEDB.COESAVEDHITLISTID, REGDB.VW_MIXTURE, REGDB.VW_REGISTRYNUMBER ");
            cmdString.Append("WHERE COEDB.COESAVEDHITLIST.HITLISTID = COEDB.COESAVEDHITLISTID.ID AND ");
            cmdString.Append("COEDB.COESAVEDHITLIST.ID = REGDB.VW_MIXTURE.MIXTUREID ");
            cmdString.Append("AND REGDB.VW_MIXTURE.REGID = REGDB.VW_REGISTRYNUMBER.REGID AND ");
            cmdString.Append("COEDB.COESAVEDHITLISTID.SEARCH_CRITERIA_TYPE = 'TEMP'");

            DataTable dtHitListInfo = DALHelper.ExecuteQuery(cmdString.ToString());

            if (dtHitListInfo != null && dtHitListInfo.Rows.Count > 0)
            {
                try
                {
                    RegRecordListInfo theRegRecordListInfo = RegistryRecordList.LoadRegistryRecordList(DuplicateAction.None, Convert.ToInt32(Convert.ToString(dtHitListInfo.Rows[0]["HITLISTID"])), true, "Actor", "Load Record");

                    Assert.IsNotNull(theRegRecordListInfo, "RegistryRecordList.LoadRegistryRecordList(DuplicateAction.None, HITLISTID, true,'Actor','Load Record') did not return the expected value.");
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
        /// Loading Temparay registry record using XML and Temparayry ID
        /// </summary>
        [TestMethod]
        public void LoadRegistryRecordsByHitList_LoadingTemparayRegistryRecordUsingXMLAndTemparayID() 
        {
            StringBuilder cmdString = new StringBuilder();
            cmdString.Append("Select DISTINCT COEDB.COESAVEDHITLIST.HITLISTID From COEDB.COESAVEDHITLIST, COEDB.COESAVEDHITLISTID, REGDB.VW_MIXTURE, REGDB.VW_REGISTRYNUMBER ");
            cmdString.Append("WHERE COEDB.COESAVEDHITLIST.HITLISTID = COEDB.COESAVEDHITLISTID.ID AND ");
            cmdString.Append("COEDB.COESAVEDHITLIST.ID = REGDB.VW_MIXTURE.MIXTUREID ");
            cmdString.Append("AND REGDB.VW_MIXTURE.REGID = REGDB.VW_REGISTRYNUMBER.REGID AND ");
            cmdString.Append("COEDB.COESAVEDHITLISTID.SEARCH_CRITERIA_TYPE = 'TEMP'");

            DataTable dtHitListInfo = DALHelper.ExecuteQuery(cmdString.ToString());

            if (dtHitListInfo != null && dtHitListInfo.Rows.Count > 0)
            {
                try
                {
                    RegRecordListInfo theRegRecordListInfo = RegistryRecordList.LoadRegistryRecordsByHitList(DuplicateAction.None, Convert.ToInt32(Convert.ToString(dtHitListInfo.Rows[0]["HITLISTID"])), "Actor", "Load Record");

                    Assert.IsNotNull(theRegRecordListInfo, "RegistryRecordList.LoadRegistryRecordsByHitList(DuplicateAction.None, HITLISTID, 'Actor','Load Record') did not return the expected value.");
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
        /// Removing registry record from Registry record list
        /// </summary>
        [TestMethod]
        public void Remove_RemovingRegistryRecordFroMRgistryRecordList() 
        {
            StringBuilder cmdString = new StringBuilder();
            cmdString.Append("Select DISTINCT COEDB.COESAVEDHITLIST.HITLISTID From COEDB.COESAVEDHITLIST, COEDB.COESAVEDHITLISTID, REGDB.VW_MIXTURE, REGDB.VW_REGISTRYNUMBER ");
            cmdString.Append("WHERE COEDB.COESAVEDHITLIST.HITLISTID = COEDB.COESAVEDHITLISTID.ID AND ");
            cmdString.Append("COEDB.COESAVEDHITLIST.ID = REGDB.VW_MIXTURE.MIXTUREID ");
            cmdString.Append("AND REGDB.VW_MIXTURE.REGID = REGDB.VW_REGISTRYNUMBER.REGID AND ");
            cmdString.Append("COEDB.COESAVEDHITLISTID.SEARCH_CRITERIA_TYPE = 'SAVED'");

            DataTable dtHitListInfo = DALHelper.ExecuteQuery(cmdString.ToString());

            if (dtHitListInfo != null && dtHitListInfo.Rows.Count > 0)
            {
                RegistryRecordList theRegistryRecordList = RegistryRecordList.GetRegistryRecordList(Convert.ToInt32(Convert.ToString(dtHitListInfo.Rows[0]["HITLISTID"])), false);

                bool blnResult = theRegistryRecordList.Remove(theRegistryRecordList[0]);

                Assert.IsTrue(blnResult, "RegistryRecordList.Remove(theRegistryRecordList[0]) did not return the expected value.");
            }
            else
            {
                Assert.IsTrue((dtHitListInfo == null && dtHitListInfo.Rows.Count == 0), "HitList information does not exist.");
            }
        }

        /// <summary>
        /// Clearing the StructureAggregation property
        /// </summary>
        [TestMethod]
        public void UpdateAggregateStructures_ClearingTheStructureAggregationProperty() 
        {
            StringBuilder cmdString = new StringBuilder();
            cmdString.Append("Select DISTINCT COEDB.COESAVEDHITLIST.HITLISTID From COEDB.COESAVEDHITLIST, COEDB.COESAVEDHITLISTID, REGDB.VW_MIXTURE, REGDB.VW_REGISTRYNUMBER ");
            cmdString.Append("WHERE COEDB.COESAVEDHITLIST.HITLISTID = COEDB.COESAVEDHITLISTID.ID AND ");
            cmdString.Append("COEDB.COESAVEDHITLIST.ID = REGDB.VW_MIXTURE.MIXTUREID ");
            cmdString.Append("AND REGDB.VW_MIXTURE.REGID = REGDB.VW_REGISTRYNUMBER.REGID AND ");
            cmdString.Append("COEDB.COESAVEDHITLISTID.SEARCH_CRITERIA_TYPE = 'SAVED'");

            DataTable dtHitListInfo = DALHelper.ExecuteQuery(cmdString.ToString());

            if (dtHitListInfo != null && dtHitListInfo.Rows.Count > 0)
            {
                RegistryRecordList theRegistryRecordList = RegistryRecordList.GetRegistryRecordList(Convert.ToInt32(Convert.ToString(dtHitListInfo.Rows[0]["HITLISTID"])), false);

                theRegistryRecordList[0].CheckOtherMixtures = true;
                theRegistryRecordList.UpdateAggregateStructures();

                Assert.AreNotEqual(theRegistryRecordList[0].CheckOtherMixtures, true, "RegistryRecordList.UpdateAggregateStructures() did not return the expected value.");
            }
            else
            {
                Assert.IsTrue((dtHitListInfo == null && dtHitListInfo.Rows.Count == 0), "HitList information does not exist.");
            }
        }
        
        #endregion
    }
}
