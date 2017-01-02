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
    /// Summary description for RoleListTest
    /// </summary>
    [TestClass]
    public class RoleListTest
    {
        public RoleListTest()
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
        public void GetDBMSList()
        {
            RoleList theRoleList = RoleList.GetDBMSList();
            Assert.IsNotNull(theRoleList);
        }

        [TestMethod]
        public void GetList()
        {
            //RoleList theRoleList = RoleList.GetList();
            //Assert.IsNotNull(theRoleList);
            Assert.Fail("DataPortal_Fetch method not found in RoleList.cs");
        }

        [TestMethod]
        public void GetGroupPersonDirectAvailableRoleList()
        {
            if (SecurityHelper.GetUserDetails())
            {
                RoleList theRoleList = RoleList.GetGroupPersonDirectAvailableRoleList(SecurityHelper.intPersonId);
                Assert.IsNotNull(theRoleList);
            }
            else
                Assert.Fail("User does not exist");
        }

        [TestMethod]
        public void GetGroupPersonDirectRoleList()
        {
            if (SecurityHelper.GetGroupPeopleDetails())
            {
                RoleList theRoleList = RoleList.GetGroupPersonDirectRoleList(SecurityHelper.intGroupId, SecurityHelper.intPersonId);
                Assert.IsNotNull(theRoleList);
            }
            else
                Assert.Fail("User does not exist for a group");
        }

        [TestMethod]
        public void GetGroupRoleAvailableList()
        {
            if (SecurityHelper.GetGroupDetails())
            {
                RoleList theRoleList = RoleList.GetGroupRoleAvailableList(SecurityHelper.intGroupId);
                Assert.IsNotNull(theRoleList);
            }
            else
                Assert.Fail("User does not exist for a group");
        }

        [TestMethod]
        public void GetGroupRoleList()
        {
            if (SecurityHelper.GetGroupDetails())
            {
                RoleList theRoleList = RoleList.GetGroupRoleList(SecurityHelper.intGroupId);
                Assert.IsNotNull(theRoleList);
            }
            else
                Assert.Fail("Group does not exist");
        }

        [TestMethod]
        public void GetApplicationDetails()
        {
            if (SecurityHelper.GetApplicationDetails())
            {
                RoleList theRoleList = RoleList.GetListByApplication(SecurityHelper.strAppName);
                Assert.IsNotNull(theRoleList);
            }
            else
                Assert.Fail("Application does not exist");
        }         

        [TestMethod]
        public void GetListByRole()
        {
            if (SecurityHelper.GetRoleDetails())
            {
                RoleList theRoleList = RoleList.GetListByRole(SecurityHelper.strRoleName);
                Assert.IsNotNull(theRoleList);
            }
            else
                Assert.Fail("Role does not exist");
        }


        [TestMethod]
        public void GetListByUser()
        {
            if (SecurityHelper.GetUserDetails())
            {
                RoleList theRoleList = RoleList.GetListByUser(SecurityHelper.strUserID);
                Assert.IsNotNull(theRoleList);
            }
            else
                Assert.Fail("User does not exist");
        }

        [TestMethod]
        public void GetListByUser_By_AppName_UserName()
        {
            if (SecurityHelper.GetApplicationDetails() && SecurityHelper.GetUserDetails())
            {
                RoleList theRoleList = RoleList.GetListByUser(SecurityHelper.strAppName, SecurityHelper.strUserID);              
                Assert.IsNotNull(theRoleList);
            }
            else
                Assert.Fail("Role does not exist");
        }             


        [TestMethod]
        public void InvalidateCache()
        {
            RoleList theRoleList = RoleList.GetDBMSList();
            RoleList.InvalidateCache();
            PrivateObject thePrivateObject = new PrivateObject(theRoleList);
            object theNewRoleList = thePrivateObject.GetField("_list", BindingFlags.NonPublic | BindingFlags.Static);
            Assert.IsNull(theNewRoleList);
        }

        #endregion Test Methods
    }
}
