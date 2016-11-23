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
using CambridgeSoft.COE.Registration.Services;
using CambridgeSoft.COE.Registration.Core;

namespace CambridgeSoft.COE.Registration.MSUnitTests
{
    /// <summary>
    /// MixtureMatchesDTOTest class contains unit test methods for MixtureMatchesDTO
    /// </summary>
    [TestClass]
    public class MixtureMatchesDTOTest
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

        #region MixtureMatchesDTO Test Methods

        /// <summary>
        /// Loading MixtureMatchesDTO object from XML
        /// </summary>
        [TestMethod]
        public void GetResponse_LoadingMixtureMatchesDTOObjectFromXML() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            try
            {
                StringBuilder theStringBuilder = new StringBuilder();
                theStringBuilder.Append("<matches uniqueMixtures ='1' mechanism='1'>");
                theStringBuilder.Append("<match mixtureRegId='1' mixtureRegNum='CS101'>");
                theStringBuilder.Append("<compound compoundRegId='101' compoundRegNum='AB-10001' structureId='1'>");
                theStringBuilder.Append("</compound>");
                theStringBuilder.Append("</match>");
                theStringBuilder.Append("</matches>");

                MixtureMatchesDTO theMixtureMatchesDTO = MixtureMatchesDTO.GetResponse(theStringBuilder.ToString());

                Assert.IsNotNull(theMixtureMatchesDTO, "MixtureMatchesDTO.GetResponse(theStringBuilder.ToString()) did not return the expected value.");

                Assert.IsTrue(theMixtureMatchesDTO.MatchCount > 0, "MixtureMatchesDTO.GetResponse(theStringBuilder.ToString()) did not return the expected value.");
                Assert.IsTrue(theMixtureMatchesDTO.Mechanism > 0, "MixtureMatchesDTO.GetResponse(theStringBuilder.ToString()) did not return the expected value.");

                Assert.IsTrue(theMixtureMatchesDTO.Mixtures[0].MixtureRegistrationId > 0, "MixtureMatchesDTO.GetResponse(theStringBuilder.ToString()) did not return the expected value.");
                Assert.IsTrue(!string.IsNullOrEmpty(theMixtureMatchesDTO.Mixtures[0].MixtureRegNumber), "MixtureMatchesDTO.GetResponse(theStringBuilder.ToString()) did not return the expected value.");

                Assert.IsTrue(theMixtureMatchesDTO.Mixtures[0].Compounds[0].CompoundRegistrationId > 0, "MixtureMatchesDTO.GetResponse(theStringBuilder.ToString()) did not return the expected value.");
                Assert.IsTrue(!string.IsNullOrEmpty(theMixtureMatchesDTO.Mixtures[0].Compounds[0].CompoundRegNumber), "MixtureMatchesDTO.GetResponse(theStringBuilder.ToString()) did not return the expected value.");
                Assert.IsTrue(theMixtureMatchesDTO.Mixtures[0].Compounds[0].StructureId > 0, "MixtureMatchesDTO.GetResponse(theStringBuilder.ToString()) did not return the expected value.");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        /// <summary>
        /// Converting MixtureMatchesDTO object into a XML
        /// </summary>
        [TestMethod]
        public void ToXml_SavingObjectIntoXML() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            try
            {
                StringBuilder theStringBuilder = new StringBuilder();
                theStringBuilder.Append("<matches uniqueMixtures ='1' mechanism='1'>");
                theStringBuilder.Append("<match mixtureRegId='1' mixtureRegNum='CS101'>");
                theStringBuilder.Append("<compound compoundRegId='101' compoundRegNum='AB-10001' structureId='1'>");
                theStringBuilder.Append("</compound>");
                theStringBuilder.Append("</match>");
                theStringBuilder.Append("</matches>");

                MixtureMatchesDTO theMixtureMatchesDTO = MixtureMatchesDTO.GetResponse(theStringBuilder.ToString());
                string strXML = theMixtureMatchesDTO.ToXml();

                Assert.IsNotNull(!string.IsNullOrEmpty(strXML), "MixtureMatchesDTO.ToXml() did not return the expected value.");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        /// <summary>
        /// Setting Mixture list
        /// </summary>
        [TestMethod]
        public void Mixtures_SettingMixtures() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            try
            {
                StringBuilder theStringBuilder = new StringBuilder();
                theStringBuilder.Append("<matches uniqueMixtures ='1' mechanism='1'>");
                theStringBuilder.Append("<match mixtureRegId='1' mixtureRegNum='CS101'>");
                theStringBuilder.Append("<compound compoundRegId='101' compoundRegNum='AB-10001' structureId='1'>");
                theStringBuilder.Append("</compound>");
                theStringBuilder.Append("</match>");
                theStringBuilder.Append("</matches>");

                MixtureMatchesDTO theMixtureMatchesDTO = MixtureMatchesDTO.GetResponse(theStringBuilder.ToString());

                List<MixtureMatchesDTO.MixtureMatchDTO> lstMixtureMatchDTO = new List<MixtureMatchesDTO.MixtureMatchDTO>();

                List<MixtureMatchesDTO.MixtureCompoundDTO> lstCompoundMatchDTO = new List<MixtureMatchesDTO.MixtureCompoundDTO>();
                MixtureMatchesDTO.MixtureCompoundDTO theMixtureCompoundDTO = new MixtureMatchesDTO.MixtureCompoundDTO();
                theMixtureCompoundDTO.CompoundRegNumber = GlobalVariables.REGNUM1;
                lstCompoundMatchDTO.Add(theMixtureCompoundDTO);
                theMixtureCompoundDTO = new MixtureMatchesDTO.MixtureCompoundDTO();
                theMixtureCompoundDTO.CompoundRegNumber = GlobalVariables.REGNUM2;
                lstCompoundMatchDTO.Add(theMixtureCompoundDTO);
                
                MixtureMatchesDTO.MixtureMatchDTO theMixtureMatchDTO = new MixtureMatchesDTO.MixtureMatchDTO(); 
                theMixtureMatchDTO.Compounds = lstCompoundMatchDTO;

                lstMixtureMatchDTO.Add(theMixtureMatchDTO);

                theMixtureMatchesDTO.Mixtures = lstMixtureMatchDTO;

                Assert.IsTrue(theMixtureMatchesDTO.Mixtures[0].Compounds.Count == 2, "MixtureMatchesDTO.Mixtures did not return the expected value.");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        #endregion
    }
}
