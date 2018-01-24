using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CambridgeSoft.COE.Framework.UnitTests.Helpers;
using CambridgeSoft.COE.Framework.COESecurityService;
using System.Reflection;

namespace CambridgeSoft.COE.Framework.UnitTests.Security
{
    /// <summary>
    /// Summary description for COEPrivilegeBOTest
    /// </summary>
    [TestClass]
    public class COEPrivilegeBOTest
    {
        public COEPrivilegeBOTest()
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
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            Authentication.Logon();
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
        public void Get()
        {
            // In DataPortal_Fetch method, code is commented
            COERoleReadOnlyBO theCOERoleReadOnlyBO = COEPrivilegeBO.Get("COEMANAGER_SEC", "COE_SECMANAGER_PRIVILEGES", "COE_SEC_ADMIN");
            Assert.IsNotNull(theCOERoleReadOnlyBO);
        }

        [TestMethod]
        public void New()
        {
            COEPrivilegeBO theCOEPrivilegeBO = COEPrivilegeBO.New();
            Assert.IsNotNull(theCOEPrivilegeBO);
        }

        [TestMethod]
        public void Save()
        {
            Type type = typeof(COEPrivilegeBO);            

            object argList = new object[] { 63, "Chem1", true, "","" };            
            COEPrivilegeBO theCOEPrivilegeBO = COEPrivilegeBO.New();
            theCOEPrivilegeBO.PrivilegeName = "Chem1";
            theCOEPrivilegeBO.Enabled = true;

            COEPrivilegeBO theNewCOEPrivilegeBO = theCOEPrivilegeBO.Save();
            Assert.IsNotNull(theNewCOEPrivilegeBO);
        }

        [TestMethod]
        public void Save_ForceUpdate()
        {
            Type type = typeof(COEPrivilegeBO);

            object argList = new object[] { 63, "Chem2", true, "", "" };            
            COEPrivilegeBO theCOEPrivilegeBO = COEPrivilegeBO.New();
            theCOEPrivilegeBO.PrivilegeName = "Chem2";
            theCOEPrivilegeBO.Enabled = true;

            COEPrivilegeBO theNewCOEPrivilegeBO = theCOEPrivilegeBO.Save(true);
            Assert.IsNotNull(theNewCOEPrivilegeBO);
        }

        [TestMethod]
        public void Delete()
        {         
            //Code is commented in DataPortal_Delete method
            COEPrivilegeBO.Delete(63);
        }
        

        #endregion

    }
}
