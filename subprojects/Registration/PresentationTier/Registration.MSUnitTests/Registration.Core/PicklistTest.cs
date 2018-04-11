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
using CambridgeSoft.COE.Registration.Core;

namespace CambridgeSoft.COE.Registration.MSUnitTests
{
    /// <summary>
    /// PicklistTest class contains unit test methods for Picklist
    /// </summary>
    [TestClass]
    public class PicklistTest
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

        #region Picklist Test Methods

        /// <summary>
        /// Loading PickList from XML
        /// </summary>
        [TestMethod]
        public void NewPicklist_LoadingPickListFromXML() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            try
            {
                StringBuilder theStringBuilder = new StringBuilder();
                theStringBuilder.Append("<Picklist id='1' count='2'>");
                theStringBuilder.Append("<PicklistItem ID='101'>Test1</PicklistItem>");
                theStringBuilder.Append("<PicklistItem ID='102'>Test2</PicklistItem>");
                theStringBuilder.Append("</Picklist>");
                Picklist thePicklist = Picklist.NewPicklist(theStringBuilder.ToString());

                Assert.IsTrue(thePicklist.PickList.Count > 0, "Picklist.NewPicklist(theStringBuilder.ToString()) did not return the expected value.");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        /// <summary>
        /// Getting valid exception message
        /// </summary>
        [TestMethod]
        public void NewPicklist_GettingValidExceptionMessage() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            try
            {
                StringBuilder theStringBuilder = new StringBuilder();
                theStringBuilder.Append("<Picklist id='1' count='2'>");
                theStringBuilder.Append("<PicklistItem ID='A'>Test1</PicklistItem>");
                theStringBuilder.Append("<PicklistItem ID='B'>Test2</PicklistItem>");
                theStringBuilder.Append("</Picklist>");
                Picklist thePicklist = Picklist.NewPicklist(theStringBuilder.ToString());

                Assert.IsTrue(thePicklist.PickList.Count > 0, "Picklist.NewPicklist(theStringBuilder.ToString()) did not return the expected value.");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        /// <summary>
        /// Adding new PickList item in PickList
        /// </summary>
        [TestMethod]
        public void IncludeValue_AddingPickListItemIntoPickList() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            try
            {
                DataTable dtPickListInfo = DALHelper.ExecuteQuery("SELECT ID, DESCRIPTION FROM REGDB.PICKLISTDOMAIN");

                if (dtPickListInfo != null && dtPickListInfo.Rows.Count > 1)
                {
                    StringBuilder theStringBuilder = new StringBuilder();
                    theStringBuilder.Append("<Picklist id='" + Convert.ToString(dtPickListInfo.Rows[0]["ID"]) + "' count='2'>");
                    theStringBuilder.Append("<PicklistItem ID='101'>Test1</PicklistItem>");
                    theStringBuilder.Append("<PicklistItem ID='102'>Test2</PicklistItem>");
                    theStringBuilder.Append("</Picklist>");
                    Picklist thePicklist = Picklist.NewPicklist(theStringBuilder.ToString());

                    int iPrevPickListCount = thePicklist.PickList.Count;
                    thePicklist.IncludeValue = Convert.ToInt32(Convert.ToString(dtPickListInfo.Rows[1]["ID"]));
                    Assert.AreEqual(iPrevPickListCount + 1, thePicklist.PickList.Count, "Picklist.IncludeValue did not return the expected value.");
                }
                else
                {
                    Assert.Fail("No Picklist found.");
                }
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        /// <summary>
        /// Loading PickList Item ID using Item Value
        /// </summary>
        [TestMethod]
        public void GetListItemIdByValue_LoadingPickListItemIDUsingItemValue() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            try
            {
                DataTable dtPickListInfo = DALHelper.ExecuteQuery("SELECT ID, DESCRIPTION FROM REGDB.PICKLISTDOMAIN");

                if (dtPickListInfo != null && dtPickListInfo.Rows.Count > 0)
                {
                    StringBuilder theStringBuilder = new StringBuilder();
                    theStringBuilder.Append("<Picklist id='" + Convert.ToString(dtPickListInfo.Rows[0]["ID"]) + "' count='2'>");
                    theStringBuilder.Append("<PicklistItem ID='101'>Test1</PicklistItem>");
                    theStringBuilder.Append("<PicklistItem ID='102'>Test2</PicklistItem>");
                    theStringBuilder.Append("</Picklist>");
                    Picklist thePicklist = Picklist.NewPicklist(theStringBuilder.ToString());

                    int iPickListItemID = thePicklist.GetListItemIdByValue("Test1");
                    Assert.IsTrue(iPickListItemID > -1, "Picklist.GetListItemIdByValue('Test1') did not return the expected value.");
                }
                else
                {
                    Assert.Fail("No Picklist found.");
                }
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        /// <summary>
        /// Loading PickList Item Value using Item ID
        /// </summary>
        [TestMethod]
        public void GetListItemValueById_LoadingPickListItemValueUsingItemID() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            try
            {
                DataTable dtPickListInfo = DALHelper.ExecuteQuery("SELECT ID, DESCRIPTION FROM REGDB.PICKLISTDOMAIN");

                if (dtPickListInfo != null && dtPickListInfo.Rows.Count > 0)
                {
                    StringBuilder theStringBuilder = new StringBuilder();
                    theStringBuilder.Append("<Picklist id='" + Convert.ToString(dtPickListInfo.Rows[0]["ID"]) + "' count='2'>");
                    theStringBuilder.Append("<PicklistItem ID='101'>Test1</PicklistItem>");
                    theStringBuilder.Append("<PicklistItem ID='102'>Test2</PicklistItem>");
                    theStringBuilder.Append("</Picklist>");
                    Picklist thePicklist = Picklist.NewPicklist(theStringBuilder.ToString());

                    string strPickListItemValue = thePicklist.GetListItemValueById(101);
                    Assert.AreEqual(strPickListItemValue, "Test1", "Picklist.GetListItemValueById(101) did not return the expected value.");
                }
                else
                {
                    Assert.Fail("No Picklist found.");
                }
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        /// <summary>
        /// Loading PickList Item by Item Description
        /// </summary>
        [TestMethod]
        public void GetPicklistByDescription_LoadingPickListItemByDescription() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            try
            {
                DataTable dtPickListInfo = DALHelper.ExecuteQuery("SELECT ID, DESCRIPTION FROM REGDB.PICKLISTDOMAIN");

                if (dtPickListInfo != null && dtPickListInfo.Rows.Count > 0)
                {
                    Picklist theResult = Picklist.GetPicklistByDescription(Convert.ToString(dtPickListInfo.Rows[0]["DESCRIPTION"]));
                    Assert.IsNotNull(theResult, "Picklist.GetPicklistByDescription(Convert.ToString(dtPickListInfo.Rows[0]['DESCRIPTION'])) did not return the expected value.");
                }
                else
                {
                    Assert.Fail("No Picklist found.");
                }
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        /// <summary>
        /// Loading PickList using SQL Query
        /// </summary>
        [TestMethod]
        public void GetPicklist_LoadingPickListUsingSQLQuery() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            try
            {
                Picklist theResult = Picklist.GetPicklist("SELECT ID, DESCRIPTION FROM REGDB.PICKLISTDOMAIN");
                Assert.IsNotNull(theResult, "Picklist.GetPicklist('SELECT ID, DESCRIPTION FROM REGDB.PICKLISTDOMAIN') did not return the expected value.");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        /// <summary>
        /// Loading PickList using Pick List Domain
        /// </summary>
        [TestMethod]
        public void GetPicklist_LoadingPickListUsingPickListDomain() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            try
            {
                DataTable dtPickListInfo = DALHelper.ExecuteQuery("SELECT ID, DESCRIPTION FROM REGDB.PICKLISTDOMAIN");

                if (dtPickListInfo != null && dtPickListInfo.Rows.Count > 0)
                {
                    PicklistDomain thePicklistDomain = PicklistDomain.GetPicklistDomain(Convert.ToString(dtPickListInfo.Rows[0]["DESCRIPTION"]));
                    Picklist theResult = Picklist.GetPicklist(thePicklistDomain);
                    Assert.IsNotNull(theResult, "Picklist.GetPicklist(thePicklistDomain) did not return the expected value.");
                }
                else
                {
                    Assert.Fail("No Picklist found.");
                }
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        #endregion
    }
}
