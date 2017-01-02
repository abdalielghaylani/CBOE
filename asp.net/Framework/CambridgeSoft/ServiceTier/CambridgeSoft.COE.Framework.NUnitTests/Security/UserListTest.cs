using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using CambridgeSoft.COE.Framework.COESecurityService;
using CambridgeSoft.COE.Framework.NUnitTests.Helpers;
using System.Reflection;

namespace CambridgeSoft.COE.Framework.NUnitTests.Security
{
    /// <summary>
    /// Summary description for UserListTest
    /// </summary>
    [TestFixture]
    public class UserListTest
    {
        public UserListTest()
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
        public static void MyClassInitialize() { }
        
        // Use ClassCleanup to run code after all tests in a class have run
        [TestFixtureTearDown]
        public static void MyClassCleanup() { }
        
        // Use TestInitialize to run code before running each test 
        [SetUp]
        public void MyTestInitialize() { }
        
        // Use TestCleanup to run code after each test has run
        [TearDown]
        public void MyTestCleanup() { }
        
        #endregion

        #region Test Methods

        [Test]
        public void GetCOEList()
        {
            UserList theUserList = UserList.GetCOEList();
            Assert.IsNotNull(theUserList);
        }

        [Test]
        public void GetCOEList_ByCoeIdentifier()
        {
            if (SecurityHelper.GetApplicationDetails())
            {
                UserList theUserList = UserList.GetCOEList(SecurityHelper.strAppName);
                Assert.IsNotNull(theUserList);
            }
        }

        [Test]
        public void GetDBMSList()
        {
            UserList theUserList = UserList.GetDBMSList();
            Assert.IsNotNull(theUserList);          
        }

        [Test]
        public void InvalidateCache()
        {
            UserList theUserList = UserList.GetCOEList();
            UserList.InvalidateCache();
            Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject thePrivateObject =new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(theUserList);
            object theNewUserList = thePrivateObject.GetField("_list", BindingFlags.NonPublic | BindingFlags.Static);
            Assert.IsNull(theNewUserList);
        }

        #endregion Test Methods

    }
}
