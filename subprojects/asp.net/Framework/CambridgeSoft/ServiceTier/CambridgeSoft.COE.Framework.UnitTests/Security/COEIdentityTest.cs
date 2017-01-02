using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CambridgeSoft.COE.Framework.UnitTests.Helpers;
using CambridgeSoft.COE.Framework.COESecurityService;
using System.Reflection;
using System.Configuration;

namespace CambridgeSoft.COE.Framework.UnitTests.Security
{
    /// <summary>
    /// Summary description for COEIdentityTest
    /// </summary>
    [TestClass]
    public class COEIdentityTest
    {
        public COEIdentityTest()
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

        [TestMethod]
        public void GetIdentity()
        {            
            Type type = typeof(COEIdentity);
            COEIdentity theCOEIdentity = (COEIdentity)Activator.CreateInstance(type, BindingFlags.NonPublic | BindingFlags.Instance, null, null, null, null);

            PrivateObject thePrivateObject = new PrivateObject(theCOEIdentity);                
            object theResult = thePrivateObject.Invoke("GetIdentity", BindingFlags.NonPublic | BindingFlags.Static, new object[] { ConfigurationManager.AppSettings["LogonUserName"], ConfigurationManager.AppSettings["LogonPassword"] });
            Assert.IsNotNull(theResult);            
        }
              

        [TestMethod]
        public void GetIdentity_isTicket_False()
        {           
            Type type = typeof(COEIdentity);
            COEIdentity theCOEIdentity = (COEIdentity)Activator.CreateInstance(type, BindingFlags.NonPublic | BindingFlags.Instance, null, null, null, null);

            PrivateObject thePrivateObject = new PrivateObject(theCOEIdentity);              
            object theResult = thePrivateObject.Invoke("GetIdentity", BindingFlags.NonPublic | BindingFlags.Static, new object[] { ConfigurationManager.AppSettings["LogonUserName"], false });
            Assert.IsNotNull(theResult);           
        }        

        [TestMethod]
        public void GetIdentity_isTicket_True()
        {            
            Type type = typeof(COEIdentity);
            COEIdentity theCOEIdentity = (COEIdentity)Activator.CreateInstance(type, BindingFlags.NonPublic | BindingFlags.Instance, null, null, null, null);

            PrivateObject thePrivateObject = new PrivateObject(theCOEIdentity);
                                
            object theNewCOEIdentity = thePrivateObject.Invoke("GetIdentity", BindingFlags.NonPublic | BindingFlags.Static, new object[] { ConfigurationManager.AppSettings["LogonUserName"], ConfigurationManager.AppSettings["LogonPassword"] });
            object theResult = thePrivateObject.Invoke("GetIdentity", BindingFlags.NonPublic | BindingFlags.Static, new object[] { (theNewCOEIdentity as COEIdentity).IdentityToken, true });
            Assert.IsNotNull(theResult);            
        }

       
        [TestMethod]
        public void GetIdentity_isTicket_ByStandAloneID()
        {           
            Type type = typeof(COEIdentity);
            COEIdentity theCOEIdentity = (COEIdentity)Activator.CreateInstance(type, BindingFlags.NonPublic | BindingFlags.Instance, null, null, null, null);

            PrivateObject thePrivateObject = new PrivateObject(theCOEIdentity);
            object theResult = thePrivateObject.Invoke("GetIdentity", BindingFlags.NonPublic | BindingFlags.Static, new object[] { ConfigurationManager.AppSettings["LogonUserName"], ConfigurationManager.AppSettings["LogonPassword"] , string.Empty});
            Assert.IsNotNull(theResult);           
        }

        
        [TestMethod]
        public void IsValidTicket_Valid()
        {           
            Type type = typeof(COEIdentity);
            COEIdentity theCOEIdentity = (COEIdentity)Activator.CreateInstance(type, BindingFlags.NonPublic | BindingFlags.Instance, null, null, null, null);
                
            PrivateObject thePrivateObject = new PrivateObject(theCOEIdentity);
            object theNewCOEIdentity = thePrivateObject.Invoke("GetIdentity", BindingFlags.NonPublic | BindingFlags.Static, new object[] { ConfigurationManager.AppSettings["LogonUserName"], ConfigurationManager.AppSettings["LogonPassword"] });
            object theResult = thePrivateObject.Invoke("IsValidTicket", BindingFlags.NonPublic | BindingFlags.Static, new object[] { (theNewCOEIdentity as COEIdentity).IdentityToken });
            Assert.AreEqual(true, Convert.ToBoolean(theResult));           
        }

        [TestMethod]
        public void IsValidTicket_NotValid()
        {            
            Type type = typeof(COEIdentity);
            COEIdentity theCOEIdentity = (COEIdentity)Activator.CreateInstance(type, BindingFlags.NonPublic | BindingFlags.Instance, null, null, null, null);

            PrivateObject thePrivateObject = new PrivateObject(theCOEIdentity);                
            object theResult = thePrivateObject.Invoke("IsValidTicket", BindingFlags.NonPublic | BindingFlags.Static, new object[] { "invalid" });
            Assert.AreEqual(false, Convert.ToBoolean(theResult));            
        }


        [TestMethod]
        public void IsInRole()
        {
            if (SecurityHelper.GetApplicationDetails())
            {
                Type type = typeof(COEIdentity);
                COEIdentity theCOEIdentity = (COEIdentity)Activator.CreateInstance(type, BindingFlags.NonPublic | BindingFlags.Instance, null, null, null, null);

                PrivateObject thePrivateObject = new PrivateObject(theCOEIdentity);
                object theNewCOEIdentity = thePrivateObject.Invoke("GetIdentity", BindingFlags.NonPublic | BindingFlags.Static, new object[] { ConfigurationManager.AppSettings["LogonUserName"], ConfigurationManager.AppSettings["LogonPassword"] });
                thePrivateObject = new PrivateObject(theNewCOEIdentity);

                COEPrivilegeBOList theCOEPrivilegeBOList = COEPrivilegeBOList.GetDefaultListForApp(SecurityHelper.strAppName);
                object theResult = thePrivateObject.Invoke("IsInRole", new object[] { theCOEPrivilegeBOList[1].PrivilegeName });
                Assert.AreEqual(true, Convert.ToBoolean(theResult));
            }
            else
            {
                Assert.Fail("Application does not exist");
            }
        }

        [TestMethod]
        public void HasAppPrivilege()
        {
            if (SecurityHelper.GetApplicationDetails())
            {
                Type type = typeof(COEIdentity);
                COEIdentity theCOEIdentity = (COEIdentity)Activator.CreateInstance(type, BindingFlags.NonPublic | BindingFlags.Instance, null, null, null, null);

                PrivateObject thePrivateObject = new PrivateObject(theCOEIdentity);
                object theNewCOEIdentity = thePrivateObject.Invoke("GetIdentity", BindingFlags.NonPublic | BindingFlags.Static, new object[] { ConfigurationManager.AppSettings["LogonUserName"], ConfigurationManager.AppSettings["LogonPassword"] });
                thePrivateObject = new PrivateObject(theNewCOEIdentity);

                COEPrivilegeBOList theCOEPrivilegeBOList = COEPrivilegeBOList.GetDefaultListForApp(SecurityHelper.strAppName);
                object theResult = thePrivateObject.Invoke("HasAppPrivilege", new object[] { SecurityHelper.strAppName, theCOEPrivilegeBOList[1].PrivilegeName });
                Assert.AreEqual(true, Convert.ToBoolean(theResult));
            }
            else
            {
                Assert.Fail("Application does not exist");
            }
        }

        [TestMethod]
        public void RenewTicket()
        {
            if (SecurityHelper.GetUserDetails())
            {
                Authentication.Logoff();
                Type type = typeof(COEIdentity);
                COEIdentity theCOEIdentity = (COEIdentity)Activator.CreateInstance(type, BindingFlags.NonPublic | BindingFlags.Instance, null, null, null, null);
                
                PrivateObject thePrivateObject = new PrivateObject(theCOEIdentity);
                
                object theNewCOEIdentity = thePrivateObject.Invoke("GetIdentity", BindingFlags.NonPublic | BindingFlags.Static, new object[] { ConfigurationManager.AppSettings["LogonUserName"], ConfigurationManager.AppSettings["LogonPassword"] });
                object theResult = thePrivateObject.Invoke("RenewTicket", BindingFlags.NonPublic | BindingFlags.Static, new object[] { (theNewCOEIdentity as COEIdentity).IdentityToken });
                
                Assert.IsNotNull(theResult);
            }
            else
            {
                Assert.Fail("User does not exist");
            }
        }       
        
    }
}
