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
    /// PrefixNameValueListTest class contains unit test methods for PrefixNameValueList
    /// </summary>
    [TestClass]
    public class PrefixNameValueListTest
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

        #region PrefixNameValueList Test Methods

        /// <summary>
        /// Loading Prefix name value list
        /// </summary>
        [TestMethod]
        public void GetPrefixNameValueList_LoadinPrefixNameValueList() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            PrefixNameValueList thePrefixNameValueList = PrefixNameValueList.GetPrefixNameValueList();

            Assert.IsTrue(thePrefixNameValueList.Count > 0, "PrefixNameValueList.GetPrefixNameValueList() did not return the expected value.");
        }

        /// <summary>
        /// Clearing Prefix name value list
        /// </summary>
        [TestMethod]
        public void InvalidateCache_ClearingnPrefixNameValueList() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            PrefixNameValueList thePrefixNameValueList = PrefixNameValueList.GetPrefixNameValueList();
            
            PrefixNameValueList.InvalidateCache();

            Type type = typeof(PrefixNameValueList);
            FieldInfo info = type.GetField("_list", BindingFlags.NonPublic | BindingFlags.Static);
            object theResult = info.GetValue(null);

            Assert.IsNull(theResult, "PrefixNameValueList.InvalidateCache() did not return the expected value.");
        }

        /// <summary>
        /// Setting and Getting Prefix ID
        /// </summary>
        [TestMethod]
        public void SelectedPrefixID_SettingAndGettingPrefixID() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            PrefixNameValueList thePrefixNameValueList = PrefixNameValueList.GetPrefixNameValueList();
            thePrefixNameValueList.SelectedPrefixID = thePrefixNameValueList[0].Key;
            int iResult = thePrefixNameValueList.SelectedPrefixID;

            Assert.AreEqual(thePrefixNameValueList[0].Key, iResult, "PrefixNameValueList.SelectedPrefixID did not return the expected value.");
        }

        /// <summary>
        /// Setting and Getting Prefix Name
        /// </summary>
        [TestMethod]
        public void SelectedPrefixName_SettingAndGettingPrefixName() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            PrefixNameValueList thePrefixNameValueList = PrefixNameValueList.GetPrefixNameValueList();
            thePrefixNameValueList.SelectedPrefixName = thePrefixNameValueList[0].Value;
            string strResult = thePrefixNameValueList.SelectedPrefixName;

            Assert.AreEqual(thePrefixNameValueList[0].Value, strResult, "PrefixNameValueList.SelectedPrefixName did not return the expected value.");
        }

        #endregion
    }
}
