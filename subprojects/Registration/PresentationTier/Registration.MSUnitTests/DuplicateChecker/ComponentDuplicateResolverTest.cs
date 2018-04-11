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
    /// ComponentDuplicateResolverTest class contains unit test methods for ComponentDuplicateResolver
    /// </summary>
    [TestClass]
    public class ComponentDuplicateResolverTest
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

        #region ComponentDuplicateResolver Test Methods

        /// <summary>
        /// Loading duplicate registry recrods as per Batch
        /// </summary>
        [TestMethod]
        public void ResolveUsingAction_LoadingDuplicateRegistryRecordsAsPerBatch() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");
            try
            {
                RegistryRecord theRegistryRecord = RegistryHelper.LoadRegistryRecord(GlobalVariables.REGNUM2,GlobalVariables.RegistryLoadType.REGNUM);
                if(theRegistryRecord==null)
                {
                    theRegistryRecord = RegistryHelper.CreateRegistryObject(GlobalVariables.REGNUM2);
                    theRegistryRecord = theRegistryRecord.Register(DuplicateAction.Compound);
                }

                List<DuplicateCheckResponse> lstDuplicateCheckResponse = RegistryRecord.FindDuplicates(theRegistryRecord,RegistryRecord.DataAccessStrategy.Atomic);
                
                DuplicateCheckResponse.MatchedRegistration theMatchedRegistration = new DuplicateCheckResponse.MatchedRegistration();
                theMatchedRegistration.RegistryNumber = GlobalVariables.REGNUM2;
                lstDuplicateCheckResponse[0].MatchedRegistrations.Add(theMatchedRegistration);

                RegistryRecord theResult = ComponentDuplicateResolver.ResolveUsingAction(theRegistryRecord, DuplicateAction.Batch, lstDuplicateCheckResponse);

                Assert.IsNotNull(theResult, "ComponentDuplicateResolver.ResolveUsingAction(theRegistryRecord, DuplicateAction.Batch, lstDuplicateCheckResponse) did not return the expected value.");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        #endregion
    }
}
