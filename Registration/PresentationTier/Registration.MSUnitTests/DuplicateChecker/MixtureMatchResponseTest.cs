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
    /// MixtureMatchResponseTest class contains unit test methods for MixtureMatchResponse
    /// </summary>
    [TestClass]
    public class MixtureMatchResponseTest
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
        /// Loading matched mixture items
        /// </summary>
        [TestMethod]
        public void MatchedItems_LoadingMatchedMixtureItems() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");
            try
            {
                MixtureMatchResponse theMixtureMatchResponse = new MixtureMatchResponse(new List<string>() { GlobalVariables.REGNUM1, GlobalVariables.REGNUM2 });

                Assert.IsTrue(theMixtureMatchResponse.MatchedItems.Count > 0, "MixtureMatchResponse.MixtureMatchResponse(new List<string>(){GlobalVariables.REGNUM1, GlobalVariables.REGNUM2}) did not return the expected value.");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        /// <summary>
        /// Loading mechanism used for matching mixture
        /// </summary>
        [TestMethod]
        public void MechanismUsed_LoadingMechanismUsedForMatchingMixture() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");
            try
            {
                MixtureMatchResponse theMixtureMatchResponse = new MixtureMatchResponse(new List<string>() { GlobalVariables.REGNUM1, GlobalVariables.REGNUM2 });

                Assert.IsTrue(!string.IsNullOrEmpty(theMixtureMatchResponse.MechanismUsed.ToString()), "MixtureMatchResponse.MixtureMatchResponse(new List<string>(){GlobalVariables.REGNUM1, GlobalVariables.REGNUM2}) did not return the expected value.");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        /// <summary>
        /// Loading duplicate mixture xml
        /// </summary>
        [TestMethod]
        public void DuplicateXML_LoadingDuplicateMixtureXML() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");
            try
            {
                MixtureMatchesDTO theMixtureMatchesDTO = new MixtureMatchesDTO();
                theMixtureMatchesDTO.Mechanism = 1;

                MixtureMatchesDTO.MixtureMatchDTO theMixtureMatchDTO = new MixtureMatchesDTO.MixtureMatchDTO();
                theMixtureMatchDTO.MixtureRegNumber = GlobalVariables.REGNUM1;
                theMixtureMatchesDTO.Mixtures.Add(theMixtureMatchDTO);

                theMixtureMatchDTO = new MixtureMatchesDTO.MixtureMatchDTO();
                theMixtureMatchDTO.MixtureRegNumber = GlobalVariables.REGNUM2;
                theMixtureMatchesDTO.Mixtures.Add(theMixtureMatchDTO);

                MixtureMatchResponse theMixtureMatchResponse = new MixtureMatchResponse(theMixtureMatchesDTO);

                Assert.IsTrue(!string.IsNullOrEmpty(theMixtureMatchResponse.DuplicateXML), "MixtureMatchResponse.MixtureMatchResponse(theMixtureMatchesDTO) did not return the expected value.");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        #endregion
    }
}
