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
    /// InvContainerTest class contains unit test methods for InvContainer
    /// </summary>
    [TestClass]
    public class InvContainerTest
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

        #region InvContainer Test Methods

        /// <summary>
        /// Creating new InvContainer using Inventory Container Information
        /// </summary>
        [TestMethod]
        public void NewInvContainer_CreatingNewInvContaienrUsingInventoryContainerInformation() 
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

            InvContainer theInvContainer = InvContainer.NewInvContainer(dtInvContainerInfo.Rows[0]);

            Assert.IsNotNull(theInvContainer, "InvContainer.NewInvContainer(dtInvContainerInfo.Rows[0]) did not return the expected value.");
        }

        #endregion
    }
}
