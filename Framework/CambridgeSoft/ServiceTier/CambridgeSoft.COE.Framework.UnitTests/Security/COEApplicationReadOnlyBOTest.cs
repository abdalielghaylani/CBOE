using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CambridgeSoft.COE.Framework.UnitTests.Helpers;
using CambridgeSoft.COE.Framework.COESecurityService;
using System.Reflection;
using CambridgeSoft.COE.Framework.Common;
using System.Data;

namespace CambridgeSoft.COE.Framework.UnitTests.Security
{
    /// <summary>
    /// Summary description for COEApplicationReadOnlyBOTest
    /// </summary>
    [TestClass]
    public class COEApplicationReadOnlyBOTest
    {

        #region Variables         
        
        private static bool blnAppDetails = false; 

        #endregion

        public COEApplicationReadOnlyBOTest()
        {         
        }

        private TestContext testContextInstance;

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

        #region Additional test attributes
        //
        // Youw can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext) 
        {
            Authentication.Logon();
            blnAppDetails = SecurityHelper.GetApplicationDetails();            

        }
        
        // Use ClassCleanup to run code after all tests in a class have run
        [ClassCleanup()]
        public static void MyClassCleanup()
        {
            Authentication.Logoff();
        }
        
        // Use TestInitialize to run code before running each test 
        [TestInitialize()]
        public void MyTestInitialize() { }
        
        // Use TestCleanup to run code after each test has run
        [TestCleanup()]
        public void MyTestCleanup() { }
    
        #endregion

        #region Test Methods

        [TestMethod]
        public void COEApplicationReadOnlyBO_GetAppId()
        {            
             Type type = typeof(COEApplicationReadOnlyBO);         

             if (blnAppDetails)
             {
                 COEApplicationReadOnlyBO theCOEApplicationReadOnlyBO = (COEApplicationReadOnlyBO)Activator.CreateInstance(type,
                     BindingFlags.NonPublic | BindingFlags.Instance, null, SecurityHelper.argList, null, null);

                 int AppId = theCOEApplicationReadOnlyBO.ID;
                 Assert.AreEqual(SecurityHelper.intAppId, AppId);
             }
             else
                 Assert.Fail("Application does not exist");
           
        }

        [TestMethod]
        public void COEApplicationReadOnlyBO_GetAppName()
        {
            Type type = typeof(COEApplicationReadOnlyBO);
            
            if (blnAppDetails)
            {
                COEApplicationReadOnlyBO theCOEApplicationReadOnlyBO = (COEApplicationReadOnlyBO)Activator.CreateInstance(type,
                    BindingFlags.NonPublic | BindingFlags.Instance, null, SecurityHelper.argList, null, null);

                string AppName = theCOEApplicationReadOnlyBO.ApplicationName;
                Assert.AreEqual(SecurityHelper.strAppName, AppName);
            }
            else
                Assert.Fail("Application does not exist");
        }

        [TestMethod]
        public void GetIdValue()
        {
            Type type = typeof(COEApplicationReadOnlyBO);          
            if (SecurityHelper.GetApplicationDetails())
            {
                COEApplicationReadOnlyBO theCOEApplicationReadOnlyBO = (COEApplicationReadOnlyBO)Activator.CreateInstance(type,
                    BindingFlags.NonPublic | BindingFlags.Instance, null, SecurityHelper.argList, null, null);

                PrivateObject thePrivateObject = new PrivateObject(theCOEApplicationReadOnlyBO);
                object theResult = thePrivateObject.Invoke("GetIdValue", null);
                Assert.AreEqual(SecurityHelper.intAppId, Convert.ToInt32(theResult));
            }
            else
                Assert.Fail("Application does not exist");
        }

        [TestMethod]
        public void SetDatabaseName()
        {
            Type type = typeof(COEApplicationReadOnlyBO);           

            if (blnAppDetails)
            {
                COEApplicationReadOnlyBO theCOEApplicationReadOnlyBO = (COEApplicationReadOnlyBO)Activator.CreateInstance(type,
                    BindingFlags.NonPublic | BindingFlags.Instance, null, SecurityHelper.argList, null, null);

                // Database name is assigned to COEDB (hard coded)
                PrivateObject thePrivateObject = new PrivateObject(theCOEApplicationReadOnlyBO);
                object theResult = thePrivateObject.Invoke("SetDatabaseName", BindingFlags.NonPublic | BindingFlags.Static, null);
                string strDatabase = COEDatabaseName.Get();
                Assert.AreEqual("COEDB", strDatabase);
            }
            else
                Assert.Fail("Application does not exist");
        }

        #endregion
    }
}
