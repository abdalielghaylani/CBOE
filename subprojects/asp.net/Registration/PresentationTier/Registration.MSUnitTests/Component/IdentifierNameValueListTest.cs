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
    /// IdentifierNameValueListTest class contains unit test methods for IdentifierNameValueList
    /// </summary>
    [TestClass]
    public class IdentifierNameValueListTest
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

        #region IdentifierNameValueList Test Methods

        /// <summary>
        /// Loading Identifier name value list
        /// </summary>
        [TestMethod]
        public void GetIdentifierNameValueList_LoadinIdentifierNameValueList() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");
            try
            {
                IdentifierNameValueList theIdentifierNameValueList = IdentifierNameValueList.GetIdentifierNameValueList();

                Assert.IsTrue(theIdentifierNameValueList.Count > 0, "IdentifierNameValueList.GetIdentifierNameValueList() did not return the expected value.");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        /// <summary>
        /// Clearing Identifier name value list
        /// </summary>
        [TestMethod]
        public void InvalidateCache_ClearingnIdentifierNameValueList() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");
            try
            {
                IdentifierNameValueList theIdentifierNameValueList = IdentifierNameValueList.GetIdentifierNameValueList();

                IdentifierNameValueList.InvalidateCache();

                Type type = typeof(IdentifierNameValueList);
                FieldInfo info = type.GetField("_list", BindingFlags.NonPublic | BindingFlags.Static);
                object theResult = info.GetValue(null);

                Assert.IsNull(theResult, "IdentifierNameValueList.InvalidateCache() did not return the expected value.");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        /// <summary>
        /// Setting and Getting Identifier ID
        /// </summary>
        [TestMethod]
        public void SelectedIdentifierID_SettingAndGettingIdentifierID() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");
            try
            {
                IdentifierNameValueList theIdentifierNameValueList = IdentifierNameValueList.GetIdentifierNameValueList();
                theIdentifierNameValueList.SelectedIdentifierID = theIdentifierNameValueList[0].Key;
                int iResult = theIdentifierNameValueList.SelectedIdentifierID;

                Assert.AreEqual(theIdentifierNameValueList[0].Key, iResult, "IdentifierNameValueList.SelectedIdentifierID did not return the expected value.");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        /// <summary>
        /// Setting and Getting Identifier Name
        /// </summary>
        [TestMethod]
        public void SelectedIdentifierName_SettingAndGettingIdentifierName() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            try
            {
                IdentifierNameValueList theIdentifierNameValueList = IdentifierNameValueList.GetIdentifierNameValueList();
                theIdentifierNameValueList.SelectedIdentifierName = theIdentifierNameValueList[0].Value;
                string strResult = theIdentifierNameValueList.SelectedIdentifierName;

                Assert.AreEqual(theIdentifierNameValueList[0].Value, strResult, "IdentifierNameValueList.SelectedIdentifierName did not return the expected value.");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        #endregion
    }
}
