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
    /// SequenceListTest class contains unit test methods for SequenceList
    /// </summary>
    [TestClass]
    public class SequenceListTest
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

        #region SequenceList Test Methods

        /// <summary>
        /// Loading Sequence list using Sequence type ID
        /// </summary>
        [TestMethod]
        public void GetSequenceList_LoadingSequenceListUsingSequenceTypeID() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");
            try
            {
                SequenceList theSequenceList = SequenceList.GetSequenceList(1);

                Assert.IsTrue(theSequenceList.Count > 0, "SequenceList.GetSequenceList(1) did not return the expected value.");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }

            //Note:- In the SequenceList class in Fetch(SafeDataReader reader) method "ROOTNUMBERLENGTH" field is used but in the select query of
            //"GETSEQUENCELIST" storedprocedure "ROOTNUMBERLENGTH" not exist. Hence, method throwing an exception
        }

        /// <summary>
        /// Loading Sequence list using Person ID
        /// </summary>
        [TestMethod]
        public void GetSequenceListByPersonID_LoadingSequenceListUsingSequencePersonID() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");
            try
            {
                DataTable dtSequenceInfo = DALHelper.ExecuteQuery("SELECT REGDB.VW_SEQUENCE.SEQUENCEID, PERSON_ID FROM REGDB.VW_SEQUENCE INNER JOIN REGDB.PREFIX_USER ON REGDB.VW_SEQUENCE.SEQUENCEID = REGDB.PREFIX_USER.SEQUENCE_ID");

                if (dtSequenceInfo != null && dtSequenceInfo.Rows.Count > 0)
                {
                    try
                    {
                        SequenceList theSequenceList = SequenceList.GetSequenceListByPersonID(Convert.ToInt32(Convert.ToString(dtSequenceInfo.Rows[0]["SEQUENCEID"])), Convert.ToInt32(Convert.ToString(dtSequenceInfo.Rows[0]["PERSON_ID"])));

                        Assert.IsTrue(theSequenceList.Count > 0, "SequenceList.GetSequenceListByPersonID(Convert.ToInt32(Convert.ToString(dtSequenceInfo.Rows[0]['SEQUENCEID'])), Convert.ToInt32(Convert.ToString(dtSequenceInfo.Rows[0]['PERSON_ID']))) did not return the expected value.");
                    }
                    catch (Exception ex)
                    {
                        Assert.Fail(ex.Message);
                    }
                }
                else
                {
                    Assert.IsTrue((dtSequenceInfo == null && dtSequenceInfo.Rows.Count == 0), "Sequence information does not exist.");
                }
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }

            //Note:- In the SequenceList class in Fetch(SafeDataReader reader) method "ROOTNUMBERLENGTH" field is used but in the select query of
            //"GETSEQUENCELISTBYPERSONID" storedprocedure "ROOTNUMBERLENGTH" not exist. Hence, method throwing an exception
        }

        /// <summary>
        /// Generating XML from Sequence List object
        /// </summary>
        [TestMethod]
        public void UpdateSelf_GeneratingXMLFromSequenceListObject()
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            SequenceList theSequenceList = SequenceList.GetSequenceList(1);

            PrivateObject thePrivateObject = new PrivateObject(theSequenceList);
            object theResult = thePrivateObject.Invoke("UpdateSelf", new object[] { true });

            Assert.IsTrue(!string.IsNullOrEmpty(Convert.ToString(theResult)), "SequenceList.UpdateSelf(true) did not return the expected value.");
        }

        /// <summary>
        /// Loading Sequence list
        /// </summary>
        [TestMethod]
        public void KeyValueList_LoadingSequenceList() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            SequenceList theSequenceList = SequenceList.GetSequenceList(1);

            System.Collections.IDictionary theDictionary = theSequenceList.KeyValueList;

            Assert.IsTrue(theDictionary.Count > 0, "SequenceList.KeyValueList did not return the expected value.");
        }

        #endregion
    }
}
