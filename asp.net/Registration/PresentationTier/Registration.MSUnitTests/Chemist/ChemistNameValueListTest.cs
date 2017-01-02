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
    /// ChemistNameValueListTest class contains unit test methods for ChemistNameValueList
    /// </summary>
    [TestClass]
    public class ChemistNameValueListTest
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

        #region ChemistNameValueList Test Methods

        /// <summary>
        /// Loading all Chemist 
        /// </summary>
        [TestMethod]
        public void GetChemistNameValueList_LoadingAllChemist() 
        {
            ChemistNameValueList theChemistNameValueList = ChemistNameValueList.GetChemistNameValueList();

            Assert.IsTrue(theChemistNameValueList.Count > 0, "ChemistNameValueList.GetChemistNameValueList() did not return the expected value.");
        }

        /// <summary>
        /// Loading Active Chemist 
        /// </summary>
        [TestMethod]
        public void GetActiveChemistNameValueList_LoadingActiveChemist()
        {
            ChemistNameValueList theChemistNameValueList = ChemistNameValueList.GetActiveChemistNameValueList();

            Assert.IsTrue(theChemistNameValueList.Count > 0, "ChemistNameValueList.GetActiveChemistNameValueList() did not return the expected value.");
        }

        /// <summary>
        /// Flusing Action Chemist and Checmist list
        /// </summary>
        [TestMethod]
        public void InvalidateCache_ClearTheChemistListAndActiveChemistList() 
        {
            ChemistNameValueList theChemistNameValueList = ChemistNameValueList.GetActiveChemistNameValueList();

            ChemistNameValueList.InvalidateCache();

            Type type = typeof(ChemistNameValueList);
            FieldInfo info = type.GetField("_activeList", BindingFlags.NonPublic | BindingFlags.Static);
            object theResult = info.GetValue(null);
            
            Assert.IsNull(theResult, "ChemistNameValueList.InvalidateCache() did not return the expected value.");
        }

        /// <summary>
        /// Setting and Getting Chemist ID
        /// </summary>
        [TestMethod]
        public void SelectedChemistId_SettingAndGettingChemistID() 
        {
            ChemistNameValueList theChemistNameValueList = ChemistNameValueList.GetActiveChemistNameValueList();

            theChemistNameValueList.SelectedChemistId = Convert.ToInt32(theChemistNameValueList[1].Key);
            int iSelectedChemistId = theChemistNameValueList.SelectedChemistId;

            Assert.AreEqual(Convert.ToInt32(theChemistNameValueList[1].Key), iSelectedChemistId, "ChemistNameValueList.SelectedChemistId did not return the expected value.");
        }

        /// <summary>
        /// Setting and Getting Chemist Name
        /// </summary>
        [TestMethod]
        public void SelectedChemistName_SettingAndGettingChemistName()
        {
            ChemistNameValueList theChemistNameValueList = ChemistNameValueList.GetActiveChemistNameValueList();

            theChemistNameValueList.SelectedChemistName = theChemistNameValueList[1].Value;
            string strSelectedChemistName = theChemistNameValueList.SelectedChemistName;

            Assert.AreEqual(theChemistNameValueList[1].Value, strSelectedChemistName, "ChemistNameValueList.SelectedChemistId did not return the expected value.");
        }

        #endregion
    }
}
