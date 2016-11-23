using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CambridgeSoft.COE.Framework.COESecurityService;
using CambridgeSoft.COE.Framework.UnitTests.Helpers;
using System.Data;

namespace CambridgeSoft.COE.Framework.UnitTests.Security
{
    /// <summary>
    /// Summary description for COEUserBOTest
    /// </summary>
    [TestClass]
    public class COEUserBOTest
    {
                

        public COEUserBOTest()
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
        public void ChangePassword()
        {
            bool blnSuccess = COEUserBO.ChangePassword("T4_85", "T4_85", "T4_85N");
            Assert.IsTrue(blnSuccess);
        }

        [TestMethod]
        public void Get()
        {            
            if (SecurityHelper.GetUserDetails())
            {
                COEUserBO theCOEUserBO = COEUserBO.Get(SecurityHelper.strUserID);
                Assert.IsNotNull(theCOEUserBO);                
            }
        }

        [TestMethod]
        public void GetLDAPUser()        
        {
            if (SecurityHelper.GetUserDetails())
            {
                COEUserBO theCOEUserBO = COEUserBO.GetLDAPUser(SecurityHelper.strUserID);
                //COEUserBO theCOEUserBO = COEUserBO.GetLDAPUser("rgupte@galaxy.in");
               Assert.IsNotNull(theCOEUserBO);                
            }
        }

        [TestMethod]
        public void GetUserByID()
        {
            if (SecurityHelper.GetUserDetails())
            {
                COEUserBO theCOEUserBO = COEUserBO.GetUserByID(SecurityHelper.intPersonId);
                Assert.IsNotNull(theCOEUserBO);
            }
        }

        [TestMethod]
        public void New()
        {            
            COEUserBO theCOEUserBO = COEUserBO.New();
            Assert.IsNotNull(theCOEUserBO);         
        }


        [TestMethod]
        public void Delete()
        {
            if (SecurityHelper.GetUserDetails())
            {
                //COEUserBO.Delete(SecurityHelper.strUserID);       
                COEUserBO.Delete("RIDDHI");                
            }
            
        }

        [TestMethod]
        public void Activate()
        {
            if (SecurityHelper.GetUserDetails())
            {
                COEUserBO theCOEUserBO = COEUserBO.GetUserByID(SecurityHelper.intPersonId);
                theCOEUserBO.Activate();                
            }

        }

        #endregion Test Methods
    }
}
