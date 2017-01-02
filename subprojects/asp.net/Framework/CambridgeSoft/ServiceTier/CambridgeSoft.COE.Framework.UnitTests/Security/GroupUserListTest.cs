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
    public class GroupUserListTest
    {
        #region Variables
        private TestContext testContextInstance;

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
        public GroupUserListTest()
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

        #region Test Methods
        /*
         * CanGetObject
         * GetUserByID
         * GetUserByUserID
         * Contains
         * AddUser
         * RemoveUser        
         * SetDatabaseName
         * GroupUserList Get(int groupID)
         * GroupUserList - constructor
         * */

        [TestMethod]
        [Priority(4)]
        public void CanGetObject_LoadAuthorizationRules()
        {
            bool blnResult = GroupUserList.CanGetObject();
            Assert.IsTrue(blnResult, "GroupUserList.CanGetObject() did not return the expected value.");
        }

        [TestMethod]
        [Priority(4)]
        public void GetUserByID_LoadingGroupUserListToReturnsListOfGroupUser()
        {
            if (SecurityHelper.GetGroupPeopleDetails())
            {
                GroupUserList theGroupUserList = new GroupUserList();
                theGroupUserList = GroupUserList.Get(SecurityHelper.intGroupId);
                GroupUser theGroupUser = theGroupUserList.GetUserByID(SecurityHelper.intPersonId);
                Assert.IsNotNull(theGroupUser, "Method Name :GetUserByID_LoadingGroupUserListToReturnsListOfGroupUser() \n GroupUserList.GetUserByID(PersonID) did not return the expected value.");
            }
        }

        [TestMethod]
        [Priority(4)]
        public void GetUserByID_LoadingGroupUserListToReturnsNull()
        {
            if (SecurityHelper.GetGroupPeopleDetails())
            {
                GroupUserList theGroupUserList = new GroupUserList();
                GroupUser theGroupUser = theGroupUserList.GetUserByID(SecurityHelper.intPersonId);
                Assert.IsNull(theGroupUser, "Method Name :GetUserByID_LoadingGroupUserListToReturnsNull() \n GroupUserList.GetUserByID(PersonID) did not return the expected value.");
            }
        }

        [TestMethod]
        [Priority(4)]
        public void GetUserByUserID_ReturnsGroupUser()
        {
            try
            {
                if (SecurityHelper.GetGroupPeopleDetails())//&& SecurityHelper.GetUserDetails())
                {
                    GroupUserList theGroupUserList = new GroupUserList();
                    theGroupUserList = GroupUserList.Get(SecurityHelper.intGroupId);
                    if (SecurityHelper.GetUserDetails())
                    {
                        GroupUser theGroupUser = theGroupUserList.GetUserByUserID(SecurityHelper.strUserID);
                        Assert.IsNotNull(theGroupUser, "Method Name :GetUserByUserID_ReturnsGroupUser() \n GroupUserList.GetUserByUserID(UserID) did not return the expected value.");
                    }
                }
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod]
        [Priority(4)]
        public void GetUserByUserID_ReturnsNull()
        {
            if (SecurityHelper.GetUserDetails())
            {
                GroupUserList theGroupUserList = new GroupUserList();
                GroupUser theGroupUser = theGroupUserList.GetUserByUserID(SecurityHelper.strUserID);
                Assert.IsNull(theGroupUser, "Method Name :GetUserByUserID_ReturnsGroupUser() \n GroupUserList.GetUserByUserID(UserID) did not return the expected value.");
            }
        }

        [TestMethod]
        [Priority(4)]
        public void Contains_ChecksPersonIDExistOrNot()
        {
            if (SecurityHelper.GetGroupPeopleDetails())
            {
                GroupUserList theGroupUserList = new GroupUserList();
                theGroupUserList = GroupUserList.Get(SecurityHelper.intGroupId);
                bool blnResult = theGroupUserList.Contains(SecurityHelper.intPersonId);
                Assert.IsTrue(blnResult, "Method Name :Contains_ChecksPersonIDExistOrNot() \n GroupUserList.Contains(PersonID) did not return the expected value.");
            }
        }

        [TestMethod]
        [Priority(4)]
        public void AddUser_LoadingNewGroupUser()
        {
            try
            {
                if (SecurityHelper.GetGroupPeopleDetails())
                {
                    GroupUserList theGroupUserList = new GroupUserList();
                    theGroupUserList = GroupUserList.Get(SecurityHelper.intGroupId);
                    if (theGroupUserList.Count > 0)
                    {

                        Type[] argsTypes = new Type[] { typeof(int), typeof(string), typeof(string), typeof(int), typeof(string), typeof(string), typeof(string), typeof(string), typeof(int), typeof(string), typeof(string), typeof(string), typeof(string), typeof(bool), typeof(RoleList), typeof(RoleList), typeof(RoleList), typeof(bool) };
                        object[] argsParams = new object[] { 1001, string.Empty, string.Empty, 0, string.Empty, string.Empty, string.Empty, string.Empty, 0, string.Empty, string.Empty, string.Empty, string.Empty, true, null, null, null, false };
                        Type theType = typeof(GroupUser);
                        ConstructorInfo theConstructorInfo = theType.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, argsTypes, null);
                        object theResult = theConstructorInfo.Invoke(argsParams);

                        GroupUser theGroupUser = theResult as GroupUser;

                        int iPrevGroupUserCOunt = theGroupUserList.Count;
                        theGroupUserList.AddUser(theGroupUser);
                        int intResult = theGroupUserList.Count;
                        Assert.AreEqual(iPrevGroupUserCOunt + 1, theGroupUserList.Count, "Method Name :AddUser_LoadingNewGroupUser() \n GroupUserList.AddUser(theGroupUser) did not return the expected value.");
                    }
                }
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            } 
        }

        [TestMethod]
        [Priority(4)]
        [ExpectedException(typeof(InvalidOperationException))]
        public void AddUser_ThrowExcepnForExistingUser()
        {            
            if (SecurityHelper.GetGroupPeopleDetails())
            {
                GroupUserList theGroupUserList = new GroupUserList();
                theGroupUserList = GroupUserList.Get(SecurityHelper.intGroupId);
                if (theGroupUserList.Count > 0)
                {
                    GroupUser theGroupUser = theGroupUserList[0].Clone();
                    int iPrevGroupUserCOunt = theGroupUserList.Count;
                    theGroupUserList.AddUser(theGroupUser);
                    //int intResult = theGroupUserList.Count;
                    //Assert.AreEqual(iPrevGroupUserCOunt + 1, theGroupUserList.Count, "Method Name :AddUser_ThrowExcepnForExistingUser() \n GroupUserList.AddUser(theGroupUser) did not return the expected value.");
                }
            }            
        }

        [TestMethod]       
        public void RemoveUser_ByGroupUser()
        {
            try
            {
                if (SecurityHelper.GetGroupPeopleDetails())
                {
                    GroupUserList theGroupUserList = new GroupUserList();
                    theGroupUserList = GroupUserList.Get(SecurityHelper.intGroupId);
                    if (theGroupUserList.Count > 0)
                    {
                        GroupUser theGroupUser = theGroupUserList[0].Clone();
                        int iPrevGroupUserCOunt = theGroupUserList.Count;
                        theGroupUserList.RemoveUser(theGroupUser);
                        int intResult = theGroupUserList.Count;
                        Assert.AreEqual(iPrevGroupUserCOunt - 1, theGroupUserList.Count, "Method Name :RemoveUser_ByGroupUser \n GroupUserList.AddUser(theGroupUser) did not return the expected value.");
                    }
                }
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        
        #endregion
    }
}
