using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CambridgeSoft.COE.Framework.COESecurityService;
using System.Configuration;
using System.Reflection;

namespace CambridgeSoft.COE.Framework.UnitTests.Security
{
    /// <summary>
    /// Summary description for COEPrincipalTest
    /// </summary>
    [TestClass]
    public class COEPrincipalTest
    {
        public COEPrincipalTest()
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
        public static void MyClassInitialize(TestContext testContext) { }
        
        // Use ClassCleanup to run code after all tests in a class have run
        [ClassCleanup()]
        public static void MyClassCleanup() { }
        
        // Use TestInitialize to run code before running each test 
        [TestInitialize()]
        public void MyTestInitialize() { }
        
        // Use TestCleanup to run code after each test has run
        [TestCleanup()]
        public void MyTestCleanup() { }
        
        #endregion

        #region Test Methods

        [TestMethod]
        public void Login()
        {
            bool theResult = COEPrincipal.Login(ConfigurationManager.AppSettings["LogonUserName"], ConfigurationManager.AppSettings["LogonPassword"]);
            Assert.AreEqual(true,Convert.ToBoolean(theResult));
        }

        [TestMethod]
        public void Login_byIsTicket_False()
        {
            bool theResult = COEPrincipal.Login(ConfigurationManager.AppSettings["LogonUserName"],false);
            Assert.AreEqual(true, Convert.ToBoolean(theResult));
        }

        [TestMethod]
        public void Login_byIsTicket_True()
        {
            bool blnLogin = COEPrincipal.Login(ConfigurationManager.AppSettings["LogonUserName"], ConfigurationManager.AppSettings["LogonPassword"]);
            if (blnLogin)
            {
                COEIdentity theCOEIdentity = (COEIdentity)Activator.CreateInstance(typeof(COEIdentity), BindingFlags.NonPublic | BindingFlags.Instance, null, null, null, null);
                PrivateObject thePrivateObject = new PrivateObject(theCOEIdentity);
                object theNewCOEIdentity = thePrivateObject.Invoke("GetIdentity", BindingFlags.NonPublic | BindingFlags.Static, new object[] { ConfigurationManager.AppSettings["LogonUserName"], ConfigurationManager.AppSettings["LogonPassword"] });

                bool theResult = COEPrincipal.Login((theNewCOEIdentity as COEIdentity).IdentityToken, true);                
                Assert.AreEqual(true, Convert.ToBoolean(theResult));
            }
        }

        [TestMethod]
        public void Login_byTicket()
        {
            bool blnLogin = COEPrincipal.Login(ConfigurationManager.AppSettings["LogonUserName"], ConfigurationManager.AppSettings["LogonPassword"]);
            if (blnLogin)
            {
                COEIdentity theCOEIdentity = (COEIdentity)Activator.CreateInstance(typeof(COEIdentity), BindingFlags.NonPublic | BindingFlags.Instance, null, null, null, null);
                PrivateObject thePrivateObject = new PrivateObject(theCOEIdentity);
                object theNewCOEIdentity = thePrivateObject.Invoke("GetIdentity", BindingFlags.NonPublic | BindingFlags.Static, new object[] { ConfigurationManager.AppSettings["LogonUserName"], ConfigurationManager.AppSettings["LogonPassword"] });

                bool theResult = COEPrincipal.Login((theNewCOEIdentity as COEIdentity).IdentityToken);
                Assert.AreEqual(true, Convert.ToBoolean(theResult));
            }
        }

        [TestMethod]
        public void IsValidToken()
        {
            bool blnLogin = COEPrincipal.Login(ConfigurationManager.AppSettings["LogonUserName"], ConfigurationManager.AppSettings["LogonPassword"]);
            if (blnLogin)
            {
                bool theResult = COEPrincipal.IsValidToken();
                Assert.AreEqual(true, Convert.ToBoolean(theResult));
            }
        }

        [TestMethod]
        public void Login_Standalone()
        {
             bool blnLogin = COEPrincipal.Login(ConfigurationManager.AppSettings["LogonUserName"], ConfigurationManager.AppSettings["LogonPassword"]);
             if (blnLogin)
             {
                 bool theResult = COEPrincipal.Login_Standalone(ConfigurationManager.AppSettings["LogonUserName"], ConfigurationManager.AppSettings["LogonPassword"], string.Empty);
                 Assert.AreEqual(true, theResult);
             }           
        }

        [TestMethod]
        public void Logout()
        {
            bool blnLogin = COEPrincipal.Login(ConfigurationManager.AppSettings["LogonUserName"], ConfigurationManager.AppSettings["LogonPassword"]);
            if (blnLogin)
            {
                COEPrincipal.Logout();
                Assert.AreEqual(COEPrincipal.IsAuthenticated, false);
            }
        }

        #endregion Test Methods
    }
}
