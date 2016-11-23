using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using CambridgeSoft.COE.Framework.NUnitTests.Helpers;
using CambridgeSoft.COE.Framework.COESecurityService;
using System.Reflection;
using System.Configuration;

namespace CambridgeSoft.COE.Framework.NUnitTests.Security
{
    /// <summary>
    /// Summary description for COEIdentityTest
    /// </summary>
    [TestFixture]
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
        [TestFixtureSetUp]
        public static void MyClassInitialize()
        {
            Authentication.Logon();
        }
        
        // Use ClassCleanup to run code after all tests in a class have run
        [TestFixtureTearDown]
        public static void MyClassCleanup() 
        {
            Authentication.Logoff();
        }
        
        // Use TestInitialize to run code before running each test 
        [SetUp]
        public void MyTestInitialize() { }
        
        // Use TestCleanup to run code after each test has run
        [TearDown]
        public void MyTestCleanup() { }
        
        #endregion

        [Test]
        public void GetIdentity()
        {            
            Type type = typeof(COEIdentity);
            COEIdentity theCOEIdentity = (COEIdentity)Activator.CreateInstance(type, BindingFlags.NonPublic | BindingFlags.Instance, null, null, null, null);

            Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject thePrivateObject =new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(theCOEIdentity);                
            object theResult = thePrivateObject.Invoke("GetIdentity", BindingFlags.NonPublic | BindingFlags.Static, new object[] { ConfigurationManager.AppSettings["LogonUserName"], ConfigurationManager.AppSettings["LogonPassword"] });
            Assert.IsNotNull(theResult);            
        }
              

        [Test]
        public void GetIdentity_isTicket_False()
        {           
            Type type = typeof(COEIdentity);
            COEIdentity theCOEIdentity = (COEIdentity)Activator.CreateInstance(type, BindingFlags.NonPublic | BindingFlags.Instance, null, null, null, null);

            Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject thePrivateObject =new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(theCOEIdentity);              
            object theResult = thePrivateObject.Invoke("GetIdentity", BindingFlags.NonPublic | BindingFlags.Static, new object[] { ConfigurationManager.AppSettings["LogonUserName"], false });
            Assert.IsNotNull(theResult);           
        }        

        [Test]
        public void GetIdentity_isTicket_True()
        {            
            Type type = typeof(COEIdentity);
            COEIdentity theCOEIdentity = (COEIdentity)Activator.CreateInstance(type, BindingFlags.NonPublic | BindingFlags.Instance, null, null, null, null);

            Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject thePrivateObject =new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(theCOEIdentity);
                                
            object theNewCOEIdentity = thePrivateObject.Invoke("GetIdentity", BindingFlags.NonPublic | BindingFlags.Static, new object[] { ConfigurationManager.AppSettings["LogonUserName"], ConfigurationManager.AppSettings["LogonPassword"] });
            object theResult = thePrivateObject.Invoke("GetIdentity", BindingFlags.NonPublic | BindingFlags.Static, new object[] { (theNewCOEIdentity as COEIdentity).IdentityToken, true });
            Assert.IsNotNull(theResult);            
        }

       
        [Test]
        public void GetIdentity_isTicket_ByStandAloneID()
        {           
            Type type = typeof(COEIdentity);
            COEIdentity theCOEIdentity = (COEIdentity)Activator.CreateInstance(type, BindingFlags.NonPublic | BindingFlags.Instance, null, null, null, null);

            Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject thePrivateObject =new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(theCOEIdentity);
            object theResult = thePrivateObject.Invoke("GetIdentity", BindingFlags.NonPublic | BindingFlags.Static, new object[] { ConfigurationManager.AppSettings["LogonUserName"], ConfigurationManager.AppSettings["LogonPassword"] , string.Empty});
            Assert.IsNotNull(theResult);           
        }

        
        [Test]
        public void IsValidTicket_Valid()
        {           
            Type type = typeof(COEIdentity);
            COEIdentity theCOEIdentity = (COEIdentity)Activator.CreateInstance(type, BindingFlags.NonPublic | BindingFlags.Instance, null, null, null, null);
                
            Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject thePrivateObject =new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(theCOEIdentity);
            object theNewCOEIdentity = thePrivateObject.Invoke("GetIdentity", BindingFlags.NonPublic | BindingFlags.Static, new object[] { ConfigurationManager.AppSettings["LogonUserName"], ConfigurationManager.AppSettings["LogonPassword"] });
            object theResult = thePrivateObject.Invoke("IsValidTicket", BindingFlags.NonPublic | BindingFlags.Static, new object[] { (theNewCOEIdentity as COEIdentity).IdentityToken });
            Assert.AreEqual(true, Convert.ToBoolean(theResult));           
        }

        [Test]
        public void IsValidTicket_NotValid()
        {            
            Type type = typeof(COEIdentity);
            COEIdentity theCOEIdentity = (COEIdentity)Activator.CreateInstance(type, BindingFlags.NonPublic | BindingFlags.Instance, null, null, null, null);

            Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject thePrivateObject =new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(theCOEIdentity);                
            object theResult = thePrivateObject.Invoke("IsValidTicket", BindingFlags.NonPublic | BindingFlags.Static, new object[] { "invalid" });
            Assert.AreEqual(false, Convert.ToBoolean(theResult));            
        }


        [Test]
        public void IsInRole()
        {
            if (SecurityHelper.GetApplicationDetails())
            {
                Type type = typeof(COEIdentity);
                COEIdentity theCOEIdentity = (COEIdentity)Activator.CreateInstance(type, BindingFlags.NonPublic | BindingFlags.Instance, null, null, null, null);

                Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject thePrivateObject =new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(theCOEIdentity);
                object theNewCOEIdentity = thePrivateObject.Invoke("GetIdentity", BindingFlags.NonPublic | BindingFlags.Static, new object[] { ConfigurationManager.AppSettings["LogonUserName"], ConfigurationManager.AppSettings["LogonPassword"] });
                thePrivateObject =new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(theNewCOEIdentity);

                COEPrivilegeBOList theCOEPrivilegeBOList = COEPrivilegeBOList.GetDefaultListForApp(SecurityHelper.strAppName);
                object theResult = thePrivateObject.Invoke("IsInRole", new object[] { theCOEPrivilegeBOList[1].PrivilegeName });
                Assert.AreEqual(true, Convert.ToBoolean(theResult));
            }
            else
            {
                Assert.Fail("Application does not exist");
            }
        }

        [Test]
        public void HasAppPrivilege()
        {
            if (SecurityHelper.GetApplicationDetails())
            {
                Type type = typeof(COEIdentity);
                COEIdentity theCOEIdentity = (COEIdentity)Activator.CreateInstance(type, BindingFlags.NonPublic | BindingFlags.Instance, null, null, null, null);

                Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject thePrivateObject =new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(theCOEIdentity);
                object theNewCOEIdentity = thePrivateObject.Invoke("GetIdentity", BindingFlags.NonPublic | BindingFlags.Static, new object[] { ConfigurationManager.AppSettings["LogonUserName"], ConfigurationManager.AppSettings["LogonPassword"] });
                thePrivateObject =new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(theNewCOEIdentity);

                COEPrivilegeBOList theCOEPrivilegeBOList = COEPrivilegeBOList.GetDefaultListForApp(SecurityHelper.strAppName);
                object theResult = thePrivateObject.Invoke("HasAppPrivilege", new object[] { SecurityHelper.strAppName, theCOEPrivilegeBOList[1].PrivilegeName });
                Assert.AreEqual(true, Convert.ToBoolean(theResult));
            }
            else
            {
                Assert.Fail("Application does not exist");
            }
        }

        [Test]
        public void RenewTicket()
        {
            if (SecurityHelper.GetUserDetails())
            {
                Authentication.Logoff();
                Type type = typeof(COEIdentity);
                COEIdentity theCOEIdentity = (COEIdentity)Activator.CreateInstance(type, BindingFlags.NonPublic | BindingFlags.Instance, null, null, null, null);
                
                Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject thePrivateObject =new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(theCOEIdentity);
                
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
