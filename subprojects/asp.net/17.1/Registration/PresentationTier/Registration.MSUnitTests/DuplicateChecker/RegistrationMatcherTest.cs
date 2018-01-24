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
    /// RegistrationMatcherTest class contains unit test methods for RegistrationMatcher
    /// </summary>
    [TestClass]
    public class RegistrationMatcherTest
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

        #region RegistrationMatcher Test Methods

        /// <summary>
        /// Loading duplicate mixture instance from database as per ComponentList
        /// </summary>
        [TestMethod]
        public void GetMatches_LoadingDuplicateMixtureInstanceExistByComponentList() 
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

                Assert.IsTrue(theMatchResponse.MatchedItems.Count > 0, "RegistrationMatcher.GetMatches(theRegistryRecord.ComponentList) did not return the expected value.");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        /// <summary>
        /// Loading duplicate mixture instance from database as per Compound
        /// </summary>
        [TestMethod]
        public void GetMatches_LoadingDuplicateMixtureInstanceExistByCompound() 
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

                IMatchResponse theMatchResponse = RegistrationMatcher.GetMatches(theRegistryRecord.ComponentList[0].Compound);

                Assert.IsTrue(theMatchResponse.MatchedItems.Count > 0, "RegistrationMatcher.GetMatches(theRegistryRecord.ComponentList[0].Compound) did not return the expected value.");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        /// <summary>
        /// Loading duplicate mixture instance from database as per Registry record
        /// </summary>
        [TestMethod]
        public void GetMatches_LoadingDuplicateMixtureInstanceExistByRegistryRecord() 
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

                theRegistryRecord.ComponentList[0].Compound.BaseFragment.Structure.DrawingType = DrawingType.NonChemicalContent;
                IdentifierList idList = IdentifierList.GetIdentifierListByType(IdentifierTypeEnum.C, true, true);
                if (idList.Count > 0)
                {
                    Identifier theIdentifier = Identifier.NewIdentifier();
                    theIdentifier.Active = true;
                    theIdentifier.Description = "Test case Identifier";
                    theIdentifier.InputText = "Test case Identifier";
                    theIdentifier.Name = "TEST_CASE_IDENTIFIER";
                    theIdentifier.IdentifierID = idList[0].IdentifierID;
                    theRegistryRecord.ComponentList[0].Compound.AddIdentifier(theIdentifier);
                }
                else
                {
                    Assert.AreEqual(idList.Count > 0, "Identifier list is empty.");
                }

                IMatchResponse theMatchResponse = RegistrationMatcher.GetMatches(theRegistryRecord);

                Assert.IsTrue(theMatchResponse.MatchedItems.Count > 0, "RegistrationMatcher.GetMatches(theRegistryRecord) did not return the expected value.");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        /// <summary>
        /// Loading duplicate mixture instance from database as per Compound ID list
        /// </summary>
        [TestMethod]
        public void GetMatches_LoadingDuplicateMixtureInstanceExistByCompoundID() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");
            try
            {
                RegistryRecord theRegistryRecord = RegistryHelper.LoadRegistryRecord(GlobalVariables.REGNUM2, GlobalVariables.RegistryLoadType.REGNUM);

                if (theRegistryRecord == null)
                {
                    theRegistryRecord = RegistryHelper.CreateRegistryObject(GlobalVariables.REGNUM2);
                    theRegistryRecord = theRegistryRecord.Register(DuplicateAction.Compound);
                }

                Component theComponent = Component.NewComponent();
                theComponent.ComponentIndex = theRegistryRecord.ComponentList[0].ComponentIndex;
                theComponent.Percentage = 100;
                theRegistryRecord.AddComponent(theComponent);

                List<int> lstCompoundIDs = new List<int>() { theRegistryRecord.ComponentList[0].Compound.ID, 2 };
                IMatchResponse theMatchResponse = RegistrationMatcher.GetMatches(lstCompoundIDs);

                Assert.IsTrue(theMatchResponse.MatchedItems.Count > 0, "RegistrationMatcher.GetMatches(lstCompoundIDs) did not return the expected value.");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        /// <summary>
        /// Loading duplicate mixture instance from database as per Registry record with non chemical structure
        /// </summary>
        [TestMethod]
        public void GetMatches_LoadingDuplicateMixtureInstanceExistByRegistryRecordWithNonChemicalStructure() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");
            try
            {
                Dictionary<string, string> theKeyValuePair = new Dictionary<string, string>();
                theKeyValuePair.Add("Level", "Component");
                theKeyValuePair.Add("Type", "Property");
                theKeyValuePair.Add("Value", "CMP_COMMENTS");
                DALHelper.SetRegistrationSettingsONOrOFF(string.Empty, string.Empty, "ENHANCED_DUPLICATE_SCAN", theKeyValuePair);

                RegistryRecord theRegistryRecord = RegistryHelper.CreateRegistryObject(GlobalVariables.REGNUM2);
                theRegistryRecord.ComponentList[0].Compound.BaseFragment.Structure.DrawingType = DrawingType.NonChemicalContent;

                IMatchResponse theMatchResponse = RegistrationMatcher.GetMatches(theRegistryRecord);

                Assert.IsTrue(theMatchResponse.MatchedItems.Count > 0, "RegistrationMatcher.GetMatches(theRegistryRecord) did not return the expected value.");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        #endregion
    }
}
