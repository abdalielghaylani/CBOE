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
using CambridgeSoft.COE.Registration.Core;

namespace CambridgeSoft.COE.Registration.MSUnitTests
{
    /// <summary>
    /// CompoundMatchResponseTest class contains unit test methods for CompoundMatchResponse
    /// </summary>
    [TestClass]
    public class CompoundMatchResponseTest
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

        #region CompoundMatchResponse Test Methods

        /// <summary>
        /// Loading matched compound items
        /// </summary>
        [TestMethod]
        public void MatchedItems_LoadingMatchedCompoundItems() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");
            try
            {
                CompoundMatchResponse theCompoundMatchResponse = new CompoundMatchResponse(new List<string>() { GlobalVariables.REGNUM1, GlobalVariables.REGNUM2 },MatchMechanism.Structure);

                Assert.IsTrue(theCompoundMatchResponse.MatchedItems.Count > 0, "CompoundMatchResponse.CompoundMatchResponse(new List<string>() { GlobalVariables.REGNUM1, GlobalVariables.REGNUM2 }, MatchMechanism.Structure) did not return the expected value.");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        /// <summary>
        /// Loading mechanism used for matching compound
        /// </summary>
        [TestMethod]
        public void MechanismUsed_LoadingMechanismUsedForMatchingCompound() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");
            try
            {
                CompoundMatchResponse theCompoundMatchResponse = new CompoundMatchResponse(new List<string>() { GlobalVariables.REGNUM1, GlobalVariables.REGNUM2 }, MatchMechanism.Structure);

                Assert.IsTrue(!string.IsNullOrEmpty(theCompoundMatchResponse.MechanismUsed.ToString()), "CompoundMatchResponse.CompoundMatchResponse(new List<string>() { GlobalVariables.REGNUM1, GlobalVariables.REGNUM2 }, MatchMechanism.Structure) did not return the expected value.");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        /// <summary>
        /// Loading duplicate compound xml
        /// </summary>
        [TestMethod]
        public void DuplicateXML_LoadingDuplicateMixtureXML() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");
            try
            {
                CompoundMatchesDTO theCompoundMatchesDTO = new CompoundMatchesDTO();
                theCompoundMatchesDTO.Mechanism = 1;

                CompoundMatchesDTO.CompoundMatchDTO theCompoundMatchDTO = new CompoundMatchesDTO.CompoundMatchDTO();
                theCompoundMatchDTO.MixtureRegNumber = GlobalVariables.REGNUM1;
                theCompoundMatchesDTO.Compounds.Add(theCompoundMatchDTO);

                theCompoundMatchDTO = new CompoundMatchesDTO.CompoundMatchDTO();
                theCompoundMatchDTO.MixtureRegNumber = GlobalVariables.REGNUM2;
                theCompoundMatchesDTO.Compounds.Add(theCompoundMatchDTO);

                CompoundMatchResponse theCompoundMatchResponse = new CompoundMatchResponse(theCompoundMatchesDTO);

                Assert.IsTrue(!string.IsNullOrEmpty(theCompoundMatchResponse.DuplicateXML), "CompoundMatchResponse.CompoundMatchResponse(theCompoundMatchesDTO) did not return the expected value.");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        #endregion
    }
}
