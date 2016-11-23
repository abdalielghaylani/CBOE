using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml;
using CambridgeSoft.COE.Framework.UnitTests.Helpers;
using CambridgeSoft.COE.Framework.COESecurityService;
using System.Reflection;

namespace CambridgeSoft.COE.Framework.UnitTests.Security
{
    [TestClass()]
    public class GroupUserTest
    {
        #region Variables
        private TestContext testContextInstance;
        
        private RoleList theRoleList_GroupGrantedRoles = null;
        private RoleList theRoleList_DirectGrantedRoles = null;
        private RoleList theRoleList_DirectAvailableRoles = null;
        private bool blnLeader = false;

        #endregion

        #region Properties

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
        #endregion

        #region Constructor
        public GroupUserTest()
        {
        }
        #endregion

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

        // Use TestCleanup to run code after each test has ru
        [TestCleanup()]
        public void MyTestCleanup() { }
        #endregion

        #region Test Method
        [TestMethod]
        public void GroupUser_ConstructorWithMultipleParam()
        {
            this.theRoleList_GroupGrantedRoles = null;
            this.theRoleList_DirectGrantedRoles = null;
            this.theRoleList_DirectAvailableRoles = null;
            this.blnLeader = false;
            if (SecurityHelper.GetPeopleDetails())
            {
                Type[] argsTypes = new Type[] { typeof(int), typeof(string), typeof(string), typeof(int), typeof(string), typeof(string), typeof(string), typeof(string), typeof(int), typeof(string), typeof(string), typeof(string), typeof(string), typeof(bool), typeof(RoleList), typeof(RoleList), typeof(RoleList), typeof(bool) };
                object[] argsParams = new object[] { SecurityHelper.personID, SecurityHelper.userCode, SecurityHelper.userID, SecurityHelper.supervisorID, SecurityHelper.title, SecurityHelper.firstName, SecurityHelper.lastName, SecurityHelper.middleName, SecurityHelper.siteID, SecurityHelper.department, SecurityHelper.telephone, SecurityHelper.email, SecurityHelper.address, SecurityHelper.active, this.theRoleList_GroupGrantedRoles, this.theRoleList_DirectGrantedRoles, this.theRoleList_DirectAvailableRoles, this.blnLeader };
                Type theType = typeof(GroupUser);
                ConstructorInfo theConstructorInfo = theType.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, argsTypes, null);
                object theResult = theConstructorInfo.Invoke(argsParams);
                Assert.IsNotNull(theResult, "GroupUser.GroupUser(int personID, string userCode, string userID, int supervisorID,string title, string firstName, string middleName, string lastName, int siteID, string department, string telephone, string email, string address, bool active,RoleList groupGrantedRoles, RoleList directGrantedRoles, RoleList directAvailableRoles, bool isleader) did not return the expected value.");
            }
            else
            {
                Assert.Fail("User does not exist");
            }
        }
        #endregion
    }
}
