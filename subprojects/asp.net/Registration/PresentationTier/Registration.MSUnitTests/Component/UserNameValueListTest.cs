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
    /// UserNameValueListTest class contains unit test methods for UserNameValueList
    /// </summary>
    [TestClass]
    public class UserNameValueListTest
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

        #region UserNameValueList Test Methods

        /// <summary>
        /// Loading User name value list
        /// </summary>
        [TestMethod]
        public void GetUserNameValueList_LoadinUserNameValueList() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");
            try
            {
                UserNameValueList theUserNameValueList = UserNameValueList.GetUserNameValueList();

                Assert.IsTrue(theUserNameValueList.Count > 0, "UserNameValueList.GetUserNameValueList() did not return the expected value.");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        /// <summary>
        /// Clearing User name value list
        /// </summary>
        [TestMethod]
        public void InvalidateCache_ClearingnUserNameValueList() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");
            try
            {
                UserNameValueList theUserNameValueList = UserNameValueList.GetUserNameValueList();

                UserNameValueList.InvalidateCache();

                Type type = typeof(UserNameValueList);
                FieldInfo info = type.GetField("_list", BindingFlags.NonPublic | BindingFlags.Static);
                object theResult = info.GetValue(null);

                Assert.IsNull(theResult, "UserNameValueList.InvalidateCache() did not return the expected value.");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        /// <summary>
        /// Setting and Getting User ID
        /// </summary>
        [TestMethod]
        public void SelectedUserID_SettingAndGettingUserID()
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");
            try
            {
                UserNameValueList theUserNameValueList = UserNameValueList.GetUserNameValueList();
                theUserNameValueList.SelectedUserID = theUserNameValueList[0].Key;
                int iResult = theUserNameValueList.SelectedUserID;

                Assert.AreEqual(theUserNameValueList[0].Key, iResult, "UserNameValueList.SelectedUserID did not return the expected value.");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        /// <summary>
        /// Setting and Getting User Name
        /// </summary>
        [TestMethod]
        public void SelectedUserName_SettingAndGettingUserName() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            try
            {
                UserNameValueList theUserNameValueList = UserNameValueList.GetUserNameValueList();
                theUserNameValueList.SelectedUserName = theUserNameValueList[0].Value;
                string strResult = theUserNameValueList.SelectedUserName;

                Assert.AreEqual(theUserNameValueList[0].Value, strResult, "UserNameValueList.SelectedUserName did not return the expected value.");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        #endregion
    }
}
