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
    /// DocumentTest class contains unit test methods for Document
    /// </summary>
    [TestClass]
    public class DocumentTest
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

        #region Document Test Methods

        /// <summary>
        /// Creating new Document using Document Information
        /// </summary>
        [TestMethod]
        public void NewDocument_CreatingNewNewDocumentUsingDocumentInfomation() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            DataTable dtDocumentInfo = new DataTable();
            DataColumn newColumn = new DataColumn("NAME");
            dtDocumentInfo.Columns.Add(newColumn);
            newColumn = new DataColumn("DOCID");
            dtDocumentInfo.Columns.Add(newColumn);
            newColumn = new DataColumn("TITLE");
            dtDocumentInfo.Columns.Add(newColumn);
            newColumn = new DataColumn("AUTHOR");
            dtDocumentInfo.Columns.Add(newColumn);
            newColumn = new DataColumn("DATE_SUBMITTED");
            dtDocumentInfo.Columns.Add(newColumn);
            newColumn = new DataColumn("RID");
            dtDocumentInfo.Columns.Add(newColumn);
            newColumn = new DataColumn("DOCID_LINKID");
            dtDocumentInfo.Columns.Add(newColumn);

            DataRow newRow = dtDocumentInfo.NewRow();
            newRow["NAME"] = "Test_Document";
            newRow["DOCID"] = "1";
            newRow["TITLE"] = "Test Document";
            newRow["AUTHOR"] = "CBOE";
            newRow["DATE_SUBMITTED"] = DateTime.Now.ToShortDateString();
            newRow["RID"] = "11";
            newRow["DOCID_LINKID"] = "111";
            dtDocumentInfo.Rows.Add(newRow);

            Document theDocument = Document.NewDocument(dtDocumentInfo.Rows[0]);

            Assert.IsNotNull(theDocument, "Document.NewDocument(dtDocumentInfo.Rows[0]) did not return the expected value.");
        }

        #endregion
    }
}
