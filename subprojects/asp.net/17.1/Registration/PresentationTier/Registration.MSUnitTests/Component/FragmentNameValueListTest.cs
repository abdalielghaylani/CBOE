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
    /// FragmentNameValueListTest class contains unit test methods for FragmentNameValueList
    /// </summary>
    [TestClass]
    public class FragmentNameValueListTest
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

        #region FragmentNameValueList Test Methods

        /// <summary>
        /// Loading Fragment name value list
        /// </summary>
        [TestMethod]
        public void GetFragmentNameValueList_LoadinFragmentNameValueList() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");
            try
            {
                FragmentNameValueList theFragmentNameValueList = FragmentNameValueList.GetFragmentNameValueList();

                Assert.IsTrue(theFragmentNameValueList.Count > 0, "FragmentNameValueList.GetFragmentNameValueList() did not return the expected value.");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        /// <summary>
        /// Loading Fragment type name value list
        /// </summary>
        [TestMethod]
        public void GetFragmentTypesNameValueList_LoadinFragmentTypeNameValueList() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");
            try
            {
                FragmentNameValueList theFragmentNameValueList = FragmentNameValueList.GetFragmentTypesNameValueList();

                Assert.IsTrue(theFragmentNameValueList.Count > 0, "FragmentNameValueList.GetFragmentTypesNameValueList() did not return the expected value.");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        /// <summary>
        /// Clearing Fragment name value list
        /// </summary>
        [TestMethod]
        public void InvalidateCache_ClearingnFragmentNameValueList() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");
            try
            {
                FragmentNameValueList theFragmentNameValueList = FragmentNameValueList.GetFragmentNameValueList();

                FragmentNameValueList.InvalidateCache();

                Type type = typeof(FragmentNameValueList);
                FieldInfo info = type.GetField("_list", BindingFlags.NonPublic | BindingFlags.Static);
                object theResult = info.GetValue(null);

                Assert.IsNull(theResult, "FragmentNameValueList.InvalidateCache() did not return the expected value.");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        /// <summary>
        /// Setting and Getting Fragment ID
        /// </summary>
        [TestMethod]
        public void SelectedFragmentID_SettingAndGettingFragmentID() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");
            try
            {
                FragmentNameValueList theFragmentNameValueList = FragmentNameValueList.GetFragmentNameValueList();
                theFragmentNameValueList.SelectedFragmentID = theFragmentNameValueList[0].Key;
                int iResult = theFragmentNameValueList.SelectedFragmentID;

                Assert.AreEqual(theFragmentNameValueList[0].Key, iResult, "UserNameValueList.SelectedFragmentID did not return the expected value.");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        /// <summary>
        /// Setting and Getting Fragment Name
        /// </summary>
        [TestMethod]
        public void SelectedFragmentName_SettingAndGettingUserName() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            try
            {
                FragmentNameValueList theFragmentNameValueList = FragmentNameValueList.GetFragmentNameValueList();
                theFragmentNameValueList.SelectedFragmentName = theFragmentNameValueList[0].Value;
                string strResult = theFragmentNameValueList.SelectedFragmentName;

                Assert.AreEqual(theFragmentNameValueList[0].Value, strResult, "UserNameValueList.SelectedFragmentName did not return the expected value.");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        #endregion
    }
}
