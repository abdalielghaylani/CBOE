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
using CambridgeSoft.COE.Registration.Services.RegSystem;
using CambridgeSoft.COE.Framework.COEConfigurationService;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.Controls.WebParts;

namespace CambridgeSoft.COE.Registration.MSUnitTests
{
    /// <summary>
    /// GetAllTempRegistriesCountTest class contains unit test methods for GetAllTempRegistriesCount
    /// </summary>
    [TestClass]
    public class GetAllTempRegistriesCountTest
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

        #region GetAllTempRegistriesCount Test Methods

        /// <summary>
        /// Get the text (first it formats the text) to display into the dashboard
        /// </summary>
        [TestMethod]
        public void GetCustomItem_LoadingFormattedText() 
        {
            try
            {
                GetAllTempRegistriesCount theGetAllTempRegistriesCount = new GetAllTempRegistriesCount();

                ApplicationHome homeData = ConfigurationUtilities.GetApplicationHomeData("REGISTRATION");

                foreach (Group myGroup in homeData.Groups)
                {
                    if (myGroup.PageSectionTarget.ToUpper().Equals("DASHBOARD"))
                    {
                        foreach (CustomItems customItems in myGroup.CustomItems)
                        {
                            Assembly assembly = Assembly.Load(customItems.AssemblyName);

                            ICustomHomeItem customItem = (ICustomHomeItem)assembly.CreateInstance(customItems.ClassName.Trim());

                            theGetAllTempRegistriesCount.SetConfiguration(customItems.Configuration);
                        }
                        break;
                    }
                }

                string strResult = theGetAllTempRegistriesCount.GetCustomItem();

                Assert.IsTrue(!string.IsNullOrEmpty(strResult), "GetAllTempRegistriesCount.GetCustomItem() did not return the expected value.");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        #endregion
    }
}
