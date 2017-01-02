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

namespace CambridgeSoft.COE.Registration.MSUnitTests
{
    /// <summary>
    /// RegistrationMatchResolverTest class contains unit test methods for RegistrationMatchResolver
    /// </summary>
    [TestClass]
    public class RegistrationMatchResolverTest
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

        #region RegistrationMatchResolver Test Methods

        /// <summary>
        /// Setting and loading registry record
        /// </summary>
        [TestMethod]
        public void Subject_SettingAndLoadingRegistryRecord() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");
            try
            {
                Dictionary<string, string> theKeyValuePair = new Dictionary<string, string>();
                theKeyValuePair.Add("Level", "Component");
                theKeyValuePair.Add("Type", "Identifier");
                theKeyValuePair.Add("Value", "TEST_CASE_IDENTIFIER");
                DALHelper.SetRegistrationSettingsONOrOFF(string.Empty, string.Empty, "ENHANCED_DUPLICATE_SCAN", theKeyValuePair);

                RegistryRecord theRegistryRecord = RegistryHelper.LoadRegistryRecord(GlobalVariables.REGNUM2, GlobalVariables.RegistryLoadType.REGNUM);

                if (theRegistryRecord == null)
                {
                    theRegistryRecord = RegistryHelper.CreateRegistryObject(GlobalVariables.REGNUM2);
                    theRegistryRecord = theRegistryRecord.Register(DuplicateAction.Compound);
                }

                IMatchResponse theMatchResponse = RegistrationMatcher.GetMatches(theRegistryRecord.ComponentList);

                RegistrationMatchResolver theRegistrationMatchResolver = new RegistrationMatchResolver(theRegistryRecord, theMatchResponse);

                Assert.IsNotNull(theRegistrationMatchResolver.Subject, "RegistrationMatchResolver.RegistrationMatchResolver(theRegistryRecord, theMatchResponse) did not return the expected value.");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        /// <summary>
        /// Loading macthed registry record as per match index
        /// </summary>
        [TestMethod]
        public void LoadMatch_LoadingMatchedRegistryRecordAsPerMatchIndex() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");
            try
            {
                Dictionary<string, string> theKeyValuePair = new Dictionary<string, string>();
                theKeyValuePair.Add("Level", "Component");
                theKeyValuePair.Add("Type", "Identifier");
                theKeyValuePair.Add("Value", "TEST_CASE_IDENTIFIER");
                DALHelper.SetRegistrationSettingsONOrOFF(string.Empty, string.Empty, "ENHANCED_DUPLICATE_SCAN", theKeyValuePair);

                RegistryRecord theRegistryRecord = RegistryHelper.LoadRegistryRecord(GlobalVariables.REGNUM2, GlobalVariables.RegistryLoadType.REGNUM);

                if (theRegistryRecord == null)
                {
                    theRegistryRecord = RegistryHelper.CreateRegistryObject(GlobalVariables.REGNUM2);
                    theRegistryRecord = theRegistryRecord.Register(DuplicateAction.Compound);
                }

                IMatchResponse theMatchResponse = RegistrationMatcher.GetMatches(theRegistryRecord.ComponentList);

                int iMatchIdex = 0;
                if (theMatchResponse.MatchedItems.Contains(GlobalVariables.REGNUM1)) 
                {
                    iMatchIdex = theMatchResponse.MatchedItems.FindIndex(m => m.Equals(GlobalVariables.REGNUM1));
                }
                else if (theMatchResponse.MatchedItems.Contains(GlobalVariables.REGNUM2))
                {
                    iMatchIdex = theMatchResponse.MatchedItems.FindIndex(m => m.Equals(GlobalVariables.REGNUM2));
                }

                RegistrationMatchResolver theRegistrationMatchResolver = new RegistrationMatchResolver(theRegistryRecord, theMatchResponse);
                theRegistrationMatchResolver.LoadMatch(iMatchIdex);

                Assert.IsNotNull(theRegistrationMatchResolver.CurrentMatch, "RegistrationMatchResolver.LoadMatch(0) did not return the expected value.");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        /// <summary>
        /// Resolving duplicates from Subject (Subject is a registry record object)
        /// </summary>
        [TestMethod]
        public void ResolveSubject_ResolvingDuplicatesFromSubject() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");
            try
            {
                Dictionary<string, string> theKeyValuePair = new Dictionary<string, string>();
                theKeyValuePair.Add("Level", "Component");
                theKeyValuePair.Add("Type", "Identifier");
                theKeyValuePair.Add("Value", "TEST_CASE_IDENTIFIER");
                DALHelper.SetRegistrationSettingsONOrOFF(string.Empty, string.Empty, "ENHANCED_DUPLICATE_SCAN", theKeyValuePair);

                RegistryRecord theRegistryRecord = RegistryHelper.LoadRegistryRecord(GlobalVariables.REGNUM2, GlobalVariables.RegistryLoadType.REGNUM);

                if (theRegistryRecord == null)
                {
                    theRegistryRecord = RegistryHelper.CreateRegistryObject(GlobalVariables.REGNUM2);
                    theRegistryRecord = theRegistryRecord.Register(DuplicateAction.Compound);
                }

                IMatchResponse theMatchResponse = RegistrationMatcher.GetMatches(theRegistryRecord.ComponentList);

                RegistrationMatchResolver theRegistrationMatchResolver = new RegistrationMatchResolver(theRegistryRecord, theMatchResponse);
                theRegistrationMatchResolver.ResolveSubject(ResolutionAction.AddCompoundBatch, "Unit testing Method", 1);

                Assert.IsNotNull(theRegistrationMatchResolver.CurrentMatch, "RegistrationMatchResolver.ResolveSubject(ResolutionAction.AddCompoundBatch, 'Unit testing Method', 1) did not return the expected value.");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        /// <summary>
        /// Resolving duplicates
        /// </summary>
        [TestMethod]
        public void ResolveSubmission_ResolvingDuplicates() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");
            try
            {
                Dictionary<string, string> theKeyValuePair = new Dictionary<string, string>();
                theKeyValuePair.Add("Level", "Component");
                theKeyValuePair.Add("Type", "Identifier");
                theKeyValuePair.Add("Value", "TEST_CASE_IDENTIFIER");
                DALHelper.SetRegistrationSettingsONOrOFF(string.Empty, string.Empty, "ENHANCED_DUPLICATE_SCAN", theKeyValuePair);

                RegistryRecord theRegistryRecord = RegistryHelper.LoadRegistryRecord(GlobalVariables.REGNUM2, GlobalVariables.RegistryLoadType.REGNUM);

                if (theRegistryRecord == null)
                {
                    theRegistryRecord = RegistryHelper.CreateRegistryObject(GlobalVariables.REGNUM2);
                    theRegistryRecord = theRegistryRecord.Register(DuplicateAction.Compound);
                }

                IMatchResponse theMatchResponse = RegistrationMatcher.GetMatches(theRegistryRecord.ComponentList);

                RegistrationMatchResolver theRegistrationMatchResolver = new RegistrationMatchResolver(theRegistryRecord, theMatchResponse);
                theRegistrationMatchResolver.ResolveSubmission(ResolutionAction.AddCompoundBatch);

                Assert.IsNotNull(theRegistrationMatchResolver.CurrentMatch, "RegistrationMatchResolver.ResolveSubmission(ResolutionAction.AddCompoundBatch) did not return the expected value.");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        #endregion
    }
}
