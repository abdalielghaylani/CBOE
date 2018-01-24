using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using CambridgeSoft.COE.Framework.NUnitTests.Helpers;
using CambridgeSoft.COE.Framework.COESecurityService;
using System.Reflection;

namespace CambridgeSoft.COE.Framework.NUnitTests.Security
{
    /// <summary>
    /// Summary description for PrivilegeListTest
    /// </summary>
    [TestFixture]
    public class PrivilegeListTest
    {
        public PrivilegeListTest()
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

        #region Test Methods

        [Test]
        public void GetList()
        {
            if (SecurityHelper.GetRoleDetails())
            {
                PrivilegeList thePrivilegeList = PrivilegeList.GetList(SecurityHelper.strRoleName);
                Assert.IsNotNull(thePrivilegeList);
            }
            else
                Assert.Fail("Role does not exist");           
        }

        [Test]
        public void GetDefaultListForApp()
        {
            if (SecurityHelper.GetApplicationDetails())
            {
                PrivilegeList thePrivilegeList = PrivilegeList.GetDefaultListForApp(SecurityHelper.strAppName);
                Assert.IsNotNull(thePrivilegeList);
            }
            else
                Assert.Fail("Application does not exist");
        }

        [Test]
        public void InvalidateCache()
        {
            PrivilegeList thePrivilegeList = PrivilegeList.GetList(SecurityHelper.strRoleName);
            PrivilegeList.InvalidateCache();
            Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject thePrivateObject =new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(thePrivilegeList);
            object theNewPrivilegeList = thePrivateObject.GetField("_list", BindingFlags.NonPublic | BindingFlags.Static);
            Assert.IsNull(theNewPrivilegeList);
        }

        #endregion Test Methods
    }
}
