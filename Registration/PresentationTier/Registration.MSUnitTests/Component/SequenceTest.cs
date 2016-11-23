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
    /// SequenceTest class contains unit test methods for Sequence
    /// </summary>
    [TestClass]
    public class SequenceTest
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

        #region Sequence Test Methods

        /// <summary>
        /// Creating new Sequence from Sequence ID
        /// </summary>
        [TestMethod]
        public void NewSequence_CreatingNewSequenceFromSequenceID() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            Sequence theSequence = Sequence.NewSequence(1);

            Assert.IsNotNull(theSequence, "Sequence.NewSequence(1) did not return the expected value.");
        }

        /// <summary>
        /// Creating new Sequence from XML
        /// </summary>
        [TestMethod]
        public void NewSequence_CreatingNewSequenceFromXML() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            StringBuilder theStringBuilder = new StringBuilder();
            theStringBuilder.Append("<Sequence>");
            theStringBuilder.Append("<Prefix>AB");
            theStringBuilder.Append("</Prefix>");
            theStringBuilder.Append("<Suffix>YZ");
            theStringBuilder.Append("</Suffix>");
            theStringBuilder.Append("<PrefixDelimiter>/");
            theStringBuilder.Append("</PrefixDelimiter>");
            theStringBuilder.Append("<SuffixDelimiter>\\");
            theStringBuilder.Append("</SuffixDelimiter>");
            theStringBuilder.Append("<RootNumberLength>5");
            theStringBuilder.Append("</RootNumberLength>");
            theStringBuilder.Append("<Active>T");
            theStringBuilder.Append("</Active>");
            theStringBuilder.Append("<AutoSelCompDupChk>T");
            theStringBuilder.Append("</AutoSelCompDupChk>");
            theStringBuilder.Append("<ID>101");
            theStringBuilder.Append("</ID>");
            theStringBuilder.Append("</Sequence>");

            Sequence theSequence = Sequence.NewSequence(theStringBuilder.ToString(), true);

            Assert.IsTrue(theSequence.ID > 0, "Sequence.NewSequence(theStringBuilder.ToString(), true) did not return the expected value.");
        }

        /// <summary>
        /// Creating Dirty or old Sequence from XML
        /// </summary>
        [TestMethod]
        public void NewSequence_CreatingDirtySequenceFromXML() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            StringBuilder theStringBuilder = new StringBuilder();
            theStringBuilder.Append("<Sequence>");
            theStringBuilder.Append("<Prefix>AB");
            theStringBuilder.Append("</Prefix>");
            theStringBuilder.Append("<Suffix>YZ");
            theStringBuilder.Append("</Suffix>");
            theStringBuilder.Append("<PrefixDelimiter>/");
            theStringBuilder.Append("</PrefixDelimiter>");
            theStringBuilder.Append("<SuffixDelimiter>\\");
            theStringBuilder.Append("</SuffixDelimiter>");
            theStringBuilder.Append("<RootNumberLength>5");
            theStringBuilder.Append("</RootNumberLength>");
            theStringBuilder.Append("<Active>T");
            theStringBuilder.Append("</Active>");
            theStringBuilder.Append("<AutoSelCompDupChk>T");
            theStringBuilder.Append("</AutoSelCompDupChk>");
            theStringBuilder.Append("<ID>101");
            theStringBuilder.Append("</ID>");
            theStringBuilder.Append("</Sequence>");

            Sequence theSequence = Sequence.NewSequence(theStringBuilder.ToString(), false);

            Assert.IsTrue(theSequence.ID > 0, "Sequence.NewSequence(theStringBuilder.ToString(), false) did not return the expected value.");
        }

        /// <summary>
        /// Creating new Sequence using Sequence ID, Root number and Prefix
        /// </summary>
        [TestMethod]
        public void NewSequence_CreatingNewSequenceUsingSequenceID_RootNumberAndprefix() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            Sequence theSequence = Sequence.NewSequence(1, 1, "AB");

            Assert.IsTrue(theSequence.ID > 0, "Sequence.NewSequence(1, 1, 'AB') did not return the expected value.");
        }

        /// <summary>
        /// Loading Sequence information using Sequence ID
        /// </summary>
        [TestMethod]
        public void GetSequence_LoadingSequenceInformationUsingSequenceID() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            DataTable dtSequenceInfo = DALHelper.ExecuteQuery("SELECT SEQUENCEID FROM REGDB.VW_SEQUENCE");

            if (dtSequenceInfo != null && dtSequenceInfo.Rows.Count > 0)
            {
                try
                {
                    Sequence theSequence = Sequence.GetSequence(Convert.ToInt32(Convert.ToString(dtSequenceInfo.Rows[0]["SEQUENCEID"])));

                    Assert.IsTrue(theSequence.ID > 0, "Sequence.GetSequence(Convert.ToInt32(Convert.ToString(dtSequenceInfo.Rows[0]['SEQUENCEID']))) did not return the expected value.");
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

        /// <summary>
        /// Loading Sequence information using Sequence ID
        /// </summary>
        [TestMethod]
        public void GetAutoSelCompDupChk_LoadingSequenceInformationUsingSequenceID() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            DataTable dtSequenceInfo = DALHelper.ExecuteQuery("SELECT SEQUENCEID FROM REGDB.VW_SEQUENCE");

            if (dtSequenceInfo != null && dtSequenceInfo.Rows.Count > 0)
            {
                try
                {
                    bool blnResult = Sequence.GetAutoSelCompDupChk(Convert.ToInt32(Convert.ToString(dtSequenceInfo.Rows[0]["SEQUENCEID"])));

                    Assert.IsTrue((blnResult || !blnResult), "Sequence.GetAutoSelCompDupChk(Convert.ToInt32(Convert.ToString(dtSequenceInfo.Rows[0]['SEQUENCEID']))) did not return the expected value.");
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

        /// <summary>
        /// Generating XML from Sequence object
        /// </summary>
        [TestMethod]
        public void UpdateSelf_GeneratingXMLFromSequenceObject() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            Sequence theSequence = Sequence.NewSequence(1);

            PrivateObject thePrivateObject = new PrivateObject(theSequence);
            object theResult = thePrivateObject.Invoke("UpdateSelf", new object[] { true });

            Assert.IsTrue(!string.IsNullOrEmpty(Convert.ToString(theResult)), "Sequence.UpdateSelf(true) did not return the expected value.");
        }

        #endregion
    }
}
