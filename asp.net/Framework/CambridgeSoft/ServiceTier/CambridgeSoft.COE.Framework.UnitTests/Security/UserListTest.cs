using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CambridgeSoft.COE.Framework.COESecurityService;
using CambridgeSoft.COE.Framework.UnitTests.Helpers;
using System.Reflection;

namespace CambridgeSoft.COE.Framework.UnitTests.Security
{
    /// <summary>
    /// Summary description for UserListTest
    /// </summary>
    [TestClass]
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
        public void GetCOEList()
        {
            UserList theUserList = UserList.GetCOEList();
            Assert.IsNotNull(theUserList);
        }

        [TestMethod]
        public void GetCOEList_ByCoeIdentifier()
        {
            if (SecurityHelper.GetApplicationDetails())
            {
                UserList theUserList = UserList.GetCOEList(SecurityHelper.strAppName);
                Assert.IsNotNull(theUserList);
            }
        }

        [TestMethod]
        public void GetDBMSList()
        {
            UserList theUserList = UserList.GetDBMSList();
            Assert.IsNotNull(theUserList);          
        }

        [TestMethod]
        public void InvalidateCache()
        {
            UserList theUserList = UserList.GetCOEList();
            UserList.InvalidateCache();
            PrivateObject thePrivateObject = new PrivateObject(theUserList);
            object theNewUserList = thePrivateObject.GetField("_list", BindingFlags.NonPublic | BindingFlags.Static);
            Assert.IsNull(theNewUserList);
        }

        #endregion Test Methods

    }
}
