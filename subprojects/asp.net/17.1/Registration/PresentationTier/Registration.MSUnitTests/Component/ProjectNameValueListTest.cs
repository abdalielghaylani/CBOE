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
    /// ProjectNameValueListTest class contains unit test methods for ProjectNameValueList
    /// </summary>
    [TestClass]
    public class ProjectNameValueListTest
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

        #region ProjectNameValueList Test Methods

        /// <summary>
        /// Loading Project name value list
        /// </summary>
        [TestMethod]
        public void GetProjectNameValueList_LoadinProjectNameValueList() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            ProjectNameValueList theProjectNameValueList = ProjectNameValueList.GetProjectNameValueList();

            Assert.IsTrue(theProjectNameValueList.Count > 0, "PrefixNameValueList.GetProjectNameValueList() did not return the expected value.");
        }

        /// <summary>
        /// Clearing Project name value list
        /// </summary>
        [TestMethod]
        public void InvalidateCache_ClearingnProjectNameValueList() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            ProjectNameValueList theProjectNameValueList = ProjectNameValueList.GetProjectNameValueList();

            ProjectNameValueList.InvalidateCache();

            Type type = typeof(ProjectNameValueList);
            FieldInfo info = type.GetField("_list", BindingFlags.NonPublic | BindingFlags.Static);
            object theResult = info.GetValue(null);

            Assert.IsNull(theResult, "ProjectNameValueList.InvalidateCache() did not return the expected value.");
        }

        /// <summary>
        /// Setting and Getting Project ID
        /// </summary>
        [TestMethod]
        public void SelectedProjectID_SettingAndGettingProjectID() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            ProjectNameValueList theProjectNameValueList = ProjectNameValueList.GetProjectNameValueList();
            theProjectNameValueList.SelectedProjectID = theProjectNameValueList[0].Key;
            int iResult = theProjectNameValueList.SelectedProjectID;

            Assert.AreEqual(theProjectNameValueList[0].Key, iResult, "ProjectNameValueList.SelectedProjectID did not return the expected value.");
        }

        /// <summary>
        /// Setting and Getting Project Name
        /// </summary>
        [TestMethod]
        public void SelectedProjectName_SettingAndGettingPojectName() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            ProjectNameValueList theProjectNameValueList = ProjectNameValueList.GetProjectNameValueList();
            theProjectNameValueList.SelectedProjectName = theProjectNameValueList[0].Value;
            string strResult = theProjectNameValueList.SelectedProjectName;

            Assert.AreEqual(theProjectNameValueList[0].Value, strResult, "ProjectNameValueList.SelectedProjectName did not return the expected value.");
        }

        #endregion
    }
}
