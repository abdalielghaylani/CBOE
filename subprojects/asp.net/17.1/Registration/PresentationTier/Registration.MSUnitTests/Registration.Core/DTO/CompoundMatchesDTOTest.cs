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
    /// CompoundMatchesDTOTest class contains unit test methods for CompoundMatchesDTO
    /// </summary>
    [TestClass]
    public class CompoundMatchesDTOTest
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

        #region CompoundMatchesDTO Test Methods

        /// <summary>
        /// Loading CompoundMatchesDTO object from XML
        /// </summary>
        [TestMethod]
        public void GetResponse_LoadingCompoundMatchesDTOObjectFromXML() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            try
            {
                StringBuilder theStringBuilder = new StringBuilder();
                theStringBuilder.Append("<matches uniqueComps ='1' mechanism='1'>");
                theStringBuilder.Append("<match mixtureRegId='1' mixtureRegNum='CS101' compoundRegId='101' compoundRegNum='AB-10001' structureId='1'>");
                theStringBuilder.Append("</match>");
                theStringBuilder.Append("</matches>");

                CompoundMatchesDTO theCompoundMatchesDTO = CompoundMatchesDTO.GetResponse(theStringBuilder.ToString());

                Assert.IsNotNull(theCompoundMatchesDTO, "CompoundMatchesDTO.GetResponse(theStringBuilder.ToString()) did not return the expected value.");

                Assert.IsTrue(theCompoundMatchesDTO.UniqueComponents > 0, "CompoundMatchesDTO.GetResponse(theStringBuilder.ToString()) did not return the expected value.");
                Assert.IsTrue(theCompoundMatchesDTO.Mechanism > 0, "CompoundMatchesDTO.GetResponse(theStringBuilder.ToString()) did not return the expected value.");

                Assert.IsTrue(theCompoundMatchesDTO.Compounds[0].MixtureRegistrationId > 0, "CompoundMatchesDTO.GetResponse(theStringBuilder.ToString()) did not return the expected value.");
                Assert.IsTrue(!string.IsNullOrEmpty(theCompoundMatchesDTO.Compounds[0].MixtureRegNumber), "CompoundMatchesDTO.GetResponse(theStringBuilder.ToString()) did not return the expected value.");
                Assert.IsTrue(theCompoundMatchesDTO.Compounds[0].CompoundRegistrationId > 0, "CompoundMatchesDTO.GetResponse(theStringBuilder.ToString()) did not return the expected value.");
                Assert.IsTrue(!string.IsNullOrEmpty(theCompoundMatchesDTO.Compounds[0].CompoundRegNumber), "CompoundMatchesDTO.GetResponse(theStringBuilder.ToString()) did not return the expected value.");
                Assert.IsTrue(theCompoundMatchesDTO.Compounds[0].StructureId > 0, "CompoundMatchesDTO.GetResponse(theStringBuilder.ToString()) did not return the expected value.");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        /// <summary>
        /// Converting CompoundMatchesDTO object into a XML
        /// </summary>
        [TestMethod]
        public void ToXml_SavingObjectIntoXML() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            try
            {
                StringBuilder theStringBuilder = new StringBuilder();
                theStringBuilder.Append("<matches uniqueComps ='1' mechanism='1'>");
                theStringBuilder.Append("<match mixtureRegId='1' mixtureRegNum='CS101' compoundRegId='101' compoundRegNum='AB-10001' structureId='1'>");
                theStringBuilder.Append("</match>");
                theStringBuilder.Append("</matches>");

                CompoundMatchesDTO theCompoundMatchesDTO = CompoundMatchesDTO.GetResponse(theStringBuilder.ToString());
                string strXML = theCompoundMatchesDTO.ToXml();
                Assert.IsNotNull(!string.IsNullOrEmpty(strXML), "CompoundMatchesDTO.ToXml() did not return the expected value.");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        /// <summary>
        /// Setting Compound list
        /// </summary>
        [TestMethod]
        public void Compounds_SettingCompounds() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            try
            {
                StringBuilder theStringBuilder = new StringBuilder();
                theStringBuilder.Append("<matches uniqueComps ='1' mechanism='1'>");
                theStringBuilder.Append("<match mixtureRegId='1' mixtureRegNum='CS101' compoundRegId='101' compoundRegNum='AB-10001' structureId='1'>");
                theStringBuilder.Append("</match>");
                theStringBuilder.Append("</matches>");

                CompoundMatchesDTO theCompoundMatchesDTO = CompoundMatchesDTO.GetResponse(theStringBuilder.ToString());

                List<CompoundMatchesDTO.CompoundMatchDTO> lstCompoundMatchDTO = new List<CompoundMatchesDTO.CompoundMatchDTO>();

                CompoundMatchesDTO.CompoundMatchDTO theCompoundMatchDTO = new CompoundMatchesDTO.CompoundMatchDTO();
                theCompoundMatchDTO.MixtureRegNumber = GlobalVariables.REGNUM1;
                lstCompoundMatchDTO.Add(theCompoundMatchDTO);

                theCompoundMatchDTO = new CompoundMatchesDTO.CompoundMatchDTO();
                theCompoundMatchDTO.MixtureRegNumber = GlobalVariables.REGNUM2;
                lstCompoundMatchDTO.Add(theCompoundMatchDTO);

                theCompoundMatchesDTO.Compounds = lstCompoundMatchDTO;
                Assert.IsTrue(theCompoundMatchesDTO.Compounds.Count == 2, "CompoundMatchesDTO.Compounds did not return the expected value.");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        #endregion
    }
}
