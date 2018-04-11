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
    /// InvContainerListTest class contains unit test methods for InvContainerList
    /// </summary>
    [TestClass]
    public class InvContainerListTest
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

        #region InvContainerList Test Methods

        /// <summary>
        /// Creating new InvContainerList using Inventory container information
        /// </summary>
        [TestMethod]
        public void NewInvContainerList_CreatingNewInvContainerUsinginventoryContainerInformation() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            InvContainerList theInvContainerList = InvContainerList.NewInvContainerList();

            Assert.IsNotNull(theInvContainerList, "InvContainerList.NewInvContainerList() did not return the expected value.");
        }

        /// <summary>
        /// Creating new InvContainerList using Registration Number
        /// </summary>
        [TestMethod]
        public void NewInvContainerList_CreatingNewNewDocumentUsingRegistrationNumber() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            InvContainerList theInvContainerList = InvContainerList.NewInvContainerList(GlobalVariables.REGNUM1);

            Assert.IsTrue(!string.IsNullOrEmpty(theInvContainerList.RegNumber), "InvContainerList.NewInvContainerList(GlobalVariables.REGNUM1) did not return the expected value.");
        }

        /// <summary>
        /// Creating new InvContainerList using Registration Number and Inventory container information
        /// </summary>
        [TestMethod]
        public void NewInvContainerList_CreatingNewNewDocumentUsingRegistrationNumberAndInvContainerInformation() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            DataTable dtInvContainerInfo = new DataTable();
            DataColumn newColumn = new DataColumn("QTYAVAILABLE", typeof(System.String));
            dtInvContainerInfo.Columns.Add(newColumn);
            newColumn = new DataColumn("CONTAINERID", typeof(System.String));
            dtInvContainerInfo.Columns.Add(newColumn);
            newColumn = new DataColumn("CONTAINERSIZE", typeof(System.String));
            dtInvContainerInfo.Columns.Add(newColumn);
            newColumn = new DataColumn("LOCATION", typeof(System.String));
            dtInvContainerInfo.Columns.Add(newColumn);
            newColumn = new DataColumn("CONTAINERTYPE", typeof(System.String));
            dtInvContainerInfo.Columns.Add(newColumn);
            newColumn = new DataColumn("REGBATCHID", typeof(System.String));
            dtInvContainerInfo.Columns.Add(newColumn);
            newColumn = new DataColumn("INVBATCHID", typeof(System.Int32));
            dtInvContainerInfo.Columns.Add(newColumn);
            newColumn = new DataColumn("COMPOUNDID", typeof(System.Int32));
            dtInvContainerInfo.Columns.Add(newColumn);
            newColumn = new DataColumn("REGNUMBER", typeof(System.String));
            dtInvContainerInfo.Columns.Add(newColumn);
            newColumn = new DataColumn("REQUEST_URL", typeof(System.String));
            dtInvContainerInfo.Columns.Add(newColumn);
            newColumn = new DataColumn("TotalQtyAvailable", typeof(System.String));
            dtInvContainerInfo.Columns.Add(newColumn);
            newColumn = new DataColumn("BARCODE", typeof(System.String));
            dtInvContainerInfo.Columns.Add(newColumn);

            DataRow newRow = dtInvContainerInfo.NewRow();
            newRow["QTYAVAILABLE"] = "100";
            newRow["CONTAINERID"] = "1";
            newRow["CONTAINERSIZE"] = "150";
            newRow["LOCATION"] = "US";
            newRow["CONTAINERTYPE"] = "CBOD";
            newRow["REGBATCHID"] = "11";
            newRow["INVBATCHID"] = 111;
            newRow["COMPOUNDID"] = 1111;
            newRow["REGNUMBER"] = GlobalVariables.REGNUM1;
            newRow["REQUEST_URL"] = "Not required";
            newRow["TotalQtyAvailable"] = "120";
            newRow["BARCODE"] = "111111";
            dtInvContainerInfo.Rows.Add(newRow);

            InvContainerList theInvContainerList = InvContainerList.NewInvContainerList(dtInvContainerInfo, GlobalVariables.REGNUM1);

            Assert.IsTrue(theInvContainerList.Count > 0, "InvContainerList.NewInvContainerList(dtInvContainerInfo, GlobalVariables.REGNUM1) did not return the expected value.");
        }

        /// <summary>
        /// Getting Last container ID
        /// </summary>
        [TestMethod]
        public void LastContainerID_GettingLastContainerID() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            DataTable dtInvContainerInfo = new DataTable();
            DataColumn newColumn = new DataColumn("QTYAVAILABLE", typeof(System.String));
            dtInvContainerInfo.Columns.Add(newColumn);
            newColumn = new DataColumn("CONTAINERID", typeof(System.String));
            dtInvContainerInfo.Columns.Add(newColumn);
            newColumn = new DataColumn("CONTAINERSIZE", typeof(System.String));
            dtInvContainerInfo.Columns.Add(newColumn);
            newColumn = new DataColumn("LOCATION", typeof(System.String));
            dtInvContainerInfo.Columns.Add(newColumn);
            newColumn = new DataColumn("CONTAINERTYPE", typeof(System.String));
            dtInvContainerInfo.Columns.Add(newColumn);
            newColumn = new DataColumn("REGBATCHID", typeof(System.String));
            dtInvContainerInfo.Columns.Add(newColumn);
            newColumn = new DataColumn("INVBATCHID", typeof(System.Int32));
            dtInvContainerInfo.Columns.Add(newColumn);
            newColumn = new DataColumn("COMPOUNDID", typeof(System.Int32));
            dtInvContainerInfo.Columns.Add(newColumn);
            newColumn = new DataColumn("REGNUMBER", typeof(System.String));
            dtInvContainerInfo.Columns.Add(newColumn);
            newColumn = new DataColumn("REQUEST_URL", typeof(System.String));
            dtInvContainerInfo.Columns.Add(newColumn);
            newColumn = new DataColumn("TotalQtyAvailable", typeof(System.String));
            dtInvContainerInfo.Columns.Add(newColumn);
            newColumn = new DataColumn("BARCODE", typeof(System.String));
            dtInvContainerInfo.Columns.Add(newColumn);

            DataRow newRow = dtInvContainerInfo.NewRow();
            newRow["QTYAVAILABLE"] = "100";
            newRow["CONTAINERID"] = "1";
            newRow["CONTAINERSIZE"] = "150";
            newRow["LOCATION"] = "US";
            newRow["CONTAINERTYPE"] = "CBOD";
            newRow["REGBATCHID"] = "11";
            newRow["INVBATCHID"] = 111;
            newRow["COMPOUNDID"] = 1111;
            newRow["REGNUMBER"] = GlobalVariables.REGNUM1;
            newRow["REQUEST_URL"] = "Not required";
            newRow["TotalQtyAvailable"] = "120";
            newRow["BARCODE"] = "111111";
            dtInvContainerInfo.Rows.Add(newRow);

            InvContainerList theInvContainerList = InvContainerList.NewInvContainerList(dtInvContainerInfo, GlobalVariables.REGNUM1);
            int iLastCOntainerID = theInvContainerList.LastContainerID;

            Assert.IsTrue(iLastCOntainerID > 0, "InvContainerList.LastContainerID did not return the expected value.");
        }

        /// <summary>
        /// Getting Last contaienr request URL
        /// </summary>
        [TestMethod]
        public void LastContainerRequestURL_GettingLastContainerRequestURL() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            DataTable dtInvContainerInfo = new DataTable();
            DataColumn newColumn = new DataColumn("QTYAVAILABLE", typeof(System.String));
            dtInvContainerInfo.Columns.Add(newColumn);
            newColumn = new DataColumn("CONTAINERID", typeof(System.String));
            dtInvContainerInfo.Columns.Add(newColumn);
            newColumn = new DataColumn("CONTAINERSIZE", typeof(System.String));
            dtInvContainerInfo.Columns.Add(newColumn);
            newColumn = new DataColumn("LOCATION", typeof(System.String));
            dtInvContainerInfo.Columns.Add(newColumn);
            newColumn = new DataColumn("CONTAINERTYPE", typeof(System.String));
            dtInvContainerInfo.Columns.Add(newColumn);
            newColumn = new DataColumn("REGBATCHID", typeof(System.String));
            dtInvContainerInfo.Columns.Add(newColumn);
            newColumn = new DataColumn("INVBATCHID", typeof(System.Int32));
            dtInvContainerInfo.Columns.Add(newColumn);
            newColumn = new DataColumn("COMPOUNDID", typeof(System.Int32));
            dtInvContainerInfo.Columns.Add(newColumn);
            newColumn = new DataColumn("REGNUMBER", typeof(System.String));
            dtInvContainerInfo.Columns.Add(newColumn);
            newColumn = new DataColumn("REQUEST_URL", typeof(System.String));
            dtInvContainerInfo.Columns.Add(newColumn);
            newColumn = new DataColumn("TotalQtyAvailable", typeof(System.String));
            dtInvContainerInfo.Columns.Add(newColumn);
            newColumn = new DataColumn("BARCODE", typeof(System.String));
            dtInvContainerInfo.Columns.Add(newColumn);

            DataRow newRow = dtInvContainerInfo.NewRow();
            newRow["QTYAVAILABLE"] = "100";
            newRow["CONTAINERID"] = "1";
            newRow["CONTAINERSIZE"] = "150";
            newRow["LOCATION"] = "US";
            newRow["CONTAINERTYPE"] = "CBOD";
            newRow["REGBATCHID"] = "11";
            newRow["INVBATCHID"] = 111;
            newRow["COMPOUNDID"] = 1111;
            newRow["REGNUMBER"] = GlobalVariables.REGNUM1;
            newRow["REQUEST_URL"] = "Not required";
            newRow["TotalQtyAvailable"] = "120";
            newRow["BARCODE"] = "111111";
            dtInvContainerInfo.Rows.Add(newRow);

            InvContainerList theInvContainerList = InvContainerList.NewInvContainerList(dtInvContainerInfo, GlobalVariables.REGNUM1);
            string strLastContainerRequestURL = theInvContainerList.LastContainerRequestURL;

            Assert.IsTrue(!string.IsNullOrEmpty(strLastContainerRequestURL), "InvContainerList.LastContainerRequestURL did not return the expected value.");
        }

        /// <summary>
        /// Getting Total Quantity available
        /// </summary>
        [TestMethod]
        public void TotalQtyAvailable_GettingTotalQuantityAvailable() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            DataTable dtInvContainerInfo = new DataTable();
            DataColumn newColumn = new DataColumn("QTYAVAILABLE", typeof(System.String));
            dtInvContainerInfo.Columns.Add(newColumn);
            newColumn = new DataColumn("CONTAINERID", typeof(System.String));
            dtInvContainerInfo.Columns.Add(newColumn);
            newColumn = new DataColumn("CONTAINERSIZE", typeof(System.String));
            dtInvContainerInfo.Columns.Add(newColumn);
            newColumn = new DataColumn("LOCATION", typeof(System.String));
            dtInvContainerInfo.Columns.Add(newColumn);
            newColumn = new DataColumn("CONTAINERTYPE", typeof(System.String));
            dtInvContainerInfo.Columns.Add(newColumn);
            newColumn = new DataColumn("REGBATCHID", typeof(System.String));
            dtInvContainerInfo.Columns.Add(newColumn);
            newColumn = new DataColumn("INVBATCHID", typeof(System.Int32));
            dtInvContainerInfo.Columns.Add(newColumn);
            newColumn = new DataColumn("COMPOUNDID", typeof(System.Int32));
            dtInvContainerInfo.Columns.Add(newColumn);
            newColumn = new DataColumn("REGNUMBER", typeof(System.String));
            dtInvContainerInfo.Columns.Add(newColumn);
            newColumn = new DataColumn("REQUEST_URL", typeof(System.String));
            dtInvContainerInfo.Columns.Add(newColumn);
            newColumn = new DataColumn("TotalQtyAvailable", typeof(System.String));
            dtInvContainerInfo.Columns.Add(newColumn);
            newColumn = new DataColumn("BARCODE", typeof(System.String));
            dtInvContainerInfo.Columns.Add(newColumn);

            DataRow newRow = dtInvContainerInfo.NewRow();
            newRow["QTYAVAILABLE"] = "100";
            newRow["CONTAINERID"] = "1";
            newRow["CONTAINERSIZE"] = "150";
            newRow["LOCATION"] = "US";
            newRow["CONTAINERTYPE"] = "CBOD";
            newRow["REGBATCHID"] = "11";
            newRow["INVBATCHID"] = 111;
            newRow["COMPOUNDID"] = 1111;
            newRow["REGNUMBER"] = GlobalVariables.REGNUM1;
            newRow["REQUEST_URL"] = "Not required";
            newRow["TotalQtyAvailable"] = "120";
            newRow["BARCODE"] = "111111";
            dtInvContainerInfo.Rows.Add(newRow);

            InvContainerList theInvContainerList = InvContainerList.NewInvContainerList(dtInvContainerInfo, GlobalVariables.REGNUM1);
            string strTotalQtyAvailable = theInvContainerList.TotalQtyAvailable;

            Assert.IsTrue(!string.IsNullOrEmpty(strTotalQtyAvailable), "InvContainerList.TotalQtyAvailable did not return the expected value.");
        }

        #endregion
    }
}
